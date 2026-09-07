using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Libs.Utils
{
    public class Encrypts
    {
        public Encrypts()
        {

        }

        public static string MD5(string data)
        {
            UTF8Encoding encoding1 = new UTF8Encoding();
            MD5CryptoServiceProvider provider1 = new MD5CryptoServiceProvider();
            byte[] buffer1 = encoding1.GetBytes(data);
            byte[] buffer2 = provider1.ComputeHash(buffer1);
            return BitConverter.ToString(buffer2).Replace("-", "").ToLower();
        }

        public static bool RSACheck(string data, string signature, string publicKey)
        {
            RSACryptoServiceProvider rsacp = new RSACryptoServiceProvider();
            rsacp.FromXmlString(publicKey);
            return rsacp.VerifyData(Encoding.UTF8.GetBytes(data), "SHA1", Convert.FromBase64String(signature));
        }

        public static string RSAGet(string data, string privateKey)
        {
            CspParameters _cpsParameter;
            RSACryptoServiceProvider rsaCryptoIPT;
            _cpsParameter = new CspParameters();
            _cpsParameter.Flags = CspProviderFlags.UseMachineKeyStore;
            rsaCryptoIPT = new RSACryptoServiceProvider(1024, _cpsParameter);

            rsaCryptoIPT.FromXmlString(privateKey);
            return Convert.ToBase64String(rsaCryptoIPT.SignData(new ASCIIEncoding().GetBytes(data), new SHA1CryptoServiceProvider()));
        }

        public static string Encrypt(string key, string data)
        {
            data = data.Trim();

            if (string.IsNullOrEmpty(data))
                return "Input string is empty!";

            byte[] keydata = Encoding.ASCII.GetBytes(key);

            string md5String = BitConverter.ToString(new

            MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-", "").ToLower();

            byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24));

            TripleDES tripdes = TripleDESCryptoServiceProvider.Create();

            tripdes.Mode = CipherMode.ECB;

            tripdes.Key = tripleDesKey;

            tripdes.GenerateIV();

            MemoryStream ms = new MemoryStream();

            CryptoStream encStream = new CryptoStream(ms, tripdes.CreateEncryptor(), CryptoStreamMode.Write);

            encStream.Write(Encoding.ASCII.GetBytes(data), 0, Encoding.ASCII.GetByteCount(data));

            encStream.FlushFinalBlock();

            byte[] cryptoByte = ms.ToArray();

            ms.Close();

            encStream.Close();

            return Convert.ToBase64String(cryptoByte, 0, cryptoByte.GetLength(0)).Trim();
        }

        public static string Decrypt(string key, string data)
        {
            byte[] keydata = Encoding.ASCII.GetBytes(key);

            string md5String = BitConverter.ToString(new

              MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-", "").Replace(" ", "+").ToLower();

            byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24));

            TripleDES tripdes = TripleDESCryptoServiceProvider.Create();

            tripdes.Mode = CipherMode.ECB;

            tripdes.Key = tripleDesKey;

            byte[] cryptByte = Convert.FromBase64String(data);

            MemoryStream ms = new MemoryStream(cryptByte, 0, cryptByte.Length);

            ICryptoTransform cryptoTransform = tripdes.CreateDecryptor();

            CryptoStream decStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Read);

            StreamReader read = new StreamReader(decStream);

            return (read.ReadToEnd());
        }

        public static string SHA256(string data)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            SHA256Managed sha256hasher = new SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(data));

            StringBuilder output = new StringBuilder("");
            for (int i = 0; i < hashedDataBytes.Length; i++)
            {
                output.Append(hashedDataBytes[i].ToString("X2"));
            }
            return output.ToString();
        }

        

        public static string SHA256(string data, string key)
        {
            String rs = "";
            byte[] keyByte = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(data);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                for (int i = 0; i < hashmessage.Length; i++)
                {
                    byte bit = hashmessage[i];
                    rs += bit.ToString("x2");
                }
            }
            return rs;
        }

        public static string HmacSHA1(string data, string key)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            byte[] keyByte = encoding.GetBytes(key);

            HMACSHA1 hmacsha1 = new HMACSHA1(keyByte);

            byte[] messageBytes = encoding.GetBytes(data);
            byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);

            string encrypted = "";
            for (int i = 0; i < hashmessage.Length; i++)
            {
                encrypted += hashmessage[i].ToString("X2"); // hex format
            }

            return encrypted;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string VGGAccessToken(string serviceId, string accountName)
        {
            string secretKey = "5f1416d19385d499e4de8a0551bf9401";

            var tmp1 = DateTime.Now.Ticks;
            var tmp = string.Format("{0}.{1}.{2}", accountName.ToLower().Replace('.', '-'), serviceId, tmp1);
            var md5 = Encrypts.MD5(string.Format("{0}{1}", tmp, secretKey));
            var tmp2 = md5;
            var accessToken = string.Format("{0}.{1}", tmp1, tmp2);

            return accessToken;
        }

    }
}
