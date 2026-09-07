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


public partial class Pages_BankEWalletService_InternalTransfer_Transfer : System.Web.UI.Page
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
        var lstBank2 = new ITBank().GetList().Where(x => x.Type == "HOLD");
        foreach (var item in lstBank2)
        {
            item.BankCode += "-" + item.BankName + "-" + item.BankId;
        }
        
        drpPartnerBankCode.DataSource = lstBank2;
        drpPartnerBankCode.DataTextField = "BankCode";
        drpPartnerBankCode.DataValueField = "BankCode";
        drpPartnerBankCode.DataBind();
        drpPartnerBankCode.Items.Insert(0, new ListItem("Tài khoản chuyển", ""));

        drpPartnerBankCode2.DataSource = lstBank2;
        drpPartnerBankCode2.DataTextField = "BankCode";
        drpPartnerBankCode2.DataValueField = "BankCode";
        drpPartnerBankCode2.DataBind();
        drpPartnerBankCode2.Items.Insert(0, new ListItem("Tài khoản chuyển", ""));

      
      

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
        TelegramClient.SendTeleV2("-1003071005377", "Hủy lệnh chuyển tiền từ user " + AppUtils.UserName + " Mã lệnh " + _tran.Id.ToString() + " , Số tiền: " + _tran.Amount.GetValueOrDefault().ToString("#,#").Replace(",", ".") + " , Tài khoản chuyển " + _tran.PartnerBankCode + "-" + _tran.PartnerBankName + " , Tài khoản nhận " + _tran.BankCode + "-" + _tran.BankName);
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

        TelegramClient.SendTeleV2("-1003071005377", "Duyệt lệnh chuyển tiền từ user " + AppUtils.UserName + " Mã lệnh " + _tran.Id.ToString() + " , Số tiền: " + _tran.Amount.GetValueOrDefault().ToString("#,#").Replace(",", ".") + " , Tài khoản chuyển " + _tran.PartnerBankCode + "-" + _tran.PartnerBankName + " , Tài khoản nhận " + _tran.BankCode + "-" + _tran.BankName);
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

        TelegramClient.SendTeleV2("-1003071005377", "Cập nhật lệnh chuyển tiền từ user " + AppUtils.UserName + " Mã lệnh " + _tran.Id.ToString() + " , Số tiền: " + _tran.Amount.GetValueOrDefault().ToString("#,#").Replace(",", ".") + " , Tài khoản chuyển " + _tran.PartnerBankCode + "-" + _tran.PartnerBankName + " , Tài khoản nhận " + _tran.BankCode + "-" + _tran.BankName);
        GetList();

    }
    public string GetQR(string BankCode, string BankId, string content, string amount)
    {
        if (BankCode == "NVB")
            BankCode = "NCB";
        return String.Format("https://img.vietqr.io/image/{0}-{1}-compact.jpg?addInfo={3}&amount={2}", BankCode, BankId, amount, content);
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

        if (!string.IsNullOrEmpty(drpPartnerBankCode.SelectedValue))
        {
            partnerBankcode = drpPartnerBankCode.SelectedValue.Split('-')[0];
            partnerBankid = drpPartnerBankCode.SelectedValue.Split('-')[2];
        }
        int top = int.Parse(drpTop.SelectedValue);
        var usename = AppUtils.UserName;
        if (RoleApp)
            usename = "";
        var lstdata = new ITTransaction().GetList(top, id, bankcode, partnerBankid, partnerBankcode, bankid, fromDate, requestTime, status, amount, usename, txtComment.Text, "TRANSFER");
        rptList.DataSource = lstdata;
        rptList.DataBind();
    }
    protected void btAdd_Click2(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(drpBankCode2.SelectedValue) || string.IsNullOrEmpty(drpPartnerBankCode2.SelectedValue))
        {
            AlertInfoss.Text = "Không đủ thông tin";
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
        _tran.CommandCode = "TRANSFER";
        _tran.Status = 0;



        if (!string.IsNullOrEmpty(drpBankCode2.SelectedValue))
        {
            _tran.BankCode = getBankCode(drpBankCode2.SelectedValue);
            _tran.BankName = txtBankName.Text;
            _tran.BankId = txtBankId.Text;
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
                return "DOB";
            case "LPBank":
                return "LPBank";
            case "Cake":
                return "CAKE";

        }
        return bank;
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