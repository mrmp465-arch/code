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
        if (!IsPostBack)
        {
            Init();
        }
    }
    protected void Init()
    {
        var user = new Users().Get(AppUtils.UserID);
        if(string.IsNullOrEmpty(user.F2a))
        {
            string googleAuthKey = "toppay";
            UserUniqueKey = (AppUtils.UserName + googleAuthKey);
            //Two Factor Authentication Setup
            TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
            var setupInfo = TwoFacAuth.GenerateSetupCode("toppay", AppUtils.UserName, ConvertSecretToBytes(UserUniqueKey, false), 200);
            //Session["UserUniqueKey"] = UserUniqueKey;
            BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            SetupCode = setupInfo.ManualEntryKey;
            dvFA.Visible = true;
        }

        var partner = new Partners().Get(AppUtils.UserName);
        txtMinBankAproveAmount.Text = "0";
        if(partner!=null)
        {
            if(!string.IsNullOrEmpty(partner.SMSPlusUrl))
            {
                txtMinBankAproveAmount.Text = partner.SMSPlusUrl;
            }    
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
        string googleAuthKey = "toppay";
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

    protected void btSubmit2_Click(object sender, EventArgs e)
    {
        var partner = new Partners().Get(AppUtils.UserName);
        if (partner != null)
        {
            partner.SMSPlusUrl = txtMinBankAproveAmount.Text;
            partner.Update();
            string KeyCache = string.Format("{0}:{1}", "RedisPartners", partner.PartnerCode);
            DataCaching.RemoveCache(KeyCache);
            AlertSuccesss.Text = "Cập nhật thành công";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);
        }
    }
    public byte[] ConvertSecretToBytes(string secret, bool secretIsBase32)
    {
        return secretIsBase32 ? Base32Encoding.ToBytes(secret) : Encoding.UTF8.GetBytes(secret);
    }

}