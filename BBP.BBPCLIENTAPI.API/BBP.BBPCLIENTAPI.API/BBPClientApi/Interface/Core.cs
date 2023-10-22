using BMSCommon.Model;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BBPAPI.Interface
{
	public static class Core
	{
		public static string GetBaseURL()
		{
			string BaseURL = String.Empty;
			if (System.Diagnostics.Debugger.IsAttached && false)
			{
				BaseURL = "http://127.0.0.1:9000/api/";
			}
			else
			{
				BaseURL = "http://unchained.biblepay.org:9000/api/";
			}
			return BaseURL;
		}
		private static string MakeURL(string sSuffix)
		{
			string sFullURL = GetBaseURL() + sSuffix;
			return sFullURL;
		}

		public async static Task<UnchainedReply> UploadFileToSanc(string sCDN, string sAPIKey, string sFilePath, string sURL)
		{
			UnchainedReply ur = new UnchainedReply();
			try
			{
				string sEP = sCDN + "pin/bbpingress";

				HttpContent bytesContent = new ByteArrayContent(System.IO.File.ReadAllBytes(sFilePath));
				using (var httpClient = new System.Net.Http.HttpClient())
				{
					using (var request = new HttpRequestMessage(new HttpMethod("POST"), sEP))
					{
						httpClient.Timeout = new System.TimeSpan(0, 60, 00);
						httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
						httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Key", sAPIKey);
						httpClient.DefaultRequestHeaders.TryAddWithoutValidation("url", sURL);
						var multipartContent = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture));
						multipartContent.Add(bytesContent, "file", System.IO.Path.GetFileName(sFilePath));
						request.Content = multipartContent;
						// the following line is not good, but OK for debugging:
						//ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
						var oInitialResponse = await httpClient.PostAsync(sEP, multipartContent);
						string sJsonResponse = await oInitialResponse.Content.ReadAsStringAsync();
						string sOut = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(sJsonResponse);
						ur = Newtonsoft.Json.JsonConvert.DeserializeObject<UnchainedReply>(sOut);
						return ur;
						//Common.Log("Uploaded " + sFilePath + " to " + sURL);
						//return true;
					}
				}
			}
			catch (Exception ex)
			{
				ur.Error = ex.Message;
				return ur;
			}
		}

		internal static async Task<T> ReturnObject<T>(string sEndPoint, object m)
		{
			string EP = MakeURL(sEndPoint);
			string sBody = Newtonsoft.Json.JsonConvert.SerializeObject(m);
			HeaderPack h = new HeaderPack();
			h.listKeys.Add("body");
			h.listValues.Add(sBody);
			string sResp2 = await HTTPUtilities.PostToWebAPIEndpoint(EP, "", h, "POST");
			object oOut = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(sResp2);
			return (T)oOut;
		}

		internal static async Task<string> ReturnObjectAsString(string sEndPoint, object m)
		{
			string EP = MakeURL(sEndPoint);
			string sBody = Newtonsoft.Json.JsonConvert.SerializeObject(m);
			HeaderPack h = new HeaderPack();
			h.listKeys.Add("body");
			h.listValues.Add(sBody);
			string sResp2 = await HTTPUtilities.PostToWebAPIEndpoint(EP, "", h, "POST");
			return sResp2;
		}

		public async static Task<DACResult> SendEmail(BBPOutboundEmail m)
		{
			DACResult r = await ReturnObject<DACResult>("core/SendEmail", m);
			return r;
		}

		public static double GetPriceQuote(string sTicker)
		{
			double n = ReturnObject<double>("core/GetPriceQuote", sTicker).Result;
			return n;
		}
	}
}
