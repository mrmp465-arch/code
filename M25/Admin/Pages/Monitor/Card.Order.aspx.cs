using Libs.API;
using Libs.Report;
using Libs.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_Monitor_Card_Order : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.CardOrder);
        if (!IsPostBack)
        {
            //DownloadOld();
            BindData();
            var lst = new Partners().GetList();
            lst = lst.Where(x => x.PartnerCode.Contains("card")).ToList();
            drpPartner.DataSource = lst;
            drpPartner.DataTextField = "Name";
            drpPartner.DataValueField = "PartnerCode";
            drpPartner.DataBind();
            drpPartner.Items.Insert(0, new ListItem("Đối tác:", ""));
        }
    }
    private void DownloadOld()
    {
        BuyCard _BuyCard = new BuyCard();
        var data = _BuyCard.GetList(30, "123", DateTime.Now, 1, "", string.Empty);
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        var lstData = new List<CardSeri>();
        foreach (var item in data)
        {
            if (item.TransactionID >= 87864153 && item.TransactionID <= 87868066)
            {
                var lstcard = serializer.Deserialize<List<CardSeri>>(item.ListCards);
                foreach (var card in lstcard)
                {
                    lstData.Add(
                        new CardSeri
                        {
                            Telco = item.Provider,
                            OrderNo = item.OrderNo,
                            Pin = card.Pin,
                            Serial = card.Serial,
                            Amount = item.Amount.ToString()
                        }
                        );
                        ;
                }
            }
        }

        try
        {
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheetTotal = xlPackage.Workbook.Worksheets.Add("Total");
                    worksheetTotal.Cells["A1"].Value = "OrderNo";
                    worksheetTotal.Cells["B1"].Value = "Loại thẻ";
                    worksheetTotal.Cells["C1"].Value = "Mệnh giá";
                    worksheetTotal.Cells["D1"].Value = "Seri";
                    worksheetTotal.Cells["E1"].Value = "Pin";
                    //worksheetTotal.Cells["F1"].Value = "Pin";
                    
                    worksheetTotal.Column(1).Width = 15;
                    worksheetTotal.Column(2).Width = 15;
                    worksheetTotal.Column(3).Width = 15;
                    worksheetTotal.Column(4).Width = 15;
                    worksheetTotal.Column(5).Width = 15;
                    worksheetTotal.Column(6).Width = 15;
                    worksheetTotal.Row(1).Style.Font.Bold = true;
                    int rowTotal = 1;
                   
                    foreach (var card in lstData)
                    {

                        rowTotal++;
                        worksheetTotal.Cells["A" + rowTotal].Value = card.OrderNo;
                        worksheetTotal.Cells["B" + rowTotal].Value = card.Telco; ;
                        worksheetTotal.Cells["C" + rowTotal].Value = card.Amount;
                        worksheetTotal.Cells["D" + rowTotal].Value = card.Serial;
                        worksheetTotal.Cells["E" + rowTotal].Value = card.Pin;
                    }



                    xlPackage.Save();
                }
                bytes = stream.ToArray();
            }
            Response.AddHeader("Content-disposition", "attachment; filename=OrderNo.xlsx");
            Response.ContentType = "text/xls";
            Response.BinaryWrite(bytes);
            Response.End();
            //return File(bytes, "text/xls", order.OrderNo+".xlsx");
        }
        catch (Exception ex)
        {

            //return RedirectToAction("Parking");
        }
    }

    protected void BindData()
    {

        DataView dv1 = new CardOrder().GetTop(100).DefaultView;
        dv1.Sort = "Id desc";
        DataTable sortedDT1 = dv1.ToTable();
        rptList1.DataSource = sortedDT1;
        rptList1.DataBind();
    }
    protected void Delete_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        int Id = Convert.ToInt32(e.CommandArgument.ToString());
        var order = new CardOrder { Id = Id };
        order.Delete();
        BindData();
    }
    protected void Confirm_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        int Id = Convert.ToInt32(e.CommandArgument.ToString());
        var order = new CardOrder { Id = Id, Status = 1 };
        order.Update();
        BindData();
    }
    protected void Download_Command(Object sender, CommandEventArgs e)
    {
        //Provider.GoBillingLogDataAccess.DeleteTopupEventLog(Convert.ToInt64(e.CommandArgument.ToString()));
        int Id = Convert.ToInt32(e.CommandArgument.ToString());
        var order = new CardOrder().Get(Id);
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        var lstPacket = new CardOrderPacket().GetList(Id);
        if (lstPacket != null)
        {
            lstPacket = lstPacket.Where(x => x.Status == 1).ToList();
            if (lstPacket != null)
            {
                try
                {
                    byte[] bytes;
                    using (var stream = new MemoryStream())
                    {
                        using (var xlPackage = new ExcelPackage(stream))
                        {
                            var worksheetTotal = xlPackage.Workbook.Worksheets.Add("Total");
                            worksheetTotal.Cells["A1"].Value = "Loại thẻ";
                            worksheetTotal.Cells["B1"].Value = "Số lượng";
                            worksheetTotal.Cells["C1"].Value = "Mệnh giá";
                            worksheetTotal.Cells["D1"].Value = "Tổng";
                            worksheetTotal.Cells["E1"].Value = "Chiết khấu";
                            worksheetTotal.Cells["F1"].Value = "Phải thu";
                            worksheetTotal.Column(1).Width = 15;
                            worksheetTotal.Column(2).Width = 15;
                            worksheetTotal.Column(3).Width = 15;
                            worksheetTotal.Column(4).Width = 15;
                            worksheetTotal.Column(5).Width = 15;
                            worksheetTotal.Column(6).Width = 15;
                            worksheetTotal.Row(1).Style.Font.Bold = true;
                            int rowTotal = 1;
                            foreach (var item in lstPacket)
                            {
                                rowTotal++;
                                worksheetTotal.Cells["A" + rowTotal].Value = string.Format("{0}{1}", item.CardType, item.CardValue / 1000);
                                worksheetTotal.Cells["B" + rowTotal].Value = item.NumberCard;
                                worksheetTotal.Cells["C" + rowTotal].Value = item.CardValue;
                                worksheetTotal.Cells["C" + rowTotal].Style.Numberformat.Format = "#,##0";
                                worksheetTotal.Cells["D" + rowTotal].Value = item.CardValue * item.NumberCard;
                                worksheetTotal.Cells["D" + rowTotal].Style.Numberformat.Format = "#,##0";
                            }
                            foreach (var item in lstPacket)
                            {
                                var worksheet = xlPackage.Workbook.Worksheets.Add(string.Format("{0}{1}K({2})", item.CardType, item.CardValue / 1000, item.NumberCard));
                                worksheet.Column(1).Width = 15;
                                worksheet.Column(2).Width = 15;
                                worksheet.Column(3).Width = 15;
                                worksheet.Column(4).Width = 15;


                                worksheet.Cells["A1"].Value = "CardType";
                                worksheet.Cells["B1"].Value = "Amount";
                                worksheet.Cells["C1"].Value = "Serial";
                                worksheet.Cells["D1"].Value = "Pin";


                                var lstcard = serializer.Deserialize<List<CardSeri>>(item.Data);
                                int row = 1;
                                foreach (var card in lstcard)
                                {

                                    row++;
                                    worksheet.Cells["A" + row].Value = item.CardType.ToLower();
                                    worksheet.Cells["B" + row].Value = item.CardValue; ;
                                    worksheet.Cells["C" + row].Value = card.Serial;
                                    worksheet.Cells["D" + row].Value = card.Pin;
                                }
                            }



                            xlPackage.Save();
                        }
                        bytes = stream.ToArray();
                    }
                    Response.AddHeader("Content-disposition", "attachment; filename=" + order.OrderNo + ".xlsx");
                    Response.ContentType = "text/xls";
                    Response.BinaryWrite(bytes);
                    Response.End();
                    //return File(bytes, "text/xls", order.OrderNo+".xlsx");
                }
                catch (Exception ex)
                {

                    //return RedirectToAction("Parking");
                }
            }
        }
    }
    protected string DetailUrl(string id)
    {
        return Constant.ADMIN_PATH + "pages/monitor/card.orderdetail.aspx/?id=" + id;
    }
    public string GetStatus(object str)
    {
        if (str.ToString() == "1")
            return "Hoàn thành";

        return "Chưa hoàn thành";
    }

    protected void btnCreate_Click(object sender, EventArgs e)
    {
        //validate
        if (string.IsNullOrEmpty(drpPartner.SelectedValue))
        {
            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", "<script> alert('Vui lòng chọn đối tác')</script>");
            return;
        }
        if (string.IsNullOrEmpty(txtOrderNo.Text))
        {
            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", "<script> alert('Vui lòng nhập mã orderno')</script>");
            return;
        }
        var order = new CardOrder { OrderNo = txtOrderNo.Text, PartnerCode = drpPartner.SelectedValue, Status = 0 };
        order.Add();
        txtOrderNo.Text = "";
        BindData();
    }
    public class CardSeri
    {
        public string Serial { get; set; }
        public string Amount { get; set; }
        public string Telco { get; set; }
        public string OrderNo { get; set; }
        public string Pin { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}