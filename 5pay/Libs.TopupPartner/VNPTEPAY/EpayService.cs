using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;
using Libs.Utils;
using System.Web.Script.Serialization;

namespace Libs.TopupPartner.VNPTEPAY
{
    public class EpayService 
    {
        

        public EpayService()
        {

        }

        //public APIResponse Topup(APITransaction transaction)
        //{
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    InterfacesService _InterfacesService = new InterfacesService(webserviceUrl);
        //    TopupMobile _TopupMobile = new TopupMobile();
        //    _TopupMobile = _TopupMobile.Get(transaction.TransactionID);

        //    string requestId = _TopupMobile.RequestNo;
        //    string provider = _TopupMobile.Telco;
        //    int type = _TopupMobile.TopupType;
        //    string account = _TopupMobile.Mobile;
        //    long amount = _TopupMobile.Amount;
        //    int timeOut = 120;
        //    string sign = requestId + provider + type + account + amount + timeOut;
        //    sign = Encrypts.RSAGet(sign, privateKey);

        //    NLogLogger.Info(new string[] { "TopupPartner", "TopupMobile", transaction.TransactionID.ToString(), "paymentCDV - Request", requestId, partnerName, provider, type.ToString(), account, amount.ToString(), timeOut.ToString(), sign });
        //    PaymentCdvResult _PaymentCdvResult = _InterfacesService.paymentCDV(requestId, partnerName, provider, type, account, amount, timeOut, sign);
        //    NLogLogger.Info(new string[] { "TopupPartner", "TopupMobile", transaction.TransactionID.ToString(), "paymentCDV - Response", serializer.Serialize(_PaymentCdvResult) });

        //    return new APIResponse();
        //}

        public string downloadSoftpin(string requestId, string provider, int amount, int quantity)
        {
            string ret = string.Empty;
            NLogLogger.Info(" --- Start Topup downloadSoftpin: requestId = " + requestId + ", provider = " + provider + ", amount = " + amount + ", quantity = " + quantity + "------");
            try
            {
                InterfacesService topupWs = new InterfacesService(TopupConstant.webserviceUrl);
                string dataSign = RSASignVerify.SignData(requestId + TopupConstant.TopupPartner + provider + amount.ToString() + quantity.ToString());
                NLogLogger.Info(" --- Start Topup downloadSoftpin: dataSign = " + dataSign + " ------");
                DownloadSoftpinResult result = topupWs.downloadSoftpin(requestId, TopupConstant.TopupPartner, provider, amount, quantity, dataSign);
                //TripleDES trip = new TripleDES();
                //trip.SetKeys = TopupConstant.Topupkey_3DES;
                //trip.SetCipherMode = TripleDES.CipherMode.ElectronicCodebook;
                DesSecurity des = new DesSecurity();
                result.listCards = des.Decryption(result.listCards, TopupConstant.Topupkey_3DES);
                ret = result.ToJSON();
                NLogLogger.Info(" --- Result Topup downloadSoftpin: = " + ret + " ------");
            }
            catch (Exception ex)
            {
                NLogLogger.Info(" --- Exception Topup downloadSoftpin: " + ex.Message + " ------");
            }

            return ret;
        }

        public int checkStore(string provider, int amount)
        {
            int ret;
            //NLogLogger.Info(" --- Start Topup downloadSoftpin: requestId = " + requestId + ", provider = " + provider + ", amount = " + amount + ", quantity = " + quantity + "------");
            try
            {
                InterfacesService topupWs = new InterfacesService(TopupConstant.webserviceUrl);
                string dataSign = RSASignVerify.SignData(TopupConstant.TopupPartner + provider + amount.ToString());
                //NLogLogger.Info(" --- Start Topup downloadSoftpin: dataSign = " + dataSign + " ------");
                ret = topupWs.checkStore(TopupConstant.TopupPartner, provider, amount, dataSign);
                //TripleDES trip = new TripleDES();
                //trip.SetKeys = TopupConstant.Topupkey_3DES;
                //trip.SetCipherMode = TripleDES.CipherMode.ElectronicCodebook;
                //DesSecurity des = new DesSecurity();
                //result.listCards = des.Decryption(result.listCards, TopupConstant.Topupkey_3DES);
                //ret = result.ToJSON();
                //NLogLogger.Info(" --- Result Topup downloadSoftpin: = " + ret + " ------");
            }
            catch (Exception ex)
            {
                ret = 0;
                NLogLogger.Info(" --- Exception Topup downloadSoftpin: " + ex.Message + " ------");
            }

            return ret;
        }

      
    }
}
