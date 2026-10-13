using Microsoft.VisualStudio.TestTools.UnitTesting;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace UnitTest
{
    [TestClass()]
    public class EncryptsTests
    {
        [TestMethod()]
        public void DecryptTest()
        {
            var result = Decrypt("2ZD7S7561poK8ksXrW+7vxymx2S6bYWmdj2zT1TwPMqf/ndsCBsLTN+XHBBveLb49gGPgRCIOguNj60StUkXAzBDOrKoXld2YALygSRVYPiQY/1g8MXru+Tdm1YQWNQAjtKuCXheD3O9OEOl1gsWGJltz7mZyvgUY10TGKj9mnVNMOoUwErwkDkW3650ZOVtoiBA3X0WOwf2SyYzrps19npOVoWx1M2Bm4y4CkvbXVaAgyYJoZkDQPSr5DlyLW0N0EpgcJauEtaZ9BwvL0/QcdeSD/SUSAjOlqQ9HBrl4D6zbnpOILvnrCg20/caPsCW7TaMHmPO9yjmpeBU05siF89x0s2SF8ly8QszM4zhnWyh3S2iDUOr87f1o385Y36rkGhtUXQoB6arsN2PMf+aqK8VJE49DOOuG1uXmWuCMQDHj12CdqqEBfeXtTkdCwL+aNO0x21OVhLfRK/IPrCDYdTiU4hAu04vfp7U01jKI2YnZxdpalktF64yipXxNsCnUlE4o7iDOiC/ML9mvBMBvDFMU+xLYTAIbb+n9KsCOkRIE2vdan6XFOQeIDyOMaxYfcMUCjwvU7bP8OjarNn1rg=="
            , "d02e46eaa1ad0c391e2d8407306ccfc8");
            Console.Write(result);

        }


        public string Encrypt(string msg, string key)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("hywebpg5");
            byte[] data = Encoding.UTF8.GetBytes(msg);
            byte[] buffer3 = Encoding.UTF8.GetBytes(key);
            byte[] b = this.Encrypt(data, buffer3, bytes);
            return Convert.ToBase64String(b);

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
                NLogLogger.Info(string.Format("A Cryptographic error occurred: {0}", exception.Message));
                return null;
            }
        }

        public string Decrypt(string msg, string key)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("hywebpg5");
            byte[] data = Convert.FromBase64String(msg);
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
                NLogLogger.Info(string.Format("A Cryptographic error occurred: {0}, Data={1}, Length={2}", exception.Message, Data.Length, Data.Length));
                return null;
            }
            catch (Exception exception2)
            {
                NLogLogger.Info(string.Format("A exception occurred: {0}", exception2.Message));
                return null;
            }
        }

        [TestMethod()]
        public void MD5Test()
        {
            var result =Encrypts.MD5("game1MOBI8803382973710801210000139808837467100008837467http://149.28.130.246:1590/CardCallback.ashxgame1@2019");
            Console.Write(result);
        }
    }
}