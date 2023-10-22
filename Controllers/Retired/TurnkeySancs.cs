using BBPAPI;
using BBPAPI.Model;
using BMSCommon;
using BMSCommon.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using static BiblePay.BMS.DSQL.SessionHelper;
using static BiblePay.BMS.DSQL.UI;
using static BiblePay.BMS.DSQL.UIWallet;
using static BMSCommon.Common;
using static BMSCommon.Encryption;

namespace BiblePay.BMS.Controllers
{
    public class TurnkeySancController : Controller
    {

        [HttpPost]
        public JsonResult ProcessDoCallback([FromBody] ClientToServer o)
        {
            ServerToClient returnVal = new ServerToClient();
            User u0 = GetUser(HttpContext);

            if (o.Action == "turnkey_fund")
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
            else if (o.Action == "turnkey_backup")
            {
                /*
                List<TurnkeySanc> d = DB.GetDatabaseObjectsAsAdmin<TurnkeySanc>("turnkeysanc");
                d = d.Where(s => s.erc20address == u0.ERC20Address).ToList();
                string sData = "<ul>Store this in a safe place.";
                for (int i = 0; i < d.Count; i++)
                {
                    string sNonce = BBPAPI.ERCUtilities.GetNonce(d[i]);
                    Encryption.KeyType k = GetKeyPair(HttpContext, sNonce);
                    string sRow = "Sanctuary " + sNonce + " has a pubkey=" + k.PubKey + ",privkey=" + k.PrivKey + "\r\n";
                    sData += "<li>" + sRow;
                }

                if (d.Count < 1)
                    sData = "Unable to find any sanctuaries to back up.";

                string d2 = MsgBoxJson(HttpContext, "Back Up Sanctuary Credentials", "Information", sData);
                return Json(d2);
                */
                throw new Exception("NI");

            }
            else if (o.Action == "turnkey_liquidate")
            {
                /*
                string sError = String.Empty;
                dynamic a = Newtonsoft.Json.JsonConvert.DeserializeObject(o.ExtraData);
                string sFromAddress = a.address.Value;
                bool fValid = BBPAPI.Sanctuary.ValidateBiblePayAddress(IsTestNet(HttpContext), sFromAddress);
                if (!fValid)
                    sError = "Invalid From Address.";
                double nBalance = ERCUtilities.QueryAddressBalance(IsTestNet(HttpContext), sFromAddress);
                if (nBalance < .25)
                    sError = "Your balance is too low to liquidate.";
                if (nBalance < 5.01)
                    sError = "Your balance is too low to liquidate when including the transaction fee.";

                List<TurnkeySanc> d = DB.GetDatabaseObjectsAsAdmin<TurnkeySanc>("turnkeysanc");
                d = d.Where(s => s.BBPAddress == sFromAddress).ToList();
                if (d.Count < 1)
                    sError = "Unable to find turnkey nonce.";
                if (sError != "")
                {
                    string d2 = MsgBoxJson(HttpContext, "Turnkey Liquidation", "Error", sError);
                    return Json(d2);
                }
                string sNonce = BBPAPI.ERCUtilities.GetNonce(d[0]);
                if (sNonce == String.Empty)
                    sError = "Unable to find the nonce.";
                string sToAddress = u0.BBPAddress;
                if (sToAddress == String.Empty || !u0.LoggedIn)
                    sError = "Sorry, you must be logged in and have a BBP address set up.";

                if (sError != String.Empty)
                {
                    string d2 = MsgBoxJson(HttpContext, "Turnkey Liquidation", "Error", sError);
                    return Json(d2);
                }
                // Lets liquidate
                string sPayload = "<XML>Liquidate Sanc</XML>";
                KeyType k = GetKeyPair(HttpContext, sNonce);

                DACResult r0 = BBPAPI.Sanctuary.SendMoney(IsTestNet(HttpContext), k, nBalance-15,  sToAddress, sPayload);

                string sResult = String.Empty;
                if (r0.TXID != String.Empty)
                {
                    sResult = "Sent " + (nBalance - 15).ToString() + " to " + sToAddress + " on TXID " + r0.TXID;
                    DSQL.UI.GetAvatarBalance(HttpContext, true);
                }
                else
                {
                    sResult = "Failed.  [" + r0.Error + "]";
                }
                string d1 = MsgBoxJson(HttpContext, "Turnkey Liquidation", "Success", sResult);
                return Json(d1);
                *
                 */
                throw new Exception("NI");

            }
            else if (o.Action == "turnkey_createsanctuary")
            {
                /*
                string sError = String.Empty;
                if (!u0.LoggedIn)
                    sError = "Sorry, you must be logged in first.";

                List<TurnkeySanc> d = DB.GetDatabaseObjectsAsAdmin<TurnkeySanc>("turnkeysanc");

                d = d.Where(s => s.erc20address == u0.ERC20Address).ToList();
                if (d.Count > 9)
                {
                    // tical todo : check the locked value here, if they actually have 4.5MM locked per address, dont throw this error.
                    sError = "Sorry, you already have 10 sanctuaries.";
                }

                if (u0.EmailAddress == String.Empty || u0.EmailAddress == null)
                {
                    sError += "<br>Sorry, your e-mail address must be populated first.";
                }

                bool fPrimary = BBPAPI.Service.IsPrimary();

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
                KeyType k = GetKeyPair(HttpContext, sNonce);
                BBPAPI.ERCUtilities.SetNonce(t, sNonce);

                t.BBPAddress = k.PubKey;
                string ERC20Signature;
                string ERC20Address;
                HttpContext.Request.Cookies.TryGetValue("erc20signature", out ERC20Signature);
                HttpContext.Request.Cookies.TryGetValue("erc20address", out ERC20Address);
                t.Signature = ERC20Signature;
                t.Added = DateTime.Now;

                t.id = Guid.NewGuid().ToString();
                bool fIns = AsAdmin<TurnkeySanc>("options.turnkeysanc", t, t.id);

                if (!fIns)
                {
                    string d12 = MsgBoxJson(HttpContext, "Turnkey Sanctuary-Create", "Error", "Sorry, we were unable to provision your sanctuary.  Please e-mail contact@biblepay.org with the Config information and we will manually deal with this or fix the issue.  Thank you for using BiblePay.  ");
                    return Json(d12);
                }

                string sNarr = "Congratulations.  Your sanctuary provisioning request has been received.  <br><br>"
                    + "You will automatically see your daily rewards increase in the <a href='bbp/turnkeysanctuaries'>Turnkey Sanctuaries Report - Click here.</a>"
                    + "<br><br>Thank you for using BiblePay!";

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
                */
                throw new Exception("NI");
            }
            else
            {
                throw new Exception("Undocumented.");
            }


        }
    

    }
}
