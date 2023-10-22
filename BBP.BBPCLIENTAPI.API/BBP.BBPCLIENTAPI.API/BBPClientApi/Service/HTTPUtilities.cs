using BMSCommon.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BBPAPI
{
	internal class HTTPUtilities
	{
		public async static Task<string> PostToEndpoint(string sURL, string sBody, HeaderPack h)
		{
			try
			{
				HttpContent content = new StringContent(sBody);
				using (var httpClient = new System.Net.Http.HttpClient())
				{
					using (var request = new HttpRequestMessage(new HttpMethod("POST"), sURL))
					{
						httpClient.Timeout = new System.TimeSpan(0, 5, 00);
						int iLoc = 0;
						if (h != null)
						{
							foreach (string sKey in h.listKeys)
							{
								string sValue = h.listValues[iLoc];
								httpClient.DefaultRequestHeaders.TryAddWithoutValidation(sKey, sValue);
								iLoc++;
							}
						}
						httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
						request.Content = content;
						ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
						var oInitialResponse = await httpClient.PostAsync(sURL, content);
						string sJsonResponse = await oInitialResponse.Content.ReadAsStringAsync();
						string sOut = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(sJsonResponse);
						return sOut;
					}
				}
			}
			catch (Exception ex)
			{
				return string.Empty;
			}
		}



		public async static Task<string> PostToWebAPIEndpoint(string sURL, string sBody, HeaderPack h, string sMethod)
		{
			try
			{
				HttpContent content = new StringContent(sBody);
				var httpClient = new System.Net.Http.HttpClient(new HttpClientHandler
				{
					UseProxy = false
				});

				using (httpClient)
				{
					using (var request = new HttpRequestMessage(new HttpMethod(sMethod), sURL))
					{
						httpClient.Timeout = new System.TimeSpan(0, 5, 00);
						int iLoc = 0;
						if (h != null)
						{
							foreach (string sKey in h.listKeys)
							{
								string sValue = h.listValues[iLoc];
								httpClient.DefaultRequestHeaders.TryAddWithoutValidation(sKey, sValue);
								iLoc++;
							}
						}
			
						httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
						request.Content = content;
						ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
						var oInitialResponse = await httpClient.PostAsync(sURL, content);
						string sJsonResponse = await oInitialResponse.Content.ReadAsStringAsync();
						string sOut = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(sJsonResponse);
						return sOut;
					}
				}
			}
			catch (Exception ex)
			{
				return string.Empty;
			}
		}





	}
}
