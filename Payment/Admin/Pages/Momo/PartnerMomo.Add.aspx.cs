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
public partial class Pages_Momo_PartnerMomo_Add : System.Web.UI.Page
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
        var lst = new PartnerMomo().GetListPartner();
        lst = lst.OrderBy(x => x.Code).ToList();
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "Id";
        drpPartner.DataBind();
        //drpPartner.Items.Insert(0, new ListItem("Đối tác:", "0"));

    }
    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
    }
    private void BindData()
    {
        var _Momo = new MomoAccounts(); 
        var data = _Momo.GetList().Where(x=>x.Type!= "OUTALL" ).OrderBy(x => x.Id).ToList();

        var name = txtName.Text.Trim();
        if (!string.IsNullOrEmpty(name))
            data = data.Where(x => x.MomoName.Contains(name)).ToList();

        var mobile = txtMobile.Text.Trim();
        if (!string.IsNullOrEmpty(mobile))
            data = data.Where(x => x.MomoId.Contains(mobile)).ToList();



        var lstMapping = new PartnerMomo().GetList();
        var lstAcount = new List<MomoAccounts>();
        foreach (var item in data)
        {
            if (!lstMapping.Exists(x => x.MomoId == item.Id))
            {
                lstAcount.Add(item);
            }
        }
        rptList.DataSource = lstAcount;
        rptList.DataBind();
    }
    //protected void drpPartner_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    txtCallbackUrl.Text = "http://127.0.0.1:1592/Callback/MDrumCallback.ashx?partnercode=" + drpPartner.SelectedValue;
    //    txtPartnerCode.Text = drpPartner.SelectedValue;
    //    txtPartnerName.Text = drpPartner.SelectedItem.Text;
    //}
    protected void btAdd_Click(object sender, EventArgs e)
    {
        //check parnter
    
        var partnerId = int.Parse(drpPartner.SelectedValue);
        //nếu chưa có thì thêm mới
        if (partnerId > 0)
        {
            for (int i = 0; i < rptList.Items.Count; i++)
            {
                CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
                Label lbPId = (Label)rptList.Items[i].FindControl("lblId");


                if (cbx.Checked)
                {
                    var _PartnerMomo = new PartnerMomo();
                    _PartnerMomo.PartnerId = partnerId;
                    _PartnerMomo.MomoId = Convert.ToInt32(lbPId.Text);
                    _PartnerMomo.Status = 1;
                    _PartnerMomo.OrderNo = 1;
                    _PartnerMomo.Add();
                }
                //NLogLogger.Info(new string[] { "Data", "lbPId", lbPId.Text });



                //JavaScriptSerializer serializer = new JavaScriptSerializer();
                //NLogLogger.Info(new string[] { "Data", "lbPId", serializer.Serialize(_PartnerMomo) });
                //thêm mapping
                
            }
            Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PartnerMomo);
        }
        else
        {
            AlertInfos.Text = "Vui lòng chọn lại đối tác";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            return;
        }
       
    }
    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PartnerMomo);
    }
    public string GetStatusActive(object statusOver)
    {
        if (statusOver.ToString() == "1")
        {
            return "<span class=\"label label-success\">Kích hoạt</span>";
        }

        if (statusOver.ToString() == "-1")
        {
            return "<span class=\"label label-default\">Bỏ qua</span>";
        }

        if (statusOver.ToString() == "0")
        {
            return "<span class=\"label label-warning\">Chưa kích hoạt</span>";
        }

        return statusOver.ToString();
    }
    public string GetStatus(object statusOver)
    {
        if (statusOver.ToString() == "1")
        {
            return "<span class=\"label label-success\">Normal</span>";
        }

        if (statusOver.ToString() == "2")
        {
            return "<span class=\"label label-warning\">OverDay</span>";
        }

        if (statusOver.ToString() == "3")
        {
            return "<span class=\"label label-danger\">OverMonth</span>";
        }
        if (statusOver.ToString() == "4")
        {
            return "<span class=\"label label-info\">OverMin</span>";
        }
        return "N/A";
    }

    public string GetStatusExtra(object statusOver)
    {
        if (statusOver.ToString() == "1")
        {
            return "<span class=\"label label-success\">Logged</span>";
        }
        if (statusOver.ToString() == "2")
        {
            return "<span class=\"label label-warning\">OTPRequired</span>";
        }
        if (statusOver.ToString() == "0")
        {
            return "<span class=\"label label-warning\">Ide</span>";
        }
        if (statusOver.ToString() == "-1")
        {
            return "<span class=\"label label-danger\">Error</span>";
        }
        if (statusOver.ToString() == "-3")
        {
            return "<span class=\"label label-danger\">LoginFailed</span>";
        }
        if (statusOver.ToString() == "-4")
        {
            return "<span class=\"label label-danger\">AccLocked</span>";
        }
        if (statusOver.ToString() == "-124" || statusOver.ToString() == "-123")
        {
            return "<span class=\"label label-warning\">OTPOver</span>";
        }

        return "N/A";
    }
}