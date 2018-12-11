using System;
using System.Data.Entity;
using PL.BookKeeping.Infrastructure.Data;
using PL.Logger;

namespace PL.BookKeeping.Data.Repositories
{
	/// <summary>
	/// Implementation of a EntityFramework-based unit-of-work factory.
	/// </summary>
	public class UnitOfWorkFactoryOfT<TContext> : IUnitOfWorkFactory
		where TContext : DbContext
	{
		private readonly ILogFile mLogFile;
		private readonly IDbConnectionFactory mDbConnectionFactory;

		public UnitOfWorkFactoryOfT(ILogFile logFile, IDbConnectionFactory dbConnectionFactory)
		{
			mLogFile = logFile;
			mDbConnectionFactory = dbConnectionFactory;
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
			var context = (TContext)Activator.CreateInstance(typeof(TContext), mDbConnectionFactory.Create());
			var uow = new UnitOfWork(context, mLogFile);

			//Set the current user.
			//var repository = uow.GetRepository<User>();
			//uow.SetCurrentUser(repository.FirstOrDefault());
			return uow;
		}
	}
}