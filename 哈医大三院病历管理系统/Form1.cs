using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 哈医大三院病历管理系统
{
    public partial class MainForm : Form
    {
        String SetFilePath = "./设置.dat";
        String sPath;
        String pPath;
        String scanPath;
        int ScanDelayTime;
        String ScanDateFolderNameFormat;
        DataTable PatientDataTable;
        DataTable PatientDataTableForView;
        DataTable DataTablePatientDocument;
        List<String> TablesName = new List<string>();
        List<PatientInfo> SearchResult = new List<PatientInfo>();
        
        int NumPatientId;
        public MainForm()
        {
            InitializeComponent();
        }

        private void LoadSet()
        {
            try {
                FileStream fs = new FileStream(this.SetFilePath, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                this.sPath = br.ReadString();
                this.pPath = br.ReadString();
                this.scanPath = br.ReadString();
                this.ScanDelayTime = br.ReadInt32();
                this.ScanDateFolderNameFormat = br.ReadString();
                br.Close();
                fs.Close();
            }catch(Exception)
            {
                this.SetDefaultSet();
            }
        }

        private void SetDefaultSet()
        {
            this.sPath = "./表单/";
            this.pPath = "./图片/";
            this.scanPath = "./图片临时文件夹/";
            this.ScanDelayTime = 1000;
            this.ScanDateFolderNameFormat = "yyyyMMdd";
        }

        private void InitTable(DataTable desTable)
        {
            desTable.Columns.Add("编号", System.Type.GetType("System.String"));
            desTable.Columns.Add("姓名", System.Type.GetType("System.String"));
            desTable.Columns.Add("病案号", System.Type.GetType("System.String"));
            desTable.Columns.Add("登记日期", System.Type.GetType("System.String"));
            desTable.Columns.Add("押金", System.Type.GetType("System.String"));
            desTable.Columns.Add("返款时间", System.Type.GetType("System.String"));
            desTable.Columns.Add("备注", System.Type.GetType("System.String"));
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.LoadSet();
            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }
            if (!Directory.Exists(pPath))
            {
                Directory.CreateDirectory(pPath);
            }
            this.PatientDataTable = new DataTable("哈医大三院病历");
            this.PatientDataTableForView = new DataTable("哈医大三院病历");
            InitTable(this.PatientDataTable);
            InitTable(this.PatientDataTableForView);
            this.NumPatientId = 1;
            this.ReadTable();
            this.PatientdataGridView.DataSource = this.PatientDataTable;
            this.PatientId.Text = Convert.ToString(this.NumPatientId);
            this.PatientName.Focus();
            this.EditBoxAmount.Text = "100";
        }

        private void ReadTable(String fileName, DataTable desTable,int mode = 0)
        {
            try
            {
                FileStream fs;
                fs = new FileStream(fileName, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                try
                {
                    while (true)
                    {
                        DataRow NewRow = desTable.NewRow();
                        NewRow["编号"] = br.ReadString();
                        NewRow["姓名"] = br.ReadString();
                        NewRow["病案号"] = br.ReadString();
                        NewRow["登记日期"] = br.ReadString();
                        NewRow["押金"] = br.ReadString();
                        NewRow["返款时间"] = br.ReadString();
                        NewRow["备注"] = br.ReadString();
                        desTable.Rows.Add(NewRow);
                        if (mode == 1)
                        {
                            try
                            {
                                this.NumPatientId = Convert.ToInt32((String)NewRow["编号"]) + 1;
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                        
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("读取文件到达末尾");
                }
                br.Close();
                fs.Close();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (DirectoryNotFoundException e1)
            {
                Console.WriteLine(e1.ToString());
            }
        }
        private void ReadTable()
        {
            String filename = this.sPath + DateTime.Now.ToString("yyyy-MM-dd") + ".dat";
            ReadTable(filename,this.PatientDataTable,1);
            
        }
        private void SaveTable(String filename,DataTable desTable)
        {

            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }
            FileStream fs;
            fs = new FileStream(filename, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            for (int i = 0;i < desTable.Rows.Count;i++)
            {
                bw.Write(desTable.Rows[i]["编号"] == System.DBNull.Value ? "" : (String)desTable.Rows[i]["编号"]);
                bw.Write(desTable.Rows[i]["姓名"] == System.DBNull.Value ? "" : (String)desTable.Rows[i]["姓名"]);
                bw.Write(desTable.Rows[i]["病案号"] == System.DBNull.Value ? "" : (String)desTable.Rows[i]["病案号"]);
                bw.Write(desTable.Rows[i]["登记日期"] == System.DBNull.Value ? "" : (String)desTable.Rows[i]["登记日期"]);
                bw.Write(desTable.Rows[i]["押金"] == System.DBNull.Value ? "" : (String)desTable.Rows[i]["押金"]);
                bw.Write(desTable.Rows[i]["返款时间"] == System.DBNull.Value ? "" : (String)desTable.Rows[i]["返款时间"]);
                bw.Write(desTable.Rows[i]["备注"] == System.DBNull.Value ? "" : (String)desTable.Rows[i]["备注"]);
                bw.Flush();
            }
            bw.Close();
            fs.Close();
        }

        private void SaveTable()
        {
            String filename = sPath + this.PatientBookInDate.Value.ToString("yyyy-MM-dd") + ".dat";
            SaveTable(filename, this.PatientDataTable);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            //test
            DataTable testTable = new DataTable();
            PatientdataGridView.DataSource = testTable;

        }
        

        private void PatientDateAdd_Click(object sender, EventArgs e)
        {
            //保存图片
            String desfilename = this.pPath + this.PatientName.Text + "-" + this.HospitalId.Text + "-1.jpg";
            this.pictureBox1.Image.Save(desfilename);
            this.pictureBox1.Image.Dispose();
            this.pictureBox1.Image = null;
            //保存
            this.NumPatientId ++;
            DataRow NewRow = this.PatientDataTable.NewRow();
            NewRow["编号"] = this.PatientId.Text;
            NewRow["姓名"] = this.PatientName.Text;
            NewRow["病案号"] = this.HospitalId.Text;
            NewRow["登记日期"] = this.PatientBookInDate.Text;
            NewRow["押金"] = this.EditBoxAmount.Text;
            this.PatientDataTable.Rows.Add(NewRow);
            this.PatientId.Text = Convert.ToString(this.NumPatientId);
            this.PatientName.Text = "";
            this.HospitalId.Text = "";
            this.PatientName.Focus();
            this.SaveTable();
        }
        

        private void PatientId_Leave(object sender, EventArgs e)
        {
            try {
                this.NumPatientId = Convert.ToInt32(this.PatientId);
            }
            catch (Exception)
            {
                this.NumPatientId = 1;
            }
        }
       

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.TableList.Items.Clear();
            for (int i = 0; i < this.TablesName.Count; i++)
            {
                if (this.TablesName[i].Contains(this.textBoxFileDate.Text))
                {
                    this.TableList.Items.Add(this.TablesName[i]);
                }
            }
        }

        private void TableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PatientDataTableForView.Rows.Clear();
            String filename = this.sPath + (String)this.TableList.SelectedItem + ".dat";
            //MessageBox.Show(filename);
            this.ReadTable(filename,this.PatientDataTableForView);
            PatientDataView.DataSource = this.PatientDataTableForView;
        }

        private void MaintabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MaintabControl.SelectedTab.Text.Equals("查看病历资料"))
            {
                this.ReadTableList();
                this.TableList.Items.Clear();
                for (int i = 0;i < this.TablesName.Count;i++)
                {
                    this.TableList.Items.Add(this.TablesName[i]);
                }
            }
            else if (MaintabControl.SelectedTab.Text.Equals("设置"))
            {
                InitSetTab();
            }
        }

        private void InitSetTab()
        {
            this.textBoxDataTableFolderPath.Text = this.sPath;
            this.textBoxImageSaveFolderPath.Text = this.pPath;
            this.textBoxScanImageFolderPath.Text = this.scanPath;
            this.textBoxScanImageDelay.Text = Convert.ToString(this.ScanDelayTime);
            this.textBoxScanImageDataFolderFormat.Text = this.ScanDateFolderNameFormat;
        }

        private void ReadTableList()
        {
            String[] FilesName = Directory.GetFiles(this.sPath, "*.dat");
            this.TablesName.Clear();
            for (int i = 0; i < FilesName.Length; i++)
            {
                String TempStr = FilesName[i];
                String[] StrA = TempStr.Split(new char[] { '/', '\\' });
                TempStr = StrA[StrA.Length - 1];
                TempStr = TempStr.Substring(0, TempStr.Length - 4);
                this.TablesName.Add(TempStr);
            }
        }

        private void PatientDataView_SelectionChanged(object sender, EventArgs e)
        {
            if (this.PatientDataView.CurrentRow != null)
            {
                //1
                try {
                    String Image1FileName = this.pPath + this.PatientDataView.Rows[this.PatientDataView.CurrentRow.Index].Cells["姓名"].Value + "-" + this.PatientDataView.Rows[this.PatientDataView.CurrentRow.Index].Cells["病案号"].Value + "-1.jpg";
                    Image ImageFromFile = Image.FromFile(Image1FileName);
                    Image ImageFromMem = new Bitmap(ImageFromFile);
                    ImageFromFile.Dispose();
                    if (this.pictureBox2.Image != null)
                    {
                        this.pictureBox2.Image.Dispose();
                    }
                    this.pictureBox2.Image = ImageFromMem;
                }
                catch (FileNotFoundException)
                {
                    System.Drawing.Image img = System.Drawing.Image.FromFile("./NoImage.png");
                    System.Drawing.Image bmp = new System.Drawing.Bitmap(img);
                    img.Dispose();
                    this.pictureBox2.Image = bmp;
                }
                //2
                String filename = this.pPath + this.PatientDataView.Rows[this.PatientDataView.CurrentRow.Index].Cells["姓名"].Value + "-" + this.PatientDataView.Rows[this.PatientDataView.CurrentRow.Index].Cells["病案号"].Value + ".jpg";
                //MessageBox.Show(filename);
                if (this.PatientDataImage.Image != null)
                {
                    this.PatientDataImage.Image.Dispose();
                }
                try
                {
                    System.Drawing.Image img = System.Drawing.Image.FromFile(filename);
                    System.Drawing.Image bmp = new System.Drawing.Bitmap(img);
                    img.Dispose();
                    this.PatientDataImage.Image = bmp;
                }
                catch (FileNotFoundException)
                {
                    try
                    {
                        String filename2 = this.pPath + this.PatientDataView.Rows[this.PatientDataView.CurrentRow.Index].Cells["病案号"].Value + ".jpg";
                        System.Drawing.Image img = System.Drawing.Image.FromFile(filename2);
                        System.Drawing.Image bmp = new System.Drawing.Bitmap(img);
                        img.Dispose();
                        this.PatientDataImage.Image = img;
                    }
                    catch (FileNotFoundException)
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromFile("./NoImage.png");
                        System.Drawing.Image bmp = new System.Drawing.Bitmap(img);
                        img.Dispose();
                        this.PatientDataImage.Image = bmp;
                    }
                }
            }
        }

        private void ImageZoom_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ImageZoom.Checked)
            {
                //1
                this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                this.pictureBox2.Location = new System.Drawing.Point(0, 0);
                this.PatientDataImage.Size = this.panel2.Size;
                //2
                this.PatientDataImage.SizeMode = PictureBoxSizeMode.Zoom;
                this.PatientDataImage.Location = new System.Drawing.Point(0, 0);
                this.PatientDataImage.Size = this.ImagePanel.Size;
            }
            else
            {
                //1
                this.pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
                this.pictureBox2.Location = new System.Drawing.Point(0, 0);
                //2
                this.PatientDataImage.SizeMode = PictureBoxSizeMode.AutoSize;
                this.PatientDataImage.Location = new System.Drawing.Point(0, 0);
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            String Content = this.SearchText.Text.Trim();
            bool IsHospitalNum = true;
            for (int i = 0;i < Content.Length;i++)
            {
                if (!(Content[i] >= '0' && Content[i] <= '9'))
                {
                    IsHospitalNum = false;
                    break;
                }
            }
            this.ReadTableList();
            if (IsHospitalNum)
            {
                //MessageBox.Show("是数字");
                LoadSearchResult(Content, "");
            }
            else
            {
                //MessageBox.Show("是名字");
                LoadSearchResult("", Content);
            }
            this.NameListBox.Items.Clear();
            for (int i = 0;i < this.SearchResult.Count;i++)
            {
                this.NameListBox.Items.Add("姓名：" + this.SearchResult[i].Name + "  病案号：" + this.SearchResult[i].HospitalId );
            }
        }

        private void LoadSearchResult(String HospitalId,String PatientName)
        {
            this.SearchResult.Clear();
            for (int i = 0; i < this.TablesName.Count; i++)
            {
                try
                {
                    FileStream fs;
                    fs = new FileStream(this.sPath + this.TablesName[i] + ".dat", FileMode.Open);
                    BinaryReader br = new BinaryReader(fs);
                    try
                    {
                        int rowNum = 0;
                        while (true)
                        {
                            PatientInfo TempInfo = new PatientInfo();
                            TempInfo.id = br.ReadString();
                            TempInfo.Name = br.ReadString();
                            TempInfo.HospitalId = br.ReadString();
                            TempInfo.BookInDate = br.ReadString();
                            TempInfo.ReturnAmount = br.ReadString();
                            TempInfo.ReturnDate = br.ReadString();
                            TempInfo.Other = br.ReadString();
                            bool IsIn = false;
                            if (!HospitalId.Equals(""))
                            {
                                if (TempInfo.HospitalId.Equals(HospitalId))
                                {
                                    IsIn = true;
                                }
                            }
                            if (!PatientName.Equals(""))
                            {
                                if(TempInfo.Name.Contains(PatientName))
                                {
                                    IsIn = true;
                                }
                            }
                            if (IsIn)
                            {
                                TempInfo.fileName = this.TablesName[i];
                                TempInfo.RowNum = rowNum;
                                this.SearchResult.Add(TempInfo);
                            }
                            rowNum++;
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("读取文件到达末尾");
                    }
                    br.Close();
                    fs.Close();
                }
                catch (FileNotFoundException)
                {
                    continue;
                }
            }
        }
            

        private class PatientInfo
        {
            public String id;
            public String Name;
            public String HospitalId;
            public String BookInDate;
            public String ReturnAmount;
            public String ReturnDate;
            public String Other;
            public String fileName;
            public int RowNum;
        }

        private void OutPutExcelButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Execl表格文件 (*.xls)|*.xls";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.Title = "导出Excel文件到";
            saveFileDialog.ShowDialog();
            Stream myStream;
            try
            {
                myStream = saveFileDialog.OpenFile();
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }
            //StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));
            string str = "";
            try
            {
                //写标题
                for (int i = 0; i < this.PatientDataView.ColumnCount; i++)
                {
                    if (i > 0)
                    {
                        str += "\t";
                    }
                    str += PatientDataView.Columns[i].HeaderText;
                }
                sw.WriteLine(str);
                //写内容
                for (int j = 0; j < PatientDataView.Rows.Count; j++)
                {
                    string tempStr = "";
                    for (int k = 0; k < PatientDataView.Columns.Count; k++)
                    {
                        if (k > 0)
                        {
                            tempStr += "\t";
                        }
                        tempStr += (PatientDataView.Rows[j].Cells[k].Value == System.DBNull.Value ? "" : PatientDataView.Rows[j].Cells[k].Value);
                    }
                    sw.WriteLine(tempStr);

                }
                sw.Close();
                myStream.Close();
                MessageBox.Show("导出成功");
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }
            finally
            {
                sw.Close();
                myStream.Close();
            }
        }

        private void NameListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.NameListBox.SelectedIndex == -1)
                return;
            PatientInfo TempInfo = this.SearchResult[this.NameListBox.SelectedIndex];
            this.labelBookInDate.Text = TempInfo.BookInDate;
            this.DataTablePatientDocument = new DataTable("病历档案");
            this.DataTablePatientDocument.Columns.Add("编号");
            this.DataTablePatientDocument.Columns.Add("姓名");
            this.DataTablePatientDocument.Columns.Add("住院号");
            this.DataTablePatientDocument.Columns.Add("返款日期");
            this.DataTablePatientDocument.Columns.Add("押金");
            DataRow TempRow = this.DataTablePatientDocument.NewRow();
            TempRow["编号"] = TempInfo.id;
            TempRow["姓名"] = TempInfo.Name;
            TempRow["住院号"] = TempInfo.HospitalId;
            TempRow["返款日期"] = TempInfo.ReturnDate;
            TempRow["押金"] = TempInfo.ReturnAmount;
            this.DataTablePatientDocument.Rows.Add(TempRow);
            this.dataGridViewDocument.DataSource = this.DataTablePatientDocument;
            //添加图像
            if (this.pictureBoxDocument.Image != null)
            {
                this.pictureBoxDocument.Image.Dispose();
            }
            String filename = this.pPath + TempInfo.Name + "-" + TempInfo.HospitalId + ".jpg";
            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(filename);
                System.Drawing.Image bmp = new System.Drawing.Bitmap(img);
                img.Dispose();
                this.pictureBoxDocument.Image = bmp;
            }
            catch (FileNotFoundException)
            {
                try
                {
                    String filename2 = this.pPath + TempInfo.HospitalId + ".jpg";
                    System.Drawing.Image img = System.Drawing.Image.FromFile(filename2);
                    System.Drawing.Image bmp = new System.Drawing.Bitmap(img);
                    img.Dispose();
                    this.pictureBoxDocument.Image = bmp;
                }
                catch (FileNotFoundException)
                {
                    System.Drawing.Image img = System.Drawing.Image.FromFile("./add.png");
                    System.Drawing.Image bmp = new System.Drawing.Bitmap(img);
                    img.Dispose();
                    this.pictureBoxDocument.Image = bmp;
                }
            }
        }

        private void pictureBoxDocument_Click(object sender, EventArgs e)
        {
            if (NameListBox.SelectedIndex != -1)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "JPEG文件 (*.jpg)|*.jpg";
                openFileDialog.FilterIndex = 0;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "打开图片文件";
                openFileDialog.ShowDialog();
                String FileName;
                FileName = openFileDialog.FileName;
                if (FileName.Equals(""))
                {
                    return;
                }
                else
                {
                    //MessageBox.Show(FileName);
                    PatientInfo TempInfo = this.SearchResult[this.NameListBox.SelectedIndex];
                    String desfilename = this.pPath + TempInfo.Name + "-" + TempInfo.HospitalId + ".jpg";
                    if (this.pictureBoxDocument.Image != null)
                        this.pictureBoxDocument.Image.Dispose();
                    Image DocumentImage = Image.FromFile(FileName);
                    DocumentImage.Save(desfilename, System.Drawing.Imaging.ImageFormat.Jpeg);
                    DocumentImage.Dispose();
                    System.Drawing.Image img = System.Drawing.Image.FromFile(desfilename);
                    System.Drawing.Image bmp = new System.Drawing.Bitmap(img);
                    img.Dispose();
                    this.pictureBoxDocument.Image = bmp;
                }
            }
        }

        private void buttonSaveDocument_Click(object sender, EventArgs e)
        {
            if (this.NameListBox.SelectedIndex == -1)
            {
                MessageBox.Show("选择一个病人");
                return;
            }
            PatientInfo TempInfo = this.SearchResult[this.NameListBox.SelectedIndex];
            TempInfo.id = this.dataGridViewDocument.Rows[0].Cells["编号"].Value == System.DBNull.Value ? "" : (String)this.dataGridViewDocument.Rows[0].Cells["编号"].Value;
            TempInfo.Name = this.dataGridViewDocument.Rows[0].Cells["姓名"].Value == System.DBNull.Value ? "" : (String)this.dataGridViewDocument.Rows[0].Cells["姓名"].Value;
            TempInfo.HospitalId = this.dataGridViewDocument.Rows[0].Cells["住院号"].Value == System.DBNull.Value ? "" : (String)this.dataGridViewDocument.Rows[0].Cells["住院号"].Value;
            TempInfo.ReturnDate = this.dataGridViewDocument.Rows[0].Cells["返款日期"].Value == System.DBNull.Value ? "" : (String)this.dataGridViewDocument.Rows[0].Cells["返款日期"].Value;
            TempInfo.ReturnAmount = this.dataGridViewDocument.Rows[0].Cells["押金"].Value == System.DBNull.Value ? "" : (String)this.dataGridViewDocument.Rows[0].Cells["押金"].Value;
            DataTable TempTable = new DataTable();
            this.InitTable(TempTable);
            this.ReadTable(this.sPath + TempInfo.fileName + ".dat", TempTable);
            TempTable.Rows[TempInfo.RowNum]["编号"] = TempInfo.id;
            TempTable.Rows[TempInfo.RowNum]["姓名"] = TempInfo.Name;
            TempTable.Rows[TempInfo.RowNum]["病案号"] = TempInfo.HospitalId;
            TempTable.Rows[TempInfo.RowNum]["登记日期"] = TempInfo.BookInDate;
            TempTable.Rows[TempInfo.RowNum]["押金"] = TempInfo.ReturnAmount;
            TempTable.Rows[TempInfo.RowNum]["返款时间"] = TempInfo.ReturnDate;
            TempTable.Rows[TempInfo.RowNum]["备注"] = TempInfo.Other;
            this.SaveTable(this.sPath + TempInfo.fileName + ".dat", TempTable);
            //刷新list
            this.NameListBox.Items.Clear();
            for (int i = 0; i < this.SearchResult.Count; i++)
            {
                this.NameListBox.Items.Add("姓名：" + this.SearchResult[i].Name + "  病案号：" + this.SearchResult[i].HospitalId);
            }
            MessageBox.Show("已保存");
        }

        private void SearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                SearchButton_Click(sender,e);
            }
        }

        private void buttonAddToday_Click(object sender, EventArgs e)
        {
            if (NameListBox.SelectedIndex == -1)
            {
                return;
            }
            String DateString = DateTime.Now.ToString("yyyy年M月d日");
            this.dataGridViewDocument.Rows[0].Cells["返款日期"].Value = DateString;
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            //MessageBox.Show(Convert.ToString(e.MarginBounds.Width));
            Font HeadFont = new Font("宋体",24);
            Font TextFont = new Font("宋体", 10);
            SolidBrush myBrush = new SolidBrush(Color.Black);
            Pen myPen = new Pen(Color.Black);
            //标题
            //g.DrawString("病历档案", HeadFont, myBrush, e.MarginBounds.Left + 246, e.MarginBounds.Top + 21);
            //时间
            PatientInfo TempInfo;
            Image DesImage;
            if (NameListBox.SelectedIndex == -1)
            {
                TempInfo = new PatientInfo();
                TempInfo.BookInDate = "    年  月  日";
                TempInfo.HospitalId = "";
                TempInfo.id = "";
                TempInfo.Name = "";
                TempInfo.ReturnAmount = "";
                TempInfo.ReturnDate = "";
                DesImage = null;
            }
            else
            {
                TempInfo = this.SearchResult[NameListBox.SelectedIndex];
                DesImage = pictureBoxDocument.Image;
            }
            /*
            g.DrawString(TempInfo.BookInDate, TextFont, myBrush, e.MarginBounds.Left + 478, e.MarginBounds.Top + 47);
            //表格
            g.DrawLine(myPen, e.MarginBounds.Left + 23, e.MarginBounds.Top + 76, e.MarginBounds.Left + 604, e.MarginBounds.Top + 76);
            g.DrawLine(myPen, e.MarginBounds.Left + 23, e.MarginBounds.Top + 93, e.MarginBounds.Left + 604, e.MarginBounds.Top + 93);
            g.DrawLine(myPen, e.MarginBounds.Left + 23, e.MarginBounds.Top + 116, e.MarginBounds.Left + 604, e.MarginBounds.Top + 116);
            g.DrawLine(myPen, e.MarginBounds.Left + 23, e.MarginBounds.Top + 76, e.MarginBounds.Left + 23, e.MarginBounds.Top + 116);
            g.DrawLine(myPen, e.MarginBounds.Left + 60, e.MarginBounds.Top + 76, e.MarginBounds.Left + 60, e.MarginBounds.Top + 116);
            g.DrawLine(myPen, e.MarginBounds.Left + 168, e.MarginBounds.Top + 76, e.MarginBounds.Left + 168, e.MarginBounds.Top + 116);
            g.DrawLine(myPen, e.MarginBounds.Left + 275, e.MarginBounds.Top + 76, e.MarginBounds.Left + 275, e.MarginBounds.Top + 116);
            g.DrawLine(myPen, e.MarginBounds.Left + 385, e.MarginBounds.Top + 76, e.MarginBounds.Left + 385, e.MarginBounds.Top + 116);
            g.DrawLine(myPen, e.MarginBounds.Left + 497, e.MarginBounds.Top + 76, e.MarginBounds.Left + 497, e.MarginBounds.Top + 116);
            g.DrawLine(myPen, e.MarginBounds.Left + 604, e.MarginBounds.Top + 76, e.MarginBounds.Left + 604, e.MarginBounds.Top + 116);
            //绘制文字
            g.DrawString("编号",TextFont,myBrush, e.MarginBounds.Left + 64, e.MarginBounds.Top + 78);
            g.DrawString("姓名", TextFont, myBrush, e.MarginBounds.Left + 172, e.MarginBounds.Top + 78);
            g.DrawString("住院号", TextFont, myBrush, e.MarginBounds.Left + 279, e.MarginBounds.Top + 78);
            g.DrawString("返款日期", TextFont, myBrush, e.MarginBounds.Left + 389, e.MarginBounds.Top + 78);
            g.DrawString("押金", TextFont, myBrush, e.MarginBounds.Left + 501, e.MarginBounds.Top + 78);
            //绘制内容
            g.DrawString(TempInfo.id, TextFont, myBrush, e.MarginBounds.Left + 64, e.MarginBounds.Top + 95);
            g.DrawString(TempInfo.Name, TextFont, myBrush, e.MarginBounds.Left + 172, e.MarginBounds.Top + 95);
            g.DrawString(TempInfo.HospitalId, TextFont, myBrush, e.MarginBounds.Left + 279, e.MarginBounds.Top + 95);
            g.DrawString(TempInfo.ReturnDate, TextFont, myBrush, e.MarginBounds.Left + 389, e.MarginBounds.Top + 95);
            g.DrawString(TempInfo.ReturnAmount, TextFont, myBrush, e.MarginBounds.Left + 501, e.MarginBounds.Top + 95);
            */
            //绘制图片
            if (DesImage != null)
            {
                //g.DrawImage(pictureBoxDocument.Image, e.MarginBounds.Left + 23, e.MarginBounds.Top + 142, 581, 820);
                g.DrawImage(pictureBoxDocument.Image, e.PageBounds.Left, e.PageBounds.Top, e.PageBounds.Width, e.PageBounds.Height);
            }
            g.DrawRectangle(myPen, e.PageBounds.Left, e.PageBounds.Top, e.PageBounds.Width, e.PageBounds.Height);
            //g.DrawRectangle(myPen, e.MarginBounds.Left + 23, e.MarginBounds.Top + 142, 581, 820);
            e.HasMorePages = false;
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            if (NameListBox.SelectedIndex == -1)
            {
                MessageBox.Show("没有选择病人将打印空病历");
            }
            try {
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDocument1;
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
                printPreviewDialog.Document = printDocument1;
                printPreviewDialog.ShowDialog();

            }
            catch (Exception)
            {
                MessageBox.Show("打印已经取消");
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.SaveTable();
            MessageBox.Show("已经保存");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (NameListBox.SelectedIndex == -1)
            {
                MessageBox.Show("请选择一位病人");
                return;
            }
            String PathAddition;
            if (!this.ScanDateFolderNameFormat.Equals(""))
            {
                PathAddition = DateTime.Now.ToString(this.ScanDateFolderNameFormat) + "/";
            }
            else
            {
                PathAddition = "";
            }
            ScanWait ScanWaitWindow = new ScanWait(this.scanPath + PathAddition);
            if (ScanWaitWindow.ShowDialog() == DialogResult.OK)
            {
                PatientInfo TempInfo = this.SearchResult[this.NameListBox.SelectedIndex];
                String desfilename = this.pPath + TempInfo.Name + "-" + TempInfo.HospitalId + ".jpg";
                MessageBox.Show("搜索到的路径：" + ScanWaitWindow.ImageFilePath);
                Thread.Sleep(this.ScanDelayTime);
                if (this.pictureBoxDocument.Image != null)
                    this.pictureBoxDocument.Image.Dispose();
                Image DocumentImage = Image.FromFile(ScanWaitWindow.ImageFilePath);
                DocumentImage.Save(desfilename, System.Drawing.Imaging.ImageFormat.Jpeg);
                DocumentImage.Dispose();
                System.Drawing.Image img = System.Drawing.Image.FromFile(desfilename);
                System.Drawing.Image bmp = new System.Drawing.Bitmap(img);
                img.Dispose();
                this.pictureBoxDocument.Image = bmp;
            }
            else
            {
                MessageBox.Show("取消");
            }
        }

        private void PatientdataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            this.SaveTable();
        }

        private void PatientdataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            this.SaveTable();
        }

        private void PatientdataGridView_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            this.SaveTable();
        }

        private void AddTime_Click(object sender, EventArgs e)
        {
            for (int i = 0;i < this.PatientDataView.SelectedRows.Count;i++)
            {
                this.PatientDataView.SelectedRows[i].Cells["返款时间"].Value = this.dateTimePickerAddTime.Text;
            }
            //保存
            this.SaveTable(this.sPath + TableList.SelectedItem.ToString() + ".dat", this.PatientDataTableForView);
        }

        private void PatientDataImage_Click(object sender, EventArgs e)
        {
            /*
            if (this.PatientDataView.CurrentRow != null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "JPEG文件 (*.jpg)|*.jpg";
                openFileDialog.FilterIndex = 0;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "打开图片文件";
                openFileDialog.ShowDialog();
                String FileName;
                FileName = openFileDialog.FileName;
                if (FileName.Equals(""))
                {
                    return;
                }
                else
                {
                    //MessageBox.Show(FileName);
                    String desfilename = this.pPath + this.PatientDataView.Rows[this.PatientDataView.CurrentRow.Index].Cells["姓名"].Value + "-" + this.PatientDataView.Rows[this.PatientDataView.CurrentRow.Index].Cells["病案号"].Value +".jpg";
                    if (this.PatientDataImage.Image != null)
                        this.PatientDataImage.Image.Dispose();
                    Image DocumentImage = Image.FromFile(FileName);
                    DocumentImage.Save(desfilename, System.Drawing.Imaging.ImageFormat.Jpeg);
                    DocumentImage.Dispose();
                    System.Drawing.Image img = System.Drawing.Image.FromFile(desfilename);
                    System.Drawing.Image bmp = new System.Drawing.Bitmap(img);
                    img.Dispose();
                    this.PatientDataImage.Image = bmp;
                }
            }
            */
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0;i < this.PatientDataTable.Rows.Count;i++)
            {
                this.PatientDataTable.Rows[i]["编号"] = i + 1;
            }
            this.NumPatientId = this.PatientDataTable.Rows.Count + 1;
            this.PatientId.Text = Convert.ToString(this.NumPatientId);
            this.SaveTable();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("iexplore.exe", "http://www.helloclyde.com.cn");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                FileStream fs = new FileStream(this.SetFilePath, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                this.sPath = this.textBoxDataTableFolderPath.Text;
                this.pPath = this.textBoxImageSaveFolderPath.Text;
                this.scanPath = this.textBoxScanImageFolderPath.Text;
                this.ScanDelayTime = Convert.ToInt32(this.textBoxScanImageDelay.Text);
                this.ScanDateFolderNameFormat = this.textBoxScanImageDataFolderFormat.Text;

                bw.Write(this.sPath);
                bw.Write(this.pPath);
                bw.Write(this.scanPath);
                bw.Write(this.ScanDelayTime);
                bw.Write(this.ScanDateFolderNameFormat);
                bw.Close();
                fs.Close();
                MessageBox.Show("设置文件保存成功！");
            }
            catch (Exception e1)
            {
                MessageBox.Show("设置文件保存失败!\n" + e1.ToString());
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            this.SetDefaultSet();
            InitSetTab();
        }

        private void PatientDataView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (TableList.SelectedItem == null)
            {
                MessageBox.Show("请在左侧列表中选择一个表单，才能自动保存");
                return;
            }
            this.SaveTable(this.sPath + TableList.SelectedItem.ToString() + ".dat", this.PatientDataTableForView);
        }

        private void PatientBookInDate_ValueChanged(object sender, EventArgs e)
        {
            //this.SaveTable();
            this.NumPatientId = 1;
            String filename = this.sPath + this.PatientBookInDate.Value.ToString("yyyy-MM-dd") + ".dat";
            this.PatientDataTable.Clear();
            ReadTable(filename, this.PatientDataTable, 1);
            this.PatientdataGridView.DataSource = this.PatientDataTable;
            this.PatientId.Text = Convert.ToString(this.NumPatientId);
            this.PatientName.Focus();
            this.EditBoxAmount.Text = "100";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (this.PatientDataView.CurrentRow == null)
            {
                MessageBox.Show("请选择一位病人");
                return;
            }
            String PathAddition;
            if (!this.ScanDateFolderNameFormat.Equals(""))
            {
                PathAddition = DateTime.Now.ToString(this.ScanDateFolderNameFormat) + "/";
            }
            else
            {
                PathAddition = "";
            }
            ScanWait ScanWaitWindow = new ScanWait(this.scanPath + PathAddition);
            if (ScanWaitWindow.ShowDialog() == DialogResult.OK)
            {
                Thread.Sleep(this.ScanDelayTime);
                MessageBox.Show("搜索到的路径：" + ScanWaitWindow.ImageFilePath);
                String desfilename;
                if (this.tabControlImage.SelectedIndex == 0)
                {
                    desfilename = this.pPath + this.PatientDataView.CurrentRow.Cells["姓名"].Value + "-" + this.PatientDataView.CurrentRow.Cells["病案号"].Value + "-1.jpg";

                }
                else
                {
                    desfilename = this.pPath + this.PatientDataView.CurrentRow.Cells["姓名"].Value + "-" + this.PatientDataView.CurrentRow.Cells["病案号"].Value + ".jpg";
                }
                Image DocumentImage = Image.FromFile(ScanWaitWindow.ImageFilePath);
                DocumentImage.Save(desfilename, System.Drawing.Imaging.ImageFormat.Jpeg);
                DocumentImage.Dispose();
                System.Drawing.Image img = System.Drawing.Image.FromFile(desfilename);
                System.Drawing.Image bmp = new System.Drawing.Bitmap(img);
                img.Dispose();
                if (this.tabControlImage.SelectedIndex == 0)
                {
                    if (pictureBox2.Image != null)
                        pictureBox2.Image.Dispose();
                    pictureBox2.Image = bmp;
                    MessageBox.Show("pictureBox2");
                }
                else
                {
                    if (pictureBoxDocument.Image != null)
                        pictureBoxDocument.Image.Dispose();
                    pictureBoxDocument.Image = bmp;
                    MessageBox.Show("pictureBoxDocument");
                }
            }
            else
            {
                MessageBox.Show("取消");
            }
            //刷新
            PatientDataView_SelectionChanged(null,null);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            String PathAddition;
            if (!this.ScanDateFolderNameFormat.Equals(""))
            {
                PathAddition = DateTime.Now.ToString(this.ScanDateFolderNameFormat) + "/";
            }
            else
            {
                PathAddition = "";
            }
            ScanWait ScanWaitWindow = new ScanWait(this.scanPath + PathAddition);
            if (ScanWaitWindow.ShowDialog() == DialogResult.OK)
            {
                Thread.Sleep(this.ScanDelayTime);
                MessageBox.Show("搜索到的路径：" + ScanWaitWindow.ImageFilePath);
                Image DocumentImage = Image.FromFile(ScanWaitWindow.ImageFilePath);
                Image MemImage = new Bitmap(DocumentImage);
                DocumentImage.Dispose();
                if (this.pictureBox1.Image != null)
                    this.pictureBox1.Image.Dispose();
                this.pictureBox1.Image = MemImage;
            }
            else
            {
                MessageBox.Show("取消");
            }
        }

        private void PatientdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= this.PatientdataGridView.RowCount)
            {
                return;
            }
            String desfilename = this.pPath + this.PatientdataGridView.Rows[e.RowIndex].Cells["姓名"].Value + "-" + this.PatientdataGridView.Rows[e.RowIndex].Cells["病案号"].Value + "-1.jpg";
            if (this.pictureBox1.Image != null)
                this.pictureBox1.Image.Dispose();
            try {
                System.Drawing.Image img = System.Drawing.Image.FromFile(desfilename);
                System.Drawing.Image bmp = new System.Drawing.Bitmap(img);
                img.Dispose();
                this.pictureBox1.Image = bmp;
            }
            catch (FileNotFoundException)
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile("./NoImage.png");
                System.Drawing.Image bmp = new System.Drawing.Bitmap(img);
                img.Dispose();
                this.pictureBox1.Image = bmp;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image != null)
            {
                this.pictureBox1.Image.Dispose();
                this.pictureBox1.Image = null;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            PictureBox TempPictureBox = (PictureBox)sender;
            if (TempPictureBox.Image == null)
            {
                MessageBox.Show("没有图片能够显示");
                return;
            }
            ImageWindow imageWindow = new ImageWindow(TempPictureBox.Image);
            imageWindow.Show();
        }
        
    }
}
