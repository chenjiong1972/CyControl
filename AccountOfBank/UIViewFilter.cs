using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.AccountOfBank
{
    public partial class UIViewFilter : Form
    {
        public DateTime d1;
        public DateTime d2;

        public UIViewFilter()
        {
            InitializeComponent();
            d1 = DateTime.Today.AddDays(-1.0);
            d2=DateTime.Today;
            this.Shown += new EventHandler(UIViewFilter_Shown);
        }

        void UIViewFilter_Shown(object sender, EventArgs e)
        {
            dateTimePicker1.Value = d1;
            dateTimePicker2.Value = d2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            d1 = dateTimePicker1.Value;
            d2 = dateTimePicker2.Value;
            this.DialogResult = DialogResult.OK; 
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
