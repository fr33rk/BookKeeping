using PL.BookKeeping.Infrastructure.Data;
using PL.Logger;
using System;
using System.Data.Entity;

namespace PL.BookKeeping.Data.Repositories
{
    /// <summary>
    /// Implementation of a EntityFramework-based unit-of-work factory.
    /// </summary>
    public class UnitOfWorkFactoryOfT<TContext> : IUnitOfWorkFactory
        where TContext : DbContext
    {
        private readonly ILogFile mLogFile;

        public UnitOfWorkFactoryOfT(ILogFile logFile)
        {
            mLogFile = logFile;
        }

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
            var context = Activator.CreateInstance<TContext>();
            var uow = new UnitOfWork(context, mLogFile);

            //Set the current user.
            //var repository = uow.GetRepository<User>();
            //uow.SetCurrentUser(repository.FirstOrDefault());
            return uow;
        }
    }
}