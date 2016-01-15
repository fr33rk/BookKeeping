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

        public IList<ProcessingRule> GetByEntry(Entry entry)
        {
            using (var unitOfWork = this.mUOWFactory.Create())
            {
                var repository = unitOfWork.GetRepository<ProcessingRule>();
                return CompleteQry(repository.GetQuery(true))
                    .Where(r => r.EntryKey == entry.Key)
                    .OrderBy(r => r.Priority)
                    .ToList();
            }
        }

        public override void Add(ProcessingRule entity)
        {
            // First add the entity to the database.
            //base.Add(entity);

            // Next update the priorities
            using (var unitOfWork = mUOWFactory.Create())
            {
                var repository = unitOfWork.GetRepository<ProcessingRule>();

                entity = AttachEntities(unitOfWork, entity);

                repository.ExecuteProcedure("UPDATE_RULE_PRIORITY", entity.Priority);

                unitOfWork.SaveChanges();

                repository.Add(entity);

                unitOfWork.SaveChanges();
            }
        }
    }
}