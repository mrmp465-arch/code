using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.CardTelco;
using CardAPILog = Libs.Report.CardAPILog;

public partial class Pages_Monitor_CardAPI_Monitor_Detail : System.Web.UI.Page
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.CardAPIMonitorDetail);

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
        
    }

    protected void txtRecheck_Click(object sender, EventArgs e)
    {

        var handler = CardTelcoFactory.GetHandler(lblProvider.Text);
        var result = handler.ReCheck(lblTransactionID.Text);
        lblRecheck.Text = result.Description;

    }
}