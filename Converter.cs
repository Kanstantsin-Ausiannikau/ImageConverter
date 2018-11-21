using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImageConverter
{
    class Converter
    {
        public static Bitmap GetWatermarkedImage(Image image,int width, int height, Image waterMark)
        {
            if (image == null)
            {
                return null;
            }


            Bitmap resizedWaterMark = ResizeImage(waterMark, width, height, waterMark.HorizontalResolution, waterMark.VerticalResolution);

            Bitmap resizedImage = ResizeImage(image, width, height, waterMark.HorizontalResolution, waterMark.VerticalResolution);


            var target = new Bitmap(resizedImage.Width, resizedImage.Height, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(target);
            graphics.CompositingMode = CompositingMode.SourceOver; // this is the default, but just to be clear

            graphics.DrawImage(resizedImage, 0, 0);
            graphics.DrawImage(resizedWaterMark, 0, 0);

            return target;
        }

        private static Bitmap ResizeImage(Image image, int width, int height, float xDpi, float yDpi)
        {
            if (image == null)
            {
                return null;
            }

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(xDpi, yDpi);

            float kw = (float)image.Width / width;
            float kh = (float)image.Height / height;

            bool isVertStretch = kw < kh;

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);

                    if (isVertStretch)
                    {
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, height*kw, GraphicsUnit.Pixel, wrapMode);
                    }
                    else
                    {
                        graphics.DrawImage(image, destRect, 0, 0, width*kh , image.Height , GraphicsUnit.Pixel, wrapMode);
                    }
                }
            }
            return destImage;
        }
    }
}
