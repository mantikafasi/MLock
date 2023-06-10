using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MLockUSBKeyGenerator
{
    internal class Utils
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetVolumeInformation(string letter, StringBuilder name, uint nameSize, out uint serialNumber, out uint serialNumberLength, out uint flags, StringBuilder systemName, uint systemNameSize);

        public static uint GetSerialNumberOfDrive(string volume)
        {
            var name = new StringBuilder(256);
            var systemName = new StringBuilder(256);
            var serialNumber = 0u;
            var serialNumberLength = 0u;
            var flags = 0u;

            volume = (volume ?? String.Empty).Trim();

            if (volume.Length == 1)
            {
                volume = $"{volume}:\\";
            }
            if (!volume.EndsWith(@"\"))
            {
                volume = $"{volume}\\";
            }

            if (GetVolumeInformation(volume, name, 256, out serialNumber, out serialNumberLength, out flags, systemName, 256))
            {
                return serialNumber;
            }
            else
            {
                return 0u;
            }
        }

    }
}
