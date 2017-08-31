using Auctus.DataAccess.Accounts;
using Auctus.DomainObjects.Accounts;
using Auctus.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Accounts
{
    public class WalletBusiness : BaseBusiness<Wallet, WalletData>
    {
        public WalletBusiness(Cache cache) : base(cache) { }

        internal Wallet Create()
        {
            var password = Util.NotShared.Security.GenerateEncryptedPassword();
            EthereumProxy.Wallet wallet = EthereumProxy.EthereumManager.CreateAccount(password);
            DomainObjects.Accounts.Wallet domainWallet = new DomainObjects.Accounts.Wallet()
            {
                Address = wallet.Address,
                CreationDate = DateTime.UtcNow,
                File = wallet.File,
                FileName = wallet.FileName,
                Password = password
            };
            Insert(domainWallet);
            return domainWallet;
        }
    }
}
