using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;

namespace ImageConverter
{
    class ImageCollection
    {
        string[] _origFilePaths;

        Hashtable hash = new Hashtable();

        public ImageCollection(string[] files)
        {
            _origFilePaths = files ?? throw new ArgumentException("Convert parameters is incorrect");
        }

        public ImageCollection(List<string> files)
        {
            if (files == null)
            {
                throw new ArgumentException("Convert parameters is incorrect");
            }

            _origFilePaths = files.ToArray();
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
                if ((index>=_origFilePaths.Length)||(string.IsNullOrEmpty(_origFilePaths[index])))
                {
                    return null;
                }

                if (_origFilePaths[index].IndexOf("http") == 0)
                {
                    if (hash.ContainsKey(_origFilePaths[index]))
                    {
                        return (Bitmap)hash[_origFilePaths[index]];
                    }
                    else
                    {

                        WebClient client = new WebClient();
                        try
                        {
                            Stream stream = client.OpenRead(_origFilePaths[index]);

                            Bitmap bitmap = new Bitmap(stream);

                            stream.Flush();
                            stream.Close();
                            client.Dispose();

                            hash.Add(_origFilePaths[index], bitmap);

                            return bitmap;
                        }
                        catch (WebException)
                        {
                            return null;
                        }
                        catch (ArgumentException)
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    return new Bitmap(_origFilePaths[index]); // your source images - assuming they're the same size
                }
            }
        }
    }
}
