using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System;
using Libs.Utils;

namespace Libs.TopupPartner.VNPTEPAY
{
    public class DesSecurity
    {
        private const string salt = "hywebpg5";

        public string Byte2Hex(byte[] b)
        {
            string str = "";
            string str2 = "";
            for (int i = 0; i < b.Length; i++)
            {
                str2 = Convert.ToString((int)(b[i] & 0xff), 0x10);
                if (str2.Length == 1)
                {
                    str = str + "0" + str2;
                }
                else
                {
                    str = str + str2;
                }
                if (i < (b.Length - 1))
                {
                    str = str ?? "";
                }
            }
            return str.ToUpper();
        }

        public string Byte2Hex_24(byte[] b)
        {
            string str = "";
            string str2 = "";
            for (int i = b.Length - 0x20; i < (b.Length - 8); i++)
            {
                str2 = Convert.ToString((int)(b[i] & 0xff), 0x10);
                if (str2.Length == 1)
                {
                    str = str + "0" + str2;
                }
                else
                {
                    str = str + str2;
                }
                if (i < (b.Length - 1))
                {
                    str = str ?? "";
                }
            }
            return str.ToUpper();
        }

        public string Byte2Hex_24_en(byte[] b)
        {
            string str = "";
            string str2 = "";
            for (int i = b.Length - 0x18; i < b.Length; i++)
            {
                str2 = Convert.ToString((int)(b[i] & 0xff), 0x10);
                if (str2.Length == 1)
                {
                    str = str + "0" + str2;
                }
                else
                {
                    str = str + str2;
                }
                if (i < (b.Length - 1))
                {
                    str = str ?? "";
                }
            }
            return str.ToUpper();
        }

        public string Byte2Hex1(byte[] b)
        {
            string str = "";
            string str2 = "";
            for (int i = b.Length - 0x10; i < (b.Length - 8); i++)
            {
                str2 = Convert.ToString((int)(b[i] & 0xff), 0x10);
                if (str2.Length == 1)
                {
                    str = str + "0" + str2;
                }
                else
                {
                    str = str + str2;
                }
                if (i < (b.Length - 1))
                {
                    str = str ?? "";
                }
            }
            return str.ToUpper();
        }

        public string Decrypt(string msg, string key)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("hywebpg5");
            byte[] data = this.Hex2Byte(msg);
            byte[] buffer3 = Encoding.UTF8.GetBytes(key);
            byte[] buffer4 = this.Decrypt(data, buffer3, bytes);
            return Encoding.UTF8.GetString(buffer4);
        }

        public byte[] Decrypt(byte[] Data, byte[] Key, byte[] IV)
        {
            try
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider
                {
                    Key = Key,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    IV = IV
                };
                MemoryStream stream = new MemoryStream(Data);
                CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Read);
                byte[] buffer = new byte[Data.Length];
                stream2.Read(buffer, 0, buffer.Length);
                List<byte> list = new List<byte>();
                foreach (byte num in buffer)
                {
                    if (num != 0)
                    {
                        list.Add(num);
                    }
                }
                return list.ToArray();
            }
            catch (CryptographicException exception)
            {
                NLogLogger.Info(string.Format("A Cryptographic error occurred: {0}, Data={1}, Length={2}", exception.Message, Data.Length, Data.Length ));
                return null;
            }
            catch (Exception exception2)
            {
                 NLogLogger.Info(string.Format("A exception occurred: {0}", exception2.Message));
                return null;
            }
        }

        public string Des3Decrypt(string data, string key)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] iv = null;
            byte[] buffer2 = this.Hex2Byte(data);
            byte[] bytes = encoding.GetBytes(key);
            byte[] buffer5 = RemovePadding(this.Des3Decrypt(buffer2, bytes, iv));
            return encoding.GetString(buffer5);
        }

        public byte[] Des3Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            try
            {
                TripleDES edes = TripleDES.Create();
                if (iv != null)
                {
                    edes.IV = iv;
                }
                edes.Key = key;
                edes.Mode = CipherMode.ECB;
                edes.Padding = PaddingMode.None;
                byte[] buffer = new byte[0];
                return edes.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
            }
            catch (CryptographicException exception)
            {
                NLogLogger.Info(string.Format("A Cryptographic error occurred: {0}, Data={1}, Length={2}", exception.Message, data.Length, data.Length));
                return null;
            }
            catch (Exception exception2)
            {
                NLogLogger.Info(string.Format("A exception occurred: {0}", exception2.Message ));
                return null;
            }
        }

        public string Des3Encrypt(string data, string key)
        {
            byte[] iv = null;
            byte[] bytes = Encoding.UTF8.GetBytes(Padding(data));
            byte[] buffer3 = Encoding.UTF8.GetBytes(key);
            byte[] b = this.Des3Encrypt(bytes, buffer3, iv);
            return this.Byte2Hex(b);
        }

        public byte[] Des3Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            byte[] buffer = new byte[0];
            try
            {
                TripleDES edes = TripleDES.Create();
                if (iv != null)
                {
                    edes.IV = iv;
                }
                edes.Key = key;
                edes.Mode = CipherMode.ECB;
                edes.Padding = PaddingMode.None;
                return edes.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
            }
            catch (CryptographicException exception)
            {
                 NLogLogger.Info(string.Format("A Cryptographic error occurred: {0}", exception.Message));
            }
            return buffer;
        }

        public string DESEDEMAC_24(string msg, string key)
        {
            int num = msg.Length % 8;
            if (num != 0)
            {
                for (int i = 0; i < (8 - num); i++)
                {
                    msg = msg + "0";
                }
            }
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes("hywebpg5");
            byte[] data = encoding.GetBytes(msg);
            byte[] buffer3 = encoding.GetBytes(key);
            byte[] b = this.Des3Encrypt(data, buffer3, bytes);
            return this.Byte2Hex1(b);
        }

        public string DESMAC(string msg, string key)
        {
            byte[] iv = null;
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            byte[] buffer3 = Encoding.UTF8.GetBytes(key);
            byte[] hashedBytes = new SHA1CryptoServiceProvider().ComputeHash(bytes);
            byte[] data = this.Padding(hashedBytes);
            byte[] b = this.Des3Encrypt(data, buffer3, iv);
            return this.Byte2Hex(b);
        }

        public string Encrypt(string msg, string key)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("hywebpg5");
            byte[] data = Encoding.UTF8.GetBytes(msg);
            byte[] buffer3 = Encoding.UTF8.GetBytes(key);
            byte[] b = this.Encrypt(data, buffer3, bytes);
            return this.Byte2Hex(b);
        }

        public byte[] Encrypt(byte[] Data, byte[] Key, byte[] IV)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider
                {
                    Key = Key,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    IV = IV
                };
                CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
                byte[] buffer = Data;
                stream2.Write(buffer, 0, buffer.Length);
                stream2.FlushFinalBlock();
                byte[] buffer2 = stream.ToArray();
                stream2.Close();
                stream.Close();
                return buffer2;
            }
            catch (CryptographicException exception)
            {
                 NLogLogger.Info(string.Format("A Cryptographic error occurred: {0}",  exception.Message));
                return null;
            }
        }
   
        public byte[] Hex2Byte(string hex)
        {
            if ((hex.Length % 2) != 0)
            {
                throw new ArgumentException();
            }
            char[] chArray = hex.ToCharArray();
            byte[] buffer = new byte[hex.Length / 2];
            int index = 0;
            int num2 = 0;
            int length = hex.Length;
            while (index < length)
            {
                int num4 = Convert.ToInt16("" + chArray[index++] + chArray[index], 0x10) & 0xff;
                buffer[num2] = Convert.ToByte(num4);
                index++;
                num2++;
            }
            return buffer;
        }

        private byte[] Padding(byte[] hashedBytes)
        {
            try
            {
                byte[] sourceArray = hashedBytes;
                int num = 8 - (sourceArray.Length % 8);
                byte[] destinationArray = new byte[sourceArray.Length + num];
                Array.Copy(sourceArray, destinationArray, sourceArray.Length);
                for (int i = sourceArray.Length; i < destinationArray.Length; i++)
                {
                    destinationArray[i] = 0;
                }
                return destinationArray;
            }
            catch (Exception exception)
            {
                 NLogLogger.Info(string.Format("Crypter.padding UnsupportedEncodingException: {0}", exception.Message));
            }
            return null;
        }

        public static string Padding(string str)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                int num = 8 - (bytes.Length % 8);
                byte[] destinationArray = new byte[bytes.Length + num];
                Array.Copy(bytes, destinationArray, bytes.Length);
                for (int i = bytes.Length; i < destinationArray.Length; i++)
                {
                    destinationArray[i] = 0;
                }
                return Encoding.UTF8.GetString(destinationArray);
            }
            catch (Exception exception)
            {
                NLogLogger.Info(string.Format("Crypter.padding UnsupportedEncodingException: {0}", exception.Message));
            }
            return null;
        }

        public static byte[] RemovePadding(byte[] oldByteArray)
        {
            int num = 0;
            for (int i = oldByteArray.Length; i >= 0; i--)
            {
                if (oldByteArray[i - 1] != 0)
                {
                    num = oldByteArray.Length - i;
                    break;
                }
            }
            byte[] destinationArray = new byte[oldByteArray.Length - num];
            Array.Copy(oldByteArray, destinationArray, destinationArray.Length);
            return destinationArray;
        }

        public string Decryption(string cypherText, string key)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] b = Convert.FromBase64String(cypherText);
            TripleDES des = CreateTripleDES(key);
            
            //ICryptoTransform ct = des.CreateDecryptor();
            byte[] output = des.CreateDecryptor().TransformFinalBlock(b, 0, b.Length);
            //byte[] output = ct.TransformFinalBlock(b, 0, b.Length);

            return ASCIIEncoding.ASCII.GetString(output);// Encoding.ASCII.GetString(output);
        }


        public TripleDES CreateTripleDES(string key)
        {
            byte[] key_byte = ASCIIEncoding.ASCII.GetBytes(MD5_Hash(key).Substring(0, 24));
            TripleDES des = new TripleDESCryptoServiceProvider();
            des.Key = key_byte;
            des.IV = ASCIIEncoding.ASCII.GetBytes(MD5_Hash(key).Substring(0, 8));
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.None;
            return des;
        }
        public string Des3DecryptTopup(string data, string key)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] iv = null;
            byte[] buffer2 = Convert.FromBase64String(data);
            byte[] bytes = encoding.GetBytes(MD5_Hash(key).Substring(0, 24));
            byte[] buffer5 = RemovePadding(this.Des3Decrypt(buffer2, bytes, iv));
            return encoding.GetString(buffer5);
        }
        public string MD5_Hash(String data)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            //compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(data));
            //get hash result after compute it
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }


    }
}
