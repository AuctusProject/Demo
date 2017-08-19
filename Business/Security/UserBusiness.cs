using Auctus.DataAccess.Contracts;
using Auctus.DataAccess.Security;
using Auctus.DomainObjects.Security;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Auctus.Business.Security
{
    public class UserBusiness : BaseBusiness<User, UserData>
    {
        public User Login(string login, string password)
        {
         
            byte[] data = Encoding.ASCII.GetBytes(password);
            data = SHA256.Create().ComputeHash(data);
            string passwordHash = Encoding.ASCII.GetString(data);
            
            return Data.Login(login, passwordHash);
        }
    }
}
