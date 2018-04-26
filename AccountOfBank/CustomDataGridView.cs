using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnvaryingSagacity.AccountOfBank
{
    public class CustomDataGridView : DataGridView
    {
        protected override bool ProcessDialogKey(Keys keyData)
        {
            Keys key = (keyData & Keys.KeyCode);
            if (key == Keys.Enter)
            {
                return this.ProcessRightKey(keyData);
            }
            return base.ProcessDialogKey(keyData);
        }


        public new bool ProcessRightKey(Keys keyData)
        {
            Keys key = (keyData & Keys.KeyCode);
            if (key == Keys.Enter)
            {
                //第一种情况：只有一行,且当光标移到最后一列时
                if ((base.CurrentCell.ColumnIndex == (base.ColumnCount - 1)) && (base.RowCount == 1))
                {
                    base.CurrentCell = base.Rows[base.RowCount - 1].Cells[0];
                    return true;
                }
                //第二种情况：有多行，且当光标移到最后一列时,移到下一行第一个单元
                if ((base.CurrentCell.ColumnIndex == (base.ColumnCount - 1)) && (base.CurrentCell.RowIndex < (base.RowCount - 1)))
                {
                    base.CurrentCell = base.Rows[base.CurrentCell.RowIndex + 1].Cells[0];
                    return true;
                }

                return base.ProcessRightKey(keyData);
            }
            return base.ProcessRightKey(keyData);
        }
    }
}
