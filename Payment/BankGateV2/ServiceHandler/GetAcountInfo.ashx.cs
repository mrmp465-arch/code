using BankGateV2.bankin;
using Libs.API;
using Libs.BankCash.Drum;
using Libs.BankDirect.MDrum;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using static BankGateV2.ServiceHandler.GetQRInfo;
using static Libs.BankCash.BankCashService;

namespace BankGateV2.ServiceHandler
{
    /// <summary>
    /// Summary description for GetAcountInfo
    /// </summary>
    public class GetAcountInfo : IHttpHandler
    {
        private const string urlBaseServiceBank = "http://127.0.0.1:9002/BankService.ashx";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public void ProcessRequest(HttpContext context)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
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
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            var request = javaScriptSerializer.Deserialize<RequestAccount>(jsonString);

            var accountname = GetBankAccount(request.BankCode, request.BankId);
            context.Response.Write(accountname);

        }
        public class RequestAccount
        {
            public string BankCode { get; set; }
            public string BankId { get; set; }

        }
        public string GetBankAccount(string bankcode, string bankId)
        {

            var key = string.Format("{0}:{1}", "BankReceivedAccount", bankcode + bankId);
            var result = DataCaching.GetCache<string>(key);
            //if (result != null)
            //{
            //    return result;
            //}
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
            if(bankcode=="MOMO")
            {
               
                var response = GetMomo(bankId);

                //NLogLogger.Info(new string[] { "MDrum", "GetBanks Request",serializer.Serialize(requestData), urlBaseServiceBank,response
                //        });

                if (!string.IsNullOrEmpty(response))
                {
                    DataCaching.SetCache(key, response, 86400 * 100);
                    return response;
                   
                }
                return "";
            }
            else
            {
                var bankrequest = new RequestAccount
                {
                    BankCode = bankcode,
                    BankId = bankId
                };
                //var Partner = new Partners().Get("order");
                var requestContent = serializer.Serialize(bankrequest);

                //var signature = Encrypts.MD5(Partner.PartnerCode + "GETLIST" + requestContent + Partner.PublicKey);
                var requestData = new RequestData()
                {
                    PartnerCode = "order",
                    CommandCode = "ACCOUNT_QUERY",
                    RequestContent = requestContent,
                    Signature = ""
                };

              
                NLogLogger.Info(new string[] { "MDrum", "GetBanks Request",serializer.Serialize(requestData), urlBaseServiceBank,
                        });
                var response = Task.Run(async () => await MDrumBankLib.PostTask(urlBaseServiceBank, serializer.Serialize(requestData))).Result;

                NLogLogger.Info(new string[] { "MDrum", "GetBanks Request",serializer.Serialize(requestData), urlBaseServiceBank,response
                        });

                if (!string.IsNullOrEmpty(response))
                {
                    var resObj = serializer.Deserialize<MDrumBankLib.BankResponse>(response);
                    if (resObj.ResponseCode > 0)
                    {
                        var account = resObj.ResponseContent;
                        DataCaching.SetCache(key, account, 86400 * 100);
                        return account;


                    }
                    else
                    {
                        System.Threading.Thread.Sleep(10000);
                        response = Task.Run(async () => await MDrumBankLib.PostTask(urlBaseServiceBank, serializer.Serialize(requestData))).Result;
                        resObj = serializer.Deserialize<MDrumBankLib.BankResponse>(response);
                        if (resObj.ResponseCode > 0)
                        {
                            var account = resObj.ResponseContent;
                            DataCaching.SetCache(key, account, 86400 * 100);
                            return account;


                        }
                        else
                        {
                            System.Threading.Thread.Sleep(20000);
                            response = Task.Run(async () => await MDrumBankLib.PostTask(urlBaseServiceBank, serializer.Serialize(requestData))).Result;
                            resObj = serializer.Deserialize<MDrumBankLib.BankResponse>(response);
                            if (resObj.ResponseCode > 0)
                            {
                                var account = resObj.ResponseContent;
                                DataCaching.SetCache(key, account, 86400 * 100);
                                return account;


                            }
                            else
                            {
                                System.Threading.Thread.Sleep(30000);
                                response = Task.Run(async () => await MDrumBankLib.PostTask(urlBaseServiceBank, serializer.Serialize(requestData))).Result;
                                resObj = serializer.Deserialize<MDrumBankLib.BankResponse>(response);
                                if (resObj.ResponseCode > 0)
                                {
                                    var account = resObj.ResponseContent;
                                    DataCaching.SetCache(key, account, 86400 * 100);
                                    return account;


                                }
                            }
                        }
                    }
                    return "";
                }
                return "";
            }
           

        }
        public string GetMomo(string account)
        {


          
            var signature = "";
            var requestData = new RequestData()
            {
                PartnerCode = "",
                CommandCode = "ACCOUNT_INQUIRY",
                RequestContent = account,
                Signature = signature
            };

            var response = Task.Run(async () => await MDrumBankLib.PostTask("http://127.0.0.1:9002/MomoService.ashx", serializer.Serialize(requestData))).Result;


            var resObj = serializer.Deserialize<DrumBankLib.CashRespone>(response);
            if (resObj.ResponseCode == 1)
            {
                var userinfo = serializer.Deserialize<DrumBankLib.UserInfo>(resObj.ResponseContent);
                return userinfo.name;
            }
            else
            {
                return "";
            }
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