using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Common;
using MLock.Modules;

namespace MLock
{
    public partial class MainWindow
    {
        private readonly int _blurValue = 100;
        private readonly Native.KHConfig config;

        public MainWindow()
        {
            InitializeComponent();

            Events.UnlockApp += OnUnlock;

            Events.LockApp += OnLock;

            config = new Native.KHConfig
            {
                ClearCountsAsFail = false,
                RequireEnter = false,
                Pw = Config.INSTANCE.Password,
                PwLength = Config.INSTANCE.Password.Length,
                OnFail = OnFail,
                OnSuccess = () => { Events.Unlock(); },
                OnInput = (input, len, fullInp) => { SetPasswordText(fullInp); }
            };

            if (Config.INSTANCE.BlurBackground)
            {
                var blurEffect = new BlurEffect();

                BackgroundGrid.Effect = blurEffect;

                Events.LockApp += () =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        var screenshot = Utils.Screenshot();

                        var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                            screenshot.GetHbitmap(),
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromWidthAndHeight(screenshot.Width, screenshot.Height));

                        BackgroundGrid.Background = new ImageBrush(bitmapSource);
                    });
                    SetBlur(false);
                };

                Events.UnlockApp += () => { SetBlur(true); };
            }

            Native.SetKHookConfig(config);

            if (Config.INSTANCE.StartLocked &&
                !Config.INSTANCE.EnableUSBUnlocking) // CheckUSBs method automaticly locks if no USBs are found
                Events.Lock();

            if (Config.INSTANCE.EnableUSBUnlocking)
            {
                USB.Initialize();
                USB.CheckUSBs();
            }

            if (Config.INSTANCE.EnableWebServer)
                Task.Run(() => new WebServer().Initialize());
        }

        private void SetPasswordText(string text)
        {
            if (text.Length == 0)
            {
                PasswordBlock.TextDecorations = null;
                PasswordBlock.Text = "Enter Password";
                PasswordBlock.Foreground = Brushes.Gray;
                return;
            }

            PasswordBlock.TextDecorations = TextDecorations.Underline;
            PasswordBlock.Foreground = Brushes.White;
            PasswordBlock.Text = string.Concat(Enumerable.Repeat("*", text.Length));
        }

        private void OnFail(string pw)
        {
            Dispatcher.Invoke(() =>
            {
                SetBackground(Brushes.Red);
                new Thread(() =>
                {
                    Thread.Sleep(500);
                    Dispatcher.Invoke(() => { SetBackground(Brushes.Transparent); });
                }).Start();
            });
        }

        public void OnUnlock()
        {
            Dispatcher.Invoke(() => { SetBackground(Brushes.LightGreen); }); // I hate this

            new Thread(() =>
            {
                Thread.Sleep(500);
                Dispatcher.Invoke(() =>
                {
                    Background = Brushes.Transparent;
                    Native.UninstallKHook();
                    Native.RestoreTskMan();
                    Hide();
                });
            }).Start();
        }

        public void OnLock()
        {
            Dispatcher.Invoke(() =>
            {
                SetPasswordText("");

                if (!Native.DisableTskMan())
                {
                    MessageBox.Show("Failed to disable task manager,be sure MLock has administrator permissions");
                    return;
                }

                if (!Config.INSTANCE.Debug)
                    Native.InstallKHook();
                Visibility = Visibility.Visible;
            });
        }

        public void SetBlur(bool clear)
        {
            Dispatcher.Invoke(() =>
            {
                var blurAnimation = new DoubleAnimation();
                if (clear)
                {
                    blurAnimation.From = _blurValue;
                    blurAnimation.To = 0;
                }
                else
                {
                    blurAnimation.From = 0;
                    blurAnimation.To = _blurValue;
                }

                blurAnimation.Duration = TimeSpan.FromMilliseconds(500);

                BackgroundGrid.Effect.BeginAnimation(BlurEffect.RadiusProperty, blurAnimation);
            });
        }

        public void SetBackground(SolidColorBrush color)
        {
            Background = color;
            /*
            var colorAnimation = new ColorAnimation();
            colorAnimation.From = (Background as SolidColorBrush).Color;

            colorAnimation.To = color.Color;

            colorAnimation.Duration = TimeSpan.FromMilliseconds(500);

            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(Grid.Background).(SolidColorBrush.Color)", null));
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(colorAnimation);
            storyboard.Begin(this);
            */ // this works kinda flashy
        }
    }
}