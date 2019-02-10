using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PL.BookKeeping.Entities;
using System;
using System.Diagnostics;

namespace PL.BookKeeping.Data
{
	// Todo Fdl: Fix logging
	public class MyLogger : ILogger
	{
		public IDisposable BeginScope<TState>(TState state)
		{
			return null;
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return true;
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (exception == null)
			{
				Debug.WriteLine($"{logLevel} - {eventId}:{state}.");
			}
			else
			{
				Debug.WriteLine($"{logLevel} - {eventId}:{state}. Exception: {exception}");
			}
		}
	}

	public class MyLoggerProvider : ILoggerProvider
	{
		public ILogger CreateLogger(string categoryName)
		{
			return new MyLogger();
		}

		public void Dispose()
		{
		}
	}

	public class DataContext : DbContext
	{
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
		public DataContext(DbContextOptions connectionOptions)
			: base(connectionOptions)
		{
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
			optionsBuilder.UseLoggerFactory(new LoggerFactory(new[] { new MyLoggerProvider() }));

			if (!optionsBuilder.IsConfigured)
			{
				// For migration only.
				optionsBuilder.UseMySQL($@"Server=localhost;Database=bookkeeping;Uid=bookkeeper;Password=books");
			}
		}
	}
}