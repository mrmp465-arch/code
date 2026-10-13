using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace Libs.BankDirect.GPay
{

    public static class QrHelper
    {
        public static string EmvToBase64Compact(string emv, bool includePrefix = true, int pixelsPerModule = 3)
        {
            using (var generator = new QRCodeGenerator())
            using (var data = generator.CreateQrCode(emv, QRCodeGenerator.ECCLevel.L))
            using (var qrCode = new QRCode(data))
            using (Bitmap bmp = qrCode.GetGraphic(pixelsPerModule))
            using (Bitmap cropped = CropWhiteBorder(bmp))
            using (var ms = new MemoryStream())
            {
                cropped.Save(ms, ImageFormat.Png);
                string base64 = Convert.ToBase64String(ms.ToArray());
                return includePrefix ? "data:image/png;base64," + base64 : base64;
            }
        }
        public static string EmvToBase64_500px(string emv, bool includePrefix = true)
        {
            using (var generator = new QRCodeGenerator())
            using (var data = generator.CreateQrCode(emv, QRCodeGenerator.ECCLevel.M))
            using (var qrCode = new QRCode(data))
            using (Bitmap qr = qrCode.GetGraphic(10))
            using (Bitmap finalImage = new Bitmap(500, 500))
            using (Graphics g = Graphics.FromImage(finalImage))
            using (var ms = new MemoryStream())
            {
                g.Clear(Color.White);

                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                g.DrawImage(qr, 0, 0, 500, 500);

                finalImage.Save(ms, ImageFormat.Png);

                string base64 = Convert.ToBase64String(ms.ToArray());

                return includePrefix
                    ? "data:image/png;base64," + base64
                    : base64;
            }
        }

        private static Bitmap CropWhiteBorder(Bitmap source)
        {
            int left = source.Width, top = source.Height, right = 0, bottom = 0;

            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    if (source.GetPixel(x, y).ToArgb() != Color.White.ToArgb())
                    {
                        if (x < left) left = x;
                        if (x > right) right = x;
                        if (y < top) top = y;
                        if (y > bottom) bottom = y;
                    }
                }
            }

            if (right < left || bottom < top) return new Bitmap(source);

            int width = right - left + 1;
            int height = bottom - top + 1;

            var target = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(source,
                    new Rectangle(0, 0, width, height),
                    new Rectangle(left, top, width, height),
                    GraphicsUnit.Pixel);
            }

            return target;
        }
    }

}
