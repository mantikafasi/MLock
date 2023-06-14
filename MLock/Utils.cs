using System.Drawing;
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

        public static Bitmap Screenshot()
        {
            var captureRectangle = Screen.PrimaryScreen.Bounds;

            var captureBitmap = new Bitmap(captureRectangle.Width, captureRectangle.Height);
            using (var graphics = Graphics.FromImage(captureBitmap))
            {
                graphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
            }

            return captureBitmap;
        }
    }
}