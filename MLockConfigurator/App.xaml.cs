using System.Windows;
using System;

namespace MLockUSBKeyGenerator
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            if (!Utils.IsAdmin())
            {
                MessageBox.Show("Configurator requires administrative privileges to properly save the config file. Please run the program as an administrator.", "Admin Privileges Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(0);
                return;
            }

            InitializeComponent();
        }
    }
}