using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;

public partial class Pages_Monitor_SMS_Monitor_Detail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.SMSReport);

        if (!IsPostBack)
        {
            BindData();
        }
    }

    private void BindData()
    {
        MessageIn _MessageIn = new MessageIn();
        _MessageIn.Id = Convert.ToInt64(Request["id"]);

        _MessageIn= _MessageIn.Get();
        lblId.Text = _MessageIn.Id.ToString();
        lblProvider.Text= _MessageIn.Provider;
        lblType.Text= _MessageIn.Type.ToString();
        lblSenderId.Text= _MessageIn.SenderId.ToString();
        lblSenderNumber.Text= _MessageIn.SenderNumber;
        lblReceiverNumber.Text= _MessageIn.ReceiverNumber.ToString();
        lblReceiverId.Text = _MessageIn.ReceiverId.ToString();
        lblSubject.Text= _MessageIn.Subject;
        lblPartnerCode.Text= _MessageIn.PartnerCode;
        lblPartnerCommand.Text= _MessageIn.PartnerCommand;
        lblContent.Text= _MessageIn.Content;
        lblSentTime.Text= _MessageIn.SentTime.ToString();
        lblReceivedTime.Text= _MessageIn.ReceivedTime.ToString();
        lblRefTranId.Text= _MessageIn.RefTranId;
        lblAmount.Text= _MessageIn.Amount.ToString();
        lblStatus.Text= _MessageIn.Status.ToString();
        lblDescription.Text= _MessageIn.Description.ToString();
        lblCreatedTime.Text= _MessageIn.CreatedTime.ToString();
        lblModifiedTime.Text= _MessageIn.ModifiedTime.ToString();
        lblStatus.Text= _MessageIn.Status + " (" + ResponseUtils.Description(_MessageIn.Status) + ")";
        lblProvider.Text = _MessageIn.Provider;
    }
}
