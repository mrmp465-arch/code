using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using System.Data;
using Libs.Utils;

public partial class Pages_PayGate_Provider_List : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.ProviderList);
        if (!IsPostBack)
        {
            BindData();
        }
    }
    protected void btApply_Click1(object sender, EventArgs e)
    {
        updateProvider(rptList1, 7);
    }
    protected void btApply_Click2(object sender, EventArgs e)
    {
        updateProvider(rptList2, 15);
    }
    protected void btApply_Click3(object sender, EventArgs e)
    {
        updateProvider(rptList3, 13);
    }
    protected void btApply_Click4(object sender, EventArgs e)
    {
        updateProvider(rptList4, 17);
    }
    protected void btApply_Click5(object sender, EventArgs e)
    {
        updateProvider(rptList5, 18);
    }
    public void updateProvider(Repeater rptList, int _Type)
    {
        NLogLogger.Info(new string[] { "rptList", rptList.Items.Count.ToString() });
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
            TextBox tqt = (TextBox)rptList.Items[i].FindControl("txtQuota");
            TextBox tbx = (TextBox)rptList.Items[i].FindControl("txtOrderNo");
            Label lbPId = (Label)rptList.Items[i].FindControl("lblProviderId");

            var tbxValue = tbx.Text;
            if (tbx.Text.Contains(','))
                tbxValue = tbx.Text.Split(',')[1];

            var tqtValue = tqt.Text;
            if (tqt.Text.Contains(','))
                tqtValue = tqt.Text.Split(',')[1];

            var _Provider = new Providers();
            _Provider.ProviderId = Convert.ToInt32(lbPId.Text);
            _Provider = _Provider.Get();
            NLogLogger.Info(new string[] { _Provider.ProviderId.ToString(), "cbx.Checked", cbx.Checked.ToString() });
            _Provider.Status = cbx.Checked ? 1 : 0;
            _Provider.Quota = Convert.ToInt64(tqtValue) * 1000;
            _Provider.OrderNo = Convert.ToInt32(tbxValue);
            _Provider.Update();
        }
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProviderList + "?Type=" + _Type);
    }
    protected void BindData()
    {
        DataView dv1 = new Providers().GetTable().DefaultView;
        dv1.Sort = "OrderNo asc";
        dv1.RowFilter = "Type = 7";
        DataTable sortedDT1 = dv1.ToTable();
        rptList1.DataSource = sortedDT1;
        rptList1.DataBind();

        DataView dv2 = new Providers().GetTable().DefaultView;
        dv2.Sort = "OrderNo asc";
        dv2.RowFilter = "Type = 15";
        DataTable sortedDT2 = dv2.ToTable();
        rptList2.DataSource = sortedDT2;
        rptList2.DataBind();

        DataView dv3 = new Providers().GetTable().DefaultView;
        dv3.Sort = "OrderNo asc";
        dv3.RowFilter = "Type = 13";
        DataTable sortedDT3 = dv3.ToTable();
        rptList3.DataSource = sortedDT3;
        rptList3.DataBind();

        DataView dv4 = new Providers().GetTable().DefaultView;
        dv4.Sort = "OrderNo asc";
        dv4.RowFilter = "Type = 17";
        DataTable sortedDT4 = dv4.ToTable();
        rptList4.DataSource = sortedDT4;
        rptList4.DataBind();

        DataView dv5 = new Providers().GetTable().DefaultView;
        dv5.Sort = "OrderNo asc";
        dv5.RowFilter = "Type = 18";
        DataTable sortedDT5 = dv5.ToTable();
        rptList5.DataSource = sortedDT5;
        rptList5.DataBind();

        drpCardType.DataSource = new Products().GetList(7, 1);
        drpCardType.DataTextField = "Name";
        drpCardType.DataValueField = "Code";
        drpCardType.DataBind();
        drpCardType.Items.Insert(0, new ListItem("Loại thẻ:", ""));

    }
    protected void btAdd_Click1(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProviderAdd + "?type=7");
    }
    protected void btAdd_Click2(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProviderAdd + "?type=14");
    }
    protected void btAdd_Click3(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProviderAdd + "?type=13");
    }
    protected void btAdd_Click4(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProviderAdd + "?type=17");
    }
    protected void btAdd_Click5(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProviderAdd + "?type=18");
    }
    public string GetOccurs(object str)
    {
        if (str.ToString() == "1")
            return "Ngày";
        else if (str.ToString() == "2")
            return "Tuần";
        else if (str.ToString() == "3")
            return "Tháng";
        else if (str.ToString() == "4")
            return "Giờ";
        return "";
    }

    protected void drpCardType_TextChanged(object sender, EventArgs e)
    {
        var dv1 = new Providers().GetList(7).OrderBy(x => x.OrderNo).ToList();

        if (drpCardType.SelectedValue != "")
        {
            dv1 = dv1.Where(x => x.ProductCode.Contains(drpCardType.SelectedValue)).ToList();
        }

        if (drpStatus.SelectedValue == "1")
            dv1 = dv1.Where(x => x.Status == 1).ToList();

        rptList1.DataSource = dv1;
        rptList1.DataBind();
    }
    protected void drpStatus_TextChanged(object sender, EventArgs e)
    {
        var dv1 = new Providers().GetList(7).OrderBy(x => x.OrderNo).ToList();

        if (drpCardType.SelectedValue != "")
        {
            dv1 = dv1.Where(x => x.ProductCode.Contains(drpCardType.SelectedValue)).ToList();
        }

        if (drpStatus.SelectedValue == "1")
            dv1 = dv1.Where(x => x.Status == 1).ToList();

        rptList1.DataSource = dv1;
        rptList1.DataBind();
        rptList1.DataSource = dv1;
        rptList1.DataBind();
    }
}