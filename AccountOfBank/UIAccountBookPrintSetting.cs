using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace UnvaryingSagacity.AccountOfBank
{
    public partial class UIAccountBookPrintSetting : Form
    {
        private ItemOfBankViewFilter[] ivfs = new ItemOfBankViewFilter [0];
        private DataProvider dp;
        private int _rowsPrePage = 0;
        private Environment _e;
        private AccountBookDateProvider _printDataProvider;
        internal Environment CurrentEnvironment { get { return _e; } set { _e = value; } }

        public UIAccountBookPrintSetting()
        {
            InitializeComponent();
            this.Shown += new EventHandler(UIAccountBookPrintSetting_Shown);
            this.dataGridView1.CellPainting += new DataGridViewCellPaintingEventHandler(dataGridView1_CellPainting);
            this.dataGridView1.CellContentClick += new DataGridViewCellEventHandler(dataGridView1_CellContentClick);
            this.dataGridView1.CellMouseClick += new DataGridViewCellMouseEventHandler(dataGridView1_CellMouseClick);
            this.dataGridView1.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dataGridView1_EditingControlShowing);
            this.dataGridView1.CellValidated += new DataGridViewCellEventHandler(dataGridView1_CellValidated); 
            #region button Event
            foreach (object button in toolStrip1.Items)
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
                    ToolStripSplitButton ddb = button as ToolStripSplitButton;
                    foreach (object o in ddb.DropDownItems)
                    {
                        if (o.GetType() == typeof(ToolStripButton))
                        {
                            ((ToolStripButton)o).Click += new EventHandler(button_Click);
                        }
                        else if (o.GetType() == typeof(ToolStripMenuItem))
                        {
                            ((ToolStripMenuItem)o).Click += new EventHandler(button_Click);
                        }
                        else if (o.GetType() == typeof(ToolStripSplitButton))
                        {
                            ((ToolStripSplitButton)o).ButtonClick += new EventHandler(button_Click);
                        }
                    }
                }
                else if (button.GetType() == typeof(ToolStripDropDownButton))
                {
                    ToolStripDropDownButton ddb = button as ToolStripDropDownButton;
                    foreach (object o in ddb.DropDownItems)
                    {
                        if (o.GetType() == typeof(ToolStripButton))
                        {
                            ((ToolStripButton)o).Click += new EventHandler(button_Click);
                        }
                        else if (o.GetType() == typeof(ToolStripMenuItem))
                        {
                            ((ToolStripMenuItem)o).Click += new EventHandler(button_Click);
                        }
                        else if (o.GetType() == typeof(ToolStripSplitButton))
                        {
                            ((ToolStripSplitButton)o).ButtonClick += new EventHandler(button_Click);
                        }
                    }
                }

            }
            #endregion

        }

        void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex == 5) && e.RowIndex >= 0)
            {
                int i;
                if(int.TryParse (dataGridView1[e.ColumnIndex ,e.RowIndex ].Value.ToString (),out i) )
                {
                    ivfs[e.RowIndex].PageRanges.BeginNumber = i;
                }
            }
        }

        void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if ((dataGridView1.CurrentCell.ColumnIndex == 5) && dataGridView1.CurrentCell.RowIndex >= 0)
            {
                e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress);
            }
        }

        void Control_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsControl(e.KeyChar))
            {
                if (!Char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }

        void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((e.ColumnIndex == 2 || e.ColumnIndex == 4) && e.RowIndex >= 0)
            {
                Size size = dataGridView1[e.ColumnIndex,e.RowIndex].Size;
                if (!(e.X > (size.Width - 18) && e.X < size.Width))
                {
                    return;
                }
                if (e.ColumnIndex == 2)
                {
                    UIViewFilter ui = new UIViewFilter();
                    ui.d1 = ivfs[e.RowIndex].Filter.d1;
                    ui.d2 = ivfs[e.RowIndex].Filter.d2;
                    if (ui.ShowDialog(this) == DialogResult.OK)
                    {
                        ivfs[e.RowIndex].Filter.d1 = ui.d1;
                        ivfs[e.RowIndex].Filter.d2 = ui.d2;
                        dataGridView1[e.ColumnIndex, e.RowIndex].Value = ivfs[e.RowIndex].Filter.ToString();
                        if ((bool)dataGridView1[0, e.RowIndex].EditedFormattedValue)
                            GetTotalPages(dataGridView1.Rows[e.RowIndex]);
                        else
                        {
                            ivfs[e.RowIndex].Complated = false;
                        }
                    }
                }
                else if (e.ColumnIndex == 4)
                {
                    Core.Printer.PrinterPageRangesAndBeginPageNumber p = Core.Printer.PrintAssign.ShowPrintPageRangeDialog(this, false);
                    if (p != null)
                    {
                        string s = p.Ranges;
                        if (p.AllPages)
                        {
                            s = "全部";
                            ivfs[e.RowIndex].PageRanges.Ranges = "";
                        }
                        else
                        {
                            ivfs[e.RowIndex].PageRanges.Ranges = s;
                        }
                        dataGridView1[e.ColumnIndex, e.RowIndex].Value = s;  
                    }
                }
            }
        }

        void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 0)
                {
                    if ((bool)dataGridView1[0, e.RowIndex].EditedFormattedValue)
                        GetTotalPages(dataGridView1.Rows[e.RowIndex]);
                    else
                    {
                        ivfs[e.RowIndex].Complated = false;
                    }

                }
                else if (e.ColumnIndex == 6)
                {
                    if ((bool)dataGridView1[0, e.RowIndex].EditedFormattedValue)
                    { ivfs[e.RowIndex].PrintCover = true; }
                    else
                    {
                        ivfs[e.RowIndex].PrintCover = false;
                    }
                }
            }
        }

        void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if( (e.ColumnIndex == 2 || e.ColumnIndex ==4) && e.RowIndex >= 0)
            {
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All);
                e.Graphics.DrawImageUnscaled(Properties.Resources.button, e.CellBounds.X + e.CellBounds.Width - 20, e.CellBounds.Y + ((e.CellBounds.Height - 16) / 2));
                e.Handled = true;
            }
        }

        void UIAccountBookPrintSetting_Shown(object sender, EventArgs e)
        {
            InitItemOfBankList();
        }

        void button_Click(object sender, EventArgs e)
        {
            string s = ((ToolStripItem)sender).Text.Trim();
            switch (s)
            {
                case "打印账本":
                    DoPrint(false);
                    break;
                case "页面设置":
                    Core.Printer.PrintAssign p = InitPrintAssign();
                    p.ShowPageSetupDialog(this);
                    break;
                case "预览":
                    DoPrint(true);
                    break;
                case "打印封面":
                    PrintCover(false, default(ItemOfBank));
                    break;
                case "预览封面":
                    PrintCover(true, default(ItemOfBank));
                    break;
                case "封面设计":
                    UIDesigner ui = new UIDesigner();
                    ui.CurrentEnvironment = CurrentEnvironment ;
                    ui.ShowDialog(this);
                    break;
                default:
                    break;
            }
        }

        void DoPrint(bool isPreview)
        {
            foreach (ItemOfBankViewFilter ivf in ivfs)
            {
                if (ivf.Complated && ivf.PrintCover )
                {
                    PrintCover(isPreview, ivf.Item); 
                }
            }
            Core.Printer.PrintAssign printAssign = InitPrintAssign();
            Core.Printer.PrintData printData = new UnvaryingSagacity.Core.Printer.PrintData();
            GetAccountPageHeader(printData, 0, null);
            printAssign.CustomPage += new UnvaryingSagacity.Core.Printer.CustomPageEventHandler(printAssign_CustomPage);
            printAssign.CustomMode = true;
            if (_printDataProvider == null)
            {
                _printDataProvider = new AccountBookDateProvider(_e);
            }
            _printDataProvider.Init(ivfs);
            if (isPreview)
            {
                printAssign.ShowPreviewDialog(this);
            }
            else
            {
                printAssign.Print();
            }
        }

        void printAssign_CustomPage(object sender, UnvaryingSagacity.Core.Printer.CustomPageEventArgs e)
        {
            if (_printDataProvider != null)
            {
                _printDataProvider.FullPrintData(e);
                if (!e.HasMorePages)
                {
                    _printDataProvider.Init(ivfs);
                }
            }
        }


        Core.Printer.PrintAssign InitPrintAssign()
        {
            this.Cursor = Cursors.WaitCursor;
            Core.Printer.PrintAssign printer = CurrentEnvironment.InitPrintAssign();
            printer.SavePageSetup += new EventHandler(printer_SavePageSetup);
            this.Cursor = Cursors.Default;
            return printer;
        }

        void printer_SavePageSetup(object sender, EventArgs e)
        {
            string filename = CurrentEnvironment.PageSetupFilename;
            Core.XmlSerializer<Core.Printer.PageSetup>.ToXmlSerializer(filename, (sender as Core.Printer.PrintAssign).PageSetup);
            _rowsPrePage = 0;
            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                GetTotalPages(r);
            }
        }

        void PrintCover(bool isPreview,ItemOfBank item)
        {
            SchemeSerialization scheme = new SchemeSerialization();
            if (!CurrentEnvironment.LoadScheme(scheme, CurrentEnvironment.AccountCoverFilename))
            {
                CurrentEnvironment.CreateScheme(scheme);
            }
            CurrentEnvironment.AccountCoverCalculator(scheme, item, false);
            CurrentEnvironment.PrintScheme(this, scheme, isPreview, false, false); 
        }

        void InitItemOfBankList()
        {
            dp = new DataProvider(CurrentEnvironment.CurrentAccount.FullPath);
            ItemOfBankCollection items = dp.GetItemOfBankList();
            foreach (ItemOfBank item in items)
            {
                Array.Resize<ItemOfBankViewFilter>(ref ivfs, ivfs.Length + 1);
                int r = dataGridView1.Rows.Add();
                ivfs[r] = new ItemOfBankViewFilter();
                ivfs[r].RowsPrePage = _rowsPrePage;
                ivfs[r].Filter =  new ViewFiter();
                ivfs[r].Filter.d1 = new DateTime(DateTime.Today.Year, 1, 1);
                ivfs[r].Filter.d2 = new DateTime(DateTime.Today.Year, 12, 31);
                ivfs[r].Item = item;
                ivfs[r].PrintCover = false;
                ivfs[r].PageRanges = new UnvaryingSagacity.Core.Printer.PrinterPageRangesAndBeginPageNumber(); 
                dataGridView1[1, r].Value = item.Name + "(" + item.OfBankName + ")";
                dataGridView1[2, r].Value = ivfs[r].Filter.ToString();// DateTime.Today.Year + "-01-01 到 " + DateTime.Today.Year + "-12-31";
                dataGridView1[4, r].Value = "全部"; 
                dataGridView1[5, r].Value = 1;
                dataGridView1.Rows[r].Tag = item;
            }
        }

        void GetTotalPages(DataGridViewRow r)
        {
            r.Cells[3].Value = "正计算页数...";
            if (_rowsPrePage == 0)
            {
                GetPageInfo();
            }
            Thread th = new Thread(CalcPagesThead);
            ItemOfBankViewFilter ivf = ivfs[r.Index];
            ivf.Complated = false; 
            ivf.RowsPrePage = _rowsPrePage;
            th.Start(ivf);
        }

        void CalcPagesThead(object data)
        {
            ItemOfBankViewFilter ivf = data as ItemOfBankViewFilter;
            DataProvider dp = new DataProvider(CurrentEnvironment.CurrentAccount.FullPath);
            string filterExpress = "id='" + ivf.Item.ID + "' AND recordDate>='" + Core.General.FromDateTime(ivf.Filter.d1) + "' AND recordDate <='" + Core.General.FromDateTime(ivf.Filter.d2) + "'";
            string s = "上期余额";
            if (ivf.Filter.d1.Month == 1 && ivf.Filter.d1.Day == 1)
            {
                if (ivf.Filter.d1.Year == _e.CurrentAccount.StartYear)
                {
                    s = "年初余额";
                }
                else
                {
                    s = "上年结转";
                }
            }
            lock (ivf)
            {
                double bal = dp.GetStartBal(ivf.Filter.d1, ivf.Item);
                ivf.Resulte = dp.GetEntryExList(bal, s, filterExpress, "recordDate,entry", ((ivf.Filter.d2.Month == 12 && ivf.Filter.d2.Day == 31) ? true : false), ivf.Filter.d1,false );
                if (ivf.Resulte.Count <= 0)
                {
                    ivf.PageCount = 1;
                }
                else
                {
                    ivf.PageCount = (int)(Math.Ceiling(((float)ivf.Resulte.Count / (ivf.RowsPrePage - 2))));//扣除承前过次行,这里忽略了第一页没有承前行
                }
                ivf.Complated = true;
            }
            foreach (DataGridViewRow r in this.dataGridView1.Rows)
            {
                if (ivf.Item.ID == (r.Tag as ItemOfBank).ID)
                {
                    lock (r.Cells[3].Value)
                    {
                        r.Cells[3].Value = ivf.PageCount;

                    }
                    return;
                }
            }
             
        }

        void GetPageInfo()
        {
            Core.Printer.PrintAssign printAssign = InitPrintAssign();
            Core.Printer.PrintData printData = new UnvaryingSagacity.Core.Printer.PrintData();
            GetAccountPageHeader(printData, 0, null);
            _rowsPrePage = printAssign.RowsPrePhyPage(printData);
        }
        void GetAccountPageHeader(Core.Printer.PrintData printData, int Year, ItemOfBank itemOfBank)
        {
            Core.Printer.PrintData p = printData;
            Core.Printer.AttachRow ar = new UnvaryingSagacity.Core.Printer.AttachRow();
            Core.Printer.PrinterAttachCell aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.InRowPercent = 33;
            aCell.Properties.Value = "";
            ar.Add(aCell, "main1");
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            aCell.InRowPercent = 33;
            aCell.Properties.Value = "银 行 存 款 日 记 账";
            aCell.Properties.FontColor = Color.Green.ToArgb();
            ar.Add(aCell, "main2");
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightTop;
            aCell.InRowPercent = 33;
            aCell.Properties.Value = "第 &P 页";
            aCell.Properties.font = new Font("宋体", 12, FontStyle.Bold);
            aCell.Properties.FontColor = Color.Green.ToArgb();
            ar.Add(aCell, "main3");
            p.MainTitle.Add(ar, "main");

            ar = new UnvaryingSagacity.Core.Printer.AttachRow();
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            aCell.InRowPercent = 25;
            aCell.Properties.Value = "";
            ar.Add(aCell, "subTitle1");
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            aCell.InRowPercent = 50;
            aCell.Properties.Value = _e.CurrentAccount.Name;
            ar.Add(aCell, "subTitle2");
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            aCell.InRowPercent = 25;
            aCell.Properties.Value = "";
            ar.Add(aCell, "subTitle3");
            ar.Offset_V = 10;
            p.SubTitles.Add(ar, "subTitle");
            #region SheetHeader

            ar = new UnvaryingSagacity.Core.Printer.AttachRow();
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            //aCell.Properties.AddTextEx (Color.Green.ToArgb (),"开户银行: ");
            //aCell.Properties.AddTextEx(Color.Black.ToArgb(), (itemOfBank == null ? "" : itemOfBank.OfBankName));
            aCell.Properties.Value = "开户银行: " + (itemOfBank == null ? "" : itemOfBank.OfBankName);
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.LeftMiddle;
            aCell.InRowPercent = 100;
            ar.Add(aCell, "left1");
            ar.Offset_V = 15;
            p.Headers.Add(ar, "h1");

            ar = new UnvaryingSagacity.Core.Printer.AttachRow();
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Value = "账    号: " + (itemOfBank == null ? "" : itemOfBank.ID);
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.LeftMiddle;
            aCell.InRowPercent = 100;
            ar.Add(aCell, "left");
            ar.Offset_V = 5;
            p.Headers.Add(ar, "h2");
            #endregion
            Core.Printer.Body body = p.Body;
            body.Offset_V = 8;
            #region SheetTitle

            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(22));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(22));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(27));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(27));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(27));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(27));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(200));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(160));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(160));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(22));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(160));
            Core.Printer.PrinterCell cell;
            int r = 1;
            #region 第一行
            p.Rows.Add(new UnvaryingSagacity.Core.Printer.Row(30));
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = Year + "年";
            body.Add("$" + r + "$1", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = Year + "年";
            body.Add("$" + r + "$2", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "记账凭证";
            cell.WordWrap = true;
            body.Add("$" + r + "$3", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "";
            body.Add("$" + r + "$4", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "支票";
            body.Add("$" + r + "$5", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "支票";
            body.Add("$" + r + "$6", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "摘  要";
            body.Add("$" + r + "$7", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "借  方";
            body.Add("$" + r + "$8", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "贷  方";
            body.Add("$" + r + "$9", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = Core.Printer.PrintAssign.OK_FLAG;
            body.Add("$" + r + "$10", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "余  额";
            body.Add("$" + r + "$11", cell);
            r++;
            #endregion
            #region 第二行
            p.Rows.Add(new UnvaryingSagacity.Core.Printer.Row(30));
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();

            SetPrintCellFullBorder(ref cell, Color.Green);


            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "月";
            body.Add("$" + r + "$1", cell);
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();

            SetPrintCellFullBorder(ref cell, Color.Green);


            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "日";
            body.Add("$" + r + "$2", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "字";
            body.Add("$" + r + "$3", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "号";
            body.Add("$" + r + "$4", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "种类";
            body.Add("$" + r + "$5", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "号码";
            body.Add("$" + r + "$6", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "摘  要";
            body.Add("$" + r + "$7", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "借  方";
            cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线标题;
            body.Add("$" + r + "$8", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "贷  方";
            cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线标题;
            body.Add("$" + r + "$9", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = Core.Printer.PrintAssign.OK_FLAG;
            body.Add("$" + r + "$10", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "余  额";
            cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线标题;
            body.Add("$" + r + "$11", cell);
            r++;
            #endregion
            p.Mergers.Add(new UnvaryingSagacity.Core.Printer.Range(1, 1, 1, 2));
            p.Mergers.Add(new UnvaryingSagacity.Core.Printer.Range(1, 3, 1, 4));
            p.Mergers.Add(new UnvaryingSagacity.Core.Printer.Range(1, 5, 1, 6));
            p.Mergers.Add(new UnvaryingSagacity.Core.Printer.Range(1, 7, 2, 7));
            p.Mergers.Add(new UnvaryingSagacity.Core.Printer.Range(1, 10, 2, 10));
            p.TopFixedRows = 2;
            #endregion

            if (itemOfBank == null)
            {
                p.Rows.Add(new UnvaryingSagacity.Core.Printer.Row(25));
            }
        }

        Core.Printer.PrintData GetPrintDataByEntryExList(EntryExList entries, ItemOfBank item)
        {
            this.Cursor = Cursors.WaitCursor;
            Core.Printer.PrintData printData = new UnvaryingSagacity.Core.Printer.PrintData();
            int y = Core.General.FromString(entries[0].Date).Year;
            GetAccountPageHeader(printData, y, item);
            int r = 3;
            int c = 1;
            Core.Printer.Body body = printData.Body;
            foreach (EntryEx entry in entries)
            {
                printData.Rows.Add(new UnvaryingSagacity.Core.Printer.Row(25));
                Core.Printer.PrinterCell cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                DateTime dt = Core.General.FromString(entry.Date);
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                cell.Value = entry.Side == 0 ? "" : dt.Month.ToString();
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                cell.Value = entry.Side == 0 ? "" : dt.Day.ToString();
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                cell.Value = entry.Side == 0 ? "" : entry.VchType;
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                cell.Value = entry.Side == 0 ? "" : entry.VchNumber;
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                cell.Value = entry.Side == 0 ? "" : entry.ChequeType;
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                cell.Value = entry.Side == 0 ? "" : entry.Cheque;
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.LeftMiddle;
                cell.Value = entry.Digest;
                if (entry.Side == 0)
                {
                    cell.FontColor = Color.Red.ToArgb();
                    cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                }
                body.Add("$" + r + "$" + c, cell);
                c++;
                if (entry.Side == 0)
                {
                    cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                    SetPrintCellFullBorder(ref cell, Color.Green);
                    cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                    cell.Value = entry.JMoney;
                    cell.L_Color = Color.Green.ToArgb();
                    cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线;
                    body.Add("$" + r + "$" + c, cell);
                    c++;
                    cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                    SetPrintCellFullBorder(ref cell, Color.Green);
                    cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                    cell.Value = entry.DMoney;
                    cell.L_Color = Color.Green.ToArgb();
                    cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线;
                    body.Add("$" + r + "$" + c, cell);
                    c++;
                }
                else
                {
                    cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                    SetPrintCellFullBorder(ref cell, Color.Green);
                    cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                    cell.Value = entry.Side == 1 ? entry.Money : 0;
                    cell.L_Color = Color.Green.ToArgb();
                    cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线;
                    body.Add("$" + r + "$" + c, cell);
                    c++;
                    cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                    SetPrintCellFullBorder(ref cell, Color.Green);
                    cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                    cell.Value = entry.Side == -1 ? entry.Money : 0;
                    cell.L_Color = Color.Green.ToArgb();
                    cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线;
                    body.Add("$" + r + "$" + c, cell);
                    c++;
                }
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                cell.Value = "";
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                cell.Value = entry.Bal;
                cell.L_Color = Color.Green.ToArgb();
                cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线;
                body.Add("$" + r + "$" + c, cell);
                c = 1;
                r++;
            }
            this.Cursor = Cursors.Default;
            return printData;
        }

        void SetPrintCellFullBorder(ref Core.Printer.PrinterCell cell)
        {
            SetPrintCellFullBorder(ref cell, Color.Black);
        }

        void SetPrintCellFullBorder(ref Core.Printer.PrinterCell cell, Color color)
        {
            cell.B_Style = UnvaryingSagacity.Core.Printer.PrinterBorderStyle.实线边框;
            cell.T_Style = UnvaryingSagacity.Core.Printer.PrinterBorderStyle.实线边框;
            cell.L_Style = UnvaryingSagacity.Core.Printer.PrinterBorderStyle.实线边框;
            cell.R_Style = UnvaryingSagacity.Core.Printer.PrinterBorderStyle.实线边框;
            cell.B_Color = color.ToArgb();
            cell.T_Color = color.ToArgb();
            cell.L_Color = color.ToArgb();
            cell.R_Color = color.ToArgb();
        }

    }



    class ItemOfBankViewFilter 
    {
        public ViewFiter Filter { get; set; }
        public ItemOfBank Item { get; set; }
        public EntryExList Resulte { get; set; }
        public int RowsPrePage { get; set; }
        public int PageCount { get; set; }
        public bool Complated { get; set; }
        public bool PrintCover { get; set; }
        public Core.Printer.PrinterPageRangesAndBeginPageNumber PageRanges { get; set; } 
    }

    class AccountBookDateProvider
    {
        private int _itemIndex = 0;
        private int _pageIndex = 0;
        private int _rowIndex = 0;
        private bool _initComplated = false;
        private ItemOfBankViewFilter[] _items;
        private Environment _e;

        private double _j = 0;//借方累计
        private double _d = 0;//贷方累计

        public AccountBookDateProvider(Environment e) { _e = e; }

        /// <summary>
        /// 
        /// </summary>
        public bool Init(ItemOfBankViewFilter[] Items)
        {
            _items = Items;
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i].Complated)
                {
                    _itemIndex = i;
                    _pageIndex = 1;
                    _rowIndex = 0;
                    _j = 0;
                    _d = 0;
                    _initComplated = true;
                    return true;
                }
            }
            return false;
        }

        public void FullPrintData(UnvaryingSagacity.Core.Printer.CustomPageEventArgs e)
        {
            if (!_initComplated)
                return;
            ItemOfBankViewFilter ivf = _items[_itemIndex];
            while (true)
            {
                EntryExList _list = new EntryExList();
                EntryEx entry;

                int rowCount = 0;
                if (_pageIndex > 1)
                {
                    entry = new EntryEx();
                    entry.Date = ivf.Filter.d1.ToString("yyyyMMdd");
                    entry.Digest = "承 前 页";
                    entry.Side = 0;
                    entry.JMoney = _j;
                    entry.DMoney = _d;
                    entry.Bal =(ivf.Resulte[_rowIndex - 1] as EntryEx).Bal;
                    _list.Add(entry);
                    rowCount++;
                }
                    
                for (int i = _rowIndex; i < ivf.Resulte.Count; i++)
                {
                    //"承 前 页"数据的来源有错误,必须从第一页开始累计.
                    entry = new EntryEx();
                    (ivf.Resulte[i] as EntryEx).CopyTo(entry);
                    _list.Add(entry);
                    if (entry.Side == 1)
                    {
                        _j += entry.Money;
                    }
                    else if (entry.Side == -1)
                    {
                        _d += entry.Money;
                    }
                    rowCount++;
                    _rowIndex++;
                    if (rowCount == (ivf.RowsPrePage - 1) && _rowIndex <ivf.Resulte.Count  )
                    {
                        entry = new EntryEx();
                        entry.Date = _list[_list.Count - 1].Date;
                        entry.Digest = "过 次 页";// "承 前 页";
                        entry.Side = 0;
                        entry.JMoney = _j;
                        entry.DMoney = _d;
                        entry.Bal = (_list[_list.Count - 1] as EntryEx).Bal;
                        _list.Add(entry);
                        break;
                    }
                }
                int finalPage = _pageIndex + (ivf.PageRanges.BeginNumber - 1);
                bool b = true;

                if (_list.Count > 0)
                {
                    if (!ivf.PageRanges.AllPages)
                    {
                        if (!ivf.PageRanges.Contains(_pageIndex))
                        {
                            b = false;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("填写账本时发生意外错误, 请对打印账本界面截屏并与开发商联系", "填写账本", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    b = true;
                }
                _pageIndex++;
                if (_rowIndex >= ivf.Resulte.Count)
                {
                    _itemIndex++;
                    _j = 0;
                    _d = 0;
                    _rowIndex = 0;
                    _pageIndex = 1;
                }
                if (b)
                {
                    e.pageContent = new UnvaryingSagacity.Core.Printer.PrintData();
                    SetPrintDataByEntryExList(e.pageContent, _list, ivf.Item, finalPage);
                    break;
                }
                else
                {
                    continue;
                }
            }
            #region 判断是否还有页
            while (_itemIndex < _items.Length)
            {
                if (_items[_itemIndex].Complated)
                {
                    e.HasMorePages = true;
                    break;
                }
                _itemIndex++;
            }
            #endregion
        }

        void GetAccountPageHeader(Core.Printer.PrintData printData, int Year, ItemOfBank itemOfBank,int pageNumber)
        {
            Core.Printer.PrintData p = printData;
            Core.Printer.AttachRow ar = new UnvaryingSagacity.Core.Printer.AttachRow();
            Core.Printer.PrinterAttachCell aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.InRowPercent = 33;
            aCell.Properties.Value = "";
            ar.Add(aCell, "main1");
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            aCell.InRowPercent = 33;
            aCell.Properties.Value = "银 行 存 款 日 记 账";
            aCell.Properties.FontColor = Color.Green.ToArgb();
            ar.Add(aCell, "main2");
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightTop;
            aCell.InRowPercent = 33;
            aCell.Properties.Value ="第 " + pageNumber+" 页";
            aCell.Properties.font = new Font("宋体", 12, FontStyle.Bold);
            aCell.Properties.FontColor = Color.Green.ToArgb();
            ar.Add(aCell, "main3");
            p.MainTitle.Add(ar, "main");

            ar = new UnvaryingSagacity.Core.Printer.AttachRow();
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            aCell.InRowPercent = 25;
            aCell.Properties.Value = "";
            ar.Add(aCell, "subTitle1");
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            aCell.InRowPercent = 50;
            aCell.Properties.Value = _e.CurrentAccount.Name;
            ar.Add(aCell, "subTitle2");
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            aCell.InRowPercent = 25;
            aCell.Properties.Value = "";
            ar.Add(aCell, "subTitle3");
            ar.Offset_V = 10;
            p.SubTitles.Add(ar, "subTitle");
            #region SheetHeader

            ar = new UnvaryingSagacity.Core.Printer.AttachRow();
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.AddTextEx(Color.Green.ToArgb(), "开户银行: ");
            aCell.Properties.AddTextEx(Color.Black.ToArgb(), (itemOfBank == null ? "" : itemOfBank.OfBankName));
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.LeftMiddle;
            aCell.InRowPercent = 100;
            ar.Add(aCell, "left1");
            ar.Offset_V = 15;
            p.Headers.Add(ar, "h1");

            ar = new UnvaryingSagacity.Core.Printer.AttachRow();
            aCell = new UnvaryingSagacity.Core.Printer.PrinterAttachCell();
            aCell.Properties.AddTextEx(Color.Green.ToArgb(), "账    号: ");
            aCell.Properties.AddTextEx(Color.Black.ToArgb(), (itemOfBank == null ? "" : itemOfBank.ID));
            //aCell.Properties.Value = "账  号: " + (itemOfBank == null ? "" : itemOfBank.ID);
            aCell.Properties.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.LeftMiddle;
            aCell.InRowPercent = 100;
            ar.Add(aCell, "left");
            ar.Offset_V = 5;
            p.Headers.Add(ar, "h2");
            #endregion
            Core.Printer.Body body = p.Body;
            body.Offset_V = 8;
            #region SheetTitle

            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(22));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(22));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(27));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(27));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(27));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(27));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(200));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(160));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(160));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(22));
            p.Cols.Add(new UnvaryingSagacity.Core.Printer.Col(160));
            Core.Printer.PrinterCell cell;
            int r = 1;
            #region 第一行
            p.Rows.Add(new UnvaryingSagacity.Core.Printer.Row(30));
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = Year + "年";
            body.Add("$" + r + "$1", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = Year + "年";
            body.Add("$" + r + "$2", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "记账凭证";
            cell.WordWrap = true;
            body.Add("$" + r + "$3", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "";
            body.Add("$" + r + "$4", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "支票";
            body.Add("$" + r + "$5", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "支票";
            body.Add("$" + r + "$6", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "摘  要";
            body.Add("$" + r + "$7", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "借  方";
            body.Add("$" + r + "$8", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "贷  方";
            body.Add("$" + r + "$9", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = Core.Printer.PrintAssign.OK_FLAG;
            body.Add("$" + r + "$10", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "余  额";
            body.Add("$" + r + "$11", cell);
            r++;
            #endregion
            #region 第二行
            p.Rows.Add(new UnvaryingSagacity.Core.Printer.Row(30));
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();

            SetPrintCellFullBorder(ref cell, Color.Green);


            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "月";
            body.Add("$" + r + "$1", cell);
            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();

            SetPrintCellFullBorder(ref cell, Color.Green);


            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "日";
            body.Add("$" + r + "$2", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "字";
            body.Add("$" + r + "$3", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "号";
            body.Add("$" + r + "$4", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "种类";
            body.Add("$" + r + "$5", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "号码";
            body.Add("$" + r + "$6", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "摘  要";
            body.Add("$" + r + "$7", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "借  方";
            cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线标题;
            body.Add("$" + r + "$8", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "贷  方";
            cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线标题;
            body.Add("$" + r + "$9", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = Core.Printer.PrintAssign.OK_FLAG;
            body.Add("$" + r + "$10", cell);

            cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
            SetPrintCellFullBorder(ref cell, Color.Green);
            cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
            cell.Value = "余  额";
            cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线标题;
            body.Add("$" + r + "$11", cell);
            r++;
            #endregion
            p.Mergers.Add(new UnvaryingSagacity.Core.Printer.Range(1, 1, 1, 2));
            p.Mergers.Add(new UnvaryingSagacity.Core.Printer.Range(1, 3, 1, 4));
            p.Mergers.Add(new UnvaryingSagacity.Core.Printer.Range(1, 5, 1, 6));
            p.Mergers.Add(new UnvaryingSagacity.Core.Printer.Range(1, 7, 2, 7));
            p.Mergers.Add(new UnvaryingSagacity.Core.Printer.Range(1, 10, 2, 10));
            p.TopFixedRows = 2;
            #endregion

            if (itemOfBank == null)
            {
                p.Rows.Add(new UnvaryingSagacity.Core.Printer.Row(25));
            }
        }

        void SetPrintDataByEntryExList(UnvaryingSagacity.Core.Printer.PrintData printData, EntryExList entries, ItemOfBank item, int pageNumber)
        {
            int y =  Core.General.FromString(entries[0].Date).Year;
            GetAccountPageHeader(printData, y, item,pageNumber);
            int r = 3;
            int c = 1;
            Core.Printer.Body body = printData.Body;
            foreach (EntryEx entry in entries)
            {
                printData.Rows.Add(new UnvaryingSagacity.Core.Printer.Row(25));
                Core.Printer.PrinterCell cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                DateTime dt = Core.General.FromString(entry.Date);
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                cell.Value = entry.Side == 0 ? "" : dt.Month.ToString();
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                cell.Value = entry.Side == 0 ? "" : dt.Day.ToString();
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                cell.Value = entry.Side == 0 ? "" : entry.VchType;
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                cell.Value = entry.Side == 0 ? "" : entry.VchNumber;
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                cell.Value = entry.Side == 0 ? "" : entry.ChequeType;
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                cell.Value = entry.Side == 0 ? "" : entry.Cheque;
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.LeftMiddle;
                cell.Value = entry.Digest;
                if (entry.Side == 0)
                {
                    cell.FontColor = Color.Red.ToArgb();
                    cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                }
                body.Add("$" + r + "$" + c, cell);
                c++;
                if (entry.Side == 0)
                {
                    cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                    SetPrintCellFullBorder(ref cell, Color.Green);
                    cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                    cell.Value = entry.JMoney;
                    cell.L_Color = Color.Green.ToArgb();
                    cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线;
                    body.Add("$" + r + "$" + c, cell);
                    c++;
                    cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                    SetPrintCellFullBorder(ref cell, Color.Green);
                    cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                    cell.Value = entry.DMoney;
                    cell.L_Color = Color.Green.ToArgb();
                    cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线;
                    body.Add("$" + r + "$" + c, cell);
                    c++;
                }
                else
                {
                    cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                    SetPrintCellFullBorder(ref cell, Color.Green);
                    cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                    cell.Value = entry.Side == 1 ? entry.Money : 0;
                    cell.L_Color = Color.Green.ToArgb();
                    cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线;
                    body.Add("$" + r + "$" + c, cell);
                    c++;
                    cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                    SetPrintCellFullBorder(ref cell, Color.Green);
                    cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                    cell.Value = entry.Side == -1 ? entry.Money : 0;
                    cell.L_Color = Color.Green.ToArgb();
                    cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线;
                    body.Add("$" + r + "$" + c, cell);
                    c++;
                }
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.CenterMiddle;
                cell.Value = "";
                body.Add("$" + r + "$" + c, cell);
                c++;
                cell = new UnvaryingSagacity.Core.Printer.PrinterCell();
                SetPrintCellFullBorder(ref cell, Color.Green);
                cell.Align = UnvaryingSagacity.Core.Printer.PrinterTextAlign.RightMiddle;
                cell.Value = entry.Bal;
                cell.L_Color = Color.Green.ToArgb();
                cell.Behave = UnvaryingSagacity.Core.Printer.PrinterCellBehave.金额线;
                body.Add("$" + r + "$" + c, cell);
                c = 1;
                r++;
            }
        }

        void SetPrintCellFullBorder(ref Core.Printer.PrinterCell cell)
        {
            SetPrintCellFullBorder(ref cell, Color.Black);
        }

        void SetPrintCellFullBorder(ref Core.Printer.PrinterCell cell, Color color)
        {
            cell.B_Style = UnvaryingSagacity.Core.Printer.PrinterBorderStyle.实线边框;
            cell.T_Style = UnvaryingSagacity.Core.Printer.PrinterBorderStyle.实线边框;
            cell.L_Style = UnvaryingSagacity.Core.Printer.PrinterBorderStyle.实线边框;
            cell.R_Style = UnvaryingSagacity.Core.Printer.PrinterBorderStyle.实线边框;
            cell.B_Color = color.ToArgb();
            cell.T_Color = color.ToArgb();
            cell.L_Color = color.ToArgb();
            cell.R_Color = color.ToArgb();
        }

    }
}
