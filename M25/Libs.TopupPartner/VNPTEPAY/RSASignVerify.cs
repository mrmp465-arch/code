using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Collections;
using System.IO;

namespace Libs.TopupPartner.VNPTEPAY
{
    public class RSASignVerify
    {


        //public static string SignData0(string data)
        //{
           
        //    byte[] privatebKEY = JavaScience.opensslkey.DecodePkcs8PrivateKey(TopupConstant.topupvmgprivatekey);
        //    RSACryptoServiceProvider rsa = JavaScience.opensslkey.DecodePrivateKeyInfo(privatebKEY); //new RSACryptoServiceProvider(1024);

        //    // Because each new RSA instance is created with new random keys, we actually need to load the XML key data
        //    // into our RSA object in real world use. This private key value should come from reading the contents of a file
        //    // so that we are using the same key every time, rather than the private member variable created above.
        //    // Create our hash object that will compute the hash of our data
        //    SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
        //    // Get our data as bytes
        //    byte[] plainBytes = Encoding.UTF8.GetBytes(data);
        //    // Create our hash
        //    byte[] hashedBytes = sha1.ComputeHash(plainBytes);
        //    // Because each new RSA instance is created with new random keys, we actually need to load the XML key data
        //    // into our RSA object in real world use. This private key value should come from reading the contents of a file
        //    // So that we are using the same key every time, rather than the private member variable created above.
        //    // Sign (encrypt) our hash
        //    byte[] signedHash = rsa.SignHash(hashedBytes, CryptoConfig.MapNameToOID("SHA1"));
        //    // Return the hashed data as a string that we can easily store in a text or xml file.
        //    return Convert.ToBase64String(signedHash);

        //}
        public static string SignData(string inputString)
        {
            int dwKeySize = 1024;
            //StreamReader streamReader = new StreamReader( ConvertUtility.get_setting("privatekey"), true);
            //string xmlString = streamReader.ReadToEnd();

            string xmlString = TopupConstant.topupvmgprivatekey;

            // TODO: Add Proper Exception Handlers
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            //rsaCryptoServiceProvider.PublicOnly = false;
            int keySize = dwKeySize / 8;
            //byte[] bytes = Encoding.UTF32.GetBytes(inputString);
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
           
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            // Get our data as bytes
            byte[] plainBytes = Encoding.UTF8.GetBytes(inputString);
            // Create our hash
            byte[] hashedBytes = sha1.ComputeHash(plainBytes);
            // Because each new RSA instance is created with new random keys, we actually need to load the XML key data
            // into our RSA object in real world use. This private key value should come from reading the contents of a file
            // So that we are using the same key every time, rather than the private member variable created above.
            // Sign (encrypt) our hash
            byte[] signedHash = rsaCryptoServiceProvider.SignHash(hashedBytes, CryptoConfig.MapNameToOID("SHA1"));
            // Return the hashed data as a string that we can easily store in a text or xml file.
            return Convert.ToBase64String(signedHash);
        }

        //public static bool VerifyHash0(string encryptedHash, string originalData)
        //{
        //    //String privatekey = "MIICeAIBADANBgkqhkiG9w0BAQEFAASCAmIwggJeAgEAAoGBAJa8w7fm3AwuqCNLTFI2ldPK7PCRbtPJJNOMOyaoSYMq+3lSx50HpqDyvM92//8Y3SL4V1GtWKTYLcSg4TaBOuEokxmJwPN59szaXyQGIcjPd9zLaWub0sTtjNMKrZBa6+d38Obf9NT530uZWfoww88F/MEedGG63B4D48LHgdw5AgMBAAECgYBHHyWRfcHRlaoLjRmjqNGkrpiBIX2TO9K+Zen64WheFUe2BNLeSp/aTO29Tb6X1FlyiI5aoVmz3bQqlGXu26a1nbqd+m/YpneebgPJPNtwESpEBhDjeVNxXbVNS/r1KmFBkJcHxP3p1eaRZ1KfCzQEMWSr6M4Pd319YXXsYZPL6QJBAOoxJ7FQmA7yM1K2xENlPTL27Bj2iUI8kfelYkTIPJaFZdoVBBprxh+uhobRSOPuDbiYssL44evxxL0LbOLI82sCQQCkxiemSWu1U70Q4wtktz09AxuUOSamKK4bnf+XBMLPKNQ5h8TvLkxERd7FX9uoCwbIn5IP9mwKv1MLC0j/wnvrAkEAkPeMkn5JIjda/cCVDQMGNx4SWAGERbQoxxthESLCHorE2ZJYz+IW6lWmgJ3cePtLExGy1m4pq1wlZMBFzryFsQJBAJsphhjOtsCP1FBcnVQAQYh8rrHLh5ucXyfehtqdPx144zAxq79Xp7X63aabd+ssRv0RVqf1cxRDyl1yCRHfAHUCQQC+ujwGOc2jZflCkJ9CxMAYkwpYm0I4AXeyKOzZ8YJzp4kEVttJzakGGQ9+KAd0fMp8316SC99byy0xmo/j6xQ5";
        //    //String publickey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCWvMO35twMLqgjS0xSNpXTyuzwkW7TySTTjDsmqEmDKvt5UsedB6ag8rzPdv//GN0i+FdRrVik2C3EoOE2gTrhKJMZicDzefbM2l8kBiHIz3fcy2lrm9LE7YzTCq2QWuvnd/Dm3/TU+d9LmVn6MMPPBfzBHnRhutweA+PCx4HcOQIDAQAB";
        //    //publickey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC5GgB2xEClynSmkz9fC6gKJCTppJjfe2J9p+uwDsxIvJYuRK/BbyvRejPiV6T9AAdAhwiixelhHlLveJKqzOkwaDZd5tDS4Q3TNAIBvFzpxyYm0uXn6qIare+mzwPdzWjb/VdsxIRrDcz8jW6Gcx8NNYvymJmtzyOYzHN8fFXCTQIDAQAB";
        //    ////RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1028);
        //    //byte[] publicbKEY = Convert.FromBase64String(publickey);
        //    byte[] publicbKEY = Convert.FromBase64String(TopupConstant.topupvmgpublickey);
        //    RSACryptoServiceProvider rsa = JavaScience.opensslkey.DecodeX509PublicKey(publicbKEY);
        //// Because each new RSA instance is created with new random keys, we actually need to load the XML key data
        //// into our RSA object in real world use. This public key value should come from reading the contents of a file
        //// or resource object that was packed into our application or dll file
        //// so that we are using the same key every time, rather than the private member variable created above.
        ////_rsa.FromXmlString(_publicKey);
        //// Get the encrypted hash as a byte array
        //byte[] encryptedHashBytes = Convert.FromBase64String(encryptedHash);
        //// Hash the originalData again
        //SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
        //byte[] hashedData = sha1.ComputeHash(Encoding.UTF8.GetBytes(originalData));
        //// Verify our encrypted hash bytes against the hash of our originalData using the public key
        //bool hashVerified = rsa.VerifyHash(hashedData, CryptoConfig.MapNameToOID("SHA1"), encryptedHashBytes);
        //// return our verification results
        //return hashVerified;
        //}


        public static Boolean VerifyHash(string inputString, 
                                     String orginaldata)
        {
            int dwKeySize = 1024;
            Boolean result = false;
            //StreamReader streamReader = new StreamReader(ConvertUtility.get_setting("publickey"), true);
            //string xmlString = streamReader.ReadToEnd();

            string xmlString = TopupConstant.topupvmgpublickey;
            // TODO: Add Proper Exception Handlers
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            byte[] encryptedHashBytes = Convert.FromBase64String(inputString);
            // Hash the originalData again
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] hashedData = sha1.ComputeHash(Encoding.UTF8.GetBytes(orginaldata));
            // Verify our encrypted hash bytes against the hash of our originalData using the public key
            bool hashVerified = rsaCryptoServiceProvider.VerifyHash(hashedData, CryptoConfig.MapNameToOID("SHA1"), encryptedHashBytes);
            return hashVerified;
        }
    }
}