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
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;

public partial class Pages_BankEWalletService_Bank_PartnerBank : System.Web.UI.Page
{
    private const string urlBaseService = "http://127.0.0.1:9001/BankService.ashx";
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankPartner);

        if (!IsPostBack)
        {
            init();
            BindData();
        }
    }

    private void init()
    {
        var lst = new PartnerBank().GetListPartner();
        lst = lst.OrderBy(x => x.Code).ToList();
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "Code";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Chọn kênh:", ""));
        drpPartner.SelectedValue = "ch01";
    }

    protected void btAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PartnerBankAdd);
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
    }
    private void BindData()
    {
        var data = new PartnerBank().GetList();
        if (!string.IsNullOrEmpty(drpPartner.SelectedValue))
            data = data.Where(x => x.Code == drpPartner.SelectedValue).ToList();

        if (!string.IsNullOrEmpty(txtBankId.Text))
            data = data.Where(x => x.BankId.Contains(txtBankId.Text)).ToList();

        // JavaScriptSerializer serializer = new JavaScriptSerializer();
        //NLogLogger.Info(new string[] { "Data", "PartnerBanko", serializer.Serialize(data) });
        rptList.DataSource = data;
        rptList.DataBind();


    }
   
    protected void btApply_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus");
            TextBox tbx = (TextBox)rptList.Items[i].FindControl("txtOrderNo");
            Label lbPId = (Label)rptList.Items[i].FindControl("lblId");



            //NLogLogger.Info(new string[] { "Data", "lbPId", lbPId.Text,"order", tbx.Text });
            var _PartnerBank = new PartnerBank();
            _PartnerBank.Id = Convert.ToInt32(lbPId.Text);
            _PartnerBank.Status = cbx.Checked ? 1 : 0;
            _PartnerBank.OrderNo = Convert.ToInt32(tbx.Text);

            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //NLogLogger.Info(new string[] { "Data", "lbPId", serializer.Serialize(_PartnerBank) });
            _PartnerBank.Update();
        }

        BindData();

        //var requestData = new RequestData()
        //{
        //    PartnerCode = "",
        //    CommandCode = "ACCOUNT_NOTIFY",
        //    RequestContent = "",
        //    Signature = ""
        //};

        //Task.Run(async () => await CallbackJson(urlBaseService, serializer.Serialize(requestData)).ConfigureAwait(false));

    }

    protected void Delete_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        int Id = Convert.ToInt32(e.CommandArgument.ToString());

        var order = new PartnerBank { Id = Id };
        order.Delete();
        BindData();
       
        //var requestData = new RequestData()
        //{
        //    PartnerCode = "",
        //    CommandCode = "ACCOUNT_NOTIFY",
        //    RequestContent = "",
        //    Signature = ""
        //};

        //Task.Run(async () => await CallbackJson(urlBaseService, serializer.Serialize(requestData)).ConfigureAwait(false));
    }
    protected void drpPartner_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindData();
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
            return "<span class=\"label label-info\">UnderMin</span>";
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
        if (statusOver.ToString() == "-124" || statusOver.ToString() == "-123")
        {
            return "<span class=\"label label-warning\">OTPOver</span>";
        }

        return "N/A";
    }

    //public static async Task<string> CallbackJson(string url, string postData)
    //{

    //    //NLogLogger.Info(new string[] { "MDrum", "Callback", "Partner", "Request", postData });
    //    var uri = new Uri(url);
    //    var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
    //    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    //    var client = new HttpClient();
    //    client.Timeout = TimeSpan.FromSeconds(60);
    //    try
    //    {
    //        NLogLogger.Info(new string[] { "CMS", "R", "Request", postData });
    //        var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
    //        if (response.Content != null)
    //        {
    //            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    //            NLogLogger.Info(new string[] { "CMS", "R",  "Response", responseContent });
    //            client.Dispose();
    //            return responseContent;
    //        }
    //        else
    //        {
    //            NLogLogger.Info(new string[] { "CMS", "Callback", "Partner", "Response Is Null" });
    //        }

    //    }
    //    catch (Exception e)
    //    {
    //        //var responseStream = e.Response.GetResponseStream();

    //        //if (responseStream != null)
    //        //{
    //        //    using (var reader = new StreamReader(responseStream))
    //        //    {
    //        //        NLogLogger.Info(new string[] { "CMS", "Exeption Post", reader.ReadToEnd() }); 
    //        //        
    //        //    }
    //        //}
    //        NLogLogger.Info(new string[] { "CMS", "Exeption Post", e.Message });
    //        return string.Empty;
    //    }
    //    client.Dispose();
    //    return string.Empty;
    //}
    //public class RequestData
    //{
    //    public string PartnerCode { get; set; }
    //    public string ServiceCode { get; set; }
    //    public string CommandCode { get; set; }
    //    public string RequestContent { get; set; }
    //    public string Signature { get; set; }
    //}

    protected void drpPartner_SelectedIndexChanged1(object sender, EventArgs e)
    {
        BindData();
    }
}