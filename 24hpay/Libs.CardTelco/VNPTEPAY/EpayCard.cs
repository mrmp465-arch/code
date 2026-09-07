using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;
using System.Security.Cryptography;
using Encrypt;

namespace Libs.CardTelco.VNPTEPAY
{
    public class EpayCard
    {
        protected string webserviceUrl = "http://115.78.133.42:9090/CardChargingGW/services/Services";
        protected string publickey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAs+JvfyTOMHqvjxHJyDZGHZpz3atV7qcOT8mijXGGG3S+8Bb2p2kREGJwrzC2IIErCQUcZ3Wa3wTugKQDxqXESPt76HN2353ufegbvTI9kYgK0MLFpY8OZAMsaTytVrvUEVHjqGXZO4z7oVTqByuBwcZAvK+sN39+MqisS6ZejACbbQLkWZgcSgt5wBAaDaEa2lvRYcVbNyO/mqTU6SSfd+w78uM07BpmxhimOMwf+l/qs+Z04LUm4Ay7b+AHHAwbaHeehC1wInzNDfipgR0H0FCa/LOnEblj2HVpptB/NY4XNG+CDHTBKkxzEw92D/Nj1JIlr1oP0l+/VdAnxxiWuQIDAQAB";
        protected string privatekey = "MIICXAIBAAKBgQCxjjvVYnRTcT70Cd68nE08DKwuMlSeOfbcgtTvgawgC/18p/Xs6wcg2U2xn0nn+9O6x+pJ9o5uQa6iqTgOJz1jVKWSwWuFD44muEyqMdeE1IYWbrJOFUFIU54Cau3qS3GKgWtOTAZUYqVh+cOPxgo0pW2is+swT6tz56/P0p22vwIDAQABAoGAaFrGQ9XHtLscWuXqKURcCG0STVx7azt6IYQrjlDST0t8wmUdHw/Lcr0E8t5B27ygZmjVBH+Kmraz4xo9vePGKb5Z/+BcRsHhkQcmRUJlvML6hl8jSartVs4g8j6qEzXsXPe4rEiSxpOSWm7tDlCB/X//FbfLOJB7kc9Xuy/PxRECQQDfEr82UpIUTPJNdL3AWSIH2KqOL3XcxjhwakQq8JdZTLDgoRHx6AQ5RvjTOeIAYDZdSM33iZeDqdTxszbKi4WLAkEAy8OD5HLTVLzRn3JMwZnTxIHdwYdRnpY/OcBLoaW2zI49hUN9YkXgkaD+JpuP7vLPOfvFvHEDhjTjW6Ms+t2CHQJABdufK9UFQwU2Q7RyGy/8Bcq5x9wVM0P9TW5s9de1kcHCz1NLflfCbKKhfCKD/dCI/PAhgIObd/iov+Qd5zm8uwJBALZDlzC7vWlo1KEpXps2fASknbXE0y9l+fwwk/ZuAsuK2GDh87/5/VyGg5AJSoBU1SRqn39mH97mZBDOLyffB8kCQDWqDp4Kw86VW1yKPl3eIT3sLlXzNq6o6PLw2sttUf0OUAzPiucS7OhMnYeSq/oqIGk/DvGsQnTgTXtDIBUCswE=";
        protected string webservicUser = "VGG";
        protected string webservicPassword = "123456";
        protected string partnerID = "VGG";
        protected string parnertcode = "00305";
        protected string mpin = "123456";

        protected JavaScriptSerializer serializer = new JavaScriptSerializer();
        protected string providerName = "vnptepay";

        public EpayCard()
        {

        }

        private string Login()
        {
            string result = "";
            try
            {
                ServicesService webServices = new ServicesService(webserviceUrl);
                LoginResponse _LoginResponse = new LoginResponse();

                string encryptPassword = EncryptDataRSA(webservicPassword, publickey);

                _LoginResponse = webServices.login(webservicUser, encryptPassword, partnerID);
                NLogLogger.Info(new string[] { "VNPTEPAY", "Login", serializer.Serialize(_LoginResponse) });

                if (_LoginResponse.status.CompareTo("1") == 0)
                {
                    result = DecryptDataRSA(_LoginResponse.sessionid, privatekey);
                }
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "VNPTEPAY", "Login", "Error", ex.Message });
                result = "";
            }
            return result;
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
                NLogLogger.Info(new string[] { "VNPTEPAY", "Logout", serializer.Serialize(_LogoutResponse) });
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "VNPTEPAY", "Logout", "Error", ex.Message });
            }
        }


        public APIResponse UseCard(APITransaction transaction)
        {
            APIResponse _APIResponse = new APIResponse();
            CardAPILog _CardAPILog = new CardAPILog();

            int step = 0;
            try
            {
                // Bước 1: Phân tích yêu cầu thành đối tượng
                step = 1;
                UseCardRequest request = new UseCardRequest();
                request = serializer.Deserialize<UseCardRequest>(transaction.RequestContent);

                // Bước 2: Ghi log giao dịch
                step = 2;

                _CardAPILog.TransactionID = transaction.TransactionID;
                _CardAPILog.PartnerID = transaction.PartnerID;
                _CardAPILog.AccountName = request.AccountName;
                _CardAPILog.AccountID = 0;
                _CardAPILog.CardSerial = request.CardSerial;
                _CardAPILog.CardCode = request.CardCode;
                _CardAPILog.CardType = request.CardType;
                _CardAPILog.Amount = 0;
                _CardAPILog.Provider = providerName;
                _CardAPILog.Status = 0;
                _CardAPILog.Description = "_CardAPILog.Add";
                _CardAPILog.Add();

                // Nếu thêm giao dịch không hợp lệ
                if (_CardAPILog.ReturnValue < 0)
                {
                    return new APIResponse(_CardAPILog.ReturnValue);
                }

                // Bước 3: Login, tạo session
                step = 3;
                string sessionid = Login();
                // Nếu không tạo được session
                if (sessionid == "")
                {
                    return new APIResponse((int)ResponseCode.SystemError);
                }

                ServicesService _ServicesService = new ServicesService(webserviceUrl);
                //string transid, string username, string partnerID, string mpin, string target, string card_data, string md5sessionid
                string transid = parnertcode + "_" + transaction.TransactionID.ToString();
                string card_data = "";
                card_data = request.CardSerial + ":" + request.CardCode + "::" + request.CardType;
                NLogLogger.Info(new string[] { "VNPTEPAY", transaction.TransactionID.ToString(), "CardRequest", transid, card_data, sessionid });

                card_data = EncryptDataTripdesPKCS7(card_data, sessionid);
                string mapin = EncryptDataTripdesPKCS7(mpin, sessionid);
                string sessionidmd5 = Encrypts.MD5(sessionid);

                // Bước 4: gọi hàm sang VNPTEPAY
                step = 4;
                ChargeReponse _ChargeReponse = new ChargeReponse();
                _ChargeReponse = _ServicesService.cardCharging(transid, webservicUser, partnerID, mapin, request.AccountName, card_data, sessionidmd5);
                NLogLogger.Info(new string[] { "VNPTEPAY", transaction.TransactionID.ToString(), "CardResponse", serializer.Serialize(_ChargeReponse) });

                // Bước 5: giải mã kết quả trả về
                step = 5;

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
                        NLogLogger.Info(new string[] { "VNPTEPAY", transaction.TransactionID.ToString(), "Error", "UseCard", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "VNPTEPAY", transaction.TransactionID.ToString(), "Error", "UseCard", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "VNPTEPAY", transaction.TransactionID.ToString(), "Error", "UseCard", "Step3", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Encrypts.Encrypt " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "VNPTEPAY", transaction.TransactionID.ToString(), "Error", "UseCard", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 5:
                        NLogLogger.Info(new string[] { "VNPTEPAY", transaction.TransactionID.ToString(), "Error", "UseCard", "Step5", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step5 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    case 6:
                        NLogLogger.Info(new string[] { "VNPTEPAY", transaction.TransactionID.ToString(), "Error", "UseCard", "Step6", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step6 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "VNPTEPAY", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
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

        // Mã hóa dữ liệu
        protected string EncryptDataRSA(string origData, string publicKey)
        {
            byte[] plaintext = System.Text.Encoding.ASCII.GetBytes(origData);


            byte[] bKey = JavaScience.opensslkey.DecodePkcs8EncPrivateKey(publicKey);


            System.Security.Cryptography.RSACryptoServiceProvider myrsa = JavaScience.opensslkey.DecodeX509PublicKey(bKey);

            byte[] encrypt = myrsa.Encrypt(plaintext, false);
            string enStr = Convert.ToBase64String(encrypt);
            return enStr;
        }

        // Giải mã dữ liệu
        protected string DecryptDataRSA(string deData, string privateKey)
        {
            try
            {
                byte[] Databyte = Convert.FromBase64String(deData);
                byte[] privatebKEY = JavaScience.opensslkey.DecodeOpenSSLPrivateKey(privateKey);
                RSACryptoServiceProvider rsa = JavaScience.opensslkey.DecodeRSAPrivateKey(privatebKEY);

                string dataDecript = Encoding.ASCII.GetString(rsa.Decrypt(Databyte, false));
                return dataDecript;
            }
            catch
            {
                return "";
            }
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
                case "00":
                    //00 Đã xử lý thành công (thẻ đã bị gạch, tiền đã được nạp vào tài khoản của merchant)
                    return (int)ResponseCode.TransactionSuccessful;
                case "99":
                    //01 Lỗi, tuy nhiên lỗi chưa được định nghĩa/không xác định
                    return (int)ResponseCode.TransactionFailed;
                case "01":
                    //01 Lỗi, địa chỉ IP truy cập API của NgânLượng.vn bị từ chối
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "02":
                    //02 Lỗi, Mã website/merchant không tồn tại hoặc website/merchant đang bị khóa
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "03":
                    //03 Lỗi, Địa chỉ IP truy cập API của NgânLượng.vn bị từ chối
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "04":
                    //04 Lỗi, Mã checksum không chính xác
                    return (int)ResponseCode.TransactionFailed;
                case "05":
                    //05 Tài khoản nhận tiền nạp của merchant không tồn tại
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "06":
                    //06 Tài khoản nhận tiền nạp của merchant đang bị khóa hoặc bị phong tỏa, không thể thực hiện được giao dịch nạp tiền
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "07":
                    //07 Thẻ đã được sử dụng, hoặc thẻ sai
                    return (int)ResponseCode.CardUsed;
                case "09":
                    //09 Thẻ hết hạn sử dụng
                    return (int)ResponseCode.CardHasExpired;
                case "10":
                    //10 Thẻ chưa được kích hoạt hoặc không tồn tại
                    return (int)ResponseCode.CardNotActivated;
                case "11":
                    //11 Mã thẻ sai định dạng
                    return (int)ResponseCode.CardCodeInvalid;
                case "12":
                    //12 Sai số serial của thẻ
                    return (int)ResponseCode.CardSerialInvalid;
                case "13":
                    //13 Mã thẻ và số serial không khớp
                    return (int)ResponseCode.CardCodeInvalid;
                case "14":
                    //14 Thẻ không tồn tại
                    return (int)ResponseCode.CardCodeInvalid;
                case "15":
                    //15 Thẻ không sử dụng được
                    return (int)ResponseCode.CardIsLocked;
                case "16":
                    //16 Số lần thử (nhập sai liên tiếp) của thẻ vượt quá giới hạn cho phép
                    return (int)ResponseCode.CardIsLocked;
                case "17":
                    //17 Hệ thống Telco bị lỗi hoặc quá tải, thẻ chưa bị trừ
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "18":
                    //18 Hệ thống Telco bị lỗi hoặc quá tải, thẻ có thể bị trừ, cần phối hợp với NgânLượng.vn để tra soát
                    return (int)ResponseCode.TransactionSuspicious;
                //19		Kết nối từ NgânLượng.vn tới hệ thống Telco bị lỗi, thẻ chưa bị trừ (thường do lỗi kết nối giữa NgânLượng.vn với Telco, ví dụ sai tham số kết nối, mà không liên quan đến merchant)
                //20	Kết nối tới telco thành công, thẻ bị trừ nhưng chưa cộng tiền trên NgânLượng.vn
                default:
                    return (int)ResponseCode.TransactionFailed;
            }
        }
    }
}
