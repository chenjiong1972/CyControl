
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

#region ���ڲ���λͳһʹ��0.01Ӣ��=1/4mm
/// <summary>
/// ���ڲ���λͳһʹ��0.01Ӣ��=1/4mm,��Ĭ�ϵ�λPrinterUnit.Display
/// ʹ�ô���༭���޸Ĵ˷��������ݡ�
/// </summary>
#endregion

namespace UnvaryingSagacity.Core.Printer
{
    public delegate void StartPageEventHandler(object sender, StartPageEventArgs e);
    public delegate void EndPageEventHandler(object sender, EndPageEventArgs e);
    public delegate void CustomPageEventHandler(object sender, CustomPageEventArgs e);
   
    public class PrintAssign
    {
        public const float TwipsPerMM = 56.7F; //ÿ����Twips
        public const float PixPerMM = 3.78F; //ÿ����Pix
        public const int SHEET_DEFAULTMAXCOL = 10;
        public const int SHEET_DEFAULTMAXROW = 30;
        public const int SHEET_DEFAULTROWHEIGHT = (int)(285 / TwipsPerMM);
        public const int SHEET_DEFAULTCOLWIDTH = (int)(1000 / TwipsPerMM);
        public const int SHEET_DEFAULTHDRWIDTH = (int)(800 / TwipsPerMM);
        public const int DOUBLELINES_H = 3;
        public const int DOUBLELINES_V = 3;
        public const int MINCYCHARS = 8;
        public const int TEXTBOX_OFFSET_H = 25;
        public const string CYTITLE = "ǧ��ʮ��ǧ��ʮ��ǧ��ʮ��ǧ��ʮ��ǧ��ʮ��ǧ��ʮԪ�Ƿ�";
        public const string CYTITLE_1 = "ǧ��ʮ ��ǧ�� ʮ��ǧ ��ʮ�� ǧ��ʮ ��ǧ�� ʮ��ǧ ��ʮԪ �Ƿ�";
        public const string OK_FLAG = "��";
        public event StartPageEventHandler StartPage;
        public event EndPageEventHandler EndCurrentPage;
        public event EventHandler SavePageSetup;
        public event CustomPageEventHandler CustomPage;

        internal PrintDocument printDoc;
        internal string printTitle;
        private PageSetup pageSetup;
        internal FrmPreview f;
        internal int phyNum = 0;

        private string userName = "";
        private int printDataIndex = 0;
        private PageArrange pageArrage;
        private PrintDatas printDatas = new PrintDatas();
        private IPrintDataFull printDataFull;
        private PrintExecutor printExecutor;
        private FrmProcess fProcess;
        private bool isPreview = true;
        private bool stopNow = false;

        public bool CustomMode { get; set; }

        public PrintAssign( PrinterUnit PrintUnit, string PrintTitle)
        {
            printDoc = new PrintDocument();
            pageSetup = new PageSetup( PrintUnit, printDoc);
            printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
            printDoc.BeginPrint += new PrintEventHandler(printDoc_BeginPrint);
            printTitle = PrintTitle;
            printDoc.DocumentName = printTitle;
            CustomMode = false;
            PrinterPageRanges = new PrinterPageRangesAndBeginPageNumber();
            fProcess = new FrmProcess(this);
        }

        public PrintAssign(PrinterUnit PrintUnit, string PrintTitle,PageSetup pageSetup)
        {
            printDoc = new PrintDocument();
            this.pageSetup = pageSetup;
            printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
            printDoc.BeginPrint += new PrintEventHandler(printDoc_BeginPrint);
            printTitle = PrintTitle;
            printDoc.DocumentName = printTitle;
            CustomMode = false;
            PrinterPageRanges = new PrinterPageRangesAndBeginPageNumber();
            fProcess = new FrmProcess(this);
        }

        internal PrinterPageRangesAndBeginPageNumber PrinterPageRanges { get; set; }

        /// <summary>
        /// ���Ҫ��ӡ��ҳ��Χ
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="allowBeginNumber">�Ƿ��������ÿ�ʼҳ��</param>
        /// <returns></returns>
        public static PrinterPageRangesAndBeginPageNumber ShowPrintPageRangeDialog(IWin32Window Parent,bool allowBeginNumber)
        {
            
            DlgPageRangeSetting f = new DlgPageRangeSetting();
            f.AllowBeginPageNumber = allowBeginNumber;
            if (f.ShowDialog(Parent) == DialogResult.OK)
            {
                PrinterPageRangesAndBeginPageNumber p = new PrinterPageRangesAndBeginPageNumber(f.BeginPageNumber, f.PageRanges);
                return p;
            }
            return default(PrinterPageRangesAndBeginPageNumber);
        }

        public bool ExistPrinter(string PrinterName)
        {
            return pageSetup.ExistPrinter(PrinterName);
        }

        public PrintDocument Document { get { return printDoc; } }

        public PageSetup PageSetup
        {
            get { return pageSetup; }
            set { pageSetup = value; }
        }

        public bool StopNow
        {
            get { return stopNow; }
            set { stopNow = value; }
        }

        public PrintDatas PrintDatas
        { get { return printDatas; } }

        public IPrintDataFull PrintDataProvider
        {
            get { return printDataFull; }
            set { printDataFull = value; }
        }

        public string PrintTitle
        {
            get { return printTitle; }
            set { printTitle = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        void printDoc_BeginPrint(object sender, PrintEventArgs e)
        {
            printDataIndex = 1;
        }

        void printDoc_EndPrint(object sender, PrintEventArgs e)
        {
            printExecutor = null;
            if (fProcess != null)
                fProcess.Close();
        }

        void printDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            bool cancel, endDoc, NewPhysicalPage;
            cancel = endDoc = NewPhysicalPage = false;
            if (pageSetup.Rotate90 && (!(sender as PrintDocument).PrintController.IsPreview) && pageSetup.Landscape)
            {
                //if (pageSetup.Landscape)//ֻ�Ժ���������.
                //{
                    e.Graphics.TranslateTransform(e.PageSettings.PrintableArea.X, -1 * (float)(e.PageBounds.Height));
                //}
                //else
                //{
                //    e.Graphics.TranslateTransform(e.PageSettings.PrintableArea.Y, -1 * (float)(e.PageBounds.Width));
                //}
                e.Graphics.RotateTransform(90F, System.Drawing.Drawing2D.MatrixOrder.Append);
            }
            if (CustomMode)
            {
                if (CustomPage != null)
                {
                    CustomPageEventArgs cpEvent = new CustomPageEventArgs();
                    CustomPage(this, cpEvent);
                    if (!cpEvent.Cancel && cpEvent.pageContent != null)
                    {
                        printDatas.Clear(); 
                        printDatas.Add(cpEvent.pageContent);
                        phyNum++;
                        printDatas.phyPageCount++;
                        printExecutor.CurrPrinter = e.Graphics;
                        printExecutor.NewPhyPage = true;
                        if (fProcess != null)
                            fProcess.PrinttingInfo(-1, 0, "���ڳ�ʼ����" + phyNum + "�����ݼ�...");
                        if (!printExecutor.InitTheAssign(pageArrage, 1))
                        {
                            fProcess.Close();
                            fProcess.Dispose();
                            fProcess = null;
                            e.Cancel = true;
                            e.HasMorePages = false;
                            return;
                        }
                        if (fProcess != null)
                            fProcess.progressBar1.Maximum = printDatas[0].LogicPageCount;
                        printExecutor.DrawPrintData(1, ref cancel, ref endDoc, ref NewPhysicalPage);
                    }
                    e.Cancel = cpEvent.Cancel;
                    e.HasMorePages = cpEvent.HasMorePages;
                    if (fProcess != null && !e.HasMorePages)
                    {
                        fProcess.Close();
                        fProcess.Dispose();
                        fProcess = null;
                    }
                    if (isPreview && f != null)
                        f.Activate();
                    return;
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }
            else
            {
                phyNum++;
                printDatas.phyPageCount++;
                printDatas.GetItem(printDataIndex).phyPageCount++;
                //�߼�ҳ������printExecutor.InitTheAssign ����
                printExecutor.CurrPrinter = e.Graphics;
                printExecutor.NewPhyPage = true;
                if (printExecutor.NewPrintData)
                {
                    if (fProcess != null)
                        fProcess.PrinttingInfo(-1, 0, "���ڳ�ʼ����" + printDataIndex + "�����ݼ�...");
                    printExecutor.InitTheAssign(pageArrage, printDataIndex);
                    if (fProcess != null)
                        fProcess.progressBar1.Maximum = printDatas.GetItem(printDataIndex).LogicPageCount;
                }

                for (int i = 1; i <= pageArrage.Count; i++)
                {
                    printExecutor.DrawPrintData(i, ref cancel, ref endDoc, ref NewPhysicalPage);
                    e.Cancel = cancel;

                    ///<��Ҫ���Ǵ�������ռ乻��ӡ��һ���߼�ҳʱ,��NewPrintData=true�����.>
                    /// if (printExecutor.FinalPrintData)
                    /// ��������ʱ�Դ��� ...
                    if (printExecutor.FinalPrintData)
                        break;
                    ///</��Ҫ���Ǵ�������ռ乻��ӡ��һ���߼�ҳʱ,��NewPrintData=true�����.>
                }

                if (printExecutor.FinalPrintData)
                {
                    printDataIndex++;
                    if (printDataIndex > printDatas.Count)
                        e.HasMorePages = false;
                    else
                        e.HasMorePages = true;
                }
                else
                    e.HasMorePages = true;
                if ((stopNow || endDoc))
                {
                    e.HasMorePages = false;
                }
                if (fProcess != null && !e.HasMorePages)
                {
                    fProcess.Close();
                    fProcess.Dispose();
                    fProcess = null;
                }
                if (isPreview && f != null)
                    f.Activate();
                return;
            }
        }

        protected virtual void OnStartPage(StartPageEventArgs e)
        {
            if (StartPage != null)
            {
                // Invokes the delegates. 
                StartPage(this, e);
            }
        }

        protected virtual void OnEndCurrentPage(EndPageEventArgs e)
        {
            if (EndCurrentPage != null)
            {
                // Invokes the delegates. 
                EndCurrentPage(this, e);
            }
        }

        internal void RasieStartPage(int PageNum, int PhysicalPageNum, int CurrPrintDataIndex, ref bool Cancel)
        {
            StartPageEventArgs e = new StartPageEventArgs(PageNum, PhysicalPageNum, CurrPrintDataIndex, Cancel);
            if (fProcess != null)
                fProcess.PrinttingInfo(PageNum, printDatas.GetItem(CurrPrintDataIndex).LogicPageCount, "���ڻ��Ƶ� " + PageNum + "/" + printDatas.GetItem(CurrPrintDataIndex).LogicPageCount + "ҳ ...");
            OnStartPage(e);
            Cancel = e.Cancel;
        }

        internal void RasieEndPage(int PageNum, int PhysicalPageNum, ref bool Cancel, ref bool EndDoc, ref bool EndCurrPrintData, ref bool NewPhysicalPage)
        {
            EndPageEventArgs e = new EndPageEventArgs(PageNum, PhysicalPageNum, Cancel, EndDoc, EndCurrPrintData, NewPhysicalPage);
            if (fProcess != null)
                fProcess.PrinttingInfo(-1, 0, "׼��������һҳ ...");
            OnEndCurrentPage(e);
            Cancel = e.Cancel;
            EndDoc = e.EndDoc;
            EndCurrPrintData = e.EndCurrPrintData;
            NewPhysicalPage = e.NewPhysicalPage;
        }

        public void Print()
        {
            if (!Init())
                return;
            if (!Core.General.Check())
                return;
            isPreview = false;
            Start();
        }

        /// <summary>
        /// ���Ĵ��벻֧�ָ÷���
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="IsShowPrintDlg"></param>
        /// <param name="allowSetBenginPage"></param>
        public void Print(IWin32Window owner, bool IsShowPrintDlg,bool allowSetBenginPage)
        {
            if (IsShowPrintDlg)
            {
                PrinterPageRangesAndBeginPageNumber p = ShowPrintPageRangeDialog(owner, allowSetBenginPage);
                if (p == null)
                    return;
                else
                    PrinterPageRanges = new PrinterPageRangesAndBeginPageNumber(p.BeginNumber, p.PrinterRanges);
            }
            Print();
        }
        public void ShowPreview(IWin32Window Parent)
        {
            if (!Init())
                return;
            if (f == null)
                f = new FrmPreview();
            f.DrawPrintDataOnShown = true;
            f.GetPrintDocument(this);
            if (Parent == null)
                f.Show();
            else
                f.Show(Parent);
        }

        public void ShowPreviewDialog(IWin32Window Parent)
        {
            if (!Init())
                return;
            if (f == null)
                f = new FrmPreview();
            f.DrawPrintDataOnShown = true;
            f.GetPrintDocument(this);
            if (Parent == null)
                f.ShowDialog();
            else
                f.ShowDialog(Parent);
        }

        public bool ShowPageSetupDialog(IWin32Window Parent)
        {
            DlgPageSetup f = new DlgPageSetup(this);
            if (Parent == null)
                f.ShowDialog();
            else
                f.ShowDialog(Parent);

            bool b = f.pageSetupChanged;
            if (b && SavePageSetup!=null )
                SavePageSetup(this, new EventArgs());
            return b;
        }

        /// <summary>
        /// �������ҳ�������ɵ����ĺ���, ��������PrintData
        /// ��������Ҫ�Ǳ������ݲ������.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public int RowsPrePhyPage(PrintData p)
        {
            PageSetup oPage = PageSetup;
            if (oPage.SheetBodyStyle==PrinterBodyStyle.�������� )
            {
                return oPage.Rows;
            }
            float h = 0;
            #region ��Ǳ���ĸ߶�
            foreach (AttachRow row in p.PageHeaders )
            {
                h += (GetFontHeight(oPage.PageHeaderFont , PrintExecutor.conStandChar) + row.Offset_V);
            }
            foreach (AttachRow row in p.PageFoots )
            {
                h += (GetFontHeight(oPage.PageFooterFont , PrintExecutor.conStandChar) + row.Offset_V);
            }
            foreach (AttachRow row in p.MainTitle)
            {
                h += (GetFontHeight(oPage.MainTitleFont, PrintExecutor.conStandChar) + row.Offset_V);
            }
            foreach (AttachRow row in p.SubTitles)
            {
                h += (GetFontHeight(oPage.SubTitleFont , PrintExecutor.conStandChar) + row.Offset_V);
            }
            foreach (AttachRow row in p.Headers )
            {
                h += (GetFontHeight(oPage.HeaderFont  , PrintExecutor.conStandChar) + row.Offset_V);
            }
            foreach (AttachRow row in p.Tails)
            {
                h += (GetFontHeight(oPage.TailFont , PrintExecutor.conStandChar) + row.Offset_V);
            }
            h += p.Body.Offset_V;
            if (oPage.LogPicPrint && !(oPage.LogPic == null))
            {
                if (oPage.LogPicLoc == PrinterLogPictureLocation.��������)
                    h += (float)oPage.GetAttributes(PageSetupKey.ePage_ͼ���ӡ�߶�);
            }
            #endregion
            #region �����ڱ���ĸ߶�
            h = oPage.ScaleHeight - h - oPage.TopMargin - oPage.BottomMargin;
            #endregion
            int rowHeight = TextRenderer.MeasureText(PrintExecutor.conStandChar, oPage.BodyFont).Height;
            if (oPage.SheetBodyStyle == PrinterBodyStyle.�����и�)
            {
                rowHeight = oPage.RowHeight;
            }
            else 
            {
                for (int i = 0; i < p.TopFixedRows; i++)
                {
                    h -= p.Rows[i].Height;
                }
                for (int i = 0; i < p.BottomFixedRows ; i++)
                {
                    h -= p.Rows[p.Rows.Count - i - 1].Height;
                }
                rowHeight = p.Rows[p.TopFixedRows].Height; 
            }
            float rows = h / rowHeight;
            return (int)rows;
        }

        int GetFontHeight(Font font,string text)
        {
            if (font == null)
                return 0;
            return TextRenderer.MeasureText(text,font).Height;
        }

        internal bool Preview(FrmPreview f)
        {
            System.Drawing.Printing.PreviewPrintController pControl = new System.Drawing.Printing.PreviewPrintController();
            printDoc.PrintController = pControl;
            isPreview = true;
            Start();
            return true;
        }

        ///<����Printting�Ĺ�����FrmProcess.Form_Activate�д���>
        /// Ϊ����ʾ��ӡ����,ʼ����ʾFrmProcess
        ///</����Printting�Ĺ�����FrmProcess.Form_Activate�д���>
        internal bool Start()
        {
            if (fProcess == null)
                fProcess = new FrmProcess(this);
            fProcess.progressBar1.Visible = false;
            if (isPreview)
            {
                if (f == null)
                    fProcess.ShowDialog();
                else
                    fProcess.ShowDialog(f);
            }
            else
                fProcess.ShowDialog();
            return true;
        }

        internal bool Printting()
        {
            
            if (PrintDataProvider != null)
            {
                PrinterDefineRowsInPage p = new PrinterDefineRowsInPage();
                if (printDataFull.Init(out  p))
                {
                    printDataFull.FullPrintData( printExecutor.RowsInPage(p), printDatas);
                }
            }
            if (printDatas.Count > 0 || CustomMode)
            {
                /// <��ӡһ��������Ĵ�������>
                ///1:����ҳ�����ù������PrintExecutor
                ///2:��ʼ��printDataIndex;pageArrageIndex;StopNow;printDatas.phyPageCount,logicPageCount
                ///3:ȷ������printDoc_BeginPrint
                /// </��ӡһ��������Ĵ�������>
                if (printExecutor != null)
                    printExecutor = null;
                printExecutor = new PrintExecutor(this);
                if (!InitAssign())
                    return false;
                if (fProcess != null)
                {
                    fProcess.progressBar1.Value = 0;
                    fProcess.progressBar1.Maximum = 100;//��ʱ�Դ���,׼ȷֵ��InitPageInPrintData������
                    fProcess.progressBar1.Visible = true;
                }
                printDatas.phyPageCount = 0;
                printDatas.logicPageCount = 0;
                foreach (PrintData pd in printDatas)
                {
                    pd.phyPageCount = 0;
                    pd.logicPageCount = 0;
                }
                printDataIndex = 0;
                StopNow = false;
                if (!isPreview)
                    printDoc.Print();
                else
                    f.InvalidatePreview();
                return true;
            }
            else
                return false;
        }

        private bool Init()
        {
            try
            {
                if (!printDoc.PrinterSettings.IsValid)
                {
                    MessageBox.Show("ϵͳû�п��õĴ�ӡ��, ���Ȱ�װ��ӡ��", "ȱ�ٴ�ӡ��", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            catch (InvalidPrinterException ipEx)
            {
                MessageBox.Show("ϵͳû�п��õĴ�ӡ��, ���Ȱ�װ��ӡ��", "ȱ�ٴ�ӡ��", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message , "����ӡ��", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (!ExistPrinter(pageSetup.PrintName))
            {
                string s = printDoc.DefaultPageSettings.PrinterSettings.PrinterName + "\n\n" + printDoc.DefaultPageSettings.PaperSize.PaperName;
                DialogResult r = MessageBox.Show("��ӡ��: " + pageSetup.PrintName + " ������, ��ʹ��ȱʡ��ӡ����ֽ��.\n\n" + s, "��ӡ��������", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.No)
                    return false;
            }
            return true;
        }

        internal bool InitAssign()
        {
            //���ô�ӡ��,ֽ��,����ֽ�ŷ���
            if (fProcess != null)
                fProcess.PrinttingInfo(-1, 0, "���ô�ӡ��,ֽ��,����ֽ�ŷ��� ...");
            if (ExistPrinter(pageSetup.PrintName))
            {
                System.Drawing.Printing.PaperSize pSize = new System.Drawing.Printing.PaperSize(pageSetup.PaperName, (int)pageSetup.PageWidth, (int)pageSetup.PageHeight);
                pSize.RawKind = pageSetup.PaperSize;

                printDoc.DefaultPageSettings.PrinterSettings.PrinterName = pageSetup.PrintName;
                printDoc.DefaultPageSettings.PaperSize = pSize;
                //printDoc.DefaultPageSettings.Landscape = pageSetup.Landscape;//����Ҫ���д���, ������ֽ�ĳߴ綨��ʱ��ȷ��,�Ժ����PaperWidth��PaperHeightʱ�ٻָ�
                pageArrage = pageSetup.PageArrange(printDoc.PrintController.IsPreview );//pageArrage�ĵ�0��Ԫ�ز�ʹ��
            }
            else
            {
                //ǰ���Ѿ���ʾ����,
                pageArrage = pageSetup.PageArrange(printDoc.DefaultPageSettings.PrintableArea.Location, new Size((int)printDoc.DefaultPageSettings.PrintableArea.Size.Width, (int)printDoc.DefaultPageSettings.PrintableArea.Size.Height), new Size((int)pageSetup.Width, (int)pageSetup.Height),printDoc.PrintController.IsPreview);
            }
            PrintDatas.phyPageCount = 0;
            PrintDatas.logicPageCount = 0;
            phyNum = 0;
            return true;
        }
    }

    internal class PrintExecutor
    {
        public const string conStandChar = "��";
        internal Graphics CurrPrinter;
        int CurrentX = 0;
        int CurrentY = 0;
        int IndexRow = 0;
        int IndexCol = 0;
        int currPageNum=0;
        int printDataIndex;
        bool newPhyPage=true ;
        bool mBodyIncludeFixRange=false ;  //�����Ƿ�����̶����в���
        bool nextData = true;
        bool finalData = true;

        PrintAssign printAssign;
        PrintData printData;
        Font backupFont = new Font("����", 9);
        Font currFont = new Font("����", 9);
        Color currColor = Color.Black;
        Font[] mKeepFont;
        SolidBrush textBrush = new SolidBrush(Color.Black);
        Pen currPen = new Pen(Color.Black);
        PrinterBound[] mCols=new PrinterBound [1];
        PrinterBound[] mRows = new PrinterBound[1];
        PageSetup oPage;
        PageArrange pageArrange;
        PrinterArrange printArrange;
        float RealDataBodyHeight=0;

        public PrintExecutor(PrintAssign p)
        {
            printAssign = p;
            oPage = printAssign.PageSetup;
        }

        public PrintData PrintData
        {
            get { return printData; }
        }

        public bool NewPhyPage
        {
            get { return newPhyPage; }
            set { newPhyPage = value; }
        }

        public bool NewPrintData
        {
            get { return nextData; }
        }

        public bool FinalPrintData
        {
            get { return finalData; }
        }

        /// <summary>
        /// ����,��PrintAssign.RowsPrePhyPage����
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal int RowsInPage(PrinterDefineRowsInPage p)
        {
            PageSetup oPage = printAssign.PageSetup;
            float h = 0;
            float linespace = 0;

            if (oPage.PrintUnit == PrinterUnit.Twips)
                linespace = TwipToPixl(p.BodyLinespace);
            else
                linespace = p.BodyLinespace;
            h = p.PageHeaderRows * (GetFontHeight(oPage.PageHeaderFont, conStandChar) + linespace);
            h = h + p.PageFootRows * (GetFontHeight(oPage.PageFooterFont, conStandChar) + linespace);
            h = h + p.MainTitleRows * (GetFontHeight(oPage.MainTitleFont, conStandChar) + linespace);
            h = h + p.SubTitleRows * (GetFontHeight(oPage.SubTitleFont, conStandChar) + linespace);
            h = h + p.HeaderRows * (GetFontHeight(oPage.HeaderFont, conStandChar) + linespace);
            h = h + p.TailRows * (GetFontHeight(oPage.TailFont, conStandChar) + linespace);
            h = h + (p.TopFixRows + p.BottomFixRows) * (GetFontHeight(oPage.BodyFont, conStandChar) + linespace);
            h = h + GetHeightLogImage();
            h = (float)(oPage.ScaleHeight - h - oPage.TopMargin - oPage.BottomMargin) / GetFontHeight(oPage.BodyFont, conStandChar);
            return (int)h;
        }

        internal bool InitTheAssign(PageArrange PageArrange, int PrintDataIndex)
        {
            //������߼�ҳ��������ҳ��,���ۼƵ����ݼ����߼�ҳ��������ҳ��
            float h;
            finalData = false;
            nextData = false;
            currPageNum = 1;
            IndexRow = 0;
            IndexCol = 0;
            pageArrange = PageArrange;
            printDataIndex = 1;// PrintDataIndex;
            printData = printAssign.PrintDatas.GetItem(printDataIndex);
            currPen.Width = float.Parse(  oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());
            mBodyIncludeFixRange = (bool)oPage.GetAttributes(PageSetupKey.ePage_������������̶�����);
            RealDataBodyHeight = GetRealSheetBodyHeight();
            PrintData.logicPageCount = GetPrintPages(ref mRows, ref mCols);
            if (PrintData.logicPageCount <= 0)
                return false;
            h = (float)(PrintData.logicPageCount / PageArrange.Count);
            printAssign.PrintDatas.logicPageCount = printAssign.PrintDatas.logicPageCount + PrintData.logicPageCount;
            if ((bool )oPage.KeepFont)
                KeepSameForSheetBodyFont();
            //ˢ�¹̶��ı�β,���p/nҳ��
            RefreshSysSheetTail(currPageNum, printData.LogicPageCount, true);
            return true;
        }

        internal bool DrawPrintData(int PrintArrangeIndex ,ref bool Cancel, ref bool EndDoc,  ref bool NewPhysicalPage)
        {
            //������Ϣ->StartPage,���ݷ���ֵ����Ӧ����
            //����һ���߼�ҳ
            //������Ϣ->EndPage,���ݷ���ֵ����Ӧ����
            //����Ƿ���굱ǰ������,�����finalData=true
            //CurrPrinter.DrawString(printData.Key, printAssign.PageSetup.MainTitleFont, Brushes.Blue, 100, 100);
            bool endData = false;

            printArrange = pageArrange.Arrange(PrintArrangeIndex);
            printAssign.RasieStartPage(currPageNum, printAssign.phyNum, printDataIndex, ref Cancel);
            if (NewPhyPage)
            {
                //�µ�����ҳ�Ż���ҳü,ҳ��
                CurrentY = (int)oPage.HeaderMargin;
                CurrentX = (int)oPage.LeftMargin;
                DrawAttachRows(printData.PageHeaders, AttachRowType.ҳü);
                CurrentY = (int)(oPage.PageHeight - oPage.FooterMargin);
                DrawAttachRows(printData.PageFoots, AttachRowType.ҳ��);
                NewPhyPage = false;
            }
            RefreshSysSheetTail(currPageNum, PrintData.LogicPageCount, false);
            if (oPage.CenterVert)
                CurrentY =(int)( printArrange.Location.Y + (printArrange.Size.Height - GetRealRowHeightByRowBound(mRows[ IndexRow])) / 2);
            else
                CurrentY = (int)(printArrange.Location.Y + oPage.TopMargin );

            if (oPage.CenterHoriz)
                CurrentX =(int)(  (printArrange.Size.Width - GetRealColWidthByColBound(mCols[ IndexCol])) / 2);
            else
                CurrentX = (int)(oPage.LeftMargin  + GetAttachTextWidth(FixColPos.��̶���));
            if (oPage.CutLine)
                DrawCutline();
            DrawLogText();
            if (oPage.LogPicLoc != PrinterLogPictureLocation.��������)
                DrawLogPicture( 0, 0);
            DrawOneLogicPage(RealDataBodyHeight,  mRows, mCols ,  currPageNum, printAssign.phyNum);
            //��ҳ
            IndexCol++;
            if (IndexCol >= mCols.Length )
            {
                IndexRow++;
                if (IndexRow >= mRows.Length)
                {
                    finalData = true;
                    IndexCol = 0;
                    IndexRow = 0;
                }
                else
                {
                    IndexCol = 0;
                    finalData = false;
                }
            }
            else
                finalData = false;
            endData = finalData;
            EndDoc=finalData;
            NewPhysicalPage = PrintArrangeIndex >= pageArrange.Count ? true : false;
            printAssign.RasieEndPage(currPageNum, printAssign.phyNum, ref Cancel, ref  EndDoc, ref  endData, ref  NewPhysicalPage);
            newPhyPage = NewPhysicalPage;
            currPageNum++;
            return true;
        }
        private void DrawOneLogicPage(float SheetBodyHeight, PrinterBound[] Rows, PrinterBound[] Cols, int PageNum, int PhyPageNum)
        {
            PrinterTextAlign CellPos;
            PrinterCell typCell;
            Range oMerg;
            float h=0, w=0, X = 0, Y = 0, FixedRowsHeight;
            int i, j, CurrRow = 0, StartCol = 0, EndCol, StartRow, EndRow;
            bool b;
            CurrentX = printArrange.Location.X+(int)oPage.LeftMargin  ;
            CurrentY = printArrange.Location.Y + (int)oPage.TopMargin;
            //hΪͳ�ƴ�ӡ�ĸ߶�
            if (oPage.CenterHoriz)
            {
                h = DrawAttachRows(printData.MainTitle, AttachRowType.������);
                CurrentY = CurrentY + (int)h;
                h = DrawAttachRows(printData.SubTitles, AttachRowType.������);
                CurrentY = CurrentY + (int)h;
                X = CurrentX;
                CurrentX = CurrentX - (printArrange.Size.Width - (int)GetRealColWidthByColBound(Cols[IndexCol])) / 2;
                if (oPage.LogPicLoc == PrinterLogPictureLocation.��������)
                {
                    //������������ʱ��DoPrintToVsPrint�д���
                    h = DrawLogPicture(CurrentX + ((printArrange.Size.Width - (int)oPage.GetAttributes(PageSetupKey.ePage_ͼ���ӡ���)) / 2), CurrentY);
                    CurrentY = CurrentY + (int)h;
                }
                CurrentX = (int)X;
                h = DrawAttachRows(printData.Headers, AttachRowType.��ͷ);
                CurrentY = CurrentY + (int)h + printData.Body.Offset_V;
            }
            else
            {
                h = DrawAttachRows(printData.MainTitle, AttachRowType.������);
                CurrentY = CurrentY + (int)h;
                h = DrawAttachRows(printData.SubTitles, AttachRowType.������);
                CurrentY = CurrentY + (int)h;
                if (oPage.LogPicLoc == PrinterLogPictureLocation.��������)
                {
                    //������������ʱ��DoPrintToVsPrint�д���
                    h = DrawLogPicture((int)(X + ((pageArrange.Arrange(1).Size.Width - (int)oPage.GetAttributes(PageSetupKey.ePage_ͼ���ӡ���)) / 2)), CurrentY);
                    CurrentY = CurrentY + (int)h;
                }
                h = DrawAttachRows(printData.Headers, AttachRowType.��ͷ);
                CurrentY = CurrentY + (int)h + printData.Body.Offset_V;
            }
            StartCol = Cols[IndexCol].Start;
            EndCol = Cols[IndexCol].End;
            StartRow = Rows[IndexRow].Start;
            EndRow = Rows[IndexRow].End;
            if (oPage.KeepFont)
                SetFont(mKeepFont[0]);
            else
            {
                printData.Body.DefaultFont = (Font)oPage.BodyFont.Clone();
                SetFont(printData.Body.DefaultFont);
            }
            //�̶�������
            DrawTopFixedRows(StartCol, EndCol, CurrentX, CurrentY);
            FixedRowsHeight = GetFixedRowsHeight(FixRowPos.ȫ���̶���);
            //�������
            h = 0;
            b = false;
            for (i = StartRow; i <= EndRow; i++)
            {
                CurrRow = i;
                h = h + printData.Rows.GetItem(i).PrintHeight;
                w = 0;
                for (j = StartCol; j <= EndCol; j++)
                {
                    CalcRectOfPictureItem(i, j, (int)(CurrentX + w), (int)(CurrentY + h + FixedRowsHeight - printData.Rows.GetItem(i).PrintHeight));
                    w = w + printData.Cols.GetItem(j).PrintWidth;
                    if (printData.InMergedRanges(i, j, out CellPos, out oMerg))
                    {
                        if (CellPos == PrinterTextAlign.LeftTop)
                            DrawMergedRowCol(i, j, EndRow, EndCol, CurrentX + w - printData.Cols.GetItem(j).PrintWidth + GetFixedColsWidth(FixColPos.��̶���), CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���) - printData.Rows.GetItem(i).PrintHeight, 0, oMerg);
                    }
                    else
                    {
                        typCell = printData.Body.GetItem("$" + i+ "$" + j);
                        //����X,Y
                        GetPointByCell(CurrentX, CurrentY, StartRow, StartCol, i, j, out X, out Y, true);
                        float w1 , h1  ;
                        w1 = printData.Cols.GetItem(j).PrintWidth;
                        h1 = printData.Rows.GetItem(i).PrintHeight;
                        DrawCellContent(typCell, X, Y, ref w1, ref h1, AttachRowType.����, i, j);
                        printData.Cols.GetItem(j).PrintWidth = w1;
                        printData.Rows.GetItem(i).PrintHeight = h1;
                        b = false;
                        if (oPage.DblLineRows > 0)
                        {
                            if ((i % oPage.DblLineRows) == 0)
                                b = true;
                        }
                        DrawBorder(typCell, X, Y, printData.Cols.GetItem(j).PrintWidth, printData.Rows.GetItem(i).PrintHeight, AttachRowType.����, b, GetCellPos(i, j, StartRow - printData.TopFixedRows, StartCol - printData.LeftFixedCols, EndRow + printData.BottomFixedRows, EndCol + printData.RightFixedCols), i, j);
                    }
                }
            }
            //�ж��Ƿ�ӿ���
            i = 0;
            if (oPage.FillBlankLines)
            {
                if (oPage.SheetBodyStyle == PrinterBodyStyle.�����и�)
                {
                    i = (int)((SheetBodyHeight - GetFixedRowsHeight(0) - h) / oPage.RowHeight);
                    for (j = 1; j <= i; j++)
                    {
                        if (j == i)
                        {
                            if (printData.BottomFixedRows > 0)
                                h = h + DrawBlankRows(CurrentX + GetFixedColsWidth(FixColPos.��̶���), CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���), CurrRow, StartCol, EndCol, false);
                            else
                                //��ֹ��˫�߱߿�ʱ�߲�����
                                h = h + DrawBlankRows(CurrentX + GetFixedColsWidth(FixColPos.��̶���), CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���), CurrRow, StartCol, EndCol, true);
                        }
                        else
                            h = h + DrawBlankRows(CurrentX + GetFixedColsWidth(FixColPos.��̶���), CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���), CurrRow, StartCol, EndCol, false);
                    }
                }
                else if (oPage.SheetBodyStyle == PrinterBodyStyle.��������)
                {
                    ///*���ж��Ƿ��ܴ����,ǿ�ƴ���ȷ������Ϊֹ
                    i = oPage.Rows - (EndRow - StartRow + 1);
                    for (j = 1; j <= i; j++)
                    {
                        if (j == i)
                        {
                            if (printData.BottomFixedRows > 0)
                                h = h + DrawBlankRows(CurrentX + GetFixedColsWidth(FixColPos.��̶���), CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���), CurrRow, StartCol, EndCol, false);
                            else
                                //��ֹ��˫�߱߿�ʱ�߲�����
                                h = h + DrawBlankRows(CurrentX + GetFixedColsWidth(FixColPos.��̶���), CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���), CurrRow, StartCol, EndCol, true);
                        }
                        else
                            h = h + DrawBlankRows(CurrentX + GetFixedColsWidth(FixColPos.��̶���), CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���), CurrRow, StartCol, EndCol, false);
                    }
                    //*/
                }
                else if (oPage.SheetBodyStyle == PrinterBodyStyle.�Զ� )
                {
                    i = (int)((SheetBodyHeight - GetFixedRowsHeight(0) - h) / printData.Rows[CurrRow-1].PrintHeight);
                    for (j = 1; j <= i; j++)
                    {
                        if (j == i)
                        {
                            if (printData.BottomFixedRows > 0)
                                h = h + DrawBlankRows(CurrentX + GetFixedColsWidth(FixColPos.��̶���), CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���), CurrRow, StartCol, EndCol, false);
                            else
                                //��ֹ��˫�߱߿�ʱ�߲�����
                                h = h + DrawBlankRows(CurrentX + GetFixedColsWidth(FixColPos.��̶���), CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���), CurrRow, StartCol, EndCol, true);
                        }
                        else
                            h = h + DrawBlankRows(CurrentX + GetFixedColsWidth(FixColPos.��̶���), CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���), CurrRow, StartCol, EndCol, false);
                    }
                }
            }
            
            //�̶�����
            DrawLeftFixedCols(StartRow, EndRow, CurrentX, CurrentY + GetFixedRowsHeight(FixRowPos.���̶���), i);
            DrawRightFixedCols(StartRow, EndRow, CurrentX + w + GetFixedColsWidth(FixColPos.��̶���), CurrentY + GetFixedRowsHeight(FixRowPos.���̶���), i);
            h = h + DrawBottomFixedRows(StartCol, EndCol, CurrentX, CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���));
            //�ĽǵĹ̶�����
            DrawFixedRowColInCorner(CurrentX, CurrentY, w, h, 1); //��
            DrawFixedRowColInCorner(CurrentX + w + GetFixedColsWidth(FixColPos.��̶���), CurrentY, w, h, 2);  //�Ҷ�
            DrawFixedRowColInCorner(CurrentX, CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���) - GetFixedRowsHeight(FixRowPos.�׹̶���), w, h, 3); //���
            DrawFixedRowColInCorner(CurrentX + w + GetFixedColsWidth(FixColPos.��̶���), CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���) - GetFixedRowsHeight(FixRowPos.�׹̶���), w, h, 4); //�ҵ�
            ///*��˫����߿�
            if ((bool)(oPage.GetAttributes(PageSetupKey.ePage_����߿�Ϊ˫��)))
            {
                //��
                CurrPrinter.DrawLine(currPen, CurrentX - PrintAssign.DOUBLELINES_H / 2, CurrentY - PrintAssign.DOUBLELINES_V / 2, CurrentX + w + GetFixedColsWidth(FixColPos.��̶���) + PrintAssign.DOUBLELINES_H / 2, CurrentY - PrintAssign.DOUBLELINES_V / 2);
                //��
                CurrPrinter.DrawLine(currPen, CurrentX - PrintAssign.DOUBLELINES_H / 2, CurrentY - PrintAssign.DOUBLELINES_V / 2, CurrentX - PrintAssign.DOUBLELINES_H / 2, CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���) + PrintAssign.DOUBLELINES_V / 2);
                //��
                CurrPrinter.DrawLine(currPen, CurrentX + w + GetFixedColsWidth(FixColPos.��̶���) + PrintAssign.DOUBLELINES_H / 2, CurrentY - PrintAssign.DOUBLELINES_V / 2,
                                     CurrentX + w + GetFixedColsWidth(FixColPos.��̶���) + PrintAssign.DOUBLELINES_H / 2, CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���) + PrintAssign.DOUBLELINES_V / 2);
                //�ҵ�
                CurrPrinter.DrawLine(currPen, CurrentX - PrintAssign.DOUBLELINES_H / 2, CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���) + PrintAssign.DOUBLELINES_V / 2,
                                     CurrentX + w + GetFixedColsWidth(FixColPos.��̶���) + PrintAssign.DOUBLELINES_H / 2, CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���) + PrintAssign.DOUBLELINES_V / 2);
            }
            //*/
            //�������Ҹ�������
            PrinterAttachCell tCell = new PrinterAttachCell();
            float tmpX;
            X = CurrentX - GetAttachTextWidth(FixColPos.��̶���);
            for (i = 1; i <= printData.LeftAttachText.Count; i++)
            {
                Y = CurrentY;
                tmpX = X;
                for (j = 1; j <= printData.LeftAttachText.TheAttachRow(i).Count; j++)
                {
                    tCell=printData.LeftAttachText.TheAttachRow(i).AttachCell(j); 
                    if (tCell.Properties.Value !=null  )
                    {
                        X = tmpX;
                        tCell.Height = (int)(h + GetFixedRowsHeight(FixRowPos.���̶���));
                        Y = Y + DrawAttachItem(tCell,ref X, Y, AttachRowType.����);
                    }
                }
                X = X + printData.LeftAttachText.TheAttachRow(i).Offset_V;
            }
            X = CurrentX + w + GetFixedColsWidth(FixColPos.��̶���);
            for (i = 1; i <= printData.RightAttachText.Count; i++)
            {
                X = X + printData.RightAttachText.TheAttachRow(i).Offset_V;
                Y = CurrentY;
                tmpX = X;
                for (j = 1; j <= printData.RightAttachText.TheAttachRow(i).Count; j++)
                {
                    tCell = printData.RightAttachText.TheAttachRow(i).AttachCell(j);
                    if (tCell.Properties.Value != null)
                    {
                        X = tmpX;
                        tCell.Height = (int)(h + GetFixedRowsHeight(FixRowPos.���̶���));
                        Y = Y + DrawAttachItem(tCell, ref X, Y, AttachRowType.����);
                    }
                }
            }
            //��β
            CurrentY = (int)(CurrentY + h + GetFixedRowsHeight(FixRowPos.���̶���));
            h = DrawAttachRows(printData.Tails, AttachRowType.��β);
            //��ͼ
            DrawPictures(StartRow, StartCol, EndRow, EndCol);
        }

        /// <summary>
        /// ���ݽ���ߵ�����ַ������ؽ������Ŀ��, ������ʱ����0
        /// </summary>
        /// <returns></returns>
        private float GetCyColMaxWidth()
        {
            int maxChars = this.printAssign.PageSetup.MaxCharsOfCyCell;
            if (maxChars > 0)
            {
                float fw = GetFontWidth(currFont, "0123456789") / 10;
                float spacingWidth = 2 + fw;
                float width = PrintAssign.DOUBLELINES_H + spacingWidth * maxChars;
                return width;
            }
            else
                return 0f;
        }

        private bool AdjustColWidthSize(float RealWidth)
        {
            int i;
            float h = 0; float h1 = 0; float ScaleRate;
            float cyWidth = GetCyColMaxWidth(); 

            if (oPage.Zoom == 1)
            {
                if ((int)oPage.GetAttributes(PageSetupKey.ePage_�Զ�����ģʽ) == 0 || (int)oPage.GetAttributes(PageSetupKey.ePage_�Զ�����ģʽ) == 2)
                {
                    RealWidth = RealWidth - GetFixedColsWidth(FixColPos.ȫ���̶���);
                    h = 0;
                    for (i = 1; i <= printData.Cols.Count; i++)
                    {
                        if (!printData.InFixedRanges(-1, i))
                        {
                            PrinterCell cell=printData.Body.GetItem("$2$"+i);
                            if (cell.Behave == PrinterCellBehave.����� || cell.Behave == PrinterCellBehave.����߱���)
                            {
                                h += (cyWidth <= 0 ? printData.Cols.GetItem(i).Width : cyWidth);
                            }
                            else
                                h = h + printData.Cols.GetItem(i).Width;
                        }
                    }
                }
                else
                    h = RealWidth;
                if (oPage.ReduceMethod == 2)
                { //��ֵ����
                    h1 = h - RealWidth;
                    h1 = h1 / (printData.Cols.Count - printData.LeftFixedCols - printData.RightFixedCols);
                    for (i = 1; i <= printData.Cols.Count; i++)
                    {
                        if (!printData.InFixedRanges(-1, i))
                        {
                            PrinterCell cell = printData.Body.GetItem("$2$" + i);
                            if (cell.Behave == PrinterCellBehave.����� || cell.Behave == PrinterCellBehave.����߱���)
                            {
                                printData.Cols.GetItem(i).PrintWidth = (cyWidth <= 0 ? (printData.Cols.GetItem(i).Width - h1) : cyWidth);
                            }
                            else
                            {
                                printData.Cols.GetItem(i).PrintWidth = printData.Cols.GetItem(i).Width - h1;
                                if (printData.Cols.GetItem(i).PrintWidth < 1)
                                {
                                    MessageBox.Show("��" + i.ToString() + "�е��п���С�󳬳���ӡ��Χ, ���ȵ������п�.", "������ӡ�п�", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return false;
                                }
                            }
                        }
                    }
                    //*/
                }
                else
                {//�ȱ�����
                    ScaleRate = RealWidth / h;
                    for (i = 1; i <= printData.Cols.Count; i++)
                    {
                        if (!printData.InFixedRanges(-1, i))
                        {
                            PrinterCell cell = printData.Body.GetItem("$2$" + i);
                            if (cell.Behave == PrinterCellBehave.����� || cell.Behave == PrinterCellBehave.����߱���)
                            {
                                printData.Cols.GetItem(i).PrintWidth = (cyWidth <= 0 ? (printData.Cols.GetItem(i).Width * ScaleRate) : cyWidth);
                            }
                            else
                            {
                                printData.Cols.GetItem(i).PrintWidth = printData.Cols.GetItem(i).Width * ScaleRate;
                                if (printData.Cols.GetItem(i).PrintWidth < 1)
                                {
                                    MessageBox.Show("��" + i.ToString() + "�е��п���С�󳬳���ӡ��Χ, ���ȵ������п�.", "������ӡ�п�", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (i = 1; i <= printData.Cols.Count; i++)
                {
                    if (!printData.InFixedRanges(-1, i))
                    {
                        PrinterCell cell = printData.Body.GetItem("$2$" + i);
                        if (cell.Behave == PrinterCellBehave.����� || cell.Behave == PrinterCellBehave.����߱���)
                        {
                            printData.Cols.GetItem(i).PrintWidth = (cyWidth <= 0 ? printData.Cols.GetItem(i).Width : cyWidth);
                        }
                        else
                        {
                            printData.Cols.GetItem(i).PrintWidth = printData.Cols.GetItem(i).Width;
                            if (printData.Cols.GetItem(i).PrintWidth < 1)
                            {
                                MessageBox.Show("��" + i.ToString() + "�е��п�̫С, ���ȵ������п�.", "������ӡ�п�", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private bool AdjustRowHeightSize(float RealHeight)
        {
            int i; float h;
            if (oPage.Zoom == 1)
            {
                RealHeight = RealHeight - GetFixedRowsHeight(FixRowPos.ȫ���̶���);
                h = 0;
                if ((int)oPage.GetAttributes(PageSetupKey.ePage_�Զ�����ģʽ) == 0 || (int)oPage.GetAttributes(PageSetupKey.ePage_�Զ�����ģʽ) == 1)
                {
                    for (i = 1; i <= printData.Rows.Count; i++)
                    {
                        if (!printData.InFixedRanges(i, -1))
                            h = h + printData.Rows.GetItem(i).Height;
                    }
                    h = h - RealHeight;
                    h = h / (printData.Rows.Count - printData.TopFixedRows - printData.BottomFixedRows);
                }
                else
                    h = 0;
                for (i = 1; i <= printData.Rows.Count; i++)
                {
                    if (!printData.InFixedRanges(i, -1))
                    {
                        if (oPage.SheetBodyStyle == PrinterBodyStyle.�����и� && oPage.RowHeight > 0)
                            printData.Rows.GetItem(i).PrintHeight = oPage.RowHeight;
                        else
                        {
                            printData.Rows.GetItem(i).PrintHeight = printData.Rows.GetItem(i).Height - h;
                            if (printData.Rows.GetItem(i).PrintHeight < 1)
                            {
                                MessageBox.Show("��" + i.ToString() + "�е��и���С�󳬳���ӡ��Χ, ���ȵ������и�.", "������ӡ�и�", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        }
                    }
                }
            }
            else
            {
                switch (oPage.SheetBodyStyle)
                {
                    case PrinterBodyStyle.�����и�:
                        if (oPage.RowHeight > 0)
                        {
                            for (i = 1; i <= printData.Rows.Count; i++)
                            {
                                if (!printData.InFixedRanges(i, -1))
                                    printData.Rows.GetItem(i).PrintHeight = oPage.RowHeight;
                            }
                        }
                        break;
                    default:
                        // ����Ҫ����.
                        break;
                }
            }
            return true;
        }

        private bool AdjustRowHeightSizeEx(float RealHeight, PrinterBound[] mRows)
        {
            int i, h, j;
            //�Ե�һ��Ϊ׼,�����и�,��ͬ����������
            if (oPage.Zoom == 1)
            {
                if ((int)oPage.GetAttributes(PageSetupKey.ePage_�Զ�����ģʽ) == 0 || (int)oPage.GetAttributes(PageSetupKey.ePage_�Զ�����ģʽ) == 1)
                {
                    RealHeight = RealHeight - GetFixedRowsHeight(FixRowPos.ȫ���̶���);
                    i = mRows[0].Start;
                    j = mRows[0].End - mRows[0].Start + 1;
                    if (j < oPage.Rows && oPage.SheetBodyStyle == PrinterBodyStyle.��������)
                        j = oPage.Rows;
                    h = (int)RealHeight / j;
                }
                else
                    h = -1;
                for (j = 0; j <= mRows.Length - 1; j++)
                {
                    for (i = mRows[j].Start; i <= mRows[j].End; i++)
                    {
                        if (!printData.InFixedRanges(i, -1))
                        {
                            if (h < 0)
                                printData.Rows.GetItem(i).PrintHeight = printData.Rows.GetItem(i).Height;
                            else
                                printData.Rows.GetItem(i).PrintHeight = h;

                            if (printData.Rows.GetItem(i).PrintHeight < 1)
                            {
                                MessageBox.Show("��" + i.ToString() + "�е��и���С�󳬳���ӡ��Χ, ���ȵ������и�.", "������ӡ�и�", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private void CalcRectOfPictureItem(int r, int c, int Offset_X, int Offset_Y)
        {
            Rectangle picRect;
            int i, top = 0, left = 0, right = 0, bottom = 0;
            for (i = 1; i <= printData.Pictures.Count; i++)
            {
                picRect = printData.Pictures.GetItem(i).PictureRect;
                if (r == (int)printData.Pictures.GetItem(i).StartRow)
                    top = (int)(Offset_Y + (printData.Pictures.GetItem(i).StartRow - r) * printData.Rows.GetItem(r).PrintHeight);

                if (r == (int)(printData.Pictures.GetItem(i).EndRow))
                    bottom = (int)(Offset_Y + (printData.Pictures.GetItem(i).EndRow - r) * printData.Rows.GetItem(r).PrintHeight);

                if (c == (int)(printData.Pictures.GetItem(i).StartCol))
                    left = (int)(Offset_X + (printData.Pictures.GetItem(i).StartCol - c) * printData.Cols.GetItem(c).PrintWidth);

                if (c == (int)(printData.Pictures.GetItem(i).EndCol))
                    right = (int)(Offset_X + (printData.Pictures.GetItem(i).EndCol - c) * printData.Cols.GetItem(c).PrintWidth);
                picRect.Location = new Point(left, top);
                picRect.Size = new Size(right - left, bottom - top);
                printData.Pictures.GetItem(i).PictureRect = picRect;
            }
        }

        private float CalcTextRealHeight(PrinterCell cell, float Width, ref string[] Value)
        {
            //��ӡ�����ɵ��ú�������
            float Height = 0;
            string[] arySplit;
            int i, j;
            string s;
            StringBuilder mCol = new StringBuilder();

            if (cell.Value.ToString().Length > 0)
            {
                arySplit = cell.Value.ToString().Split('\n'.ToString().ToCharArray());
                for (i = 0; i < arySplit.Length; i++)
                {
                    s = arySplit[i];
                    if (cell.WordWrap)
                    {
                        SplitTextByBaseWidth(s, ref Value, Width);
                        if (Value.Length > 1)
                        {
                            for (j = 0; j < Value.Length; j++)
                            {
                                if (Value[j].Trim().Length > 0)
                                    mCol.Append("^^" + Value[j].Trim());
                            }
                        }
                        else
                            mCol.Append("^^" + s);
                    }
                    else
                        mCol.Append("^^" + s);
                }
                s = mCol.ToString().Substring(2);
                Value = s.Split("^^".ToCharArray());
                j = 0;
                for (i = 0; i < Value.Length; i++)
                {
                    if (Value[i].Trim().Length == 0)
                        Height = Height + GetFontHeight(currFont, "��") / 3;  //CurrPrinter.TextHeight("��") / 3;
                    else
                    {
                        Height = Height + GetFontHeight(currFont, Value[i].Trim());//CurrPrinter.TextHeight(Trim(mCol(i)));
                        j++;
                    }
                }
                Height = Height + (j - 1) * 15; //�м��
                return Height + PrintAssign.DOUBLELINES_H / 2;
            }
            else
            {
                return GetFontHeight(currFont, "��") + PrintAssign.DOUBLELINES_H / 2; ;
            }
        }

        private float DrawAttachItem(PrinterAttachCell Item,ref float X, float Y, AttachRowType AttachType)
        {
            return DrawAttachItem(Item,ref X, Y, AttachType, false, false);
        }

        private float DrawAttachItem(PrinterAttachCell Item,ref float X, float Y, AttachRowType AttachType, bool Calc)
        {
            return DrawAttachItem(Item,ref X, Y, AttachType, Calc, false);
        }

        private float DrawAttachItem(PrinterAttachCell Item,ref float X, float Y, AttachRowType AttachType, bool Calc, bool DrawBorderAtValueIsNull)
        {
            //����ʵ�ʵĴ�ӡ�߶�
            float Width, Height;
            if (AttachType == AttachRowType.����)
            {
                Height = (Item.Height * Item.InRowPercent) / 100;
                Width = -1;
            }
            else
            {
                Height = Item.Height;
                Width = (Item.InRowPercent * (printArrange.Size.Width - (printAssign.PageSetup.LeftMargin + printAssign.PageSetup.RightMargin))) / 100;
            }
            if ((Item.Properties.Value.ToString()).Length > 0)
            {
                DrawCellContent(Item.Properties, X, Y, ref Width, ref Height, AttachType, 0, 0, Calc);
                if (!Calc)
                    DrawBorder(Item.Properties, X, Y, Width, Height, AttachType);
            }
            else if (DrawBorderAtValueIsNull)
            {
                if (!Calc)
                    DrawBorder(Item.Properties, X, Y, Width, Height, AttachType);
            }
            else
                //û�������Ҳ����ʱ����ӡ
                Height = 0;
            X = X + Width;
            return Height;
        }

        private float DrawAttachRow(AttachRow oAttach, ref  float X, ref  float Y, AttachRowType AttachType)
        {
            return DrawAttachRow(oAttach, ref X, ref  Y, AttachType, false);
        }

        private float DrawAttachRow(AttachRow oAttach, ref  float X, ref  float Y, AttachRowType AttachType, bool Calc)
        {

            int i, ItemBorderColor, c;
            float h = 0, attachRow = 0;
            bool b = false;
            PrinterAttachCell Item = new PrinterAttachCell();
            Font oFont;
            PrinterLogTextBorder ItemBorder;
            PrinterBorderStyle ItemBoderStyle;

            switch (AttachType)
            {
                case AttachRowType.ҳü:
                    oFont = (Font)printAssign.PageSetup.PageHeaderFont.Clone();
                    c = printAssign.PageSetup.PageHeaderColor;
                    b = (bool)(printAssign.PageSetup.GetAttributes(PageSetupKey.ePage_����������ҳü�ı߿�));
                    ItemBorder = printAssign.PageSetup.PageHeaderBorder;
                    ItemBorderColor = printAssign.PageSetup.BodyColor;
                    ItemBoderStyle = printAssign.PageSetup.BodyBorderStyle;
                    break;
                case AttachRowType.ҳ��:
                    oFont = (Font)printAssign.PageSetup.PageFooterFont.Clone();
                    c = printAssign.PageSetup.PageFooterColor;
                    b = (bool)(printAssign.PageSetup.GetAttributes(PageSetupKey.ePage_����������ҳ�ŵı߿�));
                    ItemBorder = printAssign.PageSetup.PageFooterBorder;
                    ItemBorderColor = printAssign.PageSetup.PageFooterColor;
                    ItemBoderStyle = printAssign.PageSetup.PageFooterBorderStyle;
                    break;
                case AttachRowType.��ͷ:
                    oFont = (Font)printAssign.PageSetup.HeaderFont.Clone();
                    c = printAssign.PageSetup.HeaderColor;
                    b = (bool)(printAssign.PageSetup.GetAttributes(PageSetupKey.ePage_���������ñ�ͷ�ı߿�));
                    ItemBorder = printAssign.PageSetup.HeaderBorder;
                    ItemBorderColor = printAssign.PageSetup.HeaderColor;
                    ItemBoderStyle = printAssign.PageSetup.HeaderBorderStyle;
                    break;

                case AttachRowType.����:
                    oFont = (Font)printAssign.PageSetup.BodyFont.Clone();
                    c = printAssign.PageSetup.BodyColor;
                    ItemBorder = printAssign.PageSetup.BodyBorder;
                    ItemBorderColor = printAssign.PageSetup.BodyColor;
                    ItemBoderStyle = printAssign.PageSetup.BodyBorderStyle;
                    break;
                case AttachRowType.��β:
                    oFont = (Font)printAssign.PageSetup.TailFont.Clone();
                    c = printAssign.PageSetup.TailColor;
                    b = (bool)(printAssign.PageSetup.GetAttributes(PageSetupKey.ePage_���������ñ�β�ı߿�));
                    ItemBorder = printAssign.PageSetup.TailBorder;
                    ItemBorderColor = printAssign.PageSetup.TailColor;
                    ItemBoderStyle = printAssign.PageSetup.TailBorderStyle;
                    break;
                case AttachRowType.������:
                    oFont = (Font)printAssign.PageSetup.SubTitleFont.Clone();
                    c = printAssign.PageSetup.SubTitleColor;
                    b = (bool)(printAssign.PageSetup.GetAttributes(PageSetupKey.ePage_���������ø�����ı߿�));
                    ItemBorder = printAssign.PageSetup.SubTitleBorder;
                    ItemBorderColor = printAssign.PageSetup.SubTitleColor;
                    ItemBoderStyle = printAssign.PageSetup.SubTitleBorderStyle;
                    break;
                case AttachRowType.������:
                    oFont = (Font)printAssign.PageSetup.MainTitleFont.Clone();
                    c = printAssign.PageSetup.MainTitleColor;
                    b = (bool)(printAssign.PageSetup.GetAttributes(PageSetupKey.ePage_����������������ı߿�));
                    ItemBorder = printAssign.PageSetup.MainTitleBorder;
                    ItemBorderColor = printAssign.PageSetup.MainTitleColor;
                    ItemBoderStyle = printAssign.PageSetup.MainTitleBorderStyle;
                    break;
                default:
                    return h;
            }
            oAttach.DefaultFont = (Font)oFont.Clone();
            SetFont(oAttach.DefaultFont);
            if (AttachType == AttachRowType.ҳ��)
                Y = Y - oAttach.Offset_V;
            else
                Y = Y + oAttach.Offset_V;
            for (i = 1; i <= oAttach.Count; i++)
            {
                Item = oAttach.AttachCell(i);
                if (Item.Properties.Value != null)
                {
                    Item.Properties.FontColor = c;
                    if (!b)
                    {
                        switch (ItemBorder)
                        {
                            case PrinterLogTextBorder.ȫ������:
                                Item.Properties.B_Color = ItemBorderColor;
                                Item.Properties.B_Style = ItemBoderStyle;
                                Item.Properties.T_Color = ItemBorderColor;
                                Item.Properties.T_Style = ItemBoderStyle;
                                Item.Properties.R_Color = ItemBorderColor;
                                Item.Properties.R_Style = ItemBoderStyle;
                                Item.Properties.L_Color = ItemBorderColor;
                                Item.Properties.L_Style = ItemBoderStyle;
                                break;
                            case PrinterLogTextBorder.�ϱ���:
                                Item.Properties.T_Color = ItemBorderColor;
                                Item.Properties.T_Style = ItemBoderStyle;
                                break;
                            case PrinterLogTextBorder.�±���:
                                Item.Properties.B_Color = ItemBorderColor;
                                Item.Properties.B_Style = ItemBoderStyle;
                                break;
                            case PrinterLogTextBorder.�ұ���:
                                Item.Properties.R_Color = ItemBorderColor;
                                Item.Properties.R_Style = ItemBoderStyle;
                                break;
                            case PrinterLogTextBorder.�����:
                                Item.Properties.L_Color = ItemBorderColor;
                                Item.Properties.L_Style = ItemBoderStyle;
                                break;
                            case PrinterLogTextBorder.�ޱ���:
                                Item.Properties.B_Color = ItemBorderColor;
                                Item.Properties.B_Style = PrinterBorderStyle.�ޱ߿�;
                                Item.Properties.T_Color = ItemBorderColor;
                                Item.Properties.T_Style = PrinterBorderStyle.�ޱ߿�;
                                Item.Properties.R_Color = ItemBorderColor;
                                Item.Properties.R_Style = PrinterBorderStyle.�ޱ߿�;
                                Item.Properties.L_Color = ItemBorderColor;
                                Item.Properties.L_Style = PrinterBorderStyle.�ޱ߿�;
                                break;
                        }
                    }
                    h = DrawAttachItem(Item,ref X, Y, AttachType, Calc);
                    if (h > attachRow)
                    {
                        if (oAttach.Offset_V > 0)
                            attachRow = h + oAttach.Offset_V;
                        else
                            attachRow = h;
                    }
                }
            }
            Y = Y + h;
            if (oAttach.Offset_V > 0)
                h = h + oAttach.Offset_V;
            return attachRow;
        }

        private float DrawAttachRows(AttachRows o, AttachRowType AttachType)
        {
            return DrawAttachRows(o, AttachType, false);
        }

        private float DrawAttachRows(AttachRows o, AttachRowType AttachType, bool Calc)
        {
            int i;
            AttachRow oAttach;
            float h = 0, X, Y;
            Pen pen;


            Y = CurrentY;
            for (i = 1; i <= o.Count; i++)
            {
                X = CurrentX;
                oAttach = o.TheAttachRow(i);
                h = h + DrawAttachRow(oAttach, ref  X, ref  Y, AttachType, Calc);
            }
            if (AttachType == AttachRowType.ҳü && printAssign.PageSetup.DrawLineUnderPageHeader && h > 0 && (!Calc))
            {
                pen = new Pen(Color.Black);
                CurrPrinter.DrawLine(pen, CurrentX, h + CurrentY + (3 * PrintAssign.DOUBLELINES_V) / 2, pageArrange.Arrange(1).Size.Width - printAssign.PageSetup.RightMargin, h + CurrentY + (3 * PrintAssign.DOUBLELINES_V) / 2);
            }
            if (AttachType == AttachRowType.ҳ�� && printAssign.PageSetup.DrawLineUpPageFooter && h > 0 && (!Calc))
            {
                pen = new Pen(Color.Black);
                CurrPrinter.DrawLine(pen, CurrentX, CurrentY - h - (3 * PrintAssign.DOUBLELINES_V) / 2, pageArrange.Arrange(1).Size.Width - printAssign.PageSetup.RightMargin, CurrentY - h - (3 * PrintAssign.DOUBLELINES_V) / 2);
            }

            return h;
        }

        private void DrawAverageText(string Content, float X, float Y, float Width, float Height)
        {
            int i, CharSpaceWidth;
            float pX, pWidth;
            string s;

            CharSpaceWidth = (int)(Width - GetFontWidth(currFont, Content));
            CharSpaceWidth = (CharSpaceWidth / Content.Length) / 2;

            pX = X + CharSpaceWidth;
            for (i = 1; i <= Content.Length; i++)
            {
                s = Content.Substring(i - 1, 1);
                pWidth = GetFontWidth(currFont, s);
                DrawText(s, pX, Y, pWidth, Height, PrinterTextAlign.LeftMiddle);
                pX = pX + 2 * CharSpaceWidth + pWidth;
            }
        }

        private void DrawBorder(PrinterCell cell, float X, float Y, float Width, float Height, AttachRowType attachType)
        {
            DrawBorder(cell, X, Y, Width, Height, attachType, false, (PrinterCellPos)(-1), 0, 0);
        }
        private void DrawBorder(PrinterCell cell, float X, float Y, float Width, float Height, AttachRowType attachType, bool BottomIsBold)
        {
            DrawBorder(cell, X, Y, Width, Height, attachType, BottomIsBold, (PrinterCellPos)(-1), 0, 0);
        }
        private void DrawBorder(PrinterCell cell, float X, float Y, float Width, float Height, AttachRowType attachType, bool BottomIsBold, PrinterCellPos CellPos)
        {
            DrawBorder(cell, X, Y, Width, Height, attachType, BottomIsBold, CellPos, 0, 0);
        }
        private void DrawBorder(PrinterCell cell, float X, float Y, float Width, float Height, AttachRowType attachType, bool BottomIsBold, PrinterCellPos CellPos, int Row)
        {
            DrawBorder(cell, X, Y, Width, Height, attachType, BottomIsBold, CellPos, Row, 0);
        }
        private void DrawBorder(PrinterCell Cell, float X, float Y, float Width, float Height, AttachRowType attachType, bool BottomIsBold, PrinterCellPos CellPos, int Row, int Col)
        {
            Pen pen = currPen;

            if (attachType == AttachRowType.ҳ�� && (oPage.GetAttributes(PageSetupKey.ePage_PageFooterBorderColor).ToString().Substring(0, 1)) == "1")
            {
                Cell.L_Color = int.Parse(oPage.GetAttributes(PageSetupKey.ePage_PageFooterBorderColor).ToString().Substring(2));
                Cell.R_Color = Cell.L_Color;
                Cell.T_Color = Cell.L_Color;
                Cell.B_Color = Cell.L_Color;
            }
            if ((attachType == AttachRowType.���� || attachType == AttachRowType.����) && (oPage.GetAttributes(PageSetupKey.ePage_BodyBorderColor).ToString().Substring(0, 1)) == "1")
            {
                Cell.L_Color = int.Parse(oPage.GetAttributes(PageSetupKey.ePage_BodyBorderColor).ToString().Substring(2));
                Cell.R_Color = Cell.L_Color;
                Cell.T_Color = Cell.L_Color;
                Cell.B_Color = Cell.L_Color;
            }
            if (attachType == AttachRowType.��ͷ && (oPage.GetAttributes(PageSetupKey.ePage_HeaderBorderColor).ToString().Substring(0, 1)) == "1")
            {
                Cell.L_Color = int.Parse(oPage.GetAttributes(PageSetupKey.ePage_HeaderBorderColor).ToString().Substring(2));
                Cell.R_Color = Cell.L_Color;
                Cell.T_Color = Cell.L_Color;
                Cell.B_Color = Cell.L_Color;
            }
            if (attachType == AttachRowType.��β && (oPage.GetAttributes(PageSetupKey.ePage_TailBorderColor).ToString().Substring(0, 1)) == "1")
            {
                Cell.L_Color = int.Parse(oPage.GetAttributes(PageSetupKey.ePage_TailBorderColor).ToString().Substring(2));
                Cell.R_Color = Cell.L_Color;
                Cell.T_Color = Cell.L_Color;
                Cell.B_Color = Cell.L_Color;
            }
            if (attachType == AttachRowType.������ && (oPage.GetAttributes(PageSetupKey.ePage_SubTitleBorderColor).ToString().Substring(0, 1)) == "1")
            {
                Cell.L_Color = int.Parse(oPage.GetAttributes(PageSetupKey.ePage_SubTitleBorderColor).ToString().Substring(2));
                Cell.R_Color = Cell.L_Color;
                Cell.T_Color = Cell.L_Color;
                Cell.B_Color = Cell.L_Color;
            }
            if (attachType == AttachRowType.ҳü && (oPage.GetAttributes(PageSetupKey.ePage_PageHeaderBorderColor).ToString().Substring(0, 1)) == "1")
            {
                Cell.L_Color = int.Parse(oPage.GetAttributes(PageSetupKey.ePage_PageHeaderBorderColor).ToString().Substring(2));
                Cell.R_Color = Cell.L_Color;
                Cell.T_Color = Cell.L_Color;
                Cell.B_Color = Cell.L_Color;
            }
            if (attachType == AttachRowType.������ && (oPage.GetAttributes(PageSetupKey.ePage_MainTitleBorderColor).ToString().Substring(0, 1)) == "1")
            {
                Cell.L_Color = int.Parse(oPage.GetAttributes(PageSetupKey.ePage_MainTitleBorderColor).ToString().Substring(2));
                Cell.R_Color = Cell.L_Color;
                Cell.T_Color = Cell.L_Color;
                Cell.B_Color = Cell.L_Color;
            }

            if (attachType == AttachRowType.ҳ��)
            {
                if (Cell.L_PenWidth != 0) pen.Width = Cell.L_PenWidth * float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());
                DrawTheBorder(Cell, 1, Cell.L_Color, Cell.L_Style, X, Y, X, Y - Height, CellPos, Row, Col);
                if (Cell.L_PenWidth != 0) pen.Width = float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());

                if (Cell.R_PenWidth != 0) pen.Width = Cell.R_PenWidth * float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());
                DrawTheBorder(Cell, 3, Cell.R_Color, Cell.R_Style, X + Width, Y, X + Width, Y - Height, CellPos, Row, Col);
                if (Cell.R_PenWidth != 0) pen.Width = float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());

                if (Cell.T_PenWidth != 0) pen.Width = Cell.T_PenWidth * float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());
                DrawTheBorder(Cell, 2, Cell.T_Color, Cell.T_Style, X, Y - Height, X + Width, Y - Height, CellPos, Row, Col);
                if (Cell.T_PenWidth != 0) pen.Width = float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());

                if (Cell.B_PenWidth != 0) pen.Width = Cell.B_PenWidth * float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());
                DrawTheBorder(Cell, 4, Cell.B_Color, Cell.B_Style, X, Y, X + Width, Y, CellPos, Row, Col);
                if (Cell.B_PenWidth != 0) pen.Width = float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());
            }
            else
            {
                if (Cell.L_PenWidth != 0) pen.Width = Cell.L_PenWidth * float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());
                DrawTheBorder(Cell, 1, Cell.L_Color, Cell.L_Style, X, Y, X, Y + Height, CellPos, Row, Col);
                if (Cell.L_PenWidth != 0) pen.Width = float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());

                if (Cell.R_PenWidth != 0) pen.Width = Cell.R_PenWidth * float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());
                DrawTheBorder(Cell, 3, Cell.R_Color, Cell.R_Style, X + Width, Y, X + Width, Y + Height, CellPos, Row, Col);
                if (Cell.R_PenWidth != 0) pen.Width = float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());

                if (Cell.T_PenWidth != 0) pen.Width = Cell.T_PenWidth * float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());
                DrawTheBorder(Cell, 2, Cell.T_Color, Cell.T_Style, X, Y, X + Width, Y, CellPos, Row, Col);
                if (Cell.T_PenWidth != 0) pen.Width = float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());

                if (BottomIsBold)
                {
                    pen.Width = float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ()) * 2;
                    DrawTheBorder(Cell, 4, Cell.B_Color, Cell.B_Style, X, Y + Height + PrintAssign.DOUBLELINES_V / 4, X + Width, Y + Height + PrintAssign.DOUBLELINES_V / 4, CellPos, Row, Col);
                    pen.Width = float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());
                }
                else
                {
                    if (Cell.B_PenWidth != 0) pen.Width = Cell.B_PenWidth * float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());
                    if (attachType != AttachRowType.����)
                        DrawTheBorder(Cell, 4, Cell.B_Color, Cell.B_Style, X, Y + Height + PrintAssign.DOUBLELINES_V / 4, X + Width, Y + Height + PrintAssign.DOUBLELINES_V / 4, CellPos, Row, Col);
                    else
                        DrawTheBorder(Cell, 4, Cell.B_Color, Cell.B_Style, X, Y + Height, X + Width, Y + Height, CellPos, Row, Col);
                    if (Cell.B_PenWidth != 0) pen.Width = float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString ());
                }
            }
        }

        private float DrawBottomFixedRows(int StartCol, int EndCol, float X, float Y)
        {
            Range oRange, oMerg;
            int i, j;
            PrinterTextAlign CellPos;
            float cX, cY, w, h;
            PrinterCell typCell;
            //���ﲻ��������ҹ̶��н��沿��
            h = 0;
            if (printData.BottomFixedRows > 0)
            {
                oRange = printData.FixRanges.GetItem ("BottomFixedRows");
                if (StartCol < printData.LeftFixedCols + 1) StartCol = printData.LeftFixedCols + 1;
                if (EndCol > printData.Cols.Count - printData.RightFixedCols) EndCol = printData.Cols.Count - printData.RightFixedCols;
                for (i = oRange.StartRow; i <= oRange.EndRow; i++)
                {
                    h = h + printData.Rows.GetItem(i).PrintHeight;
                    w = 0;
                    for (j = StartCol; j <= EndCol; j++)
                    {
                        w = w + printData.Cols.GetItem(j).PrintWidth;
                        if (printData.InMergedRanges(i, j, out CellPos, out oMerg))
                        {
                            if (CellPos == PrinterTextAlign.LeftTop)
                                DrawMergedRowCol(oRange.StartRow, StartCol, oRange.EndRow, EndCol, X + GetFixedColsWidth(FixColPos.ȫ���̶���), Y, 0, oMerg, false);
                        }
                        else
                        {
                            typCell = printData.Body.GetItem("$" + i+ "$" + j);
                            //����X,Y
                            GetPointByCell(X + GetFixedColsWidth(FixColPos.ȫ���̶���), Y, oRange.StartRow, StartCol, i, j,out cX,out cY);
                            float w1=printData.Cols.GetItem(j).PrintWidth, h1=printData.Rows.GetItem(i).PrintHeight;
                            DrawCellContent(typCell, cX, cY, ref w1, ref h1, AttachRowType.����, i, j, false, mBodyIncludeFixRange);
                            printData.Cols.GetItem(j).PrintWidth=w1;
                            printData.Rows.GetItem(i).PrintHeight=h1;
                            DrawBorder(typCell, cX, cY, printData.Cols.GetItem(j).PrintWidth, printData.Rows.GetItem(i).PrintHeight, AttachRowType.����, false, GetCellPos(i, j, 1, StartCol - printData.LeftFixedCols, oRange.EndRow, EndCol + printData.RightFixedCols), i, j);
                        }
                    }
                }
            }
            return h;
        }

        private float DrawBlankRows(float X, float Y, int StartRow, int StartCol, int EndCol, bool IsEndRow)
        {
            PrinterCell typCell;
            int i, Row=1;
            bool b;
            float h = 0;

            for (i = PrintData.Rows.Count; i >= 1; i--)
            {
                if (!PrintData.InFixedRanges(i, -1))
                {
                    Row = i;//ȡ��������
                    break;
                }
            }

            StartRow = StartRow + 1;
            b = false;
            if (printAssign.PageSetup.DblLineRows > 0)
            {
                if ((StartRow % printAssign.PageSetup.DblLineRows) == 0)
                    b = true;
            }

            for (i = StartCol; i <= EndCol; i++)
            {
                typCell = printData.Body.GetItem("$" + Row+ "$" + i);//ȡ����������
                typCell.Value = "";
                if (printAssign.PageSetup.SheetBodyStyle == PrinterBodyStyle.�����и�)
                {
                    if (IsEndRow)
                        //��ֹ��˫�߱߿�ʱ�߲�����
                        DrawBorder(typCell, X, Y, printData.Cols.GetItem(i).PrintWidth, printAssign.PageSetup.RowHeight, AttachRowType.����, b, GetCellPos(StartRow, i, 1, StartCol, StartRow, EndCol));
                    else
                        //600000��Ϊ����CurrRow���ܳ�Ϊ���һ��,��ֹ��߿�ʱ�ߴ���˫�߱߿�
                        DrawBorder(typCell, X, Y, printData.Cols.GetItem(i).PrintWidth, printAssign.PageSetup.RowHeight, AttachRowType.����, b, GetCellPos(StartRow, i, 1, StartCol, 60000, EndCol));

                    if (typCell.Behave == PrinterCellBehave.�����)
                        DrawCyCell(typCell, X, Y, printData.Cols.GetItem(i).PrintWidth, printData.Rows.GetItem(Row).PrintHeight, typCell.L_Color, StartRow, i);

                    h = printAssign.PageSetup.RowHeight;
                }
                else //if (printAssign.PageSetup.SheetBodyStyle == PrinterBodyStyle.��������)
                {
                    if (IsEndRow)
                        //��ֹ��˫�߱߿�ʱ�߲�����
                        DrawBorder(typCell, X, Y, printData.Cols.GetItem(i).PrintWidth, printData.Rows.GetItem(Row).PrintHeight, AttachRowType.����, b, GetCellPos(StartRow, i, 1, StartCol, StartRow, EndCol), Row, i);
                    else
                        //600000��Ϊ����CurrRow���ܳ�Ϊ���һ��,��ֹ��߿�ʱ�ߴ���˫�߱߿�
                        DrawBorder(typCell, X, Y, printData.Cols.GetItem(i).PrintWidth, printData.Rows.GetItem(Row).PrintHeight, AttachRowType.����, b, GetCellPos(StartRow, i, 1, StartCol, 60000, EndCol), Row, i);

                    if (typCell.Behave == PrinterCellBehave.�����)
                        DrawCyCell(typCell, X, Y, printData.Cols.GetItem(i).PrintWidth, printData.Rows.GetItem(Row).PrintHeight, typCell.L_Color, StartRow, i);

                    h = PrintData.Rows.GetItem(Row).PrintHeight;
                }
                X = X + PrintData.Cols.GetItem(i).PrintWidth;

            }
            return h;
        }

        private void DrawCellContent(PrinterCell Cell, float X, float Y, ref float Width, ref float Height, AttachRowType AttachType, int Row, int Col)
        {
            DrawCellContent(Cell, X, Y, ref  Width, ref  Height, AttachType, Row, Col, false, true);
        }
        private void DrawCellContent(PrinterCell Cell, float X, float Y, ref float Width, ref float Height, AttachRowType AttachType, int Row, int Col, bool Calc)
        {
            DrawCellContent(Cell, X, Y, ref  Width, ref  Height, AttachType, Row, Col, Calc, true);
        }
        private void DrawCellContent(PrinterCell Cell, float X, float Y, ref float Width, ref float Height, AttachRowType AttachType, int Row, int Col, bool Calc, bool IsKeepSameFontSize)
        {
            string s, s2 = "";
            string[] s1 = new string[1];
            typCellEx[] printText = new typCellEx[1];
            int i;
            bool bMultiRow, b = false;
            float RealHeight;
            bMultiRow = false;

            if (Cell.font != null)
            {
                if (AttachType == AttachRowType.���� && IsKeepSameFontSize && (printAssign.PageSetup.KeepFont))
                {
                    for (i = 0; i < mKeepFont.Length; i++)
                    {
                        if (mKeepFont[i].Name == Cell.font.Name)
                        {
                            SetFont(mKeepFont[i]);
                            currFont = (Font)Cell.font.Clone();
                            break;
                        }
                    }
                }
                else
                    SetFont(Cell.font);
            }
            else
            {
                switch (AttachType)
                {
                    case AttachRowType.����:
                        SetFont(printAssign.PageSetup.BodyFont);
                        break;
                    case AttachRowType.��ͷ:
                        SetFont(printAssign.PageSetup.HeaderFont);
                        break;
                    case AttachRowType.��β:
                        SetFont(printAssign.PageSetup.TailFont);
                        break;
                    case AttachRowType.������:
                        SetFont(printAssign.PageSetup.SubTitleFont);
                        break;
                    case AttachRowType.ҳ��:
                        SetFont(printAssign.PageSetup.PageFooterFont);
                        break;
                    case AttachRowType.ҳü:
                        SetFont(printAssign.PageSetup.PageHeaderFont);
                        break;
                }
            }
            currColor =Cell.FontColor==0?Color.Black : Color.FromArgb(Cell.FontColor);
            switch (Cell.Behave)
            {
                case PrinterCellBehave.����߱���:
                    DrawCyTitle(Cell, X, Y, Width, Height, Cell.L_Color, Row, Col);
                    b = false;
                    break;
                case PrinterCellBehave.�����:
                    if (oPage.CyLine)
                    {
                        b = false;
                        DrawCyCell(Cell, X, Y, Width, Height, Cell.L_Color, Row, Col);
                    }
                    else
                    {
                        if (double.Parse(Cell.Value.ToString()) < 0 && oPage.ColorPrint)
                        {
                            currColor = Color.FromArgb(255,0,0);
                            Cell.Value = Cell.Value.ToString().Substring(1);
                        }
                        if ((bool)oPage.GetAttributes(PageSetupKey.ePage_���Ҫ��ǧ��λ))
                        {
                            try
                            {
                                Cell.Value = double.Parse(Cell.Value.ToString()).ToString("N2");//�����ֻҪ����2λС��
                            }
                            catch
                            {
                                Cell.Value = "0.00";
                            }
                        }
                        b = true;
                    }
                    break;
                case PrinterCellBehave.��д���:
                    Cell.Value = General.ChineseAmount(Cell.Value.ToString());
                    break;
                default:
                    b = true;
                    break;
            }
            if (b && Cell.Value !=null )
            {
                s2 = Cell.Value.ToString();
                Cell.Value = GetCellText(s2);
                if (Height == 0)
                {
                    Height = CalcTextRealHeight(Cell, Width, ref s1);
                    RealHeight = Height;
                }
                else
                    RealHeight = CalcTextRealHeight(Cell, Width, ref s1);
                if (Calc)
                {
                    if (s2 != Cell.Value.ToString())
                        Cell.Value = s2;
                    return;
                }
                if (s1.Length > 1)
                    bMultiRow = true;
                if (Cell.Align == PrinterTextAlign.���ֲ�����)
                {
                    s = GetCellText(Cell.Value.ToString());
                    if (AttachType == AttachRowType.ҳü || AttachType == AttachRowType.ҳ��)
                    {
                        s = s.Replace("&P", printAssign.phyNum.ToString());
                        s = s.Replace("&N", printData.PhyPageCount.ToString());
                    }
                    else
                    {
                        s = s.Replace("&P",currPageNum.ToString());
                        s = s.Replace("&N",printData.LogicPageCount.ToString());
                    }
                    SaveCurrentFont();
                    while (GetFontWidth(currFont, s) > Width)
                    {
                        if (!FontZoomOut())
                        {
                            s = "###";//����
                            break;
                        }
                    }
                    while (CalcTextRealHeight(Cell, Width, ref s1) > Height)
                    {
                        if (!FontZoomOut())
                        {
                            s = "###";//����
                            break;
                        }
                    }

                    if (AttachType == AttachRowType.ҳ��)
                        Y = Y - Height;

                    DrawAverageText(Cell.Value.ToString(), X + PrintAssign.DOUBLELINES_H, Y + PrintAssign.DOUBLELINES_V / 4, Width - PrintAssign.DOUBLELINES_H, Height - PrintAssign.DOUBLELINES_V / 4);
                    RestoreFont();
                }
                else
                {
                    s = s2;
                    //if (AttachType == AttachRowType.ҳü || AttachType == AttachRowType.ҳ�� || AttachType == AttachRowType.��ͷ || AttachType == AttachRowType.��β || AttachType == AttachRowType.������ || AttachType == AttachRowType.������)
                    //{
                    //    s = s.Replace("&P", printAssign.phyNum.ToString());  //printSet.Properties("CurrPhyPage"));
                    //    s = s.Replace("&N", printData.PhyPageCount.ToString());  //PrintSet.Properties("PhyPageCount"));
                    //}
                    if (AttachType == AttachRowType.ҳü || AttachType == AttachRowType.ҳ��)
                    {
                        s = s.Replace("&P", printAssign.phyNum.ToString());
                        s = s.Replace("&N", printData.PhyPageCount.ToString());
                    }
                    else
                    {
                        s = s.Replace("&P", currPageNum.ToString());
                        s = s.Replace("&N", printData.LogicPageCount.ToString());
                    }
                    SaveCurrentFont();
                    if (AttachType == AttachRowType.���� && (!printData.InFixedRanges(Row, Col)) && (oPage.ReduceFont == 1) && (!(IsKeepSameFontSize && (oPage.KeepFont))))
                        currFont = new Font(currFont.Name, 36);

                    if (Width <= 0 && AttachType == AttachRowType.����)
                    {
                        for (i = 0; i < s1.Length; i++)
                        {
                            if (Width < GetFontWidth(currFont, s1[i]))
                                Width = GetFontWidth(currFont, s1[i]);

                        }
                        Width = Width + PrintAssign.DOUBLELINES_H;
                    }
                    while (CalcTextRealHeight(Cell, Width, ref s1) > Height)
                    {
                        if (!FontZoomOut())
                        {
                            s = "###";// '����
                            break;
                        }
                    }

                    bMultiRow = s1.Length > 1 ? true : false;

                    if (bMultiRow)
                    {
                        for (i = 0; i < s1.Length; i++)
                        {
                            string tmp = GetCellText(s1[i]);
                            while (GetFontWidth(currFont, tmp) + PrintAssign.DOUBLELINES_H > Width)
                            {
                                if (!FontZoomOut())
                                {
                                    s = "###";//����
                                    break;
                                }
                            }
                        }
                        RealHeight = CalcTextRealHeight(Cell, Width, ref s1);
                    }
                    else
                    {
                        string tmp = GetCellText(Cell.Value.ToString ());
                        while (GetFontWidth(currFont, tmp) + PrintAssign.DOUBLELINES_H > Width)
                        {
                            if (!FontZoomOut())
                            {
                                s = "###";// '����
                                break;
                            }
                        }
                    }
                    if (AttachType == AttachRowType.ҳ��)
                        Y = Y - Height;
                    if (bMultiRow)
                    {
                        TextArrayToCellEx(s, s1, ref printText);
                        DrawTextMultiRow(printText, X + PrintAssign.DOUBLELINES_H, Y + PrintAssign.DOUBLELINES_V / 2, Width - PrintAssign.DOUBLELINES_H, Height - PrintAssign.DOUBLELINES_V / 2, RealHeight, Cell.Align);
                    }
                    else
                    {
                        DrawText(s, X + PrintAssign.DOUBLELINES_H, Y + PrintAssign.DOUBLELINES_V / 2, Width - PrintAssign.DOUBLELINES_H, Height - PrintAssign.DOUBLELINES_V / 2, Cell.Align);
                    }
                    RestoreFont();
                }
            }
            if (Cell.Value != null)
            {
                if (s2 != Cell.Value.ToString())
                    Cell.Value = s2;
            }
        }

        private void DrawCyCell(PrinterCell Cell, float X, float Y, float Width, float Height, int pColor, int Row, int Col)
        {
            float SpacingWidth;
            float cX, cWidth;
            int i;
            Color TColor = currColor;
            string s = Cell.Value.ToString();

            if (oPage.CyLine)
            {
                try
                {
                    double d = double.Parse(s);
                    if (d == 0)
                        s = "";
                    else
                        s = d.ToString("N2");//�����ֻҪ����2λС��
                }
                catch
                {
                    s = "";
                }
                s = s.Replace(",", "");
                s = s.Replace(".", "");
            }
            if (oPage.ColorPrint)
            {
                if (s.IndexOf ("-")==0)
                {
                    s = s.Substring(1);
                    currColor = Color.FromArgb(255, 0, 0);
                }
            }
            cX = X + Width - PrintAssign.DOUBLELINES_H / 2;
            //�������µĿջ�˫��
            cWidth = Width - PrintAssign.DOUBLELINES_H;

            SaveCurrentFont();
            SetFont(new Font("����", 9.75F));//����ߵ�����ͳһ����ȷ�����
            //if (Cell.font == null)
            //    SetFont(oPage.BodyFont);
            //else
            //    SetFont(Cell.font);
            SpacingWidth =2+GetFontWidth(currFont, "0123456789") / 10;
            ///*Ԥ��һ��,�����ܻ���
            cX = cX - SpacingWidth;
            i = s.Length;
            while (i >= 1 && cX >= X + PrintAssign.DOUBLELINES_H / 2 - 30)
            {
                cX = cX - SpacingWidth;
                i = i - 1;
            }
            //*/
            if (i != 0) s = "####################";
            cX = X + Width - PrintAssign.DOUBLELINES_H / 2;
            cX = cX - SpacingWidth;
            while (s.Length >= 1 && cX >= X + PrintAssign.DOUBLELINES_H / 2 - 30)
            {
                DrawText(s.Substring(s.Length - 1), cX, Y, SpacingWidth, Height, PrinterTextAlign.CenterMiddle);
                s = s.Substring(0, s.Length - 1);
                cX = cX - SpacingWidth;
            }
            RestoreFont();
            if (TColor != currColor) currColor = TColor;
            DrawCyLine(Cell, X, Y, Width, Height, SpacingWidth, pColor);
        }

        private void DrawCyLine(PrinterCell Cell, float X, float Y, float Width, float Height, float SpacingWidth, int pColor)
        {
            Pen pen = new Pen(Color.Black  , float.Parse (printAssign.PageSetup.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString() ));
            float cX;
            int i, offset1 = 0, offset2 = 0;
            if (pColor == 0)
                pColor = Color.Black.ToArgb();   
            if (!oPage.CyLine)
                return;
            if (printAssign.PageSetup.GetAttributes(PageSetupKey.ePage_BodyBorderColor).ToString().Substring(0, 1) == "1")
                pColor = int.Parse(printAssign.PageSetup.GetAttributes(PageSetupKey.ePage_BodyBorderColor).ToString().Substring(2));
            if (oPage.CyDotLine)
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            if (printAssign.PageSetup.ColorPrint)
                pen.Color = Color.FromArgb(pColor);
            if (Cell.T_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                offset1 = PrintAssign.DOUBLELINES_V / 4;
            if (Cell.B_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                offset2 = PrintAssign.DOUBLELINES_V / 4;
            //���߻�˫��
            CurrPrinter.DrawLine(pen, X + PrintAssign.DOUBLELINES_H / 2, Y + offset1, X + PrintAssign.DOUBLELINES_H / 2, Y + Height - offset2);
            CurrPrinter.DrawLine(pen, X + Width - PrintAssign.DOUBLELINES_H / 2, Y + offset1, X + Width - PrintAssign.DOUBLELINES_H / 2, Y + Height - offset2);
            //���ұߵ���Ϊ���µĿ�
            cX = X + Width - PrintAssign.DOUBLELINES_H / 2 - SpacingWidth;
            i = 0;
            while (cX >= X + PrintAssign.DOUBLELINES_H / 2 + SpacingWidth)
            {
                i = i + 1;
                if (i == 2)
                {
                    if (printAssign.PageSetup.ColorPrint)
                        pen.Color = Color.FromArgb(255, 0, 0);
                    CurrPrinter.DrawLine(pen, cX, Y + offset1, cX, Y + Height - offset2);
                    if (printAssign.PageSetup.ColorPrint)
                        pen.Color = Color.FromArgb(pColor);
                }
                else
                {
                    if (i > 3 && ((i - 2) % 3 == 0))
                    {
                        pen.Width = (int)(printAssign.PageSetup.GetAttributes(PageSetupKey.ePage_ϸ�߿��)) * 2;
                        CurrPrinter.DrawLine(pen, cX, Y + offset1, cX, Y + Height - offset2);
                        pen.Width = (int)(printAssign.PageSetup.GetAttributes(PageSetupKey.ePage_ϸ�߿��));
                    }
                    else
                        CurrPrinter.DrawLine(pen, cX, Y + offset1, cX, Y + Height - offset2);
                }
                cX = cX - SpacingWidth;
            }
        }

        private void DrawCyTitle(PrinterCell Cell, float X, float Y, float Width, float Height, int pColor, int Row, int Col)
        {
            //�������������,������
            string s;
            float SpacingWidth, cX, cWidth;

            //���ٴ� MINCYCHARS ������
            cX = X + Width - PrintAssign.DOUBLELINES_H / 2;
            //�������µĿջ�˫��
            cWidth = Width - PrintAssign.DOUBLELINES_H;
            SaveCurrentFont();
            SetFont(new Font("����", 9.75F));//����ߵ�����ͳһ����ȷ�����
            //if (Cell.font == null)
            //    SetFont(oPage.BodyFont);
            //else
            //    SetFont(Cell.font);
            SpacingWidth =2+ GetFontWidth(currFont, "0123456789") / 10;
            //while (GetFontWidth(currFont, conStandChar) > SpacingWidth)
            //{
            //    if (!FontZoomOut())
            //        break; //�ǲ����˻���ǿ�л�?������ǿ�л�
            //}
            Font f=new Font("����_GB2312", 9F);
            if (!oPage.CyLine && (bool)(oPage.GetAttributes(PageSetupKey.ePage_���Ҫ��ǧ��λ)))
                s =PrintAssign.CYTITLE_1;
            else
                s = PrintAssign.CYTITLE;
            cX = cX - SpacingWidth;
            StringFormat _sf = new StringFormat();
            _sf.Alignment = StringAlignment.Center;
            _sf.LineAlignment = StringAlignment.Center;
            System.Drawing.Text.TextRenderingHint trh = CurrPrinter.TextRenderingHint;
            System.Drawing.Drawing2D.InterpolationMode im = CurrPrinter.InterpolationMode;
            CurrPrinter.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            CurrPrinter.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            Brush bGreen=new SolidBrush (Color.Green ); 
            while ((s.Length > 1 && cX >= X + PrintAssign.DOUBLELINES_H / 2))
            {
                CurrPrinter.DrawString(s.Substring(s.Length - 1), f, bGreen,new RectangleF ( cX, Y, SpacingWidth, Height),_sf);  
                //DrawText(s.Substring(s.Length - 1), cX, Y, SpacingWidth, Height, PrinterTextAlign.CenterMiddle);
                s = s.Substring(0, s.Length - 1);
                cX = cX - SpacingWidth;
            }
            CurrPrinter.TextRenderingHint = trh;
            CurrPrinter.InterpolationMode = im;
            RestoreFont();
            if (oPage.CyLine)
                DrawCyLine(Cell, X, Y, Width, Height, SpacingWidth, pColor);
        }

        private void DrawCutline()
        {
            DrawCutline(20);
        }
        private void DrawCutline(int CutlineLen)
        {
            Pen pen;
            float X, X1, Y;
            X =  printArrange.Location.X;  
            X1 = printArrange.Location.X + printArrange.Size.Width; 
            Y = printArrange.Location.Y; 

            if (oPage.ColorPrint)
                pen = new Pen(Color.FromArgb(0, 128, 0), float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString()));// 0x8000;
            else
                pen = new Pen(Color.Black, float.Parse(oPage.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString()));

            CurrPrinter.DrawLine(pen, X, Y, X, Y + CutlineLen);
            CurrPrinter.DrawLine(pen, X, Y, X + CutlineLen, Y);

            CurrPrinter.DrawLine(pen, X1, Y + CutlineLen, X1, Y);
            CurrPrinter.DrawLine(pen, X1, Y, X1 - CutlineLen, Y);

            CurrPrinter.DrawLine(pen, X, Y + printArrange.Size.Height - CutlineLen, X, Y + printArrange.Size.Height);
            CurrPrinter.DrawLine(pen, X, Y + printArrange.Size.Height, X + CutlineLen, Y + printArrange.Size.Height);

            CurrPrinter.DrawLine(pen, X1, Y + printArrange.Size.Height - CutlineLen, X1, Y + printArrange.Size.Height);
            CurrPrinter.DrawLine(pen, X1, Y + printArrange.Size.Height, X1 - CutlineLen, Y + printArrange.Size.Height);
        }

        private void DrawFixedRowColInCorner(float X, float Y, float MaxWidth, float MaxHeight, int Pos)
        {
            int i, j;
            PrinterTextAlign CellPos;
            float cX, cY, w, h;
            PrinterCell typCell;
            Rectangle tRect;
            Range oMerg;

            int StartRow, EndRow, StartCol, EndCol;
            StartRow = EndRow = StartCol = EndCol = 0;
            //���ﴦ��̶����н��沿��
            switch (Pos)
            {
                case 1://��
                    StartRow = 1;
                    EndRow = printData.TopFixedRows;
                    StartCol = 1;
                    EndCol = printData.LeftFixedCols;
                    break;
                case 2:// '�Ҷ�
                    StartRow = 1;
                    EndRow = printData.TopFixedRows;
                    StartCol = printData.Cols.Count - printData.RightFixedCols + 1;
                    EndCol = printData.Cols.Count;
                    break;
                case 3:// '���
                    StartRow = printData.Rows.Count - printData.BottomFixedRows + 1;
                    EndRow = printData.Rows.Count;
                    StartCol = 1;
                    EndCol = printData.LeftFixedCols;
                    break;
                case 4:// '�ҵ�
                    StartRow = printData.Rows.Count - printData.BottomFixedRows + 1;
                    EndRow = printData.Rows.Count;
                    StartCol = printData.Cols.Count - printData.RightFixedCols + 1;
                    EndCol = printData.Cols.Count;
                    break;
            }
            if (StartRow <= 0 || StartCol <= 0)
                return;
            for (i = StartRow; i <= EndRow; i++)
            {
                for (j = StartCol; j <= EndCol; j++)
                {
                    typCell = printData.Body.GetItem("$" + i+ "$" + j);
                    if (!printData.InMergedRanges(i, j, out CellPos, out oMerg))
                    {
                        if (CellPos == PrinterTextAlign.LeftTop)
                        {
                            GetRectByRange(StartRow, StartCol, X, Y, oMerg, out tRect);
                            w = tRect.Right - tRect.Left;
                            if (w > MaxWidth)
                                w = MaxWidth;
                            h = tRect.Bottom - tRect.Top;
                            if (h > MaxHeight)
                                h = MaxHeight;
                            DrawCellContent(typCell, tRect.Left, tRect.Top, ref w, ref h, AttachRowType.����, i, j, false, false);
                            DrawBorder(typCell, tRect.Left, tRect.Top, w, h, AttachRowType.����, false, GetCellPos(i, j, StartRow, StartCol, printData.Rows.Count, printData.Cols.Count), i, j);
                        }
                    }
                    else
                    {
                        //����X,Y
                        GetPointByCell(X, Y, StartRow, StartCol, i, j, out cX, out cY);
                        w = printData.Cols.GetItem(j).PrintWidth;
                        h = printData.Rows.GetItem(i).PrintHeight;
                        DrawCellContent(typCell, cX, cY, ref w, ref h, AttachRowType.����, i, j, false, mBodyIncludeFixRange);
                        printData.Cols.GetItem(j).PrintWidth = w;
                        printData.Rows.GetItem(i).PrintHeight = h;
                        DrawBorder(typCell, cX, cY, printData.Cols.GetItem(j).PrintWidth, printData.Rows.GetItem(i).PrintHeight, AttachRowType.����, false, GetCellPos(i, j, StartRow, StartCol, printData.Rows.Count, printData.Cols.Count), i, j);
                    }
                }
            }
        }

        private void DrawLeftFixedCols(int StartRow, int EndRow, float X, float Y, int BlankRow)
        {
            Range oRange, oMerg;
            PrinterTextAlign CellPos;
            float cX, cY;
            PrinterCell typCell = new PrinterCell();
            int StartCol, EndCol, i, j;
            //���ﲻ��������¹̶��н��沿��
            oRange = printData.FixRanges.GetItem("LeftFixedCols");

            if (StartRow < printData.TopFixedRows + 1)
                StartRow = printData.TopFixedRows + 1;
            if (EndRow > printData.Rows.Count - printData.BottomFixedRows)
                EndRow = printData.Rows.Count - printData.BottomFixedRows;
            StartCol = 1;
            EndCol = printData.LeftFixedCols;
            for (i = StartRow; i <= EndRow + BlankRow; i++)
            {
                for (j = StartCol; j <= EndCol; j++)
                {
                    if (printData.InMergedRanges(i, j, out CellPos, out oMerg))
                    {
                        if (CellPos == PrinterTextAlign.LeftTop)
                            DrawMergedRowCol(StartRow, StartCol, EndRow, EndCol, X, Y, BlankRow, oMerg, false);
                    }
                    else
                    {
                        if (i <= EndRow)
                        {
                            typCell = printData.Body.GetItem("$" + i+ "$" + j);
                            //����X , Y
                            GetPointByCell(X, Y, StartRow, StartCol, i, j, out cX, out cY);
                            float w, h;
                            w = printData.Cols.GetItem(j).PrintWidth;
                            h = printData.Rows.GetItem(i).PrintHeight;
                            DrawCellContent(typCell, cX, cY, ref w, ref h, AttachRowType.����, i, j, false, mBodyIncludeFixRange);
                            printData.Cols.GetItem(j).PrintWidth = w;
                            printData.Rows.GetItem(i).PrintHeight = h;
                            DrawBorder(typCell, cX, cY, printData.Cols.GetItem(j).PrintWidth, printData.Rows.GetItem(i).PrintHeight, AttachRowType.����, false, GetCellPos(i, j, StartRow - printData.TopFixedRows, StartCol, EndRow + BlankRow, 257), i, j);
                        }
                        else
                        {
                            typCell = printData.Body.GetItem("$" + EndRow+ "$" +j);
                            GetPointByCell(X, Y, StartRow, StartCol, i, j, out cX, out cY);
                            DrawBorder(typCell, cX, cY, printData.Cols.GetItem(j).PrintWidth, printData.Rows.GetItem(EndRow).PrintHeight, AttachRowType.����, false, GetCellPos(i, j, StartRow - printData.TopFixedRows, StartCol, EndRow + BlankRow, 257), i, j);
                        }
                    }
                }
            }

        }

        private int DrawLogPicture(int X, int Y)
        {
            int offestX, offestY, w, h;
            offestX = offestY = w = h = 0;
            Image oPicture;

            if (!oPage.LogPicPrint)
                return 0;
            if (oPage.LogPic==null )
                return 0;
            try
            {
                oPicture = oPage.LogPic;
            }
            catch
            {
                return 0;
            }
            if (oPage.LogPicLoc != PrinterLogPictureLocation.��������)
            {
                offestX = printArrange.Location.X;//'�߼�ҳ��ˮƽƫ��
                offestY = printArrange.Location.Y;// '�߼�ҳ�Ĵ�ֱƫ��
            }
            w = (int)oPage.GetAttributes(PageSetupKey.ePage_ͼ���ӡ���);
            h = (int)oPage.GetAttributes(PageSetupKey.ePage_ͼ���ӡ�߶�);
            switch (oPage.LogPicLoc)
            {
                case PrinterLogPictureLocation.ҳĩ��:
                    Y = offestY + printArrange.Size.Height - h - 100;
                    X = offestX + (printArrange.Size.Width - w) / 2;
                    break;
                case PrinterLogPictureLocation.���Ͻ�:
                    Y = offestY + 100;
                    X = offestX + printArrange.Size.Width - w - 100;
                    break;
                case PrinterLogPictureLocation.���½�:
                    Y = offestY + printArrange.Size.Height - h - 100;
                    X = offestX + printArrange.Size.Width - w - 100;
                    break;
                case PrinterLogPictureLocation.��������:
                    break;
                case PrinterLogPictureLocation.���Ͻ�:
                    Y = offestY + 100;
                    X = offestX + 100;
                    break;
                case PrinterLogPictureLocation.���½�:
                    Y = offestY + printArrange.Size.Height - h - 100;
                    X = offestX + 100;
                    break;
                default:
                    return 0;
            }
            CurrPrinter.DrawImage(oPicture, X, Y, w, h);
            return h;
        }

        private void DrawLogText()
        {
            int offestX, offestY, X=0, Y=0, w, h;
            string s;
            char[] s1;
            PrinterCell oCell=new PrinterCell() ;
            bool b;

            if (!oPage.LogTextPrint)
                return;
            offestX = printArrange.Location.X   ;//'�߼�ҳ��ˮƽƫ��
            offestY = printArrange.Location.Y ;//  '�߼�ҳ�Ĵ�ֱƫ��
            s = oPage.LogText.Trim();
            if (s.Length <= 0)
                return;
            s1 = s.ToCharArray();
            SaveCurrentFont();
            if (oPage.LogTextFont == null)
                oPage.LogTextFont = new Font("����", 16, FontStyle.Bold);
            SetFont(oPage.LogTextFont);
            w =(int) GetFontWidth(currFont, s);
            h = (int)GetFontHeight(currFont, s);
            b = true;
            if (oPage.LogTextBorder == PrinterLogTextBorder.ȫ������)
            {
                oCell.B_Style = oPage.LogTextBorderStyle;
                oCell.L_Style = oPage.LogTextBorderStyle;
                oCell.R_Style = oPage.LogTextBorderStyle;
                oCell.T_Style = oPage.LogTextBorderStyle;
            }
            else if (oPage.LogTextBorder == PrinterLogTextBorder.�ϱ���)
                oCell.T_Style = oPage.LogTextBorderStyle;
            else if (oPage.LogTextBorder == PrinterLogTextBorder.�±���)
                oCell.B_Style = oPage.LogTextBorderStyle;
            else if (oPage.LogTextBorder == PrinterLogTextBorder.�ұ���)
                oCell.R_Style = oPage.LogTextBorderStyle;
            else if (oPage.LogTextBorder == PrinterLogTextBorder.�����)
                oCell.L_Style = oPage.LogTextBorderStyle;
            else
                b = false;
            ///
            //
            ///
            switch (oPage.LogTextLoc)
            {
                case PrinterLogTextLocation.���к��:
                    Y = offestY + 100;
                    X = offestX + (int)(pageArrange.Arrange(1).Size.Width - w) / 2;
                    DrawText(s, X, Y, w, h, PrinterTextAlign.LeftMiddle);
                    break;
                case PrinterLogTextLocation.���к��:
                    Y = (int)(offestY + pageArrange.Arrange(1).Size.Height - h - 100);
                    X = (int)(offestX + (pageArrange.Arrange(1).Size.Width - w) / 2);
                    DrawText(s, X, Y, w, h, PrinterTextAlign.LeftMiddle);
                    break;
                case PrinterLogTextLocation.���Ϻ��:
                    Y = offestY + 100;
                    X = (int)(offestX + pageArrange.Arrange(1).Size.Width - w - 100);
                    DrawText(s, X, Y, w, h, PrinterTextAlign.LeftMiddle);
                    break;
                case PrinterLogTextLocation.���º��:
                    Y = (int) (offestY + pageArrange.Arrange(1).Size.Height - h - 100);
                    X = (int)(offestX + pageArrange.Arrange(1).Size.Width - w - 100);
                    DrawText(s, X, Y, w, h, PrinterTextAlign.LeftMiddle);
                    break;
                case PrinterLogTextLocation.���Ϻ��:
                    Y = offestY + 100;
                    X = offestX + 100;
                    DrawText(s, X, Y, w, h, PrinterTextAlign.LeftMiddle);
                    break;
                case PrinterLogTextLocation.���º��:
                    Y = (int) (offestY + pageArrange.Arrange(1).Size.Height - h - 10);
                    X = offestX + 100;
                    DrawText(s, X, Y, w, h, PrinterTextAlign.LeftMiddle);
                    break;
                case PrinterLogTextLocation.��������:
                    w =(int) ( GetFontWidth(currFont, conStandChar) + 45);
                    h =(int) ( GetFontHeight(currFont, conStandChar) * s.Length + s.Length * PrintAssign.DOUBLELINES_V);
                    Y = offestY + 100;
                    X = (int)(offestX + pageArrange.Arrange(1).Size.Width - w - 100);
                    DrawTextMultiRow(s1, X, Y, w, h + s.Length * PrintAssign.DOUBLELINES_V, h, PrinterTextAlign.LeftMiddle);
                    break;
                case PrinterLogTextLocation.��������:
                    w = (int) (GetFontWidth(currFont, conStandChar) + 45);
                    h =(int) ( GetFontHeight(currFont, conStandChar) * s.Length + s.Length * PrintAssign.DOUBLELINES_V);
                    Y =(int) ( offestY + pageArrange.Arrange(1).Size.Height - h - 100);
                    X = (int)(offestX + pageArrange.Arrange(1).Size.Width - w - 100);
                    DrawTextMultiRow(s1, X, Y, w, h + s.Length * PrintAssign.DOUBLELINES_V, h, PrinterTextAlign.LeftMiddle);
                    break;
                case PrinterLogTextLocation.��������:
                    w =(int) ( GetFontWidth(currFont, conStandChar) + 45);
                    h =(int) ( GetFontHeight(currFont, conStandChar) * s.Length + s.Length * PrintAssign.DOUBLELINES_V);
                    Y =(int) ( offestY + (pageArrange.Arrange(1).Size.Height + oPage.TopMargin + oPage.BottomMargin - h) / 2);
                    X = (int)(offestX + pageArrange.Arrange(1).Size.Width - w - 100);
                    DrawTextMultiRow(s1, X, Y, w, h + s.Length * PrintAssign.DOUBLELINES_V, h, PrinterTextAlign.LeftMiddle);
                    break;
                case PrinterLogTextLocation.��������:
                    w =(int) ( GetFontWidth(currFont, conStandChar) + 45);
                    h =(int) ( GetFontHeight(currFont, conStandChar) * s.Length + s.Length * PrintAssign.DOUBLELINES_V);
                    Y = offestY + 100;
                    X = offestX + 100;
                    DrawTextMultiRow(s1, X, Y, w, h + s.Length * PrintAssign.DOUBLELINES_V, h, PrinterTextAlign.LeftMiddle);
                    break;
                case PrinterLogTextLocation.��������:
                    w =(int) ( GetFontWidth(currFont, conStandChar) + 45);
                    h =(int) ( GetFontHeight(currFont, conStandChar) * s.Length + s.Length * PrintAssign.DOUBLELINES_V);
                    Y =(int) ( offestY + pageArrange.Arrange(1).Size.Height - h - 100);
                    X = offestX + 100;
                    DrawTextMultiRow(s1, X, Y, w, h + s.Length * PrintAssign.DOUBLELINES_V, h, PrinterTextAlign.LeftMiddle);
                    break;
                case PrinterLogTextLocation.��������:
                    w =(int) ( GetFontWidth(currFont, conStandChar) + 45);
                    h =(int) ( GetFontHeight(currFont, conStandChar) * s.Length + s.Length * PrintAssign.DOUBLELINES_V);
                    Y =(int) ( offestY + (pageArrange.Arrange(1).Size.Height + oPage.TopMargin + oPage.BottomMargin - h) / 2);
                    X = offestX + 100;
                    DrawTextMultiRow(s1, X, Y, w, h + s.Length * PrintAssign.DOUBLELINES_V, h, PrinterTextAlign.LeftMiddle);
                    break;
            }
            if (b)
                DrawBorder(oCell, X, Y, w, h, AttachRowType.����);
            RestoreFont();
        }

        private void DrawMergedRowCol(int StartRow, int StartCol, int EndRow, int EndCol, float X, float Y, int BlankRow, Range oMerg)
        {
            DrawMergedRowCol(StartRow, StartCol, EndRow, EndCol, X, Y, BlankRow, oMerg, true);
        }
        private void DrawMergedRowCol(int StartRow, int StartCol, int EndRow, int EndCol, float X, float Y, int BlankRow, Range oMerg, bool IsKeepSameFontSize)
        {
            Rectangle tRect;
            PrinterCell typCell = new PrinterCell();
            PrinterTextAlign CellPos;
            int i, j, h1;
            float h, w, cX, cY, Width, Height;

            h = w = h1 = 0;
            for (i = StartRow; i <= EndRow; i++)
            {
                h = h + printData.Rows.GetItem(i).PrintHeight;
            }
            for (i = 1; i <= BlankRow; i++)
            {
                h1 = h1 + (int)(printData.Rows.GetItem(StartRow).PrintHeight);
            }
            for (i = StartCol; i <= EndCol; i++)
            {
                w = w + printData.Cols.GetItem(i).PrintWidth;
            }
            GetRectByRange(StartRow, StartCol, X, Y, oMerg, out tRect);
            Width = tRect.Right - tRect.Left;
            Height = tRect.Bottom - tRect.Top;

            if (Width > w)
                Width = w;
            if (Height > h)
                Height = h;

            Height = Height + h1;
            if (oMerg.EndCol <= EndCol)
                EndCol = oMerg.EndCol;
            if (oMerg.StartCol >= StartCol)
                StartCol = oMerg.StartCol;
            if (oMerg.EndRow <= EndRow)
                EndRow = oMerg.EndRow;
            if (oMerg.StartRow >= StartRow)
                StartRow = oMerg.StartRow;
            for (i = StartRow; i <= EndRow; i++)
            {
                for (j = StartCol; j <= EndCol; j++)
                {
                    if (printData.InMergedRanges(i, j, out  CellPos))
                    {
                        typCell = printData.Body.GetItem("$" + i+ "$" + j);
                        GetPointByCell(tRect.Left, tRect.Top, StartRow, StartCol, i, j, out cX, out cY);
                        switch (CellPos)
                        {
                            case PrinterTextAlign.LeftTop:
                                DrawCellContent(typCell, tRect.Left, tRect.Top, ref Width, ref Height, AttachRowType.����, i, j, false, IsKeepSameFontSize);
                                if (oMerg.EndRow != oMerg.StartRow)
                                    typCell.B_Style = PrinterBorderStyle.�ޱ߿�;
                                if (oMerg.EndCol != oMerg.StartCol)
                                    typCell.R_Style = PrinterBorderStyle.�ޱ߿�;
                                DrawBorder(typCell, cX, cY, printData.Cols.GetItem(j).PrintWidth, printData.Rows.GetItem(i).PrintHeight, AttachRowType.����, false, PrinterCellPos.cellPos_LeftTop);
                                break;
                            case PrinterTextAlign.CenterBottom:
                                typCell.L_Style = PrinterBorderStyle.�ޱ߿�;
                                typCell.R_Style = PrinterBorderStyle.�ޱ߿�;
                                typCell.T_Style = PrinterBorderStyle.�ޱ߿�;
                                DrawBorder(typCell, cX, cY, printData.Cols.GetItem(j).PrintWidth, printData.Rows.GetItem(i).PrintHeight, AttachRowType.����, false, PrinterCellPos.cellPos_CenterBottom);
                                break;
                            case PrinterTextAlign.CenterTop:
                                typCell.L_Style = PrinterBorderStyle.�ޱ߿�;
                                typCell.R_Style = PrinterBorderStyle.�ޱ߿�;
                                if (oMerg.EndRow != oMerg.StartRow)
                                    typCell.B_Style = PrinterBorderStyle.�ޱ߿�;
                                DrawBorder(typCell, cX, cY, printData.Cols.GetItem(j).PrintWidth, printData.Rows.GetItem(i).PrintHeight, AttachRowType.����, false, PrinterCellPos.cellPos_CenterTop);
                                break;
                            case PrinterTextAlign.LeftBottom:
                                if (oMerg.EndCol != oMerg.StartCol)
                                    typCell.R_Style = PrinterBorderStyle.�ޱ߿�;
                                typCell.T_Style = PrinterBorderStyle.�ޱ߿�;
                                DrawBorder(typCell, cX, cY, printData.Cols.GetItem(j).PrintWidth, printData.Rows.GetItem(i).PrintHeight, AttachRowType.����, false, PrinterCellPos.cellPos_LeftBottom);
                                break;
                            case PrinterTextAlign.LeftMiddle:
                                if (oMerg.EndCol != oMerg.StartCol)
                                    typCell.R_Style = PrinterBorderStyle.�ޱ߿�;
                                typCell.T_Style = PrinterBorderStyle.�ޱ߿�;
                                typCell.B_Style = PrinterBorderStyle.�ޱ߿�;
                                DrawBorder(typCell, cX, cY, printData.Cols.GetItem(j).PrintWidth, printData.Rows.GetItem(i).PrintHeight, AttachRowType.����, false, PrinterCellPos.cellPos_LeftMiddle);
                                break;
                            case PrinterTextAlign.RightBottom:
                                typCell.L_Style = PrinterBorderStyle.�ޱ߿�;
                                typCell.T_Style = PrinterBorderStyle.�ޱ߿�;
                                DrawBorder(typCell, cX, cY, printData.Cols.GetItem(j).PrintWidth, printData.Rows.GetItem(i).PrintHeight, AttachRowType.����, false, PrinterCellPos.cellPos_RightBottom);
                                break;
                            case PrinterTextAlign.RightMiddle:
                                typCell.L_Style = PrinterBorderStyle.�ޱ߿�;
                                typCell.T_Style = PrinterBorderStyle.�ޱ߿�;
                                typCell.B_Style = PrinterBorderStyle.�ޱ߿�;
                                DrawBorder(typCell, cX, cY, printData.Cols.GetItem(j).PrintWidth, printData.Rows.GetItem(i).PrintHeight, AttachRowType.����, false, PrinterCellPos.cellPos_RightBottom);
                                break;
                            case PrinterTextAlign.RightTop:
                                typCell.L_Style = PrinterBorderStyle.�ޱ߿�;
                                if (oMerg.EndRow != oMerg.StartRow)
                                    typCell.B_Style = PrinterBorderStyle.�ޱ߿�;
                                DrawBorder(typCell, cX, cY, printData.Cols.GetItem(j).PrintWidth, printData.Rows.GetItem(i).PrintHeight, AttachRowType.����, false, PrinterCellPos.cellPos_RightTop);
                                break;
                        }
                        if (CellPos != PrinterTextAlign.LeftTop && (StartRow > oMerg.StartRow && EndRow <= oMerg.EndRow))
                        {
                            printData.Body.SetItem("$" + oMerg.StartRow+ "$" + oMerg.StartCol, typCell);
                            DrawCellContent(typCell, tRect.Left, tRect.Top, ref Width, ref Height, AttachRowType.����, i, j, false, IsKeepSameFontSize);
                        }

                    }
                }
            }
        }

        private void DrawPictures(int StartRow, int StartCol, int EndRow, int EndCol)
        {
            Rectangle picRect;
            int i;

            for (i = 1; i <= printData.Pictures.Count; i++)
            {
                picRect = printData.Pictures.GetItem(i).PictureRect;
                if (printData.Pictures.GetItem(i).StartRow >= StartRow)
                {
                    if (printData.Pictures.GetItem(i).StartCol >= StartCol)
                        CurrPrinter.DrawImage(printData.Pictures.GetItem(i).Content, picRect.Left, picRect.Top, (picRect.Right - picRect.Left), (picRect.Bottom - picRect.Top));
                }
            }
        }

        private void DrawRightFixedCols(int StartRow, int EndRow, float X, float Y, int BlankRow)
        {
            Range oRange, oMerg;
            PrinterTextAlign CellPos;
            float cX, cY;
            PrinterCell typCell;
            int i, j, StartCol, EndCol;
            //���ﲻ��������¹̶��н��沿��
            oRange = printData.FixRanges.GetItem("RightFixedCols");
            if (StartRow < printData.TopFixedRows + 1)
                StartRow = printData.TopFixedRows + 1;
            if (EndRow > printData.Rows.Count - printData.BottomFixedRows)
                EndRow = printData.Rows.Count - printData.BottomFixedRows;
            StartCol = printData.Cols.Count - printData.RightFixedCols + 1;
            EndCol = printData.Cols.Count;
            for (i = StartRow; i <= EndRow + BlankRow; i++)
            {
                for (j = StartCol; j <= EndCol; j++)
                {
                    if (printData.InMergedRanges(i, j, out CellPos, out oMerg))
                    {
                        if (CellPos == PrinterTextAlign.LeftTop || (StartRow > oMerg.StartRow && EndRow <= oMerg.EndRow))
                            DrawMergedRowCol(StartRow, StartCol, EndRow, EndCol, X, Y, BlankRow, oMerg, false);
                    }
                    else
                    {
                        if (i <= EndRow)
                        {
                            typCell = printData.Body.GetItem("$" + i+ "$" + j);
                            //����X,Y
                            GetPointByCell(X, Y, StartRow, StartCol, i, j, out cX, out cY);
                            float w, h;
                            w = printData.Cols.GetItem(j).PrintWidth;
                            h = printData.Rows.GetItem(i).PrintHeight;
                            DrawCellContent(typCell, cX, cY, ref w, ref h, AttachRowType.����, i, j, false, mBodyIncludeFixRange);
                            printData.Cols.GetItem(j).PrintWidth = w;
                            printData.Rows.GetItem(i).PrintHeight = h;
                            DrawBorder(typCell, cX, cY, printData.Cols.GetItem(j).PrintWidth, printData.Rows.GetItem(i).PrintHeight, AttachRowType.����, false, GetCellPos(i, j, StartRow - printData.TopFixedRows, 1, EndRow + BlankRow, EndCol), i, j);
                        }
                        else
                        {
                            typCell = printData.Body.GetItem("$" + EndRow+ "$" + j);
                            GetPointByCell(X, Y, StartRow, StartCol, i, j, out cX, out cY);
                            DrawBorder(typCell, cX, cY, printData.Cols.GetItem(j).PrintWidth, printData.Rows.GetItem(EndRow).PrintHeight, AttachRowType.����, false, GetCellPos(i, j, StartRow - printData.TopFixedRows, 1, EndRow + BlankRow, EndCol), i, j);

                        }
                    }
                }
            }
        }

        private void DrawText(string text, float X, float Y, float Width, float Height, PrinterTextAlign Align)
        {
            float pX = 0, pY = 0, pWidth, pHeight;
            typCellEx[] t = new typCellEx[1];
            int i;
            string s="";
            bool isTextEx = false;
            //��ǰ���Ƶ����Ͻ���pX,pY����
            isTextEx = (SplitTextToCellEx(text, currColor.ToArgb(), ref t));
            if (isTextEx)
            {
                foreach (typCellEx ti in t)
                {
                        s += ti.text;
                }
            }
            else
                s = GetCellText(text);

            pWidth = Width - GetFontWidth(currFont, s);
            pHeight = Height - GetFontHeight(currFont, s);
            if (pWidth < 0)
                pWidth = 0;
            if (pHeight < 0)
                pHeight = 0;
            switch (Align)
            {
                case PrinterTextAlign.CenterBottom:
                    {
                        pX = X + pWidth / 2;
                        pY = Y + pHeight;
                        break;
                    }
                case PrinterTextAlign.CenterMiddle:
                    {
                        pX = X + pWidth / 2;
                        pY = Y + pHeight / 2;
                        break;
                    }
                case PrinterTextAlign.CenterTop:
                    {
                        pX = X + pWidth / 2;
                        pY = Y;
                        break;
                    }
                case PrinterTextAlign.RightBottom:
                    {
                        pX = X + pWidth;// -PrintAssign.TEXTBOX_OFFSET_H;
                        pY = Y + pHeight;
                        break;
                    }
                case PrinterTextAlign.RightMiddle:
                    {
                        pX = X + pWidth;// -PrintAssign.TEXTBOX_OFFSET_H;
                        pY = Y + pHeight / 2;
                        break;
                    }
                case PrinterTextAlign.RightTop:
                    {
                        pX = X + pWidth;// -PrintAssign.TEXTBOX_OFFSET_H;
                        pY = Y;
                        break;
                    }
                case PrinterTextAlign.LeftMiddle:
                    {
                        pX = X;
                        pY = Y + pHeight / 2;
                        break;
                    }
                case PrinterTextAlign.LeftBottom:
                    {
                        pX = X;
                        pY = Y + pHeight;
                        break;
                    }
                case PrinterTextAlign.LeftTop:
                    {
                        pX = X;
                        pY = Y;
                        break;
                    }
                default:
                    break;
            }
            textBrush.Color = currColor;
            if (isTextEx)
            {
                for (i = 0; i < t.Length; i++)
                {
                    textBrush.Color = Color.FromArgb(t[i].color);
                    CurrPrinter.DrawString(t[i].text, currFont, textBrush, pX, pY, new StringFormat(StringFormatFlags.NoWrap));
                    textBrush.Color = currColor;
                    pX = pX + GetFontWidth(currFont, t[i].text);
                }
            }
            else
                CurrPrinter.DrawString(text, currFont, textBrush, pX, pY, new StringFormat(StringFormatFlags.NoWrap));
            //CurrPrinter.DrawLine(currPen, pX, pY, 0, 0);
        }

        private void DrawTextMultiRow(char[] text, float X, float Y, float Width, float Height, float RealHeight, PrinterTextAlign Align)
        {
            float oldX, oldY, pX, pY, pWidth, pHeight;
            int i = 0;
            oldX = oldY = pX = pY = pWidth = pHeight = 0;
            //�л���ʱ,ˮƽ������ÿ�������ı��Ŀ�ȼ�����뷽ʽ.��ֱ�����������忼��
            //With CurrPrinter
            //��ǰ���Ƶ����Ͻ���pX,pY����
            pHeight = Height - RealHeight;
            oldX = CurrentX;
            oldY = CurrentY;
            for (i = 0; i <= text.Length; i++)
            {
                pWidth = Width - GetFontWidth(currFont, text[i]);
                switch (Align)
                {
                    case PrinterTextAlign.CenterBottom:
                        pX = X + pWidth / 2;
                        if (i == 0) pY = Y + pHeight;
                        break;
                    case PrinterTextAlign.CenterMiddle:
                        pX = X + pWidth / 2;
                        if (i == 0) pY = Y + pHeight / 2;
                        break;
                    case PrinterTextAlign.CenterTop:
                        pX = X + pWidth / 2;
                        if (i == 0) pY = Y;
                        break;
                    case PrinterTextAlign.RightBottom:
                        pX = X + pWidth;// -PrintAssign.TEXTBOX_OFFSET_H;
                        if (i == 0) pY = Y + pHeight;
                        break;
                    case PrinterTextAlign.RightMiddle:
                        pX = X + pWidth;// -PrintAssign.TEXTBOX_OFFSET_H;
                        if (i == 0) pY = Y + pHeight / 2;
                        break;
                    case PrinterTextAlign.RightTop:
                        pX = X + pWidth;// -PrintAssign.TEXTBOX_OFFSET_H;
                        if (i == 0) pY = Y;
                        break;
                    case PrinterTextAlign.LeftMiddle:
                        pX = X;
                        if (i == 0) pY = Y + pHeight / 2;
                        break;
                    case PrinterTextAlign.LeftBottom:
                        pX = X;
                        if (i == 0) pY = Y + pHeight;
                        break;
                    case PrinterTextAlign.LeftTop:
                        pX = X;
                        if (i == 0) pY = Y;
                        break;
                }
                CurrPrinter.DrawString(text[i].ToString (), currFont, textBrush, pX, pY, new StringFormat(StringFormatFlags.NoWrap));
                if (text[i].ToString().Length == 0)
                    pY = pY + GetFontHeight(currFont, "��") / 3;
                else
                    pY = pY + GetFontHeight(currFont, text[i]) + 15;
            }

        }
        private void DrawTextMultiRow(string[] text, float X, float Y, float Width, float Height, float RealHeight, PrinterTextAlign Align)
        {
            float oldX, oldY, pX, pY, pWidth, pHeight;
            int i = 0;
            oldX = oldY = pX = pY = pWidth = pHeight = 0;
            //�л���ʱ,ˮƽ������ÿ�������ı��Ŀ�ȼ�����뷽ʽ.��ֱ�����������忼��
            //With CurrPrinter
            //��ǰ���Ƶ����Ͻ���pX,pY����
            pHeight = Height - RealHeight;
            oldX = CurrentX;
            oldY = CurrentY;
            for (i = 0; i <= text.Length; i++)
            {
                pWidth = Width - GetFontWidth(currFont, text[i]);
                switch (Align)
                {
                    case PrinterTextAlign.CenterBottom:
                        pX = X + pWidth / 2;
                        if (i == 0) pY = Y + pHeight;
                        break;
                    case PrinterTextAlign.CenterMiddle:
                        pX = X + pWidth / 2;
                        if (i == 0) pY = Y + pHeight / 2;
                        break;
                    case PrinterTextAlign.CenterTop:
                        pX = X + pWidth / 2;
                        if (i == 0) pY = Y;
                        break;
                    case PrinterTextAlign.RightBottom:
                        pX = X + pWidth;// -PrintAssign.TEXTBOX_OFFSET_H;
                        if (i == 0) pY = Y + pHeight;
                        break;
                    case PrinterTextAlign.RightMiddle:
                        pX = X + pWidth;// -PrintAssign.TEXTBOX_OFFSET_H;
                        if (i == 0) pY = Y + pHeight / 2;
                        break;
                    case PrinterTextAlign.RightTop:
                        pX = X + pWidth;// -PrintAssign.TEXTBOX_OFFSET_H;
                        if (i == 0) pY = Y;
                        break;
                    case PrinterTextAlign.LeftMiddle:
                        pX = X;
                        if (i == 0) pY = Y + pHeight / 2;
                        break;
                    case PrinterTextAlign.LeftBottom:
                        pX = X;
                        if (i == 0) pY = Y + pHeight;
                        break;
                    case PrinterTextAlign.LeftTop:
                        pX = X;
                        if (i == 0) pY = Y;
                        break;
                }
                CurrPrinter.DrawString(text[i], currFont, textBrush, pX, pY, new StringFormat(StringFormatFlags.NoWrap));
                if (text[i].Length == 0)
                    pY = pY + GetFontHeight(currFont, "��") / 3;
                else
                    pY = pY + GetFontHeight(currFont, text[i]) + 15;
            }
        }
        private void DrawTextMultiRow(typCellEx[] text, float X, float Y, float Width, float Height, float RealHeight, PrinterTextAlign Align)
        {
            float oldX, oldY, pX, pY, pWidth, pHeight;
            int i;
            oldX = oldY = pX = pY = pWidth = pHeight = 0;
            //�л���ʱ,ˮƽ������ÿ�������ı��Ŀ�ȼ�����뷽ʽ.��ֱ�����������忼��
            //With CurrPrinter
            //��ǰ���Ƶ����Ͻ���pX,pY����
            pHeight = Height - RealHeight;
            oldX = CurrentX;
            oldY = CurrentY;
            for (i = 0; i < text.Length; i++)
            {
                pWidth = Width - GetFontWidth(currFont, text[i].text);
                switch (Align)
                {
                    case PrinterTextAlign.CenterBottom:
                        pX = X + pWidth / 2;
                        if (i == 0) pY = Y + pHeight;
                        break;
                    case PrinterTextAlign.CenterMiddle:
                        pX = X + pWidth / 2;
                        if (i == 0) pY = Y + pHeight / 2;
                        break;
                    case PrinterTextAlign.CenterTop:
                        pX = X + pWidth / 2;
                        if (i == 0) pY = Y;
                        break;
                    case PrinterTextAlign.RightBottom:
                        pX = X + pWidth;// -PrintAssign.TEXTBOX_OFFSET_H;
                        if (i == 0) pY = Y + pHeight;
                        break;
                    case PrinterTextAlign.RightMiddle:
                        pX = X + pWidth;// -PrintAssign.TEXTBOX_OFFSET_H;
                        if (i == 0) pY = Y + pHeight / 2;
                        break;
                    case PrinterTextAlign.RightTop:
                        pX = X + pWidth;// -PrintAssign.TEXTBOX_OFFSET_H;
                        if (i == 0) pY = Y;
                        break;
                    case PrinterTextAlign.LeftMiddle:
                        pX = X;
                        if (i == 0) pY = Y + pHeight / 2;
                        break;
                    case PrinterTextAlign.LeftBottom:
                        pX = X;
                        if (i == 0) pY = Y + pHeight;
                        break;
                    case PrinterTextAlign.LeftTop:
                        pX = X;
                        if (i == 0) pY = Y;
                        break;
                }
                if (textBrush.Color != Color.FromArgb(text[i].color))
                    textBrush.Color = Color.FromArgb(text[i].color);
                CurrPrinter.DrawString(text[i].text, currFont, textBrush, pX, pY, new StringFormat(StringFormatFlags.NoWrap));
                if (textBrush.Color != currColor)
                    textBrush.Color = currColor;
                if (text[i].text.Length == 0)
                    pY = pY + GetFontHeight(currFont, "��") / 3;
                else
                    pY = pY + GetFontHeight(currFont, text[i].text) + 15;
            }
        }

        private void DrawTheAttachItemBorder(PrinterCell cell, int BorderPos, int PenColor, PrinterBorderStyle PenStyle, float X, float Y, float X2, float Y2, PrinterTextAlign CellPos)
        {
            Pen pen = new Pen(Color.Black, (float)printAssign.PageSetup.GetAttributes(PageSetupKey.ePage_ϸ�߿��));
            switch (PenStyle)
            {
                case PrinterBorderStyle.���߱߿�:
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    break;
                case PrinterBorderStyle.���߱߿�:
                    pen.DashPattern = new float[] { 4.0F, 2.0F, 1.0F, 3.0F };
                    break;
                case PrinterBorderStyle.�̵��߱߿�:
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
                    break;
                case PrinterBorderStyle.�̵���߱߿�:
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
                    break;
                default:
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    break;
            }
            if (PenStyle != PrinterBorderStyle.�ޱ߿�)
            {
                pen.Color = Color.FromArgb(PenColor);
                if (PenStyle == PrinterBorderStyle.˫ʵ�߱߿�)
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    if (X == X2)
                    {
                        CurrPrinter.DrawLine(pen, X - PrintAssign.DOUBLELINES_H / 4, Y, X2 - PrintAssign.DOUBLELINES_H / 4, Y2);
                        CurrPrinter.DrawLine(pen, X + PrintAssign.DOUBLELINES_H / 4, Y, X2 + PrintAssign.DOUBLELINES_H / 4, Y2);
                    }
                    else if (Y == Y2)
                    {
                        CurrPrinter.DrawLine(pen, X, Y - PrintAssign.DOUBLELINES_V / 4, X2, Y2 - PrintAssign.DOUBLELINES_V / 4);
                        CurrPrinter.DrawLine(pen, X, Y + PrintAssign.DOUBLELINES_V / 4, X2, Y2 + PrintAssign.DOUBLELINES_V / 4);
                    }
                }
                else
                    CurrPrinter.DrawLine(pen, X, Y, X2, Y2);
            }
        }

        private void DrawTheBorder(PrinterCell cell, int BorderPos, int PenColor, PrinterBorderStyle PenStyle, float X, float Y, float X2, float Y2, PrinterCellPos CellPos, int Row, int Col)
        {
            //BorderPos=1,2,3,4-->���ҵ�
            int offset1 = 0, offset2 = 0;// '�������ߵ�˫�߱߿��ܽ���
            int offset3 = 0, offset4 = 0;// '�������ߵ�˫�߱߿��ܽ���
            PrinterCell aroundCell = new PrinterCell();
            Pen pen = new Pen(Color.Black, float.Parse(printAssign.PageSetup.GetAttributes(PageSetupKey.ePage_ϸ�߿��).ToString()));
            switch (PenStyle)
            {
                case PrinterBorderStyle.���߱߿�:
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    break;
                case PrinterBorderStyle.���߱߿�:
                    pen.DashPattern = new float[] { 4.0F, 2.0F, 1.0F, 3.0F };
                    break;
                case PrinterBorderStyle.�̵��߱߿�:
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
                    break;
                case PrinterBorderStyle.�̵���߱߿�:
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
                    break;
                default:
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    break;
            }

            if (PenStyle != PrinterBorderStyle.�ޱ߿�)
            {
                if (PenColor!=0)
                    pen.Color = Color.FromArgb(PenColor);
                switch (BorderPos)
                {
                    case 1://LEFT
                        //*�ж��������Ƿ���˫ʵ�߱߿�,��������
                        if (cell.T_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                            offset1 = PrintAssign.DOUBLELINES_V / 4;
                        if (cell.B_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                            offset2 = PrintAssign.DOUBLELINES_V / 4;
                        //*/
                        //*�ж��ھ������Ƿ���PrinterBorderStyle.˫ʵ�߱߿�,��������
                        switch (CellPos)
                        {
                            case PrinterCellPos.cellPos_CenterMiddle:
                                aroundCell = printData.Body.GetItem("$" + (Row - 1)+ "$" +( Col - 1));
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.B_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset3 = PrintAssign.DOUBLELINES_V / 4;

                                }
                                aroundCell = printData.Body.GetItem("$" + (Row + 1)+ "$" +( Col - 1));
                                {
                                    if (aroundCell.T_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset4 = PrintAssign.DOUBLELINES_V / 4;
                                }
                                break;
                            case PrinterCellPos.cellPos_CenterBottom:
                                aroundCell = printData.Body.GetItem("$" + (Row - 1)+"$" + Col );
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.B_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset3 = PrintAssign.DOUBLELINES_V / 4;
                                    else
                                        offset3 = offset1;
                                }
                                offset4 = offset2;
                                break;
                            case PrinterCellPos.cellPos_CenterTop:
                                aroundCell = printData.Body.GetItem("$" + (Row + 1)+ "$" + Col );
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.T_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                    {
                                        offset4 = PrintAssign.DOUBLELINES_V / 4;
                                        offset3 = PrintAssign.DOUBLELINES_V / 4;
                                    }
                                    else
                                        offset3 = offset1;
                                }
                                break;
                            case PrinterCellPos.cellPos_LeftBottom:
                                offset4 = -1 * offset2;
                                break;
                            case PrinterCellPos.cellPos_LeftTop:
                                offset3 = -1 * offset1;
                                break;
                            case PrinterCellPos.cellPos_RightMiddle:
                                aroundCell = printData.Body.GetItem("$" + Row+ "$" +( Col - 1));
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.R_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                    {
                                        offset4 = PrintAssign.DOUBLELINES_V / 4;
                                        offset3 = PrintAssign.DOUBLELINES_V / 4;
                                    }
                                    if (CellPos == PrinterCellPos.cellPos_RightBottom)
                                        offset4 = offset2;
                                }
                                break;
                            case PrinterCellPos.cellPos_RightBottom:
                            case PrinterCellPos.cellPos_RightTop:
                                goto case PrinterCellPos.cellPos_RightMiddle;
                            case PrinterCellPos.cellPos_RightTopBottom:
                                goto case PrinterCellPos.cellPos_RightMiddle;
                            case PrinterCellPos.cellPos_TopLeftRight:
                                //'һ�㷢�����ҹ̶���ֻ��һ��ʱ
                                aroundCell = printData.Body.GetItem("$" + Row+ "$" +( Col - 1));
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.T_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                    {
                                        offset3 = PrintAssign.DOUBLELINES_V / 4;
                                        offset1 = -1 * PrintAssign.DOUBLELINES_V / 4;
                                    }
                                    if (aroundCell.B_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                    {
                                        offset2 = -1 * PrintAssign.DOUBLELINES_V / 4;
                                        offset4 = PrintAssign.DOUBLELINES_V / 4;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        //*/
                        if (cell.L_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                        {    //����
                            CurrPrinter.DrawLine(pen, X + PrintAssign.DOUBLELINES_H / 4, Y + offset1, X2 + PrintAssign.DOUBLELINES_H / 4, Y2 - offset2);
                            //����
                            CurrPrinter.DrawLine(pen, X - PrintAssign.DOUBLELINES_H / 4, Y + offset3, X2 - PrintAssign.DOUBLELINES_H / 4, Y2 - offset4);
                        }
                        else
                        {
                            if (CellPos == PrinterCellPos.cellPos_LeftBottom || CellPos == PrinterCellPos.cellPos_LeftMiddle)
                            {
                                offset1 = 0;
                                offset2 = 0;
                            }
                            CurrPrinter.DrawLine(pen, X, Y + offset1, X2, Y2 - offset2);
                        }
                        break;
                    case 2://Top
                        if (cell.L_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                            offset1 = PrintAssign.DOUBLELINES_H / 4;
                        if (cell.R_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                            offset2 = PrintAssign.DOUBLELINES_H / 4;
                        switch (CellPos)
                        {
                            case PrinterCellPos.cellPos_CenterMiddle:
                                aroundCell = printData.Body.GetItem("$" + Row+ "$" +( Col - 1));
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.R_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset3 = PrintAssign.DOUBLELINES_H / 4;
                                }
                                aroundCell = printData.Body.GetItem("$" + Row+ "$" +( Col + 1));
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.L_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset4 = PrintAssign.DOUBLELINES_H / 4;
                                }
                                break;
                            case PrinterCellPos.cellPos_CenterBottom:
                                goto case PrinterCellPos.cellPos_CenterMiddle;
                            case PrinterCellPos.cellPos_LeftMiddle:
                                aroundCell = printData.Body.GetItem("$" + Row+ "$" +( Col + 1));
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.L_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset3 = PrintAssign.DOUBLELINES_H / 4;
                                    else
                                        offset3 = offset1;
                                }
                                break;
                            case PrinterCellPos.cellPos_LeftBottom:
                                goto case PrinterCellPos.cellPos_LeftMiddle;

                            case PrinterCellPos.cellPos_CenterTop:
                                aroundCell = printData.Body.GetItem("$" + Row+ "$" +( Col + 1));
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.L_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset4 = -1 * PrintAssign.DOUBLELINES_H / 4;
                                    else
                                        offset4 = offset2;
                                }
                                break;
                            case PrinterCellPos.cellPos_RightMiddle:
                                aroundCell = printData.Body.GetItem("$" + Row+ "$" +( Col - 1));
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.R_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset4 = PrintAssign.DOUBLELINES_H / 4;
                                    else
                                        offset4 = offset2;
                                }
                                break;
                            case PrinterCellPos.cellPos_RightBottom:
                                goto case PrinterCellPos.cellPos_RightMiddle;
                            default:
                                break;

                        }
                        if (cell.T_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                        {
                            //����
                            CurrPrinter.DrawLine(pen, X + offset1, Y + PrintAssign.DOUBLELINES_V / 4, X2 - offset2, Y2 + PrintAssign.DOUBLELINES_V / 4);
                            //'����
                            CurrPrinter.DrawLine(pen, X + offset3, Y - PrintAssign.DOUBLELINES_V / 4, X2 - offset4, Y2 - PrintAssign.DOUBLELINES_V / 4);
                        }
                        else
                        {
                            if (CellPos == PrinterCellPos.cellPos_CenterBottom)
                            {
                                offset1 = 0;
                                offset2 = 0;
                            }
                            else if (CellPos == PrinterCellPos.cellPos_LeftMiddle)
                                offset2 = offset2 * -1;
                            else if (CellPos == PrinterCellPos.cellPos_LeftTop || CellPos == PrinterCellPos.cellPos_RightTop)
                            {
                                offset1 = offset1 * -1;
                                offset2 = offset2 * -1;
                            }
                            CurrPrinter.DrawLine(pen, X + offset1, Y, X2 - offset2, Y2);
                        }
                        break;
                    case 3://Right
                        if (cell.T_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                            offset1 = PrintAssign.DOUBLELINES_V / 4;
                        if (cell.B_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                            offset2 = PrintAssign.DOUBLELINES_V / 4;
                        switch (CellPos)
                        {
                            case PrinterCellPos.cellPos_CenterMiddle:
                                aroundCell = printData.Body.GetItem("$" + (Row - 1)+ "$" +( Col + 1));
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.B_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset3 = PrintAssign.DOUBLELINES_V / 4;
                                }
                                aroundCell = printData.Body.GetItem("$" + (Row + 1)+ "$" +( Col + 1));
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.T_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset4 = PrintAssign.DOUBLELINES_V / 4;
                                }
                                break;
                            case PrinterCellPos.cellPos_CenterBottom:
                                aroundCell = printData.Body.GetItem("$" + (Row - 1)+ "$" + Col);
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.B_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset3 = PrintAssign.DOUBLELINES_V / 4;
                                }
                                aroundCell = printData.Body.GetItem("$" + Row+ "$" +( Col + 1));
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.B_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset4 = offset2;
                                    else
                                        offset4 = -1 * offset2;
                                }
                                break;
                            case PrinterCellPos.cellPos_CenterTop:
                                aroundCell = printData.Body.GetItem("$" + (Row + 1)+ "$" + Col);
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.T_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset4 = PrintAssign.DOUBLELINES_V / 4;

                                }
                                aroundCell = printData.Body.GetItem("$" + Row+ "$" +( Col + 1));
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.L_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset3 = PrintAssign.DOUBLELINES_V / 4;
                                }
                                break;
                            default:
                                break;
                        }
                        if (cell.R_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                        {
                            CurrPrinter.DrawLine(pen, X - PrintAssign.DOUBLELINES_H / 4, Y + offset1, X2 - PrintAssign.DOUBLELINES_H / 4, Y2 - offset2);
                            CurrPrinter.DrawLine(pen, X + PrintAssign.DOUBLELINES_H / 4, Y + offset3, X2 + PrintAssign.DOUBLELINES_H / 4, Y2 - offset4);
                        }
                        else
                        {
                            if (CellPos == PrinterCellPos.cellPos_RightBottom || CellPos == PrinterCellPos.cellPos_RightMiddle || CellPos == PrinterCellPos.cellPos_RightTop)
                            {
                                offset1 = 0;
                                offset2 = 0;
                            }
                            CurrPrinter.DrawLine(pen, X, Y + offset1, X2, Y2 - offset2);
                        }
                        break;
                    case 4://Bottom
                        if (cell.L_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                            offset1 = PrintAssign.DOUBLELINES_H / 4;
                        if (cell.R_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                            offset2 = PrintAssign.DOUBLELINES_H / 4;
                        switch (CellPos)
                        {
                            case PrinterCellPos.cellPos_CenterMiddle:
                                aroundCell = printData.Body.GetItem("$"+Row+"$"+( Col - 1));
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.R_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset3 = PrintAssign.DOUBLELINES_H / 4;
                                }
                                aroundCell = printData.Body.GetItem("$" + Row+"$"+(  Col + 1));
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.L_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset4 = PrintAssign.DOUBLELINES_H / 4;
                                }
                                break;
                            case PrinterCellPos.cellPos_LeftMiddle:
                                aroundCell = printData.Body.GetItem("$" + Row + "$" +( Col + 1));
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.L_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset4 = PrintAssign.DOUBLELINES_H / 4;
                                }
                                offset3 = offset1;
                                break;
                            case PrinterCellPos.cellPos_LeftBottom:
                                offset3 = -1 * offset1;
                                break;
                            case PrinterCellPos.cellPos_LeftTop:
                                goto case PrinterCellPos.cellPos_LeftBottom;
                            case PrinterCellPos.cellPos_RightMiddle:
                                aroundCell = printData.Body.GetItem("$" + Row+ "$" +( Col - 1));
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.R_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset3 = PrintAssign.DOUBLELINES_H / 4;
                                }
                                if (CellPos == PrinterCellPos.cellPos_RightBottom)
                                    offset4 = -1 * offset2;
                                else
                                    offset4 = offset2;
                                break;
                            case PrinterCellPos.cellPos_RightBottom:
                                goto case PrinterCellPos.cellPos_RightMiddle;
                            case PrinterCellPos.cellPos_CenterBottom:
                                aroundCell = printData.Body.GetItem("$" + Row+ "$" +( Col + 1));
                                if (aroundCell.Value != null)
                                {
                                    if (aroundCell.L_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                                        offset4 = -1 * PrintAssign.DOUBLELINES_H / 4;
                                }
                                break;
                            default:
                                break;
                        }
                        if (cell.B_Style == PrinterBorderStyle.˫ʵ�߱߿�)
                        {
                            CurrPrinter.DrawLine(pen, X + offset1, Y - PrintAssign.DOUBLELINES_V / 4, X2 - offset2, Y2 - PrintAssign.DOUBLELINES_V / 4);
                            CurrPrinter.DrawLine(pen, X + offset3, Y + PrintAssign.DOUBLELINES_V / 4, X2 - offset4, Y2 + PrintAssign.DOUBLELINES_V / 4);
                        }
                        else
                        {
                            if (CellPos == PrinterCellPos.cellPos_RightBottom)
                                offset2 = offset2 * -1;
                            else if (CellPos == PrinterCellPos.cellPos_LeftBottom || CellPos == PrinterCellPos.cellPos_CenterBottom)
                            {
                                offset1 = offset1 * -1;
                                offset2 = offset2 * -1;
                            }
                            CurrPrinter.DrawLine(pen, X + offset1, Y, X2 - offset2, Y2);
                        }
                        break;
                }
            }
        }

        private float DrawTopFixedRows(int StartCol, int EndCol, float X, float Y)
        {
            Range oRange, oMerg;
            int i, j;
            PrinterTextAlign CellPos;
            float cX, cY, w, h;
            PrinterCell typCell;
            //���ﲻ��������ҹ̶��н��沿��
            h = 0;
            if (printData.TopFixedRows > 0)
            {
                oRange = printData.FixRanges.GetItem("TopFixedRows");
                if (StartCol < printData.LeftFixedCols + 1)
                    StartCol = printData.LeftFixedCols + 1;
                if (EndCol > printData.Cols.Count - printData.RightFixedCols)
                    EndCol = printData.Cols.Count - printData.RightFixedCols;
                for (i = 1; i <= oRange.EndRow; i++)
                {
                    h = h + printData.Rows.GetItem(i).PrintHeight;
                    w = 0;
                    for (j = StartCol; j <= EndCol; j++)
                    {
                        w = w + printData.Cols.GetItem(j).PrintWidth;
                        if (printData.InMergedRanges(i, j, out CellPos, out oMerg))
                        {
                            if (CellPos == PrinterTextAlign.LeftTop)
                                DrawMergedRowCol(i, j, oRange.EndRow, EndCol, X + w - printData.Cols.GetItem(j).PrintWidth + GetFixedColsWidth(FixColPos.��̶���), Y + h - printData.Rows.GetItem(i).PrintHeight, 0, oMerg, false);
                        }
                        else
                        {
                            typCell = printData.Body.GetItem("$" + i+ "$" + j);
                            //'����X,Y
                            GetPointByCell(X + GetFixedColsWidth(FixColPos.��̶���), Y, 1, StartCol, i, j, out cX, out cY);
                            float  w1, h1;
                            w1 = printData.Cols.GetItem(j).PrintWidth;
                            h1 = printData.Rows.GetItem(i).PrintHeight;
                            DrawCellContent(typCell, cX, cY, ref w1, ref h1, AttachRowType.����, i, j, false, mBodyIncludeFixRange);
                            printData.Cols.GetItem(j).PrintWidth = w1;
                            printData.Rows.GetItem(i).PrintHeight = h1;
                            DrawBorder(typCell, cX, cY, printData.Cols.GetItem(j).PrintWidth, printData.Rows.GetItem(i).PrintHeight, AttachRowType.����, false, GetCellPos(i, j, 1, StartCol - printData.LeftFixedCols, 620000, EndCol + printData.RightFixedCols), i, j);
                        }
                    }
                }
            }
            return h;
        }

        private float GetAttachTextWidth(FixColPos attachTextPos)
        {
            int i, j, k;
            float w = 0;
            float w1;
            string s;

            PrinterAttachCell tCell = new PrinterAttachCell();
            w = 0;
            if (attachTextPos == FixColPos.ȫ���̶��� || attachTextPos == FixColPos.��̶���)
            {
                for (i = 1; i <= PrintData.LeftAttachText.Count; i++)
                {
                    w1 = 0;
                    for (j = 1; j <= PrintData.LeftAttachText.TheAttachRow(i).Count; j++)
                    {
                        tCell = PrintData.LeftAttachText.TheAttachRow(i).AttachCell(j);
                        if (tCell.Properties.Value != null)
                        {
                            s = GetCellText(tCell.Properties.Value.ToString ());
                            if (s.Length > 0)
                            {
                                string[] arySplit = s.Split('\n'.ToString().ToCharArray());
                                if (tCell.Properties.font != null)
                                {
                                    SaveCurrentFont();
                                    SetFont(tCell.Properties.font);
                                    for (k = arySplit.GetLowerBound(0); k <= arySplit.GetUpperBound(0); k++)
                                    {
                                        if (w1 < GetFontWidth(currFont, arySplit[k]))
                                            w1 = GetFontWidth(currFont, arySplit[k]);
                                    }
                                    RestoreFont();
                                }
                                else
                                {
                                    for (k = arySplit.GetLowerBound(0); k <= arySplit.GetUpperBound(0); k++)
                                    {
                                        if (w1 < GetFontWidth(currFont, arySplit[k]))
                                            w1 = GetFontWidth(currFont, arySplit[k]);
                                    }
                                }
                            }
                        }
                    }
                    w = w + w1 + PrintData.LeftAttachText.TheAttachRow(i).Offset_V;
                }
            }
            if (attachTextPos == FixColPos.ȫ���̶��� || attachTextPos == FixColPos.�ҹ̶���)
            {
                for (i = 1; i <= PrintData.RightAttachText.Count; i++)
                {
                    w1 = 0;
                    for (j = 1; j <= PrintData.RightAttachText.TheAttachRow(i).Count; j++)
                    {
                        tCell = PrintData.RightAttachText.TheAttachRow(i).AttachCell(j);
                        if (tCell.Properties.Value != null)
                        {
                            s = GetCellText(tCell.Properties.Value.ToString ());
                            if (s.Length > 0)
                            {
                                string[] arySplit = s.Split('\n'.ToString().ToCharArray());
                                if (tCell.Properties.font != null)
                                {
                                    SaveCurrentFont();
                                    SetFont(tCell.Properties.font);
                                    for (k = arySplit.GetLowerBound(0); k <= arySplit.GetUpperBound(0); k++)
                                    {
                                        if (w1 < GetFontWidth(currFont, arySplit[k]))
                                            w1 = GetFontWidth(currFont, arySplit[k]);
                                    }
                                    RestoreFont();
                                }
                                else
                                {
                                    for (k = arySplit.GetLowerBound(0); k <= arySplit.GetUpperBound(0); k++)
                                    {
                                        if (w1 < GetFontWidth(currFont, arySplit[k]))
                                            w1 = GetFontWidth(currFont, arySplit[k]);
                                    }
                                }
                            }
                        }
                    }
                    w = w + w1 + PrintData.RightAttachText.TheAttachRow(i).Offset_V;
                }
            }
            return w;
        }

        private float GetFixedColsWidth(FixColPos pos)
        {
            int i;
            float h = 0;
            Range o;

            if (pos == FixColPos.ȫ���̶��� || pos == FixColPos.��̶���)
            {
                o = printData.FixRanges.GetItem("LeftFixedCols");
                if (o != null)
                {
                    for (i = 1; i <= o.EndCol; i++)
                        h = h + printData.Cols.GetItem(i).PrintWidth;
                }
            }
            if (pos == FixColPos.ȫ���̶��� || pos == FixColPos.�ҹ̶���)
            {
                o = printData.FixRanges.GetItem("RightFixedCols");
                if (o != null)
                {
                    for (i = o.StartCol; i <= o.EndCol; i++)
                        h = h + printData.Cols.GetItem(i).PrintWidth;
                }
            }
            return h;
        }

        private float GetFixedRowsHeight(FixRowPos pos)
        {
            int i;
            float h = 0;
            Range o;

            if (pos == FixRowPos.ȫ���̶��� || pos == FixRowPos.���̶���)
            {
                o = printData.FixRanges.GetItem("TopFixedRows");
                if (o != null)
                {
                    for (i = 1; i <= o.EndRow; i++)
                        h = h + printData.Rows.GetItem(i).PrintHeight;
                }
            }
            if (pos == FixRowPos.ȫ���̶��� || pos == FixRowPos.�׹̶���)
            {
                o = printData.FixRanges.GetItem("BottomFixedRows");
                if (o != null)
                {
                    for (i = o.StartRow; i <= o.EndRow; i++)
                        h = h + printData.Rows.GetItem(i).PrintHeight;
                }
            }
            return h;
        }

        private PrinterCellPos GetCellPos(int Row, int Col, int StartRow, int StartCol, int EndRow, int EndCol)
        {
            bool InMergedRanges=false ;
            int Rows, Cols;
            PrinterCellPos cellPos = (PrinterCellPos) (-1);

            ///*�̶����еĲ����ڵ��øú���ʱ����
            Rows = EndRow - StartRow + 1;
            Cols = EndCol - StartCol + 1;
            if (Row > 0 && Col > 0)
            {
                if (Row >= StartRow && Row <= EndRow && Col >= StartCol && Col <= EndCol)
                    InMergedRanges = true;

                if (InMergedRanges)
                {
                    if (Rows == 1 && Cols == 1)
                    {
                        cellPos = PrinterCellPos.cellPos_OnlyOne;
                    }
                    if (Rows == 1)
                    {
                        if (Col == StartCol)
                            cellPos = PrinterCellPos.cellPos_LeftTopBottom;
                        else if (Col == EndCol)
                            cellPos = PrinterCellPos.cellPos_RightTopBottom;
                        else
                            cellPos = PrinterCellPos.cellPos_CenterTopBottom;
                    }
                    else if (Cols == 1)
                    {
                        if (Row == StartRow)
                            cellPos = PrinterCellPos.cellPos_TopLeftRight;
                        else if (Row == EndRow)
                            cellPos = PrinterCellPos.cellPos_BottomLeftRight;
                        else
                            cellPos = PrinterCellPos.cellPos_CenterTopBottom;
                    }
                    else
                    {
                        if (Row == StartRow && Col == StartCol)
                            cellPos = PrinterCellPos.cellPos_LeftTop;
                        else if (Row == StartRow && Col == EndCol)
                            cellPos = PrinterCellPos.cellPos_RightTop;
                        else if (Row == EndRow && Col == StartCol)
                            cellPos = PrinterCellPos.cellPos_LeftBottom;
                        else if (Row == EndRow && Col == EndCol)
                            cellPos = PrinterCellPos.cellPos_RightBottom;
                        else if (Row == StartRow && Col < EndCol && Col > StartCol)
                            cellPos = PrinterCellPos.cellPos_CenterTop;
                        else if (Row == EndRow && Col < EndCol && Col > StartCol)
                            cellPos = PrinterCellPos.cellPos_CenterBottom;
                        else if (Col == StartCol && Row < EndRow && Row > StartRow)
                            cellPos = PrinterCellPos.cellPos_LeftMiddle;
                        else if (Col == EndCol && Row < EndRow && Row > StartRow)
                            cellPos = PrinterCellPos.cellPos_RightMiddle;
                        else
                            cellPos = PrinterCellPos.cellPos_CenterMiddle;
                    }
                }
            }
            return cellPos;
        }

        /// <summary>
        /// �õ�ʵ��Ҫ��ӡ���ı�,���ڼ����͸�
        /// </summary>
        /// <param name="CellText"></param>
        /// <returns></returns>
        private string GetCellText(string CellText)
        {
            typCellEx[] t = new typCellEx[1];
            string s = "";
            bool isTextEx = false;
            //��ǰ���Ƶ����Ͻ���pX,pY����
            isTextEx = (SplitTextToCellEx(CellText, currColor.ToArgb(), ref t));
            if (isTextEx)
            {
                foreach (typCellEx ti in t)
                {
                    s += ti.text;
                }
            }
            else
                s = CellText;
            return s;
        }

        private string GetCellText_old(string CellText)
        {
            string s = CellText;
            int i, j;
            i = j = -1;
            if (CellText.IndexOf('<') == 0 && CellText.IndexOf('>') > 1)
            {
                i = CellText.IndexOf("text=", i + 1);
                if (i >= 0)
                {
                    j = CellText.IndexOf('"', i + 6);
                    if (j >= 0)
                        s = CellText.Substring(i + 6, j - i - 6);
                }
            }
            return s;
        }

        private float GetRealRowHeightByRowBound(PrinterBound RowBound)
        {
            int i;
            float h = 0;

            for (i = RowBound.Start; i <= RowBound.End; i++)
                h = h + printData.Rows.GetItem(i).PrintHeight;
            h = h + GetFixedRowsHeight(FixRowPos.ȫ���̶���);
            return h;
        }

        private float GetRealColWidthByColBound(PrinterBound RowBound)
        {
            int i;
            float w = 0;

            for (i = RowBound.Start; i <= RowBound.End; i++)
                w = w + printData.Cols.GetItem(i).PrintWidth;
            w = w + GetFixedColsWidth(FixColPos.ȫ���̶���);
            return w;
        }

        private float GetFontHeight(Font f, string text)
        {
            Graphics g;
            //�ο�����:[������, ��ȡ];[����ϵ��, ����]
            if (CurrPrinter == null)
                g = printAssign.printDoc.PrinterSettings.CreateMeasurementGraphics();
            else
                g = CurrPrinter;
            if (f == null)
                return 0;
            Size size = TextRenderer.MeasureText(g, text, f);
            return size.Height;
        }
        private float GetFontHeight(Font f, char text)
        {
            Graphics g;
            //�ο�����:[������, ��ȡ];[����ϵ��, ����]
            if (CurrPrinter == null)
                g = printAssign.printDoc.PrinterSettings.CreateMeasurementGraphics();
            else
                g = CurrPrinter;
            if (f == null)
                return 0;
            Size size = TextRenderer.MeasureText(g, text.ToString (), f);
            return size.Height;
        }

        private float GetFontWidth(Font f, string text)
        {
            Graphics g;
            //�ο�����:[������, ��ȡ];[����ϵ��, ����]
            if (CurrPrinter == null)
                g = printAssign.printDoc.PrinterSettings.CreateMeasurementGraphics();
            else
                g = CurrPrinter;
            Size size = TextRenderer.MeasureText(g, text, f);
            return size.Width;
        }
        private float GetFontWidth(Font f, char text)
        {
            Graphics g;
            //�ο�����:[������, ��ȡ];[����ϵ��, ����]
            if (CurrPrinter == null)
                g = printAssign.printDoc.PrinterSettings.CreateMeasurementGraphics();
            else
                g = CurrPrinter;
            Size size = TextRenderer.MeasureText(g, text.ToString() , f);
            return size.Width;
        }

        private float GetRealSheetBodyHeight()
        {
            float h;
            float realHeight;

            realHeight = pageArrange.Arrange(1).Size.Height  - (oPage.TopMargin + oPage.BottomMargin);
            h = DrawAttachRows(printData.PageHeaders, AttachRowType.ҳü,true );
            h = h + DrawAttachRows(printData.PageFoots, AttachRowType.ҳ��, true);
            h = h + DrawAttachRows(printData.MainTitle, AttachRowType.������, true);
            h = h + DrawAttachRows(printData.SubTitles, AttachRowType.������, true);
            h = h + DrawAttachRows(printData.Headers, AttachRowType.��ͷ, true);
            h = h + DrawAttachRows(printData.Tails, AttachRowType.��β, true);
            h = h + GetHeightLogImage();

            realHeight = realHeight - h - printData.Body.Offset_V; //���õı���߶�
            return realHeight;
        }

        private float GetHeightLogImage()
        {
            float h = 0;
            if (oPage.LogPicPrint && !(oPage.LogPic==null ))
            {
                if (oPage.LogPicLoc == PrinterLogPictureLocation.��������)
                    h = h + (float)oPage.GetAttributes(PageSetupKey.ePage_ͼ���ӡ�߶�);
            }
            return h;
        }

        private void GetPointByCell(float StartX, float StartY, int StartRow, int StartCol, int Row, int Col, out float X, out float Y)
        {
            GetPointByCell(StartX, StartY, StartRow, StartCol, Row, Col, out  X, out  Y, false);
        }
        private void GetPointByCell(float StartX, float StartY, int StartRow, int StartCol, int Row, int Col, out float X, out float Y, bool IncludeFixedRC)
        {
            int i;
            float h;

            h = StartX;
            for (i = StartCol; i <= Col - 1; i++)
            {
                if (IncludeFixedRC)
                {
                    if (!printData.InFixedRanges(Row, i))
                        h = h + printData.Cols.GetItem(i).PrintWidth;
                }
                else
                    h = h + printData.Cols.GetItem(i).PrintWidth;
            }
            if (IncludeFixedRC)
            {
                for (i = 1; i <= Col - 1; i++)
                {
                    if (printData.InFixedRanges(Row, i))
                        h = h + printData.Cols.GetItem(i).PrintWidth;
                }
            }
            X = h;


            h = StartY;
            for (i = StartRow; i <= Row - 1; i++)
            {
                if (IncludeFixedRC)
                {
                    if (!printData.InFixedRanges(i, Col))
                        h = h + printData.Rows.GetItem(i).PrintHeight;
                }
                else
                {
                    if (i > printData.Rows.Count)
                        h = h + printData.Rows.GetItem(printData.Rows.Count - printData.BottomFixedRows).PrintHeight;
                    else
                        h = h + printData.Rows.GetItem(i).PrintHeight;
                }
            }
            if (IncludeFixedRC)
            {
                for (i = 1; i <= Row - 1; i++)
                {
                    if (printData.InFixedRanges(i, Col))
                        h = h + printData.Rows.GetItem(i).PrintHeight;
                }
            }
            Y = h;
        }

        private int GetPrintPages()
        {
            PrinterBound[] mRows = new PrinterBound[1];
            PrinterBound[] mCols = new PrinterBound[1];
            return GetPrintPages(ref mRows, ref mCols);
        }

        private int GetPrintPages(ref PrinterBound[] mRows, ref PrinterBound[] mCols)
        {
            int pageCount = 1;
            int i; int j;

            if (printData.Body.Count <= 0)
                return 1;
            if (oPage.Zoom == 1)
            {
                //'������ǿ�ƴ�һҳ��
                if ((int)oPage.GetAttributes(PageSetupKey.ePage_�Զ�����ģʽ) == 0 || (int)oPage.GetAttributes(PageSetupKey.ePage_�Զ�����ģʽ) == 2)
                {
                    System.Array.Resize<PrinterBound>(ref mCols, 1);
                    mCols[0].Start = printData.LeftFixedCols + 1;
                    mCols[0].End = printData.Cols.Count - printData.RightFixedCols;
                }
                else
                    SplitColsToArray(ref mCols);
                if (!AdjustColWidthSize(pageArrange.Arrange(1).Size.Width - oPage.LeftMargin - oPage.RightMargin - GetAttachTextWidth(FixColPos.ȫ���̶���)))
                    return 0;
                if (oPage.SheetBodyStyle == PrinterBodyStyle.��������)
                {
                    SplitRowsToArray(ref mRows);
                    AdjustRowHeightSizeEx(RealDataBodyHeight, mRows);
                }
                else if (oPage.SheetBodyStyle == PrinterBodyStyle.�����и�)
                //����ҳ���С����������,һ���Ե����� SheetRow.PrintHeight
                {
                    AdjustRowHeightSize(RealDataBodyHeight);
                    SplitRowsToArray(ref mRows);
                }
                else if ((int)oPage.GetAttributes(PageSetupKey.ePage_�Զ�����ģʽ) == 0 || (int)oPage.GetAttributes(PageSetupKey.ePage_�Զ�����ģʽ) == 1)
                {
                    System.Array.Resize<PrinterBound>(ref mCols, 1);
                    mRows[0].Start = printData.TopFixedRows + 1;
                    mRows[0].End = printData.Rows.Count - printData.BottomFixedRows;
                    AdjustRowHeightSize(RealDataBodyHeight);
                }
                else
                {
                    AdjustRowHeightSize(RealDataBodyHeight);
                    SplitRowsToArray(ref mRows);
                }
                pageCount = mRows.GetUpperBound(0) + 1;
            }
            else
            {
                //����ҳ���С����������,һ���Ե����� Row.PrintHeight
                AdjustRowHeightSize(RealDataBodyHeight);
                SplitRowsToArray(ref mRows);
                //�����ÿҳҪ��Ŀ�ʼ�кͶ�����
                SplitColsToArray(ref mCols);
                AdjustColWidthSize(pageArrange.Arrange(1).Size.Width - oPage.LeftMargin - oPage.RightMargin - GetAttachTextWidth(FixColPos.ȫ���̶���));
                i = mCols.GetLength(0) ;
                j = mRows.GetLength(0) ;
                pageCount = i * j;
            }
            return pageCount;
        }

        private void GetRectByRange(int StartRow, int StartCol, float StartX, float StartY, Range oRange, out Rectangle tRect)
        {
            int i;
            int h;
            Col oCol;
            Row oRow;

            tRect = new Rectangle();
            h =(int) StartX;
            tRect.X = h;
            for (i = StartCol; i <= oRange.EndCol; i++)
            {
                oCol = printData.Cols.GetItem(i);
                if (oCol != null)
                {
                    if (i == oRange.StartCol)
                        tRect.X = h;
                    h = h + (int)oCol.PrintWidth;
                    tRect.Width  = h-tRect.X ;
                }
            }
            h = (int)StartY;
            tRect.Y = h;
            for (i = StartRow; i <= oRange.EndRow; i++)
            {
                oRow = printData.Rows.GetItem(i);
                if (oRow != null)
                {
                    if (i == oRange.StartRow)
                        tRect.Y = h;
                    h = h + (int)oRow.PrintHeight;
                    tRect.Height  = h-tRect.Y;
                }
            }
        }

        private bool FontZoomOut()
        {
            float i, j;
            i = currFont.Size;
            j = i;
            do
            {
                i = i - 0.125F;
                if (i <= 2)
                    break;
                currFont = new Font(currFont.Name, i, currFont.Style);
            }
            while (i != currFont.Size && i > 2);
            if (j > currFont.Size)
                return true;
            else
                return false;
        }

        private bool KeepSameForSheetBodyFont()
        {
            PrinterCell typCell;
            Font oFont;
            PrinterTextAlign CellPos;
            Range oRange;
            Rectangle tRect;
            int row, col, i;
            bool b;
            float NewFontSize=10;
            string FontName;
            float h, w;

            oFont = oPage.BodyFont;
            if (oFont != null)
                printData.Body.DefaultFont = (Font)oFont.Clone();
            Array.Resize<Font>(ref  mKeepFont, 1);
            mKeepFont[0] = new Font(printData.Body.DefaultFont.Name, printData.Body.DefaultFont.Size);
            for (row = printData.TopFixedRows + 1; row <= printData.Rows.Count - printData.BottomFixedRows; row++)
            {
                for (col = printData.LeftFixedCols + 1; col <= printData.Cols.Count - printData.RightFixedCols; col++)
                {
                    typCell = printData.Body.GetItem("$" + row+ "$" + col);
                    if (typCell.font != null)
                    {
                        b = true;
                        for (i = 0; i < mKeepFont.Length; i++)
                        {
                            if (mKeepFont[i].Name == typCell.font.Name)
                            {
                                b = false;
                                FontName = currFont.Name;
                                if (mKeepFont[i].Size > typCell.font.Size)
                                    mKeepFont[i]=new Font (mKeepFont[i].Name ,typCell.font.Size,mKeepFont[i].Style );
                                break;
                            }
                        }
                        if (b)
                        {
                            Array.Resize<Font>(ref mKeepFont, mKeepFont.GetUpperBound(0) + 1);
                            mKeepFont[mKeepFont.GetUpperBound(0)] = new Font(typCell.font.Name, typCell.font.Size);
                            NewFontSize = typCell.font.Size;
                            FontName = typCell.font.Name;
                        }
                        SetFont(typCell.font);
                    }
                    else
                    {
                        FontName = mKeepFont[0].Name;
                        NewFontSize = mKeepFont[0].Size; //ȱʡ����
                        SetFont(printData.Body.DefaultFont);
                    }
                    if (printData.InMergedRanges(row, col,out CellPos,out oRange))
                    {
                        w = printData.Cols.GetItem(col).PrintWidth;
                        h = printData.Rows.GetItem(row).PrintHeight;
                    }
                    else
                    {
                        if (CellPos ==PrinterTextAlign.LeftTop)
                        {
                            GetRectByRange(row, col, 0, 0, oRange,out tRect);
                            w = tRect.Right - tRect.Left;
                            h = tRect.Bottom - tRect.Top;
                        }
                        else
                        {
                            w = -1;
                            h = -1;
                        }
                    }
                    if (w >= 0 && h >= 0)
                    {
                        if (SheetBodyCellFontChanged(typCell, w, h, NewFontSize))
                        {
                            for (i = 0; i < mKeepFont.Length; i++)
                            {
                                if (mKeepFont[i].Name == currFont.Name)
                                {
                                    if (mKeepFont[i].Size > NewFontSize)
                                        mKeepFont[i] =new Font (mKeepFont[i].Name , NewFontSize,mKeepFont[i].Style );
                                    break;
                                }
                            }
                        }
                    }

                }
            }
            return true;
        }

        private void RefreshSysSheetTail(int PageNum, int PageCount, bool IsInit)
        {
            AttachRow o;
            PrinterAttachCell typAttach = new PrinterAttachCell();
            AttachRows SheetTail = printData.Tails;
            //��ӡ����
            
            if (IsInit)
            {
                SheetTail.Remove("PrintOtherAttach");
                if (oPage.PrintOperator || oPage.PrintDate || oPage.PrintPageNum || oPage.PrintPageCount )
                {
                    o = new AttachRow(); 
                    //PrintOperator
                    o.Offset_V =PrintAssign.DOUBLELINES_V * 3;
                    typAttach.InRowPercent = 30;
                    typAttach.Properties.Align =PrinterTextAlign.LeftMiddle;
                    if (oPage.PrintOperator)
                        typAttach.Properties.Value = "����Ա: " + printAssign.UserName;
                    else
                        typAttach.Properties.Value = "";
                    o.Add(typAttach, "Operator");
                    typAttach.InRowPercent = 40;
                    typAttach.Properties.Align = PrinterTextAlign.CenterMiddle;
                    typAttach.Properties.Value = "";
                    if (oPage.PrintPageNum )
                        typAttach.Properties.Value = "��  " + PageNum.ToString() + "  ҳ";
                    if (oPage.PrintPageCount )
                        typAttach.Properties.Value = typAttach.Properties.Value.ToString() + "  ( ��  " + PageCount.ToString() + "  ҳ )";
                    o.Add(typAttach,"PageNum");
                    typAttach.InRowPercent = 30;
                    typAttach.Properties.Align = PrinterTextAlign.RightMiddle;
                    if (oPage.PrintDate)
                        typAttach.Properties.Value = "��ӡ����: " + DateTime.Now.ToString("yyyy��M��d��");
                    else
                        typAttach.Properties.Value = "";
                    o.Add(typAttach,"Date");
                    SheetTail.Add(o,"PrintOtherAttach");
                    
                }
            }
            o = SheetTail.TheAttachRow("PrintOtherAttach");
            if (o == null)
                return;
            typAttach = o.AttachCell(2);
            typAttach.Properties.Value = "";
            if (oPage.PrintPageNum )
                typAttach.Properties.Value = "��  " + PageNum.ToString() + "  ҳ";
            if (oPage.PrintPageCount )
                typAttach.Properties.Value = typAttach.Properties.Value.ToString() + "  ( ��  " + PageCount.ToString() + "  ҳ )";
            o.AttachCell(2, typAttach);
        }

        private void RestoreFont()
        {
            currFont = (Font)backupFont.Clone();
        }

        private void SaveCurrentFont()
        {
            backupFont = (Font)currFont.Clone();
        }

        private void SetFont(Font oFont)
        {
            currFont = (Font)oFont.Clone();
        }

        private bool SheetBodyCellFontChanged(PrinterCell cell, float Width, float Height, float NewFontSize)
        {
            string s;
            string[] s1=new string [1];
            int i;
            float RealHeight;
            bool b, bMultiRow, bChanged_H=false , bChanged_V=false , bChanged;

            bChanged = bMultiRow = false;
            switch (cell.Behave)
            {
                case PrinterCellBehave.����߱���:
                    b = false;
                    break;
                case PrinterCellBehave.�����:
                    if (printAssign.PageSetup.CyLine)
                        b = false;
                    else
                        b = true;
                    break;
                default:
                    b = true;
                    break;
            }
            if (b)
            {
                if (Height == 0)
                {
                    Height = CalcTextRealHeight(cell, Width,ref s1);
                    RealHeight = Height;
                }
                else
                    RealHeight = CalcTextRealHeight(cell, Width,ref s1);
                //if (!IsArrayEmpty(s1))
                //{
                    if (s1.Length > 1)
                        bMultiRow = true;
                //}
                if (cell.Align ==PrinterTextAlign.���ֲ�����)
                {
                    s = cell.Value.ToString();
                    SaveCurrentFont();
                    if (printAssign.PageSetup.ReduceFont == 1)
                        currFont  = new Font(currFont.Name, 36, currFont.Style);

                    while (CalcTextRealHeight(cell, Width, ref s1) > Height)
                    {
                        bChanged_V = FontZoomOut();
                        if (!bChanged_V)
                            break;
                    }
                    if (bChanged)
                        NewFontSize = currFont.Size;
                    RestoreFont();
                }
                else
                {
                    s = cell.Value.ToString();
                    SaveCurrentFont();
                    if (printAssign.PageSetup.ReduceFont == 1)
                        currFont = new Font(currFont.Name, 36, currFont.Style);
                    while (CalcTextRealHeight(cell, Width, ref s1) > Height)
                    {
                        bChanged_V = FontZoomOut();
                        if (!bChanged_V)
                            break;
                    }
                    //if (!IsArrayEmpty(s1))
                    //{
                        if (s1.Length > 1)
                            bMultiRow = true;
                        else
                            bMultiRow = false;
                    //}
                    if (bChanged_V)
                        NewFontSize = currFont.Size;
                    if (bMultiRow)
                    {
                        for (i = s1.GetLowerBound(1); i <= s1.GetUpperBound(1); i++)
                        {
                            while ((GetFontWidth(currFont, s1[i]) + PrintAssign.DOUBLELINES_H) > Width)
                            {
                                bChanged_H = FontZoomOut();
                                if (!bChanged_H)
                                    break;
                            }
                        }
                    }
                    else
                    {
                        while ((GetFontWidth(currFont, cell.Value.ToString()) + PrintAssign.DOUBLELINES_H) > Width)
                        {
                            bChanged_H = FontZoomOut();
                            if (!bChanged_H)
                                break;
                        }
                    }
                    if (bChanged_H)
                        NewFontSize = currFont.Size;
                    RestoreFont();
                }
            }
            if (bChanged_H || bChanged_V)
                bChanged = true;
            else
                bChanged = false;
            return bChanged;
        }

        private bool SplitRowsToArray(ref PrinterBound[] mRows)
        {
            int i = 0;
            float h = 0;
            int j = 0;
            int StartRow = 0;
            float RealHeight = 0;


            System.Array.Resize<PrinterBound>(ref mRows, 1);
            mRows[0].Start = 0;
            mRows[0].End = 0;

            if (RealDataBodyHeight <= 0)
                RealDataBodyHeight = GetRealSheetBodyHeight();
            RealHeight = RealDataBodyHeight;
            if (oPage.SheetBodyStyle == PrinterBodyStyle.��������)
            {
                for (i = printData.TopFixedRows +1; i <= printData.Rows.Count - printData.BottomFixedRows; i = i + oPage.Rows)
                {
                    h = GetFixedRowsHeight(0);
                    if (mRows.Length < (j + 1))
                        System.Array.Resize<PrinterBound>(ref mRows, j + 1);
                    mRows[j].Start = i;
                    mRows[j].End = i + oPage.Rows - 1;
                    for (StartRow = i; StartRow <= i + oPage.Rows - 1; StartRow++)
                    {
                        if (StartRow <= printData.Rows.Count - printData.BottomFixedRows)
                        {
                            if (oPage.PrintUnit == PrinterUnit.Twips)
                                printData.Rows.GetItem(StartRow).PrintHeight = TwipToPixl(printData.Rows.GetItem(StartRow).Height);
                            else
                                printData.Rows.GetItem(StartRow).PrintHeight = printData.Rows.GetItem(StartRow).Height;
                            h = h + printData.Rows.GetItem(StartRow).PrintHeight;
                        }
                        else
                        {
                            mRows[j].End = StartRow - 1;
                            break;
                        }

                    }
                    if (h > RealHeight)
                    {   //��Ȩƽ����������
                        h = (h - RealHeight) / (mRows[j].End - mRows[j].Start + 1);
                        if (h > (int)h)
                            h = (int)h + 1;
                        //��������
                        for (StartRow = i; StartRow <= i + oPage.Rows - 1; StartRow++)
                        {
                            if (StartRow <= printData.Rows.Count - printData.BottomFixedRows)
                                printData.Rows.GetItem(StartRow).PrintHeight = h;
                            else
                                break;
                        }
                    }
                    j++;
                }
                return true;
            }
            else
            {
                i = printData.TopFixedRows;
                StartRow = i + 1;

                while (i < printData.Rows.Count)
                {
                    i++;
                    if (!printData.InFixedRanges(i, -1))
                    {
                        h = h + printData.Rows.GetItem(i).PrintHeight;
                        if (h > (RealHeight - GetFixedRowsHeight(0)))
                        {
                            if (mRows.Length < (j + 1))
                                System.Array.Resize<PrinterBound>(ref mRows, j + 1);
                            mRows[j].Start = StartRow;
                            mRows[j].End = i - 1;
                            StartRow = i;
                            i = i - 1;
                            if (mRows[j].End == 0)
                            {
                                System.Array.Resize<PrinterBound>(ref mRows, 1);
                                mRows[0].End = 0;
                                return false;
                            }
                            j++;
                            h = 0;
                        }
                        else if (h == (RealHeight - GetFixedRowsHeight(0)))
                        {
                            if (mRows.Length < (j + 1))
                                System.Array.Resize<PrinterBound>(ref mRows, j + 1);
                            mRows[j].Start = StartRow;
                            mRows[j].End = i;
                            j++; h = 0;
                            StartRow = i + 1;
                        }
                    }
                }

                if (mRows[0].Start == 0)
                {
                    mRows[0].Start = printData.TopFixedRows + 1;
                    mRows[0].End = printData.Rows.Count - printData.BottomFixedRows;
                }
                else if (mRows[mRows.GetUpperBound(0)].End < (printData.Rows.Count - printData.BottomFixedRows))
                {
                    //System.Array.Resize<PrinterBound>(ref mRows, mRows.GetUpperBound(0) + 1);
                    //mRows[mRows.GetUpperBound(0)].Start = mRows[mRows.GetUpperBound(0) - 1].End + 1;
                    System.Array.Resize<PrinterBound>(ref mRows, mRows.Length  + 1);
                    mRows[mRows.GetUpperBound(0)].Start = mRows[mRows.GetUpperBound(0) - 1].End + 1;
                    mRows[mRows.GetUpperBound(0)].End = printData.Rows.Count - printData.BottomFixedRows;
                }
                return true;
            }
        }

        private bool SplitColsToArray(ref PrinterBound[] mCols)
        {
            int i = 0;
            float h = 0;
            int j = 0;
            int StartCol = 0;


            System.Array.Resize<PrinterBound>(ref mCols, 1);
            mCols[0].Start = 0;
            mCols[0].End = 0;

            i = printData.LeftFixedCols;
            StartCol = i + 1;
            while (i != printData.Cols.Count)
            {
                i++;
                if (!printData.InFixedRanges(-1, i))
                    h = h + printData.Cols.GetItem(i).PrintWidth;
                if (h > (pageArrange.Arrange(1).Size.Width - oPage.LeftMargin - oPage.RightMargin - GetFixedColsWidth(FixColPos.ȫ���̶���) - GetAttachTextWidth(FixColPos.ȫ���̶���)))
                {
                    if (mCols.Length < (j + 1))
                        System.Array.Resize<PrinterBound>(ref mCols, j + 1);
                    mCols[j].Start = StartCol;
                    mCols[j].End = i - 1;
                    StartCol = i;
                    i--;
                    if (mCols[j].End == 0)
                    {
                        System.Array.Resize<PrinterBound>(ref mCols, 1);
                        mCols[0].End = 0;
                        return false;
                    }
                    j++; h = 0;
                }
                else if (h == (pageArrange.Arrange(1).Size.Width - oPage.LeftMargin - oPage.RightMargin - GetFixedColsWidth(FixColPos.ȫ���̶���) - GetAttachTextWidth(FixColPos.ȫ���̶���)))
                {
                    if (mCols.Length < (j + 1))
                        System.Array.Resize<PrinterBound>(ref mCols, j + 1);
                    mCols[j].Start = StartCol;
                    mCols[j].End = i;
                    j++; h = 0;
                    StartCol = i + 1;
                }
            }
            if (mCols[0].Start == 0)
            {
                mCols[0].Start = printData.LeftFixedCols + 1;
                mCols[0].End = printData.Cols.Count - printData.RightFixedCols;
            }
            else if (mCols[mCols.GetUpperBound(0)].End < printData.Cols.Count - printData.RightFixedCols)
            {
                System.Array.Resize<PrinterBound>(ref mCols, mCols.GetUpperBound(0) + 1);
                if (mCols.GetUpperBound(0) > 0)
                    mCols[mCols.GetUpperBound(0)].Start = mCols[mCols.GetUpperBound(0) - 1].End + 1;
                else
                    mCols[mCols.GetUpperBound(0)].Start = 1;
                mCols[mCols.GetUpperBound(0)].End = printData.Cols.Count - printData.RightFixedCols;
            }
            return true;
        }

        private void SplitTextByBaseWidth(string text, ref string[] Value, float Width)
        {
            //��ӡ�����ɵ��ú�������
            StringBuilder mCol = new StringBuilder();
            int i;
            StringBuilder s = new StringBuilder();

            i = 1;

            do
            {
                s.Append(text.Substring(i - 1, 1));// Mid(text, i, 1)
                if (GetFontWidth(currFont, s.ToString()) > Width)
                {
                    if (s.Length == 1)
                    {
                        mCol.Append(s.ToString().Substring(0, 1));
                        i++;
                    }
                    else
                        mCol.Append(s.ToString().Substring(0, s.Length - 1));  //Left(s, Len(s) - 1)

                    s.Remove(0, s.Length);
                }
                else if (GetFontWidth(currFont, s.ToString()) == Width)
                {
                    mCol.Append(s);
                    i++;
                    s.Remove(0, s.Length);
                }
                else
                    i++;

            } while (i <= text.Length);
            if (s.Length > 0)
                mCol.Append(s);

            Value = mCol.ToString().Substring(2).Split("^^".ToCharArray());
        }

        private bool SplitTextToCellEx(string CellText, int defaultColor, ref typCellEx[] tCellEx)
        {
            int i =0;
            if (CellText.IndexOf("<root>") == 0 && CellText.IndexOf("</root>") > 5)
            {
                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                xml.LoadXml(CellText);
                System.Xml.XmlNodeList nodes = xml.GetElementsByTagName("text");
                foreach (System.Xml.XmlNode node in nodes)
                {
                    i++;
                    if (tCellEx.Length < i)
                    {
                        Array.Resize<typCellEx>(ref tCellEx, tCellEx.Length + 1);
                    }
                    int color = defaultColor;
                    if (node.Attributes["color"] != null)
                    {
                        if (!(int.TryParse(node.Attributes["color"].Value, out color)))
                        {
                            color = defaultColor;
                        }
                    }
                    Font ft = currFont;
                    if (node.Attributes["font"] != null)
                    {
                        string s = node.Attributes["font"].Value;
                        if (s.Length > 0)
                        {
                            ft = General.FontFromString(s);
                        }
                    }
                    tCellEx[i - 1].color = color;
                    tCellEx[i - 1].font = ft;
                    tCellEx[i - 1].text = node.InnerText;
                }
                if (tCellEx.Length > 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// �ֲ�س����е�typCellEx����
        /// </summary>
        /// <param name="realText"></param>
        /// <param name="s"></param>
        /// <param name="dstText"></param>
        private void TextArrayToCellEx(string realText, string[] s, ref typCellEx[] dstText)
        {
            int i;
            int color = currColor.ToArgb();
            Array.Resize<typCellEx>(ref dstText, s.Length);
            for (i = s.GetLowerBound(0); i <= s.GetUpperBound(0); i++)
            {
                dstText[i].text = s[i];
                dstText[i].color = color;
                dstText[i].font = currFont;
            }
        }

        private void TextArrayToCellEx_0ld(string realText, string[] s, ref typCellEx[] dstText)
        {
            typCellEx[] aryCell=new typCellEx [1];
            int i, color, j, p, k;

            color = currColor.ToArgb() ;
            Array.Resize<typCellEx>(ref dstText, s.Length);
            for (i = s.GetLowerBound(0); i <= s.GetUpperBound(0); i++)
            {
                dstText[i].text = s[i];
                dstText[i].color = color;
                dstText[i].font = currFont;
            }
            if (SplitTextToCellEx(realText, color,ref aryCell))
            {
                for (k = aryCell.GetLowerBound(0); k <= aryCell.GetUpperBound(0); k++)
                {
                    aryCell[k].text = aryCell[k].text.Replace("\n", "");  //Replace(aryCell(k).text, vbLf, "")
                    aryCell[k].text = aryCell[k].text.Replace("\r", ""); //Replace(aryCell(k).text, vbCr, "")
                }

                j = 0;
                p = 1;
                for (i = dstText.GetLowerBound(0); i <= dstText.GetUpperBound(0); i++)
                {
                    for (k = aryCell.GetLowerBound(0); k <= aryCell.GetUpperBound(0); k++)
                    {
                        if (p >= j)
                        {
                            p = k - 1;
                            break;
                        }
                        else if (i == 0)
                        {
                            p = k;
                            break;
                        }
                        p = p + aryCell[k].text.Length;
                    }
                    if (p < aryCell.GetLowerBound(0))
                        p = aryCell.GetLowerBound(0);
                    for (k = p; k <= aryCell.GetUpperBound(0); k++)
                    {
                        if (aryCell[k].text.IndexOf(dstText[i].text) >= 0)
                        {
                            dstText[i].color = aryCell[k].color;
                            dstText[i].font = aryCell[k].font;
                            break;
                        }
                    }
                    j = j + dstText[i].text.Length;
                }
            }
        }

        private float TwipToPixl(int twip)
        {
            return twip * 15;
        }

    }


    /// <summary>
    /// �ڲ���ʼ����Ϊ0,���ⲿΪ1
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListDictionaryEx<T> : ListDictionary<T>
    {
        public ListDictionaryEx()
        {
        }

        public override void Remove(int index)
        {
            base.Remove(index - 1); 
        }

        public override bool SetItem(int index, T newItem)
        {
            return base.SetItem(index-1, newItem);
        }

        public override T GetItem(int index)
        {
            return base.GetItem(index - 1);
        }

        public override int Index(string key)
        {
            return base.Index(key)+1;
        }

        public override string Key(int index)
        {
            return base.Key(index - 1);
        }

        
    }

    /// <summary>
    /// ҳ��Χ�����е�-1��ʾһֱ��ӡ�����ҳ
    /// </summary>
    public class PrinterPageRangesAndBeginPageNumber
    {
        private int _number = 1;
        private int[] _ranges = new int[0];
        private bool _all = true;
        private string _rangesString = "";

        public PrinterPageRangesAndBeginPageNumber() { }

        public PrinterPageRangesAndBeginPageNumber(int beginNumber, int[] ranges)
        {
            _number = beginNumber;
            _ranges = ranges;
            if (ranges.Length <= 0)
                _all = true;
            else
                _all = false;
        }

        public PrinterPageRangesAndBeginPageNumber(int beginNumber, string ranges)
        {
            _number = beginNumber;
            _rangesString = ranges;
            if (ranges.Length <= 0)
            {
                _all = true;
            }
            else
            {
                _all = false;
                _ranges = GetRangesFromString(ranges);
            }
        }

        public static int[] GetRangesFromString(string ranges)
        {
            int[] rs = new int[0];
            string[] ss = ranges.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in ss)
            {
                int result = 0;
                if (s.IndexOf('-') > 0)
                {
                    string[] cc = s.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (cc.Length > 1)
                    {
                        for (int i = int.Parse(cc[0]); i <= int.Parse(cc[1]); i++)
                        {
                            Array.Resize<int>(ref rs, rs.Length + 1);
                            rs[rs.Length - 1] = i;
                        }
                    }
                    else
                    {
                        if (int.TryParse(cc[0], out result))
                        {
                            Array.Resize<int>(ref rs, rs.Length + 1);
                            rs[rs.Length - 1] = result;

                            Array.Resize<int>(ref rs, rs.Length + 1);
                            rs[rs.Length - 1] = -1;
                        }
                    }
                }
                else
                {
                    if (int.TryParse(s, out result))
                    {
                        Array.Resize<int>(ref rs, rs.Length + 1);
                        rs[rs.Length - 1] = result;
                    }
                }
            }
            return rs;
        }

        /// <summary>
        /// ��δʵ��
        /// </summary>
        /// <param name="ranges"></param>
        /// <returns></returns>
        public static string GetRangesFromArray(int[] ranges)
        {
            //StringBuilder sb = new StringBuilder();
            //sb.Append (ranges[0]);
            //for (int i = 1; i < ranges.Length;i++ )
            //{
            //    if(ranges[i] ==ranges[i-1]+1)
            //}
            return "";
        }

        public bool Contains(int number)
        {
            if (Array.IndexOf(_ranges, number) >= 0)
            {
                return true;
            }
            else if (_ranges[_ranges.Length - 1] == -1)
            {
                if (number > _ranges[_ranges.Length - 2])
                    return true;
            }
            return false;
        }

        public bool AllPages { get { return _all; } set { _all = value; } }
        public int BeginNumber { get { return _number; } set { _number = value; } }

        public int[] PrinterRanges { get { return _ranges; } set { _ranges = value; } }
        public string Ranges
        {
            get
            {
                return _rangesString;
            }
            set
            {
                _rangesString = value;
                if (value.Length > 0)
                {
                    _ranges = GetRangesFromString(value);
                    AllPages = false;
                }
                else
                    AllPages = true;
            }
        }
    }
}
