using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.Utils;

namespace BankGateTest
{
    public partial class Bank : System.Web.UI.Page
    {
        private string partnerCode = "pp";
        private string partnerKey = "0675e5889dd17f15c9e71f25c8f1dd20";
        private string callBack = "http://localhost:62526/callback.aspx";
        private string payUrl = "http://96.9.75.2:1582/checkout.aspx";
        protected void Page_Load(object sender, EventArgs e)
        {
            //txtOrderNo.Text = NewOrderNo();
            var log = new LogInfo
            {
                LogTime = DateTime.Now,
                Url = "abc",
                TransactionID = 1,
                Request = "postData",
                Respone = "responseContent"
            };
            LogCache.LogBankCash(log);
        }

        protected void btnPay_Click(object sender, EventArgs e)
        {
            //var requestTime = "20180802083021";
            var requestTime = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var signature = partnerCode + txtOrderNo.Text.Trim() + txtOrderInfo.Text.Trim() + txtAmount.Text.Trim() + callBack.Trim() + requestTime.Trim() + partnerKey.Trim();
            signature = Encrypts.MD5(signature.ToLower());
            var orderInfo = HttpUtility.UrlEncode(txtOrderInfo.Text.Trim());
            var returnUrl = HttpUtility.UrlEncode(callBack);
            string url = payUrl + string.Format("?partnercode={0}&orderno={1}&orderinfo={2}&amount={3}&returnurl={4}&requesttime={5}&signature={6}&fullname={7}&mobile={8}", partnerCode, txtOrderNo.Text, orderInfo, txtAmount.Text, returnUrl, requestTime, signature,txtFullName.Text,txtMobile.Text);
            Response.Redirect(url);
        }

        public string NewOrderNo()
        {
            //return "MECGHAI1533198621";
            return new Orders().GenOrderCode();
        }
    }
}