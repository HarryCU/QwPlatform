using System.Configuration;

namespace QwFramework.Helper
{
    public sealed class AppSettingHelper
    {
        public static string Query(string group, string key, string defaultValue = "")
        {
            return ConfigurationManager.AppSettings[string.Concat(group, ":", key)] ?? defaultValue;
        }
    }
}
