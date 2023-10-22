using System;
using System.Collections.Generic;
using System.Text;
using static BMSCommon.Model.BitcoinSyncModel;
using static BMSCommon.Common;

namespace BMSCommon.Model
{
    public class Global
    {
        public static string FoundationPublicKey = "BB2BwSbDCqCqNsfc7FgWFJn4sRgnUt4tsM";
        public static string msContentRootPath = null;

        public static List<BMSNode> mNodes = new List<BMSNode>();
        public static Dictionary<string, BitcoinSyncTransaction> dMemoryPool = new Dictionary<string, BitcoinSyncTransaction>();
        public static List<string> mapTransactions = new List<string>();
        public static int DEFAULT_PORT = 8443;

        public static string xGetCxDN()
        {
            string sCDN = "https://zzzglobalcdn.biblepay.org:" + DEFAULT_PORT.ToString();
            return sCDN;
        }

        /*
        public static string GetPublicCxDN()
        {
            return "https://unchained.biblepay.org";
        }
        */


        

        public static int GetMemPoolCount()
        {
            int nCount = dMemoryPool.Count;
            return nCount;
        }


    }
    public class ChatItem
    {
        public string body = string.Empty;
        public DateTime time;
        public string To = string.Empty;
        public string From = string.Empty;
        public bool TestNet = false;
        public string UserID = string.Empty;
        public string Type = string.Empty;
        public string URL = string.Empty;
    }

    public class Articles
    {
        public string table = "Articles";
        public string Name { get; set; }
        public string Description { get; set; }
        public string Added { get; set; }
        public string id { get; set; }
    }

    public class Wiki
    {
        public string table = "Wiki";
        public string Name { get; set; }
        public string Description { get; set; }
        public string Added { get; set; }
        public string URL { get; set; }
    }

    public class Illustrations
    {
        public string table = "Illustrations";
        public string Name { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public string Added { get; set; }
    }




    public class Ticket
    {
        public string id { get; set; }
        public string ERC20Address { get; set; }
        public string Description { get; set; }
        public string Disposition { get; set; }
        public string Title { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedToBioURL { get; set; }
        public string Status { get; set; }
        public string NickName { get; set; }
        public string BioURL { get; set; }
        public DateTime Added { get; set; }
        public DateTime Updated { get; set; }
        public int TicketNumber { get; set; }
        public int Version { get; set; }
    }

    public class ClientToServer
    {
        public string BBPAddress { get; set; }
        public string ERC20Signature { get; set; }
        public string ExtraData { get; set; }
        public string FormData { get; set; }
        public string Action { get; set; }
    }



    public class TicketHistory
    {
        public string ParentID { get; set; }
        public string Notes { get; set; }
        public string AssignedTo { get; set; }
        public DateTime Added { get; set; }
        public DateTime Updated { get; set; }
        public string Disposition { get; set; }
        public string AssignedToBioURL { get; set; }
        public string id { get; set; }
    }

    public class ServerToClient2
    {
        public string Body { get; set; }
        public string Type { get; set; }
        public string Error { get; set; }
        public ServerToClient2()
        {
            Body = String.Empty;
            Type = String.Empty;
            Error = String.Empty;
        }
    }


    public class DropDownItem
    {
        public string key0 { get; set; }
        public string text0 { get; set; }
        
        public DropDownItem()
        {
            key0 = String.Empty;
            text0 = String.Empty;
        }
    }


    public class Attachment
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string id
        {
            get; set;
        }
        public string URL
        {
            get; set;
        }
        public string ParentID
        {
            get; set;
        }
        public DateTime Added { get; set; }
        public int Version { get; set; }
    }
}
