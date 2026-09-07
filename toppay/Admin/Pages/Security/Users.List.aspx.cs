using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using System.ComponentModel;
using ServiceStack.Common.Extensions;
using Libs.Report;

public partial class Pages_Security_Users_List : System.Web.UI.Page
{
    public List<PartnersDiscount> listpartnerDiscount;
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = Title + " - Danh sách người dùng";
        AppUtils.CheckRoles(Resources.Url.UsersList);
        if (!IsPostBack)
        {
            //var lstGroup = new PartnerGroup().GetList();
            //lstGroup= lstGroup.OrderBy(x => x.Name).ToList();
            //drpOrder.DataSource = lstGroup;
            //drpOrder.DataTextField = "Name";
            //drpOrder.DataValueField = "Name";
            //drpOrder.DataBind();
            
           
            drpOrder.Items.Insert(0, new ListItem("Nhóm:", ""));
            drpOrder.Items.Insert(1, new ListItem("inhouse", "inhouse"));
            drpOrder.Items.Insert(2, new ListItem("other", "other"));
            BindData();
        }
    }

    private void BindData()
    {
        listpartnerDiscount = new PartnersDiscount().GetList("", 2030, 1).Where(x => x.Date.Day == 1).ToList();
        var lst = new Users().GetList();
        //if (lst != null) 
        //    lst = lst.Where(e => e.ParentId == AppUtils.UserID || e.UserID == AppUtils.UserID).ToList();

        var status = int.Parse(drpStatus.SelectedValue);

        if (status > -1)
            lst = lst.Where(x => x.Status == status).ToList();


        var type = int.Parse(ddlType.SelectedValue);

        if (type == 1)
            lst = lst.Where(x => x.IsAdmin == 1).ToList();

        if (type == 2)
            lst = lst.Where(x => x.IsPartner == 1).ToList();

        if (type == 3)
            lst = lst.Where(x => x.IsTopup == 1).ToList();

        var order = drpOrder.SelectedValue;

        //if (!string.IsNullOrEmpty(order))
        //{
        //    lst = lst.Where(x => x.Source.Equals(order)).ToList();
        //}
        //else
        //{
        //    lst = lst.OrderBy(x => x.UserName).ToList();
        //}

        var total = lst.Sum(x => x.Balance);
        lblTotalBalance.Text = string.Format("Tổng số dư: {0}", total.ToString("#,#").Replace(",", "."));
        rptList.DataSource = AppUtils.ToDataTable(lst);
        rptList.DataBind();
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
    }
    protected void btApply_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            var _User = new Users();
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
            Label labelUserID = (Label)rptList.Items[i].FindControl("lblUserID");
            _User.UserID = Convert.ToInt32(labelUserID.Text);
            _User = _User.Get();
            var oldStatus = _User.Status;
            _User.Status = cbx.Checked ? 1 : 0;
            _User.Update();
            if (oldStatus != _User.Status)
            {
                //update parner;
                if (_User.IsPartner == 1)
                {
                    var _Partner = new Partners().Get(_User.UserName);
                    _Partner.Status = _User.Status;
                    _Partner.Update();
                }


            }

        }
        BindData();
    }
    public string GetUserType(string isAdmin, string isPartner, string topup)
    {
        if (isAdmin == "1")
            return "<div class=\"label label-success\">Admin</div>";
        if (isPartner == "1")
            return "<div class=\"label label-default\">Merchant</div>";
        if (topup == "1")
            return "<div class=\"label label-warning\">Support</div>";
        return "";
    }
    public string GetUserCk(string username)
    {
        if (listpartnerDiscount.Exists(x => x.PartnerCode == username))
        {
            var Discount = listpartnerDiscount.FirstOrDefault(x => x.PartnerCode == username);
            return String.Format("{0} | {1} | {2} | {3} ", Discount.DiscountBANKTRANFER.ToString("0.0###"), Discount.DiscountBANKOUTTRANFER.ToString("0.0###") ,Discount.DiscountMOMO.ToString("0.0###"), Discount.DiscountMOMOOUT.ToString("0.0###"));
        }
        return "";
    }
    public string GetUserRw(string username)
    {
        if (listpartnerDiscount.Exists(x => x.PartnerCode == username))
        {
            var Discount = listpartnerDiscount.FirstOrDefault(x => x.PartnerCode == username);
            return String.Format("{0} | {1} | {2} | {3}  ", Discount.RewardBANKTRANFER.ToString("0.0###"), Discount.RewardBANKOUTTRANFER.ToString("0.0###"), Discount.RewardVTT.ToString("0.0###") ,Discount.RewardMOMO.ToString("0.0###"), Discount.RewardMOMOOUT.ToString("0.0###"), Discount.RewardVTT.ToString("0.0###"));
        }
        return "";
    }
    protected void btAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.UsersAdd);
    }

}
