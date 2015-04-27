using System.Security.Cryptography;

namespace QwMicroKernel.Security
{
    public class QwRSASecurityKey
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }

    public class QwRSASecurity : QwBaseSecurity
    {
        public const int KeySize = 1024;

        protected override byte[] DoEncrypt(string key, byte[] data)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(KeySize);
            //将公钥导入到RSA对象中，准备加密；
            rsa.FromXmlString(key);
            //对数据data进行加密，并返回加密结果；
            //第二个参数用来选择Padding的格式
            return rsa.Encrypt(data, false);
        }

        protected override byte[] DoDecrypt(string key, byte[] data)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(KeySize);
            //将私钥导入RSA中，准备解密；
            rsa.FromXmlString(key);
            //对数据进行解密，并返回解密结果；
            return rsa.Decrypt(data, false);
        }

        public static QwRSASecurityKey CreateKey()
        {
            //声明一个RSA算法的实例，由RSACryptoServiceProvider类型的构造函数指定了密钥长度为1024位
            //实例化RSACryptoServiceProvider后，RSACryptoServiceProvider会自动生成密钥信息。
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(KeySize);
            var key = new QwRSASecurityKey();
            //将RSA算法的公钥导出到字符串PublicKey中，参数为false表示不导出私钥
            key.PublicKey = rsaProvider.ToXmlString(false);
            //将RSA算法的私钥导出到字符串PrivateKey中，参数为true表示导出私钥
            key.PrivateKey = rsaProvider.ToXmlString(true);
            return key;
        }

        public static byte[] Sign(string key, byte[] data)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(KeySize);
            //导入私钥，准备签名
            rsa.FromXmlString(key);
            //将数据使用MD5进行消息摘要，然后对摘要进行签名并返回签名数据
            return rsa.SignData(data, "MD5");
        }

        public static bool Verify(string key, byte[] data, byte[] signature)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(KeySize);
            //导入公钥，准备验证签名
            rsa.FromXmlString(key);
            //返回数据验证结果
            return rsa.VerifyData(data, "MD5", signature);
        }
    }
}
