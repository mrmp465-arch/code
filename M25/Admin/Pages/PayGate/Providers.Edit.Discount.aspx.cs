using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using System.Data;
using System.IO;
using Libs.Report;
using Libs.Utils;

public partial class Pages_PayGate_Provider_Edit_Discount : System.Web.UI.Page
{


    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.ProviderEditDiscount);
        if (!IsPostBack)
        {
            init();
            //      txtType.SelectedValue = Type = Request["type"];
        }
    }

    private void init()
    {

        //var providerId = Convert.ToInt32(AppUtils.Request("id"));
        //var _Provider = new Providers().Get(providerId);

        var providerCode = AppUtils.RequestCode("code");
        var _Provider = new Providers().Get(providerCode);

        if (_Provider == null)
        {
            Response.Redirect(Resources.Url.ProviderList);
        }
        lblProviderCode.Text = _Provider.ProviderCode;
        lblProviderId.Text = _Provider.ProviderId.ToString();

        // Discount
        datepicker.Text = DateTime.Now.ToString("MM-yyyy");
        var month = DateTime.Now.Month;
        var year = DateTime.Now.Year;
        var _partnerDiscount = new ProvidersDiscount().GetTableProvidersDiscount(_Provider.ProviderCode, year, month);
        rptDiscount.DataSource = _partnerDiscount;
        rptDiscount.DataBind();
    }

    protected void btnDiscountView_Click(object sender, EventArgs e)
    {

        var month = Convert.ToInt32(datepicker.Text.Split('-')[0]);
        var year = Convert.ToInt32(datepicker.Text.Split('-')[1]);
        var _partnerDiscount = new ProvidersDiscount().GetTableProvidersDiscount(lblProviderCode.Text.ToLower(), year, month);
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
                        AlertSuccesss.Text = "Import thành công !";
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
        var rowCount = 0;

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
                        rowCount++;
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        DataRow row = tbl.Rows.Add();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Text;
                        }
                    }

                    foreach (DataRow row in tbl.Rows)
                    {
                        //NLogLogger.Info(new string[] { "Partners.Edit.Discount", "Add", "Success", row.ItemArray[1].ToString() });
                        if (!string.IsNullOrEmpty(row.ItemArray[1].ToString()))
                        {
                            var obj = new ProvidersDiscount();
                            obj.ProviderCode = row.ItemArray[0].ToString();
                            obj.Date = Convert.ToDateTime(row.ItemArray[1]);
                            obj.Discount = Convert.ToDecimal(row.ItemArray[2].ToString());
                            obj.Reward = Convert.ToDecimal(row.ItemArray[3].ToString());
                            obj.Add();
                        }


                    }

                    rowCount = 0;

                }

            }
        }
        catch (Exception ex)
        {
            NLogLogger.Info(new string[] { "Providers.Edit.Discount", "btnUploadExcel_Click", "Error", sheetName, rowCount.ToString(), ex.Message });
            mes = "tại Sheet " + sheetName + " mô tả: " + ex.Message;
            return false;
        }


        return true;
    }
}