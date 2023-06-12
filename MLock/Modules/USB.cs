using System;
using System.IO;
using System.Management;
using Common;

namespace MLock
{
    internal class USB
    {
        public static void Initialize()
        {
            var watcher = new ManagementEventWatcher();
            var query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2 or EventType = 3");
            watcher.EventArrived += OnUSBEvent;
            watcher.Query = query;
            watcher.Start();
        }

        private static bool CheckDrive(string driveName) //driveName is in format "C:"
        {
            if (File.Exists(driveName + "/MLockUSBKey"))
            {
                var key = File.ReadAllText(driveName + "/MLockUSBKey");

                var number = Utils.GetSerialNumberOfDrive(driveName);
                if (Utils.VerifySignature(number.ToString(), key, Config.INSTANCE.publicKey)) return true;
            }

            return false;
        }

        public static void CheckUSBs()
        {
            foreach (var drive in DriveInfo.GetDrives())
                if (CheckDrive(drive.Name))
                {
                    Events.Unlock();
                    return;
                }

            Events.Lock(Events.EventSource.USB);
        }

        private static void OnUSBEvent(object sender, EventArrivedEventArgs e)
        {
            var instance = e.NewEvent;
            foreach (var property in instance.Properties)
            {
                var eventType = Convert.ToInt16(e.NewEvent.Properties["EventType"].Value);

                if (eventType == 2)
                {
                    Console.WriteLine("USB plugged in!");
                    var driveName = e.NewEvent.Properties["DriveName"].Value.ToString();

                    if (CheckDrive(driveName)) Events.Unlock();
                    return;
                }

                CheckUSBs();

                Console.WriteLine(property.Name + " = " + property.Value);
            }
        }
    }
}