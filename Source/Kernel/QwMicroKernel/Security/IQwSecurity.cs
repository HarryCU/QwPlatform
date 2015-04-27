namespace QwMicroKernel.Security
{
    public interface IQwSecurity
    {
        byte[] Encrypt(string key, byte[] data);
        byte[] Decrypt(string key, byte[] data);
    }
}
