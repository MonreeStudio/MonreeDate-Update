using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLite.Net.Platform.WinRT;
using SQLite.Net.Interop;
using SQLite.Net.Attributes;
using 夏日;
using Windows.UI;
using Microsoft.QueryStringDotNET;
using BackgroundTasks;
using Windows.UI.Popups;
using Windows.ApplicationModel.Core;
using 夏日.Models;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Animation;

namespace 倒计时
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        public CustomDataViewModel ViewModel = new CustomDataViewModel();
        public ObservableCollection<CustomData> CustomDatas = new ObservableCollection<CustomData>();
        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private const string SelectedAppThemeKey = "SelectedAppTheme";
        public static CustomData AllItem;
        public static FestivalData FestivalItem;
        static string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "mydb.sqlite");    //建立数据库  
        static SQLite.Net.SQLiteConnection conn;
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        /// 

        public static ElementTheme ActualTheme
        {
            get
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    if (rootElement.RequestedTheme != ElementTheme.Default)
                    {
                        return rootElement.RequestedTheme;
                    }
                }

                return GetEnum<ElementTheme>(Current.RequestedTheme.ToString());
            }
        }

        public static ElementTheme RootTheme
        {
            get
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    return rootElement.RequestedTheme;
                }

                return ElementTheme.Default;
            }
            set
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = value;
                }

                ApplicationData.Current.LocalSettings.Values[SelectedAppThemeKey] = value.ToString();
            }
        }
        
        public App()
        {
            this.InitializeComponent();
            
            this.Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;
            this.RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;
            //建立数据库连接   
            conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), path);
            //建表              
            conn.CreateTable<DataTemple>(); //默认表名同范型参数 
            //RequestedTheme = ApplicationTheme.Light;
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6))
            {
                this.FocusVisualKind = AnalyticsInfo.VersionInfo.DeviceFamily == "Xbox" ? FocusVisualKind.Reveal : FocusVisualKind.HighVisibility;
            }
            this.FocusVisualKind = FocusVisualKind.Reveal;
        }

        protected override void OnActivated(IActivatedEventArgs e)
        {
            Frame root = Window.Current.Content as Frame;
            if (root == null)
            {
                root = new Frame();
                Window.Current.Content = root;
            }
            if (e.Kind == ActivationKind.ToastNotification)
            {
                var toastargs =  e as ToastNotificationActivatedEventArgs;
                root.Navigate(typeof(MainPage));
                Window.Current.Activate();
            }
            if (e.Kind == ActivationKind.StartupTask)
            {
                var startupArgs = e as StartupTaskActivatedEventArgs;
                root.Navigate(typeof(MainPage));
                Window.Current.Activate();
                
            }
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
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
            if (localSettings.Values["ItemCount"] == null)
                localSettings.Values["ItemCount"] = 0;
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

        public static string Term(DateTime b, DateTime e)
        {
            if (b < e)
            {
                var t = new
                {
                    bm = b.Month,
                    em = e.Month,
                    bd = b.Day,
                    ed = e.Day
                };
                int diffMonth = (e.Year - b.Year) * 12 + (t.em - t.bm),//相差月
                    diffYear = diffMonth / 12;//相差年

                int[] d = new int[3] { 0, 0, 0 };
                if (diffYear > 0)
                {
                    if (t.em == t.bm && t.ed < t.bd)
                    {
                        d[0] = diffYear - 1;
                    }
                    else d[0] = diffYear;
                }

                if (t.ed >= t.bd)
                {
                    d[1] = diffMonth % 12;
                    d[2] = t.ed - t.bd;
                }
                else//结束日 小于 开始日
                {
                    int dm = diffMonth - 1;
                    d[1] = dm % 12;
                    TimeSpan ts = e - b.AddMonths(dm);
                    d[2] = ts.Days;
                }
                StringBuilder sb = new StringBuilder();

                if (d.Sum() > 0)
                {
                    if (d[0] > 0) sb.Append($"{d[0]}年");
                    if (d[1] > 0) sb.Append($"{d[1]}个月");
                    if (d[2] > 0) sb.Append($"{d[2]}天");
                }
                else
                {
                    int[] time = new int[2] { 0, 0 };
                    TimeSpan sj = e - b;
                    time[0] = sj.Hours;
                    time[1] = sj.Minutes % 60;
                    if (time[0] > 0) sb.Append($"{time[0]}小时");
                    if (time[1] > 0) sb.Append($"{time[1]}分钟");

                    if (time.Sum() <= 0) sb.Append($"{sj.Seconds}秒");
                }
                return sb.ToString();
            }
            else
                throw new Exception("开始日期必须小于结束日期");
        }
        
        public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
            }
            return (TEnum)Enum.Parse(typeof(TEnum), text);
        }

        
        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.Navigated += OnNavigated;
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.ExtendViewIntoTitleBar = true;

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackrequested;
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = rootFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            }


            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            //根据页面是否可以返回，在窗口显示返回按钮
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = ((Frame)sender).CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private void OnBackrequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                e.Handled = true;//这句一定要有，不然还会发生默认返回键操作
                rootFrame.GoBack();
            }
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }
    }
}
