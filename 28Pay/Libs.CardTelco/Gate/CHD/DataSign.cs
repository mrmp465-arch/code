using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Libs.CardTelco.Gate.CHD
{
    public class DataSign
    {
        private UTF8Encoding enc = new UTF8Encoding();
        private string strOriginalData;
        private RSAParameters rsaPrivateParams;
        //a byte array to store hash value

        private byte[] hashedData;
        public DataSign()
            : base()
        {
        }

        public string OriginalData
        {
            get { return strOriginalData; }
            set { strOriginalData = value; }
        }

        public RSAParameters PrivateParams
        {
            get { return rsaPrivateParams; }
            set { this.rsaPrivateParams = value; }
        }

        //Manually performs hash and then signs hashed value.
        public byte[] HashAndSign(byte[] encrypted)
        {
            //create new instance of RSACryptoServiceProvider
            RSACryptoServiceProvider rsaCSP = new RSACryptoServiceProvider();
            //create new instance of SHA1 hash algorithm to compute hash
            SHA1Managed hash = new SHA1Managed();
            try
            {
                //import private key params into instance of RSACryptoServiceProvider
                rsaCSP.ImportParameters(rsaPrivateParams);
                //compute hash with algorithm specified as here we have SHA1
                hashedData = hash.ComputeHash(encrypted);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            // Sign Data using private key and  OID is simple name of the algorithm for which to get the object identifier (OID)
            return rsaCSP.SignHash(hashedData, CryptoConfig.MapNameToOID("SHA1"));
        }
        //HashAndSign

        public RSAParameters ReadPrivateKeyFromFile(string fileName)
        {
            RSAParameters param = new RSAParameters();
            try
            {
                FileStream sw = File.OpenRead(fileName);
                param.P = ReadByteArray(sw);
                param.Q = ReadByteArray(sw);
                param.D = ReadByteArray(sw);
                param.DP = ReadByteArray(sw);
                param.DQ = ReadByteArray(sw);
                param.InverseQ = ReadByteArray(sw);
                param.Exponent = ReadByteArray(sw);
                param.Modulus = ReadByteArray(sw);
                sw.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            return param;
        }

        private byte[] ReadByteArray(FileStream writer)
        {
            int length = 0;
            for (int i = 0; i <= 3; i++)
            {
                length = length | writer.ReadByte() >> (8 * (32 - 8 - i));
            }
            if (length == 0)
            {
                return null;
            }
            byte[] array = new byte[length];
            writer.Read(array, 0, length);
            return array;
        }

        public string Sign(string OriginalData, string strPrivateKeyFile)
        {
            string strSignature = null;
            try
            {
                PrivateParams = ReadPrivateKeyFromFile(strPrivateKeyFile);
                strSignature = Convert.ToBase64String((HashAndSign(enc.GetBytes(OriginalData))));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            return strSignature;
        }

        public string Sign()
        {
            return Convert.ToBase64String((HashAndSign(enc.GetBytes(strOriginalData))));
        }

        public string SignX509_PKCS12(bool bMethod, string PrivateKeyPath, string PrivateKeyPassword, string OriginalData)
        {
            try
            {
                if (bMethod)
                {
                    X509Certificate2 cert = new X509Certificate2(PrivateKeyPath, PrivateKeyPassword, X509KeyStorageFlags.MachineKeySet);
                    RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PrivateKey;

                    SHA1Managed sha1 = new SHA1Managed();
                    byte[] data = UTF8Encoding.UTF8.GetBytes(OriginalData);
                    byte[] hash = sha1.ComputeHash(data);
                    return Convert.ToBase64String(csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1")));
                }
                else
                {
                    DataSign objSign = new DataSign();
                    return objSign.Sign(OriginalData, PrivateKeyPath);
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }

        public string SignX509(bool bMethod, string CertPath, string CertPassword, string OriginalData)
        {
            try
            {
                if (bMethod)
                {
                    X509Certificate2 x509 = new X509Certificate2(CertPath, CertPassword);

                    ContentInfo ci = new ContentInfo(enc.GetBytes(OriginalData));
                    SignedCms sc = new SignedCms(ci, true);
                    //Dim cs As CmsSigner = New CmsSigner(x509)
                    CmsSigner cs = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, x509);
                    //cs.IncludeOption = X509IncludeOption.None
                    sc.ComputeSignature(cs);
                    byte[] signed = sc.Encode();

                    return Convert.ToBase64String(signed);
                }
                else
                {
                    DataSign objSign = new DataSign();
                    return objSign.Sign(OriginalData, CertPath);
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Computes a signature for the specified data using SHA1 for hashing followed by encryption using the private key
        /// </summary>
        /// <param name="PrivKeyPath"></param>
        /// <param name="PwdPrivKey"></param>
        /// <param name="OriginData"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string SignData(string OriginData, string PrivKeyPath, string PwdPrivKey)
        {
            try
            {
                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                X509Certificate2 x509Cert = new X509Certificate2(PrivKeyPath, PwdPrivKey, X509KeyStorageFlags.MachineKeySet);
                RSACryptoServiceProvider rsaCryptoIPT = (RSACryptoServiceProvider)x509Cert.PrivateKey;
                byte[] data = UTF8Encoding.UTF8.GetBytes(OriginData);
                return Convert.ToBase64String(rsaCryptoIPT.SignData(data, sha1));
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string SignRSAKey(string OriginalData, string PrivateKeyPath)
        {
            try
            {
                RSACryptoServiceProvider RSAProvider = new RSACryptoServiceProvider();
                PrivateKeyPath = GetKeyFromFile(PrivateKeyPath);
                RSAProvider.FromXmlString(PrivateKeyPath);
                byte[] byteKey = null;
                byteKey = Encoding.ASCII.GetBytes(OriginalData);
                byte[] signature = null;
                signature = RSAProvider.SignData(byteKey, new SHA1CryptoServiceProvider());
                string sSing = Convert.ToBase64String(signature);
                return sSing;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private static string GetKeyFromFile(string Path)
        {
            StreamReader stream = new StreamReader(Path);
            string data = stream.ReadToEnd();
            stream.Close();
            return data;
        }
    }
}
