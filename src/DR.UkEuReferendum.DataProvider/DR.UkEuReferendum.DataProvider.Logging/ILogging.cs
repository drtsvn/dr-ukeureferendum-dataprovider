using System;

namespace DR.UkEuReferendum.DataProvider.Logging
{
    public interface ILogging
    {
        void LogException(Exception exception, string arg);
        void LogInformation(string message);
        bool Enabled { get; set; }
    }
}
