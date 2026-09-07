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

public partial class Pages_Momo_PartnerMomo : System.Web.UI.Page
{
    private const string urlBaseService = "http://127.0.0.1:9001/MomoService.ashx";
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.MomoPartner);

        if (!IsPostBack)
        {
            init();
            BindData();
        }
    }

    private void init()
    {
        var lst = new PartnerMomo().GetListPartner();
        lst = lst.OrderBy(x => x.Code).ToList();
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "Code";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Kênh:", ""));
        drpPartner.SelectedValue = "ch01";
    }

    protected void btAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PartnerMomoAdd);
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
        if (!string.IsNullOrEmpty(drpPartner.SelectedValue))
        {
            var requestContent = "";
            //var signature = Encrypts.MD5(drpPartner.SelectedValue + "GETLIST" + requestContent + Partner.PublicKey);
            var requestData = new RequestData()
            {
                PartnerCode = drpPartner.SelectedValue,
                CommandCode = "GETLIST",
                RequestContent = requestContent,
                Signature = ""
            };



            var response = Task.Run(async () => await PostTask("http://127.0.0.1:9002/MomoService.ashx", serializer.Serialize(requestData))).Result;
            if (!string.IsNullOrEmpty(response))
            {
                var resObj = serializer.Deserialize<BankResponse>(response);
                if (resObj.ResponseCode > 0)
                {
                    var bank = serializer.Deserialize<Bank>(resObj.ResponseContent);

                    lblAcountInfo.Visible = true;
                    lblAcountInfo.Text = String.Format("Ví hiển thị: {0}-{1}", bank.MomoId, bank.MomoName);
                }
                else
                {
                    lblAcountInfo.Visible = true;
                    lblAcountInfo.Text = "Không có ví nào ";

                }

            }
        }
    }
    private void BindData()
    {
        var data = new PartnerMomo().GetList();
        if (!string.IsNullOrEmpty(drpPartner.SelectedValue))
            data = data.Where(x => x.Code == drpPartner.SelectedValue).ToList();

        if (!string.IsNullOrEmpty(txtMomoId.Text))
            data = data.Where(x => x.MomoMobile.Contains(txtMomoId.Text)).ToList();

        // JavaScriptSerializer serializer = new JavaScriptSerializer();
        //NLogLogger.Info(new string[] { "Data", "PartnerMomoo", serializer.Serialize(data) });
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



            //NLogLogger.Info(new string[] { "Data", "lbPId", lbPId.Text });
            var _PartnerMomo = new PartnerMomo();
            _PartnerMomo.Id = Convert.ToInt32(lbPId.Text);
            _PartnerMomo.Status = cbx.Checked ? 1 : 0;
            _PartnerMomo.OrderNo = Convert.ToInt32(tbx.Text);

            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //NLogLogger.Info(new string[] { "Data", "lbPId", serializer.Serialize(_PartnerMomo) });
            _PartnerMomo.Update();
        }
        var requestData = new RequestData()
        {
            PartnerCode = "",
            CommandCode = "ACCOUNT_NOTIFY",
            RequestContent = "",
            Signature = ""
        };
        NLogLogger.Info(new string[] { "PartnerMomo", "Update", AppUtils.UserName, drpPartner.SelectedValue });
        Task.Run(async () => await CallbackJson(urlBaseService, serializer.Serialize(requestData)).ConfigureAwait(false));
        BindData();
    }

    protected void Delete_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        int Id = Convert.ToInt32(e.CommandArgument.ToString());

        var order = new PartnerMomo { Id = Id };
        order.Delete();
        BindData();
        var requestData = new RequestData()
        {
            PartnerCode = "",
            CommandCode = "ACCOUNT_NOTIFY",
            RequestContent = "",
            Signature = ""
        };

        var response = Task.Run(async () => await CallbackJson(urlBaseService, serializer.Serialize(requestData)).ConfigureAwait(false));


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
        if (statusOver.ToString() == "-3")
        {
            return "<span class=\"label label-danger\">LoginFailed</span>";
        }
        if (statusOver.ToString() == "-4")
        {
            return "<span class=\"label label-danger\">AccLocked</span>";
        }
        if (statusOver.ToString() == "-124" || statusOver.ToString() == "-123")
        {
            return "<span class=\"label label-warning\">OTPOver</span>";
        }

        return "N/A";
    }

    public static async Task<string> CallbackJson(string url, string postData)
    {

        //NLogLogger.Info(new string[] { "MDrum", "Callback", "Partner", "Request", postData });
        var uri = new Uri(url);
        var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(60);
        try
        {
            NLogLogger.Info(new string[] { "CMS", "R", "Request", postData });
            var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                NLogLogger.Info(new string[] { "CMS", "R", "Response", responseContent });
                client.Dispose();
                return responseContent;
            }
            else
            {
                NLogLogger.Info(new string[] { "CMS", "Callback", "Partner", "Response Is Null" });
            }

        }
        catch (Exception e)
        {
            //var responseStream = e.Response.GetResponseStream();

            //if (responseStream != null)
            //{
            //    using (var reader = new StreamReader(responseStream))
            //    {
            //        NLogLogger.Info(new string[] { "CMS", "Exeption Post", reader.ReadToEnd() }); 
            //        
            //    }
            //}
            NLogLogger.Info(new string[] { "CMS", "Exeption Post", e.Message });
            return string.Empty;
        }
        client.Dispose();
        return string.Empty;
    }
    public class RequestData
    {
        public string PartnerCode { get; set; }
        public string ServiceCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public string Signature { get; set; }
    }
    public class BankResponse
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string ResponseContent { get; set; }
        public string Signature { get; set; }
    }
    public class Bank
    {
        public string MomoId { get; set; }
        public string MomoName { get; set; }
    }
    protected void drpPartner_SelectedIndexChanged1(object sender, EventArgs e)
    {
        BindData();
    }
    public static async Task<string> PostTask(string url, string postData)
    {

        var uri = new Uri(url);
        var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(60);
        try
        {
            var response = await client.PostAsync(uri, httpContent);
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                client.Dispose();
                return responseContent;
            }

        }
        catch (Exception e)
        {
            NLogLogger.Info(new string[] { "MDrum", "Exeption Post", e.Message });
            return string.Empty;
        }
        client.Dispose();
        return string.Empty;
    }

}