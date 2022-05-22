using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using BiblePay.BMS;
//using BiblePay.BMS.CustomExtensions;
//using static BiblePay.BMS.Common;
//using BiblePay.BMS.DSQL;
using System.Net;
using System.Diagnostics;

namespace BiblePay.BMSD
{
    class DataLoader
    {
        public void LaunchWebSite(string sURL)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = "chrome";
            process.StartInfo.Arguments = @sURL;
            process.Start();
        }

    }
}
