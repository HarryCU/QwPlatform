namespace QwMicroKernel.Security
{
    public sealed class QwSecurityFactory
    {
        public static IQwSecurity Create(QwSecurityType securityType)
        {
            var compound = new QwCompoundSecurity();

            if ((securityType & QwSecurityType.AES) == QwSecurityType.AES)
                compound.Add(new QwAESSecurity());

            if ((securityType & QwSecurityType.DES) == QwSecurityType.DES)
                compound.Add(new QwDESSecurity());

            if ((securityType & QwSecurityType.DES3) == QwSecurityType.DES3)
                compound.Add(new QwDES3Security());

            if ((securityType & QwSecurityType.RSA) == QwSecurityType.RSA)
                compound.Add(new QwRSASecurity());

            return compound;
        }
    }
}
