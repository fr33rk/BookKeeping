using System.Collections.Generic;
using PL.BookKeeping.Entities;
using System;

namespace PL.BookKeeping.Infrastructure.Services
{
    public interface IDataProcessorService
    {
        void Process(IList<Transaction> transactions);

        /// <summary>Occurs when a transaction had been processed. </summary>
        event EventHandler<DataProcessedEventArgs> OnDataProcessed;

    }
}