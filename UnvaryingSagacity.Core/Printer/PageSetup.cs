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
    public class PageSetup
    {
        private const int PARAMCOUNT = 23;
        private object[] aryKey;

        //���¼�����浽ҳ��������
        private int mvarCopies;
        private string mvarPageContent;   //Ϊ��ʱ��ʾȫ������ӡ��ҳ��Χ��1.3.5-12��
        private bool mvarOrderByCopies;  //�Ƿ���ҳ��ӡ
        private PrinterSelectPage mvarPageMethod;  //��żҳ�Ĵ�ӡ����
        //

        private Font mvarPageHeaderFont;   //ҳü����
        private Font mvarMainTitleFont;  //����������
        private Font mvarSubTitleFont;  //����������
        private Font mvarHeaderFont;  //��ͷ����
        private Font mvarBodyFont;  //��������
        private Font mvarTailFont;  //��β����
        private Font mvarPageFooterFont;  //ҳ������

        private PrinterLogTextBorder mvarPageHeaderBorder;   //ҳü�߿�
        private PrinterLogTextBorder mvarMainTitleBorder;  //������߿�
        private PrinterLogTextBorder mvarSubTitleBorder;  //������߿�
        private PrinterLogTextBorder mvarHeaderBorder;  //��ͷ�߿�
        private PrinterLogTextBorder mvarBodyBorder;  //���ı߿�
        private PrinterLogTextBorder mvarTailBorder;  //��β�߿�
        private PrinterLogTextBorder mvarPageFooterBorder;  //ҳ�ű߿�

        private PrinterBorderStyle mvarPageHeaderBorderStyle;    //ҳü�߿�����
        private PrinterBorderStyle mvarMainTitleBorderStyle;  //������߿�����
        private PrinterBorderStyle mvarSubTitleBorderStyle;  //������߿�����
        private PrinterBorderStyle mvarHeaderBorderStyle;  //��ͷ�߿�����
        private PrinterBorderStyle mvarBodyBorderStyle;  //���ı߿�����
        private PrinterBorderStyle mvarTailBorderStyle;  //��β�߿�����
        private PrinterBorderStyle mvarPageFooterBorderStyle;  //ҳ�ű߿�����

        private int mvarPageHeaderColor = Color.Black.ToArgb();    //ҳü��ɫ
        private int mvarMainTitleColor = Color.Black.ToArgb();  //��������ɫ
        private int mvarSubTitleColor = Color.Black.ToArgb();  //��������ɫ
        private int mvarHeaderColor = Color.Black.ToArgb();  //��ͷ��ɫ
        private int mvarBodyColor = Color.Black.ToArgb();  //������ɫ
        private int mvarTailColor = Color.Black.ToArgb();  //��β��ɫ
        private int mvarPageFooterColor = Color.Black.ToArgb();  //ҳ����ɫ

        private bool mvarFullPage;    //��Ӧ��ӡ�������"ʹ�ô�ӡֽ��Ч��Χ"ѡ��
        private float mvarWidth;  //��Ӧƴ�ź���ҳ�Ŀ�
        private float mvarHeight;  //��Ӧƴ�ź���ҳ�ĸ�
        private float mvarScaleWidth;  //��Ӧʵ�ʿ�
        private float mvarScaleHeight;  //��Ӧʵ�ʸ�
        private float mvarPageWidth;  //��Ӧʵ��ҳ���
        private float mvarPageHeight;  //��Ӧʵ��ҳ���
        private string mvarPrintName;  //��ӡ��
        private int mvarPaperSize;  //ֽ��

        private bool mvarLandscape;  //ֽ�Ž�ֽ����
        private bool mvarPageLandscape;  //ҳ�淽��

        private PrinterResolutionKind mvarPrintQuality;  //��ӡ����
        private int mvarPaperBin;  //��ֽ��
        private int mvarPrintScale;  //��ӡ����

        private bool mvarCenterHoriz;   //ҳ��ˮƽ����
        private bool mvarCenterVert;  //ҳ�洹ֱ����

        private bool mvarPrintDate;  //��ӡ����
        private bool mvarPrintOperator;  //��ӡ������

        private int mvarFristPageNumber;  //��ʼҳ��
        private bool mvarPrintPageCount;  //��ӡ��ҳ��
        private bool mvarPrintPageNum;  //��ӡҳ��

        private float mvarHeaderMargin;  //top,left,right,button��ָҳ�߾�
        private float mvarTopMargin;
        private float mvarLeftMargin;
        private float mvarRightMargin;
        private float mvarBottomMargin;
        private float mvarFooterMargin;

        private bool mvarFillBlankLines;    //��Ӧ��ӡ�������"ĩҳ�ÿ��в���"
        private bool mvarColorPrint;  //��ɫ��ӡ
        private int mvarRows;  //��������
        private int mvarRowHeight;  //�����и�
        private PrinterBodyStyle mvarSheetBodyStyle;  //�������ĵ���
        private bool mvarCutLine;    //�ü���

        private int mvarAutoArrange;   //��Ӧ��ӡ�������"�Զ�ƴ��"
        private int mvarReduceFont;   //��Ӧ��ӡ�������"�Զ���С��������"
        private int mvarReduceMethod;  //��Ӧ��ӡ�������"���ŷ���"
        private int mvarZoom;      //��Ӧ��ӡ�������"�Զ�������Ӧҳ��ߴ�"

        private bool mvarKeepFont;   //��Ӧ��ӡ�������"�������屣��һ��"
        private int mvarHoriCutPage;   //��Ӧ��ӡ�������"������ҳ"

        private bool mvarCyLine;    //�����
        private bool mvarCyDotLine;    //�����Ϊ����

        private PrinterLogPictureLocation mvarLogPicLoc;    //�ձ�λ��
        private bool mvarLogPicPrint;    //�ձ��Ƿ��ӡ

        private PrinterLogTextLocation mvarLogTextLoc;     //��־�ı�λ��
        private bool mvarLogTextPrint;    //��־�ı��Ƿ��ӡ

        private PrinterLogTextBorder mvarLogTextBorder;    //��־�ı��߿�
        private string mvarLogText;     //��־�ı�
        private Font mvarLogTextFont;  //��־�ı�����
        private PrinterBorderStyle mvarLogTextBorderStyle;     //��־�ı��߿�����

        private int mvarDblLineRows;    //  �����мӴֺ���

        private bool mvarDrawLineUnderPageHeader;  //ҳü�´�ӡ����
        private bool mvarDrawLineUpPageFooter;  ////ҳ���ϴ�ӡ����

        private int mvarDisplayRate;  //��ʾ����

        private PrinterUnit printUnit;//��ӡ��λ,�ڲ�ʹ��0.01Inch

        /// <summary>
        /// ��������������ַ���
        /// </summary>
        public int MaxCharsOfCyCell { get; set; }

        public PageSetup()
        {
            aryKey = new object[PARAMCOUNT];
            Initialize(null);
            printUnit = PrinterUnit.Display;
            MaxCharsOfCyCell = 0;
        }   
        
        public PageSetup(PrinterUnit PrintUnit, PrintDocument printDoc)
        {
            aryKey = new object[PARAMCOUNT];
            Initialize(printDoc);
            printUnit = PrintUnit;
            MaxCharsOfCyCell = 0;
        }

        public string PaperName { get; set; }

        public object GetAttributes(PageSetupKey key)
        {
            try { return aryKey[(int)key]; }
            catch { return default(object); }
        }

        public object GetAttributes(int index)
        {
            try { return aryKey[index]; }
            catch { return default(object); }
        }

        public bool SetAttributes(PageSetupKey key, object value)
        {
            try { aryKey[(int)key] = value; return true; }
            catch { return false; }
        }

        public bool SetAttributes(int index, object value)
        {
            try { aryKey[index] = value; return true; }
            catch { return false; }
        }

        public PrinterUnit PrintUnit
        {
            get { return printUnit; }
            set { printUnit = value; }
        }

        public int DisplayRate
        {
            get { return mvarDisplayRate; }
            set { mvarDisplayRate = value; }
        }

        public int Copies
        {
            get { return mvarCopies; }
            set { mvarCopies = value; }
        }

        public string PageContent
        {
            get { return mvarPageContent; }
            set { mvarPageContent = value; }
        }

        internal Font PageFooterFont
        {
            get
            {
                if (mvarPageFooterFont == null)
                {
                    Font f = new Font(new FontFamily("����"), (float)9.0, FontStyle.Regular);
                    return f;
                }
                else
                    return mvarPageFooterFont;
            }
            set { mvarPageFooterFont = value; }
        }

        public string PageFooterFontString
        {
            get { return General.FontToString(PageFooterFont); }
            set { PageFooterFont = General.FontFromString(value); }
        }
        internal Font TailFont
        {
            get
            {
                if (mvarTailFont == null)
                {
                    Font f = new Font(new FontFamily("����"), (float)9.0, FontStyle.Regular);
                    return f;
                }
                else
                    return mvarTailFont;
            }
            set { mvarTailFont = value; }
        }
        public string TailFontString
        {
            get { return General.FontToString(TailFont); }
            set { TailFont = General.FontFromString(value); }
        }
        internal Font BodyFont
        {
            get
            {
                if (mvarBodyFont == null)
                {
                    Font f = new Font(new FontFamily("����"), (float)9.0, FontStyle.Regular);
                    return f;
                }
                else
                    return mvarBodyFont;
            }
            set { mvarBodyFont = value; }
        }
        public string BodyFontString
        {
            get { return General.FontToString(BodyFont); }
            set { BodyFont = General.FontFromString(value); }
        }
        internal Font HeaderFont
        {
            get
            {
                if (mvarHeaderFont == null)
                {
                    Font f = new Font(new FontFamily("����"), (float)9.0, FontStyle.Regular);
                    return f;
                }
                else
                    return mvarHeaderFont;
            }
            set { mvarHeaderFont = value; }
        }
        public string HeaderFontString
        {
            get { return General.FontToString(HeaderFont); }
            set { HeaderFont = General.FontFromString(value); }
        }
        internal Font SubTitleFont
        {
            get
            {
                if (mvarSubTitleFont == null)
                {
                    Font f = new Font(new FontFamily("����"), (float)10.0, FontStyle.Regular);
                    return f;
                }
                else
                    return mvarSubTitleFont;
            }
            set { mvarSubTitleFont = value; }
        }
        public string SubTitleFontString
        {
            get { return General.FontToString(SubTitleFont); }
            set { SubTitleFont = General.FontFromString(value); }
        }
        internal Font PageHeaderFont
        {
            get
            {
                if (mvarPageHeaderFont == null)
                {
                    Font f = new Font(new FontFamily("����"), (float)9.0, FontStyle.Regular);
                    return f;
                }
                else
                    return mvarPageHeaderFont;
            }
            set { mvarPageHeaderFont = value; }
        }
        public string PageHeaderFontString
        {
            get { return General.FontToString(PageHeaderFont); }
            set { PageHeaderFont = General.FontFromString(value); }
        }
        internal Font MainTitleFont
        {
            get
            {
                if (mvarMainTitleFont == null)
                {
                    Font f = new Font(new FontFamily("����"), (float)18.0, FontStyle.Bold);
                    return f;
                }
                else
                    return mvarMainTitleFont;
            }
            set { mvarMainTitleFont = value; }
        }
        public string MainTitleFontString
        {
            get { return General.FontToString(MainTitleFont); }
            set { MainTitleFont = General.FontFromString(value); }
        }
        internal int PageFooterColor
        {
            get { return mvarPageFooterColor; }
            set { mvarPageFooterColor = value; }
        }

        public int TailColor
        {
            get { return mvarTailColor; }
            set { mvarTailColor = value; }
        }

        public int BodyColor
        {
            get { return mvarBodyColor; }
            set { mvarBodyColor = value; }
        }

        public int HeaderColor
        {
            get { return mvarHeaderColor; }
            set { mvarHeaderColor = value; }
        }

        public int SubTitleColor
        {
            get { return mvarSubTitleColor; }
            set { mvarSubTitleColor = value; }
        }

        public int PageHeaderColor
        {
            get { return mvarPageHeaderColor; }
            set { mvarPageHeaderColor = value; }
        }

        public int MainTitleColor
        {
            get { return mvarMainTitleColor; }
            set { mvarMainTitleColor = value; }
        }

        public PrinterSelectPage PageMethod
        {
            get { return mvarPageMethod; }
            set { mvarPageMethod = value; }
        }

        public bool OrderByCopies
        {
            get { return mvarOrderByCopies; }
            set { mvarOrderByCopies = value; }
        }

        public bool PrintDate
        {
            get { return mvarPrintDate; }
            set { mvarPrintDate = value; }
        }

        public bool PrintOperator
        {
            get { return mvarPrintOperator; }
            set { mvarPrintOperator = value; }
        }

        internal bool DrawLineUnderPageHeader
        {
            get { return mvarDrawLineUnderPageHeader; }
            set { mvarDrawLineUnderPageHeader = value; }
        }

        public bool DrawLineUpPageFooter
        {
            get { return mvarDrawLineUpPageFooter; }
            set { mvarDrawLineUpPageFooter = value; }
        }

        public bool FullPage
        {
            get { return mvarFullPage; }
            set { mvarFullPage = value; }
        }

        public float ScaleWidth
        {
            get { return mvarScaleWidth; }
            set { mvarScaleWidth = value; }
        }

        public float ScaleHeight
        {
            get { return mvarScaleHeight; }
            set { mvarScaleHeight = value; }
        }

        public float Width
        {
            get { return mvarWidth; }
            set { mvarWidth = value; }
        }

        public float Height
        {
            get { return mvarHeight; }
            set { mvarHeight = value; }
        }

        public float PageWidth
        {
            get { return mvarPageWidth; }
            set { mvarPageWidth = value; }
        }

        public float PageHeight
        {
            get { return mvarPageHeight; }
            set { mvarPageHeight = value; }
        }

        public string PrintName
        {
            get { return mvarPrintName; }
            set { mvarPrintName = value; }
        }

        public int PaperSize
        {
            get { return mvarPaperSize; }
            set { mvarPaperSize = value; }
        }

        public bool Landscape
        {
            get { return mvarLandscape; }
            set { mvarLandscape = value; }
        }

        public bool PageLandscape
        {
            get { return mvarPageLandscape; }
            set { mvarPageLandscape = value; }
        }

        public PrinterResolutionKind PrintQuality
        {
            get { return mvarPrintQuality; }
            set { mvarPrintQuality = value; }
        }

        public int PaperBin
        {
            get { return mvarPaperBin; }
            set { mvarPaperBin = value; }
        }

        public int PrintScale
        {
            get { return mvarPrintScale; }
            set { mvarPrintScale = value; }
        }

        public bool CenterHoriz
        {
            get { return mvarCenterHoriz; }
            set { mvarCenterHoriz = value; }
        }

        public bool CenterVert
        {
            get { return mvarCenterVert; }
            set { mvarCenterVert = value; }
        }

        public int FristPageNumber
        {
            get { return mvarFristPageNumber; }
            set { mvarFristPageNumber = value; }
        }

        public float HeaderMargin
        {
            get { return mvarHeaderMargin; }
            set { mvarHeaderMargin = value; }
        }

        public float TopMargin
        {
            get { return mvarTopMargin; }
            set { mvarTopMargin = value; }
        }

        public float LeftMargin
        {
            get { return mvarLeftMargin; }
            set { mvarLeftMargin = value; }
        }

        public float RightMargin
        {
            get { return mvarRightMargin; }
            set { mvarRightMargin = value; }
        }

        public float BottomMargin
        {
            get { return mvarBottomMargin; }
            set { mvarBottomMargin = value; }
        }

        public float FooterMargin
        {
            get { return mvarFooterMargin; }
            set { mvarFooterMargin = value; }
        }

        public bool FillBlankLines
        {
            get { return mvarFillBlankLines; }
            set { mvarFillBlankLines = value; }
        }

        public bool ColorPrint
        {
            get { return mvarColorPrint; }
            set { mvarColorPrint = value; }
        }

        public int Rows
        {
            get { return mvarRows; }
            set { mvarRows = value; }
        }

        public int RowHeight
        {
            get { return mvarRowHeight; }
            set { mvarRowHeight = value; }
        }

        public PrinterBodyStyle SheetBodyStyle
        {
            get { return mvarSheetBodyStyle; }
            set { mvarSheetBodyStyle = value; }
        }

        public bool CutLine
        {
            get { return mvarCutLine; }
            set { mvarCutLine = value; }
        }

        public int AutoArrange
        {
            get { return mvarAutoArrange; }
            set { mvarAutoArrange = value; }
        }

        public int ReduceFont
        {
            get { return mvarReduceFont; }
            set { mvarReduceFont = value; }
        }

        public int ReduceMethod
        {
            get { return mvarReduceMethod; }
            set { mvarReduceMethod = value; }
        }

        public int Zoom
        {
            get { return mvarZoom; }
            set { mvarZoom = value; }
        }

        public bool KeepFont
        {
            get { return mvarKeepFont; }
            set { mvarKeepFont = value; }
        }

        public int HoriCutPage
        {
            get { return mvarHoriCutPage; }
            set { mvarHoriCutPage = value; }
        }

        public bool PrintPageNum
        {
            get { return mvarPrintPageNum; }
            set { mvarPrintPageNum = value; }
        }

        public bool PrintPageCount
        {
            get { return mvarPrintPageCount; }
            set { mvarPrintPageCount = value; }
        }

        public int DblLineRows
        {
            get { return mvarDblLineRows; }
            set { mvarDblLineRows = value; }
        }

        public bool CyLine
        {
            get { return mvarCyLine; }
            set { mvarCyLine = value; }
        }

        public bool CyDotLine
        {
            get { return mvarCyDotLine; }
            set { mvarCyDotLine = value; }
        }

        public Image LogPic { get; set; }

        public string LogText
        {
            get { return mvarLogText; }
            set { mvarLogText = value; }
        }

        internal Font LogTextFont
        {
            get { return mvarLogTextFont; }
            set { mvarLogTextFont = value; }
        }

        public string LogTextFontString
        {
            get { return General.FontToString(LogTextFont); }
            set { LogTextFont = General.FontFromString(value); }
        }

        public PrinterLogPictureLocation LogPicLoc
        {
            get { return mvarLogPicLoc; }
            set { mvarLogPicLoc = value; }
        }

        public bool LogTextPrint
        {
            get { return mvarLogTextPrint; }
            set { mvarLogTextPrint = value; }
        }

        public PrinterLogTextLocation LogTextLoc
        {
            get { return mvarLogTextLoc; }
            set { mvarLogTextLoc = value; }
        }

        public bool LogPicPrint
        {
            get { return mvarLogPicPrint; }
            set { mvarLogPicPrint = value; }
        }

        public PrinterBorderStyle LogTextBorderStyle
        {
            get { return mvarLogTextBorderStyle; }
            set { mvarLogTextBorderStyle = value; }
        }

        public PrinterLogTextBorder LogTextBorder
        {
            get { return mvarLogTextBorder; }
            set { mvarLogTextBorder = value; }
        }

        public PrinterLogTextBorder PageFooterBorder
        {
            get { return mvarPageFooterBorder; }
            set { mvarPageFooterBorder = value; }
        }

        public PrinterLogTextBorder TailBorder
        {
            get { return mvarTailBorder; }
            set { mvarTailBorder = value; }
        }

        public PrinterLogTextBorder BodyBorder
        {
            get { return mvarBodyBorder; }
            set { mvarBodyBorder = value; }
        }

        public PrinterLogTextBorder HeaderBorder
        {
            get { return mvarHeaderBorder; }
            set { mvarHeaderBorder = value; }
        }

        public PrinterLogTextBorder SubTitleBorder
        {
            get { return mvarSubTitleBorder; }
            set { mvarSubTitleBorder = value; }
        }

        public PrinterLogTextBorder PageHeaderBorder
        {
            get { return mvarPageHeaderBorder; }
            set { mvarPageHeaderBorder = value; }
        }

        public PrinterLogTextBorder MainTitleBorder
        {
            get { return mvarMainTitleBorder; }
            set { mvarMainTitleBorder = value; }
        }

        public PrinterBorderStyle PageFooterBorderStyle
        {
            get { return mvarPageFooterBorderStyle; }
            set { mvarPageFooterBorderStyle = value; }
        }

        public PrinterBorderStyle TailBorderStyle
        {
            get { return mvarTailBorderStyle; }
            set { mvarTailBorderStyle = value; }
        }

        public PrinterBorderStyle BodyBorderStyle
        {
            get { return mvarBodyBorderStyle; }
            set { mvarBodyBorderStyle = value; }
        }

        public PrinterBorderStyle HeaderBorderStyle
        {
            get { return mvarHeaderBorderStyle; }
            set { mvarHeaderBorderStyle = value; }
        }

        public PrinterBorderStyle SubTitleBorderStyle
        {
            get { return mvarSubTitleBorderStyle; }
            set { mvarSubTitleBorderStyle = value; }
        }

        public PrinterBorderStyle PageHeaderBorderStyle
        {
            get { return mvarPageHeaderBorderStyle; }
            set { mvarPageHeaderBorderStyle = value; }
        }

        public PrinterBorderStyle MainTitleBorderStyle
        {
            get { return mvarMainTitleBorderStyle; }
            set { mvarMainTitleBorderStyle = value; }
        }

        public bool Rotate90 { get; set; }

        public bool ExistPrinter(string PrinterName)
        {
            PrinterSettings.StringCollection printers = PrinterSettings.InstalledPrinters;
            System.Collections.IEnumerator myPrinter = printers.GetEnumerator();
            while (myPrinter.MoveNext())
            {
                if (myPrinter.Current.ToString().CompareTo(PrinterName) == 0)
                    return true;
            }
            return false;
        }

        public bool SupportColor(string PrinterName)
        {
            PrinterSettings pSet = new PrinterSettings();
            pSet.PrinterName = PrinterName;
            if (pSet.IsValid)
                return pSet.DefaultPageSettings.Color;
            else
                return false;
        }

        internal PageArrange PageArrange(bool isPreview)
        {
            PointF p = new PointF();
            p.X = (this.PageWidth - this.ScaleWidth) / 2;
            p.Y = (this.PageHeight - this.ScaleHeight) / 2;
            if (p.X <= 0)
                p.X = 0;
            if (p.Y <= 0)
                p.Y = 0;
            return PageArrange(p, new Size((int)this.ScaleWidth, (int)this.ScaleHeight), new Size((int)this.Width, (int)this.Height), isPreview);
        }
        internal PageArrange PageArrange(PointF p, Size PrintArea, Size PageArea, bool isPreview)
        {
            return PageArrange(new Point((int)p.X, (int)p.Y), PrintArea, PageArea, isPreview);
        }
        internal PageArrange PageArrange(Point p, Size PrintArea, Size PageArea,bool isPreview)
        {
            PageArrange arrange;//�Զ�ƴ�ź��ҳ,û��ƴ������һҳ
            int hCount; int vCount;

            if (this.AutoArrange == 1)
            {
                //if (Rotate90 && !isPreview)//����,�߱��
                //{
                //    vCount = (int)(PrintArea.Width / PageArea.Width);
                //    if (vCount <= 0)
                //        vCount = 1;
                //    hCount = (int)(PrintArea.Height / PageArea.Height);
                //    if (hCount <= 0)
                //        hCount = 1;
                //}
                //else
                //{
                    vCount = (int)(PrintArea.Height / PageArea.Height);
                    if (vCount <= 0)
                        vCount = 1;
                    hCount = (int)(PrintArea.Width / PageArea.Width);
                    if (hCount <= 0)
                        hCount = 1;
                //}
            }
            else
            {
                vCount = 1;
                hCount = 1;
            }
            //if (Rotate90 && !isPreview)
            //{
            //    arrange = new PageArrange(hCount, vCount, Height, Width, p);
            //}
            //else
            //{
                arrange = new PageArrange(hCount, vCount, Width, Height, p);
            //}
            return arrange;
        }

        private void Initialize(PrintDocument printDoc)
        {
            if (printDoc != null)
            {
                PrinterSettings Printer = printDoc.PrinterSettings;
                if (Printer.IsValid)
                {
                    this.PrintName = Printer.PrinterName;
                    System.Drawing.Printing.PageSettings ps = printDoc.DefaultPageSettings;
                    System.Drawing.Printing.PaperSize pSize = ps.PaperSize; 
                    this.PaperSize = pSize.RawKind;
                    this.PaperName = pSize.PaperName;
                    this.Landscape = ps.Landscape;
                    this.PrintQuality = ps.PrinterResolution.Kind;
                    this.PaperBin = ps.PaperSource.RawKind;
                    this.ColorPrint = Printer.SupportsColor;
                    SizeF szf = ps.PrintableArea.Size;
                    //this.PaperSize = printDoc.DefaultPageSettings.PaperSize.RawKind;
                    //this.PaperName = printDoc.DefaultPageSettings.PaperSize.PaperName;
                    //this.Landscape = printDoc.DefaultPageSettings.Landscape;
                    //this.PrintQuality = printDoc.DefaultPageSettings.PrinterResolution.Kind;
                    //this.PaperBin = printDoc.DefaultPageSettings.PaperSource.RawKind;
                    //this.ColorPrint = Printer.SupportsColor;
                    if (ps.Landscape)
                    {
                        this.PageHeight = pSize.Width;// Printer.ScaleHeight;// 16273
                        this.PageWidth = pSize.Height;// Printer.ScaleWidth;//11340
                        this.ScaleHeight = szf.Width;
                        this.ScaleWidth = szf.Height;
                        this.Height = szf.Width;
                        this.Width = szf.Height;
                    }
                    else
                    {
                        this.PageHeight = pSize.Height;// Printer.ScaleHeight;// 16273
                        this.PageWidth = pSize.Width;// Printer.ScaleWidth;//11340
                        this.ScaleHeight = szf.Height;// Printer.ScaleHeight;// 16273
                        this.ScaleWidth = szf.Width;// Printer.ScaleWidth;//11340
                        this.Height = szf.Height;
                        this.Width = szf.Width;
                    }
                }
            }
            this.FullPage = true;
            this.SheetBodyStyle = PrinterBodyStyle.�Զ�;
            this.DisplayRate = 1; //����
            this.TopMargin = PrinterUnitConvert.Convert(1000, System.Drawing.Printing.PrinterUnit.HundredthsOfAMillimeter, System.Drawing.Printing.PrinterUnit.Display);       //10 MM;
            this.BottomMargin = PrinterUnitConvert.Convert(1000, System.Drawing.Printing.PrinterUnit.HundredthsOfAMillimeter, System.Drawing.Printing.PrinterUnit.Display);       // 10 MM;
            this.LeftMargin = PrinterUnitConvert.Convert(1000, System.Drawing.Printing.PrinterUnit.HundredthsOfAMillimeter, System.Drawing.Printing.PrinterUnit.Display);       // 10 MM;
            this.RightMargin = PrinterUnitConvert.Convert(1000, System.Drawing.Printing.PrinterUnit.HundredthsOfAMillimeter, System.Drawing.Printing.PrinterUnit.Display);       // 10 MM;
            this.HeaderMargin = PrinterUnitConvert.Convert(100, System.Drawing.Printing.PrinterUnit.HundredthsOfAMillimeter, System.Drawing.Printing.PrinterUnit.Display);       // 1 MM;
            this.FooterMargin = PrinterUnitConvert.Convert(100, System.Drawing.Printing.PrinterUnit.HundredthsOfAMillimeter, System.Drawing.Printing.PrinterUnit.Display);       // 1 MM;
            this.ReduceMethod = 1;
            this.Zoom = 1;
            //this.BodyFont = new Font("����", 9, FontStyle.Regular);
            //this.MainTitleFont = new Font("����", 18, FontStyle.Bold);
            //this.SubTitleFont = new Font("����", 10, FontStyle.Regular);
            //this.HeaderFont = new Font("����", 9, FontStyle.Regular);
            //this.TailFont = new Font("����", 9, FontStyle.Regular);
            //this.PageHeaderFont = new Font("����", 9, FontStyle.Regular);
            //this.PageFooterFont = new Font("����", 9, FontStyle.Regular);


            this.Copies = 1;
            this.PageContent = "";
            this.OrderByCopies = true;
            this.PageMethod = PrinterSelectPage.��ӡ����ҳ;
            this.SetAttributes(PageSetupKey.ePage_ͼ���ӡ�߶�, 70);
            this.SetAttributes(PageSetupKey.ePage_ͼ���ӡ���, 122);
            this.SetAttributes(PageSetupKey.ePage_����������ҳü�ı߿�, false);
            this.SetAttributes(PageSetupKey.ePage_����������������ı߿�, false);
            this.SetAttributes(PageSetupKey.ePage_���������ø�����ı߿�, false);
            this.SetAttributes(PageSetupKey.ePage_���������ñ�ͷ�ı߿�, false);
            this.SetAttributes(PageSetupKey.ePage_���������ñ�β�ı߿�, false);
            this.SetAttributes(PageSetupKey.ePage_����������ҳ�ŵı߿�, false);
            this.SetAttributes(PageSetupKey.ePage_���������ñ�־�ı�������, false);
            this.SetAttributes(PageSetupKey.ePage_������������̶�����, false);
            this.SetAttributes(PageSetupKey.ePage_�Զ�����ģʽ, 2);
            this.SetAttributes(PageSetupKey.ePage_���Ҫ��ǧ��λ, true);
            this.SetAttributes(PageSetupKey.ePage_ϸ�߿��, 1);
            this.SetAttributes(PageSetupKey.ePage_����߿�Ϊ˫��, false);
            this.SetAttributes(PageSetupKey.ePage_PageHeaderBorderColor, "0,0");   //ҳü�߿���ɫ
            this.SetAttributes(PageSetupKey.ePage_MainTitleBorderColor, "0,0");   //������߿���ɫ
            this.SetAttributes(PageSetupKey.ePage_SubTitleBorderColor, "0,0");   //������߿���ɫ
            this.SetAttributes(PageSetupKey.ePage_HeaderBorderColor, "0,0");   //��ͷ�߿���ɫ
            this.SetAttributes(PageSetupKey.ePage_BodyBorderColor, "0,0");   //���ı߿���ɫ
            this.SetAttributes(PageSetupKey.ePage_TailBorderColor, "0,0");   //��β�߿���ɫ
            this.SetAttributes(PageSetupKey.ePage_PageFooterBorderColor, "0,0");   //ҳ�ű߿���ɫ-----------------------21
            this.SetAttributes(PageSetupKey.ePage_����ߵ�����ַ��� , 0);
        }
    }
}