using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;

namespace Libs.BankGate
{

    public class BankGateService
    {
        public string Confirm(ReceiveConfirmData receive)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
           
            Partners _Partner = new Partners().Get(receive.PartnerCode);
            string sign = Encrypts.MD5((receive.PartnerCode + receive.ResponseNo + receive.ConfirmCode + receive.ResquestTime + _Partner.PrivateKey).ToLower());

            if (sign != receive.Signature)
            {
                var responseData = new ResponseConfirmData()
                {
                    ResposeCode = (int)ResponseCode.SignatureInvalid,

                };
                NLogLogger.Info(new string[] { "BankGate", "BankGateService", "SignatureInvalid", sign, receive.Signature });
                return Response(responseData, _Partner.PrivateKey);
            }

            var bankGateAPI = new BankGateAPI().Get(Convert.ToInt64(receive.ResponseNo));

            if (bankGateAPI != null)
            {
                NLogLogger.Info(new string[] { "BankGate", "BankGateService", "Confirm Request", serializer.Serialize(receive) });
                var handler = BankGateFactory.GetHandler(bankGateAPI.ProviderCode);
                var response = handler.Confirm(receive);
                if (response.ResposeCode == (int)ResponseCode.TransactionSuccessful)
                {
                    bankGateAPI.Status = receive.ConfirmCode == 1 ? (int)ResponseCode.TransactionSuccessful  : (int)ResponseCode.TransactionRefun;
                    bankGateAPI.LogContent = bankGateAPI.LogContent + " | ConfirmCode: " + receive.ConfirmCode + " Confirm Response: " + response.ResposeCode;
                    bankGateAPI.UpdateStatus();
                }

                return Response(response, _Partner.PrivateKey);
            }

            return Response(new ResponseConfirmData() { ResposeCode = (int)ResponseCode.TransactionNotExists });


        }

        public static string Response(ResponseConfirmData responseData, string privateKey = "")
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            responseData.ResponseTime = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            responseData.Description = ResponseUtils.Description(responseData.ResposeCode);
            responseData.Signature = Encrypts.MD5((responseData.ResposeCode + responseData.ResponseNo + responseData.OrderNo + responseData.ResponseTime + responseData.Description).ToLower() + privateKey);
            return serializer.Serialize(responseData);
        }

    }

    public class ReceiveConfirmData
    {
        public string PartnerCode { get; set; }
        public string ResponseNo { get; set; }
        public int ConfirmCode { get; set; }
        public string ResquestTime { get; set; }
        public string Signature { get; set; }

    }

    public class ResponseConfirmData
    {
        [DefaultValue("0")]
        public int ResposeCode { get; set; }
        [DefaultValue("0")]
        public long ResponseNo { get; set; }
        [DefaultValue("")]
        public string OrderNo { get; set; }
        [DefaultValue("")]
        public string ResponseTime { get; set; }
        [DefaultValue("")]
        public string Description { get; set; }
        [DefaultValue("")]
        public string Signature { get; set; }

    }


}
