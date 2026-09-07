using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;

public partial class Pages_PayGate_Payments_Log : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.PaymentsLog);
        if (!IsPostBack)
        {
            BindData();
        }
    }

    private void BindData()
    {
        PaymentsLog _Log = new PaymentsLog() { ServiceID = Convert.ToInt32(AppUtils.Request("id")) };
        rptList.DataSource = _Log.ServiceID > 0 ? _Log.GetTable(_Log.ServiceID) : _Log.GetTable();
        rptList.DataBind();
    }
}