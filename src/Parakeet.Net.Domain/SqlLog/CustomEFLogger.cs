using Microsoft.Extensions.Logging;
using System;

namespace Parakeet.Net.SqlLog
{
    //public class CustomEFLogger : ILogger
    //{
    //    private string _CategoryName = null;
    //    public CustomEFLogger(string categoryName)
    //    {
    //        this._CategoryName = categoryName;
    //    }

    //    public IDisposable BeginScope<TState>(TState state)
    //    {
    //        return null;
    //    }

    //    public bool IsEnabled(LogLevel logLevel)
    //    {
    //        return true;
    //    }

    //    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    //    {
    //        System.Diagnostics.Debug.WriteLine($"************************************************************");
    //        System.Diagnostics.Debug.WriteLine($"CustomEFLogger {_CategoryName} {logLevel} {eventId} {state} start");

    //        System.Diagnostics.Debug.WriteLine($"异常信息：{exception?.Message}");
    //        System.Diagnostics.Debug.WriteLine($"信息：{formatter.Invoke(state, exception)}");

    //        System.Diagnostics.Debug.WriteLine($"CustomEFLogger {_CategoryName} {logLevel} {eventId} {state} end");
    //        System.Diagnostics.Debug.WriteLine($"************************************************************");
    //    }
    //}
}
