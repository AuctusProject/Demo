using Auctus.Business.Security;
using Auctus.DomainObjects.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Service
{
    public class SecurityServices
    {
        public User Login(string login, string password)
        {
            return new UserBusiness().Login(login, password);
        }

        public bool ForgotPassword(string email)
        {
            throw new NotImplementedException();
        }

        public bool LoginWithFacebook(string fbTokenHash)
        {
            throw new NotImplementedException();
        }
    }
}
