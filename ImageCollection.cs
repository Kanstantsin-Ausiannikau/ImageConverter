using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageConverter
{




    class ImageCollection
    {
        string[] _origFilePaths;

        public ImageCollection(string[] files)
        {
            _origFilePaths = files;
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
                ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

                return null;
            }
        }
    }
}
