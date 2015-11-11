using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PL.BookKeeping.Entities;

namespace PL.BookKeeping.Infrastructure.Services
{
    public interface IDataImporterService
    {
        event EventHandler<DataImportedEventArgs> OnDataProcessed;

        IList<Transaction> ImportFiles(IEnumerable<string> files);
    }
}
