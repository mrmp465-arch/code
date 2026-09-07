using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;


namespace SMS.CMS.Handler
{
    public class CaptchaImage : IHttpHandler, IRequiresSessionState
    {
        private FontFamily[] fonts = {


           new FontFamily("Arial"),
           new FontFamily("Tahoma"),
           new FontFamily("Calibri"),
           new FontFamily("Verdana"),
        };
        public void ProcessRequest(HttpContext context)
        {
            MemoryStream memStream = new MemoryStream();


            var rnd = new Random();
            //set name of capchar session
            //const String sessionCaptcharName = "CAPTCHA";
            //set number of character for captchar
            const int numberOfChar = 3;
            //context.Session[sessionCaptcharName] = GetNumber(numberOfChar);
            string checkNumber = GetNumber(numberOfChar);
            //NLogLogger.Info(checkNumber);
            context.Session["Captcha"] = checkNumber;

            int width;
            int.TryParse(context.Request.Params["w"], out width);
            if (width <= 0 || width > 800) width = 40;

            int height;
            int.TryParse(context.Request.Params["h"], out height);
            if (height <= 0 || height > 600) height = 22;

            //Generate an image from the text stored in session  
            Bitmap CaptchaImg = new Bitmap(120, 50);
            Graphics Graphic = Graphics.FromImage(CaptchaImg);
            Graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            //Set height and width of captcha image  
            //var br = new SolidBrush(Color.White);
            var color1 = rnd.Next(220, 255);
            var color2 = rnd.Next(220, 255);
            var color3 = rnd.Next(220, 255);
            var br = new SolidBrush(Color.FromArgb(color1, color2, color3));
            Graphic.FillRectangle(br, 0, 0, 120, 50);
            var brfill = new SolidBrush(Color.FromArgb(rnd.Next(150, 200), rnd.Next(150, 200), rnd.Next(150, 200)));
            for (int i = 0; i < 1000; i++)
            {
                Graphic.FillRectangle(brfill, rnd.Next(0, 120), rnd.Next(0, 50), 1, 1);
            }
            var strFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,

                LineAlignment = StringAlignment.Center,
            };
            SizeF measured = new SizeF(0, 0);
            SizeF workingSize = new SizeF(width, height);
            const int fontSize = 27;
            const int padding = fontSize - 10;
            for (int i = 0; i < checkNumber.Length; i++)
            {
                int h = rnd.Next(6, 6);

                // Set up the text font.
                int emSize = rnd.Next(fontSize - 3, fontSize + 3);
                FontFamily family = fonts[rnd.Next(0, fonts.Length - 1)];
                Font cFont = new Font(family, emSize);
                var brush1 = rnd.Next(0, 125);
                var brush2 = rnd.Next(0, 125);
                var brush3 = rnd.Next(0, 125);
                System.Drawing.Brush brush = new System.Drawing.SolidBrush(Color.FromArgb(brush1, brush2, brush3));
                //var cFont = new Font("Verdana", rnd.Next(fontSize, fontSize + 2), FontStyle.Italic);
                Graphic.DrawString(checkNumber[i].ToString(), cFont, brush,
                              new RectangleF((i + 1) * padding, h, (i + 2) * padding, h + height), strFormat);
            }
            ImageCodecInfo codec = GetEncoderInfo("image/jpeg");

            // set image quality
            var eps = new EncoderParameters();
            eps.Param[0] = new EncoderParameter(Encoder.Quality, (long)95);
            CaptchaImg.Save(context.Response.OutputStream, codec, eps);
            CaptchaImg.Dispose();
            br.Dispose();
        }
        private static String GetNumber(int l)
        {

            const String key = "ACDEFGHKMNPQRSTUXY45679";
            int keyLenght = key.Length;
            var rnd = new Random();
            string s = String.Empty;
            for (int i = 0; i < l; i++)
                s = s + key[rnd.Next(keyLenght)];
            return s;
        }
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] myEncoders =
                ImageCodecInfo.GetImageEncoders();

            return myEncoders.FirstOrDefault(myEncoder => myEncoder.MimeType == mimeType);
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}