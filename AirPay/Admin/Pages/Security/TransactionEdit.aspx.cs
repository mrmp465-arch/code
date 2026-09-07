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
using Telegram.Bot.Types;


public partial class Pages_Security_TransactionEdit : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
        AppUtils.CheckRoles(Resources.Url.PartnerTransaction);
        if (!AppUtils.IsAdmin)
        {
            Response.Redirect("/cmspay");

        }
        if (!IsPostBack)
        {
            var lst = new List<Users>();
            if (AppUtils.IsAdmin)
            {
                //var lstPartner = new Partners().GetList().Where(x => x.Hotline != "").GroupBy(x => x.Hotline).Select(a => a.Key);
                var lstUser = new Users().GetList().OrderBy(x => x.UserName);
                foreach (var item in lstUser)
                {
                    if (item.IsPartner == 1)
                        lst.Add(new Users { UserName = item.UserName });
                }
            }
            else
            {
                lst.Add(new Users { UserName = AppUtils.UserName });
            }

            lst = lst.OrderBy(x => x.UserName).ToList();


            ddlAccount.DataSource = lst;
            ddlAccount.DataTextField = "UserName";
            ddlAccount.DataValueField = "UserName";
            ddlAccount.DataBind();
            ddlAccount.Items.Insert(0, new ListItem("Tài khoản:", ""));
            var id = Request["id"];
            //var refCode = Request["refCode"];
            getOrdernO(id);
        }
    }
    protected void getOrdernO(string id)
    {
        UserWithdraw _BankGateAPI = new UserWithdraw();
        _BankGateAPI.Id = int.Parse(id);
        // _BankGateAPI.TransactionID = AppUtils.ToInt64(txtTransactionID.Text);

        _BankGateAPI = _BankGateAPI.Get();



        if (_BankGateAPI == null)
        {
            Response.Redirect("/cmspay");
        }
        else
        {
            if (_BankGateAPI.Status == 1)
                Response.Redirect("/cmspay");


            txtAmount2.Text = Convert.ToInt32(_BankGateAPI.Amount).ToString();
            txtFee.Text = "0";
            //txtReward.Text = "0";
            ddlAccount.SelectedValue = _BankGateAPI.UserName.ToString();
            ddlAccount.Enabled = false;
        }
    }
    protected void btAdd_Click2(object sender, EventArgs e)
    {
        var _tran = new UserWithdraw();
        var id = Request["id"];
        _tran.Id = int.Parse(id);
        if (drlType2.SelectedValue == "-1")
        {
            AlertInfoss.Text = "Vui lòng nhập loại giao dịch";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }
        //_tran.UserName = ddlAccount.SelectedValue;
        _tran.Admin = AppUtils.UserName;
        _tran.Fee = long.Parse(txtFee.Text);
        _tran.Reward = 0;
        _tran.Usdt = 0;
        _tran.RateIn = 0;
        _tran.RateOut = 0;
        //_tran.Note = txtNote.Text;
        if (txtUsdt.Text != "")
        {
            _tran.Note = _tran.Note+ "<br>usdt: " + txtUsdt.Text + " - rate: " + txtRate.Text;
            _tran.Usdt = int.Parse(txtUsdt.Text);
            _tran.RateIn = int.Parse(txtRateIn.Text);
            _tran.RateOut = int.Parse(txtRate.Text);
        }
        _tran.BankInfo = "";
        _tran.ConfirmV2();
        TelegramClient.SendTeleV2("-4873375848", "Duyệt lệnh rút tiền từ user " + AppUtils.UserName + " Mã lệnh " + _tran.Id.ToString());
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PartnerTransaction);
    }
}