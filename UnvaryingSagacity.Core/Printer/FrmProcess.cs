using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.Core.Printer
{
    public partial class FrmProcess : Form
    {
        private PrintAssign printAssign;
        private bool fristLoad = true;
        public FrmProcess(PrintAssign p)
        {
            InitializeComponent();
            printAssign = p;
        }

        public void PrinttingInfo(int CurLogicPage, int LogicPageCount, string PromptInfo)
        {
            if (this.Visible)
            {
                Graphics g = this.CreateGraphics();
                g.Clear(this.BackColor);
                g.DrawString(PromptInfo, this.Font, Brushes.Black, new PointF(33, 33));
                if (CurLogicPage >= 0)
                    progressBar1.Value = CurLogicPage;
            }
        }

        private void Form_Activated(object sender, EventArgs e)
        {
            this.Refresh();
            if (printAssign != null && fristLoad)
            {
                fristLoad = false;
                printAssign.Printting();
            }
        }

        private void Form_Closed(object sender, FormClosedEventArgs e)
        {
            fristLoad = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            printAssign.StopNow = true;
            this.Close();
        }

        private void FrmProcess_Paint(object sender, PaintEventArgs e)
        {
            System.Drawing.Drawing2D.GraphicsPath shape = new System.Drawing.Drawing2D.GraphicsPath();
            Core.ImageHandler.ArcRectanglePath(shape, 0, 0, this.Width-1, this.Height-1, 10, 10); 
            this.Region = new System.Drawing.Region(shape);
            //e.Graphics.DrawPath(new Pen(Color.Blue), shape1);
        }
    }
}