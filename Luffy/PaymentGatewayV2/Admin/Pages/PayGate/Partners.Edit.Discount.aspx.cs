using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Libs.API;
using Libs.Report;
using Libs.Utils;
using System.Data;

public partial class Pages_PayGate_Partners_Edit_Discount : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.PartnersEditDiscount);
        if (!IsPostBack)
        {
            init();
        }
    }

    private void init()
    {
        Partners _Partner = new Partners() { PartnerID = Convert.ToInt32(AppUtils.Request("id")) };
        _Partner = _Partner.Get();
        lblPartner.Text = _Partner.PartnerCode;
        lblPartnerId.Text = _Partner.PartnerID.ToString();
        // Discount
        datepicker.Text = DateTime.Now.ToString("MM-yyyy");
        var month = DateTime.Now.Month;
        var year = DateTime.Now.Year;
        var _partnerDiscount = new PartnersDiscount().GetTablePartnersDiscount(_Partner.PartnerCode, year, month);
        rptDiscount.DataSource = _partnerDiscount;
        rptDiscount.DataBind();
    }

    protected List<Providers> ListProviders(object serviceId)
    {
        return new Providers().GetList(Convert.ToInt32(serviceId));
    }

    protected void btUpdate_Click(object sender, EventArgs e)
    {
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.PartnersList);
    }
    protected void btnDiscountView_Click(object sender, EventArgs e)
    {

        var month = Convert.ToInt32(datepicker.Text.Split('-')[0]);
        var year = Convert.ToInt32(datepicker.Text.Split('-')[1]);
        var _partnerDiscount = new PartnersDiscount().GetTablePartnersDiscount(lblPartner.Text.ToLower(), year, month);
        rptDiscount.DataSource = _partnerDiscount;
        rptDiscount.DataBind();

    }
    protected void btnUploadExcel_Click(object sender, EventArgs e)
    {
        try
        {
            if (fileUploadExcel.HasFile)
            {
                string fileN = DateTime.Now.ToString("dd_MM_yyyy_hhmmss") + "_" + AppUtils.UserName + "_" + fileUploadExcel.FileName;
                FileInfo fi = new FileInfo(fileN);
                string ext = fi.Extension;
                if (ext == ".xlsx")
                {
                    string pathFile = Server.MapPath("~/Excels/Discount/" + fileN);
                    fileUploadExcel.SaveAs(pathFile);
                    string mes = string.Empty;
                    var tempDataTable = GetAndSaveDataFromExcel(pathFile, true, ref mes);
                    if (tempDataTable)
                    {
                        AlertSuccesss.Text = "Import thành công!";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertSuccess').modal()}); ", true);
                    }
                    else
                    {
                        AlertBans.Text = "Lỗi Import thao tác CSDL thất bại!</br>Lỗi: " + mes;
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertBan').modal()}); ", true);
                    }
                }
                else
                {
                    AlertBans.Text = "Lỗi: File không đúng định dạng (định dạng bắt buộc là .xlsx)";
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertBan').modal()}); ", true);
                }
            }
        }
        catch (Exception ex)
        {
            NLogLogger.Info(new string[] { "Partners.Edit.Discount", "btnUploadExcel_Click", "Error", ex.Message });
            AlertBans.Text = "Lỗi: " + ex.Message;
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertBan').modal()}); ", true);
        }
    }
    protected void btnUpdateDiscount_Click(object sender, EventArgs e)
    {

    }

    public bool GetAndSaveDataFromExcel(string path, bool hasHeader, ref string mes)
    {
        var sheetName = string.Empty;
        var columnName = string.Empty;

        try
        {
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }


                foreach (var ws in pck.Workbook.Worksheets)
                {

                    sheetName = ws.Name;
                    DataTable tbl = new DataTable();
                    foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                    {
                        tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                    }
                    var startRow = hasHeader ? 2 : 1;
                    for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        DataRow row = tbl.Rows.Add();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Text;
                        }
                    }

                    foreach (DataRow row in tbl.Rows)
                    {
                        var obj = new PartnersDiscount();
                        obj.PartnerCode = row.ItemArray[0].ToString();
                        obj.Date = Convert.ToDateTime(row.ItemArray[1]);
                        obj.DiscountVTT = Convert.ToDecimal(row.ItemArray[2].ToString());
                        obj.RewardVTT = 0;
                        obj.DiscountVNP = Convert.ToDecimal(row.ItemArray[3].ToString());
                        obj.RewardVNP = 0;
                        obj.DiscountVMS = Convert.ToDecimal(row.ItemArray[4].ToString());
                        obj.RewardVMS = 0;
                        obj.DiscountGATE = Convert.ToDecimal(row.ItemArray[5].ToString());
                        obj.RewardGATE = 0;
                        obj.DiscountZING = Convert.ToDecimal(row.ItemArray[6].ToString());
                        obj.RewardZING = 0;
                        obj.DiscountBANKTRANFER = Convert.ToDecimal(row.ItemArray[7].ToString());
                        obj.RewardBANKTRANFER = 0;
                        obj.DiscountMOMO = Convert.ToDecimal(row.ItemArray[8].ToString());
                        obj.RewardMOMO = 0;
                        obj.DiscountVTTOUT = 0;
                        obj.RewardVTTOUT = 0;
                        obj.DiscountBANKOUTTRANFER = Convert.ToDecimal(row.ItemArray[9].ToString());
                        obj.RewardBANKOUTTRANFER = 0;
                        obj.DiscountMOMOOUT = Convert.ToDecimal(row.ItemArray[10].ToString());
                        obj.RewardMOMOOUT = 0;

                        obj.RewardVNPOUT = obj.RewardVTTOUT;
                        obj.DiscountVNPOUT = obj.DiscountVTTOUT;
                        obj.RewardVMSOUT = obj.RewardVTTOUT;
                        obj.DiscountVMSOUT = obj.DiscountVTTOUT;
                        obj.Add();
                    }

                }

            }
        }
        catch (Exception ex)
        {
            NLogLogger.Info(new string[] { "Partners.Edit.Discount", "btnUploadExcel_Click", "Error", sheetName,  ex.Message });
            mes = "tại Sheet " + sheetName + " mô tả: " + ex.Message;
            return false;
        }
       

        return true;
    }
}