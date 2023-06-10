using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace MLock
{
    public partial class MainWindow
    {
        Native.KHConfig config;

        public MainWindow()
        {
            InitializeComponent();

            Events.UnlockApp += () =>
            {
                Console.WriteLine("Password correct!");
                Dispatcher.Invoke(() =>
                {
                    Background = System.Windows.Media.Brushes.LightGreen;
                    new Thread(() =>
                    {
                        Thread.Sleep(500);
                        Dispatcher.Invoke(() =>
                        {
                            Background = System.Windows.Media.Brushes.White;
                            Native.UninstallKHook();
                            Native.RestoreTskMan();
                            Hide();
                        });
                    }).Start();
                });
            };

            Events.LockApp += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    PasswordBlock.Text = "Enter Password";
                    
                    if (!Native.DisableTskMan())
                    {
                        MessageBox.Show("Failed to disable task manager,be sure MLock has administrator permissions");
                        return;
                    }

                    Native.InstallKHook();
                    Visibility = Visibility.Visible;
                });
            };

            config = new Native.KHConfig() {
                ClearCountsAsFail = false,
                RequireEnter = false,
                Pw = Config.INSTANCE.Password,
                PwLength = Config.INSTANCE.Password.Length,
                OnFail = (pw) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        Background = System.Windows.Media.Brushes.Red;
                        new Thread(() =>
                        {
                            Thread.Sleep(500);
                            Dispatcher.Invoke(() =>
                            {
                                Background = System.Windows.Media.Brushes.White;
                            });
                        }).Start();
                    });
                },
                OnSuccess = () =>
                {
                    Events.Unlock();
                },
                OnInput = (input,len,fullInp) => {

                    if (fullInp.Length == 0)
                    {
                        PasswordBlock.Text = "Enter Password";
                        return;
                    }
                    
                    PasswordBlock.Text = String.Concat(Enumerable.Repeat("*", fullInp.Length));
                    // todo: maybe use Binding instead of this
                },
                
            };

            Native.SetKHookConfig(config);

            if (Config.INSTANCE.StartLocked)
            {
                Events.Lock();
            }

            if (Config.INSTANCE.EnableUSBUnlocking)
            {
                USB.Initialize();

                USB.CheckUSBs();
            }

        }


    }
}