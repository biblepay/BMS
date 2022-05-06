using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BMSCommon
{
    public static class DSQL
    {

        public static string GetURL(string sPath)
        {
            string sNewURL = BMSCommon.Common.NormalizeURL("https://bbpipfs.s3.filebase.com/" + sPath);
            return sNewURL;
        }
        public static List<string> QueryIPFSFolderContents(string sPath, string sDelim, string sUSERID)
        {
            string sql = "Select * from pin";
            if (sUSERID != "")
            {
                sql += " where userid=@userid;";
            }
            MySqlCommand cmd1 = new MySqlCommand(sql);
            cmd1.Parameters.AddWithValue("@userid", sUSERID);

            DataTable dt = Database.GetMySqlDataTable(false, cmd1, "");
            List<string> l = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sURL = dt.Rows[i]["URL"].ToString();
                bool fTS = sURL.Contains(".ts");
                if (true)
                {
                    l.Add(sURL);
                }
            }
            return l;
        }

    }
}
