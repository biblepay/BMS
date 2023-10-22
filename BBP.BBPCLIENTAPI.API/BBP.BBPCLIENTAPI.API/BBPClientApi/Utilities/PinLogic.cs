using BMSCommon.Model;
using BMSCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BMSCommon.Encryption;

namespace BBPAPI.Utilities
{
	public static class PinLogic
	{

		public static DACResult GetFile(bool fTestNet, string sPath, string sLocalSavePath, string sBBPPrivKey)
		{
			string sLocalHash = String.Empty;
			string sPubAddress = GetPublicKeyFromPrivateKey(sBBPPrivKey, fTestNet);

			DACResult r = new DACResult();
			if (System.IO.File.Exists(sLocalSavePath))
			{
				sLocalHash = Common.GetShaOfFile(sLocalSavePath);
				if (sLocalHash != String.Empty)
				{
					HashPath hp = new HashPath();
					hp.Hash = sLocalHash;
					hp.Path = sPath;

					List<Pin> l = Interface.PinLogic.GetPinsByHash(hp);
					if (l.Count > 0)
					{
						// File already exists.
						return r;
					}
				}
			}
			// pull it down
			string sFullPath = sPubAddress + "/" + sPath;
			//bool fDL = StorjIO.StorjDownloadLg(sFullPath, sLocalSavePath).Result;

			Task<bool> fTask = Task.Run<bool>(async () => await BBPAPI.StorjIOReadOnly.StorjDownloadLg(sFullPath, sLocalSavePath));
			bool fDL = fTask.Result;


			if (!fDL)
			{
				r.Error = "Unable to download";
			}
			return r;

		}

		public static Pin StoreFile(User u, string sFilePath, string sStorjPath, string sSystem)
		{
			string sUserID = u.id;
			string sPrivKey = u.GetPrivateKey();

			if (sSystem == "TICKET")
			{
				sPrivKey = "_INTERNAL_";
			}
			Pin p = StoreFileWithPrivKey(u, sFilePath, sStorjPath, sPrivKey);
			return p;
		}
		public static Pin StoreFileWithPrivKey(User u, string sFilePath, string sStorjPath, string sBBPPrivKey)
		{
			Pin p = new Pin();
			BMSCommon.Model.DACResult r = new DACResult();

			if (!System.IO.File.Exists(sFilePath))
			{
				r.Error = "File does not exist.";
				return p;
			}
			string sHash = Common.GetShaOfFile(sFilePath);
			HashPath hp = new HashPath();
			hp.Hash = sHash;
			hp.Path = sStorjPath;
			List<Pin> l = Interface.PinLogic.GetPinsByHash(hp);
			string sURL = String.Empty;
			
			if (l.Count > 0 && false)
			{
				sURL = p.URL;
			}
			else
			{
				UploadFileObject ufo = new UploadFileObject();
				ufo.SourceFilePath = sFilePath;
				ufo.StorjDestinationPath = sStorjPath;
				ufo.OverriddenBBPPrivateKey = sBBPPrivKey;
				UploadFileResult ufoOut = BBPAPI.Interface.PinLogic.UploadFile(ufo).Result;
				sURL = ufoOut.URL;
			}

			if (sURL == String.Empty)
			{
				r.Error = "Unable to store file";
				return p;
			}
			
			p.FileHash = sHash;
			System.IO.FileInfo fi = new System.IO.FileInfo(sFilePath);
			p.BBPAddress = GetPublicKeyFromPrivateKey(sBBPPrivKey, u.TestNet);
			p.Size = fi.Length;
			p.UserID = u.id;
			p.Path = sStorjPath;
			p.URL = sURL;
			r.Response = p.URL;
			bool f1 = Interface.PinLogic.DbSavePin(p);
			if (!f1)
			{
				r.Error = "Unable to save pin.";
			}

			return p;
		}

	}
}
