using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBPAPI
{
	internal class Extensions
	{

		public static string GetPubKeyFromPrivKey(string sPrivKey, bool fTestNet)
		{
			string sPubKey = NBitcoin.Crypto.BBPTransaction.GetPubKeyFromPrivKey(fTestNet, sPrivKey);
			return sPubKey;
		}



	}
}
