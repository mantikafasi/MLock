using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

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

        public static Bitmap Screenshot()
        {
            var captureRectangle = Screen.PrimaryScreen.Bounds;

            var captureBitmap = new Bitmap(captureRectangle.Width, captureRectangle.Height);
            var captureGraphics = Graphics.FromImage(captureBitmap);
            captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);

            return captureBitmap;
        }
    }
}