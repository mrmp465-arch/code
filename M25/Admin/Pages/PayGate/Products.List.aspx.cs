using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using System.Data;

public partial class Pages_PayGate_Products_List : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.ProductList);
        if (!IsPostBack)
        {
            BindData();
        }
    }
   
    protected void BindData()
    {
        DataView dv1 = new Products().GetTable().DefaultView; 
        dv1.RowFilter = "Type = 7"; 
        rptList1.DataSource = dv1.ToTable(); ;
        rptList1.DataBind();

        DataView dv2 = new Products().GetTable().DefaultView; 
        dv2.RowFilter = "Type = 15"; 
        rptList2.DataSource = dv2.ToTable(); ;
        rptList2.DataBind();

        DataView dv3 = new Products().GetTable().DefaultView; 
        dv3.RowFilter = "Type = 13"; 
        rptList3.DataSource = dv3.ToTable(); ;
        rptList3.DataBind();

        DataView dv4 = new Products().GetTable().DefaultView;
        dv4.RowFilter = "Type = 18";
        rptList4.DataSource = dv4.ToTable(); ;
        rptList4.DataBind();

        DataView dv5 = new Products().GetTable().DefaultView;
        dv5.RowFilter = "Type = 5";
        rptList5.DataSource = dv5.ToTable(); ;
        rptList5.DataBind();

    }
    protected void btAdd_Click1(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProductAdd + "?type=1");
    }
    protected void btAdd_Click2(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProductAdd + "?type=14");
    }
    protected void btAdd_Click3(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProductAdd+"?type=13");
    }
    protected void btAdd_Click4(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProductAdd + "?type=18");
    }
    protected void btAdd_Click5(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProductAdd + "?type=5");
    }

    protected void btApply_Click1(object sender, EventArgs e)
    {
        updateProduct(rptList1, 1);
    }
    protected void btApply_Click2(object sender, EventArgs e)
    {
        updateProduct(rptList2, 14);
    }
    protected void btApply_Click3(object sender, EventArgs e)
    {
        updateProduct(rptList3, 13);
    }
    protected void btApply_Click4(object sender, EventArgs e)
    {
        updateProduct(rptList4, 18);
    }
    protected void btApply_Click5(object sender, EventArgs e)
    {
        updateProduct(rptList4, 5);
    }
    public void updateProduct(Repeater rptList, int _Type)
    {
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus"); 
            Label lbPId = (Label)rptList.Items[i].FindControl("lblId");
            var _Product = new Products();
            _Product.Id = Convert.ToInt32(lbPId.Text);
            _Product = _Product.Get();
            _Product.Status = cbx.Checked ? 1 : 0;
            _Product.Update();
        }
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProductList + "?Type=" + _Type);
    } 
}