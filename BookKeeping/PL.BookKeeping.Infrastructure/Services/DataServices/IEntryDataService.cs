using PL.BookKeeping.Entities;
using System.Collections.Generic;

namespace PL.BookKeeping.Infrastructure.Services.DataServices
{
	public interface IEntryDataService : IBaseTraceableObjectDataService<Entry>
	{
		IList<Entry> GetRootEntries();

		IList<Entry> Get3rdLevelEntries();

		IList<Entry> GetRootEntriesOfYear(int year);
	}
}