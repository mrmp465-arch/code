using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Report;
using Libs.Utils;

public partial class Pages_Momo_Account_Add : System.Web.UI.Page
{
    private string urlBaseService = "http://127.0.0.1:9002/MomoService.ashx";
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

        var _user = new Users().Get(AppUtils.UserID);
        _Momo.Source = _user.Source;
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
        var _user = new Users().Get(AppUtils.UserID);
        

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
                                obj.Source = _user.Source;
                                if (obj.MomoId.Length == 9)
                                    obj.MomoId = "0" + obj.MomoId;
                                obj.Add();
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
    protected void btnUpload2_Click(object sender, EventArgs e)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        RijndaelEnhanced rijndaelKey = new RijndaelEnhanced("pay", "@1B2c3D4e5F6g7H8");
        var _user = new Users().Get(AppUtils.UserID);
        try
        {
            if (FileUpload1.HasFile)
            {
                int i = 0;
                string fileN = DateTime.Now.ToString("dd_MM_yyyy_hhmmss") + "_" + AppUtils.UserName + "_" + FileUpload1.FileName;
                FileInfo fi = new FileInfo(fileN);
                string ext = fi.Extension;
                if (ext == ".xlsx")
                {
                    string pathFile = Server.MapPath("~/Excels/NapHo/" + fileN);
                    FileUpload1.SaveAs(pathFile);
                    var tempDataTable = GetDataTableFromExcel(pathFile, true);
                    var totalRaw = tempDataTable.Rows.Count;
                    var successRaw = 0;
                    var stepRaw = 0;
                    var errorRow = 0;
                    foreach (DataRow row in tempDataTable.Rows)
                    {

                        // NLogLogger.Info(new string[] { "Upload", serializer.Serialize(row.ItemArray)});
                        try
                        {

                            stepRaw++;
                            if (!string.IsNullOrEmpty(row.ItemArray[1].ToString().Trim()) &&
                                !string.IsNullOrEmpty(row.ItemArray[0].ToString().Trim('\'').Trim('?').Trim()) &&
                                !string.IsNullOrEmpty(row.ItemArray[2].ToString().Trim())
                               )
                            {
                                var obj = new AccountImei();

                                obj.Phone = row.ItemArray[0].ToString().ToLower().Trim('\'').Trim('?').Trim();
                                obj.Password = row.ItemArray[1].ToString().Trim();
                                if (obj.Phone.Length == 9)
                                    obj.Phone = "0" + obj.Phone;
                                obj.Imei = row.ItemArray[2].ToString().Trim();
                                //obj.Source = row.ItemArray[3].ToString().Trim();
                                //if (string.IsNullOrEmpty(obj.Source))
                                //{
                                //    obj.Source = "inhouse";
                                //}
                               
                                obj.Source = _user.Source;
                                if(_user.Source=="inhouse")
                                {
                                    obj.Source = row.ItemArray[3].ToString().Trim();
                                    if (string.IsNullOrEmpty(obj.Source))
                                    {
                                        obj.Source = "inhouse";
                                    }
                                }
                                var result = AddMomo(obj);
                                if (result.ResponseCode == 1)
                                {
                                    successRaw++;
                                }
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

                        Thread.Sleep(50);
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
    private APIResponse AddMomo(AccountImei momo)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        var requesData = new RequestData()
        {
            CommandCode = "ACCOUNT_IMEI_ADD",
            RequestContent = serializer.Serialize(momo)
        };
        //var UrlBaseService = "http://127.0.0.1:9002/MomoService.ashx";
        var res = Task.Run(async () => await CallbackJson(urlBaseService, serializer.Serialize(requesData))).Result;
        var resObj = serializer.Deserialize<APIResponse>(res);
        if (resObj != null)
        {
            return resObj;
        }
        return new APIResponse((int)ResponseCode.TransactionFailed);
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
    public static async Task<string> CallbackJson(string url, string postData)
    {
        var uri = new Uri(url);
        var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(30);
        try
        {
            NLogLogger.Info(new string[] { "CMS", "Account.Add", "Request", postData });
            var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                NLogLogger.Info(new string[] { "CMS", "Account.Add", "Response", responseContent });
                client.Dispose();
                return responseContent;
            }
            else
            {
                NLogLogger.Info(new string[] { "CMS", "Account.Add", "Response Is Null" });
            }

        }
        catch (Exception e)
        {
            NLogLogger.Info(new string[] { "CMS", "Exeption Post", e.Message });
            return string.Empty;
        }
        client.Dispose();
        return string.Empty;
    }
    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.MomoAccount);
    }
    public class AccountImei
    {
        public string Phone { get; set; }
        public string Imei { get; set; }
        public string Password { get; set; }
        public string Source { get; set; }
    }
    public class RequestData
    {
        public string PartnerCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public string Signature { get; set; }
    }
}