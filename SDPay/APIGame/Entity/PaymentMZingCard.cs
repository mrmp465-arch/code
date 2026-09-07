using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIGame.Entity
{
    public class LoginMData
    {
        public string r { get; set; }
    }

    public class LoginM
    {
        public int returnCode { get; set; }
        public string message { get; set; }
        public LoginMData data { get; set; }
    }

    public class Suggestion
    {
        public string userID { get; set; }
        public int appID { get; set; }
        public List<object> roles { get; set; }
    }

    public class LoginMBillingData
    {
        public string userID { get; set; }
        public string userName { get; set; }
        public string loginType { get; set; }
        public string jtoken { get; set; }
        public object serverID { get; set; }
        public object roleID { get; set; }
        public object roleName { get; set; }
        public object info { get; set; }
        public Suggestion suggestion { get; set; }
    }

    public class LoginMBilling
    {
        public int returnCode { get; set; }
        public string returnMessage { get; set; }
        public string returnMessage_ { get; set; }
        public LoginMBillingData data { get; set; }
    }

    //public class Info
    //{
    //    public string serverName { get; set; }
    //    public string zoneId { get; set; }
    //    public string opendate { get; set; }
    //    public string serverId { get; set; }
    //    public string zoneIP { get; set; }
    //    public double status { get; set; }
    //}

    public class Server
    {
        public string serverID { get; set; }
        public string serverName { get; set; }
        //public object serverAlias { get; set; }
        //public Info info { get; set; }
        //public int status { get; set; }
    }

    public class Role
    {
        public string roleID { get; set; }
        public string roleName { get; set; }
        
    }

    public class ZingGame
    {
        public string serverID { set; get; }
        public string roleID { set; get; }
        public string productID { set; get; }
        public string roleName { set; get; }
        public string amount { set; get; }

    }

    public class ZingCardData
    {
        public long orderNumber { get; set; }
        public string orderNumberStr { get; set; }
        public string orderNumberEncoded { get; set; }
        public string orderStatusMessage { get; set; }
        public string orderDisplayMessage { get; set; }
        public object paymentInstructions { get; set; }
        public object redirectUrl { get; set; }
        public object ztoken { get; set; }
        public object qrCode { get; set; }
        public object balance { get; set; }
        public string smsOtpToken { get; set; }
        public string dbgTransID { get; set; }
        public string dbgStatus { get; set; }
        public object postData { get; set; }
    }

    public class PaymentMZingCardResult
    {
        public int returnCode { get; set; }
        public string returnMessage { get; set; }
        public string returnMessage_ { get; set; }
        public ZingCardData data { get; set; }
    }

    public class Product
    {
        public string productID { get; set; }
        public string productName { get; set; }
        public string productType { get; set; }
        public double price { get; set; }
        public string currency { get; set; }
        public int quantity { get; set; }
        public double conversionRate { get; set; }
        public object conversionDenom { get; set; }
    }

    public class InGameInfo
    {
        public string productType { get; set; }
        public string inGameCurrency { get; set; }
        public double inGameUnitPrice { get; set; }
        public string inGameDescription { get; set; }
    }

    public class OrderExtendData
    {
        public string roleName { get; set; }
        public string serverName { get; set; }
        public string paymentGroupID { get; set; }
        public string paymentGroupName { get; set; }
        public string paymentProviderName { get; set; }
        public string productName { get; set; }
        public string productImage { get; set; }
        public double purchasingPower { get; set; }
        public double discount { get; set; }
        public InGameInfo inGameInfo { get; set; }
        public string inGameUnitAmount { get; set; }
    }

    public class CheckZingCardOrderData
    {
        public object redirectUrl { get; set; }
        public long orderNumber { get; set; }
        public string orderNumberStr { get; set; }
        public string receiptNumber { get; set; }
        public int orderStatus { get; set; }
        public string orderStatusMessage { get; set; }
        public string orderDisplayMessage { get; set; }
        public string orderPointMessage { get; set; }
        public long timestamp { get; set; }
        public List<Product> products { get; set; }
        public OrderExtendData orderExtendData { get; set; }
        public int paymentGatewayID { get; set; }
        public string paymentPartnerID { get; set; }
        public string paymentMethodID { get; set; }
        public string paymentProviderID { get; set; }
        public double paymentGrossAmount { get; set; }
        public double paymentNetAmount { get; set; }
        public string paymentCurrency { get; set; }
        public string userID { get; set; }
        public string serverID { get; set; }
        public string roleID { get; set; }
        public string serverName { get; set; }
        public string roleName { get; set; }
        public string paymentProviderName { get; set; }
        public string productImage { get; set; }
        public string payingAmountStatus { get; set; }
    }

    public class CheckZingCardOrderResult
    {
        public int returnCode { get; set; }
        public string returnMessage { get; set; }
        public string returnMessage_ { get; set; }
        public CheckZingCardOrderData data { get; set; }
    }
}