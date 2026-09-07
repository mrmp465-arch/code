using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Utils;
public partial class Pages_PayGate_Partners_Add : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.PartnersAdd);
        if (!IsPostBack)
        {
            init();
        }
    }
    private void init()
    {
        //loại thẻ
        drlBankGroup.DataSource = new PartnerBank().GetListPartner();
        drlBankGroup.DataTextField = "Code";
        drlBankGroup.DataValueField = "Code";
        drlBankGroup.DataBind();

        drlMomoGroup.DataSource = new PartnerMomo().GetListPartner();
        drlMomoGroup.DataTextField = "Code";
        drlMomoGroup.DataValueField = "Code";
        drlMomoGroup.DataBind();

        ddlUser.DataSource = new Users().GetList();
        ddlUser.DataTextField = "UserName";
        ddlUser.DataValueField = "UserName";
        ddlUser.DataBind();
        ddlUser.Items.Insert(0, new ListItem("Chọn tài khoản:", ""));
        var key = Libs.Utils.Encrypts.MD5(DateTime.Now.ToString("dd/MM/yyy hh:mm:ss"));
        txtPrivateKey.Text = key;
        txtPublicKey.Text = key;
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
        _Partner.SMSPlusUrl = drlBankGroup.SelectedValue.Trim();
        _Partner.SMSPlusCheckUrl ="";
        _Partner.Hotline = ddlUser.SelectedValue.Trim();

        _Partner.Add();

        //Log User
        var _userLog = new UserLog
        {
            UserName = AppUtils.UserName,
            Action = "partneradd",
            ActionName = "Thêm mới đối tác",
            Description = "Thêm mới đối tác" + _Partner.Name
        };
        _userLog.Add();

        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PartnersEdit + "?id=" + _Partner.PartnerID.ToString() + "&code=" + _Partner.PartnerCode.ToString());
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PartnersList);
    }
}