﻿using MySql.Data.Entity;
using PL.BookKeeping.Entities;
using System.Data.Common;
using System.Data.Entity;

namespace PL.BookKeeping.Data
{
	[DbConfigurationType(typeof(MySqlEFConfiguration))]
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
		public DataContext(DbConnection connection)
			: base(connection, true)
		{
			Configuration.LazyLoadingEnabled = false;
		}

		public DbSet<User> Users { get; set; }

		public DbSet<Transaction> Transactions { get; set; }

		public DbSet<Entry> Entries { get; set; }

		public DbSet<Period> Periods { get; set; }

		public DbSet<EntryPeriod> EntryPeriods { get; set; }

		public DbSet<ProcessingRule> ProcessingRules { get; set; }
	}
}