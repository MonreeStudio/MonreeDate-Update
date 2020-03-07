using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Media.Animation;
using Windows.System;
using Windows.Storage;
using Windows.Foundation.Metadata;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using 夏日.Models;
using Microsoft.Toolkit.Uwp.Notifications;
using 倒计时.Models;
using Windows.ApplicationModel;
using BackgroundTasks;
using Windows.UI.Popups;
using SQLite.Net.Platform.WinRT;
using Windows.UI.Core;


// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace 倒计时
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;
        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public bool SelectedPage { get; set; }
        public string SelectedPageItem { get; set; }
        public IntroPageViewModel ViewModel = new IntroPageViewModel();
        static string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "mydb.sqlite");    //建立数据库  
        static SQLite.Net.SQLiteConnection conn;

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            //建立数据库连接   
            conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), path);
            //建表              
            conn.CreateTable<DataTemple>(); //默认表名同范型参数 
            SelectedPage = true;
            var applicationView = CoreApplication.GetCurrentView();
            applicationView.TitleBar.ExtendViewIntoTitleBar = true;
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            var title = ApplicationView.GetForCurrentView().TitleBar;
            title.BackgroundColor = Colors.SkyBlue;
            title.ForegroundColor = Colors.Transparent;
            title.ButtonBackgroundColor = title.ButtonInactiveBackgroundColor = Colors.Transparent;
            title.ButtonHoverBackgroundColor = Colors.White;
            title.ButtonPressedBackgroundColor = Colors.White;
            title.ButtonForegroundColor = title.ButtonHoverForegroundColor;
            SetThemeColor();
            GetAppVersion();
            MyNav.IsBackEnabled = false;
            SelectedPageItem = "";
            localSettings.Values["mainViewId"] = ApplicationView.GetForCurrentView().Id;
            ToolAutoStart();
        }

        private void ToolAutoStart()
        {
            if (localSettings.Values["ReStart"] != null && localSettings.Values["ReStart"].ToString() == "1")
            {
                CreateTool();
                localSettings.Values["ReStart"] = "0";
                return;
            }
            if (localSettings.Values["ToolAutoStart"] != null && localSettings.Values["ToolAutoStart"].ToString() == "1")
                CreateTool();
        }

        public async void CreateTool()
        {
            var num = CoreApplication.Views.Count();
            //if (localSettings.Values["newViewId"] != null)
            //    ApplicationViewSwitcher.SwitchAsync(Convert.ToInt32(localSettings.Values["newViewId"])).Close();

            int count = (int)localSettings.Values["ItemCount"];
            List<DataTemple> datalist = new List<DataTemple>();
            var allData = conn.Query<DataTemple>("select *from DataTemple");
            switch (count)
            {
                case 1:
                    string a1 = localSettings.Values["DesktopKey0"].ToString();
                    foreach (var item in allData)
                    {
                        if (item.Schedule_name == a1)
                            datalist.Add(item);
                    }
                    break;
                case 2:
                    string b1 = localSettings.Values["DesktopKey0"].ToString();
                    string b2 = localSettings.Values["DesktopKey1"].ToString();
                    foreach (var item in allData)
                    {
                        if (item.Schedule_name == b1 || item.Schedule_name == b2)
                            datalist.Add(item);
                    }
                    break;
                case 3:
                    string c1 = localSettings.Values["DesktopKey0"].ToString();
                    string c2 = localSettings.Values["DesktopKey1"].ToString();
                    string c3 = localSettings.Values["DesktopKey2"].ToString();
                    foreach (var item in allData)
                    {
                        if (item.Schedule_name == c1 || item.Schedule_name == c2 || item.Schedule_name == c3)
                            datalist.Add(item);
                    }
                    break;
                default:
                    return;
            }
            //var num = CoreApplication.Views.Count();
            //coreWindow.Activate();
            //CoreApplication.GetCurrentView();
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(DesktopTool), datalist, new SuppressNavigationTransitionInfo());
                Window.Current.Content = frame;
                Window.Current.Activate();
                newViewId = ApplicationView.GetForCurrentView().Id;
                localSettings.Values["newViewId"] = newViewId;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            localSettings.Values["DesktopPin"] = false;
        }

        public void SetThemeColor()
        {
            if (localSettings.Values["ThemeColor"] == null)
                localSettings.Values["ThemeColor"] = "CornflowerBlue";
            switch (localSettings.Values["ThemeColor"].ToString())
            {
                case "CornflowerBlue":
                    TC.Color = Colors.CornflowerBlue;                    
                    break;
                case "DeepSkyBlue":
                    TC.Color = Color.FromArgb(255, 2, 136, 235);
                    break;
                case "Orange":
                    TC.Color = Color.FromArgb(255, 229, 103, 44);
                    break;
                case "Crimson":
                    TC.Color = Colors.Crimson;
                    break;
                case "Gray":
                    TC.Color = Color.FromArgb(255, 73, 92, 105);
                    break;
                case "Purple":
                    TC.Color = Color.FromArgb(255, 119, 25, 171);
                    break;
                case "Pink":
                    TC.Color = Color.FromArgb(255, 239, 130, 160);
                    break;
                case "Green":
                    TC.Color = Color.FromArgb(255, 124, 178, 56);
                    break;
                case "DeepGreen":
                    TC.Color = Color.FromArgb(255, 8, 128, 126);
                    break;
                case "Coffee":
                    TC.Color = Color.FromArgb(255, 183, 133, 108);
                    break;  
                default:
                    break;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.RegisterBackgroundTask();
            
        }

        private async void RegisterBackgroundTask()
        {
            const string taskName = "BlogFeedBackgroundTask";
            const string taskEntryPoint = "BackgroundTasks.BlogFeedBackgroundTask";

            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == taskName)
                    {
                        task.Value.Unregister(true);
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = taskEntryPoint;
                taskBuilder.SetTrigger(new TimeTrigger(15, false));
                var registration = taskBuilder.Register();
            }
        }

        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
          ("All", typeof(All)),
          ("New", typeof(Add)),
          ("Calculator", typeof(Calculator)),
          ("Festival", typeof(Festival)),
          ("Details",typeof(Details)),
          ("Desktop",typeof(Desktop)),
        };

        private void MyNav_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (navItemTag == "settings")
            {
                _page = typeof(Settings);
            }
            else
            {
                if (navItemTag == "Details")
                {
                    _page = typeof(Details);
                }
                else
                {
                    var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                    _page = item.Page;
                }
            }
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                ContentFrame.Navigate(_page, null, transitionInfo);
            }
        }

        private void MyNav_Loaded(object sender, RoutedEventArgs e)
        {

            // Add handler for ContentFrame navigation.
            ContentFrame.Navigated += On_Navigated;

            // NavView doesn't load any page by default, so load home page.
            MyNav.SelectedItem = MyNav.MenuItems[0];
            // If navigation occurs on SelectionChanged, this isn't needed.
            // Because we use ItemInvoked to navigate, we need to call Navigate
            // here to load the home page.
            MyNav_Navigate("All", new EntranceNavigationTransitionInfo());

            // Add keyboard accelerators for backwards navigation.
            var goBack = new KeyboardAccelerator { Key = VirtualKey.GoBack };
            goBack.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(goBack);

            // ALT routes here
            var altLeft = new KeyboardAccelerator
            {
                Key = VirtualKey.Left,
                Modifiers = VirtualKeyModifiers.Menu
            };
            altLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(altLeft);
        }

        private void MyNav_ItemInvoked(NavigationView sender,NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                MyNav_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                MyNav_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void BackInvoked(KeyboardAccelerator sender,
                        KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }

        private bool On_BackRequested()
        {

            if (!ContentFrame.CanGoBack)
                return false;
            // Don't go back if the nav pane is overlayed.
            if (MyNav.IsPaneOpen &&
                (MyNav.DisplayMode == NavigationViewDisplayMode.Compact ||
                 MyNav.DisplayMode == NavigationViewDisplayMode.Minimal))
                return false;
            if(SelectedPageItem.Equals("Add")
                ||SelectedPageItem.Equals("Calculator")
                ||SelectedPageItem.Equals("Festival")
                ||SelectedPageItem.Equals("Settings")
                ||SelectedPageItem.Equals("Desktop"))
            {
                MyNav.SelectedItem = MyNav.MenuItems[0];
                ContentFrame.Navigate(typeof(All));
                SelectedPageItem = "All";
                All.Current.LoadAllPage();
                MyNav.IsBackEnabled = false;
                return true;
            }
            else
            {
                if (SelectedPageItem.Equals("Details") && SelectedPage == false)
                {
                    ContentFrame.Navigate(typeof(Festival));
                    SelectedPageItem = "Festival";
                    return true;
                }
                if(SelectedPageItem.Equals("Details")&&SelectedPage==true)
                {
                    ContentFrame.Navigate(typeof(All));
                    SelectedPageItem = "All";
                    All.Current.LoadAllPage();
                    MyNav.IsBackEnabled = false;
                    return true;
                }
                ContentFrame.GoBack();
                return true;
            }
        }

        private void MyNav_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            On_BackRequested();
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void MyNav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected == true)
            {
                MyNav_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.SelectedItemContainer != null)
            {
                var navItemTag = args.SelectedItemContainer.Tag.ToString();
                MyNav_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
                if (navItemTag == "All")
                {
                    MyNav.IsBackEnabled = false;
                    SelectedPageItem = "All";
                    All.Current.LoadAllPage();
                    return;
                }
                if(navItemTag == "Festival")
                {
                    MyNav.IsBackEnabled = true;
                    SelectedPageItem = "Festival";
                }
            }
        }
        private async void On_Navigated(object sender, NavigationEventArgs e)
        {
            
            //localSettings.Values["FirstlyOpen"] = null;
            if (localSettings.Values["2.2.4.0"] == null)
            { 
                await MyCD.ShowAsync();
                localSettings.Values["2.2.4.0"] = "false";
            }
            //MyNav.IsBackEnabled = ContentFrame.CanGoBack;

            if (ContentFrame.SourcePageType == typeof(Settings))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                MyNav.SelectedItem = (NavigationViewItem)MyNav.SettingsItem;
                //MyNav.Header = "Settings";
            }
            else if (ContentFrame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

                //MyNav.SelectedItem = MyNav.MenuItems
                //    .OfType<NavigationViewItem>()
                //    .First(n => n.Tag.Equals(item.Tag));

                // MyNav.Header =
                //   ((NavigationViewItem)MyNav.SelectedItem)?.Content?.ToString();
            }
        }

        //private void CarouselControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    CDT.Text = (CarouselControl.SelectedIndex + 1).ToString()+"/4";
        //}

        private void GetAppVersion()
        {
            string appVersion = string.Format("此版本： {0}.{1}.{2}.{3}",
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Revision);
            Version.Text = appVersion;
        }

        private async void MyCD_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var Uri = new Uri("ms-windows-store://review/?productid=9PKBWKPCCFJ8");
            await Launcher.LaunchUriAsync(Uri);
        }
    }
}
