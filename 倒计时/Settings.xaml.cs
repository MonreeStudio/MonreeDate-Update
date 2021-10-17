using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Timers;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Services.Store;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Shell;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Connectivity;
using 倒计时.Models;
using 夏日.Models;
using Microsoft.Toolkit.Uwp.UI.Controls;
using HHChaosToolkit.UWP.Picker;
using 倒计时.ViewModels;
//using Microsoft.Toolkit.Services.Services.MicrosoftGraph;
//using Microsoft.Toolkit.Services.OneDrive;
//using Microsoft.Identity.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Threading;
using 倒计时.Manager;
using Newtonsoft.Json;
using System.Text;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 倒计时
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Settings : Page
    {
        public static Settings Current;
        public double MinMyNav = MainPage.Current.MyNav.CompactModeThresholdWidth;
        public Compositor _compositor = Window.Current.Compositor;
        public SpringVector3NaturalMotionAnimation _springAnimation;
        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        public ThemeColorDataViewModel ViewModel = new ThemeColorDataViewModel();
        public Color color;

        StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

        string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "mydb.sqlite");    //建立数据库  
        public SQLite.Net.SQLiteConnection conn;
        private StoreContext context = null;
        private int current = 0;
        private string logoutJson;

        public ToDoTasksViewModel TaskViewModel = new ToDoTasksViewModel();
        private JArray listJson;


        public Settings()
        {
            //建立数据库连接   
            conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), path);
            //建表              
            conn.CreateTable<PersonPictures>(); //默认表名同范型参数  
            color = Colors.SkyBlue;
            this.InitializeComponent();
            Current = this;
            //this.NavigationCacheMode = NavigationCacheMode.Enabled;
            Loaded += OnSettingsPageLoaded;
            ReadSettings();
            GetAppVersion();
            GetSystemVersion();
            GetPlatform();
            SetThemeColor();
            SetPersonPicture();
            MainPage.Current.MyNav.IsBackEnabled = true;
            MainPage.Current.SelectedPageItem = "Settings";
        }

        private async void SetPersonPicture()
        {
            List<PersonPictures> datalist = conn.Query<PersonPictures>("select * from PersonPictures where pictureName = ?", "picture");
            foreach (var item in datalist)
            {
                if (item != null)
                {
                    try
                    {
                        MemoryStream stream = new MemoryStream(item.picture);
                        BitmapImage bitmap = new BitmapImage();
                        await bitmap.SetSourceAsync(stream.AsRandomAccessStream());
                        MyPersonPicture.ProfilePicture = bitmap;
                        All.Current.AllPicture.ProfilePicture = bitmap;

                    }
                    catch
                    {
                        //throw ex;
                    }
                }
            }
        }
        private void SetThemeColor()
        {
            if (localSettings.Values["ThemeColor"] == null)
                localSettings.Values["ThemeColor"] = "CornflowerBlue";
            switch (localSettings.Values["ThemeColor"].ToString())
            {
                case "CornflowerBlue":
                    TC.Color = Colors.CornflowerBlue;
                    MainPage.Current.SetThemeColor();
                    ThemeColorSelected.SelectedIndex = 0;
                    try
                    {
                        All.Current.SetThemeColor();
                        Festival.Current.SetThemeColor();
                    }
                    catch { }
                    break;
                case "DeepSkyBlue":
                    TC.Color = Color.FromArgb(255,2,136,235);
                    MainPage.Current.SetThemeColor();
                    ThemeColorSelected.SelectedIndex = 1;
                    try
                    {
                        All.Current.SetThemeColor();
                        Festival.Current.SetThemeColor();
                    }
                    catch { }
                    break;
                case "Orange":
                    TC.Color = Color.FromArgb(255, 229, 103, 44);
                    MainPage.Current.SetThemeColor();
                    ThemeColorSelected.SelectedIndex = 2;
                    try
                    {
                        All.Current.SetThemeColor();
                        Festival.Current.SetThemeColor();
                    }
                    catch { }
                    break;
                case "Crimson":
                    TC.Color = Colors.Crimson;
                    MainPage.Current.SetThemeColor();
                    ThemeColorSelected.SelectedIndex = 3;
                    try
                    {
                        All.Current.SetThemeColor();
                        Festival.Current.SetThemeColor();
                    }
                    catch { }
                    break;
                case "Gray":
                    TC.Color = Color.FromArgb(255, 73, 92, 105);
                    MainPage.Current.SetThemeColor();
                    ThemeColorSelected.SelectedIndex = 4;
                    try
                    {
                        All.Current.SetThemeColor();
                        Festival.Current.SetThemeColor();
                    }
                    catch { }
                    break;
                case "Purple":
                    TC.Color = Color.FromArgb(255, 119, 25, 171);
                    MainPage.Current.SetThemeColor();
                    ThemeColorSelected.SelectedIndex = 5;
                    try
                    {
                        All.Current.SetThemeColor();
                        Festival.Current.SetThemeColor();
                    }
                    catch { }
                    break;
                case "Pink":
                    TC.Color = Color.FromArgb(255, 239, 130, 160);
                    MainPage.Current.SetThemeColor();
                    ThemeColorSelected.SelectedIndex = 6;
                    try
                    {
                        All.Current.SetThemeColor();
                        Festival.Current.SetThemeColor();
                    }
                    catch { }
                    break;
                case "Green":
                    TC.Color = Color.FromArgb(255, 124, 178, 56);
                    MainPage.Current.SetThemeColor();
                    ThemeColorSelected.SelectedIndex = 7;
                    try
                    {
                        All.Current.SetThemeColor();
                        Festival.Current.SetThemeColor();
                    }
                    catch { }
                    break;
                case "DeepGreen":
                    TC.Color = Color.FromArgb(255, 8, 128, 126);
                    MainPage.Current.SetThemeColor();
                    ThemeColorSelected.SelectedIndex = 8;
                    try
                    {
                        All.Current.SetThemeColor();
                        Festival.Current.SetThemeColor();
                    }
                    catch { }
                    break;
                case "Coffee":
                    TC.Color = Color.FromArgb(255, 183, 133, 108);
                    MainPage.Current.SetThemeColor();
                    ThemeColorSelected.SelectedIndex = 9;
                    try
                    {
                        All.Current.SetThemeColor();
                        Festival.Current.SetThemeColor();
                    }
                    catch { }
                    break;
                default:
                    break;
            }
        }

        public async void ReadSettings()
        {
            if (localSettings.Values["SetAllPageAcrylic"] != null)
            {
                if (localSettings.Values["SetAllPageAcrylic"].Equals(true))
                {
                    AllPageAcylic.IsOn = true;
                }
                else
                {
                    AllPageAcylic.IsOn = false;
                }
            }
            else
                AllPageAcylic.IsOn = true;

            if (localSettings.Values["SetAllPersonPicture"] != null)
            {
                if (localSettings.Values["SetAllPersonPicture"].Equals(true))
                    AllPersonPicture.IsOn = true;
                else
                    AllPersonPicture.IsOn = false;
            }
            else
                AllPersonPicture.IsOn = true;
            if (localSettings.Values["ToolAutoStart"] != null)
            {
                if (localSettings.Values["ToolAutoStart"].ToString() == "1")
                    ToolAutoStartSwitch.IsOn = true;
                else
                    ToolAutoStartSwitch.IsOn = false;
            }
            else
                ToolAutoStartSwitch.IsOn = false;
            if (localSettings.Values["TileTip"] != null)
            {
                if (localSettings.Values["TileTip"].ToString() == "1")
                    TileTip.IsOn = true;
                else
                    TileTip.IsOn = false;
            }
            else
                TileTip.IsOn = false;
            if (localSettings.Values["Lunar"] != null)
            {
                if (localSettings.Values["Lunar"].ToString() == "1")
                    LunarSwitch.IsOn = true;
                else
                    LunarSwitch.IsOn = false;
            }
            else
                LunarSwitch.IsOn = true;
            //if(localSettings.Values["AutoStart"] != null)
            //{
            //    if (localSettings.Values["AutoStart"].ToString() == "1")
            //        AutoStartSwitch.IsOn = true;
            //    else
            //        AutoStartSwitch.IsOn = false;
            //}
            var _NickName = localSettings.Values["NickName"];
            var _Sex = localSettings.Values["PersonalSex"];
            var _sign = localSettings.Values["Sign"];
            var _Birthday_date = localSettings.Values["BirthDay_Date"];

            //var _PersonPicture = localSettings.Values["PersonPicture"];
            if (_NickName != null && _Sex != null && _sign != null && _Birthday_date != null)
            {
                PersonalSex.Text = _Sex.ToString();
                PersonalNickName.Text = _NickName.ToString();
                PersonalBirthday.Text = _Birthday_date.ToString();
                PersonalSign.Text = _sign.ToString();

                //MyPersonPicture.ProfilePicture = new BitmapImage(new Uri(localFolder.Path + "/" + desiredName));
                //MyPersonPicture.ProfilePicture = (BitmapImage)_PersonPicture;
            }
            await LoadState();
        }

        public async Task LoadState()
        {
            //if (localSettings.Values["UserName"] != null && !localSettings.Values["UserName"].Equals(""))
            //{
            //    LoginButton.Visibility = Visibility.Collapsed;
            //    SignOutButton.Visibility = Visibility.Visible;
            //    SyncButton.Visibility = Visibility.Visible;
            //    EmailStackPanel.Visibility = Visibility.Visible;
            //    PersonalEmail.Text = localSettings.Values["UserName"].ToString();
            //}
            //else
            //{
            //    LoginButton.Visibility = Visibility.Visible;
            //    SignOutButton.Visibility = Visibility.Collapsed;
            //    SyncButton.Visibility = Visibility.Collapsed;
            //    EmailStackPanel.Visibility = Visibility.Collapsed;
            //}

            var task = await StartupTask.GetAsync("AppAutoRun");
            AutoStartTipButton.Visibility = Visibility.Collapsed;
            switch (task.State)
            {
                case StartupTaskState.Disabled:
                    // 禁用状态
                    AutoStartSwitch.OnContent = "关";
                    AutoStartSwitch.IsOn = false;
                    break;
                case StartupTaskState.DisabledByPolicy:
                    // 由管理员或组策略禁用
                    AutoStartSwitch.OffContent = "被系统策略禁用";
                    AutoStartSwitch.IsOn = false;
                    break;
                case StartupTaskState.DisabledByUser:
                    // 由用户手工禁用
                    AutoStartSwitch.OffContent = "被用户禁用";
                    AutoStartSwitch.IsOn = false;
                    AutoStartTipButton.Visibility = Visibility.Visible;
                    break;
                case StartupTaskState.Enabled:
                    // 当前状态为已启用
                    AutoStartSwitch.OnContent = "开";
                    AutoStartSwitch.IsOn = true;
                    break;
            }
        }

        private void GetAppVersion()
        {
            string appVersion = string.Format("版本： {0}.{1}.{2}.{3}",
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Revision);
            Version.Text = appVersion;
        }

        private void GetSystemVersion()
        {
            Windows.System.Profile.AnalyticsVersionInfo analyticsVersion = Windows.System.Profile.AnalyticsInfo.VersionInfo;

            //var reminder = analyticsVersion.DeviceFamily;

            ulong v = ulong.Parse(Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamilyVersion);
            ulong v1 = (v & 0xFFFF000000000000L) >> 48;
            ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            ulong v4 = (v & 0x000000000000FFFFL);
            var reminder = $"{v1}.{v2}.{v3}.{v4}";
            SystemVersion.Text = "系统版本：Windows "+ reminder;
        }

        private void GetPlatform()
        {
            Package package = Package.Current;
            Platform.Text = "平台架构：" + package.Id.Architecture.ToString();
        }

        private void OnSettingsPageLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private async void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            await AboutContent.ShowAsync();
        }

        private void AllPageAcylic_Toggled(object sender, RoutedEventArgs e)
        {         
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            
            if (toggleSwitch != null)
            {
                //if (AllPageAcylic.IsOn == true)
                //{
                //    AcrylicBrush myBrush = new AcrylicBrush();
                //    myBrush.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
                //    myBrush.TintColor = Color.FromArgb(255, 255, 255, 255);
                //    myBrush.FallbackColor = Color.FromArgb(255, 255, 255, 255);
                //    myBrush.TintOpacity = 0.8;
                //    All.Current.AllPageStackPanel.Background = myBrush;
                //    localSettings.Values["SetAllPageAcrylic"] = true;
                //    //Add.Current.AddPageGird.Background = myBrush;
                //}
                //else
                //{
                //    All.Current.AllPageStackPanel.Background = new SolidColorBrush(Colors.White);
                //    localSettings.Values["SetAllPageAcrylic"] = false;
                //    //Add.Current.AddPageGird.Background = new SolidColorBrush(Colors.White);
                //}
            }
            toggleSwitch.Toggled += AllPageAcylic_Toggled;
        }

        

        private void CreateOrUpdateSpringAnimation(float finalValue)
        {
            if (_springAnimation == null)
            {
                _springAnimation = _compositor.CreateSpringVector3Animation();
                _springAnimation.Target = "Scale";
            }

            _springAnimation.FinalValue = new Vector3(finalValue);
        }

        private void element_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            // Scale up to 1.5
            CreateOrUpdateSpringAnimation(1.5f);

            (sender as UIElement).StartAnimation(_springAnimation);
        }

        private async void MyPersonPicture_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TempPicture.ProfilePicture = MyPersonPicture.ProfilePicture;
            await CropperDialog.ShowAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Edit),null,new DrillInNavigationTransitionInfo());
        }

        private async void AboutContent_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var Uri = new Uri("ms-windows-store://review/?productid=9PKBWKPCCFJ8");
            await Launcher.LaunchUriAsync(Uri);
        }

        private async void PinButton_Click(object sender, RoutedEventArgs e)
        {
            // Get your own app list entry
            AppListEntry entry = (await Package.Current.GetAppListEntriesAsync())[0];
            bool isPinned1 = await StartScreenManager.GetDefault().ContainsAppListEntryAsync(entry);
            // And pin it to Start
            bool isPinned = await StartScreenManager.GetDefault().RequestAddAppListEntryAsync(entry);
            if (isPinned1 == true)
            {
                PopupNotice popupNotice = new PopupNotice("应用已固定在开始菜单");
                popupNotice.ShowAPopup();
            }
            else
            {
                if (isPinned == true)
                {
                    PopupNotice popupNotice = new PopupNotice("固定成功");
                    popupNotice.ShowAPopup();
                }
            }
        }

        private async void PinTaskbarButton_Click(object sender, RoutedEventArgs e)
        {
            bool isPinned1 = await TaskbarManager.GetDefault().IsCurrentAppPinnedAsync();
            if (isPinned1)
            {
                PopupNotice popupNotice = new PopupNotice("应用已固定在任务栏");
                popupNotice.ShowAPopup();
            }
            else
            {
                bool isPinned = await TaskbarManager.GetDefault().RequestPinCurrentAppAsync();
                if (isPinned)
                {
                    PopupNotice popupNotice = new PopupNotice("固定成功");
                    popupNotice.ShowAPopup();
                }  
            }
        }

        private void SupportButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ThemeColorSelected.SelectedIndex)
            {
                case 0:
                    localSettings.Values["ThemeColor"]="CornflowerBlue";
                    SetThemeColor();
                    break;
                case 1:
                    localSettings.Values["ThemeColor"] = "DeepSkyBlue";
                    SetThemeColor();
                    break;
                case 2:
                    localSettings.Values["ThemeColor"] = "Orange";
                    SetThemeColor();
                    break;
                case 3:
                    localSettings.Values["ThemeColor"] = "Crimson";
                    SetThemeColor();
                    break;
                case 4:
                    localSettings.Values["ThemeColor"] = "Gray";
                    SetThemeColor();
                    break;
                case 5:
                    localSettings.Values["ThemeColor"] = "Purple";
                    SetThemeColor();
                    break;
                case 6:
                    localSettings.Values["ThemeColor"] = "Pink";
                    SetThemeColor();
                    break;
                case 7:
                    localSettings.Values["ThemeColor"] = "Green";
                    SetThemeColor();
                    break;
                case 8:
                    localSettings.Values["ThemeColor"] = "DeepGreen";
                    SetThemeColor();
                    break;
                case 9:
                    localSettings.Values["ThemeColor"] = "Coffee";
                    SetThemeColor();
                    break;
                default:
                    break;
            }
        }

        private void MyPersonPicture_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 0);
        }

        private void MyPersonPicture_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
        }

        private void AllPersonPicture_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            if (toggleSwitch != null)
            {
                if (AllPersonPicture.IsOn == true)
                {
                    All.Current.AllPicture.Visibility = Visibility.Visible;
                    All.Current.AllCommandBar.Margin = new Thickness(0, 50, 10, 2);
                    All.Current.MarginText.Height = 30;
                    localSettings.Values["SetAllPersonPicture"] = true;
                }
                else
                {
                    All.Current.AllPicture.Visibility = Visibility.Collapsed;
                    All.Current.AllCommandBar.Margin = new Thickness(0, 50, 10, 25);
                    All.Current.MarginText.Height = 60;
                    localSettings.Values["SetAllPersonPicture"] = false;
                }
            }
            //toggleSwitch.Toggled += AllPageAcylic_Toggled;
        }

        private async void BirthCreate_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await BirthDialog.ShowAsync();
        }

        private void BirthCreate_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            BirthdayTB.FontSize = 23;
            PersonalBirthday.FontSize = 23;
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 0);
        }

        private void BirthCreate_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            BirthdayTB.FontSize = 13;
            PersonalBirthday.FontSize = 13;
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
        }

        private async void BirthDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (PersonalBirthday.Text == "未设置")
            {
                MessageDialog AboutDialog = new MessageDialog("您还没有设置生日哦，赶紧去设置吧。", "提示");
                await AboutDialog.ShowAsync();
                BirthDialog.Hide();
            }
            else
            {
                try
                {
                    DateTime birthday = Convert.ToDateTime(PersonalBirthday.Text);
                    string Tip = "";
                    string _birthday = birthday.ToString("yyyy-MM-dd");
                    All.Current.conn.Insert(new DataTemple() { Schedule_name = "出生日", CalculatedDate = CustomData.Calculator(_birthday), Date = _birthday, BgColor = "#fffbb612", TintOpacity = 0.7, IsTop = "0", AddTime = "" });
                    localSettings.Values["出生日" + _birthday] = Tip;
                    MainPage.Current.MyNav.SelectedItem = MainPage.Current.MyNav.MenuItems[0];
                    Frame.Navigate(typeof(All));
                    PopupNotice popupNotice = new PopupNotice("添加成功");
                    popupNotice.ShowAPopup();
                }
                catch
                {
                    MessageDialog AboutDialog = new MessageDialog("您已经添加过了哦。", "提示");
                    await AboutDialog.ShowAsync();
                }
            }
        }

        private void timer_Tick(object sender,EventArgs e)
        {
            current++;
        }

        private async void CheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
            {
                try
                {
                    await DownloadAndInstallAllUpdatesAsync();
                }
                catch
                {
                    MessageDialog dialog = new MessageDialog(
                        "网络请求发生超时，请确保网络畅通。", "网络请求超时！");
                    await dialog.ShowAsync();
                    UpdateRing.IsActive = false;
                }
            }
            else
            {
                MessageDialog dialog = new MessageDialog(
                        "无网络连接，请联网后再试。", "无网络连接！");
                await dialog.ShowAsync();
                UpdateRing.IsActive = false;
            }
            
        }
        public async Task DownloadAndInstallAllUpdatesAsync()
        {
            UpdateRing.IsActive = true;
            CheckUpdate.IsEnabled = false;
            if (context == null)
            {
                context = StoreContext.GetDefault();
            }

            // Get the updates that are available.
            IReadOnlyList<StorePackageUpdate> updates =
                await context.GetAppAndOptionalStorePackageUpdatesAsync();
            
            if (updates.Count > 0)
            {
                // Alert the user that updates are available and ask for their consent
                // to start the updates.
                MessageDialog dialog = new MessageDialog(
                    "立即下载并安装更新吗? 此过程应用可能会关闭。", "发现新版本的夏日！");
                dialog.Commands.Add(new UICommand("更新"));
                dialog.Commands.Add(new UICommand("取消"));
                IUICommand command = await dialog.ShowAsync();
                if (command.Label.Equals("更新", StringComparison.CurrentCultureIgnoreCase))
                {
                    //downloadProgressBar.Visibility = Visibility.Visible;
                    // Download and install the updates.
                    IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> downloadOperation =
                        context.RequestDownloadAndInstallStorePackageUpdatesAsync(updates);

                    // The Progress async method is called one time for each step in the download
                    // and installation process for each package in this request.

                    downloadOperation.Progress = async (asyncInfo, progress) =>
                    {
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                        () =>
                        { });
                    };
                    //downloadProgressBar.Visibility = Visibility.Collapsed;
                    StorePackageUpdateResult result = await downloadOperation.AsTask();
                    UpdateRing.IsActive = false;
                    CheckUpdate.IsEnabled = true;
                }
                else
                {
                    UpdateRing.IsActive = false;
                    CheckUpdate.IsEnabled = true;
                }
            }
            else
            {
                UpdateRing.IsActive = false;
                CheckUpdate.IsEnabled = true;
                PopupNotice popupNotice = new PopupNotice("已是最新版本！");
                popupNotice.ShowAPopup();
            }
        }

        private void ToolAutoStartSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (ToolAutoStartSwitch != null)
            {
                if (ToolAutoStartSwitch.IsOn == true)
                {
                    localSettings.Values["ToolAutoStart"] = "1";

                }
                else
                {
                    localSettings.Values["ToolAutoStart"] = "0";
                }
            }
        }

        private async void AppAutoStartTip_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await AutoStartTipDialog.ShowAsync();
        }      

        private async Task<ImageSource> CropImage(ImageCropperConfig config)
        {
            var startOption = new PickerOpenOption
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            var ret = await ViewModelLocator.Current.ObjectPickerService.PickSingleObjectAsync<WriteableBitmap>(
                typeof(ImageCropperPickerViewModel).FullName, config, startOption);
            if (!ret.Canceled)
            {
                return ret.Result;
            }
            return null;
        }

        private void PickImgButton_Click(object sender, RoutedEventArgs e)
        {
            TempPicture.Visibility = Visibility.Collapsed;
            EditPicture.Visibility = Visibility.Visible;
        }

        private void TileTip_Toggled(object sender, RoutedEventArgs e)
        {
            if (TileTip != null)
            {
                if (TileTip.IsOn == true)
                {
                    localSettings.Values["TileTip"] = "1";

                }
                else
                {
                    localSettings.Values["TileTip"] = "0";
                }
                All.Current.LoadSettings();
                All.Current.LoadTile();
            }
        }

        private async void AutoStartSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (AutoStartSwitch != null)
            {
                if (AutoStartSwitch.IsOn == true)
                {
                    localSettings.Values["AutoStart"] = "1";
                    var task = await StartupTask.GetAsync("AppAutoRun");
                    if (task.State == StartupTaskState.Disabled)
                    {
                        await task.RequestEnableAsync();
                    }

                    // 重新加载状态
                    await LoadState();
                }
                else
                {
                    localSettings.Values["AutoStart"] = "0";
                    var task = await StartupTask.GetAsync("AppAutoRun");
                    task.Disable();
                } 
            }
            await LoadState();
        }

        private void LunarSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (LunarSwitch != null)
            {
                if (LunarSwitch.IsOn == true)
                {
                    localSettings.Values["Lunar"] = "1";

                }
                else
                {
                    localSettings.Values["Lunar"] = "0";
                }
                All.Current.LoadSettings();
                All.Current.LoadTile();
            }
        }

        private async void SyncButton_Click(object sender, RoutedEventArgs e)
        {
            //await InitSync();
            await SyncDataDialog.ShowAsync();
        }

        //private static Task InitSync()
        //{
        //    //string[] scopes = new string[] { MicrosoftGraphScope.FilesReadWriteAppFolder };
        //    //OneDriveService.Instance.Initialize("0000000048273E9C", scopes, null, null);
        //    //if (await OneDriveService.Instance.LoginAsync())
        //    //{
        //    //    var folder = await OneDriveService.Instance.RootFolderForMeAsync();
        //    //    var OneDriveItems = await folder.NextItemsAsync();
        //    //    do
        //    //    {
        //    //        // Get the next page of items
        //    //        OneDriveItems = await folder.NextItemsAsync();
        //    //    }
        //    //    while (OneDriveItems != null);
        //    //    string newFolderName = "MonreeDate Data";
        //    //    if (!string.IsNullOrEmpty(newFolderName))
        //    //    {
        //    //        await folder.StorageFolderPlatformService.CreateFolderAsync(newFolderName, CreationCollisionOption.GenerateUniqueName);
        //    //    }
        //    //    //var currentFolder = await _graphCurrentFolder.GetFolderAsync(item.Name);
        //    //    //OneDriveItemsList.ItemsSource = await currentFolder.GetItemsAsync(20);
        //    //    //_graphCurrentFolder = currentFolder;
        //    //}
        //    //else
        //    //{
        //    //    //TipServices.TipAuthenticateFail();
        //    //    throw new Exception("Unable to sign in");
        //    //}
        //}

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }

        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = localSettings.Values["UserName"].ToString();
            logoutJson = "{\"UserName\":\"" + userName + "\"}";
            ThreadStart threadStart = new ThreadStart(DoLogout);
            Thread thread = new Thread(threadStart);
            thread.Start();
        }
        public async void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
        }

        private void DoLogout()
        {
            Invoke(() =>
            {
                SignOutButton.Content = "正在注销";
                SignOutButton.IsEnabled = false;
            });
            HttpClient client = new HttpClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://msdate.monreeing.com:3000/user/logout/");
            requestMessage.Content = new StringContent(logoutJson);
            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.SendAsync(requestMessage).GetAwaiter().GetResult();

            if (response.StatusCode.ToString() == "OK")
            {
                string result = response.Content.ReadAsStringAsync().Result.ToString();
                JObject jo = JObject.Parse(result);
                if (jo["Code"].ToString().Equals("0"))
                {
                    localSettings.Values["Token"] = "";
                    localSettings.Values["TimeStamp"] = "";
                    localSettings.Values["UserName"] = "";
                    Invoke(() =>
                    {
                        PopupNotice popupNotice = new PopupNotice("注销成功");
                        popupNotice.ShowAPopup();
                        SignOutButton.Content = "退出登录";
                        SignOutButton.IsEnabled = true;
                        SignOutButton.Visibility = Visibility.Collapsed;
                        SyncButton.Visibility = Visibility.Collapsed;
                        LoginButton.Visibility = Visibility.Visible;
                        EmailStackPanel.Visibility = Visibility.Collapsed;
                        PersonalEmail.Text = "";
                    });
                }
                else
                {
                    Invoke(() =>
                    {
                        string errorMsg = jo["Message"].ToString();
                        PopupNotice popupNotice = new PopupNotice("注销失败" + errorMsg);
                        popupNotice.ShowAPopup();
                    });
                }
            }
            else
            {
                Invoke(() =>
                {
                    PopupNotice popupNotice = new PopupNotice("服务器错误：未知原因");
                    popupNotice.ShowAPopup();
                });
            }
            Invoke(() =>
            {
                SignOutButton.Content = "退出登录";
                SignOutButton.IsEnabled = true;
            });
        }

        private void SyncDataDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private void ExitSyncButton_Click(object sender, RoutedEventArgs e)
        {
            SyncDataDialog.Hide();
        }

        private void GetLocalDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (localSettings.Values["UserName"] != null && localSettings.Values["Token"] != null)
                {
                    LoadProgressBar.IsActive = true;
                    SyncManager syncManager = new SyncManager(localSettings.Values["UserName"].ToString(), localSettings.Values["Token"].ToString());
                    string data = syncManager.GetLocalTaskData().Trim();
                    JArray taskListJson = (JArray)JsonConvert.DeserializeObject(data);
                    listJson = taskListJson;
                    TaskViewModel.ToDoDatas.Clear();
                    foreach (var item in taskListJson)
                    {
                        if (item["IsDelete"].ToString().Equals("0"))
                            TaskViewModel.ToDoDatas.Add(new ToDoTasks() { TaskId = item["TaskId"].ToString(), Name = item["TaskName"].ToString() });
                    }
                    LoadProgressBar.IsActive = false;
                    GetLocalDataIcon.Visibility = Visibility.Visible;
                    GetCloudDataIcon.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception err)
            {
                PopupNotice popupNotice = new PopupNotice("获取云端数据错误：" + err.Message);
                popupNotice.ShowAPopup();
            }
        }

        private void GetCloudDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (localSettings.Values["UserName"] != null && localSettings.Values["Token"] != null)
                {
                    LoadProgressBar.IsActive = true;
                    SyncManager syncManager = new SyncManager(localSettings.Values["UserName"].ToString(), localSettings.Values["Token"].ToString());
                    string data = syncManager.GetCloudTaskData().Trim().Replace("\n", "");
                    JArray taskListJson = (JArray)JsonConvert.DeserializeObject(data);
                    listJson = taskListJson;
                    LoadProgressBar.IsActive = false;
                    TaskViewModel.ToDoDatas.Clear();
                    foreach (var item in taskListJson)
                    {
                        if (item["IsDelete"].ToString().Equals("0"))
                            TaskViewModel.ToDoDatas.Add(new ToDoTasks() { TaskId = item["TaskId"].ToString(), Name = item["TaskName"].ToString() });
                    }
                    LoadProgressBar.IsActive = false;
                    GetLocalDataIcon.Visibility = Visibility.Collapsed;
                    GetCloudDataIcon.Visibility = Visibility.Visible;
                }
            }
            catch (Exception err)
            {
                PopupNotice popupNotice = new PopupNotice("获取云端数据错误：" + err.Message);
                popupNotice.ShowAPopup();
            }
        }

        private void ToDoTaskList_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void ToDoTaskList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var clickItem = (e.OriginalSource as FrameworkElement)?.DataContext as ToDoTasks;
            if (clickItem == null)
                return;
            if(listJson != null)
            {
                foreach (var item in listJson)
                {
                    if (item["TaskId"].ToString().Equals(clickItem.TaskId))
                    {
                        string stepsStr = item["TaskStepList"].ToString();
                        JArray steps = (JArray)JsonConvert.DeserializeObject(stepsStr);
                        StringBuilder sb = new StringBuilder();
                        sb.Append("内容：").Append("\n");
                        for(int i = 0; i < steps.Count; i++)
                        {
                            var subItem = steps[i];
                            if (subItem["IsDelete"].ToString().Equals("1"))
                                continue;
                            if(i != steps.Count - 1)
                            {
                                sb.Append(" - ").Append(subItem["Content"].ToString()).Append("\n");
                            }
                            else
                            {
                                sb.Append(" - ").Append(subItem["Content"].ToString());
                            }
                        }
                        if (sb.ToString().Equals("步骤：\n"))
                        {
                            sb.Append("无");
                        }
                        StepFlyout.Text = sb.ToString();
                        break;
                    }
                }
            }
        }

        private void SyncWayItem1_Click(object sender, RoutedEventArgs e)
        {
            SyncWaySplitButton.Content = "合并两端数据";
        }

        private void SyncWayItem2_Click(object sender, RoutedEventArgs e)
        {
            SyncWaySplitButton.Content = "同步云端数据到本地";
        }

        private void SyncWayItem3_Click(object sender, RoutedEventArgs e)
        {
            SyncWaySplitButton.Content = "同步本地数据到云端";
        }

        private void SyncWaySplitButton_Click(SplitButton sender, SplitButtonClickEventArgs args)
        {
            string way1Str = "合并两端数据";
            string way2Str = "同步云端数据到本地";
            string way3Str = "同步本地数据到云端";
            string selectedWayStr = SyncWaySplitButton.Content.ToString();
            SyncManager syncManager = new SyncManager(localSettings.Values["UserName"].ToString(), localSettings.Values["Token"].ToString());
            if (way1Str.Equals(selectedWayStr))
            {
                syncManager.MergeData();
            }
            else if(way2Str.Equals(selectedWayStr))
            {
                syncManager.UseCloudData();
            }
            else if(way3Str.Equals(selectedWayStr))
            {
                syncManager.UseLocalData();
            }
            else
            {

            }
            TaskViewModel.ToDoDatas.Clear();
            GetLocalDataIcon.Visibility = Visibility.Collapsed;
            GetCloudDataIcon.Visibility = Visibility.Collapsed;
        }

        private async void AboutDialogSecondryButton_Click(object sender, RoutedEventArgs e)
        {
            var Uri = new Uri("ms-windows-store://review/?productid=9PKBWKPCCFJ8");
            await Launcher.LaunchUriAsync(Uri);
        }

        private void AboutDialogPrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            AboutContent.Hide();
        }

        private void AboutDialogCloseButton_Click(object sender, RoutedEventArgs e)
        {
            AboutContent.Hide();
        }

        private async void BirthDialogPrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            if (PersonalBirthday.Text == "未设置")
            {
                MessageDialog AboutDialog = new MessageDialog("您还没有设置生日哦，赶紧去设置吧。", "提示");
                await AboutDialog.ShowAsync();
                BirthDialog.Hide();
            }
            else
            {
                try
                {
                    DateTime birthday = Convert.ToDateTime(PersonalBirthday.Text);
                    string Tip = "";
                    string _birthday = birthday.ToString("yyyy-MM-dd");
                    All.Current.conn.Insert(new DataTemple() { Schedule_name = "出生日", CalculatedDate = CustomData.Calculator(_birthday), Date = _birthday, BgColor = "#fffbb612", TintOpacity = 0.7, IsTop = "0", AddTime = "" });
                    localSettings.Values["出生日" + _birthday] = Tip;
                    MainPage.Current.MyNav.SelectedItem = MainPage.Current.MyNav.MenuItems[0];
                    Frame.Navigate(typeof(All));
                    PopupNotice popupNotice = new PopupNotice("添加成功");
                    popupNotice.ShowAPopup();
                }
                catch
                {
                    MessageDialog AboutDialog = new MessageDialog("您已经添加过了哦。", "提示");
                    await AboutDialog.ShowAsync();
                }
            }
        }

        private void BirthDialogCloseButton_Click(object sender, RoutedEventArgs e)
        {
            BirthDialog.Hide();
        }

        private void CropperDialogCloseButton_Click(object sender, RoutedEventArgs e)
        {
            CropperDialog.Hide();
        }

        private async void CropperDialogPrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            var imageSource = EditPicture.ProfilePicture;
            byte[] imageBuffer;
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await localFolder.CreateFileAsync("temp.jpg", CreationCollisionOption.ReplaceExisting);
            try
            {
                using (var ras = await file.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None))
                {
                    WriteableBitmap bitmap = imageSource as WriteableBitmap;
                    var stream = bitmap.PixelBuffer.AsStream();
                    byte[] buffer = new byte[stream.Length];
                    await stream.ReadAsync(buffer, 0, buffer.Length);
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ras);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, 96.0, 96.0, buffer);
                    await encoder.FlushAsync();
                    var imageStream = ras.AsStream();
                    imageStream.Seek(0, SeekOrigin.Begin);
                    imageBuffer = new byte[imageStream.Length];
                    var re = await imageStream.ReadAsync(imageBuffer, 0, imageBuffer.Length);
                }
                await file.DeleteAsync(StorageDeleteOption.Default);
                List<PersonPictures> datalist = conn.Query<PersonPictures>("select * from PersonPictures where pictureName = ?", "picture");
                if (datalist != null)
                    conn.Execute("delete from PersonPictures where pictureName = ?", "picture");
                conn.Insert(new PersonPictures() { pictureName = "picture", picture = imageBuffer });
                SetPersonPicture();
                PopupNotice popupNotice = new PopupNotice("头像已更新");
                popupNotice.ShowAPopup();
            }
            catch
            {
                TempPicture.Visibility = Visibility.Visible;
                EditPicture.Visibility = Visibility.Collapsed;
            };
            CropperDialog.Hide();
        }

        private void AutoStartTipDialogPrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            AutoStartTipDialog.Hide();
        }
    }
}
