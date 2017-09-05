using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auctus.Web.Hubs
{
    [HubName("AuctusDemo")]
    public class AuctusDemoHub : Hub
    {   
        public override Task OnConnected()
        {
            return base.OnConnected();
        }
    }
}
