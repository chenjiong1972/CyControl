using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace UnvaryingSagacity.Core.Printer
{
    public struct PrinterCell
    {
        public PrinterTextAlign Align;
        public int B_Color;
        public int L_Color;
        public int R_Color;
        public int T_Color;
        public int B_PenWidth;
        public int L_PenWidth;
        public int R_PenWidth;
        public int T_PenWidth;
        public PrinterBorderStyle  B_Style;
        public PrinterBorderStyle L_Style;
        public PrinterBorderStyle R_Style;
        public PrinterBorderStyle T_Style;
        public int FontColor;
        public Font font;
        public int PatternBG;
        public int PatternFG;
        public int PatternStyle;
        public object Value;
        public bool WordWrap;
        public string PictureFile;
        public PrinterCellBehave Behave;

        public void AddTextEx(int color, Font ft, string text)
        {
            System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
            if (Value == null)
            {
                xml.LoadXml("<root></root>");
            }
            else
                xml.LoadXml(Value.ToString());
            System.Xml.XmlNode root = xml.FirstChild ;
            if (root == null)
            {
                xml.LoadXml("<root></root>");
                root = xml.FirstChild;
            }
            System.Xml.XmlNode node = xml.CreateElement("text");
            if (color != -1)
            {
                System.Xml.XmlAttribute xmlAttr = xml.CreateAttribute("color");
                xmlAttr.Value = color.ToString();
                node.Attributes.Append(xmlAttr);
            }
            if (ft != null)
            {
                System.Xml.XmlAttribute xmlAttr = xml.CreateAttribute("font");
                xmlAttr.Value = Core.General.FontToString(ft);
                node.Attributes.Append(xmlAttr);
            }
            node.InnerText = text;
            root.AppendChild(node);
            Value = xml.InnerXml;
        }

        public void AddTextEx(int color, string text)
        {
            AddTextEx(color, null, text);
        }

        public void AddTextEx(string text)
        {
            AddTextEx(-1, text);
        }
    }

    public struct PrinterAttachCell
    {
        public byte InRowPercent;
        internal int Height;
        public PrinterCell Properties;
    }

    public struct PrinterDefineRowsInPage
    {
        public int PageHeaderRows;
        public int PageFootRows;
        public int MainTitleRows;
        public int SubTitleRows;
        public int HeaderRows;
        public int TailRows;
        public int BodyLinespace;
        public int MinRows;
        public int TopFixRows;
        public int BottomFixRows;
    }

    internal struct PrinterBound
    {
        public int Start;
        public int End;
    }

    internal struct PrinterArrange
    {
        public Point Location;
        public Size Size;
    }

    internal struct typCellEx
    {
        public int color;
        public Font font;
        public string text;
    }

    public struct CboItem
    {
        private int itemData;
        private string text;
        private string key;

        public CboItem(string Text, string Key, int ItemData)
        {
            text = Text;
            key = Key;
            itemData = ItemData;
        }

        public CboItem(string Text, int ItemData)
        {
            text = Text;
            key = "aaa";
            itemData = ItemData;
        }

        public CboItem(string Text, string Key)
        {
            text = Text;
            key = Key;
            itemData = 0;
        }

        public int ItemData
        {
            get { return itemData; }
            set { itemData = value; }
        }

        public String Key
        {
            get { return key; }
            set { key = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }
    
        public override string ToString()
        {
            return text;
        }
    }
}
