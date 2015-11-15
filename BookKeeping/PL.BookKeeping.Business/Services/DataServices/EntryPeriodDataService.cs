using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL.BookKeeping.Business.Services.DataServices
{
    public class EntryPeriodDataService : BaseTraceableObjectDataServiceOfT<EntryPeriod>, IEntryPeriodDataService
    {
        #region Constructor(s)

        public EntryPeriodDataService(IUnitOfWorkFactory uowFactory, IAuthorizationService authorizationService)
            : base(uowFactory, authorizationService)
        {
        }

        #endregion Constructor(s)

        public override EntryPeriod AttachEntities(IUnitOfWork unitOfWork, EntryPeriod entity)
        {
            if (entity.Period != null)
            {
                var periods = unitOfWork.GetRepository<Period>();
                entity.Period = periods.FirstOrDefault(e => e.Key == entity.Period.Key);
            }
            if (entity.Entry != null)
            {
                var entries = unitOfWork.GetRepository<Entry>();
                entity.Entry = entries.FirstOrDefault(e => e.Key == entity.Entry.Key);
            }
            return base.AttachEntities(unitOfWork, entity);
        }

        /// <summary>Add include statements to 'query' so that referred objects are loaded
        /// together with the entity in one query to the database.
        /// </summary>
        /// <param name="query">The query where the include statements need to be added to.</param>
        /// <returns></returns>
        public override IQueryable<EntryPeriod> CompleteQry(IQueryable<EntryPeriod> query)
        {
            return base.CompleteQry(query.Include(e => e.Period)
                                         .Include(e => e.Entry));
        }

    }
}
