using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml;

namespace UnvaryingSagacity.Core.SmartClient
{
    public delegate void CallBackUploadProcess(int index,int count,string filename);

    /// <summary>
    /// 用于在SQlserver下的文件自动更新    /// </summary>
    public class FileUpdater
    {
        SqlConnection _cnn = new SqlConnection();
        System.Data.DataSet MyDataSet = new DataSet();


        public FileUpdater(SqlConnection cnn)
        {
            _cnn = cnn;
            CheckSchema();
        }

        public UpdateFileList GetUpdateList()
        {
            string sql = "SELECT name, D, Version, LastWriteTime, ReleasePath, RevPath, UpdateLevel, Signa, Description, IsReboot FROM FileUpdateList";
            SqlCommand cm = new SqlCommand(sql, _cnn);
            SqlDataReader reader = cm.ExecuteReader();
            UpdateFileList fileList = new UpdateFileList();

            while (reader.Read())
            {
                UpdateFileContent f = new UpdateFileContent();
                f.Name = reader.GetString(0);
                f.D  = Core.General.FromString(reader.GetString(1));
                f.FileVersion = reader.GetString(2);
                f.LastWriteTime = Core.General.FromString(reader.GetString(3));
                f.ReleasePath = (FileSpecialFolder)Enum.Parse(typeof(FileSpecialFolder), reader.GetString(4));
                f.RevPath = reader.GetString(5);
                f.Level = reader.GetInt16(6);
                f.Signa = reader.GetString(7);
                f.Description = reader.GetString(8);
                f.IsReboot = reader.GetValue(9).ToString() == "1" ? true : false;
                fileList.Add(f.Name, f);
            }
            reader.Close();
            return fileList;
        }

        /// <summary>
        /// 根据UpdateFileContent的基本信息,获取其Content和签名
        /// </summary>
        /// <param name="f"></param>
        public void GetUpdateFile(UpdateFileContent f)
        {
            string sql = "select signa,content from FileUpdateList where name='" + f.Name + "'";
            SqlCommand cm = new SqlCommand(sql, _cnn);
            SqlDataReader reader = cm.ExecuteReader(CommandBehavior.SequentialAccess);

            if (reader.Read())
            {
                f.Signa = reader.GetString(0);
                if (!reader.IsDBNull(1))
                {
                    long i = reader.GetBytes(1, 0, null, 0, 1); ;
                    if (i > 0)
                    {
                        byte[] buff = new byte[i];
                        reader.GetBytes(1, 0, buff, 0, buff.Length);
                        f.Content = buff;
                    }
                }
            }
            reader.Close();
        }

        /// <summary>
        /// 获得UpdateFileContent的基本信息和Content和签名
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static UpdateFileContent GetUpdateFileContent(string filename)
        {
            UpdateFileContent f = FileUpdater.GetUpdateFile(filename) as UpdateFileContent;
            f.Content = System.IO.File.ReadAllBytes(filename);
            string k = System.Windows.Forms.Application.ProductName + System.Windows.Forms.Application.CompanyName;
            f.Signa = Core.ShaEnCoder.HashToString(Core.ShaEnCoder.GetHash(f.Content, Encoding.ASCII.GetBytes(k)));
            return f;
        }

        /// <summary>
        /// 不锁定文件获得版本等信息
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static UpdateFile GetUpdateFile(string filename)
        {
            UpdateFileContent f = new UpdateFileContent();
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filename);
            f.Name = fileInfo.Name;
            f.LastWriteTime = fileInfo.LastWriteTime;
            f.FileVersion = General.GetVersionInfo(filename).FileVersion;
            f.D = DateTime.Today;
            f.SourcePath = fileInfo.FullName.Replace(f.Name, "");
            return f;
        }

        /// <summary>
        /// 获得版本的方法,会锁定文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static UpdateFile GetUpdateFileByAssembly(string filename)
        {
            UpdateFileContent f = new UpdateFileContent();
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filename);
            f.Name = fileInfo.Name;
            f.LastWriteTime = fileInfo.LastWriteTime;
            f.FileVersion = General.GetVersionInfo(filename).FileVersion;
            f.D = DateTime.Today;
            if (!String.IsNullOrEmpty(f.FileVersion))
            {
                f.FileVersion = General.GetVersionOfAssembly(filename);
            }
            f.SourcePath = fileInfo.FullName.Replace(f.Name, "");
            return f;
        }

        /// <summary>
        /// 产生更新包到文件
        /// </summary>
        /// <param name="fcs">要打包的文件</param>
        /// <param name="filename">目标文件</param>
        /// <returns></returns>
        public static bool BuildPackage(UpdateFileContent[] fcs, string filename,ref string xmlOfDebug)
        {
            /// 文件前100K作为XML描述文件
            /// 
            Int32 ct = 1024 * 100;
            XmlDocument xml = new XmlDocument();
            xml.LoadXml("<root></root>");
            XmlNode xmlNode = xml.FirstChild;
            XmlElement xe;
            XmlAttribute xmlAttr;
            foreach (UpdateFileContent fc in fcs)
            {
                ct += fc.Content.Length;
                #region 写Xml
                xe =xml.CreateElement ("files");
                xe.InnerText = fc.Name;
                xmlAttr = xml.CreateAttribute("LastWriteTime");
                xmlAttr.Value = fc.LastWriteTime.ToString("yyyyMMddHHmmss");
                xe.Attributes.Append(xmlAttr);

                xmlAttr = xml.CreateAttribute("FileVersion");
                xmlAttr.Value = fc.FileVersion;
                xe.Attributes.Append(xmlAttr);

                xmlAttr = xml.CreateAttribute("ReleasePath");
                xmlAttr.Value = Enum.GetName(typeof(FileSpecialFolder), fc.ReleasePath);
                xe.Attributes.Append(xmlAttr);

                xmlAttr = xml.CreateAttribute("RevPath");
                xmlAttr.Value = fc.RevPath;
                xe.Attributes.Append(xmlAttr);

                xmlAttr = xml.CreateAttribute("IsReboot");
                xmlAttr.Value = fc.IsReboot ? "1" : "0";
                xe.Attributes.Append(xmlAttr);

                xmlAttr = xml.CreateAttribute("Description");
                xmlAttr.Value = fc.Description;
                xe.Attributes.Append(xmlAttr);

                xmlAttr = xml.CreateAttribute("Level");
                xmlAttr.Value = fc.Level.ToString();
                xe.Attributes.Append(xmlAttr);

                xmlAttr = xml.CreateAttribute("D");
                xmlAttr.Value = fc.D.ToString("yyyyMMdd");
                xe.Attributes.Append(xmlAttr);

                xmlAttr = xml.CreateAttribute("Length");
                xmlAttr.Value = fc.Content.Length.ToString();
                xe.Attributes.Append(xmlAttr);

                XmlElement xeSigna = xml.CreateElement("signa");
                xeSigna.InnerText = fc.Signa;
                xe.AppendChild(xeSigna);

                xmlNode.AppendChild(xe);
                #endregion
            }
            xmlOfDebug = xml.InnerXml;
            byte[] bt = new byte[ct];
            Encoding.UTF8.GetBytes(xmlOfDebug).CopyTo(bt, 0);
            int i = 100 * 1024;
            foreach (UpdateFileContent fc in fcs)
            {
                fc.Content.CopyTo(bt, i);
                i += fc.Content.Length;
            }
            System.IO.File.WriteAllBytes(filename, bt);
            return true;
        }

        public static void ShowBuildDialog(System.Windows.Forms.IWin32Window owner )
        {
            UIBuildPackageForSmartClient ui = new UIBuildPackageForSmartClient();
            ui.ShowDialog(owner); 
        }

        /// <summary>
        /// 上传更新包到到数据库
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool UpLoadPackage(string filename,CallBackUploadProcess p)
        {
            UpdateFileContent[] fcs = AnalyPackage(filename);
            if (fcs.Length > 0)
            {
                int index=1;
                SqlTransaction trans = _cnn.BeginTransaction();
                foreach (UpdateFileContent fc in fcs)
                {
                    if (p != null)
                    {
                        p(index, fcs.Length, fc.Name);
                    }
                    index++;
                    if (!fc.CheckSigna())
                    {
                        System.Windows.Forms.MessageBox.Show("上传文件的内容与签名不符, 不能上传.", "检查上传内容");
                        trans.Rollback();
                        return false;
                    }
                    if (!UpLoad(fc, trans))
                    {
                        trans.Rollback();
                        return false;
                    }
                }
                trans.Commit(); 
                return true;
            }
            return false;
        }

        public UpdateFileContent[] AnalyPackage(string filename)
        {
            UpdateFileContent[] fcs = new UpdateFileContent[0];
            byte[] bt = System.IO.File.ReadAllBytes(filename);
            int length = 1024 * 100;
            /// 前100K为XML头
            byte[] btOfXml = new byte[length];
            Array.Copy(bt, 0, btOfXml, 0, length);
            string xmlString = Encoding.UTF8.GetString(btOfXml);
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.LoadXml(xmlString);
                XmlNodeList list = xml.GetElementsByTagName("files");
                foreach (XmlNode node in list)
                {
                    Array.Resize<UpdateFileContent>(ref fcs, fcs.Length + 1);
                    int i = fcs.Length - 1;
                    fcs[i] = new UpdateFileContent();
                    fcs[i].Name = node.FirstChild.InnerText;
                    fcs[i].LastWriteTime = Core.General.FromString(node.Attributes["LastWriteTime"].Value);
                    fcs[i].D = Core.General.FromString(node.Attributes["D"].Value);
                    fcs[i].FileVersion = node.Attributes["FileVersion"].Value;
                    fcs[i].ReleasePath = (FileSpecialFolder)Enum.Parse(typeof(FileSpecialFolder), node.Attributes["ReleasePath"].Value);
                    fcs[i].RevPath = node.Attributes["RevPath"].Value;
                    fcs[i].IsReboot = (node.Attributes["IsReboot"].Value)=="1"?true :false ;
                    fcs[i].Level = Int16.Parse(node.Attributes["Level"].Value);
                    fcs[i].Description = node.Attributes["Description"].Value;
                    int ln = int.Parse(node.Attributes["Length"].Value);
                    fcs[i].Content = new byte[ln];
                    Array.Copy(bt, length, fcs[i].Content, 0, ln);
                    fcs[i].Signa = node.LastChild.InnerText;
                    length += ln;
                    i++;
                }
                return fcs;
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "分析更新包");  

                return fcs;
            }
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        public bool UpLoad(UpdateFileContent f,SqlTransaction trans)
        {
            try
            {
                string sql = "delete from FileUpdateList where name='" + f.Name + "'";
                SqlCommand cm = new SqlCommand(sql, _cnn, trans);
                cm.ExecuteNonQuery();

                sql = "Select name, D, Version, LastWriteTime, ReleasePath, RevPath, UpdateLevel, Description, IsReboot,signa,content from FileUpdateList where name='" + f.Name + "'";
                cm = new SqlCommand(sql, _cnn, trans);
                SqlDataAdapter sda = new SqlDataAdapter(cm);
                sda.Fill(MyDataSet, "FileUpdateList");

                DataRow dr = MyDataSet.Tables["FileUpdateList"].NewRow();
                dr["Name"] = f.Name;
                dr["D"] = f.D.ToString("yyyyMMdd");
                dr["Version"] = f.FileVersion;
                dr["ReleasePath"] = Enum.GetName(typeof(FileSpecialFolder), f.ReleasePath);
                dr["RevPath"] = f.RevPath;
                dr["LastWriteTime"] = f.LastWriteTime.ToString("yyyyMMddHHmmss");
                dr["Description"] = f.Description;
                dr["UpdateLevel"] = f.Level;
                dr["signa"] = f.Signa;
                dr["IsReboot"] = f.IsReboot;
                dr["content"] = f.Content;
                MyDataSet.Tables["FileUpdateList"].Rows.Add(dr);

                SqlCommandBuilder sb = new SqlCommandBuilder(sda);
                sda.Update(MyDataSet, "FileUpdateList");
                sda.Dispose();
                return true;
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message , "上传内容");  
                return false;
            }
        }

        private void CheckSchema()
        {
            try
            {
                Data.DataSchemaProvider.Sql sql = new UnvaryingSagacity.Data.DataSchemaProvider.Sql();
                sql.Connect = _cnn;
                if (!sql.ExistTable("FileUpdateList"))//不允许相同文件名的文件
                {
                    #region 架构定义
                    int i = 0;
                    Data.DataSchemaProvider.DataColumnEx[] cols = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx[11];
                    cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                    cols[i].ColumnName = "name";
                    cols[i].DataType = System.Data.DbType.String;
                    cols[i].MaxLength = 50;
                    i++;
                    cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                    cols[i].ColumnName = "D";//上传日期
                    cols[i].DataType = System.Data.DbType.VarNumeric;
                    cols[i].MaxLength = 14;
                    i++;
                    cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                    cols[i].ColumnName = "Version";
                    cols[i].DataType = System.Data.DbType.String;
                    cols[i].MaxLength = 50;
                    i++;
                    cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                    cols[i].ColumnName = "LastWriteTime";//最后修改时间
                    cols[i].DataType = System.Data.DbType.String;
                    cols[i].MaxLength = 50;
                    i++;
                    cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                    cols[i].ColumnName = "ReleasePath";//发布的绝对路径,FileSpecialFolder的枚举
                    cols[i].DataType = System.Data.DbType.String;
                    cols[i].MaxLength = 100;
                    i++;
                    cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                    cols[i].ColumnName = "RevPath";//发布的绝对路径下的相对路径
                    cols[i].DataType = System.Data.DbType.String;
                    cols[i].MaxLength = 50;
                    i++;
                    cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                    cols[i].ColumnName = "UpdateLevel";//强制更新的级别
                    cols[i].DataType = System.Data.DbType.Int16;
                    i++;
                    cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                    cols[i].ColumnName = "Signa";
                    cols[i].DataType = System.Data.DbType.String;
                    cols[i].MaxLength = 300;
                    i++;
                    cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                    cols[i].ColumnName = "Description";
                    cols[i].DataType = System.Data.DbType.String;
                    cols[i].MaxLength = 200;
                    i++;
                    cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                    cols[i].ColumnName = "IsReboot";//是否要重启软件
                    cols[i].DataType = System.Data.DbType.Int16;
                    i++;
                    cols[i] = new UnvaryingSagacity.Data.DataSchemaProvider.DataColumnEx();
                    cols[i].ColumnName = "Content";
                    cols[i].DataType = System.Data.DbType.Object;
                    sql.CreateTable("FileUpdateList", cols, new string[1] { "name" });
                    #endregion
                }
            }

            catch
            {
            }
        }
    }

    public class UpdateFile
    {
        public string Name { get; set; }
        public string SourcePath { get; set; }
        public DateTime LastWriteTime { get; set; }
        public string FileVersion { get; set; }
        /// <summary>
        /// 发布的绝对路径        /// </summary>
        public FileSpecialFolder ReleasePath { get; set; }
        /// <summary>
        /// 发布的绝对路径下的相对路径,        /// </summary>
        public string RevPath { get; set; }
        public string DestPath
        {
            get
            {
                string s = "";
                switch (this.ReleasePath)
                {
                    case FileSpecialFolder.CommonApplicationData :
                        s = System.Windows.Forms.Application.CommonAppDataPath;
                        break;
                    case FileSpecialFolder.ApplicationStartUpPath:
                        s = System.Windows.Forms.Application.StartupPath;
                        break;
                    case FileSpecialFolder.ApplicationData :
                        s = System.Windows.Forms.Application.UserAppDataPath;
                        break;
                    case FileSpecialFolder.LocalApplicationData :
                        s = System.Windows.Forms.Application.LocalUserAppDataPath;
                        break;
                    default:
                        break;
                }
                s = System.IO.Path.Combine(s, RevPath);
                return s;
            }
        }
        public string FullName
        {
            get
            {
                return System.IO.Path.Combine(DestPath, Name);
            }
        }
        public DateTime D { get; set; }
        public bool IsReboot { get; set; }
        public string Description { get; set; }
        public Int16 Level{get;set;}
    }

    public class UpdateFileContent:UpdateFile 
    {
        public byte[] Content { get; set; }

        public string Signa { get; set; }

        public bool CheckSigna()
        {
            string k = System.Windows.Forms.Application.ProductName + System.Windows.Forms.Application.CompanyName;
            string s = Core.ShaEnCoder.HashToString(Core.ShaEnCoder.GetHash(this.Content, Encoding.ASCII.GetBytes(k)));
            if (s.Equals(this.Signa))
                return true;
            else
                return false;
        }
    }

    public class UpdateFileList : Core.ListDictionary<UpdateFile> { }

    public enum FileSpecialFolder
    {
        /// <summary>
        /// 应用程序启动目录
        /// </summary>
        ApplicationStartUpPath,

        /// <summary>
        /// 用作当前漫游用户的应用程序特定数据的公共储存库
        /// </summary>
        ApplicationData,
        /// <summary>
        /// 用作当前非漫游用户使用的应用程序特定数据的公共储存库
        /// </summary>
        LocalApplicationData,
        /// <summary>
        /// 它用作所有用户使用的应用程序特定数据的公共储存库
        /// </summary>
        CommonApplicationData,
    }

    /// <summary>
    /// 和应用耦合的控制平台.
    /// 实现该接口的类名放在配置文件由主界面动态创建.
    /// 可以管理主界面状态托盘和创建自动执行的线程.
    /// </summary>
    public interface IApplyConsole
    {
        int InitConsole(ITopControlContain winForm,IEnvironment e);
        bool Start();
    }

    /// <summary>
    /// 和应用耦合的运行环境
    /// 实现该接口的类名放在配置文件由主界面动态创建.
    /// </summary>
    public interface IEnvironment
    {
        UnvaryingSagacity.License.LicenseContent CurrentLicenseContent { get; }
        UnvaryingSagacity.Core.Log CurrentLogServer { get; set; }
        UnvaryingSagacity.Core.Log CurrentLogWriter { get; set; }
        UnvaryingSagacity.UserAndRight.User CurrentUser { get; set; }
        UnvaryingSagacity.Core.ListDictionary<UnvaryingSagacity.Core.ApplyItem> GetApplyItems();
        UnvaryingSagacity.UserAndRight.RightState GetRightState(string rightID, bool hasRightPrompt);
        UnvaryingSagacity.UserAndRight.RightState GetRightState(string rightID);
        string ResourcesPath { get; }
        UnvaryingSagacity.UserAndRight.Right RightDefined { get; }
        System.Xml.XmlDocument xml { get; set; }
        string ConnectionString { get; }
        DateTime DateTimeOfServer { get; }
    }

    /// <summary>
    /// 主界面
    /// </summary>
    public interface ITopControlContain
    {
        void BringToFrontFromPanel(System.Windows.Forms.Control c);
        void CollapsePanel(bool b);
        void AddControl(System.Windows.Forms.Control c);
        void InvokeSubApply(ApplyItem item, object[] parameters);
        void AddNotifyIcon(Core.ApplyItem item);
        void RemoveNotifyIcon(string applyItemName);
        void RefreshNotifyIcon(string applyItemName);
        void RefreshAllNotifyIcon();
        int Height { get; }
        int Width { get; }
    }

    /// <summary>
    /// 调用子模块的方法
    /// </summary>
    public interface ISubApplyInvoke
    {
        /// <summary>
        /// 通过转递环境对象来检查权限等,不允许被调用则返回False
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        bool InitEnvironment(object environment,object[] parameters );
    }

    public interface ISubApplyInvoke2 : ISubApplyInvoke
    {
        object Invoke();
    }
}
