using System;
using System.Linq;
using Libs.Report;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using Libs.Utils;

public partial class Pages_Monitor_CardAPI_FixBulk : System.Web.UI.Page
{
    int PageSize = 20; 
    public Pager pages;
    protected void Page_Load(object sender, EventArgs e)
    {
        btnAdd.Text = "Thêm ";
        AppUtils.CheckRoles(Resources.Url.CardAPIFixBulk);
        if (IsPostBack) return;
        if (Request["type"] == "his")
        {
            BindDataPageWise(Convert.ToInt32(AppUtils.Request("page")));
            btnHistory.Text = "Quay lại fix bulk";
            btUpdate.Visible = false;
            panelTools.Visible = false;
        }
        else
        {
            if (Request["type"] == "del" & AppUtils.Request("id") > 0)
            {
                new CardAPILogFixBulk() { Id = AppUtils.Request("id") }.Delete();
            }
            else if (AppUtils.Request("id") > 0)
            {
                var obj = new CardAPILogFixBulk() { Id = AppUtils.Request("id") }.Get();
                txtTransactionID.Text = obj.TransactionID.ToString();
                txtAmount.Text = obj.Amount.ToString();
                txtCardSerial.Text = obj.CardSerial;
                txtId.Value = obj.Id.ToString();
                btnAdd.Text = "Cập nhập";
            }
            bindata(0);
        }
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        try
        {

            if (!string.IsNullOrEmpty(txtTransactionID.Text) && !string.IsNullOrEmpty(txtCardSerial.Text) && !string.IsNullOrEmpty(txtAmount.Text))
            {
                var tempobj = new CardAPILogFixBulk().GetList(null, AppUtils.ToInt64(txtTransactionID.Text), null);
                if (tempobj.Count() <= 0)
                {
                    CardAPILogFixBulk obj = new CardAPILogFixBulk
                    {
                        TransactionID = AppUtils.ToInt64(txtTransactionID.Text),
                        CardSerial = txtCardSerial.Text,
                        Amount = AppUtils.ToInt64(txtAmount.Text),
                        Status = 0,
                        Id = AppUtils.ToInt64(txtId.Value),
                    };

                    if (!string.IsNullOrEmpty(txtId.Value))
                    {
                        obj.Update();
                        AlertSuccesss.Text = "Cập nhập thành công";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertSuccess').modal()}); ", true);
                    }
                    else
                    {
                        obj.Add();
                        AlertSuccesss.Text = "Thêm thành công";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertSuccess').modal()}); ", true);
                    }
                    txtTransactionID.Text = "";
                    txtCardSerial.Text = "";
                    txtAmount.Text = "";
                    txtId.Value = "";
                }
                else
                {
                    AlertInfos.Text = "TransactionID đã tồn tại";
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
                }
            }
            else
            {
                AlertInfos.Text = "Bạn chưa nhập đủ thông tin";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertInfo').modal()}); ", true);
            }

        }
        catch (Exception ex)
        {
            AlertBans.Text = "Lỗi: " + ex.Message;
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertBan').modal()}); ", true);
        }
        bindata(0);
    } 
    protected void btFixBulk_Click(object sender, EventArgs e)
    {
        try
        {
            var lstData = new CardAPILogFixBulk().GetList(0, null, null);
            int fix = 0, notfix = lstData.Count();
            foreach (var item in lstData)
            {
                CardAPILog _CardAPILog = new CardAPILog()
                {
                    TransactionID = item.TransactionID
                }.Get();
                if (_CardAPILog != null)
                {
                    if (_CardAPILog.CardSerial == item.CardSerial)
                    {
                        _CardAPILog.Status = 1;
                        _CardAPILog.Amount = item.Amount;
                        _CardAPILog.Update();
                        item.Status = 1;
                        item.Update();
                        fix++;
                    }
                }
                bindata(0);
            }
            AlertSuccesss.Text = "FixBulk thành công " + fix + "/" + notfix;
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertSuccess').modal()}); ", true);
        }
        catch (Exception ex)
        {

        }
    }
  
    protected void BindDataPageWise(int page)
    {
        int totalRecord;
        var lstData = new CardAPILogFixBulk().GetListPage(1, null, null, page, PageSize, out totalRecord);
        rptList.DataSource = AppUtils.ToDataTable(lstData);
        rptList.DataBind();
        pages = new Pager(totalRecord, page, PageSize); 
    }
    protected void btHistory_Click(object sender, EventArgs e)
    {
        if (btnHistory.Text == "Lịch sử sửa lỗi")
        { 
            BindDataPageWise(1);
            btnHistory.Text = "Quay lại fix bulk";
            btUpdate.Visible = false;
            panelTools.Visible = false;
        }
        else
            Response.Redirect(Constant.ADMIN_PATH + Resources.Url.CardAPIFixBulk);
    }
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        try
        {
            if (FileUploadExcel.HasFile)
            {
                List<CardAPILogFixBulk> lstData = new List<CardAPILogFixBulk>();
                int i = 0;
                string fileN = DateTime.Now.ToString("dd_MM_yyyy_hhmmss") + "_" + FileUploadExcel.FileName;
                string pathFile = Server.MapPath("~/Excels/" + fileN);
                 FileUploadExcel.SaveAs(pathFile);
                var tempDataTable = GetDataTableFromExcel(pathFile, false);
                foreach (DataRow row in tempDataTable.Rows)
                {  
                        try
                        {
                            CardAPILogFixBulk obj = new CardAPILogFixBulk
                            {
                                TransactionID = Convert.ToInt64(row.ItemArray[0]),
                                CardSerial = row.ItemArray[1].ToString(),
                                Amount = Convert.ToInt64(row.ItemArray[2]),
                                Status = 0
                            };
                            obj.Add();
                            lstData.Add(obj);
                        }
                        catch (Exception ex)
                        { 
                        } 
                }
                bindata(0);
            }
        }
        catch (Exception ex)
        {
            AlertBans.Text = "Lỗi: " + ex.Message;
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Script", " $(document).ready(function() {$('#AlertBan').modal()}); ", true);
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
    protected void bindata(int Status)
    {
        var lstData = new CardAPILogFixBulk().GetList(Status, null, null);
        rptList.DataSource = AppUtils.ToDataTable(lstData);
        rptList.DataBind();  
    }
    
    public string Getactive(int page1,int page2)
    {
        if (page1 == page2)
            return "active";
        else
            return "";
    } 
}
