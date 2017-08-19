using Auctus.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business
{
    public abstract class BaseBusiness<T, D> where D : BaseData<T> , new()
    {
        public D Data => new D();

        public IEnumerable<T> ListAll()
        {
            return Data.ListAll();
        }

        public void Insert(params object[] values)
        {
            Data.Insert(values);
        }

        public void DeleteById(Int32 id)
        {
            Data.DeleteById(id);
        }
    }
}
