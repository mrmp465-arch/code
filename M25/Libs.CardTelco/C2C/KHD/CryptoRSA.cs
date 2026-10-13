using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Libs.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace Libs.CardTelco.C2C.KHD
{
    public class CryptoRSA
    {

        /// <summary>
        /// Read Key from pem file
        /// </summary>
        /// <param name="keyFilePath"> path to pem file</param>
        /// <returns></returns>
        private static AsymmetricKeyParameter readKey(string keyFilePath)
        {
            try
            {

                AsymmetricCipherKeyPair keyPair;
                using (var reader = File.OpenText(keyFilePath))
                    keyPair = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();
                return keyPair.Private;

            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "C2C", "Error", "UseCard", "C2CRequest", "Fail to readKey RSA", ex.Message.Replace("\n", " ") });
                return null;
            }
        }
        /// <summary>
        /// Get Signature from plainText with path to key file
        /// </summary>
        /// <param name="plainText"> text for get signature</param>
        /// <param name="keyFilePath"> path to pem file</param>
        /// <returns></returns>
        public static String GetSignature(string plainText, String keyFilePath)
        {
            try
            {
                RsaKeyParameters privateKey = (RsaKeyParameters)readKey(keyFilePath);
                var encoder = new UTF8Encoding();
                var inputData = encoder.GetBytes(plainText);

                var signer = SignerUtilities.GetSigner("SHA1withRSA");
                signer.Init(true, privateKey);
                signer.BlockUpdate(inputData, 0, inputData.Length);

                String signature = Convert.ToBase64String(signer.GenerateSignature());
                return signature;
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "C2C", "Error", "UseCard", "C2CRequest", "Fail to GetSignature RSA", keyFilePath, ex.Message.Replace("\n", " ") });

                throw; //return null;
            }
        }

        public static string RsaEncryptWithPrivate(string clearText
            , string privateKey)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(clearText);

            var encryptEngine = new Pkcs1Encoding(new RsaEngine());

            using (var txtreader = new StringReader(privateKey))
            {
                var keyPair = (AsymmetricCipherKeyPair)new PemReader(txtreader).ReadObject();

                encryptEngine.Init(true, keyPair.Public);
            }

            var encrypted = Convert.ToBase64String(encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length));
            return encrypted;
        }


        /// <summary>
        /// Confirm signature
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="sSignature"></param>
        /// <param name="keyFilePath"></param>
        /// <returns></returns>
        public static bool VerifySignature(string plainText, string sSignature, string keyFilePath)
        {
            try
            {
                RsaKeyParameters publicKey = (RsaKeyParameters)readKey(keyFilePath);
                var encoder = new UTF8Encoding();
                var inputData = encoder.GetBytes(plainText);
                var signer = SignerUtilities.GetSigner("SHA1withRSA");
                signer.Init(false, publicKey);
                signer.BlockUpdate(inputData, 0, inputData.Length);

                byte[] signature = Convert.FromBase64String(sSignature);

                return signer.VerifySignature(signature);
            }
            catch (Exception ex)
            {

                NLogLogger.Info(new string[] { "C2C", "Error", "UseCard", "C2CRequest", "Fail to VerifySignature RSA", ex.Message.Replace("\n", " ") });
                return false;
            }
        }
        /// <summary>
        /// Generate RSA Keys
        /// </summary>
        /// <param name="keySizeInBits"></param>
        /// <returns></returns>
        public static AsymmetricCipherKeyPair GenerateKeys(int keySizeInBits, String privPkcs8Filename, String pubPkcs8Filename)
        {

            var r = new RsaKeyPairGenerator();
            r.Init(new KeyGenerationParameters(new SecureRandom(), keySizeInBits));
            var keys = r.GenerateKeyPair();

            var pkcs8Gen = new Pkcs8Generator(keys.Private);
            var pemObj = pkcs8Gen.Generate();
            var pkcs8Out = new StreamWriter(privPkcs8Filename, false);
            var pemWriter = new PemWriter(pkcs8Out);
            pemWriter.WriteObject(pemObj);
            pkcs8Out.Close();

            var textWriter = new StreamWriter(pubPkcs8Filename);
            pemWriter = new PemWriter(textWriter);
            pemWriter.WriteObject(keys.Public);
            pemWriter.Writer.Flush();
            textWriter.Close();

            return keys;

        }

    }
}
