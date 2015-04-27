namespace QwMicroKernel.Security
{
    public abstract class QwBaseSecurity : IQwSecurity
    {
        public byte[] Encrypt(string key, byte[] data)
        {
            return DoEncrypt(key, data);
        }

        public byte[] Decrypt(string key, byte[] data)
        {
            return DoDecrypt(key, data);
        }

        protected abstract byte[] DoEncrypt(string key, byte[] data);
        protected abstract byte[] DoDecrypt(string key, byte[] data);
    }
}
