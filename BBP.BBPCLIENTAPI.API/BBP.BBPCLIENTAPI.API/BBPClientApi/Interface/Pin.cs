using BMSCommon.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BBPAPI.Interface.Core;

namespace BBPAPI.Interface
{
	public static class PinLogic
	{
		public static async Task<UploadFileResult> UploadFile(UploadFileObject u)
		{
			//u.FileBytes = System.IO.File.ReadAllBytes(u.SourceFilePath);

			UnchainedReply ur = await Core.UploadFileToSanc(GetBaseURL(), u.OverriddenBBPPrivateKey, u.SourceFilePath, u.StorjDestinationPath);
			UploadFileResult ur1 = new UploadFileResult();
			ur1.URL = ur.URL;
			ur1.Error = ur.Error;
			return ur1;
		}
		public static List<Pin> GetPinsByHash(HashPath p)
		{
			List<Pin> p1 = ReturnObject<List<Pin>>("pin/GetPinsByHash", p).Result;
			return p1;
		}
		public static bool DbSavePin(Pin p)
		{
			bool p1 = ReturnObject<bool>("pin/DbSavePin", p).Result;
			return p1;
		}


	}
}
