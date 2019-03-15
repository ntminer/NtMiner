using NTMiner.Notifications;
using NTMiner.Vms;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NTMiner.Views {
    public partial class NotificationSample : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Width = 800,
                Height = 500,
                CloseVisible = Visibility.Visible
            }, ucFactory: (window) => {
                var uc = new NotificationSample();
                return uc;
            }, fixedSize: false);
        }

        public NotificationSample() {
            this.InitializeComponent();
        }


        private void ButtonBaseErrorOnClick(object sender, RoutedEventArgs e)
        {
            NotiCenterWindowViewModel.Current.Manager
                .CreateMessage()
                .Accent("#F15B19")
                .Background("#F15B19")
                .HasHeader("Lost connection to server")
                .HasMessage("Reconnecting...")
                .WithOverlay(new ProgressBar
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Height = 3,
                    BorderThickness = new Thickness(0),
                    Foreground = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)),
                    Background = Brushes.Transparent,
                    IsIndeterminate = true,
                    IsHitTestVisible = false
                })
                .Dismiss().WithButton("Ignore", button => { })
                .Queue();
        }

        private void ButtonBaseWarningOnClick(object sender, RoutedEventArgs e)
        {
            NotiCenterWindowViewModel.Current.Manager
                .CreateMessage()
                .Accent("#E0A030")
                .Background("#333")
                .HasBadge("Warn")
                .HasHeader("Error")
                .HasMessage("Failed to retrieve data.")
                .WithButton("Try again", button => { })
                .Dismiss().WithButton("Ignore", button => { })
                .Queue();
        }

        private void ButtonBaseInfoOnClick(object sender, RoutedEventArgs e)
        {
            NotiCenterWindowViewModel.Current.Manager
                .CreateMessage()
                .Accent("#1751C3")
                .Background("#333")
                .HasBadge("Info")
                .HasMessage("Update will be installed on next application restart.")
                .Dismiss().WithButton("Update now", button => { })
                .Dismiss().WithButton("Release notes", button => { })
                .Dismiss().WithButton("Later", button => { })
                .Queue();
        }

        private void ButtonBaseInfoDelayOnClick(object sender, RoutedEventArgs e)
        {
            NotiCenterWindowViewModel.Current.Manager
                .CreateMessage()
                .Accent("#1751C3")
                .Animates(true)
                .AnimationInDuration(0.75)
                .AnimationOutDuration(2)
                .Background("#333")
                .HasBadge("Info")
                .HasMessage("Update will be installed on next application restart. This message will be dismissed after 5 seconds.")
                .Dismiss().WithButton("Update now", button => { })
                .Dismiss().WithButton("Release notes", button => { })
                .Dismiss().WithDelay(TimeSpan.FromSeconds(5))
                .Queue();
        }

        private void ButtonBaseAdditionalContentOnClick(object sender, RoutedEventArgs e)
        {
            NotiCenterWindowViewModel.Current.Manager
                .CreateMessage()
                .Accent("#1751C3")
                .Background("#333")
                .Foreground("#333")
                .HasBadge("Info")
                .HasHeader("Header")
                .HasMessage("This is the message!")
                .WithAdditionalContent(ContentLocation.Top, new Border
                {
                    Height = 25,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Background = Brushes.Red
                })
                .WithAdditionalContent(ContentLocation.Bottom, new Border
                {
                    Height = 25,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Background = Brushes.Green
                })
                .WithAdditionalContent(ContentLocation.Left, new Border
                {
                    Width = 25,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Background = Brushes.Yellow
                })
                .WithAdditionalContent(ContentLocation.Right, new Border
                {
                    Width = 25,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Background = Brushes.Violet
                })
                .WithAdditionalContent(ContentLocation.Main, new Border
                {
                    MinHeight = 50,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Background = Brushes.Orange
                })
                .WithAdditionalContent(ContentLocation.AboveBadge, new Border
                {
                    Height = 40,
                    Width = 40,
                    Background = Brushes.Indigo
                })
                .Dismiss().WithButton("Dismiss", button => { })
                .Queue();
        }
    }
}
