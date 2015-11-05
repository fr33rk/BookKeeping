using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL.BookKeeping.Infrastructure.Services
{
    public interface IDataImporterService
    {
        event EventHandler<DataImportedEventArgs> OnDataProcessed;

        event EventHandler OnDataProcessedDone;

        void ImportFiles(IEnumerable<string> files);
    }
}
