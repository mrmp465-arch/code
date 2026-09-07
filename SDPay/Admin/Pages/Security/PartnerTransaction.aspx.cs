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




public partial class Pages_Security_PartnerTransaction : System.Web.UI.Page
{
    public string UrlHistory;
    public bool RoleApp { get; set; }
    public bool IsDL { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.PartnerTransaction);
        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
        btView.Text = Resources.Pay.View;
        btCreate.Text = Resources.Pay.CreateWithdrawalOrder;


        RoleApp = AppUtils.CheckRolesPermission("pages/security/partnertransactionapp.aspx");

        if (AppUtils.IsTopup)
        {
            var partnerlist = new Partners().GetList();
            if (partnerlist.Exists(x => x.SMSUrl == AppUtils.UserName))
            {
                IsDL = true;
            }
        }

        //if (AppUtils.IsAdmin)
        //{
        //    if (!AppUtils.CheckRolesPermission("pages/security/partnertransactionadd.aspx"))
        //        dvRutAdmin.Visible = false;
        //}
        //else
        //{
        //    if (!AppUtils.IsPartner)
        //    {
        //        dvRut.Visible = false;
        //        var user = new Users().Get(AppUtils.UserID);
        //        if (user.Withdraw == 1)
        //            dvRut.Visible = true;
        //    }


        //    //check tiếp tk support rút đc tiền

        //}

        if (!IsPostBack)
        {
            if (AppUtils.IsAdmin)
            {
                if (!AppUtils.CheckRolesPermission("pages/security/partnertransactionadd.aspx"))
                    dvRutAdmin.Visible = false;
            }
            else
            {
                var username = AppUtils.UserName;
                if (!AppUtils.IsPartner)
                {
                    dvRut.Visible = false;
                    var user = new Users().Get(AppUtils.UserID);
                    if (user.Withdraw == 1)
                        dvRut.Visible = true;

                    var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
                    username = lstPartner.FirstOrDefault().Name;


                    txtAccountName.ReadOnly = true;
                    txtAccountNumber.ReadOnly = true;
                    drpBankCode.Enabled = false;


                    if (IsDL)
                    {
                        //IsDL = true;

                        txtAccountName.ReadOnly = false;
                        txtAccountNumber.ReadOnly = false;
                        drpBankCode.Enabled = true;
                        dvRut.Visible = true;
                        username = AppUtils.UserName;
                    }
                }

                var lstBank2 = new PartnersBankAccount().GetLis().Where(x => x.ParnerCode == username && x.Status == 1).OrderBy(x => x.Number).ToList();
                foreach (var item in lstBank2)
                {
                    item.BankCode += "-" + item.AccountName + "-" + item.AccountNumber;
                }

                drpBankCode2.DataSource = lstBank2;
                drpBankCode2.DataTextField = "BankCode";
                drpBankCode2.DataValueField = "BankCode";
                drpBankCode2.DataBind();

                drpBankCode2.Items.Insert(0, new System.Web.UI.WebControls.ListItem(Resources.Pay.ChoseAccount, ""));

                if (lstBank2.Count() == 0)
                {
                    divAccount.Visible = false;
                }

            }


            init();
            GetList();
        }

    }

    protected void btAdd_Click(object sender, EventArgs e)
    {
        if (AppUtils.IsPartner || IsDL)
        {
            if (string.IsNullOrEmpty(txtAccountName.Text) || string.IsNullOrEmpty(txtAmount.Text) || string.IsNullOrEmpty(txtAccountNumber.Text) || string.IsNullOrEmpty(drpBankCode.SelectedValue))
            {
                AlertInfoss.Text = Resources.Pay.NotEnoughInformation;
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
                return;
            }
        }
        else
        {
            if (string.IsNullOrEmpty(txtAmount.Text) || string.IsNullOrEmpty(drpBankCode2.SelectedValue))
            {
                AlertInfoss.Text = Resources.Pay.NotEnoughInformation;
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
                return;
            }
        }

        long amount = long.Parse(txtAmount.Text);
        if (amount < 10000)
        {
            AlertInfoss.Text = Resources.Pay.AmountMin10;
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }
        if (DateTime.Now.Hour >= 23 && DateTime.Now.Minute >= 50)
        {
            AlertInfoss.Text = "Tính năng rút tiền đang bảo trì. Vui lòng rút lại sau 12h";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }
        if (String.IsNullOrEmpty(drpBankCode.SelectedValue))
        {
            AlertInfoss.Text = "Vui lòng chọn bankcode";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }
        var _tran = new UserWithdraw();
        _tran.UserName = AppUtils.UserName;
        _tran.Amount = amount;
        _tran.Note = String.Format("Ngân hàng: <b>{0}</b><br>Số tài khoản: <b>{1}</b>  <br> Chủ tài khoản: <b>{2}</b>", drpBankCode.SelectedItem.Text, txtAccountNumber.Text, txtAccountName.Text);

        if (!AppUtils.IsPartner && !IsDL)
        {
            var arrbank = drpBankCode2.SelectedValue.Split('-');
            _tran.Note = String.Format("Ngân hàng: <b>{0}</b><br>Số tài khoản: <b>{1}</b>  <br> Chủ tài khoản: <b>{2}</b>", arrbank[0], arrbank[2], arrbank[1]);


        }


        _tran.BankInfo = "";
        _tran.Fee = 0;
        _tran.Reward = 0;
        _tran.Type = 1;
        _tran.Usdt = 0;

        //thêm thằng support nó cũng rút đc
        if (!AppUtils.IsPartner && !IsDL)
        {
            var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
            _tran.UserName = lstPartner.FirstOrDefault().Name;

        }



        var bankCode = getBankCode(drpBankCode.SelectedValue);
        if (!string.IsNullOrEmpty(bankCode))
        {
            _tran.BankInfo = String.Format("https://img.vietqr.io/image/{0}-{1}-print.jpg?amount={2}&accountName={3}", bankCode, txtAccountNumber.Text.Trim(), txtAmount.Text,txtAccountName.Text);
            if (_tran.Amount > 500000000)
                _tran.BankInfo = String.Format("https://img.vietqr.io/image/{0}-{1}-print.jpg?accountName={2}", bankCode, txtAccountNumber.Text.Trim(), txtAccountName.Text);
        }

        var result = _tran.Add();
        if (result < 0)
        {

            AlertBans.Text = Resources.Pay.NotEnoughBalance;
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertBan').modal()}); ", true);
            return;

        }
        TelegramClient.SendTeleV2("-5490965546", "Tạo lệnh rút tiền  " + amount.ToString("#,#").Replace(",", ".") + " từ user " + AppUtils.UserName + " Mã lệnh " + result.ToString());

        try
        {
            var partner = new Partners().GetCache(_tran.UserName);
            var chatid = partner.SMSPlusUrl;
            if (!string.IsNullOrEmpty(chatid))
            {
                TelegramClient.SendTeleV2(chatid, "Tạo lệnh rút tiền  " + amount.ToString("#,#").Replace(",", ".") + " từ user " + AppUtils.UserName + " Mã lệnh " + result.ToString() + " " + bankCode + " - " + txtAccountNumber.Text.Trim() + " - " + txtAccountName.Text.Trim());

            }
        }
        catch (Exception ex)
        {

        }
        AlertSuccesss.Text = Resources.Pay.SuccessOrder;
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);

        txtAccountName.Text = "";
        txtAccountNumber.Text = "";
        System.Threading.Thread.Sleep(200);
        //GetList();
        Response.Redirect(Request.RawUrl);

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
                if (item.IsPartner == 1 || item.IsTopup == 1)
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
        //txtReward.Text = "0";
        txtFee.Text = "0";
        txtUsdt.Text = "";
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
    public string GetType(object statusOver)
    {
        if (statusOver.ToString() == "1")
        {
            return "VND";
        }
        if (statusOver.ToString() == "2")
        {
            return "Usdt mua";
        }
        if (statusOver.ToString() == "3")
        {
            return "Usdt sẵn";
        }

        return "";
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
            AlertInfoss.Text = "Không đủ thông tin";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }
        if (drlType2.SelectedValue == "0")
        {
            AlertInfoss.Text = "Vui lòng nhập loại giao dịch";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }
        long amount = long.Parse(txtAmount2.Text);
        if (amount < 1000)
        {
            AlertInfoss.Text = "Số tiền phải lớn hơn bằng 10M";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }

        var _tran = new UserWithdraw();
        _tran.UserName = ddlAccount.SelectedValue;
        _tran.Amount = amount;
        _tran.Note = txtNote.Text;
        _tran.Usdt = 0;
        _tran.RateIn = 0;
        _tran.RateOut = 0;
        if (txtUsdt.Text != "")
        {
            _tran.Note = txtNote.Text + " - usdt: " + txtUsdt.Text + " - rate: " + txtRate.Text;
            _tran.Usdt = int.Parse(txtUsdt.Text);
            _tran.RateIn = int.Parse(txtRateIn.Text);
            _tran.RateOut = int.Parse(txtRate.Text);
        }
        _tran.BankInfo = "";
        _tran.Fee = long.Parse(txtFee.Text);
        _tran.Reward = 0;
        _tran.Type = int.Parse(drlType2.SelectedValue);
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
            //TelegramClient.SendTeleV2("-4873375848", "Tạo lệnh rút tiền  " + amount.ToString("#,#").Replace(",", ".") + " từ user " + AppUtils.UserName + " Mã lệnh " + result.ToString());
            AlertSuccesss.Text = "Tạo lệnh thành công";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);

        }
        else
        {
            _tran.Admin = AppUtils.UserName;
            _tran.Add2();
            AlertSuccesss.Text = "Tạo lệnh thành công";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);

        }


        txtAmount2.Text = "";

        System.Threading.Thread.Sleep(200);
        Response.Redirect(Request.RawUrl);
        //GetList();


    }
    protected string FixtUrl(string id)
    {
        return Constant.ADMIN_PATH + "pages/security/transactionedit.aspx?id=" + id;
    }
    protected void btAdd_QR(object sender, EventArgs e)
    {
        //NLogLogger.Info(MyBox.Text);
        var lstdata = MyBox.Text.Split('\r');
        imgqr.Visible = true;
        imgqr.ImageUrl = String.Format("https://img.vietqr.io/image/{0}-{1}-print.jpg?amount={2}&addInfo={3}", getbankCodetext(lstdata), getbankNumbertext(lstdata), getMoneytext(lstdata), getContent(lstdata));
        //NLogLogger.Info(lstdata[0]);

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
    protected void Delete_Command(Object sender, CommandEventArgs e)
    {
        var _tran = new UserWithdraw();
        _tran.Id = Convert.ToInt32(e.CommandArgument.ToString());
        _tran = _tran.Get();
        if (_tran.Status == 0)
        {
            _tran.Admin = AppUtils.UserName;
            _tran.Cancel();

            TelegramClient.SendTeleV2("-5490965546", "Hủy lệnh rút tiền từ user " + AppUtils.UserName + " Mã lệnh " + _tran.Id.ToString() + ", Số tiền " + _tran.Amount.ToString("#,#").Replace(",", "."));
            //GetList();

        }
        Response.Redirect(Request.RawUrl);
    }
    protected void Update_Command(Object sender, CommandEventArgs e)
    {

        var _tran = new UserWithdraw();
        _tran.Id = Convert.ToInt32(e.CommandArgument.ToString());
        _tran.Admin = AppUtils.UserName;
        _tran.Confirm();
        _tran = _tran.Get();
        TelegramClient.SendTeleV2("-5490965546", "Duyệt lệnh rút tiền từ user " + AppUtils.UserName + " Mã lệnh " + _tran.Id.ToString() + ", Số tiền " + _tran.Amount.ToString("#,#").Replace(",", "."));
        //GetList();
        Response.Redirect(Request.RawUrl);

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

            int type = int.Parse(drpType.SelectedValue);
            var lstDataBank = new UserWithdraw().GetList(top, name, status, fromDate, requestTime, type);
            //if(cbByUsdt.Checked==true)
            //{
            //    lstDataBank = lstDataBank.Where(m => m.Fee > 0).ToList(); 
            //}
            rptList.DataSource = lstDataBank;



            var lstDataBanksu = lstDataBank.Where(m => m.Status == 1);
            if (lstDataBanksu != null)
                lblTotalReport.Text = String.Format("{0} : {1} - {2} : {3} ", "Số lệnh rút", lstDataBanksu.Count(), "Số tiền rút", lstDataBanksu.Sum(x => x.Amount).ToString("N0").Replace(".", ","));


            //var lstbankDe = new UserDeposit().GetList(top, "cn02", 1, fromDate, requestTime);
            //var lstDataBankWi = new UserWithdraw().GetList(top, "", 1, fromDate, requestTime, 3);
            //if(lstbankDe!=null)
            //{
            //    long Money = lstbankDe.Sum(x => x.Money);
            //    long Amount = lstbankDe.Sum(x => x.Amount);
            //    int rate = 0;
            //    if (Money>0)
            //    {
            //        rate = (int)(Amount / Money);
            //    }
            //    long Usdt = 0;
            //    if(lstDataBankWi!=null)
            //     Usdt = lstDataBankWi.Sum(x => x.Usdt);
            //    lbReport2.Text = String.Format("{0} : {1} - {2} : {3} - {4} : {5}", "Tổng Usdt có sẵn", Money, "Tỉ giá trung bình ", rate, "Số usdt còn lại", Money-Usdt);

            //}
            //NLogLogger.Info(new string[] { "BindData", "BindData", requestTime.ToString(), fromDate.ToString() });
            rptList.DataBind();

        }
        else
        {
            name = AppUtils.UserName;

            if (!AppUtils.IsPartner && !IsDL)
            {


                var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
                name = lstPartner.FirstOrDefault().Name;
            }
            rptList.DataSource = new UserWithdraw().GetList(top, name, status, fromDate, requestTime);
            rptList.DataBind();
            var user = new Users().GetByUserName(name);

            lblTotal.Text = Resources.Pay.Balance + " : " + user.Balance.ToString("#,#").Replace(",", ".");
            dvUserInfo.Visible = true;
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
                return "VIETBANK";
            case "PGBank":
                return "PGB";
            case "NCB":
                return "NCB";
            case "DongA":
                return "VIKKI";
            case "LPBank":
                return "LPBank";
            case "Cake":
                return "CAKE";
            case "Wooribank":
                return "WVN";

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