using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL.BookKeeping.Infrastructure.Services
{
    public interface IDataImporterService
    {
        void ImportFiles(IEnumerable<string> files);
    }
}
