using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;

public partial class Pages_PayGate_Products_Edit : System.Web.UI.Page
{
    public string Type { get; set; }
    public string cardType { get; set; }
    public bool GetCheckCardType(string CardType)
    {
        if (!string.IsNullOrEmpty(cardType))
        {
            var lst = cardType.Split('|');
            if (lst.Contains(CardType))
                return true;
        } 
            return false;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
         AppUtils.CheckRoles(Resources.Url.ProductEdit);
        if (!IsPostBack)
        {
            init();
      //      txtType.SelectedValue = Type = Request["type"];
        }
    }

    private void init()
    {


        var Id = Convert.ToInt32(AppUtils.Request("id"));
        var _Product = new Products().Get(Id); 
        if (_Product == null)
        {
            Response.Redirect(Resources.Url.ProductList);
        } 
        txtName.Text = _Product.Name;
        txtCode.Text = _Product.Code; 
        chkIsActive.Checked = Convert.ToBoolean(_Product.Status);
        txtSubType.Text =  _Product.SubType.ToString();        
        txtType.SelectedValue = _Product.Type.ToString();  
    }

    protected void btUpdate_Click(object sender, EventArgs e)
    {

        var _Product = new Products() { Id = Convert.ToInt32(AppUtils.Request("id")) };
        _Product = _Product.Get();

        _Product.Name= txtName.Text ;
        _Product.Code= txtCode.Text ;
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
        _Product.Update();
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProductList + "?type=" + _Product.Type);
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProductList);
    }

    protected void btApply_Click(object sender, EventArgs e)
    {
        
    }
}