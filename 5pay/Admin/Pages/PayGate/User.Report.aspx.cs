using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;
using Libs.Utils;

public partial class Pages_PayGate_User_Report : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.UserReport);
       
        if (!IsPostBack)
        {


            Init();
            BindData();
        }
    }
    protected void Init()
    {
        // DateTime time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

        //txtBeginTime.Text = time.AddDays(-7).ToString();
        //txtEndTime.Text = time.AddDays(1).ToString();

        txtCreatTime.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddSeconds(-1).ToString("dd/MM/yyyy HH:mm:ss");
        txtFromDate.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("dd/MM/yyyy HH:mm:ss");

        ddlUser.DataSource = new Users().GetList().Where(x => x.IsPartner == 1 &&x.Balance>0).ToList();
        ddlUser.DataTextField = "UserName";
        ddlUser.DataValueField = "UserName";
        ddlUser.DataBind();
        ddlUser.Items.Insert(0, new ListItem("Chọn tài khoản:", ""));


    }
    protected void BindData()
    {
        DateTime requestTime = ToDateTime(txtCreatTime.Text);
        DateTime fromDate = ToDateTime(txtFromDate.Text);
       
        
        if (AppUtils.IsAdmin)
        {
            var lstData = new UserDaily().GetList(ddlUser.SelectedValue, fromDate,requestTime);
            //lblTotal.Text = String.Format("Tổng số dư : {0}", lstData.Sum(x => x.Balance).ToString("N0")).Replace(".", ",");
            rptList.DataSource = lstData;
            rptList.DataBind();
        }
        else
        {
            var lstData = new UserDaily().GetList(AppUtils.UserName,  fromDate, requestTime);
            //lblTotal.Text = String.Format("Tổng số dư : {0}", lstData.Sum(x => x.Balance).ToString("N0"));
            rptList.DataSource = lstData;
            rptList.DataBind();
        }    
        
       
    }
    public DateTime ToDateTime(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            try
            {
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                return DateTime.Parse(value, cul);
                //return Convert.ToDateTime(value);
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }
        return DateTime.Now;
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
    }
    public string formatDay(string day)
    {
        var newDate = DateTime.ParseExact(day,
                                   "yyyyMMdd",
                                    CultureInfo.InvariantCulture);
        return newDate.ToString("dd/MM/yyyy");
        //return String.Format("{0}/{1}/{2}", day[6] + day[7], day[4] + day[5], day[0] + day[1] + day[2] + day[3]);
    }
}