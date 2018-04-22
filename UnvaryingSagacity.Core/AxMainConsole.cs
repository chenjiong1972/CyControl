using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace UnvaryingSagacity.Core
{
    
    public  class AxMainConsole:UserControl
    {
        public delegate void ItemEventHandle(object sender,ItemEventArgs e);

        public event ItemEventHandle ItemMouseClick;
        public event PaintEventHandler FlowArrowPaint;

        private Items _items ;
        private ArrowItems _arrowItems;
        private Rectangle _lastPromptRectangle = new Rectangle();
        private bool _Matrix = false;
        private string  _current="";
        private int start_x = 200;
        private int start_y = 100;
        private int maxRow = 0;
        private int maxCol = 0;
        private int[] row = new int[0];
        private int[] col = new int[0];


        public AxMainConsole()
        {
            if (!Core.General.Check())
                return;
            InitializeComponent();
            _items = new Items();
            _arrowItems = new ArrowItems();
            this.ResizeRedraw = false;
            Title = "";
            TitleForeColor = this.ForeColor;
            TitleFont = this.Font;
            TitleVAlignment = StringAlignment.Far;
            TitleHAlignment = StringAlignment.Far;
        }

        public string Title { get; set; }

        public Color TitleForeColor { get; set; }

        public Font TitleFont { get; set; }

        public StringAlignment TitleVAlignment { get; set; }

        public StringAlignment TitleHAlignment { get; set; }

        public bool MatrixCompleted { get { return _Matrix; } }

        public void Clear()
        {
            _Matrix = false;
            _items.Clear();
        }

        /// <summary>
        /// 内部绘制原点
        /// </summary>
        public Point Origin { get { return new Point(start_x, start_y); } set { start_x = value.X; start_y = value.Y; } }
        /// <summary>
        /// 在流程图中增加一个模块.
        /// </summary>
        /// <param name="r">行号</param>
        /// <param name="c">列号</param>
        /// <param name="name">名称</param>
        /// <param name="description">描述</param>
        /// <param name="image">图片</param>
        public void Add(int r,int c,string name,string description,Image image,bool enable)
        {
            Add(r, c, name, description, image, enable, false);
        }

        /// <summary>
        /// 增加一个箭头标记
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        public void Add(int r,int c)
        {
            Add(r, c, "", "", null , false ,true  ); 
        }
        private  void Add(int r, int c, string name, string description, Image image, bool enable,bool arrow)
        {
            ItemDescription item = new ItemDescription();
            item.Col = c;
            item.Description = description;
            item.Enable = enable;
            item.Image = image;
            item.Name = name;
            item.Row = r;
            item.Arrow = arrow;
            if (item.Name.Length > 0)
            {
                _items.Add(name, item);
            }
            else
            {
                _items.Add(item);
            }
            if ((r + 1) > maxRow)
                maxRow = r + 1;
            if ((c + 1) > maxCol)
                maxCol = c + 1;
            if (_Matrix)
                _Matrix = false;
        }
        

        public void AddRange(ItemDescription[] itemDescriptions)
        {
            for (int i = 0; i < itemDescriptions.Length; i++)
            {
                if (!String.IsNullOrEmpty ( itemDescriptions[i].Name))
                {
                    _items.Add(itemDescriptions[i].Name, itemDescriptions[i]);
                }
                else
                {
                    _items.Add(itemDescriptions[i]);
                }
                if ((itemDescriptions[i].Row + 1) > maxRow)
                    maxRow = itemDescriptions[i].Row + 1;
                if ((itemDescriptions[i].Col + 1) > maxCol)
                    maxCol = itemDescriptions[i].Col + 1;
            }
            if (_Matrix)
                _Matrix = false;
        }

        /// <summary>
        /// 动态改变Enable
        /// </summary>
        /// <param name="itemDescriptions"></param>
        public void RefreshEnable(ItemDescription[] itemDescriptions)
        {
            foreach (ItemDescription item in itemDescriptions)
            {
                if (_items.ContainsKey(item.Name))
                {
                    ItemDescription myItem = _items[item.Name];
                    if (myItem.Enable != item.Enable)
                        myItem.Enable = item.Enable;
                }
            }
        }

        public void CreateMaxic()
        {
            CreateMaxic(new Size(128, 128), new Rectangle(8, 2, 112, 112),50,70);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sz">包含边框的实际空间</param>
        /// <param name="margin">去掉边框的实际有效空间</param>
        /// <param name="lineSpace">如果需要流程箭头,最好=0</param>
        /// <param name="columnSpace">如果需要流程箭头,最好=0</param>
        public void CreateMaxic(Size sz, Rectangle margin, int lineSpace, int columnSpace)
        {
            int i = 0;
            Array.Resize<int>(ref this.row , maxRow); 
            for (i = 0; i < maxRow; i++)
            {
                row[i] = i * (sz.Height  + lineSpace) + start_y;
            }
            Array.Resize<int>(ref col, maxCol); 
            for (i = 0; i < maxCol; i++)
            {
                col[i] = i * (sz.Width + columnSpace) + start_x;
            }
            foreach (ItemDescription item in _items)
            {
                item.Bounds = new Rectangle(col[item.Col], row[item.Row], sz.Width, sz.Height);
                item.Margin = margin;
            }
            _Matrix = true;
        }

        public int Count { get { return _items.Count; } }

        #region Draw 画流程
        public void Draw()
        {
            if (!_Matrix)
                CreateMaxic();
            Graphics g = Graphics.FromHwnd(this.Handle);
            Draw(g);
            g.Dispose();
        }

        void Draw(Graphics g)
        {
            if (_Matrix)
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                foreach (ItemDescription item in _items)
                {
                    Rectangle rt = new Rectangle(item.Bounds.Left + item.Margin.Left, item.Bounds.Top + item.Margin.Top, item.Margin.Width, item.Margin.Height);
                    if (item.Arrow )
                    {
                        ////画连接线
                        if (FlowArrowPaint != null)
                        {
                            rt = new Rectangle(item.Bounds.Left, item.Bounds.Top, item.Bounds.Width, item.Bounds.Height);
                            FlowArrowPaint(item as ArrowItem, new PaintEventArgs(g, rt));
                        }
                    }
                    else
                    {
                        Font font=new Font("宋体", 9);
                        SizeF szf = g.MeasureString(item.Name, font);
                        if (szf.Width > item.Bounds.Width)
                        {
                            g.DrawString(item.Name, font, new SolidBrush(this.ForeColor), new RectangleF(new PointF(item.Bounds.X-(szf.Width -item.Bounds.Width )/2, item.Bounds.Bottom - 14), new SizeF(szf.Width, 14)), sf);
                        }
                        else
                        {
                            g.DrawString(item.Name, font, new SolidBrush(this.ForeColor), new RectangleF(new PointF(item.Bounds.X, item.Bounds.Bottom - 14), new SizeF(item.Bounds.Width, 14)), sf);
                        }
                        if (item.Name.Equals(_current, StringComparison.OrdinalIgnoreCase) && item.Enable)
                        {
                            g.DrawImage(item.Image, item.Bounds);
                        }
                        else
                            ImageHandler.DrawImageDark(g, item.Image, rt);

                        if (this.BorderStyle != BorderStyle.None)
                            g.DrawRectangle(SystemPens.Control, item.Bounds);
                    }
                }
            }
        }
        #endregion

        void DrawPrompt(Point p)
        {
            #region 注意清理和不必要的画
            string newName = GetItemDescriptionName(p);
            if (!String.IsNullOrEmpty(newName))
            {
                if (newName.Equals(_current, StringComparison.OrdinalIgnoreCase))
                    return;
            }
            this.Invalidate(_lastPromptRectangle);
            this.Update();
            #endregion
            _lastPromptRectangle = new Rectangle();
            Graphics g = Graphics.FromHwnd(this.Handle);
            if (!String.IsNullOrEmpty(_current))
            {
                ItemDescription currentitem = _items.GetItem(_current);
                if (currentitem != null)
                {
                    if (currentitem.Enable)
                        ImageHandler.DrawImageDark(g, currentitem.Image, currentitem.Bounds);
                }
            }
            _current = newName;
            if (!String.IsNullOrEmpty(newName))
            {
                ItemDescription item = _items.GetItem(newName);
                GraphicsPath shape = new GraphicsPath();
                if (item.Enable)
                {
                    this.Invalidate(item.Bounds);
                    this.Update();
                    this.Cursor = Cursors.Hand;
                }
                SizeF sf = g.MeasureString(item.Description, this.Font);
                Core.ImageHandler.ArcRectanglePath(shape, item.Bounds.Left + (item.Bounds.Width / 2), item.Bounds.Top + ((item.Bounds.Height - 50) / 2) - (int)(70 * 0.8), (int)(sf.Width) + 20, 50, 10, 10);
                RectangleF rt = shape.GetBounds();
                rt.Inflate(1, 1);
                g.FillPath(new SolidBrush(Color.FromArgb(128, Color.Black)), shape);
                //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.DrawString(item.Name, new Font("宋体", 10, FontStyle.Bold), Brushes.AntiqueWhite, new PointF(rt.Left + 10, rt.Top + 10));
                g.DrawString(item.Description, this.Font, Brushes.AntiqueWhite, new PointF(rt.Left + 10, rt.Bottom - 18));
                _lastPromptRectangle = Rectangle.Ceiling(rt);
            }
            else
                this.Cursor = Cursors.Default;
            g.Dispose();
        }

        string GetItemDescriptionName(Point p)
        {
            foreach (ItemDescription item in _items)
            {
                if (item.Bounds.Contains(p))
                {
                    return item.Name;
                }
            }
            return "";
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (ItemMouseClick != null)
            {
                string name=GetItemDescriptionName (e.Location );
                if(!String.IsNullOrEmpty(name))
                {
                    ItemDescription item = _items.GetItem(name);
                    if (item.Enable)
                    {
                        ItemEventArgs ea = new ItemEventArgs(item, e);
                        ItemMouseClick(this, ea);
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {

            DrawPrompt(e.Location );
            base.OnMouseMove(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Title.Length > 0)
            {
                Core.TextShadow ts = new TextShadow();
                SizeF szf = e.Graphics.MeasureString(Title, TitleFont);
                float x=0;
                float y=0;
                switch (TitleHAlignment)
                {
                    case StringAlignment.Far :
                        x = this.Width - szf.Width - 10;
                        break;
                    case StringAlignment.Near :
                        x = 0;
                        break;
                    case StringAlignment.Center :
                        x = (this.Width - szf.Width) / 2;
                        break;
                    default :
                        break;
                }
                switch (TitleVAlignment)
                {
                    case StringAlignment.Far:
                        y = this.Height - szf.Height - 10;
                        break;
                    case StringAlignment.Near:
                        break;
                    case StringAlignment.Center:
                        y = (this.Height - szf.Height) / 2;
                        break;
                    default:
                        break;
                }
                ts.Draw(e.Graphics, Title, TitleFont, new PointF(x, y));
                e.Graphics.DrawString(Title, TitleFont, new SolidBrush(TitleForeColor), new PointF(x, y)); 
            }
            Draw(e.Graphics);
            base.OnPaint(e);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AxMainConsole
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Name = "AxMainConsole";
            this.Size = new System.Drawing.Size(162, 146);
            this.ResumeLayout(false);

        }


    }

    public class ItemEventArgs
    {
        private int _r;
        private int _c;
        private string _name;
        private ItemDescription _item;
        private MouseEventArgs _m;

        public ItemEventArgs(int r, int c, string name, MouseEventArgs m)
        {
            _r = r;
            _c = c;
            _m = m;
            _name = name;
        }

        public ItemEventArgs(ItemDescription item, MouseEventArgs m)
        {
            _item = item;
            _r = _item.Row;
            _c = _item.Col;
            _m = m;
            _name = _item.Name;
        }

        public ItemDescription Item { get { return _item; } }

        public MouseEventArgs MouseEvent { get { return _m; } }

        public int R { get { return _r; } }

        public int C { get { return _c; } }

        public string Name { get { return _name; } }
    }

    public class ItemDescription : ArrowItem
    {
        public Rectangle Margin { get; set; }
        public string Description { get; set; }

        public string Name { get; set; }

        [System.Xml.Serialization.XmlIgnore()]    
        public Image Image { get; set; }

        public bool Enable { get; set; }

        public void CopyTo(ItemDescription newItem)
        {
            if (newItem != null)
            {
                base.CopyTo(newItem as ArrowItem);
                newItem.Margin = this.Margin;
                newItem.Description = this.Description;
                newItem.Name = this.Name;
                newItem.Image = this.Image;
                newItem.Enable = this.Enable;
            }
        }
    }

    public class ArrowItem
    {
        [System.Xml.Serialization.XmlIgnore()] 
        public Rectangle Bounds { get; set; }

        [System.Xml.Serialization.XmlIgnore()]
        public int Row { get; set; }

        [System.Xml.Serialization.XmlIgnore()]
        public int Col { get; set; }

        [System.Xml.Serialization.XmlIgnore()]
        public bool Arrow { get; set; }

        public void CopyTo(ArrowItem newItem)
        {
            if (newItem != null)
            {
                newItem.Bounds = this.Bounds;
                newItem.Row = this.Row;
                newItem.Col = this.Col;
                newItem.Arrow = this.Arrow;
            }
        }
    }

    public class ApplyItem : ItemDescription
    {
        public string TypeName { get; set; }
        public string DllName { get; set; }
        public string ReturnTypeName { get; set; }
        public string InvokeMethod { get; set; }
        public string Lines { get; set; }

        [System.Xml.Serialization.XmlIgnore()]
        public Color LineColor { get; set; }
        public int LineWidth { get; set; }
        public string Parent { get; set; }
        public string ParentName { get; set; }
        public string Title { get; set; }
        public void CopyTo(ApplyItem newItem)
        {
            if (newItem != null)
            {
                base.CopyTo(newItem);
                newItem.TypeName = this.TypeName;
                newItem.DllName = this.DllName;
                newItem.ReturnTypeName = this.ReturnTypeName;
                newItem.InvokeMethod = this.InvokeMethod;
                newItem.Lines = this.Lines;
                newItem.LineColor = this.LineColor;
                newItem.LineWidth = this.LineWidth;
                newItem.Parent = this.Parent;
                newItem.ParentName = this.ParentName;
                newItem.Title = this.Title;
            }
        }
        public override string ToString()
        {
            return base.Name + " - " + ParentName;
        }

        [System.Xml.Serialization.XmlIgnore()]
        public MouseEventHandler OnMouseClick;
    }

    internal class Items : ListDictionary<ItemDescription> { }

    internal class ArrowItems : ListDictionary<ArrowItem> { }

}
