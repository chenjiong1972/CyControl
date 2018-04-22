using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.AccountOfBank
{
    public partial class UIAccountEditor : Form
    {
        internal Account CurrentAccount { get; set; }

        public UIAccountEditor()
        {
            InitializeComponent();
            this.Text = InternalBaseObject.账套.ToString();
            this.textBox4.TextChanged += new EventHandler(RefreshPath);
            this.textBox2.TextChanged += new EventHandler(RefreshPath); 
        }

        private void RefreshPath(object sender, EventArgs e)
        {
            string p = Application.CommonAppDataPath + @"\"; 
            if (textBox4.Text.Length > 0)
            {
                p = textBox4.Text + (textBox4.Text.EndsWith("\\") ? "" : "\\");
            }
            textBox5.Text = p + "ACT_" + textBox2.Text + Environment.SCHEME_DATA_FILEEXT;
        }
         
        private void button2_Click(object sender, EventArgs e)
        {
            CurrentAccount = default(Account );
            this.DialogResult = DialogResult.Cancel; 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!System.IO.File.Exists(textBox5.Text))
            {
                try
                {
                    DataProvider dp = new DataProvider(textBox5.Text);
                }
                catch(Exception ex) {
                    MessageBox.Show(this, ex.Message + "\n\n建立数据库失败.", "建立数据库失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            if (CurrentAccount == null)
            {
                CurrentAccount = new Account();
            }
            CurrentAccount.Name = textBox1.Text;
            CurrentAccount.ID = textBox2.Text;
            CurrentAccount.Description = textBox3.Text;
            CurrentAccount.FullPath = textBox5.Text;  

            this.DialogResult = DialogResult.OK; 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog f = new FolderBrowserDialog();
            f.ShowNewFolderButton = true;
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                textBox4.Text = f.SelectedPath; 
            }
        }
    }
}
