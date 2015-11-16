using System;
using System.Collections.Generic;
using PL.BookKeeping.Entities;

namespace PL.BookKeeping.Infrastructure.Services.DataServices
{
    public interface IPeriodDataService : IBaseTraceableObjectDataService<Period>
    {
        /// <summary>Adds a new period to the database which can 'store' a transaction with transactionDate.</summary>
        /// <param name="transactionDate">The transaction date that must fit in the period.</param>
        /// <returns>The period added to the database.</returns>
        Period AddUsingTransactionDate(DateTime transactionDate);

        IList<int> GetAvailableYears();
    }
}
