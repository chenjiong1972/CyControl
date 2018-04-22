using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UnvaryingSagacity.Core;
using UnvaryingSagacity.SuitSchemePrinter;

namespace UnvaryingSagacity.AccountOfBank
{
    partial class UIDesigner : Form
    {

        internal Environment CurrentEnvironment { get; set; }
        internal ItemOfBank CurrentItemOfBank { get; set; }
        private string [] _buildIn = new string[] { "账套名称", "账本名称", "账户名称", "账户隶属银行名称", "账户的银行账号", "当前用户名" };
        public UIDesigner()
        {
            InitializeComponent();
            this.KeyUp += new KeyEventHandler(axSuitSchemeView1_KeyUp);
            this.axSuitSchemeView1.SchemeItemMouseClick += new MouseEventHandler(axSuitSchemeView1_SchemeItemMouseClick);
            this.axSuitSchemeView1.SelectIndexChanged += new EventHandler(axSuitSchemeView1_SelectIndexChanged);
            this.Shown += new EventHandler(UIDesigner_Shown);
            #region button.Click
            foreach (ToolStripItem button in toolStrip1.Items)
            {
                if (button.GetType() == typeof(ToolStripButton))
                {
                    ((ToolStripButton)button).Click += new EventHandler(button_Click);
                }
                else if (button.GetType() == typeof(ToolStripMenuItem))
                {
                    ((ToolStripMenuItem)button).Click += new EventHandler(button_Click);
                }
                else if (button.GetType() == typeof(ToolStripSplitButton))
                {
                    ((ToolStripSplitButton)button).ButtonClick += new EventHandler(button_Click);
                    foreach (ToolStripItem b in ((ToolStripSplitButton)button).DropDownItems)
                    {
                        b.Click += new EventHandler(button_Click);
                    }
                }
            }
            foreach (ToolStripItem button in toolStrip2.Items)
            {
                if (button.GetType() == typeof(ToolStripButton))
                {
                    ((ToolStripButton)button).Click += new EventHandler(button_Click);
                }
                else if (button.GetType() == typeof(ToolStripMenuItem))
                {
                    ((ToolStripMenuItem)button).Click += new EventHandler(button_Click);
                }
                else if (button.GetType() == typeof(ToolStripSplitButton))
                {
                    ((ToolStripSplitButton)button).ButtonClick += new EventHandler(button_Click);
                }
            }
            #endregion

        }

        void UIDesigner_Shown(object sender, EventArgs e)
        {
            CurrentScheme = new SchemeSerialization();
            if (!CurrentEnvironment.LoadScheme(CurrentScheme, CurrentEnvironment.AccountCoverFilename))
            {
                CurrentEnvironment.CreateScheme(CurrentScheme);
            }
            InitSuitItems();
        }

        SchemeSerialization CurrentScheme { get; set; }

        void axSuitSchemeView1_SelectIndexChanged(object sender, EventArgs e)
        {
            bool b = axSuitSchemeView1.SelectedAxSchemeItems.Count > 1 ? true : false;
            toolStripButton25.Enabled = b;
            toolStripButton26.Enabled = b;
            toolStripButton27.Enabled = b;
            toolStripButton28.Enabled = b;
            toolStripButton29.Enabled = b;
            toolStripButton30.Enabled = b;
        }

        void axSuitSchemeView1_SchemeItemMouseClick(object sender, MouseEventArgs e)
        {
            if (toolStripButton6.Tag is SchemeItem)
            {
                SchemeItem item = sender as SchemeItem;
                SchemeItem src = toolStripButton6.Tag as SchemeItem;
                item.ItemType = src.ItemType;
                src.Style.Clone(item.Style);
                axSuitSchemeView1.RedrawItems(new string[1] { item.Name }, false);
            }
        }

        void axSuitSchemeView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (axSuitSchemeView1.View == ViewMode.定义设计)
            {
                if (e.Control && e.KeyCode == Keys.A)
                    axSuitSchemeView1.SelectAll();
            }
        }

        void InitSuitItems()
        {
            this.axSuitSchemeView1.SchemeItemParent.Size = CurrentScheme.Size;
            this.axSuitSchemeView1.Clear();
            foreach (SchemeItem item in CurrentScheme.Items)
            {
                if (item != null)
                {
                    this.axSuitSchemeView1.Add(item);
                    item.CustomAttribute += new SchemeItem.BeforSetPropertyDescriptorEventHandle(item_CustomAttribute);
                }
            }
            this.axSuitSchemeView1.RedrawSchemeBackgroundImage();
        }

        void item_CustomAttribute(object sender, BeforSetPropertyDescriptorEventArgs e)
        {
            //使名称只读
            e.Attributes[0].ReadOnly = true;
        }

        void CreateItem()
        {
            UIAppendSuitItemInDesigner ui = new UIAppendSuitItemInDesigner();
            ui.BulidInItems = _buildIn;
            string[] exists = new string[0];
            foreach (SuitSchemePrinter.SchemeItem item in axSuitSchemeView1.SchemeItemParent.Items)
            {
                Array.Resize<string>(ref exists, exists.Length + 1);
                exists[exists.Length - 1] = item.Name; 
            }
            ui.ExistsItems = exists;
            if (ui.ShowDialog(this) == DialogResult.OK)
            {
                string result = ui.ResultItems;
                Core.XmlExplorer xml = new XmlExplorer();
                xml.XmlDoc.LoadXml(result);
                System.Xml.XmlNodeList nodes = xml.XmlDoc.GetElementsByTagName("item");
                foreach (System.Xml.XmlNode node in nodes)
                {
                    SchemeItem newItem = new SchemeItem();
                    newItem.Name = axSuitSchemeView1.GetUniqueName(node.Attributes["name"].Value);
                    if (node.InnerText.Length > 0)
                        newItem.Expression =(char)34+ node.InnerText+(char )34;
                    axSuitSchemeView1.Add(newItem);
                    axSuitSchemeView1.AxSchemeItemBringToFront(newItem.Name, true);
                }
            }
        }

        void button_Click(object sender, EventArgs e)
        {
            switch (((ToolStripItem)sender).Text)
            {
                case "关闭":
                    this.Close();
                    break;
                case "打开":
                    //OpenExist();
                    break;
                case "保存":
                    Core.ToolTip tip =new UnvaryingSagacity.Core.ToolTip ();
                    tip.ShowPrompt( this,"正在保存票证设计......");
                    this.Cursor = Cursors.WaitCursor;
                    BeforeSave();

                    Core.XmlSerializer<SchemeSerialization>.ToXmlSerializer(CurrentEnvironment.AccountCoverFilename  , CurrentScheme );
                    tip.Hide();
                    this.Cursor = Cursors.Default;
                    break;
                case "更换底图":
                    //Image image = _environment.ShowImageCollectionDialog(this, this._environment.MainUIClientBounds);
                    //if (image != null)
                    //{
                    //    CurrentScheme.BackgroundImage = image;
                    //    axSuitSchemeView1.SchemeItemParent.BackgroundImage = image;
                    //    axSuitSchemeView1.RedrawSchemeBackgroundImage();
                    //}
                    break;
                case "新增":
                    CreateItem();
                    break;
                case "手绘模式":
                    if (((ToolStripButton)sender).Checked)
                    {
                        axSuitSchemeView1.HandleDrawMode = true;
                        axSuitSchemeView1.Cursor = Cursors.Cross;
                        toolStrip2.Enabled = false;
                    }
                    else
                    {
                        axSuitSchemeView1.HandleDrawMode = false;
                        axSuitSchemeView1.Cursor = Cursors.Default;
                        toolStrip2.Enabled = true;
                    }
                    break;
                case "复制":
                    SuitSchemeItemCollection items = new SuitSchemeItemCollection();
                    axSuitSchemeView1.SelectedItems(items);
                    if (items.Count > 1)
                    {
                        SchemeSerialization scheme = new SchemeSerialization();
                        foreach (SchemeItem item in items)
                        {
                            SchemeItemSerialization dst = new SchemeItemSerialization();
                            item.Clone(dst);
                            scheme.Items.Add(dst);
                        }
                        scheme.SyncItemsFromBase();
                        System.IO.MemoryStream m = new System.IO.MemoryStream();
                        Core.XmlSerializer<SchemeSerialization>.ToXmlSerializer(m, scheme);
                        Core.XmlSerializer<SchemeSerialization>.ToXmlSerializer(@"C:\aaa.txt", scheme);

                        byte[] readByte = m.ToArray();
                        string readString = Encoding.UTF8.GetString(readByte, 0, readByte.Length);
                        Clipboard.SetText("SchemeItems:" + readString, TextDataFormat.UnicodeText);

                    }
                    else
                    {
                        if (axSuitSchemeView1.ActivedSchemeItem != null)
                        {
                            SchemeItemSerialization dst = new SchemeItemSerialization();
                            axSuitSchemeView1.ActivedSchemeItem.Clone(dst);
                            System.IO.MemoryStream m = new System.IO.MemoryStream();
                            Core.XmlSerializer<SchemeItemSerialization>.ToXmlSerializer(m, dst);
                            //Core.XmlSerializer<SchemeItemSerialization>.ToXmlSerializer(@"C:\aaa.txt", dst);

                            byte[] readByte = m.ToArray();
                            string readString = Encoding.UTF8.GetString(readByte, 0, readByte.Length);
                            Clipboard.SetText("SchemeItem:" + readString, TextDataFormat.UnicodeText);
                        }
                    }
                    break;
                case "粘贴":
                    string s = Clipboard.GetText(TextDataFormat.UnicodeText);
                    if (s.IndexOf("SchemeItem:") == 0)
                    {
                        s = s.Substring("SchemeItem:".Length);
                        byte[] b = Encoding.Unicode.GetBytes(s);
                        System.IO.MemoryStream m = new System.IO.MemoryStream(b, 0, b.Length);
                        SchemeItemSerialization o = new SchemeItemSerialization();
                        if (Core.XmlSerializer<SchemeItemSerialization>.FromXmlSerializer(m, out o))
                        {
                            SchemeItem item = new SchemeItem();
                            o.Copy(item);
                            item.Name = axSuitSchemeView1.GetUniqueName(o.Name);
                            axSuitSchemeView1.Add(item);
                            axSuitSchemeView1.AxSchemeItemBringToFront(item.Name, true);
                        }
                    }
                    else if (s.IndexOf("SchemeItems:") == 0)
                    {
                        s = s.Substring("SchemeItems:".Length);
                        byte[] b = Encoding.Unicode.GetBytes(s);
                        System.IO.MemoryStream m = new System.IO.MemoryStream(b, 0, b.Length);
                        SchemeSerialization dstItems = new SchemeSerialization();
                        if (Core.XmlSerializer<SchemeSerialization>.FromXmlSerializer(m, out dstItems))
                        {
                            dstItems.SyncItemsToBase();
                            foreach (SchemeItem o in dstItems.Items)
                            {
                                SchemeItem item = new SchemeItem();
                                o.Copy(item);
                                item.Name = axSuitSchemeView1.GetUniqueName(o.Name);
                                axSuitSchemeView1.Add(item);
                                axSuitSchemeView1.AxSchemeItemBringToFront(item.Name, true);
                            }
                        }
                    }
                    break;
                case "删除":
                    if (MessageBox.Show(this, "您确定要删除选中的打印项吗?", "删除打印项", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        Delete();
                    break;
                case "锁定":
                    Locked();
                    break;
                case "解锁":
                    UnLocked();
                    break;
                case "打印":
                    break;
                case "打印设置":
                    BeforeSave();
                    CurrentEnvironment.ShowPrintSettingsDialog(this, CurrentScheme);
                    break;
                case "有票样图的预览":
                    break;
                case "预览":
                    BeforeSave();
                    SetVisualValue(CurrentScheme);
                    CurrentEnvironment.PrintScheme(this, CurrentScheme, true, false, false);
                    break;
                case "移动...":
                    break;
                case "格式刷":
                    axSuitSchemeView1.Cursor = Cursors.Default;
                    if (this.toolStripButton6.Checked)
                    {
                        this.toolStripButton6.Tag = null;
                        this.toolStripButton6.Checked = false;
                    }
                    else
                    {
                        if (axSuitSchemeView1.ActivedSchemeItem != null)
                        {
                            SchemeItem formatItem = new SchemeItem();
                            axSuitSchemeView1.ActivedSchemeItem.Clone(formatItem);
                            this.toolStripButton6.Tag = formatItem;
                            this.toolStripButton6.Checked = true;
                            axSuitSchemeView1.Cursor = Cursors.Hand;
                        }
                    }
                    break;
                case "使大小相同":
                    SameSize(0);
                    break;
                case "使高度相同":
                    SameSize(2);
                    break;
                case "使宽度相同":
                    SameSize(1);
                    break;
                case "底对齐":
                    SameLocation(5);
                    break;
                case "垂直中间对齐":
                    SameLocation(6);
                    break;
                case "顶对齐":
                    SameLocation(4);
                    break;
                case "右对齐":
                    SameLocation(2);
                    break;
                case "水平中间对齐":
                    SameLocation(3);
                    break;
                case "左对齐":
                    SameLocation(1);
                    break;
                case "全选":
                    axSuitSchemeView1.SelectAll();
                    break;
                case "使用底图尺寸":
                    if (CurrentScheme.BackgroundImage != null)
                    {
                        float w = (CurrentScheme.BackgroundImage.Width / (AxSuitSchemeView.MMTOINCH * CurrentScheme.DpiX));
                        float h = (CurrentScheme.BackgroundImage.Height / (AxSuitSchemeView.MMTOINCH * CurrentScheme.DpiY));
                        Size imageSize = new Size((int)Math.Round(w, 0), (int)(Math.Round(h, 0)));
                        CurrentScheme.Size = imageSize;
                        axSuitSchemeView1.SchemeItemParent.Size = CurrentScheme.Size;
                        axSuitSchemeView1.RedrawSchemeBackgroundImage();
                    }
                    break;
                case "封面尺寸":
                    Core.InputBox input = new InputBox("输入封面尺寸", "请准确输入封面的宽和高并用逗号(或非数字字符)分隔\n注意单位是毫米", 7, CurrentScheme.Size.Width + "," + CurrentScheme.Size.Height);
                    if (input.ShowDialog(this) == DialogResult.OK)
                    {
                        string sz = input.Result.ToString();
                        if (sz.Length < 0)
                            return;
                        if (sz == CurrentScheme.Size.Width + "," + CurrentScheme.Size.Height)
                            return;
                        int w = 0;
                        int h = 0;
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < sz.Length; i++)
                        {
                            string c = sz.Substring(i, 1);
                            if (Core.General.IsNumberic(c, false, false))
                                sb.Append(c);
                            else
                            {
                                if (sb.Length > 0 && w == 0)
                                {
                                    w = int.Parse(sb.ToString());
                                    sb.Remove(0, sb.Length);
                                }
                            }
                        }
                        if (sb.Length > 0 && h == 0)
                        {
                            h = int.Parse(sb.ToString());
                        }
                        if (w > 0 && h > 0)
                        {
                            if (MessageBox.Show(this, "您确定新的票证尺寸为: " + w + "," + h + " 毫米吗？", "票证尺寸", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                            {
                                CurrentScheme.Size = new Size(w, h);
                                axSuitSchemeView1.SchemeItemParent.Size = new Size(w, h);
                                axSuitSchemeView1.RedrawSchemeBackgroundImage();
                            }
                        }
                    }
                    break;
                case "清除水平间距":
                    ChangSpaceH(0);
                    break;
                case "增加水平间距":
                    ChangSpaceH(1);
                    break;
                case "减少水平间距":
                    ChangSpaceH(-1);
                    break;
                case "清除垂直间距":
                    ChangSpaceV(0);
                    break;
                case "增加垂直间距":
                    ChangSpaceV(1);
                    break;
                case "减少垂直间距":
                    ChangSpaceV(-1);
                    break;
                default:
                    break;
            }
        }

        void SetVisualValue(SchemeSerialization items)
        {
            foreach (SchemeItem item in items.Items)
            {
                switch (item.ItemType)
                {
                    case SchemeItemType.金额或数量:
                        item.Value = 1234567.89;
                        break;
                    case SchemeItemType.年:
                        item.Value = DateTime.Now.Year;
                        break;
                    case SchemeItemType.日:
                        item.Value = DateTime.Now.Day;
                        break;
                    case SchemeItemType.月:
                        item.Value = DateTime.Now.Month;
                        break;
                    case SchemeItemType.日期:
                        item.Value = DateTime.Today.ToShortDateString();
                        break;
                    case SchemeItemType.时间:
                        item.Value = DateTime.Now.ToString("H:mm");// "20:59";
                        break;
                    case SchemeItemType.是否:
                        item.Value = "是";
                        break;
                    case SchemeItemType.数字:
                        item.Value = "123456789";
                        break;
                    default:
                        switch (item.Name)
                        {
                            case "账套名称":
                                if (CurrentEnvironment.CurrentAccount == null)
                                {
                                    item.Value = "当前没有打开的账套";
                                }
                                else
                                {
                                    item.Value = CurrentEnvironment.CurrentAccount.Name;
                                }
                                break;
                            case "账本名称":
                                item.Value = item.Expression;
                                break;
                            case "账户名称":
                                if (CurrentItemOfBank == null)
                                {
                                    item.Value = "当前没有打开的账户,不能显示账户名称";
                                }
                                else
                                {
                                    item.Value = CurrentItemOfBank.Name;
                                }
                                break;
                            case "账户隶属银行名称":
                                if (CurrentItemOfBank == null)
                                {
                                    item.Value = "当前没有打开的账户,不能显示银行名称";
                                }
                                else
                                {
                                    item.Value = CurrentItemOfBank.OfBankName ;
                                }
                                break;
                            case "账户的银行账号":
                                if (CurrentItemOfBank == null)
                                {
                                    item.Value = "当前没有打开的账户,不能显示账号";
                                }
                                else
                                {
                                    item.Value = CurrentItemOfBank.ID ;
                                }
                                break;
                            case "当前用户名":
                                item.Value = CurrentEnvironment.CurrentUser.Name;  
                                break;
                            default :
                                item.Value = item.Expression;
                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 改变水平间距
        /// </summary>
        /// <param name="increment">变动增量,负数为减少,0为清除</param>
        private void ChangSpaceH(int increment)
        {
            if (axSuitSchemeView1.ActivedSchemeItem == null || axSuitSchemeView1.SelectedAxSchemeItems.Count <= 0)
                return;
            AxSchemeItem[] items = GetSortArrayBySelected(1);
            int index = 0;
            //取得参照项的位置和索引
            for (int i = 0; i < items.Length; i++)
            {
                AxSchemeItem item = items[i];
                if (item.Name == axSuitSchemeView1.ActiveControl.Name)
                {
                    index = i;
                    break;
                }
            }
            //调整,以参照项为准,在参照项左边的
            for (int i = index - 1; i >= 0; i--)
            {
                AxSchemeItem item = items[i];
                if (increment != 0)
                    item.Location = new Point(item.Location.X + increment * (-1) * (index - i), item.Location.Y);
                else
                {
                    Point pt = item.Location;
                    pt.Offset(-(item.Right - items[i + 1].Left) + 2 * AxSchemeItemBase.MARGIN, 0);
                    item.Location = pt;
                }
            }
            //参照项右边的
            for (int i = index + 1; i < items.Length; i++)
            {
                AxSchemeItem item = items[i];
                if (increment != 0)
                    item.Location = new Point(item.Location.X + increment * (i - index), item.Location.Y);
                else
                {
                    Point pt = item.Location;
                    pt.Offset(items[i - 1].Right - item.Left - 2 * AxSchemeItemBase.MARGIN, 0);
                    item.Location = pt;
                }
            }
        }

        private AxSchemeItem[] GetSortArrayBySelected(int sortOrder)
        {
            int _sortOrder = sortOrder;
            string[] _axItemXY = new string[axSuitSchemeView1.SelectedAxSchemeItems.Count];
            AxSchemeItem[] _axitemSort = new AxSchemeItem[axSuitSchemeView1.SelectedAxSchemeItems.Count];
            int i = 0;
            foreach (AxSchemeItem item in axSuitSchemeView1.SelectedAxSchemeItems)
            {
                Array.Resize<string>(ref _axItemXY, i + 1);
                Array.Resize<AxSchemeItem>(ref _axitemSort, i + 1);
                if (sortOrder == 1)
                    _axItemXY[i] = item.Location.X.ToString("00000");
                else if (sortOrder == 2)
                    _axItemXY[i] = item.Location.Y.ToString("00000");
                _axitemSort[i] = item;
                i++;
            }
            Array.Sort(_axItemXY, _axitemSort);
            return _axitemSort;
        }

        /// <summary>
        /// 改变垂直间距
        /// </summary>
        /// <param name="increment">变动增量,负数为减少,0为清除</param>
        private void ChangSpaceV(int increment)
        {
            AxSchemeItem[] items = GetSortArrayBySelected(2);
            int index = 0;
            //取得参照项的位置和索引
            for (int i = 0; i < items.Length; i++)
            {
                AxSchemeItem item = items[i];
                if (item.Name == axSuitSchemeView1.ActiveControl.Name)
                {
                    index = i;
                    break;
                }
            }
            //调整,以参照项为准,在参照项上边的
            for (int i = index - 1; i >= 0; i--)
            {
                AxSchemeItem item = items[i];
                if (increment != 0)
                    item.Location = new Point(item.Location.X, item.Location.Y + increment * (-1) * (index - i));
                else
                {
                    Point pt = item.Location;
                    pt.Offset(0, -(item.Bottom - items[i + 1].Top) + 2 * AxSchemeItemBase.MARGIN);
                    item.Location = pt;
                }
            }
            //参照项下边的
            for (int i = index + 1; i < items.Length; i++)
            {
                AxSchemeItem item = items[i];
                if (increment != 0)
                    item.Location = new Point(item.Location.X, item.Location.Y + increment * (i - index));
                else
                {
                    Point pt = item.Location;
                    pt.Offset(0, items[i - 1].Bottom - item.Top - 2 * AxSchemeItemBase.MARGIN);
                    item.Location = pt;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag">1=width;2=height;0=all</param>
        private void SameSize(int flag)
        {
            if (axSuitSchemeView1.ActivedSchemeItem == null || axSuitSchemeView1.SelectedAxSchemeItems.Count <= 0)
                return;
            Size size = axSuitSchemeView1.ActiveControl.Size;
            AxSchemeItemCollection items = axSuitSchemeView1.SelectedAxSchemeItems;

            foreach (AxSchemeItem item in items)
            {
                if (item.Name != axSuitSchemeView1.ActiveControl.Name && !item.Locked)
                {
                    if (flag == 1)
                    {
                        item.Size = new Size(size.Width, item.Size.Height);
                    }
                    else if (flag == 2)
                    {
                        item.Size = new Size(item.Size.Width, size.Height);
                    }
                    else
                        item.Size = size;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag">1=left;2=right;3=center;4=top;5=bottom;6=middle</param>
        private void SameLocation(int flag)
        {
            if (axSuitSchemeView1.ActivedSchemeItem == null || axSuitSchemeView1.SelectedAxSchemeItems.Count <= 0)
                return;
            Point loca = axSuitSchemeView1.ActiveControl.Location;
            Size size = axSuitSchemeView1.ActiveControl.Size;
            AxSchemeItemCollection items = axSuitSchemeView1.SelectedAxSchemeItems;

            foreach (AxSchemeItem item in items)
            {
                if (item.Name != axSuitSchemeView1.ActiveControl.Name && !item.Locked)
                {
                    if (flag == 1)
                    {
                        item.Location = new Point(loca.X, item.Location.Y);
                    }
                    else if (flag == 2)
                    {
                        item.Location = new Point((loca.X + size.Width) - item.Size.Width, item.Location.Y);
                    }
                    //else if (flag == 3)
                    //{
                    //    item.Location = new Point(item.Location.Width, loca.Height);
                    //}
                    else if (flag == 4)
                    {
                        item.Location = new Point(item.Location.X, loca.Y);
                    }
                    else if (flag == 5)
                    {
                        item.Location = new Point(item.Location.X, (loca.Y + size.Height) - item.Size.Height);
                    }
                    //else if (flag == 6)
                    //    item.Location  = new  Point();
                    else
                        return;
                }
            }
        }

         private void BeforeSave()
        {
            CurrentScheme.Items.Clear();
            foreach (SchemeItem item in axSuitSchemeView1.SchemeItemParent.Items)
            {
                SchemeItem dst = new SchemeItemSerialization();
                item.Clone(dst);
                CurrentScheme.Items.Add(dst);
            }
            (CurrentScheme as SchemeSerialization).SyncItemsFromBase();
        }

        private void Delete()
        {
            SuitSchemeItemCollection items = new SuitSchemeItemCollection();
            axSuitSchemeView1.SelectedItems(items);
            foreach (SchemeItem item in items)
            {
                axSuitSchemeView1.Remove(item.Name);
            }
        }

        private void Locked()
        {
            SuitSchemeItemCollection items = new SuitSchemeItemCollection();
            axSuitSchemeView1.SelectedItems(items);
            foreach (SchemeItem item in items)
            {
                axSuitSchemeView1.Locked(item.Name, false);
            }
        }

        private void UnLocked()
        {
            SuitSchemeItemCollection items = new SuitSchemeItemCollection();
            axSuitSchemeView1.SelectedItems(items);
            foreach (SchemeItem item in items)
            {
                axSuitSchemeView1.Locked(item.Name, true);
            }
        }

    }
}
