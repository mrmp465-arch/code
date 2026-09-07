using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace APIGame.Sercurity
{
    public class Encrypts
    {
        public static string Encrypt(string PlainText, string keyStr)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.BlockSize = 128;
            aes.KeySize = 128;

            /// In Java, Same with below code
            /// Cipher _Cipher = Cipher.getInstance("AES");  // Java Code
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;

            byte[] keyArr = Encoding.UTF8.GetBytes(keyStr);
            byte[] KeyArrBytes32Value = new byte[32];
            Array.Copy(keyArr, KeyArrBytes32Value, 32);

            aes.Key = KeyArrBytes32Value;

            ICryptoTransform encrypto = aes.CreateEncryptor();

            byte[] plainTextByte = Encoding.UTF8.GetBytes(PlainText);
            byte[] CipherText = encrypto.TransformFinalBlock(plainTextByte, 0, plainTextByte.Length);
            return Convert.ToBase64String(CipherText);
        }

        public static string Decrypt(string CipherText, string keyStr)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.BlockSize = 128;
            aes.KeySize = 128;

            /// In Java, Same with below code
            /// Cipher _Cipher = Cipher.getInstance("AES");  // Java Code
            aes.Mode = CipherMode.ECB;

            byte[] keyArr = Encoding.UTF8.GetBytes(keyStr);
            byte[] KeyArrBytes32Value = new byte[16];
            Array.Copy(keyArr, KeyArrBytes32Value, 16);

            aes.Key = KeyArrBytes32Value;

            ICryptoTransform decrypto = aes.CreateDecryptor();

            byte[] encryptedBytes = Convert.FromBase64CharArray(CipherText.ToCharArray(), 0, CipherText.Length);
            byte[] decryptedData = decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return ASCIIEncoding.UTF8.GetString(decryptedData);
        }

        public static string BuildKey(string msisdn, string token)
        {

            if (!msisdn.StartsWith("0") && msisdn.Length < 10)
            {
                msisdn = "0" + msisdn;
            }

            var tokenIndex = token.ToCharArray();
            StringBuilder key = new StringBuilder();
            int i = 0;
            foreach (var c in msisdn.TrimStart('0'))
            {
                if (i % 4 != 0)
                {
                    var j = Convert.ToInt32(c.ToString());
                    key.Append(tokenIndex[j]);

                }
                i++;
            }
            int m = 0;
            foreach (var c in msisdn)
            {

                var j = Convert.ToInt32(c.ToString());
                key.Append(tokenIndex[m * j]);
                m++;

            }



            return key.ToString();
        }

        private static string toHexString(byte[] bytes, bool lowercase = true)
        {
            StringBuilder sb = new StringBuilder();
            string format = lowercase ? "x2" : "X2";
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString(format));
            }

            return sb.ToString();
        }
        public static string HashSHA256(string input)
        {
            using (SHA256 sha1 = SHA256.Create())
            {
                return toHexString(sha1.ComputeHash(Encoding.UTF8.GetBytes(input ?? string.Empty)));
            }
        }

        public static string MD5(string data)
        {
            UTF8Encoding encoding1 = new UTF8Encoding();
            MD5CryptoServiceProvider provider1 = new MD5CryptoServiceProvider();
            byte[] buffer1 = encoding1.GetBytes(data);
            byte[] buffer2 = provider1.ComputeHash(buffer1);
            return BitConverter.ToString(buffer2).Replace("-", "").ToLower();
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string ToHexString(string str)
        {
            var sb = new StringBuilder();

            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString(); // returns: "48656C6C6F20776F726C64" for "Hello world"
        }

        public static string FromHexString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return Encoding.Unicode.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"
        }

    }
}