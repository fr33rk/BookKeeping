using PL.BookKeeping.Entities;
using System.Collections.Generic;
using System;

namespace PL.BookKeeping.Infrastructure.Services.DataServices
{
	public interface IEntryPeriodDataService : IBaseTraceableObjectDataService<EntryPeriod>
	{
		IList<EntryPeriod> GetByEntryAndYear(Entry entry, int year);

		IList<EntryPeriod> GetByYear(int year);

		void ReCalculateTotalAmounts(DateTime periodStart, DateTime periodEnd);

		void Delete(Entry ofEntry, int afterYear);
	}
}