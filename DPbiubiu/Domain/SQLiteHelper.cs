using biubiu.Domain.biuMessageBox;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace biubiu.Domain
{
    public class SQLiteHelper
    {
        private static SQLiteConnection _connection;
        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        /// <summary>
        /// 与数据库建立连接
        /// </summary>
        /// <param name="dbPath">数据库路径(包含完整文件名)</param>
        /// <returns></returns>
        public static SQLiteConnection CreateConnection(string dbPath = "./local.db")
        {
            if (_connection == null)
            {
                lock (locker)
                {
                    if (!File.Exists(dbPath))
                    {
                        CreateDatabase(dbPath);
                    }
                    _connection = new SQLiteConnection("DataSource = " + dbPath);
                }
            }
            return _connection;
        }

        private static void CreateDatabase(string dbName)
        {
            if (!string.IsNullOrEmpty(dbName))
            {
                try
                {
                    SQLiteConnection.CreateFile(dbName);


                    /*
                    // 建表语句
                    var createTableSql = @"CREATE TABLE config_ponder (
                    name  TEXT NOT NULL UNIQUE,
                    enable    INTEGER NOT NULL DEFAULT 0,
                    com   TEXT DEFAULT 'COM1',
                    baudrate  INTEGER NOT NULL DEFAULT 1200,
                    data_bits INTEGER NOT NULL DEFAULT 8,
                    parity_value  INTEGER NOT NULL DEFAULT 0,
                    stop_bits_value   INTEGER NOT NULL DEFAULT 1,
                    start_bits_value  INTEGER NOT NULL DEFAULT 2,
                    pond_type_name    TEXT NOT NULL DEFAULT '上海耀华XK3190系列1',
                    camera_id INTEGER,
                    camera_type   TEXT,
                    roadgate_com  TEXT NOT NULL DEFAULT 'COM1',
                    roadgate_in_open  TEXT NOT NULL DEFAULT '0x01,0x05,0x00,0x00,0xFF,0x00,0x8C,0x3A',
                    roadgate_in_close TEXT NOT NULL DEFAULT '0x01,0x05,0x00,0x00,0x00,0x00,0xCD,0xCA',
                    roadgate_out_open TEXT NOT NULL DEFAULT '0x01,0x05,0x00,0x00,0xFF,0x00,0x8C,0x3A',
                    roadgate_out_close    TEXT NOT NULL DEFAULT '0x01,0x05,0x00,0x00,0x00,0x00,0xCD,0xCA',
                    infrared_com  TEXT NOT NULL DEFAULT 'COM1',
                    infrared_in_trigger   TEXT NOT NULL DEFAULT '0x01,0x05,0x00,0x00,0xFF,0x00,0x8C,0x3A',
                    infrared_out_trigger  TEXT NOT NULL DEFAULT '0x01,0x05,0x00,0x00,0xFF,0x00,0x8C,0x3A',
                    PRIMARY KEY('name')); "

                    + @"CREATE TABLE config_type_details (    
                    id    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
                    type  TEXT NOT NULL,	details   TEXT NOT NULL);"

                    + @"CREATE TABLE config_camera (
                    id    INTEGER NOT NULL UNIQUE,
                    type  TEXT NOT NULL,
                    capture_enable    INTEGER NOT NULL DEFAULT 0,
                    ip    TEXT NOT NULL DEFAULT '192.168.31.177',
                    ip1   TEXT NOT NULL DEFAULT '192.168.31.177',
                    ip2   TEXT NOT NULL DEFAULT '192.168.31.177',
                    ip3   TEXT NOT NULL DEFAULT '192.168.31.177',
                    ip4   TEXT NOT NULL DEFAULT '192.168.31.177',
                    port  INTEGER NOT NULL DEFAULT 8000,
                    username  TEXT NOT NULL DEFAULT 'admin',
                    password  TEXT NOT NULL DEFAULT 'admin',
                    channel   INTEGER NOT NULL DEFAULT 1,
                    channel1  INTEGER NOT NULL DEFAULT 33,
                    channel2  INTEGER NOT NULL DEFAULT 34,
                    channel3  INTEGER NOT NULL DEFAULT 35,
                    PRIMARY KEY('id'));";

                    ExecuteNonQuery(createTableSql);
                    // 初始数据 config_type_details
                    var insertSql = @"insert into config_type_details(type, details) values('01_01', '海康威视--硬盘刻录机');"
                                    + @"insert into config_type_details(type, details) values('01_02', '海康威视--网络摄像头');"
                    // config_camera
                                    + @"insert into config_camera(id,type) values(0,'01_01');"
                                    + @"insert into config_camera(id,type) values(1,'01_01');"
                                    + @"insert into config_camera(id,type) values(2,'01_01');"
                                    + @"insert into config_camera(id,type) values(3,'01_01');"
                                    + @"insert into config_camera(id,type) values(4,'01_02');"
                                    + @"insert into config_camera(id,type) values(5,'01_02');"
                                    + @"insert into config_camera(id,type) values(6,'01_02');"
                                    + @"insert into config_camera(id,type) values(7,'01_02');"
                    // config_ponder
                    + @"insert into config_ponder(name, camera_id, camera_type) values('1Ponderation', 0, '01_01');"
                    + @"insert into config_ponder(name, camera_id, camera_type) values('2Ponderation', 0, '01_01');"
                    + @"insert into config_ponder(name, camera_id, camera_type) values('3Ponderation', 0, '01_01');"
                    + @"insert into config_ponder(name, camera_id, camera_type) values('4Ponderation', 0, '01_01');";


                    if (ExecuteNonQuery(insertSql) < 14) { throw new Exception("插入数据无效!"); }
                    */
                    
                    //var insertSql = @"CREATE TABLE 'account_history'( 'id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'account' TEXT NOT NULL UNIQUE, 'login_time'    INTEGER";
                    //if (ExecuteNonQuery(insertSql) < 1) { throw new Exception("插入数据无效!"); }
                }
                catch (Exception er)
                {
                    BiuMessageBoxWindows.BiuShow("配置库重置失败，错误：" + er.Message + " \n 可尝试卸载重装!", image: BiuMessageBoxImage.Error);
                    Application.Current.Shutdown(-1);
                }
            }
        }

        /// <summary> 
		/// 对SQLite数据库执行增删改操作，返回受影响的行数。 
		/// </summary> 
		/// <param name="sql">要执行的增删改的SQL语句。</param> 
		/// <param name="parameters">执行增删改语句所需要的参数，参数必须以它们在SQL语句中的顺序为准。</param> 
		/// <returns></returns> 
		/// <exception cref="Exception"></exception>
		public static int ExecuteNonQuery(string sql, params SQLiteParameter[] parameters)
        {
            Open(_connection);
            int affectedRows = 0;
            using (var tr = _connection.BeginTransaction())
            {
                using (SQLiteCommand command = new SQLiteCommand(_connection))
                {
                    try
                    {
                        command.CommandText = sql;
                        if (parameters.Length != 0)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        affectedRows = command.ExecuteNonQuery();
                    }
                    catch (Exception) { throw; }
                }
                tr.Commit();
            }
            return affectedRows;
        }

        /// <summary> 
        /// 执行一个查询语句，返回一个包含查询结果的DataTable 
        /// </summary> 
        /// <param name="sql">要执行的查询语句</param> 
        /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准</param> 
        /// <returns></returns> 
        public static DataTable ExecuteDataTable(string sql, params SQLiteParameter[] parameters)
        {
            using (SQLiteCommand command = new SQLiteCommand(sql, _connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                DataTable data = new DataTable();
                adapter.Fill(data);
                return data;
            }
        }

        /// <summary>
        /// 判断连接是否处于打开状态
        /// </summary>
        /// <param name="connection"></param>
        private static void Open(SQLiteConnection connection)
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
        }
    }
}
