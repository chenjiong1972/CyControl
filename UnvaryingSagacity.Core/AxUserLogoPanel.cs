using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.Core
{

    public delegate void UserLogoMouseClickEventHandle(object sender, MouseEventArgs e);

    public partial class AxUserLogoPanel : UserControl
    {
        UserAndRight.User[] _users;
        AxUserLogo[] _axUser;
        int leftIndex = 0;
        int visibleCount = 0;
        UserLogoImageSize _imageSize = UserLogoImageSize.Size128;
 
        public event UserLogoMouseClickEventHandle UserLogoMouseClick;

        /// <summary>
        /// ＝0正常显示图像，即鼠标移入正常显示，移除则变黑显示；＝1时相反
        /// </summary>
        public int SenderMode { get; set; }

        public AxUserLogoPanel()
        {
            InitializeComponent();
            this.ResizeRedraw = true;
            SenderMode = 0;
            _users = new UserAndRight.User[0];
            _axUser = new AxUserLogo[0];
            _imageSize = UserLogoImageSize.Size128; 
        }

        /// <summary>
        /// 设置用户信息并绘制到屏幕
        /// </summary>
        /// <param name="Users"></param>
        public void SetPanel(UserAndRight.User[] Users)
        {
            _users = new UserAndRight.User[Users.Length];
            Users.CopyTo(_users, 0);
            AxUserResize();
            Draw();
        }

        public void SetPanel(UserAndRight.User[] Users,UserLogoImageSize size)
        {
            _users = new UserAndRight.User[Users.Length];
            Users.CopyTo(_users, 0);
            _imageSize = size;
            AxUserResize();
            Draw();
        }

        Size  GetChildImageSize()
        {
            Size sz = new Size(128, 128);
            switch (_imageSize)
            {
                case UserLogoImageSize.Size256 :
                    sz = new Size(256, 256);
                    break;
                case UserLogoImageSize.Size128 :
                    sz = new Size(128, 128);
                    break;
                case UserLogoImageSize.Size96 :
                    sz = new Size(96, 96);
                    break;
                case UserLogoImageSize.Size64 :
                    sz = new Size(64, 64);
                    break;
                case UserLogoImageSize.Size32 :
                    sz = new Size(32, 32);
                    break;
                case UserLogoImageSize.Size16 :
                    sz = new Size(16, 16);
                    break;
                default:
                    break;
            }
            return sz;
        }

        void Draw()
        {
            Size sz = GetChildImageSize();
            if (visibleCount > _users.Length)
                visibleCount = _users.Length;
            int maxSplit = 30;
            //int i = visibleCount * (128 + maxSplit) - maxSplit;
            int i = visibleCount * (sz.Width + maxSplit) - maxSplit;
            int left = (int)((Width - i) / 2);
            int top_1 = (this.Height - 32) / 2;
            int top_2 = (this.Height - sz.Height) / 2;
            for (int j = leftIndex; j < leftIndex + visibleCount && j < _users.Length; j++)
            {
                UserAndRight.User u = _users[j];
                _axUser[j - leftIndex].Name = "AxUserLogo" + u.ID.ToString();
                _axUser[j - leftIndex].Logo = u.Logo;
                _axUser[j - leftIndex].Text = u.Name;
                _axUser[j - leftIndex].Location = new Point(left, top_2);
                _axUser[j - leftIndex].Tag = u;
                _axUser[j - leftIndex].SenderMode = this.SenderMode;
                _axUser[j - leftIndex].Refresh();
                _axUser[j - leftIndex].ForeColor = this.ForeColor;

                left = left + sz.Width  + maxSplit;
            }
        }

        void AxUserResize()
        {
            Size sz = GetChildImageSize();
            visibleCount = (Width - (32 + 3) * 2 + 30) / ((sz.Width + 30));
            this.Controls.Clear();
            _axUser = new AxUserLogo[visibleCount];
            for (int j = 0; j < visibleCount && j < _users.Length; j++)
            {
                UserAndRight.User u = _users[j];
                _axUser[j] = new AxUserLogo(_imageSize);
                _axUser[j].MouseClick += new MouseEventHandler(AxUserLogoPanel_MouseClick);
            }
            this.Controls.AddRange(_axUser);
        }

        void AxUserLogoPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (UserLogoMouseClick != null)
                UserLogoMouseClick(sender, e);
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            if (_users == null)
                return;
            if (_users.Length <= 0)
                return;
            AxUserResize();
            Draw();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Size sz = GetChildImageSize();
            int maxSplit = 30;
            int top_1 = (this.Height - 32) / 2;
            int top_2 = (this.Height - sz.Height ) / 2;
            if (visibleCount > _users.Length)
                visibleCount = _users.Length;
            int i = visibleCount * (sz.Width  + maxSplit) - maxSplit;
            //求maxSplit
            if (visibleCount < _users.Length)
            {
                //在两边加左右移动按钮
                e.Graphics.DrawImage(Properties.Resources.prev, new Rectangle(0, top_1, 32, 32), new Rectangle(0, 0, 32, 32), GraphicsUnit.Pixel);
                e.Graphics.DrawImage(Properties.Resources.next, new Rectangle(this.Width - 33, top_1, 32, 32), new Rectangle(0, 0, 32, 32), GraphicsUnit.Pixel);
            }
            base.OnPaint(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.X < 32 && leftIndex > 0)
            {
                this.Cursor = Cursors.Hand ;
            }
            else if (e.X > (this.Width - 33) && (leftIndex < (_users.Length - visibleCount)))
            {
                this.Cursor = Cursors.Hand;
            }
            else
                this.Cursor = Cursors.Default;  
            base.OnMouseMove(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.X < 32 && leftIndex > 0)
            {
                leftIndex--;
                Draw();
                Refresh();
            }
            else if (e.X > (this.Width - 33) && (leftIndex < (_users.Length - visibleCount)))
            {
                leftIndex++;
                Draw();
                Refresh();
            }
        }
    }
}
