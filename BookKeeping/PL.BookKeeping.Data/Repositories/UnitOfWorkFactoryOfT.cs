using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;

namespace PL.BookKeeping.Data.Repositories
{
    /// <summary>
    /// Implementation of a EntityFramework-based unit-of-work factory.
    /// </summary>
    public class UnitOfWorkFactoryOfT<TContext> : IUnitOfWorkFactory
        where TContext : DbContext
    {
        /// <summary>
        /// Initializes this unit-of-work factory.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Creates a new <see cref="IUnitOfWork"/>.
        /// </summary>
        /// <returns>The unit of work.</returns>
        public virtual IUnitOfWork Create()
        {
            TContext context = Activator.CreateInstance<TContext>();
            var uow = new UnitOfWork(context);

            //Set the current user.
            var repository = uow.GetRepository<User>();
            uow.SetCurrentUser(repository.FirstOrDefault());
            return uow;
        }
    }
}
