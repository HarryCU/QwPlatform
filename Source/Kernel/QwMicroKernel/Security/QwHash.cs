using System.Security.Cryptography;
using System.Text;
using StringBuilder = QwMicroKernel.Text.StringBuilder;

namespace QwMicroKernel.Security
{
    public static class QwHash
    {
        // SHA1(160bit)
        public static string SHA1(byte[] b)
        {
            SHA1 sha1 = new SHA1Managed();
            return BytesToHexString(sha1.ComputeHash(b));
        }

        public static string SHA1(string szInput)
        {
            return SHA1(StringToBytes(szInput));
        }

        // SHA256(256bit)
        public static string SHA256(byte[] b)
        {
            SHA256 sha256 = new SHA256Managed();
            return BytesToHexString(sha256.ComputeHash(b));
        }

        public static string SHA256(string szInput)
        {
            return SHA256(StringToBytes(szInput));
        }

        // SHA384(384bit)
        public static string SHA384(byte[] b)
        {
            SHA384 sha384 = new SHA384Managed();
            return BytesToHexString(sha384.ComputeHash(b));
        }

        public static string SHA384(string szInput)
        {
            return SHA384(StringToBytes(szInput));
        }

        // SHA512(512bit)
        public static string SHA512(byte[] b)
        {
            SHA512 sha512 = new SHA512Managed();
            return BytesToHexString(sha512.ComputeHash(b));
        }

        public static string SHA512(string szInput)
        {
            return SHA512(StringToBytes(szInput));
        }


        // String To Bytes
        private static byte[] StringToBytes(string s)
        {
            return Encoding.ASCII.GetBytes(s);
        }

        // Bytes To Hex String
        private static string BytesToHexString(byte[] b)
        {
            if (b == null)
                return null;

            using (StringBuilder sb = new StringBuilder(b.Length * 2))
            {
                foreach (byte t in b)
                    sb.AppendFormat("{0:x2}", t);
                return sb.ToString();
            }
        }
    }
}
