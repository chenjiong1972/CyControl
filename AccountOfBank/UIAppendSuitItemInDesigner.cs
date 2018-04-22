using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.AccountOfBank
{
    public partial class UIAppendSuitItemInDesigner : Form
    {
        public string[] ExistsItems { get; set; }
        public string[] BulidInItems { get; set; }
        public string ResultItems { get; set; }

        public UIAppendSuitItemInDesigner()
        {
            InitializeComponent();
            ExistsItems = new string[0];
            this.Shown += new EventHandler(UIAppendSuitItemInDesigner_Shown);
        }

        void UIAppendSuitItemInDesigner_Shown(object sender, EventArgs e)
        {
            if (ExistsItems.Length > 0)
            {
                foreach (string s in BulidInItems)
                {
                    if (Array.IndexOf(ExistsItems, s) < 0)
                    {
                        checkedListBox1.Items.Add(s);
                    }
                }
            }
            else
            {
                checkedListBox1.Items.AddRange(BulidInItems);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Core.XmlExplorer xml = new UnvaryingSagacity.Core.XmlExplorer();
            xml.XmlDoc.LoadXml("<root></root>");
            System.Xml.XmlElement root = xml.XmlDoc.DocumentElement;
            
            if (radioButton1.Checked)
            {
                System.Xml.XmlElement el = xml.XmlDoc.CreateElement("item");
                el.InnerText = textBox2.Text;
                System.Xml.XmlAttribute xmlAttr = xml.XmlDoc.CreateAttribute("name");
                xmlAttr.Value = textBox1.Text;
                el.Attributes.Append(xmlAttr);
                root.AppendChild(el);
            }
            else
            {
                if (checkedListBox1.CheckedItems.Count <= 0)
                {
                    MessageBox.Show(this, "没有选择要新增的封面内容", "新增封面内容", MessageBoxButtons.OK, MessageBoxIcon.Information);   
                    return;
                }
                foreach (object obj in checkedListBox1.CheckedItems)
                {
                    System.Xml.XmlElement el = xml.XmlDoc.CreateElement("item");
                    el.InnerText = "";
                    System.Xml.XmlAttribute xmlAttr = xml.XmlDoc.CreateAttribute("name");
                    xmlAttr.Value = obj.ToString();
                    el.Attributes.Append(xmlAttr);
                    root.AppendChild(el);
                }
                
            }
            ResultItems = xml.XmlDoc.InnerXml;
            this.DialogResult = DialogResult.OK ;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = radioButton1.Checked;
            textBox2.Enabled = radioButton1.Checked;
            checkedListBox1.Enabled = radioButton2.Checked; 
        }
    }
}
