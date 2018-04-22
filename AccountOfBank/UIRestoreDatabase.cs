using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.AccountOfBank
{
    public partial class UIRestoreDatabase : Form
    {

        private Environment _e;
        private string _fileName;
        private bool _validateFile = false;
        internal Environment CurrentEnvironment { set { _e = value; } }
        internal Account RestoreAccount { get; set; }

        public UIRestoreDatabase()
        {
            InitializeComponent();
            this.Shown += new EventHandler(UIRestoreDatabase_Shown);
            this.button1.Click += new EventHandler(button_Click);
            this.button2.Click += new EventHandler(button_Click);
            this.button3.Click += new EventHandler(button_Click);
            this.textBox1.KeyPress += new KeyPressEventHandler(textBox1_KeyPress);
            this.checkBox1.CheckedChanged += new EventHandler(checkBox1_CheckedChanged);
        }

        void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (_validateFile)
            {
                if (checkBox1.Checked)
                {
                    button1.Enabled = true; 
                }
            }
        }

        void UIRestoreDatabase_Shown(object sender, EventArgs e)
        {
            if (RestoreAccount != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("名称: ");
                sb.AppendLine(RestoreAccount.Name  );
                sb.Append("编号: ");
                sb.AppendLine(RestoreAccount.ID );
                sb.Append("备注: ");
                sb.AppendLine(RestoreAccount.Description );
                textBox2.Text = sb.ToString(); 
            }
        }

        void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                _fileName = textBox1.Text;
                GetRestoreFileDescription();
            }
        }

        void button_Click(object sender, EventArgs e)
        {
            switch ((sender as Control).Text)
            {
                case "还原":
                    DoRestoreAccount();
                    break;
                case "取消":
                    this.Close();
                    break;
                case "浏览...":
                    GetRestoreFile();
                    break;
                default:
                    break;
            }
        }

        void GetRestoreFileDescription()
        {
            this.richTextBox1.Text = "";
            this.button1.Enabled = false;
            if (!System.IO.File.Exists(_fileName))
            {
                this.richTextBox1.ForeColor = Color.Red;
                this.richTextBox1.Text = "备份文件不存在!";
                return;
            }
            else
            {
                try
                {
                    DataProvider dp = new DataProvider(_fileName);
                    StringBuilder sb = new StringBuilder();
                    sb.Append("备份日期:");
                    sb.AppendLine(dp.GetConfig(DataProvider.CONFIG_备份日期));
                    sb.Append("备份创建人:");
                    sb.AppendLine(dp.GetConfig(DataProvider.CONFIG_备份创建人));
                    sb.Append("备份账套名称:");
                    sb.AppendLine(dp.GetConfig(DataProvider.CONFIG_备份账套名称));
                    sb.Append("备份账套编号:");
                    sb.AppendLine(dp.GetConfig(DataProvider.CONFIG_备份账套编号));
                    sb.Append("数据结构版本:");
                    sb.AppendLine(dp.GetConfig(DataProvider.CONFIG_数据结构版本));
                    //richTextBox1.ForeColor = Color.Black ;
                    richTextBox1.Text = sb.ToString();
                    _validateFile = true;
                    if (!checkBox1.Checked)
                    {
                        //非强制还原时,名称必须相同
                        if (dp.GetConfig(DataProvider.CONFIG_备份账套名称) != RestoreAccount.Name)
                        {
                            string s = "注意: 备份文件的账套名称与要还原的账套名称不符, 如果需要还原请选中[强制还原到账套]";
                            //richTextBox1.AppendText(s);
                            richTextBox1.SelectionStart = richTextBox1.TextLength;
                            richTextBox1.SelectionColor = Color.Red;
                            richTextBox1.SelectedText = s;
                            return;
                        }
                    }
                    this.button1.Enabled = true;
                }
                catch (Exception ex)
                {
                    this.richTextBox1.ForeColor = Color.Red;
                    this.richTextBox1.Text = ex.Message;
                }
            }
        }

        void GetRestoreFile()
        {
            OpenFileDialog sf = new OpenFileDialog();
            sf.CheckFileExists = true;
            sf.SupportMultiDottedExtensions = true;
            
            if (sf.ShowDialog(this) == DialogResult.OK)
            {
                _fileName = sf.FileName;
                textBox1.Text = _fileName; 
                GetRestoreFileDescription();
            }
        }

        void DoRestoreAccount()
        {
            string name = RestoreAccount.Name ;
            Account act = RestoreAccount;
            if (act != null)
            {
                string fileName = _fileName;
                this.Cursor = Cursors.WaitCursor;
                Core.ToolTip tip = new UnvaryingSagacity.Core.ToolTip();
                tip.ShowPrompt(this, "正在还原文件:" + fileName + " 到 账套[" + act.Name + "]\n\n耗费时间由数据量决定, 请等待......");
                string fileBackup = System.Environment.GetEnvironmentVariable("TEMP") + @"\" + act.ID+DateTime.Now.ToString("yyyyMMddHHmmss");
                if (System.IO.File.Exists(fileBackup + ".tmp"))
                {
                    System.IO.File.Delete(fileBackup + ".tmp");
                }
                System.IO.File.Move(act.FullPath, fileBackup + ".tmp");
                try
                {
                    DataProvider dpDst = new DataProvider(fileBackup);
                    DataProvider dpSrc = new DataProvider(fileName);
                    ///清理数据库建好后的默认数据
                    foreach (DataTable dt in dpDst.dataSet.Tables)
                    {
                        DataTable dstDt = dpDst.dataSet.Tables[dt.TableName];
                        DataRow[] drs = dpDst.GetDataRows(dt.TableName, "", "");
                        dstDt.Rows.Clear();
                        dstDt.AcceptChanges();
                    }
                    foreach (DataTable dt in dpSrc.dataSet.Tables)
                    {
                        DataTable dstDt = dpDst.dataSet.Tables[dt.TableName];
                        DataRow[] drs = dpSrc.GetDataRows(dt.TableName, "", "");
                        bool b = false;
                        foreach (DataRow dr in drs)
                        {
                            DataRow dstDr = dstDt.NewRow();
                            object[] rItemArray = new object[dstDr.ItemArray.Length];

                            dr.ItemArray.CopyTo(rItemArray, 0);
                            dstDr.ItemArray = rItemArray;
                            dstDt.Rows.Add(dstDr);
                            if (!b)
                                b = true;
                        }
                        if (b)
                        {
                            dstDt.AcceptChanges();
                        }
                    }
                    DateTime last = DateTime.Today;
                    dpDst.LastBackupDateTime = last;
                    dpDst.SetConfig(DataProvider.CONFIG_备份账套名称, "");
                    dpDst.SetConfig(DataProvider.CONFIG_备份账套编号, "");
                    dpDst.SetConfig(DataProvider.CONFIG_备份创建人, "");
                    System.IO.File.Move(fileBackup, act.FullPath);
                    tip.Hide();
                    this.Cursor = Cursors.Default;
                    MessageBox.Show(this, "还原账套已经顺利完成.", "还原账套", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    tip.Hide();
                    this.Cursor = Cursors.Default;
                    MessageBox.Show(this, ex.Message + "\n\n还原账套失败.", "还原账套", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (!System.IO.File.Exists(act.FullPath))
                        System.IO.File.Move(fileBackup + ".tmp", act.FullPath);
                }
            }
        }
    }

}

