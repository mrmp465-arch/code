using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;

public partial class Pages_PayGate_Partners_Add : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.PartnersAdd);
    }

    protected void btAdd_Click(object sender, EventArgs e)
    {
        var _Partner = new Partners();
        _Partner.Name = txtName.Text;
        _Partner.PartnerCode = txtPartnerCode.Text.Trim().ToLower();
        _Partner.Status = Convert.ToInt32(chkIsActive.Checked);
        _Partner.SignatureType = Convert.ToInt32(drpSignatureType.SelectedValue);
        _Partner.PrivateKey = txtPrivateKey.Text.Trim();
        _Partner.PublicKey = txtPublicKey.Text.Trim();
        _Partner.SMSCommand = txtSMSCommand.Text.Trim();
        _Partner.SMSUrl = txtSMSUrl.Text.Trim();
        _Partner.SMSPlusCommand = txtSMSPlusCommand.Text.Trim();
        _Partner.SMSPlusUrl = txtSMSPlusUrl.Text.Trim();
        _Partner.SMSPlusCheckUrl = txtSMSPlusCheckUrl.Text.Trim();
        _Partner.Hotline = txtHotline.Text.Trim();

        _Partner.Add();

        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PartnersEdit + "?id=" + _Partner.PartnerID.ToString() + "&code=" + _Partner.PartnerCode.ToString());
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PartnersList);
    }
}