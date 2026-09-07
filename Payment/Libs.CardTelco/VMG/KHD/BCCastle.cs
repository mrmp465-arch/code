using System;
using System.Collections.Generic;
using System.Web;
using Org.BouncyCastle;
using System.IO;
using System.Text;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Math;
/// <summary>
/// Summary description for BCCastle
/// </summary>


namespace Libs.CardTelco.VMG.KHD
{

    public class BCCastle
    {
        public BCCastle()
        {
            //
            // TODO: Add constructor logic here
            //                

        }



        public static AsymmetricCipherKeyPair GetPrivateKey(string privateKey)
        {
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(privateKey);
                Org.BouncyCastle.OpenSsl.PemReader pr = new Org.BouncyCastle.OpenSsl.PemReader(sr);
                AsymmetricCipherKeyPair KeyPair = (AsymmetricCipherKeyPair)pr.ReadObject();

                return KeyPair;
            }
            catch
            {

            }
            finally
            {
                try
                {
                    sr.Close();
                }
                catch
                {

                }
            }
            return null;
        }

        public static AsymmetricKeyParameter GetPublicKey(string publicKey)
        {
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(publicKey);
                Org.BouncyCastle.OpenSsl.PemReader pr = new Org.BouncyCastle.OpenSsl.PemReader(sr);
                AsymmetricKeyParameter KeyPair = (AsymmetricKeyParameter)pr.ReadObject();
                return KeyPair;
            }
            catch
            {

            }
            finally
            {
                try
                {
                    sr.Close();
                }
                catch
                {

                }
            }
            return null;
        }



        public static string Decrypt2(string privateKeyFileName, string encryptString)
        {
            try
            {
                AsymmetricCipherKeyPair keyPair = GetPrivateKey(privateKeyFileName);

                AsymmetricKeyParameter privateKey = keyPair.Private;

                // Creating the RSA algorithm object
                IAsymmetricBlockCipher cipher = new RsaEngine();
                Console.WriteLine("privateKey: " + privateKey.ToString());

                // Initializing the RSA object for Decryption with RSA private key. Remember, for decryption, private key is needed
                //cipher.Init(false, KeyPair.Private);            
                cipher.Init(false, keyPair.Private);


                byte[] encryptByte = Convert.FromBase64String(encryptString);

                //Encrypting the input bytes
                //byte[] cipheredBytes = cipher.ProcessBlock(inputBytes, 0, inputMessage.Length);
                byte[] cipheredBytes = cipher.ProcessBlock(encryptByte, 0, encryptByte.Length);

                //Write the encrypted message to file
                // Write encrypted text to file
                String decryptString = System.Text.Encoding.UTF8.GetString(cipheredBytes);
                return decryptString;
            }
            catch (Exception ex)
            {
                // Any errors? Show them            
                throw new Exception("Exception encrypting file! More info:", ex);
            }
            finally
            {

            }
        }


        public static string Encrypt2(string publicKeyFileName, string inputMessage)
        {


            //AsymmetricCipherKeyPair keyPair = GetPublicKey("");
            AsymmetricKeyParameter keyPair = GetPublicKey(publicKeyFileName);
            UTF8Encoding utf8enc = new UTF8Encoding();

            try
            {
                // Converting the string message to byte array
                byte[] inputBytes = utf8enc.GetBytes(inputMessage);

                AsymmetricKeyParameter publicKey = keyPair;//ReadAsymmetricKeyParameter(publicKeyFileName);

                // Creating the RSA algorithm object
                IAsymmetricBlockCipher cipher = new RsaEngine();

                // Initializing the RSA object for Encryption with RSA public key. Remember, for encryption, public key is needed
                cipher.Init(true, publicKey);

                //Encrypting the input bytes
                byte[] cipheredBytes = cipher.ProcessBlock(inputBytes, 0, inputMessage.Length);

                String encrypt = Convert.ToBase64String(cipheredBytes);

                return encrypt;
            }
            catch (Exception ex)
            {
                // Any errors? Show them
                throw new Exception("Encrypt string fail, detail as folowing", ex);
            }

        }
    }
}