using System;
using System.Collections.Generic;
using System.Text;

namespace FryProxy.Logging
{
    public enum LogLevel
    {
        All,
        Verbose,
        Finest = Verbose,
        Trace,
        Finer = Trace,
        Debug,
        Fine = Debug,
        Info,
        Warn,
        Error,
        Fatal,
        Off
    }
}
