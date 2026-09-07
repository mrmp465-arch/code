using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using Libs.Utils;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using OfficeOpenXml;


public partial class Pages_BankEWalletService_Bank_Transaction : System.Web.UI.Page
{
    public JavaScriptSerializer serializer = new JavaScriptSerializer();
    protected List<BankAccounts> lstBank;
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankTransaction);

        if (!IsPostBack)
        {
            try
            {
                init();
                GetList();
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "Pages_Bank_Transaction", "Page_Load", exp.Message });
            }

        }
    }
    private void init()
    {

        //txtCreatTime.Text = DateTime.Now.AddDays(1).ToString();
        txtCreatTime.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(2).AddSeconds(-1).ToString("dd/MM/yyyy HH:mm:ss");
        txtFromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-30).ToString("dd/MM/yyyy HH:mm:ss");

        var lst = new PartnerBank().GetListPartner();
        lst = lst.OrderBy(x => x.Code).ToList();

        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "Code";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Kênh:", ""));
        drpPartner.Items.Insert(2, new ListItem("ext", "ext"));
    }
    //protected void btView2_Click(object sender, EventArgs e)
    //{
    //    var lstdata = new BankTransaction().GetListCallback();
    //    foreach (var item in lstdata)
    //    {
    //        var apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
    //        apiResponse.ResponseContent = item.RequestContent;
    //        NLogLogger.Logger.Debug(item.Id);
    //        System.Threading.Thread.Sleep(100);
    //        var unused = PostJson(item.CallbackUrl, serializer.Serialize(apiResponse));

    //    }
    //}
    private void GetList()
    {
        try
        {
            lstBank = new BankAccounts().GetList().OrderBy(x => x.Id).ToList();

            int top = Convert.ToInt32(drpTop.SelectedValue);
            //DateTime creatTime = AppUtils.ToDateTime(txtCreatTime.Text);
            //DateTime creatTime = ToDateTime(txtCreatTime.Text);
            DateTime creatTime = ToDateTime(txtCreatTime.Text);
            DateTime fromDate = ToDateTime(txtFromDate.Text);
            int? status = null;
            int? amount = null;
            int? minamount = null;
            int? maxamount = null;
            Int64? id = AppUtils.ToInt64(txtId.Text);
            if (id == 0) id = null;

            //if (drpStatus.SelectedValue != "-1")
            //    status = AppUtils.ToInt32(drpStatus.SelectedValue);

            if (string.IsNullOrEmpty(txtStatus.Text))
                status = null;
            else
                status = AppUtils.ToInt32(txtStatus.Text);

            if (string.IsNullOrEmpty(txtAmount.Text))
                amount = null;
            else
                amount = AppUtils.ToInt32(txtAmount.Text);

            if (string.IsNullOrEmpty(txtMinAmount.Text))
                minamount = null;
            else
                minamount = AppUtils.ToInt32(txtMinAmount.Text);

            if (string.IsNullOrEmpty(txtMaxAmount.Text))
                maxamount = null;
            else
                maxamount = AppUtils.ToInt32(txtMaxAmount.Text);


            var lstdata = new BankTransaction().GetList(top, id, drpPartner.SelectedValue, txtPartnerBankId.Text, txBankTransId.Text, drpPartnerBankCode.SelectedValue, txtBankId.Text, drpType.SelectedValue, creatTime, status, amount,minamount,maxamount, txtRefCode.Text, txtComment.Text, cbComentNull.Checked, fromDate);
            foreach (var item in lstdata)
            {
                if (item.CommandCode == "IN")
                    item.RefCode = "";
                if (item.CommandCode == "OUT")
                {
                    if (item.CommandCode == "OUT")
                    {
                        if (lstBank.Exists(x => x.BankCode == item.PartnerBankCode && x.BankId == item.PartnerBankId))
                        {
                            item.PartnerBankCode = lstBank.FirstOrDefault(x => x.BankCode == item.PartnerBankCode && x.BankId == item.PartnerBankId).Computer + " - " + lstBank.FirstOrDefault(x => x.BankCode == item.PartnerBankCode && x.BankId == item.PartnerBankId).PhoneDevice + " - " + item.PartnerBankCode;
                        }
                    }
                }



                //if (item.CommandCode != "IN")
                //    item.PartnerCode = "";
            }
            rptList.DataSource = lstdata;
            rptList.DataBind();
        }
        catch (Exception ex)
        {
            NLogLogger.Info(ex.Message);
            Response.Redirect(Constant.ADMIN_PATH + "500.html");
        }
    }
    protected void SendTele_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        long Id = Convert.ToInt64(e.CommandArgument.ToString());
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        BankTransaction callback = new BankTransaction();
        callback.Id = Id;

        //CallBack Partner
        callback = callback.Get();
        var _Bank = new BankAccounts();
        var data = _Bank.GetList().OrderBy(x => x.BankName).ToList();
        String PartnerBanKName = data.FirstOrDefault(x=>x.BankId==callback.PartnerBankId).BankName;
        var mess = String.Format("<b>Tài khoản</b>: ({0}) {1} -  {2} %0A<b>Mã giao dịch</b>: {3} %0A<b>Số tiền</b>: %2B{4} %0A<b>Thời gian</b>: {5} %0A<b>Nội dung</b>: {6}",callback.PartnerBankCode,callback.PartnerBankId, PartnerBanKName, callback.BankTransId,callback.Amount.GetValueOrDefault().ToString("#,#").Replace(",", "."), callback.CreatedTime.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm:ss"), callback.CommentOrg);
        NLogLogger.Info("mess " + mess);
        TelegramClient.SendTeleV4("-1002982506743", mess);

        GetList();
    }
    protected void Callback_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        long Id = Convert.ToInt64(e.CommandArgument.ToString());
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        BankTransaction _momoTranLog = new BankTransaction();
        _momoTranLog.Id = Id;

        //CallBack Partner
        _momoTranLog = _momoTranLog.Get();
        var apiResponse = new APIResponse((int)ResponseCode.TransactionSuccessful);
        apiResponse.ResponseContent = _momoTranLog.RequestContent;
        var unused = PostJson(_momoTranLog.CallbackUrl, serializer.Serialize(apiResponse));
        GetList();
    }
    public static async Task<string> PostJson(string url, string postData)
    {

        //NLogLogger.Info(new string[] { "MDrum", "Callback", "Partner", "Request", postData });
        var uri = new Uri(url);
        var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(60);
        try
        {
            NLogLogger.Info(new string[] { "CMS", "R", "Request", postData });
            var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                NLogLogger.Info(new string[] { "CMS", "R", "Response", responseContent });
                client.Dispose();
                return responseContent;
            }
            else
            {
                NLogLogger.Info(new string[] { "CMS", "Callback", "Partner", "Response Is Null" });
            }

        }
        catch (Exception e)
        {
            NLogLogger.Info(new string[] { "CMS", "Exeption Post", e.Message });
            return string.Empty;
        }
        client.Dispose();
        return string.Empty;
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }

    protected string CheckTranOutUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.CheckBankTranOut + "?id=" + id;
    }

    protected string EditTranInUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.EditBankTranIn + "?id=" + id;
    }

    protected void AddTranIn_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.AddBankTranIn);
    }
    public DateTime ToDateTime(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            try
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                return DateTime.Parse(value, cul);
                //return Convert.ToDateTime(value);
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }
        return DateTime.Now;
    }
    public string GetCodeStatus(string type, string sn, string code,string fullcode)
    {
        if (type == "IN")
        {
            if ((fullcode.Contains("SYS") && code.Any(char.IsDigit)) )
            {
                return "THÀNH CÔNG";
            }
            if (sn == "sn")
            {
                return "THÀNH CÔNG";
            }
            else
            {
                if(code== "[COMMENT_INCORRECT_FORMAT]"|| code == "[COMMENT_INCORRECT]" || code.ToUpper() == "[ORDER_NOTFOUND]")
                {
                    return "<div style='color:red'>SAI CODE</div>";
                }
                else
                {
                    if (code.ToUpper() == "[ORDER_DUPLICATE]")
                    {
                        return "<div style='color:red'>TRÙNG CODE</div>";
                    }
                    else
                    {
                        if(string.IsNullOrEmpty(code)|| fullcode.Contains("SYS"))
                        {
                            return "<div style='color:red'>SAI CODE</div>";
                        }
                        else
                        {
                            if (code.ToUpper() == "[ORDER_WRONGAMOUNT]")
                            {
                                return "<div style='color:red'>SAI SỐ TIỀN</div>";
                            }
                            if (code.ToUpper() == "[ORDER_OVERTIME]")
                            {
                                return "<div style='color:red'>OVERTIME</div>";
                            }
                            return code;
                        }
                        
                    }
                }
            }
        }
        return "";
    }
    protected void ExportTran2_Click(object sender, EventArgs e)
    {
        lstBank = new BankAccounts().GetList().OrderBy(x => x.Id).ToList();

        int top = Convert.ToInt32(drpTop.SelectedValue);
        //DateTime creatTime = AppUtils.ToDateTime(txtCreatTime.Text);
        //DateTime creatTime = ToDateTime(txtCreatTime.Text);
        DateTime creatTime = ToDateTime(txtCreatTime.Text);
        DateTime fromDate = ToDateTime(txtFromDate.Text);
        int? status = null;
        int? amount = null;
        int? minamount = null;
        int? maxamount = null;
        Int64? id = AppUtils.ToInt64(txtId.Text);
        if (id == 0) id = null;

        //if (drpStatus.SelectedValue != "-1")
        //    status = AppUtils.ToInt32(drpStatus.SelectedValue);

        if (string.IsNullOrEmpty(txtStatus.Text))
            status = null;
        else
            status = AppUtils.ToInt32(txtStatus.Text);

        if (string.IsNullOrEmpty(txtAmount.Text))
            amount = null;
        else
            amount = AppUtils.ToInt32(txtAmount.Text);

        if (string.IsNullOrEmpty(txtMinAmount.Text))
            minamount = null;
        else
            minamount = AppUtils.ToInt32(txtMinAmount.Text);

        if (string.IsNullOrEmpty(txtMaxAmount.Text))
            maxamount = null;
        else
            maxamount = AppUtils.ToInt32(txtMaxAmount.Text);


        var data = new BankTransaction().GetList(200000, id, drpPartner.SelectedValue, txtPartnerBankId.Text, txBankTransId.Text, drpPartnerBankCode.SelectedValue, txtBankId.Text, drpType.SelectedValue, creatTime, status, amount, minamount, maxamount, txtRefCode.Text, txtComment.Text, cbComentNull.Checked, fromDate);

        var lstData = new List<BankGateAPIExcel>();
        foreach (var bank in data)
        {
            var item = new BankGateAPIExcel();
            item.Id = bank.Id;
            item.UpdateTime = bank.UpdateTime.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm:ss"); ;
            item.CreatedTime = bank.CreatedTime.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm:ss");
            item.BankCode = bank.BankCode;
            item.BankId = bank.BankId;
            item.PartnerBankCode = bank.PartnerBankCode;
            item.BankTransId = bank.BankTransId;
            item.PartnerBankId = bank.PartnerBankId;
            item.CommentOrg = bank.CommentOrg;
            item.Amount = Convert.ToInt32(bank.Amount);
            item.Comment = bank.Comment;
            item.RefCode = bank.RefCode;
            lstData.Add(item);
        }

        ExportToExcel(lstData, "transaction-" + fromDate.ToString("ddMMyyy"));
    }
    protected void ExportToExcel(List<BankGateAPIExcel> data, string name)
    {
        using (var package = new ExcelPackage())
        {
            var ws = package.Workbook.Worksheets.Add("Data");
            ws.Cells["A1"].LoadFromCollection(data, true);

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + name + ".xlsx");
            Response.BinaryWrite(package.GetAsByteArray());
            Response.End();
        }
    }
    public class BankGateAPIExcel
    {
        public long Id { get; set; }
        public string PartnerBankCode { get; set; }
        public string PartnerBankId { get; set; }
        public string BankCode { get; set; }
        public string BankId { get; set; }


        public string RefCode { get; set; }

        public string BankTransId { get; set; }
        public int Amount { get; set; }

        public string CommentOrg { get; set; }
        public string Comment { get; set; }
        public string CreatedTime { get; set; }
        public string UpdateTime { get; set; }



    }
}