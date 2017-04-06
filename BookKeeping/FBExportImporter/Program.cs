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
			public IList<User> Users;
			public IList<Entry> Entries;
			public IList<ProcessingRule> Rules;
			public IList<Transaction> Transactions;
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

			dataContext.SaveChanges();
		}
	}
}