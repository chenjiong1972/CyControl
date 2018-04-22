using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.Core
{
    partial class UIInfoBrower : Form
    {
        public UIInfoBrower()
        {
            InitializeComponent();
            this.button1.Click += new EventHandler(button1_Click);
        }

        void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
