using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.Core.Printer
{
    public partial class DlgPageRangeSetting : Form
    {
        internal string PageRanges { get; set; }
        internal int BeginPageNumber { get; set; }
        internal bool AllowBeginPageNumber { get; set; }

        public DlgPageRangeSetting()
        {
            InitializeComponent();
            this.Shown += new EventHandler(DlgPageRangeSetting_Shown);
            this.textBox1.KeyPress += new KeyPressEventHandler(textBox1_KeyPress);
            this.textBox2.KeyPress += new KeyPressEventHandler(textBox2_KeyPress);
            this.radioButton1.CheckedChanged += new EventHandler(radioButton_CheckedChanged);
            this.radioButton2.CheckedChanged += new EventHandler(radioButton_CheckedChanged); 
            this.button1.Click += new EventHandler(button1_Click);
            this.button2.Click += new EventHandler(button2_Click);
        }

        void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = radioButton2.Checked;  
        }

        void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsControl(e.KeyChar))
            {
                if (!Char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }
        void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsControl(e.KeyChar))
            {
                if (!Char.IsDigit(e.KeyChar) && e.KeyChar !=(char )'-' && e.KeyChar !=(char )',')
                {
                    e.Handled = true;
                }
            }
        }

        void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void button1_Click(object sender, EventArgs e)
        {
            int result = 1;
            if (textBox1.Enabled)
            {
                if (!int.TryParse(textBox1.Text, out result))
                {
                    MessageBox.Show(this, "错误的起始页号, 请输入正整数.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            if (radioButton1.Checked)
            {
                PageRanges = "";
            }
            else
            {
                PageRanges = textBox2.Text; 
            }
            BeginPageNumber = result;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        void DlgPageRangeSetting_Shown(object sender, EventArgs e)
        {
            textBox1.Enabled = AllowBeginPageNumber;
        }
    }
}
