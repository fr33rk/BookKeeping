using PL.BookKeeping.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL.BookKeeping.Infrastructure.Services.DataServices
{
    public interface IEntryDataService : IBaseTraceableObjectDataService<Entry>
    {
        IList<Entry> GetRootEntries();
        IList<Entry> Get3rdLevelEntries();
    }
}
