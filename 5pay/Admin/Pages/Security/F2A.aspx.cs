using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Google.Authenticator;
using Telegram.Bot.Types;
using Libs.API;
using Libs.Utils;
using System.Data;
using Libs.Report;
using DocumentFormat.OpenXml.Wordprocessing;

using OfficeOpenXml.FormulaParsing.Utilities;
public partial class Pages_Security_F2A : System.Web.UI.Page
{
    public string UserUniqueKey;
    public string BarcodeImageUrl;
    public string SetupCode;
    protected void Page_Load(object sender, EventArgs e)
    {

        AppUtils.CheckRoles(Resources.Url.F2A);
        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
        btSubmit.Text = Resources.Pay.Confirm;
        btAdd.Text = Resources.Pay.Add;
        btUpdate.Text = Resources.Pay.Update;
        Init();
    }
    protected void Init()
    {
        var user = new Users().Get(AppUtils.UserID);
        if (string.IsNullOrEmpty(user.F2a))
        {
            string googleAuthKey = "5Pay6868";
            UserUniqueKey = (AppUtils.UserName + googleAuthKey);
            //Two Factor Authentication Setup
            TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
            var setupInfo = TwoFacAuth.GenerateSetupCode("5Pay", AppUtils.UserName, ConvertSecretToBytes(UserUniqueKey, false), 200);
            //Session["UserUniqueKey"] = UserUniqueKey;
            BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            SetupCode = setupInfo.ManualEntryKey;
            dvFA.Visible = true;
        }

        if (!IsPostBack)
        {

            BindData();
        }
    }
    protected void btSubmit_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtOTP.Text))
        {
            AlertInfos.Text = Resources.Pay.NotEnoughInformation;
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }
        TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
        string googleAuthKey = "5Pay6868";
        UserUniqueKey = (AppUtils.UserName + googleAuthKey);
        bool isValid = TwoFacAuth.ValidateTwoFactorPIN(UserUniqueKey, txtOTP.Text, false);
        if (!isValid)
        {

            AlertInfos.Text = Resources.Pay._2FAWrong;
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }
        else
        {
            var user = new Users().Get(AppUtils.UserID);
            user.F2a = UserUniqueKey;
            user.Update();

            AlertSuccesss.Text = Resources.Pay.SecuritySetupOK;
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);
        }
    }
    public byte[] ConvertSecretToBytes(string secret, bool secretIsBase32)
    {
        return secretIsBase32 ? Base32Encoding.ToBytes(secret) : Encoding.UTF8.GetBytes(secret);
    }
    private void BindData()
    {
        var data = new PartnersBankAccount().GetLis().Where(x => x.ParnerCode == AppUtils.UserName).OrderBy(x => x.Number).ToList();


        // JavaScriptSerializer serializer = new JavaScriptSerializer();
        //NLogLogger.Info(new string[] { "Data", "PartnerMomoo", serializer.Serialize(data) });
        rptList.DataSource = data;
        rptList.DataBind();


    }
    protected void btAdd_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(drpBankCode.SelectedValue))
        {
            AlertInfos.Text = "Vui lòng chọn bank";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }
        var obj = new PartnersBankAccount();
        obj.BankCode = drpBankCode.SelectedValue;
        obj.AccountName = txtAccountName.Text;
        obj.AccountNumber = txtAccountNumber.Text;
        if(string.IsNullOrEmpty(obj.AccountName))
        {
            obj.AccountName = obj.AccountNumber;
        }    
        obj.Status = Convert.ToInt32(chkIsActive.Checked);
        obj.IP = GetIP();
        obj.ParnerCode = AppUtils.UserName;
        obj.Add(obj);
        System.Threading.Thread.Sleep(100);
        //Response.Redirect("/cmspay/pages/security/F2A.aspx#bank");
        hdCurrentTab.Value = "#bank";
        drpBankCode.SelectedValue = "";
        txtAccountName.Text = "";
        txtAccountNumber.Text = "";
        BindData();

    }
    protected void btApply_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            System.Web.UI.WebControls.CheckBox cbx = (System.Web.UI.WebControls.CheckBox)rptList.Items[i].FindControl("cbxStatus");

            Label lbPId = (Label)rptList.Items[i].FindControl("lblId");
            TextBox tbx = (TextBox)rptList.Items[i].FindControl("txtOrderNo");
            var tbxValue = tbx.Text;

            //NLogLogger.Info(new string[] { "Data", "lbPId", lbPId.Text });
            var _Partner = new PartnersBankAccount();
            _Partner.Id = Convert.ToInt32(lbPId.Text);
            _Partner.Status = cbx.Checked ? 1 : 0;
            _Partner.Number = Convert.ToInt32(tbxValue);
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //NLogLogger.Info(new string[] { "Data", "lbPId", serializer.Serialize(_PartnerMomo) });
            new PartnersBankAccount().Update(_Partner);
        }
        //Response.Redirect("/cmspay/pages/security/F2A.aspx#bank");
        hdCurrentTab.Value = "#bank";
        BindData();
    }
    protected void Delete_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        int Id = Convert.ToInt32(e.CommandArgument.ToString());

        //var order = new PartnerMomo { Id = Id };
        new PartnersBankAccount().Delete(Id);
        hdCurrentTab.Value = "#bank";
        BindData();
        //Response.Redirect("/cmspay/pages/security/F2A.aspx#bank");
    }
    public string GetDate(object createDate)
    {
        if (createDate != null)
        {
            DateTime dt = Convert.ToDateTime(createDate);
            return dt.ToString("dd/MM/yyyy");
        }



        return "";
    }
    public static string GetIP()
    {
        string IP = "";
        //return IP;
        if (HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"] != null)
        {
            IP = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];
            if (IP.Contains(","))
                return IP.Split(',')[0].Trim();
            return IP;
        }

        if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
        {
            IP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (IP.Contains(","))
                return IP.Split(',')[0].Trim();
            return IP;
        }

        if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED"] != null)
        {
            IP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED"];
            if (IP.Contains(","))
                return IP.Split(',')[0].Trim();
            return IP;
        }

        if (HttpContext.Current.Request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"] != null)
        {
            IP = HttpContext.Current.Request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"];
            if (IP.Contains(","))
                return IP.Split(',')[0].Trim();
            return IP;
        }

        if (HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED_FOR"] != null)
        {
            IP = HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED_FOR"];
            if (IP.Contains(","))
                return IP.Split(',')[0].Trim();
            return IP;
        }

        if (HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED"] != null)
        {
            IP = HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED"];
            if (IP.Contains(","))
                return IP.Split(',')[0].Trim();
            return IP;
        }

        if (IP == "")
        {
            IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }
        if (IP.Contains(","))
            return IP.Split(',')[0].Trim();


        return IP;
    }

}