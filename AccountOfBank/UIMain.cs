using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using UnvaryingSagacity.Core;

namespace UnvaryingSagacity.AccountOfBank
{
    public partial class UIMain : Form
    {
        public const string RIGHT_ERROR = "对不起, 您没有操作该模块的权限."; 
        
        private const string ACCOUNT_MENUITEM_FIX = "打开账套: ";
        private Environment _e = new Environment(Application.CommonAppDataPath);
        private UIState _currentState = UIState.None ;//当前状态;0=无;1=editor;2=view
        private Core.ToolTip _tip = new UnvaryingSagacity.Core.ToolTip();
        private DateTime _defaultDate = DateTime.Today ;
        private ViewFiter _filter = new ViewFiter();
        private DataProvider _dataProvider;
        private EntryExList _currentAccountPage;
        public UIMain()
        {
            InitializeComponent();
            this.toolStripButton1.Text = "关闭";
            this.toolStripButton2.Text = Environment.TOPMODALNAME_4;
            this.toolStripButton3.Text = Environment.TOPMODALNAME_1;
            this.toolStripButton4.Text = Environment.TOPMODALNAME_2;
            this.toolStripButton8.Text = Environment.TOPMODALNAME_0;
            this.toolStripSplitButton1.Text = "预览账页";

            this.panel4.Paint += new PaintEventHandler(panel4_Paint);
            this.panel1.Paint += new PaintEventHandler(panel1_Paint);
            this.splitContainer1.Panel1.Paint += new PaintEventHandler(splitPanel1_Paint); 
            this.Shown += new EventHandler(UIMain_Shown);
            treeView1.AfterSelect += new TreeViewEventHandler(treeView1_AfterSelect);
            treeView1.BeforeSelect += new TreeViewCancelEventHandler(treeView1_BeforeSelect); 
            button5.Click += new EventHandler(button5_Click);
            dataGridView1.RowValidated += new DataGridViewCellEventHandler(dataGridView1_RowValidated);
            dataGridView1.CellPainting += new DataGridViewCellPaintingEventHandler(dataGridView1_CellPainting);
            dataGridView2.CellPainting += new DataGridViewCellPaintingEventHandler(dataGridView1_CellPainting);
            dataGridView1.DefaultValuesNeeded += new DataGridViewRowEventHandler(dataGridView1_DefaultValuesNeeded);
            dataGridView1.CellValidating += new DataGridViewCellValidatingEventHandler(dataGridView1_CellValidating);
            dataGridView1.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dataGridView1_EditingControlShowing);
            dataGridView1.CellValueChanged += new DataGridViewCellEventHandler(dataGridView1_CellValueChanged);
            dataGridView1.KeyUp += new KeyEventHandler(dataGridView1_KeyUp);
            button1.Click += new EventHandler(buttonEditor_Click);
            button2.Click += new EventHandler(buttonEditor_Click);
            button3.Click += new EventHandler(buttonEditor_Click);
            button4.Click += new EventHandler(buttonEditor_Click);
            button6.Click += new EventHandler(buttonEditor_Click);
            panelEditor.SizeChanged += new EventHandler(panelEditor_SizeChanged);
            panelViewer.SizeChanged += new EventHandler(panelViewer_SizeChanged);
            panelUpdateNote.SizeChanged += new EventHandler(panelUpdateNote_SizeChanged);
            panelUpdateNote.Paint += new PaintEventHandler(panelUpdateNote_Paint);
            picBoxUpdateNoteClosed.Click += new EventHandler(picBoxUpdateNoteClosed_Click);
            picBoxHelp.Click += new EventHandler(picBoxHelp_Click); 
            _filter.d1 = DateTime.Today.AddDays(-1.0);
            _filter.d2 = DateTime.Today;
        }

        void picBoxHelp_Click(object sender, EventArgs e)
        {
            DisplayHelp();
        }

        void panelUpdateNote_SizeChanged(object sender, EventArgs e)
        {
            picBoxUpdateNoteClosed.Left = panelUpdateNote.Width - picBoxUpdateNoteClosed.Width - 5;
        }

        void picBoxUpdateNoteClosed_Click(object sender, EventArgs e)
        {
            panelUpdateNote.Visible = false;
        }

        void panelUpdateNote_Paint(object sender, PaintEventArgs e)
        {
            UnvaryingSagacity.Core.TextShadow ts = new UnvaryingSagacity.Core.TextShadow();
            Font f = new Font("宋体", 10, FontStyle.Bold);
            string s = "软件更新说明";
            SizeF sf = e.Graphics.MeasureString(s, f);
            PointF p = new PointF(5.0F, 5.0F);
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            ts.Draw(e.Graphics, s, f, p);
            e.Graphics.DrawString(s, f, Brushes.White, p);
        }

        void panelViewer_SizeChanged(object sender, EventArgs e)
        {
            label3.Left = panelViewer.Right - label3.Width - 10;
        }

        void panelEditor_SizeChanged(object sender, EventArgs e)
        {
            cyEditor1.Left = panelEditor.Right - cyEditor1.Width - 10;
            label1.Left = cyEditor1.Left;
            button4.Left = panelEditor.Right - button4.Width - 10;
            button6.Left = button4.Left  - button6.Width - 10;
        }

        void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
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

        private void dataGridView1_CellPainting(object sender,System.Windows.Forms.DataGridViewCellPaintingEventArgs e)
        {
            const int CYTITLEHEIGHT = 25;
            CustomDataGridView grid = (sender as CustomDataGridView);
            #region Header: year,month,day,VCH;CHEQUE
            if ((e.ColumnIndex >=0 && e.ColumnIndex <=5 ) && e.RowIndex == -1 && _currentState !=UIState.ViewTotal  )
            {
                if(e.ColumnIndex ==0 || e.ColumnIndex ==2 || e.ColumnIndex ==4)
                {
                    //e.Paint(e.ClipBounds, DataGridViewPaintParts.Background |DataGridViewPaintParts.Border  );
                    Rectangle rt=e.CellBounds ;
                    Pen penGridLine=new Pen(grid.GridColor);
                    Pen penWhite = new Pen(Color.White);
                    rt.Width += grid.Columns[e.ColumnIndex+1].Width;

                    e.Graphics.FillRectangle(new SolidBrush(grid.RowHeadersDefaultCellStyle.BackColor), rt);
                    e.Graphics.DrawLine(penGridLine, e.ClipBounds.X, e.ClipBounds.Y, e.ClipBounds.X, rt.Bottom);//Left
                    e.Graphics.DrawLine(penGridLine, rt.X , rt.Y, rt.Right, rt.Y);//Top
                    e.Graphics.DrawLine(penGridLine, rt.Right-1, rt.Y, rt.Right-1, rt.Bottom);//Right
                    e.Graphics.DrawLine(penGridLine, rt.X, rt.Bottom-1, rt.Right, rt.Bottom-1);//bottom

                    e.Graphics.DrawLine(penWhite, rt.Left + 1, rt.Top + 1, rt.Right, rt.Top + 1);//Top
                    if (e.ColumnIndex == 0)
                    {
                        e.Graphics.DrawLine(penWhite, rt.X + 1, rt.Top + 1, rt.X + 1, rt.Bottom);//Left
                    }
                    else
                    {
                        e.Graphics.DrawLine(penWhite, rt.X , rt.Top + 1, rt.X, rt.Bottom);//Left
                    }
                    Rectangle rtYear = rt;
                    Rectangle rtMonth = rt;
                    Rectangle rtDay = rt;
                    rtYear.Height = rt.Height / 2;
                    rtMonth.Height = rtYear.Height;
                    rtDay.Height = rtYear.Height;
                    rtMonth.Width = rtYear.Width / 2;
                    rtDay.Width = rtMonth.Width;
                    rtMonth.Offset(0, rtYear.Height );
                    rtDay.Offset(rtMonth.Width, rtYear.Height);
                    e.Graphics.DrawLine(penGridLine, rtYear.Left + 2, rtYear.Bottom, rtYear.Right - 1, rtYear.Bottom);//center_H
                    if (e.ColumnIndex == 0)
                    {
                        e.Graphics.DrawLine(penGridLine, rtDay.X, rtDay.Top, rtDay.X, rtDay.Bottom);//center_V
                    }
                    else
                    {
                        e.Graphics.DrawLine(penGridLine, rtDay.X-1, rtDay.Top, rtDay.X-1, rtDay.Bottom);//center_V
                    }
                    StringFormat _sf = new StringFormat();
                    _sf.Alignment = StringAlignment.Center;
                    _sf.LineAlignment = StringAlignment.Center;

                string[] ss = e.Value.ToString().Split(";".ToCharArray());
                e.Graphics.DrawString(ss[0], new Font(e.CellStyle.Font.Name, 9), new SolidBrush(e.CellStyle.ForeColor), rtYear, _sf);
                e.Graphics.DrawString(ss[1], e.CellStyle.Font, new SolidBrush(e.CellStyle.ForeColor), rtMonth, _sf);
                e.Graphics.DrawString(ss[2], e.CellStyle.Font, new SolidBrush(e.CellStyle.ForeColor), rtDay, _sf);
                }
                e.Handled = true;
            }
            #endregion

            #region 金额线
            if ((7 <= e.ColumnIndex && 9>=e.ColumnIndex  && _currentState !=UIState.ViewTotal ) ||(2 <= e.ColumnIndex && 5>=e.ColumnIndex   && _currentState ==UIState.ViewTotal ))
            {
                int _charFullWidth = 0;
                int _charWidth = 0;
                RectangleF rtCyBound = new RectangleF();
                RectangleF[] rtfs = new RectangleF[0];
                Font font = grid.Columns[e.ColumnIndex].CellTemplate.Style.Font;
                e.Paint(e.ClipBounds, DataGridViewPaintParts.Border | DataGridViewPaintParts.Background|DataGridViewPaintParts.Focus  );
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

        void InitMenus()
        {
            this.toolStripSplitButton1.DropDownItems.Add(new ToolStripMenuItem ("打印账本..."));
            this.toolStripSplitButton1.DropDownItems.Add(new ToolStripSeparator());
            this.toolStripSplitButton1.DropDownItems.Add(new ToolStripMenuItem("打印当前账页"));
            this.toolStripSplitButton1.DropDownItems.Add(new ToolStripMenuItem("打印设置"));
            this.toolStripSplitButton1.DropDownItems.Add(new ToolStripSeparator());
            this.toolStripSplitButton1.DropDownItems.Add(new ToolStripMenuItem("账本封面设计"));
            this.toolStripButton8.DropDownItems.Clear();
            //this.toolStripButton8.DropDownItems.Add(new ToolStripMenuItem("结算方式管理..."));
            this.toolStripButton8.DropDownItems.Add(new ToolStripMenuItem("银行设置..."));
            this.toolStripButton8.DropDownItems.Add(new ToolStripSeparator());
            this.toolStripButton8.DropDownItems.Add(new ToolStripMenuItem("用户权限管理..."));
            this.toolStripButton8.DropDownItems.Add(new ToolStripSeparator());
            this.toolStripButton8.DropDownItems.Add(new ToolStripMenuItem("查看更新说明"));
            this.toolStripButton8.DropDownItems.Add(new ToolStripMenuItem("查看帮助"));
            this.toolStripButton8.DropDownItems.Add(new ToolStripSeparator());
            this.toolStripButton8.DropDownItems.Add(new ToolStripMenuItem("注销"));
            this.picBoxHelp.MouseEnter += new EventHandler(picBoxHelp_MouseEnter);
            this.picBoxHelp.MouseLeave += new EventHandler(picBoxHelp_MouseLeave);
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
                else if (button.GetType() == typeof(ToolStripDropDownButton ))
                {
                    ToolStripDropDownButton ddb=button  as ToolStripDropDownButton;
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
            foreach (object button in toolStrip2.Items)
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
                }
            }
            #endregion
            InitAccountMenus();//放在button Event 后面,否则事件会叠加.
        }

        void picBoxHelp_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default; 
        }


        void picBoxHelp_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        void InitAccountMenus()
        {
            this.toolStripButton2.DropDownItems.Clear();
            int i = this.toolStripButton2.DropDownItems.Add(new ToolStripMenuItem("账套管理..."));
            this.toolStripButton2.DropDownItems[i].Click += new EventHandler(button_Click);
            i = this.toolStripButton2.DropDownItems.Add(new ToolStripMenuItem("账户设置..."));
            this.toolStripButton2.DropDownItems[i].Click += new EventHandler(button_Click);
            this.toolStripButton2.DropDownItems.Add(new ToolStripSeparator());
            Accounts acts = _e.GetAccountList();
            if (acts.Count > 0)
            {
                foreach (Account act in acts)
                {
                    if (System.IO.File.Exists(act.FullPath))
                    {

                        i = this.toolStripButton2.DropDownItems.Add(new ToolStripMenuItem(ACCOUNT_MENUITEM_FIX + act.ID + " " + act.Name));
                        this.toolStripButton2.DropDownItems[i].Click += new EventHandler(button_Click);
                    }
                }
                this.toolStripButton2.DropDownItems.Add(new ToolStripSeparator());
                i = this.toolStripButton2.DropDownItems.Add(new ToolStripMenuItem("设置启用年度"));
                this.toolStripButton2.DropDownItems[i].Click += new EventHandler(button_Click);
                i = this.toolStripButton2.DropDownItems.Add(new ToolStripMenuItem("年末结账"));
                this.toolStripButton2.DropDownItems[i].Click += new EventHandler(button_Click);
            }
            else
            {
                ToolStripMenuItem menu = new ToolStripMenuItem("没有可以打开的账套...");
                menu.Enabled = false;
                this.toolStripButton2.DropDownItems.Add(menu);
            }
        }
        void ResetUI()
        {
            panelViewer.Visible = false;
            panelEditor.Visible = false;
            _currentState = UIState.None;
            CloseDataProvider();
            panel1.Refresh(); 
        }
        /// <summary>
        /// 基础资料变动后,调整界面
        /// 例如,录入时增加了结算方式,要调整网格的结算方式选择.
        /// </summary>
        /// <param name="obj"></param>
        void BaseObjectChanged(InternalBaseObject obj)
        {
            switch (obj)
            {
                case InternalBaseObject.账套 :
                    InitAccountMenus();
                    if (IsOpenAccount(false ) )
                    {
                        if (!System.IO.File.Exists(_e.CurrentAccount.FullPath))
                        {
                            _e.CurrentAccount = null;
                            ResetUI();
                        }
                    }
                    break;
                case InternalBaseObject.账户 :
                    ResetUI();
                    RefreshItemOfBankTree();
                    break;
                case InternalBaseObject.结算方式 :
                    if (_currentState == UIState.Editor)
                    {
                        DataGridViewComboBoxColumn cs = (dataGridView1.Columns[3] as DataGridViewComboBoxColumn);
                        cs.Items.Clear();
                        cs.Items.AddRange(_e.GetSetting("settle"));

                    }
                    break;
            }
        }

        void BaseObjectEdit(InternalBaseObject obj)
        {
            string id="";
            switch (obj)
            {
                case InternalBaseObject.结算方式 :
                    id = "0102";
                    break;
                case InternalBaseObject.银行 :
                    id = "0103";
                    break;
                case InternalBaseObject.账套 :
                    id = "02";
                    break;
                case InternalBaseObject.账户:
                    id = "03." + _e.CurrentAccount.ID;
                    break;
            }
            if (_e.CurrentUser.GetRightState(id)!=UserAndRight.RightState.完全  )
            {
                MessageBox.Show(this, RIGHT_ERROR, "检查权限", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            UIBaseInfoManager ui = new UIBaseInfoManager();
            ui.BaseObjectType = obj;
            ui.CurrentEnvironment = _e;
            ui.ShowDialog(this);
            if (ui.BaseObjectChanged )
            {
                BaseObjectChanged(obj);
            }
        }
        bool IsOpenAccount(bool allowPrompt)
        {
            if (_e.CurrentAccount == null)
            {
                if (allowPrompt)
                {
                    MessageBox.Show(this, "当前没有账套被打开, 请先打开账套或者新建账套", "打开账套", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return false;
            }
            return true;
        }
        void SetEditStyle()
        {
            if (IsOpenAccount(true ))
            {
                ItemOfBank item = GetCurrentItemOfBank();
                if (item == null)
                {
                    MessageBox.Show(this, "当前没有账户被选择, 请先选择账户", "录入账户的银行日记账", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (_e.CurrentUser.GetRightState("04." + _e.CurrentAccount.ID + "." + item.ID) != UserAndRight.RightState.完全)
                {
                    MessageBox.Show(this, RIGHT_ERROR, "检查权限", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                panelViewer.Visible = false;
                if (_dataProvider == null)
                {
                    _dataProvider = new DataProvider(_e.CurrentAccount.FullPath);
                }
                if (panelEditor.Dock != DockStyle.Fill)
                    panelEditor.Dock = DockStyle.Fill;
                InitEditor();
                if (item != null)
                {
                    label5.Text = "开户银行: " + item.OfBankName;
                    label6.Text = "账    号: " + item.ID;
                    dataGridView1.Enabled = true; 
                }
                else
                {
                    label5.Text = "开户银行: (请在窗口左边选中要操作的账户)";
                    label6.Text = "账    号:";
                    dataGridView1.Enabled = false;
                }
                panelEditor.Visible = true;
            }
        }

        void SetViewStyle()
        {
            if (IsOpenAccount(true))
            {
                ItemOfBank item = GetCurrentItemOfBank();
                //这里留有漏洞,查所有账户
                if (item != null)
                {
                    if (_e.CurrentUser.GetRightState("05." + _e.CurrentAccount.ID + "." + item.ID) != UserAndRight.RightState.完全)
                    {
                        MessageBox.Show(this, RIGHT_ERROR, "检查权限", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                panelEditor.Visible = false;
                if (_dataProvider == null)
                {
                    _dataProvider = new DataProvider(_e.CurrentAccount.FullPath);
                }
                if (panelViewer.Dock != DockStyle.Fill)
                    panelViewer.Dock = DockStyle.Fill;
                InitViewer();
                if (item != null)
                {
                    label7.Text = "开户银行: " + item.OfBankName;
                    label4.Text = "账    号: " + item.ID;
                }
                else
                {
                    label7.Text = "开户银行: (所有银行) ";
                    label4.Text = "账    号: (所有账号)";
                }
                panelViewer.Visible = true;
            }
        }

        bool SetStartYear()
        {
            if (IsOpenAccount(true))
            {
                Core.InputBox input = new InputBox("设置启用年度", "设置账套的启用年度，\n\n请输入启用年度.", 4, _e.CurrentAccount.StartYear.ToString());
                if (input.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        _e.CurrentAccount.StartYear = int.Parse(input.Result.ToString());
                        _dataProvider.SetConfig(DataProvider.CONFIG_STARTDATE, _e.CurrentAccount.StartYear.ToString());
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "启用年度", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
            }
            return false;
        }

        void CloseYear()
        {
            if (IsOpenAccount(true))
            {
                UIYearClose ui = new UIYearClose();
                ui.CurrentEnvironment = _e;
                ui.CurrentDataProvider = _dataProvider;
                ui.ShowDialog(this); 
            }
        }

        /// <summary>
        /// 由菜单项文本来打开账套
        /// </summary>
        /// <param name="idAndName">id+空格+name</param>
        void OpenAccount(string idAndName)
        {
            Accounts acts = _e.GetAccountList();
            foreach (Account act in acts)
            {
                if ((act.ID + " " + act.Name) == idAndName)
                {
                    if (_e.CurrentUser.GetRightState("06." + act.ID) != UserAndRight.RightState.完全)
                    {
                        MessageBox.Show(this, RIGHT_ERROR, "检查权限", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (System.IO.File.Exists(act.FullPath))
                    {
                        _e.CurrentAccount = new Account();
                        act.CopyTo(_e.CurrentAccount);
                        panel1.Refresh();
                        ResetUI();
                        _dataProvider = new DataProvider(_e.CurrentAccount.FullPath);
                        _dataProvider.SetConfig("LastAccess", DateTime.Today.ToString ());
                        #region 检查启用日期，没有就补上
                        string s = _dataProvider.GetConfig(DataProvider.CONFIG_STARTDATE);
                        if (s.Length != 4)
                        {
                            _e.CurrentAccount.StartYear = 0;
                            Core.InputBox input = new InputBox("设置启用年度", "您还未设置启用年度，\n\n请输入启用年度.", 4, DateTime.Today.Year.ToString());
                            if (input.ShowDialog(this) == DialogResult.OK)
                            {
                                int y;
                                int.TryParse(input.Result.ToString(), out y);
                                _e.CurrentAccount.StartYear = y;
                            }
                            if (_e.CurrentAccount.StartYear == 0)
                            {
                                _e.CurrentAccount.StartYear = DateTime.Today.Year;
                                MessageBox.Show(this, "您没有设置启用年度, 系统将把[" + _e.CurrentAccount.StartYear + "]做为启用年度.\n\n您可以通过主菜单[系统设置]->[设置启用年度]来重设.", "启用年度", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            _dataProvider.SetConfig(DataProvider.CONFIG_STARTDATE, _e.CurrentAccount.StartYear.ToString());
                        }
                        else
                            _e.CurrentAccount.StartYear = int.Parse(s); 
                        #endregion


                        #region　检查是否要备份
                        DateTime lastBackup=_dataProvider.LastBackupDateTime;
                        if (lastBackup.ToString("yyyy-MM-dd") == "1972-11-02")
                        {
                            _dataProvider.LastBackupDateTime = DateTime.Today;
                        }
                        else
                        {
                            System.TimeSpan diff1 = DateTime.Today.Subtract(lastBackup);
                            if (diff1.Days  >= 28)
                            {
                                MessageBox.Show(this, "该账套至少" + diff1.Days.ToString() + "天没有做备份了, 请您注意备份.", "打开账套", MessageBoxButtons.OK, MessageBoxIcon.Information);
                             }
                        }
                        #endregion
                        RefreshItemOfBankTree();
                    }
                    else
                    {
                        MessageBox.Show(this,"该账套的数据库文件不存在, 不能被打开.","打开账套",MessageBoxButtons.OK ,MessageBoxIcon.Information   ); 
                    }
                    return;

                }
            }
        }

        void RefreshItemOfBankTree()
        {
            treeView1.Nodes["root"].Nodes.Clear();
            if (IsOpenAccount(false))
            {
                if (_dataProvider == null)
                {
                    _dataProvider = new DataProvider(_e.CurrentAccount.FullPath);
                }
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("账套名称:" + _e.CurrentAccount.Name);
                sb.AppendLine("账套编号:" + _e.CurrentAccount.ID);
                sb.AppendLine("账套描述:" + _e.CurrentAccount.Description);
                sb.AppendLine("启用年度:" + _e.CurrentAccount.StartYear + "年");
                treeView1.Nodes["root"].ToolTipText = sb.ToString();
                ItemOfBankCollection items = _dataProvider.GetItemOfBankList();
                foreach (ItemOfBank it in items)
                {
                    sb.Remove(0, sb.Length);
                    sb.AppendLine("账 户 名:"+it.Name);
                    sb.AppendLine("账    号:" + it.ID);
                    sb.AppendLine("隶属银行:" + it.OfBankName);
                    sb.AppendLine("备    注:" + it.Description);
                    sb.AppendLine("期初余额:" + it.StartBal.ToString ("c"));
                    TreeNode item = new TreeNode();
                    item.ImageKey = "itemOfBank";
                    item.SelectedImageKey = "itemOfBank"; 
                    item.Text = it.Name;
                    item.ToolTipText = sb.ToString();
                    item.Tag = it;
                    treeView1.Nodes["root"].Nodes.Add(item);
                }
                if (treeView1.Nodes["root"].Nodes.Count > 0)
                {
                    treeView1.SelectedNode = treeView1.Nodes["root"].Nodes[0];
                }
            }
        }

        void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (_currentState == UIState.View || _currentState ==UIState.ViewTotal  )
            {
                SetViewStyle();
            }
            else if (_currentState == UIState.Editor)
            {
                SetEditStyle();
            }
        }
        void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (_currentState == UIState.Editor)
            {
                if (e.Node.Name == "root")
                    e.Cancel = true;
            }
        }

        void splitPanel1_Paint(object sender, PaintEventArgs e)
        {
            Control c = (sender as Control);
            Graphics g = e.Graphics;


            Rectangle rt = new Rectangle(new Point(c.Right - 2, 30), new Size(1, (c.Height - 60) / 2));
            LinearGradientBrush brush = new LinearGradientBrush(rt, c.BackColor, Color.Blue, 90);
            g.FillRectangle(brush, rt);

            rt.Offset(0, rt.Height - 3);
            brush = new LinearGradientBrush(rt, Color.Blue, c.BackColor, 90);
            g.FillRectangle(brush, rt);
        }

        void UIMain_Shown(object sender, EventArgs e)
        {
            this.Text = "我的银行日记账";
            /// 放在Login之前,取得上次登录日期（未考虑多用户的情况）
            DateTime d = System.IO.File.GetLastWriteTime(Application.StartupPath + "\\银行日记账软件修改说明.rtf");
            string[] ss = _e.GetSettingAttribute("lastLogin", "d");
            System.TimeSpan diff1;
            if (ss[0].Length > 0)
            {
                diff1 = DateTime.Parse(ss[0]).Subtract(d);
            }
            else
            {
                diff1 = d.Subtract(d);
            }
            ///
            if (Login() != DialogResult.OK)
            {
                this.Close();
                return;
            }
            InitMenus();
            TreeNode node= treeView1.Nodes.Add("本账套的账户");
            node.ImageKey = "all";
            node.Name = "root";
            
            if (diff1.Days >= 3)
            {
                panelUpdateNote.Visible = false; 
            }
            else
                richTextBox1.LoadFile(Application.StartupPath + "\\银行日记账软件修改说明.rtf");
        }

        DialogResult Login()
        {
            UILogin login = new UILogin();
            login.CurrentEnvironment = _e;
            DialogResult result = login.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                if (_e.CurrentUser.Name.Length > 0)
                {
                    this.toolStripLabel1.Text = "当前用户: " + _e.CurrentUser.Name;
                }
                ResetUI();
                CloseAccount();
            }
            return result;

        }

        void UserEditor()
        {
            if (_e.CurrentUser.GetRightState("0101") != UserAndRight.RightState.完全  )
            {
                MessageBox.Show(this,RIGHT_ERROR,"检查权限",MessageBoxButtons.OK ,MessageBoxIcon.Information   );
                return;
            }
            UIUserManager ui = new UIUserManager();
            ui.CurrentEnvironment = _e;
            ui.ShowDialog(this);
        }

        void button_Click(object sender, EventArgs e)
        {
            string s=((ToolStripItem)sender).Text.Trim();
            switch (s)
            {
                case Environment.TOPMODALNAME_1  :
                    SetEditStyle();
                    break;
                case Environment.TOPMODALNAME_2:
                    _currentState = UIState.View;
                    SetViewStyle();
                    break;
                case "查看账户日报表":
                    _currentState = UIState.ViewTotal;
                    SetViewStyle();
                    break;
                case Environment.TOPMODALNAME_3+"...":
                    UserEditor();
                    break;
                case "账套管理...":
                    BaseObjectEdit(InternalBaseObject.账套); 
                    break;
                case "账户设置...":
                    if (IsOpenAccount (true ))
                    {
                        BaseObjectEdit(InternalBaseObject.账户);
                    }
                    break;
                case "结算方式管理...":
                    BaseObjectEdit(InternalBaseObject.结算方式); 
                    break;
                case "银行设置...":
                    BaseObjectEdit(InternalBaseObject.银行 );
                    break;
                case "打印":
                    break;
                case "打印设置...":
                    break;
                case "打印预览...":
                    break;
                case "关闭":
                    this.Close();
                    break;
                case "hidden":
                    this.splitContainer1.SplitterDistance = this.splitContainer1.Panel1MinSize ;
                    this.toolStripButton6.Image = global::UnvaryingSagacity.AccountOfBank.Properties.Resources.png_1677;
                    this.toolStripButton6.Text = "display";
                    this.toolStripButton6.ToolTipText = "展开";
                    break;
                case "display":
                    this.splitContainer1.SplitterDistance = 182;
                    this.toolStripButton6.Image = global::UnvaryingSagacity.AccountOfBank.Properties.Resources.png_0063;
                    this.toolStripButton6.Text = "hidden";
                    this.toolStripButton6.ToolTipText = "隐藏";
                    break;
                case "注销":
                    if (Login() == DialogResult.OK)
                        ResetUI();
                    break;
                case "打印当前账页":
                    if (_currentState == UIState.View)
                    {
                        PrintAccountPage(false);
                    }
                    else if (_currentState == UIState.ViewTotal)
                    {
                        PrintDailyReport(false);
                    }
                    break;
                case "打印账本...":
                    PrintAccountBook();
                    break;
                case "预览账页":
                    if (_currentState == UIState.View)
                    {
                        PrintAccountPage(true);
                    }
                    else if (_currentState == UIState.ViewTotal)
                    {
                        PrintDailyReport(true);
                    }
                    break;
                case "打印设置":
                    PrintSetup();
                    break;
                case "账本封面设计":
                    AccountCoverDesigner();
                    break;
                case "查看更新说明":
                    panelUpdateNote.Visible = true;
                    break;
                case "查看帮助":
                    DisplayHelp();
                    break;
                case "设置启用年度":
                    SetStartYear();
                    break;
                case "年末结账":
                    CloseYear();
                    break;
                default:
                    if (s.IndexOf (ACCOUNT_MENUITEM_FIX)==0)
                    {
                        OpenAccount(s.Replace(UIMain.ACCOUNT_MENUITEM_FIX, "")); 
                    }
                    break;
            }
        }

        void panel4_Paint(object sender, PaintEventArgs e)
        {

            UnvaryingSagacity.Core.TextShadow ts = new UnvaryingSagacity.Core.TextShadow();
            Font f = new Font("宋体", 10, FontStyle.Bold);
            string s="开发商:"+Application.CompanyName;
            SizeF sf = e.Graphics.MeasureString(s, f);
            PointF p = new PointF(panel4.Width - sf.Width - 5.0F, panel4.Height -sf.Height -5.0F);
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias; 
            ts.Draw(e.Graphics, s, f, p);
            e.Graphics.DrawString(s, f, Brushes.White , p);
        }

        void panel1_Paint(object sender, PaintEventArgs e)
        {

            UnvaryingSagacity.Core.TextShadow ts = new UnvaryingSagacity.Core.TextShadow();
            Font f = new Font("隶书", 36, FontStyle.Italic  );
            string s = "银行存款日记账";
            SizeF sf = e.Graphics.MeasureString(s, f);
            PointF p = new PointF(5.0F, 5.0F);
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            ts.Draw(e.Graphics, s, f, p);
            e.Graphics.DrawString(s, f, Brushes.Black, p);
            StringBuilder sb = new StringBuilder();
            if (_e.CurrentAccount != null)
            {
                sb.AppendLine("账套编号: " + _e.CurrentAccount.ID);
                sb.AppendLine("名    称: " + _e.CurrentAccount.Name);
                sb.AppendLine("描    述: " + _e.CurrentAccount.Description);
                s = sb.ToString();
            }
            else
            {
                sb.Append("当前账套: 无");
                s = sb.ToString();
            }
            f = new Font(new FontFamily("宋体"), 11, FontStyle.Bold);
            SizeF sizef = e.Graphics.MeasureString(s, f);
            p = new PointF(panel1.Right - sizef.Width - 5, panel1.Bottom - sizef.Height - 5);
            ts.Draw(e.Graphics, s, f, p);
            e.Graphics.DrawString(s, f, new SolidBrush(Color.White), new RectangleF(p, sizef));

        }

        void CloseDataProvider()
        {
            if (_dataProvider != null)
            {
                _dataProvider.Closed();
                _dataProvider = null;
            }
        }
        void CloseAccount()
        {
            CloseDataProvider();
            if (_e.CurrentAccount != null)
            {
                _e.CurrentAccount = null;
                treeView1.Nodes["root"].Nodes.Clear();  
                panel1.Refresh();
            }
        }

        ItemOfBank GetCurrentItemOfBank()
        {
            if (treeView1.SelectedNode.Tag is ItemOfBank)
            {
                ItemOfBank it = new ItemOfBank();
                (treeView1.SelectedNode.Tag as ItemOfBank).CopyTo(it);
                return it;
            }
            return default(ItemOfBank);
        }

        void DisplayHelp()
        {
            UIHelp uihlp = new UIHelp();
            uihlp.Show(this);
        }

        #region 设计录入界面
        void InitEditor()
        {
            toolStrip1.Enabled = false;
            InitColumns();
            dataGridView1.Columns[0].HeaderCell.Value = (_defaultDate.Year + "年;月;日");
            SetDefaultDate(_defaultDate);
            toolStrip1.Enabled = true;
            toolStripSplitButton1.Enabled = true;
            _currentState = UIState.Editor;
        }
        
        void SetDefaultDate(DateTime d)
        {
            textBox1.Text = d.ToLongDateString();
            double dBal=GetStartBal();
            textBox2.Text = dBal.ToString();
            cyEditor1.Text = dBal.ToString();
            ItemOfBank item=GetCurrentItemOfBank ();
            if (item != null)
            {
                dataGridView1.RowValidated -= dataGridView1_RowValidated;
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
                
                bool b = _dataProvider.IsClosed(_defaultDate.Year, item);
                dataGridView1.EditMode = b ? DataGridViewEditMode.EditProgrammatically : DataGridViewEditMode.EditOnEnter; //EditOnKeystroke;// .EditProgrammatically;
                button4.Enabled = !b;
                button6.Enabled = !b;
                labelYearClosed.Visible = b;

                EntryExList entries = _dataProvider.GetEntryList("id='" + item.ID + "' AND RecordDate='" + Core.General.FromDateTime(d) + "'", "entry"); 
                if (entries != null)
                {
                    CustomDataGridView grid=dataGridView1 ;
                    foreach (Entry entry in entries)
                    {
                        DateTime dt = Core.General.FromString(entry.Date);

                        int i = grid.Rows.Add();
                        grid[0, i].Value = dt.Month;
                        grid[1, i].Value = dt.Day;
                        grid[2, i].Value = entry.VchType ;
                        grid[3, i].Value = entry.VchNumber;
                        grid[4, i].Value =entry.ChequeType  ;
                        grid[5, i].Value = entry.Cheque  ;
                        grid[6, i].Value = entry.Digest  ; 
                        if (entry.Side == 1)
                        {
                            grid[7, i].Value = entry.Money.ToString ("0.00"); 
                        }
                        else
                        {
                            grid[8, i].Value = entry.Money.ToString("0.00"); 
                        }
                        //余额列最后一起算.grid[9, i].Value = d;

                        grid[10, i].Value = UnvaryingSagacity.Core.Printer.PrintAssign.OK_FLAG;
                        grid.Rows[i].Tag = entry;
                    }
                    CalcBal(0);
                }
                dataGridView1.RowValidated += new DataGridViewCellEventHandler(dataGridView1_RowValidated);
                dataGridView1.DefaultValuesNeeded += new DataGridViewRowEventHandler(dataGridView1_DefaultValuesNeeded);
                dataGridView1.CellValidating += new DataGridViewCellValidatingEventHandler(dataGridView1_CellValidating);
                dataGridView1.CellValueChanged += new DataGridViewCellEventHandler(dataGridView1_CellValueChanged);
            }

        }

        void SetDefaultDate()
        {
            UIDateBrower ui = new UIDateBrower();
            ui.d = _defaultDate;
            if (ui.ShowDialog(this) == DialogResult.OK)
            {
                SetDefaultDate(ui.d);
            }
        }

        void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            if (dataGridView1.Rows[e.RowIndex].Tag is Entry && e.ColumnIndex <9)
            {
                (dataGridView1.Rows[e.RowIndex].Tag as Entry).Changed = true;
                dataGridView1.Rows[e.RowIndex].Cells[10].Value ="";
            }
        }

        void dataGridView1_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            ItemOfBank item = GetCurrentItemOfBank();
            Entry entry;
            bool append = false;
            if(dataGridView1.Rows[e.RowIndex].Tag is Entry )
            {
                entry = dataGridView1.Rows[e.RowIndex].Tag as Entry;
                if (!entry.Changed)
                    return;
            }
            else
            {
                entry = new Entry();
                append = true;
            }
            try{
                entry.Date = Core.General.FromDateTime(new DateTime(_defaultDate.Year, int.Parse(dataGridView1[0, e.RowIndex].Value.ToString()), int.Parse(dataGridView1[1, e.RowIndex].Value.ToString())));
                entry.VchType  = dataGridView1[2, e.RowIndex].Value == null ? "" : dataGridView1[2, e.RowIndex].Value.ToString();
                entry.VchNumber = dataGridView1[3, e.RowIndex].Value == null ? "" : dataGridView1[3, e.RowIndex].Value.ToString();
                entry.ChequeType  = dataGridView1[4, e.RowIndex].Value == null ? "" : dataGridView1[4, e.RowIndex].Value.ToString();
                entry.Cheque = dataGridView1[5, e.RowIndex].Value == null ? "" : dataGridView1[5, e.RowIndex].Value.ToString();
                entry.Digest = dataGridView1[6, e.RowIndex].Value == null ? "" : dataGridView1[6, e.RowIndex].Value.ToString();
                string s = dataGridView1[7, e.RowIndex].Value == null ? "" : dataGridView1[7, e.RowIndex].Value.ToString();
                int i = 0;
                if (s.Length <= 0)
                {
                    s = dataGridView1[8, e.RowIndex].Value.ToString();
                    entry.Side = -1;
                    entry.Money = Double.Parse(s);
                }
                else
                {
                    entry.Side = 1;
                    entry.Money = Double.Parse(s);
                }

                if (append)
                {
                    i = _dataProvider.StorgeEntry(entry, item);
                    if (i > 0)
                    {
                        entry.ID = i;
                        dataGridView1.Rows[e.RowIndex].Tag = entry;
                        dataGridView1[10, e.RowIndex].Value = Core.Printer.PrintAssign.OK_FLAG;
                        entry.Changed = false;
                    }
                    else
                        dataGridView1[10, e.RowIndex].Value = "";
                }
                else
                {
                    bool b;
                    if (entry.ID < 0)
                    {
                        b = _dataProvider.InsertEntry (entry, item);
                        if (b)
                        {
                            for (i = e.RowIndex + 1; i < dataGridView1.Rows.Count; i++)
                            {
                                if (dataGridView1.Rows[i].Tag is Entry)
                                {
                                    entry = dataGridView1.Rows[i].Tag as Entry;
                                    entry.ID++;
                                }
                            }
                        }
                    }
                    else
                    {
                        b = _dataProvider.UpdateEntry(entry, item);
                    }
                    if (b)
                    {
                        entry.Changed = false;
                        dataGridView1.Rows[e.RowIndex].Tag = entry;
                        dataGridView1[10, e.RowIndex].Value = Core.Printer.PrintAssign.OK_FLAG;
                    }
                    else
                        dataGridView1[10, e.RowIndex].Value = "";
                }
            }
            catch
            {
            }

        }

        void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
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
            if ((e.ColumnIndex >= 7 && e.ColumnIndex <= 8) || (e.ColumnIndex >= 0 && e.ColumnIndex <= 1) || (e.ColumnIndex==3 ))
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
                                    CalcBal(e.RowIndex);
                                }
                                else if (e.ColumnIndex == 8)
                                {
                                    dataGridView1[7, e.RowIndex].Value = "";
                                    CalcBal(e.RowIndex);
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

        void CalcBal(int startRow)
        {
            dataGridView1.CellValueChanged -= dataGridView1_CellValueChanged;
            double bal = 0;
            if (startRow == 0)
            {
                bal = double.Parse(textBox2.Text);
            }
            else
            {
                bal = double.Parse(dataGridView1[9, startRow - 1].Value.ToString());
            }
            for (int i = startRow; i <= dataGridView1.Rows.Count -1; i++)
            {
                if (!dataGridView1.Rows[i].IsNewRow)
                {
                    string s;
                    if (dataGridView1[7, i].IsInEditMode)
                        s = dataGridView1[7, i].EditedFormattedValue == null ? "" : dataGridView1[7, i].EditedFormattedValue.ToString();
                    else
                        s = dataGridView1[7, i].Value == null ? "" : dataGridView1[7, i].Value.ToString();
                    double d;
                    if (s.Length <= 0)
                        d = 0;
                    else
                        d = double.Parse(s);
                    bal += d;
                    if (dataGridView1[8, i].IsInEditMode)
                        s = dataGridView1[8, i].EditedFormattedValue == null ? "" : dataGridView1[8, i].EditedFormattedValue.ToString();
                    else
                        s = dataGridView1[8, i].Value == null ? "" : dataGridView1[8, i].Value.ToString();
                    if (s.Length <= 0)
                        d = 0;
                    else
                        d = double.Parse(s);
                    bal -= d;
                    dataGridView1[9, i].Value = bal.ToString("0.00");
                }
            }
            dataGridView1.CellValueChanged +=new DataGridViewCellEventHandler(dataGridView1_CellValueChanged);
        }

        void buttonEditor_Click(object sender, EventArgs e)
        {
            switch ((sender as Control).Name.ToLower() )
            {
                case "button1":
                    SetDefaultDate(_defaultDate.AddDays(-1.0));
                    break;
                case "button2":
                    SetDefaultDate(_defaultDate.AddDays(1));
                    break;
                case "button3":
                    SetDefaultDate();
                    break;
                case "button4":
                    if (dataGridView1.SelectedCells[0].OwningRow.IsNewRow)
                    {
                        return;
                    }
                    if (MessageBox.Show(this, "您确定要删除选中的分录吗?", "删除分录", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (dataGridView1.SelectedCells[0].OwningRow.Tag is Entry)
                        {
                            ItemOfBank item = GetCurrentItemOfBank();
                            if (_dataProvider.RemoveEntry(dataGridView1.SelectedCells[0].OwningRow.Tag as Entry, item))
                            {
                                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedCells[0].OwningRow.Index);
                            }
                        }
                        else
                        {
                            dataGridView1.Rows.RemoveAt(dataGridView1.SelectedCells[0].OwningRow.Index);
                        }
                    }
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
                default :
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
            cs[i].HeaderText = "记账凭证;字;号";// "凭证字";
            cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            i++;
            cs[i] = new DataGridViewTextBoxColumn();
            cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            cs[i].Width = 27;
            cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            cs[i].HeaderText = "号";// "凭证号";
            cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            i++;
            cs[i] = new DataGridViewTextBoxColumn();
            cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            cs[i].Width = 40;
            cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            cs[i].HeaderText = "支票;种类;号码";// "支票号";
            cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            i++;
            cs[i] = new DataGridViewTextBoxColumn();
            cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            cs[i].Width = 40;
            cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
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
            cs[i].Width = 160;
            cs[i].CellTemplate.Style.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            cs[i].HeaderText = "借  方";
            cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            i++;
            cs[i] = new CyEditorTextBoxColumn();
            cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            cs[i].Width = 160;
            cs[i].CellTemplate.Style.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            cs[i].HeaderText = "贷  方";
            cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            i++;
            cs[i] = new CyEditorTextBoxColumn();
            cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            cs[i].Width = 160;
            cs[i].CellTemplate.Style.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            cs[i].HeaderText = "余  额";
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
            cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cs[i].ReadOnly = true;
            dataGridView1.Columns.AddRange(cs);
            #endregion
            dataGridView1.Visible = true;
        }

        #endregion

        #region 设计查询界面
        void InitViewer()
        {
            toolStrip1.Enabled = false;
            InitViewerColumns();
            toolStrip1.Enabled = true;
            toolStripSplitButton1.Enabled = true; 

            if (_filter != null)
            {
                label3.Text = "开始日期:" + _filter.d1.ToLongDateString() + "\n\n截止日期:" + _filter.d2.ToLongDateString();
                //label3.Left = panelViewer.ClientRectangle.Right   - label3.Width - 10;
                label3.Visible = true;
                GetData();
            }
            else
            {
                label3.Visible = false; 
            }
        }

        void button5_Click(object sender, EventArgs e)
        {
            UIViewFilter f = new UIViewFilter();
            f.d1 = _filter.d1;
            f.d2 = _filter.d2;
            DialogResult result = f.ShowDialog(this);
            if (result != DialogResult.OK)
                return;
            _filter.d1 = f.d1;
            _filter.d2 = f.d2;
            GetData();
        }

        void GetData()
        {
            DataGridView grid = dataGridView2;
            grid.Rows.Clear();
            string filterExpress = GetFilterExpress();
            label3.Text = "开始日期:" + _filter.d1.ToString("yyyy年MM月dd日") + "\n\n截止日期:" + _filter.d2.ToString("yyyy年MM月dd日");
            if (_currentState == UIState.View)
            {
                string s = "上期余额";
                if (_filter.d1.Month == 1 && _filter.d1.Day == 1)
                {
                    if (_filter.d1.Year == _e.CurrentAccount.StartYear)
                    {
                        s = "年初余额";
                    }
                    else
                    {
                        s = "上年结转";
                    }
                }
                _currentAccountPage = _dataProvider.GetEntryExList(GetStartBal(_filter.d1), s, filterExpress, "RecordDate,id,entry", ((_filter.d2.Month == 12 && _filter.d2.Day == 31) ? true : false), _filter.d1,checkBox1.Checked );
                EntryExList entryList = _currentAccountPage;
                if (entryList != null)
                {
                    grid.Columns[0].HeaderCell.Value = (_filter.d1.Year + "年;月;日");
                    foreach (EntryEx dr in entryList)
                    {
                        DateTime dt = Core.General.FromString(dr.Date);
                        int i = grid.Rows.Add();
                        if (dr.Side != 0)
                        {
                            grid[0, i].Value = dt.Month;
                            grid[1, i].Value = dt.Day;
                            if (dr.Side == 1)
                            {
                                grid[7, i].Value = dr.Money.ToString("0.00");
                            }
                            else if (dr.Side == -1)
                            {
                                grid[8, i].Value = dr.Money.ToString("0.00");
                            }
                        }
                        else
                        {
                            grid[7, i].Value = dr.JMoney.ToString("0.00");
                            grid[8, i].Value = dr.DMoney.ToString("0.00");
                            grid[6, i].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            grid[6, i].Style.ForeColor = Color.Red;
                        }
                        grid[6, i].Value = dr.Digest;
                        grid[2, i].Value = dr.VchType;
                        grid[3, i].Value = dr.VchNumber;
                        grid[4, i].Value = dr.ChequeType;
                        grid[5, i].Value = dr.Cheque;
                        grid[9, i].Value = dr.Bal.ToString("0.00");
                    }
                }
            }
            else if (_currentState == UIState.ViewTotal)
            {
                GetTotalData();
            }
        }
        void InitViewerColumns()
        {
            dataGridView2.Visible = false;
            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();
            dataGridView2.AllowUserToDeleteRows = false ;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.EditMode = DataGridViewEditMode.EditProgrammatically  ;
            dataGridView2.Enabled = true;
            dataGridView2.TopLeftHeaderCell.Value = "";
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.ColumnHeadersHeight = 60;
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("楷体_GB2312", 12F); 
            int i = 0;
            DataGridViewColumn[] cs;
            if (_currentState == UIState.View)
            {
                #region 明细账列定义
                cs = new DataGridViewColumn[10];
                cs[i] = new DataGridViewTextBoxColumn();
                cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                cs[i].Width = 27;
                cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                cs[i].Resizable = DataGridViewTriState.False;
                cs[i].HeaderText = "月";
                cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
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
                cs[i].HeaderText = "记账凭证;字;号";// "凭证字";
                cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                i++;
                cs[i] = new DataGridViewTextBoxColumn();
                cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                cs[i].Width = 27;
                cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                cs[i].HeaderText = "号";// "凭证号";
                cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                i++;
                cs[i] = new DataGridViewTextBoxColumn();
                cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                cs[i].Width = 40;
                cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                cs[i].HeaderText = "支票;种类;号码";// "支票号";
                cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                i++;
                cs[i] = new DataGridViewTextBoxColumn();
                cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                cs[i].Width = 40;
                cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                cs[i].HeaderText = "号码";//"支票号";
                cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                i++;
                cs[i] = new DataGridViewTextBoxColumn();
                cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                cs[i].HeaderText = "摘  要";
                cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cs[i].ReadOnly = true;
                i++;
                cs[i] = new CyEditorTextBoxColumn();
                cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                cs[i].Width = 160;
                cs[i].CellTemplate.Style.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                cs[i].HeaderText = "借  方";
                cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                cs[i].ReadOnly = true;
                i++;
                cs[i] = new CyEditorTextBoxColumn();
                cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                cs[i].Width = 160;
                cs[i].CellTemplate.Style.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                cs[i].HeaderText = "贷  方";
                cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                cs[i].ReadOnly = true;
                i++;
                cs[i] = new CyEditorTextBoxColumn();
                cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                cs[i].Width = 160;
                cs[i].CellTemplate.Style.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                cs[i].HeaderText = "余  额";
                cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                cs[i].ReadOnly = true;
                #endregion
            }
            else
            {
                #region 总帐列定义
                cs = new DataGridViewColumn[6];
                cs[i] = new DataGridViewTextBoxColumn();
                cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                cs[i].HeaderText = "银行名称";
                cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cs[i].ReadOnly = true;
                i++;
                cs[i] = new CyEditorTextBoxColumn();
                cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                cs[i].Width = 160;
                cs[i].CellTemplate.Style.Font = new System.Drawing.Font("宋体", 9.75F);
                cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                cs[i].HeaderText = "银行账号";
                cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cs[i].ReadOnly = true;
                i++;
                cs[i] = new CyEditorTextBoxColumn();
                cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                cs[i].Width = 160;
                cs[i].CellTemplate.Style.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                cs[i].HeaderText = "昨日余额";
                cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                cs[i].ReadOnly = true;
                i++;
                cs[i] = new CyEditorTextBoxColumn();
                cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                cs[i].Width = 160;
                cs[i].CellTemplate.Style.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                cs[i].HeaderText = "借  方";
                cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                cs[i].ReadOnly = true;
                i++;
                cs[i] = new CyEditorTextBoxColumn();
                cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                cs[i].Width = 160;
                cs[i].CellTemplate.Style.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                cs[i].HeaderText = "贷  方";
                cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                cs[i].ReadOnly = true;
                i++;
                cs[i] = new CyEditorTextBoxColumn();
                cs[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                cs[i].Width = 160;
                cs[i].CellTemplate.Style.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                cs[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                cs[i].HeaderText = "今日余额";
                cs[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cs[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                cs[i].ReadOnly = true;
                #endregion
            }
            dataGridView2.Columns.AddRange(cs);
            dataGridView2.Visible = true;
        }

        #endregion


        /// <summary> 查看账户总账
        /// 
        /// </summary>
        void GetTotalData()
        {
                        DataGridView grid = dataGridView2;

            ItemOfBank item = GetCurrentItemOfBank();
            ItemOfBankCollection items=new ItemOfBankCollection ();
            if (item != null)
            {
                items.Add(item);
            }
            else
            {
                items = _dataProvider.GetItemOfBankList();
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("RecordDate>='");
            sb.Append(Core.General.FromDateTime(_filter.d1));
            sb.Append("' AND RecordDate<='");
            sb.Append(Core.General.FromDateTime(_filter.d2));
            sb.Append("'");
            string s = sb.ToString();
            double startBals = 0;
            double jMonerys = 0;
            double dMonerys = 0;
            int i = 0;
            foreach (ItemOfBank it in items)
            {
                StringBuilder sb1 = new StringBuilder(s);
                sb1.Append ( " AND id='");
                sb1.Append ( it.ID);
                sb1.Append("'");
                string filterExpress = sb1.ToString();
                double startBal = _dataProvider.GetStartBal(_filter.d1 , it);

                EntryExList entryList = _dataProvider.GetEntryList(filterExpress, "RecordDate,entry");
                i = grid.Rows.Add();
                grid[0, i].Value = it.OfBankName ;
                grid[1, i].Value = it.ID;
                grid[2, i].Value = startBal.ToString("0.00");
                double jMonery = 0;
                double dMonery = 0;
                if (entryList != null)
                {
                    foreach (Entry dr in entryList)
                    {
                        if (dr.Side == 1)
                        {
                            jMonery += dr.Money;
                        }
                        else if (dr.Side == -1)
                        {
                            dMonery += dr.Money;
                        }
                    }
                }
                grid[3, i].Value = jMonery.ToString("0.00");
                grid[4, i].Value = dMonery.ToString("0.00");
                grid[5, i].Value = (startBal+jMonery-dMonery).ToString("0.00");
                startBals += startBal;
                jMonerys += jMonery;
                dMonerys += dMonery;
            }
            i = grid.Rows.Add();
            grid[0, i].Value = "";
            grid[1, i].Value = "合  计";
            grid[1, i].Style.ForeColor = Color.Red;
            grid[1, i].Style.Alignment = DataGridViewContentAlignment.MiddleCenter; 
            grid[2, i].Value = startBals.ToString("0.00");
            grid[3, i].Value = jMonerys.ToString("0.00");
            grid[4, i].Value = dMonerys.ToString("0.00");
            grid[5, i].Value = (startBals + jMonerys - dMonerys).ToString("0.00");
        }

        /// <summary>  不包含d日,即小于d
        /// </summary>
        /// <param name="d"></param>
        double GetStartBal(DateTime d)
        {
            ItemOfBank item = GetCurrentItemOfBank();
            return _dataProvider.GetStartBal(d, item); 
        }

        double GetStartBal()
        {
            return GetStartBal(_defaultDate);
        }

        Core.Printer.PrintAssign InitPrintAssign()
        {
            this.Cursor = Cursors.WaitCursor;
            Core.Printer.PrintAssign printer = _e.InitPrintAssign();
            printer.SavePageSetup += new EventHandler(printer_SavePageSetup);
            this.Cursor = Cursors.Default;
            return printer;
        }

        void printerDailyReport_SavePageSetup(object sender, EventArgs e)
        {
            string filename = Application.CommonAppDataPath + @"\银行日报表打印设置" + ".PageSet";
            Core.XmlSerializer<Core.Printer.PageSetup>.ToXmlSerializer(filename, (sender as Core.Printer.PrintAssign).PageSetup);
        }

        void printer_SavePageSetup(object sender, EventArgs e)
        {
            string filename = _e.PageSetupFilename;
            Core.XmlSerializer<Core.Printer.PageSetup>.ToXmlSerializer(filename, (sender as Core.Printer.PrintAssign).PageSetup);
        }

        string GetFilterExpress()
        {
            ItemOfBank item = GetCurrentItemOfBank();
            string filterExpress = "RecordDate>='" + Core.General.FromDateTime(_filter.d1) + "' AND RecordDate<='" + Core.General.FromDateTime(_filter.d2) + "'";
            if (item != null)
            {
                filterExpress = "id='" + item.ID + "' AND " + filterExpress;
            }
            return filterExpress;
        }

        void GetAccountPageHeader(Core.Printer.PrintData printData,int Year,ItemOfBank itemOfBank)
        {
            Core.Printer.PrintData p = printData;
            Core.Printer.AttachRow ar = new UnvaryingSagacity.Core.Printer.AttachRow();
            Core.Printer.PrinterAttachCell aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.InRowPercent = 33;
            aCell.Properties.Value = ""; 
            ar.Add(aCell, "main1");
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle ;
            aCell.InRowPercent = 33;
            aCell.Properties.Value = "银 行 存 款 日 记 账";
            aCell.Properties.FontColor = Color.Green.ToArgb();  
            ar.Add(aCell, "main2");
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightTop;
            aCell.InRowPercent = 33;
            aCell.Properties.Value = "第 &P 页";
            aCell.Properties.font = new Font("宋体", 12, FontStyle.Bold);
            aCell.Properties.FontColor = Color.Green.ToArgb();
            ar.Add(aCell, "main3");
            p.MainTitle.Add(ar, "main");

            ar = new UnvaryingSagacity.Core.Printer.AttachRow();
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            aCell.InRowPercent = 25;
            aCell.Properties.Value ="";
            ar.Add(aCell, "subTitle1");
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            aCell.InRowPercent = 50;
            aCell.Properties.Value = _e.CurrentAccount.Name;
            ar.Add(aCell, "subTitle2");
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            aCell.InRowPercent = 25;
            aCell.Properties.Value ="";
            ar.Add(aCell, "subTitle3");
            ar.Offset_V = 10;
            p.SubTitles.Add(ar, "subTitle");
            #region SheetHeader

            ar = new UnvaryingSagacity.Core.Printer.AttachRow();
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            //aCell.Properties.Value = label7.Text;
            aCell.Properties.AddTextEx(Color.Green.ToArgb(), "开户银行: ");
            aCell.Properties.AddTextEx(Color.Black.ToArgb(), (itemOfBank == null ? "" : itemOfBank.OfBankName));
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.LeftMiddle;
            aCell.InRowPercent = 100;
            ar.Add(aCell, "left1");
            ar.Offset_V = 15;
            p.Headers.Add(ar, "h1");

            ar = new UnvaryingSagacity.Core.Printer.AttachRow();
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            //aCell.Properties.Value = label4.Text; 
            aCell.Properties.AddTextEx(Color.Green.ToArgb(), "账    号: ");
            aCell.Properties.AddTextEx(Color.Black.ToArgb(), (itemOfBank == null ? "" : itemOfBank.ID));
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.LeftMiddle;
            aCell.InRowPercent = 100;
            ar.Add(aCell, "left");
            ar.Offset_V = 5;
            p.Headers.Add(ar, "h2");
            #endregion
            Core.Printer.Body body = p.Body;
            body.Offset_V = 8;
            #region SheetTitle

            p.Cols.Add( new UnvaryingSagacity.Core.Printer.Col(22));
            p.Cols.Add( new UnvaryingSagacity.Core.Printer.Col(22));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(27));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(27));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(27));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(27));
            p.Cols.Add( new UnvaryingSagacity.Core.Printer.Col(200));
            p.Cols.Add( new UnvaryingSagacity.Core.Printer.Col(160));
            p.Cols.Add( new UnvaryingSagacity.Core.Printer.Col(160));
            p.Cols.Add( new UnvaryingSagacity.Core.Printer.Col(22));
            p.Cols.Add( new UnvaryingSagacity.Core.Printer.Col(160));
            Core.Printer.PrinterCell cell;
            int r = 1;
            #region 第一行
            p.Rows.Add(new UnvaryingSagacity.Core.Printer.Row(30));
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell,Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value =Year +"年";
            body.Add("$" + r + "$1", cell);
            
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell,Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = Year + "年";
            body.Add("$" + r + "$2", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "记账凭证";
            cell.WordWrap = true;
            body.Add("$" + r + "$3", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "";
            body.Add("$" + r + "$4", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell,Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "支票";
            body.Add("$" + r + "$5", cell);
            
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "支票";
            body.Add("$" + r + "$6", cell);
            
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "摘  要";
            body.Add("$" + r + "$7", cell);
            
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell,Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "借  方";
            body.Add("$" + r + "$8", cell);
            
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell,Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "贷  方";
            body.Add("$" + r + "$9", cell);
            
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell,Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = Core.Printer.PrintAssign.OK_FLAG;
            body.Add("$" + r + "$10", cell);
            
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell,Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "余  额";
            body.Add("$" + r + "$11", cell);
            r++;
#endregion
            #region 第二行
            p.Rows.Add(new UnvaryingSagacity.Core.Printer.Row(30));
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            
            SetPrintCellFullBorder(ref cell,Color.Green);
            
            
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value =  "月";
            body.Add("$" + r + "$1", cell);
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            
            SetPrintCellFullBorder(ref cell,Color.Green);
            
            
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "日";
            body.Add("$" + r + "$2", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "字";
            body.Add("$" + r + "$3", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "号";
            body.Add("$" + r + "$4", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "种类";
            body.Add("$" + r + "$5", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "号码";
            body.Add("$" + r + "$6", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "摘  要";
            body.Add("$" + r + "$7", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "借  方";
            cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线标题; 
            body.Add("$" + r + "$8", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "贷  方";
            cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线标题; 
            body.Add("$" + r + "$9", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = Core.Printer.PrintAssign.OK_FLAG;
            body.Add("$" + r + "$10", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "余  额";
            cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线标题;  
            body.Add("$" + r + "$11", cell);
            r++;
            #endregion
            p.Mergers.Add(new UnvaryingSagacity.Core.Printer.Range(1,1,1,2 ));
            p.Mergers.Add(new UnvaryingSagacity.Core.Printer.Range(1,3, 1, 4));
            p.Mergers.Add(new UnvaryingSagacity.Core.Printer.Range(1, 5, 1, 6));
            p.Mergers.Add(new UnvaryingSagacity.Core.Printer.Range(1, 7, 2, 7));
            p.Mergers.Add(new UnvaryingSagacity.Core.Printer.Range(1,10, 2, 10));
            p.TopFixedRows = 2;
            #endregion
        }

        Core.Printer.PrintData GetPrintDataByGrid()
        {
            Core.Printer.PrintData p = new UnvaryingSagacity.Core.Printer.PrintData();
            Core.Printer.AttachRow ar = new UnvaryingSagacity.Core.Printer.AttachRow();
            Core.Printer.PrinterAttachCell aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.InRowPercent = 33;
            aCell.Properties.Value = "";
            ar.Add(aCell, "main1");
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            aCell.InRowPercent = 33;
            aCell.Properties.Value = "银 行 存 款 日 报 表";
            //aCell.Properties.FontColor = Color.Green.ToArgb();
            ar.Add(aCell, "main2");
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightTop;
            aCell.InRowPercent = 33;
            aCell.Properties.Value = "";
            ar.Add(aCell, "main3");
            p.MainTitle.Add(ar, "main");

            ar = new UnvaryingSagacity.Core.Printer.AttachRow();
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            aCell.InRowPercent = 25;
            aCell.Properties.Value = "";
            ar.Add(aCell, "subTitle1");
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            aCell.InRowPercent = 50;
            aCell.Properties.Value = _e.CurrentAccount.Name;
            ar.Add(aCell, "subTitle2");
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            aCell.InRowPercent = 25;
            aCell.Properties.Value = "";
            ar.Add(aCell, "subTitle3");
            ar.Offset_V = 10;
            p.SubTitles.Add(ar, "subTitle");
            #region SheetHeader

            ar = new UnvaryingSagacity.Core.Printer.AttachRow();
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            //aCell.Properties.Value = label7.Text;
            aCell.Properties.AddTextEx(Color.Green.ToArgb(), "开始日期: ");
            aCell.Properties.AddTextEx(Color.Black.ToArgb(), _filter.d1.ToLongDateString());
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.LeftMiddle;
            aCell.InRowPercent = 100;
            ar.Add(aCell, "left1");
            ar.Offset_V = 15;
            p.Headers.Add(ar, "h1");

            ar = new UnvaryingSagacity.Core.Printer.AttachRow();
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            //aCell.Properties.Value = label4.Text; 
            aCell.Properties.AddTextEx(Color.Green.ToArgb(), "截止日期: ");
            aCell.Properties.AddTextEx(Color.Black.ToArgb(), _filter.d2.ToLongDateString());
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.LeftMiddle;
            aCell.InRowPercent = 100;
            ar.Add(aCell, "left");
            ar.Offset_V = 5;
            p.Headers.Add(ar, "h2");
            #endregion
            Core.Printer.Body body = p.Body;
            body.Offset_V = 8;
            #region SheetTitle

            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(200));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(200));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(160));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(160));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(160));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(160));
            Core.Printer.PrinterCell cell;
            int r = 1;
            #region 第一行
            p.Rows.Add(new UnvaryingSagacity.Core.Printer.Row(30));
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value =  "开户银行";
            body.Add("$" + r + "$1", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "银行账号";
            body.Add("$" + r + "$2", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "昨日余额";
            body.Add("$" + r + "$3", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "借  方";
            body.Add("$" + r + "$4", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "贷  方";
            body.Add("$" + r + "$5", cell);

            
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "今日余额";
            body.Add("$" + r + "$6", cell);
            r++;
            #endregion
            #region 第二行
            p.Rows.Add(new UnvaryingSagacity.Core.Printer.Row(30));
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "";
            body.Add("$" + r + "$1", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "";
            body.Add("$" + r + "$2", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "昨日余额";
            cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线标题;
            body.Add("$" + r + "$3", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "借  方";
            cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线标题;
            body.Add("$" + r + "$4", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "贷  方";
            cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线标题;
            body.Add("$" + r + "$5", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "今日余额";
            cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线标题;
            body.Add("$" + r + "$6", cell);
            r++;
            #endregion
            p.Mergers.Add(new UnvaryingSagacity.Core.Printer.Range(1, 1, 2, 1));
            p.Mergers.Add(new UnvaryingSagacity.Core.Printer.Range(1, 2, 2, 2));
            p.TopFixedRows = 2;
            #endregion

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                p.Rows.Add(new UnvaryingSagacity.Core.Printer.Row(30));
                for (int c = 0; c < row.Cells.Count; c++)
                {
                    cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                    SetPrintCellFullBorder(ref cell, Color.Green);
                    cell.Value = row.Cells[c].EditedFormattedValue;
                    if (c > 1)
                    {
                        cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                        cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线;
                    }
                    else
                    {
                        if (((r-p.TopFixedRows ) == dataGridView2.RowCount) && c == 1)
                        {
                            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                            cell.FontColor = Color.Red.ToArgb();
                        }
                        else
                        {
                            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.LeftMiddle;
                        }
                    }
                    body.Add("$" + r + "$" + (c + 1).ToString(), cell);
                }
                r++;
            }
            return p;
        }

        Core.Printer.PrintData GetPrintDataByEntryExList(EntryExList entries, ItemOfBank item)
        {
            this.Cursor = Cursors.WaitCursor; 
            Core.Printer.PrintData printData=new UnvaryingSagacity.Core.Printer.PrintData ();
            GetAccountPageHeader(printData, _filter.d1.Year, item);
            int r = 3;
            int c = 1;
            Core.Printer.Body body = printData.Body;
            foreach (EntryEx entry in entries)
            {
                printData.Rows.Add(new UnvaryingSagacity.Core.Printer.Row(25));
                Core.Printer.PrinterCell cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                DateTime dt=Core.General.FromString (entry.Date );
                SetPrintCellFullBorder(ref cell,Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                cell.Value =entry.Side ==0?"": dt.Month.ToString ();
                body.Add("$" + r + "$" + c,cell );
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell,Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                cell.Value = entry.Side == 0 ? "" : dt.Day.ToString();
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                cell.Value = entry.Side == 0 ? "" : entry.VchType;   
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                cell.Value = entry.Side == 0 ? "" : entry.VchNumber; 
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                cell.Value = entry.Side == 0 ? "" : entry.ChequeType;
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                cell.Value = entry.Side == 0 ? "" : entry.Cheque;
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell,Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.LeftMiddle;
                cell.Value = entry.Digest;
                if (entry.Side == 0)
                {
                    cell.FontColor = Color.Red.ToArgb();
                    cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                }
                body.Add("$" + r + "$" + c, cell);
                c++;
                if (entry.Side == 0)
                {
                    cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                    SetPrintCellFullBorder(ref cell,Color.Green);
                    cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle; 
                    cell.Value = entry.JMoney;
                    cell.L_Color = Color.Green.ToArgb();
                    cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线;
                    body.Add("$" + r + "$" + c, cell);
                    c++;
                    cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                    SetPrintCellFullBorder(ref cell,Color.Green);
                    cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                    cell.Value = entry.DMoney;
                    cell.L_Color = Color.Green.ToArgb();
                    cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线;
                    body.Add("$" + r + "$" + c, cell);
                    c++;
                }
                else
                {
                    cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                    SetPrintCellFullBorder(ref cell,Color.Green);
                    cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                    cell.Value = entry.Side == 1 ? entry.Money : 0;
                    cell.L_Color = Color.Green.ToArgb();
                    cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线;
                    body.Add("$" + r + "$" + c, cell);
                    c++;
                    cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                    SetPrintCellFullBorder(ref cell,Color.Green);
                    cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                    cell.Value = entry.Side == -1 ? entry.Money : 0;
                    cell.L_Color = Color.Green.ToArgb();
                    cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线;
                    body.Add("$" + r + "$" + c, cell);
                    c++;
                }
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell,Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle; 
                cell.Value = "";
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell,Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle; 
                cell.Value = entry.Bal;
                cell.L_Color = Color.Green.ToArgb();
                cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线; 
                body.Add("$" + r + "$" + c, cell);
                c=1;
                r++;
            }
            this.Cursor = Cursors.Default;
            return printData;
        }

        void SetPrintCellFullBorder(ref Core.Printer.PrinterCell cell)
        {
            SetPrintCellFullBorder(ref cell, Color.Black);
        }

        void SetPrintCellFullBorder(ref Core.Printer.PrinterCell cell,Color color)
        {
            cell.B_Style = UnvaryingSagacity.Core.Printer.PrinterBorderStyle.实线边框;
            cell.T_Style = UnvaryingSagacity.Core.Printer.PrinterBorderStyle.实线边框;
            cell.L_Style = UnvaryingSagacity.Core.Printer.PrinterBorderStyle.实线边框;
            cell.R_Style = UnvaryingSagacity.Core.Printer.PrinterBorderStyle.实线边框;
            cell.B_Color = color.ToArgb();
            cell.T_Color = color.ToArgb();
            cell.L_Color = color.ToArgb();
            cell.R_Color = color.ToArgb();
        }

        void PrintDailyReport(bool isPreview)
        {
            if (IsOpenAccount(true))
            {
                this.Cursor = Cursors.WaitCursor;
                Core.Printer.PrintAssign printer = _e.InitPrintAssignByDailyReport();
                printer.SavePageSetup += new EventHandler(printerDailyReport_SavePageSetup);
                this.Cursor = Cursors.Default;

                Core.Printer.PrintData printData = GetPrintDataByGrid();
                printer.PrintDatas.Add(printData);
                if (isPreview)
                {
                    printer.ShowPreviewDialog(this);
                }
                else
                {
                    printer.Print();
                }
            }
        }

        void PrintAccountPage(bool isPreview)
        {
            if(IsOpenAccount (true ))
            {
                ItemOfBank item = GetCurrentItemOfBank();
                if (item == null)
                {
                    MessageBox.Show(this, "没有要打印的账号, 请先选择具体的账号", "打印账页", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (_currentAccountPage == null)
                {
                    MessageBox.Show(this, "没有要打印的账页, 请先选择查看日记账", "打印账页", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Core.Printer.PrintAssign printer = InitPrintAssign();
                Core.Printer.PrintData printData = GetPrintDataByEntryExList(_currentAccountPage, item);
                printer.PrintDatas.Add(printData);
                if (isPreview)
                {
                    printer.ShowPreviewDialog(this);
                }
                else
                {
                    printer.Print();
                }
            }
        }

        void PrintAccountBook()
        {
            if (IsOpenAccount(true))
            {
                UIAccountBookPrintSetting ui = new UIAccountBookPrintSetting();
                ui.CurrentEnvironment = _e;
                ui.ShowDialog(this);
            }
        }

        void PrintSetup()
        {
            Core.Printer.PrintAssign printAssign = InitPrintAssign(); 
            printAssign.ShowPageSetupDialog(this);
        }

        void AccountCoverDesigner()
        {
            UIDesigner ui = new UIDesigner();
            ui.CurrentEnvironment = _e;
            ui.ShowDialog(this);
        }
    }

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
