using BMSCommon;
using BMSCommon.Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BBPAPI.Service;
using static BBPAPI.ERCUtilities;

namespace BBPAPI.Interface
{
	public static class EMail
	{
		public async static Task<BBPEmailModel> GetMailItem(User u, string sID)
		{
			
			HeaderPack h1 = BBPAPI.Interface.Repository.GetHmailHeaders(u);
			h1.listKeys.Add("mail-id");
			h1.listValues.Add(sID);
			string sEP = "https://seven.biblepay.org/BMS/RetrieveEmail";
			string sResp2 = await BBPAPI.HTTPUtilities.PostToEndpoint(sEP, "", h1);
			BBPEmailModel e = Newtonsoft.Json.JsonConvert.DeserializeObject<BBPEmailModel>(sResp2);
			return e;
		}
		public async static Task<HMailPack> GetMailInbox(User u)
		{
			string sEP = "https://seven.biblepay.org/BMS/RetrieveEmails";
			HeaderPack h1 = BBPAPI.Interface.Repository.GetHmailHeaders(u);
			string sResp = await BBPAPI.HTTPUtilities.PostToEndpoint(sEP, "", h1);
			HMailPack p = Newtonsoft.Json.JsonConvert.DeserializeObject<HMailPack>(sResp);
			return p;
		}



		public async static Task<int> ProvisionBBPEmailService(string sNickName, string sPrivKey)
		{
			string sEP = "https://seven.biblepay.org/BMS/ProvisionEmail";
			List<string> sHeaders = new List<string>();
			List<string> sValues = new List<string>();
			string sPubKey = Encryption.GetPubKeyFromPrivKey(sPrivKey, false);
			if (String.IsNullOrEmpty(sPubKey))
			{
				return 602;
			}

			sHeaders.Add("mail-username");
			sHeaders.Add("mail-pass");
			sHeaders.Add("mail-domain");
			sValues.Add(sNickName);
			sValues.Add(sPrivKey);
			sValues.Add("biblepay.org");
			string sOut = await HitEndpointWithHeaders(sEP, sHeaders, sValues);
			sOut = sOut.Replace("\"", "");
			sOut = sOut.Replace("\r\n", "");
			sOut = sOut.Trim();

			double nOut = (sOut.Trim()).ToDouble();
			if (nOut == 1)
			{
				EmailAccount e = new EmailAccount();
				e.UserName = sNickName;
				e.Domain = "biblepay.org";
				e.BBPAddress = sPubKey;
				bool fSaved = BBPAPI.Interface.Repository.InsertEmailAccount(e);
				if (!fSaved)
				{
					return 600;
				}
				return 1;

			}
			return (int)nOut;
		}




	}
}
