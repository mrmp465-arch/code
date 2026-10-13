using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Utils;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;
using System.Web.Hosting;

namespace Libs.CardTelco.Gate.KHD
{
    public class GateCard1 : ICardTelcoHandler
    {

        // Cấu hình kết nối Production 
        //XBOM
        //protected string webserviceUrl = "https://daily.gate.vn/api/card/ws.asmx";
        //protected string username = "01222882493";
        //protected string password = "M3SdyRZvsEZ6SrVM";

        //PayPlus
        protected string webserviceUrl = "https://daily.gate.vn/api/card/ws.asmx";
        protected string username = "0986909640";
        protected string password = "@Meoicondoi2018";

        protected string providerName = "gate1";
        protected JavaScriptSerializer serializer = new JavaScriptSerializer();
        public class RequestData
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Serial { get; set; }
            public string PinCode { get; set; }
            public string CardName { get; set; }
            public int Timestamp { get; set; }
            public string Signature { get; set; }
            public string partnerTransID { get; set; }

        }

        public class ResponseData
        {
            public string ErrorCode { get; set; }
            public string Description { get; set; }
            public string TransactionID { get; set; }
            public string CardAmount { get; set; }
            public string AmountBalance { get; set; }
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
                    return new APIResponse(_CardAPILog.ReturnValue); ;
                }

                // Bước 3: mã hóa dữ liệu
                step = 3;
                string cardType = "";
                switch (_CardAPILog.CardType.ToLower())
                {
                    case "gate":
                        cardType = "GATE";
                        break;
                    case "vms":
                        cardType = "MOBIFONE";
                        break;
                    case "vnp":
                        cardType = "VINAPHONE";
                        break;
                    case "viettel":
                        cardType = "VIETTEL";
                        break;
                }



                RequestData rq = new RequestData();
                rq.Username = username;
                rq.Password = password;
                rq.Serial = _CardAPILog.CardSerial;
                rq.PinCode = _CardAPILog.CardCode;
                rq.CardName = cardType;
                rq.Timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                rq.Signature = Encrypts.MD5(String.Format("{0}{1}{2}{3}{4}", username, password, rq.Serial, rq.PinCode, rq.CardName));
                rq.partnerTransID = _CardAPILog.TransactionID.ToString();
                // Bước 4: gọi hàm sang Gate
                step = 4;
                ws sb = new ws(webserviceUrl);
                sb.Timeout = 120000;
                string gateResponse = string.Empty;
                try
                {
                    NLogLogger.Info(new string[] { "Gate", transaction.TransactionID.ToString(), "GateRequest", serializer.Serialize(rq) });
                    gateResponse = sb.ChargeCard(rq.Username, rq.Password, rq.Serial, rq.PinCode, rq.CardName, rq.Timestamp.ToString(), rq.Signature,rq.partnerTransID); //Output : Trả về giá trị của chuỗi với định dạng (ErrorCode|Description|TransactionID|PartnerTransactionID|CardAmount) 
                    NLogLogger.Info(new string[] { "Gate", transaction.TransactionID.ToString(), "GateResponse", gateResponse });
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.Timeout)
                    {
                        gateResponse = "-5002|Ngắt kết nối đến nhà cung cấp. Timeout exception.|xxxxxxxxxxxx";
                    }

                    NLogLogger.Info(new string[]
                    {
                        "Gate", transaction.TransactionID.ToString(), "Error", "UseCard", "Timeout exception ",
                        ex.Message.Replace("\n", " ")
                    });
                }
                finally
                {
                    sb.Dispose();
                }

                // Bước 5: giải mã kết quả trả về
                step = 5;
                //Không có

                // Bước 6: phân tích kết quả
                step = 6;
                ResponseData response = GateResponse(gateResponse);

                if (response.ErrorCode.Equals("0"))
                {
                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    _CardAPILog.Amount = Convert.ToInt32(response.CardAmount);
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = " Amount: " + _CardAPILog.Amount.ToString();
                    _CardAPILog.Status = 1;
                }
                else
                {
                    _APIResponse = new APIResponse(ConvertResponCode(response.ErrorCode));
                    _APIResponse.Description = response.Description;
                    _CardAPILog.Description = gateResponse;
                    _CardAPILog.Status = _APIResponse.ResponseCode;

                }
            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "Gate", transaction.TransactionID.ToString(), "Error", "UseCard", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "Gate", transaction.TransactionID.ToString(), "Error", "UseCard", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "Gate", transaction.TransactionID.ToString(), "Error", "UseCard", "Step3", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Encrypts.Encrypt " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "Gate", transaction.TransactionID.ToString(), "Error", "UseCard", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuspicious);
                        break;
                    case 5:
                        NLogLogger.Info(new string[] { "Gate", transaction.TransactionID.ToString(), "Error", "UseCard", "Step5", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step5 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    case 6:
                        NLogLogger.Info(new string[] { "Gate", transaction.TransactionID.ToString(), "Error", "UseCard", "Step6", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - Step6 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "Gate", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
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

        public ResponseData GateResponse(string data)
        {
            //data = "ErrorCode|Description|TransactionID|PartnerTransactionID|CardAmount";
            string[] s = data.Split('|');

            if (s.Count() == 2)
            {
                return new ResponseData()
                {
                    ErrorCode = s[0],
                    Description = s[1]
                };
            }

            if (s.Count() == 3)
            {
                return new ResponseData()
                {
                    ErrorCode = s[0],
                    Description = s[1],
                    TransactionID = s[2]
                };
            }
            if (s.Count() == 5)
            {
                return new ResponseData()
                {
                    ErrorCode = s[0],
                    Description = s[1],
                    TransactionID = s[2],
                    CardAmount = s[3],
                    AmountBalance = s[4]

                };
            }

            return new ResponseData()
            {
                ErrorCode = "999", // TrungDT tự Define
                Description = "Không phân tích được ResponseData"
            };
        }

        private int ConvertResponCode(string responseStatus)
        {
            switch (responseStatus)
            {
                case "107":
                    //107: Dữ liệu đầu vào không hợp lệ.
                    return (int)ResponseCode.ParameterInvalid;
                case "139":
                    //139: Trùng giao dịch.
                    return (int)ResponseCode.TransactionDuplicate;
                case "101":
                    //101: Thẻ đã được sử dụng.
                    return (int)ResponseCode.CardUsed;
                case "102":
                    //102: Thẻ không tồn tại hoặc chưa được kích hoạt.
                    return (int)ResponseCode.CardNotActivated;
                case "103":
                    //103: Thông tin mã thẻ không đúng định dạng.
                    return (int)ResponseCode.CardCodeInvalid;
                case "104":
                    //104: Tài khoản bị tạm khóa trong 5 phút.
                    return (int)ResponseCode.AccountLocked;
                case "105":
                case "114":
                    //105: Thẻ đã hết hạn sử dụng.
                    return (int)ResponseCode.CardHasExpired;
                case "106":
                    //106: Thẻ đã bị khóa.
                    return (int) ResponseCode.CardIsLocked;
                case "115":
                    //115: Trạng thái thẻ không phù hợp.
                    return (int)ResponseCode.CardTypeInvalid;
                case "116":
                    //116: Lỗi hệ thống telco.
                    return (int)ResponseCode.SystemError;
                case "117":
                    //117: Chữ ký điện tử không hợp lệ
                    return (int)ResponseCode.SignatureInvalid;
                case "119":
                    //119: Serial va PIN khong ton tai
                    return (int)ResponseCode.CardSerialInvalid;
                case "120":
                    //120: Đối tác kết nối telco không hợp lệ
                    return (int)ResponseCode.PartnerNotExistsNotActive;
                case "121":
                    //121: Thẻ không tồn tại
                    return (int)ResponseCode.CardSerialInvalid;
                case "122":
                    //122: Đầu thẻ không đuợc hỗ trợ
                    return (int)ResponseCode.CardProviderInvalid;
                case "123":
                    //123: Mệnh giá thẻ chua đuợc cấu hình cho đối tác
                    return (int)ResponseCode.CardAmountInvalid;
                case "124":
                    //124: Lỗi không gọi đuợc nhà mạng
                    return (int)ResponseCode.SystemError;
                case "125":
                    //125: Timeout từ hệ thống core
                    return (int)ResponseCode.TransactionTimeout;
                case "126":
                    //126: Lỗi database hệ thống
                    return (int)ResponseCode.SystemError;
                case "127":
                    //127: Mất kết nối với hệ thông core
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "129":
                    //129: Đại lý không hợp pháp.
                    return (int)ResponseCode.AccountNotExists;
                case "130":
                    //130: Đại lý chua có số tiền hợp lệ.
                    return (int)ResponseCode.BalanceNotEnough;
                case "131":
                    //131: Số tiền không hợp lệ.
                    return (int)ResponseCode.CardAmountInvalid;
                case "133":
                    //133: Tài khoản đại lý không đủ tiền thực hiện giao dịch.
                    return (int)ResponseCode.BalanceNotEnough;
                case "134":
                    //134: Thông tin chiết khấu không hợp lệ.
                    return (int)ResponseCode.BankCardInfoInvalid;
                case "135":
                    //135: Mã giao dịch của Đại lý đã tồn tại.
                    return (int)ResponseCode.TransactionInvalid;
                case "137":
                    //137: Đối tác không hợp lệ.
                    return (int)ResponseCode.PartnerNotExistsNotActive;
                case "100":
                    //100: Dịch vụ đang bảo trì.
                    return (int)ResponseCode.SystemMaintain;
                case "140":
                    //140: Dịch vụ không hợp lệ.
                    return (int)ResponseCode.ServiceNotExists;
                case "141":
                    //141: Không lấy đuợc thông tin đối tác.
                    return (int)ResponseCode.AccountNotExists;
                case "142":
                    //142: Chức năng không tồn tại.
                    return (int)ResponseCode.ServiceNotExists;
                case "143":
                    //143: Xử lý giao dich thất bại.
                    return (int)ResponseCode.TransactionFailed;
                case "144":
                    //144: Thực hiện sai quá số lần cho phép.
                    return (int)ResponseCode.TransactionLimit;
                case "145":
                    //145: Mã giao dịch không tồn tại.
                    return (int)ResponseCode.TransactionInvalid;
                case "99":
                    //99: Giao dịch thất bại.
                    return (int)ResponseCode.TransactionFailed;
                case "-5002":
                    //95: Xem thêm message trả về
                    return (int)ResponseCode.TransactionTimeout;
                default:
                    return (int)ResponseCode.SystemError;
            }
        }

        public APIResponse ReCheck(string transactionId)
        {
            throw new NotImplementedException();
        }
    }

    public class DataSign
    {
        private UTF8Encoding enc = new UTF8Encoding();
        private string strOriginalData;
        private RSAParameters rsaPrivateParams;
        //a byte array to store hash value

        private byte[] hashedData;
        public DataSign()
            : base()
        {
        }

        public string OriginalData
        {
            get { return strOriginalData; }
            set { strOriginalData = value; }
        }

        public RSAParameters PrivateParams
        {
            get { return rsaPrivateParams; }
            set { this.rsaPrivateParams = value; }
        }

        //Manually performs hash and then signs hashed value.
        public byte[] HashAndSign(byte[] encrypted)
        {
            //create new instance of RSACryptoServiceProvider
            RSACryptoServiceProvider rsaCSP = new RSACryptoServiceProvider();
            //create new instance of SHA1 hash algorithm to compute hash
            SHA1Managed hash = new SHA1Managed();
            try
            {
                //import private key params into instance of RSACryptoServiceProvider
                rsaCSP.ImportParameters(rsaPrivateParams);
                //compute hash with algorithm specified as here we have SHA1
                hashedData = hash.ComputeHash(encrypted);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            // Sign Data using private key and  OID is simple name of the algorithm for which to get the object identifier (OID)
            return rsaCSP.SignHash(hashedData, CryptoConfig.MapNameToOID("SHA1"));
        }
        //HashAndSign

        public RSAParameters ReadPrivateKeyFromFile(string fileName)
        {
            RSAParameters param = new RSAParameters();
            try
            {
                FileStream sw = File.OpenRead(fileName);
                param.P = ReadByteArray(sw);
                param.Q = ReadByteArray(sw);
                param.D = ReadByteArray(sw);
                param.DP = ReadByteArray(sw);
                param.DQ = ReadByteArray(sw);
                param.InverseQ = ReadByteArray(sw);
                param.Exponent = ReadByteArray(sw);
                param.Modulus = ReadByteArray(sw);
                sw.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            return param;
        }

        private byte[] ReadByteArray(FileStream writer)
        {
            int length = 0;
            for (int i = 0; i <= 3; i++)
            {
                length = length | writer.ReadByte() >> (8 * (32 - 8 - i));
            }
            if (length == 0)
            {
                return null;
            }
            byte[] array = new byte[length];
            writer.Read(array, 0, length);
            return array;
        }

        public string Sign(string OriginalData, string strPrivateKeyFile)
        {
            string strSignature = null;
            try
            {
                PrivateParams = ReadPrivateKeyFromFile(strPrivateKeyFile);
                strSignature = Convert.ToBase64String((HashAndSign(enc.GetBytes(OriginalData))));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            return strSignature;
        }

        public string Sign()
        {
            return Convert.ToBase64String((HashAndSign(enc.GetBytes(strOriginalData))));
        }

        public string SignX509_PKCS12(bool bMethod, string PrivateKeyPath, string PrivateKeyPassword, string OriginalData)
        {
            try
            {
                if (bMethod)
                {
                    X509Certificate2 cert = new X509Certificate2(PrivateKeyPath, PrivateKeyPassword, X509KeyStorageFlags.MachineKeySet);
                    RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PrivateKey;

                    SHA1Managed sha1 = new SHA1Managed();
                    byte[] data = UTF8Encoding.UTF8.GetBytes(OriginalData);
                    byte[] hash = sha1.ComputeHash(data);
                    return Convert.ToBase64String(csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1")));
                }
                else
                {
                    DataSign objSign = new DataSign();
                    return objSign.Sign(OriginalData, PrivateKeyPath);
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }

        public string SignX509(bool bMethod, string CertPath, string CertPassword, string OriginalData)
        {
            try
            {
                if (bMethod)
                {
                    X509Certificate2 x509 = new X509Certificate2(CertPath, CertPassword);

                    ContentInfo ci = new ContentInfo(enc.GetBytes(OriginalData));
                    SignedCms sc = new SignedCms(ci, true);
                    //Dim cs As CmsSigner = New CmsSigner(x509)
                    CmsSigner cs = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, x509);
                    //cs.IncludeOption = X509IncludeOption.None
                    sc.ComputeSignature(cs);
                    byte[] signed = sc.Encode();

                    return Convert.ToBase64String(signed);
                }
                else
                {
                    DataSign objSign = new DataSign();
                    return objSign.Sign(OriginalData, CertPath);
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Computes a signature for the specified data using SHA1 for hashing followed by encryption using the private key
        /// </summary>
        /// <param name="PrivKeyPath"></param>
        /// <param name="PwdPrivKey"></param>
        /// <param name="OriginData"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string SignData(string OriginData, string PrivKeyPath, string PwdPrivKey)
        {
            try
            {
                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                X509Certificate2 x509Cert = new X509Certificate2(PrivKeyPath, PwdPrivKey, X509KeyStorageFlags.MachineKeySet);
                RSACryptoServiceProvider rsaCryptoIPT = (RSACryptoServiceProvider)x509Cert.PrivateKey;
                byte[] data = UTF8Encoding.UTF8.GetBytes(OriginData);
                return Convert.ToBase64String(rsaCryptoIPT.SignData(data, sha1));
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string SignRSAKey(string OriginalData, string PrivateKeyPath)
        {
            try
            {
                RSACryptoServiceProvider RSAProvider = new RSACryptoServiceProvider();
                PrivateKeyPath = GetKeyFromFile(PrivateKeyPath);
                RSAProvider.FromXmlString(PrivateKeyPath);
                byte[] byteKey = null;
                byteKey = Encoding.ASCII.GetBytes(OriginalData);
                byte[] signature = null;
                signature = RSAProvider.SignData(byteKey, new SHA1CryptoServiceProvider());
                string sSing = Convert.ToBase64String(signature);
                return sSing;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private static string GetKeyFromFile(string Path)
        {
            StreamReader stream = new StreamReader(Path);
            string data = stream.ReadToEnd();
            stream.Close();
            return data;
        }
    }
}
