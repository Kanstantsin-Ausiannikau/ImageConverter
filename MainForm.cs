using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                images = new ImageCollection(ofpLoad.FileNames, @"d:\temp\water.png", 400, 400);

                dgvPaths.ColumnCount = 1;
                dgvPaths.Columns[0].Name = "Файлы";

                DataGridViewCheckBoxColumn checkedImage = new DataGridViewCheckBoxColumn();
                dgvPaths.Columns.Insert(0, checkedImage);

                foreach (string item in ofpLoad.FileNames)
                {
                    string[] row = new string[] { "true", item };
                    dgvPaths.Rows.Add(row);
                }
            }
        }

        private void dgvPaths_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //pbImage.Image = Image.FromFile((string)dgvPaths.Rows[e.RowIndex].Cells[1].Value);
            Image previewImage = Converter.GetWatermarkedImage(images[e.RowIndex], int.Parse(txtWidth.Text), int.Parse(txtNewHeight.Text), new Bitmap(@"c:\Projects\ImageConverter\Samples\water.png"));

            pbImage.Image = previewImage;
        }
    }
}
