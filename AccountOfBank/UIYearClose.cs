using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.AccountOfBank
{
    public partial class UIYearClose : Form
    {

        private Environment _e;
        private DataProvider _dp;

        internal Environment CurrentEnvironment { set { _e = value; } }

        internal DataProvider CurrentDataProvider { set { _dp = value; } }

        public UIYearClose()
        {
            InitializeComponent();
            this.Shown += new EventHandler(UIYearClose_Shown);
            #region button Event
            foreach (object button in toolStrip1.Items)
            {
                if (button.GetType() == typeof(ToolStripButton))
                {
                    ((ToolStripButton)button).Click += new EventHandler(button_Click);
                }
                else if (button.GetType() == typeof(ToolStripMenuItem))
                {
                    ((ToolStripMenuItem)button).Click += new EventHandler(button_Click);
                }
                else if (button.GetType() == typeof(ToolStripSplitButton))
                {
                    ((ToolStripSplitButton)button).ButtonClick += new EventHandler(button_Click);
                    ToolStripSplitButton ddb = button as ToolStripSplitButton;
                    foreach (object o in ddb.DropDownItems)
                    {
                        if (o.GetType() == typeof(ToolStripButton))
                        {
                            ((ToolStripButton)o).Click += new EventHandler(button_Click);
                        }
                        else if (o.GetType() == typeof(ToolStripMenuItem))
                        {
                            ((ToolStripMenuItem)o).Click += new EventHandler(button_Click);
                        }
                        else if (o.GetType() == typeof(ToolStripSplitButton))
                        {
                            ((ToolStripSplitButton)o).ButtonClick += new EventHandler(button_Click);
                        }
                    }
                }
                else if (button.GetType() == typeof(ToolStripDropDownButton))
                {
                    ToolStripDropDownButton ddb = button as ToolStripDropDownButton;
                    foreach (object o in ddb.DropDownItems)
                    {
                        if (o.GetType() == typeof(ToolStripButton))
                        {
                            ((ToolStripButton)o).Click += new EventHandler(button_Click);
                        }
                        else if (o.GetType() == typeof(ToolStripMenuItem))
                        {
                            ((ToolStripMenuItem)o).Click += new EventHandler(button_Click);
                        }
                        else if (o.GetType() == typeof(ToolStripSplitButton))
                        {
                            ((ToolStripSplitButton)o).ButtonClick += new EventHandler(button_Click);
                        }
                    }
                }

            }
            #endregion

        }

        void UIYearClose_Shown(object sender, EventArgs e)
        {
            InitYearClosedList();
        }

        void button_Click(object sender, EventArgs e)
        {
            switch ((sender as ToolStripItem).Text)
            {
                case "结账":
                    CloseYear();
                    break;
                case "取消结账":
                    UnCloseYear();
                    break;
                default :
                    break;
            }
        }

        void InitYearClosedList()
        {
            for (int i = _e.CurrentAccount.StartYear; i <= DateTime.Today.Year; i++)
            {
                YearClosed yc = _dp.GetYearClosed (i );
                int r = dataGridView1.Rows.Add();
                dataGridView1[0, r].Value = yc.Closed  ? Core.Printer.PrintAssign.OK_FLAG : "";
                dataGridView1[1, r].Value = i.ToString();
                dataGridView1[2, r].Value = yc.Closed ? yc.CloseDate.ToLongDateString() : "";
                dataGridView1.Rows[r].Tag = yc;
            }
        }

        void CloseYear()
        {
            int i = dataGridView1.SelectedCells[0].RowIndex;
            int y=0;
            if (int.TryParse(dataGridView1[1, i].Value.ToString(),out y))
            {
                if (MessageBox.Show(this, "您确定要对[" + y.ToString() + "]年进行结账吗?", "结账", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.Cursor = Cursors.WaitCursor; 
                    int startRow=0;
                    for (int j = 0; j <= i; j++)
                    {
                        YearClosed yc=(dataGridView1.Rows[j].Tag  as YearClosed );
                        if (!yc.Closed)
                        {
                            startRow = j;
                            break;
                        }
                    }
                    for (int j = startRow; j <= i; j++)
                    {
                        YearClosed yc = (dataGridView1.Rows[j].Tag  as YearClosed);
                        dataGridView1[3, j].Value = "正在结账, 请稍等...";
                        bool b = _dp.SetYearClosed(yc.Year );
                        if (b)
                        {
                            yc.Closed = true;
                            dataGridView1[2, j].Value = DateTime.Today.ToLongDateString();
                            dataGridView1[3, j].Value = "";
                            dataGridView1[0, j].Value = Core.Printer.PrintAssign.OK_FLAG;
                        }
                        else
                        {
                            dataGridView1[3, j].Value = "结账失败!";
                            break;
                        }
                    }
                    this.Cursor = Cursors.Default;
                }
            }
        }

        void UnCloseYear()
        {
            int i = dataGridView1.SelectedCells[0].RowIndex;
            int y=0;
            if (int.TryParse(dataGridView1[1, i].Value.ToString(), out y))
            {
                if (MessageBox.Show(this, "您确定要反结账到[" + y.ToString() + "]年吗?", "取消结账", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    for (int j = dataGridView1.Rows.Count - 1; j >= i; j--)
                    {
                        YearClosed yc = (dataGridView1.Rows[j].Tag as YearClosed);
                        if (yc.Closed)
                        {
                            if (_dp.DeleteRecord(DataProvider.TABLE_ACCOUNTYEARCLOSED, "FYear=" + yc.Year))
                            {
                                dataGridView1[2, j].Value = "";
                                dataGridView1[3, j].Value = "";
                                dataGridView1[0, j].Value = "";
                            }
                        }
                    }

                }
            }
        }
    }
}
