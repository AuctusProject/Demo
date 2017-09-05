using Auctus.DataAccess.Accounts;
using Auctus.DomainObjects.Accounts;
using Auctus.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business.Accounts
{
    public class WalletBusiness : BaseBusiness<Wallet, WalletData>
    {
        public WalletBusiness(ILoggerFactory loggerFactory, Cache cache) : base(loggerFactory, cache) { }

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
