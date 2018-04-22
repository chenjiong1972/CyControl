using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.AccountOfBank
{
    public partial class UIBaseInfoManager : Form
    {
        private Environment _e;
        private bool changed = false;
        internal InternalBaseObject BaseObjectType { get; set; }
        internal Environment CurrentEnvironment { set { _e = value; } }
        internal bool BaseObjectChanged { get { return changed; } }

        public UIBaseInfoManager()
        {
            InitializeComponent();
            this.Shown += new EventHandler(UIBaseInfoManager_Shown);
        }

        void InitStyleBySettle()
        {
            ColumnHeader ch = new ColumnHeader();
            ch.Width = this.listView1.ClientSize.Width - 10;
            ch.Text = "名称"; 
            this.listView1.Columns.Add(ch);
            string[] ss;
            string name;
            if (BaseObjectType == InternalBaseObject.结算方式)
                name = "settle";
            else
                name ="bank";
            ss = _e.GetSetting(name);

            foreach (string s in ss)
            {
                listView1.Items.Add(s).ImageKey = name;
            }
        }

        void InitStyleByAccount()
        {
            toolStripButton6.Enabled = false; 
            //不需要引入引出,由备份和还原替代
            //toolStripButton1.Visible = true;
            //toolStripButton2.Visible = true;
            toolStripButton3.Visible = true;
            toolStripButton4.Visible = true;
            toolStripSeparator2.Visible = true;
            ColumnHeader[] ch = new ColumnHeader[5];
            int i = 0;
            ch[i] = new ColumnHeader();
            ch[i].Width = 120;
            ch[i].Text = "名称";
            i++;
            ch[i] = new ColumnHeader(); 
            ch[i].Width = 50;
            ch[i].Text = "编号";
            i++;
            ch[i] = new ColumnHeader();
            ch[i].Width = 80;
            ch[i].Text = "状态"; 
            i++;
            ch[i] = new ColumnHeader();
            ch[i].Width = 200;
            ch[i].Text = "描述";
            i++;
            ch[i] = new ColumnHeader();
            ch[i].Width = 500;
            ch[i].Text = "数据库";
            i++;
            this.listView1.Columns.AddRange(ch);
            Accounts acts = _e.GetAccountList();
            foreach (Account act in acts)
            {
                ListViewItem item = listView1.Items.Add(act.Name ,-1); //  new ListViewItem(act.Name,"all",);
                //item.Text = act.Name;
                item.ImageKey  = "all";
                item.SubItems.Add(act.ID);
                if (System.IO.File.Exists(act.FullPath))
                {
                    item.SubItems.Add("正常");
                }
                else
                {
                    item.SubItems.Add("数据库丢失");
                }
                item.SubItems.Add(act.Description);
                item.SubItems.Add(act.FullPath);
                //listView1.Items.Add(item);  
            }
        }

        void InitStyleByItemOfBank()
        {
            ColumnHeader[] ch = new ColumnHeader[4];
            int i = 0;
            ch[i] = new ColumnHeader();
            ch[i].Width = 120;
            ch[i].Text = "名称";
            i++;
            ch[i] = new ColumnHeader();
            ch[i].Width = 150;
            ch[i].Text = "账号";
            i++;
            ch[i] = new ColumnHeader();
            ch[i].Width = 120;
            ch[i].Text = "银行名称";
            i++;
            ch[i] = new ColumnHeader();
            ch[i].Width = 300;
            ch[i].Text = "描述";
            i++;
            this.listView1.Columns.AddRange(ch);
            DataProvider dp = new DataProvider(_e.CurrentAccount.FullPath);
            ItemOfBankCollection items = dp.GetItemOfBankList();
            foreach (ItemOfBank it in items)
            {
                ListViewItem item = new ListViewItem();
                item.Text = it.Name;
                item.ImageKey = "itemOfBank";
                item.SubItems.Add(it.ID);
                item.SubItems.Add(it.OfBankName );
                item.SubItems.Add(it.Description);
                listView1.Items.Add(item);  
            }
        }

        void InitStyleByBaseObject()
        {
            switch (BaseObjectType)
            {
                case InternalBaseObject.银行 :
                case InternalBaseObject.结算方式 :
                    InitStyleBySettle();
                    break;
                case InternalBaseObject.账户 :
                    InitStyleByItemOfBank();
                    break;
                case InternalBaseObject.账套 :
                    InitStyleByAccount();
                    break;
                default :
                    break;
            }
        }

        void AppendBySettle()
        {
            Core.InputBox input = new UnvaryingSagacity.Core.InputBox("新增" + BaseObjectType.ToString(), "请输入新的"+BaseObjectType.ToString ()+", 不能为空", 15, "");
            if (input.ShowDialog(this)==DialogResult.OK  )
            {
                string s=input.Result.ToString();
                if (_e.AppendSetting(s, BaseObjectType == InternalBaseObject.结算方式 ? "settles" : "banks", BaseObjectType == InternalBaseObject.结算方式 ? "settle" : "bank"))
                {
                    listView1.Items.Add(s).ImageKey = BaseObjectType == InternalBaseObject.结算方式 ? "settle" : "bank";
                }
            }
        }

        void AppendByItemOfBank()
        {
            UIItemOfBankEditor ui = new UIItemOfBankEditor();
            ui.CurrentEnvironment = _e;
            if (ui.ShowDialog(this) == DialogResult.OK)
            {
                ItemOfBank it = new ItemOfBank();
                ui.CurrentItemOfBank.CopyTo(it);
                DataProvider dp = new DataProvider(_e.CurrentAccount.FullPath);
                if (dp.AppendItemOfBank(it))
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = it.Name;
                    item.ImageKey = "itemOfBank";
                    item.SubItems.Add(it.ID);
                    item.SubItems.Add(it.OfBankName );
                    item.SubItems.Add(it.Description);
                    listView1.Items.Add(item);
                }
            }
        }
        
        void AppendByAccount()
        {
            UIAccountEditor ui = new UIAccountEditor();
            if (ui.ShowDialog(this) == DialogResult.OK)
            {
                Account act = new Account();
                ui.CurrentAccount.CopyTo(act);
                if (_e.AppendAccount(act))
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = act.Name;
                    item.ImageKey = "all";
                    item.SubItems.Add(act.ID);
                    if (System.IO.File.Exists(act.FullPath))
                    {
                        item.SubItems.Add("正常");
                    }
                    else
                    {
                        item.SubItems.Add("数据库丢失");
                    }
                    item.SubItems.Add(act.Description);
                    item.SubItems.Add(act.FullPath);
                    listView1.Items.Add(item);
                }
            }
        }

        void AppendByBaseObject()
        {
            switch (BaseObjectType)
            {
                case InternalBaseObject.银行:
                case InternalBaseObject.结算方式:
                    AppendBySettle();
                    break;
                case InternalBaseObject.账户:
                    AppendByItemOfBank();
                    break;
                case InternalBaseObject.账套:
                    AppendByAccount();
                    break;
                default:
                    break;
            }
        }

        void RenameBySettle()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem item = listView1.SelectedItems[0]; 
                Core.InputBox input = new UnvaryingSagacity.Core.InputBox("重命名"+BaseObjectType.ToString () , "请输入新的"+BaseObjectType.ToString ()+"名称, 不能为空", 15, item.Text );
                if (input.ShowDialog(this) == DialogResult.OK)
                {
                    string s = input.Result.ToString();
                    if (s != item.Text)
                    {
                        if (_e.UpadteSetting (item.Text ,s,BaseObjectType==InternalBaseObject.结算方式 ?"settle":"bank"   ))
                        {
                            item.Text = s;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 废弃, 
        /// </summary>
        void RenameByAccount()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem item = listView1.SelectedItems[0];
                UIAccountEditor ui = new UIAccountEditor();
                if (ui.ShowDialog(this) == DialogResult.OK)
                {
                    Account act = new Account();
                    act.Name = item.Text;
                    act.ID = item.SubItems[0].Text;
                    act.Description = item.SubItems[1].Text;
                    act.FullPath = item.SubItems[3].Text;
                    act.CopyTo(ui.CurrentAccount);
                    if (ui.ShowDialog(this) == DialogResult.OK)
                    {
                        ui.CurrentAccount.CopyTo(act);
                        if (_e.UpadteAccount(item.Text, act))
                        {
                            item.Text = act.Name;
                            item.SubItems.Clear();
                            item.SubItems.Add(act.ID);
                            if (System.IO.File.Exists(act.FullPath))
                            {
                                item.SubItems.Add("正常");
                            }
                            else
                            {
                                item.SubItems.Add("数据库丢失");
                            }
                            item.SubItems.Add(act.Description);
                            item.SubItems.Add(act.FullPath);
                        }
                    }
                }
            }
        }

        void RenameByItemOfBank()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                UIItemOfBankEditor ui = new UIItemOfBankEditor();
                ListViewItem item = listView1.SelectedItems[0];
                DataProvider dp = new DataProvider(_e.CurrentAccount.FullPath);
                ItemOfBank it = dp.GetItemOfBank(item.SubItems[1].Text);
                ui.CurrentItemOfBank = new ItemOfBank();
                it.CopyTo(ui.CurrentItemOfBank);
                ui.CurrentEnvironment = _e;
                if (ui.ShowDialog(this) == DialogResult.OK)
                {
                    ui.CurrentItemOfBank.CopyTo(it);
                    if (dp.UpdateItemOfBank(item.SubItems[1].Text, it))
                    {
                        item.SubItems.Clear();
                        item.Text = it.Name;
                        item.SubItems.Add(it.ID);
                        item.SubItems.Add(it.OfBankName);
                        item.SubItems.Add(it.Description);
                    }
                }
            }
        }

        void RenameByBaseObject()
        {
            switch (BaseObjectType)
            {
                case InternalBaseObject.银行 : 
                case InternalBaseObject.结算方式:
                    RenameBySettle();
                    break;
                case InternalBaseObject.账户:
                    RenameByItemOfBank();
                    break;
                case InternalBaseObject.账套:
                    RenameByAccount();
                    break;
                default:
                    break;
            }
        }

        void DeleteBySettle()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(this, "您确定要删除选择的项目吗?", "删除"+BaseObjectType.ToString()  , MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    foreach (ListViewItem item in listView1.SelectedItems)
                    {
                        if(_e.DeleteSetting(item.Text,BaseObjectType==InternalBaseObject.结算方式 ?"settle":"bank"   ))
                        {
                            listView1.Items.Remove(item);
                        }
                    }
                }
            }
        }

        void DeleteByAccount()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                DialogResult r = MessageBox.Show(this, "您确定要删除选择的项目, 并同时删除数据库文件吗?\n\n--是: 删除项目时同时删除数据库文件.\n--否: 删除项目, 但保留数据库文件.\n--取消: 取消本次操作." ,"删除"+ BaseObjectType.ToString(), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                bool b=(r==DialogResult.Yes ?true :false);
                if ( r== DialogResult.Yes|| r==DialogResult.No  )
                {
                    foreach (ListViewItem item in listView1.SelectedItems)
                    {
                        if (_e.DeleteAccount(item.Text,b))
                        {
                            listView1.Items.Remove(item);
                        }
                    }
                }
            }
        }

        void DeleteByItemOfBank()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(this, "您确定要删除选择的账户及其日记账吗?", "删除" + BaseObjectType.ToString(), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    DataProvider dp = new DataProvider(_e.CurrentAccount.FullPath);
                    foreach (ListViewItem item in listView1.SelectedItems)
                    {
                        if (dp.DeleteRecord(DataProvider.TABLE_ITEMOFBANKS, "nm='" + item.Text + "' AND id='" + item.SubItems[1].Text + "'"))
                        {
                            listView1.Items.Remove(item);
                            dp.DeleteRecord("id='" + item.SubItems[1].Text + "'");
                        }
                    }
                }
            }
        }
        void DeleteByBaseObject()
        {
            this.Text ="基础资料 - "+ BaseObjectType.ToString();
            switch (BaseObjectType)
            {
                case InternalBaseObject.银行:
                case InternalBaseObject.结算方式:
                    DeleteBySettle();
                    break;
                case InternalBaseObject.账户:
                    DeleteByItemOfBank();
                    break;
                case InternalBaseObject.账套:
                    DeleteByAccount();
                    break;
                default:
                    break;
            }
        }

        void UIBaseInfoManager_Shown(object sender, EventArgs e)
        {
            this.Text = BaseObjectType.ToString(); 
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
                }
            }
            #endregion
            InitStyleByBaseObject();
        }


        void button_Click(object sender, EventArgs e)
        {
            switch ((sender as ToolStripItem ).Text)
            {
                case "新增...":
                    AppendByBaseObject();
                    changed = true;
                    break;
                case "重命名":
                    RenameByBaseObject ();
                    changed = true;
                    break;
                case "删除":
                    DeleteByBaseObject ();
                    changed = true;
                    break;
                case "备份":
                    if(listView1.SelectedItems.Count >0)
                        BackupAccount(listView1.SelectedItems[0].Text); ;
                    break;
                case "还原...":
                    if (listView1.SelectedItems.Count > 0)
                        RestoreAccount(listView1.SelectedItems[0].Text); ;
                    break;
                default :
                    break;
            }
        }

        void RestoreAccount(string name)
        {
            if (_e.CurrentUser.GetRightState("0105") != UserAndRight.RightState.完全)
            {
                MessageBox.Show(this, UIMain.RIGHT_ERROR, "检查权限", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (_e.CurrentAccount != null)
            {
                if (name == _e.CurrentAccount.Name)
                {
                    MessageBox.Show(this, "对不起, 账套正在使用不能被还原. 请先打开其他账套或注销后再进行还原.", "还原账套", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            Account act = _e.GetAccount(name);
            if (act != null)
            {
                UIRestoreDatabase ui = new UIRestoreDatabase();
                ui.CurrentEnvironment = _e;
                ui.RestoreAccount = act;
                ui.ShowDialog(this);
            }
        }

        void BackupAccount(string name)
        {
            if (_e.CurrentUser.GetRightState("0104") != UserAndRight.RightState.完全)
            {
                MessageBox.Show(this, UIMain.RIGHT_ERROR, "检查权限", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Account act = _e.GetAccount(name);
            if (act != null)
            {
                SaveFileDialog sf = new SaveFileDialog();
                sf.AddExtension = false;
                sf.CheckPathExists = true;
                sf.OverwritePrompt = true;
                sf.ValidateNames = true;
                sf.FileName ="日记账备份_"+ act.ID + "_" + DateTime.Today.ToString("yyyyMMdd");
                if (sf.ShowDialog(this) == DialogResult.OK)
                {
                    string fileName = sf.FileName;
                    if (System.IO.File.Exists(fileName))
                    {
                        //if (MessageBox.Show(this, "文件：\n\n" + fileName + "\n\n已经存在，您确定要覆盖吗？", "账套备份", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        //{
                        //    return;
                        //}
                        System.IO.File.Delete(fileName);
                    }
                    this.Cursor = Cursors.WaitCursor; 
                    Core.ToolTip tip = new UnvaryingSagacity.Core.ToolTip();
                    tip.ShowPrompt(this, "正在备份账套["+act.Name +"] 到 文件:"+fileName +"\n\n耗费时间由数据量决定, 请等待......");
                    try
                    {
                        DataProvider dpSrc = new DataProvider(act.FullPath);
                        DataProvider dpDst = new DataProvider(fileName);
                        ///清理数据库建好后的默认数据

                        foreach (DataTable dt in dpDst.dataSet.Tables)
                        {
                            DataTable dstDt = dpDst.dataSet.Tables[dt.TableName];
                            DataRow[] drs = dpDst.GetDataRows(dt.TableName, "", "");
                            dstDt.Rows.Clear();
                            dstDt.AcceptChanges();
                        }
                        foreach (DataTable dt in dpSrc.dataSet.Tables)
                        {
                            DataTable dstDt = dpDst.dataSet.Tables[dt.TableName];
                            DataRow[] drs = dpSrc.GetDataRows(dt.TableName, "", "");
                            bool b = false;
                            foreach (DataRow dr in drs)
                            {

                                DataRow dstDr = dstDt.NewRow();
                                object[] rItemArray = new object[dstDr.ItemArray.Length];
                                for (int i = 0; i < dstDt.Columns.Count; i++)
                                {
                                    rItemArray[i] = dr[dstDt.Columns[i].ColumnName];
                                }
                                dstDr.ItemArray = rItemArray;
                                dstDt.Rows.Add(dstDr);
                                if (!b)
                                    b = true;
                            }
                            if (b)
                            {
                                dstDt.AcceptChanges();
                            }
                        }
                        DateTime last = DateTime.Today;
                        dpSrc.LastBackupDateTime = last;
                        dpDst.LastBackupDateTime = last;
                        dpDst.SetConfig(DataProvider.CONFIG_备份账套名称, act.Name);
                        dpDst.SetConfig(DataProvider.CONFIG_备份账套编号, act.ID);
                        dpDst.SetConfig(DataProvider.CONFIG_备份创建人, _e.CurrentUser.Name);
                        tip.ShowPrompt(this, "正在检查备份文件:" + fileName + "\n\n, 请稍等......");
                        dpDst.Refresh(true);
                        dpDst.Closed();
                        System.IO.FileInfo fiSrc = new System.IO.FileInfo(act.FullPath);
                        System.IO.FileInfo fiDst = new System.IO.FileInfo(fileName);
                        string prompt = "账套[" + act.Name + "]已经成功备份到文件:\n\n" + fileName;
                        if (fiDst.Length < fiSrc.Length)
                        {
                            prompt = "备份文件的大小不正确，请删除下面的文件后重新备份:\n\n" + fileName;
                        }
                        tip.Hide();
                        this.Cursor = Cursors.Default;
                        MessageBox.Show(this, prompt, "账套备份", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        tip.Hide();
                        this.Cursor = Cursors.Default;
                        MessageBox.Show(this, ex.Message, "账套备份", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }
        }
    }
}
