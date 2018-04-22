using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnvaryingSagacity.Core
{
    partial class UISampleObjectEditor : Form
    {

        public SampleObjectEditor ParentObject { get; set; }

        public UISampleObjectEditor()
        {
            InitializeComponent();
            this.button1.Click += new EventHandler(button1_Click);
            this.button2.Click += new EventHandler(button2_Click); 
            this.Shown += new EventHandler(UISampleObjectEditor_Shown);
        }

        void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void button1_Click(object sender, EventArgs e)
        {
            SampleClass s1 = new SampleClass();
            s1.ID = textBox1.Text;
            s1.Name = textBoxImeOnHalf1.Text;
            if (ParentObject.BeforeSave != null)
            {

                SampleClass s2 = default(SampleClass);
                if (ParentObject.CurrentObject != null)
                {
                    s2 = new SampleClass();
                    ParentObject.CurrentObject.CopyTo(s2);
                }
                bool isclosed = true;
                if (ParentObject.BeforeSave(s1, s2,ref isclosed))
                {
                    if (isclosed)
                    {
                        ParentObject.SetObject(s1.ID, s1.Name);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        textBox1.Text = "";
                        textBoxImeOnHalf1.Text = "";
                        textBox1.Focus(); 
                    }
                }

            }
            else
            {
                s1.CopyTo(ParentObject.NewObject);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

        }

        void UISampleObjectEditor_Shown(object sender, EventArgs e)
        {
            if (ParentObject.CurrentObject != null)
            {
                textBox1.Text = ParentObject.CurrentObject.ID;
                textBoxImeOnHalf1.Text = ParentObject.CurrentObject.Name; 
            }
        }

    }
}
