﻿using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows;
using Microsoft.Win32.TaskScheduler;

namespace MLockUSBKeyGenerator
{
    internal class Utils
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetVolumeInformation(string letter, StringBuilder name, uint nameSize,
            out uint serialNumber, out uint serialNumberLength, out uint flags, StringBuilder systemName,
            uint systemNameSize);

        public static uint GetSerialNumberOfDrive(string volume)
        {
            var name = new StringBuilder(256);
            var systemName = new StringBuilder(256);
            var serialNumber = 0u;
            var serialNumberLength = 0u;
            var flags = 0u;

            volume = (volume ?? string.Empty).Trim();

            if (volume.Length == 1) volume = $"{volume}:\\";
            if (!volume.EndsWith(@"\")) volume = $"{volume}\\";

            if (GetVolumeInformation(volume, name, 256, out serialNumber, out serialNumberLength, out flags, systemName,
                    256)) return serialNumber;

            return 0u;
        }

        public static bool IsTaskInstalled()
        {
            return TaskService.Instance.RootFolder.Tasks.Any(task => task.Name == "MLockTask");
        }

        public static bool IsAdmin()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void InstallTask()
        {
            var td = TaskService.Instance.NewTask();
            td.RegistrationInfo.Description = "Starts MLock";
            td.Principal.RunLevel = TaskRunLevel.Highest;
            td.Triggers.Add(new LogonTrigger());
            td.Actions.Add("MLock.exe", "", Directory.GetCurrentDirectory());

            TaskService.Instance.RootFolder.RegisterTaskDefinition("MLockTask", td);
            MessageBox.Show("Task Installed Successfully, MLock will start on logon now.");
        }

        public static void UninstallTask()
        {
            TaskService.Instance.RootFolder.DeleteTask("MLockTask");
            MessageBox.Show("Task Uninstalled Successfully, MLock will not start on logon now.");
        }
    }
}