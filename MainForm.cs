using System;
using System.Drawing;
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
                    if ((string)dr.Cells[0].Value == "true")
                    {
                        Image image  = Converter.GetWatermarkedImage(
                                new Bitmap((string)dr.Cells[1].Value),
                                int.Parse(txtWidth.Text),
                                int.Parse(txtNewHeight.Text),
                                new Bitmap(Environment.CurrentDirectory + @"\water.png"));

                        image.Save(path + @"\" + Path.GetFileName((string)dr.Cells[1].Value));
                    }
                }
            }
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            frmSettings settings = new frmSettings();

            settings.ShowDialog();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in dgvPaths.Rows)
            {
                if ((string)dr.Cells[0].Value == "true")
                {
                    Bitmap image = Converter.GetWatermarkedImage(
                            new Bitmap((string)dr.Cells[1].Value),
                            int.Parse(txtWidth.Text),
                            int.Parse(txtNewHeight.Text),
                            new Bitmap(Environment.CurrentDirectory + @"\water.png"));

                    bool isSended = SendToFtp(
                        image,
                        Properties.Settings.Default.FtpPath,
                        Path.GetFileName((string)dr.Cells[1].Value),
                        Properties.Settings.Default.FtpName,
                        Properties.Settings.Default.FtpPassword);
                }
            }
        }

        private bool SendToFtp(Bitmap image, string ftpPath, string fileName, string ftpName, string ftpPassword)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(String.Format("{0}/{1}",ftpPath, fileName));
            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = new NetworkCredential(ftpName, ftpPassword);

            byte[] fileContents;

            Stream st = new MemoryStream();
            image.Save(st, System.Drawing.Imaging.ImageFormat.Jpeg);

            using (StreamReader sourceStream = new StreamReader(st))
            {
                fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            }

            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                
            }

            throw new NotImplementedException();
        }
    }
}
