using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using UnvaryingSagacity.Core;
using UnvaryingSagacity.SuitSchemePrinter;

namespace UnvaryingSagacity.AccountOfBank
{
    class Environment
    {
        public const string TOPMODALNAME_0 = "系统设置";
        public const string TOPMODALNAME_1 = "录入日记账";
        public const string TOPMODALNAME_2 = "查看日记账";
        public const string TOPMODALNAME_3 = "用户权限管理";

        public const string TOPMODALNAME_4 = "账套管理";
        public const string TOPMODALNAME_5 = "账户管理";
        public const string TOPMODALNAME_6 = "结算方式管理";
        public const string TOPMODALNAME_7 = "银行设置";
        //public const string FIXMODALNAME_4 = "帮助";

        public const string SCHEME_DATA_FILEEXT = ".Aod.Data";
        public const string APPLY_SETTING = "AccountOfBank.Setting";


        private UserAndRight.User _user = new UserAndRight.User();

        private License.LicenseContent _license;
        private string _key = "UnvaryingSagacity.AccountOfBank.APPLY_SETTING";
        private string _settingFileName;
        //private ImageList _imageList16;

        public Environment(string settingPath)
        {
            _license = new UnvaryingSagacity.License.LicenseContent();
            _settingFileName = settingPath + "\\"+APPLY_SETTING;
        }
        internal License.LicenseContent License { get { return _license; } }
        public string PageSetupFilename
        {
            get { return Application.CommonAppDataPath + @"\银行日记账页打印设置" + ".PageSet"; }
        }

        public string AccountCoverFilename
        {
            get { return Application.CommonAppDataPath + @"\账本封面定义" + ".xml"; }
        }
        public string AccountCoverPageSetupFilename
        {
            get { return Application.CommonAppDataPath + @"\账本封面打印设置" + ".PageSet"; }
        }
        public Account CurrentAccount { get; set; }
        public UserAndRight.User CurrentUser { get { return _user; } }

        public UserAndRight.Right GetCurrentRights()
        {
            UserAndRight.Right _right = new UserAndRight.Right();
            UserAndRight.Right r = new UserAndRight.Right();
            r.ID = "01";
            r.Name = "系统设置";
            r.Description = r.Name;
            _right.Nodes.Add(r.ID, r);

            UserAndRight.Right child = new UserAndRight.Right();
            child.ID = "0101";
            child.Name = "用户权限管理";
            child.Description = "用户权限管理";
            r.Nodes.Add(child);

            //child = new UserAndRight.Right();
            //child.ID = "0102";
            //child.Name = "结算方式管理";
            //child.Description = "结算方式管理";
            //r.Nodes.Add(child);

            child = new UserAndRight.Right();
            child.ID = "0103";
            child.Name = "银行设置";
            child.Description = "银行设置";
            r.Nodes.Add(child);

            child = new UserAndRight.Right();
            child.ID = "0104";
            child.Name = "备份数据库";
            child.Description = "备份数据库";
            r.Nodes.Add(child);

            child = new UserAndRight.Right();
            child.ID = "0105";
            child.Name = "恢复数据库";
            child.Description = "恢复备份的数据库";
            r.Nodes.Add(child);

            r = new UserAndRight.Right();
            r.ID = "02";
            r.Name = "账套管理";
            r.Description = "增加、修改、删除及引入引出账套";
            _right.Nodes.Add(r.ID, r);

            r = new UserAndRight.Right();
            r.ID = "03";
            r.Name = "账户管理";
            r.Description = "针对每个账套进行账户增加、修改、删除的管理控制";
            _right.Nodes.Add(r.ID, r);

            r = new UserAndRight.Right();
            r.ID = "04";
            r.Name = "录入日记账";
            r.Description = "针对每个账套进行账户录入控制";
            _right.Nodes.Add(r.ID, r);

            r = new UserAndRight.Right();
            r.ID = "05";
            r.Name = "查阅日记账";
            r.Description = "针对每个账套进行账户查阅控制";
            _right.Nodes.Add(r.ID, r);
            #region 增加账套权限到"03,04,05,06"下,增加账户到04,05的每个账套下
            Accounts acts = GetAccountList();
            foreach (Account act in acts)
            {
                if (System.IO.File.Exists(act.FullPath))
                {
                    child = new UserAndRight.Right();
                    child.ID = "03." + act.ID;
                    child.Name = act.Name;
                    child.Description = act.Description;
                    _right.Nodes["03"].Nodes.Add(child);
                }
            }

            foreach (Account act in acts)
            {
                if (System.IO.File.Exists(act.FullPath))
                {
                    child = new UserAndRight.Right();
                    child.ID = "04." + act.ID;
                    child.Name = act.Name;
                    child.Description = act.Description;
                    DataProvider dp = new DataProvider(act.FullPath);
                    ItemOfBankCollection items = dp.GetItemOfBankList();
                    foreach (ItemOfBank item in items)
                    {
                        UserAndRight.Right childChild = new UserAndRight.Right();
                        childChild.ID = child.ID + "." + item.ID;
                        childChild.Name = item.Name;
                        childChild.Description = item.Description;
                        child.Nodes.Add(childChild);
                    }
                    _right.Nodes["04"].Nodes.Add(child);
                }
            }
            foreach (Account act in acts)
            {
                if (System.IO.File.Exists(act.FullPath))
                {
                    child = new UserAndRight.Right();
                    child.ID = "05." + act.ID;
                    child.Name = act.Name;
                    child.Description = act.Description;
                    DataProvider dp = new DataProvider(act.FullPath);
                    ItemOfBankCollection items = dp.GetItemOfBankList();
                    foreach (ItemOfBank item in items)
                    {
                        UserAndRight.Right childChild = new UserAndRight.Right();
                        childChild.ID = child.ID + "." + item.ID;
                        childChild.Name = item.Name;
                        childChild.Description = item.Description;
                        child.Nodes.Add(childChild);
                    }
                    _right.Nodes["05"].Nodes.Add(child);
                }
            }

            r = new UserAndRight.Right();
            r.ID = "06";
            r.Name = "打开账套";
            r.Description = "允许打开和使用的账套";
            _right.Nodes.Add(r.ID, r);
            foreach (Account act in acts)
            {
                if (System.IO.File.Exists(act.FullPath))
                {
                    child = new UserAndRight.Right();
                    child.ID = "06." + act.ID;
                    child.Name = act.Name;
                    child.Description = act.Description;
                    _right.Nodes["06"].Nodes.Add(child);
                }
            }
            #endregion
            return _right;
        }


        public string GetLastLoginUser()
        {
            Core.XmlExplorer xml = OpenSetting(false);
            string n="";
            if (xml != null)
            {
                System.Xml.XmlNodeList nodes = xml.XmlDoc.GetElementsByTagName("lastLogin");
                if (nodes.Count > 0)
                {
                    n = nodes[0].InnerText;
                }
            }
            return n;
        }
        /// <summary>
        /// 审核用户名和密码
        /// </summary>
        /// <param name="name"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public int CheckUser(string name,string p)
        {
            Core.XmlExplorer xml = OpenSetting (true );
            if (CheckSigna(true))
            {
                if (xml != null)
                {
                    System.Xml.XmlNodeList nodes = xml.XmlDoc.GetElementsByTagName("user");
                    foreach (System.Xml.XmlNode node in nodes)
                    {
                        if (node.Attributes["name"].Value == name)
                        {
                            string key = node.Attributes["key"].Value;
                            string pwd = node.Attributes["pwd"].Value;
                            string s = GetUserPassportString(p + key, key);
                            if (s != pwd)
                            {
                                return 2;
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }
                    return 3;
                }
                else
                    return 1;
            }
            return 1;
        }
        /// <summary>
        /// 根据密码明文和Key产生密码密文
        /// </summary>
        /// <param name="password"></param>
        /// <param name="key"></param>
        /// <returns></returns>

        public void SetCurrentUser(string  name)
        {
            _user.Name = name;
            GetUserInfo(_user); 
        }

        /// <summary>
        /// 根据用户名称得到其所有信息
        /// </summary>
        /// <param name="user"></param>
        public void GetUserInfo(UserAndRight.User user)
        {
            UserAndRight.Right right = GetCurrentRights(); 
            Core.XmlExplorer xml = OpenSetting(true);
            if (xml != null)
            {
                System.Xml.XmlNodeList nodes = xml.XmlDoc.GetElementsByTagName("user");
                foreach (System.Xml.XmlNode node in nodes)
                {
                    if (user.Name == node.Attributes["name"].Value)
                    {
                        user.ID = int.Parse(node.Attributes["id"].Value);
                        user.Name = node.Attributes["name"].Value;
                        right.ToRightRuler(user.Ruler); 
                        GetUserRight(user);
                    }
                }
            }
        }

        /// <summary>
        /// 使用前先装载user.Ruler
        /// </summary>
        /// <param name="user"></param>
        public void GetUserRight(UserAndRight.User user )
        {
            #region load systemRight
            Core.XmlExplorer xml = OpenSetting(false);
            if (xml != null)
            {
                if(CheckSigna (xml,true ))
                {
                    System.Xml.XmlNode node = GetSettingNode(OpenSettingNodes(xml, "user"), "name", user.Name);
                    string s = node.ChildNodes[0].InnerText;
                    if (s.Length > 0)
                    {
                        if (s == "*")
                        {
                            user.IsSupper = true;
                        }
                        else
                        {
                            user.IsSupper = false;
                            string[] ss = s.Split(";".ToCharArray());
                            foreach (string id in ss)
                            {
                                UserAndRight.RightRuler ruler = (user.Ruler.FindRight(id, true) as UserAndRight.RightRuler);
                                if (ruler != null)
                                    ruler.RightState = UserAndRight.RightState.完全; 
                            }
                        }
                    }
                    
                }
            }
            #endregion

            #region load objectRight

            #endregion

        }

        public bool SetUsersRight(Users users)
        {
            Core.XmlExplorer xml = OpenSetting(false);
            bool b = false;
            if (xml != null)
            {
                System.Xml.XmlNodeList nodes = OpenSettingNodes(xml, "user");
                foreach (UserAndRight.User u in users)
                {
                    System.Xml.XmlNode node = GetSettingNode(nodes, "name", u.Name);
                    string s = node.ChildNodes[0].InnerText;
                    string r = u.IsSupper ? "*" : u.Ruler.GetRightIDByAllow(";");
                    if (s != r)
                    {
                        b = true;
                        node.ChildNodes[0].InnerText = r;
                        //更新当前用户的权限
                        if (u.Name == CurrentUser.Name)
                        {
                            CurrentUser.IsSupper = u.IsSupper;
                            if (!u.IsSupper)
                            {
                                u.Ruler.ToRightRuler(CurrentUser.Ruler); 
                            }
                        }
                    }
                }
                if (b)
                {
                    Signa(xml);
                    xml.XmlDoc.Save(_settingFileName);
                    return true;
                }
            }
            return false;
        }

        public void SetLastLogin(string name)
        {
            Core.XmlExplorer xml = OpenSetting(false);
            if (xml != null)
            {
                System.Xml.XmlNodeList nodes = xml.XmlDoc.GetElementsByTagName("lastLogin");
                if (nodes.Count > 0)
                {
                    nodes[0].InnerText = name;
                    nodes[0].Attributes["d"].Value = DateTime.Now.ToString() ; 
                    xml.XmlDoc.Save(_settingFileName);
                }
            }
        }
        
        public Accounts GetAccountList()
        {
            Accounts accts = new Accounts();
            Core.XmlExplorer xml = OpenSetting(true);
            if (xml != null)
            {
                System.Xml.XmlNodeList nodes = xml.XmlDoc.GetElementsByTagName("account");
                foreach (System.Xml.XmlNode node in nodes)
                {
                    Account acct = new Account();
                    acct.ID = node.Attributes["id"].Value;
                    acct.Name = node.Attributes["name"].Value;
                    acct.Description = node.InnerText;
                    acct.FullPath = node.Attributes["path"].Value;
                    accts.Add(acct.ID, acct);
                }
            }
            return accts;
        }

        public Account GetAccount(string name)
        {
            Core.XmlExplorer xml = OpenSetting(true);
            Account acct = new Account();
            if (xml != null)
            {
                System.Xml.XmlNodeList nodes = xml.XmlDoc.GetElementsByTagName("account");
                foreach (System.Xml.XmlNode node in nodes)
                {
                    if (name == node.Attributes["name"].Value)
                    {
                        acct.ID = node.Attributes["id"].Value;
                        acct.Name = node.Attributes["name"].Value;
                        acct.Description = node.InnerText;
                        acct.FullPath = node.Attributes["path"].Value;
                        return acct;
                    }
                }
            }
            return default(Account);
        }

        public Core.Printer.PrintAssign InitPrintAssign()
        {
            string filename = PageSetupFilename;
            Core.Printer.PageSetup pageSetup;// = new UnvaryingSagacity.Core.Printer.PageSetup(UnvaryingSagacity.Core.Printer.PrinterUnit.Display, printer.Document);
            if (!System.IO.File.Exists(filename))
            {
                pageSetup = new UnvaryingSagacity.Core.Printer.PageSetup();
                pageSetup.SubTitleBorder = UnvaryingSagacity.Core.Printer.PrinterLogTextBorder.上边线;
                pageSetup.SubTitleBorderStyle = UnvaryingSagacity.Core.Printer.PrinterBorderStyle.双实线边框;
                pageSetup.CyLine = true;
                pageSetup.SubTitleColor =0;
                pageSetup.SetAttributes(UnvaryingSagacity.Core.Printer.PageSetupKey.ePage_网格边框为双线, true);
                pageSetup.MaxCharsOfCyCell = 13; 
                Core.XmlSerializer<Core.Printer.PageSetup>.ToXmlSerializer(filename, pageSetup);
            }
            else
            {
                Core.XmlSerializer<Core.Printer.PageSetup>.FromXmlSerializer(filename, out pageSetup);
            }
            Core.Printer.PrintAssign printer = new UnvaryingSagacity.Core.Printer.PrintAssign(UnvaryingSagacity.Core.Printer.PrinterUnit.Display, "打印账页", pageSetup);
            printer.UserName = CurrentUser.Name;
            return printer;
        }

        public Core.Printer.PrintAssign InitPrintAssignByDailyReport()
        {
            string filename = Application.CommonAppDataPath + @"\银行日报表打印设置" + ".PageSet"; 
            Core.Printer.PageSetup pageSetup;// = new UnvaryingSagacity.Core.Printer.PageSetup(UnvaryingSagacity.Core.Printer.PrinterUnit.Display, printer.Document);
            if (!System.IO.File.Exists(filename))
            {
                pageSetup = new UnvaryingSagacity.Core.Printer.PageSetup();
                pageSetup.SubTitleBorder = UnvaryingSagacity.Core.Printer.PrinterLogTextBorder.上边线;
                pageSetup.SubTitleBorderStyle = UnvaryingSagacity.Core.Printer.PrinterBorderStyle.双实线边框;
                pageSetup.CyLine = true;
                pageSetup.SubTitleColor = 0;
                pageSetup.SetAttributes(UnvaryingSagacity.Core.Printer.PageSetupKey.ePage_网格边框为双线, true);
                Core.XmlSerializer<Core.Printer.PageSetup>.ToXmlSerializer(filename, pageSetup);
            }
            else
            {
                Core.XmlSerializer<Core.Printer.PageSetup>.FromXmlSerializer(filename, out pageSetup);
            }
            Core.Printer.PrintAssign printer = new UnvaryingSagacity.Core.Printer.PrintAssign(UnvaryingSagacity.Core.Printer.PrinterUnit.Display, "打印日报表", pageSetup);
            printer.UserName = CurrentUser.Name;
            return printer;
        }

        public string[] GetSetting(string tagName)
        {
            Core.XmlExplorer xml = OpenSetting();

            System.Xml.XmlNodeList nodes = OpenSettingNodes(xml, tagName);
            string[] s = new string[nodes.Count];
            int i = 0;
            foreach (System.Xml.XmlNode node in nodes)
            {
                s[i] = node.InnerText;
                i++;
            }
            return s;
        }

        public string[] GetSettingAttribute(string tagName, string attribute,bool checkSigna)
        {
            Core.XmlExplorer xml = OpenSetting();
            if (xml == null)
            {
                return new string[0];
            }
            else
            {
                if (checkSigna)
                {
                    if (!CheckSigna(xml, true))
                    {
                        return new string[0];
                    }
                }
                System.Xml.XmlNodeList nodes = OpenSettingNodes(xml, tagName);
                string[] s = new string[nodes.Count];
                int i = 0;
                foreach (System.Xml.XmlNode node in nodes)
                {
                    s[i] = node.Attributes[attribute].Value;
                    i++;
                }
                return s;
            }
        }
        public string[] GetSettingAttribute(string tagName, string attribute)
        {
            return GetSettingAttribute(tagName, attribute, false);
        }

        public bool AppendSetting(string name,string parentTagName,string tagName)
        {
            Core.XmlExplorer xml = OpenSetting();
            if (xml != null)
            {
                System.Xml.XmlNodeList nodes = xml.XmlDoc.GetElementsByTagName(parentTagName);
                if (nodes.Count > 0)
                {
                    System.Xml.XmlElement el = xml.XmlDoc.CreateElement(tagName);
                    el.InnerText = name;
                    nodes[0].AppendChild(el);
                    xml.XmlDoc.Save(_settingFileName);
                    return true;
                }
            }
            return false;
        }

        public bool DeleteSetting(string name, string tagName)
        {
            Core.XmlExplorer xml = OpenSetting();

            System.Xml.XmlNodeList nodes = OpenSettingNodes(xml, tagName);
            if (nodes.Count > 0)
            {
                foreach (System.Xml.XmlNode node in nodes)
                {
                    if (node.InnerText == name)
                    {
                        node.ParentNode.RemoveChild(node);
                        xml.XmlDoc.Save(_settingFileName);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool UpadteSetting(string name, string newName, string tagName)
        {
            Core.XmlExplorer xml = OpenSetting();
            System.Xml.XmlNodeList nodes = OpenSettingNodes(xml, tagName);
            if (nodes.Count > 0)
            {
                foreach (System.Xml.XmlNode node in nodes)
                {
                    if (node.InnerText == name)
                    {
                        node.InnerText = newName;
                        xml.XmlDoc.Save(_settingFileName);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool AppendAccount(Account act)
        {
            Core.XmlExplorer xml = OpenSetting();
            if (xml != null)
            {
                System.Xml.XmlNodeList nodes = xml.XmlDoc.GetElementsByTagName("accounts");
                if (nodes.Count > 0)
                {
                    System.Xml.XmlElement el = xml.XmlDoc.CreateElement("account");
                    System.Xml.XmlAttribute xmlAttr = xml.XmlDoc.CreateAttribute("id");
                    xmlAttr.Value = act.ID;
                    el.Attributes.Append(xmlAttr);
                    xmlAttr = xml.XmlDoc.CreateAttribute("name");
                    xmlAttr.Value = act.Name;
                    el.Attributes.Append(xmlAttr);
                    xmlAttr = xml.XmlDoc.CreateAttribute("path");
                    xmlAttr.Value = act.FullPath;
                    el.Attributes.Append(xmlAttr);
                    el.InnerText = act.Name;
                    nodes[0].AppendChild(el);
                    xml.XmlDoc.Save(_settingFileName);
                    return true;
                }
            }
            return false;
        }

        public bool DeleteAccount(string name,bool deleteFile)
        {
            Core.XmlExplorer xml = OpenSetting();

            System.Xml.XmlNodeList nodes = OpenSettingNodes(xml, "account");
            if (nodes.Count > 0)
            {
                foreach (System.Xml.XmlNode node in nodes)
                {
                    if (node.Attributes["name"].Value   == name)
                    {
                        node.ParentNode.RemoveChild(node);
                        xml.XmlDoc.Save(_settingFileName);
                        if (deleteFile)
                        {
                            System.IO.File.Delete(node.Attributes["path"].Value );
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public bool UpadteAccount(string name,Account  act)
        {
            Core.XmlExplorer xml = OpenSetting();
            System.Xml.XmlNodeList nodes = OpenSettingNodes(xml, "account");
            if (nodes.Count > 0)
            {
                foreach (System.Xml.XmlNode node in nodes)
                {
                    if (node.Attributes["name"].Value  == name)
                    {
                        node.InnerText = act.Description ;
                        node.Attributes["id"].Value  = act.ID;
                        node.Attributes["name"].Value = act.Name;
                        node.Attributes["path"].Value = act.FullPath;
                        xml.XmlDoc.Save(_settingFileName);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Exists(string tagName, string attribute, string value)
        {
            bool b = false;
            Core.XmlExplorer xml = OpenSetting(false);
            if (xml != null)
            {
                if (GetSettingNode(OpenSettingNodes(xml, tagName), attribute, value) != null)
                {
                    b = true;
                }
            }
            return b;
        }
        /// <summary>
        /// 本方法不会检查用户名是否存在,初始的PWD:88888888
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UserAndRight.User CreateUser(string name)
        {
            UserAndRight.User user = new UserAndRight.User(); 
            Core.XmlExplorer xml = OpenSetting(true);
            if (xml != null)
            {
                System.Xml.XmlNodeList nodes = OpenSettingNodes(xml, "users");
                System.Xml.XmlElement node = xml.XmlDoc.CreateElement("user");
                if (nodes != null)
                {
                    string key = Guid.NewGuid().ToString();
                    string p = GetUserPassportString("88888888" + key, key); 
                    System.Xml.XmlAttribute xmlAttr = xml.XmlDoc.CreateAttribute("id");
                    xmlAttr.Value = "1";
                    node.Attributes.Append(xmlAttr);
                    xmlAttr = xml.XmlDoc.CreateAttribute("name");
                    xmlAttr.Value = name;
                    node.Attributes.Append(xmlAttr);
                    xmlAttr = xml.XmlDoc.CreateAttribute("pwd");
                    xmlAttr.Value = p;
                    node.Attributes.Append(xmlAttr);
                    xmlAttr = xml.XmlDoc.CreateAttribute("key");
                    xmlAttr.Value = key;
                    node.Attributes.Append(xmlAttr);

                    System.Xml.XmlElement el = xml.XmlDoc.CreateElement("systemRight");
                    node.AppendChild(el);
                    el = xml.XmlDoc.CreateElement("objectRight");
                    node.AppendChild(el);
                    nodes[0].AppendChild(node); 
                    Signa(xml);
                    xml.XmlDoc.Save(_settingFileName);
                    user.ID = 1;
                    user.Name = name;
                    return user;
                }
            }
            return default(UserAndRight.User); 
        }
        public bool DeleteUser(string name)
        {
            Core.XmlExplorer xml = OpenSetting(true);
            if (xml != null)
            {
                System.Xml.XmlNodeList nodes = OpenSettingNodes(xml, "user");
                System.Xml.XmlNode node = GetSettingNode(nodes, "name", name);
                if (node != null)
                {
                    node.ParentNode.RemoveChild(node);
                    Signa(xml);
                    xml.XmlDoc.Save(_settingFileName);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="u">要修改的对象</param>
        /// <param name="attribute">要修改的属性名</param>
        /// <param name="newValue">新的值</param>
        /// <returns></returns>
        public bool UpdateUser(UserAndRight.User u, string attribute, string newValue)
        {
            Core.XmlExplorer xml = OpenSetting(true);
            bool changed = true;
            if (xml != null)
            {
                System.Xml.XmlNodeList nodes = OpenSettingNodes(xml, "user");
                System.Xml.XmlNode node = GetSettingNode(nodes, "name", u.Name);
                if (node != null)
                {
                    switch (attribute.ToLower())
                    {
                        case "pwd":
                            string key = Guid.NewGuid().ToString();
                            string p = GetUserPassportString(newValue+key, key);
                            node.Attributes["pwd"].Value  = p;
                            node.Attributes["key"].Value  = key;
                            break;
                        case "name":
                            node.Attributes["name"].Value  = newValue;
                            break;
                        default:
                            changed = false;
                            break;
                    }
                    if (changed)
                    {
                        Signa(xml);
                        xml.XmlDoc.Save(_settingFileName);
                        return true;
                    }
                }
            }
            return false;
        }
        string GetUserPassportString(string password, string key)
        {
            byte[] b = Encoding.UTF8.GetBytes(password);
            byte[] bKey = Encoding.ASCII.GetBytes(_key+key);
            return Core.ShaEnCoder.HashToString(Core.ShaEnCoder.GetHash(b, bKey));
        }
        Core.XmlExplorer OpenSetting()
        {
            return OpenSetting(true); 
        }
        Core.XmlExplorer OpenSetting(bool allowPromptOnFaile)
        {
            Core.XmlExplorer xml = new UnvaryingSagacity.Core.XmlExplorer();
            if (xml.OpenFile(_settingFileName))
            {
                return xml;
            }
            else if (allowPromptOnFaile)
            {
                System.Windows.Forms.MessageBox.Show("读取文件[" + APPLY_SETTING + "]失败, 无法继续使用.", "文件被人为篡改", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
            return default(Core.XmlExplorer);

        }

        bool CheckSigna(Core.XmlExplorer xml, bool allowPromptOnFaile)
        {
            bool b = false;
            System.Xml.XmlNodeList nodes = xml.XmlDoc.GetElementsByTagName("users");
            if (nodes.Count != 1)
                return false;
            string s = nodes[0].OuterXml;
            byte[] bt = Encoding.UTF8.GetBytes(s);
            byte[] key = Encoding.ASCII.GetBytes(_key);
            string s1 = Core.ShaEnCoder.HashToString(Core.ShaEnCoder.GetHash(bt, key));
            nodes = xml.XmlDoc.GetElementsByTagName("signa");
            foreach (System.Xml.XmlNode node in nodes)
            {
                if (node.Attributes["id"].Value == "signa1")
                {
                    if (s1 == node.InnerText)
                    {
                        b = true;
                    }
                    break;
                }
            }
            if (!b && allowPromptOnFaile)
            {
                System.Windows.Forms.MessageBox.Show("关键文件[" + APPLY_SETTING + "]被人为非法篡改, 无法继续使用.", "文件为篡改", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
            return b;
        }

        bool CheckSigna(bool allowPromptOnFaile)
        {
            Core.XmlExplorer xml = OpenSetting(true);
            if (xml != null)
            {
                return CheckSigna(xml, allowPromptOnFaile);
            }
            else
                return false;
        }

        System.Xml.XmlNodeList OpenSettingNodes(Core.XmlExplorer xml, string nodeTagName)
        {
            if (xml != null)
            {
                return xml.XmlDoc.GetElementsByTagName(nodeTagName);
            }
            else
                return null;
        }

        System.Xml.XmlNode GetSettingNode(System.Xml.XmlNodeList nodes,string attribute,string value)
        {
            try
            {
                foreach (System.Xml.XmlNode node in nodes)
                {
                    if (node.Attributes[attribute].Value == value)
                    {
                        return node;
                    }
                }
                return default(System.Xml.XmlNode);
            }
            catch { return default(System.Xml.XmlNode); }
        }

        void Signa(Core.XmlExplorer xml)
        {
            System.Xml.XmlNodeList nodes = xml.XmlDoc.GetElementsByTagName("users");
            if (nodes.Count != 1)
                return;
            string s = nodes[0].OuterXml;
            byte[] bt = Encoding.UTF8.GetBytes(s);
            byte[] key = Encoding.ASCII.GetBytes(_key);
            string s1 = Core.ShaEnCoder.HashToString(Core.ShaEnCoder.GetHash(bt, key));
            nodes = xml.XmlDoc.GetElementsByTagName("signa");
            foreach (System.Xml.XmlNode node in nodes)
            {
                if (node.Attributes["id"].Value == "signa1")
                {
                    node.InnerText = s1;
                    break;
                }
            }
        }

        public void AccountCoverCalculator(SchemeSerialization CurrentSuitScheme, ItemOfBank CurrentItemOfBank,bool printErrorValue)
        {
            SchemeCalculator _calc = new SchemeCalculator(this);
            //计算表达式
                _calc.InitCalculator(CurrentSuitScheme);

                _calc.RecorderDate = DateTime.Today.ToString("yyyy-MM-dd");
            _calc.Calc(CurrentSuitScheme);
            #region 设置系统内置项目的值
            foreach (SchemeItem item in CurrentSuitScheme.Items)
            {
                switch (item.Name)
                {
                    case "账套名称":
                        if (CurrentAccount == null)
                        {
                            item.Value =printErrorValue? "当前没有打开的账套":"";
                        }
                        else
                        {
                            item.Value = CurrentAccount.Name;
                        }
                        break;
                    case "账本名称":
                        //item.Value = item.Expression;
                        break;
                    case "账户名称":
                        if (CurrentItemOfBank == null)
                        {
                            item.Value =printErrorValue? "当前没有打开的账户,不能显示账户名称":"";
                        }
                        else
                        {
                            item.Value = CurrentItemOfBank.Name;
                        }
                        break;
                    case "账户隶属银行名称":
                        if (CurrentItemOfBank == null)
                        {
                            item.Value =printErrorValue? "当前没有打开的账户,不能显示银行名称":"";
                        }
                        else
                        {
                            item.Value = CurrentItemOfBank.OfBankName;
                        }
                        break;
                    case "账户的银行账号":
                        if (CurrentItemOfBank == null)
                        {
                            item.Value = printErrorValue ? "当前没有打开的账户,不能显示账号" : "";
                        }
                        else
                        {
                            item.Value = CurrentItemOfBank.ID;
                        }
                        break;
                    case "当前用户名":
                        item.Value = CurrentUser.Name;
                        break;
                    default:
                        //item.Value = item.Expression;
                        break;
                }
            }
            #endregion
            CurrentSuitScheme.SyncItemsFromBase();
        }

        public bool LoadScheme(SchemeSerialization suitScheme, string fileName)
        {
            SchemeSerialization t;
            if (Core.XmlSerializer<SchemeSerialization>.FromXmlSerializer(fileName, out t))
            {
                t.SyncItemsToBase();
                int i = t.Items.Index("账本名称");
                if ( i>=0)
                {
                    t.Items[i].Expression = (char)34 + "银行存款日记账" + (char)34; ; 
                }
                SchemeSerialization.Copy(suitScheme, t);

                suitScheme.SyncItemsFromBase();
                foreach (SchemeItemSerialization item in suitScheme.ItemArgs)
                {
                    item.ID = item.Name;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CreateScheme(SchemeSerialization CurrentScheme)
        {
            CurrentScheme.Size = new Size(297, 210);
            SchemeItem item = new SchemeItem();
            item.Name = "账套名称";
            item.MMLocation = new PointF(30, 30);
            item.MMSize = new SizeF(150, 15);
            item.Font = new Font("楷体_GB2312", 22, FontStyle.Bold);
            CurrentScheme.Items.Add(item);
            item = new SchemeItem();
            item.Name = "账本名称";
            item.MMLocation = new PointF(60, 70);
            item.MMSize = new SizeF(150, 15);
            item.Expression = (char)34 + "银行存款日记账" + (char)34;
            item.Font = new Font("楷体_GB2312", 22, FontStyle.Bold);
            CurrentScheme.Items.Add(item);
            item = new SchemeItem();
            item.Name = "打印年度";
            item.MMLocation = new PointF(30, 130);
            item.MMSize = new SizeF(41, 10);
            item.Expression = "Year([填票日期])";
            item.Font = new Font("楷体_GB2312", 12);
            item.ItemType = SchemeItemType.年;
            item.Style.NumberToUCaseNumber = BoolType.是;
            item.Style.TailString = "年度";
            CurrentScheme.Items.Add(item);
        }

        public void ShowPrintSettingsDialog(IWin32Window owner, SchemeSerialization scheme)
        {
            SchemePrinter printer = new UnvaryingSagacity.SuitSchemePrinter.SchemePrinter("账本封面");
            SchemePrintSettings pset = new SchemePrintSettings();
            string setFile = AccountCoverPageSetupFilename;
            if (Core.XmlSerializer<SchemePrintSettings>.FromXmlSerializer(setFile, out pset))
                printer.PrintSettings = pset;
            printer.Scheme = scheme;
            if (printer.ShowPageSettingsDialog(owner) == DialogResult.OK)
                Core.XmlSerializer<SchemePrintSettings>.ToXmlSerializer(setFile, printer.PrintSettings);
        }
        
        public void PrintScheme(IWin32Window owner, SchemeSerialization scheme, bool isPreview, bool hasBorder, bool hasImage)
        {
            SchemePrinter printer = new UnvaryingSagacity.SuitSchemePrinter.SchemePrinter("账本封面");
            printer.HasBorder = hasBorder;
            printer.HasBackgroundImage = hasImage;
            SchemePrintSettings pset = new SchemePrintSettings();
            string setFile = AccountCoverPageSetupFilename;
            if (Core.XmlSerializer<SchemePrintSettings>.FromXmlSerializer(setFile, out pset))
                printer.PrintSettings = pset;
            printer.Scheme = scheme;
            if (isPreview)
            {
                printer.ShowPreviewDialog(owner);
                Core.XmlSerializer<SchemePrintSettings>.ToXmlSerializer(setFile, printer.PrintSettings);
            }
            else
                printer.Print();
        }


        //internal void InitImageList16()
        //{
        //    _imageList16.Images.Add("folder", (Icon)Properties.Resources.folder);
        //    _imageList16.Images.Add("envelope", (Icon)Properties.Resources.envelope);
        //    _imageList16.Images.Add("box_open", (Icon)Properties.Resources.box_open);
        //    _imageList16.Images.Add("card_file", (Icon)Properties.Resources.card_file);
        //    _imageList16.Images.Add("cancel", (Icon)Properties.Resources.cancel);
        //    _imageList16.Images.Add("Database_Delete", (Icon)Properties.Resources.Database_Delete);
        //    _imageList16.Images.Add("recycle_bin", (Icon)Properties.Resources.recycle_bin);
        //    _imageList16.Images.Add("date_and_time", (Icon)Properties.Resources.date_and_time);
        //    _imageList16.Images.Add("date_and_time_2", (Icon)Properties.Resources.date_and_time_2);
        //    _imageList16.Images.Add("history", (Icon)Properties.Resources.history);
        //    _imageList16.Images.Add("binoculars_next", (Icon)Properties.Resources.binoculars_next);
        //    _imageList16.Images.Add("Image", (Icon)Properties.Resources.Image);
        //}
    }

    class SchemeCalculator
    {
        const string METHODPREFIX = "M";

        private Core.ExpressionCalculator _calc;
        private bool _initComplate = false;
        private Environment _e;

        int[] _mustCalcIndex;//有公式的项的索引
        bool[] _calcFlag;//计算是否完成标志与_mustCalcItem对应.0=未完成;1=完成.

        public SchemeCalculator(Environment e) { _e = e; FieldMode = true; }

        public bool FieldMode { get; set; }

        /// <summary>
        /// 填票日期:yyyy-MM-dd
        /// </summary>
        public string RecorderDate { get; set; }

        /// <summary>
        /// 建立计算模型
        /// </summary>
        /// <param name="scheme">用于建立计算模型,不会被保持,如果打印项增加或减少,必须重执行该方法</param>
        /// <returns></returns>
        public bool InitCalculator(SchemeSerialization scheme)
        {
            if (_calc == null)
            {
                _calc = new ExpressionCalculator();
            }
            _calc.FieldMode = FieldMode;
            if (FieldMode)
            {
                Core.FieldDefine[] Fields = GetExps(scheme);
                if (Fields.Length > 0)
                    _initComplate = _calc.CreateCalculator(GetFieldDef(scheme), Fields);
                else
                    _initComplate = false;
            }
            else
            {
                Core.ExpressionDefine[] exps = GetExps(scheme);
                if (exps.Length > 0)
                    _initComplate = _calc.CreateCalculator(exps);
                else
                    _initComplate = false;
            }
            return _initComplate;
        }

        /// <summary>
        /// 计算某个项
        /// </summary>
        /// <param name="scheme">用于建立参数表</param>
        /// <param name="item">要计算的项</param>
        /// <param name="recorderDate">填票日期format:yyyy-MM-dd</param>
        /// <returns></returns>
        public object Calc(SchemeSerialization scheme, SchemeItem item)
        {
            if (_initComplate)
            {
                object result;
                if (_calc.FieldMode)
                    result = _calc.Eval(METHODPREFIX + item.Name, GetFields(scheme));
                else
                    result = _calc.Eval(METHODPREFIX + item.Name, GetParams(scheme));
                if (result != null)
                {
                    if (item.ItemType == SchemeItemType.金额或数量)
                    {
                        if (Core.General.IsNumberic(result.ToString()))
                            return result;
                    }
                    else
                        return result;
                }
                return default(object);
            }
            else
                return default(object);
        }

        public void Calc(SchemeSerialization scheme)
        {
            int i = 0;
            for (int j = 0; j < scheme.Items.Count; j++)
            {
                SchemeItem item = scheme.Items.GetItem(j);
                if (item.Expression != null)
                {
                    if (item.Expression.Length > 0)
                    {
                        Array.Resize<int>(ref _mustCalcIndex, i + 1);
                        Array.Resize<bool>(ref _calcFlag, i + 1);
                        _mustCalcIndex[i] = j;
                        _calcFlag[i] = false;
                        i++;
                    }
                }
            }
            if (_mustCalcIndex != null)
            {
                for (i = 0; i < _mustCalcIndex.Length; i++)
                {
                    CalcItem(scheme, i);
                }
            }
        }

        /// <summary>
        /// 计算所有直接或间接引用name的公式
        /// </summary>
        /// <param name="name">被引用的名称</param>
        public string[] CalcAnyRefByItem(SchemeSerialization scheme, string name)
        {
            int i = 0;
            string[] names = new string[0];
            for (int j = 0; j < scheme.Items.Count; j++)
            {
                SchemeItem item = scheme.Items.GetItem(j);
                if (item.Expression != null)
                {
                    if (item.Expression.Length > 0)
                    {
                        Array.Resize<int>(ref _mustCalcIndex, i + 1);
                        Array.Resize<bool>(ref _calcFlag, i + 1);
                        _mustCalcIndex[i] = j;
                        _calcFlag[i] = false;
                        i++;
                    }
                }
            }
            if (_mustCalcIndex != null)
            {
                for (i = 0; i < _mustCalcIndex.Length; i++)
                {
                    SchemeItem item = scheme.Items.GetItem(_mustCalcIndex[i]);
                    if (item.Expression.Contains("[" + name + "]"))
                        CalcRefIndex(scheme, i, ref names);
                }
            }
            return names;
        }

        private void CalcRefIndex(SchemeSerialization scheme, int index, ref string[] name)
        {
            SchemeItem item = scheme.Items.GetItem(_mustCalcIndex[index]);

            if (item == null)
                return;
            if (item.Expression == null)
                return;
            if (item.Expression.Length <= 0)
                return;
            if (_calcFlag[index])//算过了,就不在算
                return;
            item.Value = Calc(scheme, item);
            _calcFlag[index] = true;
            Array.Resize<string>(ref name, name.Length + 1);
            name[name.Length - 1] = item.Name;
            int[] refCalc = new int[0];
            bool b = ExistRefbyItem(scheme, item.Name, ref  refCalc);
            if (b)
            {
                foreach (int i in refCalc)
                {
                    CalcRefIndex(scheme, i, ref name);
                }
            }
        }

        private bool ExistRefbyItem(SchemeSerialization scheme, string name, ref int[] refname)
        {
            bool b = false;
            for (int i = 0; i < _mustCalcIndex.Length; i++)
            {
                SchemeItem item = scheme.Items.GetItem(_mustCalcIndex[i]);
                if (item.Expression.Contains("[" + name + "]"))
                {
                    Array.Resize<int>(ref refname, refname.Length + 1);
                    refname[refname.Length - 1] = i;
                    if (!b)
                        b = true;
                }
            }
            return b;
        }

        private void CalcItem(SchemeSerialization scheme, int index)
        {
            SchemeItem item = scheme.Items.GetItem(_mustCalcIndex[index]);

            if (item == null)
                return;
            if (!_calcFlag[index])//算过了,就不在算
            {
                int[] notCalc = new int[0];
                bool b = ExistNotCalcRef(scheme, item, ref  notCalc);
                if (b)
                {
                    foreach (int i in notCalc)
                    {
                        CalcItem(scheme, i);
                    }
                }
                item.Value = Calc(scheme, item);
                _calcFlag[index] = true;
            }
        }

        private bool ExistNotCalcRef(SchemeSerialization scheme, SchemeItem item, ref int[] refname)
        {
            bool b = false;
            for (int i = 0; i < _mustCalcIndex.Length; i++)
            {
                string name = scheme.Items.GetItem(_mustCalcIndex[i]).Name;
                if (item.Expression.Contains("[" + name + "]") && (!_calcFlag[i]))
                {
                    Array.Resize<int>(ref refname, refname.Length + 1);
                    refname[refname.Length - 1] = i;
                    if (!b)
                        b = true;
                }
            }
            return b;
        }

        private Core.ExpressionDefine[] GetExps(SchemeSerialization scheme)
        {
            Core.ExpressionDefine[] exps = new ExpressionDefine[0];
            string param = GetParamDef(scheme);
            foreach (SchemeItem item in scheme.Items)
            {
                if (item.Expression != null)
                {
                    if (item.Expression.Length > 0)
                    {
                        Array.Resize<ExpressionDefine>(ref  exps, exps.Length + 1);
                        exps[exps.Length - 1] = new ExpressionDefine();
                        exps[exps.Length - 1].Name = METHODPREFIX + item.Name;
                        exps[exps.Length - 1].Parame = param;
                        exps[exps.Length - 1].Expression = item.Expression;
                    }
                }
            }
            return exps;
        }

        private string GetParamDef(SchemeSerialization scheme)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //scheme.SystemVar = Environment.GetSystemVars();
            //if (scheme.SystemVar != null)
            //{
            //    foreach (Core.PropertyItem pitem in scheme.SystemVar)
            //    {
            //        sb.Append("byval " + pitem.Name + " as string ,");
            //    }
            //}
            foreach (SchemeItem item in scheme.Items)
            {
                if (item.ItemType == SchemeItemType.年 || item.ItemType == SchemeItemType.日 || item.ItemType == SchemeItemType.月)
                    sb.Append("byval " + item.Name + " as object ,");
                else if (item.ItemType == SchemeItemType.金额或数量)
                {
                    sb.Append("byval " + item.Name + " as double ,");
                }
                else
                    sb.Append("byval " + item.Name + " as string ,");
            }
            sb.Append("byval 填票日期 as string ,");
            sb.Append("byval 当前填报人 as string ");
            sb.Append("byval 票证名称 as string ");
            return sb.ToString();
        }

        private string[] GetFieldDef(SchemeSerialization scheme)
        {
            string[] sb = new string[0];
            //scheme.SystemVar = Environment.GetSystemVars();
            //if (scheme.SystemVar != null)
            //{
            //    foreach (Core.PropertyItem pitem in scheme.SystemVar)
            //    {
            //        Array.Resize<string>(ref sb, sb.Length + 1);
            //        sb[sb.Length - 1] = " Public " + pitem.Name + " as string";
            //    }
            //}
            foreach (SchemeItem item in scheme.Items)
            {
                Array.Resize<string>(ref sb, sb.Length + 1);
                if (item.ItemType == SchemeItemType.年 || item.ItemType == SchemeItemType.日 || item.ItemType == SchemeItemType.月)
                    sb[sb.Length - 1] = " Public  " + item.Name + " as object";
                else if (item.ItemType == SchemeItemType.金额或数量)
                {
                    sb[sb.Length - 1] = " Public  " + item.Name + " as double";
                }
                else
                    sb[sb.Length - 1] = " Public  " + item.Name + " as string";
            }
            Array.Resize<string>(ref sb, sb.Length + 1);
            sb[sb.Length - 1] = " Public 填票日期 as string ";
            Array.Resize<string>(ref sb, sb.Length + 1);
            sb[sb.Length - 1] = " Public 当前填报人 as string ";
            Array.Resize<string>(ref sb, sb.Length + 1);
            sb[sb.Length - 1] = " Public 票证名称 as string ";
            return sb;
        }

        private object[] GetParams(SchemeSerialization scheme)
        {
            object[] ps = new object[0];

            int i = 0;
            //scheme.SystemVar = Environment.GetSystemVars();

            //foreach (Core.PropertyItem pitem in scheme.SystemVar)
            //{
            //    Array.Resize<object>(ref ps, i + 1);
            //    ps[i] = pitem.Value.ToString();
            //    i++;
            //}
            foreach (SchemeItem item in scheme.Items)
            {
                Array.Resize<object>(ref ps, i + 1);
                if (item.ItemType == SchemeItemType.年 || item.ItemType == SchemeItemType.日 || item.ItemType == SchemeItemType.月)
                    ps[i] = (item.Value == null ? 0 : (item.Value.ToString().Length > 0 ? int.Parse(item.Value.ToString()) : 0));
                else if (item.ItemType == SchemeItemType.金额或数量)
                {
                    ps[i] = item.Value == null ? 0 : (item.Value.ToString().Length > 0 ? (Core.General.IsNumberic(item.Value.ToString()) ? double.Parse(item.Value.ToString()) : 0) : 0);
                }
                else if (item.ItemType == SchemeItemType.是否)
                    ps[i] = item.Value == null ? "否" : item.Value.ToString();

                else
                    ps[i] = item.Value == null ? "" : item.Value.ToString();
                i++;
            }
            Array.Resize<object>(ref ps, i + 1);
            ps[i] = RecorderDate;
            i++;
            Array.Resize<object>(ref ps, i + 1);
            ps[i] = _e.CurrentUser.Name;
            i++;
            Array.Resize<object>(ref ps, i + 1);
            ps[i] = scheme.Name;
            return ps;
        }

        private Core.PropertyItem[] GetFields(SchemeSerialization scheme)
        {
            Core.PropertyItem[] ps = new Core.PropertyItem[0];

            int i = 0;
            //scheme.SystemVar = Environment.GetSystemVars();

            //foreach (Core.PropertyItem pitem in scheme.SystemVar)
            //{
            //    Array.Resize<PropertyItem>(ref ps, i + 1);
            //    ps[i] = new PropertyItem();
            //    ps[i].Value = pitem.Value.ToString();
            //    ps[i].Name = pitem.Name;
            //    i++;
            //}
            foreach (SchemeItem item in scheme.Items)
            {
                Array.Resize<PropertyItem>(ref ps, i + 1);
                ps[i] = new PropertyItem();
                ps[i].Name = item.Name;
                if (item.ItemType == SchemeItemType.年 || item.ItemType == SchemeItemType.日 || item.ItemType == SchemeItemType.月)
                    ps[i].Value = (item.Value == null ? 0 : (item.Value.ToString().Length > 0 ? int.Parse(item.Value.ToString()) : 0));
                else if (item.ItemType == SchemeItemType.金额或数量)
                {
                    ps[i].Value = item.Value == null ? 0 : (item.Value.ToString().Length > 0 ? (Core.General.IsNumberic(item.Value.ToString()) ? double.Parse(item.Value.ToString()) : 0) : 0);
                }
                else if (item.ItemType == SchemeItemType.是否)
                    ps[i].Value = item.Value == null ? "否" : item.Value.ToString();
                else
                    ps[i].Value = item.Value == null ? "" : item.Value.ToString();
                i++;
            }
            Array.Resize<PropertyItem>(ref ps, i + 1);
            ps[i] = new PropertyItem();
            ps[i].Name = "填票日期";
            ps[i].Value = RecorderDate;
            i++;
            Array.Resize<PropertyItem>(ref ps, i + 1);
            ps[i] = new PropertyItem();
            ps[i].Name = "当前填报人";
            ps[i].Value = _e.CurrentUser.Name;
            i++;
            Array.Resize<PropertyItem>(ref ps, i + 1);
            ps[i] = new PropertyItem();
            ps[i].Name = "票证名称";
            ps[i].Value = scheme.Name;
            return ps;
        }
    }

}
