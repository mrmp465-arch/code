using DocumentFormat.OpenXml.Office2010.Excel;
using Libs.API;
using Libs.Report;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_Monitor_Config : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.SystemConfig);
        if (!IsPostBack)
        {
            init();
        }
    }
    private void init()
    {
        var lstconfig = new SystemDataConfig().GetListCache();
        txtMaxBankCashAmount.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("MaxBankCashAmount")).DataValue;
        txtMaxMomoCashAmount.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("MaxMomoCashAmount")).DataValue;
        txtMinBankCashAmount.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("MinBankCashAmount")).DataValue;
        txtMinMomoCashAmount.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("MinMomoCashAmount")).DataValue;

       txtMinAmountVPB.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("VPBOUTMinAmount")).DataValue;


        txtMinBankAproveAmount.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("MinBankAproveAmount")).DataValue;
        txtBankCodeInMaintain.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("BankCodeInMaintain")).DataValue;
        txtBankPrefix.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("BankPrefix")).DataValue;

        txtBlockAccount.Text = lstconfig.FirstOrDefault(x => x.DataKey.Equals("BlockAccount")).DataValue;

        ddlBankCashEnable.Checked = Convert.ToBoolean(int.Parse(lstconfig.FirstOrDefault(x => x.DataKey.Equals("BankCashEnable")).DataValue));
        hdBankCashEnable.Value = lstconfig.FirstOrDefault(x => x.DataKey.Equals("BankCashEnable")).DataValue;

        ddlMomoCashEnable.Checked = Convert.ToBoolean(int.Parse(lstconfig.FirstOrDefault(x => x.DataKey.Equals("MomoCashEnable")).DataValue));
        ddlBankInEnable.Checked = Convert.ToBoolean(int.Parse(lstconfig.FirstOrDefault(x => x.DataKey.Equals("BankInEnable")).DataValue));
        hdBankInEnable.Value = lstconfig.FirstOrDefault(x => x.DataKey.Equals("BankInEnable")).DataValue;
        ddlMomoInEnable.Checked = Convert.ToBoolean(int.Parse(lstconfig.FirstOrDefault(x => x.DataKey.Equals("MomoInEnable")).DataValue));
        ddlBankApp.Checked = Convert.ToBoolean(int.Parse(lstconfig.FirstOrDefault(x => x.DataKey.Equals("GPayEnable")).DataValue));
        hdlBankApp.Value = lstconfig.FirstOrDefault(x => x.DataKey.Equals("GPayEnable")).DataValue;

        chkCas.Checked = Convert.ToBoolean(int.Parse(lstconfig.FirstOrDefault(x => x.DataKey.Equals("CasEnable")).DataValue));
        hdCas.Value = lstconfig.FirstOrDefault(x => x.DataKey.Equals("CasEnable")).DataValue;
        chkVPB.Checked = Convert.ToBoolean(int.Parse(lstconfig.FirstOrDefault(x => x.DataKey.Equals("VPBPri")).DataValue));
        rptList.DataSource = new BankCodeTranfer().GetListCache().ToList();

        

        rptList.DataBind();


        var partnelist = new Partners().GetList().OrderBy(x=>x.Name);
        rptList2.DataSource = partnelist;
        rptList2.DataBind();

    }
   
    
    protected void btAdd_Click(object sender, EventArgs e)
    {

        var systemDataConfig = new SystemDataConfig();

        systemDataConfig.Update("BankCashEnable", Convert.ToInt32(ddlBankCashEnable.Checked).ToString());


        systemDataConfig.Update("BankInEnable", Convert.ToInt32(ddlBankInEnable.Checked).ToString());
        systemDataConfig.Update("MomoCashEnable", Convert.ToInt32(ddlMomoCashEnable.Checked).ToString());
        systemDataConfig.Update("MomoInEnable", Convert.ToInt32(ddlMomoInEnable.Checked).ToString());
        systemDataConfig.Update("GPayEnable", Convert.ToInt32(ddlBankApp.Checked).ToString());
        systemDataConfig.Update("BankCodeInMaintain", txtBankCodeInMaintain.Text);
        systemDataConfig.Update("BankPrefix", txtBankPrefix.Text);
        systemDataConfig.Update("BlockAccount", txtBlockAccount.Text);
        systemDataConfig.Update("CasEnable", Convert.ToInt32(chkCas.Checked).ToString());
        systemDataConfig.Update("VPBPri", Convert.ToInt32(chkVPB.Checked).ToString());
        systemDataConfig.DeleteCache();

        //string KeyCache = string.Format("VPB:OUT:MinAmount");
        //OTPDataCaching.SetCache(KeyCache, txtMinAmountVPB.Text.Trim(), 86400 * 180);


        AlertSuccesss.Text = "Cập nhật thành công";
        var _userLog = new UserLog
        {
            UserName = AppUtils.UserName,
            Action = "systemconfig",
            ActionName = "Cập nhật cấu hình",
            Description = "Cập nhật cấu hình"
        };
        _userLog.Add();

        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);

    }
    protected void btAdd_Click2(object sender, EventArgs e)
    {
        var systemDataConfig = new SystemDataConfig();

        systemDataConfig.Update("MaxBankCashAmount", txtMaxBankCashAmount.Text);
        systemDataConfig.Update("MaxMomoCashAmount", txtMaxMomoCashAmount.Text);
        systemDataConfig.Update("MinBankAproveAmount", txtMinBankAproveAmount.Text);
        systemDataConfig.Update("MaxBankCashAmount", txtMaxBankCashAmount.Text);
        systemDataConfig.Update("MinMomoCashAmount", txtMinMomoCashAmount.Text);
        systemDataConfig.Update("MinBankCashAmount", txtMinBankCashAmount.Text);
        systemDataConfig.Update("VPBOUTMinAmount", txtMinAmountVPB.Text);
        systemDataConfig.DeleteCache();

        string KeyCache = string.Format("VPB:OUT:MinAmount");
        OTPDataCaching.SetCache(KeyCache, txtMinAmountVPB.Text.Trim(), 86400 * 180);

        var _userLog = new UserLog
        {
            UserName = AppUtils.UserName,
            Action = "systemconfig",
            ActionName = "Cập nhật cấu hình",
            Description = "Cập nhật cấu hình"
        };
        _userLog.Add();
        AlertSuccesss.Text = "Cập nhật thành công";

        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);

    }
    protected void btAdd_Click3(object sender, EventArgs e)
    {
        var bankCodeTranfer = new BankCodeTranfer();
        for (int i = 0; i < rptList.Items.Count; i++)
        {

            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
            Label labelUserID = (Label)rptList.Items[i].FindControl("lblId");

            bankCodeTranfer.Update(Convert.ToInt32(labelUserID.Text), cbx.Checked ? 1 : 0);

        }
        var _userLog = new UserLog
        {
            UserName = AppUtils.UserName,
            Action = "systemconfig",
            ActionName = "Cập nhật cấu hình",
            Description = "Cập nhật cấu hình"
        };
        _userLog.Add();
        bankCodeTranfer.DeleteCache();
        AlertSuccesss.Text = "Cập nhật thành công";

        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);


    }
    protected void btSend_Click(object sender, EventArgs e)
    {
        var mess = txtMessage.Text.Trim();
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        if (!string.IsNullOrEmpty(mess))
        {
            mess = CleanTelegramText(mess);
            mess = HttpUtility.UrlEncode(mess);
            var lstpartner = new Partners().GetList();

            for (int i = 0; i < rptList2.Items.Count; i++)
            {

                CheckBox cbx = (CheckBox)rptList2.Items[i].FindControl("cbxParnter");
                Label labelUserID = (Label)rptList2.Items[i].FindControl("lblPartnerID");
                if(cbx.Checked)
                {
                    if(lstpartner.Exists(x => x.PartnerID.ToString() == labelUserID.Text))
                    {
                        var partner = lstpartner.FirstOrDefault(x => x.PartnerID.ToString() == labelUserID.Text);
                        //NLogLogger.Info("Send " + serializer.Serialize(partner));
                        
                        var chatid = partner.SMSPlusUrl;
                        //NLogLogger.Info("chatid " + chatid);
                        if (!string.IsNullOrEmpty(chatid))
                        {
                            TelegramClient.SendTeleV4(chatid, mess);
                        }
                    }

                }    
                //bankCodeTranfer.Update(Convert.ToInt32(labelUserID.Text), cbx.Checked ? 1 : 0);

            }
            AlertSuccesss.Text = "Gửi thông báo thành công";

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);

            //foreach (var partner in lstpartner)
            //{
            //    if (partner.Status == 1 )
            //    {
            //        var chatid = GetChatId(partner.PartnerCode);
            //        if (!string.IsNullOrEmpty(chatid))
            //        {
            //            TelegramClient.SendTeleV4(chatid, mess);
            //        }
            //    }
            //}




        }
    }
    public static string CleanTelegramText(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        string text = input;

        // Chuẩn hóa xuống dòng
        text = text.Replace("\r\n", "\n").Replace("\r", "\n");

        // Xóa HTML tags
        text = Regex.Replace(text, "<.*?>", string.Empty);

        // Xóa các ký tự điều khiển, nhưng giữ CR/LF/TAB
        text = Regex.Replace(text, @"[\x00-\x08\x0B\x0C\x0E-\x1F]", "");

        // Không cho quá nhiều dòng trống
        text = Regex.Replace(text, @"\n{3,}", "\n\n");

        return text.Trim();
    }
    //static string GetChatId(string id)
    //{

    //    string partnecode = "";
    //    var partnelist = new Partners().GetList();
    //    if (partnelist.Exists(x => x.PartnerCode == id))
    //        return partnelist.FirstOrDefault(x => x.PartnerCode == id).SMSPlusUrl;

    //    return partnecode;
    //}
    //static string GetChatId(string id)
    //{

    //    string partnecode = "";
    //    var partnelist = new Partners().GetList();
    //    if (partnelist.Exists(x => x.PartnerCode == id))
    //        return partnelist.FirstOrDefault(x => x.SMSPlusUrl == id).SMSPlusUrl;

    //    return partnecode;
    //}
}