using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;
using System.ComponentModel;


public partial class Pages_Security_UsersHistory : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.UsersHistory);
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

        txtEndTime.Text = DateTime.Now.ToString("MM/dd/yyyy");
        txtBeginTime.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("MM/dd/yyyy");
    }
    protected void BindData()
    {
        var name = Request["name"];
        //DateTime begintime = AppUtils.ToDateTime(txtBeginTime.Text);
        //DateTime endtime = AppUtils.ToDateTime(txtEndTime.Text);
        DateTime begintime = AppUtils.DateTimeParseExact(txtBeginTime.Text);
        DateTime endtime = AppUtils.DateTimeParseExact(txtEndTime.Text);
        endtime = endtime.AddDays(1).AddMilliseconds(-1);

        var lstdata = new UserTransaction().GetList(name, txtPartnerCode.Text, txtNote.Text, int.Parse(drpType.SelectedValue), begintime, endtime,int.Parse(drpTop.SelectedValue));
        rptList.DataSource = lstdata;
        rptList.DataBind();
    }
    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
    }
    public static string InsertCommaMark(string strMoney, int type,string note)
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