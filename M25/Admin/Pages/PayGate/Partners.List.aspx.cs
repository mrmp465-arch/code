using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using System.Data;
using Libs.Report;

public partial class Pages_PayGate_Partners_List : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.PartnersList);
        if (!IsPostBack)
        {
            // Year
            drpYear.Items.Add(new ListItem("Năm:", "0"));
            for (int i = 2013; i <= DateTime.Now.Year; i++)
            {
                drpYear.Items.Add(new ListItem(i.ToString()));
            }

            // Month
            drpMonth.Items.Add(new ListItem("Tháng:", "0"));
            for (int i = 1; i <= 12; i++)
            {
                drpMonth.Items.Add(new ListItem(i.ToString()));
            }

            // Day
            drpDay.Items.Add(new ListItem("Ngày", "0"));
            for (int i = 1; i <= 31; i++)
            {
                drpDay.Items.Add(new ListItem(i.ToString()));
            }
            drpYear.SelectedValue = DateTime.Now.Year.ToString();
            drpMonth.SelectedValue = DateTime.Now.Month.ToString();
            drpDay.SelectedValue = DateTime.Now.Day.ToString();

            BindData();
        }
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
    }
    protected void BindData()
    {
        var _Partner = new Partners();
        var lstPartner = _Partner.GetList().OrderBy(x => x.SMSUrl).OrderBy(x => x.PartnerCode).ToList();

        if (!string.IsNullOrEmpty(drpGroup.SelectedValue))
        {
            lstPartner = lstPartner.Where(x => x.SMSUrl == drpGroup.SelectedValue).ToList();
        }
        var status = int.Parse(drpStatus.SelectedValue);

        if (status > -1)
            lstPartner = lstPartner.Where(x => x.Status == status).ToList();

        var listpartnerDiscount = new PartnersDiscount().GetList("", 2030, 1).Where(x => x.Date.Day == 1).ToList();

        foreach (var item in lstPartner)
        {
            item.SMSCommand = "";
            if (listpartnerDiscount.Exists(x => x.PartnerCode.ToLower() == item.PartnerCode.ToLower()))
            {
                try
                {
                    var Discount = listpartnerDiscount.FirstOrDefault(x => x.PartnerCode.ToLower() == item.PartnerCode.ToLower());
                    item.SMSCommand = String.Format("{0} | {1} | {2} | {3} ", Discount.DiscountBANKTRANFER.ToString("0.0###"), Discount.DiscountBANKOUTTRANFER.ToString("0.0###"), Discount.RewardBANKTRANFER.ToString("0.0###"), Discount.RewardBANKOUTTRANFER.ToString("0.0###"));
                }
                catch
                {

                }

            }
        }
        rptList.DataSource = lstPartner;

        rptList.DataBind();
    }
    protected void btApply_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            var _Partner = new Partners();
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
            Label labelUserID = (Label)rptList.Items[i].FindControl("lblPartnerID");
            _Partner.PartnerID = Convert.ToInt32(labelUserID.Text);
            _Partner = _Partner.Get();
            _Partner.Status = cbx.Checked ? 1 : 0;
            _Partner.Update();
        }
        BindData();
    }
    protected void btAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PartnersAdd);
    }
    public string CheckActive(string partnerCode)
    {
        var result = new Partners().CheckActive(partnerCode);
        if (result==1)
            return "<span class=\"label label-success\">Có</span>";
        return "<span class=\"label label-default\">Không</span>";
    }
    protected string SignatureName(string signatureType)
    {
        int sign = Convert.ToInt32(signatureType);
        switch (sign)
        {
            case (int)SignatureType.MD5:
                return SignatureType.MD5.ToString();
            case (int)SignatureType.RSA:
                return SignatureType.RSA.ToString();
            default:
                return "";
        }
    }

    protected void drpGroup_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindData();
    }
}