using Auctus.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Business
{
    public abstract class BaseBusiness<T, D> where D : BaseData<T>
    {
        public abstract D data { get; }

        public IEnumerable<T> ListAll()
        {
            return data.ListAll();
        }

        public void Insert(params object[] values)
        {
            data.Insert(values);
        }

        public void DeleteById(Int32 id)
        {
            data.DeleteById(id);
        }
    }
}
