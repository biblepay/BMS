using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using static BMSCommon.Common;
using static BiblePay.BMS.DSQL.PoolBase;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace BiblePay.BMS.DSQL
{
    public class XMRPoolBase
    {
        private Dictionary<string, WorkerInfo> dictWorker = new Dictionary<string, WorkerInfo>();
        private Dictionary<string, WorkerInfo> dictBan = new Dictionary<string, WorkerInfo>();
        private Dictionary<string, PoolBase.XMRJob> dictJobs = new Dictionary<string, PoolBase.XMRJob>();
        private static int MAX_JOBS = 25000;
        private static int SOCKET_TIMEOUT = 5000;

        public static int iThreadCount = 0;
        public static int nGlobalHeight = 0;
        public int iXMRThreadID = 0;
        public double iXMRThreadCount = 0;
        public int BXMRC = 0;
        public int SHARES = 0;
        public static DateTime start_date = DateTime.Now;
        public static int MIN_DIFF = 1;
        public static object cs_p = new object();
        public bool fMonero2000 = true;
        private bool _fTestNet = false;
        private List<string> lBanList = new List<string>();
        private static bool fUseJobsTable = false;
        private static bool fUseBanTable = false;
        private  int nLastDeposited = 0;
        private  int nLastHourly = 0;
        private  int nLastBoarded = 0;
        public BlockTemplate _template;
        private static string mBatch = String.Empty;
     
        public BlockTemplate GetBlockTemplate()
        {
            return _template;
        }

        public int GetJobCount()
        {
            return dictJobs.Count();
        }
        public int GetWorkerCount()
        {
            return dictWorker.Count();
        }

        public XMRPoolBase(bool fTestNet)
        {
            _fTestNet = fTestNet;
            GetBlockForStratum();
            var t = new Thread(InitializeXMR);
            t.Start();
        }

        public bool IsTestNet()
        {
            return _fTestNet;
        }

        private void SetWorker(WorkerInfo worker, string sKey)
        {
            try
            {
                if (!dictWorker.ContainsKey(sKey))
                {
                    worker = GetWorker(sKey);
                }

                worker.receivedtime = BMSCommon.Common.UnixTimestamp(); // perpetually increasing dictWorkercount compared to jobCount
                dictWorker[sKey] = worker;
            }
            catch (Exception ex)
            {
                Log("SetWorker" + ex.Message);
            }
        }
        private WorkerInfo GetWorker(string socketid)
        {
            try
            {
                WorkerInfo w = new WorkerInfo();
                if (!dictWorker.ContainsKey(socketid))
                {
                    w.receivedtime = BMSCommon.Common.UnixTimestamp();
                    dictWorker[socketid] = w;
                }
                w = dictWorker[socketid];
                return w;
            }
            catch (Exception)
            {
                // This is not supposed to happen, but I see this error in the log... 
                WorkerInfo w = new WorkerInfo();
                SetWorker(w, socketid);
                return w;
            }
        }

        private string GetPoolValue(string sKey)
        {
            string sql = "Select value from sys where systemkey=@key;";
            MySqlCommand cmd1 = new MySqlCommand(sql);
            cmd1.Parameters.AddWithValue("@key", sKey);
            string value = BMSCommon.Database.GetScalarString(cmd1, "value");
            return value;
        }
        private void RemoveWorker(string socketid)
        {
            try
            {
                dictWorker.Remove(socketid);
            }
            catch (Exception ex)
            {
                Log("Rem w" + ex.Message);
            }
        }
        private WorkerInfo GetWorkerBan(string socketid)
        {
            WorkerInfo w = new WorkerInfo();
            if (!dictBan.ContainsKey(socketid))
            {
                w.receivedtime = UnixTimestamp();
                dictBan[socketid] = w;
            }
            w = dictBan[socketid];
            w.lastreceived = w.receivedtime;
            w.receivedtime = UnixTimestamp();
            dictBan[socketid] = w;
            return w;
        }
      
        private void insBanDetails(string IP, string sWHY, double iLevel)
        {
            if (!fUseBanTable) return;

            try
            {
                string sql = "Insert into BanDetails (id,IP,Notes,Added,Level) values (uuid(), '" + IP + "','" + sWHY + "',getdate(),'" + iLevel.ToString() + "');";
                MySqlCommand cmd1 = new MySqlCommand(sql);
                BMSCommon.Database.ExecuteNonQuery(cmd1);
            }
            catch (Exception x)
            {
                string test = x.Message;
            }
        }

        private static int BAN_THRESHHOLD = 512;
        private WorkerInfo Ban(string socketid, double iHowMuch, string sWhy)
        {
            string sKey = GetIPOnly(socketid);
            bool fIsBanned = lBanList.Contains(sKey);
            WorkerInfo w = GetWorkerBan(sKey);
            w.banlevel += iHowMuch;
            if (w.banlevel > BAN_THRESHHOLD)
            {
                if (!w.logged)
                {
                    //Log("Banned " + GetIPOnly(socketid));
                    w.logged = true;
                }
                w.banlevel = BAN_THRESHHOLD + 1;
            }
            if (fIsBanned)
            {
                w.banlevel = 1024;
            }
            if (w.banlevel < 0)
                w.banlevel = 0;
            w.banned = w.banlevel >= BAN_THRESHHOLD ? true : false;
            dictBan[sKey] = w;
            if (w.banlevel > 0 && (w.banlevel < 10 || w.banlevel % 10 == 0))
            {
                insBanDetails(sKey, sWhy, w.banlevel);
            }
            return w;
        }

        private void CloseSocket(Socket c)
        {
            try
            {
                c.Close();
            }
            catch (Exception)
            {

            }
        }

        private string GetIPOnly(string fullendpoint)
        {
            string[] vData = fullendpoint.Split(":");
            if (vData.Length > 1)
            {
                return vData[0];
            }
            return fullendpoint;
        }

        private static ReaderWriterLockSlim dictLock = new ReaderWriterLockSlim();
        public PoolBase.XMRJob RetrieveXMRJob(string socketid)
        {
            try
            {
                dictLock.EnterReadLock();
                if (dictJobs.ContainsKey(socketid))
                {
                    return dictJobs[socketid];
                }
                PoolBase.XMRJob x = new PoolBase.XMRJob();
                x.timestamp = UnixTimestamp();
                x.socketid = socketid;
                return x;
            }
            finally
            {
                dictLock.ExitReadLock();
            }
        }

        private void PutXMRJob(PoolBase.XMRJob x)
        {
            if (x.socketid == "")
                return;
            dictLock.EnterWriteLock();
            try
            {
                x.timestamp = UnixTimestamp();
                dictJobs[x.socketid] = x;
            }
            finally
            {
                dictLock.ExitWriteLock();
            }
        }
        public void PurgeJobs()
        {
            lock (cs_stratum)
            {
                try
                {
                    dictLock.EnterWriteLock();

                    foreach (KeyValuePair<string, PoolBase.XMRJob> entry in dictJobs.ToArray())
                    {
                        if (dictJobs.ContainsKey(entry.Key))
                        {
                            PoolBase.XMRJob w1 = dictJobs[entry.Key];
                            int nElapsed = UnixTimestamp() - w1.timestamp;
                            bool fRemove = (nElapsed > (60 * 15) && w1.timestamp > 0);
                            if (fRemove)
                            {
                                try
                                {
                                    dictJobs.Remove(entry.Key);
                                }
                                catch (Exception ex)
                                {
                                    Log("PJ " + ex.Message);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log("Purge Jobs: " + ex.Message);
                }
                finally
                {
                    dictLock.ExitWriteLock();
                }
            }
        }


        public void PurgeSockets(bool fClearBans)
        {
            try
            {
                foreach (KeyValuePair<string, WorkerInfo> entry in dictWorker.ToArray())
                {
                    if (entry.Key != null)
                    {
                        if (dictWorker.ContainsKey(entry.Key))
                        {
                            WorkerInfo w1 = GetWorker(entry.Key);
                            int nElapsed = UnixTimestamp() - w1.receivedtime;
                            bool fRemove = false;
                            fRemove = (nElapsed > (60 * 15) && w1.receivedtime > 0)
                                || (nElapsed > (60 * 15) && (w1.bbpaddress == null || w1.bbpaddress == ""));

                            if (fClearBans)
                            {
                                w1.banlevel = 0;
                                SetWorker(w1, entry.Key);
                            }

                            if (fRemove)
                            {
                                RemoveWorker(entry.Key);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Purge Sockets: " + ex.Message);
            }
        }

        private void InsShare(string bbpaddress, double nShareAdj, double nFailAdj, int height, double nBXMR, double nBXMRC, string moneroaddress)
        {
            string sSPName = _fTestNet ? "tinsShare" : "insShare";
            string sql = "call `" + sSPName + "` (@bbpid,@shareAdj,@failAdj,@height,@sxmr,@fxmr,@sxmrc,@fxmrc,@bxmr,@bxmrc);";
            MySqlCommand command = new MySqlCommand(sql);

            command.Parameters.AddWithValue("@bbpid", bbpaddress);
            command.Parameters.AddWithValue("@shareAdj", nShareAdj);
            command.Parameters.AddWithValue("@failAdj", nFailAdj);
            command.Parameters.AddWithValue("@height", height);
            command.Parameters.AddWithValue("@sxmr", 0);
            command.Parameters.AddWithValue("@fxmr", 0);
            command.Parameters.AddWithValue("@sxmrc", 0);
            command.Parameters.AddWithValue("@fxmrc", 0);
            command.Parameters.AddWithValue("@bxmr", nBXMR);
            command.Parameters.AddWithValue("@bxmrc", nBXMRC);
            if (bbpaddress == "" || height == 0)
            {
                //if (moneroaddress == GetConfigurationKeyValue("moneroaddress"))                    return;
                return;
            }
            try
            {
                PoolBase.lSQL.Add(command);
            }
            catch (Exception ex)
            {
                Log("insShare: " + ex.Message);
            }
        }


        private void MarkForBroadcast()
        {
            try
            {
                foreach (KeyValuePair<string, WorkerInfo> entry in dictWorker.ToArray())
                {
                    if (dictWorker.ContainsKey(entry.Key))
                    {
                        WorkerInfo w1 = GetWorker(entry.Key);
                        w1.Broadcast = true;
                        SetWorker(w1, entry.Key);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private double FullTest(byte[] h)
        {
            // Converts the RandomX solution hash over to the original bitcoin difficulty level
            UInt64 nAdjHash = BitConverter.ToUInt64(h, 24);
            double nDiff = 0xFFFFFFFFFFFFUL / (nAdjHash + .01);
            return nDiff;
        }

        public static string GetChartOfSancs()
        {
            BMSCommon.Pricing.BBPChart b = new BMSCommon.Pricing.BBPChart();
            b.Name = "Number of Sanctuaries vs Monthly Reward";
            BMSCommon.Pricing.ChartSeries c = new BMSCommon.Pricing.ChartSeries();
            b.CollectionSeries.Add(c);
            c.Name = "Monthly Reward";
            for (double iSancs = 20; iSancs < 104; iSancs += 1)
            {
                double dRevenue = (205 / iSancs) * 3700 * 30.01; 
                c.DataPoint.Add(dRevenue);
            }
            return BMSCommon.Pricing.GenerateJavascriptChart(b);
        }


        public string GetChartOfHashRate()
        {
            int nMax = _template.height - 1;
            int nMin = nMax - 205;
            string sTable = _fTestNet ? "thashrate" : "hashrate";
            string sql = "select HashRate,Height From " + sTable + " where height > "
                + nMin.ToString() + " and height < " + nMax.ToString() + " order by height";
            MySqlCommand cmd1 = new MySqlCommand(sql);
            DataTable dt = BMSCommon.Database.GetDataTable( cmd1 );
            BMSCommon.Pricing.BBPChart b = new BMSCommon.Pricing.BBPChart();
            b.Name = "Hashrate over 24 hours";
            BMSCommon.Pricing.ChartSeries c = new BMSCommon.Pricing.ChartSeries();
            //b.CollectionSeries = new List<BMSCommon.Pricing.ChartSeries>();
            c.BorderColor = "blue";
            c.BackgroundColor = "darkblue";

            b.CollectionSeries.Add(c);
            for (int i = 0; i < dt.Rows.Count; i += 1)
            {
                double Height = GetDouble(dt.Rows[i]["height"]);
                double dR = GetDouble(dt.Rows[i]["hashrate"]);
                b.XAxis.Add(Height);
                c.DataPoint.Add(dR);
            }
            string html = BMSCommon.Pricing.GenerateJavascriptChart(b);
            return html;
        }


        public string GetChartOfWorkers()
        {
            int nMax = _template.height - 1;
            int nMin = nMax - 205;
            string sTable = _fTestNet ? "thashrate" : "hashrate";

            string sql = "select minercount, height From " + sTable + " where height > " + nMin.ToString() + " and height < " + nMax.ToString() + " order by height";
            MySqlCommand cmd1 = new MySqlCommand(sql);
            DataTable dt = BMSCommon.Database.GetDataTable(cmd1);
            BMSCommon.Pricing.BBPChart b = new BMSCommon.Pricing.BBPChart();

            b.Name = "Workers over 24 hours";
            BMSCommon.Pricing.ChartSeries c = new BMSCommon.Pricing.ChartSeries();
            c.Name = "Workers";
            c.BorderColor = "blue";
            c.BackgroundColor = "darkblue";

            b.CollectionSeries.Add(c);
            for (int i = 0; i < dt.Rows.Count; i += 1)
            {
                double Height = GetDouble(dt.Rows[i]["height"]);
                double dWorkers = GetDouble(dt.Rows[i]["MinerCount"]);
                b.XAxis.Add(Height);
                c.DataPoint.Add(dWorkers);
            }
            string html = BMSCommon.Pricing.GenerateJavascriptChart(b);
            return html;
        }

        public string GetChartOfBlocks()
        {
            int nMax = _template.height - 1;
            int nMin = nMax - 205;
            string sTable = _fTestNet ? "thashrate" : "hashrate";
            string sql = "select SolvedCount, height From " + sTable + " where height > " + nMin.ToString() + " and height < " + nMax.ToString() + " order by height;";
            MySqlCommand cmd1 = new MySqlCommand(sql);
            DataTable dt = BMSCommon.Database.GetDataTable(cmd1);
            BMSCommon.Pricing.BBPChart b= new BMSCommon.Pricing.BBPChart();
            BMSCommon.Pricing.ChartSeries c = new BMSCommon.Pricing.ChartSeries();
            c.Name = "Blocks Solved";
            c.BorderColor = "blue";
            c.BackgroundColor = "darkblue";

            b.CollectionSeries.Add(c);
            b.Name = "Blocks Solved over 24 hours";

            for (int i = 0; i < dt.Rows.Count; i += 1)
            {
                double Height = GetDouble(dt.Rows[i]["height"]);
                double dWorkers = GetDouble(dt.Rows[i]["SolvedCount"]);
                string sNarr = Height.ToString();
                b.XAxis.Add(Height);
                c.DataPoint.Add(dWorkers);
            }
            string html = BMSCommon.Pricing.GenerateJavascriptChart(b);
            return html;
        }


        public struct BlockTemplate
        {
            public string hex;
            public string curtime;
            public string prevhash;
            public string prevblocktime;
            public string bits;
            public string target;
            public int height;
            public int updated;
        }



        public void ClearBans()
        {
            // Clear banned pool users
            try
            {
                dictBan.Clear();
                return;
                // NOTE: The table 'bans' does not exist yet (needs ported from foundation.biblepay.org);

                // Memorize the excess banlist
                string sTable = _fTestNet ? "tworker" : "worker";
                string sLeaderboard = _fTestNet ? "tLeaderboard" : "Leaderboard";
                string sql = "Select distinct dbo.iponly(ip) ip from " + sTable + " where bbpaddress in (select bbpaddress from " 
                    + sLeaderboard + " where efficiency < .20) UNION ALL Select IP from bans;";
                MySqlCommand m1 = new MySqlCommand(sql);
                DataTable dt = BMSCommon.Database.GetDataTable(m1);
                lBanList.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["ip"].ToString().Length > 1)
                        lBanList.Add(dt.Rows[i]["ip"].ToString());
                }
            }
            catch (Exception ex)
            {
                Log("Clearing ban " + ex.Message);
            }
        }


        private void TallyBXMRC()
        {
            string sTable = _fTestNet ? "tshare" : "share";
            string sql = "select sum(bxmrc) bxmrc,sum(bxmr)-sum(bxmrc) shrs from " + sTable + " where TIMESTAMPDIFF(MINUTE, updated, now()) > 30";
            MySqlCommand cmd = new MySqlCommand(sql);
            BXMRC = (int)BMSCommon.Database.GetScalarDouble(cmd,  "bxmrc");
            SHARES = (int)BMSCommon.Database.GetScalarDouble(cmd, "shrs");
        }


        public void Leaderboard()
        {
            int nElapsed = UnixTimestamp() - nLastBoarded;
            int nDepositElapsed = UnixTimestamp() - nLastDeposited;
            int nHourlyElapsed = UnixTimestamp() - nLastHourly;
            if (nElapsed < (60 * 2))
                return;
            nLastBoarded = UnixTimestamp();
            fUseBanTable = Convert.ToBoolean(GetDouble(GetPoolValue("USEBAN")));
            fUseJobsTable = Convert.ToBoolean(GetDouble(GetPoolValue("USEJOB")));
            //fLogSql = Convert.ToBoolean(GetDouble(GetPoolValue("USESQL")));

            try
            {
                // Update the leaderboard
                string sSP = _fTestNet ? "updtLeaderboard" : "updLeaderboard";
                string sql = "call " + sSP + ";";
                
                MySqlCommand command = new MySqlCommand(sql);
                PoolBase.lSQL.Add(command);
                TallyBXMRC();


                if (nHourlyElapsed > (60 * 60))
                {
                    nLastHourly = UnixTimestamp();
                    //Proposals.SubmitProposals(true);
                    //Proposals.SubmitProposals(false);
                }

            }
            catch (Exception ex)
            {
                Log("PoolLeaderboard" + ex.Message);
            }
        }

        public void RecordDifficultyHistory()
        {
            // This is for the difficulty chart
            int nBestHeight = _template.height;
            if (nBestHeight == 0) return;
            string sTable = _fTestNet ? "tDifficultyHistory" : "DifficultyHistory";
            string sql = "Select max(height) h from " + sTable;
            MySqlCommand m = new MySqlCommand(sql);

            int nH = (int)BMSCommon.Database.GetScalarDouble(m, "h");
            if (nH < nBestHeight - 1000)
                nH = nBestHeight - 1000;
            // Set subsidies first
            for (int iMyHeight = nH - 1; iMyHeight < nBestHeight; iMyHeight++)
            {
                double nSubsidy = 0;
                string sRecip = "";
                BMSCommon.WebRPC.GetSubsidy(_fTestNet, iMyHeight, ref sRecip, ref nSubsidy);
                double dPOWDiff = GetDouble(GetBlock(_fTestNet, "showblock", iMyHeight, "difficulty"));
                sql = "Delete from " + sTable + " where height = '" + iMyHeight.ToString()
                    + "';\r\nInsert Into " + sTable + " (id,height,recipient,subsidy,added,difficulty) values (uuid(),'"
                    + iMyHeight.ToString() + "','" + sRecip + "','" + nSubsidy.ToString() + "',now(),'" + dPOWDiff.ToString() + "');";
                if (iMyHeight > 0 && sRecip != "" && sRecip != null && nSubsidy > 0)
                {
                    MySqlCommand m2 = new MySqlCommand(sql);
                    BMSCommon.Database.ExecuteNonQuery(m2);
                }
            }
        }


        private int nLastGrouped = 0;
        public void GroupShares()
        {
            int nElapsed = UnixTimestamp() - nLastGrouped;
            if (nElapsed < (60 * 20))
                return;

            try
            {
                nLastGrouped = UnixTimestamp();
                if (_template.height == 0)
                {
                    GetBlockForStratum();
                }

                int nBestHeight = _template.height;
                if (nBestHeight == 0) return;
                int nLookback = 205;
                string sTable = _fTestNet ? "tshare" : "share";
                for (int iMyHeight = nBestHeight - nLookback; iMyHeight < nBestHeight - 7; iMyHeight++)
                {
                    string sql7 = "Select count(*) ct from " + sTable + " where paid is null and Subsidy is null and height = '" + iMyHeight.ToString() + "'";
                    MySqlCommand m7 = new MySqlCommand(sql7);
                    double dCt = BMSCommon.Database.GetScalarDouble(m7, "ct");

                    if (dCt > 0)
                    {
                        double nSubsidy = 0;
                        string sRecip = "";
                        BMSCommon.WebRPC.GetSubsidy(_fTestNet, iMyHeight, ref sRecip, ref nSubsidy);

                        string sPAKey = _fTestNet ? "tPoolAddress" : "PoolAddress";
                        string sPoolAddress = GetConfigurationKeyValue(sPAKey);
                        if (sPoolAddress == "")
                        {
                            Log("Unable to start pool; pool address not set.  Set PoolAddress=receiveaddress in bms.conf.");
                        }
                        if (sRecip != sPoolAddress)
                        {
                            nSubsidy = .02;
                            string sql3 = "Select * from " + sTable + " WHERE Paid is null and height = @height;";
                            MySqlCommand command3 = new MySqlCommand(sql3);
                            command3.Parameters.AddWithValue("@height", iMyHeight);
                            DataTable dt4 = BMSCommon.Database.GetDataTable(command3);

                            for (int x = 0; x < dt4.Rows.Count; x++)
                            {
                                string bbpaddress1 = dt4.Rows[x]["bbpaddress"].ToString();
                                string sSP = _fTestNet ? "tinsShare" : "insShare";
                                string sql9 = "call " + sSP + " (@bbpid,@shareAdj,@failAdj,@height,@sxmr,@fxmr,@sxmrc,@fxmrc,@bxmr,@bxmrc);";
                                MySqlCommand command5 = new MySqlCommand(sql9);
                                command5.Parameters.AddWithValue("@bbpid", bbpaddress1);
                                command5.Parameters.AddWithValue("@shareAdj", GetDouble(dt4.Rows[x]["shares"]));
                                command5.Parameters.AddWithValue("@failAdj", GetDouble(dt4.Rows[x]["fails"]));
                                command5.Parameters.AddWithValue("@height", iMyHeight + 1);
                                command5.Parameters.AddWithValue("@sxmr", GetDouble(dt4.Rows[x]["sucxmr"]));
                                command5.Parameters.AddWithValue("@fxmr", GetDouble(dt4.Rows[x]["failxmr"]));
                                command5.Parameters.AddWithValue("@sxmrc", GetDouble(dt4.Rows[x]["SucXMRC"]));
                                command5.Parameters.AddWithValue("@fxmrc", GetDouble(dt4.Rows[x]["FailXMRC"]));
                                command5.Parameters.AddWithValue("@bxmr", GetDouble(dt4.Rows[x]["BXMR"]));
                                command5.Parameters.AddWithValue("@bxmrc", GetDouble(dt4.Rows[x]["BXMRC"]));
                                try
                                {
                                    BMSCommon.Database.ExecuteNonQuery(command5);
                                }
                                catch (Exception ex2)
                                {
                                    Log("GroupShares:" + ex2.Message);
                                }


                                //now delete the source share
                                sql3 = "Delete from " + sTable + " where height=@height and bbpaddress=@bbpid;";
                                command5 = new MySqlCommand(sql3);
                                command5.Parameters.AddWithValue("@bbpid", bbpaddress1);
                                command5.Parameters.AddWithValue("@height", iMyHeight);
                                BMSCommon.Database.ExecuteNonQuery(command5);
                            }
                        }

                        string sql8 = "Update " + sTable + " set Subsidy=@subsidy,Solved=@solved where height = @height and subsidy is null;";
                        MySqlCommand command1 = new MySqlCommand(sql8);
                        command1.Parameters.AddWithValue("@subsidy", nSubsidy);
                        command1.Parameters.AddWithValue("@height", iMyHeight);
                        int iSolved = nSubsidy > 1 ? 1 : 0;
                        command1.Parameters.AddWithValue("@solved", iSolved);
                        BMSCommon.Database.ExecuteNonQuery(command1);
                    }
                }

                // Set subsidies next
                for (int iMyHeight = nBestHeight - nLookback; iMyHeight < nBestHeight - 7; iMyHeight++)
                {
                    string sHeightRange = "height='" + iMyHeight.ToString() + "'";
                    string sql = "Select shares,sucXMRC,bxmr,bbpaddress,subsidy from " + sTable + " WHERE subsidy > 1 and percentage is null and "
                        + sHeightRange + " and paid is null;";
                    MySqlCommand m0 = new MySqlCommand(sql);
                    DataTable dt1 = BMSCommon.Database.GetDataTable(m0);
                    if (dt1.Rows.Count > 0)
                    {
                        // First get the total shares
                        double nTotalShares = 0;
                        double nTotalSubsidy = GetDouble(dt1.Rows[0]["subsidy"]);
                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            double nHPS = GetDouble(dt1.Rows[i]["Shares"]) + (GetDouble(dt1.Rows[i]["bxmr"]));
                            nTotalShares += nHPS;
                        }
                        if (nTotalShares == 0)
                            nTotalShares = .01;
                        double nPoolFee = GetDouble(GetConfigurationKeyValue("PoolFee"));
                        double nBonus = GetDouble(GetConfigurationKeyValue("PoolBlockBonus"));
                        double nPercOfSubsidy = nBonus / (nTotalSubsidy + .01);

                        double nIndividualPiece = nPercOfSubsidy / (dt1.Rows.Count + .01);

                        double nMinBonusShareThresh = GetDouble(GetConfigurationKeyValue("MinBlockBonusThreshhold"));

                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            //double nShare = GetDouble(dt1.Rows[i]["Shares"]) / nTotalShares;
                            double nMinerShares = (GetDouble(dt1.Rows[i]["Shares"]) + (GetDouble(dt1.Rows[i]["bxmr"])));
                            double nMinerFee = nMinerShares * nPoolFee;

                            nMinerShares = nMinerShares - nMinerFee;
                            double nShare = nMinerShares / nTotalShares;
                            // Add on the extra bonus
                            if (nMinerShares > nMinBonusShareThresh)
                                nShare += nIndividualPiece;

                            sql = "Update " + sTable + " set Percentage=@percentage,Reward=@percentage*Subsidy where " + sHeightRange + " and bbpaddress=@bbpaddress;";
                            MySqlCommand command = new MySqlCommand(sql);
                            command.Parameters.AddWithValue("@percentage", Math.Round(nShare, 4));
                            command.Parameters.AddWithValue("@height", iMyHeight);
                            command.Parameters.AddWithValue("@bbpaddress", dt1.Rows[i]["bbpaddress"]);
                            BMSCommon.Database.ExecuteNonQuery(command);
                         }
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Group Shares " + ex.Message);
            }
        }

        public static string PoolBonusNarrative()
        {
            double nBonus = GetDouble(GetConfigurationKeyValue("PoolBlockBonus"));
            if (nBonus > 0)
            {
                double nMinBonusShareThresh = GetDouble(GetConfigurationKeyValue("MinBlockBonusThreshhold"));

                string sNarr = "We are giving away an extra " + nBonus.ToString() + " BBP per block split equally across participating miners who have more than " + nMinBonusShareThresh.ToString() + " shares in the leaderboard (see Block Bonus).";
                return sNarr;
            }
            return "";
        }

        public static string GetBlock(bool fTestNet, string sCommand, int iBlockNumber, string sJSONFieldName)
        {
            try
            {
                NBitcoin.RPC.RPCClient n = BMSCommon.WebRPC.GetRPCClient(fTestNet);
                object[] oParams = new object[1];
                oParams[0] = iBlockNumber.ToString();
                dynamic oOut = n.SendCommand("getblock", oParams);
                string sOut = oOut.Result[sJSONFieldName].ToString();
                return sOut;
            }
            catch (Exception)
            {
                return "";
            }
        }
    

        private static object cs_stratum = new object();
        public void GetBlockForStratum()
        {
            int nAge = UnixTimestamp() - _template.updated;
            if (nAge < 15)
                return;

            lock (cs_stratum)
            {
                try
                {
                    // When it expires, get new template
                    NBitcoin.RPC.RPCClient n = BMSCommon.WebRPC.GetRPCClient(_fTestNet);
                    string sPAKey = _fTestNet ? "tPoolAddress" : "PoolAddress";

                    string poolAddress = GetConfigurationKeyValue(sPAKey);
                    object[] oParams = new object[1];
                    oParams[0] = poolAddress;
                    dynamic oOut = n.SendCommand("getblockforstratum", oParams);
                    _template = new BlockTemplate();
                    _template.hex = oOut.Result["hex"].ToString();
                    _template.curtime = oOut.Result["curtime"].ToString();
                    _template.prevhash = oOut.Result["prevblockhash"].ToString();
                    _template.height = (int)oOut.Result["height"];
                    _template.bits = oOut.Result["bits"].ToString();
                    _template.prevblocktime = oOut.Result["prevblocktime"].ToString();
                    _template.target = oOut.Result["target"].ToString();
                    _template.updated = UnixTimestamp();
                    if (nGlobalHeight != _template.height)
                    {
                        MarkForBroadcast();
                    }
                    nGlobalHeight = _template.height;
                }
                catch (Exception ex)
                {
                    bool f11000 = false;

                    // BMSCommon.Common.Log("GBFS1.1 " + ex.Message, true);
                }
            }
        }


        private bool SendXMRPacketToMiner(Socket oClient, byte[] oData, int iSize)
        {
            try
            {
                    oClient.Send(oData, iSize, SocketFlags.None);
                    return true;
            }
            catch (Exception ex)
            {
                    if (ex.Message.Contains("was aborted"))
                    {

                    }
                    else
                    {
                        bool fPrint = !(ex.Message.Contains("A connection attempt failed because the connected party did not properly respond after a period of time")
                            || ex.Message.Contains("An existing connection was forcibly closed"));
                        if (fPrint)
                            Log("SEND " + ex.Message);
                    }
                    return false;
                }
            }
            private bool SubmitBiblePayShare(string socketid)
            {

                int nLowHeight = _fTestNet ? 100000 : 300000;
                if (_template.height < nLowHeight)
                {
                    //Chain is still syncing...
                    return false;
                }

                try
                {
                    string sTable = _fTestNet ? "tshare" : "share";
                    PoolBase.XMRJob x = RetrieveXMRJob(socketid);
                    if (x.hash == null || x.hash == "")
                    {
                        //BMSCommon.Common.Log("SubmitBBPShare::emptyhash");
                        return false;
                    }

                    byte[] oRX = BMSCommon.Common.StringToByteArr(x.hash);
                    double nSolutionDiff = FullTest(oRX);
                    string revBBPHash = BMSCommon.Common.ReverseHexString(x.hash);
                    // Check to see if this share actually solved the block:
                    NBitcoin.uint256 uBBP = new NBitcoin.uint256(revBBPHash);
                    NBitcoin.uint256 uTarget = new NBitcoin.uint256(GetBlockTemplate().target);
                    NBitcoin.arith256 aBBP = new NBitcoin.arith256(uBBP);
                    NBitcoin.arith256 aTarget = new NBitcoin.arith256(uTarget);
                    int nTest = aBBP.CompareTo(aTarget);

                    if (aBBP.CompareTo(aTarget) == -1)
                    {
                    // We solved the block
                        string poolAddress = _fTestNet ? GetConfigurationKeyValue("tPoolAddress") : GetConfigurationKeyValue("PoolAddress");
                        string hex = BMSCommon.WebRPC.GetBlockForStratumHex(IsTestNet(), poolAddress, x.seed, x.solution);
                        bool fSuccess = BMSCommon.WebRPC.SubmitBlock(IsTestNet(), hex);
                        if (fSuccess)
                        {
                            string sql = "Update " + sTable + " set Solved=1 where height=@height;";
                            MySqlCommand command = new MySqlCommand(sql);

                            command.Parameters.AddWithValue("@height", _template.height);
                            BMSCommon.Database.ExecuteNonQuery(command);
                            Log("SUBMIT_SUCCESS: Success for nonce " + x.nonce + " at height " + _template.height.ToString() + " hex " + hex);
                        }
                        else
                        {
                            Log("SUBMITBLOCK: Tried to submit the block for nonce " + x.nonce + " and target "
                                + _template.target + " with seed " + x.seed + " and solution "
                                + x.solution + " with hex " + hex + " and failed");
                        }

                        _template.updated = 0; // Forces us to get a new block
                        GetBlockForStratum();
                    }
                    else
                    {
                        GetBlockForStratum();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Log("submitshare::Unable to submit bbp share:  " + ex.Message);
                }
                return false;
            }

            private void PersistWorker(WorkerInfo w)
            {
                try
                {
                    if (w.IP == null || w.IP == "")
                        return;
                string sTable = _fTestNet ? "tworker" : "worker";

                string sql = "SELECT moneroaddress FROM " + sTable + "  WHERE moneroaddress=@m;";
                MySqlCommand m1 = new MySqlCommand(sql);
                m1.Parameters.AddWithValue("@m", w.moneroaddress);
                DataTable mdata = BMSCommon.Database.GetDataTable(m1);
                if (mdata.Rows.Count > 0)
                    return;


                sql = "INSERT INTO " + sTable + " (id,added,moneroaddress,bbpaddress,ip) values (uuid(), now(),"
                              + "@m,@b,'" + GetIPOnly(w.IP) + "');";
                    MySqlCommand m = new MySqlCommand(sql);
                    m.Parameters.AddWithValue("@m", w.moneroaddress);
                    m.Parameters.AddWithValue("@b", w.bbpaddress);
                 bool f2=   BMSCommon.Database.ExecuteNonQuery(m);

                bool f1001 = false;

                }
                catch (Exception ex)
                {
                    Log("Exception PW " + ex.Message);
                }
            }

            private static double ConvertTargetToDifficulty(PoolBase.XMRJob x)
            {
                string sDiff = "000000" + x.target + "0000000000000000000000000000000000000000000000000000000000000000";
                sDiff = sDiff.Substring(0, 64);
                System.Numerics.BigInteger biDiff = new System.Numerics.BigInteger(BMSCommon.Common.StringToByteArr(sDiff));
                System.Numerics.BigInteger biMin = new System.Numerics.BigInteger(BMSCommon.Common.StringToByteArr("0x00000000FFFF0000000000000000000000000000000000000000000000000000"));
                System.Numerics.BigInteger bidiff = System.Numerics.BigInteger.Divide(biMin, biDiff);
                double nDiff = GetDouble(bidiff.ToString());
                return nDiff;
            }

            private static double WeightAdjustedShare(PoolBase.XMRJob x)
            {
                if (x.difficulty <= 0)
                    return 0;
                double nAdj = x.difficulty / 256000;
                return nAdj;
            }

            private static int nDebugCount = 0;
            private void minerXMRThread(Socket client, TcpClient t, string socketid)
            {
                bool fCharity = false;
                string bbpaddress = String.Empty;
                string moneroaddress = String.Empty;
                double nTrace = 0;
                string sData = String.Empty;
                string sParseData = String.Empty;
                int nLastReceived = UnixTimestamp();
                // Miner to XMR Pool
                try
                {
                    client.ReceiveTimeout = 65000;
                    client.SendTimeout = 65000;

                    while (true)
                    {
                        int size = 0;
                        int nElapsed = UnixTimestamp() - nLastReceived;

                        if (nElapsed > (60 * 60 * 5))
                        {
                            client.Close();
                            t.Close();
                            iXMRThreadCount--;
                            return;
                        }
                        if (!client.Connected)
                        {
                            iXMRThreadCount--;
                            return;
                        }

                        WorkerInfo w1 = GetWorker(socketid);
                        bool fBanned = w1.banneduntil > BMSCommon.Common.UnixTimestamp();

                        // From Miner to Pool
                        if (!fBanned && client.Available > 0)
                        {
                            byte[] data = new byte[client.Available];
                            nLastReceived = UnixTimestamp();

                            try
                            {
                                size = client.Receive(data);
                                nTrace = 1;
                            }
                            catch (ThreadAbortException)
                            {
                                return;
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.Contains("An existing connection was forcibly closed"))
                                {
                                    Console.WriteLine("ConnectionClosed");
                                    return;
                                }
                                else if (ex.Message.Contains("was aborted"))
                                {
                                    return;
                                }
                                Console.WriteLine("Error occurred while receiving data " + ex.Message);
                            }
                            if (size > 0)
                            {
                                nTrace = 2;
                                sData = Encoding.UTF8.GetString(data, 0, data.Length);
                                sData = sData.Replace("\0", "");
                                // {"id":107,"jsonrpc":"2.0","method":"submit","params":{"id":"1","job_id":"5","nonce":"08af0200","result":"542      
                                // We are seeing nTrace==2, with a truncation occurring around position 107 having no json terminator:
                                if (sData.Contains("jsonrpc") && sData.Contains("submit") && sData.Contains("params") && sData.Length < 128)
                                {
                                    if (sData.Contains("{") && sData.Contains("id") && !sData.Contains("}"))
                                    {
                                        //Log("XMRPool::Received " + socketid + " truncated message.  ", true);
                                        iXMRThreadCount--;
                                        client.Close();
                                        WorkerInfo wban = Ban(socketid, 1, "BAD-CONFIG");
                                        return;
                                    }
                                }

                                // The Stratum data is first split by \r\n
                                string[] vData = sData.Split("\n");
                                for (int i = 0; i < vData.Length; i++)
                                {
                                    string sJson = vData[i];
                                    if (sJson.Contains("submit"))
                                    {
                                        // See if this is a biblepay share:
                                        if (fMonero2000)
                                        {
                                            sParseData = sJson;
                                            JObject oStratum = JObject.Parse(sJson);
                                            string nonce = "00000000" + oStratum["params"]["nonce"].ToString();
                                            double nJobID = GetDouble(oStratum["params"]["job_id"].ToString());
                                            string hash = oStratum["params"]["result"].ToString();
                                            XMRJob xmrJob = RetrieveXMRJob(socketid);
                                            string rxheader = xmrJob.blob;
                                            string rxkey = xmrJob.seed;

                                            if (rxheader == null)
                                            {
                                                //Log("cant find the job " + nJobID.ToString());
                                                WorkerInfo wban = Ban(socketid, 1, "CANT-FIND-JOB");
                                            }
                                            if (rxheader != null)
                                            {
                                                nTrace = 4;
                                                nonce = nonce.Substring(8, 8);
                                                xmrJob.solution = rxheader.Substring(0, 78) + nonce + rxheader.Substring(86, rxheader.Length - 86);
                                                xmrJob.hash = oStratum["params"]["result"].ToString();
                                                xmrJob.hashreversed = BMSCommon.Common.ReverseHexString(hash);
                                                xmrJob.nonce = nonce;
                                                xmrJob.bbpaddress = bbpaddress;
                                                xmrJob.moneroaddress = moneroaddress;
                                                PutXMRJob(xmrJob);
                                                SubmitBiblePayShare(xmrJob.socketid);
                                            }
                                        }
                                    }
                                    else if (sJson.Contains("login"))
                                    {
                                        //{"id":1,"jsonrpc":"2.0","method":"login","params":{"login":"41s2xqGv4YLfs5MowbCwmmLgofywnhbazPEmL2jbnd7p73mtMH4XgvBbTxc6fj4jUcbxEqMFq7ANeUjktSiZYH3SCVw6uat","pass":"x","agent":"bbprig/5.10.0 (Windows NT 6.1; Win64; x64) libuv/1.34.0 gcc/9.2.0","algo":["cn/0","cn/1","cn/2","cn/r","cn/fast","cn/half","cn/xao","cn/rto","cn/rwz","cn/zls","cn/double","","cn-lite/0","cn-lite/1","cn-heavy/0","cn-heavy/tube","cn-heavy/xhv","cn-pico","cn-pico/tlo","rx/0","rx/wow","rx/loki","rx/arq","rx/sfx","rx/keva","argon2/chukwa","argon2/wrkz","astrobwt"]}}
                                        nTrace = 8;
                                        sParseData = sJson;
                                        if (sJson.Contains("User-Agent:") || sJson.Contains("HTTP/1.1") || sJson.Contains("xmrig-proxy"))
                                        {
                                            // Someone is trying to connect to the pool with a web browser?  (Instead of a miner):
                                            if (false)
                                                Log("XMRPool::Received " + socketid + " Web browser Request ");
                                            iXMRThreadCount--;
                                            client.Close();
                                            WorkerInfo wban = Ban(socketid, 1, "BAD-CONFIG");
                                            return;
                                        }
                                        JObject oStratum = JObject.Parse(sJson);
                                        dynamic params1 = oStratum["params"];
                                        if (fMonero2000)
                                        {
                                            moneroaddress = params1["login"].ToString();
                                            bbpaddress = params1["pass"].ToString();
                                            if (bbpaddress.Length != 34 || moneroaddress.Length < 95)
                                            {
                                                iXMRThreadCount--;
                                                client.Close();
                                                WorkerInfo wban = Ban(socketid, 1, "BAD-CONFIG");
                                                return;
                                            }
                                            WorkerInfo w = GetWorker(socketid);
                                            w.moneroaddress = moneroaddress;
                                            w.bbpaddress = bbpaddress;
                                            w.IP = GetIPOnly(socketid);
                                            SetWorker(w, socketid);
                                            PersistWorker(w);
                                        }
                                        nTrace = 10;
                                    }
                                    else if (sJson.Contains("keepalive"))
                                    {
                                        // No Op (Leave in)
                                    }
                                    else if (sJson != "")
                                    {
                                        Console.WriteLine("msg1:"+sJson);
                                    }
                                }

                                // Miner->XMR Pool
                                Stream stmOut = t.GetStream();
                                stmOut.Write(data, 0, size);
                            }
                            else
                            {
                                if (true)
                                {
                                    // Keepalive (prevents the pool from hanging up on the miner)
                                    nTrace = 15;
                                    var json = "{ \"id\": 0, \"method\": \"keepalived\", \"arg\": \"na\" }\r\n";
                                    data = Encoding.ASCII.GetBytes(json);
                                    Stream stmOut = t.GetStream();
                                    stmOut.Write(data, 0, json.Length);
                                }
                            }
                        }

                        // ****************************************** In from XMR Pool -> Miner *******************************************************
                        nTrace = 16;
                        NetworkStream stmIn = t.GetStream();
                        nTrace = 18;
                        int bytesIn = 0;

                        try
                        {
                            t.ReceiveTimeout = SOCKET_TIMEOUT;
                            t.SendTimeout = SOCKET_TIMEOUT;
                            nTrace = 19;

                            if (stmIn.DataAvailable)
                            {

                                byte[] bIn = new byte[65536];

                                bytesIn = stmIn.Read(bIn, 0, 65536);
                                if (bytesIn > 0)
                                {
                                    nTrace = 20;
                                    sData = Encoding.UTF8.GetString(bIn, 0, bytesIn);
                                    sData = sData.Replace("\0", "");
                                    string[] vData = sData.Split("\n");
                                    for (int i = 0; i < vData.Length; i++)
                                    {
                                        string sJson = vData[i];
                                        if (sJson.Contains("result"))
                                        {
                                            WorkerInfo w = GetWorker(socketid);
                                            SetWorker(w, socketid);
                                            JObject oStratum = JObject.Parse(sJson);
                                            string status = oStratum["result"]["status"].ToString();
                                            int id = (int)GetDouble(oStratum["id"]);
                                            if (id == 1 && status == "OK" && sJson.Contains("blob"))
                                            {
                                                // BiblePay Pool to Miner
                                                nTrace = 22;
                                                double nJobId = GetDouble(oStratum["result"]["job"]["job_id"].ToString());
                                                XMRJob x = RetrieveXMRJob(socketid);
                                                x.blob = oStratum["result"]["job"]["blob"].ToString();
                                                x.target = oStratum["result"]["job"]["target"].ToString();
                                                x.seed = oStratum["result"]["job"]["seed_hash"].ToString();
                                                x.difficulty = ConvertTargetToDifficulty(x);
                                                PutXMRJob(x);
                                            }
                                            else if (id > 1 && status == "OK")
                                            {
                                                // They solved an XMR
                                                int iCharity = fCharity ? 1 : 0;
                                                nTrace = 24;
                                                // Weight adjusted share
                                                XMRJob x = RetrieveXMRJob(socketid);
                                                double nShareAdj = WeightAdjustedShare(x);
                                                nDebugCount++;
                                                System.Diagnostics.Debug.WriteLine("solved " + nDebugCount.ToString());
                                                InsShare(bbpaddress, nShareAdj, 0, _template.height, nShareAdj, iCharity, moneroaddress);
                                            }
                                            else if (id > 1 && status != "OK" && status != "KEEPALIVED")
                                            {
                                                nTrace = 25;
                                                InsShare(bbpaddress, 0, 1, _template.height, 0, 0, moneroaddress);
                                            }
                                        }
                                        else if (sJson.Contains("submit"))
                                        {
                                            // Noop
                                        }
                                        else if (sJson.Contains("\"method\":\"job\""))
                                        {
                                            nTrace = 26;
                                            JObject oStratum = JObject.Parse(sJson);
                                            nTrace = 27;
                                            double nJobId = GetDouble(oStratum["params"]["job_id"].ToString());
                                            XMRJob x = RetrieveXMRJob(socketid);
                                            x.blob = oStratum["params"]["blob"].ToString();
                                            x.target = oStratum["params"]["target"].ToString();
                                            x.seed = oStratum["params"]["seed_hash"].ToString();
                                            x.difficulty = ConvertTargetToDifficulty(x);
                                            PutXMRJob(x);
                                            nTrace = 27.9;
                                        }
                                        else if (sJson != "")
                                        {
                                            Console.WriteLine("msg15:"+sJson);
                                            if (sJson.ToLower().Contains("invalid share"))
                                            {
                                               // Lets ban the user for 5 mins otherwise the XMR pool might ban us.
                                               WorkerInfo w = GetWorker(socketid);  
                                               w.banneduntil = BMSCommon.Common.UnixTimestamp() + (60 * 60 * 5);
                                               SetWorker(w, socketid);
                                            }
                                        }
                                    }
                                }

                                // Back to Miner
                                if (bytesIn > 0)
                                {
                                    // This goes back to the miner
                                    SendXMRPacketToMiner(client, bIn, bytesIn);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!ex.Message.Contains("did not properly respond"))
                            {
                                Log("minerXMRThread[0]: Trace=" + nTrace.ToString() + ":" + ex.Message);
                                Ban(socketid, 1, ex.Message.Substring(0, 12));
                            }
                        }

                        Thread.Sleep(100);
                    }
                }
                catch (ThreadAbortException)
                {
                    // Log("minerXMRThread is going down...", true);
                    return;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("was aborted"))
                    {
                        // Noop
                    }
                    else if (ex.Message.Contains("forcibly closed"))
                    {

                    }
                    else if (!ex.Message.Contains("being aborted"))
                    {
                        // This is where we see Unexpected end of content while loading JObject. Path 'params.job_id', line 1, position 72.
                        // and Unterminated string. Expected delimiter: ". Path 'params.result', line 1, position 144.
                        // Invalid character after parsing property name. Expected ':' but got:
                        // and Unterminated string. Expected delimiter: ". Path 'params.id', line 1, position 72.
                        Log("minerXMRThread2 v2.1: " + ex.Message + " [sdata=" + sData + "], Trace=" + nTrace.ToString() + ", PARSEDATA     \r\n" + sParseData);
                    }
                }
                iXMRThreadCount--;
            }


            // Initialize a new XMR Pool
            private void InitializeXMR()
            {

               TcpListener listener = null;

               retry:

                if (listener != null)
                {
                    listener.Stop();
                    System.Threading.Thread.Sleep(5000);
                }

                try
                {
                    {
                        string poolAccount = GetConfigurationKeyValue("PoolPayAccount");
                        string sPAKey = _fTestNet ? "tPoolAddress" : "PoolAddress";
                        string sPoolAddress = GetConfigurationKeyValue(sPAKey);

                        if (poolAccount == "" || sPoolAddress == "")
                        {
                            BMSCommon.Common.Log("This pool configuration has no key set for PoolPayAccount.  Unable to start pool.  You also need to set PoolAddress.  ");
                            return;
                        }

                        int nPortMainnet = (int)GetDouble(GetConfigurationKeyValue("XMRPort"));
                        int nPortTestNet = (int)GetDouble(GetConfigurationKeyValue("XMRPortTestNet"));
                        if (nPortMainnet == 0)
                           nPortMainnet = 3001;
                        if (nPortTestNet == 0)
                            nPortTestNet = 3002;

                        int nPort = IsTestNet() ? nPortTestNet : nPortMainnet;

                        listener = new TcpListener(IPAddress.Any, nPort);
                        listener.Start();
                        double nPoolBalance = BMSCommon.WebRPC.GetCoreWalletBalance(_fTestNet);

                        string sNarr = "BBP XMR POOL is starting up on port " + nPort.ToString() + " with a balance of " + nPoolBalance.ToString() + " on address " + sPoolAddress;
                        BMSCommon.WebRPC.LogRPCError(sNarr);
                        Log(sNarr);

                    }
                }
                catch (Exception ex1)
                {
                    Log("Problem starting XMR pool:" + ex1.Message);
                     goto retry;
                }

                while (true)
                {
                    //  Complimentary outbound socket
                    try
                    {
                        Thread.Sleep(10);
                        if (listener.Pending())
                        {
                            Socket client = listener.AcceptSocket();
                        try
                        {
                            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                        }
                        catch(Exception ex)
                        {
                            BMSCommon.Common.Log("NonCrit reuseaddr " + ex.Message);
                        }
                        try
                        {
                            if (BMSCommon.Common.IsWindows())
                                  client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, false);
                        }
                        catch(Exception ex)
                        {
                            BMSCommon.Common.Log("Logging non-critical dontlinger " + ex.Message);
                        }

                            int nSockTrace = 0;
                            string socketid = client.RemoteEndPoint.ToString();
                            try
                            {
                                WorkerInfo wban = Ban(socketid, .25, "XMR-Connect");
                                nSockTrace = 1;

                                if (!wban.banned)
                                {
                                    iXMRThreadID++;
                                    TcpClient tcp = new TcpClient();
                                    nSockTrace = 2;
                                // MineXMR.com is going out of business (old pool=pool.minexmr.com, newpool=pool.supportxmr.com)

                                string XMRExternalPool = GetConfigurationKeyValue("XMRExternalPool");
                                if (XMRExternalPool == "")
                                      XMRExternalPool = "pool.supportxmr.com";
                                    int nExternalPort = (int)GetDouble(GetConfigurationKeyValue("XMRExternalPort"));
                                    if (nExternalPort == 0)
                                      nExternalPort = 5555;

                                    tcp.Connect(XMRExternalPool, nExternalPort);
                                    nSockTrace = 3;

                                    ThreadStart starter = delegate { minerXMRThread(client, tcp, socketid); };
                                    var childSocketThread = new Thread(starter);
                                    nSockTrace = 4;

                                    iXMRThreadCount++;
                                    nSockTrace = 5;

                                    childSocketThread.Start();
                                    nSockTrace = 6;

                                }
                                else
                                {
                                    // They are already banned
                                    nSockTrace = 7;
                                    CloseSocket(client);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log("We have a big issue answering sockets " + ex.Message + ", sock trace=" + nSockTrace.ToString());
                            }
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        Log("XMR Pool is going down...");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Log("InitializeXMRPool v1.3: " + ex.Message);
                        Thread.Sleep(5000);
                        goto retry;
                    }
                }
            }

    }
}
