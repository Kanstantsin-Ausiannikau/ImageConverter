using System.Drawing;

namespace ImageConverter
{
    class WaterMark
    {
        Bitmap _watermark;

        public WaterMark(string pathToWaterMark)
        {
            if (!string.IsNullOrEmpty(pathToWaterMark))
            {
                _watermark = new Bitmap(pathToWaterMark);
            }
        }

        public Image GetWaterMark()
        {
            return _watermark;
        }
    }
}
