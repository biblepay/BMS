using MySql.Data.MySqlClient;
using System;
using System.Data;
using static BiblePay.BMS.Common;

namespace BiblePay.BMS.DSQL
{
    public static class Metrics
    {
        public static int nFolders = 0;
        public static int nObjects = 0;
        private static int nConnections = 0;
        public static int nPeerCount = 0;
        public static int nLatency = 0;
        private static int nStartTime = 0;
        private static double nElapsed1 = 0;
        public static int Connections
        {
            get
            {
                return nConnections;
            }
        }

        public static double Latency()
        {
            if (nConnections > 0)
            {
                return nElapsed1 / nConnections;
            }
            return 0;
        }
        public static void AddPeer()
        {
            nConnections++;
            nPeerCount++;
        }
        public static void StopwatchStop()
        {
            nElapsed1 = UnixTimestamp() - nStartTime;
        }
        public static void StopwatchStart()
        {
            nStartTime = UnixTimestamp();
        }

    }


    public static class Database
    {
        // This is a class that can speak to mysql; reserved for future use
        // We may want to make an IPFS database that replicates back and forth into mysql, design is still pending.

        private static string PCS(bool fTestNet, string sDomain)
        {
            string sDB = "unknonwn";
            string sUser = "bms";
            string sPass = "unknown";
            string connStr = "server=unknown.biblepay.org;user=" + sUser + ";database=" + sDB + ";port=3306;password=" + sPass + ";";
            return connStr;
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


        public static DataTable GetDataTableFromCluster(bool fTestNet, string sTable, string sDomain)
        {
            string sql = "Select * from " + sTable + " WHERE 1=1;";
            MySqlCommand command1 = new MySqlCommand(sql);
            DataTable dt = GetMySqlDataTable(fTestNet, command1, sDomain);
            return dt;
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
                return dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return dt;
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

    }
}