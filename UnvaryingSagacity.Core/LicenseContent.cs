using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace UnvaryingSagacity.License
{
    public sealed class LicenseContent
    {
        Core.XmlExplorer xml;
        bool _isValid = false;
        bool _try = true;
        bool _moved = true ;

        public LicenseContent ()
        {
            xml = new UnvaryingSagacity.Core.XmlExplorer();
            if (xml.OpenFile(Application.StartupPath + @"\" + Application.ProductName + ".lic"))
                _isValid = IsValid();
            if (_isValid)
            {
                _try = (GetContent("深圳远睿恒软件有限公司软件运行许可//使用许可//许可类型") == "2" ? false : true);
                _moved = Moved();
            }
        }
        /// <summary>
        /// 获得许可项的内容
        /// </summary>
        /// <param name="fullName">许可项的满名称</param>
        /// <returns></returns>
        public string GetContent( string fullName)
        {
            string result = "非法试用";
            if (_isValid)
            {
                result = GetContentString(fullName);
            }
            return result;
        }

        public bool Valid { get { return _isValid; } }

        public bool IsTryVersion { get { return _try; } }

        public bool IsMoved { get { return _moved; } }

        public string LicInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("深圳远睿恒软件有限公司软件运行许可");
            sb.AppendLine();
            string s = GetContentString("深圳远睿恒软件有限公司软件运行许可//使用许可//许可类型");
            sb.AppendLine("许可类型:" + (s == "2" ? "正式许可" : "试用许可"));
            sb.AppendLine("授权用户:" + GetContentString("深圳远睿恒软件有限公司软件运行许可//使用许可//授权用户名称"));
            s = GetContentString("深圳远睿恒软件有限公司软件运行许可//使用许可//网络//网络类型");
            sb.AppendLine("网络类型:" + (s == "1" ? "单机" : "网络"));
            sb.AppendLine("站点数:" + GetContentString("深圳远睿恒软件有限公司软件运行许可//使用许可//网络//站点"));
            s = GetContentString("深圳远睿恒软件有限公司软件运行许可//使用许可//标识//标识类型");
            sb.AppendLine("标识类型:" + (s == "0" ? "软加密标识" : (s == "1" ? "加密狗" : "附赠U盘")));
            sb.AppendLine("标识码:" + GetContentString("深圳远睿恒软件有限公司软件运行许可//使用许可//标识//标识码"));
            sb.AppendLine("许可号:" + GetContentString("深圳远睿恒软件有限公司软件运行许可//使用许可//标识//许可号"));
            s = GetContentString("深圳远睿恒软件有限公司软件运行许可//使用许可//使用数据库");
            sb.AppendLine("数据库类型:" + s);// == "0" ? "其他" : (s == "1" ? "MS Access" : "MS SqlServer")));
            s = GetContentString("深圳远睿恒软件有限公司软件运行许可//使用许可//使用期限");
            s = s.Replace(":", " 到 ");
            s = s.Replace(";", " ; ");
            sb.AppendLine("使用期限:" + s);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("联系我们:");
            string[] keys = xml.KeyList("深圳远睿恒软件有限公司软件运行许可//联系我们");
            foreach (string key in keys)
            {
                sb.AppendLine("   " + key + ":" + GetContentString("深圳远睿恒软件有限公司软件运行许可//联系我们//" + key));
            }
            return sb.ToString();
        }

        public void ShowInfoDialog(IWin32Window owner)
        {
            string s = LicInfo();
            Core.GeneralForm f = new UnvaryingSagacity.Core.GeneralForm();
            TextBox textbox1 = new TextBox();
            textbox1.Multiline = true;
            textbox1.ReadOnly = true;
            textbox1.Dock = DockStyle.Fill;
            textbox1.AppendText(s);
            f.Size = new System.Drawing.Size(400, 400);
            f.StartPosition = FormStartPosition.CenterScreen; 
            f.Controls.Add (textbox1);
            f.ShowDialog(owner);
        }

        public string FileName { get { return xml.FileName; } }

        public string RegisterName
        {
            get
            {
                return GetContent("深圳远睿恒软件有限公司软件运行许可//使用许可//授权用户名称");
            }
        }
        public bool DateTimeInAllowRange(DateTime dt)
        {
            DateTime[,] dts = LicenseDateTimeRanges();
            if (dts != null)
            {
                for (int i = 0; i <= dts.GetUpperBound(0); i++)
                {
                    if (dts[i, 0].CompareTo(dt) <= 0 && dts[i, 1].CompareTo(dt) >= 0)
                        return true;
                }
            }
            return false;
        }

        private DateTime[,] LicenseDateTimeRanges()
        {
            DateTime[,] dts;
            string ss = GetContent("深圳远睿恒软件有限公司软件运行许可//使用许可//使用期限");
            string[] ranges = ss.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (ranges.Length > 0)
            {
                dts = new DateTime[ranges.Length, 2];
                int i = 0;
                foreach (string range in ranges)
                {
                    string[] dt = range.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    dts[i, 0] = Core.General.FromString(Core.General.ToShortDateString(dt[0], ""));
                    dts[i, 1] = Core.General.FromString(Core.General.ToShortDateString(dt[1], ""));
                    i++;
                }
                return dts;
            }
            return default(DateTime[,]);
        }
        private bool Moved()
        {
            
            string publicKey = xml.ReadValue("深圳远睿恒软件有限公司软件运行许可//加密区","Key3","");
            string key = Core.General.GetPrivateKey(publicKey);

            string keyName = "Key4";
            string realValue = xml.ReadValue("深圳远睿恒软件有限公司软件运行许可//加密区", keyName, "");
            byte[] value = Core.ShaEnCoder.UnHashFromString(realValue);
            Core.RC4.rc4_crypt(ref value, key);
            realValue = Encoding.UTF8.GetString(value);
            if (realValue == "0")
                return false;

            keyName = "Key1";
            realValue = xml.ReadValue("深圳远睿恒软件有限公司软件运行许可//加密区", keyName, "");
            value =Core.ShaEnCoder.UnHashFromString(realValue);
            Core.RC4.rc4_crypt(ref value, key);
            realValue = Encoding.UTF8.GetString(value);
            string s1=System.IO.File.GetCreationTime(xml.FileName).ToString("HH:mm:ss.fff yyyy-MM-dd");
            if (s1 != realValue)
            {
                int d1 = int.Parse(s1.Substring(6, 2));
                int d2=int.Parse (realValue.Substring(6,2));
                if (Math.Abs(d1 - d2) > 1)
                    return true;
            }
            keyName = "Key2";
            realValue = xml.ReadValue("深圳远睿恒软件有限公司软件运行许可//加密区", keyName, "");
            value = Core.ShaEnCoder.UnHashFromString(realValue);
            Core.RC4.rc4_crypt(ref value, key);
            realValue = Encoding.UTF8.GetString(value);
            s1 = System.IO.File.GetLastWriteTime(xml.FileName).ToString("HH:mm:ss.fff yyyy-MM-dd");
            if (s1 != realValue)
            {
                int d1 = int.Parse(s1.Substring(6, 2));
                int d2 = int.Parse(realValue.Substring(6, 2));
                if (Math.Abs(d1 - d2) > 1)
                    return true;
            }
            return false;
        }

        private string GetContentString(string fullName)
        {
            string result = "xxx";
            string xPath = fullName.Substring(0, fullName.LastIndexOf("//"));
            string xKey = fullName.Substring(fullName.LastIndexOf("//") + 2);
            result = xml.ReadValue(xPath, xKey, result);
            return result;
        }

        /// <summary>
        /// 检查文件是否被非法修改
        /// </summary>
        /// <returns></returns>
        private bool IsValid()
        {
            string publicKey = GetContentString("深圳远睿恒软件有限公司软件运行许可//加密区//Key3");
            string privateKey = Core.General.GetPrivateKey(publicKey);
            string s = System.IO.File.ReadAllText(Application.StartupPath + @"\" + Application.ProductName + ".lic", Encoding.UTF8);
            int i = s.IndexOf("<主签名>");
            int j = s.IndexOf("</主签名>");
            string tmp = s.Substring(i, j - i);
            string dst = s.Replace(tmp, "<主签名>签名");
            string s2 = Core.ShaEnCoder.HashToString(Core.ShaEnCoder.GetHash(Encoding.UTF8.GetBytes(dst), Encoding.UTF8.GetBytes(privateKey)));
            string s1 = GetContentString("深圳远睿恒软件有限公司软件运行许可//签名区//主签名");
            return s1==s2;
        }
    }

}
