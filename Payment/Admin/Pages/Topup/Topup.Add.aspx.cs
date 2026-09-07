using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Libs.API;
using Libs.Report;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using Libs.Utils;
using Org.BouncyCastle.Bcpg.OpenPgp;

public partial class Pages_Topup_Topup_Add : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.TopupAdd);
        if (!IsPostBack)
        {
            //var _Products = new Products();
            //txtTelco.DataSource = _Products.GetList(13,null); 
            //txtTelco.DataBind();
            //txtTelco.DataTextField = "Name";
            //txtTelco.DataValueField = "Code";
            //txtTelco.DataBind(); 
        }
    }

    protected void btAdd_Click(object sender, EventArgs e)
    {
        string Msg = string.Empty;
        string MsgDup = "<br/><br/><b>Cảnh báo trùng lặp (7 ngày)</b><br/>";
        bool validate = true;
        var _TopupMobileLog = new TopupMobileLog();

        //if (!IsValidPhone(txtMobile.Text))
        //{
        //    validate = false;
        //    Msg = "Số điện thoại không đúng định dạng!";
        //}

        if (string.IsNullOrEmpty(txtTelco.SelectedValue) || string.IsNullOrEmpty(txtTopupType.SelectedValue) || string.IsNullOrEmpty(txtAmount.Text) ||
            string.IsNullOrEmpty(txtMobile.Text) || string.IsNullOrEmpty(txtOrderNo.Text) || string.IsNullOrEmpty(txtAmountAll.SelectedValue) //|| string.IsNullOrEmpty(txtFirtAmout.Text)
            || string.IsNullOrEmpty(txtPriority.SelectedValue)
            )
        {
            Msg = "Các trường có * là bắt buộc!";
            validate = false;
        }

        if (validate)
        {
            var partners = string.Empty;
            var providers = string.Empty;
            if (!AppUtils.IsAdmin)
            {
                //partners = string.Join(",", AppUtils.PartnerUser.Select(x => x.PartnerCode).ToArray());
                //providers = string.Join(",", AppUtils.ProviderUser.Select(x => x.ProviderCode).ToArray());
                providers = string.Join(",", new Providers().GetListByUserId(AppUtils.UserID).Select(x => x.ProviderCode).ToArray());
                NLogLogger.Info(new string[] { "Add Order Form", AppUtils.UserName, "Providers", providers });

            }


            _TopupMobileLog.Amount = Convert.ToInt32(txtAmount.Text);
            _TopupMobileLog.Telco = txtTelco.SelectedValue.ToLower();
            _TopupMobileLog.TopupType = Convert.ToInt32(txtTopupType.SelectedValue);
            _TopupMobileLog.Mobile = txtMobile.Text.ToLower().Trim('\'').Trim('?').Trim();
            _TopupMobileLog.FullName = txtFullName.Text;
            _TopupMobileLog.UserId = AppUtils.UserID;
            _TopupMobileLog.UserName = AppUtils.UserName;
            _TopupMobileLog.RequestNo = GenOrderCode();
            _TopupMobileLog.AmountPending = 0;
            _TopupMobileLog.AmountTopupSuccess = 0;
            _TopupMobileLog.LogContent = "";
            _TopupMobileLog.Status = (txtTelco.SelectedValue.ToLower() == "vms" && string.IsNullOrEmpty(txtPassword.Text.Trim())) || (Convert.ToInt32(txtTopupType.SelectedValue) == 16) || (Convert.ToInt32(txtTopupType.SelectedValue) == 17) ? -3 : 1;
            _TopupMobileLog.Partners = partners;
            _TopupMobileLog.Providers = providers;
            _TopupMobileLog.AmountMin = 0; //Convert.ToInt32(txtFirtAmout.SelectedValue);
            _TopupMobileLog.AmountMinAll = Convert.ToInt32(txtAmountAll.SelectedValue);
            _TopupMobileLog.Priority = Convert.ToInt32(txtPriority.SelectedValue);
            _TopupMobileLog.OrderNo = txtOrderNo.Text.Trim();
            _TopupMobileLog.AccountName = txtAccountName.Text.ToLower().Trim();
            _TopupMobileLog.Password = txtPassword.Text.Trim();
            _TopupMobileLog.Ussd = Convert.ToInt32(txtUssd.SelectedValue);
            _TopupMobileLog.Add();
            if (_TopupMobileLog.ReturnValue >= 0)
            {
                Msg = string.Empty;
            }
            else
                Msg = "Lỗi không thêm được Order!";

            var listDup = _TopupMobileLog.GetListDuplicate(txtOrderNo.Text.Trim(), AppUtils.UserID);
            foreach (var dup in listDup)
            {
                MsgDup = MsgDup + " - " + dup.Mobile + " xuất hiện " + dup.Dup + " lần</br>";
            }


        }

        if (string.IsNullOrEmpty(Msg))
        {
            AlertSuccesss.Text = "Thêm thành công! <br/> Click <a href=" + Constant.ADMIN_PATH +
                                 Resources.Url.TopupList + "> Danh sách Order</a> xem chi tiết" + MsgDup;
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script",
                " $(document).ready(function() {$('#AlertSuccess').modal()}); ", true);
        }
        else
        {

            AlertBans.Text = Msg;
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertBan').modal()}); ", true);
        }


    }
    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.TopupAdd);
    }
    public bool IsValidPhone(string Phone)
    {
        try
        {
            if (string.IsNullOrEmpty(Phone))
                return false;
            var r = new Regex(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$");
            return (r.IsMatch(Phone) && Phone.Length > 5 && Phone.Length < 15);
        }
        catch (Exception) { return false; }
    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        var partners = string.Empty;
        var providers = string.Empty;
        var OrderNo = string.Empty;

        if (!AppUtils.IsAdmin)
        {
            //partners = string.Join(",", AppUtils.PartnerUser.Select(x => x.PartnerCode).ToArray());
            //providers = string.Join(",", AppUtils.ProviderUser.Select(x => x.ProviderCode).ToArray());
            //partners = string.Join(",", new Partners().GetListByUserId(AppUtils.UserID).Select(x => x.PartnerCode).ToArray());
            providers = string.Join(",", new Providers().GetListByUserId(AppUtils.UserID).Select(x => x.ProviderCode).ToArray());
            NLogLogger.Info(new string[] { "Add Order Exel", AppUtils.UserName, "Providers", providers });

        }
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
                                var obj = new TopupMobileLog();
                                obj.UserId = AppUtils.UserID;
                                obj.UserName = AppUtils.UserName;
                                obj.Mobile = row.ItemArray[0].ToString().ToLower().Trim('\'').Trim('?').Trim();
                                obj.FullName = row.ItemArray[1].ToString().Trim();
                                obj.OrderNo = row.ItemArray[2].ToString().Trim();
                                obj.Telco = row.ItemArray[3].ToString().Trim().ToLower();
                                obj.TopupType = int.Parse(row.ItemArray[4].ToString().Trim());
                                obj.Amount = int.Parse(row.ItemArray[5].ToString().Trim(), NumberStyles.Number);
                                obj.AmountMin = int.Parse(row.ItemArray[6].ToString().Trim(), NumberStyles.Number);
                                obj.AmountMinAll = int.Parse(row.ItemArray[7].ToString().Trim(), NumberStyles.Number);
                                obj.Priority = int.Parse(row.ItemArray[8].ToString().Trim());
                                obj.RequestNo = GenOrderCode();
                                obj.AmountPending = 0;
                                obj.AmountTopupSuccess = 0;
                                obj.LogContent = "Add";
                                obj.Status = row.ItemArray[3].ToString().Trim().ToLower() == "vms" && string.IsNullOrEmpty(row.ItemArray[10].ToString().Trim()) ? -3 : 1;
                                obj.Partners = partners;
                                obj.Providers = providers;
                                obj.AccountName = row.ItemArray[9].ToString().ToLower().Trim();
                                obj.Password = row.ItemArray[10].ToString().Trim();
                                obj.Ussd = int.Parse(row.ItemArray[11].ToString().Trim(), NumberStyles.Number);
                                if (obj.Mobile.Length == 9)
                                    obj.Mobile = "0" + obj.Mobile;
                                obj.Add();
                            }
                            else
                            {
                                errorRow++;
                            }

                            OrderNo = row.ItemArray[2].ToString().Trim();

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

                    var listDup = new TopupMobileLog().GetListDuplicate(OrderNo, AppUtils.UserID);
                    foreach (var dup in listDup)
                    {
                        MsgDup = MsgDup + " - " + dup.Mobile + " xuất hiện " + dup.Dup + " lần </br>";
                    }

                    if (errorRow != 0)
                    {
                        AlertSuccesss.Text = "Import thành công! <br/>" + successRaw + " đơn trên tổng " + totalRaw +
                                             " dòng đơn! Tuy nhiên gặp lỗi từ dòng số " + stepRaw + " bạn hãy kiểm tra lại <br/> Click <a href=" +
                                             Constant.ADMIN_PATH + Resources.Url.TopupList + "> Danh sách Order</a> xem chi tiết" + MsgDup;
                    }
                    else
                    {
                        AlertSuccesss.Text = "Import thành công <br/>" + successRaw + " đơn trên tổng " + totalRaw + " dòng đơn!<br/> Click <a href=" + Constant.ADMIN_PATH + Resources.Url.TopupList + ">Danh sách Order</a> xem chi tiết" + MsgDup;
                    }

                    Page.ClientScript.RegisterStartupScript(
                         this.GetType(),
                         "popup",
                         "$('#AlertSuccess').modal('show');",
                         true
                     );
                }
                else
                {
                    AlertBans.Text = "Lỗi: File không đúng định dạng (định dạng bắt buộc là .xlsx)";

                    Page.ClientScript.RegisterStartupScript(
                         this.GetType(),
                         "popup",
                         "$('#AlertSuccess').modal('show');",
                         true
                     );
                }
            }
        }
        catch (Exception ex)
        {
            NLogLogger.Info(new string[] { "Topup.Add", "TopupMobileLog", "Error", ex.Message });
            AlertBans.Text = "Lỗi: " + ex.Message;

            Page.ClientScript.RegisterStartupScript(
                 this.GetType(),
                 "popup",
                 "$('#AlertSuccess').modal('show');",
                 true
             );
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

    public string GenOrderCode()
    {
        string[] pp = ("q,w,e,r,t,y,u,i,o,p,a,s,d,f,g,h,j,k,l,z,x,c,v,b,n,m,0,1,2,3,4,5,6,7,8,9").Split(',');
        string tmp = "";
        Random rd = new Random();
        for (int i = 1; i <= 17; i++)
        {
            tmp += pp[rd.Next(0, pp.Length - 1)];
        }
        //var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        return tmp.ToUpper(); //+ timeSpan.ToString();
    }
}