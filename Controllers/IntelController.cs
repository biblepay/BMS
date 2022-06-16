using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using BMSCommon;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using static BiblePay.BMS.Controllers.BBPController;
using static BiblePay.BMS.DSQL.UI;
using static BMSCommon.Encryption;
using static BMSCommon.WebRPC;

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
            ViewBag.Notifications = DSQL.UI.GetNotifications();
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

        [HttpPost]
        public JsonResult ProcessDoCallback([FromBody] ClientToServer o)
        {
            ServerToClient returnVal = new ServerToClient();

            if (o.Action == "Profile_Save")
            {
                BMSCommon.CryptoUtils.User u = DSQL.UI.GetUser(HttpContext);
                bool fTestNet = DSQL.UI.IsTestNet(HttpContext);

                u.EmailAddress = GetFormData(o.FormData, "txtEmailAddress");
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
                bool f = BMSCommon.CryptoUtils.PersistUser(IsTestNet(HttpContext), u);
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
                BMSCommon.CryptoUtils.User u = DSQL.UI.GetUser(HttpContext);

                return Json("");
            }
            else if (o.Action == "Profile_Authenticate_Full")
            {
                BMSCommon.CryptoUtils.User u = DSQL.UI.GetUser(HttpContext);
                string m = "location.href='/page/profile';";
                returnVal.returnbody = m;
                returnVal.returntype = "javascript";
                string o1 = JsonConvert.SerializeObject(returnVal);
                return Json(o1);
            }
            else if (o.Action == "nft_create")
            {
                string sRedir = BBPController.btnSubmitNFT_Click(HttpContext, o.FormData, "create");
                return Json(sRedir);
            }
            else if (o.Action == "nft_editme")
            {
                string sRedir = BBPController.btnSubmitNFT_Click(HttpContext, o.FormData, "edit");
                return Json(sRedir);
            }
            else if (o.Action == "timeline_post")
            {
                string sData = GetFormData(o.FormData, "txtPost");
                Timeline t = new Timeline();
                t.Body = sData;
                t.ERC20Address = GetUser(HttpContext).ERC20Address;
                t.BBPAddress = GetUser(HttpContext).BBPAddress;
                t.Added = DateTime.Now.ToString();
                string sError = "";
             
                if (sError != "")
                {
                    string s1 = MsgBoxJson(HttpContext, "Portfolio Builder - Add", "Error", sError);
                    return Json(s1);
                }

                t.Save(IsTestNet(HttpContext));
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
                NFT n = BMSCommon.NFT.GetNFT(sChain, sNFTID);
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

                BMSCommon.WebRPC.DACResult r0 = DSQL.UI.SendBBP(HttpContext, n.OwnerBBPAddress, nAmount, sPayload);
                if (r0.TXID != "")
                {
                    // Transfer the actual NFT
                    n.TXID = r0.TXID;
                    n.Action = "buy";
                    n.OwnerERC20Address = GetUser(HttpContext).ERC20Address;
                    n.OwnerBBPAddress = GetUser(HttpContext).BBPAddress;
                    n.Marketable = 0;
                    n.time = BMSCommon.Common.UnixTimestamp();
                    n.Save(IsTestNet(HttpContext));
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
                string sBBPAddress = GetUser(HttpContext).BBPAddress;
                double n1 = BlockChair.AddressToPin(sBBPAddress, sForeignAddress);
                string m = "var p = document.getElementById('txtPin');p.value='" + n1.ToString() + "';";
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
                u.ERC20Address = GetUser(HttpContext).ERC20Address;
                u.BBPAddress = GetUser(HttpContext).BBPAddress;
                u.Added = DateTime.Now.ToString();
                string sError = "";
                bool fValid = BMSCommon.BlockChair.ValidateForeignAddress(u.Symbol, u.ForeignAddress);
                if (!fValid)
                    sError = "Foreign Address invalid.";

                if (sError != "")
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
            else if (o.Action == "admin_addexpense")
            {
                // Add revenue record, add expense record, and add OrphanExpense distribution
                string sql = "select * from SponsoredOrphan2 where Charity=@Charity;";
                string sCharityName = GetFormData(o.FormData, "txtCharityName").ToUpper();
                string sNotes = GetFormData(o.FormData, "txtNotes");
                string sAdded = GetFormData(o.FormData, "txtAdded");
                sAdded = Convert.ToDateTime(sAdded).ToString();




                dynamic a = Newtonsoft.Json.JsonConvert.DeserializeObject(o.ExtraData);
                string sOpType= a.OpType.Value;
                string sError = "";

                if (sOpType != "PAYMENT" && sOpType != "CHARGE")
                    sError = "Invalid Operation Type";


                if (sCharityName != "CAMEROON-ONE" && sCharityName != "SAI" && sCharityName != "KAIROS")
                    sError = "Invalid Charity Name";
                if (sAdded == "")
                    sError = "Added date must be populated.";

                double nExpenseTotal = BMSCommon.Common.GetDouble(GetFormData(o.FormData, "txtAmount"));

                if (nExpenseTotal < 1)
                    sError = "Invalid expense amount.  It should be above zero.";
                if (sNotes == "")
                {
                    sError = "Sorry, notes must be populated.";
                }

                if (sError != "")
                {
                    string s3 = MsgBoxJson(HttpContext, "Error", "Error", sError);
                    return Json(s3);
                }

                MySqlCommand m1 = new MySqlCommand(sql);
                m1.Parameters.AddWithValue("@Charity", sCharityName);
                DataTable dt = BMSCommon.Database.GetDataTable(m1);
                if (dt.Rows.Count == 0)
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
                double nAmount = nModifier * (nExpenseTotal / dt.Rows.Count);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BMSCommon.BitcoinSync.OrphanExpense3 oe = new BMSCommon.BitcoinSync.OrphanExpense3();
                    oe.Added = sAdded;
                    oe.Amount = nAmount;
                    oe.Charity = sCharityName;
                    oe.URL = "";
                    oe.HandledBy = "bible_pay";
                    oe.ChildID = dt.Rows[i]["ChildID"].ToString();
                    double nOldBalance = BMSCommon.BitcoinSync.GetChildBalance(oe.ChildID);
                    oe.Balance = nOldBalance += nAmount;
                    oe.Notes = sNotes;
                    BMSCommon.CryptoUtils.Transaction t1 = new BMSCommon.CryptoUtils.Transaction();
                    t1.Time = BMSCommon.Common.UnixTimestamp();
                    t1.Data = Newtonsoft.Json.JsonConvert.SerializeObject(oe);
                    BMSCommon.BitcoinSync.AddToMemoryPool2(false, t1);
                }

                if (sOpType == "PAYMENT")
                {
                    // Add the orphan expense record
                    BMSCommon.BitcoinSync.Expense x = new BMSCommon.BitcoinSync.Expense();
                    x.Added = sAdded;
                    x.Amount = nExpenseTotal;
                    x.URL = "";
                    x.Charity = sCharityName;
                    x.HandledBy = "bible_pay";
                    x.Notes = sNotes;
                    BMSCommon.CryptoUtils.Transaction t = new BMSCommon.CryptoUtils.Transaction();
                    t.Time = BMSCommon.Common.UnixTimestamp();
                    t.Data = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                    BMSCommon.BitcoinSync.AddToMemoryPool2(false, t);
                    // Add the Revenue Record
                    BMSCommon.BitcoinSync.Revenue r = new BMSCommon.BitcoinSync.Revenue();
                    r.Added = sAdded;
                    r.BBPAmount = 0;
                    r.BTCRaised = 0;
                    r.Amount = nExpenseTotal;
                    r.Notes = "[Donation for ] " + sNotes;
                    r.Charity = sCharityName;
                    r.HandledBy = "bible_pay";
                    BMSCommon.CryptoUtils.Transaction t3 = new BMSCommon.CryptoUtils.Transaction();
                    t3.Time = BMSCommon.Common.UnixTimestamp();
                    t3.Data = Newtonsoft.Json.JsonConvert.SerializeObject(r);
                    BMSCommon.BitcoinSync.AddToMemoryPool2(false, t3);
                }
                 
                string s4 = MsgBoxJson(HttpContext, "Success", "Success", "Success");
                return Json(s4);
            }
            else if (o.Action == "portfoliobuilder_donation")
            {
                double nAmount = BMSCommon.Common.GetDouble(GetFormData(o.FormData, "txtDonationAmount"));
                string sPAKey = IsTestNet(HttpContext) ? "tPoolAddress" : "PoolAddress";
                string sDonationAddress = BMSCommon.Common.GetConfigurationKeyValue(sPAKey);
                string sError = "";
                if (sDonationAddress == "")
                    sError = "Sorry, this page is disabled.";
                if (nAmount <= 0)
                {
                    sError += "Buy amount must be > 0.";
                }
                string sPayload = "<XML>Donation</XML>";
                if (sError != "")
                {
                    string s3 = MsgBoxJson(HttpContext, "Error", "Error", "Donation error: " + sError);
                    return Json(s3);
                }
                BMSCommon.WebRPC.DACResult r0 = DSQL.UI.SendBBP(HttpContext, sDonationAddress, nAmount, sPayload);
                if (r0.TXID != "")
                {
                    string sTable = IsTestNet(HttpContext) ? "tpbdonation" : "pbdonation";

                    string sql = "Insert into " + sTable + " (id,txid,amount,added) values (uuid(),@txid,@amount,now());";
                    MySqlCommand cmd1 = new MySqlCommand(sql);
                    cmd1.Parameters.AddWithValue("@txid", r0.TXID);
                    cmd1.Parameters.AddWithValue("@amount", nAmount);
                    bool f = Database.ExecuteNonQuery(false, cmd1, "");
                    string s3 = MsgBoxJson(HttpContext, "Donation", "Success", "Thank you for your donation; your help is greatly appreciated in adding users to our project and ultimately helping more orphans. "
                        + "<br><br>Your receipt TXID: " + r0.TXID + "<br><br> Thank you for using BIBLEPAY.  ");
                    return Json(s3);
                }
                else
                {
                    string s3 = MsgBoxJson(HttpContext, "Error", "Error", "Donation error: Unable to send.");
                    return Json(s3);
                }
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
                string sTable = IsTestNet(HttpContext) ? "tturnkeysanctuaries" : "turnkeysanctuaries";
                string sql = "Select * from " + sTable + " where erc20address=@erc20;";
                MySqlCommand m1 = new MySqlCommand(sql);
                m1.Parameters.AddWithValue("@erc20", GetUser(HttpContext).ERC20Address);
                DataTable d = BMSCommon.Database.GetDataTable(m1);
                string sData = "<ul>Store this in a safe place.";
                for (int i = 0; i < d.Rows.Count;i++)
                {
                    string sNonce = d.Rows[0]["nonce"].ToString();
                    BMSCommon.Encryption.KeyType k = DSQL.UI.GetKeyPair(HttpContext, sNonce);
                    string sRow = "Sanctuary " + sNonce + " has a pubkey=" + k.PubKey + ",privkey=" + k.PrivKey + "\r\n";
                    sData += "<li>" + sRow;
                }

                if (d.Rows.Count < 1)
                    sData = "Unable to find any sanctuaries to back up.";
                
               string d2 = MsgBoxJson(HttpContext, "Back Up Sanctuary Credentials", "Information", sData);
                    return Json(d2);

            }
            else if (o.Action == "turnkey_liquidate")
            {
                string sError = "";
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

                string sTable = IsTestNet(HttpContext) ? "tturnkeysanctuaries" : "turnkeysanctuaries";
                string sql = "Select * from " + sTable + " where bbpaddress=@bbpaddress;";
                MySqlCommand m1 = new MySqlCommand(sql);
                m1.Parameters.AddWithValue("@bbpaddress", sFromAddress);
                DataTable d = BMSCommon.Database.GetDataTable(m1);
                
                if (d.Rows.Count < 1)
                    sError = "Unable to find turnkey nonce.";

                if (sError != "")
                {
                    string d2 = MsgBoxJson(HttpContext, "Turnkey Liquidation", "Error", sError);
                    return Json(d2);
                }

                string sNonce = d.Rows[0]["nonce"].ToString();
                if (sNonce == "")
                    sError = "Unable to find the nonce.";
                string sToAddress = GetUser(HttpContext).BBPAddress;
                if (sToAddress == "" || !GetUser(HttpContext).LoggedIn)
                    sError = "Sorry, you must be logged in and have a BBP address set up.";

                if (sError != "")
                {
                    string d2 = MsgBoxJson(HttpContext, "Turnkey Liquidation", "Error", sError);
                    return Json(d2);
                }
                // Lets liquidate
                string sPayload = "<XML>Liquidate Sanc</XML>";
                
                BMSCommon.WebRPC.DACResult r0 = DSQL.UI.SendBBP(HttpContext, sToAddress, nBalance-5, sPayload, sNonce);

                string sResult = "";
                if (r0.TXID != "")
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
            else if (o.Action == "Profile_SendBBP")
            {
                string sToAddress = GetFormData(o.FormData, "txtSendToAddress");
                double nAmount = BMSCommon.Common.GetDouble(GetFormData(o.FormData, "txtAmountToSend"));
                string sPayload = "<XML>Send_BBP</XML>";
                BMSCommon.WebRPC.DACResult r0 = DSQL.UI.SendBBP(HttpContext, sToAddress, nAmount, sPayload);
                string sResult = "";
                if (r0.TXID != "")
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
                string sError = "";
                if (sScratchID == "" || sScratchValue == "")
                {
                    sError = "Values must be populated";
                }
                else
                {
                    BMSCommon.Pricing.SetKeyValue("scratch_" + sScratchID, sScratchValue);
                }
                string sNarr = sError != "" ? sError : "Successfully saved.  Your data will be saved for 10 minutes and then erased.  "
                    + "NOTE: Once you access the data, we will rewrite the value with 'accessed' so that it cannot be accessed again (it can only be accessed once).";
                string modal = DSQL.UI.GetModalDialog("Results", sNarr);
                returnVal.returnbody = modal;
                returnVal.returntype = "modal";
                string outdata = JsonConvert.SerializeObject(returnVal);
                return Json(outdata);
            }
            else if (o.Action == "turnkey_createsanctuary")
            {

                string sError = "";
                if (!GetUser(HttpContext).LoggedIn)
                    sError = "Sorry, you must be logged in first.";

                string sTable = IsTestNet(HttpContext) ? "tturnkeysanctuaries" : "turnkeysanctuaries";

                string sql1 = "Select count(*) c from " + sTable + " where ERC20Address=@erc20address;";
                MySqlCommand m2 = new MySqlCommand(sql1);
                m2.Parameters.AddWithValue("@erc20address", GetUser(HttpContext).ERC20Address);
                double nCt = Database.GetScalarDouble(m2, "c");
                if (nCt > 5)
                {
                    // mission critical todo : check the locked value here, if they actually have 4.5MM locked per address, dont throw this error.
                    sError = "Sorry, you already have 5 sanctuaries.";
                }

                if (GetUser(HttpContext).EmailAddress == "" || GetUser(HttpContext).EmailAddress == null)
                {
                    sError += "<br>Sorry, your e-mail address must be populated first.";
                }

                bool fPrimary = BMSCommon.Common.IsPrimary();

                if (!fPrimary && !IsTestNet(HttpContext))
                    sError += "Sorry, this feature is not available on this sanctaury. ";

                if (sError != "")
                {
                    string d = MsgBoxJson(HttpContext, "Turnkey Provision", "Error", sError);
                    return Json(d);
                }

                // Get Nonce  and  Determine BBP Address
                
                string sql = "Insert into " + sTable + " (id,erc20address,nonce,bbpaddress,Added) values (uuid(),@erc20address,@nonce,@bbpaddress,now());";
                MySqlCommand m1 = new MySqlCommand(sql);
                string sNonce = Guid.NewGuid().ToString();
                KeyType k = DSQL.UI.GetKeyPair(HttpContext, sNonce);
                m1.Parameters.AddWithValue("@erc20address", GetUser(HttpContext).ERC20Address);
                m1.Parameters.AddWithValue("@nonce", sNonce);
                m1.Parameters.AddWithValue("@bbpaddress", k.PubKey);

                bool fIns = BMSCommon.Database.ExecuteNonQuery(false, m1, "");
                if (!fIns)
                {
                    string d = MsgBoxJson(HttpContext, "Turnkey Sanctuary-Create", "Error", "Sorry, we were unable to provision your sanctuary.  Please e-mail contact@biblepay.org with the Config information and we will manually deal with this or fix the issue.  Thank you for using BiblePay.  ");
                    return Json(d);
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
                mm.Body += " for " + GetUser(HttpContext).NickName + "\r\n\r\n Sincerely Yours, \r\nBiblePay BMS";
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
                string sRedir = BBPController.btnSubmitProposal_Click(HttpContext, o.FormData);
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
           
            if (false)
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = new DateTimeOffset(DateTime.Now.AddDays(7));
            }
            return Json("");

        }


        public IActionResult AnalyticsDashboard() => View();
        public IActionResult Introduction() => View();
        
        public IActionResult Privacy() => View();
    }
}
