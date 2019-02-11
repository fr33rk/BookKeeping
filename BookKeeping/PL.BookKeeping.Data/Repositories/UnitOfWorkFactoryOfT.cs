using Microsoft.EntityFrameworkCore;
using PL.BookKeeping.Infrastructure.Data;
using PL.Logger;
using System;
using Microsoft.Extensions.Logging;
using PL.BookKeeping.Infrastructure;
using PL.BookKeeping.Infrastructure.Services;

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
		private readonly ILoggerFactory mLoggerFactory;
		private readonly ISettingsService<Settings> mSettingsService;

		public UnitOfWorkFactoryOfT(ILogFile logFile, IDbConnectionFactory dbConnectionFactory,
			ILoggerFactory loggerFactory, ISettingsService<Settings> settingsService)
		{
			mLogFile = logFile;
			mDbConnectionFactory = dbConnectionFactory;
			mSettingsService = settingsService;
			mLoggerFactory = loggerFactory;
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
			var context = (TContext)Activator.CreateInstance(typeof(TContext), 
				mDbConnectionFactory.Create(), mLoggerFactory, mSettingsService);
			var uow = new UnitOfWork(context, mLogFile);

			//Set the current user.
			//var repository = uow.GetRepository<User>();
			//uow.SetCurrentUser(repository.FirstOrDefault());
			return uow;
		}
	}
}