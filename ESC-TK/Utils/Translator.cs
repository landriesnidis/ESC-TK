using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Landriesnidis.ESC_TK.Utils
{
    public class Translator
    {
        /// <summary>
        /// Byte数组 转 16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Bytes2HexString(byte[] bytes)
        {
            StringBuilder ret = new StringBuilder();
            foreach (byte b in bytes)
            {
                ret.AppendFormat("{0:X2} ", b);
            }
            return ret.ToString();
        }

        /// <summary>
        /// 16进制字符串 转 明文字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string HexString2String(string input)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string[] hexValuesSplit = input.Split(' ');
                foreach (String hex in hexValuesSplit)
                {
                    if (hex == "") continue;
                    int value = Convert.ToInt32(hex, 16);
                    string stringValue = Char.ConvertFromUtf32(value);
                    //char charValue = (char)value;
                    sb.Append(stringValue);
                }
            }
            catch (FormatException ex)
            {
                throw ex;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 明文字符串 转 16进制字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string String2HexString(string input)
        {
            char[] values = input.ToCharArray();
            StringBuilder sb = new StringBuilder();
            foreach (char letter in values)
            {
                int value = Convert.ToInt32(letter);
                string hexOutput = String.Format("{0:X} ", value);
                sb.Append(hexOutput);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 16进制字符串 转 Byte数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexString2Bytes(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }
                catch (FormatException ex)
                {
                    throw ex;
                }
            }
            return bytes;
        }
    }

    public class HexCalculator
    {
        public static string HexString_Add(string hex1, string hex2, string format = "X2")
        {
            byte b1 = Convert.ToByte(hex1, 16);
            byte b2 = Convert.ToByte(hex2, 16);
            return (b1 + b2).ToString(format);
        }
        public static string HexString_Sub(string hex1, string hex2, string format = "X2")
        {
            byte b1 = Convert.ToByte(hex1, 16);
            byte b2 = Convert.ToByte(hex2, 16);
            return (b1 - b2).ToString(format);
        }
        public static string HexString_Mul(string hex1, string hex2, string format = "X2")
        {
            byte b1 = Convert.ToByte(hex1, 16);
            byte b2 = Convert.ToByte(hex2, 16);
            return (b1 * b2).ToString(format);
        }
        public static string HexString_Div(string hex1, string hex2, string format = "X2")
        {
            byte b1 = Convert.ToByte(hex1, 16);
            byte b2 = Convert.ToByte(hex2, 16);
            return (b1 / b2).ToString(format);
        }

        public static string HexString_DispLeft(string hex1, int n, string format = "X2")
        {
            byte b1 = Convert.ToByte(hex1, 16);
            return ((b1 << n) * 0xFF).ToString(format);
        }

        public static string HexString_DispRight(string hex1, int n, string format = "X2")
        {
            byte b1 = Convert.ToByte(hex1, 16);
            return ((b1 >> n)* 0xFF).ToString(format);
        }
    }
}
