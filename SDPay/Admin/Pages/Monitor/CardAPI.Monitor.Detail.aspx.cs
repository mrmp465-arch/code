using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.CardTelco;
using Libs.Utils;
using CardAPILog = Libs.Report.CardAPILog;

public partial class Pages_Monitor_CardAPI_Monitor_Detail : System.Web.UI.Page
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.CardAPIMonitorDetail);
        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
        if (!IsPostBack)
        {
            BindData();
        }
    }

    private void BindData()
    {
        CardAPILog _CardAPILog = new CardAPILog();
        _CardAPILog.TransactionID = Convert.ToInt64(Request["id"]);
        _CardAPILog = _CardAPILog.Get();
        lblTransactionID.Text = _CardAPILog.TransactionID.ToString();
        lblAccountName.Text = _CardAPILog.AccountName;
        lblCardSerial.Text = _CardAPILog.CardSerial;
        lblCardCode.Text = _CardAPILog.CardCode;
        lblAmount.Text = _CardAPILog.Amount.ToString();
        lblCardType.Text = _CardAPILog.CardType;
        lblProvider.Text = _CardAPILog.Provider;
        lblCreatTime.Text = _CardAPILog.CreatTime.ToString();
        lblLastTime.Text = _CardAPILog.LastTime.ToString();
        lblRefCode.Text = _CardAPILog.RequestNo;
        lblCallbackUrl.Text = _CardAPILog.CallbackUrl;
        lblStatus.Text = _CardAPILog.Status + " (" + ResponseUtils.Description(_CardAPILog.Status) + ")";
        txtLog.Text = _CardAPILog.Description;

        //log callback
        var lstLogData = LogCache.GetLogCard(_CardAPILog.TransactionID);
        rptList.DataSource = lstLogData;
        rptList.DataBind();

    }
    public string DecodeFromUtf8(string str)
    {
        // copy the string as UTF-8 bytes.
        return Regex.Replace(str, @"\\u([0-9a-fA-F]{4})", match =>
        {
            var unicodeValue = Convert.ToInt32(match.Groups[1].Value, 16);
            var unicodeChar = char.ConvertFromUtf32(unicodeValue);
            return unicodeChar;
        });
    }
    protected void txtRecheck_Click(object sender, EventArgs e)
    {

        var handler = CardTelcoFactory.GetHandler(lblProvider.Text);
        var result = handler.ReCheck(lblTransactionID.Text);
        lblRecheck.Text = result.Description;

    }
}