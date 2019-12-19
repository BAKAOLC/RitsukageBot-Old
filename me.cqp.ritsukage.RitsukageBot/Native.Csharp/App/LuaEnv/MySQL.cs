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

        private static MySqlConnection connection = null;

        private static MySqlDataReader lastDataReader = null;

        private MySQLHelper()
        {
        }

        public static MySQLHelper GetInstance()
        {
            if (mInstance == null)
            {
                mInstance = new MySQLHelper();
            }
            return mInstance;
        }

        private static bool open()
        {
            try
            {
                connection.Open();
            }
            catch
            {
                return false;
            }
            return true;
        }

        private static bool close()
        {
            try
            {
                if (connection != null) connection.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static string Set(string host, int post, string username, string password, string database)
        {
            try
            {
                mConnStr = "server=" + host + ";port=" + post +
                    ";user=" + username + ";password=" + password +
                    ";database=" + database + ";Allow User Variables=True;CharSet=utf8mb4";
            }
            catch (Exception ex)
            {
                return "error: " + ex.ToString();
            }
            return "success";
        }

        public static string Connect()
        {
            try
            {
                connection = new MySqlConnection(mConnStr);
                if (!open()) return "error: failed to connect mysql";
            }
            catch
            {
                return "error: failed to connect mysql";
            }
            return "success";
        }

        public static string Disconnect()
        {
            if (!close()) return "error: failed to close mysql connection";
            connection = null;
            return "success";
        }

        public static string DoSQLCommand(string command)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(command, connection);
                lastDataReader = cmd.ExecuteReader();
                cmd = null;
            }
            catch (Exception ex)
            {
                lastDataReader = null;
                return "error: " + ex.ToString();
            }

            return "success";
        }

        public static string ExecuteSQLCommand(string command)
        {
            try
            {
                var c = new MySqlConnection(mConnStr);
                c.Open();
                MySqlCommand cmd = new MySqlCommand(command, c);
                cmd.ExecuteNonQuery();
                c.Close();
            }
            catch (Exception ex)
            {
                return "error: " + ex.ToString();
            }
            return "success";
        }

        public static MySqlDataReader GetLastDataReader()
        {
            return lastDataReader;
        }

    }
}
