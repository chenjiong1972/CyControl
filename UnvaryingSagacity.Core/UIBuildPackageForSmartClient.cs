using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UnvaryingSagacity.Core;

namespace UnvaryingSagacity.Core.SmartClient
{
    partial class UIBuildPackageForSmartClient : Form
    {
        public UIBuildPackageForSmartClient()
        {
            InitializeComponent();
            this.button1.Click += new EventHandler(button_Click);
            this.button2.Click += new EventHandler(button_Click);
            this.button3.Click += new EventHandler(button_Click);
            this.button4.Click += new EventHandler(button_Click);
            this.Shown += new EventHandler(UIBuildPackageForSmartClient_Shown);
        }

        void button_Click(object sender, EventArgs e)
        {
            switch ((sender as Button).Text)
            {
                case "添加...":
                    AddFiles();
                    break;
                case "删除":
                    dataGridView1.Rows.RemoveAt(dataGridView1.SelectedCells[0].RowIndex); 
                    break;
                case "打包":
                    Build();
                    break;
                case "关闭":
                    this.Close();
                    break;
            }
        }

        void UIBuildPackageForSmartClient_Shown(object sender, EventArgs e)
        {
            DataGridViewComboBoxColumn c = dataGridView1.Columns[5] as DataGridViewComboBoxColumn;
            c.Items.AddRange(Enum.GetNames(typeof(FileSpecialFolder))); 
        }

        void AddFiles()
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Multiselect = true;
            if (of.ShowDialog(this) == DialogResult.OK)
            {
                foreach (string s in of.FileNames)
                {
                    UpdateFile fc = FileUpdater.GetUpdateFile(s);
                    int i = dataGridView1.Rows.Add();
                    dataGridView1[1, i].Value = fc.Name;
                    dataGridView1[2, i].Value = fc.SourcePath;
                    dataGridView1[3, i].Value = fc.FileVersion;
                    dataGridView1[4, i].Value = fc.LastWriteTime.ToString();
                    dataGridView1[5, i].Value = Enum.GetName(typeof(FileSpecialFolder), FileSpecialFolder.ApplicationStartUpPath);
                }
            }
        }

        void Build()
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.AddExtension = true;
            sf.DefaultExt = "Unv.Package";
            if (sf.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            this.Cursor = Cursors.WaitCursor; 
            string filename = sf.FileName;
            UpdateFileContent[] fcs = new UpdateFileContent[dataGridView1.RowCount];
            int i=0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                fcs[i] = FileUpdater.GetUpdateFileContent(row.Cells[2].EditedFormattedValue.ToString() + row.Cells[1].EditedFormattedValue.ToString());
                fcs[i].ReleasePath = (FileSpecialFolder)Enum.Parse(typeof(FileSpecialFolder), row.Cells[5].EditedFormattedValue.ToString());
                fcs[i].RevPath = row.Cells[6].EditedFormattedValue.ToString();
                fcs[i].IsReboot = (bool)row.Cells[7].EditedFormattedValue;
                i++;
            }
            string debug = "";
            FileUpdater.BuildPackage(fcs, filename,ref debug);
#if(DEBUG)
            {
                System.IO.File.WriteAllText("c:\\debug.xml", debug);
            }
#endif
            this.Cursor = Cursors.Default ;
        }
        
    }
}
