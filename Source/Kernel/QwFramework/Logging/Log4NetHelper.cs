using System.Collections.Generic;
using System.IO;
using System.Text;
using QwMicroKernel.Configuration;
using StringBuilder = QwMicroKernel.Text.StringBuilder;

namespace QwFramework.Logging
{
    internal sealed class Log4NetHelper
    {
        private static readonly string Log4NetLoggerTemplate = string.Join("\r\n", new List<string>()
        {
            "<logger name=\"{0}\">",
            "  <level value=\"{1}\" />",
            "  <appender-ref ref=\"{2}\" />",
            "</logger>",
            "<appender name=\"{2}\" type=\"log4net.Appender.RollingFileAppender, log4net\">",
            "  <file value=\"GwLogs/{3}/\"/>",
            "  <appendToFile value=\"true\"/>",
            "  <rollingStyle value=\"Date\"/>",
            "  <param name=\"StaticLogFileName\" value=\"false\"/>",
            "  <datePattern value=\"yyyyMMddHH.qwl\"/>",
            "  <layout type=\"log4net.Layout.PatternLayout,log4net\">",
            "    <param name=\"ConversionPattern\" value=\"%d{{ABSOLUTE}} %c{{1}}(%L) [%-5p] - %m%n\"/>",
            "  </layout>",
            "  <lockingModel type=\"log4net.Appender.FileAppender+MinimalLock\" />",
            "</appender>"
        });

        private static string CreateLoggerString(LoggerInfo loggerInfo)
        {
            return CreateLoggerString(loggerInfo.Name, loggerInfo.Level.ToUpper(), loggerInfo.Appender, loggerInfo.Dir);
        }

        private static string CreateLoggerString(string loggerName, string levelName, string appenderName, string logDirName)
        {
            return string.Format(Log4NetLoggerTemplate, loggerName, levelName, appenderName, logDirName);
        }

        private static IEnumerable<LoggerInfo> LoadLogger(string fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
            {
                var protperties = PropertiesParser.LoadFromFileResource(fileName);

                var logModules = protperties.GetGroup("log.module");

                foreach (var moduleKey in logModules.AllKeys)
                {
                    yield return new LoggerInfo()
                    {
                        Name = moduleKey,
                        Level = logModules[moduleKey]
                    };
                }
            }
        }

        public static Stream Configure(string fileName)
        {
            var log4Net = new StringBuilder(1024);
            log4Net.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            log4Net.AppendLine("<configuration>");
            log4Net.AppendLine("  <configSections>");
            log4Net.AppendLine("    <section name=\"log4net\" type=\"log4net.Config.Log4NetConfigurationSectionHandler, log4net\"/>");
            log4Net.AppendLine("  </configSections>");
            log4Net.AppendLine("  <log4net>");

            // ALL
            log4Net.Append(CreateLoggerString("GwALL", "ALL", "ALLAppender", "System"));

            foreach (var loggerInfo in LoadLogger(fileName))
            {
                log4Net.Append(CreateLoggerString(loggerInfo));
            }

            log4Net.AppendLine("  </log4net>");
            log4Net.AppendLine("</configuration>");

            return new MemoryStream(Encoding.UTF8.GetBytes(log4Net.ToString()));
        }

        class LoggerInfo
        {
            public string Name;

            public string Level;

            public string Appender
            {
                get { return string.Concat(Name, "Appender"); }
            }

            public string Dir
            {
                get { return Name; }
            }
        }
    }
}
