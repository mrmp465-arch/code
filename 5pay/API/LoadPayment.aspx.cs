using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Microsoft.Web.Administration;

public partial class LoadPayment : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ServerManager serverManager = new ServerManager();
        ApplicationPool appPool = serverManager.ApplicationPools["paymentapi"];
        if (appPool != null)
        {
            if (appPool.State == ObjectState.Stopped)
            {
                appPool.Start();
            }
            else
            {
                appPool.Recycle();
            }
        }

        //Application.Lock();
        //Application["ServiceList"] = new Payments().GetList();
        //Application.UnLock();

    }
}