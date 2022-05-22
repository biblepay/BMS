using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using static BMSCommon.Common;
using static BiblePay.BMS.DSQL.SessionExtensions;
using Microsoft.AspNetCore.Http;
using System.Data;
using MySql.Data.MySqlClient;
using static BiblePay.BMS.DSQL.UI;

namespace BiblePay.BMS.Controllers
{
	public class MemorizeScripturesController : Controller
	{
		private TestingMemory _testingmemory = new TestingMemory();

		public IActionResult MemorizeScriptures()
		{
			Page_Load();
			return View();
		}

		private enum TestingMode
		{
			TESTING = 0,
			TRAIN = 1
		}

		private struct TestingMemory
		{
			public TestingMode TestMode;
			public int nCorrectVerse;
			public int nCorrectChapter;
			public string sCorrectBook;
			public int nTrainingQuestionsTaken;
			public int nTestingQuestionsTaken;
			public double nTrainingScore;
			public double nTestingScore;
		};

		protected void Page_Load()
		{
			if (HttpContext.Session.ObjectExists("testingmemory"))
			{ 
				_testingmemory  = HttpContext.Session.GetObject<TestingMemory>("testingmemory");
			}
    	  	else
			{
				_testingmemory.TestMode = TestingMode.TRAIN;
				HttpContext.Session.SetObject("testingmemory", _testingmemory);
			}
			UpdateDisplay();
		}

		void PopulateNewVerse()
		{
			string sql = "Select * from VerseMemorizer where 1=1";
			MySqlCommand m1 = new MySqlCommand(sql);
			DataTable dt = BMSCommon.Database.GetMySqlDataTable(false, m1, "");
			if (dt.Rows.Count < 1)
					return;
			Random r = new Random();
			int iChapter = 0;
			string sBook = "";
			int iVerseStart = 0, iVerseEnd = 0;
			string sTotalVerses = "";
			for (int y = 0; y < 9; y++)
			{
				int iRow = (int)r.Next(0, dt.Rows.Count);
 	 		    iVerseStart = (int)BMSCommon.Common.GetDouble(dt.Rows[iRow]["VerseFrom"]);
				iVerseEnd = (int)BMSCommon.Common.GetDouble(dt.Rows[iRow]["VerseTo"]);
				if (iVerseEnd < iVerseStart)
						iVerseEnd = iVerseStart;
				sBook = dt.Rows[iRow]["BookFrom"].ToString();
				iChapter = (int)BMSCommon.Common.GetDouble(dt.Rows[iRow]["ChapterFrom"]);
				sTotalVerses = "";
				for (int j = iVerseStart; j <= iVerseEnd && j >= iVerseStart; j++)
				{
					string sPrefix = _testingmemory.TestMode == TestingMode.TESTING ? "" : j.ToString() + ".  ";
					string sVerse = sPrefix + BibleRef._bible.GetVerse(sBook, iChapter, j);
					sTotalVerses += sVerse + "\r\n";
				}
				if (iVerseStart > 1 && iVerseEnd > 1 && sTotalVerses.Length > 7)
						break;
			}
			if (iVerseStart == 0)
					return;
			clear();
			ViewBag.txtChapter = iChapter.ToString();
			ViewBag.txtVerse = iVerseStart.ToString();
			string sLocalBook = BibleRef._bible.GetBookByName(sBook);
			if (sLocalBook == "")
				sLocalBook = sBook;

			_testingmemory.sCorrectBook = sLocalBook;
			_testingmemory.nCorrectVerse = iVerseStart;
			_testingmemory.nCorrectChapter = iChapter;
			string sTest = "";
			List<string> l = BibleRef._bible.GetBookList();

			for (int i = 0; i < l.Count; i++)
			{
					sTest += l[i] + " ";
			}
			if (sLocalBook.ToUpper() == "ROM")
			{
					sLocalBook = "1 PAUL TO THE ROMANS";
			}
		    ViewBag.txtScripture = sTotalVerses;
			// Set read only
			ViewBag.txtChapterReadOnly = _testingmemory.TestMode == TestingMode.TRAIN;
			ViewBag.ddBookEnabled = _testingmemory.TestMode == TestingMode.TESTING;
			if (_testingmemory.TestMode == TestingMode.TESTING)
			{
				// In test mode we need to clear the fields
				ViewBag.txtChapter = "";
				ViewBag.txtVerse = "";
				ViewBag.ddBookSelectedValue = "";
			}

			// Populate the dropdown values with the books
			List<string> lBooks = BibleRef._bible.GetBookList();
			List<string> ddBook = new List<string>();
			ddBook.Add("");
			for (int i = 0; i < lBooks.Count; i++)
			{
				ddBook.Add(lBooks[i].ToUpper());
			}
			ViewBag.ddBook = ListToHTMLSelect(ddBook, sLocalBook);
			// end of dropdown values

			string sCaption = _testingmemory.TestMode == TestingMode.TRAIN ? "Switch to TEST Mode" : "Switch to TRAIN Mode";
			ViewBag.btnSwitchToTestCaption = sCaption;

			HttpContext.Session.SetObject("testingmemory", _testingmemory);
		}

		public static double WordComparer(string Verse, string UserEntry)
		{
			Verse = Verse.ToUpper();
			UserEntry = UserEntry.ToUpper();
			string[] vVerse = Verse.Split(" ");
			string[] vUserEntry = UserEntry.Split(" ");
			double dTotal = vVerse.Length;
			double dCorrect = 0;
			for (int i = 0; i < vVerse.Length; i++)
			{
				bool f = UserEntry.Contains(vVerse[i]);
				if (f)
					dCorrect++;
			}
			double dPct = dCorrect / (dTotal + .01);
			return dPct;
		}

		public static string ShowResults(HttpContext h)
		{
			TestingMemory _testingmemory = h.Session.GetObject<TestingMemory>("testingmemory");
			double nTrainingPct = _testingmemory.nTrainingScore / (_testingmemory.nTrainingQuestionsTaken + .01);
			double nTestingPct = _testingmemory.nTestingScore / (_testingmemory.nTestingQuestionsTaken + .01);
			string sSummary = "Congratulations!";
			if (_testingmemory.nTrainingQuestionsTaken > 0)
			{
				sSummary += "<br>In training mode you worked through "
					+ _testingmemory.nTrainingQuestionsTaken.ToString()
					+ ", and your score is " + Math.Round(nTrainingPct * 100, 2).ToString()
					+ "%!  ";
			}

			if (_testingmemory.nTestingQuestionsTaken > 0)
			{
				sSummary += "<br>In testing mode you worked through "
					+ _testingmemory.nTestingQuestionsTaken.ToString()
					+ ", and your score is " + Math.Round(nTestingPct * 100, 2).ToString()
					+ "%! ";
			}
			sSummary += "<br>Please come back and see us again. ";
			// Clear the results so they can start again if they want:
			_testingmemory.nTrainingQuestionsTaken = 0;
			_testingmemory.nTestingQuestionsTaken = 0;
			_testingmemory.nTrainingScore = 0;
			_testingmemory.nTestingScore = 0;
			h.Session.SetObject("testingmemory", _testingmemory);
			return sSummary;
		}

		void UpdateDisplay()
		{
				string sMode = _testingmemory.TestMode == TestingMode.TRAIN ? "<font color=red>TRAINING MODE</font>" : "<font color=red>TESTING MODE</font>";
				string sInfo = sMode + "<br><br>Welcome to the Scripture Memorizer, " + DSQL.UI.GetUser(HttpContext).NickName + "!";
				ViewBag.lblInfo = sInfo;
				// Find the first verse to do the initial population.
				PopulateNewVerse();
		}

		void clear()
		{
			ViewBag.txtChapter = "";
  		    ViewBag.txtVerse = "";
			ViewBag.txtScripture = "";
			ViewBag.txtPractice = "";
		}

		public static void btnNextScripture_Click(HttpContext h, string sFormData)
		{
			Score(h, sFormData);
		}
		public static void btnSwitchMode_Click(HttpContext h)
		{
			TestingMemory _testingmemory = h.Session.GetObject<TestingMemory>("testingmemory");
			if (_testingmemory.TestMode == TestingMode.TRAIN)
			{
				_testingmemory.TestMode = TestingMode.TESTING;
			}
			else
			{
				_testingmemory.TestMode = TestingMode.TRAIN;
			}
			h.Session.SetObject("testingmemory", _testingmemory);
		}

		public static string btnGrade_Click(HttpContext h, string sFormData)
		{
			Score(h, sFormData);
			string sResult = ShowResults(h);
			return sResult;
		}

		public static double Grade(HttpContext h, string sFormData)
		{
			TestingMemory _testingmemory = h.Session.GetObject<TestingMemory>("testingmemory");
			string sUserBook = DSQL.UI.GetFormData(sFormData, "ddBook");//selected value?
			int iChapter = (int)GetDouble(GetFormData(sFormData, "txtChapter"));
			int iVerse = (int)GetDouble(GetFormData(sFormData, "txtVerse"));
			sUserBook = sUserBook.ToUpper();
			_testingmemory.sCorrectBook = _testingmemory.sCorrectBook.ToUpper();
			double nResult = 0;
			if (sUserBook == _testingmemory.sCorrectBook)
					nResult += .3333;
			if (_testingmemory.nCorrectChapter == iChapter)
					nResult += .3333;
			if (_testingmemory.nCorrectVerse == iVerse)
					nResult += .3334;
			h.Session.SetObject("testingmemory", _testingmemory);
			return nResult;
		}

		public static void Score(HttpContext h, string sFormData)
		{
			TestingMemory _testingmemory = h.Session.GetObject<TestingMemory>("testingmemory");

			if (_testingmemory.TestMode == TestingMode.TESTING)
			{
				string sTxtScripture = DSQL.UI.GetFormData(sFormData, "txtScripture");
				string sTxtPractice = DSQL.UI.GetFormData(sFormData, "txtPractice");
				double nPct = WordComparer(sTxtScripture, sTxtPractice);
				_testingmemory.nTrainingQuestionsTaken++;
				_testingmemory.nTrainingScore += nPct;
			}
			// Score the current Testing session
			if (_testingmemory.TestMode == TestingMode.TRAIN)
			{
					_testingmemory.nTestingQuestionsTaken++;
					double nTestPct = Grade(h, sFormData);
					_testingmemory.nTestingScore += nTestPct;
			}
			h.Session.SetObject("testingmemory", _testingmemory);
		}
	}
}

