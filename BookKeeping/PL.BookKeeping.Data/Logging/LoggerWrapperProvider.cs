using Microsoft.Extensions.Logging;
using PL.Logger;

namespace PL.BookKeeping.Data.Logging
{
	public class LoggerWrapperProvider : ILoggerProvider
	{
		private readonly ILogFile mLogFile;

		public LoggerWrapperProvider(ILogFile logFile)
		{
			mLogFile = logFile;
		}

		public void Dispose()
		{
			// Nothing to do here. This class does not own the mLogFile.
		}

		public ILogger CreateLogger(string categoryName)
		{
			return new LoggerWrapper(mLogFile);
		}
	}
}