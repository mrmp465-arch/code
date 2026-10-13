using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Libs.API;

namespace BankGateV2
{
    public class BankGateUtils
    {
        public BankGateUtils()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        // Url trả về cho đối tác
        public static string UrlResponse(string urlReturn, string orderNo, string status, string responseNo, int signatureType, string privateKey)
        {
            urlReturn += urlReturn.IndexOf("?") > 0 ? "&" : "?";

            string responseTime = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string data = orderNo + status + responseNo + responseTime;
            string signature = PaymentUtils.Signature(data, privateKey, signatureType);
            string url = urlReturn + string.Format("orderno={0}&status={1}&responseno={2}&responsetime={3}&signature={4}", orderNo, status, responseNo, responseTime, signature);

            return url;
        }


        // Url trả về cho đối tác
        public static string UrlResponse(string urlReturn, string orderNo, string status, string responseNo, string bankCode, string totalAmount, string fullName, string mobile, int signatureType, string privateKey, int confirmRequired)
        {
            urlReturn += urlReturn.IndexOf("?") > 0 ? "&" : "?";

            string responseTime = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string data = orderNo + status + responseNo + responseTime;
            string signature = PaymentUtils.Signature(data, privateKey, signatureType);
            string url = urlReturn + string.Format("orderno={0}&responsecode={1}&responseno={2}&responsetime={3}&bankcode={4}&totalamount={5}&fullname={6}&mobile={7}&signature={8}&confirmrequired={9}", orderNo, status, responseNo, responseTime, bankCode, totalAmount, fullName, mobile, signature, confirmRequired);

            return url;
        }
    }
}