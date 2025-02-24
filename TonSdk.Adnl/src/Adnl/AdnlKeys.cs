using System;
using System.Numerics;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace TonSdk.Adnl;

internal class Ed25519
{
    private static readonly BigInteger Ed25519P =
        BigInteger.Parse("57896044618658097711785492504343953926634992332820282019728792003956564819949");

    private readonly Ed25519PrivateKeyParameters _privateKey;
    private readonly Ed25519PublicKeyParameters _publicKey;

    internal Ed25519()
    {
        var keyPairGenerator = new Ed25519KeyPairGenerator();
        keyPairGenerator.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));

        var keyPair = keyPairGenerator.GenerateKeyPair();
        _privateKey = (Ed25519PrivateKeyParameters)keyPair.Private;
        _publicKey = (Ed25519PublicKeyParameters)keyPair.Public;
    }

    internal byte[] PublicKey => _publicKey.GetEncoded();
    internal byte[] PrivateKey => _privateKey.GetEncoded();

    private byte[] Ed25519PrivateKeyToCurve25519(byte[] privateKey)
    {
        using var sha512 = SHA512.Create();
        var sha512Hash = sha512.ComputeHash(privateKey);

        sha512Hash[0] &= 248;
        sha512Hash[31] &= 127;
        sha512Hash[31] |= 64;

        var x25519PrivateKey = new byte[32];
        Array.Copy(sha512Hash, x25519PrivateKey, 32);

        return x25519PrivateKey;
    }

    private static byte[] EdwardsToMontgomeryPublicKey(byte[] publicKey)
    {
        if (publicKey == null || publicKey.Length != 32) throw new ArgumentException("Invalid public key.");

        var yBytes = new byte[publicKey.Length];
        Array.Copy(publicKey, yBytes, publicKey.Length);

        yBytes[^1] &= 0b01111111; // clear sign bit
        var y = new BigInteger(yBytes, true);

        var montgomeryU =
            (BigInteger.One + y) * BigInteger.ModPow(BigInteger.One - y,
                Ed25519P - (BigInteger.One + BigInteger.One), Ed25519P);
        montgomeryU %= Ed25519P;
        if (montgomeryU < 0)
            montgomeryU += Ed25519P;
        return montgomeryU.ToByteArray();
    }

    internal byte[] CalculateSharedSecret(byte[] otherPublicKey)
    {
        var x25519PrivateKey =
            new X25519PrivateKeyParameters(Ed25519PrivateKeyToCurve25519(_privateKey.GetEncoded()), 0);
        var x25519OtherPublicKey =
            new X25519PublicKeyParameters(EdwardsToMontgomeryPublicKey(otherPublicKey), 0);

        var sharedSecret = new byte[32];

        x25519PrivateKey.GenerateSecret(x25519OtherPublicKey, sharedSecret, 0);

        return sharedSecret;
    }
}

internal class AdnlKeys
{
    private readonly Ed25519 _ed25519;
    private readonly byte[] _peer;
    private byte[] _public;
    private byte[] _shared;

    internal AdnlKeys(byte[] peerPublicKey)
    {
        _peer = peerPublicKey;
        _ed25519 = new Ed25519();
    }

    internal byte[] Public => _ed25519.PublicKey;

    internal byte[] Private => _ed25519.PrivateKey;
    internal byte[] Shared => _ed25519.CalculateSharedSecret(_peer);

    internal static byte[] GenerateRandomBytes(int byteSize)
    {
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        var randomBytes = new byte[byteSize];
        randomNumberGenerator.GetBytes(randomBytes);
        return randomBytes;
    }
}