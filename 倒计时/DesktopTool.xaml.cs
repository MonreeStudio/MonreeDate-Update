using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 倒计时
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DesktopTool : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        string _Event;
        List<string> list;
        public DesktopTool()
        {
            this.InitializeComponent();
            list = new List<string>();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            var title = ApplicationView.GetForCurrentView().TitleBar;
            title.BackgroundColor = Colors.SkyBlue;
            title.ForegroundColor = Colors.Transparent;
            title.ButtonBackgroundColor = title.ButtonInactiveBackgroundColor = Colors.Transparent;
            title.ButtonHoverBackgroundColor = Colors.White;
            title.ButtonPressedBackgroundColor = Colors.White;
            title.ButtonForegroundColor = title.ButtonHoverForegroundColor;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(220, 120));
            ApplicationView.PreferredLaunchViewSize = new Size(220, 120);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            Pip();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                DT_ScheduleName.Text = localSettings.Values["DesktopEvent"].ToString();
                DT_CalDate.Text = localSettings.Values["DesktopCalDate"].ToString();
                DT_Date.Text = localSettings.Values["DesktopDate"].ToString();
            }
            catch { }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e != null)
            {
                try
                {
                    var DetailsList = (List<string>)e.Parameter;
                    
                    localSettings.Values["DesktopEvent"] = DetailsList[0];
                    localSettings.Values["DesktopCalDate"] = DetailsList[1];
                    localSettings.Values["DesktopDate"] = DetailsList[2];
                    DT_ScheduleName.Text = DetailsList[0];
                    _Event = localSettings.Values["DesktopEvent"].ToString();
                    DT_CalDate.Text = DetailsList[1];
                    DT_Date.Text = DetailsList[2];
                }
                catch { }
            }
        }
        private async void Pip()
        {
            var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            preferences.CustomSize = new Size(320, 120);
            await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, preferences);
            var DesktopName = "Desktop" + _Event;
            if (localSettings.Values[DesktopName] == null)
                localSettings.Values[DesktopName] = false;
            if (localSettings.Values[DesktopName].Equals(false))
            {
                localSettings.Values[DesktopName] = true;
                Frame.Navigate(typeof(DesktopTool),null, new SuppressNavigationTransitionInfo());
                
            }
            ApplicationView.PreferredLaunchViewSize = new Size(320, 120);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ////返回默认模式
            //var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.Default);
            //await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default, preferences);
        }

        private async void Unpip()
        {
            var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.Default);
            preferences.CustomSize = new Size(320, 120);
            await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default, preferences);
            ApplicationView.PreferredLaunchViewSize = new Size(320, 120);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            var DesktopName = "Desktop" + _Event;
            localSettings.Values[DesktopName] = false;
        }

        private void SetTopBtn_Click(object sender, RoutedEventArgs e)
        {
            Pip();
            SetTopBtn.Visibility = Visibility.Collapsed;
            DeSetTopBtn.Visibility = Visibility.Visible;
        }

        private void DeSetTopBtn_Click(object sender, RoutedEventArgs e)
        {
            Unpip();
            SetTopBtn.Visibility = Visibility.Visible;
            DeSetTopBtn.Visibility = Visibility.Collapsed;
        }
    }
}
