using PL.BookKeeping.Entities;
using System;
using System.Collections.Generic;

namespace PL.BookKeeping.Infrastructure.Services
{
    public interface IDataImporterService
    {
        event EventHandler<DataImportedEventArgs> OnDataProcessed;

        IList<Transaction> ImportFiles(IEnumerable<string> files);
    }
}