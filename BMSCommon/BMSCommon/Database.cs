using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace BMSCommon
{
    public static class MySQLDatabase
    {

        public static string GetDatabaseName()
        {
            string sDB = GetConfigurationKeyValue("dbname");
            if (sDB == "")
                sDB = "bms";
            return sDB;
        }
        private static string PCS(bool fTestNet, string sDomain)
        {
            string sDB = GetDatabaseName();
            string sUser = "bmslocal";
            string sPass = "bms";
            string sDbHost = GetConfigurationKeyValue("dbhost");
            if (sDbHost == "")
                sDbHost = "localhost";

            string connStr = "server=" + sDbHost + ";user=" + sUser + ";database=" + sDB + ";port=3306;Allow User Variables=True;Connect Timeout=30;password=" + sPass + ";";
            return connStr;
        }

        public static bool DatabaseExists(string sName)
        {
            string sql = "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @sname;";
            MySqlCommand cmd1 = new MySqlCommand(sql);
            cmd1.Parameters.AddWithValue("@sname", sName);
            DataTable dt = GetDataTable2(cmd1);
            if (dt.Rows.Count == 0)
                return false;
            return true;
        }

        public static bool TableExists(string sDBName, string sName)
        {
            string sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = @yourdb and table_name=@sname;";
            MySqlCommand cmd1 = new MySqlCommand(sql);
            cmd1.Parameters.AddWithValue("@yourdb", sDBName);
            cmd1.Parameters.AddWithValue("@sname", sName);
            DataTable dt = GetDataTable2(cmd1);
            if (dt.Rows.Count == 0)
                return false;
            return true;
        }

        public static bool SPExists(bool fTestNet, string sDBName, string SPName)
        {
            string sFullObjName = sDBName + "." + SPName;
            string sql = "SELECT * FROM information_schema.routines WHERE ROUTINE_SCHEMA='" + sDBName + "' and ROUTINE_NAME = '" + SPName + "';";
            MySqlCommand cmd1 = new MySqlCommand(sql);
            DataTable dt = GetDataTable2(cmd1);
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

        public static DataTable GetDataTable2(MySqlCommand cmd)
        {
            bool fTestNet = false;
            MySqlConnection conn = new MySqlConnection(PCS(fTestNet, ""));
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
                conn.Close();
                Console.WriteLine(ex.ToString());
            }
            return dt;
        }


        public static double GetScalarDouble(MySqlCommand cmd, string sField)
        {
            DataTable dt = GetDataTable2(cmd);
            if (dt.Rows.Count < 1)
                return 0;
            double n = GetDouble(dt.Rows[0][sField]);
            return n;
        }

        public static double GetScalarAge(MySqlCommand cmd, object vCol, bool bLog = true)
        {
            DataTable dt1 = GetDataTable2(cmd);
            try
            {
                if (dt1.Rows.Count > 0)
                {
                    object oOut = null;
                    if (vCol.GetType().ToString() == "System.String")
                    {
                        oOut = dt1.Rows[0][vCol.ToString()];
                    }
                    else
                    {
                        oOut = dt1.Rows[0][Convert.ToInt32(vCol)];
                    }
                    if (oOut.GetType().ToString() == "System.DBNull")
                    {
                        oOut = Convert.ToDateTime("1-1-1970");
                    }
                    DateTime d1 = Convert.ToDateTime(oOut);
                    TimeSpan vAge = DateTime.Now - d1;
                    return vAge.TotalSeconds;

                }
            }
            catch (Exception)
            {
            }
            return 0;
        }

        public static string GetScalarString(MySqlCommand cmd, string sField)
        {
            DataTable dt = GetDataTable2(cmd);
            if (dt.Rows.Count < 1)
                return String.Empty;

            string s = dt.Rows[0][sField].ToString();
            return s;
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
                conn.Close();
                Common.Log("ExecuteNonQuery[mysql]::" + ex.Message);
                return false;
            }
        }

        // Notes:  If we have a string date column, mysql function to convert back to date for sorting: order by by STR_TO_DATE(Added,'%m/%d/%Y %h:%i:%s') desc;
        public static bool ExecuteNonQuery2(MySqlCommand cmd1)
        {
            MySqlConnection conn = new MySqlConnection(PCS(false, ""));
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
                conn.Close();

                Log("ExecuteNonQueryCommand2[mysql]::" + ex.Message + " for " + cmd1.CommandText);

                return false;
            }
        }

    }


}
