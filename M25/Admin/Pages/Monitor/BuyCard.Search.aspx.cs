using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;

public partial class Pages_Monitor_BuyCard_Search : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BuyCardSearch);


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
        string partnerIDs = string.Empty;
        if (string.IsNullOrEmpty(partnerIDs) && !AppUtils.IsAdmin)
        {
            List<Partners> lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
            if (lstPartner != null && lstPartner.Count > 0)
                partnerIDs = string.Join(",", lstPartner.Select(e => e.PartnerID.ToString()).ToArray());
            else
                partnerIDs = "-1";
        }

        int top;
        top = Convert.ToInt32(drpTop.SelectedValue);
        string accountName = txtAccountName.Text.Trim().ToLower();
        string OrderNo = txtOrderNo.Text.Trim().ToLower();

        BuyCard _BuyCard = new BuyCard();
        if (txtCreatTime.Text.Trim() == "")
        {
            rptList.DataSource = _BuyCard.Search(partnerIDs,top, accountName, OrderNo);
        }
        else
        {
            DateTime creatTime = AppUtils.ToDateTime(txtCreatTime.Text);
            rptList.DataSource = _BuyCard.Search(partnerIDs,top, accountName, OrderNo, creatTime);
        }
        rptList.DataBind();
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }

    protected string DetailUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.BuyCardMonitorDetail + "?id=" + id;
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