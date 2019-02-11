using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure;
using PL.BookKeeping.Infrastructure.Services;
using System;

namespace PL.BookKeeping.Data
{
	public class DataContext : DbContext
	{
		private readonly ILoggerFactory mLoggerFactory;
		private readonly ISettingsService<Settings> mSettingsService;

		/// <summary>Gets or sets the current user.</summary>
		/// <value>The current user.</value>
		public User CurrentUser { get; set; }

		/// <inheritdoc />
		/// <summary>The default constructor is required for database migrations. Should not
		/// be used in any other case.
		/// </summary>
		public DataContext()
		{
		}

		/// <inheritdoc />
		public DataContext(DbContextOptions connectionOptions,
			ILoggerFactory loggerFactory, ISettingsService<Settings> settingsService)
			: base(connectionOptions)
		{
			mLoggerFactory = loggerFactory;
			mSettingsService = settingsService;
		}

		public DbSet<User> Users { get; set; }

		public DbSet<Transaction> Transactions { get; set; }

		public DbSet<Entry> Entries { get; set; }

		public DbSet<Period> Periods { get; set; }

		public DbSet<EntryPeriod> EntryPeriods { get; set; }

		public DbSet<ProcessingRule> ProcessingRules { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			var systemUser = new User()
			{
				Key = 1,
				CreationDT = DateTime.Now,
				Name = "System user"
			};

			modelBuilder.Entity<User>().HasData(
					systemUser
				);

			modelBuilder.Entity<Entry>()
				.HasData(
					new Entry()
					{
						Key = 1,
						CreationDT = DateTime.Now,
						CreatorKey = systemUser.Key,
						Description = "Uitgaven"
					},
					new Entry()
					{
						Key = 2,
						CreationDT = DateTime.Now,
						CreatorKey = systemUser.Key,
						Description = "Inkomsten"
					}
				);

			modelBuilder.FinalizeModel();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (mSettingsService != null && mSettingsService.Settings.EnableEntityFrameworkLogging)
			{
				optionsBuilder.UseLoggerFactory(mLoggerFactory);
			}

			if (!optionsBuilder.IsConfigured)
			{
				// For migration only.
				optionsBuilder.UseMySQL($@"Server=localhost;Database=bookkeeping;Uid=bookkeeper;Password=books");
			}
		}
	}
}