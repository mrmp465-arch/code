using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using System.Data;

public partial class Pages_PayGate_Provider_Add : System.Web.UI.Page
{
    public string Type { get; set; }
   
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.ProviderAdd);
        if (!IsPostBack)
        {
            txtSignatureType.Text = "0";
            txtOderNo.Text = "0";
            txtQuota.Text = "0";
            txtType.SelectedValue = Type = Request["type"];
            DataView dv1 = new Products().GetTable().DefaultView;
            if (string.IsNullOrEmpty(Type))
                Type = "7";
            dv1.RowFilter = "Status=1 and Type = " + Type;
            rptList.DataSource = dv1.ToTable();
            rptList.DataBind();
        }
     
    }

    protected void btAdd_Click(object sender, EventArgs e)
    {
        var _Provider = new Providers(); 
        _Provider.Name = txtName.Text;
        _Provider.ProviderCode = txtProviderCode.Text;
        _Provider.Status = Convert.ToInt32(chkIsActive.Checked);
        _Provider.SignatureType = AppUtils.ToInt32(txtSignatureType.Text);
        _Provider.PrivateKey = txtPrivateKey.Text;
        _Provider.PublicKey = txtPublicKey.Text;
        _Provider.OrderNo = AppUtils.ToInt32(txtOderNo.Text);
        _Provider.Quota = AppUtils.ToInt32(txtQuota.Text);
        _Provider.Occurs = Convert.ToInt32(drpOccurs.SelectedValue);  
        _Provider.Type = AppUtils.ToInt32(txtType.SelectedValue);


        //var list = new Products().GetList(_Provider.Type, 1);
        var _ProductCode = new List<string>();
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxProCode");
            HiddenField txtProduct = (HiddenField)rptList.Items[i].FindControl("txtProCode");
            if (cbx.Checked)
                _ProductCode.Add(txtProduct.Value);
        }
        _Provider.ProductCode = String.Join("|", _ProductCode.ToArray());


        _Provider.Add();
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProviderList+"?type="+ _Provider.Type);
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.ProviderList);
    }
}