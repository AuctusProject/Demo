using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Contracts
{
    public enum TransactionStatus
    {
        NotSent = 0,
        Pending = 1,
        Completed = 2,
        AutoRecovery = 3
    }
}
