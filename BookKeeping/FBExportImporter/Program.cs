using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PL.BookKeeping.Data;
using PL.BookKeeping.Entities;

namespace FBExportImporter
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			ImportData();
		}


		private class DataBaseContents
		{
#pragma warning disable CS0649 // Because the fields are created when deserializing from JSON.
			public IList<User> Users;
			public IList<Entry> Entries;
			public IList<ProcessingRule> Rules;
			public IList<Transaction> Transactions;
#pragma warning restore CS0649

		}

		private static void ImportData()
		{
			DataBaseContents data = JsonConvert.DeserializeObject<DataBaseContents>(File.ReadAllText(@"d:\Database.json"));

			var dataContext = new DataContext();

			foreach (var dataUser in data.Users)
			{
				dataContext.Users.Add(dataUser);
			}
			dataContext.SaveChanges();

			foreach (var dataEntry in data.Entries)
			{
				dataContext.Entries.Add(dataEntry);
			}

			foreach (var processingRule in data.Rules)
			{
				dataContext.ProcessingRules.Add(processingRule);
			}

			// Clear all references to other objects. The transaction need to be reprocessed after import.
			foreach (var dataTransaction in data.Transactions)
			{
				dataTransaction.EntryPeriodKey = null;
				dataContext.Transactions.Add(dataTransaction);
			}

			dataContext.SaveChanges();
		}
	}
}