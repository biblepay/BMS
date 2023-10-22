using System;
using System.Collections.Generic;
using System.Text;

namespace BMSCommon.Model
{

    public class Proposal
    {
        public DateTime Added { get; set; }
        public string URL { get; set; }
        public String SubmitTXID { get; set; }
        public String PrepareTXID { get; set; }
        public String NickName { get; set; }
        public string ExpenseType { get; set; }
        public string Name { get; set; }
        public int nStartTime { get; set; }
        public int nEndTime { get; set; }
        public string BBPAddress { get; set; }
        public double Amount { get; set; }
        public string Chain { get; set; }
        public string id { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Submitted { get; set; }
        public string Hex { get; set; }
        public string ERC20Address { get; set; }
        public User User { get; set; }
        public bool TestNet { get; set; }

    }

    public class CharityReport
    {
        public DateTime Added { get; set; }
        public string Type { get; set; }
        public string Notes { get; set; }
        public double Amount { get; set; }
    }

    public class OrphanExpense
    {
        public DateTime Added { get; set; }
        public double Amount { get; set; }
        public string URL { get; set; }
        public string Charity { get; set; }
        public string HandledBy { get; set; }
        public string ChildID { get; set; }
        public double Balance { get; set; }
        public string Notes { get; set; }
        public int Version = 9;
        public string id { get; set; }
        public string table = "OrphanExpense3";

    }

    public class VerseMemorizer
    {
        public string id { get; set; }
        public string BookFrom { get; set; }
        public string BookTo { get; set; }
        public int ChapterFrom { get; set; }
        public int VerseFrom { get; set; }
        public int ChapterTo { get; set; }
        public int VerseTo { get; set; }
        public DateTime Added { get; set; }
    }
    public class SponsoredOrphan
    {
        public string id { get; set; }
        public string ChildID { get; set; }
        public string Charity { get; set; }
        public string BioURL { get; set; }
        public DateTime Added { get; set; }
        public double MonthlyAmount { get; set; }
        public string Name
        {
            get; set;
        }
        public String BioPicture { get; set; }
        public int Active
        {
            get; set;
        }
    }


    public class Expense
    {
        public string Added { get; set; }
        public double Amount { get; set; }
        public string URL { get; set; }
        public string Charity { get; set; }
        public string HandledBy { get; set; }
        public string Notes { get; set; }
        public string table = "Expense";
        public string id { get; set; }
    }

    public class Revenue
    {
        public string Added { get; set; }
        public double BBPAmount { get; set; }
        public double BTCRaised { get; set; }
        public double BTCPrice { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }
        public string HandledBy { get; set; }
        public string Charity { get; set; }
        public string table = "Revenue";
        public string id { get; set; }
    }
}
