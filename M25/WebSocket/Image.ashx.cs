using Libs.Utils;
using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Net.WebSockets;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;
using WebGrease.Css.Ast.Selectors;
using System.Web.Script.Serialization;


namespace BankSocket
{
    /// <summary>
    /// Summary description for Image
    /// </summary>
    public class Image : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var base64String = "";
            base64String = Convert.ToBase64String(System.IO.File.ReadAllBytes("C:\\WebSocket\\BankSocket\\Images\\seab01.jpg"));

            base64String = AddLine(base64String);
            context.Response.Write(base64String);
        }
        public static string AddLine(string base64Image)
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
                NLogLogger.Info(new string[] { "ImageHelper", "AddLine", "Base64 Invalid", e.Message });
            }

            return String.Empty;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}