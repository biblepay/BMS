using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BMSCommon
{
    public static class Database
    {
        public static string GetDatabaseName()
        {
            string sDB = Common.GetConfigurationKeyValue("dbname");
            if (sDB == "")
                sDB = "bms";
            return sDB;
        }
        private static string PCS(bool fTestNet, string sDomain)
        {
            string sDB = GetDatabaseName();
            string sUser = "bmslocal";
            string sPass = "bms";
            string sDbHost = Common.GetConfigurationKeyValue("dbhost");
            if (sDbHost == "")
                sDbHost = "localhost";
            
            string connStr = "server=" + sDbHost + ";user=" + sUser + ";database=" + sDB + ";port=3306;Connect Timeout=30;password=" + sPass + ";";
            return connStr;
        }

        public static bool DatabaseExists(bool fTestNet, string sName)
        {
            string sql = "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @sname;";
            MySqlCommand cmd1 = new MySqlCommand(sql);
            cmd1.Parameters.AddWithValue("@sname", sName);
            DataTable dt = GetMySqlDataTable(fTestNet, cmd1, "biblepay.org");
            if (dt.Rows.Count == 0)
                return false;
            return true;
        }

        public static bool TableExists(bool fTestNet, string sDBName, string sName)
        {
            string sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = @yourdb and table_name=@sname;";
            MySqlCommand cmd1 = new MySqlCommand(sql);
            cmd1.Parameters.AddWithValue("@yourdb", sDBName);
            cmd1.Parameters.AddWithValue("@sname", sName);
            DataTable dt = GetMySqlDataTable(fTestNet, cmd1, "biblepay.org");
            if (dt.Rows.Count == 0)
                return false;
            return true;
        }

        public static MySqlDataReader GetMySqlDataReader(bool fTestNet, string sql, string sDomain)
        {
            MySqlConnection conn = new MySqlConnection(PCS(fTestNet, sDomain));
            MySqlDataReader rdr = null;
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return rdr;
        }


        public static DataTable GetMySqlDataTable(bool fTestNet, MySqlCommand cmd, string sDomain)
        {
            MySqlConnection conn = new MySqlConnection(PCS(fTestNet, sDomain));
            DataTable dt = new DataTable();
            try
            {
                cmd.Connection = conn;
                
                conn.Open();
                

                dt.Load(cmd.ExecuteReader());
                conn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return dt;
        }

        public static double GetScalarDouble(bool fTestNet, MySqlCommand cmd, string sField)
        {
            DataTable dt = GetMySqlDataTable(fTestNet, cmd, "");
            if (dt.Rows.Count < 1)
                return 0;
            double n = Common.GetDouble(dt.Rows[0][sField]);
            return n;
        }


        public static bool ExecuteNonQuery(bool fTestNet, string sql, string sDomain)
        {
            MySqlConnection conn = new MySqlConnection(PCS(fTestNet, sDomain));
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Common.Log("ExecuteNonQuery[mysql]::" + ex.Message);
                return false;
            }
        }


        public static bool ExecuteNonQuery(bool fTestNet, MySqlCommand cmd1, string sDomain)
        {
            MySqlConnection conn = new MySqlConnection(PCS(fTestNet, sDomain));
            try
            {
                conn.Open();
                MySqlCommand cmdNew = new MySqlCommand(cmd1.CommandText, conn);
                for (int i = 0; i < cmd1.Parameters.Count; i++)
                {
                    cmdNew.Parameters.Add(cmd1.Parameters[i]);
                }
                cmdNew.CommandTimeout = 7 * 60;
                cmdNew.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Common.Log("ExecuteNonQueryCommand2[mysql]::" + ex.Message);
                return false;
            }
        }
    }

}
