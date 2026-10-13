using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;

public partial class Pages_Monitor_BuyCard_Monitor_Detail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BuyCardMonitorDetail);

        if (!IsPostBack)
        {
            BindData();
        }
    }

    private void BindData()
    {
        BuyCard _BuyCard = new BuyCard();
        _BuyCard.TransactionID = Convert.ToInt64(Request["id"]);

        _BuyCard = _BuyCard.Get();
        lblTransactionID.Text = _BuyCard.TransactionID.ToString();
        //lblTransactionNo.Text = _BuyCard.TransactionNo;
        //lblPartnerID.Text = _BuyCard.PartnerID.ToString();
        lblPartnerCode.Text = _BuyCard.PartnerCode;
        lblAccountName.Text = _BuyCard.AccountName;
        //lblAccountId.Text = _BuyCard.AccountId.ToString();
        lblOrderNo.Text = _BuyCard.OrderNo;
        //lblRequestTime.Text = _BuyCard.RequestTime.ToString();
        lblCardType.Text = _BuyCard.Provider;
        lblAmount.Text = _BuyCard.Amount.ToString();
        lblQuantity.Text = _BuyCard.Quantity.ToString();
        //lblErrorCode.Text = _BuyCard.ErrorCode.ToString();
        //lblMessage.Text = _BuyCard.Message;
        lblLogContent.Text = _BuyCard.LogContent;
        lblListCards.Text = _BuyCard.ListCards;
        lblCreatedTime.Text = _BuyCard.CreatedTime.ToString();
        lblStatus.Text = _BuyCard.Status + " (" + ResponseUtils.Description(_BuyCard.Status) + ")";
        lblProvider.Text = _BuyCard.ProviderCode;
    }
}
