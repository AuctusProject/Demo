using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Security
{
    public class User
    {
        public Int32 Id { get; set; }
        public String Login { get; set; }
        public String Password { get; set; }
        public String Email { get; set; }
    }
}
