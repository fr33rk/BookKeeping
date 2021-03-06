﻿using PL.Logger;
using Prism.Logging;

namespace PL.BookKeeping.Infrastructure
{
	public class PlLoggerFacade : ILoggerFacade
	{
		private readonly ILogFile mLogFile;

		public PlLoggerFacade(ILogFile logFile)
		{
			mLogFile = logFile;
		}

		public void Log(string message, Category category, Priority priority)
		{
			switch (category)
			{
				case Category.Debug:
					mLogFile.Debug(message);
					break;

				case Category.Warn:
					mLogFile.Warning(message);
					break;

				case Category.Exception:
					mLogFile.Error(message);
					break;

				case Category.Info:
					mLogFile.Info(message);
					break;
			}
		}
	}
}