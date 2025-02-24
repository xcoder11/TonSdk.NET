using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TonSdk.Adnl;

public enum AdnlClientState
{
    Connecting,
    Open,
    Closing,
    Closed
}

public class AdnlClientTcp
{
    private readonly AdnlAddress _address;
    private readonly string _host;
    private readonly int _port;
    private readonly TcpClient _socket;

    // private List<byte> _buffer;
    private byte[] _buffer;
    private Cipher _cipher;
    private Decipher _decipher;
    private AdnlKeys _keys;
    private NetworkStream _networkStream;
    private AdnlAesParams _params;

    public AdnlClientTcp(int host, int port, byte[] peerPublicKey) : this(ConvertToIPAddress(host), port,
        new AdnlAddress(peerPublicKey))
    {
    }

    public AdnlClientTcp(int host, int port, string peerPublicKey) : this(ConvertToIPAddress(host), port,
        new AdnlAddress(peerPublicKey))
    {
    }

    public AdnlClientTcp(string host, int port, byte[] peerPublicKey) : this(host, port, new AdnlAddress(peerPublicKey))
    {
    }

    public AdnlClientTcp(string host, int port, string peerPublicKey) : this(host, port, new AdnlAddress(peerPublicKey))
    {
    }

    private AdnlClientTcp(string host, int port, AdnlAddress adnlAddress)
    {
        _host = host;
        _port = port;
        _address = adnlAddress;
        _socket = new TcpClient();
        _socket.ReceiveBufferSize = 1 * 1024 * 1024;
        _socket.SendBufferSize = 1 * 1024 * 1024;
    }

    public AdnlClientState State { get; private set; } = AdnlClientState.Closed;

    public event Action Connected;
    public event Action Ready;
    public event Action Closed;
    public event Action<byte[]> DataReceived;
    public event Action<Exception> ErrorOccurred;

    private async Task Handshake(CancellationToken ct = default)
    {
        var key = _keys.Shared.Take(16).Concat(_params.Hash.Skip(16).Take(16)).ToArray();
        var nonce = _params.Hash.Take(4).Concat(_keys.Shared.Skip(20).Take(12)).ToArray();

        var cipher = CipherFactory.CreateCipheriv(key, nonce);

        var payload = cipher.Update(_params.Bytes).ToArray();
        var packet = _address.Hash.Concat(_keys.Public).Concat(_params.Hash).Concat(payload).ToArray();

        await _networkStream.WriteAsync(packet, ct).ConfigureAwait(false);
    }

    private void OnBeforeConnect()
    {
        if (State != AdnlClientState.Closed) return;
        var keys = new AdnlKeys(_address.PublicKey);

        _keys = keys;
        _params = new AdnlAesParams();
        _cipher = CipherFactory.CreateCipheriv(_params.TxKey, _params.TxNonce);
        _decipher = CipherFactory.CreateDecipheriv(_params.RxKey, _params.RxNonce);
        _buffer = [];
        State = AdnlClientState.Connecting;
    }

    private async Task ReadDataAsync()
    {
        try
        {
            var buffer = new byte[1024 * 1024];

            while (_socket.Connected)
            {
                var bytesRead = await _networkStream.ReadAsync(buffer).ConfigureAwait(false);
                if (bytesRead > 0)
                {
                    var receivedData = new byte[bytesRead];
                    Array.Copy(buffer, receivedData, bytesRead);

                    OnDataReceived(receivedData);
                }
                else if (bytesRead == 0)
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(ex);
        }
        finally
        {
            OnClose();
        }
    }

    private void OnReady()
    {
        State = AdnlClientState.Open;
        Ready?.Invoke();
    }

    private void OnClose()
    {
        State = AdnlClientState.Closed;
        Closed?.Invoke();
    }

    private void OnDataReceived(byte[] data)
    {
        _buffer = _buffer.Concat(Decrypt(data)).ToArray();
        while (_buffer.Length >= AdnlPacket.PacketMinSize)
        {
            var packet = AdnlPacket.Parse(_buffer);
            if (packet == null) break;

            _buffer = _buffer.Skip(packet.Length).ToArray();

            if (State == AdnlClientState.Connecting)
            {
                if (packet.Payload.Length != 0)
                {
                    ErrorOccurred?.Invoke(new Exception("AdnlClient: Bad handshake."));
                    End();
                    State = AdnlClientState.Closed;
                }
                else
                {
                    OnReady();
                }

                break;
            }

            DataReceived?.Invoke(packet.Payload);
        }
    }

    public async Task Connect(CancellationToken ct = default)
    {
        OnBeforeConnect();
        try
        {
            await _socket.ConnectAsync(_host, _port, ct).ConfigureAwait(false);
            _networkStream = _socket.GetStream();
            _ = ReadDataAsync().ConfigureAwait(false);
            Connected?.Invoke();
            await Handshake(ct).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            ErrorOccurred?.Invoke(e);
            End();
            State = AdnlClientState.Closed;
        }
    }

    public void End()
    {
        if (State == AdnlClientState.Closed || State == AdnlClientState.Closing) return;
        _socket.Close();
        OnClose();
    }

    public async Task Write(byte[] data, CancellationToken ct = default)
    {
        var packet = new AdnlPacket(data);
        var encrypted = Encrypt(packet.Data);
        await _networkStream.WriteAsync(encrypted, ct).ConfigureAwait(false);
    }

    private byte[] Encrypt(byte[] data)
    {
        return _cipher.Update(data);
    }

    private byte[] Decrypt(byte[] data)
    {
        return _decipher.Update(data);
    }

    private static string ConvertToIPAddress(int number)
    {
        var unsignedNumber = (uint)number;
        var bytes = new byte[4];

        bytes[0] = (byte)((unsignedNumber >> 24) & 0xFF);
        bytes[1] = (byte)((unsignedNumber >> 16) & 0xFF);
        bytes[2] = (byte)((unsignedNumber >> 8) & 0xFF);
        bytes[3] = (byte)(unsignedNumber & 0xFF);

        return $"{bytes[0]}.{bytes[1]}.{bytes[2]}.{bytes[3]}";
    }
}