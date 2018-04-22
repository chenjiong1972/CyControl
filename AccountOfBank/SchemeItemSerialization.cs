using System;
using System.Collections.Generic;

using System.Text;
using System.Drawing;
using System.Xml.Serialization; 
using UnvaryingSagacity.Core ;
using UnvaryingSagacity.SuitSchemePrinter;
namespace UnvaryingSagacity.AccountOfBank
{
    
    public class SchemeItemSerialization :SchemeItem 
    {
        public SchemeItemSerialization() 
        {
            
        }
        public string FontName { get { return Font.Name; } set { Font = new Font(value, FontSize,FontStyle.Regular); } }

        public float FontSize { get { return Font.Size; } set { Font = new Font(FontName,value  ,FontStyle); } }

        public FontStyle FontStyle { get { return Font.Style; } set { Font = new Font(FontName, FontSize, value); } }

        public new int ForeColor { get { return base.ForeColor.ToArgb(); } set { base.ForeColor = Color.FromArgb(value); } }

        public new SchemeItemStyleSerialization Style 
        {
            get 
            {
                SchemeItemStyleSerialization style = new SchemeItemStyleSerialization();
                base.Style.Clone(style);
                return style;
            }
            set { base.Style = value; }
        }
    }

    public class SchemeItemStyleSerialization : SchemeItemStyle
    {
        public SchemeItemStyleSerialization() { }

        public new byte[] CurrencyFlag
        {
            get
            {
                if (base.CurrencyFlag == null)
                    return null;
                return ImageHandler.FromImage(base.CurrencyFlag);
            }
            set
            {
                base.CurrencyFlag = Core.ImageHandler.FromByteArray(value);
            }
        }

        public new byte[] FalseToImage
        {
            get
            {
                if (base.CurrencyFlag == null)
                    return null;
                return ImageHandler.FromImage(base.FalseToImage); 
            }
            set
            {
                base.FalseToImage = Core.ImageHandler.FromByteArray(value);
            }
        }

        public new byte[] FillCharToImage
        {
            get
            {
                if (base.CurrencyFlag == null)
                    return null;
                return ImageHandler.FromImage(base.FillCharToImage);
            }
            set
            {
                base.FillCharToImage = Core.ImageHandler.FromByteArray(value);
            }
        }
    }
}
