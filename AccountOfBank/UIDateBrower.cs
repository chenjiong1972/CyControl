using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.AccountOfBank
{
    public partial class UIDateBrower : Form
    {
        public DateTime d { get; set; }

        public UIDateBrower()
        {
            InitializeComponent();
            this.monthCalendar1.TodayDate = DateTime.Today;
            this.Shown += new EventHandler(UIDateBrower_Shown);
            d = DateTime.Today;
        }

        void UIDateBrower_Shown(object sender, EventArgs e)
        {
            this.monthCalendar1.SelectionStart = d;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            d = this.monthCalendar1.SelectionStart;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
