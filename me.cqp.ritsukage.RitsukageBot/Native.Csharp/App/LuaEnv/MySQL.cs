using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Native.Csharp.App.LuaEnv
{

    public class MySQLHelper
    {
        private static MySQLHelper mInstance = null;

        private static string mConnStr = null;

        private static string serverHost { get; set; }
        private static int serverPost { get; set; }
        private static string MySQLUsername { get; set; }
        private static string MySQLPassword { get; set; }
        private static string MySQLDataBase { get; set; }

        private MySQLHelper()
        {
        }

        public static bool Set(string host, int post, string username, string password)
        {
            serverHost = host;
            serverPost = post;
            MySQLUsername = username;
            MySQLPassword = password;
            return true;
        }

        public static MySQLHelper GetInstance()
        {
            if (mInstance == null)
            {
                mInstance = new MySQLHelper();
            }
            mConnStr = "server=" + serverHost + ";port=" + serverPost +
                ";user=" + MySQLUsername + ";password="+ MySQLPassword +
                ";database=" + MySQLDataBase + ";CharSet=utf8";

            return mInstance;
        }

        /// <summary>  
        /// 对SQLite数据库执行增删改操作，返回受影响的行数。  
        /// </summary>  
        /// <param name="sql">要执行的增删改的SQL语句</param>  
        /// <returns></returns>  
        public int ExecuteNonQuery(String sql)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mConnStr))
                {
                    connection.Open();
                    MySqlTransaction transaction = connection.BeginTransaction();

                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        try
                        {
                            PrepareCommand(cmd, connection, transaction, CommandType.Text, sql, null);

                            int rows = cmd.ExecuteNonQuery();
                            transaction.Commit();

                            cmd.Parameters.Clear();
                            return rows;
                        }
                        catch (MySqlException e1)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception e2)
                            {
                                throw e2;
                            }

                            throw e1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>  
        /// 对SQLite数据库执行增删改操作，返回受影响的行数。  
        /// </summary>  
        /// <param name="sql">要执行的增删改的SQL语句</param>  
        /// <returns></returns>  
        public int ExecuteNonQuery(String sql, MySqlParameter[] cmdParams)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mConnStr))
                {
                    connection.Open();
                    MySqlTransaction transaction = connection.BeginTransaction();

                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        try
                        {
                            PrepareCommand(cmd, connection, transaction, CommandType.Text, sql, cmdParams);

                            int rows = cmd.ExecuteNonQuery();
                            transaction.Commit();

                            cmd.Parameters.Clear();
                            return rows;
                        }
                        catch (MySqlException e1)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception e2)
                            {
                                throw e2;
                            }

                            throw e1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 对SQLite数据库执行操作，返回 返回第一行第一列数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteScalar(String sql)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mConnStr))
                {
                    connection.Open();
                    MySqlTransaction transaction = connection.BeginTransaction();

                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        try
                        {
                            int line = 0;

                            PrepareCommand(cmd, connection, transaction, CommandType.Text, sql, null);

                            String str = cmd.ExecuteScalar().ToString();
                            transaction.Commit();

                            line = Convert.ToInt32(str);
                            cmd.Parameters.Clear();

                            return line;
                        }
                        catch (MySqlException e1)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception e2)
                            {
                                throw e2;
                            }

                            throw e1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 对SQLite数据库执行操作，返回 返回第一行第一列数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteScalar(String sql, MySqlParameter[] cmdParams)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mConnStr))
                {
                    connection.Open();
                    MySqlTransaction transaction = connection.BeginTransaction();

                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        try
                        {
                            int line = 0;

                            PrepareCommand(cmd, connection, transaction, CommandType.Text, sql, cmdParams);

                            String str = cmd.ExecuteScalar().ToString();
                            transaction.Commit();

                            line = Convert.ToInt32(str);
                            cmd.Parameters.Clear();

                            return line;
                        }
                        catch (MySqlException e1)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception e2)
                            {
                                throw e2;
                            }

                            throw e1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        ///  用执行的数据库连接执行一个返回数据集的sql命令
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public MySqlDataReader ExecuteReader(String sql)
        {
            try
            {
                //创建一个MySqlConnection对象
                using (MySqlConnection connection = new MySqlConnection(mConnStr))
                {
                    connection.Open();
                    MySqlTransaction transaction = connection.BeginTransaction();

                    //创建一个MySqlCommand对象
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        try
                        {
                            PrepareCommand(cmd, connection, transaction, CommandType.Text, sql, null);

                            MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                            transaction.Commit();

                            cmd.Parameters.Clear();
                            return reader;
                        }
                        catch (MySqlException e1)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception e2)
                            {
                                throw e2;
                            }

                            throw e1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 查询返回Dtaset
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(String sql)
        {
            try
            {
                //创建一个MySqlConnection对象
                using (MySqlConnection connection = new MySqlConnection(mConnStr))
                {
                    connection.Open();
                    MySqlTransaction transaction = connection.BeginTransaction();

                    //创建一个MySqlCommand对象
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        try
                        {
                            PrepareCommand(cmd, connection, transaction, CommandType.Text, sql, null);

                            MySqlDataAdapter adapter = new MySqlDataAdapter();
                            adapter.SelectCommand = cmd;
                            DataSet ds = new DataSet();

                            adapter.Fill(ds);

                            transaction.Commit();

                            //清除参数
                            cmd.Parameters.Clear();
                            return ds;

                        }
                        catch (MySqlException e1)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception e2)
                            {
                                throw e2;
                            }

                            throw e1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 查询返回Dtaset
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(String sql, MySqlParameter[] cmdParams)
        {
            try
            {
                //创建一个MySqlConnection对象
                using (MySqlConnection connection = new MySqlConnection(mConnStr))
                {
                    connection.Open();
                    MySqlTransaction transaction = connection.BeginTransaction();

                    //创建一个MySqlCommand对象
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        try
                        {
                            PrepareCommand(cmd, connection, transaction, CommandType.Text, sql, cmdParams);

                            MySqlDataAdapter adapter = new MySqlDataAdapter();
                            adapter.SelectCommand = cmd;
                            DataSet ds = new DataSet();

                            adapter.Fill(ds);

                            transaction.Commit();

                            //清除参数
                            cmd.Parameters.Clear();
                            return ds;

                        }
                        catch (MySqlException e1)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception e2)
                            {
                                throw e2;
                            }

                            throw e1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 查询返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(String sql)
        {
            try
            {
                //创建一个MySqlConnection对象
                using (MySqlConnection connection = new MySqlConnection(mConnStr))
                {
                    connection.Open();
                    MySqlTransaction transaction = connection.BeginTransaction();

                    //创建一个MySqlCommand对象
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        try
                        {
                            PrepareCommand(cmd, connection, transaction, CommandType.Text, sql, null);

                            MySqlDataAdapter adapter = new MySqlDataAdapter();
                            adapter.SelectCommand = cmd;
                            DataTable dt = new DataTable();

                            adapter.Fill(dt);

                            transaction.Commit();

                            //清除参数
                            cmd.Parameters.Clear();
                            return dt;

                        }
                        catch (MySqlException e1)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception e2)
                            {
                                throw e2;
                            }

                            throw e1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 查询返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(String sql, MySqlParameter[] cmdParams)
        {
            try
            {
                //创建一个MySqlConnection对象
                using (MySqlConnection connection = new MySqlConnection(mConnStr))
                {
                    connection.Open();
                    MySqlTransaction transaction = connection.BeginTransaction();

                    //创建一个MySqlCommand对象
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        try
                        {
                            PrepareCommand(cmd, connection, transaction, CommandType.Text, sql, cmdParams);

                            MySqlDataAdapter adapter = new MySqlDataAdapter();
                            adapter.SelectCommand = cmd;
                            DataTable dt = new DataTable();

                            adapter.Fill(dt);

                            transaction.Commit();

                            //清除参数
                            cmd.Parameters.Clear();
                            return dt;

                        }
                        catch (MySqlException e1)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception e2)
                            {
                                throw e2;
                            }

                            throw e1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 准备执行一个命令
        /// </summary>
        /// <param name="cmd">sql命令</param>
        /// <param name="conn">OleDb连接</param>
        /// <param name="trans">OleDb事务</param>
        /// <param name="cmdType">命令类型例如 存储过程或者文本</param>
        /// <param name="cmdText">命令文本,例如:Select * from Products</param>
        /// <param name="cmdParms">执行命令的参数</param>
        private void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, CommandType cmdType, string cmdText, MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (MySqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
    }
}
