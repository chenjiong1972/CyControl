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
    public partial class FrmVchView : Form
    {
        public FrmVchView()
        {
            InitializeComponent();
            //dataGridView1.Rows.Add(1);
            dataGridView2.Rows.Add(100);
            dataGridView2.ColumnWidthChanged += dataGridView2_ColumnWidthChanged;
        }

        void dataGridView2_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            //Console.WriteLine("dataGridView2_ColumnWidthChanged:{0}", e.Column);
            dataGridView1.Columns[1].Width = (dataGridView2.Columns[0].Width + dataGridView2.Columns[1].Width + dataGridView2.Columns[2].Width + dataGridView2.Columns[3].Width );
            
        }
    }
}
