using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;
using Libs.Utils;
using System.Globalization;
using System.IO;
using ServiceStack.Common.Extensions;

using System.Net;
using System.Text;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Data;
using System;

public partial class Rut : System.Web.UI.Page
{
    string urlService = "http://127.0.0.1:1592/bankout/cash.ashx";
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    public string QR { get; set; }

    public string MomoId { get; set; }
    public string MomoName
    {
        get; set;
    }
    public string url = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (AppUtils.CheckRolesPermission("rut.aspx"))
        {
        }
        else
        {

            HttpContext.Current.Response.Redirect(Constant.ADMIN_PATH + Resources.Url.Default);
        }
        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
        if (!IsPostBack)
        {
            Init();

            //GetListBankLog();

        }
    }
    public string GetStatusExtra(object statusOver)
    {

        if (statusOver.ToString() == "1")
        {
            return "<div class=\"label label-success\">" + Resources.Pay.Success + "</div>";
        }
        if (statusOver.ToString() == "-1")
        {
            return "<div class=\"label label-danger\">" + Resources.Pay.Fail + "</div>";
        }
        if (statusOver.ToString() == "0")
        {
            return "<div class=\"label label-info\">" + Resources.Pay.Processing + "</div>";
        }



        return "<div class=\"label label-info\">" + statusOver + "</div>";
    }
    private void Init()
    {
        var lst = new List<Partners>();
        if (AppUtils.IsAdmin)
            lst = new Partners().GetList().Where(e => e.Status == 1).ToList();
        else
        {
            //drpPartner.Visible = false;
            //drpCardType.Visible = false;
            lst = new Partners().GetListByUserId(AppUtils.UserID);

            //if (AppUtils.IsPartner)
            //{
            //    foreach (var item in lst)
            //    {
            //        balance += item.Balance;
            //    }
            //}

        }

        var lstbankcode = new BankCodeTranfer().GetListCache();
        // var bankList = BankList.GetBankList();
        drpBankCode.DataSource = lstbankcode;
        drpBankCode.DataTextField = "shortName";
        drpBankCode.DataValueField = "code";
        drpBankCode.DataBind();
        drpBankCode.Items.Insert(0, new ListItem("Chọn Bank:", ""));
        //btView.Text = Resources.Pay.View;
        ////{
        ////    lst = new Partners().GetListByUserId(AppUtils.UserID);
        ////}
        //drpPartner2.DataSource = lst;
        //drpPartner2.DataTextField = "Name";
        //drpPartner2.DataValueField = "PartnerCode";
        //drpPartner2.DataBind();
        //drpPartner2.Items.Insert(0, new ListItem(Resources.Pay.Partner, ""));

        ////drpPartner3.DataSource = lst;
        ////drpPartner3.DataTextField = "Name";
        ////drpPartner3.DataValueField = "PartnerCode";
        ////drpPartner3.DataBind();
        ////drpPartner3.Items.Insert(0, new ListItem("Đối tác:", ""));


        //txtEndTime.Text = DateTime.Now.ToString("MM/dd/yyyy");
        //txtBeginTime.Text = DateTime.Now.ToString("MM/dd/yyyy");


    }

    public string GetInOut(object TotalTrans, object TotalTranSucess)
    {
        var totalTrans = long.Parse(TotalTrans.ToString());
        var totalTranSucess = long.Parse(TotalTranSucess.ToString());


        return (totalTrans - totalTranSucess).ToString("#,#").Replace(".", ",");
    }
    protected void btAdd_Click(object sender, EventArgs e)
    {
        var partner = new Partners().GetCache(AppUtils.UserName);
        Order(partner.PartnerCode, partner.PublicKey);
    }
    //private void GetListBankLog()
    //{

    //    DateTime requestTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddSeconds(-1);

    //    DateTime fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-10);
    //    BankGateAPI _BankGateAPI = new BankGateAPI();
    //    rpBanklog.DataSource = _BankGateAPI.GetTable(50, AppUtils.UserName.Replace("nap",""), string.Empty, fromDate, requestTime, null, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, 1);
    //    rpBanklog.DataBind();
    //}


    private void Order(string partnerCode, string partnerKey)
    {

        if (string.IsNullOrEmpty(txtAmount.Text) || string.IsNullOrEmpty(txtAccountName.Text) || string.IsNullOrEmpty(txtAccountNumber.Text) || string.IsNullOrEmpty(drpBankCode.SelectedValue))
        {
            Page.ClientScript.RegisterClientScriptBlock(
                 this.GetType(),
                 "Script",
                 "$(document).ready(function() { alert('Vui lòng nhập đủ thông tin'); });",
                 true
             );
            return;
        }
      
        var requestContent = new OrderRequest()
        {

            Amount = int.Parse(txtAmount.Text),
            CallbackUrl = "",
            RefCode = partnerCode +"_out_"+ DateTime.Now.ToString("MMddHHmmss"),
            AccountName = txtAccountName.Text,
            AccountNumber = txtAccountNumber.Text,
            BankCode = drpBankCode.SelectedValue,
            PartnerCode = partnerCode,
        };
        var signature = Encrypts.MD5(partnerCode + requestContent.AccountNumber + requestContent.AccountName + requestContent.BankCode + requestContent.Amount + requestContent.RefCode + requestContent.CallbackUrl + partnerKey);
        requestContent.Signature = signature;
        NLogLogger.Info(new string[] { "Bank Test", "Requst Core", urlService, serializer.Serialize(requestContent) });
        var serviceResponse = PostJson(urlService, serializer.Serialize(requestContent));
        NLogLogger.Info(new string[] { "Bank Test", "Response Core", serviceResponse });
        var resObj = serializer.Deserialize<APIResponse>(serviceResponse);
        if (resObj.ResponseCode > 0)
        {

            Page.ClientScript.RegisterClientScriptBlock(
                this.GetType(),
                "Script",
                "$(document).ready(function() { alert('Tạo đơn thành công'); });",
                true
            );
            return;
        }
        else
        {
            Page.ClientScript.RegisterClientScriptBlock(
                 this.GetType(),
                 "Script",
                 "$(document).ready(function() { alert('" + resObj.Description + "'); });",
                 true
             );
            return;
        }


        //Response.Redirect(orderResponse.Url);
    }
    private string PostJson(string uri, string postData)
    {
        var request = (HttpWebRequest)WebRequest.Create(uri);
        request.ContentType = "application/json";
        request.Method = "POST";//GET
                                //request.Accept = "JSON";
        using (Stream requestStream = request.GetRequestStream())
        {
            byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
            requestStream.Write(postDatabytes, 0, postDatabytes.Length);
        }
        var webResponse = request.GetResponse();
        if (webResponse == null)
        {
            return "Unable to connect to the remote server";
        }
        var sr = new StreamReader(webResponse.GetResponseStream());
        return sr.ReadToEnd().Trim();
    }
    public class RequestData
    {
        public string PartnerCode { get; set; }
        public string RequestTime { get; set; }

        public string Signature { get; set; }
    }
    public class APIResponse
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string ResponseContent { get; set; }
        public string Signature { get; set; }
    }

    class OrderResponse
    {
        public string QRCode { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        //public int Amount { get; set; }
        public string OrderNo { get; set; }
        public string Url { get; set; }
        //public int Timeout { get; set; }
    }
    public class MomoAccount
    {

        public string QR { get; set; }
        public string MomoId { get; set; }
        public string MomoName { get; set; }
        //public string Content { get; set; }
    }
    public class OrderRequest
    {
        public string Type { get; set; }
        public string Signature { get; set; }
        public string BankCode { get; set; }
        public string PartnerCode { get; set; }
        public string AccountName { get; set; }
       
        public string AccountNumber { get; set; }
        public string RefCode { get; set; }
        public int Amount { get; set; }
        public string CallbackUrl { get; set; }
    }
}