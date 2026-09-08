using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using Libs.Utils;

public partial class Pages_Monitor_CardAPI_Search : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.CardAPISearch);


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
        string providerCodes = string.Empty;

        if (AppUtils.IsPartner && !AppUtils.IsAdmin)
        {

            if (string.IsNullOrEmpty(partnerCodes))
            {
                var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
                if (lstPartner != null && lstPartner.Count > 0)
                {
                    partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode).ToArray());
                }
            }


        }

        if (AppUtils.IsProvider && !AppUtils.IsAdmin)
        {
            if (string.IsNullOrEmpty(providerCodes))
            {
                var lstProvider = new Providers().GetListByUserId(AppUtils.UserID);
                if (lstProvider != null && lstProvider.Count > 0)
                {
                    providerCodes = string.Join(",", lstProvider.Select(e => e.ProviderCode).ToArray());
                }
            }

        }

        int top = Convert.ToInt32(drpTop.SelectedValue);
        string accountName = txtAccountName.Text.Trim().ToLower();
        string cardSerial = txtCardSerial.Text.Trim().ToLower();
        string cardCode = txtCardCode.Text.Trim().ToLower();
        string refCode = txtRefCode.Text.Trim().ToLower();

        CardAPILog _CardAPILog = new CardAPILog();
        if (txtCreatTime.Text.Trim() == "")
        {
            
            rptList.DataSource = _CardAPILog.Search(partnerCodes, top, accountName, cardSerial,cardCode, refCode, providerCodes);
        }
        else
        {
            DateTime creatTime = AppUtils.ToDateTime(txtCreatTime.Text);
            rptList.DataSource = _CardAPILog.Search(partnerCodes, top, accountName, cardSerial, cardCode, refCode, creatTime, providerCodes);
        }
        rptList.DataBind();
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }

    protected string DetailUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.CardAPIMonitorDetail + "?id=" + id;
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