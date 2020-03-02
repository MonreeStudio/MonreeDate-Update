using SQLite.Net.Platform.WinRT;
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

namespace BackgroundTasks
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    /// 
    public sealed partial class Tool : Page
    {
        static string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "mydb.sqlite");    //建立数据库  
        static SQLite.Net.SQLiteConnection conn;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        List<BgDataTemlate> list;
        public BgDataViewModel ViewModel = new BgDataViewModel();
        public Tool()
        {
            this.InitializeComponent();
            list = new List<BgDataTemlate>();
            //建立数据库连接   
            conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), path);
            //建表              
            conn.CreateTable<BgDataTemlate>(); //默认表名同范型参数 
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            var title = ApplicationView.GetForCurrentView().TitleBar;
            title.BackgroundColor = Colors.SkyBlue;
            title.ForegroundColor = Colors.Transparent;
            title.ButtonBackgroundColor = title.ButtonInactiveBackgroundColor = Colors.Transparent;
            title.ButtonHoverBackgroundColor = Colors.White;
            title.ButtonPressedBackgroundColor = Colors.White;
            title.ButtonForegroundColor = title.ButtonHoverForegroundColor;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(330, 320));
            ApplicationView.PreferredLaunchViewSize = new Size(330, 320);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            Pip();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                switch (list.Count())
                {
                    case 1:
                        List<BgDataTemlate> datalist1 = conn.Query<BgDataTemlate>("select * from BgDataTemlate where Schedule_name = ?",list[0]);
                        foreach(var item in datalist1)
                        {
                            ViewModel.BgDatas.Add(new BgData { ScheduleName = item.Schedule_name, Date = item.Date, CalDate = Calculator(item.Date) });
                        }
                        break;
                    case 2:
                        List<BgDataTemlate> datalist2 = conn.Query<BgDataTemlate>("select * from BgDataTemlate where Schedule_name = ?", list[0],list[1]);
                        foreach(var item in datalist2)
                        {
                            ViewModel.BgDatas.Add(new BgData { ScheduleName = item.Schedule_name, Date = item.Date, CalDate = Calculator(item.Date) });
                        }
                        break;
                    case 3:
                        List<BgDataTemlate> datalist3 = conn.Query<BgDataTemlate>("select * from BgDataTemlate where Schedule_name = ?", list[0], list[1],list[2]);
                        foreach (var item in datalist3)
                        {
                            ViewModel.BgDatas.Add(new BgData { ScheduleName = item.Schedule_name, Date = item.Date, CalDate = Calculator(item.Date) });
                        }
                        break;
                    default:
                        break;
                }
            }
            catch { }
        }

        public string Calculator(string s1)
        {
            string str1 = s1;
            string str2 = DateTime.Now.ToShortDateString().ToString();
            string s2;
            DateTime d1 = Convert.ToDateTime(str1);
            DateTime d2 = Convert.ToDateTime(str2);
            DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
            DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));
            int days = (d4 - d3).Days;
            if (days < 0)
            {
                days = -days;
                s2 = "还有" + days.ToString() + "天";
            }
            else
            {
                if (days != 0)
                    s2 = "已过" + days.ToString() + "天";
                else
                {
                    s2 = "就在今天";
                }
            }
            return s2;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e != null)
            {
                try
                {
                    list = (List<BgDataTemlate>)e.Parameter;
                    
                    localSettings.Values["DesktopPin"] = false;
                }
                catch { }
            }
        }
        private async void Pip()
        {
            var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            preferences.CustomSize = new Size(330, 320);
            await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, preferences);
            if (localSettings.Values["DesktopPin"] == null)
                localSettings.Values["DesktopPin"] = false;
            if (localSettings.Values["DesktopPin"].Equals(false))
            {
                localSettings.Values["DesktopPin"] = true;
                Frame.Navigate(typeof(Tool), null, new SuppressNavigationTransitionInfo()); ;
            }
            ApplicationView.PreferredLaunchViewSize = new Size(330, 320);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ////返回默认模式
            //var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.Default);
            //await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default, preferences);
        }

        private async void Unpip()
        {
            var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.Default);
            preferences.CustomSize = new Size(330, 320);
            await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default, preferences);
            ApplicationView.PreferredLaunchViewSize = new Size(330, 320);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            localSettings.Values["DesktopPin"] = false;
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
