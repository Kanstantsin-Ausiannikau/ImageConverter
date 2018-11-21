using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace ImageConverter
{
    public partial class MainForm : Form
    {

        ImageCollection images;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (ofpLoad.ShowDialog()==DialogResult.OK)
            {
                images = new ImageCollection(ofpLoad.FileNames);

                if (dgvPaths.ColumnCount == 0)
                {
                    dgvPaths.ColumnCount = 1;
                    dgvPaths.Columns[0].Name = "Файлы";
                    


                    DataGridViewCheckBoxColumn checkedImage = new DataGridViewCheckBoxColumn();
                    dgvPaths.Columns.Insert(0, checkedImage);

                    dgvPaths.Columns[1].Width = 300;
                    
                }

                dgvPaths.Rows.Clear();

                foreach (string item in ofpLoad.FileNames)
                {
                    string[] row = new string[] { "true", item };
                    dgvPaths.Rows.Add(row);
                }
            }
        }

        private void dgvPaths_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Image previewImage = null;

            if (e.RowIndex < images.Lenght)
            {
                previewImage = Converter.GetWatermarkedImage(
                    images[e.RowIndex], 
                    int.Parse(txtWidth.Text), 
                    int.Parse(txtNewHeight.Text), 
                    new Bitmap(Environment.CurrentDirectory+ @"\water.png"));
            }

            pbImage.Image = previewImage;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string path = folderBrowserDialog.SelectedPath;

                foreach (DataGridViewRow dr in dgvPaths.Rows)
                {
                    if (Convert.ToBoolean(dr.Cells[0].Value))
                    {
                        ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

                        System.Drawing.Imaging.Encoder myEncoder =
        System.Drawing.Imaging.Encoder.Quality;

                        EncoderParameters myEncoderParameters = new EncoderParameters(1);

                        EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder,
                            100L);

                        myEncoderParameters.Param[0] = myEncoderParameter;


                        Image image  = Converter.GetWatermarkedImage(
                                new Bitmap((string)dr.Cells[1].Value),
                                int.Parse(txtWidth.Text),
                                int.Parse(txtNewHeight.Text),
                                new Bitmap(Environment.CurrentDirectory + @"\water.png"));

                        image.Save(path + @"\" + Path.GetFileName((string)dr.Cells[1].Value), jgpEncoder, myEncoderParameters);
                    }
                }
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            frmSettings settings = new frmSettings();

            settings.ShowDialog();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            List<bool> checkedList = new List<bool>();
            List<string> linkList = new List<string>();
            foreach (DataGridViewRow dr in dgvPaths.Rows)
            {
                if (Convert.ToBoolean(dr.Cells[0].Value))
                {
                    checkedList.Add(true);

                    Bitmap image = Converter.GetWatermarkedImage(
                            images[dr.Cells[1].RowIndex],
                            int.Parse(txtWidth.Text),
                            int.Parse(txtNewHeight.Text),
                            new Bitmap(Environment.CurrentDirectory + @"\water.png"));

                    bool isSended = SendToFtp(
                        image,
                        Properties.Settings.Default.FtpPath,
                        Path.GetFileName((string)dr.Cells[1].Value),
                        Properties.Settings.Default.FtpName,
                        Properties.Settings.Default.FtpPassword);

                    

                    if (!isSended)
                    {
                        MessageBox.Show("Не удалось загрузить файл " + (string)dr.Cells[1].Value);

                        linkList.Add(null);
                    }
                    else
                    {
                        linkList.Add(String.Format("{0}docs/{1}", Properties.Settings.Default.FtpPath, Path.GetFileName((string)dr.Cells[1].Value)).Replace("ftp://", "http://"));
                    }
                }
                else
                {
                    checkedList.Add(false);
                    linkList.Add(null);
                }
            }
            MessageBox.Show("Загрузка завершена");

            if (MessageBox.Show("Сохранить пути к картинкам в Excel-файл?", "Сохранение в Excel", MessageBoxButtons.YesNo)==DialogResult.Yes)
            {
                ExcelData.SaveDataToExcel(checkedList, linkList);
            }
        }

        private bool SendToFtp(Bitmap image, string ftpPath, string fileName, string ftpName, string ftpPassword)
        {
            if (image==null)
            {
                return false;
            }

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(@String.Format("{0}/{1}",ftpPath, fileName));
            //request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = new NetworkCredential(ftpName, ftpPassword);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                return true;
            }
            catch (WebException ex)
            {
                FtpWebRequest sendRequest = (FtpWebRequest)WebRequest.Create(@String.Format("{0}/{1}", ftpPath, fileName));
                //request.Method = WebRequestMethods.Ftp.UploadFile;

                sendRequest.Credentials = new NetworkCredential(ftpName, ftpPassword);
                sendRequest.Method = WebRequestMethods.Ftp.UploadFile;

                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode ==
                    FtpStatusCode.ActionNotTakenFileUnavailable)
                {

                    System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();

                    byte[] fileContents;

                    Stream st = new MemoryStream();
                    image.Save(st, System.Drawing.Imaging.ImageFormat.Jpeg);

                    using (StreamReader sourceStream = new StreamReader(st))
                    {
                        fileContents = (byte[])converter.ConvertTo(image, typeof(byte[]));
                    }

                    sendRequest.ContentLength = fileContents.Length;

                    using (Stream requestStream = sendRequest.GetRequestStream())
                    {
                        requestStream.Write(fileContents, 0, fileContents.Length);
                    }

                    using (FtpWebResponse response1 = (FtpWebResponse)sendRequest.GetResponse())
                    {
                        if (response1.StatusCode == FtpStatusCode.ClosingData)
                        {
                            return true;
                        }
                    }
                }
            }




            //System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();

            //byte[] fileContents;

            //Stream st = new MemoryStream();
            //image.Save(st, System.Drawing.Imaging.ImageFormat.Jpeg);

            //using (StreamReader sourceStream = new StreamReader(st))
            //{
            //    fileContents = (byte[])converter.ConvertTo(image, typeof(byte[]));
            //}

            //request.ContentLength = fileContents.Length;

            //using (Stream requestStream = request.GetRequestStream())
            //{
            //    requestStream.Write(fileContents, 0, fileContents.Length);
            //}

            //using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            //{
            //    if (response.StatusCode==FtpStatusCode.ClosingData)
            //    {
            //        return true;
            //    }
            //}

            return false;
        }

        private void btnLoadExcel_Click(object sender, EventArgs e)
        {
            if (ofdLoadFromExcel.ShowDialog() == DialogResult.OK)
            {
                var links = ExcelData.GetDataFromExcel(ofdLoadFromExcel.FileName);

                if (dgvPaths.ColumnCount == 0)
                {
                    dgvPaths.ColumnCount = 1;
                    dgvPaths.Columns[0].Name = "Файлы";



                    DataGridViewCheckBoxColumn checkedImage = new DataGridViewCheckBoxColumn();
                    dgvPaths.Columns.Insert(0, checkedImage);

                    dgvPaths.Columns[1].Width = 300;

                }

                dgvPaths.Rows.Clear();

                foreach (string item in links)
                {
                    string[] row = new string[] { "true", item };
                    dgvPaths.Rows.Add(row);
                }

                if (links.Count > 0)
                {
                    images = new ImageCollection(links);
                }

            }
        }
    }
}
