using System;
using System.Collections.Generic;
using System.Text;
//using Excel = Microsoft.Office.Interop.Excel;

namespace UnvaryingSagacity.Core
{
    public class ExcelExplorerInterop
    {
        private UnvaryingSagacity.ExcelExplorer.Interop.ExcelCellTextReaderClass _excel; 

        public ExcelExplorerInterop() 
        {
        }

        public bool OpenExcelFile(string fileName, string password, bool visible)
        {
            try
            {
                if (!Core.General.Check())
                    return false;
                _excel = new UnvaryingSagacity.ExcelExplorer.Interop.ExcelCellTextReaderClass();
                return _excel.OpenExcelFile(fileName, password);
            }
            catch { return false; }
        }

        public bool OpenExcelFile(string fileName, string password)
        {
            return OpenExcelFile(fileName, password, false);
        }

        public bool OpenExcelFile(string fileName)
        {
            return OpenExcelFile(fileName, "", false);
        }

        public int WorksheetCount
        {
            get
            {
                if (_excel != null)
                    return _excel.WorksheetCount ;
                else
                    return 0;
            }
        }

        /// <summary>
        /// 获得指定工作表的名称
        /// </summary>
        /// <param name="index">从1开始的工作表索引</param>
        /// <returns></returns>
        public string WorksheetName(int index)
        {
            if (_excel != null)
            {
                return _excel.WorksheetName(index);

            }
            return "";
        }

        public System.Data.DataRowCollection CellsToDataRow(int index)
        {
            System.Data.DataTable dataTable = new System.Data.DataTable();
            string[] rows = CellsToStringArray(index);
            if (rows.Length <= 0)
                return default(System.Data.DataRowCollection);
            string[] cs = rows[rows.Length-1].Split(new char[1] { (char)9 }, StringSplitOptions.None);  
            for (int i = 0; i < cs.Length ; i++)
            {
                dataTable.Columns.Add("Col" + i, typeof(object));
            }
            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i] != null)
                {
                    System.Data.DataRow dr = dataTable.NewRow();
                    cs = rows[i].Split(new char[1] { (char)9 }, StringSplitOptions.None);
                    object[] values = new object[cs.Length];
                    for (int j = 0; j < cs.Length; j++)
                    {
                        values[j] = cs[j];
                    }
                    dataTable.Rows.Add(values);
                }
            }
            //RaiseEvent Complete(exSheet)
            return dataTable.Rows;
        }

        /// <summary>
        /// 返回用制表符分割的字符串数组
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string [] CellsToStringArray(int index)
        {
            string[] rows = new string[0];
            try
            {
                _excel.CellsText(index, ref rows);
                return rows;

            }
            catch
            {
                return rows;
            }
        }
        /// <summary>
        /// 显示选择工作表的窗口
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="multiSelect">是否允许多选</param>
        /// <param name="selects">被选中的工作表索引</param>
        /// <returns></returns>
        public System.Windows.Forms.DialogResult ShowSelectWorksheet(System.Windows.Forms.IWin32Window owner, bool multiSelect, Core.ListDictionary<int> selects)
        {
            if (_excel  == null)
            {
                return System.Windows.Forms.DialogResult.Cancel;
            }
            #region wiondow code
            Core.GeneralForm gf = new GeneralForm();
            System.Windows.Forms.ListView listView1;
            System.Windows.Forms.ColumnHeader columnHeader1;
            System.Windows.Forms.Button button1;
            System.Windows.Forms.Button button2;

            listView1 = new System.Windows.Forms.ListView();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            button1 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            gf.SuspendLayout();
            // 
            // listView1
            // 
            listView1.CheckBoxes = multiSelect;
            listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader1});
            listView1.Location = new System.Drawing.Point(12, 12);
            listView1.Name = "listView1";
            listView1.Size = new System.Drawing.Size(373, 253);
            listView1.TabIndex = 3;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "现存的工作表";
            columnHeader1.Width = 260;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(218, 278);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(75, 23);
            button1.TabIndex = 1;
            button1.Text = "确定";
            button1.UseVisualStyleBackColor = true;
            button1.Click += delegate
            {
                if (multiSelect)
                {
                    foreach (System.Windows.Forms.ListViewItem item in listView1.Items)
                    {
                        if (item.Checked)
                        {
                            selects.Add(item.Index + 1);
                        }
                    }
                }
                else
                {
                    selects.Add(listView1.SelectedItems[0].Index + 1);
                }
                if (selects.Count <= 0)
                {
                    System.Windows.Forms.MessageBox.Show(owner, "至少要选择一张工作表", "选择工作表", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    return;
                }
                gf.DialogResult = System.Windows.Forms.DialogResult.OK;
            };
            // 
            // button2
            // 
            button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            button2.Location = new System.Drawing.Point(310, 278);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(75, 23);
            button2.TabIndex = 2;
            button2.Text = "取消";
            button2.UseVisualStyleBackColor = true;
            button2.Click += delegate
            {
                gf.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                gf.Close();
            };
            // 
            // Form1
            // 
            gf.AcceptButton = button1;
            gf.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            gf.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            gf.CancelButton = button2;
            gf.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog; 
            gf.ClientSize = new System.Drawing.Size(400, 309);
            gf.Controls.Add(button2);
            gf.Controls.Add(button1);
            gf.Controls.Add(listView1);
            gf.MaximizeBox = false;
            gf.MinimizeBox = false;
            gf.Name = "Form1";
            gf.ShowInTaskbar = false;
            gf.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            gf.Text = "现存的工作表";
            gf.ResumeLayout(false);
            gf.Shown += delegate
            {
                for (int i = 1; i <= _excel.WorksheetCount ; i++)
                {
                    listView1.Items.Add(_excel.WorksheetName (i));
                }
            };
            #endregion
            return gf.ShowDialog(owner);

        }

        public void Dispose()
        {
            try
            {
                _excel = null;
            }
            catch { }
        }

    }

    /// <summary>
    /// 废弃类,因为客户端必须安装offices .net编程组件.
    /// </summary>
    //public class ExcelExplorer
    //{
    //    Excel.Application _exApp;
    //    Excel.Workbook _exBook;

    //    public ExcelExplorer() { }

    //    public bool OpenExcelFile(string fileName, string password, bool visible)
    //    {
    //        try
    //        {
    //            //Core.General.Check();
    //            if (!Core.General.Check())
    //                return false;
    //            _exApp = new Microsoft.Office.Interop.Excel.Application();
    //            _exApp.DisplayAlerts = true;
    //            _exApp.Visible = visible;
    //            _exBook = _exApp.Workbooks.Open(fileName, 0, false, 1, password, "", true, Excel.XlPlatform.xlWindows, ",", false, true, 0, false, true, Excel.XlCorruptLoad.xlNormalLoad);
    //            return true;
    //        }
    //        catch { return false; }
    //    }

    //    public bool OpenExcelFile(string fileName, string password)
    //    {
    //        return OpenExcelFile(fileName ,password ,false );
    //    }

    //    public bool OpenExcelFile(string fileName)
    //    {
    //        return OpenExcelFile(fileName, "", false);
    //    }

    //    /// <summary>
    //    /// 获得指定工作表的最后一个有效网格
    //    /// </summary>
    //    /// <param name="name"></param>
    //    /// <returns></returns>
    //    public Excel.Range LastCell(string name)
    //    {
    //        if (_exBook != null)
    //        {
    //            Excel.Worksheet exSheet = _exBook.Sheets[name] as Excel.Worksheet;
    //            return LastCell(exSheet);
    //        }
    //        return default(Excel.Range);
    //    }

    //    /// <summary>
    //    /// 获得指定工作表的最后一个有效网格
    //    /// <param name="index">从1开始的工作表索引</param>
    //    /// <returns></returns>
    //    public Excel.Range LastCell(int index)
    //    {

    //        if (_exBook != null)
    //        {
    //            Excel.Worksheet exSheet = _exBook.Sheets[index] as Excel.Worksheet;
    //            return LastCell(exSheet);
    //        }
    //        return default(Excel.Range);
    //    }

    //    public string[] WorksheetNames()
    //    {
    //        string[] names = new string[0];
    //        if (_exBook != null)
    //        {
    //            Array.Resize<string>(ref names, _exBook.Worksheets.Count);
    //            for (int index = 1; index <= names.Length; index++)
    //                names[index - 1] = (_exBook.Worksheets[index] as Excel.Worksheet).Name;

    //        }
    //        return names;
    //    }

    //    public int WorksheetCount
    //    {
    //        get
    //        {
    //            if (_exBook != null)
    //                return _exBook.Worksheets.Count;
    //            else
    //                return 0;
    //        }
    //    }

    //    /// <summary>
    //    /// 获得指定工作表的名称
    //    /// </summary>
    //    /// <param name="index">从1开始的工作表索引</param>
    //    /// <returns></returns>
    //    public string WorksheetName(int index)
    //    {
    //        if (_exBook != null)
    //        {
    //            if (index > 0 && index <= _exBook.Worksheets.Count)
    //                return (_exBook.Worksheets[index] as Excel.Worksheet).Name;

    //        }
    //        return "";
    //    }

    //    /// <summary>
    //    /// 获得指定工作表的网格内容.
    //    /// </summary>
    //    /// <param name="index">从1开始的工作表索引</param>
    //    /// <returns></returns>
    //    public System.Data.DataRowCollection CellsToDataRow(int index)
    //    {
    //        Excel.Worksheet exSheet;
    //        System.Data.DataTable dataTable = new System.Data.DataTable();
    //        try
    //        {
    //            exSheet = _exBook.Worksheets[index] as Excel.Worksheet;
    //        }
    //        catch { return default(System.Data.DataRowCollection); }

    //        Excel.Range lastCell = LastCell(exSheet);
    //        for (int i = 0; i < lastCell.Column; i++)
    //        {
    //            dataTable.Columns.Add("Col" + i, typeof(object));
    //        }
    //        for (int i = 1; i <= lastCell.Row; i++)
    //        {
    //            System.Data.DataRow dr = dataTable.NewRow();
    //            object[] values = new object[lastCell.Column];
    //            for (int j = 1; j <= lastCell.Column; j++)
    //            {
    //                //RaiseEvent ReadCells(i, j, SheetIndex, UserCanceled)

    //                Excel.Range exCell = exSheet.Cells.get_Item(i, j) as Excel.Range;
    //                if (exCell != null)
    //                {
    //                    values[j - 1] = exCell.Text;
    //                }
    //                else
    //                    values[j - 1] = default(object);
    //            }
    //            dataTable.Rows.Add(values);

    //        }
    //        //RaiseEvent Complete(exSheet)
    //        return dataTable.Rows;
    //    }

    //    public void Dispose()
    //    {
    //        try
    //        {
    //            _exBook = null;
    //            if (_exApp.Visible)
    //                _exApp.Quit();
    //            _exApp = null;
    //        }
    //        catch { }
    //    }

    //    /// <summary>
    //    /// 显示选择工作表的窗口
    //    /// </summary>
    //    /// <param name="owner"></param>
    //    /// <param name="multiSelect">是否允许多选</param>
    //    /// <param name="selects">被选中的工作表索引</param>
    //    /// <returns></returns>
    //    public System.Windows.Forms.DialogResult ShowSelectWorksheet(System.Windows.Forms.IWin32Window owner, bool multiSelect, Core.ListDictionary<int> selects)
    //    {
    //        if (_exBook == null)
    //        {
    //            return System.Windows.Forms.DialogResult.Cancel;
    //        }
    //        Core.GeneralForm gf = new GeneralForm();
    //        System.Windows.Forms.ListView listView1;
    //        System.Windows.Forms.ColumnHeader columnHeader1;
    //        System.Windows.Forms.Button button1;
    //        System.Windows.Forms.Button button2;

    //        listView1 = new System.Windows.Forms.ListView();
    //        columnHeader1 = new System.Windows.Forms.ColumnHeader();
    //        button1 = new System.Windows.Forms.Button();
    //        button2 = new System.Windows.Forms.Button();
    //        gf.SuspendLayout();
    //        // 
    //        // listView1
    //        // 
    //        listView1.CheckBoxes = multiSelect;
    //        listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
    //        columnHeader1});
    //        listView1.Location = new System.Drawing.Point(12, 12);
    //        listView1.Name = "listView1";
    //        listView1.Size = new System.Drawing.Size(373, 253);
    //        listView1.TabIndex = 3;
    //        listView1.UseCompatibleStateImageBehavior = false;
    //        listView1.View = System.Windows.Forms.View.Details;
    //        // 
    //        // columnHeader1
    //        // 
    //        columnHeader1.Text = "现存的工作表";
    //        columnHeader1.Width = 260;
    //        // 
    //        // button1
    //        // 
    //        button1.Location = new System.Drawing.Point(218, 278);
    //        button1.Name = "button1";
    //        button1.Size = new System.Drawing.Size(75, 23);
    //        button1.TabIndex = 1;
    //        button1.Text = "确定";
    //        button1.UseVisualStyleBackColor = true;
    //        button1.Click += delegate
    //            {
    //                if (multiSelect)
    //                {
    //                    foreach (System.Windows.Forms.ListViewItem item in listView1.Items  )
    //                    {
    //                        if (item.Checked)
    //                        {
    //                            selects.Add(item.Index + 1);
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    selects.Add(listView1.SelectedItems[0].Index + 1);
    //                }
    //                if (selects.Count <= 0)
    //                {
    //                    System.Windows.Forms.MessageBox.Show(owner, "至少要选择一张工作表", "选择工作表", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
    //                    return;
    //                }
    //                gf.DialogResult = System.Windows.Forms.DialogResult.OK;
    //            };
    //        // 
    //        // button2
    //        // 
    //        button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
    //        button2.Location = new System.Drawing.Point(310, 278);
    //        button2.Name = "button2";
    //        button2.Size = new System.Drawing.Size(75, 23);
    //        button2.TabIndex = 2;
    //        button2.Text = "取消";
    //        button2.UseVisualStyleBackColor = true;
    //        button2.Click += delegate
    //        {
    //            gf.DialogResult = System.Windows.Forms.DialogResult.Cancel;
    //            gf.Close();
    //        };
    //        // 
    //        // Form1
    //        // 
    //        gf.AcceptButton = button1;
    //        gf.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
    //        gf.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    //        gf.CancelButton = button2;
    //        gf.ClientSize = new System.Drawing.Size(400, 309);
    //        gf.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog; 
    //        gf.Controls.Add(button2);
    //        gf.Controls.Add(button1);
    //        gf.Controls.Add(listView1);
    //        gf.MaximizeBox = false;
    //        gf.MinimizeBox = false;
    //        gf.Name = "Form1";
    //        gf.ShowInTaskbar = false;
    //        gf.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
    //        gf.Text = "现存的工作表";
    //        gf.ResumeLayout(false);
    //        gf.Shown += delegate
    //        {
    //            for (int i = 1; i <= WorksheetCount; i++)
    //            {
    //                listView1.Items.Add(WorksheetName(i));
    //            }
    //        };

    //        return gf.ShowDialog(owner);

    //    }

    //    Excel.Range LastCell(Excel.Worksheet exSheet)
    //    {
    //        try
    //        {
    //            Excel.Range exCell = exSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Excel.XlSpecialCellsValue.xlTextValues);
    //            if ((bool)exCell.MergeCells)
    //            {
    //                string[] a = exCell.get_Address(exCell.Row, exCell.Column, Excel.XlReferenceStyle.xlR1C1, null, null).Split(":".ToCharArray());
    //                string[] rc = a[1].Split(new char[2] { 'R', 'C' }, StringSplitOptions.RemoveEmptyEntries);
    //                Excel.Range lastCell = exSheet.Cells.get_Item(int.Parse(rc[0]), int.Parse(rc[1])) as Excel.Range;
    //                return lastCell;
    //            }
    //            return exCell;
    //        }
    //        catch { return default(Excel.Range); }
    //    }

    //}
}

