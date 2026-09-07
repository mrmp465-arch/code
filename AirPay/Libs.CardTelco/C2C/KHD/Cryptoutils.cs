using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Libs.Utils;

namespace Libs.CardTelco.C2C.KHD
{
    public static class CryptoUtils
    {
        public static string Encrypt(string toEncrypt, string key)
        {
            try
            {
                TripleDES des = CreateDES(key);
                ICryptoTransform cTransform = des.CreateEncryptor();
                byte[] input = Encoding.UTF8.GetBytes(toEncrypt);
                byte[] output = cTransform.TransformFinalBlock(input, 0, input.Length);
                return Convert.ToBase64String(output);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "C2C", "Error", "UseCard", "C2CRequest", "Fail to encrypt TripleDES", ex.Message.Replace("\n", " ") });
                return null;
            }
        }

        /// <summary>
        /// Decrypt text with key
        /// </summary>
        /// <param name="cipherString">text to decrypt</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string Decrypt(string cipherString, string key)
        {
            try
            {
                byte[] bCipher = Convert.FromBase64String(cipherString);
                TripleDES des = CreateDES(key);

                ICryptoTransform cTransform = des.CreateDecryptor();
                byte[] output = cTransform.TransformFinalBlock(bCipher, 0, bCipher.Length);
                return Encoding.UTF8.GetString(output);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "C2C", "Error", "UseCard", "C2CRequest", "Fail to decrypt TripleDES", ex.Message.Replace("\n", " ") });
                return ex.Message;
            }
        }
        static TripleDES CreateDES(string key)
        {
            TripleDES des = new TripleDESCryptoServiceProvider();
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                String hex = ConvertToHex(GetHash(key)).Substring(0, 24);
                des.Key = Encoding.ASCII.GetBytes(hex);
                des.Padding = PaddingMode.PKCS7;
                des.Mode = CipherMode.ECB;
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "C2C", "Error", "UseCard", "C2CRequest", "Fail to CreateDES TripleDES", ex.Message.Replace("\n", " ") });
                return null;
            }
            return des;
        }
        public static string ConvertToHex(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  // SHA1.Create()
            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            Encoding utf8 = Encoding.UTF8;
            byte[] utfBytes = utf8.GetBytes(inputString);
            byte[] isoBytes = Encoding.Convert(utf8, iso, utfBytes);
            return algorithm.ComputeHash(isoBytes);
        }
    }
}