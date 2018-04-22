using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.AccountOfBank
{
    public partial class UIUserManager : Form
    {
        const string KEY_TOP_USER = "User";
        const string KEY_TOP_USER_STOP = "StopUser";
        const string KEY_TOP_GROUP = "Group";
        #region controls
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private ToolStripLabel toolStripLabel1;
        private ToolStripButton toolStripButton1;
        private ToolStripButton toolStripButton2;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButton3;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButton4;
        private ToolStripButton toolStripButton5;
        private System.Windows.Forms.Panel panel3;
        private SplitContainer splitContainer2;
        private Panel panel5;
        private Panel panel4;
        private Label label1;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton toolStripButton8;
        private ToolStripButton toolStripButton9;
        private ToolStripButton toolStripButton10;
        #endregion
        private TreeView treeView2;
        private bool _changed = false;
        private Environment _e;
        private UserAndRight.Right _right = new UserAndRight.Right();

        internal Environment CurrentEnvironment { set { _e = value; } }

        public UIUserManager()
        {
            InitializeComponent();
            this.Text = "用户和权限管理";
            this.Shown += new EventHandler(UIUserManager_Shown);
            this.FormClosing += new FormClosingEventHandler(UIUserManager_FormClosing);
            this.treeView1.NodeMouseClick += new TreeNodeMouseClickEventHandler(treeView1_NodeMouseClick);
            //this.listView1.ItemChecked += new ItemCheckedEventHandler(listView1_ItemChecked);
            this.treeView2.AfterCheck += new TreeViewEventHandler(treeView2_AfterCheck);
            this.btnAllRight.Click += new EventHandler(btnAllRight_Click);
            #region 工具按钮事件
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
        }

        void btnAllRight_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Tag is UserAndRight.User)
                {
                    if (!(treeView1.SelectedNode.Tag  as UserAndRight.User).IsSupper)
                    {
                        (treeView1.SelectedNode.Tag  as UserAndRight.User).IsSupper = true;
                        Changed();
                        SetNodeCheckedByRightRuler(treeView1.SelectedNode.Tag  as UserAndRight.User);
                    }
                }
            }
        }

        void UIUserManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (RightStateChanged)
            {
                this.Cursor = Cursors.WaitCursor; 
                Users us=new Users ();
                foreach (TreeNode node in treeView1.Nodes[KEY_TOP_USER].Nodes)
                {
                    if(node.Tag is UserAndRight.User)
                    us.Add (node.Tag as UserAndRight.User );
                }
                _e.SetUsersRight(us);
                this.Cursor = Cursors.Default;
            }
        }

        public bool RightStateChanged { get { return _changed; } }

        void button_Click(object sender, EventArgs e)
        {
            switch (((ToolStripItem)sender).Text)
            {
                case "新增用户":
                    CreateUser();
                    break;
                case "删除用户":
                    DeleteUser();
                    break;
                case "换用户图片":
                    UpdateImage();
                    break;
                case "换用户密码":
                    UpdatePass();
                    break;
                case "关闭":
                    this.Close();
                    break;
                default:
                    break;
            }
        }

        void treeView2_AfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (node != null)
            {
                UserAndRight.RightRuler ruler = new UserAndRight.RightRuler();
                if (node.FullPath.IndexOf("用户\\") == 0)
                {
                    ruler = (node.Tag as UserAndRight.User).Ruler;
                }
                else
                    return;


                string id = (e.Node.Tag as UserAndRight.Right).ID;
                UserAndRight.RightRuler r = ruler.FindRight(id, true);
                if (r != null)
                {
                    r.RightState = (e.Node.Checked ? UserAndRight.RightState.完全 : UserAndRight.RightState.禁止);
                    if (e.Node.Checked)
                    {
                        this.treeView2.AfterCheck -= treeView2_AfterCheck;
                        SetParentNodeChecked(e.Node);
                        this.treeView2.AfterCheck += new TreeViewEventHandler(treeView2_AfterCheck);
                    }
                    else if ((node.Tag as UserAndRight.User).IsSupper)
                        (node.Tag as UserAndRight.User).IsSupper = false;
                    Changed();
                }

            }
        }

        void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.FullPath.IndexOf("用户\\") == 0)
            {
                treeView1.Enabled = false;
                UserAndRight.User u;
                if (e.Node.Tag == null)
                {
                    u = new UserAndRight.User();
                    u.Name = e.Node.Text;
                    _e.GetUserInfo(u);
                    e.Node.Tag = u;
                }
                if (e.Node.Tag != null)
                {
                    u = (e.Node.Tag as UserAndRight.User);
                    label1.Text = e.Node.Text + " 的权限";
                    SetNodeCheckedByRightRuler(u);
                    this.treeView2.Enabled = true;
                    btnAllRight.Enabled = true;
                }
                else
                {
                    label1.Text = "";
                    treeView2.Enabled = false;
                    btnAllRight.Enabled = false;
                }
                treeView1.Enabled = true;
            }
            else
            {
                treeView2.Enabled = false;
                return;
            }
        }

        void SetNodeCheckedByRightRuler(UserAndRight.User u )
        {
            this.treeView2.AfterCheck -= treeView2_AfterCheck;
            this.Cursor = Cursors.WaitCursor; 
            foreach (TreeNode node in treeView2.Nodes)
            {
                if (u.IsSupper)
                {
                    node.Checked = true;
                    SetNodeChildCheckedByRightRuler(node, true);
                }
                else
                {
                    UserAndRight.RightRuler r = u.Ruler.FindRight(node.Name, true) as UserAndRight.RightRuler;
                    if (r != null)
                    {
                        node.Checked = r.RightState == UserAndRight.RightState.完全 ? true : false;
                        if (node.Checked)
                            SetParentNodeChecked(node);
                        SetNodeChildCheckedByRightRuler(node, u.Ruler);
                    }
                    else
                    {
                        node.Checked = false;
                        SetNodeChildCheckedByRightRuler(node, u.Ruler);
                    }
                }
            }
            this.treeView2.AfterCheck += new TreeViewEventHandler(treeView2_AfterCheck);
            this.Cursor = Cursors.Default;
        }

        void SetNodeChildCheckedByRightRuler(TreeNode node, UserAndRight.RightRuler r)
        {
            foreach (TreeNode childNode in node.Nodes)
            {

                UserAndRight.RightRuler right = r.FindRight(childNode.Name, true) as UserAndRight.RightRuler;
                if (right != null)
                {
                    childNode.Checked = right.RightState == UserAndRight.RightState.完全 ? true : false;
                    SetNodeChildCheckedByRightRuler(childNode, r);
                    if (childNode.Checked)
                        SetParentNodeChecked(childNode);
                }
                else
                {
                    childNode.Checked = false;
                    SetNodeChildCheckedByRightRuler(childNode, r);
                }
            }
        }

        void SetNodeChildCheckedByRightRuler(TreeNode node, bool isSupper)
        {
            foreach (TreeNode childNode in node.Nodes)
            {
                childNode.Checked = isSupper;
                SetNodeChildCheckedByRightRuler(childNode, isSupper);
            }
        }

        void UIUserManager_Shown(object sender, EventArgs e)
        {
            _right = _e.GetCurrentRights();
            this.treeView1.Nodes.Add(KEY_TOP_USER, "用户").ImageKey ="all";

            TreeNode node = treeView1.Nodes[KEY_TOP_USER];
            string[] ss = _e.GetSettingAttribute("user", "name", true);
            foreach (string s in ss)
            {
                TreeNode newNode= node.Nodes.Add(s);
                newNode.ImageKey = "itemOfBank";
                newNode.SelectedImageKey = newNode.ImageKey;

            }
            node.Expand();
            
            foreach (UserAndRight.Right r in _right.Nodes)
            {
                node = treeView2.Nodes.Add(r.ID, r.Name, -1);
                node.Tag = r;
                node.ImageKey = "itemOfBank";
                node.SelectedImageKey = node.ImageKey;
                node.ToolTipText = r.Description;
                if (r.Nodes.Count > 0)
                {
                    foreach (UserAndRight.Right rChild in r.Nodes)
                    {
                        TreeNode nodeChild = node.Nodes.Add(rChild.ID,rChild.Name, -1);
                        nodeChild.Tag = rChild;
                        nodeChild.ImageKey = "itemOfBank"; 
                        nodeChild.ToolTipText = rChild.Description;
                        nodeChild.SelectedImageKey = nodeChild.ImageKey;
                        if (rChild.Nodes.Count > 0)
                        {
                            foreach (UserAndRight.Right rChildChild in rChild.Nodes)
                            {
                                TreeNode nodeChildChild = nodeChild.Nodes.Add(rChildChild.ID,  rChildChild.Name, -1);
                                nodeChildChild.Tag = rChildChild;
                                nodeChildChild.ImageKey = "itemOfBank";
                                nodeChildChild.SelectedImageKey = nodeChildChild.ImageKey;
                                nodeChildChild.ToolTipText = rChildChild.Description;
                            }
                            nodeChild.Expand();
                        }
                    }
                    node.Expand();
                }
            }
            treeView2.Enabled = false;
        }

        /// <summary>
        /// 请先取消treeView2_AfterCheck事件
        /// </summary>
        /// <param name="node"></param>
        void SetParentNodeChecked(TreeNode node)
        {
            TreeNode n = node.Parent ;
            while (n != null)
            {
                n.Checked = true;
                n = n.Parent;
            }
        }

        void CreateUser()
        {
            string name = "";
            Core.InputBox inputbox = new Core.InputBox("新建用户", "请录入新用户的名称, 不能为空或重复.", 40, name);
            inputbox.ShowDialog(this);
            if (inputbox.Result != null)
            {
                name = inputbox.Result as string;
                if (name.Length > 0)
                {
                    UserAndRight.User u = _e.CreateUser(name);
                    if (u != null)
                    {
                        _right.ToRightRuler(u.Ruler);
                        TreeNode node = treeView1.Nodes[KEY_TOP_USER];
                        TreeNode newNode = node.Nodes.Add(u.Name);
                        newNode.Tag = u;
                        newNode.ImageKey = "itemOfBank";
                        newNode.SelectedImageKey = newNode.ImageKey;
                    }
                }
            }
            inputbox = null;
        }

        void DeleteUser()
        {
            TreeNode node = treeView1.SelectedNode;
            if (node != null)
            {
                if (node.FullPath.IndexOf("用户\\") == 0)
                {
                    string name = (node.Tag as UserAndRight.User).Name;
                    if (_e.DeleteUser(name))
                    {
                        treeView1.Nodes[KEY_TOP_USER].Nodes.Remove(node);
                    }
                }
            }
        }

        void UpdateImage()
        {
            TreeNode node = treeView1.SelectedNode;
            if (node != null)
            {
                if (node.FullPath.IndexOf("用户\\") == 0)
                {
                    OpenFileDialog f = new OpenFileDialog();
                    f.Filter = "(PNG图片文件)|*.png";
                    if (f.ShowDialog(this) == DialogResult.OK)
                    {
                        try
                        {
                            System.Drawing.Image image = System.Drawing.Image.FromFile(f.FileName);
                            //if (_userDataHandle.UpdateImage((node.Tag as UserAndRight.User), image))
                            //{
                            //    //treeView1.Nodes[KEY_TOP_GROUP].Nodes.Remove(node);
                            //}
                        }
                        catch { }
                    }
                }
            }
        }

        void UpdatePass()
        {
            TreeNode node = treeView1.SelectedNode;
            if (node != null)
            {
                if (node.FullPath.IndexOf("用户\\") == 0)
                {
                    Core.InputBox inputbox = new Core.InputBox("设置密码", "请输入用户[" + (node.Tag as UserAndRight.User).Name + "]的新密码,不能超过20个或少于6字符.", 20, "");
                    inputbox.ShowDialog(this);
                    string s = inputbox.Result as string;
                    inputbox = null;
                    if (s != null)
                    {
                        if (s.Length >= 6)
                        {
                            if (!_e.UpdateUser(node.Tag as UserAndRight.User, "pwd", s))
                            {
                                MessageBox.Show("修改密码失败, 请继续使用原密码.", "检查并修改密码", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            MessageBox.Show("密码长度不够, 修改密码失败. 请继续使用原密码.", "检查并修改密码", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        void Changed()
        {
            if (!_changed)
                _changed = true;
        }

    }
}

