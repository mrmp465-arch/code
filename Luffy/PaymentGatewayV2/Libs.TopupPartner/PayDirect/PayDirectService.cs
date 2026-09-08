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

namespace Libs.TopupPartner.PayDirect
{
    public class PayDirectService : IBuyCardHandler
    {

        //protected string ServiceUrl = "https://sandbox.paydirect.vn/paygate-core/rest/buyCard";
        //protected string PartnerCode = "homedirect";
        //protected string Password = "12345678";
        //protected string SecretKey = "12345678";

        protected string ServiceUrl = "https://khohang.paydirect.vn/paygate-core/rest/buyCard";
        protected string PartnerCode = "XBOM";
        protected string Password = "mt#@!qwe";
        protected string SecretKey = "123!@qasd";

        JavaScriptSerializer serializer = new JavaScriptSerializer();

        public APIResponse downloadSoftpin(string requestId, string provider, int amount, int quantity, ref string providerResponse)
        {

            switch (provider.ToUpper())
            {
                case "VTT":
                    provider = "VTT";
                    break;
                case "VMS":
                    provider = "VMS";
                    break;
                case "VNP":
                    provider = "VNP";
                    break;
                case "VNM":
                    provider = "VNM";
                    break;
                case "GMB":
                    provider = "GMB";
                    break;
                case "SFO":
                    provider = "SFO";
                    break;
                case "ZING":
                    provider = "ZING";
                    break;
                case "GATE":
                    provider = "GATE";
                    break;
                case "VCOIN":
                    provider = "VCOIN";
                    break;

            }

            RequestData requestData = new RequestData();
            requestData.serviceCode = provider;
            requestData.price = amount;
            requestData.quantity = quantity;
            requestData.orgTransId = requestId;
            requestData.partnerCode = PartnerCode;
            requestData.password = Password;
            string data = requestData.serviceCode + requestData.orgTransId + requestData.partnerCode + requestData.password + SecretKey;
            requestData.signature = CreateMD5(data);
            string responseData = PostData(ServiceUrl, serializer.Serialize(requestData));
            providerResponse = responseData;
            var response = serializer.Deserialize<ResultData>(responseData);
            return ConvertResultCode(response);
        }

        public APIResponse ConvertResultCode(ResultData result)
        {

            switch (result.resultCode)
            {
                case "1":

                    List<CardDVO> listCardDVO = new List<CardDVO>();
                    try
                    {
                        TripleDESImplementation tripleDes = new TripleDESImplementation(result.trippdesKey, "PAY-GATE");
                        //<CardCode1>|<CardSerial1>|<ExpriceDate1>,<CardCode1>|<CardSerial1>|<ExpriceDate1>
                        var _listCardResult = tripleDes.Decrypt(result.listCard);
                        var _listCard = _listCardResult.Split(',');
                       
                        foreach (var _lc in _listCard)
                        {
                            var _c = _lc.Split('|');
                            var cardDVO = new CardDVO()
                            {
                                Serial = _c[1],
                                Pin = _c[0],
                                ExpireDate = DateTime.ParseExact(_c[2], "HH:mm:ss dd/MM/yyyy", CultureInfo.InvariantCulture)
                            };
                            listCardDVO.Add(cardDVO);
                        }
                    }
                    catch (Exception exp)
                    {
                        return new APIResponse((int)ResponseCode.SystemError)
                        {
                            ResponseContent = "Parser error listCard"
                        };
                    }
                    
                    return new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {
                        ResponseContent = serializer.Serialize(listCardDVO)
                    };
                    
                case "100":
                    return new APIResponse((int)ResponseCode.CardProviderInvalid)
                    {
                        ResponseContent = string.Empty
                    };

                case "010":
                    return new APIResponse((int)ResponseCode.CardQuantityLimit)
                    {
                        ResponseContent = string.Empty
                    };

                case "011":
                    return new APIResponse((int)ResponseCode.CardOutOfStock)
                    {
                        ResponseContent = string.Empty
                    };

                case "012": case "101":
                    return new APIResponse((int)ResponseCode.CardAmountInvalid)
                    {
                        ResponseContent = string.Empty
                    };

                default:
                    return new APIResponse((int)ResponseCode.TransactionFailed)
                    {
                        ResponseContent = string.Empty
                    };


                    //case "000":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "000",
                    //        message = "Tài khoản đang được nạp tiền",
                    //        listCards = result.listCards
                    //    };
                    //case "001":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "001",
                    //        message = "Giao dịch đang chờ xử lý",
                    //        listCards = result.listCards
                    //    };
                    //case "013":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "013",
                    //        message = "Giao dịch chưa xác định",
                    //        listCards = result.listCards
                    //    };
                    //case "114":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "114",
                    //        message = "Lỗi chưa xác định",
                    //        listCards = result.listCards
                    //    };
                    //case "115":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "115",
                    //        message = "Lỗi timeout",
                    //        listCards = result.listCards
                    //    };
                    //case "014":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "014",
                    //        message = "Lỗi hệ thống NCC",
                    //        listCards = result.listCards
                    //    };
                    //case "002":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "002",
                    //        message = "Lỗi kết nối với NCC",
                    //        listCards = result.listCards
                    //    };
                    //case "003":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "003",
                    //        message = "Lỗi thông tin giao dịch",
                    //        listCards = result.listCards
                    //    };
                    //case "004":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "004",
                    //        message = "Hệ thống NCC đang bận",
                    //        listCards = result.listCards
                    //    };
                    //case "005":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "005",
                    //        message = "Loại hình thanh toán không hỗ trợ",
                    //        listCards = result.listCards
                    //    };
                    //case "006":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "006",
                    //        message = "Tài khoản nạp tiền không tồn tại hoặc không hợp lệ",
                    //        listCards = result.listCards
                    //    };
                    //case "007":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "007",
                    //        message = "Dịch vụ này không tồn tại hoặc đang tạm dừng",
                    //        listCards = result.listCards
                    //    };
                    //case "008":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "008",
                    //        message = "Giao dịch thất bại",
                    //        listCards = result.listCards
                    //    };
                    //case "009":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "009",
                    //        message = "NCC đang bảo trì",
                    //        listCards = result.listCards
                    //    };
                    //case "010":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "010",
                    //        message = "Số lượng thẻ mua vượt giới hạn cho phép",
                    //        listCards = result.listCards
                    //    };
                    //case "011":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "011",
                    //        message = "Loại thẻ này trong kho hiện đã hết hoặc tạm ngừng xuất",
                    //        listCards = result.listCards
                    //    };
                    //case "012":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "012",
                    //        message = "Mệnh giá không hợp lệ hoặc đang tạm dừng",
                    //        listCards = result.listCards
                    //    };
                    //case "027":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "027",
                    //        message = "Mã giao dịch không hợp lệ",
                    //        listCards = result.listCards
                    //    };
                    //case "028":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "028",
                    //        message = "Sai mật khẩu",
                    //        listCards = result.listCards
                    //    };
                    //case "100":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "100",
                    //        message = "Mã thẻ không đúng",
                    //        listCards = result.listCards
                    //    };
                    //case "101":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "101",
                    //        message = "Mệnh giá thẻ không hợp lệ hoặc đang tạm khóa",
                    //        listCards = result.listCards
                    //    };
                    //case "102":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "102",
                    //        message = "Số lượng thẻ không hợp lệ",
                    //        listCards = result.listCards
                    //    };
                    //case "103":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "103",
                    //        message = "Mã giao dịch đã tồn tại",
                    //        listCards = result.listCards
                    //    };
                    //case "104":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "104",
                    //        message = "Mã giao dịch không tồn tại",
                    //        listCards = result.listCards
                    //    };
                    //case "105":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "105",
                    //        message = "Partner không tồn tại hoặc đang bị khóa",
                    //        listCards = result.listCards
                    //    };
                    //case "106":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "106",
                    //        message = "Địa chỉ IP không đúng",
                    //        listCards = result.listCards
                    //    };
                    //case "107":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "107",
                    //        message = "Partner chưa được set IP.",
                    //        listCards = result.listCards
                    //    };
                    //case "108":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "108",
                    //        message = "Tài khoản chưa có key mã hóa softpin.",
                    //        listCards = result.listCards
                    //    };
                    //case "109":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "109",
                    //        message = "Sai chữ ký.",
                    //        listCards = result.listCards
                    //    };
                    //case "110":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "110",
                    //        message = "Mật khẩu không đúng",
                    //        listCards = result.listCards
                    //    };
                    //case "111":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "111",
                    //        message = "Hệ thống đang bảo trì",
                    //        listCards = result.listCards
                    //    };
                    //case "112":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "112",
                    //        message = "Giá trị giao dịch vượt quá hạn mức cho phép",
                    //        listCards = result.listCards
                    //    };
                    //case "113":
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = "113",
                    //        message = "Số dư tài khoản không đủ",
                    //        listCards = result.listCards
                    //    };
                    //default:
                    //    return new BuyCardService.BuyCardResponse()
                    //    {
                    //        errorCode = result.errorCode,
                    //        message = result.errorCode,
                    //        listCards = result.listCards
                    //    };
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
        public class CardDVO
        {
            public string Serial { get; set; }
            public string Pin { get; set; }
            public DateTime ExpireDate { get; set; }
        }

        public class RequestData
        {
            public string serviceCode { get; set; }
            public int price { get; set; }
            public int quantity { get; set; }
            public string orgTransId { get; set; }
            public string partnerCode { get; set; }
            public string password { get; set; }
            public string signature { get; set; }
        }

        public class ResultData
        {
            public string resultCode { get; set; }
            public string orgTransId { get; set; }
            public int? amount { get; set; }
            public string listCard { get; set; }
            public int? partnerBalance { get; set; }
            public string trippdesKey { get; set; }
        }

        //public class BuyCardResponse
        //{
        //    public string errorCode { get; set; }
        //    public string message { get; set; }
        //    public string listCards { get; set; }
        //}

        private string PostData(string uri, string postData)
        {
            var request = (HttpWebRequest)WebRequest.Create(ServiceUrl);
            request.ContentType = "application/json";
            request.Method = "POST";//GET
            //request.Accept = "JSON";
            using (Stream requestStream = request.GetRequestStream())
            {
                byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                requestStream.Write(postDatabytes, 0, postDatabytes.Length);
            }
            var webResponse = request.GetResponse();
            if (webResponse == null)
            {
                return "Unable to connect to the remote server";
            }
            var sr = new StreamReader(webResponse.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        public string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public int checkStore(string provider, int amount)
        {
            throw new NotImplementedException();
        }
    }

    public class TripleDESImplementation
    {
        //Encryption Key
        private byte[] EncryptionKey { get; set; }
        // The Initialization Vector for the DES encryption routine
        private byte[] IV { get; set; }

        /// Constructor for TripleDESImplementation class
        /// </summary>
        /// <param name="encryptionKey">The 24-byte encryption key (24 character ASCII)</param>
        /// <param name="IV">The 8-byte DES encryption initialization vector (8 characters ASCII)</param>
        public TripleDESImplementation(string encryptionKey, string IV)
        {
            if (string.IsNullOrEmpty(encryptionKey))
            {
                throw new ArgumentNullException("'encryptionKey' parameter cannot be null.", "encryptionKey");
            }
            if (string.IsNullOrEmpty(IV))
            {
                throw new ArgumentException("'IV' parameter cannot be null or empty.", "IV");
            }
            EncryptionKey = Encoding.ASCII.GetBytes(encryptionKey);
            // Ensures length of 24 for encryption key
            Trace.Assert(EncryptionKey.Length == 24, "Encryption key must be exactly 24 characters of ASCII text (24 bytes)");
            this.IV = Encoding.ASCII.GetBytes(IV);
            // Ensures length of 8 for init. vector
            Trace.Assert(IV.Length == 8, "Init. vector must be exactly 8 characters of ASCII text (8 bytes)");
        }

        /// Encrypts a text block
        public string Encrypt(string textToEncrypt)
        {
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = EncryptionKey;
            tdes.IV = IV;
            byte[] buffer = Encoding.ASCII.GetBytes(textToEncrypt);
            return Convert.ToBase64String(tdes.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));
        }

        /// Decrypts an encrypted text block

        public string Decrypt(string textToDecrypt)
        {
            byte[] buffer = Convert.FromBase64String(textToDecrypt);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = EncryptionKey;
            des.IV = IV;
            return Encoding.ASCII.GetString(des.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
        }
    }
}
