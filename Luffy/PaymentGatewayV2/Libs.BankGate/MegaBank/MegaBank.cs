using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using Libs.Db;
using Libs.Utils;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;
using System.Web.Script.Serialization;
using Libs.API;

namespace Libs.BankGate.MegaBank
{
    public class MegaBank : IBankGateHandler
    {
        public long TransactionID { get; set; }
        public string merchantid { get; set; }
        public string stan { get; set; }
        public string termtxndatetime { get; set; }
        public string txnAmount { get; set; }
        public string fee { get; set; }
        public string userName { get; set; }
        public string IssuerID { get; set; }
        public string tranID { get; set; }
        public string bankID { get; set; }
        public string mac { get; set; }
        public string respUrl { get; set; }
        public string responsecode { get; set; }
        public string descriptionvn { get; set; }
        public string descriptionen { get; set; }
        public string status { get; set; }
        public string url { get; set; }
        public bool? isConfirm { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? LastTime { get; set; }
        public DateTime? ConfirmTime { get; set; }
        public int ReturnValue { get; set; }

        private string WebserviceUrl = ConfigurationManager.AppSettings["MegaBank_WebserviceUrl"] ?? "http://113.164.227.19:8015/service.asmx";
        private string Merchant_send_key = ConfigurationManager.AppSettings["MegaBank_MerchantSendKey"] ?? "reesatersuusrtiy12312kty";
        private string Merchant_recieve_key = ConfigurationManager.AppSettings["MegaBank_MerchantRecieveKey"] ?? "k43423553535gsgrthkladgt";
        private string MegaBankUrlReturn = ConfigurationManager.AppSettings["MegaBank_UrlReturn"] ?? "http://96.9.75.2:1582/Pages/MegaBank/MegaBankReturn.aspx";
        private string mid = ConfigurationManager.AppSettings["MegaBank_MerchantId"] ?? "TA123";

        private string Country_code = "vn";
        private string PrivateKey = "8b80027f460BC9b2ef1d677adE4f3b02";

        public MegaBank()
        {

        }

        public MegaBank Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<MegaBank>("sp_pay_MegaBank_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }


        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[10];
            pars[0] = new SqlParameter("@ReturnValue", ReturnValue) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@merchantid", mid);
            pars[3] = new SqlParameter("@stan", stan);
            pars[4] = new SqlParameter("@termtxndatetime", termtxndatetime);
            pars[5] = new SqlParameter("@txnAmount", txnAmount);
            pars[6] = new SqlParameter("@fee", fee);
            pars[7] = new SqlParameter("@userName", userName);
            pars[8] = new SqlParameter("@IssuerID", IssuerID);
            pars[9] = new SqlParameter("@bankID", bankID);
            db.ExecuteNonQuerySP("sp_pay_MegaBank_Insert", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[19];
            pars[0] = new SqlParameter("@ReturnValue", ReturnValue) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = string.IsNullOrEmpty(merchantid) ? new SqlParameter("@merchantid", DBNull.Value) : new SqlParameter("@merchantid", merchantid);
            pars[3] = string.IsNullOrEmpty(stan) ? new SqlParameter("@stan", DBNull.Value) : new SqlParameter("@stan", stan);
            pars[4] = string.IsNullOrEmpty(termtxndatetime) ? new SqlParameter("@termtxndatetime", DBNull.Value) : new SqlParameter("@termtxndatetime", termtxndatetime);
            pars[5] = string.IsNullOrEmpty(txnAmount) ? new SqlParameter("@txnAmount", DBNull.Value) : new SqlParameter("@txnAmount", txnAmount);
            pars[6] = string.IsNullOrEmpty(fee) ? new SqlParameter("@fee", DBNull.Value) : new SqlParameter("@fee", fee);
            pars[7] = string.IsNullOrEmpty(userName) ? new SqlParameter("@userName", DBNull.Value) : new SqlParameter("@userName", userName);
            pars[8] = string.IsNullOrEmpty(IssuerID) ? new SqlParameter("@IssuerID", DBNull.Value) : new SqlParameter("@IssuerID", IssuerID);
            pars[9] = string.IsNullOrEmpty(bankID) ? new SqlParameter("@bankID", DBNull.Value) : new SqlParameter("@bankID", bankID);
            pars[10] = string.IsNullOrEmpty(responsecode) ? new SqlParameter("@responsecode", DBNull.Value) : new SqlParameter("@responsecode", responsecode);
            pars[11] = string.IsNullOrEmpty(descriptionvn) ? new SqlParameter("@descriptionvn", DBNull.Value) : new SqlParameter("@descriptionvn", descriptionvn);
            pars[12] = string.IsNullOrEmpty(descriptionen) ? new SqlParameter("@descriptionen", DBNull.Value) : new SqlParameter("@descriptionen", descriptionen);
            pars[13] = string.IsNullOrEmpty(status) ? new SqlParameter("@status", DBNull.Value) : new SqlParameter("@status", status);
            pars[14] = string.IsNullOrEmpty(url) ? new SqlParameter("@url", DBNull.Value) : new SqlParameter("@url", url);
            pars[15] = isConfirm == null ? new SqlParameter("@isConfirm", DBNull.Value) : new SqlParameter("@isConfirm", isConfirm);
            pars[16] = CreateTime == null ? new SqlParameter("@CreateTime", DBNull.Value) : new SqlParameter("@CreateTime", CreateTime);
            pars[17] = LastTime == null ? new SqlParameter("@LastTime", DBNull.Value) : new SqlParameter("@LastTime", LastTime);
            pars[18] = ConfirmTime == null ? new SqlParameter("@ConfirmTime", DBNull.Value) : new SqlParameter("@ConfirmTime", ConfirmTime);
            db.ExecuteNonQuerySP("sp_pay_MegaBank_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public string GetUrlCheckOut(BankGateAPI bankGateAPI)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                DesSecurity encrypter = new DesSecurity();

                var _megaBankRequest = new MegaBank()
                {
                    merchantid = mid,
                    stan = "720527",
                    termtxndatetime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    txnAmount = bankGateAPI.Amount.ToString("####"),
                    fee = (bankGateAPI.TotalAmount - bankGateAPI.Amount).ToString("####"),
                    userName = "huent",
                    IssuerID = "EPAY",
                    tranID = bankGateAPI.TransactionID.ToString(),
                    bankID = bankGateAPI.BankCode, //BankCode.Bank().FirstOrDefault(x => x.Value == bankGateAPI.BankCode).Key,,
                    respUrl = MegaBankUrlReturn,

                };
                var macdata = _megaBankRequest.merchantid + _megaBankRequest.stan + _megaBankRequest.termtxndatetime + _megaBankRequest.txnAmount + _megaBankRequest.fee + _megaBankRequest.userName + _megaBankRequest.IssuerID + _megaBankRequest.tranID + _megaBankRequest.bankID + _megaBankRequest.respUrl;
                NLogLogger.Info(new string[] { "BankGate", bankGateAPI.TransactionID.ToString(), "Mac data", macdata });
                _megaBankRequest.mac = encrypter.DESMAC(macdata, Merchant_send_key);
                //Add DB MegaBank Log
                _megaBankRequest.TransactionID = bankGateAPI.TransactionID;
                _megaBankRequest.CreateTime = DateTime.Now;
                _megaBankRequest.Add();

                Service _megaBankService = new Service(WebserviceUrl);
                NLogLogger.Info(new string[] { "BankGate", bankGateAPI.TransactionID.ToString(), "MegaBank", "Request", serializer.Serialize(_megaBankRequest) });
                var resReponse = _megaBankService.Deposit(_megaBankRequest.merchantid, _megaBankRequest.stan, _megaBankRequest.termtxndatetime, _megaBankRequest.txnAmount, _megaBankRequest.fee, _megaBankRequest.userName, _megaBankRequest.IssuerID, _megaBankRequest.tranID, _megaBankRequest.bankID, _megaBankRequest.mac, _megaBankRequest.respUrl);
                NLogLogger.Info(new string[] { "BankGate", bankGateAPI.TransactionID.ToString(), "MegaBank", "Webservice response", serializer.Serialize(resReponse) });

                if (resReponse.responsecode == "00")
                {
                    bankGateAPI.LogContent = bankGateAPI.LogContent + " | Deposit: " + serializer.Serialize(resReponse);
                    bankGateAPI.UpdateStatus();
                    return resReponse.url;
                }
                else
                {
                    _megaBankRequest.LastTime = DateTime.Now;
                    _megaBankRequest.responsecode = resReponse.responsecode;
                    _megaBankRequest.descriptionvn = resReponse.descriptionvn;
                    _megaBankRequest.descriptionen = resReponse.descriptionen;
                    _megaBankRequest.Update();

                    bankGateAPI.LogContent = bankGateAPI.LogContent + " | Deposit: " + resReponse;
                    bankGateAPI.UpdateStatus();

                    return resReponse.descriptionvn;
                }
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "BankGate", bankGateAPI.TransactionID.ToString(), "Error", "MegaBank", ex.Message.Replace("\n", " ") });
                return "";
            }
        }

        public bool ConfirmTransaction(long transactionID, bool status)
        {
            MegaBank _MegaBank = new MegaBank() { TransactionID = transactionID };
            _MegaBank = _MegaBank.Get();
            if (_MegaBank == null) return false;
            var mac = string.Empty;
            string confirmCode = status ? "00" : "01";
            DesSecurity encrypter = new DesSecurity();
            mac = _MegaBank.merchantid + _MegaBank.TransactionID + _MegaBank.txnAmount + confirmCode;
            mac = encrypter.DESMAC(mac, Merchant_send_key);
            Service _megaBankService = new Service(WebserviceUrl);
            var response = _megaBankService.comfirm(_MegaBank.merchantid, _MegaBank.TransactionID.ToString(), _MegaBank.txnAmount, confirmCode, mac);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            NLogLogger.Info(new string[] { "BankGate", transactionID.ToString(), "MegaBank", "Confirm Response", serializer.Serialize(response) });
            if (response.responsecode != "00") return false;
            return true;
        }

        public string UrlReturn(long transactionID, int status)
        {
            string transactionTime = DateTime.UtcNow.ToString("yyMMddHHmmss");
            string sign = Encrypts.MD5(transactionID.ToString() + status.ToString() + transactionTime + PrivateKey);
            return MegaBankUrlReturn + string.Format("?transactionid={0}&status={1}&transactiontime={2}&sign={3}", transactionID, status, transactionTime, sign);
        }

        public bool CheckUrlReturn(string transid, string responCode, string mac)
        {
            DesSecurity encrypter = new DesSecurity();
            if (string.IsNullOrEmpty(transid) || string.IsNullOrEmpty(responCode) || string.IsNullOrEmpty(mac))
                return false;
            if (mac != encrypter.DESMAC(transid + responCode, Merchant_recieve_key))
                return false;
            return true;
        }

        public ResponseConfirmData Confirm(ReceiveConfirmData receive)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            var megaBank = new MegaBank();
            megaBank.TransactionID = Convert.ToInt64(receive.ResponseNo);
            var transaction = megaBank.Get();
            NLogLogger.Info(new string[] { "BankGate", "MegaBank", "Confirm Tran", serializer.Serialize(transaction) });
            if (transaction.isConfirm == true)
            {
                return new ResponseConfirmData()
                {
                    ResposeCode = (int)ResponseCode.TransactionConfirmed
                };
            }

            if (ConfirmTransaction(transaction.TransactionID, Convert.ToBoolean(receive.ConfirmCode)))
            {

                megaBank.isConfirm = true;
                megaBank.ConfirmTime = DateTime.Now;
                megaBank.Update();

                return new ResponseConfirmData()
                {
                    ResposeCode = (int)ResponseCode.TransactionSuccessful
                };
            }

            return new ResponseConfirmData()
            {
                ResposeCode = (int)ResponseCode.TransactionFailed
            };
        }
    }

}
