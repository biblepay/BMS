using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;

namespace BiblePay.BMS.DSQL
{
    public class ERC712Authenticator
    {
        private readonly Eip712TypedDataSigner _signer = new Eip712TypedDataSigner();

        //Message types for easier input
        [Struct("BiblePayAuthenticator")]
        public class BiblePayAuthenticator
        {
            [Parameter("tuple", "from", 1, "Person")]
            public Person From { get; set; }
            [Parameter("string", "contents", 3)]
            public string Contents { get; set; }
        }
        [Struct("Person")]
        public class Person
        {
            [Parameter("string", "name", 1)]
            public string Name { get; set; }
        }
        //The generic EIP712 Typed schema defintion for this message
        public TypedData<Domain> GetAuthTypedDefinition()
        {

            return new TypedData<Domain>
            {

                Domain = new Domain
                {
                    ChainId = 137,
                    Name = "BiblePay Unchained",
                    VerifyingContract = "0x0",
                    Version = "1",

                },
                Types = MemberDescriptionFactory.GetTypesMemberDescription(typeof(Domain), typeof(BiblePayAuthenticator), typeof(Person)),
                PrimaryType = nameof(BiblePayAuthenticator),
            };
        }

        public bool VerifyERC712Signature(string sUserSig, string sERC20Address)
        {
            if (sUserSig == "")
                return false;

            var typedData = GetAuthTypedDefinition();
            var a = new BiblePayAuthenticator
            {
                From = new Person
                {
                    Name = "BiblePay Unchained",
                },
                Contents = "Please sign this message to log into BiblePay Unchained."
            };
            typedData.Domain.ChainId = 0;
            if (false)
            {
                // This is if we want to actually sign a wallet tx with nethereum
                var key = new EthECKey("0x0");
                var signature = _signer.SignTypedDataV4(a, typedData, key);
            }
            // In our case we just check the sig for authentication purposes here
            string addressRecovered = _signer.RecoverFromSignatureV4(a, typedData, sUserSig);
            bool fEquals = sERC20Address.ToLower() == addressRecovered.ToLower();

            return fEquals;
        }

    }

}
