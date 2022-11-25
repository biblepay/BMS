using System;
using System.Collections.Generic;
using System.Text;
using static BMSCommon.Model;

namespace BMSCommon.Retired
{
    public static class ERC20Assets
    {
		public static List<ERCAsset> GetAssetList()
		{
			List<ERCAsset> l1 = new List<ERCAsset>();
			// Layer 1 ERC-20 Assets:
			l1.Add(new ERCAsset { Chain = "POLYGON", Symbol = "MATIC", ERCAddress = "0x0", ChainlinkAddress = "0xAB594600376Ec9fD91F8e885dADF0CE036862dE0", Price = 0 });
			l1.Add(new ERCAsset { Chain = "BSC", Symbol = "BSC", ERCAddress = "0x0", ChainlinkAddress = "0x0567F2323251f0Aab15c8dFb1967E4e8A7D42aeE", Price = 0 });
			l1.Add(new ERCAsset { Chain = "POLYGON", Symbol = "WETH", ERCAddress = "0x7ceb23fd6bc0add59e62ac25578270cff1b9f619", ChainlinkAddress = "0xF9680D99D6C9589e2a93a78A04A279e509205945", Price = 0 });
			// Layer 2 ERC-20 Assets:
			l1.Add(new ERCAsset { Chain = "BSC", Symbol = "CAKE", ERCAddress = "0x0e09fabb73bd3ade0a17ecc321fd13a19e81ce82", ChainlinkAddress = "0xB6064eD41d4f67e353768aA239cA86f4F73665a1", Price = 0 });
			l1.Add(new ERCAsset { Chain = "BSC", Symbol = "FIELD", ERCAddress = "0x04d50c032f16a25d1449ef04d893e95bcc54d747", Price = .003 });
			l1.Add(new ERCAsset { Chain = "ETH", Symbol = "SHIB", ERCAddress = "0x95ad61b0a150d79219dcf64e1e6cc01f0b64c4ce", Price = .00001081 });
			l1.Add(new ERCAsset { Chain = "POLYGON", Symbol = "SHIB", ERCAddress = "0x6f8a06447ff6fcf75d803135a7de15ce88c1d4ec", Price = .00001081 });
			l1.Add(new ERCAsset { Chain = "POLYGON", Symbol = "rETH", ERCAddress = "0x0266F4F08D82372CF0FcbCCc0Ff74309089c74d1", Price = 1000.00 });
			l1.Add(new ERCAsset { Chain = "ETH", Symbol = "wstETH", ERCAddress = "0x7f39C581F595B53c5cb19bD0b3f8dA6c935E2Ca0", Price = 1000.00 });
			// Wrapped ERC-20 Assets:
			l1.Add(new ERCAsset { Chain = "BSC", Symbol = "WBBP", ERCAddress = "0xcb1eec8630c5176611f72799853c3b7dbe4b8953", Price = 0 });
			l1.Add(new ERCAsset { Chain = "POLYGON", Symbol = "renDOGE", ERCAddress = "0xcE829A89d4A55a63418bcC43F00145adef0eDB8E", ChainlinkAddress = "0xbaf9327b6564454F4a3364C33eFeEf032b4b4444", Price = 0 });
			// Native Non ERC-20 Layer 1 Assets:
			l1.Add(new ERCAsset { Chain = "DOGE", Symbol = "DOGE", ERCAddress = "", Price = 0 });
			l1.Add(new ERCAsset { Chain = "BITCOIN", Symbol = "BTC", ERCAddress = "", Price = 0 });
			l1.Add(new ERCAsset { Chain = "DASH", Symbol = "DASH", ERCAddress = "", Price = 0 });
			return l1;
		}

	}
}
