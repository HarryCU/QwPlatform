using System.Security.Cryptography;

namespace QwMicroKernel.Security
{
    public class QwDES3Security : QwBaseDESSecurity
    {
        public QwDES3Security()
            : base(new TripleDESCryptoServiceProvider())
        {
        }
    }
}
