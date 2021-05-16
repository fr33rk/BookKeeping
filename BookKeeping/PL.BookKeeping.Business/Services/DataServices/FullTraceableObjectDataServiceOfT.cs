using PL.BookKeeping.Entities.Traceability;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PL.BookKeeping.Business.Services.DataServices
{
    public class FullTraceableObjectDataServiceOfT<TEntity> : BaseTraceableObjectDataServiceOfT<TEntity>, IFullTraceableObjectDataService<TEntity>
        where TEntity : FullTraceableObject, new()
    {
        public FullTraceableObjectDataServiceOfT(IUnitOfWorkFactory uowFactory, IAuthorizationService authorizationService) 
            : base(uowFactory, authorizationService)
        {
        }

        public override bool Add(TEntity entity)
        {
            entity.State = FullTraceableObject.ObjectState.Active;
            entity.Action = FullTraceableObject.ObjectAction.Created;
            
            return base.Add(entity);
        }

        public override bool Update(TEntity entity)
        {
            if (!MakeHistoric(entity))
                return false;

            entity.State = FullTraceableObject.ObjectState.Active;
            entity.Action = FullTraceableObject.ObjectAction.Edited;
            return base.Add(entity);
        }

        public override bool Delete(TEntity entity)
        {
            if (!MakeHistoric(entity)) 
                return false;

            entity.State = FullTraceableObject.ObjectState.Deleted;
            entity.Action = FullTraceableObject.ObjectAction.Deleted;
            return base.Add(entity);
        }

        private bool MakeHistoric(TEntity entity)
        {
            var currentVersion = GetActiveById(entity.Id);
            currentVersion.State = FullTraceableObject.ObjectState.Historic;
            return base.Update(currentVersion);
        }

        public TEntity GetActiveById(int id, bool complete = false)
        {
            using (var unitOfWork = mUOWFactory.Create())
            {
                var repository = unitOfWork.GetRepository<TEntity>();
                var qry = repository.GetQuery()
                    .Where(e => e.Id == id && e.State == FullTraceableObject.ObjectState.Active);
                if (complete)
                {
                    qry = CompleteQry(qry);
                }

                return qry.SingleOrDefault();
            }
        }

        public IList<TEntity> GetActiveEntities(bool complete = false)
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> GetByState(FullTraceableObject.ObjectState state, bool complete = false)
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> GetHistory(int id, bool complete = false)
        {
            throw new NotImplementedException();
        }
    }
}