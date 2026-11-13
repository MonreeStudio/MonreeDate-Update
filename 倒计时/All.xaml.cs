using SQLite.Net.Platform.WinRT;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using SQLite.Net;
using SQLite.Net.Attributes;
using SQLite.Net.Interop;
using 夏日.Models;
using static 倒计时.App;
using Windows.UI;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications; // Notifications library
using Windows.Data.Xml.Dom;
using Windows.UI.StartScreen;
using Windows.UI.Core;
using 倒计时.Models;
using 倒计时.Manager;
using Windows.UI.Xaml.Media.Imaging;
using BackgroundTasks;
using Microsoft.UI.Xaml.Controls;
using CountdownRecord = 夏日.Models.CountdownRecord;


// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 倒计时
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class All : Page
    {
        string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "mydb.sqlite");    //建立数据库
        public SQLite.Net.SQLiteConnection conn;
        public CustomDataViewModel ViewModel = new CustomDataViewModel();
        public string str1, str2, str3;
        public AcrylicBrush str4;
        public Color BgsColor;
        private double MinMyNav = MainPage.Current.MyNav.CompactModeThresholdWidth;
        public static All Current;
        public CountdownRepository CountdownRepository { get; private set; }
        internal CountdownService ScheduleService { get; private set; }
        internal CountdownSettingsService ScheduleSettings { get; private set; }
        internal CountdownTileService TileService { get; private set; }
        public string Model_event;
        public string Model_Date;
        //public CustomData SelectedItem;
        public String dbname;
        public int _index;
        private double percentage;
        private bool TopTap;
        private DispatcherTimer timer;
        private DateTime lastRefreshDate = DateTime.Today;
        private Storyboard viewIndicatorAnimation;
        private Storyboard viewContentAnimation;
        private string activeHomeView;
        private bool currentGroupExpanded = true;
        private bool upcomingGroupExpanded = true;
        private bool pastGroupExpanded = true;
        private int currentRangeCount;
        private int currentPointCount;
        private int upcomingRangeCount;
        private int upcomingPointCount;
        private int pastRangeCount;
        private int pastPointCount;
        private Storyboard currentGroupAnimation;
        private Storyboard upcomingGroupAnimation;
        private Storyboard pastGroupAnimation;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public All()
        {
            this.InitializeComponent();
            CountdownRepository = new CountdownRepository();
            ScheduleSettings = new CountdownSettingsService(localSettings);
            ScheduleService = new CountdownService(CountdownRepository, ScheduleSettings);
            ScheduleSettings.MigrateLegacySettings(ScheduleService.GetAll()
                .OrderBy(record => record.Id, StringComparer.Ordinal)
                .Select(record => new ScheduleIdentity(record.Id, record.Schedule_name, record.Date)));
            TileService = new CountdownTileService(ScheduleService, ScheduleSettings);
            conn = CountdownRepository.Connection;
            Current = this;
            ScheduleList.ItemsSource = ViewModel.CustomDatas;
            ScheduleList.ContextFlyout = CurrentSchedules.ContextFlyout = CurrentPointSchedules.ContextFlyout
                = UpcomingSchedules.ContextFlyout = UpcomingPointSchedules.ContextFlyout
                = PastSchedules.ContextFlyout = PastPointSchedules.ContextFlyout = MyMenuFlyout;
            LoadAllPage();
            ApplyHomeView();
            MainPage.Current.MyNav.IsBackEnabled = false;
            MainPage.Current.SelectedPageItem = "All";
            NavigationCacheMode = NavigationCacheMode.Enabled;
            localSettings.Values["newMainId"] = "0";
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;//每秒触发这个事件，以刷新指针
            timer.Start();
            localSettings.Values["mainViewId"] = ApplicationView.GetForCurrentView().Id;
            localSettings.Values["newMain"] = "0";
        }

        private void Timer_Tick(object sender, object e)
        {
            //SetToptext();

            if (DateTime.Today != lastRefreshDate)
            {
                lastRefreshDate = DateTime.Today;
                LoadAllPage();
            }
        }

        public void LoadAllPage()
        {
            lastRefreshDate = DateTime.Today;
            SetToptext();
            SetTitle();
            LoadSettings();
            LoadDateData();
            LoadTile();
            SetThemeColor();
            SetPersonPicture();
        }

        private async void SetPersonPicture()
        {
            try
            {
                List<PersonPictures> datalist = conn.Query<PersonPictures>("select * from PersonPictures where pictureName = ?", "picture");
                foreach (var item in datalist)
                {
                    try
                    {
                        var bitmap = await AvatarImage.LoadAsync(item.picture);
                        AllPicture.ProfilePicture = bitmap;
                    }
                    catch (ArgumentNullException ex)
                    {
                        throw ex;
                    }
                }
            }
            catch { }
        }

        public void SetThemeColor()
        {
            TC.Color = App.ApplySavedAccentColor();
            MyProgressBar.Foreground = TC;
            MyProgressBar.Background = new SolidColorBrush(Color.FromArgb(32, TC.Color.R, TC.Color.G, TC.Color.B));
        }

        private void SetToptext()
        {
            if (localSettings.Values["TopTap"] == null)
                localSettings.Values["TopTap"] = "1";
            Today.Text = DateTime.Now.ToString("yyyy年M月d日 · dddd");
            percentage = 100.0 * DateTime.Today.DayOfYear / (DateTime.IsLeapYear(DateTime.Today.Year) ? 366 : 365);
            if (localSettings.Values["TopTap"].ToString() == "0")
            {
                TopText.Text = "今年已走过 " + ((int)percentage).ToString() + "%";
            }
            else
            {
                TopText.Text = "今年已走过 " + DateTime.Now.DayOfYear.ToString() + " 天";
            }
            MyProgressBar.Value = percentage;
        }

        private void SetTitle()
        {
            var applicationView = CoreApplication.GetCurrentView();
            applicationView.TitleBar.ExtendViewIntoTitleBar = true;
            var title = ApplicationView.GetForCurrentView().TitleBar;
            title.BackgroundColor = Colors.Transparent;
            title.InactiveBackgroundColor = Colors.Transparent;
            title.ForegroundColor = Colors.Transparent;
            title.ButtonBackgroundColor = title.ButtonInactiveBackgroundColor = Colors.Transparent;
            title.ButtonHoverBackgroundColor = Color.FromArgb(26, 0, 0, 0);
            title.ButtonPressedBackgroundColor = Color.FromArgb(51, 0, 0, 0);
            title.ButtonForegroundColor = title.ButtonHoverForegroundColor = title.ButtonPressedForegroundColor = null;
        }

        public async void LoadTile()
        {
            try { await TileService.RefreshAllAsync(); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }
        }

        /* TileContent construction moved to CountdownTileService so the
           foreground and background task use one adaptive template. */
        private void CreateTile(string scheduleName, string calculatedDate, string date,
            string displayDates = null, string scheduleId = null)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Update(new TileNotification(
                CountdownTileService.BuildTileContent(scheduleName, calculatedDate,
                    displayDates ?? date, "夏日").GetXml()));
        }

        /* Legacy call sites can request a secondary refresh without owning
           SecondaryTile IDs or Windows prompts. */
        public void CreateSecondaryTile(string scheduleName, string calculatedDate, string date,
            string displayDates = null, string scheduleId = null)
        {
            if (string.IsNullOrWhiteSpace(scheduleId)) return;
            _ = TileService.UpdateScheduleAsync(scheduleId);
        }

        public void LoadDateData()
        {
            AllProgressRing.IsActive = true;
            AllScrollViewer.ChangeView(null, 0, null);
            if (localSettings.Values["DisplayMode"] == null)
                localSettings.Values["DisplayMode"] = "All";
            var dm = localSettings.Values["DisplayMode"].ToString();
            if (dm == "Upcoming")
            {
                dm = "Future";
                localSettings.Values["DisplayMode"] = dm;
            }
            switch (dm)
            {
                case "All":
                    LoadAll();
                    break;
                case "Past":
                    LoadPast();
                    break;
                case "Future":
                    LoadFuture();
                    break;
                case "Current":
                    ContentChoise.Label = "进行中 / 今天";
                    PopulateSchedules(ScheduleService.GetPinnedOrderByAddTime(), ScheduleService.GetAll(), dm);
                    break;
                default:
                    localSettings.Values["DisplayMode"] = "All";
                    LoadAll();
                    break;
            }
            AllProgressRing.IsActive = false;
        }

        private void LoadAll()
        {
            ContentChoise.Label = "显示全部";
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            PopulateSchedules(ScheduleService.GetPinnedOrderByAddTime(),
                ScheduleService.GetFuture(today).Concat(ScheduleService.GetPast(today)), "All");
        }

        private void LoadPast()
        {
            ContentChoise.Label = "显示已过日程";
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            PopulateSchedules(ScheduleService.GetPinnedPast(today), ScheduleService.GetPast(today), "Past");
        }

        private void LoadFuture()
        {
            ContentChoise.Label = "显示未过日程";
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            PopulateSchedules(ScheduleService.GetPinnedFuture(today), ScheduleService.GetFuture(today), "Future");
        }

        private void PopulateSchedules(IEnumerable<CountdownRecord> pinned, IEnumerable<CountdownRecord> records, string displayMode)
        {
            string mode = localSettings.Values["DateMode"] as string ?? "Day";
            localSettings.Values["DateMode"] = mode;
            ChangeABB.Label = mode == "Week" ? "周数模式" : mode == "Year" ? "年数模式" : "天数模式";
            ViewModel.CustomDatas.Clear();
            var addedIds = new HashSet<string>();
            var items = new List<CustomData>();
            foreach (var record in pinned.Concat(records))
            {
                if (record != null && addedIds.Add(record.Id))
                {
                    var data = CreateDisplayData(record, mode);
                    if ((displayMode == "Current" || displayMode == "Upcoming") && data.GroupKey != displayMode) continue;
                    items.Add(data);
                }
            }
            bool byName = localSettings.Values["HomeSort"] as string == "Name";
            SortButton.Label = byName ? "名称排序" : "日期排序";
            var ordered = items.OrderByDescending(item => item.IsPinned);
            IEnumerable<CustomData> sorted = byName ? ordered.ThenBy(item => item.Str1, StringComparer.CurrentCultureIgnoreCase)
                : ordered.ThenBy(item => item.GroupKey == "Current" ? 0 : item.GroupKey == "Upcoming" ? 1 : 2)
                    .ThenBy(item => item.GroupKey == "Past" ? -DateTime.Parse(item.Presentation.IsRange ? item.EndDate : item.Str3).Ticks
                        : DateTime.Parse(item.Presentation.IsRange ? item.EndDate : item.Str3).Ticks);
            foreach (var item in sorted) ViewModel.CustomDatas.Add(item);
            RefreshHomeGroups();

            bool isEmpty = ViewModel.CustomDatas.Count == 0;
            NewTB.Text = displayMode == "All" ? "创建你的第一个日程吧！" : "这里空空如也~";
            NewTB2.Text = displayMode == "All" ? "（创建后可右键删除）"
                : displayMode == "Past" ? "（暂无已过日程）" : "（暂无未过日程）";
            NewTB.Visibility = NewTB2.Visibility = isEmpty ? Visibility.Visible : Visibility.Collapsed;
            EmptyStateIcon.Visibility = isEmpty ? Visibility.Visible : Visibility.Collapsed;
        }

        public CustomData CreateDisplayData(CountdownRecord record, string mode = "Day")
        {
            Color color = GetColor(record.BgColor);
            return new CustomData
            {
                RecordId = record.Id,
                Str1 = record.Schedule_name,
                Str2 = ScheduleDateCalculator.FormatCountdown(record.Date, record.EndDate, record.ScheduleType, mode),
                Str3 = record.Date,
                ScheduleType = record.ScheduleType,
                EndDate = record.EndDate,
                IsPinned = record.IsTop == "1",
                Presentation = ScheduleService.Present(record, mode),
                Str4 = ColorfulBrush(color, record.TintOpacity),
                BackGroundColor = color
            };
        }
        public void LoadSettings()
        {
            UserName.Visibility = Visibility.Visible;
            UserName.Text = localSettings.Values["NickName"] as string ?? "游客";
            UserSign.Text = localSettings.Values["Sign"] as string ?? "";
            UserSign.Visibility = string.IsNullOrWhiteSpace(UserSign.Text) ? Visibility.Collapsed : Visibility.Visible;
            ToolTipService.SetToolTip(UserSign, UserSign.Text);
            AllPicture.Visibility = localSettings.Values["SetAllPersonPicture"] is bool visible && !visible
                ? Visibility.Collapsed : Visibility.Visible;
            AllCommandBar.Margin = new Thickness(0);
            MarginText.Height = 12;
            TopText.Margin = MyProgressBar.Margin = new Thickness(0);
            _ = TileService.RefreshSecondaryTilesAsync();
        }

        public AcrylicBrush ColorfulBrush(Color temp,double tintOpacity)
        {
            AcrylicBrush myBrush = new AcrylicBrush();
            myBrush.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
            myBrush.TintColor = temp;
            myBrush.FallbackColor = temp;
            // Honor the opacity saved by the schedule's background slider.
            myBrush.TintOpacity = Math.Max(0, Math.Min(1, tintOpacity));
            return myBrush;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            // The add page is opened from the home page and has no visible navigation item.
            // Keep the current navigation selection instead of forcing a tool item.
            Frame.Navigate(typeof(Add));
        }

        private void MyGirdView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var _item = (CustomData)e.ClickedItem;
            AllItem = _item;
            str1 = _item.Str1;
            str2 = _item.Str2;
            str3 = _item.Str3;
            str4 = _item.Str4;
            BgsColor = _item.BackGroundColor;
            MainPage.Current.SelectedPage = true;
            Frame.Navigate(typeof(Details), null, new DrillInNavigationTransitionInfo());
        }

        private void MyGirdView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string key = e.AddedItems[0].ToString();

            }
        }

        private string ConvertToYMD(String _date)
        {
            return CountdownDateCalculator.FormatYearMonthDayCountdown(_date);
        }

        public string ConvertToWeek(string _date)
        {
            return CountdownDateCalculator.FormatWeekCountdown(_date);
        }

        private void MyGirdView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var _item = (e.OriginalSource as FrameworkElement)?.DataContext as CustomData;
            PrepareScheduleMenu(_item);
        }

        private void Schedule_ContextRequested(UIElement sender, ContextRequestedEventArgs args)
        {
            var source = args.OriginalSource as FrameworkElement;
            Point position;
            if (!args.TryGetPosition(sender, out position))
                source = FocusManager.GetFocusedElement() as FrameworkElement;
            PrepareScheduleMenu(source?.DataContext as CustomData);
        }

        private void PrepareScheduleMenu(CustomData _item)
        {
            List<CountdownRecord> toplist = ScheduleService.GetPinned();
            AllItem = _item;
            if (AllItem != null)
            {
                bool isMainTile = ScheduleSettings.GetMainTileId(ScheduleService.GetAll()
                    .Select(item => new ScheduleIdentity(item.Id, item.Schedule_name))) == AllItem.RecordId;
                if (isMainTile)
                {
                    PinToSC.Visibility = Visibility.Collapsed;
                    unPinToSC.Visibility = Visibility.Collapsed;
                    FS2.Visibility = Visibility.Visible;
                    MainTile.Visibility = Visibility.Visible;
                }
                else
                {
                    MainTile.Visibility = Visibility.Collapsed;
                    bool isPinned = TileService.IsPinned(AllItem.RecordId);
                    if (!isPinned)
                    {
                        PinToSC.Visibility = Visibility.Visible;
                        FS2.Visibility = Visibility.Visible;
                        unPinToSC.Visibility = Visibility.Collapsed;
                        ScheduleSettings.SetTileState(AllItem.RecordId, AllItem.Str1, false);
                    }
                    else
                    {
                        if (isPinned && !isMainTile)
                        {
                            PinToSC.Visibility = Visibility.Collapsed;
                            unPinToSC.Visibility = Visibility.Visible;
                            FS2.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            PinToSC.Visibility = Visibility.Visible;
                            unPinToSC.Visibility = Visibility.Collapsed;
                            FS2.Visibility = Visibility.Visible;
                        }
                    }
                }
                MyFlyout.IsEnabled = true;
                if (toplist.Count() == 0)
                {
                    FS.Visibility = Visibility.Visible;
                    SetTop.Visibility = Visibility.Visible;
                    SetTop.IsEnabled = true;
                    Cancel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    bool in_toplist = false;
                    foreach(var item in toplist)
                    {
                        if (item.Id == AllItem.RecordId)
                            in_toplist = true;
                    }
                    if (in_toplist==true)
                    {
                        FS.Visibility = Visibility.Visible;
                        SetTop.Visibility = Visibility.Collapsed;
                        Cancel.Visibility = Visibility.Visible;
                        Cancel.IsEnabled = true;
                    }
                    else
                    {
                        SetTop.Visibility = Visibility.Visible;
                        Cancel.Visibility = Visibility.Collapsed;
                        FS.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                MyFlyout.IsEnabled = false;
                MainTile.Visibility = Visibility.Collapsed;
                FS.Visibility = Visibility.Collapsed;
                FS2.Visibility = Visibility.Collapsed;
                PinToSC.Visibility = Visibility.Collapsed;
                unPinToSC.Visibility = Visibility.Collapsed;
                SetTop.Visibility = Visibility.Collapsed;
                Cancel.Visibility = Visibility.Collapsed;
            }

        }

        private void MyGridView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //AbbFlyout.Hide();
        }

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await DeleteDialog.ShowAsync();
        }

        private void TopText_Tapped(object sender, TappedRoutedEventArgs e)
        {
            localSettings.Values["TopTap"] = localSettings.Values["TopTap"] as string == "1" ? "0" : "1";
            SetToptext();
        }

        private void SetTop_Click(object sender, RoutedEventArgs e)
        {
            if (AllItem == null) return;
            ScheduleService.SetPinnedById(AllItem.RecordId, true);
            LoadDateData();
            LoadSettings();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (AllItem == null) return;
            ScheduleService.SetPinnedById(AllItem.RecordId, false);
            LoadDateData();
            LoadSettings();
        }

        private void DeleteDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private async void PinToSC_Click(object sender, RoutedEventArgs e)
        {
            bool isPinned = await TileService.PinAsync(AllItem.RecordId);
            if (isPinned)
            {
                PinToSC.Visibility = Visibility.Collapsed;
                unPinToSC.Visibility = Visibility.Visible;
                ScheduleSettings.SetTileState(AllItem.RecordId, AllItem.Str1, true);
                PopupNotice popupNotice = new PopupNotice("日程固定成功");
                popupNotice.ShowAPopup();
            }
            LoadDateData();
            LoadSettings();
            LoadTile();
        }

        private async void unPinToSC_Click(object sender, RoutedEventArgs e)
        {
            bool deleted = await TileService.UnpinAsync(AllItem.RecordId);
            if (!deleted) return;
            PinToSC.Visibility = Visibility.Visible;
            unPinToSC.Visibility = Visibility.Collapsed;
            ScheduleSettings.SetTileState(AllItem.RecordId, AllItem.Str1, false);
            PopupNotice popupNotice = new PopupNotice("日程已取消固定");
            popupNotice.ShowAPopup();
            LoadDateData();
            LoadSettings();
            LoadTile();
        }

        private void GridViewStackPanel_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            AnimateScheduleHover(sender as FrameworkElement, true);
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 0);
        }

        private void GridViewStackPanel_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            AnimateScheduleHover(sender as FrameworkElement, false);
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
        }

        private static void AnimateScheduleHover(FrameworkElement element, bool isHovered)
        {
            if (element == null) return;

            var transform = element.RenderTransform as CompositeTransform;
            if (transform == null)
            {
                transform = new CompositeTransform();
                element.RenderTransform = transform;
            }

            var animation = new DoubleAnimation
            {
                To = isHovered ? -2 : 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(140)),
                EnableDependentAnimation = true,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(animation, transform);
            Storyboard.SetTargetProperty(animation, "TranslateY");

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        private void TopText_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 0);
        }

        private void TopText_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
        }

        private void AllPicture_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            UserName.Visibility = Visibility.Visible;
            UserSign.Visibility = Visibility.Visible;
        }

        private void AllPicture_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            UserName.Visibility = Visibility.Collapsed;
            UserSign.Visibility = Visibility.Collapsed;
        }

        private void AllPicture_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainPage.Current.MyNav.SelectedItem = MainPage.Current.MyNav.SettingsItem;
            Frame.Navigate(typeof(Settings));
        }

        private void DisplayAll_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["DisplayMode"] = "All";
            LoadDateData();
        }

        private void DisplayCurrent_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["DisplayMode"] = "Current";
            LoadDateData();
        }

        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["HomeSort"] = (sender as FrameworkElement)?.Tag as string ?? "Date";
            LoadDateData();
        }

        private void ViewMode_Click(object sender, RoutedEventArgs e)
        {
            string next = (sender as FrameworkElement)?.Tag as string ?? "Cards";
            bool changed = next != activeHomeView;
            localSettings.Values["HomeViewMode"] = next;
            ApplyHomeView(changed);
            if (changed) AllScrollViewer.ChangeView(null, 0, null, true);
        }

        private void ApplyHomeView(bool animate = false)
        {
            string view = localSettings.Values["HomeViewMode"] as string;
            if (view != "List" && view != "Groups") view = "Cards";
            localSettings.Values["HomeViewMode"] = view;
            int direction = HomeViewIndex(view) >= HomeViewIndex(activeHomeView) ? 1 : -1;
            activeHomeView = view;
            CardsViewButton.IsChecked = view == "Cards";
            ListViewButton.IsChecked = view == "List";
            GroupsViewButton.IsChecked = view == "Groups";
            MyGridView.Visibility = view == "Cards" ? Visibility.Visible : Visibility.Collapsed;
            ScheduleList.Visibility = view == "List" ? Visibility.Visible : Visibility.Collapsed;
            GroupedSchedules.Visibility = view == "Groups" ? Visibility.Visible : Visibility.Collapsed;
            UpdateViewIndicator(animate);
            if (animate) AnimateHomeContent(direction);
        }

        private static int HomeViewIndex(string view) => view == "List" ? 1 : view == "Groups" ? 2 : 0;

        private void ViewTabs_Loaded(object sender, RoutedEventArgs e) => UpdateViewIndicator(false);
        private void ViewTabs_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateViewIndicator(false);

        private void UpdateViewIndicator(bool animate)
        {
            if (ViewTabs == null || ViewIndicatorTransform == null) return;
            var button = activeHomeView == "List" ? ListViewButton : activeHomeView == "Groups" ? GroupsViewButton : CardsViewButton;
            if (button == null || button.ActualWidth <= 0) return;
            double target = button.TransformToVisual(ViewTabs).TransformPoint(new Point()).X
                + (button.ActualWidth - ViewIndicator.Width) / 2;
            double previous = ViewIndicatorTransform.X;
            viewIndicatorAnimation?.Stop();
            ViewIndicatorTransform.X = target;
            if (!animate || !new UISettings().AnimationsEnabled) return;

            viewIndicatorAnimation = new Storyboard();
            AddViewAnimation(viewIndicatorAnimation, ViewIndicatorTransform, "X", previous, target, 220);
            viewIndicatorAnimation.Begin();
        }

        private void AnimateHomeContent(int direction)
        {
            viewContentAnimation?.Stop();
            ScheduleViewHost.Opacity = 1;
            ScheduleViewTransform.X = 0;
            if (!new UISettings().AnimationsEnabled) return;
            viewContentAnimation = new Storyboard();
            AddViewAnimation(viewContentAnimation, ScheduleViewHost, "Opacity", 0, 1, 240);
            AddViewAnimation(viewContentAnimation, ScheduleViewTransform, "X", direction * 18, 0, 240);
            viewContentAnimation.Begin();
        }

        private static void AddViewAnimation(Storyboard storyboard, DependencyObject target, string property,
            double from, double to, int milliseconds)
        {
            var animation = new DoubleAnimation
            {
                From = from, To = to, Duration = TimeSpan.FromMilliseconds(milliseconds),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                FillBehavior = FillBehavior.Stop
            };
            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, property);
            storyboard.Children.Add(animation);
        }

        private void ScheduleViewHost_Unloaded(object sender, RoutedEventArgs e)
        {
            viewIndicatorAnimation?.Stop();
            viewContentAnimation?.Stop();
            ScheduleViewHost.Opacity = 1;
            ScheduleViewTransform.X = 0;
        }

        private void HomeSummary_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (YearSummary == null) return;
            bool narrow = e.NewSize.Width < 660;
            Grid.SetColumn(YearSummary, narrow ? 0 : 2);
            Grid.SetColumnSpan(YearSummary, narrow ? 3 : 1);
            Grid.SetRow(YearSummary, narrow ? 1 : 0);
            YearSummary.Width = narrow ? double.NaN : Math.Min(360, Math.Max(280, e.NewSize.Width * 0.4));
            YearSummary.Margin = narrow ? new Thickness(0, 12, 0, 0) : new Thickness(0);
        }

        private void RefreshHomeGroups()
        {
            var current = ViewModel.CustomDatas.Where(item => item.GroupKey == "Current" && item.Presentation.IsRange).ToList();
            var currentPoints = ViewModel.CustomDatas.Where(item => item.GroupKey == "Current" && !item.Presentation.IsRange).ToList();
            var upcoming = ViewModel.CustomDatas.Where(item => item.GroupKey == "Upcoming" && item.Presentation.IsRange).ToList();
            var upcomingPoints = ViewModel.CustomDatas.Where(item => item.GroupKey == "Upcoming" && !item.Presentation.IsRange).ToList();
            var past = ViewModel.CustomDatas.Where(item => item.GroupKey == "Past" && item.Presentation.IsRange).ToList();
            var pastPoints = ViewModel.CustomDatas.Where(item => item.GroupKey == "Past" && !item.Presentation.IsRange).ToList();
            CurrentSchedules.ItemsSource = current;
            CurrentPointSchedules.ItemsSource = currentPoints;
            UpcomingSchedules.ItemsSource = upcoming;
            UpcomingPointSchedules.ItemsSource = upcomingPoints;
            PastSchedules.ItemsSource = past;
            PastPointSchedules.ItemsSource = pastPoints;
            CurrentGroupTitle.Text = "进行中 / 今天 · " + (current.Count + currentPoints.Count);
            UpcomingGroupTitle.Text = "未过的日程 · " + (upcoming.Count + upcomingPoints.Count);
            PastGroupTitle.Text = "已过的日程 · " + (past.Count + pastPoints.Count);
            currentRangeCount = current.Count;
            currentPointCount = currentPoints.Count;
            upcomingRangeCount = upcoming.Count;
            upcomingPointCount = upcomingPoints.Count;
            pastRangeCount = past.Count;
            pastPointCount = pastPoints.Count;
            UpdateGroupVisibility();
            ScheduleCount.Text = ViewModel.CustomDatas.Count + " 个日程";
        }

        private void UpdateGroupVisibility(bool animateGroupChanges = false, FrameworkElement animatedGroup = null)
        {
            bool hasCurrent = currentRangeCount + currentPointCount > 0;
            bool hasUpcoming = upcomingRangeCount + upcomingPointCount > 0;
            bool hasPast = pastRangeCount + pastPointCount > 0;

            CurrentGroupHeader.Visibility = hasCurrent ? Visibility.Visible : Visibility.Collapsed;
            UpcomingGroupHeader.Visibility = hasUpcoming ? Visibility.Visible : Visibility.Collapsed;
            PastGroupHeader.Visibility = hasPast ? Visibility.Visible : Visibility.Collapsed;

            CurrentSchedules.Visibility = hasCurrent && currentRangeCount > 0
                ? Visibility.Visible : Visibility.Collapsed;
            CurrentPointSchedules.Visibility = hasCurrent && currentPointCount > 0
                ? Visibility.Visible : Visibility.Collapsed;
            UpcomingSchedules.Visibility = hasUpcoming && upcomingRangeCount > 0
                ? Visibility.Visible : Visibility.Collapsed;
            UpcomingPointSchedules.Visibility = hasUpcoming && upcomingPointCount > 0
                ? Visibility.Visible : Visibility.Collapsed;
            PastSchedules.Visibility = hasPast && pastRangeCount > 0
                ? Visibility.Visible : Visibility.Collapsed;
            PastPointSchedules.Visibility = hasPast && pastPointCount > 0
                ? Visibility.Visible : Visibility.Collapsed;

            if (!animateGroupChanges)
            {
                SetGroupContentVisibility(CurrentGroupContent, hasCurrent && currentGroupExpanded,
                    false, ref currentGroupAnimation);
                SetGroupContentVisibility(UpcomingGroupContent, hasUpcoming && upcomingGroupExpanded,
                    false, ref upcomingGroupAnimation);
                SetGroupContentVisibility(PastGroupContent, hasPast && pastGroupExpanded,
                    false, ref pastGroupAnimation);
            }
            else if (ReferenceEquals(animatedGroup, CurrentGroupContent))
            {
                SetGroupContentVisibility(CurrentGroupContent, hasCurrent && currentGroupExpanded,
                    true, ref currentGroupAnimation);
            }
            else if (ReferenceEquals(animatedGroup, UpcomingGroupContent))
            {
                SetGroupContentVisibility(UpcomingGroupContent, hasUpcoming && upcomingGroupExpanded,
                    true, ref upcomingGroupAnimation);
            }
            else if (ReferenceEquals(animatedGroup, PastGroupContent))
            {
                SetGroupContentVisibility(PastGroupContent, hasPast && pastGroupExpanded,
                    true, ref pastGroupAnimation);
            }

            CurrentGroupChevron.Glyph = currentGroupExpanded ? "\uE70D" : "\uE70E";
            UpcomingGroupChevron.Glyph = upcomingGroupExpanded ? "\uE70D" : "\uE70E";
            PastGroupChevron.Glyph = pastGroupExpanded ? "\uE70D" : "\uE70E";
        }

        private void CurrentGroupHeader_Click(object sender, RoutedEventArgs e)
        {
            currentGroupExpanded = !currentGroupExpanded;
            UpdateGroupVisibility(true, CurrentGroupContent);
        }

        private void UpcomingGroupHeader_Click(object sender, RoutedEventArgs e)
        {
            upcomingGroupExpanded = !upcomingGroupExpanded;
            UpdateGroupVisibility(true, UpcomingGroupContent);
        }

        private void PastGroupHeader_Click(object sender, RoutedEventArgs e)
        {
            pastGroupExpanded = !pastGroupExpanded;
            UpdateGroupVisibility(true, PastGroupContent);
        }

        private void SetGroupContentVisibility(FrameworkElement content, bool isVisible, bool animate, ref Storyboard animation)
        {
            if (content == null) return;
            animation?.Stop();

            var transform = content.RenderTransform as CompositeTransform;
            if (transform == null)
            {
                transform = new CompositeTransform();
                content.RenderTransform = transform;
                content.RenderTransformOrigin = new Point(0.5, 0);
            }

            bool animationsEnabled = animate && new UISettings().AnimationsEnabled;
            if (!animationsEnabled)
            {
                content.Opacity = 1;
                transform.TranslateY = 0;
                content.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                return;
            }

            content.Visibility = Visibility.Visible;
            content.Opacity = isVisible ? 0 : 1;
            transform.TranslateY = isVisible ? -12 : 0;
            var storyboard = new Storyboard();
            var fade = new DoubleAnimation
            {
                From = isVisible ? 0 : 1,
                To = isVisible ? 1 : 0,
                Duration = TimeSpan.FromMilliseconds(180),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(fade, content);
            Storyboard.SetTargetProperty(fade, "Opacity");
            storyboard.Children.Add(fade);
            var slide = new DoubleAnimation
            {
                From = isVisible ? -12 : 0,
                To = isVisible ? 0 : -12,
                Duration = TimeSpan.FromMilliseconds(220),
                EnableDependentAnimation = true,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(slide, transform);
            Storyboard.SetTargetProperty(slide, "TranslateY");
            storyboard.Children.Add(slide);
            if (!isVisible)
            {
                storyboard.Completed += (sender, args) =>
                {
                    content.Visibility = Visibility.Collapsed;
                    content.Opacity = 1;
                    transform.TranslateY = 0;
                };
            }
            else
            {
                storyboard.Completed += (sender, args) => transform.TranslateY = 0;
            }
            animation = storyboard;
            storyboard.Begin();
        }

        private void GroupedPoints_Loaded(object sender, RoutedEventArgs e) => SizeGroupedPoints(sender);
        private void GroupedPoints_SizeChanged(object sender, SizeChangedEventArgs e) => SizeGroupedPoints(sender);

        private static void SizeGroupedPoints(object sender)
        {
            if (sender is Microsoft.Toolkit.Uwp.UI.Controls.AdaptiveGridView grid && grid.ActualWidth > 0)
                grid.DesiredWidth = grid.ActualWidth / (grid.ActualWidth >= 620 ? 2 : 1);
        }

        private void ScheduleList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            MyGirdView_RightTapped(sender, e);
        }

        private void DisplatPast_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["DisplayMode"] = "Past";
            LoadDateData();
        }

        private void DisplayFuture_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["DisplayMode"] = "Future";
            LoadDateData();
        }

        private void RootThumb_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AllScrollViewer.ChangeView(null, 0, null);
        }

        private void AllScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            //var y = AllScrollViewer.VerticalOffset;
            //if (y == 0)
            //    RootThumb.Visibility = Visibility.Collapsed;
            //else
            //    RootThumb.Visibility = Visibility.Visible;
            //RootThumb.Margin = new Thickness(0, y, 20, 20);
        }

        private void AllScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var y = AllScrollViewer.VerticalOffset;
            if (y == 0)
            {
                //AllThumbShadow.Visibility = Visibility.Collapsed;
                RootThumb.Visibility = Visibility.Collapsed;
            }
            else
            {
                //AllThumbShadow.Visibility = Visibility.Visible;
                RootThumb.Visibility = Visibility.Visible;
            }
        }

        private void DayMode_Click(object sender, RoutedEventArgs e)
        {
            ChangeABB.Label = "天数模式";
            localSettings.Values["DateMode"] = "Day";
            LoadDateData();
        }

        private void WeekMode_Click(object sender, RoutedEventArgs e)
        {
            ChangeABB.Label = "周数模式";
            localSettings.Values["DateMode"] = "Week";
            LoadDateData();
        }

        private void YearMode_Click(object sender, RoutedEventArgs e)
        {
            ChangeABB.Label = "年数模式";
            localSettings.Values["DateMode"] = "Year";
            LoadDateData();
        }

        private async void DeleteDialogPrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            if (TileService.IsPinned(AllItem.RecordId) && !await TileService.UnpinAsync(AllItem.RecordId))
                return;
            int _start = ViewModel.CustomDatas.Count();
            ViewModel.CustomDatas.Remove(AllItem);
            ScheduleService.DeleteWithSettings(AllItem.RecordId);
            int _end = ViewModel.CustomDatas.Count();
            if (_start != _end)
            {
                PopupNotice popupNotice = new PopupNotice("删除成功");
                popupNotice.ShowAPopup();
            }
            var dm = localSettings.Values["DisplayMode"].ToString();
            switch (dm)
            {
                case "All":
                    {
                        List<CountdownRecord> datalist = ScheduleService.GetAll();
                        if (datalist.Count() == 0)
                        {
                            NewTB.Text = "创建你的第一个日程吧！";
                            NewTB2.Text = "（创建后可右键删除）";
                            NewTB.Visibility = Visibility.Visible;
                            NewTB2.Visibility = Visibility.Visible;
                        }
                    }
                    break;
                case "Past":
                    {
                        List<CountdownRecord> datalist = ScheduleService.GetUnorderedPast(DateTime.Now.ToString("yyyy-MM-dd"));
                        if (datalist.Count == 0)
                        {
                            NewTB.Text = "这里空空如也~";
                            NewTB2.Text = "（暂无已过日程）";
                            NewTB.Visibility = Visibility.Visible;
                            NewTB2.Visibility = Visibility.Visible;
                        }
                    }
                    break;
                case "Future":
                    {
                        List<CountdownRecord> datalist = ScheduleService.GetUnorderedFuture(DateTime.Now.ToString("yyyy-MM-dd"));
                        if (datalist.Count == 0)
                        {
                            NewTB.Text = "这里空空如也~";
                            NewTB2.Text = "（暂无未过日程）";
                            NewTB.Visibility = Visibility.Visible;
                            NewTB2.Visibility = Visibility.Visible;
                        }
                    }
                    break;
            }
            DeleteDialog.Hide();
            LoadDateData();
            LoadTile();
        }

        private void DeleteDialogCloseButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteDialog.Hide();
        }

        public Color GetColor(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            return Color.FromArgb(a, r, g, b);
        }

    }
}
