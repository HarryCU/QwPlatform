using System;

namespace QwMicroKernel.Security
{
    [Flags]
    public enum QwSecurityType
    {
        DES = 1,
        DES3 = 2,
        RSA = 4,
        AES = 8,
        All = DES | DES3 | RSA | AES
    }
}
