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

using System.Net;
using System.Text;

using System.Data;
using System;
using System.Drawing.Imaging;
using System.Drawing;
using QRCoder;


namespace BankGateV2.Pages
{
    public partial class Nap : System.Web.UI.Page
    {
        string urlService = "http://127.0.0.1:1592/bankin/Order.ashx";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public string QR { get; set; }

        public string MomoId { get; set; }
        public string MomoName
        {
            get; set;
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (HttpContext.Current.Session["UserName"] == null)
                {
                    HttpContext.Current.Response.Redirect("/pages/Login.aspx");
                    return;
                }
                Init();
                //BindData();
                GetListBankLog();

            }
        }
        private void Init()
        {
           // txtAmount3.Text = "5000000";

        }
        //protected void BindData()
        //{
        //    try
        //    {
        //        DateTime begintime = AppUtils.DateTimeParseExact(txtBeginTime.Text);
        //        DateTime endtime = AppUtils.DateTimeParseExact(txtEndTime.Text);
        //        endtime = endtime.AddDays(1).AddMilliseconds(-1);
        //        string partnerCodes = drpPartner2.SelectedValue;


        //        if (AppUtils.IsPartner && !AppUtils.IsAdmin)
        //        {

        //            if (string.IsNullOrEmpty(partnerCodes))
        //            {
        //                var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
        //                if (lstPartner != null && lstPartner.Count > 0)
        //                {
        //                    partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode).ToArray());
        //                }
        //            }

        //        }
        //        var lstDataBank = new BankGateAPI().ReportDashboard(partnerCodes, begintime, endtime);

        //        var lstData = new List<BankDashboardReport>();
        //        var item = new BankDashboardReport();
        //        item.Type = "MOMO";

        //        if (lstDataBank.Exists(x => x.Type == 1))
        //        {
        //            var data = lstDataBank.FirstOrDefault(x => x.Type == 1);
        //            item.TotalFee = data.TotalFee;
        //            item.TotalTrans = data.TotalTrans;
        //            item.TotalTransSuccess = data.TotalTransSuccess;
        //            item.TotalAmountSuccess = data.TotalAmountSuccess;
        //        }
        //        else
        //        {
        //            item.TotalFee = 0;
        //            item.TotalTrans = 0;
        //            item.TotalTransSuccess = 0;
        //            item.TotalAmountSuccess = 0;
        //        }

        //        lstData.Add(item);

        //        rptListBank.DataSource = lstData;
        //        rptListBank.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        NLogLogger.Info(ex.Message);
        //        Response.Redirect(Constant.ADMIN_PATH + "500.html");
        //    }
        //}
        //protected void btView_Click(object sender, EventArgs e)
        //{

        //    BindData();

        //}
        public string GetInOut(object TotalTrans, object TotalTranSucess)
        {
            var totalTrans = long.Parse(TotalTrans.ToString());
            var totalTranSucess = long.Parse(TotalTranSucess.ToString());


            return (totalTrans - totalTranSucess).ToString("#,#").Replace(".", ",");
        }
        protected void btAdd_Click(object sender, EventArgs e)
        {

            Order("paytest", "0baba54aae3c2700686926526edb6e87");
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

            //if (string.IsNullOrEmpty(txtAmount3.Text))
            //    return;
            var requestContent = new OrderRequest()
            {

                Amount = int.Parse(txtAmount3.Text),
                CallbackUrl = "https://test.1ppay.org/callback.ashx",
                RefCode = DateTime.Now.ToString("MMddHHmmss"),
                BankCode = "random",
                PartnerCode = partnerCode,
            };
            var signature = Encrypts.MD5(partnerCode + requestContent.BankCode + requestContent.Amount + requestContent.RefCode + requestContent.CallbackUrl + partnerKey);
            requestContent.Signature = signature;
            NLogLogger.Info(new string[] { "Bank Test", "Requst Core", urlService, serializer.Serialize(requestContent) });
            var serviceResponse = PostJson(urlService, serializer.Serialize(requestContent));
            NLogLogger.Info(new string[] { "Bank Test", "Response Core", serviceResponse });
            var resObj = serializer.Deserialize<APIResponse>(serviceResponse);


            if (resObj.ResponseCode > 0)
            {
                var orderResponse = serializer.Deserialize<OrderResponse>(resObj.ResponseContent);
                dvBankInfo.Visible = true;
                //lbAccountName.Text = orderResponse.BankAccountName;
                lbAccountNumber.Text = orderResponse.AccountNumber;
                MomoId = orderResponse.AccountName;
                MomoName = orderResponse.AccountNumber;

                lbAccountName.Text = orderResponse.AccountName;
                lbContent.Text = orderResponse.OrderNo;
                qrCode.Width = 250;
                qrCode.ImageUrl = GenerateQrBase64(orderResponse.QRCode);
                GetListBankLog();
            }
            else
            {

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
        public class OrderRequest
        {
            public string Type { get; set; }
            public string Signature { get; set; }
            public string BankCode { get; set; }
            public string PartnerCode { get; set; }
            //public string BankAccountName { get; set; }
            //public string AppCode { get; set; }
            public string RefCode { get; set; }
            public int Amount { get; set; }
            public string CallbackUrl { get; set; }
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
            public string Url { get; set; }
            public string AccountName { get; set; }
            public string AccountNumber { get; set; }
            public string BankName { get; set; }
            public string QRCode { get; set; }
            public string OrderNo { get; set; }
            //public int Timeout { get; set; }
        }
        public class MomoAccount
        {

            public string QR { get; set; }
            public string MomoId { get; set; }
            public string MomoName { get; set; }
            //public string Content { get; set; }
        }
        public static string GenerateQrBase64(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);

            QRCode qrCode = new QRCode(qrCodeData);
            using (Bitmap qrImage = qrCode.GetGraphic(20))
            {
                using (var ms = new MemoryStream())
                {
                    qrImage.Save(ms, ImageFormat.Png);
                    byte[] bytes = ms.ToArray();
                    return "data:image/png;base64," + Convert.ToBase64String(bytes);
                }
            }
        }
        private void GetListBankLog()
        {

            DateTime requestTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddSeconds(-1);

            DateTime fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-10);
            BankGateAPI _BankGateAPI = new BankGateAPI();
            rpBanklog.DataSource = _BankGateAPI.GetTable(20, "paytest", string.Empty, fromDate, requestTime, null, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, 1);
            rpBanklog.DataBind();
        }
        public string GetStatusExtra(object statusOver)
        {

            if (statusOver.ToString() == "1")
            {
                return "<div class=\"label label-success\">Thành công</div>";
            }
            if (statusOver.ToString() == "-1")
            {
                return "<div class=\"label label-danger\">Thất bại</div>";
            }
            if (statusOver.ToString() == "0")
            {
                return "<div class=\"label label-info\">Đagn xử lý</div>";
            }



            return "<div class=\"label label-info\">" + statusOver + "</div>";
        }

    }
}