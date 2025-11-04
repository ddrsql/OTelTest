using Abp.Dependency;
using log4net;
using log4net.Appender;
using log4net.Core;
using Microsoft.Extensions.Logging;
using System.Reflection.Emit;

namespace AbpFramework.OTel
{
    public class LoggerAppender : TraceAppender
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            var loggerFactory = LogManager.GetRepository().Properties["ILoggerFactory"] as ILoggerFactory;
            var log = loggerFactory.CreateLogger(loggingEvent.LoggerName);

            var level = loggingEvent.Level;
            var message = loggingEvent.MessageObject.ToString();
            if (loggingEvent.ExceptionObject != null)
            {
                message = message + " " + loggingEvent.ExceptionObject.ToString();
            }

            if (level >= Level.Error)
            {
                log.LogError(message);
            }
            else if (level >= Level.Warn)
            {
                log.LogWarning(message);
            }
            else if (level >= Level.Info)
            {
                log.LogInformation(message);
            }
            else
            {
                log.LogTrace(message);
            }
        }
    }
}
