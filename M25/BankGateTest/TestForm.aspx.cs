using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Web.Administration;

namespace BankGateTest
{
    public partial class TestForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnPoll_Click(object sender, EventArgs e)
        {
            ServerManager serverManager = new ServerManager();
            ApplicationPool appPool = serverManager.ApplicationPools["APIMyViettel"];
            if (appPool != null)
            {
                if (appPool.State == ObjectState.Started)
                {
                    appPool.Stop();
                }
                else if (appPool.State == ObjectState.Stopped)
                {
                    appPool.Start();
                }
            }
            serverManager.CommitChanges();
        }
    }
}