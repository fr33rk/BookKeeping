using PL.BookKeeping.Entities;
using System;
using System.Collections.Generic;

namespace PL.BookKeeping.Infrastructure.Services.DataServices
{
	public interface ITransactionDataService : IBaseTraceableObjectDataService<Transaction>
	{
		new bool Add(Transaction transaction);

		IEnumerable<Transaction> GetByEntryPeriod(EntryPeriod entryPeriod);

		void ResetPeriodEntryLinks();

		IList<Transaction> GetOfPeriod(DateTime startDate, DateTime endDate);
	}
}