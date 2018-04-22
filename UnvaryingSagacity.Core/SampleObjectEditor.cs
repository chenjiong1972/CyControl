using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms ;

namespace UnvaryingSagacity.Core
{
    public delegate bool CallbackBeforeSave(SampleClass obj, SampleClass old, ref bool isClose);

    public class SampleObjectEditor
    {
        private SampleClass _obj = new SampleClass();

        public SampleObjectEditor()
        {
        }

        public SampleClass CurrentObject { get; set; }

        public SampleClass NewObject { get { return _obj; } }

        public CallbackBeforeSave BeforeSave { get; set; }

        public DialogResult ShowDialog(IWin32Window owner, string title)
        {
            return ShowDialog(owner, title, "编号", "名称");
        }

        public DialogResult ShowDialog(IWin32Window owner,string title,string label1,string label2)
        {
            UISampleObjectEditor ui = new UISampleObjectEditor();
            ui.ParentObject = this;
            ui.Text = title;
            ui.label1.Text = label1;
            ui.label2.Text = label2; 
            DialogResult ret = ui.ShowDialog(owner);
            return ret;
        }

        internal void SetObject(string id ,string name)
        {
            _obj.ID = id;
            _obj.Name = name;
        }
    }
}
