using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnvaryingSagacity.AccountOfBank
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.cyEditor1.Text = "123456.78";
            
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Right)
            {
                return false;
            }
            else if( keyData == Keys.Left){
                return true;
            }
            return  base.ProcessDialogKey(keyData);
        }
    }
}
