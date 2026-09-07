using Libs.API;
using Libs.BankGate.Entity;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.DynamicData;
using System.Web.Script.Serialization;

namespace BankGateV2.ServiceHandler
{
    /// <summary>
    /// Summary description for GetBankImage
    /// </summary>
    public class GetDeviceImage : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            var AppDeviceId = HttpContext.Current.Request.QueryString["AppDeviceId"];
            var type = HttpContext.Current.Request.QueryString["type"];
            try
            {
                //NLogLogger.Info(new string[] { "GetImg ", bankId.ToString() });
                if (!string.IsNullOrEmpty(AppDeviceId))
                {
                    var bank = new BankAccounts().GetByAppDeviceIdImage(AppDeviceId);
                    if (bank != null)
                    {

                        var bankCode = bank.BankCode;
                        var bankId = bank.BankId;

                        //lblId.Text = AppUtils.Request("id").ToString();
                        //lblBankInfo.Text = "Upload - Tài khoản " + bankCode + " : " + bankId;
                        bankCode = Regex.Replace(bankCode, @"[^a-zA-Z0-9]", "");
                        bankId = Regex.Replace(bankId, @"[^a-zA-Z0-9]", "");

                        string UploadFolderPhysical = Path.Combine(@"Z:\FASTPAY", bankCode, bankId);
                        var index = 1;
                        if (bank.BankCode == "VPB")
                        {
                            var verdict = HttpContext.Current.Request.QueryString["verdict"];
                            switch (verdict)
                            {
                                case "main":
                                case "frontal":
                                    index = new Random().Next(1, 5);
                                    break;
                                case "up":
                                    index = 5;
                                    break;
                                case "down":
                                    index = 6;
                                    break;
                                case "left":
                                    index = 7;
                                    break;
                                case "right":
                                    index = 8;
                                    break;
                            }

                        }
                        string filePath = Path.Combine(UploadFolderPhysical, index.ToString() + ".jpg");
                        if (File.Exists(filePath))
                        {

                            var fileWatermark = FileToBase64ImageUrl(filePath); 
                            fileWatermark = ImageHelper.AddLine(fileWatermark, bankId);
                            context.Response.Write(fileWatermark);
                            return;
                        }

                       
                        context.Response.Write("");
                        return;

                    }
                    context.Response.Write("");
                    return;
                }
                context.Response.Write("");
            }
            catch
            {
                context.Response.Write("");
            }


        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        private string FileToBase64ImageUrl(string filePath)
        {
            if (!File.Exists(filePath))
                return "";

            byte[] bytes = File.ReadAllBytes(filePath);
            string base64 = Convert.ToBase64String(bytes);

            return "data:image/jpeg;base64," + base64;
        }
        public class ImageHelper
        {
            public static List<string> WatermarkTexts = new List<string> { "Samsung", "Xiaomi", "Vivo", "Oppo", "Huawei", "Realme", "TECNO", "HONOR", "Masstel", "Mobell", "itel", "Lenovo", "Motorola", "Sony", "ASUS" };
            public static string AddWatermark(string base64Image, string bankId)
            {
                // Chọn một chuỗi ngẫu nhiên từ danh sách
                Random random = new Random();
                string selectedText = $"{WatermarkTexts[random.Next(WatermarkTexts.Count)]} Camera {DateTimeOffset.Now.ToUnixTimeMilliseconds()}";

                // Chuyển đổi Base64 thành Image
                byte[] imageBytesFromBase64 = Convert.FromBase64String(base64Image);
                System.Drawing.Image image;
                using (MemoryStream ms = new MemoryStream(imageBytesFromBase64))
                {
                    image = System.Drawing.Image.FromStream(ms);
                }

                // Tạo một đối tượng Graphics từ Image
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    // Thiết lập font và màu sắc cho watermark
                    Font watermarkFont = new Font("Arial", 24, FontStyle.Bold, GraphicsUnit.Pixel);
                    Color color = Color.FromArgb(128, 255, 255, 255); // Màu trắng với độ trong suốt
                    SolidBrush brush = new SolidBrush(color);

                    // Vị trí đặt watermark góc dưới bên trái
                    int margin = 10;
                    SizeF textSize = graphics.MeasureString(selectedText, watermarkFont);
                    Point point = new Point(margin, image.Height - (int)textSize.Height - margin);

                    // Vẽ watermark lên hình ảnh
                    graphics.DrawString(selectedText, watermarkFont, brush, point);
                }

                // Chuyển đổi Image trở lại thành Base64
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Png);
                    byte[] imageBytes = ms.ToArray();
                    string base64ImageRes = Convert.ToBase64String(imageBytes);

                    //NLogLogger.Info(new string[] { "ImageHelper", "LoAddWatermarkgin", "Base64Watermark", base64ImageRes });

                    return base64ImageRes;
                }
            }
            public static string ImgResizeVPB(string base64Image, string bankId)
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(base64Image);

                    using (var input = new MemoryStream(bytes))
                    using (var original = System.Drawing.Image.FromStream(input))
                    {
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
                    NLogLogger.Info(new string[] { "ImageHelper", "ImgResize", "Base64 Invalid", e.Message, bankId });
                    return string.Empty;
                }
            }
            public static string ImgResize(string base64Image, string bankId)
            {
                try
                {
                    Random random = new Random();

                    // Convert Base64 to Image
                    byte[] imageBytesFromBase64 = Convert.FromBase64String(base64Image);
                    System.Drawing.Image image;
                    using (MemoryStream ms = new MemoryStream(imageBytesFromBase64))
                    {
                        image = System.Drawing.Image.FromStream(ms);
                    }
                    if (image.Height > 640)
                    {
                        var w = (int)(image.Width * 640 / image.Height);
                        //NLogLogger.Info(new string[] { "ImageHelper", "ImgResize", w.ToString() });
                        image = (System.Drawing.Image)(new Bitmap(image, new System.Drawing.Size(w, 640)));
                    }


                    // Convert Image back to Base64
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.Save(ms, ImageFormat.Jpeg);
                        byte[] imageBytes = ms.ToArray();
                        string base64ImageRes = Convert.ToBase64String(imageBytes);
                        return base64ImageRes;
                    }
                }
                catch (Exception e)
                {
                    NLogLogger.Info(new string[] { "ImageHelper", "ImgResize", "Base64 Invalid", e.Message, bankId });
                }

                return String.Empty;
            }
            public static string ImgResizePVCBBig(string base64Image, string bankId)
            {
                try
                {
                    byte[] imageBytes = Convert.FromBase64String(base64Image);

                    using (var inputStream = new MemoryStream(imageBytes))
                    using (var originalImage = System.Drawing.Image.FromStream(inputStream))
                    {
                        int targetWidth = 1080;
                        int targetHeight = 1440;

                        // Tính tỷ lệ scale
                        double ratioW = (double)targetWidth / originalImage.Width;
                        double ratioH = (double)targetHeight / originalImage.Height;
                        double scale = Math.Max(ratioW, ratioH); // Scale lớn hơn để đảm bảo crop vừa

                        int scaledWidth = (int)(originalImage.Width * scale);
                        int scaledHeight = (int)(originalImage.Height * scale);

                        // Resize ảnh trước
                        using (var resized = new Bitmap(originalImage, new Size(scaledWidth, scaledHeight)))
                        {
                            // Tính toạ độ crop ở giữa
                            int x = (scaledWidth - targetWidth) / 2;
                            int y = (scaledHeight - targetHeight) / 2;

                            // Crop đúng kích thước 480x922
                            using (var cropped = new Bitmap(targetWidth, targetHeight))
                            using (var g = Graphics.FromImage(cropped))
                            {
                                g.DrawImage(resized, new Rectangle(0, 0, targetWidth, targetHeight),
                                    new Rectangle(x, y, targetWidth, targetHeight), GraphicsUnit.Pixel);

                                using (var outputStream = new MemoryStream())
                                {
                                    cropped.Save(outputStream, ImageFormat.Jpeg);
                                    return Convert.ToBase64String(outputStream.ToArray());
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    NLogLogger.Info(new string[] { "ImageHelper", "ImgResize", "Base64 Invalid", e.Message, bankId });
                }

                return String.Empty;
            }
            public static string ImgResizePVCB(string base64Image, string bankId)
            {
                try
                {
                    byte[] imageBytes = Convert.FromBase64String(base64Image);

                    using (var inputStream = new MemoryStream(imageBytes))
                    using (var originalImage = System.Drawing.Image.FromStream(inputStream))
                    {
                        int targetWidth = 480;
                        int targetHeight = 922;

                        // Tính tỷ lệ scale
                        double ratioW = (double)targetWidth / originalImage.Width;
                        double ratioH = (double)targetHeight / originalImage.Height;
                        double scale = Math.Max(ratioW, ratioH); // Scale lớn hơn để đảm bảo crop vừa

                        int scaledWidth = (int)(originalImage.Width * scale);
                        int scaledHeight = (int)(originalImage.Height * scale);

                        // Resize ảnh trước
                        using (var resized = new Bitmap(originalImage, new Size(scaledWidth, scaledHeight)))
                        {
                            // Tính toạ độ crop ở giữa
                            int x = (scaledWidth - targetWidth) / 2;
                            int y = (scaledHeight - targetHeight) / 2;

                            // Crop đúng kích thước 480x922
                            using (var cropped = new Bitmap(targetWidth, targetHeight))
                            using (var g = Graphics.FromImage(cropped))
                            {
                                g.DrawImage(resized, new Rectangle(0, 0, targetWidth, targetHeight),
                                    new Rectangle(x, y, targetWidth, targetHeight), GraphicsUnit.Pixel);

                                using (var outputStream = new MemoryStream())
                                {
                                    cropped.Save(outputStream, ImageFormat.Jpeg);
                                    return Convert.ToBase64String(outputStream.ToArray());
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    NLogLogger.Info(new string[] { "ImageHelper", "ImgResize", "Base64 Invalid", e.Message, bankId });
                }

                return String.Empty;
            }
            public static string ImgResizeNAB(string base64Image, string bankId)
            {
                try
                {
                    Random random = new Random();

                    // Convert Base64 to Image
                    byte[] imageBytesFromBase64 = Convert.FromBase64String(base64Image);
                    System.Drawing.Image image;
                    using (MemoryStream ms = new MemoryStream(imageBytesFromBase64))
                    {
                        image = System.Drawing.Image.FromStream(ms);
                    }
                    if (image.Height > 400)
                    {
                        var w = 300;
                        //NLogLogger.Info(new string[] { "ImageHelper", "ImgResize", w.ToString() });
                        image = (System.Drawing.Image)(new Bitmap(image, new System.Drawing.Size(w, 400)));
                    }


                    // Convert Image back to Base64
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.Save(ms, ImageFormat.Jpeg);
                        byte[] imageBytes = ms.ToArray();
                        string base64ImageRes = Convert.ToBase64String(imageBytes);
                        return base64ImageRes;
                    }
                }
                catch (Exception e)
                {
                    NLogLogger.Info(new string[] { "ImageHelper", "ImgResize", "Base64 Invalid", e.Message, bankId });
                }

                return String.Empty;
            }
            public static string ImgResize2(string base64Image, string bankId)
            {
                try
                {
                    Random random = new Random();

                    // Convert Base64 to Image
                    byte[] imageBytesFromBase64 = Convert.FromBase64String(base64Image);
                    System.Drawing.Image image;
                    using (MemoryStream ms = new MemoryStream(imageBytesFromBase64))
                    {
                        image = System.Drawing.Image.FromStream(ms);
                    }
                    //if (image.Height > 960)
                    //{
                    //    var w = (int)(image.Width * 960 / image.Height);
                    //    //NLogLogger.Info(new string[] { "ImageHelper", "ImgResize", w.ToString() });
                    //    image = (System.Drawing.Image)(new Bitmap(image, new System.Drawing.Size(w, 960)));
                    //}


                    // Convert Image back to Base64
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.Save(ms, ImageFormat.Jpeg);
                        byte[] imageBytes = ms.ToArray();
                        string base64ImageRes = Convert.ToBase64String(imageBytes);
                        return base64ImageRes;
                    }
                }
                catch (Exception e)
                {
                    NLogLogger.Info(new string[] { "ImageHelper", "ImgResize2", "Base64 Invalid", e.Message, bankId });
                }

                return String.Empty;
            }
            public static string AddLine(string base64Image, string bankId)
            {
                try
                {
                    if (base64Image.Contains(","))
                    {
                        base64Image = base64Image.Substring(base64Image.IndexOf(",") + 1);
                    }

                    byte[] imageBytesFromBase64 = Convert.FromBase64String(base64Image);

                    using (MemoryStream inputMs = new MemoryStream(imageBytesFromBase64))
                    using (System.Drawing.Image image = System.Drawing.Image.FromStream(inputMs))
                    using (Graphics graphics = Graphics.FromImage(image))
                    {
                        Random random = new Random();

                        int startX = random.Next(0, image.Width);
                        int startY = random.Next(0, Math.Min(101, image.Height));

                        double angle = random.NextDouble() * 2 * Math.PI;
                        int length = 10;

                        int endX = startX + (int)(length * Math.Cos(angle));
                        int endY = startY + (int)(length * Math.Sin(angle));

                        endX = Math.Max(0, Math.Min(image.Width - 1, endX));
                        endY = Math.Max(0, Math.Min(image.Height - 1, endY));

                        using (Pen pen = new Pen(Color.DarkGray, 3))
                        {
                            graphics.DrawLine(pen, new Point(startX, startY), new Point(endX, endY));
                        }

                        using (MemoryStream outputMs = new MemoryStream())
                        {
                            image.Save(outputMs, ImageFormat.Jpeg);
                            return Convert.ToBase64String(outputMs.ToArray());
                        }
                    }
                }
                catch (Exception e)
                {
                    NLogLogger.Info(new string[] { "ImageHelper", "AddLine", "Base64 Invalid", e.Message, bankId });
                }

                return string.Empty;
            }
            public static string AddLineV2(string base64Image, string bankId)
            {
                try
                {



                    Bitmap image = Base64ToImage(base64Image);
                    var newimage = DrawRandomRedDot(image);
                    //var newimage = image;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        newimage.Save(ms, ImageFormat.Jpeg);
                        byte[] imageBytes = ms.ToArray();
                        string base64ImageRes = Convert.ToBase64String(imageBytes);
                        return base64ImageRes;
                    }
                }
                catch (Exception e)
                {
                    NLogLogger.Info(new string[] { "ImageHelper", "AddLine2", "Base64 Invalid", e.Message, bankId });
                }

                return String.Empty;
            }

            public static Bitmap Base64ToImage(string base64String)
            {
                // 1. Xử lý prefix nếu có
                if (base64String.Contains(","))
                {
                    base64String = base64String.Substring(base64String.IndexOf(",") + 1);
                }

                // 2. Decode base64 → byte[]
                byte[] byteBuffer = Convert.FromBase64String(base64String);

                // 3. Sử dụng using để quản lý stream an toàn
                using (MemoryStream memoryStream = new MemoryStream(byteBuffer))
                {
                    // 4. Tạo bitmap
                    using (Image img = Image.FromStream(memoryStream))
                    {
                        // 5. Clone ra bitmap mới (tách khỏi stream)
                        return new Bitmap(img);
                    }
                }
            }

            public static Bitmap DrawRandomRedDot(Bitmap bitmap)
            {
                // Tạo bản sao có thể chỉnh sửa
                Bitmap resultBitmap = new Bitmap(bitmap);

                int width = resultBitmap.Width;
                int height = resultBitmap.Height;

                Random random = new Random();
                int xCenter = random.Next(10, width - 10);
                int yCenter = random.Next(10, height - 10);

                // Vẽ điểm tròn đỏ với bán kính 10px
                for (int dx = -10; dx <= 10; dx++)
                {
                    for (int dy = -10; dy <= 10; dy++)
                    {
                        if (dx * dx + dy * dy <= 100) // bán kính <= 10
                        {
                            int x = xCenter + dx;
                            int y = yCenter + dy;

                            if (x >= 0 && x < width && y >= 0 && y < height)
                            {
                                resultBitmap.SetPixel(x, y, Color.Red); // tương đương -65536
                            }
                        }
                    }
                }

                return resultBitmap;
            }


        }



    }

}