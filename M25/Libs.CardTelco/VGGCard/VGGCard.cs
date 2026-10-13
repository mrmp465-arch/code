using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;
using System.Security.Cryptography;

namespace Libs.CardTelco.VGGCard
{
    public class VGGCard
    {
        // API
        protected string providerVGG = "vgg";
        protected string ServiceUrl = "http://api.vgg.vn/vggcard/";
        protected string PartnerCode = "VGGBIL";
        protected string PartnerKey = "94392fc7dabdcef8c08795caf78f7f31";
        protected string Passwordtext = "30a20aaf9d34400c37027055ac8dcca7";
        protected string PrivateKey = "<RSAKeyValue><Modulus>yVU7CmsDLQ5KhlhRSAMdLdkVNwYqFJtM7kd0xsR3Z6z6LonCBWosXcWFho3xhspYiojEmvW1HV74ypv6x1zutG9gSISzUfzxuHN3ia3uPI5OyFkB2uyRGmmM5jrGsvIrHuh9Emrc+vzgnLVkUpNfXxr3mukd2L1wxU/qgnRcVY0=</Modulus><Exponent>AQAB</Exponent><P>6GNpB7XVDbdvqud27UZm3ggTc57501aFfeGkW22XG/i7bTG5PGkP3FRFRdVPK2DPbBfuQ/rGW5MNxGolUPXB+w==</P><Q>3coMCioGnB1uUHPPEBNQUzEB3diaMg7JaY3XDEBqk5Pj2xt+x/z6juXRlUFF5znok9YcRSmSNqcH4zXFdTs4Fw==</Q><DP>Fx2kWe79ZWLkybRCgTGqk9Vr0elzYlFN8Aqc5bdN+6tPQXpbuaWdvUXoqhygxmjX2SG8QqjUSFGlYe9JDPeVNw==</DP><DQ>q9sJpT6XTJTJi/GQuuYcUbohtiNGqA8pJm9P+SBY9O2dm4Qcox51kBCoa7zeS6NlglhgI39o2oxm4joiPQqcFQ==</DQ><InverseQ>0KdQDIle9U/VHQRRvaWOJmILxrXeFnUUKJAQUAynxLsia6ZAtwxuV+9mZ3hytQPMyDmSzwIcYe2MxKoqJpPkig==</InverseQ><D>inHreYyA0JjFMoDE0+ogRIjEh+PbR/RLAIdui8hJ+6b9Gsdr/eE2xPywhwhQj8nQ/OJzn8NaWeoW1uxPlIwZMA4dfz20n+cjLi/t77YbS2HpLAUBVyRb/0NoCy9CCZKv/z8d6OcQ3vipM94cGzXTBL4Y44i7vyD4J+5n7Mm90tk=</D></RSAKeyValue>";

        // Sandbox
        //protected string providerVGG = "vgg";
        //protected string ServiceUrl = "http://service.vgg.vn/vggcard/";
        //protected string PartnerCode = "VGGTEST";
        //protected string PartnerKey = "f039f83e3e93c5ef001e403b6e4a1a8d";
        //protected string Passwordtext = "5addca7adbc29e3f5a3e4f599043488f";
        //protected string PrivateKey = "<RSAKeyValue><Modulus>06nxNAb+cnqFxIq1Hg2GEdp3Hm936NrEw9vmLjiLrDW61ZUPv/9Bd89TgyFoynFARlEVhR3iC780WdsuehYZ9UO/cB+jHfXBi7K5nEL5YsByvuQiu90vfoKk9FNAcE0QpKsSeKwwwYdnpasEwdx48NIrS8e+28fRwiGtDA2u2kc=</Modulus><Exponent>AQAB</Exponent><P>6P+b8APvL+NrzgKLvRiOjKlGTPEjGm6h0UnHeB5MjFxOtZB/Mc30X6kJQq6Wm/yqfuBrI8kpSAcqAyrh+dzPew==</P><Q>6I8oxBZLZxG5vioGvxMYByVcWxwN5ewJ8qQw0O3QNmWu9aGXITf5DVxdQ8deP4odcLWUlBq+pfdDB6wB+P1gpQ==</Q><DP>sj2wNhlcZf8NHmjWK49YpYRtMjkaCINvqOJPBLXxC0MmN0FV/Gy4sGFooylaL/RHHY5ih9rAiMhqPEgW3QUtdw==</DP><DQ>QO5HmYgIAX0HeTrOq2avXL9nq+CHWNywYljJcGwVqDp3wQke4x13oe1Z9OSjQeZUFfX8x+U3uarxwiCaZGSKKQ==</DQ><InverseQ>vmHT4ZM2KlWjBu/v9Z9+UOs5S+33EVLpC3YPSFoP/RMsYqZuyv7CBEbCQItmEPqqGS1qTCyNAe5M+RM5FLeyVw==</InverseQ><D>C5cNQSCptpHYJbUrfVP9cmU4pPvMYH2SM3m+drhCVasZHVbgABub2+tukqPPfb5XAakXX5NAuUBYDBxakcIIdVjCxEnLVH/wlOtcr7T2HdOBjPykeInmqwxZL4XmuM6+Lc2ddzH6WWQa2x0T8+Yuws33CBPNYarpmonGLxE0Prk=</D></RSAKeyValue>";

        public VGGCard()
        {
        
        }

        public APIResponse UseCard(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            APIResponse _APIResponse = new APIResponse();
            CardAPILog _CardAPILog = new CardAPILog();

            int step = 0;
            try
            {
                // Bước: Phân tích yêu cầu thành đối tượng
                step = 1;
                UseCardRequest request = new UseCardRequest();
                request = serializer.Deserialize<UseCardRequest>(transaction.RequestContent);

                // Bước: Ghi log giao dịch
                step = 2;
                _CardAPILog.TransactionID = transaction.TransactionID;
                _CardAPILog.PartnerID = transaction.PartnerID;
                _CardAPILog.AccountName = request.AccountName;
                _CardAPILog.AccountID = 0;
                _CardAPILog.CardSerial = request.CardSerial;
                _CardAPILog.CardCode = request.CardCode;
                _CardAPILog.CardType = request.CardType;
                _CardAPILog.Amount = 0;
                _CardAPILog.Provider = providerVGG;
                _CardAPILog.Status = 0;
                _CardAPILog.Description = "_CardAPILog.Add";
                _CardAPILog.Add();

                // Nếu thêm giao dịch không hợp lệ
                if (_CardAPILog.ReturnValue < 0)
                {
                    return new APIResponse(_CardAPILog.ReturnValue); ;
                }

                // Bước: gọi hàm sang API
                step = 3;

                CardGGRequest cardRequest = new CardGGRequest();
                cardRequest.CardSerial = request.CardSerial;
                cardRequest.CardNumber = request.CardCode;
                cardRequest.AccountName = request.AccountName;
                cardRequest.FunctionName = "deactivecard";
                cardRequest.PartnerLogId = transaction.TransactionID;

                NLogLogger.Info(new string[] { "VGGCard", transaction.TransactionID.ToString(), "VGGCardRequest", serializer.Serialize(cardRequest) });
                CardGGResult cardResult = ProcessCard(cardRequest);
                NLogLogger.Info(new string[] { "VGGCard", transaction.TransactionID.ToString(), "VGGCardResponse", serializer.Serialize(cardResult) });

                step = 4;
                // Nếu thành công
                if (cardResult.ReturnCode == 200)
                {
                    cardResult.ReturnData = VggDecryptString(cardResult.ReturnData, Passwordtext);
                    CardData cardData = serializer.Deserialize<CardData>(cardResult.ReturnData);

                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    _CardAPILog.Amount = cardData.Value;
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = "Amount: " + _CardAPILog.Amount.ToString();
                    _CardAPILog.Status = 1;
                }
                else
                {
                    switch (cardResult.ReturnCode)
                    {
                        case -202:
                            //-202		Thẻ đã hết hạn
                            _APIResponse = new APIResponse((int)ResponseCode.CardHasExpired);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -201:
                            //-201		Thẻ không tồn tại hoặc đã sử dụng
                            _APIResponse = new APIResponse((int)ResponseCode.CardSerialInvalid);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                        case -100:
                            //-100		Dữ liệu không hợp lệ
                        case -101:
                            //-101		PublicKey không tồn tại
                        case -102:
                            //-102		Không có quyền thực hiện, vui lòng lên hệ: support@vgg.vn
                        case -103:
                            //-103		Không giải mã được thông tin thẻ, vui lòng lên hệ: support@vgg.vn
                        case -200:
                            //-200		Partner không hợp lệ
                        case -203:
                            //-203		Thông tin không hợp lệ
                        case -204:
                            //-204		Lỗi không xác định 
                        case -999:
                            //-999		Lỗi hệ thống
                        default:
                            _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                            _CardAPILog.Status = _APIResponse.ResponseCode;
                            break;
                    }
                    _CardAPILog.Description = serializer.Serialize(cardResult);
                }
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "VGGCard", transaction.TransactionID.ToString(), "Error", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "VGGCard", transaction.TransactionID.ToString(), "Error", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "VGGCard", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - UseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "VGGCard", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "VGGCard", transaction.TransactionID.ToString(), "Error", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }
            }

            if (_CardAPILog.TransactionID > 0)
            {
                _CardAPILog.Update();
            }
            return _APIResponse;
        }

        private CardGGResult ProcessCard(CardGGRequest cardRequest)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            cardRequest.PasswordText = Passwordtext;
            cardRequest.PartnerCode = PartnerCode;
            cardRequest.PartnerKey = PartnerKey;
            cardRequest.IPAddress = Libs.Utils.IPAddress.Get();

            string plantext = cardRequest.PartnerCode
                + "-" + cardRequest.PartnerKey
                + "-" + cardRequest.CardNumber
                + "-" + cardRequest.CardSerial
                + "-" + cardRequest.AccountName;
            cardRequest.Sign = Encrypts.RSAGet(plantext.ToLower(), PrivateKey);

            NLogLogger.Info(new string[] { "VGGCard", "VGGCardRequest", serializer.Serialize(cardRequest) });
            string responseData = PostData(ServiceUrl, serializer.Serialize(cardRequest));
            NLogLogger.Info(new string[] { "VGGCard", "VGGCardResponse", responseData });

            return serializer.Deserialize<CardGGResult>(responseData);
        }

        public static string VggDecryptString(string data, string Passphrase)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] DataToDecrypt = Convert.FromBase64String(data);
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            return UTF8.GetString(Results);
        }

        private string PostData(string uri, string postData)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Method = "POST";//GET
            request.Accept = "JSON";
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
        
    }

    public class CardGGResult
    {
        public int ReturnCode { get; set; }
        public string ReturnData { get; set; }
        public string ReturnDescription { get; set; }
    }

    public class CardGGRequest
    {
        public string PasswordText { get; set; }    // Mật khẩu để giải mã
        public string PartnerCode { get; set; }     // Mã đối tác
        public string PartnerKey { get; set; }      // Mật khẩu của đối tác
        public string CardSerial { get; set; }      // Số serial cua thẻ
        public string CardNumber { get; set; }      // Mã thẻ
        public string AccountName { get; set; }     // Tài khoản sử dụng
        public string IPAddress { get; set; }       // Ip của khách hàng
        public string FunctionName { get; set; }    // Tên chức năng, trong đó: validatecard - kiem tra thong tin card, deactivecard - gạch thẻ
        public string Sign { get; set; }            // Chữ ký RSA
        public long PartnerLogId { get; set; }      // ID giao dịch của đối tác
    }

    public class CardData
    {
        public string Serial { get; set; }
        public string Code { get; set; }
        public int Value { get; set; }
        public string Type { get; set; }
    }
}
