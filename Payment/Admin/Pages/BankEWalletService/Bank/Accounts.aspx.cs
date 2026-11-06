using Libs.API;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_BankEWalletService_Bank_Accounts : System.Web.UI.Page
{
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankAccount);
        if (!IsPostBack)
        {
            var _Bank = new BankAccounts();
            var data = _Bank.GetList().OrderBy(x => x.Type).ToList();

            var lstComputer = data.GroupBy(x => x.Computer).Where(b => b.Key != "");
            drpComputer.DataSource = lstComputer;
            drpComputer.DataTextField = "Key";
            drpComputer.DataValueField = "Key";
            drpComputer.DataBind();
            drpComputer.Items.Insert(0, new ListItem("Chọn PC:", ""));

            if (HttpContext.Current.Session["Amount"] != null)
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["Amount"].ToString()))
                {
                    txtAmount.Text = HttpContext.Current.Session["Amount"].ToString();
                }
            }
            BindData();

        }
        if (IsPostBack)
        {
            string eventTarget = Request["__EVENTTARGET"];
            string eventArgument = Request["__EVENTARGUMENT"];

            if (eventTarget == "UpdateAll")
            {


                btUpdateAllClick();
            }

        }
    }
    protected void BindData()
    {
        var _Bank = new BankAccounts();
        var data = _Bank.GetList().OrderBy(x => x.Type).ToList();

        long Balance = data.Where(x => x.Type.Contains("OUT") && x.Status == 1 && x.StatusExtra != -4).Sum(a => a.BalanceTotal);
        if (Balance > 30000000)
        {
            lblTotal.Text = "Số dư bank out: " + Balance.ToString("#,#").Replace(",", ".");
            lblTotal.CssClass = "";
        }
        else
        {
            lblTotal.Text = "Số dư bank out: " + Balance.ToString("#,#").Replace(",", ".");
            lblTotal.CssClass = "bwarning";
        }

        var bank = drpBank.SelectedValue;
        if (!string.IsNullOrEmpty(bank))
            data = data.Where(x => x.BankCode.Equals(bank)).ToList();

        var banktype = drpBankType.SelectedValue;
        if (!string.IsNullOrEmpty(banktype))
            data = data.Where(x => x.BankType.Equals(banktype)).ToList();


        lblTota2.Text = String.Format("Tổng số bank : {0} - tổng số dư : {1}", data.Where(x => x.Status == 1).Count().ToString(), data.Where(x => x.Type == "IN" || x.Type == "INOUT").Where(a => a.StatusExtra == 1).Sum(x => Convert.ToInt64(x.BalanceTotal)).ToString("N0"));



        var name = txtName.Text.Trim();
        if (!string.IsNullOrEmpty(name))
            data = data.Where(x => x.BankName.ToLower().Contains(name.ToLower())).ToList();

        var mobile = txtMobile.Text.Trim();
        if (!string.IsNullOrEmpty(mobile))
            data = data.Where(x => x.BankId.Contains(mobile)).ToList();


        var type = drpType.SelectedValue;
        if (!string.IsNullOrEmpty(type))
        {
            if (type == "OUTALL")
            {
                data = data.Where(x => x.Type.Contains("OUT")).ToList();
            }
            else
            {
                data = data.Where(x => x.Type.Equals(type)).ToList();
            }
            //data = data.Where(x => x.Type.Equals(type)).ToList();
        }




        var statusExtra = int.Parse(drpStatusExtra.SelectedValue);
        //if (statusExtra == 0)
        //    data = data;
        if (statusExtra == -1)
            data = data.Where(x => x.StatusExtra != 1).ToList();
        if (statusExtra == 1)
            data = data.Where(x => x.StatusExtra == 1).ToList();

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

        //if(txtPC.Text=="PC" || txtPC.Text == "")
        // {

        // }
        // else
        // {
        //     data = data.Where(x => x.Computer.Equals(txtPC.Text)).ToList();
        // }
        if (!string.IsNullOrEmpty(drpComputer.SelectedValue))

            data = data.Where(x => x.Computer.Equals(drpComputer.SelectedValue)).ToList();
        //JavaScriptSerializer serializer = new JavaScriptSerializer();
        // NLogLogger.Info(new string[] { "Data", "Callback", "data NULL", serializer.Serialize(data) });
        rptList.DataSource = data;
        rptList.DataBind();


    }
    protected void UpdateTime_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        int Id = Convert.ToInt32(e.CommandArgument.ToString());
        var _Bank = new BankAccounts();
        _Bank.StopScanAt_Update(Id, DateTime.Now.AddMinutes(10));

        _Bank.Id = Id;
        _Bank = _Bank.Get();
        if (_Bank.Solution == "APIV3")
        {
            var url = "http://127.0.0.1:9003/BankService.ashx";

            
            var requesData = new
            {
                CommandCode = "PROCESS_TRANS_IN",
                RequestContent = serializer.Serialize(new ProcessTransIn
                {
                    BankCode=_Bank.BankCode,
                    BankId=_Bank.BankId,
                    FromDate="",
                    ToDate = "",
                    MaxRow =50,
                })
            };
            Task.Run(async () => await CallbackJson(
               url,
               serializer.Serialize(requesData)
           ));
        }



        BindData();
    }
    protected void btAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.BankAccountAdd);
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
    }
    protected void Reset_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        int Id = Convert.ToInt32(e.CommandArgument.ToString());
        var _Bank = new BankAccounts();
        _Bank.Id = Id;

        _Bank.Reset();
        BindData();
    }
    protected void cbxStatus_CheckedChanged(object sender, EventArgs e)
    {
        if (sender != null)
        {
            var id = int.Parse(((CheckBox)sender).ToolTip);
            var _Bank = new BankAccounts();
            _Bank = _Bank.Get(id);
            if (_Bank.Status == 1)
            {
                _Bank.Status = 0;
            }
            else
            {
                _Bank.Status = 1;
            }
            _Bank.Update();
            var _userLog = new UserLog
            {
                UserName = AppUtils.UserName,
                Action = "bankupdate",
                ActionName = "Cập nhật bank",
                Description = "Cập nhật trạng thái bank " + _Bank.BankCode + " |" + _Bank.BankId + " |" + _Bank.Status.ToString()
            };
            _userLog.Add();

        }
    }
    public void NotifyBank(string bankId, string bankCode, int status)
    {
        if(bankCode!="ACB")
            return;
        var UrlBaseService = "http://127.0.0.1:9003/BankService.ashx";
        var obj = new { BankId = bankId, BankCode = bankCode, Action = "start" };
        if (status != 1)
            obj = new { BankId = bankId, BankCode = bankCode, Action = "stop" };
        var requesData = new RequestData()
        {
            CommandCode = "NOTIFY_ACTION",
            RequestContent = serializer.Serialize(obj),
        };
       
        Task.Run(async () => await CallbackJson(
            UrlBaseService,
            serializer.Serialize(requesData)
        ));
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
    public string GetBankClass(object Type, object Status, object Balance, object MDIN, object BankCode)
    {
        try
        {


            if (Status.ToString() == "1")
            {

                if (Type.ToString().Equals("IN"))
                {
                    if (int.Parse(Balance.ToString()) > 20000000)
                    {
                        return "bwarning";
                    }
                }
                if (Type.ToString().Equals("INOUT"))
                {
                    if (int.Parse(Balance.ToString()) > 200000000)
                    {
                        return "bwarning";
                    }
                    if (BankCode.ToString() == "ACB")
                    {
                        if (int.Parse(Balance.ToString()) > 100000000)
                        {
                            return "bwarning";
                        }
                    }
                }
                if (Type.ToString().Equals("OUT"))
                {
                    if (int.Parse(MDIN.ToString()) > 20)
                    {
                        if (int.Parse(Balance.ToString()) < 10000000)
                        {
                            return "bwarning";
                        }
                        if (BankCode.ToString() == "VPB")
                        {
                            if (int.Parse(Balance.ToString()) < 10000000)
                            {
                                return "bwarning";
                            }
                        }
                    }
                }

            }
        }
        catch { }

        return "";
    }
    public string GetBankType(object banktype)
    {
        if (banktype.ToString() == "IND")
        {
            return "Cá nhân";
        }
        return "Danh nghiệp";
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

    public string GetStatusExtra(object statusExtra)
    {
        if (statusExtra.ToString() == "1")
        {
            return "<span class=\"label label-success\">Logged</span>";
        }
        if (statusExtra.ToString() == "2")
        {
            return "<span class=\"label label-warning\">OTPRequired</span>";
        }
        if (statusExtra.ToString() == "0")
        {
            return "<span class=\"label label-warning\">Ide</span>";
        }
        if (statusExtra.ToString() == "-1")
        {
            return "<span class=\"label label-danger\">DeActive</span>";
        }
        if (statusExtra.ToString() == "-3")
        {
            return "<span class=\"label label-danger\">LoginFailed</span>";
        }
        if (statusExtra.ToString() == "-4")
        {
            return "<span class=\"label label-danger\">AccLocked</span>";
        }
        if (statusExtra.ToString() == "-124" || statusExtra.ToString() == "-123")
        {
            return "<span class=\"label label-warning\">OTPOver</span>";
        }

        return "N/A";
    }
    protected void btCancelClick(object sender, EventArgs e)

    {
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            var _Partner = new Partners();
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus2");
            Label TransactionID = (Label)rptList.Items[i].FindControl("ID");
            if (cbx.Checked)
            {
                int id = Convert.ToInt32(TransactionID.Text);
                var _Bank = new BankAccounts();
                _Bank = _Bank.Get(id);
                if (_Bank.Status == 1)
                {
                    _Bank.Status = 0;
                    _Bank.Update();

                    if (_Bank.Solution == "APIV3")
                        NotifyBank(_Bank.BankId, _Bank.BankCode, _Bank.Status);
                    var _userLog = new UserLog
                    {
                        UserName = AppUtils.UserName,
                        Action = "bankupdate",
                        ActionName = "Cập nhật bank",
                        Description = "Cập nhật trạng thái bank " + _Bank.BankCode + " |" + _Bank.BankId + " |" + _Bank.Status.ToString()
                    };
                    _userLog.Add();
                }

            }
        }
        System.Threading.Thread.Sleep(1000);
        Response.Redirect(Request.RawUrl);
    }

    protected void btApp2Click(object sender, EventArgs e)

    {
        for (int i = 0; i < rptList.Items.Count; i++)
        {
            var _Partner = new Partners();
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus2");
            Label TransactionID = (Label)rptList.Items[i].FindControl("ID");
            if (cbx.Checked)
            {
                int id = Convert.ToInt32(TransactionID.Text);
                var _Bank = new BankAccounts();
                _Bank = _Bank.Get(id);
                if (_Bank.Status == 0)
                {
                    _Bank.Status = 1;
                    _Bank.Update();

                    if (_Bank.Solution == "APIV3")
                        NotifyBank(_Bank.BankId, _Bank.BankCode, _Bank.Status);

                    var _userLog = new UserLog
                    {
                        UserName = AppUtils.UserName,
                        Action = "bankupdate",
                        ActionName = "Cập nhật bank",
                        Description = "Cập nhật trạng thái bank " + _Bank.BankCode + " |" + _Bank.BankId + " |" + _Bank.Status.ToString()
                    };
                    _userLog.Add();
                }

            }
        }
        System.Threading.Thread.Sleep(1000);
        Response.Redirect(Request.RawUrl);
    }
    //quet
    protected void btScanClick(object sender, EventArgs e)

    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        for (int i = 0; i < rptList.Items.Count; i++)
        {
            var _Partner = new Partners();
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus2");
            Label TransactionID = (Label)rptList.Items[i].FindControl("ID");
            if (cbx.Checked)
            {
                int id = Convert.ToInt32(TransactionID.Text);
                var _Bank = new BankAccounts();
                _Bank = _Bank.Get(id);

                _Bank.StopScanAt_Update(id, DateTime.Now.AddMinutes(10));


                if (_Bank.Solution == "APIV3")
                {
                    var url = "http://127.0.0.1:9003/BankService.ashx";


                    var requesData = new
                    {
                        CommandCode = "PROCESS_TRANS_IN",
                        RequestContent = serializer.Serialize(new ProcessTransIn
                        {
                            BankCode = _Bank.BankCode,
                            BankId = _Bank.BankId,
                            FromDate = "",
                            ToDate = "",
                            MaxRow = 50,
                        })
                    };
                    Task.Run(async () => await CallbackJson(
                       url,
                       serializer.Serialize(requesData)
                   ));
                }


            }
        }
        Response.Redirect(Request.RawUrl);
    }
    protected void btUpdateAllClick()

    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();


        for (int i = 0; i < rptList.Items.Count; i++)
        {
            var _Partner = new Partners();
            CheckBox cbx = (CheckBox)rptList.Items[i].FindControl("cbxStatus2");
            Label TransactionID = (Label)rptList.Items[i].FindControl("ID");
            if (cbx.Checked)
            {
                int id = Convert.ToInt32(TransactionID.Text);
                var _Bank = new BankAccounts();
                _Bank = _Bank.Get(id);

                if (Convert.ToInt32(txtBalanceMaxDay.Text) > 0)
                    _Bank.BalanceMaxDay = Convert.ToInt32(txtBalanceMaxDay.Text);
                if (Convert.ToInt32(txtBalanceMaxMonth.Text) > 0)
                    _Bank.BalanceMaxMonth = Convert.ToInt32(txtBalanceMaxMonth.Text);
                if (!string.IsNullOrEmpty(drpType2.SelectedValue))
                    _Bank.Type = drpType2.SelectedValue;

                if (!string.IsNullOrEmpty(drpSolution2.SelectedValue))
                    _Bank.Solution = drpSolution2.SelectedValue;

                _Bank.Update();
                System.Threading.Thread.Sleep(100);
                if (drpSolution2.SelectedValue == "APIV2"|| drpSolution2.SelectedValue == "APIV")
                {
                    NotifyBank(_Bank.BankId,_Bank.BankCode, 0);
                }


            }
        }
        Response.Redirect(Request.RawUrl);
    }
    public string  GetQR(string BankCode, string BankId, string AccountName)
    {
        if (BankCode == "NVB")
            BankCode = "NCB";
        return String.Format("https://img.vietqr.io/image/{0}-{1}-print.jpg?accountName={2}", BankCode, BankId, AccountName);
    }
    public string GetSolutionStyle(object statusOver)
    {
        if (statusOver.ToString() == "PC002")
        {
            return "font-weight:bold";
        }


        return "";
    }

    public class RequestData
    {
        public string PartnerCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public string Signature { get; set; }
    }
    public class ProcessTransIn
    {
        public string BankCode { get; set; }
        public string BankId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int MaxRow { get; set; }
    }
}