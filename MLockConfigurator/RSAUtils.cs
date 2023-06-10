using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MLockUSBKeyGenerator
{
    internal class RSAUtils
    {

        public static string Encrypt(string text, string key)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(text);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(key);

                // Encrypt the message using RSA
                byte[] encryptedMessage = rsa.Encrypt(messageBytes, true);

                return encryptedMessage.ToString();
            }
        }

        public static string SignMessage(string message, string privateKey)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);

                // Compute signature
                byte[] signatureBytes = rsa.SignData(messageBytes, new SHA256CryptoServiceProvider());

                // Convert signature to base64 string
                string signature = Convert.ToBase64String(signatureBytes);

                return signature;
            }
        }

        public static bool VerifySignature(string message, string signature, string publicKey)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] signatureBytes = Convert.FromBase64String(signature);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);

                // Verify signature
                bool isValid = rsa.VerifyData(messageBytes, new SHA256CryptoServiceProvider(), signatureBytes);

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


        public RSAParameters SerilizeToParameters(string key) => 
            (RSAParameters)new System.Xml.Serialization
            .XmlSerializer(typeof(RSAParameters))
            .Deserialize(new System.IO.StringReader(key));

        public static string SerilizeToString(RSAParameters key)
        {
            var sw = new System.IO.StringWriter();
            new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters)).Serialize(sw, key);
           
            return sw.ToString();
        }

        public static (string,string) GenerateKeyPairs()
        {
            var csp = new RSACryptoServiceProvider();

            var privKey = SerilizeToString(csp.ExportParameters(true));
            var pubKey = SerilizeToString(csp.ExportParameters(false));

            return (privKey, pubKey);
        }
    }
}
