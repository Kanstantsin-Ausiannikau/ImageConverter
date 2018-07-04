using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageConverter
{
    class ImageCollection
    {
        string[] _origFilePaths;
        string _waterMarksPath;
        int _vertical;
        int _horisontal;

        public ImageCollection(string[] files, string waterMarksPath, int vertical, int horisontal)
        {
            if (files == null||vertical<=0||horisontal<=0)
            {
                throw new ArgumentException("Convert parameters is incorrect");
            }

            _origFilePaths = files;
            _waterMarksPath = waterMarksPath;
            _vertical = vertical;
            _horisontal = horisontal;
        }

        public string[] GetPaths()
        {
            return _origFilePaths;
        }

        public int Lenght
        {
            get { return _origFilePaths.Length; }
        }

        public Image this[int index]
        {
            get
            {
                Bitmap source1 = new Bitmap(_origFilePaths[index]); // your source images - assuming they're the same size
                Bitmap source2 = new Bitmap(_waterMarksPath);
                var target = new Bitmap(source1.Width, source1.Height, PixelFormat.Format32bppArgb);
                var graphics = Graphics.FromImage(target);
                graphics.CompositingMode = CompositingMode.SourceOver; // this is the default, but just to be clear

                graphics.DrawImage(source1, 0, 0);
                graphics.DrawImage(source2, 0, 0);

                target.Save("filename.png", ImageFormat.Jpeg);

                return target;
            }
        }
    }
}
