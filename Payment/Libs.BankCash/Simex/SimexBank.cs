using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.BankCash.Drum;
using Libs.Report;
using Libs.Utils;
using static Libs.BankCash.BankCashService;

using static Libs.BankCash.Jav.SimexBankLib;


namespace Libs.BankCash.Simex
{
    public class SimexBank : IBankCashHandler
    {
        // Production MoBo
        private const string urlBaseService = "https://api.bobap.xyz/api/";
        private const string callbackurl = "https://bankgate.coroach.xyz//Callback/SimexCash.ashx";
        private const string pw = "6bCG3N2uej";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private const string secretKey = "2c4bc033e05e45db807ab9b2ff07d165";
        private const string secretKey2 = "2c4bc033e05e45db807ab9b2ff07d165";

        private const string pw2 = "6bCG3N2uej";

        public APIResponse ReCallBack(long id)
        {
            throw new NotImplementedException();
        }
        public APIResponse Check(APITransaction transaction)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();




            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public APIResponse Cash(APITransaction transaction)
        {
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            OrderRequest request = new OrderRequest();
            request = serializer.Deserialize<OrderRequest>(transaction.RequestContent);
            var systemDataConfig = new SystemDataConfig();
            var lstConfig = systemDataConfig.GetListCache();

            var partner = new Partners().GetCache(transaction.PartnerCode);
            var chatid = partner.SMSPlusUrl;



            request = serializer.Deserialize<OrderRequest>(transaction.RequestContent);
            //if (DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 58)
            //{
            //    return new APIResponse((int)ResponseCode.SystemMaintain);
            //}
            if (request.Type == "momocash")
            {
                request.BankName = "MOMO";
            }




            if (transaction.PartnerCode != "pp")
            {
                if (request.BankName.ToUpper() == "MOMO")
                {
                    //var provider = new Providers().GetCache("drummomo");
                    if (systemDataConfig.GetKey(lstConfig, "MomoCashEnable") == "0")
                        return new APIResponse((int)ResponseCode.SystemMaintain);

                }
                else
                {
                    //var provider = new Providers().GetCache("drumbank");
                    if (systemDataConfig.GetKey(lstConfig, "BankCashEnable") == "0")
                        return new APIResponse((int)ResponseCode.SystemMaintain);
                }

            }
            if (request.BankName == "MBB")
                request.BankName = "MB";
            if (request.BankName == "MBBANK")
                request.BankName = "MB";
            if (request.BankName == "AGR")
                request.BankName = "VBA";
            if (request.BankName == "AGB")
                request.BankName = "VBA";
            if (request.BankName == "AGRIBANK")
                request.BankName = "VBA";

            if (request.BankName == "SHIB")
                request.BankName = "SHBVN";

            if (request.BankName == "PVB")
                request.BankName = "PVCB";



            if (request.BankName == "VTB")
                request.BankName = "VIETINBANK";
            if (request.BankName == "VIETINBANK")
                request.BankName = "VIETINBANK";

            if (request.BankName == "ICB")
                request.BankName = "VIETINBANK";

            if (request.BankName == "PVB")
                request.BankName = "PVCB";
            if (request.BankName == "PVC")
                request.BankName = "PVCB";

            if (request.BankName == "DAB")
                request.BankName = "DOB";





            if (request.BankName == "Techcombank")
                request.BankName = "TCB";

            if (request.BankName == "EXB")
                request.BankName = "EIB";

            if (request.BankName == "LVPB")
                request.BankName = "LPB";

            if (request.BankName == "LIOBANK")
                request.BankName = "LIOB";

            if (request.BankName == "COB")
                request.BankName = "COOPBANK";

            if (request.BankName == "DOB")
                request.BankName = "VIKKI";

            if (request.BankName == "OJB")
                request.BankName = "MBV";



            if (request.BankName.ToLower() == "pvcombank")
                request.BankName = "PVCB";



            if (request.BankName.ToLower() == "mbbank")
                request.BankName = "MB";

            if (request.BankName.ToLower() == "vietcombank")
                request.BankName = "VCB";

            if (request.BankName == "NVB")
                request.BankName = "NCB";

            if (request.BankName == "MBV")
                request.BankName = "Oceanbank";
            if (request.BankName == "SEAB")
                request.BankName = "SEABANK";

            if (request.BankName == "SAB")
                request.BankName = "SEABANK";

            if (request.BankName == "ABB")
                request.BankName = "ABBANK";
            //check số dư
            request.BankAccountName = GlobalHelper.ReplaceVietnameseChar(request.BankAccountName);

            //var partner = new Partners().GetCache(transaction.PartnerCode);
            //var chatid = DrumBankLib.GetChatId(transaction.PartnerCode);
            //string chatid = DrumBankLib.GetChatId(transaction.PartnerCode);
            if (!string.IsNullOrEmpty(partner.Hotline))
            {
                var user = new Users().GetByUserName(partner.Hotline.Trim());
                if (user.Balance < request.Amount)
                {
                    if (string.IsNullOrEmpty(chatid))
                    {
                        TelegramNotify.SendTeleV2("-4882076202", "Đối tác " + transaction.PartnerCode + "  không đủ số dư để tạo lệnh bank out - " + request.RefCode);
                    }
                    else
                    {
                        if (partner.PartnerCode == "bp7")
                        {
                            TelegramNotify.SendTeleFast(chatid, "Số dư đã hết. Các anh nạp thêm tiền để user out nhé ");
                        }
                        else
                        {
                            TelegramNotify.SendTeleFast(chatid, "Đối tác " + transaction.PartnerCode + "  không đủ số dư để tạo lệnh bank out - " + request.RefCode);
                        }

                    }

                    return new APIResponse((int)ResponseCode.BalanceNotEnough);
                }


            }

            if (request.Amount >= 10000000)
            {
                if (request.Type == "momocash")
                {
                    TelegramNotify.SendTeleV2("-4775802643", "Có lệnh rút momo từ đối tác " + transaction.PartnerCode + ", số tiền " + request.Amount.ToString("#,#").Replace(",", "."));
                }
                else
                {
                    TelegramNotify.SendTeleV2("-4775802643", "Có lệnh rút bank từ đối tác " + transaction.PartnerCode + ", số tiền " + request.Amount.ToString("#,#").Replace(",", "."));
                }
            }

            APIResponse _APIResponse = new APIResponse();
            var _BankCashAPI = new BankCashAPI()
            {
                PartnerID = transaction.PartnerID,
                PartnerCode = transaction.PartnerCode,
                ProviderCode = transaction.ProviderCode,
                OrderNo = DrumBankLib.GenOrderCode(),
                OrderInfo = string.Empty,
                Amount = request.Amount,
                TotalAmount = request.Amount,
                Currency = "VND",
                ReturnUrl = request.CallbackUrl,
                RequestTime = 0,
                Signature = "",
                LogContent = "Add Order",
                Note = request.RefCode,
                BankCode = request.BankName,
                FullName = request.RefCode,
                Mobile = string.Empty,
                RefCode = request.RefCode,
                BankAccountName = request.BankAccountName,
                BankAccountNumber = request.BankAccountNumber
            };
            int step = 0;
            try
            {
                // Bước: Phân tích yêu cầu thành đối tượng
                step = 1;

                step = 1;
                if (transaction.PartnerCode != "pp")
                {

                    var MaxBankCashAmount = int.Parse(systemDataConfig.GetKey(lstConfig, "MaxBankCashAmount"));
                    var MinBankCashAmount = int.Parse(systemDataConfig.GetKey(lstConfig, "MinBankCashAmount"));
                    if (request.Amount < MinBankCashAmount || request.Amount > MaxBankCashAmount)
                    {
                        return new APIResponse((int)ResponseCode.BankAmountInvalid);


                    }
                }
                step = 2;

                _BankCashAPI.ReturnValue = _BankCashAPI.Add();
                // Nếu thêm giao dịch không hợp lệ
                if (_BankCashAPI.ReturnValue < 0)
                {
                    if (_BankCashAPI.ReturnValue == -99)
                    {
                        NLogLogger.Info(new string[] { "JavBank", transaction.TransactionID.ToString(), transaction.PartnerCode, "TopupRequest", request.BankAccountNumber, request.RefCode, "Error Insert Data" });
                        return new APIResponse((int)ResponseCode.TransactionIgnore);
                    }
                    return new APIResponse((int)_BankCashAPI.ReturnValue);
                }
                //trừ tiền luôn
                var ck = getck(transaction.PartnerCode, request.BankName.ToUpper());
                var Fee = Convert.ToInt64(Convert.ToInt64(_BankCashAPI.Amount) * (ck - 1));
                var Deductdes = String.Format("Trừ tiền rút bank số tiền: {2} mgd: {0}-{1}", _BankCashAPI.TransactionID, _BankCashAPI.BankCode + "-" + _BankCashAPI.RefCode, Convert.ToInt64(_BankCashAPI.Amount).ToString("#,#").Replace(",", "."));
                if (_BankCashAPI.BankCode == "MOMO")
                {
                    Deductdes = String.Format("Trừ tiền rút momo số tiền: {2} mgd: {0}-{1}", _BankCashAPI.TransactionID, _BankCashAPI.BankCode + "-" + _BankCashAPI.RefCode, Convert.ToInt64(_BankCashAPI.Amount).ToString("#,#").Replace(",", "."));
                }



                var resultdeduct = UpdatePartnerBalance(_BankCashAPI.PartnerCode, Convert.ToInt64(_BankCashAPI.Amount), Fee, Deductdes, "BankOut_" + _BankCashAPI.TransactionID.ToString());
                if (resultdeduct < 0)
                {
                    TelegramNotify.SendTeleV2("-4006848376", "Đối tác " + transaction.PartnerCode + "  không đủ số dư để tạo lệnh xuất khoản");
                    _BankCashAPI.Status = -1;
                    _BankCashAPI.LogContent = "Số dư không đủ";
                    _APIResponse.Description = "Không đủ số dư để thực hiện giao dịch";
                    _BankCashAPI.LastTime = DateTime.Now;

                    _BankCashAPI.Update();
                    return new APIResponse((int)ResponseCode.BalanceNotEnough);
                }
                // Bước: gọi hàm sang API
                step = 3;

                var MinBankAproveAmount = int.Parse(systemDataConfig.GetKey(lstConfig, "MinBankAproveAmount"));

                //var Partner = new Partners().GetCache(transaction.PartnerCode);
                if (!string.IsNullOrEmpty(partner.SMSPlusCommand))
                {
                    if (int.TryParse(partner.SMSPlusCommand, out int smsValue))
                    {
                        if (request.Amount >= smsValue)
                        {
                            //  var chatId = partner.SMSPlusUrl;
                            var mess =
                               "🏦 <b>ConfirmBankOut</b>\n\n" +
                               "📌 RefCode: <code>" + HttpUtility.HtmlEncode(request.RefCode) + "</code>\n" +
                               "💰 Amount: <b>" + request.Amount.ToString("#,#").Replace(",", ".") + "</b>\n" +
                               "🏛 Bank: " + request.BankName + "-" + request.BankAccountNumber + "-" + request.BankAccountName + "\n\n" +
                               "⚠️ Cần xác nhận giao dịch này.\n" +
                               "👉 Vui lòng bấm <b>Confirm</b> hoặc <b>Cancel</b> bên dưới.\n\n" +
                               "────────────────────\n" +
                               "🇬🇧 <b>Bankout Awaiting Confirmation</b>\n" +
                               "RefCode: <code>" + request.RefCode + "</code>\n" +
                               "Amount: <b>" + request.Amount.ToString("#,#").Replace(",", ".") + "</b>" + "\n" +
                               "Bank: " + request.BankName + "-" + request.BankAccountNumber + "-" + request.BankAccountName;


                            var mess2 =
                              "🏦 ConfirmBankOut\n\n" +
                             "📌 RefCode: " + request.RefCode + "\n" +
                             "💰 Amount: " + request.Amount.ToString("#,#").Replace(",", ".") + "\n\n" +
                             "🏛 Bank: " + request.BankName + "-" + request.BankAccountNumber + "-" + request.BankAccountName + "\n" +
                             "⚠️ Cần xác nhận giao dịch này.\n" +
                             "👉 Vui lòng bấm Confirm hoặc Cancel bên dưới.\n\n" +
                             "────────────────────\n" +
                             "🇬🇧 Bankout Awaiting Confirmation\n" +
                             "RefCode: " + request.RefCode + "\n" +
                             "Amount: " + request.Amount.ToString("#,#").Replace(",", ".") + "\n" +
                             "Bank: " + request.BankName + "-" + request.BankAccountNumber + "-" + request.BankAccountName;

                            try
                            {
                                TelegramNotify.SendConfirmMessage(chatid, mess, request.RefCode);
                            }
                            catch
                            {
                                TelegramNotify.SendConfirmMessage(chatid, mess2, request.RefCode);
                            }


                            _BankCashAPI.Status = -2;
                            _BankCashAPI.LogContent = "Chờ xác nhận";
                            _BankCashAPI.ApproveUser = "";
                            _BankCashAPI.LastTime = DateTime.Now;
                            _BankCashAPI.Update();
                            return new APIResponse(1);
                        }
                    }
                }

                if (request.Amount >= MinBankAproveAmount)
                {
                    TelegramNotify.SendTeleV2("-4762440012", "[Duyệt] Có lệnh bankout cần duyệt  " + request.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + transaction.PartnerCode + " RefCode " + request.RefCode);


                    _BankCashAPI.Status = -2;
                    _BankCashAPI.LogContent = "Chờ duyệt";
                    _BankCashAPI.ApproveUser = "";
                    _BankCashAPI.LastTime = DateTime.Now;
                    _BankCashAPI.Update();
                    return new APIResponse(1);

                }


                CashRespone cashResult = new CashRespone();
                CashRequest _cashRequest = new CashRequest();
                var signature = Utils.Encrypts.MD5(request.BankAccountNumber + request.Amount.ToString() + _BankCashAPI.ReturnValue.ToString() + pw);
                var urlService = $"{urlBaseService}Bank/ChargeOut?apiKey={secretKey}&bank_code={_BankCashAPI.BankCode}&amount={request.Amount}&bank_account={request.BankAccountNumber}&bank_accountName={request.BankAccountName}&requestId={_BankCashAPI.ReturnValue}&member_identity={_BankCashAPI.ReturnValue}&callback={callbackurl}&msg={_BankCashAPI.ReturnValue}&signature={signature}";

                NLogLogger.Info(new string[] { "Simex", "Cash Request", urlService });
                var response = Task.Run(async () => await DrumBankLib.GetTask(urlService)).Result;
                NLogLogger.Info(new string[] { "Simex", "Cash Response", response });

                var resObj = serializer.Deserialize<SimexBankLib.CashRespone>(response);
                if (resObj.stt == 1)
                {
                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                }
                else
                {

                    if (resObj.msg.Contains("không đủ tiền"))
                    {
                        TelegramNotify.SendTeleV2("-4882076202", "Provider Simex không đủ tiền");
                        var provider = new Providers().GetCache("simexcash");
                        provider.Status = -1;
                        provider.Update();

                    }

                    _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                    _BankCashAPI.Status = -1;
                    _BankCashAPI.LogContent = response;
                    _BankCashAPI.LastTime = DateTime.Now;
                    _BankCashAPI.Update();

                    //thất bại thì hoàn tiền
                    UpdatePartnerBalanceTopup(_BankCashAPI.PartnerCode, Convert.ToInt64(_BankCashAPI.Amount), Fee, String.Format("Hoàn tiền rút bank số tiền: {2} mgd: {0}-{1}", _BankCashAPI.TransactionID, _BankCashAPI.BankCode + "-" + _BankCashAPI.RefCode, Convert.ToInt64(_BankCashAPI.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _BankCashAPI.TransactionID.ToString());
                    //hoàn tiền



                }



            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "Simex", transaction.TransactionID.ToString(), "Error", "Cash", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "Simex", transaction.TransactionID.ToString(), "Error", "Cash", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "Simex", transaction.TransactionID.ToString(), "Error", "Cash", "Step3", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "Simex", transaction.TransactionID.ToString(), "Error", "Cash", "Step4", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "Simex", transaction.TransactionID.ToString(), "Error", "Cash", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }

                _BankCashAPI.Status = _APIResponse.ResponseCode;
            }


            return _APIResponse;


        }

        public APIResponse Callback(SimexBankLib.Callback callback)
        {


            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            var signature = Utils.Encrypts.MD5(secretKey + callback.requestId + callback.amount + callback.status + callback.bankcode);
            if (signature != callback.signature)
            {
                NLogLogger.Info(new string[] { "simex", "Callback", "Signature Failed", signature, callback.signature });
                return new APIResponse((int)ResponseCode.SignatureInvalid);
            }
            if (callback.status == "Success")
            {
                var order = new BankCashAPI().Get(long.Parse(callback.requestId));
                if (order == null)
                {
                    NLogLogger.Info(new string[] { "simex", "Callback", "Order NULL", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }
                if (order.Status == -1)
                {
                    NLogLogger.Info(new string[] { "simex", "Callback", "Order NULL", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }
                if (order.Status < 1)
                {
                    //apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
                    //{
                    //    ResponseContent = serializer.Serialize(new DataCallback()
                    //    {
                    //        RefCode = order.RefCode,
                    //        TransactionID = order.TransactionID.ToString(),
                    //        Amount = order.Amount,
                    //    })
                    //};
                    var ck = getck(order.PartnerCode, order.BankCode.ToUpper());
                    var rw = getrw(order.PartnerCode, order.BankCode.ToUpper());
                    order.Status = (int)ResponseCode.TransactionSuccessful;
                    order.TotalAmount = order.Amount;
                    order.LastTime = DateTime.Now;
                    order.Fee = Convert.ToInt64(Convert.ToInt64(order.Amount) * (ck - 1));
                    order.Reward = Convert.ToInt64(Convert.ToInt64(order.Amount) * (rw));
                    order.Mobile = "";
                    order.Update();

                    //var Amount = Math.Min(long.Parse(result.ResponseContent), request.AmountUser);


                }
                var checkOrder = new CheckOrder
                {
                    LasTime = DateTime.Now,
                    RefCode = order.RefCode,
                    Amount = Convert.ToInt32(order.Amount),
                    TransactionID = order.TransactionID.ToString()
                };
                DataCaching.SetCache("CheckOutOrder:" + order.PartnerCode + order.RefCode, checkOrder, 900);
                //Callback for Partner
                if (!string.IsNullOrEmpty(order.ReturnUrl))
                {
                    var partner = new Partners().Get(order.PartnerCode);
                    var datacb = new DataCallback()
                    {
                        RefCode = order.RefCode,
                        TransactionID = order.TransactionID.ToString(),
                        Amount = order.Amount,
                        OrderInfo = callback.requestId,
                        Type = "bankout"
                    };
                    apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
                    {

                    };
                    apiResponse.ResponseContent = serializer.Serialize(datacb);
                    apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                    //apiResponse.Signature = PaymentUtils.Signature(datacb.RefCode.ToString() + datacb.TransactionID + datacb.Amount, partner.PrivateKey, partner.SignatureType);
                    //Task.Run(async () => await DrumBankLib.CallbackJsonV2(order.ReturnUrl, serializer.Serialize(apiResponse), order.TransactionID, order.RefCode).ConfigureAwait(false));
                    //if (partner.PartnerID > 1284)
                    //{
                    //    var datacb2 = new DataCallbackV3();
                    //    datacb2.RefCode = datacb.RefCode;
                    //    datacb2.Amount = Convert.ToInt32(datacb.Amount);
                    //    datacb2.Type = datacb.Type;
                    //    datacb2.OrderInfo = datacb.OrderInfo;
                    //    datacb2.TransactionID = datacb.TransactionID;
                    //    datacb2.ResponseCode = apiResponse.ResponseCode;
                    //    datacb2.Description = apiResponse.Description;
                    //    datacb2.Signature = PaymentUtils.Signature(datacb2.ResponseCode.ToString() + datacb2.Description + datacb2.RefCode, partner.PrivateKey, partner.SignatureType);
                    //    Task.Run(async () => await DrumBankLib.CallbackJsonV2(order.ReturnUrl, serializer.Serialize(datacb2), order.TransactionID, order.RefCode).ConfigureAwait(false));
                    //}
                    //else
                    //{
                        Task.Run(async () => await DrumBankLib.CallbackJsonV2(order.ReturnUrl, serializer.Serialize(apiResponse), order.TransactionID, order.RefCode).ConfigureAwait(false));
                    //}
                }
            }
            else
            if (callback.status == "Failed" || callback.status == "Timeout")
            {

                var order = new BankCashAPI().Get(long.Parse(callback.requestId));
                if (order == null)
                {
                    NLogLogger.Info(new string[] { "Jav", "Callback", "Order NULL", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }
                if (order.Status == -1)
                {
                    NLogLogger.Info(new string[] { "Jav", "Callback", "Order fail", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }
                if (order.Status < 1)
                {

                    order.Status = -1;
                    order.TotalAmount = 0;
                    order.LastTime = DateTime.Now;
                    order.Mobile = "";
                    order.LogContent = callback.requestId;
                    var resultupdate = order.UpdateFail();

                    //var Amount = Math.Min(long.Parse(result.ResponseContent), request.AmountUser);
                    if (resultupdate < 0)
                        return apiResponse;

                    var ck = getck(order.PartnerCode, order.BankCode.ToUpper());
                    var Fee = Convert.ToInt64(Convert.ToInt64(order.Amount) * (ck - 1));
                    var toupdesc = String.Format("Hoàn tiền rút bank số tiền: {2} mgd: {0}-{1}", order.TransactionID, order.BankCode + "-" + order.RefCode, Convert.ToInt64(order.Amount).ToString("#,#").Replace(",", "."));
                    if (order.BankCode == "MOMO")
                    {
                        toupdesc = String.Format("Hoàn tiền rút momo số tiền: {2} mgd: {0}-{1}", order.TransactionID, order.BankCode + "-" + order.RefCode, Convert.ToInt64(order.Amount).ToString("#,#").Replace(",", "."));
                    }
                    if (order.PartnerCode == "cn02")
                    {
                        toupdesc = String.Format("Refund withdrawal amount: {2} mgd: {0}-{1}", order.TransactionID, order.BankCode + "-" + order.RefCode, Convert.ToInt64(order.Amount).ToString("#,#").Replace(",", "."));
                    }
                    UpdatePartnerBalanceTopup(order.PartnerCode, Convert.ToInt64(order.Amount), Fee, toupdesc, "BankOutRefund_" + order.TransactionID.ToString());


                    //Callback for Partner
                    if (!string.IsNullOrEmpty(order.ReturnUrl))
                    {
                        var partner = new Partners().Get(order.PartnerCode);
                        var datacb = new DataCallback()
                        {
                            RefCode = order.RefCode,
                            TransactionID = order.TransactionID.ToString(),
                            Amount = order.Amount,
                            OrderInfo = callback.requestId,
                            Type = "bankout"
                        };
                        apiResponse.ResponseContent = serializer.Serialize(datacb);
                        apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                        //datacb.Signature = PaymentUtils.Signature(datacb.RefCode.ToString() + datacb.TransactionID + datacb.Amount, partner.PrivateKey, partner.SignatureType);
                        //Task.Run(async () => await DrumBankLib.CallbackJsonV2(order.ReturnUrl, serializer.Serialize(apiResponse), order.TransactionID, order.RefCode).ConfigureAwait(false));
                        //if (partner.PartnerID > 1284)
                        //{
                        //    var datacb2 = new DataCallbackV3();
                        //    datacb2.RefCode = datacb.RefCode;
                        //    datacb2.Amount = Convert.ToInt32(datacb.Amount);
                        //    datacb2.Type = datacb.Type;
                        //    datacb2.OrderInfo = datacb.OrderInfo;
                        //    datacb2.TransactionID = datacb.TransactionID;
                        //    datacb2.ResponseCode = apiResponse.ResponseCode;
                        //    datacb2.Description = apiResponse.Description;
                        //    datacb2.Signature = PaymentUtils.Signature(datacb2.ResponseCode.ToString() + datacb2.Description + datacb2.RefCode, partner.PrivateKey, partner.SignatureType);
                        //    Task.Run(async () => await DrumBankLib.CallbackJsonV2(order.ReturnUrl, serializer.Serialize(datacb2), order.TransactionID, order.RefCode).ConfigureAwait(false));
                        //}
                        //else
                        //{
                            Task.Run(async () => await DrumBankLib.CallbackJsonV2(order.ReturnUrl, serializer.Serialize(apiResponse), order.TransactionID, order.RefCode).ConfigureAwait(false));
                        //}
                    }
                }

            }

            return apiResponse;
        }
        private decimal getck(string PartnerCode, string Type)
        {
            decimal ck = 1;
            //var partner = new Partners().GetCache(PartnerCode);
            var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, DateTime.Now.Year, DateTime.Now.Month);
            if (listpartnerDiscount == null)
            {
                //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return ck;
            }

            if (!listpartnerDiscount.Exists(x => x.Date.Day == DateTime.Now.Day))
            {
                //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return ck;
            }
            var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == DateTime.Now.Day);
            ck = _partnerDiscount.DiscountBANKOUTTRANFER;
            if (Type == "MOMO")
                ck = _partnerDiscount.DiscountMOMOOUT;
            return ck;
        }
        private decimal getrw(string PartnerCode, string Type)
        {
            decimal ck = 1;
            //var partner = new Partners().GetCache(PartnerCode);
            var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, DateTime.Now.Year, DateTime.Now.Month);
            if (listpartnerDiscount == null)
            {
                //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return ck;
            }

            if (!listpartnerDiscount.Exists(x => x.Date.Day == DateTime.Now.Day))
            {
                //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return ck;
            }
            var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == DateTime.Now.Day);
            ck = _partnerDiscount.RewardBANKOUTTRANFER;
            if (Type == "MOMO")
                ck = _partnerDiscount.RewardMOMOOUT;
            return ck;
        }
        private int UpdatePartnerBalance(string PartnerCode, long Amount, long Fee, string Note, string RefCode)
        {
            try
            {
                NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), Fee.ToString(), Note.ToString(), RefCode });
                var partner = new Partners().GetCache(PartnerCode);
                if (string.IsNullOrEmpty(partner.Hotline))
                {
                    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                    return 0;
                }
                var user = new Users().GetByUserName(partner.Hotline.Trim());
                if (user == null)
                {
                    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                    return 0;
                }




                long realAmount = Amount + Fee;
                //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
                return new Users().Deduct(realAmount, user.UserName, PartnerCode, Note, RefCode);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
                return -99;
            }


        }
        private int UpdatePartnerBalanceTopup(string PartnerCode, long Amount, long Fee, string TranId, string RefCode)
        {
            try
            {
                NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), Fee.ToString(), TranId.ToString(), RefCode });
                var partner = new Partners().GetCache(PartnerCode);
                if (string.IsNullOrEmpty(partner.Hotline))
                {
                    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                    return 0;
                }
                var user = new Users().GetByUserName(partner.Hotline.Trim());
                if (user == null)
                {
                    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                    return 0;
                }


                long realAmount = Amount + Fee;
                //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
                return new Users().Topup(realAmount, user.UserName, PartnerCode, TranId, RefCode);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
                return -99;
            }


        }

    }


}
