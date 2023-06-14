using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Common
{
    public class RSAUtils
    {
        public static string SignMessage(string message, string privateKey)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);

                // Compute signature
                var signatureBytes = rsa.SignData(messageBytes, new SHA256CryptoServiceProvider());

                // Convert signature to base64 string
                var signature = Convert.ToBase64String(signatureBytes);

                return signature;
            }
        }

        public static bool VerifySignature(string message, string signature, string publicKey)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var signatureBytes = Convert.FromBase64String(signature);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);

                // Verify signature
                var isValid = rsa.VerifyData(messageBytes, new SHA256CryptoServiceProvider(), signatureBytes);

                return isValid;
            }
        }

        public static string GetPublicKey(string privateKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                var publicKey = rsa.ExportParameters(false);
                return SerilizeToString(publicKey);
            }
        }

        public static string SerilizeToString(RSAParameters key)
        {
            var sw = new StringWriter();
            new XmlSerializer(typeof(RSAParameters)).Serialize(sw, key);

            return sw.ToString();
        }

        public static (string, string) GenerateKeyPairs()
        {
            var csp = new RSACryptoServiceProvider();

            var privKey = SerilizeToString(csp.ExportParameters(true));
            var pubKey = SerilizeToString(csp.ExportParameters(false));

            return (privKey, pubKey);
        }
    }
}