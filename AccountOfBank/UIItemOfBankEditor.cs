using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.AccountOfBank
{
    public partial class UIItemOfBankEditor : Form
    {
        internal ItemOfBank CurrentItemOfBank { get; set; }
        internal Environment CurrentEnvironment { get; set; }

        public UIItemOfBankEditor()
        {
            InitializeComponent();
            this.Text = InternalBaseObject.账户.ToString();
            this.Shown += new EventHandler(UIItemOfBankEditor_Shown);
        }

        void UIItemOfBankEditor_Shown(object sender, EventArgs e)
        {
            RefreshBanks();
            if (CurrentItemOfBank != null)
            {
                DisplayItemOfBank();
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            CurrentItemOfBank = default(ItemOfBank);
            this.DialogResult = DialogResult.Cancel; 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (CurrentItemOfBank == null)
            {
                CurrentItemOfBank = new ItemOfBank();
            }
            CurrentItemOfBank.Name = textBox1.Text;
            CurrentItemOfBank.ID = textBox2.Text;
            CurrentItemOfBank.Description = textBox4.Text;
            CurrentItemOfBank.OfBankName = comboBox1.Text;
            CurrentItemOfBank.StartBal = double.Parse(cyEditor1.Text);
            this.DialogResult = DialogResult.OK; 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UIBaseInfoManager ui = new UIBaseInfoManager();
            ui.CurrentEnvironment = CurrentEnvironment;
            ui.BaseObjectType = InternalBaseObject.银行;
            ui.ShowDialog(this);
            if (ui.BaseObjectChanged)
            {
                RefreshBanks();
            }
        }

        void RefreshBanks()
        {
            comboBox1.Items.Clear();
            string[] ss = CurrentEnvironment.GetSetting("bank");
            foreach (string s in ss)
            {
                comboBox1.Items.Add(s);
            }
        }

        void DisplayItemOfBank()
        {
            textBox1.Text = CurrentItemOfBank.Name;
            textBox2.Text = CurrentItemOfBank.ID ;
            if (comboBox1.Items.IndexOf(CurrentItemOfBank.OfBankName) >= 0)
                comboBox1.SelectedItem  = CurrentItemOfBank.OfBankName;
            cyEditor1.Text = CurrentItemOfBank.StartBal.ToString ("0.00") ;
            textBox4.Text = CurrentItemOfBank.Description;
        }
    }
}
