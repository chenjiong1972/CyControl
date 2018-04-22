using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.Core.Printer
{
    internal partial class FrmPreview : Form
    {
        const string FRMTEXT = "打印预览 - ";
        public bool DrawPrintDataOnShown = false;
        private PrintAssign printAssign;

        public FrmPreview()
        {
            InitializeComponent();
            this.printPreviewControl1.UseAntiAlias = true; 
        }

        public void GetPrintDocument(System.Drawing.Printing.PrintDocument p)
        {
            printPreviewControl1.Document = p;
            printAssign = default(PrintAssign);
            this.Text = FRMTEXT + p.DocumentName;
        }

        public void GetPrintDocument(PrintAssign p)
        {
            printPreviewControl1.Document = p.printDoc;
            printAssign = p;
            this.Text = FRMTEXT + p.printDoc.DocumentName;
        }

        public void InvalidatePreview()
        {
            if (!DrawPrintDataOnShown)
            {
                printPreviewControl1.InvalidatePreview();
                this.Text = FRMTEXT + printAssign.PrintTitle;
                toolStripComboBox1.Items.Clear();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (printAssign.ShowPageSetupDialog(this))
            {
                printAssign.Preview(this);
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if ((printPreviewControl1.StartPage - printPreviewControl1.Columns * printPreviewControl1.Rows) > 0)
                printPreviewControl1.StartPage = printPreviewControl1.StartPage - printPreviewControl1.Columns * printPreviewControl1.Rows;
            else
                printPreviewControl1.StartPage = 0;
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            printPreviewControl1.StartPage = printPreviewControl1.StartPage + printPreviewControl1.Columns * printPreviewControl1.Rows;
        }

        private void OpacityChenged(object sender, EventArgs e)
        {
            switch (toolStripComboBox2.SelectedIndex)
            {
                case 0:
                    this.Opacity = 1.0; break;
                case 1:
                    this.Opacity = 0.9; break;
                case 2:
                    this.Opacity = 0.7; break;
                case 3:
                    this.Opacity = 0.5; break;
                case 4:
                    this.Opacity = 0.3; break;
                default:
                    break;
            }
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            float h; float w; float hZoom; float wZoom;
            printPreviewControl1.Columns = 1;
            printPreviewControl1.Rows = 1;
            //完整显示一页
            Rectangle rect = printPreviewControl1.ClientRectangle;
            rect.Width = rect.Width - 10;
            rect.Height = rect.Height - 10;
            if (!printPreviewControl1.Document.DefaultPageSettings.Landscape)
            {
                wZoom = (float)rect.Width / printPreviewControl1.Document.DefaultPageSettings.PaperSize.Width;
                hZoom = (float)rect.Height / printPreviewControl1.Document.DefaultPageSettings.PaperSize.Height;
            }
            else
            {
                wZoom = (float)printPreviewControl1.Document.DefaultPageSettings.PaperSize.Height / rect.Width;
                hZoom = (float)printPreviewControl1.Document.DefaultPageSettings.PaperSize.Width / rect.Height;
            }
            if (!printPreviewControl1.Document.DefaultPageSettings.Landscape)
            {
                w = printPreviewControl1.Document.DefaultPageSettings.PaperSize.Width * wZoom;
                h = printPreviewControl1.Document.DefaultPageSettings.PaperSize.Height * wZoom;
            }
            else
            {
                w = printPreviewControl1.Document.DefaultPageSettings.PaperSize.Height * wZoom;
                h = printPreviewControl1.Document.DefaultPageSettings.PaperSize.Width * wZoom;
            }
            if (h >= rect.Height)
                printPreviewControl1.Zoom = hZoom;
        }

        private void 双页ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Zoom = 0.2;
            printPreviewControl1.Columns = 2;
        }

        private void 页宽ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //按页宽完整显示一页
            float wZoom;
            printPreviewControl1.Columns = 1;
            printPreviewControl1.Rows = 1;
            //完整显示一页
            Rectangle rect = printPreviewControl1.ClientRectangle;
            rect.Width = rect.Width - 10;
            if (!printPreviewControl1.Document.DefaultPageSettings.Landscape)
            {
                wZoom = (float)rect.Width / printPreviewControl1.Document.DefaultPageSettings.PaperSize.Width;
            }
            else
            {
                wZoom = (float)printPreviewControl1.Document.DefaultPageSettings.PaperSize.Height / rect.Width;
            }
            printPreviewControl1.Zoom = wZoom;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Zoom = 0.2;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Zoom = 0.4;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Zoom = 0.6;
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Zoom = 0.8;
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Zoom = 1.0;
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Zoom = 1.2;
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Zoom = 1.4;
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Zoom = 1.6;
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Zoom = 1.8;
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Zoom = 2.0;
        }

        private void x2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Zoom = 0.2;
            printPreviewControl1.Columns = 2;
            printPreviewControl1.Rows = 1;
        }

        private void x3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Zoom = 0.2;
            printPreviewControl1.Columns = 3;
            printPreviewControl1.Rows = 1;
        }

        private void x1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Zoom = 0.2;
            printPreviewControl1.Columns = 1;
            printPreviewControl1.Rows = 2;
        }

        private void x2ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Zoom = 0.2;
            printPreviewControl1.Columns = 2;
            printPreviewControl1.Rows = 2;
        }

        private void x3ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Zoom = 0.2;
            printPreviewControl1.Columns = 3;
            printPreviewControl1.Rows = 2;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Columns = 1;
            printPreviewControl1.Rows = 1;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            printPreviewControl1.Document.PrintController = new System.Drawing.Printing.StandardPrintController();
            if (!(printAssign == null))
                printAssign.Print();
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            if (toolStripComboBox1.Items.Count <= 0)
            {
                for (int i = 1; i <= printAssign.PrintDatas.phyPageCount; i++)
                {
                    this.toolStripComboBox1.Items.Add("第 " + i.ToString() + " 页");
                }
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            printPreviewControl1.Columns = 1;
            printPreviewControl1.Rows = 1;
            printPreviewControl1.StartPage = toolStripComboBox1.SelectedIndex;
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            if (DrawPrintDataOnShown)
            {
                printAssign.Preview(this);
                DrawPrintDataOnShown = false;
            }
        }
    }
}