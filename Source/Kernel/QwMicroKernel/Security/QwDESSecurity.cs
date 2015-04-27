using System.Security.Cryptography;

namespace QwMicroKernel.Security
{
    public class QwDESSecurity : QwBaseDESSecurity
    {
        public QwDESSecurity()
            : base(new DESCryptoServiceProvider())
        {
        }
    }
}
