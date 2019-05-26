using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace 倒计时
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        public CustomDataViewModel ViewModel = new CustomDataViewModel();
        public ObservableCollection<CustomData> CustomDatas = new ObservableCollection<CustomData>();
        private const string SelectedAppThemeKey = "SelectedAppTheme";
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
            this.RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6))
            {
                this.FocusVisualKind = AnalyticsInfo.VersionInfo.DeviceFamily == "Xbox" ? FocusVisualKind.Reveal : FocusVisualKind.HighVisibility;
            }
        }

        public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
            }
            return (TEnum)Enum.Parse(typeof(TEnum), text);
        }

        public class CustomData
        {
            public string Str1 { get; set; }
            public string Str2 { get; set; }
            public string Str3 { get; set; }
            public int ItemWidth { get; set; }
            public string BackGroundColor { get; set; }

            public CustomData()
            {
                this.Str1 = "string 1";
                this.Str2 = "string 2";
                this.Str3 = "string 3";
                this.BackGroundColor = "SkyBlue";
            }

            static public CustomData DateCalculator(string D, string E)
            {
                string s1 = E;
                string s2;
                string d = D;
                string str2 = DateTime.Now.ToShortDateString().ToString();
                DateTime d1 = Convert.ToDateTime(d);
                DateTime d2 = Convert.ToDateTime(str2);
                DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
                DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));
                int days = (d4 - d3).Days;
                if (days < 0)
                {
                    days = Math.Abs(days);
                    s2 = "已过" + days.ToString() + "天";
                }
                else
                    s2 = "还有" + days.ToString() + "天";
                return new CustomData()
                {
                    Str1 = s1,
                    Str2 = s2,
                    Str3 = d
                };
            }
        }

        public class CustomDataViewModel
        {
            public ObservableCollection<CustomData> CustomDatas = new ObservableCollection<CustomData>();

            public CustomDataViewModel()
            {
                CustomDatas.Add(new CustomData() { Str1 = "测试测试", Str2 = "已过xx天", Str3 = "201X/XX/XX", BackGroundColor = "SkyBlue" });
                CustomDatas.Add(new CustomData() { Str1 = "测试测试", Str2 = "已过xx天", Str3 = "201X/XX/XX", BackGroundColor = "Pink" });
                CustomDatas.Add(new CustomData() { Str1 = "测试测试", Str2 = "已过xx天", Str3 = "201X/XX/XX", BackGroundColor = "CornFlowerBlue" });
                CustomDatas.Add(new CustomData() { Str1 = "测试测试", Str2 = "已过xx天", Str3 = "201X/XX/XX", BackGroundColor = "Lavender" });
                CustomDatas.Add(new CustomData() { Str1 = "测试测试", Str2 = "已过xx天", Str3 = "201X/XX/XX", BackGroundColor = "Azure" });
                CustomDatas.Add(new CustomData() { Str1 = "测试测试", Str2 = "已过xx天", Str3 = "201X/XX/XX", BackGroundColor = "Purple" });
            }

            public void AddData(CustomData data)
            {
                CustomDatas.Add(data);
            }
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

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
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
