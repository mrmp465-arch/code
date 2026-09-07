using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;

public partial class Pages_Monitor_SMS_Search : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.SMSSearch);


        if (!IsPostBack)
        {
            init();
            //GetList();
        }

    }

    private void init()
    {
        txtCreatTime.Text = DateTime.Now.AddDays(1).ToString();
    }

    private void GetList()
    {
        string partnerCodes = string.Empty;
        if (string.IsNullOrEmpty(partnerCodes) && !AppUtils.IsAdmin)
        {
            List<Partners> lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
            if (lstPartner != null && lstPartner.Count > 0)
                partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode.ToString()).ToArray());
            else
                partnerCodes = "-1";
        }

        int top;
        top = Convert.ToInt32(drpTop.SelectedValue);
        string SenderNumber = txtSenderNumber.Text.Trim().ToLower();
        string Subject = txtSubject.Text.Trim().ToLower();

        MessageIn _MessageIn = new MessageIn();
        if (txtCreatTime.Text.Trim() == "")
        {
            rptList.DataSource = _MessageIn.Search(partnerCodes, top, SenderNumber, Subject);
        }
        else
        {
            DateTime creatTime = AppUtils.ToDateTime(txtCreatTime.Text);
            rptList.DataSource = _MessageIn.Search(partnerCodes, top, SenderNumber, Subject, creatTime);
        }
        rptList.DataBind();
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }

    protected string DetailUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.SMSMonitorDetail + "?id=" + id;
    }
    public string CheckPartner(object str)
    {

        //if (!AppUtils.IsAdmin)
        //{
        //    if (str.ToString() != AppUtils.PartnerCode)
        //        return "Đối tác khác";
        //}
        return str.ToString();
    }
}