using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Libs.CardTelco._1Pay.KHD
{
  public static class CryptoUtils
  {
    public static string HashMD5(string input)
    {
      using (MD5 md5 = MD5.Create())
      {
        // Tránh dùng Encoding.Default vì phụ thuộc môi trường thực thi 
        return toHexString(md5.ComputeHash(Encoding.UTF8.GetBytes(input ?? string.Empty)), false);
      }
    }

    public static string HashSHA1(string input)
    {
      using (SHA1 sha1 = SHA1.Create())
      {
        return toHexString(sha1.ComputeHash(Encoding.UTF8.GetBytes(input ?? string.Empty)));
      }
    }

    public static string EncryptTripleDES(string input, string key)
    {
      return Convert.ToBase64String(EncryptTripleDES(Encoding.UTF8.GetBytes(input ?? string.Empty), Encoding.ASCII.GetBytes(key ?? string.Empty)));
    }

    public static string ZopostEncryptTripleDES(string input, string key)
    {
        return Convert.ToBase64String(ZopostEncryptTripleDES(Encoding.UTF8.GetBytes(input ?? string.Empty), Encoding.ASCII.GetBytes(key ?? string.Empty)));
    }

    public static string DecryptTripleDES(string input, string key)
    {
      return Encoding.UTF8.GetString(DecryptTripleDES(Convert.FromBase64String(input ?? string.Empty), Encoding.ASCII.GetBytes(key ?? string.Empty)));
    }

    private static byte[] EncryptTripleDES(byte[] input, byte[] key)
    {
      using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider())
      {
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
          tripleDES.Key = md5.ComputeHash(key);
          tripleDES.Mode = CipherMode.ECB;

          return tripleDES.CreateEncryptor().TransformFinalBlock(input, 0, input.Length);
        }
      }
    }

    private static byte[] ZopostEncryptTripleDES(byte[] input, byte[] key)
    {
        using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider())
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                tripleDES.Key = md5.ComputeHash(key);
                tripleDES.Mode = CipherMode.CBC;

                return tripleDES.CreateEncryptor().TransformFinalBlock(input, 0, input.Length);
            }
        }
    }

    private static byte[] DecryptTripleDES(byte[] input, byte[] key)
    {
      using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider())
      {
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
          tripleDES.Key = md5.ComputeHash(key);
          tripleDES.Mode = CipherMode.ECB;

          return tripleDES.CreateDecryptor().TransformFinalBlock(input, 0, input.Length);
        }
      }
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
  }
}