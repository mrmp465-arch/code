using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;

public partial class Pages_SystemConfig_SystemConfig : System.Web.UI.Page
{
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.PartnersEdit);
        if (!IsPostBack)
        {
            init();
        }
    }

    private void init()
    {

        var _SystemConfig = new SystemConfig();

        rptList.DataSource = new SystemConfig().GetList();
        rptList.DataBind();

        List<SystemConfig> list = new SystemConfig().GetList();
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
            TextBox txtDescription = (TextBox)rptList.Items[i].FindControl("Description");
            Label lblServiceID = (Label)rptList.Items[i].FindControl("lblServiceID");
            Label LblServiceCode = (Label)rptList.Items[i].FindControl("LblServiceCode");
            Label lblListFeature = (Label)rptList.Items[i].FindControl("lblListFeature");
            int serviceID = Convert.ToInt32(lblServiceID.Text);
            string serviceCode = LblServiceCode.Text;
            for (int j = 0; j < list.Count; j++)
            {
                _SystemConfig = list[j];
                if (serviceID == _SystemConfig.Id)
                {
                    cbx.Checked = _SystemConfig.Status == 1;
                    break;
                }
            }

            Repeater rptListPro = rptList.Items[i].FindControl("rptProList") as Repeater;
            
            var listFeature = serializer.Deserialize<List<WServiceAutoBuyCardService>>(lblListFeature.Text);
            

            for (int k = 0; k < rptListPro.Items.Count; k++)
            {
                CheckBox cbxProStatus = (CheckBox)rptListPro.Items[k].FindControl("cbxProStatus");
                HiddenField txtProId = (HiddenField)rptListPro.Items[k].FindControl("txtProId");
                cbxProStatus.Checked = listFeature[k].Status == 1 ? true : false;

            }

        }

    }



    protected List<WServiceAutoBuyCardService> listFeature(object serviceId)
    {
        var service = new SystemConfig();
        service.Id = Convert.ToInt32(serviceId);
        return serializer.Deserialize<List<WServiceAutoBuyCardService>>(service.Get().Feature); ;
    }

    protected void btApply_Click(object sender, EventArgs e)
    {

        for (int i = 0; i < rptList.Items.Count; i++)
        {
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
            Label lblServiceId = (Label)rptList.Items[i].FindControl("lblServiceID");
            Label lblListFeature = (Label)rptList.Items[i].FindControl("lblListFeature");

            var service = new SystemConfig();
            service.Id = Convert.ToInt32(lblServiceId.Text);
            service.Status = cbx.Checked == true ? 1 : 0;

            var listFeature = serializer.Deserialize<List<WServiceAutoBuyCardService>>(lblListFeature.Text);

            Repeater rptListPro = rptList.Items[i].FindControl("rptProList") as Repeater;
            for (int j = 0; j < rptListPro.Items.Count; j++)
            {
                CheckBox cbxProStatus = (CheckBox)rptListPro.Items[j].FindControl("cbxProStatus");
                HiddenField txtProId = (HiddenField)rptListPro.Items[j].FindControl("txtProId");
                listFeature[j].Status = cbxProStatus.Checked == true ? 1 : 0;
            }

            service.Feature = serializer.Serialize(listFeature);
            service.Update();
        }



    }
}