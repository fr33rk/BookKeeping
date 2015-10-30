using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;

namespace PL.BookKeeping.Business.Services.DataServices
{
	class ProcessingRuleDataService : BaseTraceableObjectDataServiceOfT<ProcessingRule>, IProcessingRuleDataService

	{
		public ProcessingRuleDataService(IUnitOfWorkFactory uowFactory, IAuthorizationService authorizationService)
			: base(uowFactory, authorizationService)
		{
			
		}
	}
}
