using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using System.Windows.Forms;
using Common;
using MessageBox = System.Windows.MessageBox;

namespace MLock
{
    public partial class App
    {
        public static string MLOCK_DIR =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MLock";

        public App()
        {
            var isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            if (!isAdmin)
            {
                MessageBox.Show("Please run MLock as administrator", "Admin Privileges Required", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                Environment.Exit(0);
            }

            if (!ParseConfig()) Environment.Exit(0);

            InitializeComponent();

            var notifyIcon = new NotifyIcon();
            notifyIcon.Visible = true;

            using (var stream = Assembly.GetExecutingAssembly()
                       .GetManifestResourceStream("MLock.Resources.taskicon.ico"))
            {
                if (stream != null)
                    // wish there was shorter way to do this
                    notifyIcon.Icon = new Icon(stream);
            }


            notifyIcon.DoubleClick += (s, e) => { Events.Lock(); };

            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Lock", null, (s, e) => { Events.Lock(); });

            if (Config.INSTANCE.EnablePasswordUnlocking)
                notifyIcon.ContextMenuStrip.Items.Add("Quit", null, (s, e) =>
                {
                    var inputDialog = new InputDialog("Enter Your Password:");
                    if (inputDialog.ShowDialog() == true)
                    {
                        if (inputDialog.Answer == Config.INSTANCE.Password)
                            Environment.Exit(0);
                        else
                            MessageBox.Show("Incorrect Password");
                    }
                });
        }

        public bool ParseConfig()
        {
            if (File.Exists(MLOCK_DIR + "\\config.json"))
            {
                var jsonString = File.ReadAllText(MLOCK_DIR + "\\config.json");
                var config = jsonString.FromJson<Config>();
                Config.INSTANCE = config;
            }
            else
            {
                MessageBox.Show("Config file not found, please run the config installer first");
                return false;
            }

            if (Config.INSTANCE.EnableUSBUnlocking)
            {
                if (!File.Exists(MLOCK_DIR + "\\publicKey.xml"))
                {
                    MessageBox.Show(
                        "Public Key for USB Locking not found, please run the USB key generator and config installer first");
                    return false;
                }

                Config.INSTANCE.publicKey = File.ReadAllText(MLOCK_DIR + "\\publicKey.xml");
            }

            if (Config.INSTANCE.EnablePasswordUnlocking &&
                (Config.INSTANCE.Password == null || Config.INSTANCE.Password.Trim() == ""))
            {
                MessageBox.Show("Password not set, please run the config installer first");
                return false;
            }

            if (!Config.INSTANCE.EnablePasswordUnlocking && !Config.INSTANCE.EnableUSBUnlocking &&
                !Config.INSTANCE.EnableWebServer)
            {
                MessageBox.Show("No unlocking methods enabled, please run the config installer first");
                return false;
            }


            return true;
        }
    }
}