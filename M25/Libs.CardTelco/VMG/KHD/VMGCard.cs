using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;
using System.Security.Cryptography;
using Encrypt;
using System.Web;

namespace Libs.CardTelco.VMG.KHD
{
    public class VMGCard : ICardTelcoHandler
    {
        protected string webserviceUrl = "http://charging-partner.vmgmedia.vn:8080/CardChargingGW_V2.0/services/Services";
        protected string publickey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAs+JvfyTOMHqvjxHJyDZGHZpz3atV7qcOT8mijXGGG3S+8Bb2p2kREGJwrzC2IIErCQUcZ3Wa3wTugKQDxqXESPt76HN2353ufegbvTI9kYgK0MLFpY8OZAMsaTytVrvUEVHjqGXZO4z7oVTqByuBwcZAvK+sN39+MqisS6ZejACbbQLkWZgcSgt5wBAaDaEa2lvRYcVbNyO/mqTU6SSfd+w78uM07BpmxhimOMwf+l/qs+Z04LUm4Ay7b+AHHAwbaHeehC1wInzNDfipgR0H0FCa/LOnEblj2HVpptB/NY4XNG+CDHTBKkxzEw92D/Nj1JIlr1oP0l+/VdAnxxiWuQIDAQAB";
        protected string privatekey = "MIICXwIBAAKBgQDOxgmGk1IvNsPWNiPIR+sYTrf7+Dunm66lJUxfDGkTpLF1PQ/DeQmZzzmzY58SQ5QDMAs0tYIobx8nrGXBiQbQhYC6TtsIBFrNjbSR09pLmzHZdORNQnunSlLzeNRnfMLy7VuBRvdrTPw7huL4RvOLQSoUL10ujXC5XZc3+6cJUQIDAQABAoGBAIp7b53FxOECGJ66m7RjFjytW+NRGQLj7U+Fb2So9ybEwmT2hDwyMA/nDYnrSnn88IBCCP9AIO/bnE4B1BSOJRUF+5pA3T2RbrBevBPtarP9y9FElnL5G10Fm3qP+2Jd0gsFhR6GV5JwQdZjOpzdSGMX4qShPtO2DdhPWS2i9wWRAkEA8mAiN0ZaEkMEpU2ds9kffqfUzjoKbOZHIiMY2lH2KsXOlxeM4W8fZVcCeeYF5NDMZu8Rk5JSWWNDIjAEW4leKwJBANpllFtJkOhW573kbhvcHbbGWuBWpiEcxT17+o+jEU9xh8WeYadzY2Yvh+xX9OFYzIWyIU+9ZKfCoPczbTnzNHMCQQCeZ9X06jXT2ZkfsCpxcGX1ERsz7RsDMT0sQmPry8VnDwCGHw4kB8wtH0CvCnavpQbx/y0tlWPcp9ModNlkOdMhAkEAmtximWSN2yIci1sZ9KldbIg0UlU+0cX72oA6CHYBxUpkku2eo0U/22qiwCUSYGQ+CiNoWYmbgRWSXBdcQTZCHwJBAOV0tavGcR3GR1TVc5hq2LFQ5V8KHgI1qtem3Ip1+Q/exCRO3XtcvhI3nsbTRaPFJAYZjP+0/A3mIAxVg1uK6WU=";
        protected string webservicUser = "VMG_DAVOS";
        protected string webservicPassword = "jneqyjwcj";
        protected string partnerID = "VMG_DAVOS";
        protected string parnertcode = "01276";
        protected string mpin = "chtzbmfif";
        protected string KeyPathPrivate = "\\private.pem";
        protected string KeyPathPublic = "\\public.pem";

        protected JavaScriptSerializer serializer = new JavaScriptSerializer();
        protected string providerName = "vmg";

        public VMGCard()
        {

        }

        private string Login()
        {
            //string result = "";
            var token = (string)HttpContext.Current.Application["VMGToken"];
            if (string.IsNullOrEmpty(token))
            {
                try
                {
                    ServicesService webServices = new ServicesService(webserviceUrl);
                    LoginResponse _LoginResponse = new LoginResponse();

                    NLogLogger.Info(new string[] { "VMG", "Key: ", System.Web.HttpContext.Current.Server.MapPath("~/Shakey/VMG") + KeyPathPublic });
                    string encryptPassword = BCCastle.Encrypt2(System.Web.HttpContext.Current.Server.MapPath("~/Shakey/VMG") + KeyPathPublic, webservicPassword);
                    NLogLogger.Info(new string[] {"VMG", "Login Request: ", webservicUser + "|" + encryptPassword + "|" + partnerID});
                    _LoginResponse = webServices.login(webservicUser, encryptPassword, partnerID);
                    NLogLogger.Info(new string[] { "VMG", "Login", serializer.Serialize(_LoginResponse) });

                    if (_LoginResponse.status.CompareTo("1") == 0)
                    {
                        token = BCCastle.Decrypt2(System.Web.HttpContext.Current.Server.MapPath("~/Shakey/VMG") + KeyPathPrivate, _LoginResponse.sessionid);
                        HttpContext.Current.Application["VMGToken"] = token;
                    }
                }
                catch (Exception ex)
                {
                    NLogLogger.Info(new string[] { "VMG", "Login", "Error", ex.Message });
                    token = "";
                }
            }

            return token;
        }

        private void Logout(string sessionid)
        {
            try
            {
                string md5hex2str = HashWithMD5(sessionid);

                byte[] cvmd52 = Convert.FromBase64String(md5hex2str);
                string md5sess = ByteArrayToHexString(cvmd52);
                LogoutResponse _LogoutResponse = new LogoutResponse();

                ServicesService webServices = new ServicesService(webserviceUrl);
                _LogoutResponse = webServices.logout(webservicUser, partnerID, md5sess);
                NLogLogger.Info(new string[] { "VMG", "Logout", serializer.Serialize(_LogoutResponse) });
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "VMG", "Logout", "Error", ex.Message });
            }
        }


        public APIResponse UseCard(APITransaction transaction)
        {
            APIResponse _APIResponse = new APIResponse();
            CardAPILog _CardAPILog = new CardAPILog();

            int step = 0;
            try
            {
                // Bước 1: Login, tạo session gọi VMG
                step = 1;
                string sessionid = Login();
                // Nếu không tạo được session
                if (sessionid == "")
                {
                    return new APIResponse((int)ResponseCode.LoginFail);
                }

                // Bước 2: Phân tích yêu cầu thành đối tượng
                step = 2;
                UseCardRequest request = new UseCardRequest();
                request = serializer.Deserialize<UseCardRequest>(transaction.RequestContent);

                // Bước 3: Ghi log giao dịch
                step = 3;
                _CardAPILog.TransactionID = transaction.TransactionID;
                _CardAPILog.PartnerID = transaction.PartnerID;
                _CardAPILog.PartnerCode = transaction.PartnerCode;
                _CardAPILog.AccountName = request.AccountName;
                _CardAPILog.AccountID = 0;
                _CardAPILog.CardSerial = request.CardSerial;
                _CardAPILog.CardCode = request.CardCode;
                _CardAPILog.CardType = request.CardType;
                _CardAPILog.Amount = 0;
                _CardAPILog.Provider = providerName;
                _CardAPILog.Status = 0;
                _CardAPILog.Description = "_CardAPILog.Add";
                _CardAPILog.RequestNo = request.RefCode;
                _CardAPILog.Add();

                // Nếu thêm giao dịch không hợp lệ
                if (_CardAPILog.ReturnValue < 0)
                {
                    return new APIResponse(_CardAPILog.ReturnValue);
                }

                string telcoCode = "";
                switch (_CardAPILog.CardType.ToLower())
                {
                    case "vms":
                        telcoCode = "VMS";
                        break;
                    case "vnp":
                        telcoCode = "VNP";
                        break;
                    case "viettel":
                        telcoCode = "VTT";
                        break;
                    case "vcoin":
                        telcoCode = "VCOIN";
                        break;
                    case "gate":
                        telcoCode = "FPT";
                        break;
                    case "bit":
                        telcoCode = "BIT";
                        break;
                    case "zing":
                        telcoCode = "ZING";
                        break;
                    case "vnm":
                        telcoCode = "VNM";
                        break;
                    case "mgc":
                        telcoCode = "MGC";
                        break;
                    case "onc":
                        telcoCode = "ONC";
                        break;
                    case "true":
                        telcoCode = "TRUE";
                        break;
                    case "telcs":
                        telcoCode = "TELCS";
                        break;
                    case "unip":
                        telcoCode = "UNIP";
                        break;
                    case "indm":
                        telcoCode = "INDM";
                        break;
                    case "thcll":
                        telcoCode = "THCLL";
                        break;
                    case "dtac":
                        telcoCode = "DTAC";
                        break;

                }

                // Bước 4: Mã hóa giao dịch
                step = 4;
                ServicesService _ServicesService = new ServicesService(webserviceUrl);
                //string transid, string username, string partnerID, string mpin, string target, string card_data, string md5sessionid
                string transid = parnertcode + "_" + transaction.TransactionID.ToString();
                string card_data = "";
                card_data = request.CardSerial + ":" + request.CardCode + "::" + telcoCode;
                card_data = EncryptDataTripdesPKCS7(card_data, sessionid);
                string mapin = EncryptDataTripdesPKCS7(mpin, sessionid);
                string sessionidmd5 = Encrypts.MD5(sessionid);

                // Bước 5: gọi hàm Gạch thẻ sang VMG
                step = 5;
                ChargeReponse _ChargeReponse = new ChargeReponse();
                NLogLogger.Info(new string[] { "VMG", transaction.TransactionID.ToString(), "CardRequest", transid, card_data, sessionid });
                _ChargeReponse = _ServicesService.cardCharging(transid, webservicUser, partnerID, mapin, request.AccountName, card_data, sessionidmd5);
                NLogLogger.Info(new string[] { "VMG", transaction.TransactionID.ToString(), "CardResponse", serializer.Serialize(_ChargeReponse) });

                // Bước 6: phân tích kết quả
                step = 6;
                if (_ChargeReponse.status == "1")
                {
                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    _CardAPILog.Amount = Convert.ToInt64(DecryptDataTripdesPKCS7(_ChargeReponse.responseamount, sessionid));
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = " Amount: " + _CardAPILog.Amount.ToString();
                    _CardAPILog.Status = 1;
                }
                else
                {
                    _APIResponse = new APIResponse(ConvertResponCode(_ChargeReponse.status));
                    _CardAPILog.Description = _ChargeReponse.message;
                    _CardAPILog.Status = _APIResponse.ResponseCode;

                }
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "VMG", transaction.TransactionID.ToString(), "Error", "UseCard", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "VMG", transaction.TransactionID.ToString(), "Error", "UseCard", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "VMG", transaction.TransactionID.ToString(), "Error", "UseCard", "Step3", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Encrypts.Encrypt " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "VMG", transaction.TransactionID.ToString(), "Error", "UseCard", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 5:
                        NLogLogger.Info(new string[] { "VMG", transaction.TransactionID.ToString(), "Error", "UseCard", "Step5", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step5 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    case 6:
                        NLogLogger.Info(new string[] { "VMG", transaction.TransactionID.ToString(), "Error", "UseCard", "Step6", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step6 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "VMG", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error " + ex.Message.Replace("\n", " ");
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

        protected static string HashWithMD5(string text)
        {

            MD5CryptoServiceProvider hashAlgorithm = new MD5CryptoServiceProvider();

            return HashString(text, hashAlgorithm);

        }

        protected static string HashString(string stringToHash, HashAlgorithm algo)
        {

            byte[] bytes = Encoding.UTF8.GetBytes(stringToHash);

            bytes = algo.ComputeHash(bytes);
            return Convert.ToBase64String(bytes);
        }

        private static string ByteArrayToHexString(byte[] Bytes)
        {
            StringBuilder Result = new StringBuilder();
            string HexAlphabet = "0123456789ABCDEF";

            foreach (byte B in Bytes)
            {
                Result.Append(HexAlphabet[(int)(B >> 4)]);
                Result.Append(HexAlphabet[(int)(B & 0xF)]);
            }

            return Result.ToString();
        }


        private string EncryptDataTripdesPKCS7(string origData, string key)
        {
            try
            {
                byte[] tripkey = Encrypt.hexa.hexatobyte(key);
                //string erro = "";
                byte[] entrip = TripleDES_Encrypt_Byte(tripkey, System.Text.Encoding.UTF8.GetBytes(origData));//Encrypt.Encrypt._EncryptTripleDes(origData, tripkey, ref erro);
                return ByteArrayToHexString(entrip);
            }
            catch
            {
                return "";
            }
        }

        private byte[] TripleDES_Encrypt_Byte(byte[] Keys, byte[] clearText)
        {
            byte[] IVs = new byte[8];
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.IV = IVs;
            des.KeySize = 192; //24Bytes                
            des.Key = Keys;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            //clearText = /System.Text.Encoding.ASCII.GetBytes()//FillBlock(clearText);
            byte[] cipherText = des.CreateEncryptor().TransformFinalBlock(clearText, 0, clearText.Length);
            return cipherText;
        }

        public string DecryptDataTripdesPKCS7(string origData, string key)
        {
            try
            {
                byte[] tripkey = Encrypt.hexa.hexatobyte(key);
                //string erro = "";
                byte[] entrip = TripleDES_Decrypt_Byte(tripkey, Encrypt.hexa.hexatobyte(origData));//Encrypt.Encrypt._EncryptTripleDes(origData, tripkey, ref erro);
                return System.Text.Encoding.UTF8.GetString(entrip);
            }
            catch
            {
                return "";
            }
        }

        private byte[] TripleDES_Decrypt_Byte(byte[] Keys, byte[] clearText)
        {
            byte[] IVs = new byte[8];
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.IV = IVs;
            des.KeySize = 192; //24Bytes                
            des.Key = Keys;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            //clearText = /System.Text.Encoding.ASCII.GetBytes()//FillBlock(clearText);
            byte[] cipherText = des.CreateDecryptor().TransformFinalBlock(clearText, 0, clearText.Length);
            return cipherText;
        }

        private int ConvertResponCode(string error_code)
        {
            switch (error_code)
            {
                case "-24":
                    //Dữ liệu carddata không đúng
                    return (int)ResponseCode.SignatureInvalid;
                case "-11":
                    //Nhà cung không tồn tại
                    return (int)ResponseCode.ProviderNotFound;
                case "-10":
                    //Mã thẻ sai định dạng
                    return (int)ResponseCode.CardCodeInvalid;
                case "0":
                    //Transaction fail
                    return (int)ResponseCode.TransactionFailed;
                case "3":
                    //Sai Session
                    return (int)ResponseCode.AccessDenied;
                case "4":
                    //Thẻ không dụng được
                    return (int)ResponseCode.CardFormatInvalid;
                case "5":
                    //partner nhập sai mã thẻ quá 5 lần.
                    return (int)ResponseCode.AccountLocked;
                case "7":
                    //Session hết hạn
                    return (int)ResponseCode.TransactionTimeout;
                case "8":
                    //Sai Ip
                    return (int)ResponseCode.IpInvalid;
                case "9":
                    //Tạm thời khóa kênh nạp VMS do quá tải.
                    return (int)ResponseCode.AccountLocked;
                case "10":
                    //Hệ thống nhà cung cấp gặp lỗi.
                    return (int)ResponseCode.SystemError;
                case "16":
                    //Mã giao dịch không tồn tại
                    return (int)ResponseCode.TransactionInvalid;
                case "11":
                    //Kết nối với nhà cung cấp tạm thời bị gián đoạn
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "12":
                    //Trùng transactionID với một giao dịch trước đó
                    return (int)ResponseCode.TransactionDuplicate;
                case "13":
                    //Hệ thống tạm thời bận
                    return (int)ResponseCode.SystemError;
                case "-2":
                    //Thẻ đã bị khóa
                    return (int)ResponseCode.CardIsLocked;
                case "-3":
                    //Thẻ hết hạn sử dụng
                    return (int)ResponseCode.CardHasExpired;
                case "50":
                    //Thẻ đã sử dụng hoặc không tồn tại
                    return (int)ResponseCode.CardUsed;
                case "51":
                    //Seri thẻ không đúng
                    return (int)ResponseCode.CardSerialInvalid;
                case "52":
                //Mã thẻ và serial không khớp
                case "53":
                    //Serial hoặc mã thẻ không đúng
                    return (int)ResponseCode.CardFormatInvalid;
                case "55":
                    //Card tạm thời bị block 24 h.
                    return (int)ResponseCode.CardIsLocked;
                case "62":
                //Sai mật khẩu
                case "57":
                //Sai mpin
                case "58":
                //Sai tham số đầu vào
                case "60":
                //Sai partnerid
                case "61":
                    //Sai user
                    return (int)ResponseCode.ParameterInvalid;
                case "59":
                    //Mã thẻ chưa được kích hoạt.
                    return (int)ResponseCode.CardNotActivated;
                case "56":
                    //TargetAccount tạm thời bị khóa do Charging sai nhiều lần.
                    return (int)ResponseCode.AccountLocked;
                case "63":
                    //Không tìm thấy giao dịch này(Hàm kiểm tra trạng thái gd)
                    return (int)ResponseCode.TransactionNotExists;
                case "64":
                    //Giãi mã dữ liệu fail mật khẩu, hoặc mpin gửi lên không thành công.
                    return (int)ResponseCode.SignatureInvalid;
                case "65":
                    //Số lượng kết nối của partner quá mức cho phép..
                    return (int)ResponseCode.TransactionLimit;
                case "66":
                    //Mã thẻ đã gửi một giao dịch thành công lên hệ thống.
                    return (int)ResponseCode.CardUsed;
                case "67":
                    //Serial hoặc mã thẻ không đúng định dạng của hệ thống
                    return (int)ResponseCode.CardFormatInvalid;
                case "99":
                    //Serial hoặc mã thẻ không đúng định dạng của hệ thống
                    return (int)ResponseCode.TransactionTimeout;
                case "68":
                    //Nhà cung cấp bị lỗi hoặc đã bị khóa.
                    return (int)ResponseCode.AccountLocked;
                default:
                    return (int)ResponseCode.TransactionFailed;
            }
        }

        public APIResponse ReCheck(string transactionId)
        {
            throw new NotImplementedException();
        }
    }
}
