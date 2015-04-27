using System;
using System.IO;

namespace QwFramework.Helper
{
    public sealed class FileHelper
    {
        public static string GetCurrent(string path)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }
    }
}
