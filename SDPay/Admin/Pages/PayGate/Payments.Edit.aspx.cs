using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;

public partial class Pages_PayGate_Payments_Edit : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.PaymentsEdit);
        if (!IsPostBack)
        {
            init();
        }
    }

    private void init()
    {
        var _Payment = new Payments() { ServiceID = Convert.ToInt32(AppUtils.Request("id")) };
        _Payment = _Payment.Get();

        if (_Payment == null)
        {
            Response.Redirect(Resources.Url.PaymentsList);
        }

        txtName.Text = _Payment.Name;
        txtServiceCode.Text = _Payment.ServiceCode;
        txtClassName.Text = _Payment.ClassName;
        cbxIsActive.Checked = Convert.ToBoolean(_Payment.Status);
        txtConfig.Text = _Payment.Config;
        txtLastTransactionInfo.Text = _Payment.LastTransactionInfo;
        txtStartTime.Text = _Payment.StartTime.ToString();
        txtErrorCount.Text = _Payment.ErrorCount.ToString();

        // Đối tác
        Partners _Partner = new Partners();
        rptList.DataSource = _Partner.GetTable();
        rptList.DataBind();

        var _PartnerService = new PartnerService();
        List<PartnerService> list = _PartnerService.GetList(0, _Payment.ServiceID);
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
            TextBox txtIPAddress = (TextBox)rptList.Items[i].FindControl("txtIPAddress");
            TextBox txtCommandCode = (TextBox)rptList.Items[i].FindControl("txtCommandCode");
            Label lbl = (Label)rptList.Items[i].FindControl("lblPartnerID");
            int partnerID = Convert.ToInt32(lbl.Text);
            TextBox txtQuota = (TextBox)rptList.Items[i].FindControl("txtQuota");
            DropDownList drpOccurs = (DropDownList)rptList.Items[i].FindControl("drpOccurs");
            for (int j = 0; j < list.Count; j++)
            {
                _PartnerService = list[j];
                if (partnerID == _PartnerService.PartnerID)
                {
                    cbx.Checked = _PartnerService.Status == 1;
                    txtCommandCode.Text = _PartnerService.CommandCode;
                    txtIPAddress.Text = _PartnerService.IPAddress;
                    txtQuota.Text = _PartnerService.Quota.ToString();
                    drpOccurs.SelectedValue = _PartnerService.Occurs.ToString();
                    break;
                }
            }
        }

    }

    protected void btUpdate_Click(object sender, EventArgs e)
    {
        var _Payment = new Payments() { ServiceID = Convert.ToInt32(AppUtils.Request("id")) };
        _Payment = _Payment.Get();

        _Payment.Name = txtName.Text.Trim();
        _Payment.ServiceCode = txtServiceCode.Text.Trim().ToLower();
        _Payment.Description = txtDescription.Text.Trim();
        _Payment.ClassName = txtClassName.Text.Trim();
        _Payment.Config = txtConfig.Text.Trim();
        _Payment.Status = Convert.ToInt32(cbxIsActive.Checked);
        _Payment.LastTransactionInfo = txtLastTransactionInfo.Text.Trim();
        _Payment.StartTime = AppUtils.ToDateTime(txtStartTime.Text);
        _Payment.ErrorCount = AppUtils.ToInt32(txtErrorCount.Text);

        if (fileUploadClass.PostedFile != null && fileUploadClass.PostedFile.ContentLength != 0)
        {
            _Payment.DataSize = fileUploadClass.PostedFile.ContentLength;
            _Payment.ClassData = new byte[_Payment.DataSize];
            fileUploadClass.PostedFile.InputStream.Read(_Payment.ClassData, 0, _Payment.DataSize);
        }
        _Payment.Update();

        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PaymentsList);
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PaymentsList);
    }

    protected void btApply_Click(object sender, EventArgs e)
    {
        int serviceID = Convert.ToInt32(AppUtils.Request("id"));
        var _PartnerService = new PartnerService();
        List<PartnerService> list = _PartnerService.GetList(0, serviceID);
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
            Label lbl = (Label)rptList.Items[i].FindControl("lblPartnerID");
            TextBox txtIPAddress = (TextBox)rptList.Items[i].FindControl("txtIPAddress");
            TextBox txtCommandCode = (TextBox)rptList.Items[i].FindControl("txtCommandCode");
            TextBox txtQuota = (TextBox)rptList.Items[i].FindControl("txtQuota");
            DropDownList drpOccurs = (DropDownList)rptList.Items[i].FindControl("drpOccurs");
            _PartnerService.PartnerID = Convert.ToInt32(lbl.Text);
            _PartnerService.ServiceID = serviceID;
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
        }

    }
}