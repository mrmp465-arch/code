using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.BankCash.CoCo;
using Libs.BankCash.Drum;
using Libs.Report;
using Libs.Utils;
using static Libs.BankCash.BankCashService;

using static Libs.BankCash.Drum.DrumBankLib;


namespace Libs.BankCash.DrumV2
{
    public class DrumV2Bank : IBankCashHandler
    {
        // Production MoBo
        private const string urlBaseService = "http://127.0.0.1:9002/MomoService.ashx";
        private const string urlBaseService2 = "http://127.0.0.1:9002/BankService.ashx";
        private const string callbackurl = "http://127.0.0.1:1592/Callback/DrumMomoCash.ashx";
        private const string callbackbankurl = "http://127.0.0.1:1592/Callback/DrumBankCash.ashx";
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        public APIResponse ReCallBack(long id)
        {
            throw new NotImplementedException();
        }
        public APIResponse Check(APITransaction transaction)
        {


            CheckAccountRequest request = new CheckAccountRequest();
            request = serializer.Deserialize<CheckAccountRequest>(transaction.RequestContent);

            APIResponse _APIResponse = new APIResponse();
            if (request.Type == "momocash")
            {
                request.BankAccountNumber = request.BankAccountNumber.TrimEnd();
                if (!CheckValidMobile(request.BankAccountNumber))
                    return new APIResponse((int)ResponseCode.ParameterInvalid);
                var signature = "";
                var requestData = new RequestData()
                {
                    PartnerCode = "",
                    CommandCode = "ACCOUNT_INQUIRY",
                    RequestContent = request.BankAccountNumber,
                    Signature = signature
                };
                NLogLogger.Info(new string[] { "MDrum", "Cash Request",serializer.Serialize(requestData), urlBaseService
                    });
                var response = Task.Run(async () => await PostTask(urlBaseService, serializer.Serialize(requestData))).Result;
                NLogLogger.Info(new string[] { "MDrum", "Cash Response", response, urlBaseService
                     });
                var resObj = serializer.Deserialize<DrumBankLib.CashRespone>(response);
                if (resObj.ResponseCode == 1)
                {
                    var userinfo = serializer.Deserialize<DrumBankLib.UserInfo>(resObj.ResponseContent);
                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    _APIResponse.ResponseContent = userinfo.name;

                }
                else
                {
                    _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                    _APIResponse.Description = resObj.Description;
                }
                return _APIResponse;
            }


            return new APIResponse((int)ResponseCode.TransactionFailed);
        }

        public APIResponse Cash(APITransaction transaction)
        {
            var systemDataConfig = new SystemDataConfig();
            var lstConfig = systemDataConfig.GetListCache();

            //var chatid = DrumBankLib.GetChatId(transaction.PartnerCode);
            var partner = new Partners().GetCache(transaction.PartnerCode);
            var chatid = partner.SMSPlusUrl;


            //if (transaction.PartnerCode == "hyn1" || transaction.PartnerCode == "hng"|| transaction.PartnerCode == "hen")
            //     return new APIResponse((int)ResponseCode.SystemBusy);


            // return new APIResponse((int)ResponseCode.SystemBusy);
            //DrumaScriptSerializer serializer = new DrumaScriptSerializer();
            OrderRequest request = new OrderRequest();


            request = serializer.Deserialize<OrderRequest>(transaction.RequestContent);
            //if (DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 58 && transaction.PartnerCode != "cn02")
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
                    //if (systemDataConfig.GetKey(lstConfig, "MomoCashEnable") == "0")
                    //    return new APIResponse((int)ResponseCode.SystemMaintain);

                }
                else
                {
                    //var provider = new Providers().GetCache("drumbank");
                    if (systemDataConfig.GetKey(lstConfig, "BankCashEnable") == "0")
                    {
                        if (!string.IsNullOrEmpty(chatid))
                        {
                            TelegramNotify.SendTeleV2(chatid, "[bankout]  " + request.RefCode + " => Hệ thống bảo trì | System Maintain | 维护系统");
                        }
                        return new APIResponse((int)ResponseCode.SystemMaintain);
                    }
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

            if (request.BankName == "SAB")
                request.BankName = "SEAB";

            if (request.BankName == "VTB")
                request.BankName = "ICB";
            if (request.BankName == "VIETINBANK")
                request.BankName = "ICB";

            if (request.BankName == "PVB")
                request.BankName = "PVCB";
            if (request.BankName == "PVC")
                request.BankName = "PVCB";

            if (request.BankName == "DAB")
                request.BankName = "DOB";

            if (request.BankName == "ABBANK")
                request.BankName = "ABB";

            if (request.BankName == "SEABANK")
                request.BankName = "SEAB";

            if (request.BankName == "Techcombank")
                request.BankName = "TCB";

            if (request.BankName == "EXB")
                request.BankName = "EIB";

            if (request.BankName == "LVPB")
                request.BankName = "LPB";

            if (request.BankName == "NCB")
                request.BankName = "NVB";

            if (request.BankName == "LIOBANK")
                request.BankName = "LIOB";

            if (request.BankName == "COB")
                request.BankName = "COOPBANK";

            if (request.BankName == "DOB")
                request.BankName = "VIKKI";

            if (request.BankName == "OJB")
                request.BankName = "MBV";

            if (request.BankName == "OceanBank")
                request.BankName = "MBV";


            if (request.BankName.ToLower() == "pvcombank")
                request.BankName = "PVCB";

            if (request.BankName.ToLower() == "vietinbank")
                request.BankName = "ICB";

            if (request.BankName.ToLower() == "mbbank")
                request.BankName = "MB";

            if (request.BankName.ToLower() == "vietcombank")
                request.BankName = "VCB";

            //if (request.BankName.Length > 10)
            //    return new APIResponse((int)ResponseCode.BankCodeInvalid);
            request.BankAccountName = GlobalHelper.ReplaceVietnameseChar(request.BankAccountName);
           

            //
            if (request.Type == "momocash")
            {
                //if (!GlobalHelper.CheckUnicode(request.BankAccountName))
                //    return new APIResponse((int)ResponseCode.BankAccountInvalid);
            }
            else
            {
                //if (string.IsNullOrEmpty(request.BankName))
                //    return new APIResponse((int)ResponseCode.BankCodeInvalid);
                if (request.BankAccountNumber.Length < 5 || request.BankAccountNumber.Length > 25)
                {
                    return new APIResponse((int)ResponseCode.BankAccountInvalid);
                }
                if (!string.IsNullOrEmpty(request.BankAccountName))
                {
                    if (!GlobalHelper.CheckUnicode(request.BankAccountName))
                    {
                        request.BankAccountName = GlobalHelper.ReplaceVietnameseChar(request.BankAccountName);
                    }
                    //return new APIResponse((int)ResponseCode.BankAccountInvalid);
                }

                //if(request.BankName == "UNDEFINED")

                //{
                //    return new APIResponse((int)ResponseCode.BankCodeInvalid);
                //}
                var lstbankcode = new BankCodeTranfer().GetListCache();

                //var lstbankcode = "SHBVN,SEAB,ICB,PVCB,VCB,ICB,BIDV,TCB,VBA,STB,ACB,MB,TPB,SHBVN,VIB,VPB,SHB,OCB,EIB,VCCB,SCB,VRB,ABB,PVB,OJB,NAB,HDB,VB,PB,HLB,PGB,COB,CIMB,NCB,IDB,DAB,GPB,BAB,VAB,SGB,MSB,LVPB,KLB,IBK,WRB,SEAB,UOB,DOB";
                if (!lstbankcode.Exists(x => x.code.ToUpper() == request.BankName.ToUpper()))
                {
                    if (string.IsNullOrEmpty(chatid))
                    {
                        TelegramNotify.SendTeleV2("-4882076202", "[" + transaction.PartnerCode + "]: lệnh rút  " + request.RefCode + " " + request.BankName + " => sai mã ngân hàng");
                    }
                    else
                    {
                        TelegramNotify.SendTeleFast(chatid, "[" + transaction.PartnerCode + "]: lệnh rút  " + request.RefCode + " " + request.BankName + " => sai mã ngân hàng");
                    }

                    return new APIResponse((int)ResponseCode.BankCodeInvalid);
                }
                else
                {
                    if (lstbankcode.Exists(x => x.code.ToUpper() == request.BankName.ToUpper() && x.isTransfer == 0))
                    {
                        if (string.IsNullOrEmpty(chatid))
                        {
                            TelegramNotify.SendTeleV2("-4882076202", "[" + transaction.PartnerCode + "]: lệnh rút  " + request.RefCode + " " + request.BankName + "=> mã ngân hàng nhận bảo trì ");
                        }
                        else
                        {
                            TelegramNotify.SendTeleFast(chatid, "[" + transaction.PartnerCode + "]: lệnh rút  " + request.RefCode + " " + request.BankName + "=> mã ngân hàng nhận bảo trì ");
                        }

                        return new APIResponse((int)ResponseCode.BankCodeMaintain);
                    }
                }

            }
            //check số dư
            //check tk
            var blockacount = systemDataConfig.GetKey(lstConfig, "BlockAccount");
            //check tk
            if (blockacount.Contains(request.BankAccountNumber))
            {

                return new APIResponse((int)ResponseCode.BankAccountInvalid);
            }
            //var partner = new Partners().GetCache(transaction.PartnerCode);
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
                LogContent = "Add Order - " + IPAddress.Get(),
                Note = "",
                ApproveUser = "auto",
                BankCode = request.BankName.ToUpper(),
                FullName = request.RefCode,
                Mobile = string.Empty,
                RefCode = request.RefCode,
                BankAccountName = request.BankAccountName,
                BankAccountNumber = request.BankAccountNumber
            };
            if (transaction.PartnerCode == "cn02" || transaction.PartnerCode == "pp")
            {
                _BankCashAPI.Note = request.Note;
            }
            int step = 0;
            try
            {
                // Bước: Phân tích yêu cầu thành đối tượng
                step = 1;
                if (transaction.PartnerCode != "pp")
                {
                    if (request.Type == "momocash")
                    {
                        var MaxMomoCashAmount = int.Parse(systemDataConfig.GetKey(lstConfig, "MaxMomoCashAmount"));
                        var MinMomoCashAmount = int.Parse(systemDataConfig.GetKey(lstConfig, "MinMomoCashAmount"));
                        if (request.Amount < MinMomoCashAmount || request.Amount > MaxMomoCashAmount)
                        {
                            return new APIResponse((int)ResponseCode.BankAmountInvalid);
                        }
                        request.BankAccountNumber = request.BankAccountNumber.TrimEnd();
                        if (!CheckValidMobile(request.BankAccountNumber))
                            return new APIResponse((int)ResponseCode.ParameterInvalid);
                    }
                    else
                    {
                        var MaxBankCashAmount = int.Parse(systemDataConfig.GetKey(lstConfig, "MaxBankCashAmount"));
                        var MinBankCashAmount = int.Parse(systemDataConfig.GetKey(lstConfig, "MinBankCashAmount"));
                        if (request.Amount < MinBankCashAmount || request.Amount > MaxBankCashAmount)
                        {
                            return new APIResponse((int)ResponseCode.BankAmountInvalid);


                        }

                    }
                }


                // Bước: Ghi log giao dịch
                step = 2;

                _BankCashAPI.ReturnValue = _BankCashAPI.Add();

                // Nếu thêm giao dịch không hợp lệ
                if (_BankCashAPI.ReturnValue < 0)
                {
                    if (_BankCashAPI.ReturnValue == -99)
                    {
                        NLogLogger.Info(new string[] { "DrumBank", transaction.TransactionID.ToString(), transaction.PartnerCode, "TopupRequest", request.BankAccountNumber, request.RefCode, "Error Insert Data" });
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
                if (transaction.PartnerCode == "cn02")
                {
                    Deductdes = String.Format("Deduct cash amount: {2} transId: {0}-{1}", _BankCashAPI.TransactionID, _BankCashAPI.BankCode + "-" + _BankCashAPI.RefCode, Convert.ToInt64(_BankCashAPI.Amount).ToString("#,#").Replace(",", "."));

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
                //check số dư
                //if (request.Amount >= 3000000 && request.Amount <= 25000000)
                //{
                //    try
                //    {
                //        var _Bank = new BankAccounts();
                //        var data = _Bank.GetList().OrderBy(x => x.Id).ToList();
                //        long Balance = data.Where(x => x.Type.Contains("OUT") && x.Status == 1 && x.StatusExtra != -4).Sum(a => a.BalanceTotal);
                //        //NLogLogger.Info("Bankout Ballance "+ Balance.ToString());
                //        if (Balance < 20000000)
                //        {
                //            TelegramNotify.SendTeleV2("-4762440012", "Sô dư bank out dưới 20tr => anh em bơm tiền vào nhé  !!!! ");

                //        }
                //    }
                //    catch
                //    {

                //    }

                //}

                // Bước: gọi hàm sang API
                step = 3;

                var MinBankAproveAmount = int.Parse(systemDataConfig.GetKey(lstConfig, "MinBankAproveAmount"));

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
             
                //var Partner = new Partners().GetCache(transaction.PartnerCode);
     

                if (request.Amount >= MinBankAproveAmount ||  request.BankAccountNumber == "123456789")
                {
                    TelegramNotify.SendTeleV2("-4762440012", "[Duyệt] Có lệnh bankout cần duyệt  " + request.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + transaction.PartnerCode + " RefCode " + request.RefCode);


                    _BankCashAPI.Status = -2;
                    _BankCashAPI.LogContent = "Chờ duyệt";
                    _BankCashAPI.ApproveUser = "";
                    _BankCashAPI.LastTime = DateTime.Now;
                    _BankCashAPI.Update();
                    return new APIResponse(1);

                }
               
                if (request.Type == "momocash")
                {
                    CashBankRequest _cashRequest = new CashBankRequest();

                    _cashRequest.MomoName = request.BankAccountName;
                    _cashRequest.MomoId = request.BankAccountNumber;
                    _cashRequest.Note = _BankCashAPI.ReturnValue.ToString();
                    _cashRequest.Amount = request.Amount;
                    _cashRequest.CallbackUrl = callbackurl;
                    _cashRequest.TransId = _BankCashAPI.ReturnValue.ToString();

                    var signature = "";
                    var requestData = new RequestData()
                    {
                        PartnerCode = "order",
                        CommandCode = "CASHOUT",
                        Source = transaction.PartnerCode,
                        RequestContent = serializer.Serialize(_cashRequest),
                        Signature = signature
                    };
                    //var partner = new Partners().Get(transaction.PartnerCode);
                    //if (string.IsNullOrEmpty(partner.SMSPlusUrl))
                    //{
                    //    requestData.PartnerCode = "order";
                    //}
                    CashRespone cashResult = new CashRespone();

                    NLogLogger.Info(new string[] { "MDrum", "Cash Request",serializer.Serialize(requestData), urlBaseService
                    });
                    var response = Task.Run(async () => await PostTask(urlBaseService, serializer.Serialize(requestData))).Result;
                    NLogLogger.Info(new string[] { "MDrum", "Cash Response", response, urlBaseService
                     });



                    try
                    {
                        var resObj = serializer.Deserialize<DrumBankLib.CashRespone>(response);
                        if (resObj.ResponseCode == 1)
                        {

                            _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                        }
                        else
                        {
                            _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                            _BankCashAPI.Status = -1;
                            _BankCashAPI.LogContent = resObj.Description;
                            _APIResponse.Description = resObj.Description;
                            _BankCashAPI.LastTime = DateTime.Now;

                            _BankCashAPI.Update();

                            //}
                            //thất bại thì hoàn tiền

                            if (transaction.PartnerCode == "cn02")
                            {
                                UpdatePartnerBalanceTopup(_BankCashAPI.PartnerCode, Convert.ToInt64(_BankCashAPI.Amount), Fee, String.Format("Refund withdrawal amount: {2} trasnId: {0}-{1}", _BankCashAPI.TransactionID, _BankCashAPI.BankCode + "-" + _BankCashAPI.RefCode, Convert.ToInt64(_BankCashAPI.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _BankCashAPI.ReturnValue.ToString());
                            }
                            else
                            {
                                UpdatePartnerBalanceTopup(_BankCashAPI.PartnerCode, Convert.ToInt64(_BankCashAPI.Amount), Fee, String.Format("Hoàn tiền rút momo số tiền: {2} mgd: {0}-{1}", _BankCashAPI.TransactionID, _BankCashAPI.BankCode + "-" + _BankCashAPI.RefCode, Convert.ToInt64(_BankCashAPI.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _BankCashAPI.ReturnValue.ToString());
                                //if (resObj.Description.ToLower().Contains("system") || resObj.Description.ToLower().Contains("fail"))
                            }

                            //{
                            //    TelegramNotify.SendTeleV2("-4006848376", "MOMOOUT :" + resObj.Description);
                            //}
                            var log = new LogInfo
                            {
                                LogTime = DateTime.Now,
                                Url = "",
                                TransactionID = _BankCashAPI.ReturnValue,
                                Request = "Repone fail",
                                Respone = ""
                            };
                            LogCache.LogBankCash(log);
                        }
                    }
                    catch
                    {
                        //_APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        _BankCashAPI.Status = -3;

                        _BankCashAPI.LastTime = DateTime.Now;
                        _BankCashAPI.Update();

                        //UpdatePartnerBalanceTopup(_BankCashAPI.PartnerCode, Convert.ToInt64(_BankCashAPI.Amount), Fee, String.Format("Hoàn tiền rút momo số tiền: {2} mgd: {0}-{1}", _BankCashAPI.TransactionID, _BankCashAPI.BankCode + "-" + _BankCashAPI.RefCode, Convert.ToInt64(_BankCashAPI.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _BankCashAPI.ReturnValue.ToString());

                        //var log = new LogInfo
                        //{
                        //    LogTime = DateTime.Now,
                        //    Url = "",
                        //    TransactionID = _BankCashAPI.ReturnValue,
                        //    Request = "Repone fail",
                        //    Respone = ""
                        //};
                        //LogCache.LogBankCash(log);
                    }


                }
                else
                {
                    CashBankRequestV2 _cashRequest = new CashBankRequestV2();

                    //if (transaction.PartnerCode == "sn1")
                    //{
                    //    _cashRequest.BankName = "NOCHECK";
                    //}

                    _cashRequest.BankName = request.BankAccountName;
                    _cashRequest.BankId = request.BankAccountNumber;
                    _cashRequest.BankCode = request.BankName;
                    _cashRequest.Comment = "";
                    if (transaction.PartnerCode == "cn02" || transaction.PartnerCode == "pp")
                    {
                        _cashRequest.Comment = _BankCashAPI.Note;
                        if (_BankCashAPI.Note == _BankCashAPI.RefCode)
                        {
                            _cashRequest.Comment = "";
                        }
                    }
                    _cashRequest.Amount = request.Amount;
                    _cashRequest.CallbackUrl = callbackbankurl;
                    _cashRequest.TransId = _BankCashAPI.ReturnValue.ToString();


                    var signature = "";

                    var requestData = new RequestData()
                    {
                        PartnerCode = "order",
                        CommandCode = "TRANS_OUT",
                        Source = transaction.PartnerCode,
                        RequestContent = serializer.Serialize(_cashRequest),
                        Signature = signature
                    };
                    //if (transaction.PartnerCode == "pp")
                    //{
                    //    requestData.Source = "tiger02";
                    //}
                    if (transaction.PartnerCode == "cn02" || transaction.PartnerCode == "pp")
                    {
                        _cashRequest.BankName = "NOCHECK";
                    }
                    //var partner = new Partners().Get(transaction.PartnerCode);
                    //if (string.IsNullOrEmpty(partner.SMSPlusUrl))
                    //{
                    //    requestData.PartnerCode = "order";
                    //}
                    CashRespone cashResult = new CashRespone();

                    NLogLogger.Info(new string[] { "MDrum", "Cash Request",serializer.Serialize(requestData), urlBaseService2, MinBankAproveAmount.ToString()
                     });
                    var response = Task.Run(async () => await PostTask(urlBaseService2, serializer.Serialize(requestData))).Result;
                    NLogLogger.Info(new string[] { "MDrum", "Cash Response", response, urlBaseService2
                    });

                    try
                    {
                        var resObj = serializer.Deserialize<DrumBankLib.CashRespone>(response);
                        if (resObj.ResponseCode == 1)
                        {

                            _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);

                        }
                        else
                        {
                            //duyệt tay
                            if (resObj.Description.Contains("busy") || resObj.Description.Contains("đủ điều kiện"))
                            {
                                _BankCashAPI.Status = -2;
                                _BankCashAPI.LogContent = "Chờ duyệt";
                                _BankCashAPI.LastTime = DateTime.Now;
                                _BankCashAPI.Update();


                                TelegramNotify.SendTeleV2("-4762440012", "[Duyệt] Có lệnh bankout cần duyệt  " + request.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + transaction.PartnerCode + " RefCode " + request.RefCode);
                                System.Threading.Thread.Sleep(800);
                                if (resObj.Description.Contains("All account") || resObj.Description.Contains("đủ điều kiện"))
                                {
                                    TelegramNotify.SendTeleV2("-4762440012", "Tài khoản bank out không đủ tiền=> Vui lòng duyệt tay lệnh này");
                                }
                                return new APIResponse(1);
                            }

                            _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                            _BankCashAPI.Status = -1;
                            _BankCashAPI.LogContent = resObj.Description;
                            _APIResponse.Description = resObj.Description;
                            _BankCashAPI.LastTime = DateTime.Now;
                            _BankCashAPI.Update();
                            if (resObj.Description.ToLower().Contains("system") || resObj.Description.ToLower().Contains("fail"))
                            {
                                TelegramNotify.SendTeleV2("-4006848376", "BANKOUT :" + resObj.Description);
                            }
                            //thất bại thì hoàn tiền
                            if (transaction.PartnerCode == "cn02")
                            {
                                UpdatePartnerBalanceTopup(_BankCashAPI.PartnerCode, Convert.ToInt64(_BankCashAPI.Amount), Fee, String.Format("Refund withdrawal amount: {2} transId: {0}-{1}", _BankCashAPI.TransactionID, _BankCashAPI.BankCode + "-" + _BankCashAPI.RefCode, Convert.ToInt64(_BankCashAPI.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _BankCashAPI.TransactionID.ToString());
                                //hoàn tiền
                            }
                            else
                            {
                                UpdatePartnerBalanceTopup(_BankCashAPI.PartnerCode, Convert.ToInt64(_BankCashAPI.Amount), Fee, String.Format("Hoàn tiền rút bank số tiền: {2} mgd: {0}-{1}", _BankCashAPI.TransactionID, _BankCashAPI.BankCode + "-" + _BankCashAPI.RefCode, Convert.ToInt64(_BankCashAPI.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _BankCashAPI.TransactionID.ToString());
                                //hoàn tiền
                            }

                            //hoàn tiền
                            var log = new LogInfo
                            {
                                LogTime = DateTime.Now,
                                Url = "",
                                TransactionID = _BankCashAPI.ReturnValue,
                                Request = "Repone fail",
                                Respone = ""
                            };
                            LogCache.LogBankCash(log);

                        }
                    }

                    catch
                    {
                        //_APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        _BankCashAPI.Status = -3;
                        _BankCashAPI.LogContent = "suspicious";
                        _BankCashAPI.LastTime = DateTime.Now;
                        _BankCashAPI.Update();

                        systemDataConfig.Update("BankCashEnable", "0");
                        systemDataConfig.DeleteCache();
                        TelegramNotify.SendTeleV2("-4882076202", "Nghi vấn lỗi TCP !!!!! ");
                        return new APIResponse(1);
                        ////if (resObj.Description.ToLower().Contains("system") || resObj.Description.ToLower().Contains("fail"))
                        ////{
                        ////    TelegramNotify.SendTeleV2("-4006848376", "BANKOUT :" + resObj.Description);
                        ////}
                        //UpdatePartnerBalanceTopup(_BankCashAPI.PartnerCode, Convert.ToInt64(_BankCashAPI.Amount), Fee, String.Format("Hoàn tiền rút bank số tiền: {2} mgd: {0}-{1}", _BankCashAPI.TransactionID, _BankCashAPI.BankCode + "-" + _BankCashAPI.RefCode, Convert.ToInt64(_BankCashAPI.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _BankCashAPI.ReturnValue.ToString());
                        //var log = new LogInfo
                        //{
                        //    LogTime = DateTime.Now,
                        //    Url = "",
                        //    TransactionID = _BankCashAPI.ReturnValue,
                        //    Request = "Repone fail",
                        //    Respone = ""
                        //};
                        //LogCache.LogBankCash(log);
                    }


                }


            }
            catch (Exception ex)
            {
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "DrumBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "DrumBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "DrumBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step3", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.TransactionFailed);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "DrumBank", transaction.TransactionID.ToString(), "Error", "Cash", "Step4", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "DrumBank", transaction.TransactionID.ToString(), "Error", "Cash", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }

                _BankCashAPI.Status = _APIResponse.ResponseCode;
            }


            return _APIResponse;


        }

        public APIResponse Callback(DrumBankLib.BankResponse callback)
        {


            APIResponse apiResponse = new APIResponse((int)ResponseCode.TransactionFailed);

            //var signature = Utils.Encrypts.MD5(callback.chargeId + callback.chargeType + callback.chargeCode + callback.chargeAmount + callback.status + callback.requestId + pw);
            //if (signature != callback.signature)
            //{
            //    NLogLogger.Info(new string[] { "VNPAY", "Callback", "Signature Failed", signature, callback.signature });
            //    return new APIResponse((int)ResponseCode.SignatureInvalid);
            //}
            var newclObj = serializer.Deserialize<DrumBankLib.Callback>(callback.ResponseContent);
            long transId = long.Parse(newclObj.transId.Replace("_re2call", "").Replace("_recall", ""));
            if (callback.ResponseCode > 0)
            {

                var order = new BankCashAPI().Get(transId);
                if (order == null)
                {
                    NLogLogger.Info(new string[] { "Drum", "Callback", "Order NULL", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }
                if (order.Status == 0)
                {
                    //bắn tele
                    //if (order.PartnerCode == "c11")
                    //{
                    //    //var mess = $"<b>Tài khoản</b>: ({order.BankCode}) {order.BankAccountNumber} -  {order.BankAccountName} %0A<b>Mã giao dịch</b>: {newclObj.BankTransId} -  {order.RefCode} %0A<b>Số tiền</b>: -{newclObj.Amount.ToString("#,#").Replace(",", ".")} %0A<b>Thời gian</b>: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}";
                    //    //TelegramNotify.SendTeleV4("-4672443962", mess);
                    //    TelegramNotify.SendTeleV2("-5097341935", "Lệnh rút " + order.RefCode + " , số tiền: " + newclObj.Amount.ToString("#,#").Replace(",", ".") + ", tk nhận: " + order.BankCode + " - " + order.BankAccountNumber + " - " + order.BankAccountName);
                    //}
                    var ck = getck(order.PartnerCode, order.BankCode.ToUpper());
                    var rw = getrw(order.PartnerCode, order.BankCode.ToUpper());


                    if (string.IsNullOrEmpty(newclObj.MomoTransId))
                        newclObj.MomoTransId = newclObj.BankTransId;
                    order.Status = (int)ResponseCode.TransactionSuccessful;
                    order.TotalAmount = order.Amount;
                    order.LastTime = DateTime.Now;
                    order.OrderInfo = newclObj.MomoTransId;
                    order.Mobile = string.Format("{0} - {1} - {2}", newclObj.PartnerBankCode, newclObj.PartnerBankId, newclObj.PartnerBankName);
                    order.Note = GlobalHelper.ReplaceVietnameseChar(newclObj.Comment);
                    order.Fee = Convert.ToInt64(Convert.ToInt64(order.Amount) * (ck - 1));
                    order.Reward = Convert.ToInt64(Convert.ToInt64(order.Amount) * (rw ));
                    order.LogContent = "core update";

                    if (!string.IsNullOrEmpty(newclObj.BankName))
                        order.BankAccountName = newclObj.BankName;

                    if (order.BankCode == "MOMO")
                    {
                        order.Note = GlobalHelper.ReplaceVietnameseChar(newclObj.Note);
                        order.Mobile = newclObj.MomoPartnerId;
                    }
                    if (order.BankCode.ToLower() == "momo" && order.PartnerCode.StartsWith("hai"))
                    {
                        try
                        {
                            var check = new MomoTransaction().GetByRefcode(newclObj.transId);
                            if (check != null)
                            {
                                order.BankAccountName = check.Description.Replace(" đã nhận tiền qua MoMo", "").Trim();
                            }
                        }
                        catch
                        {

                        }


                    }
                    order.Update();

                    //var Amount = Math.Min(long.Parse(result.ResponseContent), request.AmountUser);
                    //Callback for Partner

                    //Action<string, long, string, string> send = UpdatePartnerBalance;
                    //var asynSend = send.BeginInvoke(order.PartnerCode, Convert.ToInt64(order.Amount), order.BankCode, String.Format("Trừ tiền rút bank số tiền: {2} mgd: {0}-{1}", order.TransactionID,  order.BankCode, Convert.ToInt64(order.Amount).ToString("#,#").Replace(",", ".")), null, null);
                    var checkOrder = new CheckOrder
                    {
                        LasTime = DateTime.Now,
                        RefCode = order.RefCode,
                        Amount = Convert.ToInt32(order.Amount),
                        TransactionID = order.TransactionID.ToString()
                    };
                    DataCaching.SetCache("CheckOutOrder:" + order.PartnerCode + order.RefCode, checkOrder, 900);
                    if (!string.IsNullOrEmpty(order.ReturnUrl))
                    {
                        //if (order.PartnerCode == "cn02")
                        //{

                        //}
                        var partner = new Partners().GetCache(order.PartnerCode);
                        var datacb = new DataCallback()
                        {
                            RefCode = order.RefCode,
                            TransactionID = order.TransactionID.ToString(),
                            Amount = order.Amount,
                            MomoTransId = newclObj.MomoTransId,
                            OrderInfo = newclObj.MomoTransId,
                            Mobile = order.Mobile,
                            Type = "bankout"
                        };
                        if (order.BankCode.ToLower() == "momo")
                            datacb.Type = "momoout";
                        if (order.BankCode.ToLower() == "momo" && order.PartnerCode.StartsWith("hai"))
                        {
                            datacb.MomoName = order.BankAccountName;
                        }
                        apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
                        {

                        };
                        apiResponse.ResponseContent = serializer.Serialize(datacb);
                        apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                        //apiResponse.Signature = PaymentUtils.Signature(datacb.RefCode.ToString() + datacb.TransactionID + datacb.Amount, partner.PrivateKey, partner.SignatureType);



                        //if (partner.PartnerID>1283)
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
            if (callback.ResponseCode < 0)
            {
                var order = new BankCashAPI().Get(transId);
                if (order == null)
                {
                    NLogLogger.Info(new string[] { "Drum", "Callback", "Order NULL", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }
                if (order.Status == -1)
                {
                    NLogLogger.Info(new string[] { "Drum", "Callback", "Order fail", serializer.Serialize(callback) });
                    var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                    return apiResponse;
                }
                //duyệt tay
                if ((callback.Description.Contains("busy") || callback.Description.Contains("Bank Code Invalid")) && order.BankCode != "MOMO")
                {

                    order.Status = -2;
                    order.LogContent = "Chờ duyệt";
                    order.LastTime = DateTime.Now;
                    order.Update();
                    var mess = "[Duyệt] Có lệnh bankout cần duyệt  " + order.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + order.PartnerCode + " RefCode " + order.RefCode;

                    if (newclObj.transId.Contains("recall"))
                        mess = "[Duyệt lần 2] Có lệnh bankout cần duyệt  " + order.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + order.PartnerCode + " RefCode " + order.RefCode;

                    if (newclObj.transId.Contains("re2call"))
                        mess = "[Duyệt lần 3] Có lệnh bankout cần duyệt tay " + order.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + order.PartnerCode + " RefCode " + order.RefCode;
                    TelegramNotify.SendTeleV2("-4762440012", mess);

                    return new APIResponse(1);
                }
                //if ((callback.Description.Contains("không trùng ") || callback.Description.Contains("tìm thấy")) && order.BankCode != "MOMO")
                //{
                //    if (order.PartnerCode == "go99" || order.PartnerCode == "nohu888" || order.PartnerCode == "hn002" || order.PartnerCode.Contains("shdsn"))
                //    {
                //        //string chatid = DrumBankLib.GetChatId(order.PartnerCode);
                //        TelegramNotify.SendTeleFast(chatid, "[" + order.PartnerCode + "]: lệnh xuất khoản  " + order.RefCode + " thất bại do sai tên tài khoản hoặc không truy vấn được tài khoản=> game update giúp nhé");
                //    }
                //}
                //pending va  thì cần duyệt tay
                if (callback.ResponseCode == -8)
                {
                    if (order.Status == 0)
                    {
                        order.Status = -2;
                        order.LogContent = "Chờ duyệt";
                        order.LastTime = DateTime.Now;
                        order.Note = newclObj.Comment;
                        order.Update();

                        var mess = "[Duyệt] Có lệnh bankout cần duyệt  " + order.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + order.PartnerCode + " RefCode " + order.RefCode;

                        if (newclObj.transId.Contains("recall"))
                            mess = "[Duyệt lần 2] Có lệnh bankout cần duyệt  " + order.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + order.PartnerCode + " RefCode " + order.RefCode;

                        if (newclObj.transId.Contains("re2call"))
                            mess = "[Duyệt lần 3] Có lệnh bankout cần duyệt tay " + order.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + order.PartnerCode + " RefCode " + order.RefCode;
                        TelegramNotify.SendTeleV2("-4762440012", mess);
                        return new APIResponse(1);
                    }
                }
                //nghi vấn
                if (callback.ResponseCode == -2)
                {
                    if (order.Status == 0)
                    {
                        order.Status = -3;
                        order.TotalAmount = 0;
                        order.LastTime = DateTime.Now;
                        order.Mobile = string.Format("{0} - {1} - {2}", newclObj.PartnerBankCode, newclObj.PartnerBankId, newclObj.PartnerBankName);
                        order.LogContent = "suspicious";
                        order.Note = newclObj.Comment;
                        order.Update();
                    }
                    TelegramNotify.SendTeleV2("-4762440012", "có giao dịch nghi vấn cần xử lý");
                    return apiResponse;
                }

                //thất bại
                if (order.Status == 0)
                {


                    order.Status = -1;
                    order.TotalAmount = 0;
                    order.LastTime = DateTime.Now;
                    order.Mobile = "";

                    order.LogContent = callback.Description;
                    if (order.PartnerCode == "cn02")
                    {
                        if ((callback.Description.Contains("không trùng ") || callback.Description.Contains("tìm thấy")) && order.BankCode != "MOMO")
                        {
                            order.Status = -357;
                        }

                    }
                    order.Update();

                    //var Amount = Math.Min(long.Parse(result.ResponseContent), request.AmountUser);

                    //Action<string, long, string, string, string> send = UpdatePartnerBalanceTopup;
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
                            Type = "bankout"

                        };
                        if (order.BankCode.ToLower() == "momo")
                            datacb.Type = "momoout";
                        if (order.PartnerCode == "cn02")
                        {
                            if ((callback.Description.Contains("không trùng ") || callback.Description.Contains("tìm thấy")) && order.BankCode != "MOMO")
                            {
                                apiResponse.ResponseCode = (int)ResponseCode.BankAccountInvalid;
                            }

                        }
                        apiResponse.Description = callback.Description;
                        apiResponse.ResponseContent = serializer.Serialize(datacb);
                        apiResponse.Signature = PaymentUtils.Signature(apiResponse.ResponseCode.ToString() + apiResponse.Description + apiResponse.ResponseContent, partner.PrivateKey, partner.SignatureType);
                        //datacb.Signature = PaymentUtils.Signature(datacb.RefCode.ToString() + datacb.TransactionID + datacb.Amount, partner.PrivateKey, partner.SignatureType);
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
                        //Task.Run(async () => await DrumBankLib.CallbackJson(order.ReturnUrl, serializer.Serialize(apiResponse), order.TransactionID, order.RefCode).ConfigureAwait(false));
                    }
                    //if (callback.Description.ToLower().Contains("system"))
                    //{
                    //    if (order.BankCode == "MOMO")
                    //    {
                    //        TelegramNotify.SendTeleV2("-4006848376", "MOMOOUT :" + callback.Description);
                    //    }
                    //    else
                    //    {
                    //        TelegramNotify.SendTeleV2("-4006848376", "BANKOUT :" + callback.Description);
                    //    }
                    //}

                }



            }

            return apiResponse;
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
