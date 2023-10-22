using BBPAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using static BMSCommon.Common;

namespace BMSCommon.Models
{
    public class Video
    {
        public DateTime Added { get; set; }
        public int Time { get; set; }
        public int Version { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Cover { get; set; }
        public string BBPAddress { get; set; }
        public string Source { get; set; }
        public double Duration { get; set; }
        public string id { get; set; }
        
    }

}
