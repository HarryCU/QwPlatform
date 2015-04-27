using System;
using System.Globalization;

namespace QwMicroKernel
{
    public class Guard
    {
        public static void Against<TException>(bool assertion, string message) where TException : Exception
        {
            if (!assertion)
            {
                return;
            }

            Exception exception;
            try
            {
                exception = (TException)Activator.CreateInstance(typeof(TException), message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("{0} 是错误的异常类型!", typeof(TException).FullName), ex);
            }

            throw exception;
        }

        public static void Against<TException>(Func<bool> assertion, string message) where TException : Exception
        {
            Against<TException>(assertion(), message);
        }

        public static void AgainstNull(object value, string name)
        {
            if (value == null)
            {
                throw new NullReferenceException(string.Format(CultureInfo.CurrentCulture, "{0} 是空值!", name));
            }
        }

        public static void AgainstNullOrEmptyString(string value, string name)
        {
            AgainstNull(value, name);

            if (value.Length == 0)
                throw new Exception(string.Format(CultureInfo.CurrentCulture, "{0} 是空字符串!", name));
        }

        public static void AgainstReassignment(object variable, string name)
        {
            if (variable == null)
            {
                return;
            }

            throw new Exception(string.Format(CultureInfo.CurrentCulture, "{0} 是空值!", name));
        }
    }
}