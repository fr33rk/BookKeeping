using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;

namespace PL.BookKeeping.Business.Services
{
	/// <summary>Exports all data from the database to a JSON file.
	/// </summary>
	/// <seealso cref="PL.BookKeeping.Infrastructure.Services.IDataExporterService" />
	public class DataExporterService : IDataExporterService
	{
		#region Definitions

		private readonly IAuthorizationService mAuthorizationService;
		private readonly IEntryDataService mEntryDataService;
		private readonly IProcessingRuleDataService mProcessingRuleDataService;
		private readonly ITransactionDataService mTransactionDataService;

		private class DataBaseContents
		{
#pragma warning disable 414 // Member are used in the JsonSerializer.
			public IList<User> Users;
			public IList<Entry> Entries;
			public IList<ProcessingRule> Rules;
			public IList<Transaction> Transactions;
#pragma warning restore 414
		}

		#endregion Definitions

		#region Constructors

		/// <summary>Initializes a new instance of the <see cref="DataExporterService"/> class.
		/// </summary>
		/// <param name="authorizationService">The authorization service.</param>
		/// <param name="entryDataService">The entry data service.</param>
		/// <param name="processingRuleDataService">The processing rule data service.</param>
		/// <param name="transactionDataService">The transaction data service.</param>
		public DataExporterService(IAuthorizationService authorizationService, IEntryDataService entryDataService, IProcessingRuleDataService processingRuleDataService,
			ITransactionDataService transactionDataService)
		{
			mAuthorizationService = authorizationService;
			mEntryDataService = entryDataService;
			mProcessingRuleDataService = processingRuleDataService;
			mTransactionDataService = transactionDataService;
		}

		#endregion Constructors

		/// <summary>Export the data in a JSON format to a file specified in fileName.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		public void Export(string fileName)
		{
			var database = new DataBaseContents
			{
				Users = new List<User> { mAuthorizationService.CurrentUser },
				Entries = mEntryDataService.GetAll(),
				Rules = mProcessingRuleDataService.GetAll(),
				Transactions = mTransactionDataService.GetAll()
			};

			// Clear all child entries to prevent circulair reference when exporting to JSON.
			foreach (var databaseEntry in database.Entries)
			{
				databaseEntry.ChildEntries?.Clear();
				databaseEntry.ParentEntry = null;
			}

			using (var file = File.CreateText(fileName))
			{
				var serializer = new JsonSerializer();
				serializer.Serialize(file, database);
			}
		}
	}
}