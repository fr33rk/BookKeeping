using PL.BookKeeping.Entities;
using System.Collections.Generic;

namespace PL.BookKeeping.Infrastructure.Services.DataServices
{
    public interface IProcessingRuleDataService : IBaseTraceableObjectDataService<ProcessingRule>
    {
        IList<ProcessingRule> GetAllSorted();

        IList<ProcessingRule> GetByEntry(Entry entry);

        void SwapByPriority(ProcessingRule swapThis, ProcessingRule swapWithThat);
    }
}