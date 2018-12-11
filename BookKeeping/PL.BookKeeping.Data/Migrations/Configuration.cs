using PL.BookKeeping.Entities;
using System.Linq;

namespace PL.BookKeeping.Data.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public sealed class Configuration : DbMigrationsConfiguration<DataContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
			ContextKey = "PL.BookKeeping.Data.DataContext";
		}

		protected override void Seed(DataContext context)
		{
			//  This method will be called after migrating to the latest version.

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method
			//  to avoid creating duplicate seed data. E.g.
			//
			//    context.People.AddOrUpdate(
			//      p => p.FullName,
			//      new Person { FullName = "Andrew Peters" },
			//      new Person { FullName = "Brice Lambson" },
			//      new Person { FullName = "Rowan Miller" }
			//    );
			//

			var mSystemUser = new User() { CreationDT = DateTime.Now, Name = "System user" };

			if (!context.Users.Any())
			{
				context.Users.AddOrUpdate(
					mSystemUser
				);
			}

			if (!context.Entries.Any())
			{
				context.Entries.AddOrUpdate(
					new Entry() { CreationDT = DateTime.Now, Creator = mSystemUser, Description = "Uitgaven" },
					new Entry() { CreationDT = DateTime.Now, Creator = mSystemUser, Description = "Inkomsten" }
				);
			}
		}
	}
}