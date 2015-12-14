using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PL.BookKeeping.Business.Services.DataServices
{
	public class ProcessingRuleDataService : BaseTraceableObjectDataServiceOfT<ProcessingRule>, IProcessingRuleDataService

	{
		public ProcessingRuleDataService(IUnitOfWorkFactory uowFactory, IAuthorizationService authorizationService)
			: base(uowFactory, authorizationService)
		{
			
		}

        public IList<ProcessingRule> GetAllSorted()
        {
            using (var unitOfWork = this.mUOWFactory.Create())
            {
                var repository = unitOfWork.GetRepository<ProcessingRule>();
                return CompleteQry(repository.GetQuery(true))
                    .OrderBy(pr => pr.Priority)
                    .ToList();
            }
        }

        public override IQueryable<ProcessingRule> CompleteQry(IQueryable<ProcessingRule> query)
        {
            return base.CompleteQry(query.Include(e => e.Entry));
        }
    }
}
