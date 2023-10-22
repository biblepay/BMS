using BMSCommon;
using BMSCommon.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static BBPAPI.ERCUtilities;
using static BBPAPI.Interface.Core;
using System.Data;

namespace BBPAPI.Interface
{
	public static class Phone
	{

		public static List<DropDownItem> GetDropDownItems(DataTable dt)
		{
			List<DropDownItem> dd = new List<DropDownItem>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				DropDownItem ddItem = new DropDownItem();
				ddItem.key0 = dt.Rows[i]["key0"].ToStr();
				ddItem.text0 = dt.Rows[i]["value0"].ToStr();
				dd.Add(ddItem);
			}
			return dd;
		}


		public static string GetDefaultGreeting()
		{
			string sGreeting = "Hello Compadre!  You have reached my Voice Mailbox!  Please leave a message after the tone.";
			return sGreeting;
		}
		
		public static PhoneUser GetPhoneUser(User u)
		{
			PhoneUser r = ReturnObject<PhoneUser>("phone/GetPhoneUser", u).Result;
			return r;
		}
		public static DataTable GetDistinctSMSUserPhoneNumbers(User u)
		{
			DataTable r = ReturnObject<DataTable>("phone/GetDistinctSMSUserPhoneNumbers", u).Result;
			return r;
		}

		public static DataTable GetPhoneMappings(User u)
		{
			DataTable r = ReturnObject<DataTable>("phone/GetPhoneMappings", u).Result;
			return r;
		}

		public static long SendSMS(SMSMessage msg)
		{
			long r = ReturnObject<long>("phone/SendSMS", msg).Result;
			return r;
		}

		public static bool SetVoicemailGreeting(VoiceGreeting vg)
		{
			bool f = ReturnObject<bool>("phone/SetVoiceMailGreeting", vg).Result;
			return f;
		}

		public static bool SetPhoneUserPhoneNumber(NewPhoneUser npu)
		{
			bool f = ReturnObject<bool>("phone/SetPhoneUserPhoneNumber", npu).Result;
			return f;
		}
		public static DataTable GetSMSMessages(SMSMessage s1)
		{
			DataTable dt = ReturnObject<DataTable>("phone/GetSMSMessages", s1).Result;
			return dt;
		}

		public static bool ValidateCountryCode(string s)
		{
			bool f = ReturnObject<bool>("phone/ValidateCountryCode", s).Result;
			return f;
		}
		public static string GetRoute(string sDestination, string sCallerID)
		{
			if (String.IsNullOrEmpty(sCallerID)) sCallerID = "UNKNOWN";
			if (String.IsNullOrEmpty(sDestination)) sDestination = "UNKNOWN";
			string s = ReturnObjectAsString("phone/GetRoute?callerid=" + sCallerID + "&destination=" + sDestination, null).Result;
			return s;
		}


		public static bool InsertCountryOverride(PhoneUserCountry p)
		{
			bool r = ReturnObject<bool>("phone/InsertCountryOverride", p).Result;
			return r;
		}

		public static DataTable GetRateOverrideReport(int nUserID)
		{
			DataTable r = ReturnObject<DataTable>("phone/GetRateOverrideReport", nUserID).Result;
			return r;
		}

		public static string GetPhoneUserNameBasedOnRecordCount(User u)
		{
			string s = ReturnObject<string>("phone/GetPhoneUserNameBasedOnRecordCount", u).Result;
			return s;
		}

		public static DataTable GetCallHistoryReport(long nID)
		{
			DataTable dt = ReturnObject<DataTable>("phone/GetCallHistoryReport", nID).Result;
			return dt;
		}

		public static bool UpdatePhoneUserMapping(PhoneUserMappingUpdate m)
		{
			bool r = ReturnObject<bool>("phone/UpdatePhoneUserMapping", m).Result;
			return r;
		}

		public static List<DropDownItem> GetRegions(string sState)
		{
			var r = ReturnObject<List<DropDownItem>>("phone/GetRegions", sState).Result;
			return r;
		}
		public static DataTable GetVoiceMailSettings(int nUser)
		{
			var r = ReturnObject<DataTable>("phone/GetVoiceMailSettings", nUser).Result;
			return r;
		}
		public static DataTable GetRatesReport(double nAmt)
		{
			var r = ReturnObject<DataTable>("phone/GetRatesReport", nAmt).Result;
			return r;
		}
		public static DataTable GetVoiceMailReport(int nUser)
		{
			var r = ReturnObject<DataTable>("phone/GetVoiceMailReport", nUser).Result;
			return r;
		}

		public static bool DeleteVoiceMail(string guid)
		{
			var r = ReturnObject<bool>("phone/DeleteVoiceMail", guid).Result;
			return r;
		}

		public static List<DropDownItem> GetPhoneNumbersOwnedByAddress(User u)
		{
			var r = ReturnObject<List<DropDownItem>>("phone/GetPhoneNumbersOwnedByAddress", u).Result;
			return r;
		}
		public static bool InsertPhoneUser(NewPhoneUser p)
		{
			var r = ReturnObject<bool>("phone/InsertPhoneUser", p).Result;
			return r;
		}

		public static string ProcessVoiceMailLD(PhoneCallerDestination ld)
		{
			var r = ReturnObject<string>("phone/ProcessVoiceMailLD", ld).Result;
			return r;
		}

		public static long AddNewPhoneUser(BBPAddressKey b)
		{
			var r = ReturnObject<long>("phone/AddNewPhoneUser", b).Result;
			return r;
		}
		public static string BuyAndGetNewPhoneNumber(PhoneRegionCountryAddress b)
		{
			var r = ReturnObject<string>("phone/BuyAndGetNewPhoneNumber", b).Result;
			return r;
		}

		public static PhoneUser GetPhoneUserRouting(PhoneCallerDestination b)
		{
			var r = ReturnObject<PhoneUser>("phone/GetPhoneUserRouting", b).Result;
			return r;
		}
		public static string GetRoute2(PhoneCallerDestination b)
		{
			var r = ReturnObject<string>("phone/GetRoute", b).Result;
			return r;
		}

		public static bool SetPhoneRulesCreated(long n)
		{
			var r = ReturnObject<bool>("phone/SetPhoneRulesCreated", n).Result;
			return r;
		}

		public static void ProcessWebHookLD(string sBody)
		{
			var r = ReturnObject<string>("phone/ProcessWebHookLongDistance", sBody);
		}

	}
}
