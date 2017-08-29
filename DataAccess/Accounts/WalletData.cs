using Auctus.DomainObjects.Accounts;

namespace Auctus.DataAccess.Accounts
{
    public class WalletData : BaseData<Wallet>
    {
        public override string TableName => "Wallet";
    }
}
