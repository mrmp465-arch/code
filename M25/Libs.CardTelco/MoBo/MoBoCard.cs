using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Utils;
using Libs.API;
using System.Security.Cryptography;
using Libs.CardTelco.MoBo;
using Libs.Report;

namespace Libs.CardTelco.MoBo
{
    public class MoBoCard : ICardTelcoHandler
    {

        protected string providerName = string.Empty;

        // Production MoBo
        //protected string ServiceUrl = "https://btcvn.me/v3.0/recharge";
        //protected string client_id = "3ybcucmcna9tdct5vwgycuqg3ujdddkm";
        //protected string key = "P.Huy@3ybcucmcna9t";

        protected string ServiceUrl = "https://joliesa.net/api/scratch/request";
        protected string client_id = "1008";
        protected string key = "87cb9918b611676e8d626c23fc9343e8";

        public APIResponse UseCard(APITransaction transaction)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            APIResponse _APIResponse = new APIResponse();
            CardAPILog _CardAPILog = new CardAPILog();

            providerName = transaction.ProviderCode;

            int step = 0;
            try
            {
                // Bước: Phân tích yêu cầu thành đối tượng
                step = 1;
                UseCardRequest request = new UseCardRequest();
                request = serializer.Deserialize<UseCardRequest>(transaction.RequestContent);

                if (!Utils.GlobalHelper.CheckCardCode(request.CardType, request.CardCode))
                {
                    return new APIResponse((int)ResponseCode.CardCodeInvalid);
                }

                if (!Utils.GlobalHelper.CheckCardSerial(request.CardType, request.CardSerial))

                {
                    return new APIResponse((int)ResponseCode.CardSerialInvalid);
                }

                // Bước: Ghi log giao dịch
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
                _CardAPILog.CallbackUrl = request.CallbackUrl;
                _CardAPILog.Add();

                // Nếu thêm giao dịch không hợp lệ
                if (_CardAPILog.ReturnValue < 0)
                {
                    if (_CardAPILog.ReturnValue == -99)
                    {
                        NLogLogger.Info(new string[] { "MOBO", transaction.TransactionID.ToString(), transaction.PartnerCode, "TopupRequest", request.CardSerial, request.CardCode, "Error Insert Data" });
                        return new APIResponse((int)ResponseCode.TransactionIgnore);
                    }
                    return new APIResponse(_CardAPILog.ReturnValue);
                }

                // Bước: gọi hàm sang API
                step = 3;
                string telcoCode = "";
                switch (_CardAPILog.CardType.ToLower())
                {
                    case "gate":
                        telcoCode = "GATE";
                        break;
                        //case "vnp":
                        //    telcoCode = "VINAPHONE";
                        //    break;
                        //case "viettel":
                        //    telcoCode = "VIETTEL";
                        //    break;
                }

                CardRequest cardRequest = new CardRequest();
                cardRequest.clientId = this.client_id;
                cardRequest.serial = request.CardSerial;
                cardRequest.pinCode = request.CardCode;
                cardRequest.urlCallback = string.Empty;
                cardRequest.customerId = transaction.TransactionID.ToString();
                cardRequest.clientAmount = "0";
                cardRequest.codeType = "GATE";
                cardRequest.customer = request.AccountName;
                //var sign = this.client_id + "&" + cardRequest.customer + "&" + cardRequest.codeType + "&" + cardRequest.serial + "&" + cardRequest.pinCode + "&" + cardRequest.clientAmount + "&" + cardRequest.urlCallback + "&" + cardRequest.customerId + "&" + this.key;
                var sign = $"clientAmount=0&clientId={this.client_id}&codeType={cardRequest.codeType}&customer={cardRequest.customer}&customerId={cardRequest.customerId}&pinCode={cardRequest.pinCode}&serial={cardRequest.serial}&urlCallback={this.key}";
                //NLogLogger.Info(new string[] { "MOBOsign", sign });
                cardRequest.sign = Utils.Encrypts.MD5(sign);
                NLogLogger.Info(new string[] { "MOBO", transaction.TransactionID.ToString(), "MOBORequest", serializer.Serialize(cardRequest) });
                CardResult cardResult = ProcessCard(cardRequest);

                step = 4;
                // Nếu thành công
                if (cardResult.code == 0)
                {
                    _APIResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
                    _CardAPILog.Amount = Convert.ToInt64(cardResult.data.cardInfo.realAmount);
                    _APIResponse.ResponseContent = _CardAPILog.Amount.ToString();
                    _CardAPILog.Description = "Amount: " + _CardAPILog.Amount.ToString();
                    _CardAPILog.Status = 1;

                    var Amount = _CardAPILog.Amount;
                    Action<string, long, string, string, string> send = UpdatePartnerBalance;
                    var asynSend = send.BeginInvoke(transaction.PartnerCode, Amount, request.CardType.ToLower(), String.Format("Cộng tiền nạp thẻ {4} mgd: {0}-{1}-{2}-{3}", _CardAPILog.TransactionID, _CardAPILog.CardType, _CardAPILog.CardSerial, _CardAPILog.CardCode, Amount.ToString("#,#").Replace(",", ".")), "Card_" + _CardAPILog.TransactionID, null, null);
                }
                else
                {
                    _APIResponse = new APIResponse(MoboCardLib.ConvertResponCode(cardResult.code));
                    _CardAPILog.Status = _APIResponse.ResponseCode;
                    _CardAPILog.Description = serializer.Serialize(cardResult);
                }
            }
            catch (Exception ex)
            {
                
                switch (step)
                {
                    case 1:
                        NLogLogger.Info(new string[] { "MOBO", transaction.TransactionID.ToString(), "Error", "UseCard", "Step1", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.RequestContentInvalid);
                        break;
                    case 2:
                        NLogLogger.Info(new string[] { "MOBO", transaction.TransactionID.ToString(), "Error", "UseCard", "Step2", ex.Message.Replace("\n", " ") });
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 3:
                        NLogLogger.Info(new string[] { "MOBO", transaction.TransactionID.ToString(), "Error", "UseCard", "Step3", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Error - UseCard " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    case 4:
                        NLogLogger.Info(new string[] { "MOBO", transaction.TransactionID.ToString(), "Error", "UseCard", "Step4", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                    default:
                        NLogLogger.Info(new string[] { "MOBO", transaction.TransactionID.ToString(), "Error", "UseCard", ex.Message.Replace("\n", " ") });
                        _CardAPILog.Description = "Step4 " + ex.Message.Replace("\n", " ");
                        _APIResponse = new APIResponse((int)ResponseCode.SystemError);
                        break;
                }

                _CardAPILog.Status = _APIResponse.ResponseCode;
            }

            if (_CardAPILog.TransactionID > 0)
            {
                _CardAPILog.Update();
            }
            return _APIResponse;
        }
        private void UpdatePartnerBalance(string PartnerCode, long Amount, string CardType, string Note, string RefCode)
        {
            try
            {
                NLogLogger.Info(new string[] { "Update Balance", PartnerCode, Amount.ToString(), CardType, Note });
                var partner = new Partners().GetCache(PartnerCode);
                if (string.IsNullOrEmpty(partner.Hotline))
                {
                    //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                    return;
                }
                var user = new Users().GetByUserName(partner.Hotline.Trim());
                if (user == null)
                {
                    //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật tài khoản đối ứng cho đối tác " + PartnerCode);
                    return;
                }
                var listpartnerDiscount = new PartnersDiscount().GetList(PartnerCode, DateTime.Now.Year, DateTime.Now.Month);
                if (listpartnerDiscount == null)
                {
                    //TelegramNotify.SendTeleV2("-4006848376", "Chưa cập nhật chiếu khấu bank cho đối tác " + PartnerCode);
                    return;
                }

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
                new Users().Topup(realAmount, user.UserName, PartnerCode, Note, RefCode);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(ex.Message);
            }
        }
        private CardResult ProcessCard(CardRequest cardRequest)
        {
            //string parameters = "clientid={0}&serial={1}&pin={2}&callback={3}";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string responseData = MoboCardLib.PostJson(ServiceUrl, serializer.Serialize(cardRequest));
            NLogLogger.Info(new string[] { "MOMO", cardRequest.customerId.ToString(), "MOBOResponseRaw", responseData });
            return serializer.Deserialize<CardResult>(responseData);
        }


        public APIResponse ReCheck(string transactionId)
        {
            throw new NotImplementedException();
        }

    }


}
