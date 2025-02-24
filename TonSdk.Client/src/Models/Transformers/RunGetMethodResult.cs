using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json.Linq;
using TonSdk.Client.Stack;
using TonSdk.Core.Boc;

namespace TonSdk.Client;

public struct RunGetMethodResult
{
    public int GasUsed;
    public object[] Stack;
    public int ExitCode;
    public IStackItem[] StackItems;

    internal RunGetMethodResult(OutRunGetMethod outRunGetMethod)
    {
        GasUsed = outRunGetMethod.GasUsed;
        ExitCode = outRunGetMethod.ExitCode;
        Stack = new object[outRunGetMethod.Stack.Length];
        for (int i = 0; i < outRunGetMethod.Stack.Length; i++)
        {
            Stack[i] = ParseStackItem(outRunGetMethod.Stack[i]);
        }

        StackItems = new IStackItem[] { };
    }
        
    internal RunGetMethodResult(OutV3RunGetMethod outRunGetMethod)
    {
        GasUsed = outRunGetMethod.GasUsed;
        ExitCode = outRunGetMethod.ExitCode;
        Stack = new object[outRunGetMethod.Stack.Length];
        for (int i = 0; i < outRunGetMethod.Stack.Length; i++)
        {
            Stack[i] = ParseStackItem(outRunGetMethod.Stack[i]);
        }

        StackItems = new IStackItem[] { };
    }

    internal static object ParseObject(JObject x)
    {
        string typeName = x["@type"].ToString();
        switch (typeName)
        {
            case "tvm.list":
            case "tvm.tuple":
                object[] list = new object[x["elements"].Count()];
                for (int c = 0; c < x["elements"].Count(); c++)
                {
                    list[c] = ParseObject((JObject)x["elements"][c]);
                }

                return list;
            case "tvm.cell":
                return Cell.From(x["bytes"].ToString()); // Cell.From should be defined elsewhere in your code.
            case "tvm.stackEntryCell":
                return ParseObject((JObject)x["cell"]);
            case "tvm.stackEntryTuple":
                return ParseObject((JObject)x["tuple"]);
            case "tvm.stackEntryNumber":
                return ParseObject((JObject)x["number"]);
            case "tvm.numberDecimal":
                string number = x["number"].ToString();
                return BigInteger.Parse(number);
            default:
                throw new Exception($"Unknown type {typeName}");
        }
    }
        
    internal static object ParseStackItem(JObject item)
    {
        string type = item["type"].ToString();

        switch (type)
        {
            case "num":
            {
                string valueStr = item["value"].ToString();
                if (valueStr == null)
                    throw new Exception("Expected a string value for 'num' type.");

                bool isNegative = valueStr[0] == '-';
                string slice = isNegative ? valueStr.Substring(3) : valueStr.Substring(2);
                BitsSlice bitsSlice = new Bits(slice).Parse();
                BigInteger x = bitsSlice.LoadUInt(bitsSlice.RemainderBits);

                return isNegative ? 0 - x : x;
            }
            case "cell":
            {
                return Cell.From(item["value"].ToString());
            }
            case "list":
            case "tuple":
            {
                if (item["value"] is JObject jObject)
                {
                    return ParseObject(jObject);
                }
                else
                {
                    throw new Exception("Expected a JObject value for 'list' or 'tuple' type.");
                }
            }
            default:
            {
                throw new Exception("Unknown type " + type);
            }
        }
    }

    internal static object ParseStackItem(object[] item)
    {
        string type = item[0].ToString();
        object value = item[1];

        switch (type)
        {
            case "num":
            {
                string valueStr = value as string;
                if (valueStr == null)
                    throw new Exception("Expected a string value for 'num' type.");
                        
                bool isNegative = valueStr[0] == '-';
                string slice = isNegative ? valueStr.Substring(3) : valueStr.Substring(2);
                        
                if (slice.Length % 2 != 0)
                {
                    slice = "0" + slice;
                }

                int length = slice.Length;
                byte[] bytes = new byte[length / 2];
                for (int i = 0; i < length; i += 2)
                {
                    bytes[i / 2] = Convert.ToByte(slice.Substring(i, 2), 16);
                }
                        
                if (bytes[0] >= 0x80)
                {
                    byte[] temp = new byte[bytes.Length + 1];
                    Array.Copy(bytes, 0, temp, 1, bytes.Length);
                    bytes = temp;
                }

                Array.Reverse(bytes);
                var bigInt = new BigInteger(bytes);
                        
                return isNegative ? 0 - bigInt : bigInt;
            }
            case "cell":
            {
                if (value is JObject jObject && jObject["bytes"] is JValue jValue)
                {
                    return Cell.From((string)jValue.Value);
                }
                else
                {
                    throw new Exception("Expected a JObject value for 'cell' type.");
                }
            }
            case "list":
            case "tuple":
            {
                if (value is JObject jObject)
                {
                    return ParseObject(jObject);
                }
                else
                {
                    throw new Exception("Expected a JObject value for 'list' or 'tuple' type.");
                }
            }
            default:
            {
                throw new Exception("Unknown type " + type);
            }
        }
    }
}