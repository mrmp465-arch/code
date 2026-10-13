using Libs.API;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using static BankGateV2.bankout.cash;
using System.Web.Script.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Web.Routing;
using Libs.Report;

namespace BankGateV2.bankin
{
    /// <summary>
    /// Summary description for getdetailva
    /// </summary>
    public class getdetailva : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Request.ContentType = "application/json";
            context.Response.ContentType = "application/json";
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            var jsonString = String.Empty;
            var result = string.Empty;
            context.Request.InputStream.Position = 0;
            using (var inputStream = new StreamReader(context.Request.InputStream))
            {
                jsonString = inputStream.ReadToEnd();
            }
            try
            {

                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                var request = javaScriptSerializer.Deserialize<RequestDetail>(jsonString);

                NLogLogger.Info(new string[] { "VPGJsonService", "Request", jsonString });

                Partners _Partner = new Partners().GetCache(request.PartnerCode);
                //Kiểm tra _Partner tồn tại hoặc Active không
                if (_Partner == null || _Partner.Status == 0)
                {

                    context.Response.Write(ResponseUtils.Response((int)ResponseCode.PartnerNotExistsNotActive));
                    return;
                }
                // Kiểm tra chữ ký
                if (!PaymentUtils.CheckSignature(_Partner.PartnerCode + request.BankTransId, request.Signature, _Partner.PublicKey, _Partner.SignatureType))
                {

                    context.Response.Write(ResponseUtils.Response((int)ResponseCode.SignatureInvalid));
                    return;
                }
                var checkorder = DataCaching.GetCache<DataCheckOrder>("CheckOrderV3:" + request.PartnerCode + request.BankTransId);
                if (checkorder != null)
                {
                    //NLogLogger.Info("check order by cache");
                    //return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    //{
                    //    ResponseContent = javaScriptSerializer.Serialize(checkorder)
                    //};
                    var data = new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        ResponseContent = javaScriptSerializer.Serialize(checkorder)
                    };
                    context.Response.Write(javaScriptSerializer.Serialize(data));
                    return;
                }

                var trans = new BankGateAPI().GetByOrderInfo(request.BankTransId);

                if (trans == null)
                {
                    //return new APIResponse((int)ResponseCode.TransactionNotExists);
                    context.Response.Write(ResponseUtils.Response((int)ResponseCode.TransactionNotExists));
                    return;
                }
                if (trans.Status >= 1)
                {
                    var checkOrder = new DataCheckOrder
                    {
                        Content = trans.Signature,
                        Amount = Convert.ToInt32(trans.Amount),
                        AccountInfo = trans.Mobile,
                        BankTransId = trans.OrderInfo,
                        TimeBankSuccess = trans.CreatedTime
                    };
                    var data = new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        ResponseContent = javaScriptSerializer.Serialize(checkOrder)
                    };
                    context.Response.Write(javaScriptSerializer.Serialize(data));
                    return;
                }
                else
                {

                    context.Response.Write(ResponseUtils.Response((int)ResponseCode.CardProcessing));
                    return;
                }
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "VPGJsonService", "BankoutExeption", exp.Message, jsonString });
                context.Response.Write(ResponseUtils.Response((int)ResponseCode.ParameterInvalid));
            }
            context.Response.Write(result);
        }
        public class DataCheckOrder
        {

            public string Content { get; set; }
            public int Amount { get; set; }
            public string AccountInfo { get; set; }
            public string BankTransId { get; set; }
            public string Signature { get; set; }
            public DateTime TimeBankSuccess { get; set; }
        }
        public class RequestDetail
        {
            public string PartnerCode { get; set; }
            public string BankTransId { get; set; }

            public string Signature { get; set; }

        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}