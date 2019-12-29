using System;
using MySql.Data.MySqlClient;

namespace Native.Csharp.App.LuaEnv {
    public class MySQLHelper {
        private static MySQLHelper mInstance;

        private static string mConnStr;

        private static MySqlConnection connection;

        private static MySqlDataReader lastDataReader;

        private MySQLHelper() {
        }

        public static MySQLHelper GetInstance() {
            return mInstance ?? (mInstance = new MySQLHelper());
        }

        private static bool Open() {
            try {
                connection?.Open();
            } catch {
                return false;
            }
            return true;
        }

        private static bool Close() {
            try {
                connection?.Close();
            } catch {
                return false;
            }
            return true;
        }

        public static string Set(string host, int post, string username, string password, string database) {
            try {
                mConnStr = "server=" + host + ";port=" + post + ";user=" + username + ";password=" + password +
                           ";database=" + database + ";Allow User Variables=True;CharSet=utf8mb4";
            } catch (Exception ex) {
                return "error: " + ex;
            }
            return "success";
        }

        public static string Connect() {
            try {
                connection = new MySqlConnection(mConnStr);
                if (!Open()) return "error: failed to connect mysql";
            } catch {
                return "error: failed to connect mysql";
            }
            return "success";
        }

        public static string Disconnect() {
            if (!Close()) return "error: failed to close mysql connection";
            connection = null;
            return "success";
        }

        public static string DoSQLCommand(string command) {
            try {
                MySqlCommand cmd = new MySqlCommand(command, connection);
                lastDataReader = cmd.ExecuteReader();
            } catch (Exception ex) {
                lastDataReader = null;
                return "error: " + ex;
            }

            return "success";
        }

        public static string ExecuteSQLCommand(string command) {
            MySqlConnection c = null;
            try {
                c = new MySqlConnection(mConnStr);
                c.Open();
                MySqlCommand cmd = new MySqlCommand(command, c);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            } catch (Exception ex) {
                return "error: " + ex;
            } finally {
                c?.Close();
            }
            return "success";
        }

        public static MySqlDataReader GetLastDataReader() {
            return lastDataReader;
        }
    }
}