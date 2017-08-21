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
            return Data.Select<T>();
        }

        public void Insert(T obj)
        {
            Data.Insert(obj);
        }

        public void Update(T obj)
        {
            Data.Update(obj);
        }

        public void Delete(T obj)
        {
            Data.Delete(obj);
        }
    }
}
