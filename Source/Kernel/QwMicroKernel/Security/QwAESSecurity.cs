using System;
using System.Security.Cryptography;
using System.Text;

namespace QwMicroKernel.Security
{
    public class QwAESSecurity : QwBaseSecurity
    {
        protected override byte[] DoEncrypt(string key, byte[] data)
        {
            byte[] keyBuffer = Encoding.ASCII.GetBytes(key);
            byte[] dataBuffer = data;

            Aes aes = Aes.Create("AES");
            if (aes != null)
            {
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = keyBuffer;
                ICryptoTransform cTransform = aes.CreateEncryptor();
                byte[] cryptData = cTransform.TransformFinalBlock(dataBuffer, 0, dataBuffer.Length);
                string HexCryptString = Hex2To16(cryptData);

                return Encoding.UTF8.GetBytes(HexCryptString);
            }
            return null;
        }

        protected override byte[] DoDecrypt(string key, byte[] data)
        {
            byte[] keyBuffer = Encoding.ASCII.GetBytes(key);
            Aes aes = Aes.Create("AES");
            if (aes != null)
            {
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = keyBuffer;
                ICryptoTransform cTransform = aes.CreateDecryptor();

                string encryptedString = Encoding.UTF8.GetString(data);
                byte[] source = Hex16To2(encryptedString);
                byte[] originalSrouceData = cTransform.TransformFinalBlock(source, 0, source.Length);

                return originalSrouceData;
            }
            return null;
        }

        /// <summary>  
        /// 2进制转16进制  
        /// </summary>  
        private static String Hex2To16(Byte[] bytes)
        {
            String hexString = String.Empty;
            Int32 iLength = 65535;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                if (bytes.Length < iLength)
                {
                    iLength = bytes.Length;
                }

                for (int i = 0; i < iLength; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }

        /// <summary>  
        /// 16进制转2进制  
        /// </summary>  
        private static Byte[] Hex16To2(String hexString)
        {
            if ((hexString.Length % 2) != 0)
            {
                hexString += " ";
            }
            Byte[] returnBytes = new Byte[hexString.Length / 2];
            for (Int32 i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
    }
}
