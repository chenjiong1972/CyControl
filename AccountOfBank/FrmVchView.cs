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
    public partial class FrmVchView : Form
    {
        private Control[] _seqOfTabKey;
        private int[] _seqColumnOfTabKey;
        private int _currentIndexOfFocus;

        public bool RadOnly { get; set; }

        public FrmVchView()
        {
            InitializeComponent();
            InitControlEvents();
            this.Load += FrmVchView_Load;
            this.Shown += FrmVchView_Shown;
        }

        void FrmVchView_Shown(object sender, EventArgs e)
        {
            SetControlFocus(0);
        }

        void FrmVchView_Load(object sender, EventArgs e)
        {
            InitControlStyle();
            InitControlData();
        }

        /// <summary>
        /// 设置控件的输入焦点
        /// </summary>
        /// <param name="step">-1=后移;0=回到第一个;1=前移</param>
        void SetControlFocus(int step)
        {
            if (RadOnly)
                return;
            if (step == 0)
            {
                _seqOfTabKey[0].Focus();
                _currentIndexOfFocus = 0;
            }
            else
            {
                ///如果当前激活控件是网格，则另行处理；
                if (_seqOfTabKey[_currentIndexOfFocus] is VchDataGridView)
                {
                    return;
                }
                ///检查当前控件是否输入正确
                ///如果正确，则执行以下代码
                if (step == 1)
                {
                    for (int i = _currentIndexOfFocus + 1; i < _seqOfTabKey.Length; i++)
                    {
                        if (_seqOfTabKey[i].Enabled)
                        {
                            if (_seqOfTabKey[i].Focus())
                            {
                                _currentIndexOfFocus = i;
                                break;
                            }
                        }
                    }
                }
                else if (step == -1)
                {
                    for (int i = _currentIndexOfFocus - 1; i >=0; i--)
                    {
                        if (_seqOfTabKey[i].Enabled)
                        {
                            if (_seqOfTabKey[i].Focus())
                            {
                                _currentIndexOfFocus = i;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 不应重入
        /// </summary>
        void InitControlEvents()
        {
            foreach (Label l in new Label[6] { label3, label6, label8, label10, label12, label14 })
            {
                l.Text = "";
                l.Paint += attachLabel_Paint;
            }
            dataGridView1.ColumnWidthChanged += dataGridView1_ColumnWidthChanged;

        }

        /// <summary>
        /// 可重入
        /// 根据显示模式和录入模式的不同，定义Tab顺序，显示的列数
        /// </summary>
        void InitControlStyle()
        {
           _seqOfTabKey =new Control[]{
           textBox5,textBox6,textBox7,textBox8,
           //dataGridView1,
           textBox1,textBox2,textBox3,textBox4
           };
           _seqColumnOfTabKey = new int[]{
           1,2,3,4,5
           };
           foreach (DataGridView grid in new DataGridView[3] { dataGridView1, dataGridView2, dataGridViewFoot })
           {
               foreach (DataGridViewColumn col in dataGridView1.Columns)
               {
                   if (col is CyEditorTextBoxColumn)
                   {
                       col.CellTemplate.Style.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                   }
               }
           }
           SysnDataGridViewColumnWidth();
        }

        void InitControlData()
        {
            dataGridView1.Rows.Add(100);
            DateTime dt=DateTime.Now;
            textBox6.Text=dt.Year.ToString();
            textBox7.Text=dt.Month.ToString();
            textBox8.Text = dt.Day.ToString();
        }

        void SysnDataGridViewColumnWidth()
        {
            dataGridViewFoot.Columns[1].Width = (dataGridView1.Columns[1].Width + dataGridView1.Columns[2].Width + dataGridView1.Columns[3].Width);
            dataGridViewFoot.Columns[2].Width = (dataGridView1.Columns[dataGridView1.Columns.Count - 2].Width);
            dataGridViewFoot.Columns[3].Width = (dataGridView1.Columns[dataGridView1.Columns.Count - 1].Width);
        }

        void attachLabel_Paint(object sender, PaintEventArgs e) 
        {
            ///如果主管等的内容需要显示有颜色的外框时，执行以下代码
            if (!string.IsNullOrWhiteSpace((sender as Label).Text))
            {
                Rectangle rt = e.ClipRectangle;
                rt.Inflate(-1, -1);
                e.Graphics.DrawRectangle(new Pen(Color.Red), rt);
            }
        }

        void dataGridView1_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            SysnDataGridViewColumnWidth();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            Console.WriteLine("Form.ProcessDialogKey");
            ///在datagridview中就由datagridview控制
            if (keyData == (keyData | Keys.Tab) || keyData == (keyData |Keys.Enter))
            {
                if (keyData == (keyData | Keys.Shift))
                {
                    SetControlFocus(-1);
                }
                else {
                    SetControlFocus(1);
                }
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }

    public class VchDataGridView : DataGridView
    {
        protected override bool ProcessDataGridViewKey(KeyEventArgs e)
        {
            Console.WriteLine("VchDataGridView.ProcessDataGridViewKey");
            Keys keyData = e.KeyData;
            if (e.KeyValue == 13)
            {
                if (e.Shift)
                {
                    return this.ProcessLeftKey(keyData);
                }
                else
                {
                    return this.ProcessRightKey(keyData);
                }
            }
            else if (keyData == Keys.Right)
            {
                Console.WriteLine("ProcessDialogKey");
                return this.ProcessRightKey(keyData);
            }
            else if (keyData == Keys.Left)
            {
                Console.WriteLine("ProcessDialogKey");
                return this.ProcessLeftKey(keyData);
            }
            return base.ProcessDataGridViewKey(e);
        }
        
        public new bool ProcessRightKey(Keys keyData)
        {
            Console.WriteLine("ProcessRightKey");
            if (keyData == Keys.Enter)
            {
                ///Keys.Enter 无需检查是否处于编辑模式 
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
            }
            else if (keyData == Keys.Right)
            {
                ///应检查是否处于编辑模式
                ///处于编辑模式时，左右键用于在网格内部移动，直到在内容的首尾后才跳出
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
            }
            return base.ProcessRightKey(keyData);
        }

        public new bool ProcessLeftKey(Keys keyData)
        {
            Console.WriteLine("ProcessLeftKey");
            ///第0列是分录序号列，用键盘不能进入。
            if (keyData ==(keyData | Keys.Enter))
            {
                ///Keys.Enter 无需检查是否处于编辑模式 
                //第一种情况：只有一行,且当光标移到第一列时
                if ((base.CurrentCell.ColumnIndex <= 1) && (base.CurrentCell.RowIndex == 0))
                {
                    base.CurrentCell = base.Rows[0].Cells[(base.ColumnCount - 1)];
                    return true;
                }
                //第二种情况：有多行，且当光标移到第一列时,移到上一行第一个单元
                if ((base.CurrentCell.ColumnIndex <= 1) && (base.CurrentCell.RowIndex > 0))
                {
                    base.CurrentCell = base.Rows[base.CurrentCell.RowIndex - 1].Cells[(base.ColumnCount - 1)];
                    return true;
                }
            }
            if (keyData == Keys.Left)
            {
                ///应检查是否处于编辑模式
                ///处于编辑模式时，左右键用于在网格内部移动，直到在内容的首尾后才跳出
                //第一种情况：只有一行,且当光标移到第一列时
                if ((base.CurrentCell.ColumnIndex <= 1) && (base.CurrentCell.RowIndex == 0))
                {
                    base.CurrentCell = base.Rows[0].Cells[(base.ColumnCount - 1)];
                    return true;
                }
                //第二种情况：有多行，且当光标移到第一列时,移到上一行第一个单元
                if ((base.CurrentCell.ColumnIndex <= 1) && (base.CurrentCell.RowIndex > 0))
                {
                    base.CurrentCell = base.Rows[base.CurrentCell.RowIndex - 1].Cells[(base.ColumnCount - 1)];
                    return true;
                }
            }
            return base.ProcessLeftKey(keyData);
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            if ((this.Columns[e.ColumnIndex] is CyEditorTextBoxColumn))
            {
                CyEdit_CellPainting(this, e);
            }
            else
            {
                base.OnCellPainting(e);
            }
        }

        private void CyEdit_CellPainting(object sender, System.Windows.Forms.DataGridViewCellPaintingEventArgs e)
        {
            const int CYTITLEHEIGHT = 25;
            VchDataGridView grid = sender as VchDataGridView;
            #region 金额线
            if ((grid.Columns[e.ColumnIndex] is CyEditorTextBoxColumn))
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
                        if (grid.SelectedColumns.Contains(grid.Columns[e.ColumnIndex]) && grid.SelectedRows.Contains(grid.Rows[e.RowIndex]))
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

    }
}
