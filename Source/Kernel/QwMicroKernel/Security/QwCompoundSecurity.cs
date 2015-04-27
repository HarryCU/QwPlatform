using System.Collections.Generic;
using System.Linq;

namespace QwMicroKernel.Security
{
    public class QwCompoundSecurity : QwBaseSecurity
    {
        private readonly List<IQwSecurity> _securities;

        public QwCompoundSecurity()
        {
            _securities = new List<IQwSecurity>(1);
        }

        public void Add(IQwSecurity security)
        {
            _securities.Add(security);
        }

        protected override byte[] DoEncrypt(string key, byte[] data)
        {
            return _securities.Aggregate(data, (current, security) => security.Encrypt(key, current));
        }

        protected override byte[] DoDecrypt(string key, byte[] data)
        {
            return _securities.Aggregate(data, (current, security) => security.Decrypt(key, current));
        }
    }
}
