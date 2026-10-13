using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;
using Libs.Utils;

public partial class Pages_PayGate_Partners_Edit : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.PartnersEdit);
        if (!IsPostBack)
        {
            init();
        }
    }

    private void init()
    {
        Partners _Partner = new Partners() { PartnerID = Convert.ToInt32(AppUtils.Request("id")) };
        _Partner = _Partner.Get();


        if (_Partner == null)
        {
            Response.Redirect(Resources.Url.PartnersList);
        }

        lblPartner.Text = _Partner.PartnerCode;
        lblPartnerId.Text = _Partner.PartnerID.ToString();
        txtName.Text = _Partner.Name;
        txtRequestType.SelectedValue = _Partner.RequestType.ToString();

        txtPartnerCode.Text = _Partner.PartnerCode;
        txtPrivateKey.Text = _Partner.PrivateKey;
        txtPublicKey.Text = _Partner.PublicKey;
        chkIsActive.Checked = Convert.ToBoolean(_Partner.Status);
        drpSignatureType.SelectedValue = _Partner.SignatureType.ToString();

        txtSMSCommand.Text = _Partner.SMSCommand;
        txtSMSUrl.Text = _Partner.SMSUrl;
        txtSMSPlusCommand.Text = _Partner.SMSPlusCommand;

        //txtHotline.Text = _Partner.Hotline;
        //loại thẻ
        //drlBankGroup.DataSource = new PartnerBank().GetListPartner();
        //drlBankGroup.DataTextField = "Code";
        //drlBankGroup.DataValueField = "Code";
        //drlBankGroup.DataBind();

        drlMomoGroup.DataSource = new PartnerMomo().GetListPartner();
        drlMomoGroup.DataTextField = "Code";
        drlMomoGroup.DataValueField = "Code";
        drlMomoGroup.DataBind();

        ddlUser.DataSource = new Users().GetList().Where(x => x.IsPartner == 1).ToList();
        ddlUser.DataTextField = "UserName";
        ddlUser.DataValueField = "UserName";
        ddlUser.DataBind();
        ddlUser.Items.Insert(0, new ListItem("Chọn tài khoản:", ""));


        txtSMSPlusUrl.Text = _Partner.SMSPlusUrl;
        drlMomoGroup.SelectedValue = _Partner.SMSPlusCheckUrl;
        ddlUser.SelectedValue = _Partner.Hotline;


        //getProvider(drpCardType.SelectedValue);
        // Dịch vụ
        var _Payment = new Payments();
        rptList.DataSource = _Payment.GetTable();
        rptList.DataBind();

        var _PartnerService = new PartnerService();
        List<PartnerService> list = _PartnerService.GetList(_Partner.PartnerID, 0);
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
            TextBox txtIPAddress = (TextBox)rptList.Items[i].FindControl("txtIPAddress");
            TextBox txtCommandCode = (TextBox)rptList.Items[i].FindControl("txtCommandCode");
            Label lbl = (Label)rptList.Items[i].FindControl("lblServiceID");
            TextBox txtQuota = (TextBox)rptList.Items[i].FindControl("txtQuota");
            Label lbQuota = (Label)rptList.Items[i].FindControl("lbQuota");
            DropDownList drpOccurs = (DropDownList)rptList.Items[i].FindControl("drpOccurs");

            int serviceID = Convert.ToInt32(lbl.Text);
            for (int j = 0; j < list.Count; j++)
            {
                _PartnerService = list[j];
                if (serviceID == _PartnerService.ServiceID)
                {
                    cbx.Checked = _PartnerService.Status == 1;
                    txtCommandCode.Text = _PartnerService.CommandCode;
                    txtIPAddress.Text = _PartnerService.IPAddress;
                    txtQuota.Text = _PartnerService.Quota.ToString();
                    if (_PartnerService.Quota > 0)
                        lbQuota.Text = _PartnerService.Quota.ToString("#,#").Replace(",", ".");
                    drpOccurs.SelectedValue = _PartnerService.Occurs.ToString();
                    break;
                }
            }

            Repeater rptListPro = rptList.Items[i].FindControl("rptProList") as Repeater;
            var listProvider = new Providers().GetList(serviceID);
            var listPartnerProvider = new PartnerProvider().GetList(_Partner.PartnerID, 0);

            for (int k = 0; k < rptListPro.Items.Count; k++)
            {
                CheckBox cbxProStatus = (CheckBox)rptListPro.Items[k].FindControl("cbxProStatus");
                HiddenField txtProId = (HiddenField)rptListPro.Items[k].FindControl("txtProId");
                int providerId = Convert.ToInt32(txtProId.Value);
                foreach (var partnerProvider in listPartnerProvider)
                {
                    if (providerId == partnerProvider.ProviderId)
                    {
                        cbxProStatus.Checked = true;
                    }
                }




            }
        }

    }

    protected List<Providers> ListProviders(object serviceId)
    {
        return new Providers().GetList(Convert.ToInt32(serviceId));
    }

    protected void btUpdate_Click(object sender, EventArgs e)
    {
        var _Partner = new Partners() { PartnerID = Convert.ToInt32(AppUtils.Request("id")) };
        _Partner = _Partner.Get();

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
        _Partner.SMSPlusCheckUrl = drlMomoGroup.SelectedValue.Trim();
        _Partner.Hotline = ddlUser.SelectedValue.Trim();
        _Partner.RequestType = int.Parse(txtRequestType.SelectedValue);
        _Partner.Update();
        string KeyCache = string.Format("{0}:{1}", "RedisPartners", _Partner.PartnerCode);
        DataCaching.RemoveCache(KeyCache);

        var _userLog = new UserLog
        {
            UserName = AppUtils.UserName,
            Action = "partnerupdate",
            ActionName = "Cập nhật đối tác",
            Description = "Cập nhật đối tác" + _Partner.Name
        };
        _userLog.Add();

        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PartnersList);
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PartnersList);
    }
    protected void drpCardType_TextChanged(object sender, EventArgs e)
    {
        getProvider(drpCardType.SelectedValue);
    }
    private void getProvider(string productCode)
    {
        lblProvider.Text = new Providers().GetCardConditionList(productCode, lblPartner.Text, 0);
    }
    protected void btApply_Click(object sender, EventArgs e)
    {
        int partnerID = Convert.ToInt32(AppUtils.Request("id"));
        string partnerCode = AppUtils.RequestCode("code");
        var _PartnerService = new PartnerService();
        //List<PartnerService> list = _PartnerService.GetList(partnerID, 0);
        var LstPartnerProvider = new PartnerProvider().GetList(0, 0);

        for (int i = 0; i < rptList.Items.Count; i++)
        {
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
            Label lbl = (Label)rptList.Items[i].FindControl("lblServiceID");
            TextBox txtIPAddress = (TextBox)rptList.Items[i].FindControl("txtIPAddress");
            TextBox txtCommandCode = (TextBox)rptList.Items[i].FindControl("txtCommandCode");
            TextBox txtQuota = (TextBox)rptList.Items[i].FindControl("txtQuota");
            DropDownList drpOccurs = (DropDownList)rptList.Items[i].FindControl("drpOccurs");
            _PartnerService.PartnerID = partnerID;
            _PartnerService.ServiceID = Convert.ToInt32(lbl.Text);
            if (cbx.Checked)
            {
                _PartnerService.Get();
                _PartnerService.IPAddress = txtIPAddress.Text;
                _PartnerService.CommandCode = txtCommandCode.Text;
                _PartnerService.Status = Convert.ToInt32(cbx.Checked);
                _PartnerService.Quota = Convert.ToInt64(txtQuota.Text);
                _PartnerService.Occurs = Convert.ToInt32(drpOccurs.SelectedValue);
                _PartnerService.Update();
            }
            else
            {
                _PartnerService.Delete();
            }

            Repeater rptListPro = rptList.Items[i].FindControl("rptProList") as Repeater;

            for (int j = 0; j < rptListPro.Items.Count; j++)
            {
                CheckBox cbxProStatus = (CheckBox)rptListPro.Items[j].FindControl("cbxProStatus");
                HiddenField txtProCode = (HiddenField)rptListPro.Items[j].FindControl("txtProCode");
                HiddenField txtProId = (HiddenField)rptListPro.Items[j].FindControl("txtProId");
                var item = LstPartnerProvider.FirstOrDefault(a => a.PartnerId == partnerID && a.ProviderId == Convert.ToInt32(txtProId.Value));
                if (cbxProStatus.Checked)
                {
                    if (item == null)
                    {
                        var _PartnerProvider = new PartnerProvider();
                        _PartnerProvider.PartnerId = partnerID;
                        _PartnerProvider.PartnerCode = partnerCode;
                        _PartnerProvider.ProviderId = Convert.ToInt32(txtProId.Value);
                        _PartnerProvider.ProviderCode = txtProCode.Value.ToString();
                        _PartnerProvider.Add();
                    }
                }
                else
                if (item != null)
                    item.Delete();

            }
        }



    }
}