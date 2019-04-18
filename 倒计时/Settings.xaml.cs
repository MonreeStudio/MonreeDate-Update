using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 倒计时
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Settings : Page
    {
        public double MinMyNav = MainPage.Current.MyNav.CompactModeThresholdWidth;
        
        public Settings()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            Loaded += OnSettingsPageLoaded;
        }

        private void OnSettingsPageLoaded(object sender, RoutedEventArgs e)
        {

        }

        private async void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Details),AboutButton.Content);
            MessageDialog AboutDialog = new MessageDialog("emmmmm\n...........");
            await AboutDialog.ShowAsync();
        }

        private void AllPageAcylic_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (AllPageAcylic.IsOn == true)
                {
                    AcrylicBrush myBrush = new AcrylicBrush();
                    myBrush.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
                    myBrush.TintColor = Color.FromArgb(255, 255, 255, 255);
                    myBrush.FallbackColor = Color.FromArgb(255, 255, 255, 255);
                    myBrush.TintOpacity = 0.8;
                    All.Current.AllPageStackPanel.Background = myBrush;
                }
                else
                {
                    All.Current.AllPageStackPanel.Background = new SolidColorBrush(Colors.White);
                }
            }
            toggleSwitch.Toggled += AllPageAcylic_Toggled;
        }
    }
}
