using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BiblePay.BMS.DSQL
{
    public static class PoolBase
    {
        public static XMRPoolBase tPool = null;
        public static XMRPoolBase mPool = null;
        public static List<MySqlCommand> lSQL = new List<MySqlCommand>();
        public static int pool_version = 1030;

        public struct XMRJob
        {
            public string blob;
            public double jobid0;
            public string socketid;
            public int timestamp;
            public string target;
            public double difficulty;
            public string seed;
            public string solution;
            public string nonce;
            public string hash;
            public string hashreversed;
            public string bbpaddress;
            public string moneroaddress;
            public bool fNeedsSubmitted;
        }

        public struct WorkerInfo
        {
            public string bbpaddress;
            public string moneroaddress;
            public int difficulty;
            public int nextdifficulty;
            public int height;
            public int jobid;
            public int updated;
            public long banneduntil;
            public bool Broadcast;
            public int receivedtime;
            public int lastreceived;
            public string IP;
            public bool reset;
            public int solvetime;
            public int priorsolvetime;
            public double banlevel;
            public int starttime;
            public bool logged;
            public bool banned;
        }


        public static void PoolService()
        {
            BMSCommon.Common.Log("PoolService::Start New Pools");

            tPool = new XMRPoolBase(true);
            mPool = new XMRPoolBase(false);
            BMSCommon.Common.Log("PoolService::Finished Starting New Pools");

            // Services - Executes batch jobs
            while (true)
            {
 
                System.Threading.Thread.Sleep(60000);
                if (!Debugger.IsAttached || true)
                {
                    tPool.GroupShares();
                    tPool.Leaderboard();
                    PoolPayments.PayPoolParticipants(tPool);
                    tPool.PurgeSockets(false);
                    tPool.PurgeJobs();
                    // Main
                    mPool.GroupShares();
                    mPool.Leaderboard();
                    PoolPayments.PayPoolParticipants(mPool);
                    mPool.PurgeSockets(false);
                    mPool.PurgeJobs();
                }
            }
        }

        public static void SQLExecutor()
        {
            while (true)
            {
                try
                {
                    // This thread executes SQL in a way that prevents deadlocks
                    for (int i = 0; i < lSQL.Count; i++)
                    {
                        try
                        {
                            BMSCommon.Database.ExecuteNonQuery2(lSQL[i]);
                        }
                        catch (Exception ex2)
                        {
                            try
                            {
                                BMSCommon.Common.Log("SQLExecutor::" + ex2.Message + ":" + lSQL[i].CommandText);
                            }
                            catch (Exception x)
                            {

                            }
                        }
                        lSQL.RemoveAt(i);
                        i--;
                    }
                }catch(Exception e10)
                {

                }
                System.Threading.Thread.Sleep(100);
            }
        }

        public static void NewPool()
        {
            var t1 = new Thread(PoolService);
            t1.Start();
            var t2 = new Thread(SQLExecutor);
            t2.Start();
        }
    }

}
