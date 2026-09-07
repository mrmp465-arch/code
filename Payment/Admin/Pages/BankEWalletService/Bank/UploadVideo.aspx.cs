using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FluentFTP;
using System.Security.Authentication;
using Libs.API;
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

                var allowExt = new[] { ".mp4", ".mov" };
                if (!allowExt.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    errorFiles.Add(originalFileName + " (chỉ hỗ trợ mp4, mov)");
                    continue;
                }

                if (string.IsNullOrEmpty(file.ContentType) || !file.ContentType.ToLower().Contains("video"))
                {
                    errorFiles.Add(originalFileName + " (không phải file video)");
                    continue;
                }

                string safeFileName = Path.GetFileNameWithoutExtension(originalFileName);
                safeFileName = RemoveInvalidFileNameChars(safeFileName);

                if (string.IsNullOrWhiteSpace(safeFileName))
                {
                    errorFiles.Add(originalFileName + " (tên file không hợp lệ)");
                    continue;
                }

                string newFileName = safeFileName + extension;

                var bankCode = AppUtils.RequestCode("bankcode");
                var bankId = AppUtils.RequestCode("bankId");

                bankCode = Regex.Replace(bankCode ?? "", @"[^a-zA-Z0-9]", "");
                bankId = Regex.Replace(bankId ?? "", @"[^a-zA-Z0-9]", "");

                if (string.IsNullOrWhiteSpace(bankCode) || string.IsNullOrWhiteSpace(bankId))
                {
                    errorFiles.Add(originalFileName + " (thiếu bankCode hoặc bankId)");
                    continue;
                }

                string ftpPath = string.Format("{0}/{1}/{2}", bankCode, bankId, newFileName);

                using (Stream stream = file.InputStream)
                {
                    if (!UploadOnFTP(ftpPath, stream))
                    {
                        errorFiles.Add(originalFileName + " (upload FTP thất bại)");
                        continue;
                    }
                }

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
    protected void EnsureFtpDirectory(string ftpFolder)
    {
        string ftpServerIP = "45.32.115.186:1521";
        string ftpUserID = "fastpay";
        string ftpPassword = "Bank@123@";

        string[] folders = ftpFolder.Split('/');

        string currentPath = "";

        foreach (var folder in folders)
        {
            if (string.IsNullOrWhiteSpace(folder)) continue;

            currentPath += "/" + folder;

            try
            {
                string url = "ftp://" + ftpServerIP + currentPath;

                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(url);
                req.Method = WebRequestMethods.Ftp.MakeDirectory;
                req.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                req.UseBinary = true;
                req.UsePassive = true;
                req.KeepAlive = false;

                using (var resp = (FtpWebResponse)req.GetResponse())
                {
                    NLogLogger.Info("Created folder: " + currentPath);
                }
            }
            catch (WebException ex)
            {
                var resp = ex.Response as FtpWebResponse;

                // 550 = đã tồn tại → bỏ qua
                if (resp != null && resp.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    continue;
                }

                // lỗi khác thì log
                NLogLogger.Info("Create folder lỗi: " + ex.ToString());
            }
        }
    }



    protected bool UploadOnFTP(string ftpFileUrl, Stream sourceStream)
    {
        try
        {
            var config = new FtpConfig();
            config.EncryptionMode = FtpEncryptionMode.Explicit;
            config.ValidateAnyCertificate = true;
            config.DataConnectionType = FtpDataConnectionType.AutoPassive;
            config.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

            using (var client = new FtpClient(
                "45.32.115.186",
                new NetworkCredential("fastpay", "Bank@123@"),
                1521,
                config))
            {
                client.Connect();

                string folder = ftpFileUrl.Substring(0, ftpFileUrl.LastIndexOf('/'));
                client.CreateDirectory(folder, true);

                if (sourceStream.CanSeek)
                    sourceStream.Position = 0;

                var status = client.UploadStream(sourceStream, ftpFileUrl, FtpRemoteExists.Overwrite, true);

                return status == FtpStatus.Success;
            }
        }
        catch (Exception ex)
        {
            NLogLogger.Info("FluentFTP upload ex: " + ex.ToString());
            return false;
        }
    }

    private void BindVideos()
    {
        var bankCode = AppUtils.RequestCode("bankcode");
        var bankId = AppUtils.RequestCode("bankId");
        bankCode = Regex.Replace(bankCode, @"[^a-zA-Z0-9]", "");
        bankId = Regex.Replace(bankId, @"[^a-zA-Z0-9]", "");

        string UploadFolderPhysical = Path.Combine(@"Z:\FASTPAY", bankCode, bankId);

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