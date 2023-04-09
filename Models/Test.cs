using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BiblePay.BMS.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name1 { get; set; }
        public string State { get; set; }
        public int Zip { get; set; }

        public string myfield = "haha";
        public DateTime StartDate { get; set; }
    }
}
