using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.Utils;

namespace Libs.API
{
    public class PaymentUtils
    {
        public PaymentUtils()
        {

        }

        public static bool CheckSignature(string data, string signature, string key, int signatureType)
        {
            switch (signatureType)
            {
                case (int)SignatureType.MD5:
                    //NLogLogger.Info(new string[] { "ClientSign", signature, "ServerSign", Encrypts.MD5(data + key) });
                    return Encrypts.MD5(data + key) == signature;

                case (int)SignatureType.RSA:
                    return Encrypts.RSACheck(data, signature, key);

                case (int)SignatureType.SHA256:
                    return Encrypts.SHA256(data + key) == signature;
                default:
                    return false;
            }
        }

        public static string Signature(string data, string key, int signatureType)
        {
            switch (signatureType)
            {
                case (int)SignatureType.MD5:
                    return Encrypts.MD5(data + key);

                case (int)SignatureType.RSA:
                    return Encrypts.RSAGet(data, key);

                case (int)SignatureType.SHA256:
                    return Encrypts.SHA256(data + key);

                default:
                    return "";
            }
        }

        public static string Response(int responseCode)
        {
            return "";
        }

    }
}
