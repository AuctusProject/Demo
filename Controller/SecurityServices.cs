using Auctus.Business.Security;
using Auctus.DomainObjects.Security;
using Auctus.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Service
{
    public class SecurityServices : BaseServices
    {
        public SecurityServices(Cache cache) : base(cache) { }

        public User Login(string login, string password)
        {
            return UserBusiness.Login(login, password);
        }

        public bool ChangePassword(string newPassword, string validationToken)
        {
            throw new NotImplementedException();
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
