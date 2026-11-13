using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Windows.Data.Xml.Dom;
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
using 倒计时.Manager;
using Microsoft.UI.Xaml.Controls;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItemInvokedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs;
using NavigationViewBackRequestedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs;
using NavigationViewSelectionChangedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs;
using NavigationViewDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode;
using Microsoft.Toolkit.Uwp.Helpers;
using CountdownRecord = 夏日.Models.CountdownRecord;
// NavigationViewExpandedPaneBackground

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
        static CountdownRepository countdownRepository;
        private CountdownService scheduleService;
        private CountdownSettingsService scheduleSettings;
        private int viewHeight;
        public string CurrentDate { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            countdownRepository = new CountdownRepository();
            scheduleSettings = new CountdownSettingsService(localSettings);
            scheduleService = new CountdownService(countdownRepository, scheduleSettings);
            SelectedPage = true;
            var applicationView = CoreApplication.GetCurrentView();
            applicationView.TitleBar.ExtendViewIntoTitleBar = true;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            var title = ApplicationView.GetForCurrentView().TitleBar;
            title.BackgroundColor = Colors.Transparent;
            title.InactiveBackgroundColor = Colors.Transparent;
            title.ForegroundColor = Colors.Transparent;
            title.ButtonBackgroundColor = title.ButtonInactiveBackgroundColor = Colors.Transparent;
            title.ButtonHoverBackgroundColor = Color.FromArgb(26, 0, 0, 0);
            title.ButtonPressedBackgroundColor = Color.FromArgb(51, 0, 0, 0);
            title.ButtonForegroundColor = title.ButtonHoverForegroundColor = title.ButtonPressedForegroundColor = null;
            SetThemeColor();
            GetAppVersion();
            MyNav.IsBackEnabled = false;
            SelectedPageItem = "";
            //ApplicationView.PreferredLaunchViewSize = new Size;

            localSettings.Values["mainViewId"] = ApplicationView.GetForCurrentView().Id;
            localSettings.Values["hasBeenOpened"] = "0";
            // PreferAuto so Windows restores the last main-window size.
            // Never write PreferredLaunchViewSize from the desktop tool — it is process-wide.
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
            // Main window should never inherit the desktop tool min size (330x...).
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(640, 480));
            localSettings.Values["HasOpenTool"] = "0";
            CurrentDate = DateTime.Now.ToShortDateString();
            SyncTitleBarSpacerHeight();
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += (s, a) => ApplyChromeForDisplayMode();
            ApplyChromeForDisplayMode();
            ApplyMaterialStyle();
            ActualThemeChanged += (sender, args) => ApplyMaterialStyle();
        }

        /// <summary>Keep the dedicated title-bar row aligned with the system caption height.</summary>
        private void SyncTitleBarSpacerHeight()
        {
            var coreBar = CoreApplication.GetCurrentView().TitleBar;
            double height = coreBar.Height > 0 ? coreBar.Height : 48;
            RootLayout.RowDefinitions[0].Height = new GridLength(height);
            TitleBarSpacer.Height = height;
        }

        // Keep the current acrylic cards as the default for existing installations.
        public bool UsesMica => localSettings.Values["MaterialStyle"]?.ToString() == "Mica";

        /// <summary>NavigationView default back chrome; do not resize it or it will misalign with the pane toggle.</summary>
        private Button navBackButton;

        private void ExpandBackButtonHitTarget()
        {
            try
            {
                var queue = new Queue<DependencyObject>();
                queue.Enqueue(MyNav);
                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    if (current is Button button &&
                        (button.Name == "NavigationViewBackButton" || button.Name == "BackButton"))
                    {
                        navBackButton = button;
                        return;
                    }

                    int count = VisualTreeHelper.GetChildrenCount(current);
                    for (int i = 0; i < count; i++)
                        queue.Enqueue(VisualTreeHelper.GetChild(current, i));
                }
            }
            catch { }
        }

        private static double TitleBarChromeHeight()
        {
            var coreBar = CoreApplication.GetCurrentView().TitleBar;
            return coreBar.Height > 0 ? coreBar.Height : 32;
        }

        private void SetNavBackButtonTopOffset(double top)
        {
            if (navBackButton == null)
                ExpandBackButtonHitTarget();
            if (navBackButton != null)
                navBackButton.Margin = new Thickness(0, top, 0, 0);
        }

        public void ApplyMaterialStyle()
        {
            bool mica = UsesMica;
            bool dark = ActualTheme == ElementTheme.Dark;
            Color tint = dark ? Color.FromArgb(255, 32, 32, 32) : Color.FromArgb(255, 243, 243, 243);

            // Only the shell owns the window backdrop; child pages remain transparent.
            Microsoft.UI.Xaml.Controls.BackdropMaterial.SetApplyToRootOrPageBackground(this, false);
            Background = null;
            MyNav.Background = new SolidColorBrush(Colors.Transparent);
            if (mica)
            {
                Microsoft.UI.Xaml.Controls.BackdropMaterial.SetApplyToRootOrPageBackground(this, true);
            }
            else
            {
                Background = new AcrylicBrush
                {
                    BackgroundSource = AcrylicBackgroundSource.HostBackdrop,
                    TintColor = tint,
                    TintOpacity = 0.85,
                    FallbackColor = tint
                };
            }

            // Refresh actual surfaces, including cards in the currently open page.
            Controls.MaterialSurface.Refresh();
        }

        private void GetViewHeight()
        {
            var allData = scheduleService.GetAll();
            var selectedIds = scheduleSettings.GetDesktopScheduleIds(
                allData.Select(item => new ScheduleIdentity(item.Id, item.Schedule_name)));
            int count = selectedIds.Count;
            viewHeight = count == 2 ? 230 : count == 3 ? 315 : 110;
        }

        private void ToolAutoStart()
        {
            localSettings.Values["hasBeenOpened"] = "1";
            if (localSettings.Values["ReStart"] != null && localSettings.Values["ReStart"].ToString() == "1")
            {
                CreateTool();
                localSettings.Values["ReStart"] = "0";
                return;
            }
            if (localSettings.Values["ToolAutoStart"] != null && localSettings.Values["ToolAutoStart"].ToString() == "1")
                CreateTool();
        }


        private void EnsureMainWindowChrome()
        {
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(640, 480));
        }
        public async void CreateTool()
        {
            //GetViewHeight();
            localSettings.Values["pip"] = "1";
            var num = CoreApplication.Views.Count();
            //if (localSettings.Values["newViewId"] != null)
            //    ApplicationViewSwitcher.SwitchAsync(Convert.ToInt32(localSettings.Values["newViewId"])).Close();
            if (localSettings.Values["ItemCount"] == null)
                localSettings.Values["ItemCount"] = 0;
            var allData = scheduleService.GetAll();
            var selectedIds = scheduleSettings.GetDesktopScheduleIds(
                allData.Select(item => new ScheduleIdentity(item.Id, item.Schedule_name)));
            int count = selectedIds.Count;
            if (count == 0) return;
            List<CountdownRecord> datalist = allData.Where(item => selectedIds.Contains(item.Id)).ToList();
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
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                try
                {
                    var view = ApplicationView.GetForCurrentView();
                    var prop = typeof(ApplicationView).GetProperty("IsIconVisibleInTaskBar")
                        ?? typeof(ApplicationView).GetProperty("IsIconVisibleOnTaskBar");
                    if (prop != null && prop.CanWrite)
                        prop.SetValue(view, false);
                }
                catch { }
            });
            localSettings.Values["DesktopPin"] = false;
            EnsureMainWindowChrome();
        }

        public void SetThemeColor()
        {
            TC.Color = App.ApplySavedAccentColor();
            ResetTitleBar();
        }

        public void ApplyImmersiveTitleBar(Brush pageBackground, Color captionColor)
        {
            // Let the content host fill under the system title bar so page background is continuous.
            MyNav.IsTitleBarAutoPaddingEnabled = false;

            var title = ApplicationView.GetForCurrentView().TitleBar;
            title.BackgroundColor = Colors.Transparent;
            title.InactiveBackgroundColor = Colors.Transparent;
            title.ForegroundColor = Colors.White;
            // Caption buttons sit on the immersive page color — keep them transparent.
            title.ButtonBackgroundColor = Colors.Transparent;
            title.ButtonInactiveBackgroundColor = Colors.Transparent;
            title.ButtonHoverBackgroundColor = Color.FromArgb(40, 255, 255, 255);
            title.ButtonPressedBackgroundColor = Color.FromArgb(70, 255, 255, 255);
            title.ButtonForegroundColor = Colors.White;
            title.ButtonHoverForegroundColor = Colors.White;
            title.ButtonPressedForegroundColor = Colors.White;
            title.ButtonInactiveForegroundColor = Color.FromArgb(160, 255, 255, 255);
        }

        /// <summary>Official spacing: NavigationView sits below the dedicated title-bar row (expanded).</summary>
        public void ApplyLightImmersiveTitleBar()
        {
            ApplyChromeForDisplayMode();

            var title = ApplicationView.GetForCurrentView().TitleBar;
            title.BackgroundColor = Colors.Transparent;
            title.InactiveBackgroundColor = Colors.Transparent;
            title.ForegroundColor = Colors.Transparent;
            title.ButtonBackgroundColor = Colors.Transparent;
            title.ButtonInactiveBackgroundColor = Colors.Transparent;
            title.ButtonHoverBackgroundColor = Color.FromArgb(20, 0, 0, 0);
            title.ButtonPressedBackgroundColor = Color.FromArgb(40, 0, 0, 0);
            title.ButtonForegroundColor = null;
            title.ButtonHoverForegroundColor = null;
            title.ButtonPressedForegroundColor = null;
            title.ButtonInactiveForegroundColor = null;
        }

        /// <summary>Kept for call sites; official spacing does not paint the title-bar strip.</summary>
        public void ApplyTitleBarTint(Color color)
        {
            ApplyLightImmersiveTitleBar();
        }

        public void ResetTitleBar()
        {
            ApplyChromeForDisplayMode();
            SetNavBackButtonTopOffset(0);

            var title = ApplicationView.GetForCurrentView().TitleBar;
            title.BackgroundColor = Colors.Transparent;
            title.InactiveBackgroundColor = Colors.Transparent;
            title.ForegroundColor = Colors.Transparent;
            title.ButtonBackgroundColor = title.ButtonInactiveBackgroundColor = Colors.Transparent;
            title.ButtonHoverBackgroundColor = Color.FromArgb(26, 0, 0, 0);
            title.ButtonPressedBackgroundColor = Color.FromArgb(51, 0, 0, 0);
            title.ButtonForegroundColor = title.ButtonHoverForegroundColor = title.ButtonPressedForegroundColor = null;
            title.ButtonInactiveForegroundColor = null;
        }

        private void MyNav_DisplayModeChanged(NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            ApplyChromeForDisplayMode();
        }

        private void RootLayout_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplyChromeForDisplayMode();
        }

        /// <summary>
        /// Always keep the dedicated title-bar row.
        /// In compact/minimal mode, add top padding on ContentFrame so every page
        /// keeps a safe area under the back/hamburger chrome.
        /// </summary>
        private void ApplyChromeForDisplayMode()
        {
            MyNav.IsTitleBarAutoPaddingEnabled = false;
            MyNav.Margin = new Thickness(0);
            SyncTitleBarSpacerHeight();

            string mode = MyNav.DisplayMode.ToString();
            bool compact = mode == "Minimal" || mode == "Compact" || mode == "LeftCompact"
                || ActualWidth < MyNav.CompactModeThresholdWidth;
            double safeTop = compact ? Math.Max(48, TitleBarChromeHeight()) : 0;
            ContentFrame.Padding = new Thickness(0, safeTop, 0, 0);
            ContentFrame.Margin = new Thickness(0);
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
          ("ToDo", typeof(ToDo)),
          ("New", typeof(Add)),
          ("Calculator", typeof(Calculator)),
          ("Festival", typeof(Festival)),
          ("Details",typeof(Details)),
          ("Desktop",typeof(Desktop)),
          ("Feedback",typeof(Feedback)),
          ("Timer", typeof(TimerPage)),
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
            //ToolAutoStart();
            // Add handler for ContentFrame navigation.
            ContentFrame.Navigated += On_Navigated;

            ExpandBackButtonHitTarget();
            ApplyChromeForDisplayMode();

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

        private void MyNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            FeedbackItem.IsSelected = false;
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
            if (SelectedPageItem.Equals("Add")
                || SelectedPageItem.Equals("ToDo")
                || SelectedPageItem.Equals("Calculator")
                || SelectedPageItem.Equals("Festival")
                || SelectedPageItem.Equals("Settings")
                || SelectedPageItem.Equals("Desktop")
                || SelectedPageItem.Equals("Feedback")
                || SelectedPageItem.Equals("Timer"))
            {
                FeedbackItem.IsSelected = false;
                DesktopItem.IsSelected = false;
                NewItem.IsSelected = false;
                CalculatorItem.IsSelected = false;
                FestivalItem.IsSelected = false;
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
                if (SelectedPageItem.Equals("Details") && SelectedPage == true)
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
            //throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
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
                if (navItemTag == "Festival")
                {
                    MyNav.IsBackEnabled = true;
                    SelectedPageItem = "Festival";
                }
            }
        }


        private async void On_Navigated(object sender, NavigationEventArgs e)
        {
            if (localSettings.Values["hasBeenOpened"] == null)
                localSettings.Values["hasBeenOpened"] = "0";
            if (localSettings.Values["hasBeenOpened"].ToString() == "0")
                ToolAutoStart();
            var version = Package.Current.Id.Version;
            string versionKey = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            if (localSettings.Values[versionKey]?.ToString() != "true")
            {
                bool firstLaunch = localSettings.Values[versionKey] == null;
                bool hasCurrentUpdate = false;
                try
                {
                    using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(8) })
                    {
                        var xml = await client.GetStringAsync("https://www.monreeing.com/notice/monreedate_version_info.xml");
                        var document = new XmlDocument();
                        document.LoadXml(xml);
                        var entries = document.GetElementsByTagName("content");
                        var updates = new List<KeyValuePair<System.Version, string>>();
                        for (uint i = 0; i < entries.Length; i++)
                        {
                            string text = entries.Item(i).InnerText.Trim();
                            string heading = text.Split('\n')[0].Trim().TrimEnd('\r');
                            if (heading.StartsWith("V", StringComparison.OrdinalIgnoreCase)
                                && System.Version.TryParse(heading.Substring(1), out var entryVersion))
                                updates.Add(new KeyValuePair<System.Version, string>(entryVersion, text));
                        }
                        var currentVersion = version.Revision == 0
                            ? new System.Version(version.Major, version.Minor, version.Build)
                            : new System.Version(version.Major, version.Minor, version.Build, version.Revision);
                        string currentUpdate = updates.FirstOrDefault(item => item.Key.CompareTo(currentVersion) == 0).Value;
                        hasCurrentUpdate = !string.IsNullOrEmpty(currentUpdate);
                        if (hasCurrentUpdate)
                        {
                            int firstLineEnd = currentUpdate.IndexOf('\n');
                            currentUpdate = firstLineEnd >= 0 ? currentUpdate.Substring(firstLineEnd + 1).Trim() : string.Empty;
                        }
                        var history = updates.Where(item => item.Key.CompareTo(currentVersion) < 0)
                            .OrderByDescending(item => item.Key)
                            .Take(3)
                            .Select(item => "往期版本：" + item.Value);
                        WelcomeVersionInfo.Text = string.Join("\n\n",
                            new[] { currentUpdate ?? "当前版本暂无更新内容。" }.Concat(history));
                    }
                }
                catch (Exception)
                {
                    WelcomeVersionInfo.Text = "暂时无法获取版本更新内容，请稍后重试。";
                }
                if (firstLaunch || hasCurrentUpdate)
                {
                    await MyCD.ShowAsync();
                    localSettings.Values[versionKey] = hasCurrentUpdate ? "true" : "false";
                }
            }
            //MyNav.IsBackEnabled = ContentFrame.CanGoBack;

            if (ContentFrame.SourcePageType == typeof(Settings))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                MyNav.SelectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)MyNav.SettingsItem;
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
            string appVersion = string.Format("此版本： V{0}.{1}.{2}",
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build);
            Version.Text = appVersion;
        }

        private void MyCD_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private void FeedbackItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SelectedPageItem = "Feedback";
            MyNav.SelectedItem = FeedbackItem;
            ContentFrame.Navigate(typeof(Feedback));
        }

        private void MyCDPrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            MyCD.Hide();
        }

        private async void MyCDSecondaryButton_Click(object sender, RoutedEventArgs e)
        {
            var Uri = new Uri("ms-windows-store://review/?productid=9PKBWKPCCFJ8");
            await Launcher.LaunchUriAsync(Uri);
            MyCD.Hide();
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (ContentFrame.SourcePageType == typeof(Details))
                return;
            ResetTitleBar();
        }
    }
}
