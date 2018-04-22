using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;
using System.Drawing;
using UnvaryingSagacity.Core;
using UnvaryingSagacity.SuitSchemePrinter;

namespace UnvaryingSagacity.AccountOfBank
{
    #region 要序列化的列不能实现IDictionary,这里使用数组

    public class SchemeSerialization : SuitScheme 
    {
        private SchemeItemSerialization[] suitItems;

        public SchemeSerialization() { suitItems = new SchemeItemSerialization[0];}

        /// <summary>
        /// ItemArgs与base.Items 的内容不同步,使用时需要自行复制.
        /// ItemArg是在序列化和反序列化时使用.
        /// </summary>
        [XmlArrayItem("SuitItem", typeof(SchemeItemSerialization))]
        public SchemeItemSerialization [] ItemArgs
        {
            get { return suitItems; }
            set { suitItems = value; }
        }

        public void SyncItemsFromBase()
        {
            Array.Resize<SchemeItemSerialization>(ref  suitItems, base.Items.Count);
            int i=0;
            foreach (SchemeItem item in base.Items)
            {
                suitItems[i] = new SchemeItemSerialization();
                item.Clone (suitItems[i]);
                i++;
            }
        }

        public void SyncItemsToBase()
        {
            base.Items.Clear();
            for( int i = 0;i<suitItems.Length ;i++)
            {
                SchemeItemSerialization src=suitItems[i];
                SchemeItem item = new SchemeItem();
                src.Clone(item);
                base.Items.Add(item.Name , item);
            }
        }

        public static bool Copy(SuitScheme dst, SuitScheme src)
        {
            if (src == null || dst == null)
                return false;
            dst.BackgroundImage = src.BackgroundImage;
            dst.Size = src.Size;
            dst.Items.Clear();
            foreach (SchemeItem item in src.Items)
            {
                SchemeItem dstItem = new SchemeItem();
                item.Clone(dstItem);
                dst.Items.Add(dstItem.Name, dstItem);
            }
            return true;
        }

    }
    #endregion

}
