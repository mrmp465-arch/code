using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_Monitor_BankGateAPI_Account : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankGateAPIFixMomo);
        if (!IsPostBack)
        {

            GetList();
        }
    }
    private void GetList()
    {
        //var cachedata = DataCaching.GetCache("BankInfoKZ");
        var lstBank = DataCaching.GetCache<List<BankAccountV3>>("BankInfoKZ");
        lstBank = lstBank.OrderBy(x => x.BankCode).ToList();
        rptList.DataSource = lstBank;
        rptList.DataBind();

    }
    public class BankAccountV3
    {

        public string AccountName { get; set; }

        public string AccountId { get; set; }
        public string BankCode { get; set; }
    }

}