using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace QwMicroKernel.Security
{
    public abstract class QwBaseDESSecurity : QwBaseSecurity
    {
        private readonly SymmetricAlgorithm _provider;

        protected QwBaseDESSecurity(SymmetricAlgorithm algorithm)
        {
            _provider = algorithm;
        }

        public byte[] Key
        {
            get { return _provider.Key; }
            set { _provider.Key = value; }
        }

        public byte[] IV
        {
            get { return _provider.IV; }
            set { _provider.IV = value; }
        }

        private void MarkKey(string serialNumber)
        {
            SHA1 sha1 = new SHA1Managed();
            byte[] buffer = sha1.ComputeHash(Encoding.ASCII.GetBytes(serialNumber));
            byte[] key = new byte[8];
            byte[] iv = new byte[8];

            Array.Copy(buffer, 0, key, 0, 8);
            Array.Copy(buffer, 8, iv, 0, 8);

            _provider.Key = key;
            _provider.IV = iv;
        }

        protected override byte[] DoEncrypt(string key, byte[] data)
        {
            MarkKey(key);

            using (var ms = new MemoryStream())
            {
                using (var encStream = new CryptoStream(ms, _provider.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    encStream.Write(data, 0, data.Length);
                }
                ms.Position = 0;
                if (ms.CanSeek)
                    ms.Seek(0, SeekOrigin.Begin);

                byte[] buffer = ms.ToArray();
                return buffer;
            }
        }

        protected override byte[] DoDecrypt(string key, byte[] data)
        {
            MarkKey(key);

            using (var ms = new MemoryStream(data))
            {
                using (var encStream = new CryptoStream(ms, _provider.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (MemoryStream tmpStream = new MemoryStream())
                    {
                        int b;
                        while ((b = encStream.ReadByte()) != -1)
                        {
                            tmpStream.WriteByte((byte)b);
                        }

                        ms.Position = 0;
                        if (ms.CanSeek)
                            ms.Seek(0, SeekOrigin.Begin);

                        byte[] buffer = tmpStream.ToArray();
                        return buffer;
                    }
                }
            }
        }
    }
}
