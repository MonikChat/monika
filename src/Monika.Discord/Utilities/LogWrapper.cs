using System;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Logging;

namespace Monika.Utilities
{
    static class LogWrapper
    {
        public static Func<LogMessage, Task> WrapLogger(ILogger logger)
        {
            return (message) =>
            {
                switch (message.Severity)
                {
                    case LogSeverity.Critical:
                        logger.LogCritical(message.Exception,
                            "{Source}: {Message}", message.Source,
                            message.Message);
                        break;
                    case LogSeverity.Error:
                        logger.LogError(message.Exception,
                            "{Source}: {Message}", message.Source,
                            message.Message);
                        break;
                    case LogSeverity.Warning:
                        logger.LogWarning(message.Exception,
                            "{Source}: {Message}", message.Source,
                            message.Message);
                        break;
                    case LogSeverity.Info:
                        logger.LogInformation(message.Exception,
                            "{Source}: {Message}", message.Source,
                            message.Message);
                        break;
                    case LogSeverity.Verbose:
                        logger.LogDebug(message.Exception,
                            "{Source}: {Message}", message.Source,
                            message.Message);
                        break;
                    case LogSeverity.Debug:
                        logger.LogTrace(message.Exception,
                            "{Source}: {Message}", message.Source,
                            message.Message);
                        break;
                }

                return Task.CompletedTask;
            };
        }
    }
}