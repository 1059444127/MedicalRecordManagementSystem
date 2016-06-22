using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace 哈医大三院病历管理系统
{
    public partial class ScanWait : Form
    {

        public String ImageFilePath = "";
        public String ScanImageFolderPath;

        public ScanWait(String scanImageFolderPath)
        {
            this.ScanImageFolderPath = scanImageFolderPath;
            InitializeComponent();
        }

        private void ScanWait_Load(object sender, EventArgs e)
        {
            this.pictureBox1.Image = Image.FromFile("scaning.gif");
            this.backgroundWorker1.RunWorkerAsync();
            
        }

        private void ScanWait_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            String ImageFileDirectory = this.ScanImageFolderPath;// "./图片临时文件夹/";
            if (!Directory.Exists(ImageFileDirectory))
            {
                Directory.CreateDirectory(ImageFileDirectory);
            }
            String[] OldFileNames = Directory.GetFiles(ImageFileDirectory, "*.jpg");
            while (true)
            {
                String[] NewFileNames = Directory.GetFiles(ImageFileDirectory, "*.jpg");
                bool FindNew = false;
                for (int i = 0; i < NewFileNames.Length; i++)
                {
                    if (OldFileNames.Contains(NewFileNames[i]))
                    {
                        continue;
                    }
                    else
                    {
                        e.Result = NewFileNames[i];
                        FindNew = true;
                        break;
                    }
                }
                if (FindNew)
                {
                    break;
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.ImageFilePath = (String)e.Result;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }

    public class ScanImage
    {
        ScanWait ParentWindow;
        public ScanImage(ScanWait ParentWindow)
        {
            this.ParentWindow = ParentWindow;
        }

        public void Prog()
        {

        }
    }
}
