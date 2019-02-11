using Microsoft.Extensions.Logging;
using PL.Logger;
using System;

namespace PL.BookKeeping.Data.Logging
{
	public class LoggerWrapper : ILogger
	{
		private readonly ILogFile mLogFile;

		public LoggerWrapper(ILogFile logFile)
		{
			mLogFile = logFile;
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			var message = $"{eventId}: {state}";

			if (exception != null)
			{
				message = message + $" Exception: {exception}";
			}

			switch (logLevel)
			{
				case LogLevel.Trace:
				case LogLevel.Debug:
					mLogFile.Debug(message);
					break;

				case LogLevel.None:
				case LogLevel.Information:
					mLogFile.Info(message);
					break;

				case LogLevel.Warning:
					mLogFile.Warning(message);
					break;

				case LogLevel.Error:
				case LogLevel.Critical:
					mLogFile.Error(message);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
			}
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return true;
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			return null;
		}
	}
}