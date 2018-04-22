using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;

namespace UnvaryingSagacity.Core.Printer
{
    
    public class AttachRow
    {
        
        private Font defaultFont;
        private int offset_V;
        private ListDictionaryEx <PrinterAttachCell> attachs = new ListDictionaryEx<PrinterAttachCell>();
        private string key;

        public Font DefaultFont
        {
            get { return defaultFont; }
            set { defaultFont = value; }
        }

        public int Offset_V
        {
            get { return offset_V; }
            set { offset_V = value; }
        }

        public bool Add(PrinterAttachCell attachCell, string key)
        {
            if (!attachs.ContainsKey(key))
            {
                if (attachCell.Properties.Value == null)
                    attachCell.Properties.Value = "";
                attachs.Add( key,attachCell);
                return true;
            }
            else
                return false;
        }

        public int Count { get { return attachs.Count; } }

        public void Clear() { attachs.Clear(); }

        public void Remove(int index) { attachs.Remove(index); }

        public void Remove(string key) { attachs.Remove(key); }

        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        public PrinterAttachCell AttachCell(int index) { return attachs.GetItem(index); }
        public PrinterAttachCell AttachCell(string key) { return attachs.GetItem(key); }

        public bool AttachCell(int index, PrinterAttachCell newCell)
        {
            return attachs.SetItem(index - 1, newCell);
        }
        public bool AttachCell(string key, PrinterAttachCell newCell)
        {
            return attachs.SetItem(key, newCell);
        }
    }

    public class AttachRows
    {
        private ListDictionaryEx<AttachRow> attachRows = new ListDictionaryEx<AttachRow>();

        public IEnumerator<AttachRow> GetEnumerator()
        {
            for (int i = 0; i < attachRows.Count; i++)
            {
                yield return attachRows[i];
            }
        }

        public bool Add(AttachRow attachRow , string key)
        {
            if (!attachRows.ContainsKey(key))
            {
                attachRows.Add(key,attachRow);
                return true;
            }
            else
                return false;
        }

        public AttachRow TheAttachRow(int index) { return attachRows.GetItem(index); }

        public AttachRow TheAttachRow(string key) { return attachRows.GetItem(key); }

        public int Count { get { return attachRows.Count; } }

        public void Clear() { attachRows.Clear(); }

        public void Remove(int index) { attachRows.Remove(index); }

        public void Remove(string key) { attachRows.Remove(key); }
    }

    public class Range
    {
        private int startRow;
        private int startCol;
        private int endRow;
        private int endCol;

        public Range(int StartRow, int StartCol, int EndRow,int EndCol)
        {
            startRow = StartRow; startCol = StartCol; endRow = EndRow; endCol = EndCol;
        }

        public int StartRow
        {
            get { return startRow; }
            set { startRow = value; }
        }

        public int StartCol
        {
            get { return startCol; }
            set { startCol = value; }
        }

        public int EndRow
        {
            get { return endRow; }
            set { endRow = value; }
        }

        public int EndCol
        {
            get { return endCol; }
            set { endCol = value; }
        }
    }

    public class Ranges:ListDictionaryEx<Range>{}

    public class Body:ListDictionaryEx<PrinterCell>
    {
        private Font defaultFont = default(Font);
        private int offset_V;

        public Font DefaultFont
        {
            get { return defaultFont; }
            set { defaultFont = (Font)value.Clone(); }
        }
        public int Offset_V
        {
            get { return offset_V; }
            set { offset_V = value; }
        }
    }

    public class Col
    {
        private int width;
        private float printWidth;
        private string key;

        public Col(int Width)
        {
            width = Width;
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        internal float PrintWidth
        {
            get { return printWidth; }
            set { printWidth = value; }
        }

        public string Key
        {
            get { return key; }
            set { key = value; }
        }
    }

    public class Cols:ListDictionaryEx <Col >
    {
        public int ColWith(int index)        
        {
            Col col=this.GetItem(index );
            if (col != null)
                return col.Width;
            else
                return 0;
        }

        public int ColWidth(string key)
        {
            Col col = this.GetItem(key);
            if (col != null)
                return col.Width;
            else
                return 0;
        }

        internal  float  ColPrintWith(int index)
        {
            Col col = this.GetItem(index);
            if (col != null)
                return col.PrintWidth ;
            else
                return 0;
        }

        internal float ColPrintWith(string key)
        {
            Col col = this.GetItem(key);
            if (col != null)
                return col.PrintWidth;
            else
                return 0;
        }

    }

    public class Row
    {
        private string  key;
        private int height;
        private float printHeight;

        public Row(int Height)
        {
            height = Height;
            printHeight = Height;
        }

        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        public int Height
        {
            get { return height; }
            set 
            { 
                height = value;
                printHeight = height;
            }
        }

        public float PrintHeight
        {
            get { return printHeight; }
            set { printHeight = value; }
        }
    }

    public class Rows:ListDictionaryEx<Row>
    {
        public int RowHeight(int index)
        {
            Row r = this.GetItem(index);
            if (r != null)
                return r.Height;
            else
                return 0;
        }

        public int RowHeight(string key)
        {
            Row r = this.GetItem(key);
            if (r != null)
                return r.Height;
            else
                return 0;
        }

        internal float  RowPrintHeight(int index)
        {
            Row r = this.GetItem(index);
            if (r != null)
                return r.PrintHeight;
            else
                return 0;
        }

        internal float RowPrintHeight(string key)
        {
            Row r = this.GetItem(key);
            if (r != null)
                return r.PrintHeight;
            else
                return 0;
        }
    }

    public class PictureItem
    {
        private Rectangle rect;
        private float startRow;
        private float startCol;
        private float endRow;
        private float endCol;
        private Image image=default(Image) ;

        public Image Content
        {
            get { return image; }
            set { image = value; }
        }

        public float StartRow
        {
            get { return startRow; }
            set { startRow = value; }
        }

        public float StartCol
        {
            get { return startCol; }
            set { startCol = value; }
        }

        public float EndRow
        {
            get { return endRow; }
            set { endRow = value; }
        }

        public float EndCol
        {
            get { return endCol; }
            set { endCol = value; }
        }

        internal Rectangle PictureRect
        {
            get { return rect; }
            set { rect = value; }
        }
    }

    public class Pictures:ListDictionaryEx<PictureItem>{}

    public class PrintData
    {
        internal int phyPageCount = 0;
        internal int logicPageCount = 0;

        private int bottomFixRows = 0;
        private int leftFixedCols = 0;
        private int rightFixedCols = 0;
        private int topFixedRows = 0;
        private Body body = new Body();
        private Cols cols = new Cols();
        private Rows rows = new Rows();
        private Ranges mergers = new Ranges();
        private Pictures pictures = new Pictures();
        private AttachRows mainTitle = new AttachRows();
        private AttachRows headers = new AttachRows();
        private AttachRows pageFoots = new AttachRows();
        private AttachRows pageHeaders = new AttachRows();
        private AttachRows subTitles = new AttachRows();
        private AttachRows tails = new AttachRows();
        private AttachRows leftAttachText = new AttachRows();  //Íø¸ñ×ó±ßµÄÃèÊöÐÔÎÄ×Ö
        private AttachRows rightAttachText = new AttachRows();
        private Ranges ranges = new Ranges();

        public string Key;

        public PrintData()
        {
        }

        public int PhyPageCount
        {
            get { return phyPageCount; }
        }

        public int LogicPageCount
        {
            get { return logicPageCount; }
        }

        public AttachRows MainTitle
        {
            get { return mainTitle; }
            set { mainTitle = value; }
        }

        public AttachRows SubTitles
        {
            get
            {
                return subTitles;
            }
            set
            {
                subTitles = value;
            }
        }

        public AttachRows PageHeaders
        {
            get
            {
                return pageHeaders;
            }
            set
            {
                pageHeaders = value;
            }
        }

        public AttachRows PageFoots
        {
            get
            {
                return pageFoots;
            }
            set
            {
                pageFoots = value;
            }
        }

        public Cols Cols
        {
            get
            {
                return cols;
            }
            set
            {
                cols = value;
            }
        }

        public Rows Rows
        {
            get
            {
                return rows;
            }
            set
            {
                rows = value;
            }
        }

        public Body Body
        {
            get
            {
                return body;
            }
            set
            {
                body = value;
            }
        }

        public AttachRows Headers
        {
            get
            {
                return headers;
            }
            set
            {
                headers = value;
            }
        }

        public AttachRows Tails
        {
            get
            {
                return tails;
            }
            set
            {
                tails = value;
            }
        }

        public AttachRows RightAttachText
        {
            get
            {
                return rightAttachText;
            }
            set
            {
                rightAttachText = value;
            }
        }

        public AttachRows LeftAttachText
        {
            get
            {
                return leftAttachText;
            }
            set
            {
                leftAttachText = value;
            }
        }

        public Pictures Pictures
        {
            get
            {
                return pictures;
            }
            set
            {
                pictures = value;
            }
        }

        public Ranges Mergers
        {
            get
            {
                return mergers;
            }
            set
            {
                mergers = value;
            }
        }

        public int TopFixedRows
        {
            get
            {
                return topFixedRows;
            }
            set
            {
                topFixedRows = value;
                ranges.Remove("TopFixedRows");
                Range r = new Range(1, 1, topFixedRows, cols.Count); ;
                ranges.Add("TopFixedRows", r);
            }
        }

        public int LeftFixedCols
        {
            get
            {
                return leftFixedCols;
            }
            set
            {
                leftFixedCols = value;
                ranges.Remove("LeftFixedCols");
                Range r = new Range(1, 1, rows.Count, leftFixedCols); ;
                ranges.Add("LeftFixedCols", r);
            }
        }

        public int RightFixedCols
        {
            get
            {
                return rightFixedCols;
            }
            set
            {
                rightFixedCols = value;
                ranges.Remove("RightFixedCols");
                Range r = new Range(1, cols.Count - rightFixedCols + 1, rows.Count, cols.Count); ;
                ranges.Add("RightFixedCols", r);
            }
        }

        public int BottomFixedRows
        {
            get
            {
                return bottomFixRows;
            }
            set
            {
                bottomFixRows = value;
                ranges.Remove("BottomFixedRows");
                Range r = new Range(rows.Count - bottomFixRows + 1, 1, rows.Count, cols.Count); ;
                ranges.Add("BottomFixedRows", r);
            }
        }

        internal Ranges FixRanges
        {
            get
            {
                return ranges;
            }
        }

        internal bool InFixedRanges(int row, int col)
        {
            Range o;
            bool b = false;

            if (row > 0 && col > 0)
            {
                for (int i = 1; i <= FixRanges.Count; i++)
                {
                    o = FixRanges.GetItem(i);
                    if (row >= o.StartRow && row <= o.EndRow && col >= o.StartCol && col <= o.EndCol)
                        return true;
                }
            }
            else if (row > 0)
            {
                o = FixRanges.GetItem("TopFixedRows");
                if (o != null)
                {
                    if (row >= o.StartRow && row <= o.EndRow)
                        return true;
                }
                o = FixRanges.GetItem("BottomFixedRows");
                if (o != null)
                {
                    if (row >= o.StartRow && row <= o.EndRow)
                        return true;
                }
            }
            else if (col > 0)
            {
                o = FixRanges.GetItem("LeftFixedCols");
                if (o != null)
                {
                    if (col >= o.StartCol && col <= o.EndCol)
                        return true;
                }
                o = FixRanges.GetItem("RightFixedCols");
                if (o != null)
                {
                    if (col >= o.StartCol && col <= o.EndCol)
                        return true;
                }
            }
            return b;
        }

        internal bool InMergedRanges(int Row, int Col, out PrinterTextAlign Pos)
        {
            Range o;
            return InMergedRanges(Row, Col, out  Pos, out o);
        }

        internal bool InMergedRanges(int Row, int Col, out PrinterTextAlign Pos, out  Range o)
        {
            bool InMergedRanges = false;

            for (int i = 1; i <= mergers.Count; i++)
            {
                o = mergers.GetItem(i);
                if (Row > 0 && Col > 0)
                {
                    if (Row >= o.StartRow && Row <= o.EndRow && Col >= o.StartCol && Col <= o.EndCol)
                        InMergedRanges = true;
                    if (InMergedRanges)
                    {
                        if (Row == o.StartRow && Col == o.StartCol)
                            Pos = PrinterTextAlign.LeftTop;
                        else if (Row == o.StartRow && Col == o.EndCol)
                            Pos = PrinterTextAlign.RightTop;
                        else if (Row == o.EndRow && Col == o.StartCol)
                            Pos = PrinterTextAlign.LeftBottom;
                        else if (Row == o.EndRow && Col == o.EndCol)
                            Pos = PrinterTextAlign.RightBottom;
                        else if (Row == o.StartRow && Col < o.EndCol && Col > o.StartCol)
                            Pos = PrinterTextAlign.CenterTop;
                        else if (Row == o.EndRow && Col < o.EndCol && Col > o.StartCol)
                            Pos = PrinterTextAlign.CenterBottom;
                        else if (Col == o.StartCol && Row < o.EndRow && Row > o.StartRow)
                            Pos = PrinterTextAlign.LeftMiddle;
                        else if (Col == o.EndCol && Row < o.EndRow && Row > o.StartRow)
                            Pos = PrinterTextAlign.RightMiddle;
                        else
                            Pos = PrinterTextAlign.CenterMiddle;
                        return InMergedRanges;
                    }
                }
            }
            o = default(Range);
            Pos = (PrinterTextAlign)(-1);
            return InMergedRanges;
        }
    }

    public class PrintDatas:ListDictionaryEx<PrintData> 
    {
        internal int phyPageCount = 0;
        internal int logicPageCount = 0;

        public int PhyPageCount
        {
            get { return phyPageCount; }
        }

        public int LogicPageCount
        {
            get { return logicPageCount; }
        }

        public PrintData Create(string key)
        {
            PrintData p = new PrintData();
            if (!ContainsKey(key))
            {
                Add(key,p);
                return GetItem(key);
            }
            else
                return default(PrintData);
        }
    }

    internal class PageArrange
    {
        private PrinterArrange[] aryArrange = new PrinterArrange[1];
        private int hCount;
        private int vCount;
        /// <summary>
        /// °üº¬±ß¾à
        /// </summary>
        public PageArrange(int HCount, int VCount, float pageWidth, float pageHeight, Point p)
        {
            int k = 1; float h; float w;
            hCount = HCount; vCount = VCount;
            System.Array.Resize<PrinterArrange>(ref aryArrange, hCount * vCount + 1);
            h = p.Y;
            for (int i = 1; i <= vCount; i++)
            {
                w =p.X;
                for (int j = 1; j <= hCount; j++)
                {
                    aryArrange[k].Size.Height = (int)pageHeight;
                    aryArrange[k].Size.Width = (int)pageWidth;
                    //if (j == 1)
                    //    aryArrange[k].Location.X = (int);
                    //else
                        aryArrange[k].Location.X = (int)w;
                    //if (i == 1)
                    //    aryArrange[k].Location.Y = (int);
                    //else
                        aryArrange[k].Location.Y = (int)h;
                    w = w + pageWidth;
                    k++;
                }
                h = h + pageHeight;
            }
        }

        public int Count_H
        {
            get { return hCount; }
        }

        public int Count_V
        {
            get { return vCount; }
        }

        public int Count
        {
            get { return aryArrange.GetUpperBound(0); }
        }

        public PrinterArrange Arrange(int index_H, int index_V)
        {
            int i;

            i = index_H * index_V;
            if (i > aryArrange.GetUpperBound(0) || i <= 0)
                return default(PrinterArrange);
            else
                return aryArrange[i];
        }

        public PrinterArrange Arrange(int index)
        {
            if (index > aryArrange.GetUpperBound(0) || index <= 0)
                return default(PrinterArrange);
            else
                return aryArrange[index];
        }

    }
}
