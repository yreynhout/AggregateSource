using System;
using System.Configuration;
using System.IO;

namespace AggregateSource.GEventStore.Framework
{
    static class EmbeddedEventStoreConfiguration
    {
        public static readonly bool RunWithLogging = GetRunWithLoggingOrDefaultFallback(ConfigurationManager.AppSettings["RunWithLogging"]);

        public static readonly string LogPath = GetLogPathOrDefaultFallback(ConfigurationManager.AppSettings["LogPath"]);

        public static readonly string StoragePath = GetStoragePathOrDefaultFallback(ConfigurationManager.AppSettings["StoragePath"]);

        static bool GetRunWithLoggingOrDefaultFallback(string runWithLogging)
        {
            bool result;
            if (string.IsNullOrEmpty(runWithLogging) || !bool.TryParse(runWithLogging, out result))
            {
                return false;
            }
            return result;
        }

        static string GetLogPathOrDefaultFallback(string logPath)
        {
            if (string.IsNullOrEmpty(logPath))
            {
                return Path.Combine(Path.GetTempPath(), "EventStore", "logs");
            }
            return logPath;
        }

        static string GetStoragePathOrDefaultFallback(string storagePath)
        {
            if (string.IsNullOrEmpty(storagePath))
            {
                return Path.Combine(Path.GetTempPath(), "EventStore");
            }
            return storagePath;
        }
    }

}
