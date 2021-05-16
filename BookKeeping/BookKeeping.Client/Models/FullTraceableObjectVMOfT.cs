using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PL.BookKeeping.Entities.Traceability;

namespace BookKeeping.Client.Models
{
    public class FullTraceableObjectVMOfT<TEntity, TEntityVM> : BaseTraceableObjectVMOfT<TEntity, TEntityVM>
        where TEntity : FullTraceableObject
    {
        public Guid Id { get; set; }
    }
}
