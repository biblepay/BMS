namespace BiblePay.BMS.Controllers.Retired
{
    public class RetiredController
    {



        /*
        public static async Task<bool> MigrateTable2()
        {
            string sTable = "bms1.NFT";
            string sql = "Select * from " + sTable + " where Deleted=0 and chain='main'";
            NpgsqlCommand m1 = new NpgsqlCommand(sql);
            DataTable dt = BMSCommon.CockroachDatabase.GetDataTable(m1);
            List<Task> l0 = new List<Task>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NFT e = new NFT();
                e.id = dt.Rows[i]["hash"].ToString();
                e.Name = dt.Rows[i]["Name"].ToString();
                e.Action = dt.Rows[i]["Action"].ToString();
                e.Description = dt.Rows[i]["Description"].ToString();
                e.AssetURL = dt.Rows[i]["AssetURL"].ToString();
                e.AssetBIO = dt.Rows[i]["AssetBIO"].ToString();
                e.AssetMonths = (int)GetDouble(dt.Rows[i]["AssetMonths"]);
                e.Type = dt.Rows[i]["Type"].ToString();
                e.MinimumBidAmount = GetDouble(dt.Rows[i]["MinimumBidAmount"]);
                e.ReserveAmount = GetDouble(dt.Rows[i]["ReserveAmount"]);
                e.BuyItNowAmount = GetDouble(dt.Rows[i]["BuyItNowAmount"]);
                e.OwnerERC20Address = dt.Rows[i]["OwnerERC20Address"].ToString();
                e.nIteration = (int)GetDouble(dt.Rows[i]["nIteration"]);
                e.Marketable = (int)GetDouble(dt.Rows[i]["Marketable"]);
                e.Deleted = 0;
                e.Version = (int)GetDouble(dt.Rows[i]["Version"]);
                e.TXID = dt.Rows[i]["TXID"].ToString();
                e.OwnerBBPAddress = dt.Rows[i]["OwnerBBPAddress"].ToString();
                e.Hash = dt.Rows[i]["Hash"].ToString();
                if (e.Version > 2)
                {
                   // l0.Add(StorjIO.StoreDatabaseData<NFT>("nft", e, e.id));
                }
            }
            await Task.WhenAll(l0);
            return true;
        }
        

        public static async Task<bool> MigrateTable3()
        {
            string sTable = "bms1.user";
            string sql = "Select * from " + sTable + " ";
            MySqlCommand m1 = new MySqlCommand(sql);
            DataTable dt = BMSCommon.Database.GetDataTable2(m1);
            double nCheck = 0;
            List<Task> l0 = new List<Task>();
            string sPrivKey = BMSCommon.Common.Value("foundationprivkey");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                User e = new User();
                e.id = dt.Rows[i]["_id"].ToString();
                e.ERC20Address = dt.Rows[i]["erc20address"].ToString();
                e.EmailAddress = Encryption.EncryptAES256(dt.Rows[i]["EmailAddress"].ToString(), sPrivKey);
                e.NickName = dt.Rows[i]["NickName"].ToString();
                e.Updated = dt.Rows[i]["Updated"].ToString();
                e.BioURL = dt.Rows[i]["BioURL"].ToString();
                e.PortfolioBuilderAddress = dt.Rows[i]["PortfolioBuilderAddress"].ToString();
                e.PBSignature = dt.Rows[i]["PBSignature"].ToString();
                e.BBPAddress = dt.Rows[i]["BBPAddress"].ToString();
                //l0.Add(StorjIO.StoreDatabaseData<User>("user", e, e.id));
            }
            await Task.WhenAll(l0);
            // save the total amount here.291246$
            return true;
        }
        */


    }
}
