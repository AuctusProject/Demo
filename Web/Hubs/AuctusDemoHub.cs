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
          //  string userid = Context.User.Identity.Name;
          //  Clients.ToString();
          //  Groups.Add(Context.ConnectionId, userid);
            return base.OnConnected();
        }
    }
}
