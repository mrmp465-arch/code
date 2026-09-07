using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;
using Libs.Utils;

public partial class Pages_Momo_Account_Add : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.MomoAccountAdd);
    }

    protected void btAdd_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtMomoId.Text))
            return;
        RijndaelEnhanced rijndaelKey = new RijndaelEnhanced("pay", "@1B2c3D4e5F6g7H8");

        var _Momo = new MomoAccounts();
        _Momo.MomoId = txtMomoId.Text;
        _Momo.MomoName = txtMomoName.Text.Trim();
        _Momo.Status = Convert.ToInt32(chkIsActive.Checked);
        _Momo.Type = drpType.SelectedValue;
        _Momo.MomoPass = rijndaelKey.Encrypt(txtMomoPass.Text.Trim());
        _Momo.BalanceMaxDay = Convert.ToInt32(txtBalanceMaxDay.Text);
        _Momo.BalanceMaxMonth = Convert.ToInt32(txtBalanceMaxMonth.Text);
        _Momo.Solution = drpSolution.SelectedValue;
        _Momo.Source = AppUtils.UserName;
        var result=_Momo.Add();
        if (result > 0)
        {
            //Log User
            var _userLog = new UserLog
            {
                UserName = AppUtils.UserName,
                Action = "momoadd",
                ActionName = "Thêm mới momo",
                Description = "Thêm mới momo " + _Momo.MomoId
            };
            _userLog.Add();

            //phân bổ
            var _PartnerBank = new PartnerMomo();
            _PartnerBank.PartnerId = 1;
            _PartnerBank.MomoId = result;
            _PartnerBank.Status = 1;
            _PartnerBank.OrderNo = 1;
            _PartnerBank.Add();
            Response.Redirect(Constant.ADMIN_PATH + Resources.Url.MomoAccount);
        }
        else
        {
            AlertBans.Text = "Có lỗi trong quá trình xử lý";
            if (result == -319)
                AlertBans.Text = "Tài khoản đã tồn tại";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertBan').modal()}); ", true);
        }
       

    }
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        RijndaelEnhanced rijndaelKey = new RijndaelEnhanced("pay", "@1B2c3D4e5F6g7H8");

        try
        {
            if (FileUploadExcel.HasFile)
            {
                int i = 0;
                string fileN = DateTime.Now.ToString("dd_MM_yyyy_hhmmss") + "_" + AppUtils.UserName + "_" + FileUploadExcel.FileName;
                FileInfo fi = new FileInfo(fileN);
                string ext = fi.Extension;
                if (ext == ".xlsx")
                {
                    string pathFile = Server.MapPath("~/Excels/NapHo/" + fileN);
                    FileUploadExcel.SaveAs(pathFile);
                    var tempDataTable = GetDataTableFromExcel(pathFile, true);
                    var totalRaw = tempDataTable.Rows.Count;
                    var successRaw = 0;
                    var stepRaw = 0;
                    var errorRow = 0;
                    foreach (DataRow row in tempDataTable.Rows)
                    {

                        //NLogLogger.Info(new string[] { "Upload", serializer.Serialize(row.ItemArray)});
                        try
                        {

                            stepRaw++;
                            if (!string.IsNullOrEmpty(row.ItemArray[2].ToString().Trim()) &&
                                !string.IsNullOrEmpty(row.ItemArray[0].ToString().Trim('\'').Trim('?').Trim()) &&
                                !string.IsNullOrEmpty(row.ItemArray[3].ToString().Trim())
                               )
                            {
                                var obj = new MomoAccounts();

                                obj.MomoId = row.ItemArray[0].ToString().ToLower().Trim('\'').Trim('?').Trim();
                                obj.MomoName = row.ItemArray[1].ToString().Trim();
                                obj.Status = 0;
                                obj.Type = "INOUT";
                                obj.Solution = "APIV3";
                                obj.MomoPass = rijndaelKey.Encrypt(row.ItemArray[2].ToString().Trim());
                                obj.BalanceMaxDay = int.Parse(row.ItemArray[3].ToString().Trim().ToLower());
                                obj.BalanceMaxMonth = int.Parse(row.ItemArray[4].ToString().Trim());
                                obj.Source = AppUtils.UserName;

                                if (obj.MomoId.Length == 9)
                                    obj.MomoId = "0" + obj.MomoId;
                                var result = obj.Add();

                                //phân bổ
                                var _PartnerBank = new PartnerMomo();
                                _PartnerBank.PartnerId = 1;
                                _PartnerBank.MomoId = result;
                                _PartnerBank.Status = 1;
                                _PartnerBank.OrderNo = 1;
                                _PartnerBank.Add();
                            }
                            else
                            {
                                errorRow++;
                            }



                        }
                        catch (Exception ex)
                        {
                            errorRow++;
                            NLogLogger.Info(new string[] { "Topup.Add", "TopupMobileLog", "Error", ex.Message });
                            break;
                        }
                        successRaw++;
                        Thread.Sleep(5);
                    }


                    if (errorRow != 0)
                    {
                        AlertSuccesss.Text = "Import thành công! <br/>" + successRaw + " tk trên tổng " + totalRaw;

                    }
                    else
                    {
                        AlertSuccesss.Text = "Import thành công <br/>" + successRaw + " tk trên tổng " + totalRaw;
                    }

                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertSuccess').modal()}); ", true);
                }
                else
                {
                    AlertBans.Text = "Lỗi: File không đúng định dạng (định dạng bắt buộc là .xlsx)";
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertBan').modal()}); ", true);
                }
            }
        }
        catch (Exception ex)
        {
            NLogLogger.Info(new string[] { "Topup.Add", "TopupMobileLog", "Error", ex.Message });
            AlertBans.Text = "Lỗi: " + ex.Message;
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#MAlertBan').modal()}); ", true);
        }
    }

    public DataTable GetDataTableFromExcel(string path, bool hasHeader)
    {
        using (var pck = new OfficeOpenXml.ExcelPackage())
        {
            using (var stream = File.OpenRead(path))
            {
                pck.Load(stream);
            }
            var ws = pck.Workbook.Worksheets.First();
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
            return tbl;
        }
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.MomoAccount);
    }
}