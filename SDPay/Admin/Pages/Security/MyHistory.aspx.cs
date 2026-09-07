using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;
using System.ComponentModel;
using System.Net.NetworkInformation;
using DocumentFormat.OpenXml.Drawing;
using System.Globalization;
using System.IO;
using Libs.Utils;
using OfficeOpenXml;
public partial class Pages_Security_MyHistory : System.Web.UI.Page
{
    public bool IsDL { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.MyHistory);
        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
        if (AppUtils.IsTopup)
        {
            var partnerlist = new Partners().GetList();
            if (partnerlist.Exists(x => x.SMSUrl == AppUtils.UserName))
            {
                IsDL = true;
            }
        }
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
        btView.Text = Resources.Pay.View;
        btExcel.Text = Resources.Pay.ExportExcel;
        txtEndTime.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddSeconds(-1).ToString("dd/MM/yyyy HH:mm:ss");
        txtBeginTime.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).ToString("dd/MM/yyyy HH:mm:ss");

        drpType.Items.Insert(0, new ListItem(Resources.Pay.TransactionType, "-1"));
        drpType.Items.Insert(1, new ListItem(Resources.Pay.AddMoney, "1"));
        drpType.Items.Insert(2, new ListItem(Resources.Pay.DeductMoney, "2"));
        //drpStatus.Items.Insert(2, new ListItem(Resources.Pay.Processing, "0"));
        drpType.Items.Insert(3, new ListItem(Resources.Pay.WithdrawMoney, "3"));

        var lst = new List<Users>();
        if (AppUtils.IsAdmin)
        {
            lst = new Users().GetList().Where(x => x.IsPartner == 1).OrderBy(x => x.UserName).ToList();

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
        var name = AppUtils.UserName;
        if (AppUtils.IsAdmin)
        {
            name = drpUser.SelectedValue;
        }
        else
        {
            if (!AppUtils.IsPartner && !IsDL)
            {


                var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
                name = lstPartner.FirstOrDefault().Name;
            }
        }
        //if (!string.IsNullOrEmpty(name))
        //{
        //DateTime begintime = AppUtils.ToDateTime(txtBeginTime.Text);
        //DateTime endtime = AppUtils.ToDateTime(txtEndTime.Text);
        DateTime begintime = ToDateTime(txtBeginTime.Text);
        DateTime endtime = ToDateTime(txtEndTime.Text);
        //endtime = endtime.AddDays(1).AddMilliseconds(-1);

        var lstdata = new UserTransaction().GetList(name, txtPartnerCode.Text, txtNote.Text, int.Parse(drpType.SelectedValue), begintime, endtime, int.Parse(drpTop.SelectedValue));
        rptList.DataSource = lstdata;
        rptList.DataBind();
        //}

    }
    protected void btView_Click(object sender, EventArgs e)
    {
        BindData();
    }
    protected void ExportTran2_Click(object sender, EventArgs e)
    {
        var name = AppUtils.UserName;
        if (AppUtils.IsAdmin)
        {
            name = drpUser.SelectedValue;
        }
        else
        {
            if (!AppUtils.IsPartner && !IsDL)
            {


                var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
                name = lstPartner.FirstOrDefault().Name;
            }
        }
        DateTime begintime = ToDateTime(txtBeginTime.Text);
        DateTime endtime = ToDateTime(txtEndTime.Text);

        if (!string.IsNullOrEmpty(name))
        {
            var lstdata = new UserTransaction().GetList(name, "", "", -1, begintime, endtime, 200000);
            var Data = new List<HistoryExcel>();
            foreach (var bank in lstdata)
            {
                var item = new HistoryExcel();
                item.Id = bank.Id;
                item.RefCode = bank.RefCode;
                item.TransactionId = gettran(bank.Note);
                item.CreatedTime = bank.CreatedTime.ToString("dd/MM/yyyy HH:mm:ss"); ;
                item.Note = bank.Note;
                item.Balance = bank.Balance;
                item.Amount = bank.Amount;
                item.BalanceBefore = bank.BalanceBefore;
                item.Type = GetSType(bank.Type);
                Data.Add(item);
            }

            ExportToExcel(Data, "history-" + name.Replace(",", "") + endtime.ToString("ddMMyyy"));
        }

    }
    public string gettran(string value)
    {
        if (value.Contains("-"))
        {
            return value.Split('-').LastOrDefault();
        }
        return "";

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
    public class HistoryExcel
    {
        public long Id { get; set; }
        public string RefCode { get; set; }
        public string TransactionId { get; set; }
        public string Type { get; set; }
        //1- topup 2 deduct 3 widraw
        public long Amount { get; set; }
        public string Note { get; set; }
        public long BalanceBefore { get; set; }

        public long Balance { get; set; }
        public string CreatedTime { get; set; }

    }

    //protected void ExportToExcel(List<HistoryExcel> data, string name)
    //{
    //    Response.Clear();
    //    Response.Buffer = true;
    //    //Response.Charset = "UTF-8"; 
    //    Response.AppendHeader("Content-Disposition", "attachment;filename=" + name + ".xls");
    //    Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
    //    Response.ContentType = "application/ms-excel";
    //    EnableViewState = false;
    //    var myCItrad = new CultureInfo("VI-VN", true);
    //    var oStringWriter = new StringWriter(myCItrad);
    //    var oHtmlTextWriter = new HtmlTextWriter(oStringWriter);


    //    var grid = new DataGrid { DataSource = data };
    //    grid.DataBind();
    //    grid.RenderControl(oHtmlTextWriter);

    //    Response.Write(oStringWriter.ToString());
    //    Response.Flush();
    //    Response.End();
    //}
    protected void ExportToExcel(List<HistoryExcel> data, string name)
    {
        using (var package = new ExcelPackage())
        {
            var ws = package.Workbook.Worksheets.Add("Data");
            ws.Cells["A1"].LoadFromCollection(data, true);

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + name + ".xlsx");
            Response.BinaryWrite(package.GetAsByteArray());
            Response.End();
        }
    }
    public string GetSType(int statusOver)
    {

        if (statusOver.ToString() == "1")
        {
            return Resources.Pay.AddMoney;
        }
        if (statusOver.ToString() == "2")
        {
            return Resources.Pay.DeductMoney;
        }
        if (statusOver.ToString() == "3")
        {
            return Resources.Pay.WithdrawMoney;
        }



        return "<div class=\"label label-info\">" + statusOver + "</div>";
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
        if (type == 1 || type == 4)
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

        return "<span class='" + classcss + "'>" + sign + strMoney.Replace(".", ",") + "</span>";
    }
}