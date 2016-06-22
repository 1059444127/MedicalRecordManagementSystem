using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 哈医大三院病历管理系统
{
    public partial class ImageWindow : Form
    {
        Image image;
        public ImageWindow(Image srcImage)
        {
            this.image = srcImage;
            InitializeComponent();
        }

        private void ImageWindow_Load(object sender, EventArgs e)
        {
            this.pictureBox1.Image = this.image;
        }

        private void buttonPrintImage_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = this.printDocument1;
                if (printDialog.ShowDialog() != DialogResult.OK)
                {
                    throw new Exception();
                }
                /*
                PageSetupDialog pageSetupDialog = new PageSetupDialog();
                pageSetupDialog.Document = printDocument1;
                if (pageSetupDialog.ShowDialog() != DialogResult.OK)
                {
                    throw new Exception();
                }
                */
                PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
                printPreviewDialog.Document = this.printDocument1;
                printPreviewDialog.ShowDialog();

            }
            catch (Exception)
            {
                MessageBox.Show("打印已经取消");
            }
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            //绘制图片
            if (this.image != null)
            {
                g.DrawImage(this.image, e.PageBounds.Left, e.PageBounds.Top, e.PageBounds.Width, e.PageBounds.Height);
            }
            e.HasMorePages = false;
        }
    }
}
