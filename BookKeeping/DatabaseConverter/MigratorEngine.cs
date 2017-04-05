using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PL.BookKeeping.Entities;

namespace DatabaseConverter
{
	public class MigratorEngine
	{
		private class DataBaseContents
		{
			public IList<User> Users;
			public IList<Entry> Entries;
		}

		public void ExportToFile()
		{
			var mainUser = new User()
			{
				CreationDT = DateTime.Now,
				Creator = null,
				CreatorKey = null,
				Key = 1,
				Name = "Freerk de Leeuw"
			};

			var expenses = new Entry()
			{
				CreationDT = DateTime.Now,
				Creator = mainUser,
				CreatorKey = mainUser.Key,
				Description = "Uitgaven",
				Key = 1,
				ParentEntry = null,
				ParentEntryKey = null
			};

			var income = new Entry()
			{
				CreationDT = DateTime.Now,
				Creator = mainUser,
				CreatorKey = mainUser.Key,
				Description = "Inkomsten",
				Key = 2,
				ParentEntry = null,
				ParentEntryKey = null
			};

			var gadgets = new Entry()
			{
				CreationDT = DateTime.Now,
				Creator = mainUser,
				CreatorKey = mainUser.Key,
				Description = "Gadgets",
				Key = 3,
				ParentEntry = expenses,
				ParentEntryKey = expenses.Key
			};

			var users = new List<User> { mainUser };
			var entries = new List<Entry> { expenses, income, gadgets };

			var content = new DataBaseContents()
			{
				Users = users,
				Entries = entries
			};

			var testOutput = JsonConvert.SerializeObject(content);

			EngineLog?.Invoke(this, new EngineLogEventArgs(testOutput));
		}

		public class EngineLogEventArgs : EventArgs
		{
			public EngineLogEventArgs(string output)
			{
				Output = output;
			}

			public string Output { get; private set; }
		}

		public EventHandler<EngineLogEventArgs> EngineLog;

		public void ImportFromFile(string file)
		{
			var content = JsonConvert.DeserializeObject<DataBaseContents>(file);

			var output = content.Entries.Aggregate(string.Empty, (current, contentEntry) => current + (contentEntry.Description + ","));

			EngineLog?.Invoke(this, new EngineLogEventArgs(output));
		}
	}
}