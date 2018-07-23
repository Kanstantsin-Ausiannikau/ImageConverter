using System;
using System.Drawing;

namespace ImageConverter
{
    class ImageCollection
    {
        string[] _origFilePaths;

        public ImageCollection(string[] files)
        {
            if (files == null)
            {
                throw new ArgumentException("Convert parameters is incorrect");
            }

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
                if (index>=_origFilePaths.Length)
                {
                    return null;
                }
                return new Bitmap(_origFilePaths[index]); // your source images - assuming they're the same size
            }
        }
    }
}
