using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient; 
 
namespace UnvaryingSagacity.Data.DataSchemaProvider
{
    /// <summary>
    /// 互斥操作的对象状态
    /// </summary>
    public enum MutexStatus
    {
        开始,
        进行中,
        停止,
    }

    /// <summary>
    /// 互斥操作的对象
    /// </summary>
    public class ObjectMutex
    {
        public string ID{get;set;}
        /// <summary>
        /// 超时,以秒为计
        /// </summary>
        public int Timeout { get; set; }
        public string UserName { get; set; }
        public string Digest { get; set; }
    }

    public class DataColumnEx : DataColumn
    {
        public DataColumnEx()
        {
            base.AllowDBNull = false;
            base.MaxLength = 50;
        }

        public string JetDBType
        {
            get
            {
                string s = "varchar";
                switch (DataType)
                {
                    case DbType.AnsiString:
                    case DbType.AnsiStringFixedLength:
                    case DbType.String:
                    case DbType.StringFixedLength:
                    case DbType.Guid:
                    case DbType.VarNumeric:
                        s = "varchar";
                        break;
                    case DbType.Binary:
                        s = "memo";
                        break;
                    case DbType.Object:
                        s = "image";
                        break;
                    case DbType.Boolean:
                        s = "bit";
                        break;
                    case DbType.SByte:
                    case DbType.Byte:
                        s = "byte";
                        break;
                    case DbType.Currency:
                        s = "Currency";
                        break;
                    case DbType.Date:
                    case DbType.DateTime:
                    case DbType.DateTimeOffset:
                    case DbType.DateTime2:
                    case DbType.Time:
                        s = "DateTime";
                        break;
                    case DbType.Decimal:
                    case DbType.Single:
                    case DbType.Double:
                        s = "double";
                        break;
                    case DbType.Int16:
                    case DbType.UInt16:
                        s = "smallint";
                        break;
                    case DbType.Int32:
                    case DbType.UInt32:
                        s = "int";
                        break;
                    case DbType.Int64:
                    case DbType.UInt64:
                        s = "long";
                        break;

                }
                return s;
            }
        }

        public string SqlDBType
        {
            get
            {
                string s = "nvarchar";
                switch (DataType)
                {
                    case DbType.AnsiString:
                    case DbType.String:
                        s="nvarchar";
                        break;
                    case DbType.AnsiStringFixedLength:
                    case DbType.StringFixedLength:
                    case DbType.Guid:
                    case DbType.VarNumeric:
                        s = "nchar";
                        break;
                    case DbType.Binary:
                        s = "nText";
                        break;
                    case DbType.Object:
                        s = "image";
                        break;
                    case DbType.Boolean:
                        s = "bit";
                        break;
                    case DbType.SByte:
                    case DbType.Byte:
                        s = "byte";
                        break;
                    case DbType.Currency:
                        s = "money";
                        break;
                    case DbType.Date:
                    case DbType.DateTime:
                    case DbType.DateTimeOffset:
                    case DbType.DateTime2:
                    case DbType.Time:
                        s = "DateTime";
                        break;
                    case DbType.Single:
                    case DbType.Decimal:
                    case DbType.Double:
                        s = "decimal";
                        break;
                    case DbType.Int16:
                    case DbType.UInt16:
                        s = "smallint";
                        break;
                    case DbType.Int32:
                    case DbType.UInt32:
                        s = "int";
                        break;
                    case DbType.Int64:
                    case DbType.UInt64:
                        s = "bigint";
                        break;

                }
                return s;
            }
        }

        public new System.Data.DbType DataType
        {
            get;
            set;
        }
    }

    public class Sql 
    {
        private SqlConnection _dbConn;

        public Sql() 
        {
            if (Core.General.Check())
            {
                _dbConn = new SqlConnection();
            }
        }

        public ConnectionState Open(string connect)
        {
            if (_dbConn.State != ConnectionState.Closed)
                _dbConn.Close();
            _dbConn.ConnectionString = connect;
            _dbConn.Open();
            return _dbConn.State;
        }

        public SqlConnection Connect{ get { return _dbConn; } set { _dbConn = value; } }

        public bool ExistTable(string tbname)
        {
            StringBuilder sb = new StringBuilder("select id from sysobjects where xtype='U' AND name='");
            sb.Append(tbname);
            sb.Append("'");
            string sql = sb.ToString();

            SqlCommand cm = new SqlCommand(sql, _dbConn);
            object result = cm.ExecuteScalar();
            if (result != null)
                return true;
            return false;
        }

        public bool ExistColumn(string tbname, string columnname)
        {
            StringBuilder sb = new StringBuilder("select t1.name from syscolumns t1 inner join sysobjects t2 on t1.id=t2.id where t2.name='");
            sb.Append(tbname);
            sb.Append("' AND t2.xtype='U' AND t1.name='");
            sb.Append(columnname);
            sb.Append("'");
            string sql = sb.ToString();

            SqlCommand cm = new SqlCommand(sql, _dbConn);
            object result = cm.ExecuteScalar();
            if (result != null)
                return true;
            return false;
        }

        public bool ExistIndex(string name)
        {
            string sql = "select id from sysindexes where name='" + name + "'";
            SqlCommand cm = new SqlCommand(sql, _dbConn);
            object result = cm.ExecuteScalar();
            if (result != null)
                return true;
            return false;
        }

        public static bool ExistDB(SqlConnection connectMaster ,string dbname)
        {
            StringBuilder sb = new StringBuilder("select dbid from sysdatabases where name='");
            sb.Append(dbname);
            sb.Append("'");
            string sql = sb.ToString();

            SqlCommand cm = new SqlCommand(sql, connectMaster);
            object result = cm.ExecuteScalar();
            if(result  !=null )
                return true ;
            return false;
        }

        public static bool CreateDatabaseByDefaultInfo(SqlConnection connectMaster, string dbname)
        {
            StringBuilder sb = new StringBuilder("CREATE DATABASE [");
            sb.Append(dbname);
            sb.Append("]");
            string sql = sb.ToString();
            try
            {
                SqlCommand cm = new SqlCommand(sql, connectMaster);
                int result = cm.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "CreateDatabaseByDefaultInfo", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                return false;
            }
        }
        public bool CreateTable(string tableName, DataColumnEx[] columns, string[] primaryKeys)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Create table [");
            sb.Append(tableName);
            sb.Append("] (");
            int i = 0;
            foreach (DataColumnEx dc in columns)
            {
                sb.Append("[");
                sb.Append(dc.ColumnName);
                sb.Append("] ");

                sb.Append(dc.SqlDBType );

                if (dc.DataType == DbType.String || dc.DataType == DbType.VarNumeric)
                {
                    if (dc.MaxLength != -1)
                    {
                        sb.Append(" (");
                        sb.Append(dc.MaxLength);
                        sb.Append(") ");
                    }
                }
                
                if (!dc.AllowDBNull && dc.AutoIncrement && (dc.DataType == DbType.Int16 || dc.DataType == DbType.Int32 || dc.DataType == DbType.Int64))
                {   // "IDENTITY (1, 1) NOT NULL ,"
                    sb.Append(" IDENTITY (");
                    sb.Append(dc.AutoIncrementSeed);
                    sb.Append(",");
                    sb.Append(dc.AutoIncrementStep);
                    sb.Append(") ");
                }

                sb.Append(dc.AllowDBNull ? " NULL" : " NOT NULL");
                string defaultValue = dc.DefaultValue.ToString();
                if (defaultValue.Length > 0)
                {
                    if (!Core.General.IsNumberic(defaultValue))
                        sb.Append(" DEFAULT '" + dc.DefaultValue + "'");
                    sb.Append(" DEFAULT " + dc.DefaultValue);
                }
                i++;
                if (i < columns.Length)
                    sb.Append(",");
            }
            sb.Append(")");
            string sql = sb.ToString();
            SqlCommand cm = new SqlCommand(sql, _dbConn);
            try
            {
                cm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message); 
            }

            if (primaryKeys.Length > 0)
            {
                sb.Remove(0, sb.Length);
                sb.Append("ALTER TABLE [");
                sb.Append(tableName);
                sb.Append("] ADD ");
                sb.Append("CONSTRAINT [PK_");
                sb.Append(tableName);
                sb.Append("] PRIMARY KEY");
                sb.Append("(");
                i = 0;
                foreach (string key in primaryKeys)
                {
                    sb.Append("[");
                    sb.Append(key);
                    sb.Append("] ");
                    i++;
                    if (i < primaryKeys.Length)
                        sb.Append(",");
                }
                sb.Append(")");
                sql = sb.ToString();
                cm.CommandText = sql;
                cm.ExecuteNonQuery();
            }
            return true;
        }

        public bool CreatePrimaryKey(string tableName, string primaryName, string[] primaryKeys)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ALTER TABLE [");
            sb.Append(tableName);
            sb.Append("] ADD ");
            sb.Append("CONSTRAINT [");
            sb.Append(primaryName);
            sb.Append("] PRIMARY KEY");
            sb.Append("(");
            int i = 0;
            foreach (string key in primaryKeys)
            {
                sb.Append("[");
                sb.Append(key);
                sb.Append("] ");
                i++;
                if (i < primaryKeys.Length)
                    sb.Append(",");
            }
            sb.Append(")");
            string sql = sb.ToString();
            SqlCommand cm = new SqlCommand(sql, _dbConn);
            cm.ExecuteNonQuery();
            return true;
        }

        public bool AddColumns(string tableName, DataColumnEx[] columns)
        {
            //ALTER TABLE WorkCalendar1 ADD Term1 NCHAR(20) NOT NULL
            StringBuilder sb = new StringBuilder();
            sb.Append("ALTER TABLE [");
            sb.Append(tableName);
            sb.Append("] ADD ");
            int i = 0;
            foreach (DataColumnEx dc in columns)
            {
                sb.Append("[");
                sb.Append(dc.ColumnName);
                sb.Append("] ");

                sb.Append(dc.SqlDBType);

                if (dc.DataType == DbType.String || dc.DataType == DbType.VarNumeric)
                {
                    if (dc.MaxLength != -1)
                    {
                        sb.Append(" (");
                        sb.Append(dc.MaxLength);
                        sb.Append(") ");
                    }
                }
                if (!dc.AllowDBNull && dc.AutoIncrement && (dc.DataType == DbType.Int16 || dc.DataType == DbType.Int32 || dc.DataType == DbType.Int64))
                {   // "IDENTITY (1, 1) NOT NULL ,"
                    sb.Append(" IDENTITY (");
                    sb.Append(dc.AutoIncrementSeed);
                    sb.Append(",");
                    sb.Append(dc.AutoIncrementStep);
                    sb.Append(") ");
                }
                sb.Append(dc.AllowDBNull ? " NULL" : " NOT NULL");
                string defaultValue = dc.DefaultValue.ToString();
                if (defaultValue.Length > 0)
                {
                    if (!Core.General.IsNumberic(defaultValue))
                        sb.Append(" DEFAULT '" + dc.DefaultValue + "'");
                    sb.Append(" DEFAULT " + dc.DefaultValue);
                }
                i++;
                if (i < columns.Length)
                    sb.Append(",");
            }
            string sql = sb.ToString();
            SqlCommand cm = new SqlCommand(sql, _dbConn);
            cm.ExecuteNonQuery();
            return true;
        }

        public bool CreateIndex(string tableName, string indexName,string[] columns)
        {
            return CreateIndex(tableName, indexName,false , columns);
        }

        public bool CreateIndex(string tableName, string indexName, bool isUnique, string[] columns)
        {
            //CREATE UNIQUE INDEX [IX_StudentInterest] ON [dbo].[StudentInterest]([Term], [InterestID]) ON [PRIMARY]
            StringBuilder sb = new StringBuilder();
            sb.Append("Create ");
            if (isUnique)
            {
                sb.Append(" UNIQUE ");
            }
            sb.Append (" INDEX [");
            sb.Append(indexName);
            sb.Append("] ON [");
            sb.Append(tableName);
            sb.Append("] (");
            int i = 0;
            foreach (string c in columns)
            {
                sb.Append("[");
                sb.Append(c);
                sb.Append("] ");

                i++;
                if (i < columns.Length)
                    sb.Append(",");
            }
            sb.Append(")");
            string sql = sb.ToString();
            SqlCommand cm = new SqlCommand(sql, _dbConn);
            cm.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// 这里只修改列的数据类型和长度
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns">要修改的列</param>
        /// <returns></returns>
        public bool AlterColumns(string tableName, DataColumnEx[] columns)
        {
            //ALTER TABLE MyTable ALTER COLUMN NullCOl NVARCHAR(20) NOT NULL
            StringBuilder sb = new StringBuilder();
            sb.Append("ALTER TABLE [");
            sb.Append(tableName);
            sb.Append("] ALTER Column ");
            int i = 0;
            foreach (DataColumnEx dc in columns)
            {
                sb.Append("[");
                sb.Append(dc.ColumnName);
                sb.Append("] ");

                sb.Append(dc.SqlDBType);

                if (dc.DataType == DbType.String || dc.DataType == DbType.VarNumeric)
                {
                    if (dc.MaxLength != -1)
                    {
                        sb.Append(" (");
                        sb.Append(dc.MaxLength);
                        sb.Append(") ");
                    }
                }
                sb.Append(dc.AllowDBNull ? " NULL" : " NOT NULL");
                string defaultValue = dc.DefaultValue.ToString();
                if (defaultValue.Length > 0)
                {
                    if (!Core.General.IsNumberic(defaultValue))
                        sb.Append(" DEFAULT '" + dc.DefaultValue + "'");
                    sb.Append(" DEFAULT " + dc.DefaultValue);
                }
                i++;
                if (i < columns.Length)
                    sb.Append(",");
            }
            string sql = sb.ToString();
            SqlCommand cm = new SqlCommand(sql, _dbConn);
            cm.ExecuteNonQuery();
            return true;
        }

        public bool DropPrimarykey(string tableName, string primaryName)
        {
            //ALTER TABLE [samlpe] DROP CONSTRAINT [PK_samlpe]
            StringBuilder sb = new StringBuilder();
            sb.Append("ALTER TABLE [");
            sb.Append(tableName);
            sb.Append("] DROP CONSTRAINT [");
            sb.Append(primaryName);
            sb.Append("]");
            string sql = sb.ToString();
            SqlCommand cm = new SqlCommand(sql, _dbConn);
            cm.ExecuteNonQuery();
            return true;
        }

        public bool DropIndex(string tableName, string indexName)
        {
            //DROP INDEX authors.au_id_ind
            StringBuilder sb = new StringBuilder();
            sb.Append("DROP INDEX ");
            sb.Append(tableName);
            sb.Append(".");
            sb.Append(indexName);
            string sql = sb.ToString();
            SqlCommand cm = new SqlCommand(sql, _dbConn);
            cm.ExecuteNonQuery();
            return true;
        }

        public bool DropTable(string name)
        {
            StringBuilder sb = new StringBuilder("drop table [");
            sb.Append(name);
            sb.Append("]");
            string sql = sb.ToString();
            SqlCommand cm = new SqlCommand(sql, _dbConn);
            cm.ExecuteNonQuery();
            return true;
        }

        public bool DropFields(string tableName, string[] columns)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ALTER TABLE [");
            sb.Append(tableName);
            sb.Append("] Drop Column ");
            int i = 0;
            foreach (string c in columns)
            {
                sb.Append("[");
                sb.Append(c);
                sb.Append("] ");

                i++;
                if (i < columns.Length)
                    sb.Append(",");
            }
            string sql = sb.ToString();
            SqlCommand cm = new SqlCommand(sql, _dbConn);
            cm.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// 能否开始一个互斥操作,用完后请CloseMutex()
        /// </summary>
        /// <param name="om"></param>
        /// <param name="hasErrorPrompt"></param>
        /// <returns></returns>
        public bool StartMutex(ObjectMutex om, bool hasErrorPrompt)
        {
            MutexStatus stat= GetMutexState(om, true);
            if (stat == MutexStatus.进行中)
            {
                if (hasErrorPrompt)
                    System.Windows.Forms.MessageBox.Show("用户[" + om.UserName + "]正在 " + om.Digest + ", 预计至多剩余" + om.Timeout + "秒. \n\n" + "本次操作被终止, 请稍后再继续执行.", "检查并发操作", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                return false;
            }
            return true;
        }
        /// <summary>检查互斥. 在allowBeginMutex=true时,用完后请CloseMutex()
        /// ,互斥表ObjectMutex字段:ID=LC.Bal.Sum.Year.Term;BeginTime:起始点(服务器时间);EndTime:完成点(服务器时间);TimeOut:超时秒;UseName:用户名,
        /// 判断是否互斥 只需要检查(serverdatetime-begintime)是否>timeout即可,
        /// EndTime可以不填
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="timeOutSec"></param>
        /// <param name="UserName"></param>
        /// <param name="Digest"></param>
        /// <param name="allowBeginMutex"></param>
        /// <returns></returns>
        public MutexStatus GetMutexState(ObjectMutex om, bool allowBeginMutex)
        {
            int i = 0;
            MutexStatus b;
            #region 建立互斥表
            if (!ExistTable("ObjectMutex"))
            {
                i = 0;
                DataColumnEx[] cols = new DataColumnEx[6];
                cols[i] = new DataColumnEx();
                cols[i].ColumnName = "ID";//对象类型
                cols[i].DataType = DbType.String;
                i++;
                cols[i] = new DataColumnEx();
                cols[i].ColumnName = "BeginTime";//对象枚举
                cols[i].DataType = DbType.DateTime;
                i++;
                cols[i] = new DataColumnEx();
                cols[i].ColumnName = "EndTime";//系统内置
                cols[i].DataType = DbType.DateTime ;
                cols[i].AllowDBNull =true ;
                i++;
                cols[i] = new DataColumnEx();
                cols[i].ColumnName = "TimeOut";//对象类型
                cols[i].DataType = DbType.Int32 ;
                i++;
                cols[i] = new DataColumnEx();
                cols[i].ColumnName = "UserName";//对象枚举
                cols[i].DataType = DbType.String;
                i++;
                cols[i] = new DataColumnEx();
                cols[i].ColumnName = "Digest";//系统内置
                cols[i].DataType = DbType.String;
                CreateTable("ObjectMutex", cols, new string[1] { "ID" });
            }
            #endregion
            StringBuilder sb = new StringBuilder();
            sb.Append("select datediff(s,BeginTime,T2.d) as dd,TimeOut,UserName,Digest from (");
            sb.Append(" SELECT getdate() as d,BeginTime,TimeOut,UserName,Digest from ObjectMutex");
            sb.Append(" Where ID='" + om.ID);
            sb.Append("') T2");
            string sql = sb.ToString();
            SqlCommand cm = new SqlCommand(sql, _dbConn);
            SqlDataReader rs = cm.ExecuteReader();
            if (!rs.HasRows)
            {
                b = MutexStatus.停止;
            }
            else
            {
                rs.Read();
                int d=int.Parse(rs.GetValue(0).ToString());
                int to=int.Parse(rs.GetValue(1).ToString());
                if (d > to)
                {
                    b = MutexStatus.停止;
                }
                else
                {
                    om.UserName = rs.GetString(2).Trim ();
                    om.Digest = rs.GetString(3).Trim();
                    om.Timeout = to - d;
                    b = MutexStatus.进行中;
                }
            }
            rs.Close();
            if (b == MutexStatus.停止 && allowBeginMutex)
            {
                cm = new SqlCommand("delete from ObjectMutex Where ID='" + om.ID + "'", _dbConn);
                cm.ExecuteNonQuery();
                sb = new StringBuilder();
                sb.Append("INSERT INTO ObjectMutex ([ID], [BeginTime], [TimeOut], [UserName], [Digest])");
                sb.Append(" VALUES('");
                sb.Append(om.ID);
                sb.Append("', getdate() ,");
                sb.Append(om.Timeout);
                sb.Append(",'" + om.UserName);
                sb.Append("','" + om.Digest + "')");
                sql = sb.ToString();
                cm = new SqlCommand(sql, _dbConn);
                i = cm.ExecuteNonQuery();
                if (i > 0)
                {
                    b = MutexStatus.开始;
                }
            }
            return b;
        }

        public void CloseMutex(string id)
        {
            SqlCommand cm = new SqlCommand("Delete from ObjectMutex Where ID='" + id  + "'", _dbConn);
            cm.ExecuteNonQuery();
        }

    }

    public class JetDB
    {
        private OleDbConnection _dbConn;

        public JetDB() 
        {
            _dbConn = new OleDbConnection();
        }

        public ConnectionState Open(string connect)
        {
            if (_dbConn.State != ConnectionState.Closed)
                _dbConn.Close();
            _dbConn.ConnectionString = connect;
            _dbConn.Open();
            return _dbConn.State;
        }

        public OleDbConnection Connect { get { return _dbConn; } set { _dbConn = value; } }

        public bool CreateTable(string tableName, DataColumnEx[] columns, string[] primaryKeys)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Create table [");
            sb.Append(tableName);
            sb.Append("] (");
            int i = 0;
            foreach (DataColumnEx dc in columns)
            {
                sb.Append("[");
                sb.Append(dc.ColumnName);
                sb.Append("] ");

                sb.Append(dc.JetDBType);

                if (dc.DataType == DbType.String || dc.DataType == DbType.VarNumeric)
                {
                    if (dc.MaxLength != -1)
                    {
                        sb.Append(" (");
                        sb.Append(dc.MaxLength);
                        sb.Append(") ");
                    }
                }
                sb.Append(dc.AllowDBNull ? " NULL" : " NOT NULL");
                string defaultValue = dc.DefaultValue.ToString();
                if (defaultValue.Length > 0)
                {
                    if (!Core.General.IsNumberic(defaultValue))
                        sb.Append(" DEFAULT '" + dc.DefaultValue + "'");
                    sb.Append(" DEFAULT " + dc.DefaultValue);
                }
                i++;
                if (i < columns.Length)
                    sb.Append(",");
            }
            sb.Append(")");
            string sql = sb.ToString();
            OleDbCommand cm = new OleDbCommand(sql, _dbConn);
            cm.ExecuteNonQuery();

            if (primaryKeys.Length > 0)
            {
                sb.Remove(0, sb.Length);
                sb.Append("ALTER TABLE [");
                sb.Append(tableName);
                sb.Append("] ADD ");
                sb.Append("CONSTRAINT [PK_");
                sb.Append(tableName);
                sb.Append("] PRIMARY KEY");
                sb.Append("(");
                i = 0;
                foreach (string key in primaryKeys)
                {
                    sb.Append("[");
                    sb.Append(key);
                    sb.Append("] ");
                    i++;
                    if (i < primaryKeys.Length)
                        sb.Append(",");
                }
                sb.Append(")");
                sql = sb.ToString();
                cm.CommandText = sql;
                cm.ExecuteNonQuery();
            }
            return true;
        }

        public bool CreatePrimaryKey(string tableName, string primaryName, string[] primaryKeys)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ALTER TABLE [");
            sb.Append(tableName);
            sb.Append("] ADD ");
            sb.Append("CONSTRAINT [");
            sb.Append(primaryName);
            sb.Append("] PRIMARY KEY");
            sb.Append("(");
            int i = 0;
            foreach (string key in primaryKeys)
            {
                sb.Append("[");
                sb.Append(key);
                sb.Append("] ");
                i++;
                if (i < primaryKeys.Length)
                    sb.Append(",");
            }
            sb.Append(")");
            string sql = sb.ToString();
            OleDbCommand cm = new OleDbCommand(sql, _dbConn);
            cm.ExecuteNonQuery();
            return true;
        }

        public bool AddColumns(string tableName, DataColumnEx[] columns)
        {
            //ALTER TABLE WorkCalendar1 ADD Term1 NCHAR(20) NOT NULL
            StringBuilder sb = new StringBuilder();
            sb.Append("ALTER TABLE [");
            sb.Append(tableName);
            sb.Append("] ADD ");
            int i = 0;
            foreach (DataColumnEx dc in columns)
            {
                sb.Append("[");
                sb.Append(dc.ColumnName);
                sb.Append("] ");

                sb.Append(dc.JetDBType);

                if (dc.DataType == DbType.String || dc.DataType == DbType.VarNumeric)
                {
                    if (dc.MaxLength != -1)
                    {
                        sb.Append(" (");
                        sb.Append(dc.MaxLength);
                        sb.Append(") ");
                    }
                }
                sb.Append(dc.AllowDBNull ? " NULL" : " NOT NULL");
                string defaultValue = dc.DefaultValue.ToString();
                if (defaultValue.Length > 0)
                {
                    if (!Core.General.IsNumberic(defaultValue))
                        sb.Append(" DEFAULT '" + dc.DefaultValue + "'");
                    sb.Append(" DEFAULT " + dc.DefaultValue);
                }
                i++;
                if (i < columns.Length)
                    sb.Append(",");
            }
            string sql = sb.ToString();
            OleDbCommand cm = new OleDbCommand(sql, _dbConn);
            cm.ExecuteNonQuery();
            return true;
        }

        public bool CreateIndex(string tableName, string indexName, string[] columns)
        {
            //CREATE  INDEX [IX_StudentInterest] ON [dbo].[StudentInterest]([Term], [InterestID]) ON [PRIMARY]
            StringBuilder sb = new StringBuilder();
            sb.Append("Create INDEX [");
            sb.Append(tableName);
            sb.Append("] ON [");
            sb.Append(indexName);
            sb.Append("] (");
            int i = 0;
            foreach (string c in columns)
            {
                sb.Append("[");
                sb.Append(c);
                sb.Append("] ");

                i++;
                if (i < columns.Length)
                    sb.Append(",");
            }
            sb.Append(")");
            string sql = sb.ToString();
            OleDbCommand cm = new OleDbCommand(sql, _dbConn);
            cm.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// 这里只修改列的数据类型和长度
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns">要修改的列</param>
        /// <returns></returns>
        public bool AlterColumns(string tableName, DataColumnEx[] columns)
        {
            //ALTER TABLE MyTable ALTER COLUMN NullCOl NVARCHAR(20) NOT NULL
            StringBuilder sb = new StringBuilder();
            sb.Append("ALTER TABLE [");
            sb.Append(tableName);
            sb.Append("] ALTER Column ");
            int i = 0;
            foreach (DataColumnEx dc in columns)
            {
                sb.Append("[");
                sb.Append(dc.ColumnName);
                sb.Append("] ");

                sb.Append(dc.JetDBType);

                sb.Append(dc.SqlDBType);

                if (dc.DataType == DbType.String || dc.DataType == DbType.VarNumeric)
                {
                    if (dc.MaxLength != -1)
                    {
                        sb.Append(" (");
                        sb.Append(dc.MaxLength);
                        sb.Append(") ");
                    }
                } 
                sb.Append(dc.AllowDBNull ? " NULL" : " NOT NULL");
                //这里不能加缺省值
                //string defaultValue = dc.DefaultValue.ToString();
                //if (defaultValue.Length > 0)
                //{
                //    //if(!core.Isnumeric(defaultValue))
                //    //sb.Append(" DEFAULT '" + dc.DefaultValue+"'");
                //    sb.Append(" DEFAULT " + dc.DefaultValue);
                //}
                i++;
                if (i < columns.Length)
                    sb.Append(",");
            }
            string sql = sb.ToString();
            OleDbCommand cm = new OleDbCommand(sql, _dbConn);
            cm.ExecuteNonQuery();
            return true;
        }

        public bool DropPrimarykey(string tableName, string primaryName)
        {
            //ALTER TABLE [samlpe] DROP CONSTRAINT [PK_samlpe]
            StringBuilder sb = new StringBuilder();
            sb.Append("ALTER TABLE [");
            sb.Append(tableName);
            sb.Append("] DROP CONSTRAINT [");
            sb.Append(primaryName);
            sb.Append("]");
            string sql = sb.ToString();
            OleDbCommand cm = new OleDbCommand(sql, _dbConn);
            cm.ExecuteNonQuery();
            return true;
        }

        public bool DropIndex(string tableName, string indexName)
        {
            //DROP INDEX authors.au_id_ind
            StringBuilder sb = new StringBuilder();
            sb.Append("DROP INDEX ");
            sb.Append(tableName);
            sb.Append(".");
            sb.Append(indexName);
            string sql = sb.ToString();
            OleDbCommand cm = new OleDbCommand(sql, _dbConn);
            cm.ExecuteNonQuery();
            return true;
        }

        public bool DropTable(string name)
        {
            StringBuilder sb = new StringBuilder("drop table [");
            sb.Append(name);
            sb.Append("]");
            string sql = sb.ToString();
            OleDbCommand cm = new OleDbCommand(sql, _dbConn);
            cm.ExecuteNonQuery();
            return true;
        }

        public bool DropFields(string tableName, string[] columns)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ALTER TABLE [");
            sb.Append(tableName);
            sb.Append("] Drop Column ");
            int i = 0;
            foreach (string c in columns)
            {
                sb.Append("[");
                sb.Append(c);
                sb.Append("] ");

                i++;
                if (i < columns.Length)
                    sb.Append(",");
            }
            string sql = sb.ToString();
            OleDbCommand cm = new OleDbCommand(sql, _dbConn);
            cm.ExecuteNonQuery();
            return true;
        }

        /// <summary>通过OleDbConnection.GetOleDbSchemaTable()获得目录中的表
        /// 
        /// </summary>
        /// <param name="tbname"></param>
        /// <returns></returns>
        public bool ExistTable(string tbname)
        {
            DataTable schemaTable = _dbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

            foreach (DataRow row in schemaTable.Rows)
            {
                if (tbname.Equals(row["TABLE_NAME"].ToString(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>未完成在Jet下, 检查互斥,
        /// ,互斥表ObjectMutex字段:ID=LC.Bal.Sum.Year.Term;BeginTime:起始点(服务器时间);EndTime:完成点(服务器时间);TimeOut:超时秒;UseName:用户名,
        /// 判断是否互斥 只需要检查(serverdatetime-begintime)是否>timeout即可,
        /// EndTime可以不填
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="timeOutSec"></param>
        /// <param name="UserName"></param>
        /// <param name="Digest"></param>
        /// <param name="allowBeginMutex"></param>
        /// <returns></returns>
        public MutexStatus GetMutexState(ObjectMutex om, bool allowBeginMutex)
        {
            int i = 0;
            MutexStatus b;
            #region 建立互斥表
            if (!ExistTable("ObjectMutex"))
            {
                i = 0;
                DataColumnEx[] cols = new DataColumnEx[6];
                cols[i] = new DataColumnEx();
                cols[i].ColumnName = "ID";//对象类型
                cols[i].DataType = DbType.String;
                i++;
                cols[i] = new DataColumnEx();
                cols[i].ColumnName = "BeginTime";//对象枚举
                cols[i].DataType = DbType.DateTime;
                i++;
                cols[i] = new DataColumnEx();
                cols[i].ColumnName = "EndTime";//系统内置
                cols[i].DataType = DbType.DateTime;
                cols[i].AllowDBNull = true;
                i++;
                cols[i] = new DataColumnEx();
                cols[i].ColumnName = "TimeOut";//对象类型
                cols[i].DataType = DbType.Int32;
                i++;
                cols[i] = new DataColumnEx();
                cols[i].ColumnName = "UserName";//对象枚举
                cols[i].DataType = DbType.String;
                i++;
                cols[i] = new DataColumnEx();
                cols[i].ColumnName = "Digest";//系统内置
                cols[i].DataType = DbType.String;
                CreateTable("ObjectMutex", cols, new string[1] { "ID" });
            }
            #endregion
            StringBuilder sb = new StringBuilder();
            sb.Append("select datediff('s',BeginTime,T2.d) as dd,TimeOut,UserName,Digest from (");
            sb.Append(" SELECT now() as d,BeginTime,TimeOut,UserName,Digest from ObjectMutex");
            sb.Append(" Where ID='" + om.ID);
            sb.Append("') T2");
            string sql = sb.ToString();
            OleDbCommand cm = new OleDbCommand(sql, _dbConn);
            OleDbDataReader rs = cm.ExecuteReader();
            if (!rs.HasRows)
            {
                b = MutexStatus.停止;
            }
            else
            {
                rs.Read();
                int d = int.Parse(rs.GetValue(0).ToString());
                int to = int.Parse(rs.GetValue(1).ToString());
                if (d > to)
                {
                    b = MutexStatus.停止;
                }
                else
                {
                    om.UserName = rs.GetString(2).Trim();
                    om.Digest = rs.GetString(3).Trim();
                    om.Timeout = to - d;
                    b = MutexStatus.进行中;
                }
            }
            rs.Close();
            if (b == MutexStatus.停止 && allowBeginMutex)
            {
                cm = new OleDbCommand("delete from ObjectMutex Where ID='" + om.ID + "'", _dbConn);
                cm.ExecuteNonQuery();
                sb = new StringBuilder();
                sb.Append("INSERT INTO ObjectMutex ([ID], [BeginTime], [TimeOut], [UserName], [Digest])");
                sb.Append(" VALUES('");
                sb.Append(om.ID);
                sb.Append("', now() ,");
                sb.Append(om.Timeout);
                sb.Append(",'" + om.UserName);
                sb.Append("','" + om.Digest + "')");
                sql = sb.ToString();
                cm = new OleDbCommand(sql, _dbConn);
                i = cm.ExecuteNonQuery();
                if (i > 0)
                {
                    b = MutexStatus.开始;
                }
            }
            return b;
        }

        public void CloseMutex(string id)
        {
            OleDbCommand cm = new OleDbCommand("Delete from ObjectMutex Where ID='" + id + "'", _dbConn);
            cm.ExecuteNonQuery();
        }
    }

    public interface IDatabaseCreater
    {

        bool InitEnvment(string SqlMasterConnection, string Databasename);

        bool SchemaCreater(CallbackPromptShower promptShowerCallback);
    }

    public delegate void CallbackPromptShower(string prompt);
}
