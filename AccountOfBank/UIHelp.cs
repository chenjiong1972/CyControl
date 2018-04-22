using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.AccountOfBank
{
    public partial class UIHelp : Form
    {
        public UIHelp()
        {
            InitializeComponent();
            this.Shown += new EventHandler(UIHelp_Shown);
        }

        void UIHelp_Shown(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(Application.StartupPath + "\\银行日记账软件.help"))
            {
                this.richTextBox1.LoadFile(Application.StartupPath + "\\银行日记账软件.help");
            }
            else
            {
                this.richTextBox1.Text = "缺少帮助文件<<银行日记账软件.help>>, 在文件夹["+Application.StartupPath+"]中"; 
            }
        }
    }
}
