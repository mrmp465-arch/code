using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Libs.API;

public partial class Pages_Topup_Topup_Device : System.Web.UI.Page
{


    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.TopupDevice);
        if (!IsPostBack)
        {
            init();
        }
    }
    private void init()
    {
        var _Providers = new Providers();
        drpProviders.DataSource = _Providers.GetList(7);
        drpProviders.DataBind();
        drpProviders.DataTextField = "Name";
        drpProviders.DataValueField = "ProviderCode";
        drpProviders.DataBind();
        drpProviders.Items.Insert(0, new ListItem("Provider:", ""));

        var providerCodes = string.Empty;
        var lst = new List<Providers>();

        if (!AppUtils.IsAdmin)
        {
            lst = new Providers().GetListByUserId(AppUtils.UserID);
            if (lst != null && lst.Count > 0)
                providerCodes = string.Join(",", lst.Select(e => e.ProviderCode).ToArray());
            else
                providerCodes = "Empty";
        }

        GetList(providerCodes);
    }
    private void GetList(string providerCodes)
    {
        var _DeviceUSSD = new DeviceUSSD();
        rptList.DataSource = _DeviceUSSD.GetListByProvides(providerCodes);
        rptList.DataBind();
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        var providerCodes = drpProviders.SelectedValue;
        GetList(providerCodes);
    }

    public string GetStatus(int status)
    {
        if (status == 1)
            return "Actived";
        else if (status == 0)
            return "Disabled";
        else
            return status.ToString();
    }

    public string GetStatusSim(int status)
    {
        if (status == 1)
            return "Actived";
        else if (status == 0)
            return "Disabled";
        else if (status == -2)
            return "Lock";
        else if (status == -1)
            return "Over quota";
        else
            return status.ToString();
    }
    protected List<SimUSSD> ListSim(object deviceId)
    {
        var sim = new SimUSSD();
        sim.DeviceId = Convert.ToInt32(deviceId);
        return sim.GetList();
    }

    protected void LinkResetAmount_Click(object sender, EventArgs e)
    {
        string id = (sender as LinkButton).CommandArgument;
        var sim = new SimUSSD();
        sim.Id = Convert.ToInt32(id);
        sim.Amount = 0;
        sim.Update();
        init();
    }
}