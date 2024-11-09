using NLog;
using System;

namespace GüzellikMerkeziProjesi
{
    public static class LoggerService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static void LogInfo(string message)
        {
            logger.Info(message);
        }


        public static void LogWarning(string message)
        {
            logger.Warn(message);
        }


        public static void LogError(Exception ex)
        {
            logger.Error(ex, ex.Message);
        }


        public static void LogError(string message, Exception ex)
        {
            logger.Error(ex, message);
        }


        public static void LogDebug(string message)
        {
            logger.Debug(message);
        }
    }
}
