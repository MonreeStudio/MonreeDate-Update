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
        public Settings()
        {
            this.InitializeComponent();
            Loaded += OnSettingsPageLoaded;
        }

        private void OnSettingsPageLoaded(object sender, RoutedEventArgs e)
        {

        }

        private async void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            if (All.Current != null)
            {
                All.Current.AllPageStackPanel.Background = new SolidColorBrush(Colors.SkyBlue);
                All.Current.TopText.Text = "看看有没有用呢？";
                MainPage.Current.MyNav.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
            }
            else
            {
                MainPage.Current.MyNav.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
            }
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
                    MainPage.Current.MyNav.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                    //All.current.AllFrame.Background = Windows.UI.Colors.BlanchedAlmond;
                    //TestTb.Text = "现在是开着的噢。";
                }
                else
                {
                    MainPage.Current.MyNav.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                }
            }
            toggleSwitch.Toggled += AllPageAcylic_Toggled;
        }
    }
}
