using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UnvaryingSagacity.AccountOfBank
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

//            string fileName = "\\" + Environment.APPLY_SETTING;
//            if (!System.IO.File.Exists(Application.CommonAppDataPath + fileName))
//            {
//                if (!System.IO.File.Exists(Application.StartupPath + fileName))
//                {
//                    MessageBox.Show("原始配置文件[" + Environment.APPLY_SETTING + "]丢失, 无法继续运行.\n\n请尽快和软件开发商联系.", "关键文件丢失.", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                    return;
//                }
//                System.IO.File.Copy(Application.StartupPath + fileName, Application.CommonAppDataPath + fileName);
//            }
            Application.Run(new FrmVchView());
            return;
//            #region 建立数据模板
//#if(DEBUG)
//            fileName = Application.StartupPath + @"\" + DataProvider.DATATEMPLATE_FILENAME;
//            if (!System.IO.File.Exists(fileName))
//            {
//                DataProvider dp = new DataProvider(fileName);
//            }
//#endif
//            #endregion
//            new FrmVchView().Show();
//            Application.Run(new UIMain());
        }
    }
}
