using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;

public partial class Pages_PayGate_Products_Add : System.Web.UI.Page
{
    public string Type { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.ProductAdd);
        if (!IsPostBack)
        {
             
            txtType.SelectedValue = Type = Request["type"]; 
        }
     
    }

    protected void btAdd_Click(object sender, EventArgs e)
    {
        var _Product = new Products(); 
        _Product.Name = txtName.Text;
        _Product.Code = txtProductCode.Text;
        _Product.Status = Convert.ToInt32(chkIsActive.Checked);
        try
        {
            _Product.SubType = AppUtils.ToInt32(txtSubType.Text);
        }
        catch (Exception)
        {
            _Product.SubType = 0;
        }
        _Product.Type = AppUtils.ToInt32(txtType.SelectedValue);
        _Product.Add();
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProductList+"?type="+ _Product.Type);
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProductList);
    }
}