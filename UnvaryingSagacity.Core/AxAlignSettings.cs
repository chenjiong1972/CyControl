using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.Core
{
    public partial class AxAlignSettings : UserControl
    {
        public AxAlignSettings()
        {
            InitializeComponent();
            this.VisibleChanged += new EventHandler(AxAlignSettings_VisibleChanged);
        }

        void AxAlignSettings_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                foreach (Control c in this.Controls)
                {
                    if (c is Button)
                    {
                        (c as Button).FlatStyle = FlatStyle.Standard; 
                    }
                }
                switch  (Align )
                {
                    case ContentAlignment.BottomCenter:
                        button9.FlatStyle = FlatStyle.Flat; 
                        break;
                    case ContentAlignment.BottomLeft:
                        button3.FlatStyle = FlatStyle.Flat;
                        break;
                    case ContentAlignment.BottomRight:
                        button4.FlatStyle = FlatStyle.Flat;
                        break;
                    case ContentAlignment.MiddleCenter:
                        button8.FlatStyle = FlatStyle.Flat;
                        break;
                    case ContentAlignment.MiddleLeft:
                        button2.FlatStyle = FlatStyle.Flat;
                        break;
                    case ContentAlignment.MiddleRight:
                        button5.FlatStyle = FlatStyle.Flat;
                        break;
                    case ContentAlignment.TopCenter:
                        button7.FlatStyle = FlatStyle.Flat;
                        break;
                    case ContentAlignment.TopLeft:
                        button1.FlatStyle = FlatStyle.Flat;
                        break;
                    case ContentAlignment.TopRight:
                        button6.FlatStyle = FlatStyle.Flat;
                        break;
                    default :
                        break;
                }
                foreach (Control c in this.Controls)
                {
                    if (c is Button)
                    {
                        if ((c as Button).FlatStyle == FlatStyle.Flat)
                            c.Focus();

                    }
                }
            }
        }

        public ContentAlignment Align { get; set; }

        public bool DisplayText
        {
            set
            {
                if (!value)
                {
                    foreach (Control c in this.Controls)
                    {
                        if (c is Button)
                        {
                            (c as Button).Text = "";
                        }
                    }
                }
                else
                {
                    button1.Text = "左顶";
                    button2.Text = "左中";
                    button3.Text = "左底";
                    button4.Text = "右底";
                    button5.Text = "右中";
                    button6.Text = "右顶";
                    button7.Text = "顶部居中";
                    button8.Text = "居中";
                    button9.Text = "底部居中";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Align = ContentAlignment.TopLeft;
            this.Visible = false;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Align = ContentAlignment.TopCenter;
            this.Visible = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Align = ContentAlignment.TopRight;
            this.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Align = ContentAlignment.MiddleLeft;
            this.Visible = false;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Align = ContentAlignment.MiddleCenter ;
            this.Visible = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Align = ContentAlignment.MiddleRight ;
            this.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Align = ContentAlignment.BottomLeft  ;
            this.Visible = false;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Align = ContentAlignment.BottomCenter ;
            this.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Align = ContentAlignment.BottomRight ;
            this.Visible = false;
        }
    }
}
