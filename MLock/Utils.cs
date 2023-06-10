using System;
using System.Security.Cryptography;
using System.Text;

namespace MLock
{
    internal class Utils
    {
        public static uint GetSerialNumberOfDrive(string volume)
        {
            var name = new StringBuilder(256);
            var systemName = new StringBuilder(256);
            var serialNumber = 0u;

            volume = $"{volume}\\";

            if (Native.GetVolumeInformation(volume, name, 256, out serialNumber, 0u, 0u, systemName, 256))
                return serialNumber;
            return 0u;
        }

        public static bool VerifySignature(string message, string signature, string publicKey)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var signatureBytes = Convert.FromBase64String(signature);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);

                return rsa.VerifyData(messageBytes, new SHA256CryptoServiceProvider(), signatureBytes);
            }
        }
    }
}