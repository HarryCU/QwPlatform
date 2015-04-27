using System.Security.Cryptography;
using QwMicroKernel.Text;

namespace QwMicroKernel.Data.Implements
{
    internal static class Utils
    {
        internal static string GetUniqueIdentifier(int length)
        {
            int maxSize = length;
            char[] chars = new char[62];
            string a;
            a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            using (StringBuilder result = new StringBuilder(size))
            {
                foreach (byte b in data)
                {
                    result.Append(chars[b % (chars.Length - 1)]);
                }
                // Unique identifiers cannot begin with 0-9
                if (result[0] >= '0' && result[0] <= '9')
                {
                    return GetUniqueIdentifier(length);
                }
                return result.ToString();
            }
        }
    }
}
