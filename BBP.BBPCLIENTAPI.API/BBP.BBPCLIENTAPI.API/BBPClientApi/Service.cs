using BMSCommon.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static BMSCommon.Common;

namespace BBPAPI
{
	public static class Service
    {
        public static List<MasternodeListItem> _mlSanctuaries = new List<MasternodeListItem>();
        public static bool IsPrimary()
        {
            string sBindURL = GetConfigKeyValue("bindurl");
            bool fPrimary = sBindURL.Contains("sanc1") || sBindURL.Contains("209.145.56.214");
            return fPrimary;
        }
        
        internal static bool _IsSanctuary()
        {
            string sBindURL = GetConfigKeyValue("bindurl");
            sBindURL = sBindURL.Replace("https://", "");
            sBindURL = GetElement(sBindURL, 0, ":");
            bool has = _mlSanctuaries.Any(c => c.address.Contains(sBindURL));
            return has;
        }

        private static bool? fIsSanctuary = false;
        public static bool IsSanctuary()
        {
            if (fIsSanctuary == null)
            {
                fIsSanctuary = _IsSanctuary();
            }
            return (fIsSanctuary == true)  ? true : false;
        }








        public static void RegisterProControls(Action<string> o)
        {
           
            string sLic = "Ngo9BigBOggjHTQxAR8/V1NGaF1cWGhIfEx1RHxQdld5ZFRHallYTnNWUj0eQnxTdEZjUX5ZcX1QRmBYVEx1Vw==";
            o(sLic);
            if (File.Exists(Directory.GetCurrentDirectory() + "/wwwroot/scripts/index.js"))
            {
                    string regexPattern = "ej.base.registerLicense(.*);";
                    string jsContent = File.ReadAllText(Directory.GetCurrentDirectory() + "/wwwroot/scripts/index.js");
                    MatchCollection matchCases = Regex.Matches(jsContent, regexPattern);
                    foreach (Match matchCase in matchCases)
                    {
                        var replaceableString = matchCase.ToString();
                        jsContent = jsContent.Replace(replaceableString, "ej.base.registerLicense('" + sLic + "');");
                    }
                    File.WriteAllText(Directory.GetCurrentDirectory() + "/wwwroot/scripts/index.js", jsContent);
            }
            
        }





    }
}
