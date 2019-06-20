using System;
using System.Collections.Generic;
using System.Text;

namespace Lavalink4NET.Tests
{
    internal class NullLogger : ILogger
    {
        public void Log(object source, string message, LogLevel level = LogLevel.Information, Exception exception = null)
        {
            throw new NotImplementedException();
        }
    }
}