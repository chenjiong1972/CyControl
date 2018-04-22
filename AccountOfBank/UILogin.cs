using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.AccountOfBank
{
    public partial class UILogin : Form
    {
        int count = 1;

        public UILogin()
        {
            InitializeComponent();
            this.Text =Application.ProductName +  " - 登录";
            this.Shown += new EventHandler(UILogin_Shown);
            button1.Click += new EventHandler(button1_Click);

        }

        internal Environment CurrentEnvironment { get; set; }

        void UILogin_Shown(object sender, EventArgs e)
        {
            if (CurrentEnvironment != null)
            {
                string[] ss = CurrentEnvironment.GetSettingAttribute("user", "name");
                foreach (string s in ss)
                {
                    comboBox1.Items.Add(s); 
                }
                string last= CurrentEnvironment.GetLastLoginUser();
                if (comboBox1.Items.Contains(last))
                {
                    comboBox1.SelectedItem = last; 
                }
                this.textBox2.Focus(); 
            }
        }

        void button1_Click(object sender, EventArgs e)
        {
            string u=comboBox1.SelectedItem.ToString ();
            int result = CurrentEnvironment.CheckUser(u  , textBox2.Text);
            if (result == 2)
            {
                if (count >= 4)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
                else
                {
                    label3.Text = "(密码错误,注意大小写开关状态)";
                    textBox2.Text = "";
                    this.textBox2.Focus(); 
                    label3.Visible = true;
                    count++;
                }

            }
            else if (result == 0)
            {
                CurrentEnvironment.SetCurrentUser(u);
                CurrentEnvironment.SetLastLogin(u);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else if (result == 3)
            {
                label4.Text = "(用户名不存在)";
                //this.textBox1.Focus(); 
                label4.Visible  = true ;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
    }
}
