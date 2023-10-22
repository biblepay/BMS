using BMSCommon.Model;
using BMSCommon.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static BBPAPI.Interface.Core;

namespace BBPAPI.Interface
{
	public static class Repository
	{

		public static List<T> GetDatabaseObjects<T>(DatabaseQuery m)
		{
			List<T> r = ReturnObject<List<T>>("repository/GetDatabaseObjects", m).Result;
			return r;
		}
		public static List<T> GetDatabaseObjects<T>(string sTableName, string sOrderBy="")
		{
			DatabaseQuery m = new DatabaseQuery();
			m.TableName = sTableName;
			m.OrderBy = sOrderBy;
			m.FullyQualifiedName = typeof(T).FullName;

			List<T> r = ReturnObject<List<T>>("repository/GetDatabaseObjects", m).Result;
			return r;
		}

		public static bool StoreData<T>(string sTable, Object o, string sKey)
		{
			DatabaseQuery m = new DatabaseQuery();
			m.TableName = sTable;
			m.BusinessObject = o;
			m.Key = sKey;
			m.FullyQualifiedName = typeof(T).FullName;
			bool f = ReturnObject<bool>("repository/StoreData", m).Result;
			return f;
		}

		public static double GetUserCountByNickName(string m)
		{
			double r = ReturnObject<double>("repository/GetUserCountByNickName", m).Result;
			return r;
		}

		public static HeaderPack GetHmailHeaders(User u)
		{
			HeaderPack h = ReturnObject<HeaderPack>("EMail/GetHmailHeaders", u).Result;
			return h;
		}

		public static string EncAes3(string s)
		{
			string e = ReturnObject<string>("Repository/EncAes3", s).Result;
			return e;
		}


		public static string DecAes3(string s)
		{
			string e = ReturnObject<string>("Repository/DecAes3", s).Result;
			return e;
		}
		public static DataTable GetWellsReport()
		{
			DataTable dt = ReturnObject<DataTable>("repository/GetWellsReport", null).Result;
			return dt;
		}

		public static bool SaveTimeLine(Timeline t)
		{
			bool b = ReturnObject<bool>("repository/SaveTimeLine", t).Result;
			return b;
		}

		public static List<Timeline> GetTimeLine(GetBusinessObject bo)
		{
			List<Timeline> l = ReturnObject<List<Timeline>>("repository/GetTimeLine", bo).Result;
			return l;
		}

		public static List<string> GetWellsPinsReport()
		{
			List<string> dt = ReturnObject<List<string>>("repository/GetWellsPinsReport", null).Result;
			return dt;
		}

		public static List<Video> GetVideos(GetBusinessObject s)
		{
			List<Video> l = ReturnObject<List<Video>>("video/GetVideos", s).Result;
			return l;
		}

		public static bool PersistChatItem(ChatItem ci)
		{
			bool f = ReturnObject<bool>("repository/PersistChatItem", ci).Result;
			return f;
		}
		public static DataTable GetNotifications(GetBusinessObject b)
		{
			DataTable f = ReturnObject<DataTable>("repository/GetNotifications", b).Result;
			return f;
		}
		public static List<VerseMemorizer> GetVerseMemorizers()
		{
			List<VerseMemorizer> f = ReturnObject<List<VerseMemorizer>>("repository/GetVerseMemorizers", null).Result;
			return f;
		}
		public static bool InsertChatNotification(ChatItem ci)
		{
			bool f= ReturnObject<bool>("repository/InsertChatNotification", ci).Result;
			return f;
		}


		public static bool UpdateUserEmailAsVerified(string sUserID)
		{
			bool f = ReturnObject<bool>("repository/UpdateUserEmailAsVerified",sUserID).Result;
			return f;
		}

		public static bool InsertEmailAccount(EmailAccount e)
		{
			return ReturnObject<bool>("repository/InsertEmailAccount", e).Result;
		}
		public static bool StoreAttachment(Attachment e)
		{
			return ReturnObject<bool>("repository/StoreAttachment", e).Result;
		}
		public static DataTable GetArticles(string sType)
		{
			return ReturnObject<DataTable>("repository/GetArticles", sType).Result;
		}
		public static DataTable GetChats(bool f)
		{
			return ReturnObject<DataTable>("repository/GetChats", f).Result;
		}

		public static List<EmailAccount> GetEmailAccounts(string s)
		{
			List<EmailAccount> e = ReturnObject<List<EmailAccount>>("repository/GetEmailAccounts", s).Result;
			return e;
		}

		public static bool SaveVideo(Video v)
		{
			bool f = ReturnObject<bool>("video/SaveVideo", v).Result;
			return f;
		}


		public static bool PersistUser(User p)
		{
			bool f = ReturnObject<bool>("user/PersistUser", p).Result;
			return f;
		}
		public static bool GobjectSerialize(Proposal p)
		{
			bool f = ReturnObject<bool>("Proposal/gobject_serialize", p).Result;
			return f;
		}

		public static List<Pin> GetPinsByUserID(string p)
		{
			List<Pin> p1 = ReturnObject<List<Pin>>("pin/GetPinsByUserID", p).Result;
			return p1;
		}
		public static double GetUserCountByEmail(string p)
		{
			double n = ReturnObject<double>("user/GetUserCountByEmail", p).Result;
			return n;
		}



	}
}
