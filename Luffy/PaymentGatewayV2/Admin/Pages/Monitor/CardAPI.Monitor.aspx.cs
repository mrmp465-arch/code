using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.Report;
using Libs.API;
using Libs.Utils;
using System.Globalization;

public partial class Pages_Monitor_CardAPI_Monitor : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.CardAPIMonitor);

        if (!IsPostBack)
        {
            init();
            GetList();
        }

    }

    private void init()
    {
        txtCreatTime.Text = DateTime.Now.AddDays(1).ToString();

        var lst = new List<Partners>();
        if (AppUtils.IsAdmin)
            lst = new Partners().GetList();
        else
            lst = new Partners().GetListByUserId(AppUtils.UserID);
        lst = lst.OrderBy(x => x.PartnerCode).ToList();
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "PartnerCode";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Đối tác:", ""));

        var lstProvider = new List<Providers>();
        if (AppUtils.IsAdmin)
            lstProvider = new Providers().GetList(7);
        else
            lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x=>x.Type==7).ToList();
        lstProvider = lstProvider.OrderBy(x => x.ProviderCode).ToList();
        drpProvider.DataSource = lstProvider;
        drpProvider.DataTextField = "Name";
        drpProvider.DataValueField = "ProviderCode";
        drpProvider.DataBind();
        drpProvider.Items.Insert(0, new ListItem("Nhà cung cấp:", ""));
        
        drpCardType.DataSource = new Products().GetList(7, 1);
        drpCardType.DataTextField = "Name";
        drpCardType.DataValueField = "Code";
        drpCardType.DataBind();
        drpCardType.Items.Insert(0, new ListItem("Loại thẻ:", ""));


    }

    private void GetList()
    {
        //NLogLogger.Info(new string[] { "IsProvider", AppUtils.IsProvider.ToString(), "IsPartner", AppUtils.IsPartner.ToString() });

        bool erro = false;
        int? status = null;
        if (txtStatus.Text.ToLower() != "all")
            status = AppUtils.ToInt32(txtStatus.Text, out erro);
        if (erro)
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfos').html('Chưa nhập Status'); $('#AlertInfo').modal()}); ", true);
            return;
        }

        int top = Convert.ToInt32(drpTop.SelectedValue);
        DateTime creatTime = AppUtils.ToDateTime(txtCreatTime.Text);


        string cardType = drpCardType.SelectedValue;


        string partnerCodes = drpPartner.SelectedValue;
        string providerCodes = drpProvider.SelectedValue;


        if (AppUtils.IsPartner && !AppUtils.IsAdmin)
        {

            if (string.IsNullOrEmpty(partnerCodes))
            {
                var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
                if (lstPartner != null && lstPartner.Count > 0)
                {
                    partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode).ToArray());
                }
            }
            

        }

        if (AppUtils.IsProvider && !AppUtils.IsAdmin)
        {
            if (string.IsNullOrEmpty(providerCodes))
            {
                var lstProvider = new Providers().GetListByUserId(AppUtils.UserID).Where(x => x.Type == 7).ToList(); ;
                if (lstProvider != null && lstProvider.Count > 0)
                {
                    providerCodes = string.Join(",", lstProvider.Select(e => e.ProviderCode).ToArray());
                }
            }

        }
        
        CardAPILog _CardAPILog = new CardAPILog();
        rptList.DataSource = _CardAPILog.GetTable(top, partnerCodes, creatTime, status, cardType, providerCodes);
        rptList.DataBind();
    }

    protected void btView_Click(object sender, EventArgs e)
    {
        GetList();
    }

    protected string DetailUrl(string id)
    {
        return Constant.ADMIN_PATH + Resources.Url.CardAPIMonitorDetail + "?id=" + id;
    }
    protected int Datediff(string LastTime, string CreatTime)
    {
        
        return (DateTime.ParseExact(LastTime, "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture) - DateTime.ParseExact(CreatTime, "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture)).Seconds;
    }
}