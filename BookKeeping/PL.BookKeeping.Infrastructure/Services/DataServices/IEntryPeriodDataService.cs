using PL.BookKeeping.Entities;
using System.Collections.Generic;

namespace PL.BookKeeping.Infrastructure.Services.DataServices
{
    public interface IEntryPeriodDataService : IBaseTraceableObjectDataService<EntryPeriod>
    {
        IList<EntryPeriod> GetByEntryAndYear(Entry entry, int year);
    }
}
