using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Script.Serialization;
using Libs.API;
using Libs.Db;
using Libs.TopupPartner.PayDirect;
using Libs.TopupPartner.VGG;
using Libs.TopupPartner.VNPTEPAY;
using Libs.Utils;
using Libs.Report;
using System.Linq;

namespace Libs.TopupPartner
{
    //public class BuyCardService : APIService
    public class BuyCardService : IServiceHandler
    {

        //public override APIResponse Request(APITransaction transaction)
        public APIResponse Request(APITransaction transaction)
        {
            switch (transaction.CommandCode)
            {
                case "buycard":
                    return BuyCard(transaction);
                case "checkstore":
                    return CheckStore(transaction);
                case "checkStatus":
                    return CheckStatus(transaction);
                default:
                    return new APIResponse((int)ResponseCode.AccessDenied);
            }
        }

        private APIResponse BuyCard(APITransaction transaction)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();


            BuyCardRequest request = new BuyCardRequest();
            BuyCardAPILog byCardApiLog = new BuyCardAPILog();
            try
            {
                //Get Provider Switch dịch vụ bằng đoạn code này

                request = serializer.Deserialize<BuyCardRequest>(transaction.RequestContent);

                if (request.Amount < 50000)
                {
                    return new APIResponse((int)ResponseCode.CardAmountInvalid);
                }
                if (request.Quantity < 1 || request.Quantity > 5)
                {
                    return new APIResponse((int)ResponseCode.CardQuantityLimit);
                }

                //kiểm tra số dư
                var partner = new Partners().GetCache(transaction.PartnerCode);
                if(!string.IsNullOrEmpty(partner.Hotline))
                {
                    var user = new Users().GetByUserName(partner.Hotline.Trim());
                    if(user!=null)
                    {
                        if (user.Balance < request.Quantity * request.Amount)
                        {
                            TelegramNotify.SendWarning("-4278595388", "Đối tác " + transaction.PartnerCode + "  không đủ số dư để tạo lệnh rút thẻ");
                            return new APIResponse((int)ResponseCode.BalanceNotEnough);
                        }
                    }    
                    

                }    
               
                //Check Parnter Quota
                var partnerStatus = new Partners().GetBuyCardCondition(transaction.PartnerID, transaction.ServiceID);
                switch (partnerStatus)
                {
                    case "F":
                        return new APIResponse((int)ResponseCode.TransactionLimit);
                    case "I":
                        return new APIResponse((int)ResponseCode.ServiceIsLocked);
                }


                //Switch Provider
                var providerCode = new Providers().GetBuyCardCondition(request.Provider, transaction.PartnerCode);
                if (providerCode.Equals("CPI"))
                {
                    return new APIResponse((int)ResponseCode.CardProviderInvalid);
                }

                //Ghi Log BuyCard
                byCardApiLog.TransactionID = transaction.TransactionID;
                byCardApiLog.TransactionNo = transaction.TransactionID.ToString();
                byCardApiLog.PartnerID = transaction.PartnerID;
                byCardApiLog.PartnerCode = transaction.PartnerCode;
                byCardApiLog.ProviderCode = providerCode;
                byCardApiLog.AccountName = request.AccountName;
                byCardApiLog.AccountId = request.AccountId;
                byCardApiLog.OrderNo = request.OrderNo;
                //byCardApiLog.RequestTime = 0;
                byCardApiLog.Provider = request.Provider;
                byCardApiLog.Amount = request.Amount;
                byCardApiLog.Quantity = request.Quantity;
                //byCardApiLog.ErrorCode = -1; // Khoi tao GD
                byCardApiLog.Message = string.Empty;
                byCardApiLog.LogContent = string.Empty;
                byCardApiLog.ListCards = string.Empty;
                //byCardApiLog.CreatedTime = DateTime.Now;
                //byCardApiLog.LastTime = DateTime.Now;
                byCardApiLog.Status = 0;
                byCardApiLog.Add();
                // Nếu thêm giao dịch không hợp lệ
                if (byCardApiLog.ReturnValue < 0)
                {
                    return new APIResponse(byCardApiLog.ReturnValue);
                }

                var handler = BuyCardFactory.GetHandler(providerCode);
                //NLogLogger.Info(new string[] { "BuyCard", transaction.TransactionID.ToString(), "Info", "ProviderCode", providerCode });
                //PayDirectService payDirectService = new PayDirectService();
                //var result = payDirectService.downloadSoftpin(byCardApiLog.TransactionNo, byCardApiLog.Provider, byCardApiLog.Amount, byCardApiLog.Quantity);
                string responseData = string.Empty;
                APIResponse result = null;
                long fee = 0;
                try
                {
                    result = handler.downloadSoftpin(byCardApiLog.TransactionNo, byCardApiLog.Provider, byCardApiLog.Amount, byCardApiLog.Quantity, byCardApiLog.PartnerCode, byCardApiLog.ProviderCode, ref responseData);

                    if (result.ResponseCode > 0)
                    {
                        var Amount = byCardApiLog.Amount * byCardApiLog.Quantity;
                        var ck = getCk( byCardApiLog.PartnerCode);
                        fee= Convert.ToInt64(Amount * ck);
                        Action<string, long, string, string, string> send = UpdatePartnerBalance;
                        var asynSend = send.BeginInvoke(transaction.PartnerCode, Amount, byCardApiLog.Provider, String.Format("Trừ tiền mua thẻ {1} mgd: {0}", byCardApiLog.TransactionID, Amount.ToString("#,#").Replace(",", ".")), "BuyCard_" + byCardApiLog.TransactionID, null, null);
                    }
                    //HardCode: Nếu Provider là nội tại thì chyển qua Provide được chỉ định khác để mua
                    if (providerCode == "pl" && result.ResponseCode == (int)ResponseCode.CardOutOfStock)
                    {
                        NLogLogger.Info(new string[] { "BuyCard", "Card Out Of Stock", "Redirect to m32cash" });
                        byCardApiLog.ProviderCode = "m32cash";
                        var handlerGate = BuyCardFactory.GetHandler("m32cash");
                        result = handlerGate.downloadSoftpin(byCardApiLog.TransactionNo, byCardApiLog.Provider, byCardApiLog.Amount, byCardApiLog.Quantity, byCardApiLog.PartnerCode, byCardApiLog.ProviderCode, ref responseData);
                    }
                    //End HardCode
                }
                catch (Exception ex)
                {
                    byCardApiLog.Status = (int)ResponseCode.SystemError;
                    byCardApiLog.Update();
                    NLogLogger.Info(new string[] { "BuyCard", transaction.TransactionID.ToString(), "Error", ex.Message.Replace("\n", " ") });
                    return new APIResponse((int)ResponseCode.RequestContentInvalid);
                }

                if (result.ResponseCode == (int)ResponseCode.TransactionSuccessful)
                {

                    byCardApiLog.Status = (int)ResponseCode.TransactionSuccessful;

                    byCardApiLog.ListCards = result.ResponseContent;
                    if (byCardApiLog.Quantity > 200)
                    {
                        byCardApiLog.ListCards = "";
                    }
                    byCardApiLog.AccountId = fee;
                    byCardApiLog.Update();
                    //Partners _Partner = new Partners().Get(transaction.PartnerCode); // Lay key de ma hoa
                }
                else
                {
                    byCardApiLog.Status = result.ResponseCode;
                    byCardApiLog.LogContent = responseData;
                    byCardApiLog.Update();
                }

                return result;

            }
            catch (Exception ex)
            {
                byCardApiLog.Status = (int)ResponseCode.SystemError;
                byCardApiLog.Update();
                NLogLogger.Info(new string[] { "BuyCard", transaction.TransactionID.ToString(), "Error", "Deserialize(requestContent)", ex.Message.Replace("\n", " ") });
                return new APIResponse((int)ResponseCode.RequestContentInvalid);
            }

        }
        private decimal getCk(string PartnerCode)
        {
            try
            {
                //NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), TranId.ToString(), RefCode });
               
                var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, DateTime.Now.Year, DateTime.Now.Month);
                if (listpartnerDiscount == null)
                {
                    //TelegramNotify.SendTeleV2("-5539262476", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return 0;
                }

                if (!listpartnerDiscount.Exists(x => x.Date.Day == DateTime.Now.Day))
                    return 0;

                var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == DateTime.Now.Day);
                decimal ck = _partnerDiscount.DiscountVTTOUT;

                return ck;
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
                return 0;
            }
        }
        private void UpdatePartnerBalance(string PartnerCode, long Amount, string CardType, string TranId, string RefCode)
        {
            try
            {
                NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), TranId.ToString(), RefCode });
                var partner = new Partners().GetCache(PartnerCode);
                if (string.IsNullOrEmpty(partner.Hotline))
                {
                    //TelegramNotify.SendTeleV2("-5539262476", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                    return;
                }
                var user = new Users().GetByUserName(partner.Hotline.Trim());
                if (user == null)
                {
                    //TelegramNotify.SendTeleV2("-5539262476", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                    return;
                }
                var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, DateTime.Now.Year, DateTime.Now.Month);
                if (listpartnerDiscount == null)
                {
                    //TelegramNotify.SendTeleV2("-5539262476", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return;
                }

                if (!listpartnerDiscount.Exists(x => x.Date.Day == DateTime.Now.Day))
                    return;

                var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == DateTime.Now.Day);
                decimal ck = _partnerDiscount.DiscountVTTOUT;

                if (ck == 0)
                    return;

                long realAmount = Amount - Convert.ToInt64(Amount * ck);
                //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
                new Users().Deduct(realAmount, user.UserName, PartnerCode, TranId, RefCode);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
            }
        }
        private APIResponse CheckStatus(APITransaction transaction)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();


            BuyCardRequest request = new BuyCardRequest();
            BuyCardAPILog byCardApiLog = new BuyCardAPILog();
            try
            {
                //Get Provider Switch dịch vụ bằng đoạn code này

                request = serializer.Deserialize<BuyCardRequest>(transaction.RequestContent);

                if (string.IsNullOrEmpty(request.OrderNo))
                {
                    return new APIResponse((int)ResponseCode.TransactionInvalid);
                }

                //Get Card from DB
                var cardDB = byCardApiLog.GetByOrderNoPartnerCode(request.OrderNo, transaction.PartnerCode);

                if (cardDB == null)
                {
                    return new APIResponse((int)ResponseCode.TransactionInvalid);
                }


                APIResponse result = new APIResponse();
                result.ResponseCode = cardDB.Status;
                result.ResponseContent = cardDB.ListCards;
                result.Description = string.Empty;
                result.Signature = string.Empty;

                return result;

            }
            catch (Exception ex)
            {
                byCardApiLog.Status = (int)ResponseCode.SystemError;
                byCardApiLog.Update();
                NLogLogger.Info(new string[] { "BuyCard", transaction.TransactionID.ToString(), "Error", "Deserialize(requestContent)", ex.Message.Replace("\n", " ") });
                return new APIResponse((int)ResponseCode.RequestContentInvalid);
            }

        }

        public class BuyCardRequest
        {
            public string Provider { get; set; } // CardType
            public int Amount { get; set; }
            public int Quantity { get; set; }
            public string AccountName { get; set; }
            public long AccountId { get; set; }
            public string OrderNo { get; set; }
        }

        public class ListCards
        {
            public string Provider { get; set; } // CardType
            public int Amount { get; set; }
            public string serial { get; set; }
            public string pin { get; set; }
            public string expire { get; set; }

        }

        private APIResponse CheckStore(APITransaction transaction)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BuyCardRequest request = new BuyCardRequest();
            try
            {
                request = serializer.Deserialize<BuyCardRequest>(transaction.RequestContent);


                //Kiểm tra kho hàng

                int resultCheckStore = 0;
                //if (request.Provider.Equals("CC"))
                //{
                //    VGGService vggService = new VGGService();
                //    resultCheckStore = vggService.checkStore(request.Provider, request.Amount);
                //}
                //else
                //{
                //    EpayService epayService = new EpayService();
                //    resultCheckStore = epayService.checkStore(request.Provider, request.Amount);
                //}

                //Switch Provider
                var providerCode = new Providers().GetBuyCardCondition(request.Provider, transaction.PartnerCode);
                if (providerCode.Equals("CPI"))
                {
                    return new APIResponse((int)ResponseCode.CardProviderInvalid);
                }

                var handler = BuyCardFactory.GetHandler(providerCode);
                resultCheckStore = handler.checkStore(request.Provider, request.Amount);

                if (request.Quantity > 0)
                {
                    if (resultCheckStore < request.Quantity)
                    {
                        return new APIResponse()
                        {
                            ResponseCode = -107,
                            Description = "Thẻ trong kho không đủ cho giao dịch.",
                            ResponseContent = resultCheckStore.ToString()
                        };
                    }

                    return new APIResponse()
                    {
                        ResponseCode = (int)ResponseCode.TransactionSuccessful,
                        Description = "Có thể mua thẻ và số lượng này.",
                        ResponseContent = resultCheckStore.ToString()
                    };
                }
                else
                {
                    return new APIResponse()
                    {
                        ResponseCode = (int)ResponseCode.TransactionSuccessful,
                        Description = "Kiểm tra kho thành công.",
                        ResponseContent = resultCheckStore.ToString()
                    };
                }

            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "CheckStore", transaction.TransactionID.ToString(), "Error", "Deserialize(requestContent)", ex.Message.Replace("\n", " ") });
                return new APIResponse((int)ResponseCode.SystemError);
            }


            //return new APIResponse();
        }


    }
}
