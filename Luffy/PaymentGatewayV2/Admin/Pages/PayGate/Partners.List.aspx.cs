using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using System.Data;

public partial class Pages_PayGate_Partners_List : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.PartnersList);
        if (!IsPostBack)
        {
            BindData();
        }
    }
    protected void BindData()
    {
        var _Partner = new Partners();
        rptList.DataSource = _Partner.GetList().OrderBy(x => x.PartnerCode).ToList();
       
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
}