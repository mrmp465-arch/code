using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using System.Data;
using Libs.Utils;
using System.Web.Script.Serialization;

public partial class Pages_Momo_Partner_Add : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.MomoPartnerAdd);

        if (!IsPostBack)
        {
            init();
            BindData();
        }
    }
    private void init()
    {
        var lst = new Partners().GetList();
        lst = lst.OrderBy(x => x.PartnerCode).ToList();
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "PartnerCode";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Đối tác:", ""));

    }
    protected void btView_Click(object sender, EventArgs e)
    {
        //BindData();
    }
    private void BindData()
    {
    
    }
    protected void drpPartner_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtCallbackUrl.Text = "http://127.0.0.1:1592/Callback/MDrumCallback.ashx?partnercode="+drpPartner.SelectedValue;
        txtPartnerCode.Text = drpPartner.SelectedValue;
        txtPartnerName.Text = drpPartner.SelectedItem.Text;
    }
    protected void btAdd_Click(object sender, EventArgs e)
    {
        //check parnter
        if (string.IsNullOrEmpty(drpPartner.SelectedValue))
            return;
        var partnerId = 0;
        var partner = new PartnerMomo().GetPartner(drpPartner.SelectedValue);
        if(partner==null)
        {
            var mPartner = new MPartner
            {
                Code = drpPartner.SelectedValue,
                InCallbackUrl = txtCallbackUrl.Text,
                Name = drpPartner.SelectedItem.Text,
                Status=1,
                Secret=""
            };
            partnerId = new PartnerMomo().AddPartner(mPartner);
        }
        else
        {
            partnerId = partner.Id;
        }

        //nếu chưa có thì thêm mới
        //if(partnerId>0)
        //{
        //    for (int i = 0; i < rptList.Items.Count; i++)
        //    {
        //        CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
        //        Label lbPId = (Label)rptList.Items[i].FindControl("lblId");


        //        if(cbx.Checked)
        //        {
        //            var _PartnerMomo = new PartnerMomo();
        //            _PartnerMomo.PartnerId = partnerId;
        //            _PartnerMomo.MomoId = Convert.ToInt32(lbPId.Text);
        //            _PartnerMomo.Status = 0;
        //            _PartnerMomo.OrderNo = 1;
        //            _PartnerMomo.Add();
        //        }    
        //        //NLogLogger.Info(new string[] { "Data", "lbPId", lbPId.Text });
               
                

        //        //JavaScriptSerializer serializer = new JavaScriptSerializer();
        //        //NLogLogger.Info(new string[] { "Data", "lbPId", serializer.Serialize(_PartnerMomo) });
                
        //    }
        //}    
        //thêm mapping
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.MomoPartner);
    }
    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.MomoPartner);
    }
}