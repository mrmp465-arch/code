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
using System.Reflection;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;

public partial class Pages_Momo_Accounts : System.Web.UI.Page
{
    protected long total;
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.MomoAccount);
        if (!IsPostBack)
        {
            init();
            BindData();
        }
    }
    protected void BindData()
    {
        var _Momo = new MomoAccounts();
        var data = _Momo.GetList().OrderBy(x => x.Id).ToList();

        if(!AppUtils.UserName.Contains("admin"))
        {
            data = data.Where(x => x.Source == AppUtils.UserName).ToList();
        }    


        var status = int.Parse(drpStatus.SelectedValue);
        //if (status == -2)
        //    data = data;
        //chưa kích hoạt
        if (status == 0)
            data = data.Where(x => x.Status == 0).ToList();
        //kích hoạt
        if (status == 1)
            data = data.Where(x => x.Status == 1).ToList();
        //bỏ qua
        if (status == -1)
            data = data.Where(x => x.Status == -1).ToList();
        //sẵn sàng
        if (status == 2)
            data = data.Where(x => x.Status == 1 || x.Status == 0).ToList();


        var statusExtra = int.Parse(drpStatusExtra.SelectedValue);
        if (statusExtra == -999)
        {
            data = data;
        }
        else
        {
            if (statusExtra == -16)
            {
                data = data.Where(x => x.StatusExtra == -16 || x.StatusExtra == -123 || x.StatusExtra == -124).ToList();
            }
            else
            {
                if (statusExtra == 4)
                {
                    data = data.Where(x => x.StatusExtra != -4).ToList();
                }
                else
                {
                    data = data.Where(x => x.StatusExtra == statusExtra).ToList();
                }

            }
        }






        var statusDetech = int.Parse(drpDetect.SelectedValue);
        if (statusDetech > -2)
            data = data.Where(x => x.StatusDetection == statusDetech).ToList();

        if (!string.IsNullOrEmpty(drpSolution.SelectedValue))
        {
            data = data.Where(x => x.Solution == drpSolution.SelectedValue).ToList();
        }

        //var lstdata = new MomoTransaction().Report(DateTime.Now.Year, DateTime.Now.Month, 0);
        total = (long)data.Where(x => x.Status >= 0).Sum(x => (long)x.BalanceMonthIn) / 1000000;

        lblTotal.Text = String.Format("Tổng số ví : {0} - tổng số dư : {1}- Sản lượng: {3}/ {2}", data.Count().ToString(), data.Sum(x => Convert.ToInt64(x.BalanceTotal)).ToString("N0"), (data.Where(x => x.Status >= 0).Sum(x => x.BalanceMaxMonth)).ToString("N0"), total.ToString("N0"));

        var name = txtName.Text.Trim();
        if (!string.IsNullOrEmpty(name))
            data = data.Where(x => x.MomoName.ToLower().Contains(name.ToLower()) || x.MomoId.Contains(name)).ToList();

        //var mobile = txtMobile.Text.Trim();
        //if (!string.IsNullOrEmpty(mobile))
        //    data = data.Where(x => x.MomoId.Contains(mobile)).ToList();


        var type = drpType.SelectedValue;
        if (!string.IsNullOrEmpty(type))
            data = data.Where(x => x.Type.Equals(type)).ToList();




        //JavaScriptSerializer serializer = new JavaScriptSerializer();
        // NLogLogger.Info(new string[] { "Data", "Callback", "data NULL", serializer.Serialize(data) });
        if (!string.IsNullOrEmpty(drpPartner.SelectedValue))
            //data = data.Where(x => x.Source == drpPartner.SelectedValue).ToList();
            //data = data.Where(x => x.Source == drpPartner.SelectedValue).ToList();
            data = data.Where(x => x.PartnerName == drpPartner.SelectedValue).ToList();

        int page = int.Parse(ddlPage.SelectedValue);
        if (page > 0)
            data = data.Skip((page - 1) * 200).Take(200).ToList();

        rptList.DataSource = data;
        rptList.DataBind();
    }

    private void init()
    {

        //var lst = new List<PartnerMomo>();
        //lst.Add(new PartnerMomo {Name="order" });
        //lst.Add(new PartnerMomo { Name = "cn001" });
        //drpPartner.DataSource = lst;
        //drpPartner.DataTextField = "Name";
        //drpPartner.DataValueField = "Name";
        //drpPartner.DataBind();
        //drpPartner.Items.Insert(0, new ListItem("Source:", ""));

        var lst = new PartnerMomo().GetListPartner();
        lst = lst.OrderBy(x => x.Code).ToList();
        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "Name";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem("Partner:", ""));

    }
    protected void btAppClick(object sender, EventArgs e)

    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        var UrlBaseService = "http://127.0.0.1:9002/MomoService.ashx";
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            var _Partner = new Partners();
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus2");
            Label TransactionID = (Label)rptList.Items[i].FindControl("ID");
            if (cbx.Checked)
            {
                string MomoId = TransactionID.Text;
                var requesData = new RequestData()
                {
                    PartnerCode = string.Empty,
                    CommandCode = "TRANSFER",
                    RequestContent = serializer.Serialize(new Transfer() { TransId = "", PartnerMomoId = MomoId, MomoId = txtUser.Text.Trim(), MomoName = txtUser.Text, Amount = 100, Note = "test123" }),
                };

              
                var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;
                NLogLogger.Info(new string[] { "CMS", "Account", "tranfer", serializer.Serialize(requesData), res });
            }
        }
        Response.Redirect("/cmspay/pages/momo/transaction.aspx");
    }
    protected void drpPartner_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindData();
    }
    protected void btAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.MomoAccountAdd);
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
    }

    protected void cbxStatus_CheckedChanged(object sender, EventArgs e)
    {
        if (sender != null)
        {
            var id = int.Parse(((CheckBox)sender).ToolTip);
            var _Momo = new MomoAccounts();
            _Momo = _Momo.Get(id);
            if (_Momo != null)
            {
                if (_Momo.Status == 1)
                {
                    _Momo.Status = 0;
                }
                else
                {
                    _Momo.Status = 1;
                }
                //NLogLogger.Info(new string[] { "Momo", "UpdateStatus", AppUtils.UserName, id.ToString(), _Momo.MomoId });
                var _userLog = new UserLog
                {
                    UserName = AppUtils.UserName,
                    Action = "momoupdate",
                    ActionName = "Cập nhật momo",
                    Description = "Cập nhật trạng thái momo " + _Momo.MomoId + " |" + _Momo.Status.ToString()
                };
                _userLog.Add();
                _Momo.Update();
            }


        }
    }
    protected void UpdateTime_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        int Id = Convert.ToInt32(e.CommandArgument.ToString());
        var _Bank = new MomoAccounts();

        _Bank.StopScanAt_Update(Id, DateTime.Now.AddMinutes(10));

        _Bank.Id = Id;
        _Bank = _Bank.Get();
        if (_Bank.Solution == "APIV3")
        {
            var url = "http://127.0.0.1:9002/MomoService.ashx";

            var requesData = new
            {
                CommandCode = "TRANS_SCAN",
                RequestContent = _Bank.MomoId,
            };
            var res = Task.Run(async () => await CallbackJson(url, serializer.Serialize(requesData))).Result;
        }

        BindData();
    }
    public string GetSolutionStyle(object statusOver)
    {
        if (statusOver.ToString() == "1")
        {
            return "font-weight:bold";
        }



        return "";
    }
    public string GetDate(object createDate)
    {
        if(createDate!=null)
        {
            DateTime dt = Convert.ToDateTime(createDate);
            return dt.ToString("dd/MM/yyyy");
        }    



        return "";
    }
    protected void btApp2Click(object sender, EventArgs e)

    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        for (int i = 0; i < rptList.Items.Count; i++)
        {
            var _Partner = new Partners();
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus2");
            Label TransactionID = (Label)rptList.Items[i].FindControl("ID");
            if (cbx.Checked)
            {
                string MomoId = TransactionID.Text;
                var _Bank = new MomoAccounts();
                _Bank = _Bank.Get(MomoId);
                if (_Bank.Status == 0)
                {
                    _Bank.Status = 1;
                    _Bank.Update();
                    NotifyMomo(_Bank.MomoId, _Bank.Status);
                }


            }
        }
        Response.Redirect("/cmspay/pages/momo/accounts.aspx");
    }
    public void NotifyMomo(string momoid, int status)
    {
        var UrlBaseService = "http://127.0.0.1:9002/MomoService.ashx";
        var obj = new { momoId = momoid, action = "start" };
        if (status != 1)
            obj = new { momoId = momoid, action = "stop" };
        var requesData = new RequestData()
        {
            CommandCode = "NOTIFY_ACTION",
            RequestContent = serializer.Serialize(obj),
        };
        var res = Task.Run(async () => await CallbackJson(UrlBaseService, serializer.Serialize(requesData))).Result;
    }
    public static async Task<string> CallbackJson(string url, string postData)
    {
        var uri = new Uri(url);
        var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(30);
        try
        {
            NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Request", postData });
            var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Response", responseContent });
                client.Dispose();
                return responseContent;
            }
            else
            {
                NLogLogger.Info(new string[] { "CMS", "Account.Edit", "Response Is Null" });
            }

        }
        catch (Exception e)
        {
            NLogLogger.Info(new string[] { "CMS", "Exeption Post", e.Message });
            return string.Empty;
        }
        client.Dispose();
        return string.Empty;
    }
    public string GetStatusActive(object statusOver)
    {
        if (statusOver.ToString() == "1")
        {
            return "<span class=\"label label-success\">Kích hoạt</span>";
        }

        if (statusOver.ToString() == "-1")
        {
            return "<span class=\"label label-default\">Bỏ qua</span>";
        }

        if (statusOver.ToString() == "0")
        {
            return "<span class=\"label label-warning\">Chưa kích hoạt</span>";
        }

        return statusOver.ToString();
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
            return "<span class=\"label label-info\">OverMin</span>";
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
        if (statusOver.ToString() == "-5" || statusOver.ToString() == "-123")
        {
            return "<span class=\"label label-warning\">OTPOver</span>";
        }

        if (statusOver.ToString() == "-6")
        {
            return "<span class=\"label label-warning\">FaceOver</span>";
        }
        if (statusOver.ToString() == "-7")
        {
            return "<span class=\"label label-danger\">FaceNotMatched</span>";
        }
        if (statusOver.ToString() == "-8")
        {
            return "<span class=\"label label-danger\">MissingKYC</span>";
        }
        if (statusOver.ToString() == "-9")
        {
            return "<span class=\"label label-warning\">CaptchaRequired</span>";
        }
        return "N/A (" + statusOver.ToString() + ")";
    }
    public class Transfer
    {
        public string TransId { get; set; } //Transaction cua he thông Pay
        public string PartnerMomoId { get; set; }
        public string MomoId { get; set; } // NG nhan
        public string MomoName { get; set; }
        public int Amount { get; set; }
        public string Note { get; set; }

    }

    public class RequestData
    {
        public string PartnerCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public string Signature { get; set; }
    }
}