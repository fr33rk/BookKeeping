using System;

namespace PL.Logger
{
    public interface ILogFile
    {
        event EventHandler<LogEventArgs> OnLog;

        void Critical(string message);
        void Debug(string message);
        void Error(string message);
        void Info(string message);
        void Warning(string message);
        void WriteLogEnd();
        void WriteLogStart();
    }
}