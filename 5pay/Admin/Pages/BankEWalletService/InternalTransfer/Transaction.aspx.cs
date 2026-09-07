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
using ServiceStack.Common.Extensions;


public partial class Pages_BankEWalletService_InternalTransfer_Transaction : System.Web.UI.Page
{
    public bool RoleApp { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        RoleApp = AppUtils.CheckRolesPermission(Resources.Url.ITransactionApp);
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

        var lstBank = new BankAccounts().GetList().ToList();
        foreach (var item in lstBank)
        {
            item.BankCode += "-" + item.BankName + "-" + item.BankId;
        }
        var lstMomo = new MomoAccounts().GetList().Where(x => x.Type.Contains("IN") );
        foreach (var item in lstMomo)
        {
            var obj = new BankAccounts();
            obj.BankCode = "MOMO-" + item.MomoName + "-" + item.MomoId;
            lstBank.Add(obj);

        }
        var lstBank3 = new ITBank().GetList().Where(x => x.Type == "OUTSIDE");
        foreach (var item in lstBank3)
        {
            var obj = new BankAccounts();
            obj.BankCode = item.BankCode + "-" + item.BankName + "-" + item.BankId;
            lstBank.Add(obj);
        }
        var obj1 = new BankAccounts();
        obj1.BankCode = "Card-88888888-Thẻ";
        lstBank.Add(obj1);
        drpPartnerBankCode.DataSource = lstBank;
        drpPartnerBankCode.DataTextField = "BankCode";
        drpPartnerBankCode.DataValueField = "BankCode";
        drpPartnerBankCode.DataBind();
        drpPartnerBankCode.Items.Insert(0, new ListItem("Tài khoản chuyển", ""));

        drpPartnerBankCode2.DataSource = lstBank;
        drpPartnerBankCode2.DataTextField = "BankCode";
        drpPartnerBankCode2.DataValueField = "BankCode";
        drpPartnerBankCode2.DataBind();
        drpPartnerBankCode2.Items.Insert(0, new ListItem("Tài khoản chuyển", ""));

        var lstBank2 = new ITBank().GetList().Where(x => x.Type == "HOLD").OrderBy(x=>x.BankCode).ToList();
        //var bankSH = lstBank2.FirstOrDefault(x => x.BankCode == "SHBVN");
        //lstBank2= lstBank2.Where(x=>x.BankCode!= "SHBVN").ToList();
        //lstBank2.Insert(0, bankSH);

       
        foreach (var item in lstBank2)
        {
            item.BankCode += "-" + item.BankName + "-" + item.BankId;
        }

        //lstBank2 = lstBank2.OrderByDescending(x => x.BankCode).ToList();

        drpBankCode.DataSource = lstBank2;
        drpBankCode.DataTextField = "BankCode";
        drpBankCode.DataValueField = "BankCode";
        drpBankCode.DataBind();
        drpBankCode.Items.Insert(0, new ListItem("Tài khoản nhận", ""));

        drpBankCode2.DataSource = lstBank2.Where(x=>x.Status==1);
        drpBankCode2.DataTextField = "BankCode";
        drpBankCode2.DataValueField = "BankCode";
        drpBankCode2.DataBind();
        drpBankCode2.Items.Insert(0, new ListItem("Tài khoản nhận", ""));

    }
    public string GetStatus(object statusOver)
    {
        if (statusOver.ToString() == "2")
        {
            return "<div class=\"label label-success\"> Đã duyệt</div>";
        }
        if (statusOver.ToString() == "-1")
        {
            return "<div class=\"label label-danger\"> Thất bại </div>";
        }
        if (statusOver.ToString() == "1")
        {
            return "<div class=\"label label-info\"> Đợi duyệt  </div>";
        }
        if (statusOver.ToString() == "0")
        {
            return "<div class=\"label label-default\"> Khởi tạo  </div>";
        }
        return "";
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }
    protected void Delete_Command(Object sender, CommandEventArgs e)
    {

        var _tran = new ITTransaction();
        _tran.Id = Convert.ToInt32(e.CommandArgument.ToString());
        _tran = _tran.Get();
        _tran.Status = -1;
        _tran.Update();
        TelegramClient.SendTeleV2("-1003071005377", "Hủy lệnh gom tiền từ user " + AppUtils.UserName + " Mã lệnh " + _tran.Id.ToString() + " , Số tiền: " + _tran.Amount.GetValueOrDefault().ToString("#,#").Replace(",", ".") + " , Tài khoản chuyển " + _tran.PartnerBankCode + "-" + _tran.PartnerBankName + " , Tài khoản nhận " + _tran.BankCode + "-" + _tran.BankName);
        GetList();

    }
    //duyệt
    protected void App_Command(Object sender, CommandEventArgs e)
    {
        var _tran = new ITTransaction();
        _tran.Id = Convert.ToInt32(e.CommandArgument.ToString());
        _tran = _tran.Get();
        _tran.AppName = AppUtils.UserName;
        _tran.Status = 2;
        _tran.Update();

        TelegramClient.SendTeleV2("-1003071005377", "Duyệt lệnh gom tiền từ user " + AppUtils.UserName + " Mã lệnh " + _tran.Id.ToString() + " , Số tiền: " + _tran.Amount.GetValueOrDefault().ToString("#,#").Replace(",", ".") + " , Tài khoản chuyển " + _tran.PartnerBankCode + "-" + _tran.PartnerBankName + " , Tài khoản nhận " + _tran.BankCode + "-" + _tran.BankName);
        GetList();

    }
    //cập nhật
    protected void Update_Command(Object sender, CommandEventArgs e)
    {
        var _tran = new ITTransaction();
        _tran.Id = Convert.ToInt32(e.CommandArgument.ToString());
        _tran = _tran.Get();
        _tran.Status = 1;
        _tran.Update();

        TelegramClient.SendTeleV2("-1003071005377", "Cập nhật lệnh gom tiền từ user " + AppUtils.UserName + " Mã lệnh " + _tran.Id.ToString() + " , Số tiền: " + _tran.Amount.GetValueOrDefault().ToString("#,#").Replace(",", ".") + " , Tài khoản chuyển " + _tran.PartnerBankCode + "-" + _tran.PartnerBankName + " , Tài khoản nhận " + _tran.BankCode + "-" + _tran.BankName);
        GetList();

    }
    public string GetQR(string BankCode, string BankId, string content, string amount)
    {
        return String.Format("https://img.vietqr.io/image/{0}-{1}-compact.jpg?addInfo=gui tien {3}&amount={2}", BankCode, BankId, amount, content);
    }
    private void GetList()
    {
        int? status = null;
        int? amount = null;
        Int64? id = AppUtils.ToInt64(txtId.Text);
        if (id == 0) id = null;



        if (string.IsNullOrEmpty(drpStatus.SelectedValue))
            status = null;
        else
            status = AppUtils.ToInt32(drpStatus.SelectedValue);

        if (string.IsNullOrEmpty(txtAmount.Text))
            amount = null;
        else
            amount = AppUtils.ToInt32(txtAmount.Text);
        DateTime requestTime = ToDateTime(txtCreatTime.Text);
        DateTime fromDate = ToDateTime(txtFromDate.Text);

        string bankcode = "";
        string bankid = "";
        string partnerBankcode = "";
        string partnerBankid = "";

        if (!string.IsNullOrEmpty(drpBankCode.SelectedValue))
        {
            bankcode = drpBankCode.SelectedValue.Split('-')[0];
            bankid = drpBankCode.SelectedValue.Split('-')[2];
        }
        if (!string.IsNullOrEmpty(drpPartnerBankCode.SelectedValue))
        {
            partnerBankcode = drpPartnerBankCode.SelectedValue.Split('-')[0];
            partnerBankid = drpPartnerBankCode.SelectedValue.Split('-')[2];
        }
        int top = int.Parse(drpTop.SelectedValue);
        var usename = AppUtils.UserName;
        if (RoleApp)
            usename = "";
        var lstdata = new ITTransaction().GetList(top, id, bankcode, partnerBankid, partnerBankcode, bankid, fromDate, requestTime, status, amount, usename, txtComment.Text, "IN");
        rptList.DataSource = lstdata;
        rptList.DataBind();
    }
    protected void btAdd_Click2(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(drpBankCode2.SelectedValue) || string.IsNullOrEmpty(drpPartnerBankCode2.SelectedValue))
        {
            AlertInfoss.Text = "Không đủ thông itn";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }
        long amount = long.Parse(txtAmount2.Text);
        if (amount < 1000000)
        {
            AlertInfoss.Text = "Số tiền phải lớn 1M";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }

        var _tran = new ITTransaction();
        _tran.UserName = AppUtils.UserName;
        _tran.Amount = int.Parse(txtAmount2.Text); ;
        _tran.Description = txtNote.Text;
        _tran.CommandCode = "IN";
        _tran.Status = 0;

       
        if (drpPartnerBankCode.SelectedValue.Contains("ACB") || drpPartnerBankCode.SelectedValue.Contains("VPB"))
        {
            _tran.CommandCode = "INCMS";
        }
        //else
        //{

        //}
        //if (!cbIn.Checked)
        //{
        //    _tran.CommandCode = "INCMS";
        //}
        //else
        //{
        //    _tran.CommandCode = "IN";
        //}
        if (!string.IsNullOrEmpty(drpBankCode2.SelectedValue))
        {
            _tran.BankCode = drpBankCode2.SelectedValue.Split('-')[0];
            _tran.BankName = drpBankCode2.SelectedValue.Split('-')[1];
            _tran.BankId = drpBankCode2.SelectedValue.Split('-')[2];
        }
        if (!string.IsNullOrEmpty(drpPartnerBankCode2.SelectedValue))
        {
            _tran.PartnerBankCode = drpPartnerBankCode2.SelectedValue.Split('-')[0];
            _tran.PartnerBankName = drpPartnerBankCode2.SelectedValue.Split('-')[1];
            _tran.PartnerBankId = drpPartnerBankCode2.SelectedValue.Split('-')[2];
        }


        _tran.Add();

        txtAmount2.Text = "";

        System.Threading.Thread.Sleep(200);
        Response.Redirect(Request.RawUrl);
        //GetList();


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


}