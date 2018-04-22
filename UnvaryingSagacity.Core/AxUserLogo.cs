using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D ;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace UnvaryingSagacity.Core
{
    public enum UserLogoImageSize
    {
        Size16=16,
        Size32=32,
        Size64=64,
        Size96=96,
        Size128=128,
        Size256=256,

    }

    public class AxUserLogo:UserControl 
    {
        private Image _logo;
        private string _text;
        UserLogoImageSize _imageSize=UserLogoImageSize.Size128  ;

        private bool mouseIn = false;

        public AxUserLogo()
        {
            _imageSize = UserLogoImageSize.Size128;
            SenderMode = 0;
            InitializeComponent();
        }

        public AxUserLogo(UserLogoImageSize imageSize)
        {
            _imageSize = imageSize;
            SenderMode = 0;
            InitializeComponent();
        }

        public UserLogoImageSize ImageSize { get { return _imageSize; } }

        public new string Text { get { return _text; } set { _text =value ;} }

        /// <summary>
        /// image size=128,128
        /// </summary>
        public Image Logo { get { return _logo; } set { _logo = value; } }
        
        /// <summary>
        /// ＝0正常显示图像，即鼠标移入正常显示，移除则变黑显示；＝1时相反
        /// </summary>
        public int SenderMode { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                if (_logo == null)
                {
                    _logo = global::UnvaryingSagacity.Core.Properties.Resources.defultLogo;
                }
                if (mouseIn)
                {
                    if (SenderMode == 0)
                    {
                        e.Graphics.DrawImage(_logo, new Rectangle(0, 0, (int)_imageSize - 1, (int)_imageSize - 1));
                    }
                    else
                    {
                        ImageHandler.DrawImageDark(e.Graphics, _logo, new Rectangle(0, 0, (int)_imageSize - 1, (int)_imageSize - 1));
                    }
                }
                else
                {
                    if (SenderMode == 0)
                    {
                        ImageHandler.DrawImageDark(e.Graphics, _logo, new Rectangle(0, 0, (int)_imageSize - 1, (int)_imageSize - 1));
                    }
                    else
                    {
                        e.Graphics.DrawImage(_logo, new Rectangle(0, 0, (int)_imageSize - 1, (int)_imageSize - 1));
                    }
                }
                SizeF sizef = e.Graphics.MeasureString(_text, this.Font);
                float left = (this.Width - sizef.Width) / 2;
                if (_text.Length > 0)
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    e.Graphics.DrawString(_text, this.Font, new SolidBrush(this.ForeColor), new RectangleF(new PointF(left, (float)_imageSize - 1), sizef), sf);
                }
                base.OnPaint(e);
            }
            catch { base.OnPaint(e); }
            
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            mouseIn = true;
            this.Cursor = Cursors.Hand;
            this.Refresh();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            mouseIn = false;
            this.Cursor = Cursors.Default;
            this.Refresh();
            base.OnMouseLeave(e);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AxUserLogo
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "AxUserLogo";
            this.Size = new System.Drawing.Size((int)_imageSize, (int)_imageSize + 13);
            this.ResumeLayout(false);
        }
    }
}
