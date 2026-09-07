using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;
using Libs.Utils;
using System.Globalization;
using System.IO;
using ServiceStack.Common.Extensions;

using System.Net;
using System.Text;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Data;

public partial class Default : System.Web.UI.Page
{
    protected string graphLineData = String.Empty;
    protected string graphPieData = String.Empty;
    protected long totalAmount = 0;
    protected int totalTransaction = 0;
    protected string totalMonthAmount;
    protected float monthRate = 0;
    protected int providerCount = 0;
    protected long balance = 0;
    protected bool checkAdd;
    string urlService = "http://127.0.0.1:1592/bankin/order.ashx";
    public List<BankDashboardReport> lstReportDoiSoat1 { get; set; }
    protected int partnerCount = 0;
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckLogin();
        Page.Culture = Libs.Utils.GlobalHelper.GetLanguage();
        Page.UICulture = Libs.Utils.GlobalHelper.GetLanguage();
        checkAdd = AppUtils.CheckRolesPermission("add.aspx");
        if (AppUtils.IsAdmin)
            checkAdd = false;
        if (checkAdd)
            Response.Redirect("/cmspay/Nap.aspx");
        if (AppUtils.CheckRolesPermission(Resources.Url.Dashboard))
        {
            if (!IsPostBack)
            {
                Init();
                GetList(string.Empty, string.Empty, string.Empty);
                BindData();
                if (AppUtils.IsAdmin)
                    BindData2();
                //if (checkAdd)
                //    GetListBankLog();
            }
        }
        else
        {
            PanelContent.Visible = false;
        }

    }

    private void Init()
    {
        var lst = new List<Partners>();
        if (AppUtils.IsAdmin)
            lst = new Partners().GetList().Where(e => e.Status == 1).ToList();
        else
        {
            //drpPartner.Visible = false;
            //drpCardType.Visible = false;
            lst = new Partners().GetListByUserId(AppUtils.UserID);

            //if (AppUtils.IsPartner)
            //{
            //    foreach (var item in lst)
            //    {
            //        balance += item.Balance;
            //    }
            //}
            var user = new Users().Get(AppUtils.UserID);
            balance = user.Balance;
        }
        var lstGroup = new PartnerGroup().GetList();
        lstGroup = lstGroup.OrderBy(x => x.Name).ToList();
        drpGroup.DataSource = lstGroup;
        drpGroup.DataTextField = "Name";
        drpGroup.DataValueField = "Name";
        drpGroup.DataBind();

        drpGroup.Items.Insert(0, new ListItem("Nhóm:", ""));
        drpGroup.Items.Insert(1, new ListItem("other", "other"));

        btView.Text = Resources.Pay.View;
        Button1.Text = Resources.Pay.View;
        btExcel.Text = Resources.Pay.ExportExcel;
        lst = lst.OrderBy(x => x.PartnerCode).ToList();
        // Month
        drpMonth.Items.Add(new ListItem(Resources.Pay.Month, "0"));
        for (int i = 1; i <= 12; i++)
        {
            drpMonth.Items.Add(new ListItem(i.ToString()));
        }

        drpMonth.SelectedValue = DateTime.Now.Month.ToString();

        drpPartner.DataSource = lst;
        drpPartner.DataTextField = "Name";
        drpPartner.DataValueField = "PartnerID";
        drpPartner.DataBind();
        drpPartner.Items.Insert(0, new ListItem(Resources.Pay.Partner, ""));

        //if (AppUtils.IsPartner && !AppUtils.IsAdmin)
        //{
        //    lst = new Partners().GetListByUserId(AppUtils.UserID);
        //}
        drpPartner2.DataSource = lst;
        drpPartner2.DataTextField = "Name";
        drpPartner2.DataValueField = "PartnerCode";
        drpPartner2.DataBind();
        drpPartner2.Items.Insert(0, new ListItem(Resources.Pay.Partner, ""));

        //drpPartner3.DataSource = lst;
        //drpPartner3.DataTextField = "Name";
        //drpPartner3.DataValueField = "PartnerCode";
        //drpPartner3.DataBind();
        //drpPartner3.Items.Insert(0, new ListItem("Đối tác:", ""));


        txtEndTime.Text = DateTime.Now.ToString("MM/dd/yyyy");
        txtBeginTime.Text = DateTime.Now.ToString("MM/dd/yyyy");

        txtEndTime2.Text = DateTime.Now.ToString("MM/dd/yyyy");
        //txtBeginTime2.Text = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("MM/dd/yyyy");
        txtBeginTime2.Text = DateTime.Now.ToString("MM/dd/yyyy");
        var _CardType = new Products();
        drpCardType.DataSource = new List<Products>();
        drpCardType.DataTextField = "Name";
        drpCardType.DataValueField = "Code";
        drpCardType.DataBind();
        drpCardType.Items.Insert(0, new ListItem("Type:", ""));
        drpCardType.Items.Insert(1, new ListItem("BankIN", "BankIN"));
        drpCardType.Items.Insert(2, new ListItem("BankOUT", "BankOUT"));
        //drpCardType.Items.Insert(3, new ListItem("Card", "Card"));
        //drpCardType.Items.Insert(4, new ListItem("BuyCard", "BuyCard"));
    }
    //protected void btAdd_Click(object sender, EventArgs e)
    //{
    //    var partner = new Partners().GetCache(AppUtils.UserName);
    //    Order(partner.PartnerCode, partner.PublicKey);
    //}
    protected void btView_Click(object sender, EventArgs e)
    {
        GetList(string.Empty, string.Empty, string.Empty);
        BindData();
        var user = new Users().Get(AppUtils.UserID);
        balance = user.Balance;
    }
    protected void btView_Click2(object sender, EventArgs e)
    {
        GetList(string.Empty, string.Empty, string.Empty);
        BindData2();
    }
    protected void BindData()
    {
        try
        {
            DateTime begintime = AppUtils.DateTimeParseExact(txtBeginTime.Text);
            DateTime endtime = AppUtils.DateTimeParseExact(txtEndTime.Text);
            endtime = endtime.AddDays(1).AddMilliseconds(-1);
            string partnerCodes = drpPartner2.SelectedValue;

            if (!AppUtils.IsAdmin)
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
            var lstDataBank = new BankGateAPI().ReportDashboard(partnerCodes, begintime, endtime);
            var lstDataBankCash = new BankCashAPI().ReportDashboard(partnerCodes, begintime, endtime);

            //var lstDataCard = new CardAPILog().ReportDashboard(partnerCodes, begintime, endtime);
            //var lstDataBuyCard = new BuyCard().ReportDashboard(partnerCodes, begintime, endtime);


            var lstData = new List<BankDashboardReport>();
            //var item = new BankDashboardReport();
            //item.Type = "MOMO";

            //if (lstDataBank.Exists(x => x.Type == 1))
            //{
            //    var data = lstDataBank.FirstOrDefault(x => x.Type == 1);
            //    item.TotalFee = data.TotalFee;
            //    item.TotalTrans = data.TotalTrans;
            //    item.TotalTransSuccess = data.TotalTransSuccess;
            //    item.TotalAmountSuccess = data.TotalAmountSuccess;
            //}
            //else
            //{
            //    item.TotalFee = 0;
            //    item.TotalTrans = 0;
            //    item.TotalTransSuccess = 0;
            //    item.TotalAmountSuccess = 0;
            //}
            //if (lstDataBankCash.Exists(x => x.Type == 1))
            //{
            //    var data = lstDataBankCash.FirstOrDefault(x => x.Type == 1);
            //    item.TotalFeeCash = data.TotalFee;
            //    item.TotalTransCash = data.TotalTrans;
            //    item.TotalTransSuccessCash = data.TotalTransSuccess;
            //    item.TotalAmountSuccessCash = data.TotalAmountSuccess;
            //}
            //else
            //{
            //    item.TotalFeeCash = 0;
            //    item.TotalTransCash = 0;
            //    item.TotalTransSuccessCash = 0;
            //    item.TotalAmountSuccessCash = 0;
            //}


            var item2 = new BankDashboardReport();
            item2.Type = "BANK";
            if (lstDataBank.Exists(x => x.Type == 2))
            {
                var data = lstDataBank.FirstOrDefault(x => x.Type == 2);
                item2.TotalFee = data.TotalFee;
                item2.TotalTrans = data.TotalTrans;
                item2.TotalTransSuccess = data.TotalTransSuccess;
                item2.TotalAmountSuccess = data.TotalAmountSuccess;
            }
            else
            {
                item2.TotalFee = 0;
                item2.TotalTrans = 0;
                item2.TotalTransSuccess = 0;
                item2.TotalAmountSuccess = 0;
            }
            if (lstDataBankCash.Exists(x => x.Type == 2))
            {
                var data = lstDataBankCash.FirstOrDefault(x => x.Type == 2);
                item2.TotalFeeCash = data.TotalFee;
                item2.TotalTransCash = data.TotalTrans;
                item2.TotalTransSuccessCash = data.TotalTransSuccess;
                item2.TotalAmountSuccessCash = data.TotalAmountSuccess;
            }
            else
            {
                item2.TotalFeeCash = 0;
                item2.TotalTransCash = 0;
                item2.TotalTransSuccessCash = 0;
                item2.TotalAmountSuccessCash = 0;
            }
            lstData.Add(item2);
            //lstData.Add(item);

            //var item4 = new BankDashboardReport();
            //item4.Type = "CARD";

            //var datacard = lstDataCard.FirstOrDefault();
            //item4.TotalFee = datacard.TotalFee;
            //item4.TotalTrans = datacard.TotalTrans;
            //item4.TotalTransSuccess = datacard.TotalTransSuccess.Value;
            //item4.TotalAmountSuccess = datacard.TotalAmountSuccess;
            //lstData.Add(item4);
            var item3 = new BankDashboardReport();
            item3.Type = Resources.Pay.Total;
            item3.TotalFee =  item2.TotalFee ;
            item3.TotalTrans = item2.TotalTrans ;
            item3.TotalTransSuccess =  item2.TotalTransSuccess ;
            item3.TotalAmountSuccess =  item2.TotalAmountSuccess ;
            item3.TotalFeeCash = item2.TotalFeeCash ;
            item3.TotalTransCash =  item2.TotalTransCash ;
            item3.TotalTransSuccessCash =  item2.TotalTransSuccessCash ;
            item3.TotalAmountSuccessCash =  item2.TotalAmountSuccessCash ;


            if (AppUtils.IsAdmin)
            {
                var lisWithdraw = new UserWithdraw().GetList(100, drpPartner2.SelectedValue, 1, begintime, endtime);
                item3.TotalDeduct = lisWithdraw.Sum(x => x.Amount);

                var lisWithdraw2 = new UserDeposit().GetList(100, drpPartner2.SelectedValue, 1, begintime, endtime);
                item3.TotalTopup = lisWithdraw2.Sum(x => x.Amount);
            }
            else
            {
                var lisWithdraw = new UserWithdraw().GetList(100, AppUtils.UserName, 1, begintime, endtime);
                item3.TotalDeduct = lisWithdraw.Sum(x => x.Amount);

                var lisWithdraw2 = new UserDeposit().GetList(100, AppUtils.UserName, 1, begintime, endtime);
                item3.TotalTopup = lisWithdraw2.Sum(x => x.Amount);
            }

            lstData.Add(item3);


            rptListBank.DataSource = lstData;
            rptListBank.DataBind();
        }
        catch (Exception ex)
        {
            NLogLogger.Info(ex.Message);
            Response.Redirect(Constant.ADMIN_PATH + "500.html");
        }
    }
    protected void BindData2()
    {
        try
        {

            DateTime begintime = AppUtils.DateTimeParseExact(txtBeginTime2.Text);
            DateTime endtime = AppUtils.DateTimeParseExact(txtEndTime2.Text);
            endtime = endtime.AddDays(1).AddMilliseconds(-1);

            var lstDataBank = new BankGateAPI().ReportDashboardPartner(string.Empty, begintime, endtime);
            var lstDataBankCash = new BankCashAPI().ReportDashboardPartner(string.Empty, begintime, endtime);

            var lstDataCard = new CardAPILog().ReportDashboardPartner(string.Empty, begintime, endtime);
            //var lstDataBuyCard = new BuyCard().ReportDashboardPartner(string.Empty, begintime, endtime);

            var lstData = new List<BankDashboardReport>();
            var listPartnerCode = lstDataBank.GroupBy(x => x.PartnerCode).Select(x => x.Key).ToList();
            var listPartnerCode2 = lstDataCard.GroupBy(x => x.PartnerCode).Select(x => x.Key).ToList();
            var listPartnerCode3 = lstDataBankCash.GroupBy(x => x.PartnerCode).Select(x => x.Key).ToList();

            foreach (var p in listPartnerCode2)
            {
                if (!listPartnerCode.Exists(x => x.Equals(p)))
                {
                    listPartnerCode.Add(p);
                }
            }
            foreach (var p in listPartnerCode3)
            {
                if (!listPartnerCode.Exists(x => x.Equals(p)))
                {
                    listPartnerCode.Add(p);
                }
            }
            //NLogLogger.Info(new string[] { "dataa", serializer.Serialize(listPartnerCode) });
            string partnerCodes = "";
            if (!string.IsNullOrEmpty(drpGroup.SelectedValue))
            {
                var lstPartner = new Partners().GetList().Where(x => x.SMSUrl == drpGroup.SelectedValue).ToList();
                if (lstPartner != null && lstPartner.Count > 0)
                {
                    partnerCodes = string.Join(",", lstPartner.Select(e => e.PartnerCode).ToArray());
                }
            }

            foreach (var pacode in listPartnerCode)
            {
                if (pacode.Contains(partnerCodes) || partnerCodes == "")
                {
                    if (!string.IsNullOrEmpty(pacode))
                    {
                        var item2 = new BankDashboardReport();
                        item2.Type = "BANK";
                        item2.PartnerCode = pacode;
                        if (lstDataBank.Exists(x => x.Type == 2 && x.PartnerCode == pacode))
                        {
                            var data = lstDataBank.FirstOrDefault(x => x.Type == 2 && x.PartnerCode == pacode);
                            item2.TotalFee = data.TotalFee;
                            item2.TotalTrans = data.TotalTrans;
                            item2.TotalTransSuccess = data.TotalTransSuccess;
                            item2.TotalAmountSuccess = data.TotalAmountSuccess;

                        }
                        else
                        {
                            item2.TotalFee = 0;
                            item2.TotalTrans = 0;
                            item2.TotalTransSuccess = 0;
                            item2.TotalAmountSuccess = 0;
                        }
                        if (lstDataBankCash.Exists(x => x.Type == 2 && x.PartnerCode == pacode))
                        {
                            var data = lstDataBankCash.FirstOrDefault(x => x.Type == 2 && x.PartnerCode == pacode);
                            item2.TotalFeeCash = data.TotalFee;
                            item2.TotalTransCash = data.TotalTrans;
                            item2.TotalTransSuccessCash = data.TotalTransSuccess;
                            item2.TotalAmountSuccessCash = data.TotalAmountSuccess;
                        }
                        else
                        {
                            item2.TotalFeeCash = 0;
                            item2.TotalTransCash = 0;
                            item2.TotalTransSuccessCash = 0;
                            item2.TotalAmountSuccessCash = 0;
                        }
                        //if (CheckPartCard(pacode))
                        //{
                        var item = new BankDashboardReport();
                        item.Type = "MOMO";
                        item.PartnerCode = pacode;
                        if (lstDataBank.Exists(x => x.Type == 1 && x.PartnerCode == pacode))
                        {
                            var data = lstDataBank.FirstOrDefault(x => x.Type == 1 && x.PartnerCode == pacode);
                            item.TotalFee = data.TotalFee;
                            item.TotalTrans = data.TotalTrans;
                            item.TotalTransSuccess = data.TotalTransSuccess;
                            item.TotalAmountSuccess = data.TotalAmountSuccess;
                        }
                        else
                        {
                            item.TotalFee = 0;
                            item.TotalTrans = 0;
                            item.TotalTransSuccess = 0;
                            item.TotalAmountSuccess = 0;
                        }
                        if (lstDataBankCash.Exists(x => x.Type == 1 && x.PartnerCode == pacode))
                        {
                            var data = lstDataBankCash.FirstOrDefault(x => x.Type == 1 && x.PartnerCode == pacode);
                            item.TotalFeeCash = data.TotalFee;
                            item.TotalTransCash = data.TotalTrans;
                            item.TotalTransSuccessCash = data.TotalTransSuccess;
                            item.TotalAmountSuccessCash = data.TotalAmountSuccess;
                        }
                        else
                        {
                            item.TotalFeeCash = 0;
                            item.TotalTransCash = 0;
                            item.TotalTransSuccessCash = 0;
                            item.TotalAmountSuccessCash = 0;
                        }

                        //}
                        lstData.Add(item2);
                        if (CheckPartCard(pacode))
                        {
                            lstData.Add(item);
                        }

                        if (CheckPartCard(pacode))
                        {
                            var item3 = new BankDashboardReport();
                            item3.Type = "CARD";
                            item3.PartnerCode = pacode;
                            if (lstDataCard.Exists(x => x.PartnerCode == pacode))
                            {
                                var data = lstDataCard.FirstOrDefault(x => x.PartnerCode == pacode);
                                item3.TotalFee = data.TotalFee;
                                item3.TotalTrans = data.TotalTrans;
                                item3.TotalTransSuccess = data.TotalTransSuccess.Value;
                                item3.TotalAmountSuccess = data.TotalAmountSuccess;
                            }
                            else
                            {
                                item3.TotalFee = 0;
                                item3.TotalTrans = 0;
                                item3.TotalTransSuccess = 0;
                                item3.TotalAmountSuccess = 0;
                            }
                            item3.TotalFeeCash = 0;
                            item3.TotalTransCash = 0;
                            item3.TotalTransSuccessCash = 0;
                            item3.TotalAmountSuccessCash = 0;

                            lstData.Add(item3);
                        }
                    }
                }


            }
            //var totalMOMO = new BankDashboardReport();
            //graphPieData = serializer.Serialize(pieData);

            lstData = lstData.OrderByDescending(x => x.TotalAmountSuccessCash).OrderByDescending(x => x.TotalAmountSuccess).ToList();
            lstReportDoiSoat1 = lstData;
            rptListBank2.DataSource = lstData;
            rptListBank2.DataBind();
        }
        catch (Exception ex)
        {
            NLogLogger.Info(ex.Message);
            //Response.Redirect(Constant.ADMIN_PATH + "500.html");
        }
    }
    public string GetPartCol(string partnercode)
    {
        if (CheckPartCard(partnercode))
            return "3";
        return "1";
    }
    public bool CheckPartCard(string partnercode)
    {

        if (partnercode.Contains("sn1"))
            return true;

        if (partnercode.Contains("b23"))
            return true;
        return false;
    }
    protected void ExportTran2_Click(object sender, EventArgs e)
    {
        GetList(string.Empty, string.Empty, string.Empty);
        DateTime begintime = AppUtils.DateTimeParseExact(txtBeginTime2.Text);
        DateTime endtime = AppUtils.DateTimeParseExact(txtEndTime2.Text);
        endtime = endtime.AddDays(1).AddMilliseconds(-1);

        var lstDataBank = new BankGateAPI().ReportDashboardPartner(string.Empty, begintime, endtime);
        var lstDataBankCash = new BankCashAPI().ReportDashboardPartner(string.Empty, begintime, endtime);

        var lstData = new List<BankDashboardReport>();
        var listPartnerCode = lstDataBank.GroupBy(x => x.PartnerCode).Select(x => x.Key).ToList(); ;
        var listPartnerCode3 = lstDataBankCash.GroupBy(x => x.PartnerCode).Select(x => x.Key).ToList();

        //foreach (var p in listPartnerCode2)
        //{
        //    if (!listPartnerCode.Exists(x => x.Equals(p)))
        //    {
        //        listPartnerCode.Add(p);
        //    }
        //}
        foreach (var p in listPartnerCode3)
        {
            if (!listPartnerCode.Exists(x => x.Equals(p)))
            {
                listPartnerCode.Add(p);
            }
        }
        string partnerCodes = "";
        if (!string.IsNullOrEmpty(drpGroup.SelectedValue))
        {
            var lstPartner = new Partners().GetList().Where(x => x.SMSUrl == drpGroup.SelectedValue).ToList();
            if (lstPartner != null && lstPartner.Count > 0)
            {
                partnerCodes = string.Join(",", lstPartner.Select(x => x.PartnerCode).ToArray());
            }
        }
        foreach (var pacode in listPartnerCode)
        {
            if (partnerCodes.Contains(partnerCodes))
            {
                var item2 = new BankDashboardReport();
                item2.Type = "BANK";
                item2.PartnerCode = pacode;
                if (lstDataBank.Exists(x => x.Type == 2 && x.PartnerCode == pacode))
                {
                    var data = lstDataBank.FirstOrDefault(x => x.Type == 2 && x.PartnerCode == pacode);
                    item2.TotalFee = data.TotalFee;
                    item2.TotalTrans = data.TotalTrans;
                    item2.TotalTransSuccess = data.TotalTransSuccess;
                    item2.TotalAmountSuccess = data.TotalAmountSuccess;

                }
                else
                {
                    item2.TotalFee = 0;
                    item2.TotalTrans = 0;
                    item2.TotalTransSuccess = 0;
                    item2.TotalAmountSuccess = 0;
                }
                if (lstDataBankCash.Exists(x => x.Type == 2 && x.PartnerCode == pacode))
                {
                    var data = lstDataBankCash.FirstOrDefault(x => x.Type == 2 && x.PartnerCode == pacode);
                    item2.TotalFeeCash = data.TotalFee;
                    item2.TotalTransCash = data.TotalTrans;
                    item2.TotalTransSuccessCash = data.TotalTransSuccess;
                    item2.TotalAmountSuccessCash = data.TotalAmountSuccess;
                }
                else
                {
                    item2.TotalFeeCash = 0;
                    item2.TotalTransCash = 0;
                    item2.TotalTransSuccessCash = 0;
                    item2.TotalAmountSuccessCash = 0;
                }

                //var item = new BankDashboardReport();
                //item.Type = "MOMO";
                //item.PartnerCode = pacode;
                //if (lstDataBank.Exists(x => x.Type == 1 && x.PartnerCode == pacode))
                //{
                //    var data = lstDataBank.FirstOrDefault(x => x.Type == 1 && x.PartnerCode == pacode);
                //    item.TotalFee = data.TotalFee;
                //    item.TotalTrans = data.TotalTrans;
                //    item.TotalTransSuccess = data.TotalTransSuccess;
                //    item.TotalAmountSuccess = data.TotalAmountSuccess;
                //}
                //else
                //{
                //    item.TotalFee = 0;
                //    item.TotalTrans = 0;
                //    item.TotalTransSuccess = 0;
                //    item.TotalAmountSuccess = 0;
                //}
                //if (lstDataBankCash.Exists(x => x.Type == 1 && x.PartnerCode == pacode))
                //{
                //    var data = lstDataBankCash.FirstOrDefault(x => x.Type == 1 && x.PartnerCode == pacode);
                //    item.TotalFeeCash = data.TotalFee;
                //    item.TotalTransCash = data.TotalTrans;
                //    item.TotalTransSuccessCash = data.TotalTransSuccess;
                //    item.TotalAmountSuccessCash = data.TotalAmountSuccess;
                //}
                //else
                //{
                //    item.TotalFeeCash = 0;
                //    item.TotalTransCash = 0;
                //    item.TotalTransSuccessCash = 0;
                //    item.TotalAmountSuccessCash = 0;
                //}
                lstData.Add(item2);
            }
            //lstData.Add(item);
        }


        lstReportDoiSoat1 = lstData;
        ExportToExcel(lstReportDoiSoat1, "report-" + begintime.ToString("ddMMyyy"));

    }
    protected void ExportToExcel(List<BankDashboardReport> data, string name)
    {
        Response.Clear();
        Response.Buffer = true;
        //Response.Charset = "UTF-8"; 
        Response.AppendHeader("Content-Disposition", "attachment;filename=" + name + ".xls");
        Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
        Response.ContentType = "application/ms-excel";
        EnableViewState = false;
        var myCItrad = new CultureInfo("VI-VN", true);
        var oStringWriter = new StringWriter(myCItrad);
        var oHtmlTextWriter = new HtmlTextWriter(oStringWriter);


        var grid = new DataGrid { DataSource = data };
        grid.DataBind();
        grid.RenderControl(oHtmlTextWriter);

        Response.Write(oStringWriter.ToString());
        Response.Flush();
        Response.End();
    }
    public float GetPerCent(object TotalTrans, object TotalTranSucess)
    {
        var totalTrans = long.Parse(TotalTrans.ToString());
        var totalTranSucess = long.Parse(TotalTranSucess.ToString());
        if (totalTrans > 0)
            return (totalTranSucess * 100 / totalTrans);

        return 0;
    }
    public string GetInOut(object TotalTrans, object TotalTranSucess, object deduct, object topup)
    {
        var totalTrans = long.Parse(TotalTrans.ToString());
        var totalTranSucess = long.Parse(TotalTranSucess.ToString());

        var totaldeduct = long.Parse(deduct.ToString());
        var totaltopup = long.Parse(topup.ToString());
        return (totalTrans - totalTranSucess - totaldeduct - totaltopup).ToString("#,#").Replace(".", ",");
    }
    public string GetInOut2(object TotalTrans, object TotalTranSucess, object deduct, object topup, object totalfee, object totaloutfee)
    {
        var totalTrans = long.Parse(TotalTrans.ToString());
        var totalTranSucess = long.Parse(TotalTranSucess.ToString());

        var totaldeduct = long.Parse(deduct.ToString());
        var totaltopup = long.Parse(topup.ToString());
        //var totaldeduct = long.Parse(totalfee.ToString());
        //var totaltopup = long.Parse(totaloutfee.ToString());
        return (totalTrans - totalTranSucess - totaldeduct + totaltopup - long.Parse(totalfee.ToString()) - long.Parse(totaloutfee.ToString())).ToString("#,#").Replace(".", ",");
    }
    private void GetList(string partnerIDs, string provider, string cardType)
    {
        //var partnerIDs = ""; 
        //var provider = "";
        //var cardType = "";

        try
        {
            if (!AppUtils.IsAdmin)
            {
                var lstPartner = new Partners().GetListByUserId(AppUtils.UserID);
                if (lstPartner != null && lstPartner.Count > 0)
                    partnerIDs = string.Join(",", lstPartner.Select(e => e.PartnerID.ToString()).ToArray());
                else
                    partnerIDs = "-1";
            }

            string providerCodes = string.Empty;
            if (!AppUtils.IsAdmin)
            {
                if (string.IsNullOrEmpty(providerCodes))
                {
                    var lstProvider = new Providers().GetListByUserId(AppUtils.UserID);
                    if (lstProvider != null && lstProvider.Count > 0)
                    {
                        providerCodes = string.Join(",", lstProvider.Select(e => e.ProviderCode).ToArray());
                    }
                }

            }

            //if (!AppUtils.IsTopup)
            //{
            Dashboard dsDashboard = new Dashboard();
            var lineData = dsDashboard.Report3DayLineChart(partnerIDs, providerCodes, cardType);
            graphLineData = serializer.Serialize(lineData);
            //var pieData = dsDashboard.ReportPieChart(partnerIDs, cardType, providerCodes);
            var pieData = new List<GraphPieData>();
            pieData.Add(
                new GraphPieData
                {
                    value = "100",
                    color = "#e12d2c",
                    highlight = "#e12d2c",
                    label = "acb",
                    cssClass = "text-gate",
                    totalTran = "100"

                }
                );
            graphPieData = serializer.Serialize(pieData);
            //NLogLogger.Info(new string[] { "graphPieData", partnerIDs, providerCodes, cardType, graphPieData });
            rptListProduct.DataSource = pieData;
            rptListProduct.DataBind();
            rptListProductTrans.DataSource = pieData;
            rptListProductTrans.DataBind();

            long totalThisMonth = 0;
            long totalLastMonth = 0;
            int totalProvider = 0;
            int totalPartner = 0;

            //NLogLogger.Info(new string[] { "Statis", partnerIDs, providerCodes });
            dsDashboard.ReportDashboardStatic(cardType, int.Parse(drpMonth.SelectedValue), partnerIDs, providerCodes, ref totalThisMonth, ref totalLastMonth, ref totalProvider, ref totalPartner);

            //totalMonthAmount = totalThisMonth;
            totalMonthAmount = Libs.Utils.AbbrevationUtility.AbbreviateNumber(totalThisMonth);
            monthRate = (((float)totalThisMonth / (float)totalLastMonth)) * 100;
            providerCount = totalProvider;
            partnerCount = totalPartner;
            //}
        }
        catch (Exception ex)
        {
            NLogLogger.Info(ex.Message);
            Response.Redirect(Constant.ADMIN_PATH + "500.html");
        }

    }

    protected void drpMonth_SelectedIndexChanged(object sender, EventArgs e)
    {
        var partnerIDs = drpPartner.SelectedValue;
        string providerCodes = string.Empty;
        var cardType = drpCardType.SelectedValue;
        GetList(partnerIDs, providerCodes, cardType);


    }

    protected void drpPartner_SelectedIndexChanged(object sender, EventArgs e)
    {
        var partnerIDs = drpPartner.SelectedValue;
        string providerCodes = string.Empty;
        var cardType = drpCardType.SelectedValue;
        GetList(partnerIDs, providerCodes, cardType);

        //NLogLogger.Info(new string[] { "Default", "SelectedIndexChanged", partnerIDs, providerCodes, cardType });

        //Dashboard dsDashboard = new Dashboard();
        //var lineData = dsDashboard.Report3DayLineChart(partnerIDs, providerCodes, cardType);
        //graphLineData = serializer.Serialize(lineData);
        //var pieData = dsDashboard.ReportPieChart(partnerIDs, cardType, providerCodes);
        //graphPieData = serializer.Serialize(pieData);
        //rptListProduct.DataSource = pieData;
        //rptListProduct.DataBind();
        //rptListProductTrans.DataSource = pieData;
        //rptListProductTrans.DataBind();

        //long totalThisMonth = 0;
        //long totalLastMonth = 0;
        //int totalProvider = 0;
        //int totalPartner = 0;

        //NLogLogger.Info(new string[] { "Statis", partnerIDs, providerCodes });
        //dsDashboard.ReportDashboardStatic(partnerIDs, providerCodes, ref totalThisMonth, ref totalLastMonth, ref totalProvider, ref totalPartner);

        //totalMonthAmount = totalThisMonth;
        //monthRate = (((float)totalThisMonth / (float)totalLastMonth)) * 100;
        //providerCount = totalProvider;
        //partnerCount = totalPartner;
    }

    protected void drpCardType_SelectedIndexChanged(object sender, EventArgs e)
    {
        var partnerIDs = drpPartner.SelectedValue;
        string providerCodes = string.Empty;
        var cardType = drpCardType.SelectedValue;
        GetList(partnerIDs, providerCodes, cardType);
    }
    public string GetLang()
    {
        return Libs.Utils.GlobalHelper.GetLanguage();
    }
    //private void GetListBankLog()
    //{

    //    DateTime requestTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddSeconds(-1);

    //    DateTime fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-10);
    //    BankGateAPI _BankGateAPI = new BankGateAPI();
    //    rpBanklog.DataSource = _BankGateAPI.GetTable(20, AppUtils.UserName, string.Empty, fromDate, requestTime, null, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, 1);
    //    rpBanklog.DataBind();
    //}
    //private void Order(string partnerCode, string partnerKey)
    //{

    //    if (string.IsNullOrEmpty(txtAmount.Text))
    //        return;
    //    var requestContent = new OrderRequest()
    //    {

    //        Amount = int.Parse(txtAmount.Text),
    //        CallbackUrl = "",
    //        RefCode = DateTime.Now.ToString("MMddHHmmss"),
    //        BankCode = "random",
    //        PartnerCode = partnerCode,
    //    };
    //    var signature = Encrypts.MD5(partnerCode + "random" + requestContent.Amount + requestContent.RefCode + requestContent.CallbackUrl + partnerKey);
    //    requestContent.Signature = signature;
    //    NLogLogger.Info(new string[] { "Bank Test", "Requst Core", urlService, serializer.Serialize(requestContent) });
    //    var serviceResponse = PostJson(urlService, serializer.Serialize(requestContent));
    //    NLogLogger.Info(new string[] { "Bank Test", "Response Core", serviceResponse });
    //    var resObj = serializer.Deserialize<APIResponse>(serviceResponse);
    //    if (resObj.ResponseCode > 0)
    //    {
    //        var orderResponse = serializer.Deserialize<OrderResponse>(resObj.ResponseContent);
    //        dvBankInfo.Visible = true;
    //        //lbAccountName.Text = orderResponse.BankAccountName;
    //        lbAccountNumber.Text = orderResponse.BankName + " - " + orderResponse.BankAccountNumber + " - " + orderResponse.BankAccountName;
    //        lbOrderNo.Text = orderResponse.OrderNo;
    //        qrCode.Width = 300;
    //        qrCode.ImageUrl = orderResponse.QRCode;
    //        GetListBankLog();
    //    }
    //    else
    //    {

    //    }


    //    //Response.Redirect(orderResponse.Url);
    //}
    private string PostJson(string uri, string postData)
    {
        var request = (HttpWebRequest)WebRequest.Create(uri);
        request.ContentType = "application/json";
        request.Method = "POST";//GET
                                //request.Accept = "JSON";
        using (Stream requestStream = request.GetRequestStream())
        {
            byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
            requestStream.Write(postDatabytes, 0, postDatabytes.Length);
        }
        var webResponse = request.GetResponse();
        if (webResponse == null)
        {
            return "Unable to connect to the remote server";
        }
        var sr = new StreamReader(webResponse.GetResponseStream());
        return sr.ReadToEnd().Trim();
    }
    public class APIResponse
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string ResponseContent { get; set; }
        public string Signature { get; set; }
    }

    class OrderResponse
    {
        public string QRCode { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        //public int Amount { get; set; }
        public string OrderNo { get; set; }
        //public int Timeout { get; set; }
    }
    public class OrderRequest
    {
        public string Type { get; set; }
        public string Signature { get; set; }
        public string BankCode { get; set; }
        public string PartnerCode { get; set; }
        //public string BankAccountName { get; set; }
        //public string AppCode { get; set; }
        public string RefCode { get; set; }
        public int Amount { get; set; }
        public string CallbackUrl { get; set; }
    }
    public string GetStatusExtra(object statusOver)
    {

        if (statusOver.ToString() == "1")
        {
            return "<div class=\"label label-success\">" + Resources.Pay.Success + "</div>";
        }
        if (statusOver.ToString() == "-1")
        {
            return "<div class=\"label label-danger\">" + Resources.Pay.Fail + "</div>";
        }
        if (statusOver.ToString() == "0")
        {
            return "<div class=\"label label-info\">" + Resources.Pay.Processing + "</div>";
        }



        return "<div class=\"label label-info\">" + statusOver + "</div>";
    }
    public string GetInOut3(object TotalTrans, object TotalTranSucess)
    {
        var totalTrans = long.Parse(TotalTrans.ToString());
        var totalTranSucess = long.Parse(TotalTranSucess.ToString());

        if (totalTrans == 0 && totalTranSucess == 0)
            return "0";
        return (totalTrans + totalTranSucess).ToString("#,#").Replace(".", ",");
    }

}