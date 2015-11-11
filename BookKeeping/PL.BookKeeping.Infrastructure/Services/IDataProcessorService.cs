using System.Collections.Generic;
using PL.BookKeeping.Entities;

namespace PL.BookKeeping.Infrastructure.Services
{
    public interface IDataProcessorService
    {
        void Process(IList<Transaction> transactions);
    }
}