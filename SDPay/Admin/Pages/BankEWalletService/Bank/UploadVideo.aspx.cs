using Libs.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Pages_BankEWalletService_Bank_UploadVideo : System.Web.UI.Page
{
    public string bankcode
    {
        get { return AppUtils.RequestCode("bankcode"); }
    }

    public string bankId
    {
        get { return AppUtils.RequestCode("bankId"); }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        AppUtils.CheckRoles(Resources.Url.BankUploadVideo);
        if (!IsPostBack)
        {
            // EnsureFolderExists();
            var bankCode = AppUtils.RequestCode("bankcode");
            var bankId = AppUtils.RequestCode("bankId");

            lblId.Text = AppUtils.Request("id").ToString();
            var _bank = new BankAccounts();
            _bank.Id = Convert.ToInt32(AppUtils.Request("id"));
            _bank = _bank.Get();

            bankCode = _bank.BankCode;
            bankId = _bank.BankId;

            lblBankInfo.Text = "Upload - Tài khoản " + bankCode + " : " + bankId;
            BindVideos();
        }
    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        try
        {
            //EnsureFolderExists();

            if (fuVideos == null || fuVideos.PostedFiles == null || fuVideos.PostedFiles.Count == 0)
            {
                ShowMessage("Vui lòng chọn ít nhất 1 file", false);
                return;
            }

            int successCount = 0;
            List<string> errorFiles = new List<string>();

            foreach (var postedFile in fuVideos.PostedFiles)
            {
                var file = postedFile as System.Web.HttpPostedFile;
                if (file == null || file.ContentLength <= 0)
                    continue;

                string originalFileName = Path.GetFileName(file.FileName);
                string extension = Path.GetExtension(originalFileName);

                // ✅ Cho phép mp4 + mov
                var allowExt = new[] { ".mp4", ".mov" };

                if (!allowExt.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    errorFiles.Add(originalFileName + " (chỉ hỗ trợ mp4, mov)");
                    continue;
                }

                // (Optional) check MIME
                if (!file.ContentType.Contains("video"))
                {
                    errorFiles.Add(originalFileName + " (không phải file video)");
                    continue;
                }

                // Làm sạch tên file
                string safeFileName = Path.GetFileNameWithoutExtension(originalFileName);
                safeFileName = RemoveInvalidFileNameChars(safeFileName);

                // Giữ nguyên extension
                string newFileName = safeFileName  + extension;

                var bankCode = AppUtils.RequestCode("bankcode");
                var bankId = AppUtils.RequestCode("bankId");

                //lblId.Text = AppUtils.Request("id").ToString();
                //lblBankInfo.Text = "Upload - Tài khoản " + bankCode + " : " + bankId;
                bankCode = Regex.Replace(bankCode, @"[^a-zA-Z0-9]", "");
                bankId = Regex.Replace(bankId, @"[^a-zA-Z0-9]", "");

                string UploadFolderPhysical = Path.Combine(@"Z:\SDPAY", bankCode, bankId);

                if (!Directory.Exists(UploadFolderPhysical))
                {
                    Directory.CreateDirectory(UploadFolderPhysical);
                }

                string savePath = Path.Combine(UploadFolderPhysical, newFileName);

                file.SaveAs(savePath);
                successCount++;
            }

            string msg = "Upload thành công " + successCount + " file";

            if (errorFiles.Count > 0)
            {
                msg += ". File lỗi: " + string.Join(", ", errorFiles);
            }

            ShowMessage(msg, successCount > 0);
            BindVideos();
        }
        catch (Exception ex)
        {
            ShowMessage("Lỗi upload: " + ex.Message, false);
        }
    }

    private void BindVideos()
    {
        var bankCode = AppUtils.RequestCode("bankcode");
        var bankId = AppUtils.RequestCode("bankId");
        bankCode = Regex.Replace(bankCode, @"[^a-zA-Z0-9]", "");
        bankId = Regex.Replace(bankId, @"[^a-zA-Z0-9]", "");

        string UploadFolderPhysical = Path.Combine(@"Z:\SDPAY", bankCode, bankId);

        if (!Directory.Exists(UploadFolderPhysical))
        {
            Directory.CreateDirectory(UploadFolderPhysical);
        }

        DirectoryInfo dir = new DirectoryInfo(UploadFolderPhysical);

        var data = dir.GetFiles()
                      .Where(x => x.Extension.Equals(".mp4", StringComparison.OrdinalIgnoreCase)
                               || x.Extension.Equals(".mov", StringComparison.OrdinalIgnoreCase))
                      .OrderByDescending(x => x.LastWriteTime)
                      .Select(x => new
                      {
                          FileName = x.Name,
                          FileSize = x.Length,
                          FileSizeText = FormatFileSize(x.Length),
                          LastWriteTime = x.LastWriteTime,
                          LastWriteTimeText = x.LastWriteTime.ToString("dd/MM/yyyy HH:mm:ss"),
                          //FileUrl = UploadFolderVirtual + x.Name
                      })
                      .ToList();

        rptVideos.DataSource = data;
        rptVideos.DataBind();
    }

    //private void EnsureFolderExists()
    //{
    //    if (!Directory.Exists(UploadFolderPhysical))
    //    {
    //        Directory.CreateDirectory(UploadFolderPhysical);
    //    }
    //}

    private void ShowMessage(string message, bool isSuccess)
    {
        lblMessage.Text = message;
        lblMessage.CssClass = "message " + (isSuccess ? "success" : "error");
    }

    private string RemoveInvalidFileNameChars(string fileName)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(c.ToString(), "");
        }

        return fileName.Replace(" ", "_");
    }

    private string FormatFileSize(long bytes)
    {
        if (bytes >= 1024 * 1024 * 1024)
            return (bytes / 1024d / 1024d / 1024d).ToString("0.00") + " GB";

        if (bytes >= 1024 * 1024)
            return (bytes / 1024d / 1024d).ToString("0.00") + " MB";

        if (bytes >= 1024)
            return (bytes / 1024d).ToString("0.00") + " KB";

        return bytes + " B";
    }
}