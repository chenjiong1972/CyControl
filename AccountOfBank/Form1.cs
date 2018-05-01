using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnvaryingSagacity.Core;

namespace UnvaryingSagacity.AccountOfBank
{
    public partial class Form1 : Form
    {
        private DateTime _defaultDate = DateTime.Today;
        private UIState _currentState = UIState.View;//当前状态;0=无;1=editor;2=view

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            dataGridView1.CellPainting += new DataGridViewCellPaintingEventHandler(dataGridView1_CellPainting);
            dataGridView1.DefaultValuesNeeded += new DataGridViewRowEventHandler(dataGridView1_DefaultValuesNeeded);
            dataGridView1.CellValidating += new DataGridViewCellValidatingEventHandler(dataGridView1_CellValidating);
            //dataGridView1.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dataGridView1_EditingControlShowing);
            dataGridView1.CellValueChanged += new DataGridViewCellEventHandler(dataGridView1_CellValueChanged);
            dataGridView1.KeyUp += new KeyEventHandler(dataGridView1_KeyUp);
            
            dataGridView1.ColumnWidthChanged += dataGridView1_ColumnWidthChanged;
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Right)
            {
                return false;
            }
            else if (keyData == Keys.Left)
            {
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.BeginEdit(false);
        }

        void dataGridView1_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            textBox1.Text = e.Column.Index.ToString();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            InitEditor();
        }

        #region 设计录入界面
        void InitEditor()
        {
            InitColumns();
            dataGridView1.Columns[0].HeaderCell.Value = (_defaultDate.Year + "年;月;日");
            SetDefaultDate(_defaultDate);
        }

        void SetDefaultDate(DateTime d)
        {

            dataGridView1.DefaultValuesNeeded -= dataGridView1_DefaultValuesNeeded;
            dataGridView1.CellValidating -= dataGridView1_CellValidating;
            dataGridView1.CellValueChanged -= dataGridView1_CellValueChanged;
            dataGridView1.Rows.Clear();
            if (_defaultDate.Year != d.Year)
            {
                dataGridView1.Columns[0].HeaderCell.Value = (d.Year + "年;月;日");
                dataGridView1.Refresh();
            }
            _defaultDate = d;

            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically; //EditOnEnter; //EditOnKeystroke;// .EditProgrammatically;


            CustomDataGridView grid = dataGridView1;

            DateTime dt = DateTime.Now;

            int i = grid.Rows.Add();
            grid[0, i].Value = dt.Month;
            grid[1, i].Value = dt.Day;
            grid[2, i].Value = "J";
            grid[3, i].Value = "100";
            grid[4, i].Value = "dg";
            grid[5, i].Value = "dfgdhfgh";
            grid[6, i].Value = "fhfgjghjghkjhgkgjkgh";
            
                grid[7, i].Value = 134523.45.ToString("0.00");
            
                grid[8, i].Value = 456456.ToString("0.00");
            
            //余额列最后一起算.grid[9, i].Value = d;

            grid[10, i].Value = UnvaryingSagacity.Core.Printer.PrintAssign.OK_FLAG;
            grid.Rows[i].Tag = "";


            // dataGridView1.RowValidated += new DataGridViewCellEventHandler(dataGridView1_RowValidated);
            dataGridView1.DefaultValuesNeeded += new DataGridViewRowEventHandler(dataGridView1_DefaultValuesNeeded);
            dataGridView1.CellValidating += new DataGridViewCellValidatingEventHandler(dataGridView1_CellValidating);
            dataGridView1.CellValueChanged += new DataGridViewCellEventHandler(dataGridView1_CellValueChanged);
        }

        void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            if (dataGridView1.Rows[e.RowIndex].Tag is Entry && e.ColumnIndex < 9)
            {
                (dataGridView1.Rows[e.RowIndex].Tag as Entry).Changed = true;
                dataGridView1.Rows[e.RowIndex].Cells[10].Value = "";
            }
        }


        void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            Console.WriteLine("dataGridView1_KeyUp：{0}",e.KeyCode); 
            if (e.KeyCode == Keys.Return && (sender as Control).Name == "dataGridView1")
            {
                //第一种情况：最末行
                if (dataGridView1.CurrentCell.RowIndex == (dataGridView1.RowCount - 1))
                {
                    //且当光标移到最后一列时,移到第一个单元
                    if ((dataGridView1.CurrentCell.ColumnIndex == (dataGridView1.ColumnCount - 1)))
                    {
                        dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0];
                        e.SuppressKeyPress = true;
                    }
                    else
                    {
                        dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[dataGridView1.CurrentCell.ColumnIndex + 1];
                        e.SuppressKeyPress = true;
                    }
                }
                //第二种情况：非最末行，
                else
                {
                    //且当光标移到最后一列时,移到下一行第一个单元
                    if ((dataGridView1.CurrentCell.ColumnIndex == (dataGridView1.ColumnCount - 1)))
                    {
                        dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex + 1].Cells[0];
                        e.SuppressKeyPress = true;
                    }
                    else
                    {
                        dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[dataGridView1.CurrentCell.ColumnIndex + 1];
                        e.SuppressKeyPress = true;
                    }
                }
            }
        }

        void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            
            if ((e.ColumnIndex >= 7 && e.ColumnIndex <= 8) || (e.ColumnIndex >= 0 && e.ColumnIndex <= 1) || (e.ColumnIndex == 3))
            {
                try
                {
                    string s = dataGridView1[e.ColumnIndex, e.RowIndex].EditedFormattedValue.ToString();
                    if (s.Length > 0)
                    {
                        if (!Core.General.IsNumberic(s, true))
                        {
                            e.Cancel = true;
                        }
                        else
                        {
                            if (double.Parse(s) != 0)
                            {
                                if (e.ColumnIndex == 7)
                                {
                                    dataGridView1[8, e.RowIndex].Value = "";
                                }
                                else if (e.ColumnIndex == 8)
                                {
                                    dataGridView1[7, e.RowIndex].Value = "";
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }

        void dataGridView1_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[0].Value = _defaultDate.Month;
            e.Row.Cells[1].Value = _defaultDate.Day;
        }

        void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            Console.WriteLine("dataGridView1_EditingControlShowing"); 
            e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress);
            //if (e.Control is TextBox && ((sender as DataGridView ).CurrentCell.ColumnIndex ==6 ))
            //{
            //    (e.Control as TextBox).AutoCompleteCustomSource.AddRange(_dataProvider.DistinctRecords("digest", ""));
            //    (e.Control as TextBox).AutoCompleteMode = AutoCompleteMode.Suggest;
            //}
        }

        void Control_KeyPress(object sender, KeyPressEventArgs e)
        {
            byte[] b = Encoding.Unicode.GetBytes(e.KeyChar.ToString());
            if (b.Length == 2)
            {
                if (b[1] == 255)
                {
                    b[0] = (byte)(b[0] + 32);
                    b[1] = 0;
                    char[] c = Encoding.Unicode.GetChars(b);
                    e.KeyChar = c[0];
                }
            }
        }

        private void dataGridView1_CellPainting(object sender, System.Windows.Forms.DataGridViewCellPaintingEventArgs e)
        {
            const int CYTITLEHEIGHT = 25;
            CustomDataGridView grid = (sender as CustomDataGridView);
            #region Header: year,month,day,VCH;CHEQUE
            if ((e.ColumnIndex >= 0 && e.ColumnIndex <= 5) && e.RowIndex == -1 && _currentState != UIState.ViewTotal)
            {
                if (e.ColumnIndex == 0 || e.ColumnIndex == 2 || e.ColumnIndex == 4)
                {
                    //e.Paint(e.ClipBounds, DataGridViewPaintParts.Background |DataGridViewPaintParts.Border  );
                    Rectangle rt = e.CellBounds;
                    Pen penGridLine = new Pen(grid.GridColor);
                    Pen penWhite = new Pen(Color.White);
                    rt.Width += grid.Columns[e.ColumnIndex + 1].Width;

                    e.Graphics.FillRectangle(new SolidBrush(grid.RowHeadersDefaultCellStyle.BackColor), rt);
                    e.Graphics.DrawLine(penGridLine, e.ClipBounds.X, e.ClipBounds.Y, e.ClipBounds.X, rt.Bottom);//Left
                    e.Graphics.DrawLine(penGridLine, rt.X, rt.Y, rt.Right, rt.Y);//Top
                    e.Graphics.DrawLine(penGridLine, rt.Right - 1, rt.Y, rt.Right - 1, rt.Bottom);//Right
                    e.Graphics.DrawLine(penGridLine, rt.X, rt.Bottom - 1, rt.Right, rt.Bottom - 1);//bottom

                    e.Graphics.DrawLine(penWhite, rt.Left + 1, rt.Top + 1, rt.Right, rt.Top + 1);//Top
                    if (e.ColumnIndex == 0)
                    {
                        e.Graphics.DrawLine(penWhite, rt.X + 1, rt.Top + 1, rt.X + 1, rt.Bottom);//Left
                    }
                    else
                    {
                        e.Graphics.DrawLine(penWhite, rt.X, rt.Top + 1, rt.X, rt.Bottom);//Left
                    }
                    Rectangle rtYear = rt;
                    Rectangle rtMonth = rt;
                    Rectangle rtDay = rt;
                    rtYear.Height = rt.Height / 2;
                    rtMonth.Height = rtYear.Height;
                    rtDay.Height = rtYear.Height;
                    rtMonth.Width = rtYear.Width / 2;
                    rtDay.Width = rtMonth.Width;
                    rtMonth.Offset(0, rtYear.Height);
                    rtDay.Offset(rtMonth.Width, rtYear.Height);
                    e.Graphics.DrawLine(penGridLine, rtYear.Left + 2, rtYear.Bottom, rtYear.Right - 1, rtYear.Bottom);//center_H
                    if (e.ColumnIndex == 0)
                    {
                        e.Graphics.DrawLine(penGridLine, rtDay.X, rtDay.Top, rtDay.X, rtDay.Bottom);//center_V
                    }
                    else
                    {
                        e.Graphics.DrawLine(penGridLine, rtDay.X - 1, rtDay.Top, rtDay.X - 1, rtDay.Bottom);//center_V
                    }
                    StringFormat _sf = new StringFormat();
                    _sf.Alignment = StringAlignment.Center;
                    _sf.LineAlignment = StringAlignment.Center;

                    string[] ss = e.Value.ToString().Split(";".ToCharArray());
                    e.Graphics.DrawString(ss[0], new Font(e.CellStyle.Font.Name, 9), new SolidBrush(e.CellStyle.ForeColor), rtYear, _sf);
                    e.Graphics.DrawString(ss[1], new Font(e.CellStyle.Font.Name, 9), new SolidBrush(e.CellStyle.ForeColor), rtMonth, _sf);
                    e.Graphics.DrawString(ss[2], new Font(e.CellStyle.Font.Name, 9), new SolidBrush(e.CellStyle.ForeColor), rtDay, _sf);
                }
                e.Handled = true;
            }
            #endregion

            #region 金额线
            if ((7 <= e.ColumnIndex && 9 >= e.ColumnIndex && _currentState != UIState.ViewTotal) || (2 <= e.ColumnIndex && 5 >= e.ColumnIndex && _currentState == UIState.ViewTotal))
            {
                int _charFullWidth = 0;
                int _charWidth = 0;
                RectangleF rtCyBound = new RectangleF();
                RectangleF[] rtfs = new RectangleF[0];
                Font font = grid.Columns[e.ColumnIndex].CellTemplate.Style.Font;
                e.Paint(e.ClipBounds, DataGridViewPaintParts.Border | DataGridViewPaintParts.Background | DataGridViewPaintParts.Focus);
                StringFormat _sf = new StringFormat();
                _sf.Alignment = StringAlignment.Center;
                _sf.LineAlignment = StringAlignment.Center;
                if (e.RowIndex >= 0)
                {
                    if (InitCyPaint(font, e.CellBounds, ref _charFullWidth, ref _charWidth, ref rtCyBound, ref rtfs))
                    {
                        string s = e.FormattedValue.ToString();
                        s = s.Replace(".", "");
                        DrawTextContent(e, s, rtfs, rtCyBound, _charFullWidth, _charWidth);
                        if (dataGridView1.SelectedColumns.Contains(dataGridView1.Columns[e.ColumnIndex]) && dataGridView1.SelectedRows.Contains(dataGridView1.Rows[e.RowIndex]))
                        {
                            Pen pen2 = new Pen(Color.Red, 2F);
                            e.Graphics.DrawRectangle(pen2, e.CellBounds);
                        }
                        e.Handled = true;
                    }
                }
                else
                {
                    Rectangle rtCyTitle = e.CellBounds;
                    Rectangle rtTitle = e.CellBounds;
                    rtCyTitle.Offset(0, e.CellBounds.Height - CYTITLEHEIGHT);
                    rtCyTitle.Height = CYTITLEHEIGHT;
                    rtTitle.Height = e.CellBounds.Height - CYTITLEHEIGHT;
                    if (InitCyPaint(font, rtCyTitle, ref _charFullWidth, ref _charWidth, ref rtCyBound, ref rtfs))
                    {
                        e.Graphics.DrawString(e.Value.ToString(), e.CellStyle.Font, new SolidBrush(e.CellStyle.ForeColor), rtTitle, _sf);
                        DrawCyTitle(e, rtfs, rtCyBound, _charFullWidth, _charWidth);
                        e.Graphics.DrawLine(Pens.Green, e.CellBounds.Left, rtCyTitle.Top, e.CellBounds.Right - 2, rtCyTitle.Top);
                        e.Handled = true;
                    }
                }
            }
            #endregion
        }

        void DrawCyTitle(System.Windows.Forms.DataGridViewCellPaintingEventArgs e, RectangleF[] rtfs, RectangleF rtCyTitle, int _charFullWidth, int _charWidth)
        {
            StringFormat _sf = new StringFormat();
            _sf.Alignment = StringAlignment.Center;
            _sf.LineAlignment = StringAlignment.Center;
            Font CyTitleFont = new Font("楷体_GB2312", 9F);
            Graphics g = e.Graphics;
            System.Drawing.Text.TextRenderingHint trh = g.TextRenderingHint;
            System.Drawing.Drawing2D.InterpolationMode im = g.InterpolationMode;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            RectangleF rtf = new RectangleF(rtCyTitle.Right - _charWidth, rtCyTitle.Y, _charWidth, rtCyTitle.Height);
            int maxLength = rtfs.Length > CyEditor.CYTITLE.Length ? CyEditor.CYTITLE.Length : rtfs.Length;//用小的
            int j = 0;
            for (int i = CyEditor.CYTITLE.Length - 1; i >= 0; i--)
            {
                if (j >= maxLength)
                    break;
                string s = CyEditor.CYTITLE.Substring(i, 1);
                g.DrawString(s, CyTitleFont, new SolidBrush(Color.Green), rtf, _sf);
                rtf.Offset(-_charFullWidth, 0);
                j++;
            }
            g.TextRenderingHint = trh;
            g.InterpolationMode = im;
            DrawCyLine(g, rtCyTitle.X, rtCyTitle.Y, rtCyTitle.Width, rtCyTitle.Height, _charFullWidth);
        }

        bool InitCyPaint(Font CyTextFont, Rectangle bound, ref int _charFullWidth, ref int _charWidth, ref RectangleF rtCyBound, ref RectangleF[] rtfs)
        {
            _charWidth = TextRenderer.MeasureText("0123456789", CyTextFont).Width / 10 + 2;
            _charFullWidth = _charWidth + CyEditor.WORD_SPACE / 2;

            float x;
            rtCyBound = new RectangleF(bound.X, bound.Top, bound.Width - CyEditor.DOUBLELINES_H, bound.Height - 1);
            x = rtCyBound.Right;
            rtfs = new RectangleF[0];
            while (x > (rtCyBound.Left + _charFullWidth))
            {
                Array.Resize<RectangleF>(ref rtfs, rtfs.Length + 1);
                rtfs[rtfs.Length - 1] = new RectangleF(x - _charFullWidth, bound.Top, _charFullWidth, bound.Height - 1);
                x -= _charFullWidth;
            }
            return true;
        }

        void DrawTextContent(System.Windows.Forms.DataGridViewCellPaintingEventArgs e, string ss, RectangleF[] rtfs, RectangleF rtCyText, int _charFullWidth, int _charWidth)
        {
            Graphics g = e.Graphics;
            bool _isSub = false;
            if (ss.Length > 0)
            {
                if (double.Parse(ss) != 0)
                {
                    StringFormat _sf = new StringFormat();
                    _sf.Alignment = StringAlignment.Center;
                    _sf.LineAlignment = StringAlignment.Center;

                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                    //这里和标题不一样
                    RectangleF rtf = new RectangleF(rtCyText.Right - _charFullWidth, rtCyText.Y, _charWidth, rtCyText.Height);
                    Brush b;
                    Brush _bRed = new SolidBrush(Color.Red);
                    Brush _bBlack = new SolidBrush(Color.Black);
                    _isSub = ss.Substring(0, 1) == "-" ? true : false;

                    if (_isSub)
                    {
                        ss = ss.Substring(1);
                        b = _bRed;
                    }
                    else
                    {
                        b = _bBlack;
                    }
                    if (ss.Length > rtfs.Length)
                        ss = new string('#', rtfs.Length);

                    for (int i = ss.Length - 1; i >= 0; i--)
                    {
                        string s = ss.Substring(i, 1);
                        g.DrawString(s, e.CellStyle.Font, b, rtf, _sf);
                        rtf.Offset(-_charFullWidth, 0);
                    }
                }
            }
            DrawCyLine(g, rtCyText.X, rtCyText.Y, rtCyText.Width, rtCyText.Height, _charFullWidth);
        }

        private void DrawCyLine(Graphics g, float X, float Y, float Width, float Height, float SpacingWidth)
        {
            Pen pen = new Pen(Color.Green);
            Pen pen2 = new Pen(Color.Green, 2F);
            Pen penRed = new Pen(Color.Red);
            float cX;
            int i, offset1 = 0, offset2 = 0;

            //两边画双线
            g.DrawLine(penRed, X + CyEditor.DOUBLELINES_H / 2, Y + offset1, X + CyEditor.DOUBLELINES_H / 2, Y + Height - offset2);
            g.DrawLine(penRed, X + Width - CyEditor.DOUBLELINES_H / 2, Y + offset1, X + Width - CyEditor.DOUBLELINES_H / 2, Y + Height - offset2);
            //最右边的列为余下的空
            cX = X + Width - SpacingWidth;
            i = 0;
            while (cX >= X + SpacingWidth)
            {
                i = i + 1;
                if (i == 2)
                {
                    g.DrawLine(penRed, cX, Y + offset1, cX, Y + Height - offset2);
                }
                else
                {
                    if (i > 3 && ((i - 2) % 3 == 0))
                    {
                        g.DrawLine(pen2, cX, Y + offset1, cX, Y + Height - offset2);
                    }
                    else
                        g.DrawLine(pen, cX, Y + offset1, cX, Y + Height - offset2);

                }
                cX = cX - SpacingWidth;
            }
        }

        void CalcBal(int startRow)
        {
            dataGridView1.CellValueChanged -= dataGridView1_CellValueChanged;
            
            dataGridView1.CellValueChanged += new DataGridViewCellEventHandler(dataGridView1_CellValueChanged);
        }

        void buttonEditor_Click(object sender, EventArgs e)
        {
            switch ((sender as Control).Name.ToLower())
            {
                case "button1":
                    SetDefaultDate(_defaultDate.AddDays(-1.0));
                    break;
                case "button2":
                    SetDefaultDate(_defaultDate.AddDays(1));
                    break;
               case "button6":
                    if (dataGridView1.SelectedCells[0].OwningRow.IsNewRow)
                    {
                        return;
                    }
                    else
                    {
                        DataGridViewRow r = dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex];
                        DataGridViewRow newRow = new DataGridViewRow();
                        newRow.CreateCells(dataGridView1);
                        newRow.Cells[0].Value = _defaultDate.Month;
                        newRow.Cells[1].Value = _defaultDate.Day;
                        Entry en = new Entry();
                        en.ID = -(r.Index + 1);
                        newRow.Tag = en;
                        dataGridView1.Rows.Insert(r.Index, newRow);
                    }
                    break;
                default:
                    break;
            }
        }

        void InitColumns()
        {
            dataGridView1.Visible = false;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.AllowUserToDeleteRows = true;
            dataGridView1.AllowUserToAddRows = true;
            dataGridView1.TopLeftHeaderCell.Value = "";
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersHeight = 60;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("楷体_GB2312", 12F);

            int i = 0;
            #region 列定义
            DataGridViewColumn[] cs = new DataGridViewColumn[11];
            cs[i] = new DataGridViewTextBoxColumn();
            cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            cs[i].Width = 27;
            cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            cs[i].Resizable = DataGridViewTriState.False;
            cs[i].HeaderText = "月";
            cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            i++;
            cs[i] = new DataGridViewTextBoxColumn();
            cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            cs[i].Width = 27;
            cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            cs[i].Resizable = DataGridViewTriState.False;
            cs[i].HeaderText = "日";
            cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            i++;
            cs[i] = new DataGridViewTextBoxColumn();
            cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            cs[i].Width = 27;
            cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            cs[i].Resizable = DataGridViewTriState.False;
            cs[i].HeaderText = "记账凭证;字;号";// "凭证字";
            cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            i++;
            cs[i] = new DataGridViewTextBoxColumn();
            cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            cs[i].Width = 27;
            cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            cs[i].Resizable = DataGridViewTriState.False;
            cs[i].HeaderText = "号";// "凭证号";
            cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            i++;
            cs[i] = new DataGridViewTextBoxColumn();
            cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            cs[i].Width = 40;
            cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            cs[i].Resizable = DataGridViewTriState.False;
            cs[i].HeaderText = "支票;种类;号码";// "支票号";
            cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            i++;
            cs[i] = new DataGridViewTextBoxColumn();
            cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            cs[i].Width = 40;
            cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            cs[i].Resizable = DataGridViewTriState.False;
            cs[i].HeaderText = "号码";//"支票号";
            cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            i++;
            cs[i] = new DataGridViewTextBoxColumn();
            cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            cs[i].HeaderText = "摘  要";
            cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            i++;
            cs[i] = new CyEditorTextBoxColumn();
            cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            cs[i].Width = 128;
            cs[i].Resizable = DataGridViewTriState.False;
            cs[i].CellTemplate.Style.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            cs[i].HeaderText = "借  方";
            cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            i++;
            cs[i] = new CyEditorTextBoxColumn();
            cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            cs[i].Width = 126;
            cs[i].Resizable = DataGridViewTriState.False;
            cs[i].CellTemplate.Style.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            cs[i].HeaderText = "贷  方";
            cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            i++;
            cs[i] = new CyEditorTextBoxColumn();
            cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            cs[i].Width = 127;
            cs[i].CellTemplate.Style.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            cs[i].HeaderText = "余  额";
            cs[i].Resizable = DataGridViewTriState.False;
            cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            cs[i].ReadOnly = true;
            i++;
            cs[i] = new DataGridViewTextBoxColumn();
            cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            cs[i].Width = 22;
            cs[i].CellTemplate.Style.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            cs[i].HeaderText = "保存";
            cs[i].Resizable = DataGridViewTriState.False;
            cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].ReadOnly = true;
            dataGridView1.Columns.AddRange(cs);
            #endregion
            dataGridView1.Visible = true;
        }

        #endregion

    }
}
