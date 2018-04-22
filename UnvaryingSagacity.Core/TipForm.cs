using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D ;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.Core
{
    partial class TipForm : Form
    {
        public TipForm()
        {
            InitializeComponent();
            Core.Win32API.SetClassLong(this.Handle, Core.Win32API.GCL_STYLE, Core.Win32API.GetClassLong(this.Handle, Core.Win32API.GCL_STYLE) | Core.Win32API.CS_DROPSHADOW);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            GraphicsPath shape=new GraphicsPath ();
            Core.ImageHandler.ArcRectanglePath(shape, 0, 0, this.Width, this.Height, 10, 10);
            this.Region = new Region(shape);
             //e.Graphics.DrawRectangle (Pens.Black , new Rectangle (0,0,this.Width-1 ,this.Height-1 ));  
           base.OnPaint(e);
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.HWnd == this.Handle)
            {
                if (m.Msg == (int)Core.Msgs.WM_ACTIVATE)
                {
                    if (m.WParam.ToInt32() == (int)Core.Msgs.WA_ACTIVE)
                        m.Result = new IntPtr(-1);
                    else
                        base.WndProc(ref m);
                }
                else if (m.Msg == (int)Core.Msgs.WM_MOUSEACTIVATE)
                {
                    m.Result = new IntPtr(-1);
                }
                else if (m.Msg == (int)Core.Msgs.WM_LBUTTONDBLCLK)
                {
                    m.Result = new IntPtr(-1);
                }
                else
                    base.WndProc(ref m);
            }
            else
                base.WndProc(ref m);
        }
    }
}
