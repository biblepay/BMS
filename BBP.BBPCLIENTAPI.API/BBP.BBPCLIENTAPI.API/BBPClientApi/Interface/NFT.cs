using BMSCommon.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BBPAPI.Interface.Core;

namespace BBPAPI.Interface
{
	public static class NFTLogic
	{

		public async static Task<DACResult> Save(NFTBuy m)
		{
			DACResult r = await ReturnObject<DACResult>("nft/Save", m);
			return r;
		}

		public async static Task<NFT> GetNFT(NFTSearch m)
		{
			NFT r = await ReturnObject<NFT>("nft/GetNFT", m);
			return r;
		}

		public async static Task<List<NFT>> GetListOfNFTs(NFTSearch m)
		{
			List<NFT> r = await ReturnObject<List<NFT>>("nft/GetListOfNFTs", m);
			return r;
		}
		public async static Task<DACResult> BuyNFT(NFTBuy m)
		{
			DACResult r = await ReturnObject<DACResult>("nft/BuyNFT", m);
			return r;
		}
	}
}
