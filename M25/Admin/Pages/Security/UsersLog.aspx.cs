using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using Libs.Utils;


public partial class Pages_Security_UsersLog : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.UsersLog);
        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
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

        var lst = new List<Users>();
        if (AppUtils.IsAdmin)
        {
            lst = new Users().GetList().Where(x=>x.IsAdmin==1).OrderBy(x => x.UserName).ToList();

        }
        else
        {
            lst.Add(new Users { UserName = AppUtils.UserName });
        }


        drpUser.DataSource = lst;
        drpUser.DataTextField = "UserName";
        drpUser.DataValueField = "UserName";
        drpUser.DataBind();
        drpUser.Items.Insert(0, new ListItem("Tài khoản:", ""));

    }
    protected void BindData()
    {
        DateTime requestTime = ToDateTime(txtCreatTime.Text);
        DateTime fromDate = ToDateTime(txtFromDate.Text);
        //NLogLogger.Info(new string[] { "BindData", "BindData", drpTop.SelectedValue, drpType.SelectedValue, drpUser.SelectedValue, txtNote.Text, requestTime.ToString(), fromDate.ToString() });
        var lstdata = new UserLog().GetList(int.Parse(drpTop.SelectedValue), drpType.SelectedValue,drpUser.SelectedValue,txtNote.Text, fromDate, requestTime);
        
        rptList.DataSource = lstdata;
        rptList.DataBind();
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
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
    public static string InsertCommaMark(string strMoney, int type, string note)
    {
        if (string.IsNullOrEmpty(strMoney))
            return string.Empty;

        string sign = "-";
        string classcss = "blue_txt";
        if (type == 2 || type == 3)
        {

            strMoney = strMoney.Replace(sign, "");
            classcss = "red_txt";
            if (note.ToLower().Contains("hủy lệnh rút tiền"))
            {
                sign = "+";
                classcss = "blue_txt";
            }

        }
        else
        if (type == 1)
        {
            if (strMoney == "0")
                sign = "";
            else
                sign = "+";
        }

        int length = strMoney.Length;
        while (length > 3)
        {
            strMoney = strMoney.Insert(length - 3, ".");
            length = strMoney.IndexOf('.');
        }

        return "<span class='" + classcss + "'>" + sign + strMoney + "</span>";
    }
}