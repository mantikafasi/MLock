﻿using MLock;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;

namespace MLockUSBKeyGenerator
{
    public partial class ConfiguratorWindow
    {
        DriveInfo[] drives = DriveInfo.GetDrives();
        public static string MLOCK_DIR = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MLock";
        Config config = new Config();

        public ConfiguratorWindow()
        {
            InitializeComponent();

            if (File.Exists(MLOCK_DIR + "\\config.json"))
            {
                var jsonString = File.ReadAllText(MLOCK_DIR + "\\config.json");

                config = JsonSerializer.Deserialize<Config>(jsonString);
            }

            DataContext = config;

            foreach (DriveInfo drive in drives)
            {
                USBComboBox.Items.Add(drive.Name + " " + drive.VolumeLabel);
                if (File.Exists("privateKey.xml"))
                {
                    PrivateKeyPathBox.Text = Directory.GetCurrentDirectory() + "\\privateKey.xml";
                }
            }

            if (Utils.IsTaskInstalled())
            {
                InstallTaskButton.Content = "Uninstall Task";
            }

        }

        public void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            var keys= RSAUtils.GenerateKeyPairs();
            File.WriteAllText("privateKey.xml", keys.Item1);
            //get current path

            PrivateKeyPathBox.Text = Directory.GetCurrentDirectory() + "\\privateKey.xml";
        }

        public void GenerateKey_Click(object sender,RoutedEventArgs e)
        {

            var ix = USBComboBox.SelectedIndex;

            if (PrivateKeyPathBox.Text == "")
            {
                MessageBox.Show("Please select a private key file");
                return;
            }

            if (ix == -1)
            {
                MessageBox.Show("Please select a USB drive");
                return;
            }
            var privateKey = File.ReadAllText(PrivateKeyPathBox.Text);


            
            var drive = drives[ix];
            var number = Utils.GetSerialNumberOfDrive(drive.Name);    
            Console.WriteLine(number);
            var sign = RSAUtils.SignMessage(number.ToString(), privateKey);
            try
            {
                File.WriteAllText(drive.Name + "MLockUSBKey", sign);
            } catch (Exception _)
            {
                MessageBox.Show("Error writing to USB drive, are you sure its writeable");
                return;
            }
            MessageBox.Show("Key generated successfully");
        }

        public void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "privateKey"; // Default file name
            dialog.DefaultExt = ".xml"; // Default file extension
            dialog.Filter = "XML Files (.xml)|*.xml"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                PrivateKeyPathBox.Text = dialog.FileName;
            }
        }

        public void InstallConfigButton_Click(object sender,RoutedEventArgs e)
        {
            if (ValidateConfig()) {

                if (!Directory.Exists(MLOCK_DIR))
                {
                    Directory.CreateDirectory(MLOCK_DIR);
                }

                var jsonString = JsonSerializer.Serialize(config);
                File.WriteAllText(MLOCK_DIR + "\\config.json", jsonString);

                if (config.EnableUSBUnlocking)
                {
                    File.WriteAllText(MLOCK_DIR + "\\publicKey.xml", RSAUtils.GetPublicKey(config.privateKey));
                }
                MessageBox.Show("Config installed successfully");
            }

        }

        public bool ValidateConfig()
        {
            if (config.EnableUSBUnlocking)
            {
                if (!File.Exists(PrivateKeyPathBox.Text))
                {
                    MessageBox.Show("Private Key for USB Locking not found, please select the existent one or generate");
                    return false;

                }
                else
                {
                    config.privateKey = File.ReadAllText(PrivateKeyPathBox.Text);
                }
            }


            if (config.Password.Contains(" "))
            {
                MessageBox.Show("Password cannot contain spaces");
                return false;
            }

            if (config.EnablePasswordUnlocking && (config.Password == null || config.Password.Trim() == ""))
            {
                MessageBox.Show("Password not set");
                return false;
            }

            if (!config.EnablePasswordUnlocking && !config.EnableUSBUnlocking)
            {
                MessageBox.Show("No unlocking methods enabled, select one first");
                return false;
            }

            return true;
        }

        private void InstallTaskButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Utils.IsTaskInstalled())
            {
                Utils.UninstallTask();
            }
            else
            {
                Utils.InstallTask();
            }

            InstallTaskButton.Content = Utils.IsTaskInstalled() ? "Uninstall startup Task" : "Install startup Task";
        }
    }

}