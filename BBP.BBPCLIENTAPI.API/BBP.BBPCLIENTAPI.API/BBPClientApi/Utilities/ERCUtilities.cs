using Amazon.S3.Model;
using BBPAPI.Model;
using BMSCommon;
using BMSCommon.Model;
using BMSCommon.Models;
using Google.Authenticator;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static BMSCommon.Encryption;

namespace BBPAPI
{

	internal static class SecureString
	{

		internal static List<string> listRPCErrors = new List<string>();

		internal static void LogRPCError(string sError)
		{
			if (listRPCErrors.Contains(sError))
				return;

			listRPCErrors.Add(sError);
			if (listRPCErrors.Count > 50)
			{
				listRPCErrors.RemoveAt(0);
			}
		}

		/*
		public static void SDCK3(string sKeyName, string sKeyValue, string sKeyPass)
		{
			// This function Set DatabaseConfigKeyValue is protected by sKeyPass, so callers cant abuse it.
			string sInternalPass = SecureString.GetCSCKey3();
			if (sInternalPass == sKeyPass)
			{
				string sql = "Upsert into options.config (systemkey,value,added) values (@key,@value,now());";
				NpgsqlCommand c = new NpgsqlCommand(sql);
				c.Parameters.AddWithValue("@key", sKeyName);
				c.Parameters.AddWithValue("@value", BMSCommon.Encryption.EncryptAES256(sKeyValue, SecureString.GetCSCKey3()));
				//DB.ExecuteNonQuery(c);
			}
		}
        
		internal static string GetCSCKey3()
		{
			string s1 = "b92204f4";
			string s2 = "b031";
			string s3 = "472d";
			string s4 = "9041" + "8be8a6231a7b";
			return s1 + s2 + s3 + s4;
		}

		internal static string GetDBConfiationKeyValue3(string sKeyName)
		{
			string sql = "Select Value from options.config where systemkey=@key;";
			NpgsqlCommand c = new NpgsqlCommand(sql);
			c.Parameters.AddWithValue("@key", sKeyName);
            string v = "";// DB.GetScalarString(c, "Value");
			string data = Encryption.DecryptAES256(v, GetCSCKey3());
			return data;
		}
        */

	}




	public static class ERCUtilities
    {

		public async static Task<string> HitEndpointWithHeaders(string sEndpoint, List<string> headerKeys, List<string> headerValues)
		{
			try
			{

				using (var httpClient = new System.Net.Http.HttpClient())
				{
					using (var request = new HttpRequestMessage(new HttpMethod("POST"), sEndpoint))
					{
						httpClient.Timeout = new System.TimeSpan(0, 60, 00);
						httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
						for (int i = 0; i < headerKeys.Count; i++)
						{
							httpClient.DefaultRequestHeaders.TryAddWithoutValidation(headerKeys[i], headerValues[i]);
						}
						var multipartContent = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture));
						//multipartContent.Add(bytesContent, "file", System.IO.Path.GetFileName(sFilePath));
						request.Content = multipartContent;
						//ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
						var oInitialResponse = await httpClient.PostAsync(sEndpoint, multipartContent);
						string sJsonResponse = await oInitialResponse.Content.ReadAsStringAsync();
						return sJsonResponse;
					}
				}
			}
			catch (Exception ex)
			{
				return "399";
			}
		}



		public static void SetUserEmail2(User u, string sEmail)
        {
            u.EmailAddress = BBPAPI.Interface.Repository.EncAes3(sEmail);
        }

        public static string BindEmailAddressValue(User u)
        {
            BMSCommon.Common.Log(u.EmailAddress.ToString());
            return BBPAPI.Interface.Repository.DecAes3(u.EmailAddress);
        }

        public static DACResult SendVerificationEmail(User u)
        {
            BBPOutboundEmail e = new BBPOutboundEmail();

            e.To.Add(BBPAPI.Interface.Repository.EncAes3(u.EmailAddress)); //Can we add the address + name? ::  u.NickName
            e.BCC.Add("Rob@biblepay.org"); //, "team biblepay");
            e.Subject = "E-Mail Verification";

            string sURL = "https://unchained.biblepay.org/profile/verifyemailaddress?id=" + u.id.ToString()
                + "&key=" + Encryption.Base64Encode(BBPAPI.Interface.Repository.EncAes3(u.id));
            e.Body = "<br>Dear " + u.NickName + ", <br><br> Please verify your e-mail address <a href='" + sURL + "'>by clicking here.</a><br><br><br>"
                + "<br>Thank you for using BiblePay!";
            e.IsBodyHTML = true;
            e.TestNet = false;
            DACResult r = BBPAPI.Interface.Core.SendEmail(e).Result;
            return r;
        }

        public static bool DecryptionMatches(string sSecret, string sUserID)
        {
            string sDec = BBPAPI.Interface.Repository.EncAes3(sSecret);
            return (sDec == sUserID);
        }

        /*
        public static DACResult SendNFTEmail(bool fTestNet, User uBuyer, User dtSeller, NFT n, double nAmount)
        {
            try
            {
                BBPOutboundEmail e = new BBPOutboundEmail();
                e.To.Add("contact@biblepay.org");

                //MailMessage m = new MailMessage();
                //m.To.Add(mTo);
                e.BCC.Add(BBPAPI.ERCUtilities.GetEmailRetired(uBuyer.EmailAddress));//, uBuyer.NickName);

                string sSellerEmail = BBPAPI.ERCUtilities.GetEmailRetired(dtSeller.EmailAddress);
                string sSellerNickName = dtSeller.NickName ?? String.Empty;
                if (sSellerEmail == null)
                    sSellerEmail = "Unknown Seller";
                if (sSellerNickName == String.Empty)
                    sSellerNickName = "Unknown Seller NickName";
                if (n.Type.ToLower() == "orphan")
                {
                    //e.CC.Add(sSellerEmail);
                    try
                    {
                        e.CC.Add(sSellerEmail);

                       // m.Bcc.Add(mBCC2);
                    }
                    catch (Exception) { }
                }
                else
                {
                    //e.BCC.Add(sSellerEmail);
                    try
                    {
                        //MailAddress mBCC2 = new MailAddress(sSellerEmail, sSellerNickName);
                        e.BCC.Add(sSellerEmail);
                    }
                    catch (Exception) { }
                }

                string sSubject = (n.Type.ToLower() == "orphan") ? "Orphan Sponsored " + n.GetHash() : "Bought NFT " + n.GetHash();

                e.Subject = sSubject;
                string sPurchaseNarr = (n.Type.ToLower() == "orphan") ? "has been sponsored" : "has been purchased";

                e.Body = "<br>Dear " + sSellerNickName + ", " + sPurchaseNarr + " by " + uBuyer.NickName + " for "
                    + nAmount.ToString() + ".  <br><br><br><h3>"
                    + n.Name + "</h3><br><br><div><span>" + n.Description + "</div><br><br><br><img src='" + n.AssetURL
                    + "' width=400 height=400/><br><br><br>Thank you for using BiblePay!";

                e.IsBodyHTML = true;
                e.TestNet = fTestNet;

                // TODO: Charge 5 BBP per mail message, so it cant be abused.

                return BBPAPI.Interface.Core.SendEmail(e).Result;
            }catch(Exception ex)
            {
                Common.Log(ex.Message);
                DACResult r = new DACResult();
                return r;
            }
        }
        */

        /*
        private static string GetEmailRetired(string sEmail)
        {
            return Encryption2.DecAES3(sEmail);
        }
        */


        public static bool ValidateMFA(User u, string sCode)
        {
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            bool isValid = twoFactor.ValidateTwoFactorPIN(BBPAPI.Interface.Repository.EncAes3(u.MFA), sCode);
            return isValid;
        }

        
        public static void SetMFAKey(User u, string sMFA)
        {
            u.MFA = BBPAPI.Interface.Repository.EncAes3(sMFA);
        }

        public static DACResult SendBBPFromSubscription(bool fTestNet, BBPKeyPair kPayer, string sToAddress, double nAmount, string sOptPayload = "", string sOptNonce = "")
        {
            BBPNetAddress b = new BBPNetAddress();
            b.TestNet = fTestNet;
            b.Address = kPayer.PubKey;
            string sData = BBPAPI.Interface.WebRPC.GetAddressUTXOs(b).Result;
            string sErr = String.Empty;
            string sTXID = String.Empty;
            NBitcoin.Crypto.BBPTransaction.PrepareFundingTransaction(fTestNet, nAmount, sToAddress, kPayer.PrivKey, sOptPayload, sData, out sErr, out sTXID);
            DACResult r = new DACResult();
            if (sErr != String.Empty)
            {
                r.Error = sErr;
                return r;
            }
            BBPNetHex r10 = new BBPNetHex();
            r10.TestNet = fTestNet;
            r10.Hex = sTXID;
            r = BBPAPI.Interface.WebRPC.SendRawTx(r10).Result;
            return r;
        }



        public static double QueryAddressBalance(bool fTestNet, string sAddress)
        {
            if (String.IsNullOrEmpty(sAddress))
            {
                return 0;
            }
            BBPNetAddress ba = new BBPNetAddress();
            ba.TestNet = fTestNet;
            ba.Address = sAddress;
            string sUTXOData = BBPAPI.Interface.WebRPC.GetAddressUTXOs(ba).Result;
            double nAmt = NBitcoin.Crypto.BBPTransaction.QueryAddressBalance(fTestNet, sAddress, sUTXOData);
            BMSCommon.Common.Log("QAB::Address " + sAddress + "=" + nAmt.ToString());
            return nAmt;
        }

    }
}
