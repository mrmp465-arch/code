using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            //if (transaction.PartnerCode == "hyn1" || transaction.PartnerCode == "hng"|| transaction.PartnerCode == "hen")
            //     return new APIResponse((int)ResponseCode.SystemBusy);


            // return new APIResponse((int)ResponseCode.SystemBusy);
            //DrumaScriptSerializer serializer = new DrumaScriptSerializer();
            OrderRequest request = new OrderRequest();


            request = serializer.Deserialize<OrderRequest>(transaction.RequestContent);
            var partner = new Partners().GetCache(transaction.PartnerCode);
            var chatid = partner.SMSPlusUrl;
            //if (DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 56)
            //{

            //    if (!string.IsNullOrEmpty(chatid))
            //    {
            //        TelegramNotify.SendTeleV2(chatid, "[bankout]  " + request.RefCode + " => Hệ thống bảo trì | System Under Maintenance | 系统维护中");
            //    }
            //    return new APIResponse((int)ResponseCode.SystemMaintain);
            //}

            //if (DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 59 && transaction.PartnerCode == "cn02")
            //{
            //    return new APIResponse((int)ResponseCode.SystemMaintain);
            //}
            //if (DateTime.Now.Hour == 0 && DateTime.Now.Minute <= 1 && transaction.PartnerCode != "cn02")
            //{

            //    return new APIResponse((int)ResponseCode.SystemMaintain);
            //}
            if (request.Type == "momocash")
            {
                request.BankName = "MOMO";
            }



            if (transaction.PartnerCode != "paytest")
            {
                //if (request.BankName.ToUpper() == "MOMO")
                //{
                //    //var provider = new Providers().GetCache("drummomo");
                //    if (systemDataConfig.GetKey(lstConfig, "MomoCashEnable") == "0")
                //        return new APIResponse((int)ResponseCode.SystemMaintain);

                //}
                //else
                //{
                //    //var provider = new Providers().GetCache("drumbank");
                //    if (systemDataConfig.GetKey(lstConfig, "BankCashEnable") == "0")
                //    {
                //        if (!string.IsNullOrEmpty(chatid))
                //        {
                //            TelegramNotify.SendTeleV2(chatid, "[bankout]  " + request.RefCode + " => Hệ thống bảo trì | System Under Maintenance | 系统维护中");
                //        }
                //        return new APIResponse((int)ResponseCode.SystemMaintain);
                //    }

                //}
                if (systemDataConfig.GetKey(lstConfig, "BankCashEnable") == "0")
                {
                    if (!string.IsNullOrEmpty(chatid))
                    {
                        TelegramNotify.SendTeleV2(chatid, "[bankout]  " + request.RefCode + " => Hệ thống bảo trì | System Under Maintenance | 系统维护中");
                    }
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

            if (request.BankName == "EXIM")
                request.BankName = "EIB";

            if (request.BankName == "COB")
                request.BankName = "COOPBANK";

            if (request.BankName == "DOB")
                request.BankName = "VIKKI";

            if (request.BankName == "OJB")
                request.BankName = "MBV";

            if (request.BankName == "OceanBank")
                request.BankName = "MBV";
            if (request.BankName == "Oceanbank")
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
            //var chatid = DrumBankLib.GetChatId(transaction.PartnerCode);

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
                if (request.BankAccountNumber.Length < 4 || request.BankAccountNumber.Length > 25)
                {
                    TelegramNotify.SendTeleV2(chatid, "[bankout]  " + request.RefCode + request.BankAccountNumber + " => Tài khoản nhận không chính xác | Invalid Recipient Account | 收款账户信息有误");
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
                        TelegramNotify.SendTeleV2("-1003578695759", "[" + transaction.PartnerCode + "]: lệnh rút  " + request.RefCode + " " + request.BankName + " => sai mã ngân hàng");
                    }
                    else
                    {
                        TelegramNotify.SendTeleV2(chatid, "[bankout]  " + request.RefCode + " " + request.BankName + " => Sai mã ngân hàng | Invalid Bank Code | 银行代码错误");
                    }

                    return new APIResponse((int)ResponseCode.BankCodeInvalid);
                }
                else
                {
                    if (lstbankcode.Exists(x => x.code.ToUpper() == request.BankName.ToUpper() && x.isTransfer == 0))
                    {
                        if (string.IsNullOrEmpty(chatid))
                        {
                            TelegramNotify.SendTeleV2("-1003578695759", "[" + transaction.PartnerCode + "]: lệnh rút  " + request.RefCode + " " + request.BankName + "=> mã ngân hàng nhận bảo trì ");
                        }
                        else
                        {
                            TelegramNotify.SendTeleV2(chatid, "[bankout]  " + request.RefCode + " " + request.BankName + " => Mã ngân hàng nhận bảo trì | Receiving Bank Under Maintenance | 收款银行维护中 ");
                        }

                        return new APIResponse((int)ResponseCode.BankCodeMaintain);
                    }
                }

            }
            //check tk
            var blockacount = systemDataConfig.GetKey(lstConfig, "BlockAccount");
            //check tk
            if (blockacount.Contains(request.BankAccountNumber))
            {

                var resultaccount = new APIResponse((int)ResponseCode.BankAccountInvalid);
                resultaccount.Description = "Tài khoản nhận không hợp lệ hoặc nghi vấn lừa đảo";
                return resultaccount;
            }

            // var partner = new Partners().GetCache(transaction.PartnerCode);
            //string chatid = DrumBankLib.GetChatId(transaction.PartnerCode);
            if (!string.IsNullOrEmpty(partner.Hotline))
            {
                var user = new Users().GetByUserName(partner.Hotline.Trim());
                if (user.Balance < request.Amount)
                {
                    if (string.IsNullOrEmpty(chatid))
                    {
                        TelegramNotify.SendTeleV2("-1003578695759", "Đối tác " + transaction.PartnerCode + "  không đủ số dư để tạo lệnh bank out - " + request.RefCode);
                    }
                    else
                    {
                        TelegramNotify.SendTeleV2(chatid, "[bankout]  " + request.RefCode + " => Số dư không đủ | Insufficient Balance | 余额不足");

                    }

                    return new APIResponse((int)ResponseCode.BalanceNotEnough);
                }



            }

            //if (request.Amount >= 10000000)
            //{
            //    if (request.Type == "momocash")
            //    {
            //        TelegramNotify.SendTeleV2("-4898852125", "Có lệnh rút momo từ đối tác " + transaction.PartnerCode + ", số tiền " + request.Amount.ToString("#,#").Replace(",", "."));
            //    }
            //    else
            //    {
            //        TelegramNotify.SendTeleV2("-4898852125", "Có lệnh rút bank từ đối tác " + transaction.PartnerCode + ", số tiền " + request.Amount.ToString("#,#").Replace(",", "."));
            //    }
            //}

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
                Note = "",
                ApproveUser = "auto",
                BankCode = request.BankName.ToUpper(),
                FullName = request.RefCode,
                Mobile = string.Empty,
                RefCode = request.RefCode,
                Group = partner.SMSUrl,
                BankAccountName = request.BankAccountName,
                BankAccountNumber = request.BankAccountNumber
            };
            //if (transaction.PartnerCode == "cn02" || transaction.PartnerCode == "pp")
            //{
            //    _BankCashAPI.Note = request.Note;
            //}
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
                        if (request.Amount < MinMomoCashAmount && request.Amount > MaxMomoCashAmount)
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
                        if (request.Amount > 50000000 && transaction.PartnerCode.Contains("lime"))
                            return new APIResponse((int)ResponseCode.BankAmountInvalid);

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
                var Fee = Convert.ToInt64(Convert.ToInt64(_BankCashAPI.Amount) * ck / 100);
                var Deductdes = String.Format("Deduct bank cash amount: {2} transId: {0}-{1}", _BankCashAPI.TransactionID, _BankCashAPI.BankCode + "-" + _BankCashAPI.RefCode, Convert.ToInt64(_BankCashAPI.Amount).ToString("#,#").Replace(",", "."));
                if (_BankCashAPI.BankCode == "MOMO")
                {
                    Deductdes = String.Format("Deduct momo cash amount: {2} transId: {0}-{1}", _BankCashAPI.TransactionID, _BankCashAPI.BankCode + "-" + _BankCashAPI.RefCode, Convert.ToInt64(_BankCashAPI.Amount).ToString("#,#").Replace(",", "."));
                }
                if (transaction.PartnerCode == "cn02")
                {
                    Deductdes = String.Format("Deduct cash amount: {2} transId: {0}-{1}", _BankCashAPI.TransactionID, _BankCashAPI.BankCode + "-" + _BankCashAPI.RefCode, Convert.ToInt64(_BankCashAPI.Amount).ToString("#,#").Replace(",", "."));

                }


                var resultdeduct = UpdatePartnerBalance(_BankCashAPI.PartnerCode, Convert.ToInt64(_BankCashAPI.Amount), Fee, Deductdes, "BankOut_" + _BankCashAPI.TransactionID.ToString());
                if (resultdeduct < 0)
                {
                    TelegramNotify.SendTeleV2("-1003578695759", "Đối tác " + transaction.PartnerCode + "  không đủ số dư để tạo lệnh bankout");
                    _BankCashAPI.Status = -1;
                    _BankCashAPI.LogContent = "Số dư không đủ";
                    _APIResponse.Description = "Không đủ số dư để thực hiện giao dịch";
                    _BankCashAPI.LastTime = DateTime.Now;

                    _BankCashAPI.Update();

                    TelegramNotify.SendTeleV2(chatid, "[bankout]  " + request.RefCode + " => Số dư không đủ | Insufficient Balance | 余额不足");
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
                //            TelegramNotify.SendTeleV2("-1003976924226", "Sô dư bank out dưới 20tr => anh em bơm tiền vào nhé  !!!! ");

                //        }
                //    }
                //    catch
                //    {

                //    }

                //}

                // Bước: gọi hàm sang API
                step = 3;

                var MinBankAproveAmount = int.Parse(systemDataConfig.GetKey(lstConfig, "MinBankAproveAmount"));
                //NLogLogger.Info(new string[] { "CheckApp", "Amount",request.RefCode,request.Amount.ToString(),MinBankAproveAmount.ToString()
                //    });
                if (!string.IsNullOrEmpty(partner.SMSCommand))
                {
                    if (int.TryParse(partner.SMSCommand, out int smsValue))
                    {
                        if (request.Amount >= smsValue)
                        {
                            //  var chatId = partner.SMSPlusUrl;
                            var mess =
                              "🏦 <b>ConfirmBankOut</b>\n\n" +
                              "📌 订单号 RefCode: <code>" + HttpUtility.HtmlEncode(request.RefCode) + "</code>\n" +
                              "💰 金额 Amount: <b>" + request.Amount.ToString("#,#").Replace(",", ".") + "</b>\n" +
                              "🏛 收款信息 Bank: " +
                                  HttpUtility.HtmlEncode(request.BankName) + " - " +
                                  HttpUtility.HtmlEncode(request.BankAccountNumber) + " - " +
                                  HttpUtility.HtmlEncode(request.BankAccountName) + "\n\n" +

                              "⚠️ Cần xác nhận giao dịch này. Vui lòng bấm <b>Confirm</b> hoặc <b>Cancel</b> bên dưới.\n" +
                              "⚠️ This transaction requires confirmation. Please click <b>Confirm</b> or <b>Cancel</b> below.\n" +
                              "⚠️ 该笔订单需要确认，请点击下方的 <b>确认（Confirm）</b> 或 <b>取消（Cancel）</b>。\n\n";

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
                             "Bank: " + request.BankName + "-" + request.BankAccountNumber + "-" + request.BankAccountName + "\n\n";


                            var userconfirm = partner.SMSPlusCheckUrl;
                            if (!string.IsNullOrEmpty(userconfirm))
                            {
                                userconfirm = userconfirm.Replace(',', ' ');
                                mess = mess + userconfirm;
                                mess2 = mess2 + userconfirm;
                            }

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
                if (request.Amount >= MinBankAproveAmount || request.BankAccountNumber == "123456789")
                {
                    TelegramNotify.SendTeleV2("-1003976924226", "[Duyệt] Có lệnh bankout cần duyệt  " + request.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + transaction.PartnerCode + " RefCode " + request.RefCode);


                    _BankCashAPI.Status = -2;
                    _BankCashAPI.LogContent = "Chờ duyệt";
                    _BankCashAPI.ApproveUser = "";
                    _BankCashAPI.LastTime = DateTime.Now;
                    _BankCashAPI.Update();
                    return new APIResponse(1);

                }
                //if ((transaction.PartnerCode == "nh88" || transaction.PartnerCode == "nh99")  && request.Amount > 1000000)
                //{
                //    long total = 0; int totaltrans = 0; ;
                //    new Libs.Report.BankCashAPI().ReportCheck(request.BankName.ToUpper(), transaction.PartnerCode, request.BankAccountNumber, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ref totaltrans, ref total);
                //    if (total + request.Amount >= 5000000)
                //    {
                //        TelegramNotify.SendTeleV2("-4611125295", "[CheckBankOut] Bankout Pendding cần xác nhận " + request.BankName + "-" + request.BankAccountNumber + " số tiền " + request.Amount.ToString("#,#").Replace(",", ".") + " RefCode " + request.RefCode + " ae xem giúp nhé");


                //        _BankCashAPI.Status = -2;
                //        _BankCashAPI.LogContent = "Chờ xác nhận";
                //        _BankCashAPI.ApproveUser = "";
                //        _BankCashAPI.LastTime = DateTime.Now;
                //        _BankCashAPI.Update();
                //        return new APIResponse(1);
                //    }
                //}
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
                            //    TelegramNotify.SendTeleV2("-1003578695759", "MOMOOUT :" + resObj.Description);
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
                            if (resObj.Description.Contains("busy")|| resObj.Description.Contains("đủ điều kiện"))
                            {
                                _BankCashAPI.Status = -2;
                                _BankCashAPI.LogContent = "Chờ duyệt";
                                _BankCashAPI.LastTime = DateTime.Now;
                                _BankCashAPI.Update();


                                TelegramNotify.SendTeleV2("-1003976924226", "[Duyệt] Có lệnh bankout cần duyệt  " + request.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + transaction.PartnerCode + " RefCode " + request.RefCode);
                                System.Threading.Thread.Sleep(800);
                                if (resObj.Description.Contains("All account")|| resObj.Description.Contains("đủ điều kiện"))
                                {
                                    TelegramNotify.SendTeleV2("-1003976924226", "Tài khoản bank out không đủ tiền=> Vui lòng duyệt tay lệnh này");
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
                                TelegramNotify.SendTeleV2("-1003578695759", "BANKOUT :" + resObj.Description);
                            }

                            //thất bại thì hoàn tiền
                            UpdatePartnerBalanceTopup(_BankCashAPI.PartnerCode, Convert.ToInt64(_BankCashAPI.Amount), Fee, String.Format("Refund withdrawal amount: {2} transId: {0}-{1}", _BankCashAPI.TransactionID, _BankCashAPI.BankCode + "-" + _BankCashAPI.RefCode, Convert.ToInt64(_BankCashAPI.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _BankCashAPI.TransactionID.ToString());

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
                        NLogLogger.Info(new string[] { "MDrum", "TCP", request.RefCode });
                        TelegramNotify.SendTeleV2("-1003578695759", "Nghi vấn lỗi TCP !!!!! ");
                        return new APIResponse(1);

                        ////if (resObj.Description.ToLower().Contains("system") || resObj.Description.ToLower().Contains("fail"))
                        ////{
                        ////    TelegramNotify.SendTeleV2("-1003578695759", "BANKOUT :" + resObj.Description);
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
        public void ReCash(BankCashAPI _CardAPILog)
        {

            if (_CardAPILog.ProviderCode == "drumbankcash")
            {

                _CardAPILog.Status = 0;
                _CardAPILog.LogContent = " duyệt tự động bởi core";
                //_CardAPILog.ApproveUser = AppUtils.UserName;
                _CardAPILog.LastTime = DateTime.Now;
                _CardAPILog.Update();


                CashBankRequestV2 _cashRequest = new CashBankRequestV2();

                _cashRequest.BankName = _CardAPILog.BankAccountName;
                _cashRequest.BankId = _CardAPILog.BankAccountNumber;
                _cashRequest.BankCode = _CardAPILog.BankCode;
                _cashRequest.Comment = "chuyen khoan " + DateTime.Now.ToString("HHmmss");

                if (_CardAPILog.PartnerCode == "cn02" || _CardAPILog.PartnerCode == "pp")
                {
                    //_cashRequest.Comment = _CardAPILog.Note +;
                    _cashRequest.BankName = "NOCHECK";
                }
                _cashRequest.Amount = Convert.ToInt32(_CardAPILog.Amount);
                _cashRequest.CallbackUrl = callbackbankurl;
                _cashRequest.TransId = _CardAPILog.TransactionID.ToString() + "_recall";





                var signature = "";
                var requestData = new RequestData()
                {
                    PartnerCode = "order",
                    CommandCode = "TRANS_OUT",
                    RequestContent = serializer.Serialize(_cashRequest),
                    Signature = signature
                };
                //var partner = new Partners().Get(transaction.PartnerCode);
                //if (string.IsNullOrEmpty(partner.SMSPlusUrl))
                //{
                //    requestData.PartnerCode = "order";
                //}
                CashRespone cashResult = new CashRespone();

                NLogLogger.Info(new string[] { "MDrum", "Cash Request",serializer.Serialize(requestData), urlBaseService2
                                     });
                var response = Task.Run(async () => await CallbackJson(urlBaseService2, serializer.Serialize(requestData), 0)).Result;
                NLogLogger.Info(new string[] { "MDrum", "Cash Response", response, urlBaseService2
                                    });
                var ck = getck(_CardAPILog.PartnerCode, _CardAPILog.BankCode.ToUpper());
                var Fee = Convert.ToInt64(Convert.ToInt64(_CardAPILog.Amount) * ck / 100);
                try
                {

                    var resObj = serializer.Deserialize<CashRespone>(response);
                    if (resObj.ResponseCode == 1)
                    {

                    }
                    else
                    {
                        if (resObj.Description.Contains("is busy"))
                        {
                            _CardAPILog.Status = -2;
                            _CardAPILog.LogContent = "Chờ duyệt";
                            _CardAPILog.LastTime = DateTime.Now;
                            _CardAPILog.Update();


                            TelegramNotify.SendTeleV2("-1003976924226", "Có lệnh bankout cần duyệt  " + _CardAPILog.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + _CardAPILog.PartnerCode + " RefCode " + _CardAPILog.RefCode);

                            //return new APIResponse(1);
                        }
                        else
                        {
                            UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Refund bank withdrawal amount: {2} transId: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());


                            _CardAPILog.Status = -1;

                            _CardAPILog.LogContent = resObj.Description;
                            _CardAPILog.LastTime = DateTime.Now;
                            _CardAPILog.Update();
                        }



                    }
                }
                catch (Exception ex)
                {
                    NLogLogger.Info(ex.Message);
                    _CardAPILog.Status = -3;

                    _CardAPILog.LogContent = "suspicious";
                    _CardAPILog.LastTime = DateTime.Now;
                    _CardAPILog.Update();
                    //UpdatePartnerBalanceTopup(_CardAPILog.PartnerCode, Convert.ToInt64(_CardAPILog.Amount), Fee, String.Format("Hoàn tiền rút bank số tiền: {2} mgd: {0}-{1}", _CardAPILog.TransactionID, _CardAPILog.BankCode + "-" + _CardAPILog.RefCode, Convert.ToInt64(_CardAPILog.Amount).ToString("#,#").Replace(",", ".")), "BankOutRefund_" + _CardAPILog.TransactionID.ToString());

                }

            }

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
                var partner = new Partners().GetCache(order.PartnerCode);
                if (order.Status == 0)
                {
                    //bắn tele
                    //if (order.PartnerCode == "sn1")
                    //{
                    //    var mess = $"<b>Tài khoản</b>: ({order.BankCode}) {order.BankAccountNumber} -  {order.BankAccountName} %0A<b>Mã giao dịch</b>: {newclObj.BankTransId} -  {order.RefCode} %0A<b>Số tiền</b>: -{newclObj.Amount.ToString("#,#").Replace(",", ".")} %0A<b>Thời gian</b>: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}";
                    //    TelegramNotify.SendTeleV4("-1002857697732", mess);
                    //}
                    //if (order.PartnerCode == "b23")
                    //{
                    //    var mess = $"<b>Tài khoản</b>: ({order.BankCode}) {order.BankAccountNumber} -  {order.BankAccountName} %0A<b>Mã giao dịch</b>: {newclObj.BankTransId} -  {order.RefCode} %0A<b>Số tiền</b>: -{newclObj.Amount.ToString("#,#").Replace(",", ".")} %0A<b>Thời gian</b>: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}";
                    //    TelegramNotify.SendTeleV4("-5050441570", mess);
                    //}
                    //if (order.PartnerCode == "cn02")
                    //{
                    //    if (!newclObj.transId.Contains("call"))
                    //    {
                    //        if (order.CreatedTime.AddSeconds(100) < DateTime.Now)
                    //            TelegramNotify.SendTeleV4("-1003578695759", "Out VPB có hiện tượng chậm=>Vui lòng kiểm tra  lại  hệ thống");
                    //    }
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
                    order.Note = newclObj.Comment;
                    order.Fee = Convert.ToInt64(Convert.ToInt64(order.Amount) * ck / 100);
                    order.Reward = Convert.ToInt64(Convert.ToInt64(order.Amount) * (rw) / 100);
                    order.LogContent = "core update";

                    if (!string.IsNullOrEmpty(newclObj.BankName))
                        order.BankAccountName = newclObj.BankName;
                    if (order.BankCode == "MOMO")
                    {
                        order.Note = GlobalHelper.ReplaceVietnameseChar(newclObj.Note);
                        //order.Mobile = newclObj.MomoPartnerId;
                    }
                    order.Update();

                    //var Amount = Math.Min(long.Parse(result.ResponseContent), request.AmountUser);
                    //Callback for Partner

                    //Action<string, long, string, string> send = UpdatePartnerBalance;
                    //var asynSend = send.BeginInvoke(order.PartnerCode, Convert.ToInt64(order.Amount), order.BankCode, String.Format("Trừ tiền rút bank số tiền: {2} mgd: {0}-{1}", order.TransactionID,  order.BankCode, Convert.ToInt64(order.Amount).ToString("#,#").Replace(",", ".")), null, null);

                    if (!string.IsNullOrEmpty(order.ReturnUrl))
                    {
                        var checkOrder = new CheckOrder
                        {
                            LasTime = DateTime.Now,
                            RefCode = order.RefCode,
                            Amount = Convert.ToInt32(order.Amount),
                            TransactionID = order.TransactionID.ToString()
                        };

                        DataCaching.SetCache("CheckOutOrder:" + order.PartnerCode + order.RefCode, checkOrder, 900);
                        var datacb = new DataCallback()
                        {
                            RefCode = order.RefCode,
                            TransactionID = order.TransactionID.ToString(),
                            Amount = Convert.ToInt32(order.Amount),
                            //MomoTransId = newclObj.MomoTransId,
                            OrderInfo = newclObj.MomoTransId,
                            Type = "bankout"
                        };
                        if (order.BankCode.ToLower() == "momo")
                            datacb.Type = "momoout";
                        apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful)
                        {

                        };
                        datacb.ResponseCode = apiResponse.ResponseCode;
                        datacb.Description = apiResponse.Description;
                        datacb.Signature = PaymentUtils.Signature(datacb.ResponseCode.ToString() + datacb.Description + datacb.RefCode, partner.PrivateKey, partner.SignatureType);
                        Task.Run(async () => await DrumBankLib.CallbackJsonV2(order.ReturnUrl, serializer.Serialize(datacb), order.TransactionID, order.RefCode).ConfigureAwait(false));


                        if (partner.RequestType > 0)
                        {
                            if (order.Amount >= partner.RequestType)
                            {
                                var url = String.Format("http://149.28.151.96:1468/DetailScreen.aspx?orderNo={0}&refcode={1}&chatid={2}", order.TransactionID, order.RefCode, partner.SMSPlusUrl);
                                SendBill(url);
                            }
                        }

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
                var isCancel = 0;
                if (order.BankCode == "VBA" && newclObj.PartnerBankCode == "VPB")
                {
                    isCancel = 1;
                }
                //duyệt tay
                if ((callback.Description.Contains("busy") || callback.Description.Contains("Bank Code Invalid")) && order.ProviderCode == "drumbankcash" && isCancel == 0)
                {
                    //if (order.Amount >= 200000 && newclObj.PartnerBankCode == "VPB" && callback.Description.Contains("busy"))
                    //{
                    //    var check2 = new BankTransaction().GetByRefcode(newclObj.transId);
                    //    if (check2 != null)
                    //    {
                    //        if (check2.Description.Contains("Exception"))
                    //        {
                    //            if (order.Status == 0)
                    //            {
                    //                order.Status = -3;
                    //                order.TotalAmount = 0;
                    //                order.LastTime = DateTime.Now;
                    //                order.Mobile = string.Format("{0} - {1} - {2}", newclObj.PartnerBankCode, newclObj.PartnerBankId, newclObj.PartnerBankName);
                    //                order.LogContent = "suspicious";
                    //                order.Note = newclObj.Comment;
                    //                order.Update();
                    //            }
                    //            TelegramNotify.SendTeleV2("-1003578695759", "có giao dịch nghi vấn cần xử lý  RefCode " + order.RefCode + " ,đối tác " + order.PartnerCode);
                    //            return apiResponse;
                    //        }
                    //    }
                    //}

                    order.Status = -2;
                    order.LogContent = "Chờ duyệt";
                    order.LastTime = DateTime.Now;
                    order.Update();

                    var mess = "[Duyệt] Có lệnh bankout cần duyệt  " + order.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + order.PartnerCode + " RefCode " + order.RefCode + " Id " + order.TransactionID + " bank " + order.BankCode;

                    if (newclObj.transId.Contains("recall"))
                        mess = "[Duyệt lần 2] Có lệnh bankout cần duyệt  " + order.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + order.PartnerCode + " RefCode " + order.RefCode + " Id " + order.TransactionID + " bank " + order.BankCode;

                    if (newclObj.transId.Contains("re2call"))
                        mess = "[Duyệt lần 3] Có lệnh bankout cần duyệt tay " + order.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + order.PartnerCode + " RefCode " + order.RefCode + " Id " + order.TransactionID + " bank " + order.BankCode;
                    if (callback.Description.Contains("All account"))
                    {
                        mess += " => Tài khoản bank out không đủ tiền";
                    }
                    TelegramNotify.SendTeleV2("-1003976924226", mess);

                    try
                    {
                        var check = new BankTransaction().GetByRefcode(newclObj.transId);
                        if (check != null)
                        {


                            if (check.Description == "Start Operation Failed")
                            {

                                //TelegramNotify.SendTeleV2("-1003578695759", "Nghi vấn lỗi Proxy");
                            }
                            else
                            {
                                //if (check.Description == "Exception from SyncRequestReceived")
                                //{

                                //    TelegramNotify.SendTeleV2("-1003578695759", "Lỗi treo phone client, bật phần mềm trên máy tính kết nối lại");
                                //}
                                if (check.Description.Contains("Device") || check.Description.Contains("OTP") || check.Description.Contains("Start") || check.Description.Contains("Screen") || check.Description.Contains("connected"))
                                {
                                    var bank = new BankAccounts().Get(check.PartnerBankId, check.PartnerBankCode);
                                    System.Threading.Thread.Sleep(200);
                                    if (check.Description.Contains("App ACB Not Start") || check.Status == -17)
                                    {
                                        TelegramNotify.SendTeleV2("-1003578695759", "Lỗi thiết bị " + bank.Computer + "-" + bank.PhoneDevice + " : " + check.Description + " => vui lòng kiểm tra lại thiết bị ok mới duyệt");
                                    }
                                    else
                                    {
                                        TelegramNotify.SendTeleV2("-1003976924226", "Lỗi thiết bị " + bank.Computer + "-" + bank.PhoneDevice + " : " + check.Description + " => vui lòng kiểm tra lại thiết bị ok mới duyệt");
                                    }

                                }


                            }

                        }
                    }
                    catch
                    {

                    }
                    return new APIResponse(1);
                }

                //pending va  thì cần duyệt tay
                if (callback.ResponseCode == -8 && isCancel == 0)
                {
                    if (order.Status == 0)
                    {
                        order.Status = -2;
                        order.LogContent = "Chờ duyệt";
                        order.LastTime = DateTime.Now;
                        order.Note = newclObj.Comment;
                        order.Update();

                        var mess = "[Duyệt] Có lệnh bankout cần duyệt  " + order.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + order.PartnerCode + " RefCode " + order.RefCode + " bank " + order.BankCode;

                        if (newclObj.transId.Contains("recall"))
                            mess = "[Duyệt lần 2] Có lệnh bankout cần duyệt  " + order.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + order.PartnerCode + " RefCode " + order.RefCode + " bank " + order.BankCode;

                        if (newclObj.transId.Contains("re2call"))
                            mess = "[Duyệt lần 3] Có lệnh bankout cần duyệt tay " + order.Amount.ToString("#,#").Replace(",", ".") + " từ đối tác " + order.PartnerCode + " RefCode " + order.RefCode + " bank " + order.BankCode;
                        if (callback.Description.Contains("All account"))
                        {
                            mess += " => Tài khoản bank out không đủ tiền";
                        }
                        int sendtele = 1;


                        try
                        {
                            var check = new BankTransaction().GetByRefcode(newclObj.transId);
                            if (check != null)
                            {
                                //if (check.Description == "Exception from SyncRequestReceived")
                                //{

                                //    TelegramNotify.SendTeleV2("-1003578695759", "Lỗi treo phone client, bật phần mềm trên máy tính kết nối lại");
                                //}
                                if ((check.Description.Contains("Verify Method") || check.Description.Contains("Submit Method: Null or Empty") || check.Description.Contains("AccountInquiry Null or Empty") || check.Description.Contains("OTP not is numeric")))
                                {
                                    TelegramNotify.SendTeleV2("-1003976924226", mess);
                                    sendtele = 0;
                                    System.Threading.Thread.Sleep(500);
                                    //TelegramNotify.SendTeleV2("-1003976924226", "Lỗi verify => duyệt lại luôn");
                                }
                                else
                                {
                                    if (check.Description.Contains("Device") || check.Description.Contains("OTP") || check.Description.Contains("Start") || check.Description.Contains("Screen") || check.Description.Contains("connected"))
                                    {
                                        TelegramNotify.SendTeleV2("-1003976924226", mess);
                                        sendtele = 0;
                                        var bank = new BankAccounts().Get(check.PartnerBankId, check.PartnerBankCode);
                                        System.Threading.Thread.Sleep(500);
                                        if (check.Description.Contains("App ACB Not Start") || check.Status == -17 || check.Description.Contains("connected"))

                                        {
                                            TelegramNotify.SendTeleV2("-1003578695759", "Lỗi thiết bị " + bank.Computer + "-" + bank.PhoneDevice + " : " + check.Description + " => vui lòng kiểm tra lại thiết bị ok mới duyệt");
                                        }
                                        else
                                        {
                                            TelegramNotify.SendTeleV2("-1003976924226", "Lỗi thiết bị " + bank.Computer + "-" + bank.PhoneDevice + " : " + check.Description + " => vui lòng kiểm tra lại thiết bị ok mới duyệt");
                                        }

                                    }
                                    if (check.Status == -51)
                                    {
                                        TelegramNotify.SendTeleV2("-1003976924226", mess);
                                        var bank = new BankAccounts().Get(check.PartnerBankId, check.PartnerBankCode);
                                        System.Threading.Thread.Sleep(500);
                                        TelegramNotify.SendTeleV2("-1003578695759", "Tài khoản " + check.PartnerBankCode + " -" + bank.BankName + " lệch số dư=> động bộ lại trước khi duyệt");
                                    }
                                }


                                if (check.Description.Contains("An error occurred! (6802)"))
                                {
                                    var bank = new BankAccounts().Get(check.PartnerBankId, check.PartnerBankCode);
                                    TelegramNotify.SendTeleV2("-1003578695759", "Nghi vấn khoá " + bank.BankCode + " - " + bank.BankName + " => vui lòng kiểm tra lại ");
                                    bank.Status = -1;
                                    bank.Update();
                                }

                            }
                        }
                        catch
                        {

                        }
                        if (sendtele == 1)
                            TelegramNotify.SendTeleV2("-1003976924226", mess);
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
                    TelegramNotify.SendTeleV2("-1003578695759", "có giao dịch nghi vấn cần xử lý  RefCode " + order.RefCode);
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
                    if ((callback.Description.Contains("không trùng ") || callback.Description.Contains("tìm thấy")) && order.ProviderCode == "drumbankcash")
                    {
                        order.Status = -357;
                    }
                    order.Update();



                    //var Amount = Math.Min(long.Parse(result.ResponseContent), request.AmountUser);

                    //Action<string, long, string, string, string> send = UpdatePartnerBalanceTopup;
                    var ck = getck(order.PartnerCode, order.BankCode.ToUpper());
                    var Fee = Convert.ToInt64(Convert.ToInt64(order.Amount) * ck / 100);
                    var toupdesc = String.Format("Refund withdraw bank amount: {2} transId: {0}-{1}", order.TransactionID, order.BankCode + "-" + order.RefCode, Convert.ToInt64(order.Amount).ToString("#,#").Replace(",", "."));
                    if (order.BankCode == "MOMO")
                    {
                        toupdesc = String.Format("Refund withdraw bank amount: {2} transId: {0}-{1}", order.TransactionID, order.BankCode + "-" + order.RefCode, Convert.ToInt64(order.Amount).ToString("#,#").Replace(",", "."));
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
                            Amount = 0,
                            Type = "bankout"

                        };
                        if (order.BankCode.ToLower() == "momo")
                            datacb.Type = "momoout";
                        //if ((callback.Description.Contains("không trùng ") || callback.Description.Contains("tìm thấy")) && order.BankCode != "MOMO")
                        //{

                        //    apiResponse.ResponseCode = (int)ResponseCode.BankAccountInvalid;
                        //}
                        apiResponse.Description = callback.Description;
                        datacb.ResponseCode = apiResponse.ResponseCode;
                        datacb.Description = apiResponse.Description;
                        datacb.Signature = PaymentUtils.Signature(datacb.ResponseCode.ToString() + datacb.Description + datacb.RefCode, partner.PrivateKey, partner.SignatureType);
                        //datacb.Signature = PaymentUtils.Signature(datacb.RefCode.ToString() + datacb.TransactionID + datacb.Amount, partner.PrivateKey, partner.SignatureType);
                        Task.Run(async () => await DrumBankLib.CallbackJsonV2(order.ReturnUrl, serializer.Serialize(datacb), order.TransactionID, order.RefCode).ConfigureAwait(false));
                    }
                    //if (callback.Description.ToLower().Contains("system"))
                    //{
                    //    if (order.BankCode == "MOMO")
                    //    {
                    //        TelegramNotify.SendTeleV2("-1003578695759", "MOMOOUT :" + callback.Description);
                    //    }
                    //    else
                    //    {
                    //        TelegramNotify.SendTeleV2("-1003578695759", "BANKOUT :" + callback.Description);
                    //    }
                    //}

                }



            }

            return apiResponse;
        }

        private decimal getck(string PartnerCode, string Type)
        {
            decimal ck = 1;
            ///var partner = new Partners().GetCache(PartnerCode);
            var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, 2030, 1);
            if (listpartnerDiscount == null)
            {
                //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return ck;
            }

            if (!listpartnerDiscount.Exists(x => x.Date.Day == 1))
            {
                //TelegramNotify.SendTeleV2("-4714349747", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return ck;
            }
            var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == 1);
            ck = _partnerDiscount.DiscountBANKOUTTRANFER;
            if (Type == "MOMO")
                ck = _partnerDiscount.DiscountMOMOOUT;
            return ck;
        }
        private decimal getrw(string PartnerCode, string Type)
        {
            decimal ck = 0;
            ///var partner = new Partners().GetCache(PartnerCode);
            var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, 2030, 1);
            if (listpartnerDiscount == null)
            {
                //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return ck;
            }

            if (!listpartnerDiscount.Exists(x => x.Date.Day == 1))
            {
                //TelegramNotify.SendTeleV2("-4714349747", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                return ck;
            }
            var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == 1);
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
                //var partner = new Partners().GetCache(PartnerCode);
                //if (string.IsNullOrEmpty(partner.Hotline))
                //{
                //    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                //    return 0;
                //}
                //var user = new Users().GetByUserName(partner.Hotline.Trim());
                //if (user == null)
                //{
                //    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                //    return 0;
                //}




                long realAmount = Amount + Fee;
                //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
                return new Users().Deduct(realAmount, PartnerCode, PartnerCode, Note, RefCode);
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
                //var partner = new Partners().GetCache(PartnerCode);
                //if (string.IsNullOrEmpty(partner.Hotline))
                //{
                //    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                //    return 0;
                //}
                //var user = new Users().GetByUserName(partner.Hotline.Trim());
                //if (user == null)
                //{
                //    //TelegramNotify.SendWarning("-4214596800", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                //    return 0;
                //}


                long realAmount = Amount + Fee;
                //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
                return new Users().Topup(realAmount, PartnerCode, PartnerCode, TranId, RefCode);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
                return -99;
            }


        }
    }

}
