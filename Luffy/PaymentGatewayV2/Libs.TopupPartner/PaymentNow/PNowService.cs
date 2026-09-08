using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;

namespace Libs.TopupPartner.PaymentNow
{
    public class PNowService : IBuyCardHandler
    {

        protected string ServiceUrl = "http://api.paymentnow.net:8989/card/award";
        protected int user_id = 8;
        protected int project_id = 11;
        protected string key = "qf8Qcxs5qUk6bkWMhkgbuZSgT37Xn6kwpN2V23vueqYzb8wJ2m";

        JavaScriptSerializer serializer = new JavaScriptSerializer();

        public APIResponse downloadSoftpin(string requestId, string provider, int amount, int quantity, ref string providerResponse)
        {

            switch (provider.ToUpper())
            {
                case "VTT":
                    provider = "VIETTEL";
                    break;
                case "VMS":
                    provider = "MOBIFONE";
                    break;
                case "VNP":
                    provider = "VINAPHONE";
                    break;
                case "ZING":
                    provider = "ZING";
                    break;
                case "GATE":
                    provider = "GATE";
                    break;
                case "VCOIN":
                    provider = "VTC";
                    break;

            }

            RequestData requestData = new RequestData();
            requestData.card_id = provider;
            requestData.project_id = project_id;
            requestData.user_id = user_id;
            requestData.trans_id = requestId;
            requestData.quantity = quantity;
            requestData.amount = amount;
            requestData.time = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            //string data = requestData.user_id + requestData.project_id + requestData.trans_id + requestData.card_id + requestData.amount + requestData.quantity + requestData.time + key;
            var data = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}", requestData.user_id, requestData.project_id, requestData.trans_id, requestData.card_id, requestData.amount, requestData.quantity, requestData.time, key);
            requestData.sign = Encrypts.MD5(data);
            string parameters = "project_id={0}&user_id={1}&trans_id={2}&card_id={3}&quantity={4}&amount={5}&time={6}&sign={7}";
            string responseData = HttpPost(ServiceUrl, string.Format(parameters, requestData.project_id, requestData.user_id, requestData.trans_id, requestData.card_id, requestData.quantity, requestData.amount, requestData.time, requestData.sign));
            //string responseData = "{\"status\":1,\"data\":\"[{\\\"pin_code\\\":\\\"4548734123123\\\",\\\"serial\\\":\\\"586112323342\\\",\\\"date\\\":\\\"31/12/2022\\\"}]\",\"msg\": \"Thành công\"}";
            NLogLogger.Info(new string[] { "BuyCard", "Response", responseData });
            providerResponse = responseData;
            var response = serializer.Deserialize<ResultData>(responseData);
            return ConvertResultCode(response);
        }

        public APIResponse ConvertResultCode(ResultData result)
        {

            switch (result.status)
            {
                
                case 1:
                    //Thành công
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        ResponseContent = result.data
                    };

                case 0:
                case -99:
                    //Lỗi chưa xác định
                    return new APIResponse((int)ResponseCode.UndefinedError)
                    {
                        ResponseContent = string.Empty
                    };

                case -1:
                    //Lỗi hệ thống
                    return new APIResponse((int)ResponseCode.SystemError)
                    {
                        ResponseContent = string.Empty
                    };

                case -55:
                case -310:
                case 170:
                    //Số dư tài khoản không đủ
                    return new APIResponse((int)ResponseCode.BalanceNotEnough)
                    {
                        ResponseContent = string.Empty
                    };

                case -302:
                    //Partner không tồn tại hoặc đang tạm dừng hoạt động
                    return new APIResponse((int)ResponseCode.PartnerNotExistsNotActive)
                    {
                        ResponseContent = string.Empty
                    };

                case -304:
                    //Dịch vụ này không tồn tại hoặc đang tạm dừng
                    return new APIResponse((int)ResponseCode.ServiceNotExists)
                    {
                        ResponseContent = string.Empty
                    };

                case -305:
                    //Chữ ký không hợp lệ
                    return new APIResponse((int)ResponseCode.SignatureInvalid)
                    {
                        ResponseContent = string.Empty
                    };

                case -306:
                    //Mệnh giá không hợp lệ hoặc đang tạm dừng
                    return new APIResponse((int)ResponseCode.CardAmountInvalid)
                    {
                        ResponseContent = string.Empty
                    };

                case -308:
                case -309:
                    //RequesData không hợp lệ
                    return new APIResponse((int)ResponseCode.ParameterInvalid)
                    {
                        ResponseContent = string.Empty
                    };

                case -317:
                    //Số lượng thẻ không hợp lệ
                    return new APIResponse((int)ResponseCode.CardQuantityLimit)
                    {
                        ResponseContent = string.Empty
                    };

                case -320:
                    //Hệ thống gián đoạn
                    return new APIResponse((int)ResponseCode.SystemMaintain)
                    {
                        ResponseContent = string.Empty
                    };

                case -307:
                case -350:
                    //Tài khoản không tồn tại
                    return new APIResponse((int)ResponseCode.AccountNotExists)
                    {
                        ResponseContent = string.Empty
                    };

                case -500:
                    //Loại thẻ này trong kho hiện đã hết hoặc tạm ngừng 
                    return new APIResponse((int)ResponseCode.CardProviderInvalid)
                    {
                        ResponseContent = string.Empty
                    };

                case -501:
                case -509:
                    //Giao dịch không thành công
                    return new APIResponse((int)ResponseCode.TransactionFailed)
                    {
                        ResponseContent = string.Empty
                    };

                case -600:
                    //Quá hạn mức
                    return new APIResponse((int)ResponseCode.CardOutOfStock)
                    {
                        ResponseContent = string.Empty
                    };

                case 110:
                case 120:
                    //User id không đúng
                    //Project id không đúng
                    return new APIResponse((int)ResponseCode.ParameterInvalid)
                    {
                        ResponseContent = string.Empty
                    };

                case 130:
                    //Lỗi trong quá trình xử lí
                    return new APIResponse((int)ResponseCode.SystemError)
                    {
                        ResponseContent = string.Empty
                    };

                case 140:
                    //Chữ kí không đúng
                    return new APIResponse((int)ResponseCode.SignatureInvalid)
                    {
                        ResponseContent = string.Empty
                    };

                case 150:
                    //Card id không đúng
                    return new APIResponse((int)ResponseCode.CardTypeInvalid)
                    {
                        ResponseContent = string.Empty
                    };

                default:
                    return new APIResponse((int)ResponseCode.TransactionFailed)
                    {
                        ResponseContent = string.Empty
                    };


            }
        }

        //public int checkStore(string provider, int amount)
        //{
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    RequestData requestData = new RequestData();
        //    requestData.FunctionName = "checkstore";
        //    requestData.PartnerCode = PartnerCode;
        //    requestData.ProviderCode = provider;
        //    requestData.Amount = amount;
        //    requestData.RequestTime = Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        //    string data = requestData.OrderNo + requestData.PartnerCode + requestData.ProviderCode + requestData.Amount.ToString() + requestData.Quantity + requestData.RequestTime.ToString() + PartnerKey;
        //    requestData.Signature = Encrypts.MD5(data);
        //    string responseData = PostData(ServiceUrl, serializer.Serialize(requestData));
        //    return int.Parse(responseData);
        //}
        //public class CardDVO
        //{
        //    public string Serial { get; set; }
        //    public string Pin { get; set; }
        //    public DateTime ExpireDate { get; set; }
        //}

        //public class Data
        //{
        //    public string pin_code { get; set; }
        //    public string serial { get; set; }
        //    public string date { get; set; }
        //}

        public class RequestData
        {
            public int project_id { get; set; }
            public int user_id { get; set; }
            public string trans_id { get; set; }
            public string card_id { get; set; }
            public int quantity { get; set; }
            public int amount { get; set; }
            public int time { get; set; }
            public string sign { get; set; }
        }

        public class ResultData
        {
            public int status { get; set; }
            public string data { get; set; }
            public string msg { get; set; }

        }

        public static string HttpPost(string url, string parameters)
        {
            NLogLogger.Info(new string[] { "BuyCard", "Request", parameters });
            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                req.Timeout = 30000;
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(parameters);
                req.ContentLength = bytes.Length;
                System.IO.Stream os = req.GetRequestStream();
                os.Write(bytes, 0, bytes.Length); //Push it out there
                os.Close();
                System.Net.WebResponse resp = req.GetResponse();
                if (resp == null) return null;
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                return sr.ReadToEnd().Trim();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout)
                {
                    //Handle timeout exception
                    return "{\"status\":-5002,\"data\":\"\",\"msg\": \"Ngắt kết nối đến nhà cung cấp. Timeout exception\"}"; ;
                }
                else
                {
                    throw;
                }
            }

        }

        public int checkStore(string provider, int amount)
        {
            throw new NotImplementedException();
        }
    }

}


