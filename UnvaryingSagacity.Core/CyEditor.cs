using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.Core
{
    public partial class CyEditor : UserControl
    {
        public const string CYTITLE = "千百十万千百十亿千百十万千百十亿千百十万千百十亿千百十万千百十元角分";
        public const int CURSOR_WIDTH = 5;
        public const int CURSOR_HEIGHT = 3;
        public const int WORD_SPACE = 4;
        public const int DOUBLELINES_H = 3;

        Pen pen = new Pen(Color.Green);
        Pen pen2 = new Pen(Color.Green, 2F);
        Pen penRed = new Pen(Color.Red);
        Brush _bBlack = new SolidBrush(Color.Black);
        Brush _bRed = new SolidBrush(Color.Red);
        StringBuilder sb = new StringBuilder();
        string[] _decimals = new string[2]{"0","0"};
        RectangleF[] rtfs = new RectangleF[0];
        StringFormat _sf = new StringFormat();
        RectangleF rtCyTitle = new RectangleF();
        RectangleF rtCyText = new RectangleF();
        PointF ptCyCursor = new PointF(0, 36);
        int _charWidth = 12;//默认的字符宽度
        int _charFullWidth = 12 + WORD_SPACE / 2;//默认的字符宽度,包含两边的空
        int _index = 2;//当前光标的文本位置,缺省在金额元
        bool _autoDrawCursor = true ;
        bool _isSub = false;
        public Font CyTitleFont { get; set; }
        public Font CyTextFont { get; set; }
        public DisplayMode Mode { get; set; }
        public bool ReadOnly { get; set; }
        public bool DisplayZero { get; set; }

        public override string Text
        {
            get
            {
                string s =( _isSub ? "-" : "") + sb.ToString() + "." + _decimals[1] + _decimals[0];
                try
                {
                    double d = double.Parse(s);
                    if (d == 0d && DisplayZero)
                    {
                        s = "";
                    }
                    else
                    {
                        s = d.ToString("0.00");
                    }
                    return s;

                }
                catch { return s; }
            }
            set
            {
                sb.Remove (0,sb.Length );
                _isSub = false;
                _decimals[0] = _decimals[1] = "0";
                double f=0F;
                try
                {
                    f = double.Parse(value);
                }
                catch { return; }
                _isSub = f < 0;
                if (f != 0)
                {
                    f = Math.Abs(Math.Round (f,2));
                    string[] ss = f.ToString("0.00").Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    sb.Append(ss[0]);
                    _decimals[1] = ss[1][0].ToString();
                    _decimals[0] = ss[1][1].ToString();
                }
                this.Invalidate(new Rectangle(new Point(0, 0), this.ClientSize));
            }
        }
        /// <summary>
        /// 未实现
        /// </summary>
        public int Decimals { get; set; }
        public CyEditor()
        {
            InitializeComponent();
            Decimals = 2;
            _sf.Alignment = StringAlignment.Center;
            _sf.LineAlignment = StringAlignment.Center;
            this.KeyPress += new KeyPressEventHandler(CyEditor_KeyPress);
            this.KeyUp += new KeyEventHandler(CyEditor_KeyUp);
            this.Paint += new PaintEventHandler(CyEditor_Paint);
            this.timer1.Tick += new EventHandler(timer1_Tick);
            this.VisibleChanged += new EventHandler(CyEditor_VisibleChanged);
            this.GotFocus += new EventHandler(CyEditor_GotFocus);
            this.LostFocus += new EventHandler(CyEditor_LostFocus);
            this.SizeChanged += new EventHandler(CyEditor_SizeChanged);
            this.MouseClick += new MouseEventHandler(CyEditor_MouseClick);
            this.FontChanged += new EventHandler(CyEditor_FontChanged);
            CyTitleFont = new Font("楷体_GB2312", 9F);
            CyTextFont = this.Font;// new Font("宋体", 9.75F, FontStyle.Bold);
            DisplayZero = false;
            SetDefaultRectangleF();
              
        }


        void CyEditor_FontChanged(object sender, EventArgs e)
        {
            CyTextFont = this.Font;
            this.Invalidate(Rectangle.Ceiling(rtCyText), false);
        }

        void CyEditor_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.Focused)
            {
                Console.WriteLine("CyEditor_MouseClick"); 

                GetIndexAtPoint(e.X, e.Y);
                ClearCursor();
            }
        }

        void CyEditor_SizeChanged(object sender, EventArgs e)
        {
            SetDefaultRectangleF();
            this.Refresh();
        }

        void CyEditor_LostFocus(object sender, EventArgs e)
        {
            _index = 2;
            ClearCursor();
        }

        void CyEditor_GotFocus(object sender, EventArgs e)
        {
            _autoDrawCursor = true;
            DrawCursor(Graphics.FromHwnd(this.Handle));
        }

        void CyEditor_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                _charWidth = TextRenderer.MeasureText("0123456789", CyTextFont).Width / 10+2;
                _charFullWidth = _charWidth + WORD_SPACE / 2;
                SetDefaultRectangleF();
            }
        }

        void CyEditor_Paint(object sender, PaintEventArgs e)
        {
            if (Mode != DisplayMode.金额)
                DrawCyTitle(e.Graphics);
            DrawTextContent(e.Graphics);
            DrawCursor(e.Graphics);
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            _autoDrawCursor = !_autoDrawCursor;
            this.Invalidate(Rectangle.Ceiling(new RectangleF(0, ptCyCursor.Y, this.Width, CURSOR_HEIGHT)), false);
        }

        void CyEditor_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete :
                    if (sb.Length > 0 && _index > 1 && (_index - 2) < sb.Length)
                    {
                        sb.Remove(sb.Length - (_index - 2 + 1), 1);
                        this.Invalidate(Rectangle.Ceiling(rtCyText), false);
                    }
                    break;
                case Keys.Right :
                    MoveCursor(-1);
                    break;
                case Keys.Left :
                    MoveCursor(1); 
                    break;
                default :
                    break;
            }
        }

        void CyEditor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (ReadOnly)
                return;
            if (e.KeyChar == (char)8)
            {
                if (sb.Length > 0 && _index >1 && (_index-1) <sb.Length )
                {
                    sb.Remove(sb.Length - _index, 1);
                }
            }
            else if (e.KeyChar == '-')
                _isSub = !_isSub;
            else if (Char.IsDigit(e.KeyChar) || e.KeyChar == '.')
            {
                if (e.KeyChar == '.')
                {
                    _index = 1;
                    _autoDrawCursor = true;
                    ClearCursor();
                    return;
                }
                else
                {
                    if (_index > 1)
                    {
                        if (Control.IsKeyLocked(Keys.Insert))
                        {
                            if ((_index - 2) >= sb.Length)
                            {
                                //第一个数字不允许为0
                                if (e.KeyChar != '0')
                                {
                                    sb.Insert(0, e.KeyChar);
                                }
                            }
                            else
                            {
                                if ((_index - 1) >= sb.Length)
                                {
                                    if (e.KeyChar != '0')
                                        sb.Replace(sb[sb.Length - (_index - 2 + 1)], e.KeyChar, sb.Length - (_index - 2 + 1), 1);
                                }
                                else
                                    sb.Replace(sb[sb.Length - (_index - 2 + 1)], e.KeyChar, sb.Length - (_index - 2 + 1), 1);

                            }
                        }
                        else
                        {
                            if ((_index - 2) >= sb.Length)
                            {
                                //第一个数字不允许为0
                                if (e.KeyChar != '0')
                                {
                                    sb.Insert(0, e.KeyChar);
                                }
                            }
                            else if (_index > 2)
                                sb.Insert(sb.Length - (_index - 2), e.KeyChar);

                            else
                                sb.Append(e.KeyChar);
                        }
                    }
                    else
                    {
                        _decimals[_index] = e.KeyChar.ToString();
                        if (_index == 1)
                            _index = 0;
                    }
                }
            }
            else
            {
                return;
            }
            this.Invalidate(Rectangle.Ceiling(rtCyText), false);
            OnTextChanged(new EventArgs());
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
        }

        void MoveCursor(int offset)
        {
            _index += offset;
            if (_index > (sb.Length + 2))
                _index = sb.Length + 2;
            if (_index < 0)
                _index = 0;
            _autoDrawCursor = true;
            ClearCursor();
        }

        void GetCursorPoint()
        {
            if (Mode == DisplayMode.金额)
                ptCyCursor.X = rtCyText.Right - (_index + 1) * _charFullWidth + (_charFullWidth - CURSOR_WIDTH ) / 2;
            else
                ptCyCursor.X = rtCyTitle.Right - (_index + 1) * _charFullWidth + (_charFullWidth - CURSOR_WIDTH) / 2;
        }

        void GetIndexAtPoint(int x, int y)
        {
            for (int i = 0; i < rtfs.Length; i++)
            {
                if (rtfs[i].Contains(new PointF(x, y)))
                {
                    _index = i;
                    if (_index > (sb.Length + 2))
                        _index = sb.Length + 2;
                    return;
                }
            }

        }

        void ClearCursor()
        {
            timer1.Enabled = false;
            this.Invalidate(Rectangle.Ceiling(new RectangleF(0, ptCyCursor.Y, this.Width, CURSOR_HEIGHT)), false);
        }

        void SetDefaultRectangleF()
        {
            float x;
            if (Mode == DisplayMode.全部)
            {
                rtCyTitle = new RectangleF(0, 0, this.Width - DOUBLELINES_H, 18);
                rtCyText = new RectangleF(0, rtCyTitle.Height, this.Width - DOUBLELINES_H, this.ClientSize.Height - rtCyTitle.Height);
                ptCyCursor = new Point(0, (int)rtCyText.Bottom - 4);
                x = rtCyTitle.Right;

            }
            else if (Mode == DisplayMode.金额)
            {
                rtCyTitle = new RectangleF();
                rtCyText = new RectangleF(0, 0, this.Width - DOUBLELINES_H, this.ClientSize.Height);
                ptCyCursor  = new  Point (0, (int)rtCyText.Bottom - 4);
                x = rtCyText.Right;
            }
            else
            {
                rtCyTitle = new RectangleF(0, 0, this.Width - DOUBLELINES_H, this.ClientSize.Height);
                rtCyText = new RectangleF();
                ptCyCursor = new Point();
                x = rtCyTitle.Right;
            }
            rtfs = new RectangleF[0];
            while (x>=(_charFullWidth ))
            {
                Array.Resize<RectangleF>(ref rtfs, rtfs.Length + 1);
                rtfs[rtfs.Length - 1] = new RectangleF(x - _charFullWidth, 0, _charFullWidth, this.Height);
                x -= _charFullWidth;
            }
        }

        void DrawCursor()
        {
            DrawCursor(Graphics.FromHwnd(this.Handle));
        }

        void DrawCursor(Graphics g)
        {
            if (this.Focused)
            {
                if(_autoDrawCursor)
                {
                    GetCursorPoint(); 
                    g.DrawImageUnscaled(Properties.Resources.arrow_1, Point.Ceiling(ptCyCursor));
                }
                if (!timer1.Enabled)
                    timer1.Enabled = true;
            }
        }

        void DrawCyTitle(Graphics g)
        {
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            _sf.Alignment = StringAlignment.Center;
            RectangleF rtf = new RectangleF(rtCyTitle.Right - _charWidth, rtCyTitle.Y, _charWidth, rtCyTitle.Height);
            int maxLength = rtfs.Length > CYTITLE.Length ? CYTITLE.Length : rtfs.Length;//用小的
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
            if (Mode == DisplayMode.全部)
                g.DrawLine(Pens.Green, rtCyTitle.Left + 1, rtCyTitle.Bottom, rtCyTitle.Right - 1, rtCyTitle.Bottom);
            DrawCyLine(g, rtCyTitle.X, rtCyTitle.Y, rtCyTitle.Width, rtCyTitle.Height, _charFullWidth);
        }

        void DrawTextContent(Graphics g)
        {
            string ss = (sb.Length + 2) > rtfs.Length ? new string('#', rtfs.Length) : sb.ToString() + _decimals[1] + _decimals[0];
            if (ss.Length > 0)
            {
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault ;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default ;
                //这里和标题不一样
                RectangleF rtf = new RectangleF(rtCyText.Right  - _charFullWidth , rtCyText.Y, _charWidth, rtCyText.Height);
                Brush b;
                if (_isSub)
                    b = _bRed;
                else
                    b = _bBlack;
                for (int i = ss.Length - 1; i >= 0; i--)
                {
                    string s = ss.Substring(i, 1);
                    g.DrawString(s, CyTextFont, b, rtf, _sf);
                    rtf.Offset(-_charFullWidth, 0);
                }
            }
            DrawCyLine(g, rtCyText.X, rtCyText.Y, rtCyText.Width, rtCyText.Height, _charFullWidth); 
        }


        private void DrawCyLine(Graphics g, float X, float Y, float Width, float Height, float SpacingWidth)
        {
            float cX;
            int i, offset1 = 0, offset2 = 0;

            //两边画双线
            g.DrawLine(pen, X + DOUBLELINES_H / 2, Y + offset1, X + DOUBLELINES_H / 2, Y + Height - offset2);
            g.DrawLine(pen, X + Width - DOUBLELINES_H / 2, Y + offset1, X + Width - DOUBLELINES_H / 2, Y + Height - offset2);
            //最右边的列为余下的空
            cX = X + Width  - SpacingWidth;
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

    public enum DisplayMode
    {
        全部,
        金额标题,
        金额,

    }
}
