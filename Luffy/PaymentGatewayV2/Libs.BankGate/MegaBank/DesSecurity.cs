using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libs.BankGate.MegaBank
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class DesSecurity
    {
        private const string salt = "hywebpg5";

        public string Byte2Hex(byte[] b)
        {
            string str1 = "";
            for (int index = 0; index < b.Length; ++index)
            {
                string str2 = Convert.ToString((int)b[index] & (int)byte.MaxValue, 16);
                str1 = str2.Length != 1 ? str1 + str2 : str1 + "0" + str2;
                if (index < b.Length - 1)
                    str1 = str1 ?? "";
            }
            return str1.ToUpper();
        }

        public string Byte2Hex_24(byte[] b)
        {
            string str1 = "";
            for (int index = b.Length - 32; index < b.Length - 8; ++index)
            {
                string str2 = Convert.ToString((int)b[index] & (int)byte.MaxValue, 16);
                str1 = str2.Length != 1 ? str1 + str2 : str1 + "0" + str2;
                if (index < b.Length - 1)
                    str1 = str1 ?? "";
            }
            return str1.ToUpper();
        }

        public string Byte2Hex_24_en(byte[] b)
        {
            string str1 = "";
            for (int index = b.Length - 24; index < b.Length; ++index)
            {
                string str2 = Convert.ToString((int)b[index] & (int)byte.MaxValue, 16);
                str1 = str2.Length != 1 ? str1 + str2 : str1 + "0" + str2;
                if (index < b.Length - 1)
                    str1 = str1 ?? "";
            }
            return str1.ToUpper();
        }

        public string Byte2Hex1(byte[] b)
        {
            string str1 = "";
            for (int index = b.Length - 16; index < b.Length - 8; ++index)
            {
                string str2 = Convert.ToString((int)b[index] & (int)byte.MaxValue, 16);
                str1 = str2.Length != 1 ? str1 + str2 : str1 + "0" + str2;
                if (index < b.Length - 1)
                    str1 = str1 ?? "";
            }
            return str1.ToUpper();
        }

        public string Decrypt(string msg, string key)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("hywebpg5");
            return Encoding.UTF8.GetString(this.Decrypt(this.Hex2Byte(msg), Encoding.UTF8.GetBytes(key), bytes));
        }

        public byte[] Decrypt(byte[] Data, byte[] Key, byte[] IV)
        {
            try
            {
                DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
                cryptoServiceProvider.Key = Key;
                cryptoServiceProvider.Mode = CipherMode.CBC;
                cryptoServiceProvider.Padding = PaddingMode.PKCS7;
                cryptoServiceProvider.IV = IV;
                CryptoStream cryptoStream = new CryptoStream((Stream)new MemoryStream(Data), cryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Read);
                byte[] buffer = new byte[Data.Length];
                cryptoStream.Read(buffer, 0, buffer.Length);
                List<byte> byteList = new List<byte>();
                foreach (byte num in buffer)
                {
                    if (num != (byte)0)
                        byteList.Add(num);
                }
                return byteList.ToArray();
            }
            catch (CryptographicException ex)
            {
                return (byte[])null;
            }
            catch (Exception ex)
            {
                return (byte[])null;
            }
        }

        public string Des3Decrypt(string data, string key)
        {
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            byte[] iv = (byte[])null;
            byte[] bytes = DesSecurity.RemovePadding(this.Des3Decrypt(this.Hex2Byte(data), asciiEncoding.GetBytes(key), iv));
            return asciiEncoding.GetString(bytes);
        }

        public byte[] Des3Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            try
            {
                TripleDES tripleDes = TripleDES.Create();
                if (iv != null)
                    tripleDes.IV = iv;
                tripleDes.Key = key;
                tripleDes.Mode = CipherMode.ECB;
                tripleDes.Padding = PaddingMode.None;
                byte[] numArray = new byte[0];
                return tripleDes.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
            }
            catch (CryptographicException ex)
            {
                return (byte[])null;
            }
            catch (Exception ex)
            {
                return (byte[])null;
            }
        }

        public string Des3Encrypt(string data, string key)
        {
            byte[] iv = (byte[])null;
            return this.Byte2Hex(this.Des3Encrypt(Encoding.UTF8.GetBytes(DesSecurity.Padding(data)), Encoding.UTF8.GetBytes(key), iv));
        }

        public byte[] Des3Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            byte[] numArray = new byte[0];
            try
            {
                TripleDES tripleDes = TripleDES.Create();
                if (iv != null)
                    tripleDes.IV = iv;
                tripleDes.Key = key;
                tripleDes.Mode = CipherMode.ECB;
                tripleDes.Padding = PaddingMode.None;
                return tripleDes.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
            }
            catch (CryptographicException ex)
            {
            }
            return numArray;
        }

        public string DESEDEMAC_24(string msg, string key)
        {
            int num = msg.Length % 8;
            if (num != 0)
            {
                for (int index = 0; index < 8 - num; ++index)
                    msg += "0";
            }
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            byte[] bytes = asciiEncoding.GetBytes("hywebpg5");
            return this.Byte2Hex1(this.Des3Encrypt(asciiEncoding.GetBytes(msg), asciiEncoding.GetBytes(key), bytes));
        }

        public string DESMAC(string msg, string key)
        {
            byte[] iv = (byte[])null;
            return this.Byte2Hex(this.Des3Encrypt(this.Padding(new SHA1CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(msg))), Encoding.UTF8.GetBytes(key), iv));
        }

        public string Encrypt(string msg, string key)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("hywebpg5");
            return this.Byte2Hex(this.Encrypt(Encoding.UTF8.GetBytes(msg), Encoding.UTF8.GetBytes(key), bytes));
        }

        public byte[] Encrypt(byte[] Data, byte[] Key, byte[] IV)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
                cryptoServiceProvider.Key = Key;
                cryptoServiceProvider.Mode = CipherMode.CBC;
                cryptoServiceProvider.Padding = PaddingMode.PKCS7;
                cryptoServiceProvider.IV = IV;
                CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, cryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
                byte[] buffer = Data;
                cryptoStream.Write(buffer, 0, buffer.Length);
                cryptoStream.FlushFinalBlock();
                byte[] array = memoryStream.ToArray();
                cryptoStream.Close();
                memoryStream.Close();
                return array;
            }
            catch (CryptographicException ex)
            {
                return (byte[])null;
            }
        }

        public byte[] Hex2Byte(string hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException();
            char[] charArray = hex.ToCharArray();
            byte[] numArray = new byte[hex.Length / 2];
            int num1 = 0;
            int index1 = 0;
            int length = hex.Length;
            while (num1 < length)
            {
                char[] chArray = charArray;
                int index2 = num1;
                int index3 = index2 + 1;
                int num2 = (int)Convert.ToInt16(((int)chArray[index2]).ToString() + (object)charArray[index3], 16) & (int)byte.MaxValue;
                numArray[index1] = Convert.ToByte(num2);
                num1 = index3 + 1;
                ++index1;
            }
            return numArray;
        }

        private byte[] Padding(byte[] hashedBytes)
        {
            try
            {
                byte[] numArray1 = hashedBytes;
                int num = 8 - numArray1.Length % 8;
                byte[] numArray2 = new byte[numArray1.Length + num];
                Array.Copy((Array)numArray1, (Array)numArray2, numArray1.Length);
                for (int length = numArray1.Length; length < numArray2.Length; ++length)
                    numArray2[length] = (byte)0;
                return numArray2;
            }
            catch (Exception ex)
            {
            }
            return (byte[])null;
        }

        public static string Padding(string str)
        {
            try
            {
                byte[] bytes1 = Encoding.UTF8.GetBytes(str);
                int num = 8 - bytes1.Length % 8;
                byte[] bytes2 = new byte[bytes1.Length + num];
                Array.Copy((Array)bytes1, (Array)bytes2, bytes1.Length);
                for (int length = bytes1.Length; length < bytes2.Length; ++length)
                    bytes2[length] = (byte)0;
                return Encoding.UTF8.GetString(bytes2);
            }
            catch (Exception ex)
            {
            }
            return (string)null;
        }

        public static byte[] RemovePadding(byte[] oldByteArray)
        {
            int num = 0;
            for (int length = oldByteArray.Length; length >= 0; --length)
            {
                if (oldByteArray[length - 1] != (byte)0)
                {
                    num = oldByteArray.Length - length;
                    break;
                }
            }
            byte[] numArray = new byte[oldByteArray.Length - num];
            Array.Copy((Array)oldByteArray, (Array)numArray, numArray.Length);
            return numArray;
        }
    }

}
