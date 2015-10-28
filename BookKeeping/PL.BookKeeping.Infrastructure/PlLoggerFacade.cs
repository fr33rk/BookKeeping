using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using PL.Logger;
using Prism.Logging;

namespace PL.BookKeeping.Infrastructure
{
    public class PlLoggerFacade : ILoggerFacade
    {
        private IUnityContainer mContainer;

        public PlLoggerFacade(IUnityContainer container)
        {
            mContainer = container;
        }

        public void Log(string message, Category category, Priority priority)
        {
            ILogFile logFile = mContainer.Resolve<ILogFile>();

            switch (category)
            {
                case Category.Debug:
                    logFile.Debug(message);
                    break;
                case Category.Warn:
                    logFile.Warning(message);
                    break;
                case Category.Exception:
                    logFile.Error(message);
                    break;
                case Category.Info:
                    logFile.Info(message);
                    break;
            }
        }
    }
}
