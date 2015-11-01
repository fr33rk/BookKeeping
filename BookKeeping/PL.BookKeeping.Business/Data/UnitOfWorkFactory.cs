using PL.BookKeeping.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL.BookKeeping.Business.Data
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWork Create()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
