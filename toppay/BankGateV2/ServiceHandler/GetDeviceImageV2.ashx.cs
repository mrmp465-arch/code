using Libs.API;
using Libs.BankGate.Entity;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Script.Serialization;

namespace BankGateV2.ServiceHandler
{
    /// <summary>
    /// Summary description for GetBankImage
    /// </summary>
    public class GetDeviceImageV2 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var bankId = HttpContext.Current.Request.QueryString["AppDeviceId"];

            try
            {
                //NLogLogger.Info(new string[] { "GetImg ", bankId.ToString() });
                if (!string.IsNullOrEmpty(bankId))
                {
                    var bank = new BankAccounts().GetByAppDeviceId(bankId);
                    if (bank != null)
                    {
                        var profileImages = serializer.Deserialize<List<BankProfileImage>>(bank.ProfileImage).Where(x => x.Base64 != "/cmspay/content/noimage.png").ToList();
                        if (profileImages != null)
                        {
                            if (profileImages.Count > 0)
                            {
                                var random = new Random();
                                var randomIndex = random.Next(profileImages.Count);
                                var file = profileImages[randomIndex].Base64;
                                if (bank.BankCode != "TPB")
                                {
                                    //var fileWatermark = ImageHelper.ImgResize(file, bankId);
                                    //fileWatermark = ImageHelper.AddLineV2(fileWatermark, bankId);
                                    context.Response.Write(file);
                                }
                                else
                                {
                                    //var fileWatermark = ImageHelper.ImgResize2(file, bankId);
                                    var fileWatermark = ImageHelper.AddLine(file, bankId);
                                    context.Response.Write(fileWatermark);
                                }

                                return;
                            }
                            context.Response.Write("");
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
                    Random random = new Random();

                    // Convert Base64 to Image
                    byte[] imageBytesFromBase64 = Convert.FromBase64String(base64Image);
                    System.Drawing.Image image;
                    using (MemoryStream ms = new MemoryStream(imageBytesFromBase64))
                    {
                        image = System.Drawing.Image.FromStream(ms);
                    }

                    // Create a Graphics object from the Image
                    using (Graphics graphics = Graphics.FromImage(image))
                    {
                        // Random starting point within the bounds (0, 0) to (maxWidth, 100)
                        int startX = random.Next(0, image.Width);
                        int startY = random.Next(0, 101);

                        // Random angle for the line
                        double angle = random.NextDouble() * 2 * Math.PI; // Angle in radians

                        // Length of the line (approximately 10 pixels)
                        int length = 10;

                        // Calculate the end point using trigonometry
                        int endX = startX + (int)(length * Math.Cos(angle));
                        int endY = startY + (int)(length * Math.Sin(angle));

                        // Ensure the end point is within the image bounds
                        endX = Math.Max(0, Math.Min(image.Width, endX));
                        endY = Math.Max(0, Math.Min(image.Height, endY));

                        // Draw the line
                        Pen pen = new Pen(Color.DarkGray, 3); // Red line with 5-pixel thickness
                        graphics.DrawLine(pen, new Point(startX, startY), new Point(endX, endY));
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
                    NLogLogger.Info(new string[] { "ImageHelper", "AddLine", "Base64 Invalid", e.Message, bankId });
                }

                return String.Empty;
            }
            public static Bitmap Base64ToImage(string
                                           base64String)
            {
                Bitmap bmpReturn = null;


                byte[] byteBuffer = Convert.FromBase64String(base64String);
                MemoryStream memoryStream = new MemoryStream(byteBuffer);

                memoryStream.Position = 0;
                bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);

                memoryStream.Close();
                memoryStream = null;
                byteBuffer = null;

                return bmpReturn;
            }

            public static string AddLineV2(string base64Image, string bankId)
            {
                try
                {
                    Random random = new Random();


                    Bitmap image = Base64ToImage(base64Image);
                    using (Graphics graphics = Graphics.FromImage(image))
                    {
                        // Random starting point within the bounds (0, 0) to (maxWidth, 100)
                        int startX = random.Next(0, image.Width);
                        int startY = random.Next(0, 101);

                        // Random angle for the line
                        double angle = random.NextDouble() * 2 * Math.PI; // Angle in radians

                        // Length of the line (approximately 10 pixels)
                        int length = 10;

                        // Calculate the end point using trigonometry
                        int endX = startX + (int)(length * Math.Cos(angle));
                        int endY = startY + (int)(length * Math.Sin(angle));

                        // Ensure the end point is within the image bounds
                        endX = Math.Max(0, Math.Min(image.Width, endX));
                        endY = Math.Max(0, Math.Min(image.Height, endY));

                        // Draw the line
                        Pen pen = new Pen(Color.DarkGray, 3); // Red line with 5-pixel thickness
                        graphics.DrawLine(pen, new Point(startX, startY), new Point(endX, endY));
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
                    NLogLogger.Info(new string[] { "ImageHelper", "AddLine", "Base64 Invalid", e.Message, bankId });
                }

                return String.Empty;
            }
            public static Bitmap AddLineBM(Bitmap image)
            {

                Random random = new Random();
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    // Random starting point within the bounds (0, 0) to (maxWidth, 100)
                    int startX = random.Next(0, image.Width);
                    int startY = random.Next(0, 101);

                    // Random angle for the line
                    double angle = random.NextDouble() * 2 * Math.PI; // Angle in radians

                    // Length of the line (approximately 10 pixels)
                    int length = 10;

                    // Calculate the end point using trigonometry
                    int endX = startX + (int)(length * Math.Cos(angle));
                    int endY = startY + (int)(length * Math.Sin(angle));

                    // Ensure the end point is within the image bounds
                    endX = Math.Max(0, Math.Min(image.Width, endX));
                    endY = Math.Max(0, Math.Min(image.Height, endY));

                    // Draw the line
                    Pen pen = new Pen(Color.DarkGray, 3); // Red line with 5-pixel thickness
                    graphics.DrawLine(pen, new Point(startX, startY), new Point(endX, endY));
                }

                // Convert Image back to Base64
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Jpeg);
                    byte[] imageBytes = ms.ToArray();
                    string base64ImageRes = Convert.ToBase64String(imageBytes);
                    Bitmap bmpReturn = null;


                    byte[] byteBuffer = Convert.FromBase64String(base64ImageRes);
                    MemoryStream memoryStream = new MemoryStream(byteBuffer);

                    memoryStream.Position = 0;
                    bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);

                    memoryStream.Close();
                    memoryStream = null;
                    byteBuffer = null;

                    return bmpReturn;

                }

            }
            public static Bitmap drawRandomRedDot(Bitmap bitmap)
            {
                Bitmap resultBitmap = (Bitmap)bitmap.Clone();
                Random random = new Random();
                int startX = random.Next(0, resultBitmap.Width);
                int startY = random.Next(0, 101);

                // Random angle for the line
                double angle = random.NextDouble() * 2 * Math.PI; // Angle in radians

                // Length of the line (approximately 10 pixels)
                int length = 10;

                // Calculate the end point using trigonometry
                int endX = startX + (int)(length * Math.Cos(angle));
                int endY = startY + (int)(length * Math.Sin(angle));

                // Ensure the end point is within the image bounds
                endX = Math.Max(0, Math.Min(resultBitmap.Width, endX));
                endY = Math.Max(0, Math.Min(resultBitmap.Height, endY));

                resultBitmap.SetPixel(endX, endY, Color.Gray);
                return resultBitmap;
            }
           
        }
    }
}