using BiblePay.BMS.Extensions;
using BMSCommon;
using BMSCommon.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OptionsShared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using static BiblePay.BMS.DSQL.UI;
using static BMSCommon.BitcoinSync;
using static BMSCommon.BitcoinSyncModel;
using static BMSCommon.CryptoUtils;
using static BMSCommon.Encryption;
using static BMSCommon.Model;

namespace BiblePay.BMS.Controllers
{

    public class IntelController : Controller
    {

        public class Employee
        {
            public string phone;
            public string name;
            public string date1;
            public string email;
            public string num1;
            public string name2;
            public string amt;
            public string dumb1;
        }
        
        public IActionResult MarketingDashboard()
        {
            List<Employee> e = new List<Employee>();
            Employee e1 = new Employee();
            Employee e2 = new Employee();
            e.Add(e1);
            e.Add(e2);
            var model = e;
            ViewBag.DynamicTable = DSQL.UI.GetBasicTable("t", "the new dynamic table");
            ViewBag.Accordian = DSQL.UI.GetAccordian("a1", "[Accordian] Collapse", "<br>A dynamic table inside a dynamic accordian.");
            return View(model);
        }
        public class ClientToServer
        {
            public string BBPAddress { get; set; }
            public string ERC20Signature { get; set; }
            public string ExtraData { get; set; }
            public string FormData { get; set; }
            public string Action { get; set; }
        }

       
        private static string Coalesce(string a,string b, string c, string d)
        {
            if (a != "")
                return a;
            if (b != "")
                return b;
            if (c != "")
                return c;
            if (d != "")
                return d;
            return "";
        }
        private static void UnivStoreAnswer(HttpContext h, string sFormData)
        {
            string sRadioA = GetFormData(sFormData, "radioAnswerA");
            string sRadioB = GetFormData(sFormData, "radioAnswerB");
            string sRadioC = GetFormData(sFormData, "radioAnswerC");
            string sRadioD = GetFormData(sFormData, "radioAnswerD");
            string sChosen = Coalesce(sRadioA, sRadioB, sRadioC, sRadioD);
            UnivFinalExamController.RecordAnswer(sChosen, h);
        }

        private static void VerifyUserExists(User u)
        {
            if (u.LoggedIn == false)
                return;
            string sql = "Select * from Users where id=@UserID;";
            SqlCommand s1 = new SqlCommand(sql);
            s1.Parameters.AddWithValue("@UserID", u.ERC20Address);
            DataTable dt1 = SQLDatabase.GetDataTable(s1);
            if (dt1.Rows.Count == 0)
            {
                string sql2 = "Insert into Users (id,added,updated,nickname) values (@UserID,getdate(),getdate(),@nickname);";
                SqlCommand s2 = new SqlCommand(sql2);
                s2.Parameters.AddWithValue("@UserID", u.ERC20Address);
                s2.Parameters.AddWithValue("@nickname", u.NickName);
                SQLDatabase.ExecuteNonQuery(s2,"localhost");

            }

        }

        [HttpPost]
        public async Task<JsonResult> ProcessDoCallback([FromBody] ClientToServer o)
        {
            ServerToClient returnVal = new ServerToClient();
            User u0 = await GetUser(HttpContext);

            if (o.Action == "Profile_SaveQuant")
            {
                double nLiq = GetFormData(o.FormData, "txtNetLiq").ToDouble();
                if (nLiq > 0)
                {
                    User u1 = await GetUser(this.HttpContext);
                    VerifyUserExists(u1);
                    string sql = "Update Users set NetLiquidationValue=@NetLiq where ID=@UserID;";
                    SqlCommand s1 = new SqlCommand(sql);
                    s1.Parameters.AddWithValue("@UserID", u1.ERC20Address);
                    s1.Parameters.AddWithValue("@NetLiq", nLiq.ToString());
                    bool f = SQLDatabase.ExecuteNonQuery(s1, "localhost");
                    string sResult = f ? "Saved." : "Failed to save user record.";
                    string modal = DSQL.UI.GetModalDialogJson("Save User Record", sResult);
                    return Json(modal);
                }


            }
            else if (o.Action == "Profile_Save")
            {
                BMSCommon.CryptoUtils.User u = await  DSQL.UI.GetUser(HttpContext);
                bool fTestNet = DSQL.UI.IsTestNet(HttpContext);

                u.EmailAddress = EncAES(GetFormData(o.FormData, "txtEmailAddress"));
                u.Updated = System.DateTime.Now.ToString();
                u.NickName = GetFormData(o.FormData, "txtNickName");
                u.ERC20Address = GetFormData(o.FormData, "txtERC20Address");

                if (fTestNet)
                {
                    u.tPBSignature = GetFormData(o.FormData, "txtPBSignature");
                    u.tPortfolioBuilderAddress = GetFormData(o.FormData, "txtPortfolioBuilderAddress");
                }
                else
                {
                    u.PBSignature = GetFormData(o.FormData, "txtPBSignature");
                    u.PortfolioBuilderAddress = GetFormData(o.FormData, "txtPortfolioBuilderAddress");
                }


                DSQL.UI.SetUser(u, HttpContext);
                bool f = await BMSCommon.CryptoUtils.PersistUser(IsTestNet(HttpContext), u);
                string sResult = f ? "Saved." : "Failed to save user record.";
                string modal = DSQL.UI.GetModalDialogJson("Save User Record", sResult);
                return Json(modal);
            }
            else if (o.Action == "PortfolioBuilder_ToggleMode")
            {
                if (HttpContext.Session.GetString("PortfolioBuilderLeaderboardMode") != "Detail")
                {
                    HttpContext.Session.SetString("PortfolioBuilderLeaderboardMode", "Detail");
                }
                else
                {
                    HttpContext.Session.SetString("PortfolioBuilderLeaderboardMode", "Summary");
                }

                string m = "location.href='/bbp/portfoliobuilderleaderboard';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "Profile_Authenticate")
            {
                BMSCommon.CryptoUtils.User u = await DSQL.UI.GetUser(HttpContext);

                return Json("");
            }
            else if (o.Action == "Profile_Authenticate_Full")
            {
                BMSCommon.CryptoUtils.User u = await DSQL.UI.GetUser(HttpContext);
                string m = "location.href='/page/profile';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "nft_create")
            {
                string sRedir = await BBPController.btnSubmitNFT_Click(HttpContext, o.FormData, "create");
                return Json(sRedir);
            }
            else if (o.Action == "nft_editme")
            {
                string sRedir = await BBPController.btnSubmitNFT_Click(HttpContext, o.FormData, "edit");
                return Json(sRedir);
            }
            else if (o.Action == "timeline_post")
            {
                string sData = GetFormData(o.FormData, "txtBody");
                string sPaste = GetFormData(o.FormData, "divPaste");
                dynamic oExtra = Newtonsoft.Json.JsonConvert.DeserializeObject(o.ExtraData);
                string sParentID = oExtra.parentid.Value;
                string sError = "";

                if (sParentID == null || sParentID == "")
                    sError = "Parent ID invalid.";

                Timeline t = new Timeline();
                t.Body = sData;
                t.dataPaste = sPaste;
                t.ERC20Address = u0.ERC20Address;
                t.BBPAddress = u0.BBPAddress;
                t.Added = DateTime.Now.ToString();
                t.ParentID = sParentID;
                
                if (sError != "")
                {
                    string s1 = MsgBoxJson(HttpContext, "Timeline Post", "Error", sError);
                    return Json(s1);
                }

                t.Save(IsTestNet(HttpContext));
                // Redirect user to the Timeline to show the post
                string m = "location.reload();";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);

            }
            else if (o.Action == "video_search")
            {
                string sSearch = GetFormData(o.FormData, "txtSearcher");
                string m = "location.href='/bbp/videos?search=" + sSearch + "';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);

            }
            else if (o.Action == "nft_buy")
            {
                dynamic oExtra = Newtonsoft.Json.JsonConvert.DeserializeObject(o.ExtraData);
                string sNFTID = oExtra.nftid.Value;
                double nBuyItNowAmount = oExtra.Amount.Value;
                string sChain = IsTestNet(HttpContext) ? "test" : "main";
                NFT n = await NFT.GetNFT(sChain, sNFTID);
                string sError = "";
                if (n == null)
                {
                    sError = "NFT not found";
                    string s1 = MsgBoxJson(HttpContext, "Error", "Error", "NFT not found.");
                    return Json(s1);
                }
                if (n.OwnerERC20Address == "" || n.OwnerBBPAddress == "" || n.OwnerERC20Address == null)
                {
                    sError += "NFT ERC 20 address is not populated; invalid nft.";
                }

                bool fValid = BMSCommon.WebRPC.ValidateBiblepayAddress(IsTestNet(HttpContext), n.OwnerBBPAddress);
                if (!fValid)
                {
                    sError += "Owner BBP address is not valid. ";
                }

                if (sError != "")
                {
                    string s1 = MsgBoxJson(HttpContext, "Error", "Error", "NFT not found.");
                    return Json(s1);
                }

                double nAmount = nBuyItNowAmount;

                if (nAmount <= 0)
                {
                    sError += "Buy amount must be > 0.";
                }

                if (n.LowestAcceptableAmount() <= 0 || nAmount < n.LowestAcceptableAmount())
                {
                    sError += "Lowest acceptable amount is too low.";
                }
                string sPayload = "<XML>BuyNFT " + nAmount.ToString() + "</XML>";

                DACResult r0 = DSQL.UI.SendBBP(HttpContext, n.OwnerBBPAddress, nAmount, sPayload);
                if (r0.TXID != String.Empty)
                {
                    User uBuyer = HttpContext.GetCurrentUser();
                    User dtSeller = await GetCachedUser(IsTestNet(HttpContext), n.OwnerERC20Address);
                    MailAddress mTo = new MailAddress("rob@biblepay.org","Team BiblePay");
                    MailMessage m = new MailMessage();
                    m.To.Add(mTo);
                    MailAddress mBCC1 = new MailAddress(DecAES(uBuyer.EmailAddress), uBuyer.NickName);
                    string sSellerEmail = DecAES(dtSeller.EmailAddress);
                    string sSellerNickName = dtSeller.NickName ?? String.Empty;
                    if (sSellerEmail == null)
                        sSellerEmail = "Unknown Seller";
                    if (sSellerNickName == String.Empty)
                        sSellerNickName = "Unknown Seller NickName";

                    if (n.Type.ToLower() == "orphan")
                    {
                        m.CC.Add(mBCC1);
                        try
                        {
                            MailAddress mBCC2 = new MailAddress(sSellerEmail, sSellerNickName);
                            m.Bcc.Add(mBCC2);
                        }
                        catch (Exception ex2) { }
                    }
                    else
                    {
                        m.Bcc.Add(mBCC1);
                        try
                        {
                            MailAddress mBCC2 = new MailAddress(sSellerEmail, sSellerNickName);
                            m.Bcc.Add(mBCC2);
                        }
                        catch (Exception ex2) { }
                    }

                    string sSubject = (n.Type.ToLower() == "orphan") ? "Orphan Sponsored " + n.GetHash() : "Bought NFT " + n.GetHash();

                    m.Subject = sSubject;
                    string sPurchaseNarr = (n.Type.ToLower() == "orphan") ? "has been sponsored" : "has been purchased";

                    m.Body = "<br>Dear " + sSellerNickName + ", " + sPurchaseNarr + " by " + uBuyer.NickName + " for " + nAmount.ToString() + ".  <br><br><br><h3>" 
                        + n.Name + "</h3><br><br><div><span>" + n.Description + "</div><br><br><br><img src='" + n.AssetURL 
                        + "' width=400 height=400/><br><br><br>Thank you for using BiblePay!";

                    m.IsBodyHtml = true;
                    BBPTestHarness.Common.SendMail(false, m);

                    // Transfer the actual NFT
                    n.TXID = r0.TXID;
                    n.Action = "buy";
                    n.OwnerERC20Address = u0.ERC20Address;
                    n.OwnerBBPAddress = u0.BBPAddress;
                    n.Marketable = 0;
                    n.time = BMSCommon.Common.UnixTimestamp();
                    await n.Save(IsTestNet(HttpContext));
                    string s2 = MsgBoxJson(HttpContext, "Success", "Success", "You have successfully purchased this NFT on TXID " + r0.TXID + ".  ");
                    return Json(s2);
                }
                else
                {
                    string s3 = MsgBoxJson(HttpContext, "Error", "Error", "Purchase error. ");
                    return Json(s3);
                }
            }
            else if (o.Action == "portfoliobuilder_pinchange")
            {
                string sForeignAddress = GetFormData(o.FormData, "txtAddress");
                string sBBPAddress = u0.BBPAddress;
                double n1 = BlockChair.AddressToPin(sBBPAddress, sForeignAddress);
                string m = "var p = document.getElementById('txtPin');p.value='" + n1.ToString() + "';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "scrapy_paste")
            {
                string sBody = GetFormData(o.FormData, "txtBody");
                string sParsed = String.Empty;
                for (int i = 0; i < 3; i++)
                {
                    sParsed = await DSQL.UI.Scrapper(sBody);
                    if (sParsed != String.Empty)
                        break;
                }

                string m = "var p = document.getElementById('divPaste');p.innerHTML=\"" + sParsed + "\";";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "portfoliobuilder_add")
            {
                UTXOPosition u = new UTXOPosition();
                u.Symbol = GetFormData(o.FormData, "ddSymbol");
                u.ForeignAddress = GetFormData(o.FormData, "txtAddress");
                u.ERC20Address = u0.ERC20Address;
                u.BBPAddress = u0.BBPAddress;
                u.Added = DateTime.Now.ToString();
                string sError = String.Empty;
                bool fValid = BMSCommon.BlockChair.ValidateForeignAddress(u.Symbol, u.ForeignAddress);
                if (!fValid)
                    sError = "Foreign Address invalid.";

                fValid = BMSCommon.BlockChair.ValidateTicker(u.Symbol);
                if (!fValid)
                    sError = "Invalid ticker symbol.";

                if (sError != String.Empty)
                {
                    string s1 = MsgBoxJson(HttpContext, "Portfolio Builder - Add", "Error", sError);
                    return Json(s1);
                }

                u.Save(IsTestNet(HttpContext));
                string narr = "Your position has been added.  Thank you for using biblepay. ";
                string s2 = MsgBoxJson(HttpContext, "Portfolio Builder - Add", "Success", narr + "<br><br>");
                return Json(s2);
            }
            else if (o.Action == "turnkey_fund")
            {
                dynamic a = Newtonsoft.Json.JsonConvert.DeserializeObject(o.ExtraData);
                string sAddress = a.address.Value;
                string sNarr = "To fund this sanctuary simply send 4,500,001 BBP to the address " + sAddress + ".";
                string modal = DSQL.UI.GetModalDialog("Funding a Sanctuary", sNarr);
                returnVal.returnbody = modal;
                returnVal.returntype = "modal";
                string outdata = JsonConvert.SerializeObject(returnVal);
                return Json(outdata);
            }
            else if (o.Action=="quant_subscribe")
            {
                dynamic a = Newtonsoft.Json.JsonConvert.DeserializeObject(o.ExtraData);
                string sProdID = a.strategyid.Value;
                string sResult = QuantController.SubscribeToProduct(sProdID, this.HttpContext);
                if (sResult == String.Empty)
                {
                    DSQL.UI.MsgBox(HttpContext, "Subscribed", "Subscribed", 
                        "Thank you for subscribing to this quant strategy. "
                        +"Your account will automatically be debited on the first of each month for the monthly "
                        +"subscription fee denominated in BBP.  You may cancel at any time by visiting My Subscriptions. <br><br>As long as your account is in good standing, you will receive a weekly "
                        +"Signal e-mail containing the analysis service Signal Output for hypothetical trades that this strategy would execute if "
                        +"the computer were following these rules for a hypothetical process in an investment fund.  <br><br>By using this service you agree that all investment signals are for informational purposes only, "
                        +" and do not constitute trading advice.  It is at your sole discretion to fully evaluate each possible trade and make a SELF DIRECTED DECISION.   "
                        +" By using this service you agree to take responsibility for your own actions, and you hereby hold BiblePay and our Quant division harmless "
                        +" from all harm that may arise by acting on your Self Directed actions in your personal trading account.  <br><br>PAST PERFORMANCE IS NOT A GUARANTEE OF FUTURE RESULTS.\r\n", false);
                }
                else
                {
                    DSQL.UI.MsgBox(HttpContext, "Error", "Error", sResult, false);
                }

                string m = "location.href='bbp/messagepage';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);

            }
            else if (o.Action == "chat_select")
            {
                dynamic a = Newtonsoft.Json.JsonConvert.DeserializeObject(o.ExtraData);
                string sUID = a.id.Value;
                if (sUID == u0.ERC20Address)
                {
                    string s1 = MsgBoxJson(HttpContext, "Chat Error", "Error", "You cannot chat with yourself.");
                    return Json(s1);
                }
                HttpContext.Session.SetString("CHATTING_WITH", sUID);
                string m = "location.href='/bbp/chat';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "Chat_Poll")
            {
                string sMsgs = await BBPController.GetChatMessages(HttpContext);
                string m = "var p = document.getElementById('chat_container');"
                    + "if (p.innerHTML != `" + sMsgs + "`) { p.innerHTML=`" + sMsgs + "`;p.scrollTop = p.scrollHeight; } setTimeout(`DoCallback('Chat_Poll')`,5000);";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "Chat_Send")
            {
                if (!u0.LoggedIn)
                {
                    string s1 = MsgBoxJson(HttpContext, "Chat Error", "Error", "You must be logged in to chat.");
                    return Json(s1);
                }

                string sSend = GetFormData(o.FormData, "msgr_input");
                try
                {
                    if (sSend != String.Empty)
                    {
                        DSQL.UI.ChatItem ci = new ChatItem();
                        ci.From = u0.ERC20Address;
                        string sToUID = HttpContext.Session.GetString("CHATTING_WITH");
                        if (sToUID != null)
                        {
                            ci.To = sToUID;
                            ci.body = sSend;
                            ci.time = DateTime.Now;
                            DSQL.UI.AddChatItem(IsTestNet(HttpContext), ci, true);
                        }
                        else
                        {
                            string s1 = MsgBoxJson(HttpContext, "Chat Error", "Error", "You must choose someone to chat with first.");
                            return Json(s1);
                        }
                    }
                    string sMsgs = await BBPController.GetChatMessages(HttpContext);
                    string m = "var b=document.getElementById('msgr_input');b.value='';var p = document.getElementById('chat_container');"
                        +"p.innerHTML=`" + sMsgs + "`;p.scrollTop = p.scrollHeight;";
                    returnVal.returnbody = m;
                    returnVal.returntype = "javascript";
                    string o1 = JsonConvert.SerializeObject(returnVal);
                    return Json(o1);
                }
                catch(Exception ex)
                {
                    return Json(ex.Message);
                }

            }
            else if (o.Action == "admin_addexpense")
            {
                // Add revenue record, add expense record, and add OrphanExpense distribution
                List<SponsoredOrphan2> dt = await StorjIO.GetDatabaseObjects<SponsoredOrphan2>("sponsoredorphan");
                string sCharity = GetFormData(o.FormData, "txtCharityName").ToUpper();
                dt = dt.Where(s => s.Charity.ToLower() == sCharity.ToLower()).ToList();
                string sNotes = GetFormData(o.FormData, "txtNotes");
                string sAdded = GetFormData(o.FormData, "txtAdded");
                sAdded = Convert.ToDateTime(sAdded).ToString();
                dynamic a = Newtonsoft.Json.JsonConvert.DeserializeObject(o.ExtraData);
                string sOpType= a.OpType.Value;
                string sError = String.Empty;

                if (sOpType != "PAYMENT" && sOpType != "CHARGE")
                    sError = "Invalid Operation Type";

                if (sCharity != "CAMEROON-ONE" && sCharity != "SAI" && sCharity != "KAIROS")
                    sError = "Invalid Charity Name";
                if (sAdded == String.Empty)
                    sError = "Added date must be populated.";

                double nExpenseTotal = BMSCommon.Common.GetDouble(GetFormData(o.FormData, "txtAmount"));

                if (nExpenseTotal < 1)
                    sError = "Invalid expense amount.  It should be above zero.";
                if (sNotes == String.Empty)
                {
                    sError = "Sorry, notes must be populated.";
                }

                if (sError != String.Empty)
                {
                    string s3 = MsgBoxJson(HttpContext, "Error", "Error", sError);
                    return Json(s3);
                }
                if (dt.Count == 0)
                    sError = "No rows.";

                if (sError != "")
                {
                    string s3 = MsgBoxJson(HttpContext, "Error", "Error", sError);
                    return Json(s3);
                }

                int nModifier = 0;
                if (sOpType == "PAYMENT")
                {
                    nModifier = -1;
                }
                else
                {
                    nModifier = 1;
                }
                double nAmount = nModifier * (nExpenseTotal / dt.Count);
                

                for (int i = 0; i < dt.Count; i++)
                {
                    OrphanExpense3 oe = new OrphanExpense3();
                    oe.Added = sAdded;
                    oe.Amount = nAmount;
                    oe.Charity = sCharity;
                    oe.URL = String.Empty;
                    oe.HandledBy = "bible_pay";
                    oe.ChildID = dt[i].ChildID;
                    double nOldBalance = await BMSCommon.BitcoinSync.GetChildBalance(oe.ChildID);
                    oe.Balance = nOldBalance += nAmount;
                    oe.Notes = sNotes;
                    oe.id = Guid.NewGuid().ToString();
                    //BMSCommon.CryptoUtils.Transaction t1 = new BMSCommon.CryptoUtils.Transaction();
                   // t1.Time = BMSCommon.Common.UnixTimestamp();
                    string sData = Newtonsoft.Json.JsonConvert.SerializeObject(oe);
                    await StorjIO.UplinkStoreDatabaseData("orphanexpense", oe.id, sData, String.Empty);
                }

                if (sOpType == "PAYMENT")
                {
                    // Add the orphan expense record
                    Expense x = new Expense();
                    x.Added = sAdded;
                    x.Amount = nExpenseTotal;
                    x.URL = "";
                    x.Charity = sCharity;
                    x.HandledBy = "bible_pay";
                    x.Notes = sNotes;
                    x.id = Guid.NewGuid().ToString();
                    string sData = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                    await StorjIO.UplinkStoreDatabaseData("expense", x.id, sData, String.Empty);
                    // Add the Revenue Record
                    Revenue r = new Revenue();
                    r.Added = sAdded;
                    r.BBPAmount = 0;
                    r.BTCRaised = 0;
                    r.Amount = nExpenseTotal;
                    r.Notes = "[Donation for ] " + sNotes;
                    r.Charity = sCharity;
                    r.HandledBy = "bible_pay";
                    r.id = Guid.NewGuid().ToString();
                    sData = Newtonsoft.Json.JsonConvert.SerializeObject(r);
                    await StorjIO.UplinkStoreDatabaseData("revenue", r.id, sData, String.Empty);
                }
                string s4 = MsgBoxJson(HttpContext, "Success", "Success", "Success");
                return Json(s4);
            }
            else if (o.Action == "profile_logout")
            {
                string m = "setCookie('erc20signature', '', 30);setCookie('erc20address','',30);location.href='/gospel/about';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "nft_edit")
            {
                dynamic a = Newtonsoft.Json.JsonConvert.DeserializeObject(o.ExtraData);
                string sID = a.nftid.Value;
                string m = "location.href='/bbp/nftadd?mode=edit&id=" + sID + "';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "turnkey_backup")
            {
                List<TurnkeySanc> d = await StorjIO.GetDatabaseObjects<TurnkeySanc>("turnkeysanctuaries");
                d = d.Where(s => s.erc20address == u0.ERC20Address).ToList();
                string sData = "<ul>Store this in a safe place.";
                for (int i = 0; i < d.Count;i++)
                {
                    string sNonce = DecAES(d[i].Nonce.ToString());
                    BMSCommon.Encryption.KeyType k = DSQL.UI.GetKeyPair(HttpContext, sNonce);
                    string sRow = "Sanctuary " + sNonce + " has a pubkey=" + k.PubKey + ",privkey=" + k.PrivKey + "\r\n";
                    sData += "<li>" + sRow;
                }

                if (d.Count < 1)
                    sData = "Unable to find any sanctuaries to back up.";
                
               string d2 = MsgBoxJson(HttpContext, "Back Up Sanctuary Credentials", "Information", sData);
                    return Json(d2);

            }
            else if (o.Action == "turnkey_liquidate")
            {
                string sError = String.Empty;
                dynamic a = Newtonsoft.Json.JsonConvert.DeserializeObject(o.ExtraData);
                string sFromAddress = a.address.Value;
                bool fValid = BMSCommon.WebRPC.ValidateAddress(IsTestNet(HttpContext), sFromAddress);
                if (!fValid)
                    sError = "Invalid From Address.";
                double nBalance = QueryAddressBalance(IsTestNet(HttpContext), sFromAddress);
                if (nBalance < .25)
                    sError = "Your balance is too low to liquidate.";
                if (nBalance < 5.01)
                    sError = "Your balance is too low to liquidate when including the transaction fee.";

                List<TurnkeySanc> d = await StorjIO.GetDatabaseObjects<TurnkeySanc>("turnkeysanctuaries");
                d = d.Where(s => s.BBPAddress == sFromAddress).ToList();

                
                if (d.Count < 1)
                    sError = "Unable to find turnkey nonce.";

                if (sError != "")
                {
                    string d2 = MsgBoxJson(HttpContext, "Turnkey Liquidation", "Error", sError);
                    return Json(d2);
                }

                string sNonce = DecAES(d[0].Nonce);
                if (sNonce == "")
                    sError = "Unable to find the nonce.";
                string sToAddress = u0.BBPAddress;
                if (sToAddress == "" || !u0.LoggedIn)
                    sError = "Sorry, you must be logged in and have a BBP address set up.";

                if (sError != "")
                {
                    string d2 = MsgBoxJson(HttpContext, "Turnkey Liquidation", "Error", sError);
                    return Json(d2);
                }
                // Lets liquidate
                string sPayload = "<XML>Liquidate Sanc</XML>";
                
                DACResult r0 = DSQL.UI.SendBBP(HttpContext, sToAddress, nBalance-5, sPayload, sNonce);

                string sResult = String.Empty;
                if (r0.TXID != String.Empty)
                {
                    sResult = "Sent " + (nBalance-5).ToString() + " to " + sToAddress + " on TXID " + r0.TXID;
                    DSQL.UI.GetAvatarBalance(HttpContext, true);
                }
                else
                {
                    sResult = "Failed.  [" + r0.Error + "]";
                }
                string d1 = MsgBoxJson(HttpContext, "Turnkey Liquidation", "Success", sResult);
                return Json(d1);

            }
            else if (o.Action == "Profile_RefreshBalance")
            {
                DSQL.UI.GetAvatarBalance(HttpContext, true);
                string m = "location.href='page/profile';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "Profile_SendBBP")
            {
                string sToAddress = GetFormData(o.FormData, "txtSendToAddress");
                double nAmount = BMSCommon.Common.GetDouble(GetFormData(o.FormData, "txtAmountToSend"));
                string sPayload = "<XML>Send_BBP</XML>";
                DACResult r0 = DSQL.UI.SendBBP(HttpContext, sToAddress, nAmount, sPayload);
                string sResult = String.Empty;
                if (r0.TXID != String.Empty)
                {
                    sResult = "Sent " + nAmount.ToString() + " to " + sToAddress + " on TXID " + r0.TXID;
                    DSQL.UI.GetAvatarBalance(HttpContext, true);
                }
                else
                {
                    sResult = "Failed.  [" + r0.Error + "]";
                }

                string modal = DSQL.UI.GetModalDialog("Send BBP", sResult);
                returnVal.returnbody = modal;
                returnVal.returntype = "modal";
                string outdata = JsonConvert.SerializeObject(returnVal);
                return Json(outdata);
            }
            else if (o.Action == "Univ_Grade")
            {
                string sResults = BiblePay.BMS.Controllers.UnivFinalExamController.ShowResults(HttpContext);
                string modal = DSQL.UI.GetModalDialog("Final Exam Results", sResults);
                returnVal.returnbody = modal;
                returnVal.returntype = "modal";
                string outdata = JsonConvert.SerializeObject(returnVal);
                return Json(outdata);
            }
            else if (o.Action == "MemorizeScriptures_Grade")
            {
                string s = MemorizeScripturesController.btnGrade_Click(HttpContext, o.FormData);
                string modal = DSQL.UI.GetModalDialog("Results", s);
                returnVal.returnbody = modal;
                returnVal.returntype = "modal";
                string outdata = JsonConvert.SerializeObject(returnVal);
                return Json(outdata);
            }
            else if (o.Action == "scratch_submit")
            {
                string sScratchID = GetFormData(o.FormData, "txtScratchID");
                string sScratchValue = GetFormData(o.FormData, "txtScratchValue");
                string sError = String.Empty;
                if (sScratchID == String.Empty || sScratchValue == String.Empty)
                {
                    sError = "Values must be populated";
                }
                else
                {
                    BMSCommon.Database.SetKeyValue("scratch_" + sScratchID, sScratchValue);
                }
                string sNarr = sError != String.Empty ? sError : "Successfully saved.  Your data will be saved for 10 minutes and then erased.  "
                    + "NOTE: Once you access the data, we will rewrite the value with 'accessed' so that it cannot be accessed again (it can only be accessed once).";
                string modal = DSQL.UI.GetModalDialog("Results", sNarr);
                returnVal.returnbody = modal;
                returnVal.returntype = "modal";
                string outdata = JsonConvert.SerializeObject(returnVal);
                return Json(outdata);
            }
            else if (o.Action == "turnkey_createsanctuary")
            {
                // Mission Critical

                string sError = String.Empty;
                if (!u0.LoggedIn)
                    sError = "Sorry, you must be logged in first.";

                List<TurnkeySanc> d = await StorjIO.GetDatabaseObjects<TurnkeySanc>("turnkeysanctuaries");
                d = d.Where(s => s.erc20address == u0.ERC20Address).ToList();

//                string sql1 = "Select count(*) c from " + sTable + " where ERC20Address=@erc20address;";
  //              Command m2 = new Command(sql1);
    //            m2.Parameters.AddWithValue("@erc20address", u0.ERC20Address);
//                double nCt = BMSCommon.Database.GetScalarDouble(m2, "c");
                if (d.Count > 9)
                {
                    // mission critical todo : check the locked value here, if they actually have 4.5MM locked per address, dont throw this error.
                    sError = "Sorry, you already have 10 sanctuaries.";
                }

                if (u0.EmailAddress == String.Empty || u0.EmailAddress == null)
                {
                    sError += "<br>Sorry, your e-mail address must be populated first.";
                }

                bool fPrimary = BMSCommon.Common.IsPrimary();

                if (!fPrimary && !IsTestNet(HttpContext))
                    sError += "Sorry, this feature is not available on this sanctaury. ";

                if (sError != String.Empty)
                {
                    string d11 = MsgBoxJson(HttpContext, "Turnkey Provision", "Error", sError);
                    return Json(d11);
                }

                // Get Nonce  and  Determine BBP Address
                TurnkeySanc t = new TurnkeySanc();
                t.erc20address = u0.ERC20Address;
                string sNonce = Guid.NewGuid().ToString();

                KeyType k = DSQL.UI.GetKeyPair(HttpContext, sNonce);
                t.Nonce = EncAES(sNonce);
                t.BBPAddress = k.PubKey;
                string ERC20Signature;
                string ERC20Address;
                HttpContext.Request.Cookies.TryGetValue("erc20signature", out ERC20Signature);
                HttpContext.Request.Cookies.TryGetValue("erc20address", out ERC20Address);
                t.Signature = ERC20Signature;
                t.id = Guid.NewGuid().ToString();
                string sData = Newtonsoft.Json.JsonConvert.SerializeObject(t);
                bool fIns = await StorjIO.UplinkStoreDatabaseData("turnkeysanctuaries", t.id, sData, String.Empty);

                if (!fIns)
                {
                    string d12 = MsgBoxJson(HttpContext, "Turnkey Sanctuary-Create", "Error", "Sorry, we were unable to provision your sanctuary.  Please e-mail contact@biblepay.org with the Config information and we will manually deal with this or fix the issue.  Thank you for using BiblePay.  ");
                    return Json(d12);
                }

                string sNarr = "Congratulations.  Your sanctuary provisioning request has been received.  <br><br>"
                    + "You will automatically see your daily rewards increase in the <a href='bbp/turnkeysanctuaries'>Turnkey Sanctuaries Report - Click here.</a>"
                    +"<br><br>Thank you for using BiblePay!";

                string d1 = MsgBoxJson(HttpContext, "Turnkey Provisioner", "Success", sNarr);

                MailMessage mm = new MailMessage();
                MailAddress mTo = new MailAddress("rob@biblepay.org", "Rob Andrews");
                mm.To.Add(mTo);
                mm.Subject = "New Sanctuary Provisioning Request";
                mm.Body = "Dear Team BiblePay, \r\nWe have received a new turnkey sanctuary provisioning request.  \r\n";
                mm.Body += " for " + u0.NickName + "\r\n\r\n Sincerely Yours, \r\nBiblePay BMS";
                mm.IsBodyHtml = false;
                // DACResult dr1 = BMSCommon.API.SendMail(false, mm);
                // ToDo: move this to the Pay area where we detect that we have a new 4.5MM sanc, notify via email.
                // Redirect the user to turnkey sancs
                string m = "location.href='bbp/turnkeysanctuaries';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "proposal_add")
            {
                string sRedir = await BBPController.btnSubmitProposal_Click(HttpContext, o.FormData);
                return Json(sRedir);
            }
            else if (o.Action == "show_modal")
            {
                //Extra Data
                dynamic msg = Newtonsoft.Json.JsonConvert.DeserializeObject(o.ExtraData);
                string modal = DSQL.UI.GetModalDialog("Information", msg.Narr.Value);
                returnVal.returnbody = modal;
                returnVal.returntype = "modal";
                string outdata = JsonConvert.SerializeObject(returnVal);
                return Json(outdata);
            }
            else if (o.Action == "MemorizeScriptures_Switch")
            {
                MemorizeScripturesController.btnSwitchMode_Click(HttpContext);
                string m = "location.href='memorizescriptures/memorizescriptures';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "MemorizeScriptures_Next")
            {
                MemorizeScripturesController.btnNextScripture_Click(HttpContext,o.FormData);
                string m = "location.href='memorizescriptures/memorizescriptures';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "test_1")
            {
                // MessagePage
                DSQL.UI.MsgBox(HttpContext, "BBP Long Process", "Error occurred", "object not set to an object.", false);
                string m = "location.href='bbp/messagepage';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "Univ_Next")
            {
                string test = Request.Query["test"];
                UnivStoreAnswer(HttpContext, o.FormData);
                UnivFinalExamController.btnNext_Click(HttpContext);
                string m = "location.href='univfinalexam/univfinalexam';"; 
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "Univ_Back")
            {
                string test = Request.Query["test"];
                UnivStoreAnswer(HttpContext, o.FormData);
                UnivFinalExamController.btnBack_Click(HttpContext);
                string m = "location.href='univfinalexam/univfinalexam';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "Univ_Switch")
            {
                string test = Request.Query["test"];
                UnivFinalExamController.btnSwitch_Click(HttpContext);
                string m = "location.href='univfinalexam/univfinalexam';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "Profile_ChangeChain")
            {
                if (DSQL.UI.GetChain(HttpContext) == "MAINNET")
                {
                    HttpContext.Session.SetString("Chain", "TESTNET");
                }
                else
                {
                    HttpContext.Session.SetString("Chain", "MAINNET");
                }
                string sNewChain = DSQL.UI.GetChain(HttpContext);
                string m = "location.href='page/profile';"; // DSQL.UI.GetModalDialog("Switch Block Chain", "Chain has been switched to " + sNewChain, "location.href='page/profile';");
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
           
            return Json(String.Empty);
        }

        public IActionResult AnalyticsDashboard() => View();
        public IActionResult Introduction() => View();
        public IActionResult Privacy() => View();
    }
}
