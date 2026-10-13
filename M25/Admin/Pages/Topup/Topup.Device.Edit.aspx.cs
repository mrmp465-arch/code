using System;
using Libs.API;
using Libs.Utils;

public partial class Pages_PayGate_Topup_Device_Edit : System.Web.UI.Page
{
    public int Id { set; get; }
    public string Slot { set; get; }
    public string DeviceName { set; get; }
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.TopupSimEdit);
        if (!IsPostBack)
        {
            init();
            //      txtType.SelectedValue = Type = Request["type"];
        }
    }


    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.TopupDevice);
    }

    protected void btUpdate_Click(object sender, EventArgs e)
    {
        var Id = Convert.ToInt32(AppUtils.Request("id"));

        var sim = new SimUSSD()
        {
            Id = Id,
            Amount = GlobalHelper.TryParseNullable(txtAmout.Text),
            DeviceId = GlobalHelper.TryParseNullable(ddlDevice.SelectedValue),
            Quota = GlobalHelper.TryParseNullable(txtQouta.Text),
            Sim = txtSim.Text,
            Slot = GlobalHelper.TryParseNullable(ddlSlot.SelectedValue),
            Telco = ddlTelco.SelectedValue,
            Status = GlobalHelper.TryParseNullable(ddlStatus.SelectedValue)
        };
        var res = sim.Update();

        if (res == 0)
            Response.Redirect(Constant.ADMIN_PATH + Resources.Url.TopupDevice);
    }

    private void init()
    {

        if (!AppUtils.IsAdmin)
            txtAmout.Enabled = false;

        //Init Device
        ddlDevice.DataSource = new DeviceUSSD().GetList();
        ddlDevice.DataTextField = "Name";
        ddlDevice.DataValueField = "Id";
        ddlDevice.DataBind();

        Id = Convert.ToInt32(AppUtils.Request("id"));
        var slot = new SimUSSD();
        slot.Id = Id;
        var sim = slot.Get();
        txtSim.Text = sim.Sim;
        ddlSlot.SelectedValue = sim.Slot.ToString();
        ddlTelco.SelectedValue = sim.Telco;
        ddlDevice.SelectedValue = sim.DeviceId.ToString();
        txtAmout.Text = sim.Amount.ToString();
        txtQouta.Text = sim.Quota.ToString();
        ddlStatus.SelectedValue = sim.Status.ToString();

        Slot = sim.Slot.ToString();
        DeviceName = ddlDevice.SelectedItem.Text;

        if (!AppUtils.IsAdmin)
        {
            ddlDevice.Enabled = false;
            txtSim.Enabled = false;
            //ddlSlot.Enabled = false;
            //ddlTelco.Enabled = false;

        }
    }
}