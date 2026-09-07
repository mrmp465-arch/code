using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using Libs.Utils;
using System.Globalization;
using System.Data;






public partial class Pages_Security_UserDeposit : System.Web.UI.Page
{
    public string UrlHistory;
    public bool IsDL { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.UserDeposit);
        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
        btView.Text = Resources.Pay.View;
        if (AppUtils.IsAdmin)
        {
            if (!AppUtils.CheckRolesPermission("pages/security/userdepositadd.aspx"))
                dvNap.Visible = false;
        }
        if (AppUtils.IsTopup)
        {
            var partnerlist = new Partners().GetList();
            if (partnerlist.Exists(x => x.SMSUrl == AppUtils.UserName))
            {
                IsDL = true;
            }
        }
        if (!IsPostBack)
        {
            init();
            GetList();
        }

    }

   
    private void init()
    {
        txtCreatTime.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddSeconds(-1).ToString("dd/MM/yyyy HH:mm:ss");
        txtFromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("dd/MM/yyyy HH:mm:ss");


        var lst = new List<Users>();
        if (AppUtils.IsAdmin)
        {
            //var lstPartner = new Partners().GetList().Where(x => x.Hotline != "").GroupBy(x => x.Hotline).Select(a => a.Key);
            var lstUser = new Users().GetList().OrderBy(x => x.UserName);
            foreach (var item in lstUser)
            {
                if (item.IsPartner == 1||item.IsTopup==1)
                    lst.Add(new Users { UserName = item.UserName });
            }
        }
        else
        {
            lst.Add(new Users { UserName = AppUtils.UserName });
        }

        lst = lst.OrderBy(x => x.UserName).ToList();
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "UserName";
        drpPartner.DataValueField = "UserName";
        drpPartner.DataBind();


        ddlAccount.DataSource = lst;
        ddlAccount.DataTextField = "UserName";
        ddlAccount.DataValueField = "UserName";
        ddlAccount.DataBind();
        ddlAccount.Items.Insert(0, new ListItem("Tài khoản:", ""));


        drpStatus.Items.Insert(0, new ListItem(Resources.Pay.Status, "-99"));
        drpStatus.Items.Insert(1, new ListItem(Resources.Pay.Finish, "1"));
        drpStatus.Items.Insert(2, new ListItem(Resources.Pay.WaitApproval, "0"));
        //drpStatus.Items.Insert(2, new ListItem(Resources.Pay.Processing, "0"));
        drpStatus.Items.Insert(3, new ListItem(Resources.Pay.Cancel, "-1"));

        if (AppUtils.IsAdmin)
        {
            drpPartner.Items.Insert(0, new ListItem("Tài khoản:", ""));
            drpTop.SelectedValue = "500";
        }
        else
        {
            drpPartner.Visible = false;

        }
        //var name = Request["name"];
        //if (!string.IsNullOrEmpty(name))
        //{
        //    var user = new Users().GetByUserName(name);

        //    lblTotal.Text = " Số dư: " + user.Balance.ToString("#,#").Replace(",", ".");
        //    UrlHistory = Constant.ADMIN_PATH + Resources.Url.UsersHistory + "?name=" + name;
        //    drpPartner.SelectedValue = name;
        //    dvUserInfo.Visible = true;
        //}



    }
    public string GetStatus(object statusOver)
    {
        if (statusOver.ToString() == "1")
        {
            return "<div class=\"label label-success\"> " + Resources.Pay.Finish + " </div>";
        }
        if (statusOver.ToString() == "-1")
        {
            return "<div class=\"label label-danger\">  " + Resources.Pay.Cancel + " </div>";
        }
        if (statusOver.ToString() == "0")
        {
            return "<div class=\"label label-info\"> " + Resources.Pay.WaitApproval + "  </div>";
        }

        return "";
    }
    protected void btAdd_Click2(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ddlAccount.SelectedValue) || string.IsNullOrEmpty(txtAmount2.Text))
        {
            AlertInfoss.Text = "Không đủ thông itn";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }
        long amount = long.Parse(txtAmount2.Text);
        if (amount < 10)
        {
            AlertInfoss.Text = "Số tiền phải lớn hơn bằng 10M";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }

        var _tran = new UserDeposit();
        _tran.UserName = ddlAccount.SelectedValue;
        _tran.Amount = long.Parse(txtAmount.Text);;
        _tran.Note = txtNote.Text;
        _tran.Money = long.Parse(txtAmount2.Text);
        _tran.Admin = AppUtils.UserName;
        var lstPartner = new Partners().GetList().Where(x => x.Hotline != "").GroupBy(x => x.Hotline).Select(a => a.Key).ToList();

        if (lstPartner.Exists(x => x.Equals(_tran.UserName)))
        {
            var result = _tran.Add();
            if (result < 0)
            {

                AlertBans.Text = "Số dư không đủ";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertBan').modal()}); ", true);
                return;

            }
            TelegramClient.SendTeleV2("-1003928584819", "Tạo lệnh nạp tiền  " + amount.ToString("#,#").Replace(",", ".") + " từ user " + AppUtils.UserName + " Mã lệnh " + result.ToString());
            AlertSuccesss.Text = "Tạo lệnh thành công";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);

        }
        else
        {
           
            _tran.Add2();
            AlertSuccesss.Text = "Tạo lệnh thành công";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);

        }


        txtAmount2.Text = "";

        System.Threading.Thread.Sleep(200);
        Response.Redirect(Request.RawUrl);
        //GetList();


    }
  
    protected string getbankCodetext(string[] lstdata)
    {
        foreach (var item in lstdata)
        {
            if (item.Contains("Bank: "))
            {
                var bank = item.Replace("Bank: ", "").Trim().TrimEnd();
                return getBankCode2(bank);
            }
        }
        return "";
    }
    protected string getbankNumbertext(string[] lstdata)
    {
        foreach (var item in lstdata)
        {
            if (item.Contains("Bank Number: "))
            {
                return item.Replace("Bank Number: ", "").Trim().TrimEnd();

            }
        }
        return "";
    }
    protected string getMoneytext(string[] lstdata)
    {
        foreach (var item in lstdata)
        {
            if (item.Contains("Money: "))
            {
                return item.Replace("Money: ", "").Trim().TrimEnd().Replace(".", "");

            }
        }
        return "";
    }
    protected string getContent(string[] lstdata)
    {
        foreach (var item in lstdata)
        {
            if (item.Contains("Content: "))
            {
                return item.Replace("Content: ", "").Trim().TrimEnd();

            }
        }
        return "";
    }
    
    private void GetList()
    {
        //var name = Request["name"];
        //if (!string.IsNullOrEmpty(name))
        //{
        //    var user = new Users().GetByUserName(name);

        //    lblTotal.Text = " Số dư: " + user.Balance.ToString("#,#").Replace(",", ".");
        //    dvUserInfo.Visible = true;
        //    UrlHistory = Constant.ADMIN_PATH + Resources.Url.UsersHistory + "?name=" + name;
        //}
        //if (string.IsNullOrEmpty(name))
        //    name = "";

        DateTime requestTime = ToDateTime(txtCreatTime.Text);
        DateTime fromDate = ToDateTime(txtFromDate.Text);
        int status = int.Parse(drpStatus.SelectedValue);
        int top = int.Parse(drpTop.SelectedValue);
        var name = drpPartner.SelectedValue;
        if (AppUtils.IsAdmin)
        {
            var lstDataBank= new UserDeposit().GetList(top, name, status, fromDate, requestTime);
           
            //if (cbByUsdt.Checked == true)
            //{
            //    lstDataBank = lstDataBank.Where(m => m.UserName =="cn02").ToList();

            //}
            rptList.DataSource = lstDataBank;
            //NLogLogger.Info(new string[] { "BindData", "BindData", requestTime.ToString(), fromDate.ToString() });
            rptList.DataBind();
            //var lstDataBanksu = lstDataBank.Where(m => m.Status == 1 && m.UserName == "cn02");
            //long Money = lstDataBanksu.Sum(x => x.Money);
            //long Amount = lstDataBanksu.Sum(x => x.Amount);
            //if(Money>0)
            //{
            //    int rate = (int)(Amount / Money);
            //    lblTotalReport.Text = String.Format("{0} : {1} - {2} : {3} - {4} : {5} - {6} : {7}", "Số lệnh nạp", lstDataBanksu.Count(), "Số tiền VND ", Amount.ToString("N0").Replace(".", ","), "Số usdt", Money, "Tỷ giá trung bình", rate);

            //}

        }
        else
        {
            name = AppUtils.UserName;
            if (!AppUtils.IsPartner && !IsDL)
            {


                var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
                name = lstPartner.FirstOrDefault().Name;
            }
            rptList.DataSource = new UserDeposit().GetList(top, name, status, fromDate, requestTime);
            rptList.DataBind();
            var user = new Users().GetByUserName(name);

            //lblTotal.Text = Resources.Pay.Balance + " : " + user.Balance.ToString("#,#").Replace(",", ".");
            //dvUserInfo.Visible = true;
            UrlHistory = Constant.ADMIN_PATH + "pages/security/myhistory.aspx";
        }


    }
    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
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
    public string getBankCode(string bank)
    {
        switch (bank.ToString())
        {
            case "VietcomBank":
                return "VCB";
            case "Vietinbank":
                return "ICB";
            case "Techcombank":
                return "TCB";
            case "BIDV":
                return "BIDV";
            case "AgriBank":
                return "VBA";
            case "Sacombank":
                return "STB";
            case "ACB":
                return "ACB";
            case "MB":
                return "MB";

            case "TPBank":
                return "TPB";
            case "VIB":
                return "VIB";
            case "VP Bank":
                return "VPB";
            case "OCB":
                return "OCB";
            case "Eximbank":
                return "EIB";
            case "BaoVietBank":
                return "BVB";
            case "SCB":
                return "SCB";
            case "ABB":
                return "ABB";

            case "VCCB":
                return "VCCB";
            case "PVcombank":
                return "PVCB";
            case "OceanBank":
                return "OceanBank";
            case "NamABank":
                return "NAB";
            case "HDB":
                return "HDB";
            case "VietBank":
                return "EIB";
            case "PGBank":
                return "PGB";
            case "NCB":
                return "NCB";
            case "DongA":
                return "DOB";
            case "LPBank":
                return "LPBank";
            case "Cake":
                return "CAKE";

        }
        return bank;
    }
    public string getBankCode2(string bank)
    {
        switch (bank.ToString().ToLower())
        {
            case "vietcombank":
                return "VCB";
            case "vietinbank":
                return "ICB";
            case "techcombank":
                return "TCB";
            case "bidv":
                return "BIDV";
            case "agribank":
                return "VBA";
            case "sacombank":
                return "STB";
            case "acb":
                return "ACB";
            case "mb":
                return "MB";

            case "tpbank":
            case "tpb":
                return "TPB";
            case "vip":
                return "VIB";
            case "vp bank":
            case "vpb":
            case "vpbank":
                return "VPB";
            case "ocb":
                return "OCB";
            case "eximbank":
                return "eib";
            case "baovietbank":
                return "BVB";
            case "scb":
                return "SCB";
            case "abb":
                return "ABB";

            case "vccb":
                return "VCCB";
            case "pvcombank":
                return "PVCB";
            case "cceanbank":
                return "OceanBank";
            case "namabank":
                return "NAB";
            case "hdb":
            case "hd bank":
                return "HDB";
            case "vietbank":
                return "EIB";
            case "pgbank":
                return "PGB";
            case "ncb":
                return "NCB";
            case "donga":
                return "DOB";
            case "lpbank":
                return "LPBank";
            case "cake":
                return "cake";

        }
        return bank;
    }
    //protected void drpPartner_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PartnerTransaction + "?name=" + drpPartner.SelectedValue);
    //}
}