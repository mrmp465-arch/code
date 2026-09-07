using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Utils;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Text.RegularExpressions;
using System.Data;
using System.Linq;
public partial class Pages_BankEWalletService_Bank_UploadImage : System.Web.UI.Page
{
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    public string Id
    {
        get
        {
            return AppUtils.Request("id").ToString();
        }

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        serializer.MaxJsonLength = int.MaxValue;
        AppUtils.CheckRoles(Resources.Url.BankAccountEdit);

        if (!IsPostBack)
        {
            init();
        }
    }
    private void init()
    {
        var _bank = new BankAccounts();
        _bank.Id = Convert.ToInt32(AppUtils.Request("id"));
        _bank = _bank.GetProfile();

        if (_bank == null)
        {
            Response.Redirect(Resources.Url.BankAccount);
        }
        lblAccount.Text = _bank.Solution + " - Tài khoản " + _bank.BankCode + " : " + _bank.BankId;

        var bankCode = _bank.BankCode;
        var bankId = _bank.BankId;

        //lblId.Text = AppUtils.Request("id").ToString();
        //lblBankInfo.Text = "Upload - Tài khoản " + bankCode + " : " + bankId;
        bankCode = Regex.Replace(bankCode, @"[^a-zA-Z0-9]", "");
        bankId = Regex.Replace(bankId, @"[^a-zA-Z0-9]", "");

        string UploadFolderPhysical = Path.Combine(@"Z:\SDPAY", bankCode, bankId);

        if (!Directory.Exists(UploadFolderPhysical))
        {
            Directory.CreateDirectory(UploadFolderPhysical);
        }

        //imge
        var lstProfileImage = new List<BankProfileImage>();
        lstProfileImage.Add(new BankProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = ""
        });
        lstProfileImage.Add(new BankProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = ""
        });
        lstProfileImage.Add(new BankProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = ""
        });
        lstProfileImage.Add(new BankProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = ""
        });
        lstProfileImage.Add(new BankProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = ""
        });
        lstProfileImage.Add(new BankProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = ""
        });
        lstProfileImage.Add(new BankProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = ""
        });
        lstProfileImage.Add(new BankProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = ""
        });

        //if (!string.IsNullOrEmpty(_bank.ProfileImage))
        //{
        //    try
        //    {
        //        lstProfileImage = serializer.Deserialize<List<BankProfileImage>>(_bank.ProfileImage);

        //    }
        //    catch
        //    {

        //    }
        //}

        string filePath1 = Path.Combine(UploadFolderPhysical, "1.jpg");
        if (File.Exists(filePath1))
        {
            img1.ImageUrl = FileToBase64ImageUrl(filePath1);
        }
        else if (!string.IsNullOrEmpty(lstProfileImage[0].Base64) && lstProfileImage[0].Base64.Contains("/cmspay"))
        {
            img1.ImageUrl = lstProfileImage[0].Base64;
        }
        else if (!string.IsNullOrEmpty(lstProfileImage[0].Base64))
        {
            img1.ImageUrl = "data:image/png;base64," + ImgResize(lstProfileImage[0].Base64);
        }
        else
        {
            img1.ImageUrl = "/cmspay/content/noimage.png";
        }

        string filePath2 = Path.Combine(UploadFolderPhysical, "2.jpg");
        if (File.Exists(filePath2))
        {
            img2.ImageUrl = FileToBase64ImageUrl(filePath2);
        }
        else if (!string.IsNullOrEmpty(lstProfileImage[1].Base64) && lstProfileImage[1].Base64.Contains("/cmspay"))
        {
            img2.ImageUrl = lstProfileImage[1].Base64;
        }
        else if (!string.IsNullOrEmpty(lstProfileImage[1].Base64))
        {
            img2.ImageUrl = "data:image/png;base64," + ImgResize(lstProfileImage[1].Base64);
        }
        else
        {
            img2.ImageUrl = "/cmspay/content/noimage.png";
        }

        string filePath3 = Path.Combine(UploadFolderPhysical, "3.jpg");
        if (File.Exists(filePath3))
        {
            img3.ImageUrl = FileToBase64ImageUrl(filePath3);
        }
        else if (!string.IsNullOrEmpty(lstProfileImage[2].Base64) && lstProfileImage[2].Base64.Contains("/cmspay"))
        {
            img3.ImageUrl = lstProfileImage[2].Base64;
        }
        else if (!string.IsNullOrEmpty(lstProfileImage[2].Base64))
        {
            img3.ImageUrl = "data:image/png;base64," + ImgResize(lstProfileImage[2].Base64);
        }
        else
        {
            img3.ImageUrl = "/cmspay/content/noimage.png";
        }

        string filePath4 = Path.Combine(UploadFolderPhysical, "4.jpg");
        if (File.Exists(filePath4))
        {
            img4.ImageUrl = FileToBase64ImageUrl(filePath4);
        }
        else if (!string.IsNullOrEmpty(lstProfileImage[3].Base64) && lstProfileImage[3].Base64.Contains("/cmspay"))
        {
            img4.ImageUrl = lstProfileImage[3].Base64;
        }
        else if (!string.IsNullOrEmpty(lstProfileImage[3].Base64))
        {
            img4.ImageUrl = "data:image/png;base64," + ImgResize(lstProfileImage[3].Base64);
        }
        else
        {
            img4.ImageUrl = "/cmspay/content/noimage.png";
        }

        string filePath5 = Path.Combine(UploadFolderPhysical, "5.jpg");
        if (File.Exists(filePath5))
        {
            img5.ImageUrl = FileToBase64ImageUrl(filePath5);
        }
        else if (!string.IsNullOrEmpty(lstProfileImage[4].Base64) && lstProfileImage[4].Base64.Contains("/cmspay"))
        {
            img5.ImageUrl = lstProfileImage[4].Base64;
        }
        else if (!string.IsNullOrEmpty(lstProfileImage[4].Base64))
        {
            img5.ImageUrl = "data:image/png;base64," + ImgResize(lstProfileImage[4].Base64);
        }
        else
        {
            img5.ImageUrl = "/cmspay/content/noimage.png";
        }

        string filePath6 = Path.Combine(UploadFolderPhysical, "6.jpg");
        if (File.Exists(filePath6))
        {
            img6.ImageUrl = FileToBase64ImageUrl(filePath6);
        }
        else if (!string.IsNullOrEmpty(lstProfileImage[5].Base64) && lstProfileImage[5].Base64.Contains("/cmspay"))
        {
            img6.ImageUrl = lstProfileImage[5].Base64;
        }
        else if (!string.IsNullOrEmpty(lstProfileImage[5].Base64))
        {
            img6.ImageUrl = "data:image/png;base64," + ImgResize(lstProfileImage[5].Base64);
        }
        else
        {
            img6.ImageUrl = "/cmspay/content/noimage.png";
        }

        string filePath7 = Path.Combine(UploadFolderPhysical, "7.jpg");
        if (File.Exists(filePath7))
        {
            img7.ImageUrl = FileToBase64ImageUrl(filePath7);
        }
        else if (!string.IsNullOrEmpty(lstProfileImage[6].Base64) && lstProfileImage[6].Base64.Contains("/cmspay"))
        {
            img7.ImageUrl = lstProfileImage[6].Base64;
        }
        else if (!string.IsNullOrEmpty(lstProfileImage[6].Base64))
        {
            img7.ImageUrl = "data:image/png;base64," + ImgResize(lstProfileImage[6].Base64);
        }
        else
        {
            img7.ImageUrl = "/cmspay/content/noimage.png";
        }

        string filePath8 = Path.Combine(UploadFolderPhysical, "8.jpg");
        if (File.Exists(filePath8))
        {
            img8.ImageUrl = FileToBase64ImageUrl(filePath8);
        }
        else if (!string.IsNullOrEmpty(lstProfileImage[7].Base64) && lstProfileImage[7].Base64.Contains("/cmspay"))
        {
            img8.ImageUrl = lstProfileImage[7].Base64;
        }
        else if (!string.IsNullOrEmpty(lstProfileImage[7].Base64))
        {
            img8.ImageUrl = "data:image/png;base64," + ImgResize(lstProfileImage[7].Base64);
        }
        else
        {
            img8.ImageUrl = "/cmspay/content/noimage.png";
        }
    }
    protected void btCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.BankAccount);
    }
    protected void btAdd_Click(object sender, EventArgs e)
    {

        var _bank = new BankAccounts();
        _bank.Id = Convert.ToInt32(AppUtils.Request("id"));
        _bank = _bank.GetProfile();

        var bankCode = _bank.BankCode;
        var bankId = _bank.BankId;

        //lblId.Text = AppUtils.Request("id").ToString();
        //lblBankInfo.Text = "Upload - Tài khoản " + bankCode + " : " + bankId;
        bankCode = Regex.Replace(bankCode, @"[^a-zA-Z0-9]", "");
        bankId = Regex.Replace(bankId, @"[^a-zA-Z0-9]", "");

        string UploadFolderPhysical = Path.Combine(@"Z:\SDPAY", bankCode, bankId);

        if (!Directory.Exists(UploadFolderPhysical))
        {
            Directory.CreateDirectory(UploadFolderPhysical);
        }


        var lstProfileImage = new List<BankProfileImage>();
        lstProfileImage.Add(new BankProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = ""
        });
        lstProfileImage.Add(new BankProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = ""
        });
        lstProfileImage.Add(new BankProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = ""
        });
        lstProfileImage.Add(new BankProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = ""
        });
        lstProfileImage.Add(new BankProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = ""
        });
        lstProfileImage.Add(new BankProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = ""
        });
        lstProfileImage.Add(new BankProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = ""
        });
        lstProfileImage.Add(new BankProfileImage
        {
            Base64 = "/cmspay/content/noimage.png",
            ImgName = ""
        });

        if (!string.IsNullOrEmpty(_bank.ProfileImage))
        {
            try
            {
                lstProfileImage = serializer.Deserialize<List<BankProfileImage>>(_bank.ProfileImage);

            }
            catch
            {

            }
        }

        if (fileUpload1.HasFile)
        {
            if (fileUpload1.FileName.ToLower().Contains(".jpg") || fileUpload1.FileName.ToLower().Contains(".png") || fileUpload1.FileName.ToLower().Contains(".jpeg"))
            {

                SaveUploadImage(fileUpload1, UploadFolderPhysical, 1);
                lstProfileImage[0].ImgName = "1.jpg";
                lstProfileImage[0].Base64 = "";
            }

        }
        else
        {
            if (hdimg1.Value == "0")
            {
                DeleteUploadImage(UploadFolderPhysical, 1);
                lstProfileImage[0].ImgName = "";
                lstProfileImage[0].Base64 = "/cmspay/content/noimage.png";

            }


        }
        if (fileUpload2.HasFile)

        {
            if (fileUpload2.FileName.ToLower().Contains(".jpg") || fileUpload2.FileName.ToLower().Contains(".png") || fileUpload2.FileName.ToLower().Contains(".jpeg"))
            {

                SaveUploadImage(fileUpload2, UploadFolderPhysical, 2);
                lstProfileImage[1].ImgName = "2.jpg";
                lstProfileImage[1].Base64 = "";
            }

        }
        else
        {
            if (hdimg2.Value == "0")
            {
                DeleteUploadImage(UploadFolderPhysical, 2);
                lstProfileImage[1].ImgName = "";
                lstProfileImage[1].Base64 = "/cmspay/content/noimage.png";
            }

        }
        if (fileUpload3.HasFile)
        {
            if (fileUpload3.FileName.ToLower().Contains(".jpg") || fileUpload3.FileName.ToLower().Contains(".png") || fileUpload3.FileName.ToLower().Contains(".jpeg"))
            {

                SaveUploadImage(fileUpload3, UploadFolderPhysical, 3);
                lstProfileImage[2].ImgName = "3.jpg";
                lstProfileImage[2].Base64 = "";

            }

        }
        else
        {
            if (hdimg3.Value == "0")
            {
                DeleteUploadImage(UploadFolderPhysical, 3);
                lstProfileImage[2].ImgName = "";
                lstProfileImage[2].Base64 = "/cmspay/content/noimage.png";
            }

        }
        if (fileUpload4.HasFile)
        {
            if (fileUpload4.FileName.ToLower().Contains(".jpg") || fileUpload4.FileName.ToLower().Contains(".png") || fileUpload4.FileName.ToLower().Contains(".jpeg"))
            {

                SaveUploadImage(fileUpload4, UploadFolderPhysical, 4);
                lstProfileImage[3].ImgName = "4.jpg";
                lstProfileImage[3].Base64 = "";
            }

        }
        else
        {
            if (hdimg4.Value == "0")
            {
                DeleteUploadImage(UploadFolderPhysical, 4);
                lstProfileImage[3].ImgName = "";
                lstProfileImage[3].Base64 = "/cmspay/content/noimage.png";
            }

        }

        if (fileUpload5.HasFile)
        {
            if (fileUpload5.FileName.ToLower().Contains(".jpg") || fileUpload5.FileName.ToLower().Contains(".png") || fileUpload5.FileName.ToLower().Contains(".jpeg"))
            {

                SaveUploadImage(fileUpload5, UploadFolderPhysical, 5);
                lstProfileImage[4].ImgName = "5.jpg";
                lstProfileImage[4].Base64 = "";
            }

        }
        else
        {
            if (hdimg5.Value == "0")
            {
                DeleteUploadImage(UploadFolderPhysical, 5);
                lstProfileImage[4].ImgName = "";
                lstProfileImage[4].Base64 = "/cmspay/content/noimage.png";
            }

        }
        if (fileUpload6.HasFile)

        {
            if (fileUpload6.FileName.ToLower().Contains(".jpg") || fileUpload6.FileName.ToLower().Contains(".png") || fileUpload6.FileName.ToLower().Contains(".jpeg"))
            {

                SaveUploadImage(fileUpload6, UploadFolderPhysical, 6);
                lstProfileImage[5].ImgName = "6.jpg";
                lstProfileImage[5].Base64 = "";
            }

        }
        else
        {
            if (hdimg6.Value == "0")
            {
                DeleteUploadImage(UploadFolderPhysical, 6);
                lstProfileImage[5].ImgName = "";
                lstProfileImage[5].Base64 = "/cmspay/content/noimage.png";
            }

        }
        if (fileUpload7.HasFile)
        {
            if (fileUpload7.FileName.ToLower().Contains(".jpg") || fileUpload7.FileName.ToLower().Contains(".png") || fileUpload7.FileName.ToLower().Contains(".jpeg"))
            {

                SaveUploadImage(fileUpload7, UploadFolderPhysical, 7);
                lstProfileImage[6].ImgName = "7.jpg";
                lstProfileImage[6].Base64 = "";

            }

        }
        else
        {
            if (hdimg7.Value == "0")
            {
                DeleteUploadImage(UploadFolderPhysical, 7);
                lstProfileImage[6].ImgName = "";
                lstProfileImage[6].Base64 = "/cmspay/content/noimage.png";
            }

        }
        if (fileUpload8.HasFile)
        {
            if (fileUpload8.FileName.ToLower().Contains(".jpg") || fileUpload8.FileName.ToLower().Contains(".png") || fileUpload8.FileName.ToLower().Contains(".jpeg"))
            {

                SaveUploadImage(fileUpload8, UploadFolderPhysical, 8);
                lstProfileImage[7].ImgName = "8.jpg";
                lstProfileImage[7].Base64 = "";
            }

        }
        else
        {
            if (hdimg8.Value == "0")
            {
                DeleteUploadImage(UploadFolderPhysical, 8);
                lstProfileImage[7].ImgName = "";
                lstProfileImage[7].Base64 = "/cmspay/content/noimage.png";
            }

        }

        //_bank.ProfileImage = serializer.Serialize(lstProfileImage);
        //_bank.UpdateProfile(_bank.Id, _bank.ProfileImage);
        //Log User
        var _userLog = new UserLog
        {
            UserName = AppUtils.UserName,
            Action = "bankupdate",
            ActionName = "Cập nhật bank",
            Description = "Cập nhật bank " + _bank.BankCode + " |" + _bank.BankId
        };
        _userLog.Add();
        Response.Redirect(Constant.ADMIN_PATH + Resources.Url.BankAccount);
    }
    protected string ConvertBase64(Stream Path)
    {
        using (System.Drawing.Image image = System.Drawing.Image.FromStream(Path))
        {
            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, image.RawFormat);
                byte[] imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                //return base64String;
                return ImgResize2(base64String);
            }
        }
    }
    private static void FixImageOrientation(System.Drawing.Image img)
    {
        const int ExifOrientationId = 0x0112;

        if (!img.PropertyIdList.Contains(ExifOrientationId))
            return;

        var prop = img.GetPropertyItem(ExifOrientationId);
        int orientation = BitConverter.ToUInt16(prop.Value, 0);

        switch (orientation)
        {
            case 2:
                img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                break;
            case 3:
                img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                break;
            case 4:
                img.RotateFlip(RotateFlipType.Rotate180FlipX);
                break;
            case 5:
                img.RotateFlip(RotateFlipType.Rotate90FlipX);
                break;
            case 6:
                img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                break;
            case 7:
                img.RotateFlip(RotateFlipType.Rotate270FlipX);
                break;
            case 8:
                img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                break;
        }

        img.RemovePropertyItem(ExifOrientationId);
    }
    public static string ImgResize2(string base64Image)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(base64Image);

            using (var input = new MemoryStream(bytes))
            using (var original = System.Drawing.Image.FromStream(input))
            {
                FixImageOrientation(original);
                int maxHeight = 1440;

                if (original.Height <= maxHeight)
                    return base64Image;

                int newHeight = maxHeight;
                int newWidth = (int)Math.Round(original.Width * (double)newHeight / original.Height);

                using (var resized = new Bitmap(newWidth, newHeight))
                using (var g = Graphics.FromImage(resized))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(original, 0, 0, newWidth, newHeight);

                    using (var output = new MemoryStream())
                    {
                        resized.Save(output, ImageFormat.Png);
                        return Convert.ToBase64String(output.ToArray());
                    }
                }
            }
        }
        catch (Exception e)
        {
            NLogLogger.Info(new string[] { "ImageHelper", "ImgResize", "Base64 Invalid", e.Message });
            return string.Empty;
        }
    }
    public static string ImgResize(string base64Image)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(base64Image);

            using (var input = new MemoryStream(bytes))
            using (var original = System.Drawing.Image.FromStream(input))
            {
                int maxHeight = 1440;
                //NLogLogger.Info(new string[] { "ImageHelper", "ImgResize", "Base64 Invalid", original.Height.ToString() });
                if (original.Height <= maxHeight)
                    return base64Image;

                int newHeight = maxHeight;
                int newWidth = (int)Math.Round(original.Width * (double)newHeight / original.Height);

                using (var resized = new Bitmap(newWidth, newHeight))
                using (var g = Graphics.FromImage(resized))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(original, 0, 0, newWidth, newHeight);

                    using (var output = new MemoryStream())
                    {
                        resized.Save(output, ImageFormat.Png);
                        return Convert.ToBase64String(output.ToArray());
                    }
                }
            }
        }
        catch (Exception e)
        {
            NLogLogger.Info(new string[] { "ImageHelper", "ImgResize", "Base64 Invalid", e.Message });
            return string.Empty;
        }
    }
    private void SaveUploadImage(FileUpload file, string folder, int index)
    {
        string filePath = Path.Combine(folder, index + ".jpg");

        // Xóa file cũ nếu tồn tại
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        using (var img = System.Drawing.Image.FromStream(file.FileContent))
        {
            FixImageOrientation(img);

            int maxHeight = 1440;
            System.Drawing.Image finalImg = img;

            if (img.Height > maxHeight)
            {
                int newHeight = maxHeight;
                int newWidth = (int)Math.Round(img.Width * (double)newHeight / img.Height);

                var resized = new Bitmap(newWidth, newHeight);

                using (var g = Graphics.FromImage(resized))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(img, 0, 0, newWidth, newHeight);
                }

                finalImg = resized;
            }

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                finalImg.Save(fs, ImageFormat.Jpeg);
            }

            if (!ReferenceEquals(finalImg, img))
                finalImg.Dispose();
        }
    }
    private void DeleteUploadImage(string folder, int index)
    {
        string filePath = Path.Combine(folder, index + ".jpg");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
    private string FileToBase64ImageUrl(string filePath)
    {
        if (!File.Exists(filePath))
            return "/cmspay/content/noimage.png";

        byte[] bytes = File.ReadAllBytes(filePath);
        string base64 = Convert.ToBase64String(bytes);

        return "data:image/jpeg;base64," + base64;
    }
}