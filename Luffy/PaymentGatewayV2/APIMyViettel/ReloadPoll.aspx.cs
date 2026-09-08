using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Web.Administration;

namespace APIMyViettel
{
    public partial class ReloadPoll : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ServerManager serverManager = new ServerManager();
            ApplicationPool appPool = serverManager.ApplicationPools["APIMyViettel"];
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
            serverManager.CommitChanges();
        }
    }
}