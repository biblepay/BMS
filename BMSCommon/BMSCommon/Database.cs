using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BMSCommon
{
    public static class Database
    {
        public static string msContentRootPath = null;

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
            
            string connStr = "server=" + sDbHost + ";user=" + sUser + ";database=" + sDB + ";port=3306;Allow User Variables=True;Connect Timeout=30;password=" + sPass + ";";
            return connStr;
        }

        



        public static bool DatabaseExists(string sName)
        {
            string sql = "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @sname;";
            MySqlCommand cmd1 = new MySqlCommand(sql);
            cmd1.Parameters.AddWithValue("@sname", sName);
            DataTable dt = GetDataTable(cmd1);
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
            DataTable dt = GetDataTable(cmd1);
            if (dt.Rows.Count == 0)
                return false;
            return true;
        }

        public static bool SPExists(bool fTestNet, string sDBName, string SPName)
        {
            string sFullObjName = sDBName + "." + SPName;
            string sql = "SELECT * FROM information_schema.routines WHERE ROUTINE_SCHEMA='" + sDBName + "' and ROUTINE_NAME = '" + SPName + "';";
            MySqlCommand cmd1 = new MySqlCommand(sql);
            DataTable dt = GetDataTable( cmd1 );
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

        public static DataTable GetDataTable(MySqlCommand cmd)
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
            DataTable dt = GetDataTable(cmd);
            if (dt.Rows.Count < 1)
                return 0;
            double n = Common.GetDouble(dt.Rows[0][sField]);
            return n;
        }

        public static string GetScalarString(MySqlCommand cmd, string sField)
        {
            DataTable dt = GetDataTable(cmd);
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
                conn.Close();
                

                Common.Log("ExecuteNonQueryCommand2[mysql]::" + ex.Message + " for " + cmd1.CommandText);

                return false;
            }
        }
    }



    public static class DataTableExtensions
    {

        /// <summary>
        ///     A DateTime extension method that elapsed the given datetime.
        public static TimeSpan Elapsed(this DateTime datetime)
        {
            return DateTime.Now - datetime;
        }

        public static DataTable FilterDataTable(this DataTable table, string sql)
        {
            try
            {
                DataRow[] dr1 = table.Select(sql);
                DataTable dtNew = new DataTable();
                if (dr1.Length > 0)
                {
                    dtNew = table.Clone();

                    foreach (DataRow temp in dr1)
                    {
                        dtNew.ImportRow(temp);
                    }
                }
                return dtNew;
            }
            catch (Exception ex)
            {
                DataTable dt1 = new DataTable();
                return dt1;
            }
        }

        public static DataTable SortDataTable(this DataTable table, string sql)
        {
            try
            {
                table.DefaultView.Sort = sql;
                table.DefaultView.ApplyDefaultSort = true;
                return table;
            }
            catch (Exception ex)
            {
                return table;
            }

        }

        public static DataTable FilterAndSort(this DataTable table, string sFilter, string sSort)
        {
            try
            {
                DataRow[] dr1 = table.Select(sFilter, sSort);
                DataTable dtNew = new DataTable();
                if (dr1.Length > 0)
                {
                    dtNew = table.Clone();

                    foreach (DataRow temp in dr1)
                    {
                        dtNew.ImportRow(temp);
                    }
                }
                return dtNew;
            }
            catch (Exception)
            {
                DataTable dt1 = new DataTable();
                return dt1;
            }
        }


        public static string GetColValue(this DataTable table, string colName)
        {
            if (table.Rows.Count < 1)
                return String.Empty;
            if (!table.Columns.Contains(colName))
                return String.Empty;
            return table.Rows[0][colName].ToString();
        }

        public static double GetColDouble(this DataTable table, string colName)
        {
            return BMSCommon.Common.GetDouble(table.Rows[0][colName].ToString());
        }

        public static int GetColInt(this DataTable table, string colName)
        {
            return (int)BMSCommon.Common.GetDouble(table.Rows[0][colName].ToString());
        }

        public static string GetColValue(this DataTable table, int iRow, string colName)
        {
            if (!table.Columns.Contains(colName))
            {
                return "";
            }
            return table.Rows[iRow][colName].ToString();
        }

        public static double GetColDouble(this DataRowView dr, string colName)
        {
            if (dr == null)
                return 0;


            double nOut = BMSCommon.Common.GetDouble(dr[colName].ToString());
            return nOut;
        }


        public static double GetColDouble(this DataTable table, int iRow, string colName)
        {
            if (table.Rows.Count == 0)
                return 0;

            double nOut = BMSCommon.Common.GetDouble(table.Rows[iRow][colName].ToString());
            return nOut;
        }
        public static DateTime GetColDateTime(this DataTable table, int iRow, string sColName)
        {
            DateTime dt = new DateTime();

            if (table.Rows.Count == 0)
                return dt;

            double nOut = BMSCommon.Common.GetDouble(table.Rows[iRow][sColName].ToString());
            dt = BMSCommon.Common.FromUnixTimeStamp((int)nOut);
            return dt;
        }

    }




}
