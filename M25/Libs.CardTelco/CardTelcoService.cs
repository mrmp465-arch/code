using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

using Libs.API;
using Libs.CardTelco.Gate;
using Libs.Report;
using Libs.Utils;

namespace Libs.CardTelco
{
    //public class CardTelcoService : APIService
    public class CardTelcoService : IServiceHandler
    {

        //public override APIResponse Request(APITransaction transaction)
        public APIResponse Request(APITransaction transaction)
        {
            switch (transaction.CommandCode.ToLower())
            {
                case "usecard":
                    return UseCard(transaction);
                case "checktrans":
                    return CheckTrans(transaction);
                default:
                    return new APIResponse((int)API.ResponseCode.AccessDenied);
            }
        }
        private APIResponse UseCard(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            UseCardRequest request = new UseCardRequest();

            // Phân tích yêu cầu thành đối tượng UseCardRequest đặt các điều kiện checking tại đây
            try
            {
                request = serializer.Deserialize<UseCardRequest>(transaction.RequestContent);
                if (string.IsNullOrEmpty(request.AccountName))
                    return new APIResponse((int)ResponseCode.AccountNameReuired);
                if (string.IsNullOrEmpty(request.RefCode))
                    return new APIResponse((int)ResponseCode.RefCodeReuired);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "CardTelco", transaction.TransactionID.ToString(), "Error", "Deserialize(requestContent)", ex.Message.Replace("\n", " ") });
                return new APIResponse((int)ResponseCode.RequestContentInvalid);
            }


            //Get Provider

            //Split for Big parnerCode
            var partnerSplit = SplitPartner(transaction.PartnerCode);
            if (request.CardType.ToLower() == "viettel")
                transaction.PartnerCode = partnerSplit;

            var providerCode = new Providers().GetCardCondition(request.CardType.ToLower(), transaction.PartnerCode, request.AmountUser);
            //var providerCode = new Providers().GetCardConditionCache(request.CardType.ToLower());

            if (providerCode.Equals("CTI"))
                return new APIResponse((int)ResponseCode.CardTypeInvalid);

            if (request.AmountUser >= 500000)
            {
                TelegramNotify.SendTeleV2("-4775802643", "Có lệnh nạp thẻ từ đối tác " + transaction.PartnerCode + ", mệnh gía khai báo " + request.AmountUser.ToString("#,#").Replace(",", "."));
            }

            transaction.ProviderCode = providerCode;
            var handler = CardTelcoFactory.GetHandler(providerCode);
            var result = handler.UseCard(transaction);

            //update ballace parent
            //NLogLogger.Info(new string[] { "CardTelco Topup", transaction.PartnerCode, result.ResponseContent, request.CardType.ToLower() });
            if (result.ResponseCode == (int)ResponseCode.TransactionSuccessful || result.ResponseCode == (int)ResponseCode.CardAmountInvalid)
            {
                //NLogLogger.Info(new string[] { "CardTelco Topup", transaction.PartnerCode, result.ResponseContent, request.CardType.ToLower() });
                //if (transaction.PartnerCode == "pp" || transaction.PartnerCode == "huv" || transaction.PartnerCode == "zab")
                //{
                if (!string.IsNullOrEmpty(result.ResponseContent)) // Kiêm tra thêm trường hợp cho CardAmountInvalid đầu vào ko phù hợp
                {
                    //var Amount = Math.Min(long.Parse(result.ResponseContent), request.AmountUser);
                    //var Amount = long.Parse(result.ResponseContent);
                    //Action<string, long, string, string> send = UpdatePartnerBalance;
                    //var asynSend = send.BeginInvoke(transaction.PartnerCode, Amount, request.CardType.ToLower(), request.CardCode, null, null);
                }
                //}
            }
            //if (transaction.PartnerCode == "huv")
            //{
            //    if (result.ResponseCode == (int)ResponseCode.CardAmountInvalid)
            //    {

            //        if (request.AmountUser < long.Parse(result.ResponseContent))
            //        {
            //            TelegramNotify.SendWarning("583426534", $"CẢNH BÁO sai mệnh giá: {request.CardType} TranId: {transaction.TransactionID}, RefCode: {request.RefCode}, CardSerial: {request.CardSerial},  Mệnh giá {request.AmountUser}/{result.ResponseContent}, Partner: {transaction.PartnerCode}");
            //            TelegramNotify.SendWarning("1497473671", $"CẢNH BÁO sai mệnh giá: {request.CardType} TranId: {transaction.TransactionID}, RefCode: {request.RefCode}, CardSerial: {request.CardSerial},  Mệnh giá {request.AmountUser}/{result.ResponseContent}, Partner: {transaction.PartnerCode}");
            //        }

            //    }

            //}

            return result;

            //switch (request.CardType.ToLower())
            //{
            //    case "vms":
            //        return handler.UseCard(transaction);
            //    case "vnp":
            //        return handler.UseCard(transaction);
            //    case "viettel":
            //        return handler.UseCard(transaction);

            //    default:
            //        return new APIResponse();
            //}
        }
        private void UpdatePartnerBalance(string PartnerCode, long Amount, string CardType, string CardCode)
        {
            NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), CardType, CardCode });
            var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, DateTime.Now.Year, DateTime.Now.Month);
            if (listpartnerDiscount == null)
                return;
            if (!listpartnerDiscount.Exists(x => x.Date.Day == DateTime.Now.Day))
                return;

            var _partnerDiscount = listpartnerDiscount.FirstOrDefault(x => x.Date.Day == DateTime.Now.Day);
            decimal ck = 0;
            switch (CardType)
            {
                case "vms":
                    ck = _partnerDiscount.DiscountVMS;
                    break;
                case "vnp":
                    ck = _partnerDiscount.DiscountVNP;
                    break;
                case "viettel":
                    ck = _partnerDiscount.DiscountVTT;
                    break;
                case "zing":
                    ck = _partnerDiscount.DiscountZING;
                    break;
                case "gate":
                    ck = _partnerDiscount.DiscountGATE;
                    break;
            }
            if (ck == 0)
                return;

            long realAmount = Amount - Convert.ToInt64(Amount * ck);
            //NLogLogger.Info(new string[] { "CardTelco Topup", realAmount.ToString(), ck.ToString() });
            new Partners().Topup(realAmount, PartnerCode);

        }
        private APIResponse CheckTrans(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            CheckTransRequest request;

            // Phân tích yêu cầu thành đối tượng UseCardRequest đặt các điều kiện checking tại đây
            try
            {
                request = serializer.Deserialize<CheckTransRequest>(transaction.RequestContent);
                if (string.IsNullOrEmpty(request.RefCode))
                    return new APIResponse((int)ResponseCode.AccountNameReuired);

            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "CardTelco", transaction.TransactionID.ToString(), "Error", "Deserialize(requestContent)", ex.Message.Replace("\n", " ") });
                return new APIResponse((int)ResponseCode.RequestContentInvalid);
            }

            // Kiểm tra trên hệ thống 

            var trans = new CardAPILog().CheckTrans(transaction.PartnerID, request.RefCode);

            if (trans == null || trans.Count == 0)
            {
                return new APIResponse((int)ResponseCode.TransactionNotExists);
            }

            var cardsResponse = new List<CheckCardResponse>();
            foreach (var tran in trans)
            {
                var card = new CheckCardResponse()
                {
                    CardType = tran.CardType,
                    Amount = tran.Amount,
                    CardSerial = tran.CardSerial,
                    RefCode = tran.RequestNo,
                    Status = tran.Status,
                    TransactionID = tran.TransactionID,
                    CreateTime = tran.CreatTime,
                    LasTime = tran.LastTime
                };
                cardsResponse.Add(card);
            }

            return new APIResponse((int)ResponseCode.TransactionSuccessful)
            {
                ResponseContent = serializer.Serialize(cardsResponse)
            };

        }
        public static string SplitPartner(string partnerCode)
        {
            var partnerSplit = partnerCode;
            if (partnerCode == "nut")
            {
                string[] pp = ("nut1,nut2").Split(',');
                Random rd = new Random();
                partnerSplit = pp[rd.Next(0, pp.Length)];
            }

            if (partnerCode == "vnom")
            {
                string[] pp = ("vnom1,vnom2").Split(',');
                Random rd = new Random();
                partnerSplit = pp[rd.Next(0, pp.Length)];
            }

            return partnerSplit;

        }

    }

    public class UseCardRequest
    {
        public string CardSerial { get; set; }
        public string CardCode { get; set; }
        public string CardType { get; set; }
        public string AccountName { get; set; }
        public string AppCode { get; set; }
        public string RefCode { get; set; }
        public int Amount { get; set; }
        public int AmountUser { get; set; }
        public string CallbackUrl { get; set; }
    }

    public class CheckTransRequest
    {
        public string RefCode { get; set; }
    }

    public class CheckCardResponse
    {
        public string CardSerial { get; set; }
        public string CardType { get; set; }
        public long Amount { get; set; }
        public int Status { get; set; }
        public long TransactionID { get; set; }
        public string RefCode { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LasTime { get; set; }
    }

}
