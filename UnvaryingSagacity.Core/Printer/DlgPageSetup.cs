using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#region 在内部单位统一使用0.01英寸=1/4mm
/// <summary>
/// 在内部单位统一使用0.01英寸=1/4mm,即默认单位PrinterUnit.Display
/// 使用代码编辑器修改此方法的内容。
/// </summary>
#endregion

namespace UnvaryingSagacity.Core.Printer
{
    internal partial class DlgPageSetup : Form
    {
        private PrintAssign printAssign;
        private PageSetup myPageSetup;
        private int mTop, mLeft, mWidth, mHeight;
        private float Ratio=1;
        private bool isFrist = true;
        public bool pageSetupChanged = false;

        public DlgPageSetup(PrintAssign PrintAssign)
        {
            InitializeComponent();
            printAssign = PrintAssign;
            myPageSetup = PrintAssign.PageSetup;
            InitControls();
        }

        private void InitControls()
        {
            cboZoom.Items.Clear();
            cboZoom.Items.Add("行高列宽均改变");
            cboZoom.Items.Add("仅行高改变");
            cboZoom.Items.Add("仅列宽改变");
            cboZoom.SelectedIndex = 0;
            if (!(bool)(myPageSetup.GetAttributes(PageSetupKey.ePage_不允许设置页眉的边框)))
                cboPageItem.Items.Add(new CboItem("页眉字体", 1));
            if (!(bool)(myPageSetup.GetAttributes(PageSetupKey.ePage_不允许设置主标题的边框)))
                cboPageItem.Items.Add(new CboItem("主标题字体", 2));
            if (!(bool)(myPageSetup.GetAttributes(PageSetupKey.ePage_不允许设置副标题的边框)))
                cboPageItem.Items.Add(new CboItem("副标题字体", 3));
            if (!(bool)(myPageSetup.GetAttributes(PageSetupKey.ePage_不允许设置表头的边框)))
                cboPageItem.Items.Add(new CboItem("表头字体", 4));
            cboPageItem.Items.Add(new CboItem("正文字体", 5));
            if (!(bool)(myPageSetup.GetAttributes(PageSetupKey.ePage_不允许设置表尾的边框)))
                cboPageItem.Items.Add(new CboItem("表尾字体", 6));
            if (!(bool)(myPageSetup.GetAttributes(PageSetupKey.ePage_不允许设置页脚的边框)))
                cboPageItem.Items.Add(new CboItem("页脚字体", 7));

            cboLogText.Items.Add("左上横放");
            cboLogText.Items.Add("右上横放");
            cboLogText.Items.Add("左下横放");
            cboLogText.Items.Add("右下横放");
            cboLogText.Items.Add("左中竖放");
            cboLogText.Items.Add("右中竖放");
            cboLogText.Items.Add("上中横放");
            cboLogText.Items.Add("下中横放");
            cboLogText.Items.Add("左上竖放");
            cboLogText.Items.Add("左下竖放");
            cboLogText.Items.Add("右上竖放");
            cboLogText.Items.Add("右下竖放");
            General.SelectStringInComboBox(cboLogText, "左上横放");


            cboLogImage.Items.Add("主标题下");
            cboLogImage.Items.Add("左上角");
            cboLogImage.Items.Add("右上角");
            cboLogImage.Items.Add("左下角");
            cboLogImage.Items.Add("右下角");
            cboLogImage.Items.Add("页末中");
            cboLogImage.SelectedIndex = 0;
            General.SelectStringInComboBox(cboLogImage, "主标题下");


            cboLogBorder.Items.Add("(无)");
            cboLogBorder.Items.Add("左边线");
            cboLogBorder.Items.Add("上边线");
            cboLogBorder.Items.Add("下边线");
            cboLogBorder.Items.Add("右边线");
            cboLogBorder.Items.Add("全部边线");
            cboLogBorder.SelectedIndex = 0;
            General.SelectStringInComboBox(cboLogBorder, "(无)");

            cboLogBorderStyle.Items.Add(new CboItem("透明边框 ", (int)PrinterBorderStyle.无边框));
            cboLogBorderStyle.Items.Add(new CboItem("实线边框", (int)PrinterBorderStyle.实线边框));
            cboLogBorderStyle.Items.Add(new CboItem("短线边框", (int)PrinterBorderStyle.短线边框));
            cboLogBorderStyle.Items.Add(new CboItem("点线边框 ", (int)PrinterBorderStyle.点线边框));
            cboLogBorderStyle.Items.Add(new CboItem("短点线边框", (int)PrinterBorderStyle.短点线边框));
            cboLogBorderStyle.Items.Add(new CboItem("短点点线边框", (int)PrinterBorderStyle.短点点线边框));
            cboLogBorderStyle.Items.Add(new CboItem("双实线边框", (int)PrinterBorderStyle.双实线边框));
            cboLogBorderStyle.SelectedIndex = 0;
            General.SelectStringInComboBox(cboLogBorderStyle, "透明边框");


            cboPageItemBorder.Items.Add("(无)");
            cboPageItemBorder.Items.Add("左边线");
            cboPageItemBorder.Items.Add("上边线");
            cboPageItemBorder.Items.Add("下边线");
            cboPageItemBorder.Items.Add("右边线");
            cboPageItemBorder.Items.Add("全部边线");
            cboPageItemBorder.SelectedIndex = 0;
            General.SelectStringInComboBox(cboPageItemBorder, "(无)");


            cboPageItemBorderStyle.Items.Add(new CboItem("透明边框 ", (int)PrinterBorderStyle.无边框));
            cboPageItemBorderStyle.Items.Add(new CboItem("实线边框", (int)PrinterBorderStyle.实线边框));
            cboPageItemBorderStyle.Items.Add(new CboItem("短线边框", (int)PrinterBorderStyle.短线边框));
            cboPageItemBorderStyle.Items.Add(new CboItem("点线边框 ", (int)PrinterBorderStyle.点线边框));
            cboPageItemBorderStyle.Items.Add(new CboItem("短点线边框", (int)PrinterBorderStyle.短点线边框));
            cboPageItemBorderStyle.Items.Add(new CboItem("短点点线边框", (int)PrinterBorderStyle.短点点线边框));
            cboPageItemBorderStyle.Items.Add(new CboItem("双实线边框", (int)PrinterBorderStyle.双实线边框));
            cboPageItemBorderStyle.SelectedIndex = 0;
            General.SelectStringInComboBox(cboPageItemBorderStyle, "透明边框");

            txtLogText.Enabled = !bool.Parse(myPageSetup.GetAttributes(PageSetupKey.ePage_不允许设置标志文本的内容).ToString());
            if (myPageSetup.LogPic!=null )
            {
                ImgLog.SizeMode = PictureBoxSizeMode.StretchImage;
                ImgLog.Image  = myPageSetup.LogPic;
            }
        }
        private bool CheckFmt()
        {
            bool b;
            float x;
            float y;
            x = myPageSetup.ScaleWidth;
            y = myPageSetup.ScaleHeight;

            if (x  < float.Parse(txtWidth.Text.Length >0?txtWidth.Text :"0" )*PrintAssign.PixPerMM  )
            {
                MessageBox.Show("页面宽度不能超过打印纸宽度!", "页面设置", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (y < float.Parse(txtHeight.Text.Length > 0 ? txtHeight.Text : "0") * PrintAssign.PixPerMM)
            {
                MessageBox.Show("页面高度不能超过打印纸高度!", "页面设置", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if ((float.Parse(txtWidth.Text.Length > 0 ? txtWidth.Text : "0") - float.Parse(txtLeft.Text.Length >0?txtLeft.Text :"0" ) - float.Parse(txtRight.Text.Length>0?txtRight.Text :"0"  )) < 10f)
            {
                MessageBox.Show("页面有效宽度(页面宽度-左空-右空)不能太小!", "页面设置", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if ((float.Parse(txtHeight.Text.Length >0?txtHeight.Text:"0") - float.Parse(txtTop.Text.Length >0?txtTop.Text:"0") - float.Parse(txtBottom.Text.Length>0?txtBottom.Text :"0"  )) < 10f)
            {
                MessageBox.Show("页面有效高度(页面高度-顶空-底空)不能太小!", "页面设置", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            b = CheckColor(myPageSetup.SubTitleColor);
            return b;
        }

        private bool CheckColor(int color)
        {
            switch (color)
            {
                case -16777216: break;
                case 0: break;
                case 128: break;
                case 32768: break;
                case 32896: break;
                case 8388608: break;
                case 8388736: break;
                case 8421376: break;
                case 8421504: break;
                case 12632256: break;
                case 255: break;
                case 65280: break;
                case 65535: break;
                case 16711680: break;
                case 16711935: break;
                case 16776960: break;
                default:
                    MessageBox.Show("前景色不符合要求, 请重新选择前景色.", "前景色", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
            }
            return true;
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            this.Text = "页面设置 - " + printAssign.printTitle;
            RefreshPrintName(new System.Drawing.Printing.PaperSize(printAssign.PageSetup.PaperName, (int)(printAssign.PageSetup.PageWidth), (int)(printAssign.PageSetup.PageHeight)));
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (SavePageSetting())
            {
                pageSetupChanged = true;
                this.Close();
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            PrintSelect();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (SavePageSetting())
            {
                pageSetupChanged = true;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PrintSelect();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PageSelect();
        }

        private void PrintSelect()
        {
            PrintDialog p = new PrintDialog();
            System.Drawing.Printing.PrinterSettings pSet = new System.Drawing.Printing.PrinterSettings();
            pSet.PrinterName = myPageSetup.PrintName;
            pSet.DefaultPageSettings.Landscape = myPageSetup.Landscape;
            p.PrinterSettings = pSet;
            p.UseEXDialog = false;
            if (p.ShowDialog() == DialogResult.OK)
            {
                myPageSetup.PrintName = pSet.PrinterName;
                myPageSetup.Landscape = pSet.DefaultPageSettings.Landscape;
                myPageSetup.PaperSize = pSet.DefaultPageSettings.PaperSize.RawKind;
                myPageSetup.PageWidth = pSet.DefaultPageSettings.PaperSize.Width;
                myPageSetup.PageHeight = pSet.DefaultPageSettings.PaperSize.Height;
                myPageSetup.ScaleWidth = pSet.DefaultPageSettings.PrintableArea.Width;
                myPageSetup.ScaleHeight = pSet.DefaultPageSettings.PrintableArea.Height;
                myPageSetup.ColorPrint = pSet.SupportsColor;
                RefreshPrintName(pSet.DefaultPageSettings.PaperSize);
            }
        }

        private void PageSelect()
        {
            PageSetupDialog p = new PageSetupDialog();
            System.Drawing.Printing.PageSettings pSet = new System.Drawing.Printing.PageSettings();
            pSet.Landscape = myPageSetup.Landscape;
            System.Drawing.Printing.PaperSize pSize = new System.Drawing.Printing.PaperSize();
            pSize.RawKind = myPageSetup.PaperSize;
            pSet.PaperSize = pSize;
            p.PageSettings = pSet;
            if (p.ShowDialog() == DialogResult.OK)
            {
                myPageSetup.Landscape = pSet.Landscape;
                myPageSetup.PaperSize = pSet.PaperSize.RawKind;
                myPageSetup.PaperName = pSet.PaperSize.PaperName;
                if (myPageSetup.Landscape)
                {
                    myPageSetup.PageWidth = pSet.PaperSize.Height;
                    myPageSetup.PageHeight = pSet.PaperSize.Width;
                    myPageSetup.ScaleWidth = pSet.PrintableArea.Height;
                    myPageSetup.ScaleHeight = pSet.PrintableArea.Width;
                }
                else
                {
                    myPageSetup.PageWidth = pSet.PaperSize.Width;
                    myPageSetup.PageHeight = pSet.PaperSize.Height;
                    myPageSetup.ScaleWidth = pSet.PrintableArea.Width;
                    myPageSetup.ScaleHeight = pSet.PrintableArea.Height;
                }
                RefreshPrintName(pSet.PaperSize);
            }
        }

        private void RefreshPrintName(System.Drawing.Printing.PaperSize pSize)
        {
            float x, y;

            checkBoxRoate.Enabled = myPageSetup.Landscape;
            lblPrint.Text = myPageSetup.PrintName;
            lblPage.Text = pSize.PaperName + "纸张规格(宽 x 高): " + ((int)(myPageSetup.PageWidth / PrintAssign.PixPerMM)).ToString() + " x " + ((int)(myPageSetup.PageHeight / PrintAssign.PixPerMM)).ToString();
            lblPageSize.Text = "有效范围(宽 x 高): " + ((int)(myPageSetup.ScaleWidth / PrintAssign.PixPerMM)).ToString() + " x " + ((int)(myPageSetup.ScaleHeight / PrintAssign.PixPerMM)).ToString();
            if (myPageSetup.SupportColor(myPageSetup.PrintName))
            {
                chkColor.Text = "使用彩色打印";
                chkColor.Checked = true;
                chkColor.Enabled = true;
            }
            else
            {
                chkColor.Text = "使用彩色打印(打印机不能打印彩色)";
                chkColor.Checked = false;
                chkColor.Enabled = false;
            }
            x = myPageSetup.ScaleWidth;
            y = myPageSetup.ScaleHeight;
            if (chkFullPage.Checked)
            {
                txtWidth.Text = ((int)(x / PrintAssign.PixPerMM)).ToString();
                txtHeight.Text = ((int)(y / PrintAssign.PixPerMM)).ToString();
            }
            if (mWidth / x < mHeight / y)
                Ratio = mWidth / x;
            else
                Ratio = mHeight / y;
            picPrintRange.Width = (int)(myPageSetup.PageWidth * Ratio) + 3;
            picPrintRange.Height = (int)(myPageSetup.PageHeight * Ratio) + 3;
            picPreview.Width = (int)(x * Ratio) + 1;
            picPreview.Height = (int)(y * Ratio) + 1;
            picPrintRange.Top = mTop + (mHeight - picPrintRange.Height) / 2;
            picPrintRange.Left = mLeft + (mWidth - picPrintRange.Width) / 2;
            picPreview.Top =  (picPrintRange.Height - picPreview.Height) / 2;
            picPreview.Left =  (picPrintRange.Width - picPreview.Width) / 2;
            picPrintRange.Width = (int)(myPageSetup.PageWidth * Ratio) + 6;
            picPrintRange.Height = (int)(myPageSetup.PageHeight * Ratio) + 6;
            Lbl_Ratio.Text = "比例  1:" + (int)(1 / Ratio);
            //picPreview.Scale(new SizeF(1/Ratio, 1/Ratio));
            //picPreview.Scale(new SizeF(1 , 1));
            picPreview.Refresh();
        }

        private void cboPageItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            Font oFont = new Font("宋体", 9);

            GetPageItemBorder();
            GetPageItemFont(out oFont);
            SetPictureFont(oFont);
            picPageItemFont.Refresh();
        }

        private void RefreshControlBoxState()
        {
            bool b;
            b = chkZoom.Checked;
            if (b)
            {
                chkCenterH.Checked = !b;
                chkCenterV.Checked = !b;
            }
            chkCenterH.Enabled = !b;
            chkCenterV.Enabled = !b;
            OptAdjustMethod0.Enabled = b;
            OptAdjustMethod1.Enabled = b;
            b = chkFullPage.Checked;
            txtWidth.Enabled = !b;
            txtHeight.Enabled = !b;

            txtRows.Enabled = OptBody0.Checked;
            txtRowHeight.Enabled = OptBody1.Checked;
            chkAppendRows.Enabled = true; //OptBody1.Checked || OptBody0.Checked;

            b = chkLogText.Checked;
            if ((bool)(myPageSetup.GetAttributes(PageSetupKey.ePage_不允许设置标志文本的内容)))
                txtLogText.Enabled = b;

            cboLogBorder.Enabled = b;
            cboLogBorderStyle.Enabled = b;
            cboLogText.Enabled = b;

            b = chkLogPic.Checked;
            cboLogImage.Enabled = b;
            ImgLog.Visible = b;

            txtDbLinesRows.Enabled = chkDbLinesRows.Checked;
            picPreview.Refresh();
        }

        private bool SavePageSetting()
        {
            if (CheckFmt())
            {
                myPageSetup.Rotate90 = checkBoxRoate.Checked; 
                myPageSetup.FullPage = chkFullPage.Checked;
                myPageSetup.Width = float.Parse(txtWidth.Text.Length >0?txtWidth.Text:"0" ) * PrintAssign.PixPerMM;
                myPageSetup.Height = float.Parse(txtHeight.Text.Length >0?txtHeight.Text :"0" ) * PrintAssign.PixPerMM;
                myPageSetup.HeaderMargin = float.Parse(txtHeader.Text.Length >0?txtHeader.Text :"0" ) * PrintAssign.PixPerMM;
                myPageSetup.TopMargin = float.Parse(txtTop.Text.Length >0?txtTop.Text :"0") * PrintAssign.PixPerMM;
                myPageSetup.LeftMargin = float.Parse(txtLeft.Text.Length >0?txtLeft.Text :"0") * PrintAssign.PixPerMM;
                myPageSetup.RightMargin = float.Parse(txtRight.Text.Length > 0 ? txtRight.Text : "0") * PrintAssign.PixPerMM;
                myPageSetup.BottomMargin = float.Parse(txtBottom.Text.Length > 0 ? txtBottom.Text : "0") * PrintAssign.PixPerMM;
                myPageSetup.FooterMargin = float.Parse(txtFoot.Text.Length > 0 ? txtFoot.Text : "0") * PrintAssign.PixPerMM;
                myPageSetup.CenterHoriz = chkCenterH.Checked;
                myPageSetup.CenterVert = chkCenterV.Checked;
                myPageSetup.Rows = txtRows.Text.Length > 0 ? int.Parse(txtRows.Text) : 0;
                myPageSetup.RowHeight = txtRowHeight.Text.Length > 0 ? (int)(int.Parse(txtRowHeight.Text) * PrintAssign.PixPerMM) : 0;
                if (OptBody0.Checked)
                {
                    myPageSetup.SheetBodyStyle = PrinterBodyStyle.正文行数;
                    if (myPageSetup.Rows <= 0)
                    {
                        MessageBox.Show("正文行数必须大于零.", "页面设置", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
                else if (OptBody1.Checked)
                {
                    myPageSetup.SheetBodyStyle = PrinterBodyStyle.正文行高;
                    if (myPageSetup.RowHeight <= 0)
                    {
                        MessageBox.Show("正文行高数必须大于零.", "页面设置", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
                else
                    myPageSetup.SheetBodyStyle = PrinterBodyStyle.自动;

                myPageSetup.FillBlankLines = chkAppendRows.Checked;
                myPageSetup.CutLine = chkCutline.Checked;
                myPageSetup.HoriCutPage = chkHoriCutPage.Checked ? 1 : 0;
                myPageSetup.CyLine = chkCyLine.Checked;
                myPageSetup.CyDotLine = chkCyLineDot.Checked;
                myPageSetup.AutoArrange = chkAutoArrange.Checked ? 1 : 0;
                myPageSetup.Zoom = chkZoom.Checked ? 1 : 0;
                myPageSetup.ReduceFont = chkReduceFont.Checked ? 1 : 0;
                if (OptAdjustMethod0.Checked)
                    myPageSetup.ReduceMethod = 1;
                else
                    myPageSetup.ReduceMethod = 2;

                myPageSetup.KeepFont = chkFontKeep.Checked;
                myPageSetup.ColorPrint = chkColor.Checked;
                myPageSetup.LogPicPrint = chkLogPic.Checked;
                if (myPageSetup.LogPicPrint)
                    myPageSetup.LogPic = ImgLog.Image ;

                myPageSetup.LogPicLoc = (PrinterLogPictureLocation)cboLogImage.SelectedIndex;
                myPageSetup.LogTextPrint = chkLogText.Checked;
                myPageSetup.LogTextLoc = (PrinterLogTextLocation)cboLogText.SelectedIndex;
                myPageSetup.LogText = txtLogText.Text;
                myPageSetup.LogTextBorder = (PrinterLogTextBorder)cboLogBorder.SelectedIndex;
                myPageSetup.LogTextBorderStyle = (PrinterBorderStyle)((CboItem)cboLogBorderStyle.SelectedItem).ItemData;
                myPageSetup.PrintPageNum = chkPageNum.Checked ? true : false;
                myPageSetup.PrintPageCount = chkPageCount.Checked ? true : false;
                myPageSetup.PrintDate = chkPrintDate.Checked;
                myPageSetup.PrintOperator = chkOper.Checked;
                myPageSetup.DrawLineUnderPageHeader = chkLineUnderHeader.Checked;
                myPageSetup.DrawLineUpPageFooter = chkLineUpFoot.Checked;
                myPageSetup.DblLineRows = chkDbLinesRows.Checked ? int.Parse(txtDbLinesRows.Text) : 0;
                myPageSetup.SetAttributes(PageSetupKey.ePage_自动缩放模式, cboZoom.SelectedIndex);
                myPageSetup.SetAttributes(PageSetupKey.ePage_金额要求千分位, chkCySplitChar.Checked);
                myPageSetup.SetAttributes(PageSetupKey.ePage_细线宽度, int.Parse(txtChinLineWidth.Text));
                myPageSetup.SetAttributes(PageSetupKey.ePage_正文字体包括固定行列, chkFixInBody.Checked);
                return true;
            }
            else
                return false;
        }

        private void SetBorderColor(int color)
        {
            PageSetupKey i;
            if (cboPageItemBorder.Items.Count < 0) return;
            switch (((CboItem)cboPageItem.SelectedItem).ItemData)
            {
                case 1:
                    i = PageSetupKey.ePage_PageHeaderBorderColor; break;
                //myPageSetup.Attributes(PageSetupKey.ePage_PageHeaderBorderColor) = myPageSetup.GetAttributes(PageSetupKey.ePage_PageHeaderBorderColor).ToString().Substring (1 , 1) & "," & color; break;
                case 2:
                    i = PageSetupKey.ePage_MainTitleBorderColor; break;
                //myPageSetup.Attributes(PageSetupKey.ePage_MainTitleBorderColor) = Left(myPageSetup.Attributes(PageSetupKey.ePage_MainTitleBorderColor), 1) & "," & color; break;
                case 3:
                    i = PageSetupKey.ePage_SubTitleBorderColor; break;
                //myPageSetup.Attributes(PageSetupKey.ePage_SubTitleBorderColor) = Left(myPageSetup.Attributes(PageSetupKey.ePage_SubTitleBorderColor), 1) & "," & color; break;
                case 4:
                    i = PageSetupKey.ePage_HeaderBorderColor; break;
                //myPageSetup.Attributes(PageSetupKey.ePage_HeaderBorderColor) = Left(myPageSetup.Attributes(PageSetupKey.ePage_HeaderBorderColor), 1) & "," & color; break;
                case 5:
                    i = PageSetupKey.ePage_BodyBorderColor; break;
                //myPageSetup.Attributes(PageSetupKey.ePage_BodyBorderColor) = Left(myPageSetup.Attributes(PageSetupKey.ePage_BodyBorderColor), 1) & "," & color; break;
                case 6:
                    i = PageSetupKey.ePage_TailBorderColor; break;
                //myPageSetup.Attributes(PageSetupKey.ePage_TailBorderColor) = Left(myPageSetup.Attributes(PageSetupKey.ePage_TailBorderColor), 1) & "," & color; break;
                case 7:
                    i = PageSetupKey.ePage_PageFooterBorderColor; break;
                //myPageSetup.Attributes(PageSetupKey.ePage_PageFooterBorderColor) = Left(myPageSetup.Attributes(PageSetupKey.ePage_PageFooterBorderColor), 1) & "," & color; break;
                default:
                    return;
            }
            myPageSetup.SetAttributes(i, myPageSetup.GetAttributes(i).ToString().Substring(1, 1) + "," + color.ToString());
        }

        private void SetFontColor(int color)
        {
            if (cboPageItemBorder.Items.Count < 0) return;
            switch (((CboItem)cboPageItem.SelectedItem).ItemData)
            {
                case 1:
                    myPageSetup.PageHeaderColor = color; break;
                case 2:
                    {
                        if (CheckColor(color))
                        {
                            myPageSetup.MainTitleColor = color;
                            break;
                        }
                        else
                            return;
                    }
                case 3:
                    {
                        if (CheckColor(color))
                        {
                            myPageSetup.SubTitleColor = color;
                            break;
                        }
                        else
                            return;
                    }
                case 4:
                    myPageSetup.HeaderColor = color; break;
                case 5:
                    myPageSetup.BodyColor = color; break;
                case 6:
                    {
                        if (CheckColor(color))
                        {
                            myPageSetup.TailColor = color;
                            break;
                        }
                        else
                            return;
                    }
                case 7:
                    myPageSetup.PageFooterColor = color; break;
                default:
                    return;
            }
            picPageItemColor.BackColor = Color.FromArgb(color);

        }
        private void GetPageItemBorder()
        {
            int i = -1;
            bool b = false;
            string s = "";

            if (cboPageItemBorder.Items.Count < 0) return;
            cboPageItemBorder.Enabled = true;
            cboPageItemBorderStyle.Enabled = true;
            picPageItemColor.Enabled = true;
            picPageItemBorderColor.Enabled = true;
            switch (((CboItem)cboPageItem.SelectedItem).ItemData)
            {
                case 1:
                    i = (int)myPageSetup.PageHeaderBorder;
                    b = !(bool)(myPageSetup.GetAttributes(PageSetupKey.ePage_不允许设置页眉的边框));
                    s = myPageSetup.GetAttributes(PageSetupKey.ePage_PageHeaderBorderColor).ToString();
                    break;
                case 2:
                    i = (int)myPageSetup.MainTitleBorderStyle;
                    b = !(bool)(myPageSetup.GetAttributes(PageSetupKey.ePage_不允许设置主标题的边框));
                    s = myPageSetup.GetAttributes(PageSetupKey.ePage_MainTitleBorderColor).ToString();
                    break;
                case 3:
                    i = (int)myPageSetup.SubTitleBorderStyle;
                    b = !(bool)(myPageSetup.GetAttributes(PageSetupKey.ePage_不允许设置副标题的边框));
                    s = myPageSetup.GetAttributes(PageSetupKey.ePage_SubTitleBorderColor).ToString();
                    break;
                case 4:
                    i = (int)myPageSetup.HeaderBorderStyle;
                    b = !(bool)(myPageSetup.GetAttributes(PageSetupKey.ePage_不允许设置表头的边框));
                    s = myPageSetup.GetAttributes(PageSetupKey.ePage_HeaderBorderColor).ToString();
                    break;
                case 5:
                    i = (int)myPageSetup.BodyBorderStyle;
                    b = false;
                    cboPageItemBorderStyle.Enabled = false;
                    picPageItemColor.Enabled = false;
                    s = myPageSetup.GetAttributes(PageSetupKey.ePage_BodyBorderColor).ToString();
                    break;
                case 6:
                    i = (int)myPageSetup.TailBorderStyle;
                    b = !(bool)(myPageSetup.GetAttributes(PageSetupKey.ePage_不允许设置表尾的边框));
                    s = myPageSetup.GetAttributes(PageSetupKey.ePage_TailBorderColor).ToString();
                    break;
                case 7:
                    i = (int)myPageSetup.PageFooterBorderStyle;
                    b = !(bool)(myPageSetup.GetAttributes(PageSetupKey.ePage_不允许设置页脚的边框));
                    chkBorderColor.Enabled = cboPageItemBorderStyle.Enabled;
                    s = myPageSetup.GetAttributes(PageSetupKey.ePage_PageFooterBorderColor).ToString();
                    break;
                default:
                    cboPageItemBorderStyle.Enabled = true;
                    break;
            }
            General.SelectStringInComboBox(cboPageItemBorder, General.GetTextInComboBox(cboPageItemBorder, (short)i));
            General.SelectStringInComboBox(cboPageItemBorderStyle, General.GetTextInComboBox(cboPageItemBorderStyle, i));
            cboPageItemBorderStyle.Enabled = b;
            chkBorderColor.Enabled = cboPageItemBorderStyle.Enabled;
            chkBorderColor.Checked = s.Substring(0, 1) == "1" ? true : false;
            picPageItemBorderColor.BackColor = Color.FromArgb(int.Parse(s.Substring(2)));
        }

        private void GetPageItemFont(out Font oFont)
        {
            GetPageItemFont(out oFont, false);
        }

        private void GetPageItemFont(out Font oFont, bool NewFontIfNothing)
        {
            //string s = "";
            int color = 0;
            switch (((CboItem)(cboPageItem.SelectedItem)).ItemData)
            {
                case 1:
                    if ((myPageSetup.PageHeaderFont == null) && NewFontIfNothing)
                    {
                        myPageSetup.PageHeaderFont = new Font("宋体", 9);
                    }
                    oFont = (Font)myPageSetup.PageHeaderFont.Clone();
                    //s = "PageHeaderFont";
                    color = myPageSetup.PageHeaderColor;
                    break;
                case 2:
                    if ((myPageSetup.MainTitleFont == null) && NewFontIfNothing)
                    {
                        myPageSetup.MainTitleFont = new Font("宋体", 9);
                    }
                    oFont = (Font)myPageSetup.MainTitleFont.Clone();
                    //s = "MainTitleFont";
                    color = myPageSetup.MainTitleColor;
                    break;
                case 3:
                    if ((myPageSetup.SubTitleFont == null) && NewFontIfNothing)
                    {
                        myPageSetup.SubTitleFont = new Font("宋体", 9);
                    }
                    oFont = (Font)myPageSetup.SubTitleFont.Clone();
                    //s = "SubTitleFont";
                    color = myPageSetup.SubTitleColor;
                    break;
                case 4:
                    if ((myPageSetup.HeaderFont == null) && NewFontIfNothing)
                    {
                        myPageSetup.HeaderFont = new Font("宋体", 9);
                    }
                    oFont = (Font)myPageSetup.HeaderFont.Clone();
                    //s = "HeaderFont";
                    color = myPageSetup.HeaderColor;
                    break;
                case 5:
                    if ((myPageSetup.BodyFont == null) && NewFontIfNothing)
                    {
                        myPageSetup.BodyFont = new Font("宋体", 9);
                    }
                    oFont = (Font)myPageSetup.BodyFont.Clone();
                    //s = "BodyFont";
                    color = myPageSetup.BodyColor;
                    break;
                case 6:
                    if ((myPageSetup.TailFont == null) && NewFontIfNothing)
                    {
                        myPageSetup.TailFont = new Font("宋体", 9);
                    }
                    oFont = (Font)myPageSetup.TailFont.Clone();
                    //s = "TailFont";
                    color = myPageSetup.TailColor;
                    break;
                case 7:
                    if ((myPageSetup.PageFooterFont == null) && NewFontIfNothing)
                    {
                        myPageSetup.PageFooterFont = new Font("宋体", 9);
                    }
                    oFont = (Font)myPageSetup.PageFooterFont.Clone();
                    //s = "PageFooterFont";
                    color = myPageSetup.PageFooterColor;
                    break;
                default:
                    oFont = new Font("宋体", 9);
                    break;
            }
            picPageItemColor.BackColor = Color.FromArgb(color);
        }

        private void GetPrinterSetting()
        {
            int i;
            float f;
            checkBoxRoate.Enabled = myPageSetup.Landscape;  
            checkBoxRoate.Checked = myPageSetup.Rotate90;
            chkFullPage.Checked = myPageSetup.FullPage;
            f = (myPageSetup.ScaleWidth / PrintAssign.PixPerMM);
            txtWidth.Text = f.ToString();

            f = (myPageSetup.ScaleHeight / PrintAssign.PixPerMM);
            txtHeight.Text = f.ToString();
            f = (myPageSetup.HeaderMargin / PrintAssign.PixPerMM);
            txtHeader.Text = f.ToString();
            f = (myPageSetup.TopMargin / PrintAssign.PixPerMM);
            txtTop.Text = f.ToString();
            f = (myPageSetup.LeftMargin / PrintAssign.PixPerMM);
            txtLeft.Text = f.ToString();
            f = (myPageSetup.RightMargin / PrintAssign.PixPerMM);
            txtRight.Text = f.ToString();
            f = (myPageSetup.BottomMargin / PrintAssign.PixPerMM);
            txtBottom.Text = f.ToString();
            f = (myPageSetup.FooterMargin / PrintAssign.PixPerMM);
            txtFoot.Text = f.ToString();
            chkCenterH.Checked = myPageSetup.CenterHoriz;
            chkCenterV.Checked = myPageSetup.CenterVert;
            txtRows.Text = myPageSetup.Rows.ToString();
            f = (myPageSetup.RowHeight / PrintAssign.PixPerMM);
            txtRowHeight.Text = f.ToString();
            switch (myPageSetup.SheetBodyStyle)
            {
                case PrinterBodyStyle.正文行高:
                    OptBody1.Checked = true;
                    break;
                case PrinterBodyStyle.正文行数:
                    OptBody0.Checked = true;
                    break;
                default:
                    OptBody2.Checked = true;
                    break;
            }
            chkAppendRows.Checked = myPageSetup.FillBlankLines;
            chkHoriCutPage.Checked = myPageSetup.HoriCutPage==1?true :false ;
            chkAutoArrange.Checked = myPageSetup.AutoArrange == 1 ? true : false;

            chkLogPic.Checked = myPageSetup.LogPicPrint;
            if (myPageSetup.LogPicPrint)
                ImgLog.Image = myPageSetup.LogPic;

            cboLogImage.SelectedIndex = (int)myPageSetup.LogPicLoc;
            chkLogText.Checked = myPageSetup.LogTextPrint;
            cboLogText.SelectedIndex = (int)myPageSetup.LogTextLoc;
            cboLogBorder.SelectedIndex = (int)myPageSetup.LogTextBorder;
            i = General.GetIndexInComboBox(cboLogBorderStyle, (int)myPageSetup.LogTextBorderStyle);
            if (i >= 0)
                cboLogBorderStyle.SelectedIndex = i;
            if (!(myPageSetup.LogTextFont == null))
                txtLogText.Font = (Font)myPageSetup.LogTextFont.Clone();
            txtLogText.Text = myPageSetup.LogText;
            txtLogText.Refresh();

            chkCyLine.Checked = myPageSetup.CyLine;
            chkCyLineDot.Checked = myPageSetup.CyDotLine;
            chkOper.Checked = myPageSetup.PrintOperator;
            chkPrintDate.Checked = myPageSetup.PrintDate;
            chkPageNum.Checked = myPageSetup.PrintPageNum;
            chkPageCount.Checked = myPageSetup.PrintPageCount;
            chkCutline.Checked = myPageSetup.CutLine;
            chkColor.Checked = myPageSetup.ColorPrint;
            chkLineUnderHeader.Checked = myPageSetup.DrawLineUnderPageHeader;
            chkLineUpFoot.Checked = myPageSetup.DrawLineUpPageFooter;
            chkZoom.Checked = myPageSetup.Zoom > 0 ? true : false;
            chkReduceFont.Checked = myPageSetup.ReduceFont > 0 ? true : false;
            chkFixInBody.Checked = (bool)myPageSetup.GetAttributes(PageSetupKey.ePage_正文字体包括固定行列);
            OptAdjustMethod0.Enabled = chkZoom.Checked;
            OptAdjustMethod1.Enabled = chkZoom.Checked;
            if (myPageSetup.ReduceMethod == 2)
                OptAdjustMethod1.Checked = true;
            else
                OptAdjustMethod0.Checked = true;
            chkFontKeep.Checked = myPageSetup.KeepFont;
            chkDbLinesRows.Checked = myPageSetup.DblLineRows > 0 ? true : false;
            txtDbLinesRows.Text = myPageSetup.DblLineRows > 0 ? myPageSetup.DblLineRows.ToString() : "0";
            cboZoom.SelectedIndex = (int)(myPageSetup.GetAttributes(PageSetupKey.ePage_自动缩放模式));
            chkCySplitChar.Checked =( bool)(myPageSetup.GetAttributes(PageSetupKey.ePage_金额要求千分位));
            txtChinLineWidth.Text = myPageSetup.GetAttributes(PageSetupKey.ePage_细线宽度).ToString();
        }


        private void SetPageItemBorder()
        {
            if (cboPageItemBorder.Items.Count < 0) return;
            if (cboPageItemBorderStyle.Items.Count < 0) return;
            switch (((CboItem)cboPageItem.SelectedItem).ItemData)
            {
                case 1:
                    myPageSetup.PageHeaderBorder = (PrinterLogTextBorder)cboPageItemBorder.SelectedIndex;
                    myPageSetup.PageFooterBorderStyle = (PrinterBorderStyle)((CboItem)cboPageItemBorderStyle.SelectedItem).ItemData;
                    break;
                case 2:
                    myPageSetup.MainTitleBorder = (PrinterLogTextBorder)cboPageItemBorder.SelectedIndex;
                    myPageSetup.MainTitleBorderStyle = (PrinterBorderStyle)((CboItem)cboPageItemBorderStyle.SelectedItem).ItemData;
                    break;
                case 3:
                    myPageSetup.SubTitleBorder = (PrinterLogTextBorder)cboPageItemBorder.SelectedIndex;
                    myPageSetup.SubTitleBorderStyle = (PrinterBorderStyle)((CboItem)cboPageItemBorderStyle.SelectedItem).ItemData;
                    break;
                case 4:
                    myPageSetup.HeaderBorder = (PrinterLogTextBorder)cboPageItemBorder.SelectedIndex;
                    myPageSetup.HeaderBorderStyle = (PrinterBorderStyle)((CboItem)cboPageItemBorderStyle.SelectedItem).ItemData;
                    break;
                case 5:
                    myPageSetup.BodyBorder = (PrinterLogTextBorder)cboPageItemBorder.SelectedIndex;
                    myPageSetup.BodyBorderStyle = (PrinterBorderStyle)((CboItem)cboPageItemBorderStyle.SelectedItem).ItemData;
                    break;
                case 6:
                    myPageSetup.TailBorder = (PrinterLogTextBorder)cboPageItemBorder.SelectedIndex;
                    myPageSetup.TailBorderStyle = (PrinterBorderStyle)((CboItem)cboPageItemBorderStyle.SelectedItem).ItemData;
                    break;
                case 7:
                    myPageSetup.PageFooterBorder = (PrinterLogTextBorder)cboPageItemBorder.SelectedIndex;
                    myPageSetup.PageFooterBorderStyle = (PrinterBorderStyle)((CboItem)cboPageItemBorderStyle.SelectedItem).ItemData;
                    break;
                default:
                    return;
            }
        }

        private void SetPictureFont(Font oFont)
        {
            if (oFont == null)
            {
                oFont = new Font("宋体", 9, picPageItemFont.Font.Style);
            }
            if (oFont.Size <= 6f)
                return;
            picPageItemFont.Tag = new Font(oFont.Name, oFont.Size, oFont.Style);
            if (cboPageItemBorder.Items.Count < 0) return;

            switch (((CboItem)cboPageItem.SelectedItem).ItemData)
            {
                case 1:
                    if (myPageSetup.PageHeaderFont == null)
                        myPageSetup.PageHeaderFont = new Font(oFont.Name, oFont.Size, oFont.Style);
                    if (!myPageSetup.PageHeaderFont.Equals(oFont))
                        myPageSetup.PageHeaderFont = new Font(oFont.Name, oFont.Size, oFont.Style);
                    break;
                case 2:
                    if (myPageSetup.MainTitleFont == null)
                        myPageSetup.MainTitleFont = new Font(oFont.Name, oFont.Size, oFont.Style);
                    if (!myPageSetup.MainTitleFont.Equals(oFont))
                        myPageSetup.MainTitleFont = new Font(oFont.Name, oFont.Size, oFont.Style);
                    break;
                case 3:
                    if (myPageSetup.SubTitleFont == null)
                        myPageSetup.SubTitleFont = new Font(oFont.Name, oFont.Size, oFont.Style);
                    if (!myPageSetup.SubTitleFont.Equals(oFont))
                        myPageSetup.SubTitleFont = new Font(oFont.Name, oFont.Size, oFont.Style);
                    break;
                case 4:
                    if (myPageSetup.HeaderFont == null)
                        myPageSetup.HeaderFont = new Font(oFont.Name, oFont.Size, oFont.Style);
                    if (!myPageSetup.HeaderFont.Equals(oFont))
                        myPageSetup.HeaderFont = new Font(oFont.Name, oFont.Size, oFont.Style);
                    break;
                case 5:
                    if (myPageSetup.BodyFont == null)
                        myPageSetup.BodyFont = new Font(oFont.Name, oFont.Size, oFont.Style);
                    if (!myPageSetup.BodyFont.Equals(oFont))
                        myPageSetup.BodyFont = new Font(oFont.Name, oFont.Size, oFont.Style);
                    break;
                case 6:
                    if (myPageSetup.TailFont == null)
                        myPageSetup.TailFont = new Font(oFont.Name, oFont.Size, oFont.Style);
                    if (!myPageSetup.TailFont.Equals(oFont))
                        myPageSetup.TailFont = new Font(oFont.Name, oFont.Size, oFont.Style);
                    break;
                case 7:
                    if (myPageSetup.PageFooterFont == null)
                        myPageSetup.PageFooterFont = new Font(oFont.Name, oFont.Size, oFont.Style);
                    if (!myPageSetup.PageFooterFont.Equals(oFont))
                        myPageSetup.PageFooterFont = new Font(oFont.Name, oFont.Size, oFont.Style);
                    break;
                default:
                    return;
            }
        }

        private void ShowPrinterCap(Graphics g)
        {
            float x, y;
            int xx, yy;
            int PgFmtWidth, PgFmtHeight;
            int PgFmtRealWidth, PgFmtRealHeight;
            int l, t, r, b;
            Color color = new Color();
            SolidBrush sb;
            string s;
            x = myPageSetup.ScaleWidth;
            y = myPageSetup.ScaleHeight;

            if (chkFullPage.Checked)
            {
                PgFmtWidth = (int)x;
                PgFmtHeight = (int)y;
            }
            else
            {
                PgFmtWidth = (int)(float.Parse(txtWidth.Text.Length >0?txtWidth.Text:"0") * PrintAssign.PixPerMM);
                PgFmtHeight = (int)(float.Parse(txtHeight.Text.Length >0?txtHeight.Text:"0") * PrintAssign.PixPerMM);
            }
            xx = 0;
            yy = 0;
            if (chkCenterH.Checked)
                l = r = (int)(5 / Ratio);
            else
            {
                l = (int)(float.Parse(txtLeft.Text.Length >0?txtLeft.Text:"0") * PrintAssign.PixPerMM);
                r = (int)(float.Parse(txtRight.Text.Length >0?txtRight.Text:"0") * PrintAssign.PixPerMM);
            }
            if (chkCenterV.Checked)
                t = b = (int)(5 / Ratio);
            else
            {
                if (txtTop.Text.Length <= 0)
                {
                    s = "0";
                }
                else
                    s = txtTop.Text;
                t = (int)(float.Parse(s) * PrintAssign.PixPerMM);
                if (txtBottom.Text.Length <= 0)
                {
                    s = "0";
                }
                else
                    s = txtBottom.Text;
                b = (int)(float.Parse(s) * PrintAssign.PixPerMM);
            }
            PgFmtRealWidth = PgFmtWidth - l - r;
            PgFmtRealHeight = PgFmtHeight - t - b;
            if (PgFmtRealWidth <= 0 || PgFmtRealHeight <= 0)
                return;
            g.ScaleTransform(Ratio, Ratio);
            while (true)
            {
                color = Color.FromArgb(255, 255, 0);
                sb = new SolidBrush(color);
                g.FillRectangle(sb, xx, yy, xx + PgFmtWidth, yy + PgFmtHeight);
                color = Color.FromArgb(255, 0, 0);
                g.DrawRectangle(new Pen(color), xx, yy,  PgFmtWidth ,  PgFmtHeight);
                color = Color.FromArgb(255, 255, 128);
                sb = new SolidBrush(color);
                g.FillRectangle(sb, xx + l, yy + t,  PgFmtRealWidth,  PgFmtRealHeight);
                color = Color.FromArgb(0, 128, 0);
                g.DrawRectangle(new Pen(color), xx + l, yy + t,  PgFmtRealWidth ,  PgFmtRealHeight);

                if (!chkAutoArrange.Checked) break;
                if (xx + 2 * PgFmtWidth <= x)
                    xx = (int)(xx + PgFmtWidth + (int)(1 / Ratio));
                else if (yy + 2 * PgFmtHeight <= y)
                {
                    xx = 0;
                    yy = (int)(yy + PgFmtHeight );
                }
                else
                    break;
            }
            g.ScaleTransform(1, 1);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            mTop = picPrintRange.Top;
            mLeft = picPrintRange.Left;
            mWidth = picPrintRange.Width;
            mHeight = picPrintRange.Height;
        }

        private void picPreview_Paint(object sender, PaintEventArgs e)
        {
            ShowPrinterCap(e.Graphics);
        }

        private void picPageItemFont_Paint(object sender, PaintEventArgs e)
        {
            const string Content = "远睿恒软件";
            Size size;
            float x, y;
            Font font = new Font("宋体", 9);
            SolidBrush sb = new SolidBrush(picPageItemColor.BackColor);
            font = (Font)((Font)picPageItemFont.Tag).Clone();
            size = TextRenderer.MeasureText(e.Graphics, Content, font);
            x = (float)((picPageItemFont.ClientRectangle.Width - size.Width) / 2);
            y = (float)((picPageItemFont.ClientRectangle.Height - size.Height) / 2);
            e.Graphics.DrawString(Content, font, sb, x, y);
        }

        private void Form_Activated(object sender, EventArgs e)
        {
            if (isFrist)
            {
                GetPrinterSetting();
                RefreshControlBoxState();
                cboPageItem.SelectedIndex = 0;
            }
            isFrist = false;
        }

        private void OptBody2_CheckChanged(object sender, EventArgs e)
        {
            RefreshControlBoxState();
        }

        private void OptBody1_CheckChanged(object sender, EventArgs e)
        {
            RefreshControlBoxState();
        }

        private void OptBody0_CheckChanged(object sender, EventArgs e)
        {
            RefreshControlBoxState();
        }

        private void chkZoom_CheckedChanged(object sender, EventArgs e)
        {
            RefreshControlBoxState();
        }

        private void picPageItemBorderColor_Click(object sender, EventArgs e)
        {
            Color c = picPageItemBorderColor.BackColor;
            ColorDialog colorDlg = new ColorDialog();
            DialogResult dlgResult = colorDlg.ShowDialog(this);
            if (dlgResult == DialogResult.OK)
            {
                c = colorDlg.Color;
                picPageItemBorderColor.BackColor = c;
                SetBorderColor(c.ToArgb() );
            }
        }

        private void picPageItemColor_Click(object sender, EventArgs e)
        {
            Color c = picPageItemColor.BackColor;
            ColorDialog colorDlg = new ColorDialog();
            DialogResult dlgResult = colorDlg.ShowDialog(this);
            if (dlgResult == DialogResult.OK)
            {
                c = colorDlg.Color;
                SetFontColor(c.ToArgb());
            }
        }

        private void chkBorderColor_CheckedChanged(object sender, EventArgs e)
        {
            picPageItemBorderColor.Enabled = chkBorderColor.Checked;
        }

        private void picPageItemFont_Click(object sender, EventArgs e)
        {
            Font oFont;
            Color color = picPageItemColor.BackColor;
            bool b = chkColor.Checked;
            GetPageItemFont(out   oFont, true);
            FontDialog fontDlg = new FontDialog();
            fontDlg.Font = oFont;
            fontDlg.ShowColor = b;
            fontDlg.Color = color;
            //fontDlg.ShowColor = true;
            DialogResult dlgResult = fontDlg.ShowDialog(this);
            if (dlgResult == DialogResult.OK)
            {
                color = fontDlg.Color;
                oFont = fontDlg.Font;
                SetPictureFont(oFont);
                if ((color.ToArgb() != picPageItemColor.BackColor.ToArgb()) && b)
                {
                    SetFontColor(color.ToArgb());
                }
                picPageItemFont.Refresh();
            }
        }

        private void chkColor_CheckedChanged(object sender, EventArgs e)
        {
            picPageItemColor.Visible  = chkColor.Checked;  
            label19.Visible = chkColor.Checked;  
        }

        private void chkAutoArrange_CheckedChanged(object sender, EventArgs e)
        {
            picPreview.Refresh(); 
        }

    }
}