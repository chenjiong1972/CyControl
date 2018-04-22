using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

#region 在内部单位统一使用0.01英寸=1/4mm
/// <summary>
/// 在内部单位统一使用0.01英寸=1/4mm,即默认单位PrinterUnit.Display
/// 使用代码编辑器修改此方法的内容。
/// </summary>
#endregion

namespace UnvaryingSagacity.Core.Printer
{
    public class PageSetup
    {
        private const int PARAMCOUNT = 23;
        private object[] aryKey;

        //以下及项不保存到页面设置中
        private int mvarCopies;
        private string mvarPageContent;   //为空时表示全部，打印的页范围（1.3.5-12）
        private bool mvarOrderByCopies;  //是否逐页打印
        private PrinterSelectPage mvarPageMethod;  //奇偶页的打印方法
        //

        private Font mvarPageHeaderFont;   //页眉字体
        private Font mvarMainTitleFont;  //主标题字体
        private Font mvarSubTitleFont;  //副标题字体
        private Font mvarHeaderFont;  //表头字体
        private Font mvarBodyFont;  //正文字体
        private Font mvarTailFont;  //表尾字体
        private Font mvarPageFooterFont;  //页脚字体

        private PrinterLogTextBorder mvarPageHeaderBorder;   //页眉边框
        private PrinterLogTextBorder mvarMainTitleBorder;  //主标题边框
        private PrinterLogTextBorder mvarSubTitleBorder;  //副标题边框
        private PrinterLogTextBorder mvarHeaderBorder;  //表头边框
        private PrinterLogTextBorder mvarBodyBorder;  //正文边框
        private PrinterLogTextBorder mvarTailBorder;  //表尾边框
        private PrinterLogTextBorder mvarPageFooterBorder;  //页脚边框

        private PrinterBorderStyle mvarPageHeaderBorderStyle;    //页眉边框线形
        private PrinterBorderStyle mvarMainTitleBorderStyle;  //主标题边框线形
        private PrinterBorderStyle mvarSubTitleBorderStyle;  //副标题边框线形
        private PrinterBorderStyle mvarHeaderBorderStyle;  //表头边框线形
        private PrinterBorderStyle mvarBodyBorderStyle;  //正文边框线形
        private PrinterBorderStyle mvarTailBorderStyle;  //表尾边框线形
        private PrinterBorderStyle mvarPageFooterBorderStyle;  //页脚边框线形

        private int mvarPageHeaderColor = Color.Black.ToArgb();    //页眉颜色
        private int mvarMainTitleColor = Color.Black.ToArgb();  //主标题颜色
        private int mvarSubTitleColor = Color.Black.ToArgb();  //副标题颜色
        private int mvarHeaderColor = Color.Black.ToArgb();  //表头颜色
        private int mvarBodyColor = Color.Black.ToArgb();  //正文颜色
        private int mvarTailColor = Color.Black.ToArgb();  //表尾颜色
        private int mvarPageFooterColor = Color.Black.ToArgb();  //页脚颜色

        private bool mvarFullPage;    //对应打印设置里的"使用打印纸有效范围"选项
        private float mvarWidth;  //对应拼排和折页的宽
        private float mvarHeight;  //对应拼排和折页的高
        private float mvarScaleWidth;  //对应实际宽
        private float mvarScaleHeight;  //对应实际高
        private float mvarPageWidth;  //对应实际页面宽
        private float mvarPageHeight;  //对应实际页面高
        private string mvarPrintName;  //打印机
        private int mvarPaperSize;  //纸张

        private bool mvarLandscape;  //纸张进纸方向
        private bool mvarPageLandscape;  //页面方向

        private PrinterResolutionKind mvarPrintQuality;  //打印质量
        private int mvarPaperBin;  //送纸器
        private int mvarPrintScale;  //打印比例

        private bool mvarCenterHoriz;   //页面水平居中
        private bool mvarCenterVert;  //页面垂直居中

        private bool mvarPrintDate;  //打印日期
        private bool mvarPrintOperator;  //打印操作人

        private int mvarFristPageNumber;  //起始页号
        private bool mvarPrintPageCount;  //打印总页号
        private bool mvarPrintPageNum;  //打印页号

        private float mvarHeaderMargin;  //top,left,right,button是指页边距
        private float mvarTopMargin;
        private float mvarLeftMargin;
        private float mvarRightMargin;
        private float mvarBottomMargin;
        private float mvarFooterMargin;

        private bool mvarFillBlankLines;    //对应打印设置里的"末页用空行补齐"
        private bool mvarColorPrint;  //彩色打印
        private int mvarRows;  //正文行数
        private int mvarRowHeight;  //正文行高
        private PrinterBodyStyle mvarSheetBodyStyle;  //描述正文的行
        private bool mvarCutLine;    //裁剪线

        private int mvarAutoArrange;   //对应打印设置里的"自动拼排"
        private int mvarReduceFont;   //对应打印设置里的"自动缩小正文字体"
        private int mvarReduceMethod;  //对应打印设置里的"缩放方法"
        private int mvarZoom;      //对应打印设置里的"自动缩放适应页面尺寸"

        private bool mvarKeepFont;   //对应打印设置里的"正文字体保持一致"
        private int mvarHoriCutPage;   //对应打印设置里的"横向折页"

        private bool mvarCyLine;    //金额线
        private bool mvarCyDotLine;    //金额线为虚线

        private PrinterLogPictureLocation mvarLogPicLoc;    //徽标位置
        private bool mvarLogPicPrint;    //徽标是否打印

        private PrinterLogTextLocation mvarLogTextLoc;     //标志文本位置
        private bool mvarLogTextPrint;    //标志文本是否打印

        private PrinterLogTextBorder mvarLogTextBorder;    //标志文本边框
        private string mvarLogText;     //标志文本
        private Font mvarLogTextFont;  //标志文本字体
        private PrinterBorderStyle mvarLogTextBorderStyle;     //标志文本边框类型

        private int mvarDblLineRows;    //  隔几行加粗横线

        private bool mvarDrawLineUnderPageHeader;  //页眉下打印横线
        private bool mvarDrawLineUpPageFooter;  ////页脚上打印横线

        private int mvarDisplayRate;  //显示比例

        private PrinterUnit printUnit;//打印单位,内部使用0.01Inch

        /// <summary>
        /// 金额线网格的最大字符数
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
                    Font f = new Font(new FontFamily("宋体"), (float)9.0, FontStyle.Regular);
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
                    Font f = new Font(new FontFamily("宋体"), (float)9.0, FontStyle.Regular);
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
                    Font f = new Font(new FontFamily("宋体"), (float)9.0, FontStyle.Regular);
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
                    Font f = new Font(new FontFamily("宋体"), (float)9.0, FontStyle.Regular);
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
                    Font f = new Font(new FontFamily("宋体"), (float)10.0, FontStyle.Regular);
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
                    Font f = new Font(new FontFamily("宋体"), (float)9.0, FontStyle.Regular);
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
                    Font f = new Font(new FontFamily("宋体"), (float)18.0, FontStyle.Bold);
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
            PageArrange arrange;//自动拼排后的页,没有拼排则有一页
            int hCount; int vCount;

            if (this.AutoArrange == 1)
            {
                //if (Rotate90 && !isPreview)//宽变高,高变宽
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
            this.SheetBodyStyle = PrinterBodyStyle.自动;
            this.DisplayRate = 1; //索引
            this.TopMargin = PrinterUnitConvert.Convert(1000, System.Drawing.Printing.PrinterUnit.HundredthsOfAMillimeter, System.Drawing.Printing.PrinterUnit.Display);       //10 MM;
            this.BottomMargin = PrinterUnitConvert.Convert(1000, System.Drawing.Printing.PrinterUnit.HundredthsOfAMillimeter, System.Drawing.Printing.PrinterUnit.Display);       // 10 MM;
            this.LeftMargin = PrinterUnitConvert.Convert(1000, System.Drawing.Printing.PrinterUnit.HundredthsOfAMillimeter, System.Drawing.Printing.PrinterUnit.Display);       // 10 MM;
            this.RightMargin = PrinterUnitConvert.Convert(1000, System.Drawing.Printing.PrinterUnit.HundredthsOfAMillimeter, System.Drawing.Printing.PrinterUnit.Display);       // 10 MM;
            this.HeaderMargin = PrinterUnitConvert.Convert(100, System.Drawing.Printing.PrinterUnit.HundredthsOfAMillimeter, System.Drawing.Printing.PrinterUnit.Display);       // 1 MM;
            this.FooterMargin = PrinterUnitConvert.Convert(100, System.Drawing.Printing.PrinterUnit.HundredthsOfAMillimeter, System.Drawing.Printing.PrinterUnit.Display);       // 1 MM;
            this.ReduceMethod = 1;
            this.Zoom = 1;
            //this.BodyFont = new Font("宋体", 9, FontStyle.Regular);
            //this.MainTitleFont = new Font("宋体", 18, FontStyle.Bold);
            //this.SubTitleFont = new Font("宋体", 10, FontStyle.Regular);
            //this.HeaderFont = new Font("宋体", 9, FontStyle.Regular);
            //this.TailFont = new Font("宋体", 9, FontStyle.Regular);
            //this.PageHeaderFont = new Font("宋体", 9, FontStyle.Regular);
            //this.PageFooterFont = new Font("宋体", 9, FontStyle.Regular);


            this.Copies = 1;
            this.PageContent = "";
            this.OrderByCopies = true;
            this.PageMethod = PrinterSelectPage.打印所有页;
            this.SetAttributes(PageSetupKey.ePage_图标打印高度, 70);
            this.SetAttributes(PageSetupKey.ePage_图标打印宽度, 122);
            this.SetAttributes(PageSetupKey.ePage_不允许设置页眉的边框, false);
            this.SetAttributes(PageSetupKey.ePage_不允许设置主标题的边框, false);
            this.SetAttributes(PageSetupKey.ePage_不允许设置副标题的边框, false);
            this.SetAttributes(PageSetupKey.ePage_不允许设置表头的边框, false);
            this.SetAttributes(PageSetupKey.ePage_不允许设置表尾的边框, false);
            this.SetAttributes(PageSetupKey.ePage_不允许设置页脚的边框, false);
            this.SetAttributes(PageSetupKey.ePage_不允许设置标志文本的内容, false);
            this.SetAttributes(PageSetupKey.ePage_正文字体包括固定行列, false);
            this.SetAttributes(PageSetupKey.ePage_自动缩放模式, 2);
            this.SetAttributes(PageSetupKey.ePage_金额要求千分位, true);
            this.SetAttributes(PageSetupKey.ePage_细线宽度, 1);
            this.SetAttributes(PageSetupKey.ePage_网格边框为双线, false);
            this.SetAttributes(PageSetupKey.ePage_PageHeaderBorderColor, "0,0");   //页眉边框颜色
            this.SetAttributes(PageSetupKey.ePage_MainTitleBorderColor, "0,0");   //主标题边框颜色
            this.SetAttributes(PageSetupKey.ePage_SubTitleBorderColor, "0,0");   //副标题边框颜色
            this.SetAttributes(PageSetupKey.ePage_HeaderBorderColor, "0,0");   //表头边框颜色
            this.SetAttributes(PageSetupKey.ePage_BodyBorderColor, "0,0");   //正文边框颜色
            this.SetAttributes(PageSetupKey.ePage_TailBorderColor, "0,0");   //表尾边框颜色
            this.SetAttributes(PageSetupKey.ePage_PageFooterBorderColor, "0,0");   //页脚边框颜色-----------------------21
            this.SetAttributes(PageSetupKey.ePage_金额线的最大字符数 , 0);
        }
    }
}