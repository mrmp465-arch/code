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

namespace Libs.TopupPartner._1Pay
{
    public class _1PayService : IBuyCardHandler
    {

        protected string ServiceUrl = "https://merchant.truemoney.com.vn/api/v1/service/requestTransaction";
        protected string userName = "chinhpoor1990@gmail.com";
        protected string apiCode = "822960";
        protected string apiUsername = "ak3qtp7qyj6r1tvdwexe";
        protected string apiPass = "dqmkrn8wdxatc3pvvvaswzjuhonl7f40";
        protected string privateKey = "<RSAKeyValue><Modulus>yZiawNtkEh8vjr04wmVR4mj2GBDfx73lpNKAZmxAWZtDlUK7X/QVtu6ip1Uju7yTr0RPtSN9ALHhyILw2GDI1BmJLroAacU2A4iEdjoqLIXF2J5ctJk+MZZAX0s58HOVwVJmWDBvQOqxzdpuq6j00Qsl++Oc301KLFQzdHbfw+0=</Modulus><Exponent>AQAB</Exponent><P>+aYqhFxhVi37JOvn9Ta5MQnykyDx7F5hChIgzZmkAAUfR14rw6qdRS8euvprMHebZeHLPvINSH4g+zhXEvlr5w==</P><Q>zrl+maZKh578xueQwkk5bR/xO3DRQ1TQXYC2LvAM4hzY3DbADKAHa+23p6A7Uu11qwPGaKwoRjY/0VK6ZQq3Cw==</Q><DP>JXTqAiv+KrkBanu+tMr+JD5y1+JkietrMeCgbVi1A4/BLqsRc+0gZaX5PWKfPIlllna3UP1uQscx2Z+NaPplVw==</DP><DQ>M324rwTgo49SXyjmwb4Dc871A11Cgobpr6CTwHVCgD+3NILeq0ZhEt0PVc1veWR0Xrh9/yyCi5qPos/8ZZaZqQ==</DQ><InverseQ>6iMkU54LC3oGwow5qFZtZYZPxIfZQprft1W9k2gdFo4u7KM382LcGRVvYcqBGWxSPqypBjO4+p6/kAchC2ykMw==</InverseQ><D>nzYEUfdyatVyS7qpDu3R44udf5gkA623pZoRidJIZ/w1PbY8ISW12IlSr/CPjf76a9r5UEc8u8XAWQkAiQK211L+BeUSWl5eGaugJDaDlcOS6Z8J2erOulAjAJmUnD+VP/fXsU2k3DSlr2NVcNMfWjCbmsrS7t/zkZALrlM0xUk=</D></RSAKeyValue>";

        static JavaScriptSerializer serializer = new JavaScriptSerializer();

        public APIResponse downloadSoftpin(string requestId, string provider, int amount, int quantity, string partnerCode, string providerCode, ref string providerResponse)
        {

            switch (provider.ToUpper())
            {
                case "VTT":
                    provider = "1PAY0050";
                    break;
                case "VMS":
                    provider = "1PAY0052";
                    break;
                case "VNP":
                    provider = "1PAY0051";
                    break;
                case "ZING":
                    provider = "1PAY0056";
                    break;
                case "GATE":
                    provider = "1PAY0057";
                    break;
                case "VCOIN":
                    provider = "1PAY0058";
                    break;
                case "GARENA":
                    provider = "1PAY0059";
                    break;

            }

            RequestData requestData = new RequestData();
            requestData.username = userName;
            requestData.apiCode = apiCode;
            requestData.apiUsername = apiUsername;
            requestData.price = amount;
            requestData.quantity = quantity;
            requestData.requestId = requestId;
            requestData.serviceCode = provider;
            var data = String.Format("{0}|{1}|{2}|{3}|{4}", requestData.username, requestData.apiCode, requestData.apiUsername, requestData.serviceCode, requestData.requestId);
            requestData.dataSign = CreateSignRSA(data, privateKey);
            string responseData = HttpPost(ServiceUrl, requestData);
            NLogLogger.Info(new string[] { "BuyCard", "Response", responseData });
            providerResponse = responseData;
            var response = serializer.Deserialize<ResultData>(responseData);
            return ConvertResultCode(response);
        }

        public APIResponse ConvertResultCode(ResultData result)
        {

            switch (result.status.code)
            {

                case "0":
                    //Thành công
                    var listCard = DecryptString(apiPass, result.encryptCards);
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        
                        ResponseContent = listCard
                    };

                case "1":
                    //Giao dịch thất bại
                    return new APIResponse((int)ResponseCode.TransactionFailed)
                    {
                        ResponseContent = string.Empty
                    };

                case "2":
                case "901":
                case "903":
                case "4001":
                case "4311":
                    //Thông tin xác thực không chính xác
                    return new APIResponse((int)ResponseCode.AccountNotExists)
                    {
                        ResponseContent = string.Empty
                    };

                case "3":
                case "4005":
                    //Thao tác không được cấp phép
                    return new APIResponse((int)ResponseCode.AccessDenied)
                    {
                        ResponseContent = string.Empty
                    };

                case "800":
                case "4100":
                case "4101":
                case "4102":
                case "4103":
                    //Số dư không đủ or error
                    return new APIResponse((int)ResponseCode.BalanceNotEnough)
                    {
                        ResponseContent = string.Empty
                    };

                case "1000":
                    //Không có kết nối nhà cung cấp
                    return new APIResponse((int)ResponseCode.PaymentConnectionFailed)
                    {
                        ResponseContent = string.Empty
                    };

                case "1001":
                case "1002":
                    //Nhà mạng topup ngừng hoạt động hoặc đang bảo trì
                    return new APIResponse((int)ResponseCode.SystemMaintain)
                    {
                        ResponseContent = string.Empty
                    };

                case "1004":
                    //Nhà mạng topup ngừng hoạt động hoặc đang bảo trì
                    return new APIResponse((int)ResponseCode.TransactionInvalid)
                    {
                        ResponseContent = string.Empty
                    };

                case "2000":
                case "2001":
                case "2002":
                case "2003":
                case "4003":
                case "4300":
                case "4302":
                case "4303":
                    //Tham số đầu vào không đúng
                    return new APIResponse((int)ResponseCode.ParameterInvalid)
                    {
                        ResponseContent = string.Empty
                    };

                case "2005":
                    //Giao dịch bị trùng lặp
                    return new APIResponse((int)ResponseCode.TransactionDuplicate)
                    {
                        ResponseContent = string.Empty
                    };

                case "2702":
                    //Trùng requestId
                    return new APIResponse((int)ResponseCode.TransactionInvalid)
                    {
                        ResponseContent = string.Empty
                    };

                case "4002":
                    //Trùng requestId
                    return new APIResponse((int)ResponseCode.AccountLocked)
                    {
                        ResponseContent = string.Empty
                    };
                
                case "4004":
                    //Địa chỉ IP không hợp lệ
                    return new APIResponse((int)ResponseCode.IpInvalid)
                    {
                        ResponseContent = string.Empty
                    };

                case "4104":
                case "4201":
                    //Không hỗ trợ tra cứu số dư tài khoản
                    return new APIResponse((int)ResponseCode.ServiceNotExists)
                    {
                        ResponseContent = string.Empty
                    };

                case "4200":
                case "4202":
                    //Lỗi dịch vụ 
                    return new APIResponse((int)ResponseCode.SystemError)
                    {
                        ResponseContent = string.Empty
                    };
                
                case "4301":
                    //Ký dữ liệu không đúng ( Dữ liệu bị sửa đổi hoặc bị mất)
                    return new APIResponse((int)ResponseCode.SignatureInvalid)
                    {
                        ResponseContent = string.Empty
                    };

                case "4304":
                    //Loại thẻ nạp không hợp lệ
                    return new APIResponse((int)ResponseCode.CardTypeInvalid)
                    {
                        ResponseContent = string.Empty
                    };

                case "4308":
                case "4309":
                    //Số lượng yêu cầu không hợp lệ (-1, 1.5 thẻ, hoặc vượt cấu hình hệ thống)
                    return new APIResponse((int)ResponseCode.CardQuantityLimit)
                    {
                        ResponseContent = string.Empty
                    };

                case "4400":
                case "4401":
                case "4402":
                case "4403":
                    //Vi phạm cấu hình hệ thống, Quá hạn mức giao dịch cho phép trong ngày
                    return new APIResponse((int)ResponseCode.TransactionLimit)
                    {
                        ResponseContent = string.Empty
                    };

                case "4501":
                    //Giao dịch nghi vấn (timeout)
                    return new APIResponse((int)ResponseCode.TransactionSuspicious)
                    {
                        ResponseContent = string.Empty
                    };

                case "4999":
                    //Không có phản hồi từ nhà mạng
                    return new APIResponse((int)ResponseCode.TransactionTimeout)
                    {
                        ResponseContent = string.Empty
                    };

                case "9999":
                    //Không có phản hồi từ nhà mạng
                    return new APIResponse((int)ResponseCode.UndefinedError)
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


        public class RequestData
        {
            public string username { get; set; }
            public string apiCode { get; set; }
            public string apiUsername { get; set; }
            public string requestId { get; set; }
            public string serviceCode { get; set; }
            public int price { get; set; }
            public int quantity { get; set; }
            public string dataSign { get; set; }
        }

        public class ResultData
        {
            public StatusBuyCard status { get; set; }
            public string transactionId { get; set; }
            public string encryptCards { get; set; }
            public string dataSign { get; set; }

        }

        public class StatusBuyCard
        {
            public string code { get; set; }
            public string value { get; set; }
        }

        public static string HttpPost(string url, RequestData requestData)
        {
            string urlParameter = serializer.Serialize(requestData);
            NLogLogger.Info(new string[] { "BuyCard", "Request", urlParameter });
            string result = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                //request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "POST";
                request.ContentType = "application/json; charset=UTF-8";
                request.UserAgent = "Mozilla/5.0";
                request.Accept = "application/json";
                request.Timeout = 30000;
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                var data = Encoding.ASCII.GetBytes(urlParameter);
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                // send url param
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                return result;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout)
                {
                    //Handle timeout exception
                    return "{\"unstructuredData\":[],\"status\":{\"value\":\"Ngắt kết nối đến nhà cung cấp. Timeout exception\",\"code\":-5002},\"conversationId\":null,\"cards\":null,\"encryptCards\":null,\"txnId\":null,\"balance\":0.0,\"dataSign\":null,\"transaction\":null}";
                }
                else
                {
                    throw;
                }
            }


        }

        public string CreateSignRSA(string data, string privateKey)
        {
            RSACryptoServiceProvider rsaCryptoIPT = new RSACryptoServiceProvider(1024);
            rsaCryptoIPT.FromXmlString(privateKey);
            return Convert.ToBase64String(rsaCryptoIPT.SignData(new ASCIIEncoding().GetBytes(data), new SHA1CryptoServiceProvider()));
        }

        public bool CheckSignRSA(string data, string sign, string publicKey)
        {
            try
            {
                RSACryptoServiceProvider rsacp = new RSACryptoServiceProvider();
                rsacp.FromXmlString(publicKey);
                bool status = rsacp.VerifyData(Encoding.UTF8.GetBytes(data), "SHA1", Convert.FromBase64String(sign));
                return status;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string DecryptString(string key, string toDecrypt)
        {
            byte[] keydata = Encoding.ASCII.GetBytes(key);
            string md5String = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-", "").Replace(" ", "+").ToLower();
            byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24));
            TripleDES tripdes = TripleDESCryptoServiceProvider.Create();
            tripdes.Mode = CipherMode.ECB;
            tripdes.Key = tripleDesKey;
            byte[] cryptByte = Convert.FromBase64String(toDecrypt);
            MemoryStream ms = new MemoryStream(cryptByte, 0, cryptByte.Length);
            ICryptoTransform cryptoTransform = tripdes.CreateDecryptor();
            CryptoStream decStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Read);
            StreamReader read = new StreamReader(decStream);
            return (read.ReadToEnd());
        }

        public int checkStore(string provider, int amount)
        {
            throw new NotImplementedException();
        }

    }

}


