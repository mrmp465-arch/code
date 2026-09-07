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

public partial class Pages_Momo_Accounts : System.Web.UI.Page
{
    protected long total;
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
                data = data.Where(x => x.StatusExtra==-16|| x.StatusExtra == -123||  x.StatusExtra == -124).ToList();
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

        lblTotal.Text = String.Format("Tổng số ví : {0} - tổng số dư : {1}- Sản lượng: {3}/ {2}", data.Count().ToString(), data.Where(x => x.Status == 1).Sum(x => Convert.ToInt64(x.BalanceTotal)).ToString("N0"), (data.Where(x => x.Status >= 0).Sum(x => x.BalanceMaxMonth)).ToString("N0"), total.ToString("N0"));

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

    protected void drpPartner_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindData();
    }
    protected void btAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.MomoAccountAdd);
    }
    protected void btAdd2_Click(object sender, EventArgs e)
    {
        Response.Redirect( "cmspay/pages/momo/account.add2.aspx");
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
    public string GetSolutionStyle(object statusOver)
    {
        if (statusOver.ToString() == "APIV2")
        {
            return "font-weight:bold";
        }



        return "";
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
            return "<span class=\"label label-warning\">Captcha Required</span>";
        }
        return "N/A ("+ statusOver.ToString()+")";
    }
}