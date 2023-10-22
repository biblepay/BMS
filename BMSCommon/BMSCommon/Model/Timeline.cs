using System;
using System.Collections.Generic;
using System.Linq;
using static BMSCommon.Common;


namespace BMSCommon.Models
{
    public class Timeline
    {
        public DateTime Added { get; set; }
        public int Time { get; set; }
        public string Body { get; set; }
        public string dataPaste { get; set; }
        public string ERC20Address { get; set; }
        public string BBPAddress { get; set; }
        public string ParentID { get; set; }
        public string id { get; set; }
        public int Version { get; set; }
        

    }

}
