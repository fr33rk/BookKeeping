using PL.BookKeeping.Entities;
using System.Collections.Generic;

namespace PL.BookKeeping.Infrastructure.Services.DataServices
{
    public interface IProcessingRuleDataService : IBaseTraceableObjectDataService<ProcessingRule>
    {
        IList<ProcessingRule> GetAllSorted();
    }
    
}
