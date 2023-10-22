using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BBPAPI
{

	public class BBPOutboundEmail
	{
		public string From { get; set; }
		public List<string> To { get; set; }
		public List<string> CC { get; set; }
		public List<string> BCC { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public bool IsBodyHTML { get; set; }
		public bool TestNet { get; set; }

		public BBPOutboundEmail()
		{
			CC = new List<string>();	
			BCC = new List<string>();	
			To = new List<string>();	
			From = String.Empty;
			Subject = String.Empty;
			Body = String.Empty;	
		}
	}

	public class BBPEmailModel
	{
		public string ID { get; set; }
		public string Subject { get; set; }
		public string Textbody { get; set; }
		public string HTMLbody { get; set; }
		public string From { get; set; }
		public string ReplyTo { get; set; }

		public List<string> To { get; set; }
		public List<string> CC { get; set; }
		public List<string> Attachments { get; set; }
		public List<String> AttachmentFileNames { get; set; }

		public string messagefilename { get; set; }
		public string messagefrom { get; set; }
		public int messagesize { get; set; }
		public int messageflags { get; set; }
		public string messagecreatetime { get; set; }

		public string ShortBody()
		{
			string sText = Textbody ?? String.Empty;
			if (sText.Length > 80)
			{
				sText = sText.Substring(0, 80);
			}
			return sText;
		}

		public string GetAttachments()
		{
			var filePath = "./wwwroot/Downloads"; // Path.GetTempPath();
			string html = String.Empty;
			for (int i = 0; i < Attachments.Count; i++)
			{
				string sFN = AttachmentFileNames[i];
				if (sFN == "")
				{
					sFN = "empty.dat";
				}
				string sPath = Path.Combine(filePath, sFN);
				byte[] bytes = Convert.FromBase64String(Attachments[i]);
				System.IO.File.WriteAllBytes(sPath, bytes);
				string sAnchor = "<a target='_blank' href='./Downloads/" + sFN + "'>" + sFN + "</a>";
				html += "&nbsp;" + sAnchor + "&nbsp;";
			}
			return html;
		}

		public string GetMessageID()
		{
			string id = messagefilename.Replace("}.eml", "");
			id = id.Replace("{", "");
			return id;
		}
		public string ActiveBody()
		{
			string sMyHTML = HTMLbody ?? String.Empty;
			string sBody = String.Empty;
			if (sMyHTML.Length > 1)
			{
				sBody = sMyHTML;
			}
			else 
			{ 
				sBody = Textbody;
				sBody = sBody.Replace("\r\n", "\n");
				sBody = sBody.Replace("\n", "<br>");
			}

			return sBody;
		}

		public BBPEmailModel()
		{
			Subject = string.Empty;
			Textbody = string.Empty;
			HTMLbody = string.Empty;
			CC = new List<string>();
			To = new List<string>();
			CC = new List<string>();
			ID = String.Empty;
			From = String.Empty;
			Attachments = new List<string>();
			AttachmentFileNames = new List<String>();

			ReplyTo = String.Empty;
			messagefilename = String.Empty;
			messagefrom = String.Empty;
			messagesize = 0;
			messageflags = 0;
			messagecreatetime = String.Empty;
		}

	}


	public class HMailPack
    {
        public List<BBPEmailModel> hMailStubs = new List<BBPEmailModel>();
    }

}
