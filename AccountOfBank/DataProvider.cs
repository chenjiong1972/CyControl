using System;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Text;
using System.Data;
using System.Xml;

namespace UnvaryingSagacity.AccountOfBank
{
    /// <summary>
    /// 不稳定，现象：在进入过历史数据后录入的数据不能保存包括修改结构。原因是共享冲突。
    /// 解决方法：使用时创建，生命周期保持在单独的类中
    /// </summary>

    class DataProvider
    {

        public const string DATATEMPLATE_FILENAME = "AccountOfBank.Data.Template";
        public const string TABLE_MAIN = "mainTable";
        public const string TABLE_ITEMOFBANKS = "ItemOfBanks";
        public const string TABLE_CONFIG = "DBParams";
        public const string TABLE_ACCOUNTYEARCLOSED = "YearClosed";
        public const string CONFIG_备份日期 = "LastBackup";
        public const string CONFIG_备份创建人 = "BackupAuthor";
        public const string CONFIG_备份账套名称 = "AccountName";
        public const string CONFIG_备份账套编号 = "AccountID";
        public const string CONFIG_数据结构版本 = "Version";
        public const string CONFIG_STARTDATE = "StartDate";
        //数据架构更新后要增加数据版本元素到最后
        //前两节为程序版本,第3,4,5节为年月日,最后节为当日的累计
        private string[] DATASCHEMA_VERSIONS = new string[]{"1.0.0.0.0.0",
                                                            "1.0.2010.1.25.0",
                                                            "1.0.2010.2.2.0",
                                                            };
        private DataSet _dataSet = new DataSet();
        private string _fileName;

        public DataProvider(string fileName)
        {
            _fileName = fileName;
            if (!System.IO.File.Exists(fileName))
            {
                CreateDataStorage();
            }
            else
                _dataSet.ReadXml(fileName);
            CheckSchema();
        }

        /// <summary>
        /// 将由整数和小数部分组成的指定天数与当前的 DateTimeOffset 对象相加.例如，4.5 等效于 4 天 12 小时 0 分 0 秒 0 毫秒。days 参数被舍入到最近的毫秒
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string InDateAddDays(double value)
        {
            return DateTime.Today.AddDays(value).ToString("yyyMMdd");
        }

        public static string InDateAddDays(DateTime theDay, double value)
        {
            return theDay.AddDays(value).ToString("yyyyMMdd");
        }

        public string[] DistinctRecords(string columnname, string filterExpression)
        {
            return DistinctRecords(TABLE_MAIN, columnname, filterExpression);
        }

        public string[] DistinctRecords(string tableName, string columnname, string filterExpression)
        {
            string[] s = new string[0];
            int columnIndex = _dataSet.Tables[tableName].Columns.IndexOf(columnname);
            if (columnIndex < 0)
                return s;
            //对columnname排序,排序后循环一次就够了
            DataRow[] dr = _dataSet.Tables[tableName].Select(filterExpression, columnname);

            int j = 0;
            string preValue = "";
            for (int i = 0; i < dr.Length; i++)
            {
                DataRow r = dr[i];
                string thisValue = r[columnIndex].ToString();
                if (!thisValue.Equals(preValue) && thisValue.Length > 0)
                {
                    preValue = thisValue;
                    Array.Resize<string>(ref s, j + 1);
                    s[j] = preValue;
                    j++;
                }
            }
            return s;
        }

        public DataRow[] GetDataRows(string filterExpress, string sort)
        {
            return GetDataRows(TABLE_MAIN, filterExpress, sort);
        }

        public DataRow[] GetDataRows(string tableName, string filterExpress, string sort)
        {
            DataRow[] foundRows = new DataRow[0];

            try
            {
                DataTable dataTable = _dataSet.Tables[tableName];

                if (filterExpress.Length <= 0)
                    foundRows = dataTable.Select();
                else
                {
                    if (sort.Length <= 0)
                        foundRows = dataTable.Select(filterExpress);
                    else
                        foundRows = dataTable.Select(filterExpress, sort);
                }
            }
            catch
            {
                return null; ;
            }
            return foundRows;
        }

        /// <summary>
        /// 不保存为提交的改变
        /// </summary>
        public void Refresh()
        {
            Refresh(false); 
        }

        public void Refresh(bool isUpdate)
        {
            if (isUpdate)
                _dataSet.WriteXml(_fileName, XmlWriteMode.WriteSchema);
            _dataSet.Reset();
            _dataSet.ReadXml(_fileName);
        }
        /// <summary>
        /// 不保存为提交的改变
        /// </summary>
        public void Closed()
        {
            _dataSet.Reset();
            _dataSet.Dispose();
        }
        public bool HasRows()
        {
            return HasRows(TABLE_MAIN);
        }

        public bool HasRows(string tableName)
        {
            DataTable dt = _dataSet.Tables[tableName];
            return dt.Rows.Count > 0 ? true : false;
        }

        public DataSet dataSet
        {
            get { return _dataSet; }
        }

        public void CreateDataStorage()
        {
            DataTable dataTable = new DataTable(TABLE_MAIN);
            DataColumn[] dc = GetSchemaColumns(TABLE_MAIN);
            dataTable.Columns.AddRange(dc);
            dataTable.PrimaryKey = new DataColumn[3]{
                                        dc[0],
                                        dc[1],
                                        dc[2],
                                        };
            _dataSet.Tables.Add(dataTable);

            dataTable = new DataTable(TABLE_ITEMOFBANKS);
            DataColumn[] dc1 = GetSchemaColumns(TABLE_ITEMOFBANKS);
            dataTable.Columns.AddRange(dc1);
            dataTable.PrimaryKey = new DataColumn[2]{
                                        dc1[0],
                                        dc1[1],
                                        };
            _dataSet.Tables.Add(dataTable);

            dataTable = new DataTable(TABLE_CONFIG);
            DataColumn[] dc2 = GetSchemaColumns(TABLE_CONFIG);
            dataTable.Columns.AddRange(dc2);
            dataTable.PrimaryKey = new DataColumn[2]{
                                        dc2[0],
                                        dc2[1],
                                        };
            _dataSet.Tables.Add(dataTable);

            dataTable = new DataTable(TABLE_ACCOUNTYEARCLOSED);
            DataColumn[] dc3 = GetSchemaColumns(TABLE_ACCOUNTYEARCLOSED);
            dataTable.Columns.AddRange(dc3);
            dataTable.PrimaryKey = new DataColumn[2]{
                                        dc3[0],
                                        dc3[1],
                                        };
            _dataSet.Tables.Add(dataTable);
            CurrentSchemaVersion = CurrentSchemaVersion;
            _dataSet.WriteXml(_fileName, XmlWriteMode.WriteSchema);
        }

        public string SchemaVersion()
        {
            string ver = "1.0.0.0.0.0";
            if (_dataSet.Tables.IndexOf(TABLE_CONFIG) >= 0)
            {
                ver = GetConfig(CONFIG_数据结构版本); 
            }
            return ver;
        }

        public string CurrentSchemaVersion
        {
            get { return DATASCHEMA_VERSIONS[DATASCHEMA_VERSIONS.Length - 1]; }
            set
            {
                SetConfig(CONFIG_数据结构版本, value);
            }
        }

        public DateTime LastBackupDateTime
        {
            get
            {
                DataRow[] rs = GetDataRows(TABLE_CONFIG, "id='"+CONFIG_备份日期  +"'", "");
                if (rs.Length > 0)
                {
                    return DateTime.Parse(rs[0]["value"].ToString());
                }
                else
                    return new DateTime(1972,11,2);
            }
            set
            {
                SetConfig(CONFIG_备份日期, value.ToString("yyyy-MM-dd"));
            }
        }

        public string GetConfig(string id)
        {
            return GetConfig(id, "*");
        }
        public string GetConfig(string id, string itemOfBank)
        {
            string filter = "id='" + id + "'";
            if (itemOfBank != "*")
            {
                filter += "AND ItemOfBank='" + itemOfBank + "'";
            }
            DataRow[] rs = GetDataRows(TABLE_CONFIG, filter, "");
            if (rs.Length > 0)
            {
                return rs[0]["value"].ToString();
            }
            else
                return "";
        }

        public void SetConfig(string id, string value)
        {
            SetConfig(id, "*", value); ;
        }
        public void SetConfig(string id, string itemOfBank, string value)
        {

            string filter = "id='" + id + "'";
            if (itemOfBank != "*")
            {
                filter += "AND ItemOfBank='" + itemOfBank + "'";
            }
            DataRow[] rs = GetDataRows(TABLE_CONFIG, filter, "");
            lock (_dataSet)
            {
                DataTable tb = _dataSet.Tables[TABLE_CONFIG];
                DataRow r;
                if (rs.Length > 0)
                {
                    r = rs[0];
                }
                else
                {
                    r = tb.NewRow();
                }
                object[] rItemArray = new object[r.ItemArray.Length];
                rItemArray[tb.Columns.IndexOf("id")] = id;
                rItemArray[tb.Columns.IndexOf("ItemOfBank")] = itemOfBank;
                rItemArray[tb.Columns.IndexOf("Value")] = value;
                r.ItemArray = rItemArray;
                if (rs.Length <= 0)
                {
                    tb.Rows.Add(r);
                }
                tb.AcceptChanges();
                _dataSet.WriteXml(_fileName, XmlWriteMode.WriteSchema);
            }
        }
    
        /// <summary>
        /// 比较版本大小,为0则相等,为1则ver1大于ver2,为-1则ver1小于ver2
        /// </summary>
        /// <param name="ver1"></param>
        /// <param name="ver2"></param>
        /// <returns></returns>
        public static int CompareVer(string ver1,string ver2)
        {
            int result = 0;
            string[] ss1 = ver1.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] ss2 = ver2.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            int i=0;
            if (ss1.Length >= ss2.Length)
            {
                //以ss2为准
                for (i = 0; i < ss2.Length; i++)
                {
                    result = ss1[i].CompareTo(ss2[i]);
                    if (result != 0)
                        return result;
                }
                //到这里时, 如果长度不等则SS1大
                if (ss1.Length > ss2.Length)
                {
                    return 1;
                }
                else
                    return 0;
            }
            else
            {
                //以ss1为准
                for (i = 0; i < ss1.Length; i++)
                {
                    result = ss1[i].CompareTo(ss2[i]);
                    if (result != 0)
                        return result;
                }
                return 0;
            }
        }
        void CheckSchema()
        {
            string ver=SchemaVersion();
            if (CompareVer(  ver, CurrentSchemaVersion)==-1)
            {
                SchemaUpgrade(ver);
            }
        }

        void SchemaUpgrade(string ver)
        {
            bool b = false;
            Assembly a = Assembly.GetCallingAssembly();
            Type t = a.GetType("UnvaryingSagacity.AccountOfBank.SchemaUpGrade");
            int start = Array.IndexOf(DATASCHEMA_VERSIONS, ver);
            for (int i = start + 1; i < DATASCHEMA_VERSIONS.Length; i++)
            {
                MethodInfo mi = t.GetMethod("UpgradeTo" + DATASCHEMA_VERSIONS[i].Replace('.', '_'));
                b = ((bool)mi.Invoke(null, new object[1] { this._dataSet }));
                if (b)
                {
                    CurrentSchemaVersion = DATASCHEMA_VERSIONS[i];
                }
                else
                    return;
            }
            _dataSet.WriteXml(_fileName, XmlWriteMode.WriteSchema);
        }
        /// <summary>
        /// 
        /// </summary>
        public static DataColumn[] GetSchemaColumns(string tableName)
        {
            DataColumn[] dc;
            if (tableName == TABLE_MAIN)
            {
                dc = new DataColumn[10]{
                            new DataColumn("id",typeof (string ) ),
                            new DataColumn("RecordDate",typeof (string ) ),
                            new DataColumn("entry", typeof(int)),
                            new DataColumn("VchType", typeof(string)),
                            new DataColumn("VchNumber", typeof(string)),
                            new DataColumn("ChequeType", typeof(string)),
                            new DataColumn("ChequeId", typeof(string)),
                            new DataColumn("Digest", typeof(string)),
                            new DataColumn("Side", typeof(int)),
                            new DataColumn("Money", typeof(double)),
                        };
            }
            else if (tableName == TABLE_ITEMOFBANKS)
            {
                dc = new DataColumn[5]{
                            new DataColumn("nm",typeof (string ) ),
                            new DataColumn("id", typeof(string)),
                            new DataColumn("Digest", typeof(string)),
                            new DataColumn("OfBankName", typeof(string)),
                            new DataColumn ("startBal",typeof (double )),
                        };
            }
            else if (tableName == TABLE_CONFIG)
            {
                dc = new DataColumn[3]{
                            new DataColumn("id", typeof(string)),
                            new DataColumn("ItemOfBank", typeof(string)),
                            new DataColumn ("Value",typeof (string )),
                        };
            }
            else if (tableName == TABLE_ACCOUNTYEARCLOSED)
            {
                dc = new DataColumn[6]{
                            new DataColumn("Item", typeof(string)),
                            new DataColumn("FYear", typeof(int)),
                            new DataColumn ("StartBal",typeof (double )),
                            new DataColumn ("EndBal",typeof (double )),
                            new DataColumn ("ClosedDate",typeof (string )),
                            new DataColumn ("Closed",typeof (int )),
                        };
            }
            else
                dc = new DataColumn[0];
            return dc;
        }
        /// <summary>
        /// 增加一个新的分录,返回该分录的id
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public int StorgeEntry(Entry entry,ItemOfBank item)
        {
            DataRow[] rs = GetDataRows("id='"+item.ID +"' AND RecordDate" + "='" + entry.Date + "'", "entry" + " DESC");
            int id = 0;
            if (rs.Length >0)
            {
                id = int.Parse(rs[0].ItemArray[2].ToString()) + 1;
            }
            else
                id = 1;
            try
            {
                lock (_dataSet)
                {
                    DataRow r = _dataSet.Tables[TABLE_MAIN].NewRow();
                    object[] rItemArray = new object[r.ItemArray.Length];
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("id")] = item.ID;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("RecordDate")] = entry.Date;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("entry")] = id;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("Digest")] = entry.Digest;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("VchType")] = entry.VchType;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("VchNumber")] = entry.VchNumber;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("ChequeType")] = entry.ChequeType;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("ChequeId")] = entry.Cheque;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("Side")] = entry.Side;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("Money")] = entry.Money;
                    r.ItemArray = rItemArray;
                    _dataSet.Tables[TABLE_MAIN].Rows.Add(r);
                    _dataSet.Tables[TABLE_MAIN].AcceptChanges();
                    _dataSet.WriteXml(_fileName, XmlWriteMode.WriteSchema);
                }
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 插入分录,以entry.ID的绝对值,所有ID>=abs(entry.ID)的都+1,该分录仍是加在文件最后.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool InsertEntry(Entry entry, ItemOfBank item)
        {
            int id = Math.Abs(entry.ID);
            DataRow[] rs = GetDataRows("id='" + item.ID + "' AND RecordDate='" + entry.Date + "' AND entry>=" + id, "entry DESC");

            lock (_dataSet)
            {
                //现存的分录号都加1;
                foreach( DataRow row in rs)
                {
                    object[] rowArray = new object[row.ItemArray.Length];
                    row.ItemArray.CopyTo(rowArray, 0);
                    rowArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("entry")] = ((int)row["entry"]) + 1;
                    row.ItemArray = rowArray;
                }
                //增加新的分录
                DataRow r = _dataSet.Tables[TABLE_MAIN].NewRow();
                object[] rItemArray = new object[r.ItemArray.Length];
                rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("id")] = item.ID;
                rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("RecordDate")] = entry.Date;
                rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("entry")] = id;
                rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("Digest")] = entry.Digest;
                rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("VchType")] = entry.VchType;
                rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("VchNumber")] = entry.VchNumber;
                rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("ChequeType")] = entry.ChequeType;
                rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("ChequeId")] = entry.Cheque;
                rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("Side")] = entry.Side;
                rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("Money")] = entry.Money;
                r.ItemArray = rItemArray;
                _dataSet.Tables[TABLE_MAIN].Rows.Add(r);
                _dataSet.Tables[TABLE_MAIN].AcceptChanges();
                _dataSet.WriteXml(_fileName, XmlWriteMode.WriteSchema);
                entry.ID = id;
            }
            return true; 
        }

        public bool UpdateEntry(Entry entry, ItemOfBank item)
        {
            DataRow[] rs = GetDataRows("id='" + item.ID + "' AND RecordDate" + "='" + entry.Date + "' AND entry=" + entry.ID, "id");
            
            lock (_dataSet)
            {
                if (rs.Length > 0)
                {
                    DataRow r = rs[0];
                    object[] rItemArray = new object[r.ItemArray.Length];
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("id")] = item.ID;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("RecordDate")] = entry.Date;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("entry")] = entry.ID;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("Digest")] = entry.Digest;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("VchType")] = entry.VchType;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("VchNumber")] = entry.VchNumber;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("ChequeType")] = entry.ChequeType;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("ChequeId")] = entry.Cheque;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("Side")] = entry.Side;
                    rItemArray[_dataSet.Tables[TABLE_MAIN].Columns.IndexOf("Money")] = entry.Money;
                    r.ItemArray = rItemArray;
                    _dataSet.Tables[TABLE_MAIN].AcceptChanges();
                    _dataSet.WriteXml(_fileName, XmlWriteMode.WriteSchema);
                }
            }
            return true; 
        }

        public bool RemoveEntry(Entry entry, ItemOfBank item)
        {
            return DeleteRecord("id='" + item.ID + "' AND RecordDate" + "='" + entry.Date + "' AND entry=" + entry.ID);
        }

        public bool DeleteRecord(string filterExpress)
        {
            return DeleteRecord(TABLE_MAIN, filterExpress);
        }
        public bool DeleteRecord(string tablename, string filterExpress)
        {
            DataRow[] rs = GetDataRows(tablename, filterExpress, "");
            if (rs.Length > 0)
            {
                foreach (DataRow r in rs)
                {
                    r.Delete();
                }
                try
                {
                    _dataSet.Tables[tablename].AcceptChanges();
                    _dataSet.WriteXml(_fileName, XmlWriteMode.WriteSchema);
                    return true;
                }
                catch { return false; }
            }
            return false;
        }

        public bool AppendItemOfBank( ItemOfBank item)
        {
            try
            {
                lock (_dataSet)
                {
                    DataTable tb= _dataSet.Tables[TABLE_ITEMOFBANKS ];
                    DataRow r =tb.NewRow();
                    object[] rItemArray = new object[r.ItemArray.Length];
                    rItemArray[tb.Columns.IndexOf("nm")] = item.Name ;
                    rItemArray[tb.Columns.IndexOf("id")] = item.ID;
                    rItemArray[tb.Columns.IndexOf("OfBankName")] = item.OfBankName;
                    rItemArray[tb.Columns.IndexOf("Digest")] = item.Description;
                    rItemArray[tb.Columns.IndexOf("StartBal")] = item.StartBal;
                    r.ItemArray = rItemArray;
                    tb.Rows.Add(r);
                    tb.AcceptChanges();
                    _dataSet.WriteXml(_fileName, XmlWriteMode.WriteSchema);
                }
                return true;
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "新增账户", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);  
                return false;
            }
        }
        public bool UpdateItemOfBank(string id, ItemOfBank item)
        {
            try
            {
                DataRow[] rs = GetDataRows(TABLE_ITEMOFBANKS,"id='" + id + "'", "id");

                lock (_dataSet)
                {
                    if (rs.Length > 0)
                    {
                        DataTable tb = _dataSet.Tables[TABLE_ITEMOFBANKS];
                        DataRow r = rs[0];
                        object[] rItemArray = new object[r.ItemArray.Length];
                        rItemArray[tb.Columns.IndexOf("nm")] = item.Name;
                        rItemArray[tb.Columns.IndexOf("id")] = item.ID;
                        rItemArray[tb.Columns.IndexOf("OfBankName")] = item.OfBankName;
                        rItemArray[tb.Columns.IndexOf("Digest")] = item.Description;
                        rItemArray[tb.Columns.IndexOf("StartBal")] = item.StartBal ;
                        r.ItemArray = rItemArray;
                        tb.AcceptChanges();
                        _dataSet.WriteXml(_fileName, XmlWriteMode.WriteSchema);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "修改账户", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                return false;
            }
        }

        public ItemOfBankCollection GetItemOfBankList()
        {
            ItemOfBankCollection items = new ItemOfBankCollection();
            DataRow[] rows = GetDataRows(DataProvider.TABLE_ITEMOFBANKS, "", "nm");
            foreach (DataRow row in rows)
            {
                ItemOfBank item = new ItemOfBank();
                item.Name = row["nm"].ToString();
                item.ID = row["id"].ToString();
                item.OfBankName = row["OfBankName"].ToString();
                item.Description = row["Digest"].ToString();
                item.StartBal = row["StartBal"].ToString() == "" ? 0 : ((double)row["StartBal"]);
                items.Add(item);
            }
            return items;
        }

        public ItemOfBank GetItemOfBank(string id)
        {
            DataRow[] rows = GetDataRows(DataProvider.TABLE_ITEMOFBANKS, "id='" + id + "'", "nm");
            DataRow row = rows[0];
            ItemOfBank item = new ItemOfBank();
            item.Name = row["nm"].ToString();
            item.ID = row["id"].ToString();
            item.OfBankName = row["OfBankName"].ToString();
            item.Description = row["Digest"].ToString();
            item.StartBal = row["StartBal"].ToString() == "" ? 0 : ((double)row["StartBal"]);
            return item;
        }

        public bool SetYearClosed(int year)
        {
            ItemOfBankCollection items=GetItemOfBankList ();
            DateTime start = Core.General.FromString(GetConfig(CONFIG_STARTDATE)+"0101");
            foreach (ItemOfBank item in items)
            {
                YearClosed yc = new YearClosed();
                yc.Year = year;
                yc.ItemOfBankID = item.ID;
                yc.CloseDate = DateTime.Today;
                yc.Closed = true;
                if (year > start.Year)
                {
                    YearClosed pre = GetYearClosed(year - 1, item);
                    yc.StartBal = pre.EndBal;
                }
                else
                {
                    yc.StartBal = item.StartBal;
                }
                ///
                ///开始计算余额01-01~12-31
                ///
                string filterExpress = "";
                filterExpress = "id='" + item.ID + "' AND (RecordDate >='" + year + "0101' AND RecordDate <='" + year + "1231')";
                double bal = yc.StartBal;
                DataRow[] drs = GetDataRows(filterExpress, "RecordDate");
                if (drs != null)
                {
                    foreach (DataRow dr in drs)
                    {
                        if (((int)dr["Side"]) == 1)
                        {
                            bal = bal + (double)dr["money"];
                        }
                        else
                        {
                            bal = bal - (double)dr["money"];
                        }
                    }
                }
                yc.EndBal = bal;
                ///
                ///写余额
                ///
                DataRow[] rs = GetDataRows(TABLE_ACCOUNTYEARCLOSED, "Item='" + item.ID + "' AND FYear=" + yc.Year, "");
                try
                {
                    lock (_dataSet)
                    {
                        DataRow r;
                        if (rs.Length > 0)
                        {
                            r = rs[0];
                        }
                        else
                        {
                            r = _dataSet.Tables[TABLE_ACCOUNTYEARCLOSED].NewRow();
                        }
                        object[] rItemArray = new object[r.ItemArray.Length];
                        rItemArray[_dataSet.Tables[TABLE_ACCOUNTYEARCLOSED].Columns.IndexOf("item")] = yc.ItemOfBankID;
                        rItemArray[_dataSet.Tables[TABLE_ACCOUNTYEARCLOSED].Columns.IndexOf("FYear")] = yc.Year;
                        rItemArray[_dataSet.Tables[TABLE_ACCOUNTYEARCLOSED].Columns.IndexOf("StartBal")] = yc.StartBal ;
                        rItemArray[_dataSet.Tables[TABLE_ACCOUNTYEARCLOSED].Columns.IndexOf("EndBal")] = yc.EndBal ;
                        rItemArray[_dataSet.Tables[TABLE_ACCOUNTYEARCLOSED].Columns.IndexOf("ClosedDate")] = Core.General.FromDateTime(yc.CloseDate);
                        rItemArray[_dataSet.Tables[TABLE_ACCOUNTYEARCLOSED].Columns.IndexOf("Closed")] = yc.Closed ? 1 : 0;
                        r.ItemArray = rItemArray;
                        if (rs.Length <= 0)
                        {
                            _dataSet.Tables[TABLE_ACCOUNTYEARCLOSED].Rows.Add(r);
                        }
                        _dataSet.Tables[TABLE_ACCOUNTYEARCLOSED].AcceptChanges();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "结账错误", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    return false;
                }
            }
            _dataSet.WriteXml(_fileName, XmlWriteMode.WriteSchema);
            return true;
        }

        public YearClosed GetYearClosed(int year, ItemOfBank item)
        {
            YearClosed yc = new YearClosed();
            bool b =true ;
            string filterExpress;
            if (item != null)
            {
                filterExpress = "item='" + item.ID + "' AND FYear=" + year;
                yc.ItemOfBankID = item.ID;
            }
            else
            {
                filterExpress = "FYear=" + year;
                yc.ItemOfBankID = "";
            }
            yc.Year = year;
            DataRow[] drs = GetDataRows(TABLE_ACCOUNTYEARCLOSED, filterExpress, "");
            if (drs.Length > 0)
            {
                foreach (DataRow dr in drs)
                {
                    if (((int)dr["closed"])==0)
                    {
                        b = false;
                    }
                    yc.EndBal += (double)dr["endbal"];
                    yc.StartBal += (double)dr["StartBal"]; ;
                    yc.CloseDate = Core.General.FromString(dr["closedDate"].ToString());
                }
            }
            else
            {
                b = false;
                yc.CloseDate = Core.General.FromString("19721102");
            }
            yc.Closed = b;
            return yc;
        }


        public YearClosed GetYearClosed(int year)
        {
            return GetYearClosed(year, null); 
        }

        public bool IsClosed(int year, ItemOfBank item)
        {
            double endBal = 0;
            return IsClosed(year, ref endBal, item);
        }

        public bool IsClosed(int year, ref double endBal)
        {
            return IsClosed(year, ref endBal, null);
        }

        public bool IsClosed(int year, ref double endBal, ItemOfBank item)
        {
            YearClosed y = GetYearClosed(year, item);
            endBal = y.EndBal ;
            return y.Closed;
        }

        /// <summary>
        /// 这里未使用余额表中的记录
        /// </summary>
        /// <param name="d"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public double GetStartBal(DateTime d,ItemOfBank item)
        {
            string filterExpress = "";
            if (item == null)
                filterExpress = "RecordDate" + "<'" + Core.General.FromDateTime(d) + "'";
            else
                filterExpress = "id='" + item.ID + "' AND RecordDate" + "<'" + Core.General.FromDateTime(d) + "'";
            int startYear=int.Parse ( GetConfig(CONFIG_STARTDATE ));
            double bal=0;
            if (item == null)
            {
                ItemOfBankCollection items = GetItemOfBankList();
                foreach (ItemOfBank it in items)
                {
                    bal += it.StartBal;
                }
            }
            else
            {
                bal = item.StartBal;
            }
            DataRow[] drs = GetDataRows(filterExpress, "RecordDate");
            if (drs != null)
            {
                foreach (DataRow dr in drs)
                {
                    if (((int)dr["Side"]) == 1)
                    {
                        bal = bal + (double)dr["money"];
                    }
                    else
                    {
                        bal = bal - (double)dr["money"];
                    }
                }
            }
            return bal;
        }


        public EntryExList GetEntryExList(double bal,string fristDigest ,string filterExpress, string sort,bool CloseYearDigest,DateTime startDate,bool allowDailyTotal)
        {
            EntryExList entries = new EntryExList();
            double d = bal;
            double dy1 = 0;
            double dm1 = 0;
            double dDaily1 = 0;
            double dy2 = 0;
            double dm2 = 0;
            double dDaily2 = 0;
            int entryID = 0;

            DataRow[] drs = GetDataRows(filterExpress, sort);

            EntryEx entryStart = new EntryEx();
            entryStart.ID = entryID;
            entryID++;
            entryStart.Digest = fristDigest;// "上期余额";
            entryStart.Side = 0;
            entryStart.Bal = d;
            entryStart.Date = Core.General.FromDateTime(startDate);    
            entries.Add(entryStart);
            int y = 0;
            int m = 0;
            int daily = 0;
            if (drs != null)
            {
                foreach (DataRow dr in drs)
                {
                    DateTime dt = Core.General.FromString(dr["RecordDate"].ToString());
                    EntryEx entry = new EntryEx();
                    #region 写每日小计,每月合计,本期累计
                    if (daily != 0 && daily != dt.Day && allowDailyTotal)
                    {
                        entry = new EntryEx();
                        entry.ID = entryID;
                        entryID++;
                        entry.Digest = "本日小计";
                        entry.JMoney = dDaily1;
                        entry.DMoney = dDaily2;
                        entry.Bal = d;
                        entry.Side = 0;
                        entries.Add(entry);
                        dDaily1 = 0;
                        dDaily2 = 0;
                    }
                    if (m != 0 && m != dt.Month)
                    {
                        entry = new EntryEx();
                        entry.ID = entryID;
                        entryID++;
                        entry.Digest = "本月合计";
                        entry.JMoney = dm1;
                        entry.DMoney = dm2;
                        entry.Bal = d;
                        entry.Side = 0;
                        entries.Add(entry);
                        dm1 = 0;
                        dm2 = 0;
                        if (!(y != 0 && y != dt.Year))
                        {
                            //同时写"本年累计",注意dy1,dy2不要归零
                            entry = new EntryEx();
                            entry.ID = entryID;
                            entryID++;
                            entry.Digest = "本年累计";
                            entry.JMoney = dy1;
                            entry.DMoney = dy2;
                            entry.Bal = d;
                            entry.Side = 0;
                            entries.Add(entry);
                        }
                    }
                    if (y != 0 && y != dt.Year)
                    {
                        entry = new EntryEx();
                        entry.ID = entryID;
                        entryID++;
                        entry.Digest = "本年累计";
                        entry.JMoney = dy1;
                        entry.DMoney = dy2;
                        entry.Bal = d;
                        entry.Side = 0;
                        entries.Add(entry);
                        dy1 = 0;
                        dy2 = 0;
                    }
                    #endregion
                    entry = new EntryEx();
                    entry.ID = entryID;
                    entryID++;
                    entry.Date = dr["RecordDate"].ToString();
                    entry.Digest = dr["digest"].ToString();
                    entry.VchType = dr["VchType"].ToString();
                    entry.VchNumber = dr["VchNumber"].ToString();
                    entry.ChequeType  = dr["ChequeType"].ToString();
                    entry.Cheque = dr["ChequeId"].ToString();
                    entry.Money = (double)(dr["money"]);
                    entry.Side = (int)dr["side"];
                    if (entry.Side == 1)
                    {
                        d += entry.Money;
                        dDaily1 += entry.Money;
                        dm1 += entry.Money;
                        dy1 += entry.Money;
                    }
                    else
                    {
                        d -= entry.Money;
                        dDaily2 += entry.Money;
                        dm2 += entry.Money;
                        dy2 += entry.Money;
                    }
                    entry.Bal = d;
                    entries.Add(entry);
                    y = dt.Year;
                    m = dt.Month;
                    daily = dt.Day;
                }
            }
            #region 写每日小计,每月合计,本年累计

            if (allowDailyTotal)
            {
                entryStart = new EntryEx();
                entryStart.ID = entryID;
                entryID++;
                entryStart.Digest = "本日小计";
                entryStart.JMoney = dDaily1;
                entryStart.DMoney = dDaily2;
                entryStart.Bal = d;
                entryStart.Side = 0;
                entries.Add(entryStart);
                dDaily1 = 0;
                dDaily2 = 0;
            }
            entryStart = new EntryEx();
            entryStart.ID = entryID;
            entryID++;
            entryStart.Digest = "本月合计";
            entryStart.JMoney = dm1;
            entryStart.DMoney = dm2;
            entryStart.Bal = d;
            entryStart.Side = 0;
            entries.Add(entryStart);
            dm1 = 0;
            dm2 = 0;

            entryStart = new EntryEx();
            entryStart.ID = entryID;
            entryID++;
            entryStart.Digest = "本年累计";
            entryStart.JMoney = dy1;
            entryStart.DMoney = dy2;
            entryStart.Bal = d;
            entryStart.Side = 0;
            entries.Add(entryStart);

            if (CloseYearDigest)
            {
                entryStart = new EntryEx();
                entryStart.ID = entryID;
                entryID++;
                entryStart.Digest = "结转下年";
                entryStart.JMoney = 0;
                entryStart.DMoney = 0;
                entryStart.Bal = d;
                entryStart.Side = 0;
                entries.Add(entryStart);
            }
            dy1 = 0;
            dy2 = 0;
            #endregion

            return entries;
        }

        public EntryExList GetEntryList(string filterExpress, string sort)
        {
            EntryExList entries = new EntryExList();

            DataRow[] drs = GetDataRows(filterExpress, sort);
            if (drs != null)
            {
                foreach (DataRow dr in drs)
                {
                    DateTime dt = Core.General.FromString(dr["RecordDate"].ToString());
                    Entry entry = new Entry();
                    entry = new Entry();
                    entry.ID = (int)dr["entry"];
                    entry.Date = dr["RecordDate"].ToString();
                    entry.Digest = dr["digest"].ToString();
                    entry.VchType = dr["VchType"].ToString();
                    entry.VchNumber = dr["VchNumber"].ToString () ;
                    entry.ChequeType = dr["ChequeType"].ToString();
                    entry.Cheque = dr["ChequeId"].ToString();
                    entry.Money = (double)(dr["money"]);
                    entry.Side = (int)dr["side"];
                    entries.Add(entry);
                }
            }
            return entries;
        }
    }

    class SchemaUpGrade
    {
        public static bool UpgradeTo1_0_2010_1_25_0(DataSet dSet)
        {
            if (dSet.Tables.IndexOf(DataProvider.TABLE_CONFIG) < 0)
            {
                DataColumn[]  dc = new DataColumn[3]{
                            new DataColumn("id", typeof(string)),
                            new DataColumn("ItemOfBank", typeof(string)),
                            new DataColumn ("Value",typeof (string )),
                        };
                DataTable dataTable = new DataTable(DataProvider.TABLE_CONFIG);
                dataTable.Columns.AddRange(dc);
                dataTable.PrimaryKey = new DataColumn[2]{
                                        dc[0],
                                        dc[1],
                                                         };
                dSet.Tables.Add(dataTable);
            }
            if (dSet.Tables[DataProvider.TABLE_MAIN].Columns.IndexOf("VchType") < 0)
            {
                DataColumn dcVchType = new DataColumn("VchType");
                dcVchType.DataType = typeof(string);
                dSet.Tables[DataProvider.TABLE_MAIN].Columns.Add(dcVchType);
            }
            if (dSet.Tables[DataProvider.TABLE_MAIN].Columns.IndexOf("VchNumber") < 0)
            {
                DataColumn dcVchNumber = new DataColumn("VchNumber");
                dcVchNumber.DataType = typeof(string);
                dSet.Tables[DataProvider.TABLE_MAIN].Columns.Add(dcVchNumber);
            }
            if (dSet.Tables[DataProvider.TABLE_MAIN].Columns.IndexOf("ChequeType") < 0)
            {
                DataColumn dcChequeType = new DataColumn("ChequeType");
                dcChequeType.DataType = typeof(string);
                dSet.Tables[DataProvider.TABLE_MAIN].Columns.Add(dcChequeType);
            }
            if (dSet.Tables[DataProvider.TABLE_ITEMOFBANKS].Columns.IndexOf("StartBal") < 0)
            {
                DataColumn dcStartBal = new DataColumn("StartBal");
                dcStartBal.DataType = typeof(double);
                dSet.Tables[DataProvider.TABLE_ITEMOFBANKS].Columns.Add(dcStartBal);
            } 
            return true;
        }

        public static bool UpgradeTo1_0_2010_2_2_0(DataSet dSet)
        {
            if (dSet.Tables.IndexOf(DataProvider.TABLE_ACCOUNTYEARCLOSED) < 0)
            {
                DataColumn[] dc = DataProvider.GetSchemaColumns(DataProvider.TABLE_ACCOUNTYEARCLOSED);
                DataTable dataTable = new DataTable(DataProvider.TABLE_ACCOUNTYEARCLOSED);
                dataTable.Columns.AddRange(dc);
                dataTable.PrimaryKey = new DataColumn[2]{
                                        dc[0],
                                        dc[1],
                                                         };
                dSet.Tables.Add(dataTable);
            }
            return true;
        }
    }

    class Entry
    {
        public string Date { get; set; }
        public int ID { get; set; }
        public string Digest { get; set; }
        public string VchType { get; set; }
        public string VchNumber { get; set; }
        public string ChequeType { get; set; }
        public string Cheque { get; set; }
        public int Side { get; set; }
        public double Money { get; set; }
        public bool Changed { get; set; }
        public virtual  void CopyTo(object dst)
        {
            Entry en = dst as Entry;
            if (en != null)
            {
                en.Date = Date;
                en.ID = ID;
                en.Digest = Digest;
                en.VchType = VchType;
                en.VchNumber = VchNumber;
                en.ChequeType = ChequeType;
                en.Cheque = Cheque;
                en.Side = Side;
                en.Money = Money;
            }
        }
    }

    class EntryEx : Entry
    {
        public double JMoney { get; set; }
        public double DMoney { get; set; }
        public double Bal { get; set; }

        public override void CopyTo(object dst)
        {
            EntryEx en = dst as EntryEx;
            if (en != null)
            {
                base.CopyTo(en);
                en.JMoney = JMoney;
                en.DMoney = DMoney;
                en.Bal = Bal;
            }
        }
        
    }
    class EntryExList : List<Entry>
    {
    }
    class ViewFiter
    {
        public DateTime d1 { get; set; }
        public DateTime d2 { get; set; }

        public override string ToString()
        {
            return d1.ToLongDateString() + " 到 " + d2.ToLongDateString();
        }

        public string ToString(string preFixString)
        {
            return preFixString + d1.ToLongDateString() + " 到 " + d2.ToLongDateString();
        }
    }
}
