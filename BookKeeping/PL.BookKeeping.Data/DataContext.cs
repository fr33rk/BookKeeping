using FirebirdSql.Data.FirebirdClient;
using PL.BookKeeping.Entities;
using System.Data.Entity;
using MySql.Data.MySqlClient;
using MySql.Data.Entity;

namespace PL.BookKeeping.Data
{
	[DbConfigurationType(typeof(MySqlEFConfiguration))]
	public class DataContext : DbContext
	{
		/// <summary>Gets or sets the current user.</summary>
		/// <value>The current user.</value>
		public User CurrentUser { get; set; }

		public DataContext()
			: base(new MySqlConnection(@"Server=Helium;Database=Bookkeeping;Uid=Bookkeeper;Password=books"), true)
		{
			this.Configuration.LazyLoadingEnabled = false;
		}

		public DbSet<User> Users { get; set; }

		public DbSet<Transaction> Transactions { get; set; }

		public DbSet<Entry> Entries { get; set; }

		public DbSet<Period> Periods { get; set; }

		public DbSet<EntryPeriod> EntryPeriods { get; set; }

		public DbSet<ProcessingRule> ProcessingRules { get; set; }
	}
}