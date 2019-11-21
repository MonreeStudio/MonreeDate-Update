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
using Windows.ApplicationModel;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Security.ExchangeActiveSyncProvisioning;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using 倒计时.Models;
using 夏日.Models;

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
        Compositor _compositor = Window.Current.Compositor;
        SpringVector3NaturalMotionAnimation _springAnimation;
        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        private StorageFile Picture_file;//UWP 采用StorageFile来读写文件
        public ThemeColorDataViewModel ViewModel = new ThemeColorDataViewModel();
        public Color color;
      //  private StorageFile sampleFile;
      //  private string filename = "sampleFile.dat";
        StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

        string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "mydb.sqlite");    //建立数据库  
        public SQLite.Net.SQLiteConnection conn;


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
            MainPage.Current.MyNav.IsBackEnabled = false;
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
                    break;
                case "DeepSkyBlue":
                    TC.Color = Colors.DeepSkyBlue;
                    MainPage.Current.SetThemeColor();
                    ThemeColorSelected.SelectedIndex = 1;
                    break;
                case "Orange":
                    TC.Color = Colors.Orange;
                    MainPage.Current.SetThemeColor();
                    ThemeColorSelected.SelectedIndex = 2;
                    break;
                case "Crimson":
                    TC.Color = Colors.Crimson;
                    MainPage.Current.SetThemeColor();
                    ThemeColorSelected.SelectedIndex = 3;
                    break;
                case "Gray":
                    TC.Color = Color.FromArgb(255, 73, 92, 105);
                    MainPage.Current.SetThemeColor();
                    ThemeColorSelected.SelectedIndex = 4;
                    break;
                default:
                    break;
            }
        }

        private void ReadSettings()
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

        //private static async Task<BitmapImage> OpenWriteableBitmapFile(StorageFile file)
        //{

        //    using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
        //    {
        //        var fileStream = await file.OpenReadAsync();
        //        var bitmap = new BitmapImage();
        //        await bitmap.SetSourceAsync(fileStream);
        //        return bitmap;
        //    }
        //}

        //public static object GetSetting(string name)
        //{
        //    if (localSettings.Values.ContainsKey(name))
        //    {
        //        return localSettings.Values[name];
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //public static void RemoveSetting(string name)
        //{
        //    if (localSettings.Values.ContainsKey(name))
        //    {
        //        localSettings.Values.Remove(name);
        //    }
        //    else
        //    {
        //        //
        //    }
        //}

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
                if (AllPageAcylic.IsOn == true)
                {
                    AcrylicBrush myBrush = new AcrylicBrush();
                    myBrush.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
                    myBrush.TintColor = Color.FromArgb(255, 255, 255, 255);
                    myBrush.FallbackColor = Color.FromArgb(255, 255, 255, 255);
                    myBrush.TintOpacity = 0.8;
                    All.Current.AllPageStackPanel.Background = myBrush;
                    localSettings.Values["SetAllPageAcrylic"] = true;
                    //Add.Current.AddPageGird.Background = myBrush;
                }
                else
                {
                    All.Current.AllPageStackPanel.Background = new SolidColorBrush(Colors.White);
                    localSettings.Values["SetAllPageAcrylic"] = false;
                    //Add.Current.AddPageGird.Background = new SolidColorBrush(Colors.White);
                }
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

        //private void element_PointerExited(object sender, PointerRoutedEventArgs e)
        //{
        //    // Scale back down to 1.0
        //    CreateOrUpdateSpringAnimation(1.0f);

        //    (sender as UIElement).StartAnimation(_springAnimation);
        //}


        private async void MyPersonPicture_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var srcImage = new BitmapImage();
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            StorageFile file = await openPicker.PickSingleFileAsync();
            Picture_file = file;
            if (file != null)
            {
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    await srcImage.SetSourceAsync(stream);
                    await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                }

            }
            if (file != null)
            {
                var inputFile = SharedStorageAccessManager.AddFile(file);
                var destination = await ApplicationData.Current.LocalFolder.CreateFileAsync("Cropped.jpg", CreationCollisionOption.ReplaceExisting);
                var destinationFile = SharedStorageAccessManager.AddFile(destination);
                var options = new LauncherOptions();
                options.TargetApplicationPackageFamilyName = "Microsoft.Windows.Photos_8wekyb3d8bbwe";
                var parameters = new ValueSet();
                parameters.Add("InputToken", inputFile);
                parameters.Add("DestinationToken", destinationFile);
                parameters.Add("ShowCamera", false);
                parameters.Add("EllipticalCrop", true);
                parameters.Add("CropWidthPixals", 300);
                parameters.Add("CropHeightPixals", 300);
                var result = await Launcher.LaunchUriForResultsAsync(new Uri("microsoft.windows.photos.crop:"), options, parameters);
                if (result.Status.Equals(LaunchUriStatus.Success) && result.Result != null)
                {
                    try
                    {
                        var stream = await destination.OpenReadAsync();
                        var bitmap = new BitmapImage();
                        using (var dataRender = new DataReader(stream))
                        {
                            if (stream.Size == 0)
                                return;
                            var imgBytes = new byte[stream.Size];
                            await dataRender.LoadAsync((uint)stream.Size);
                            dataRender.ReadBytes(imgBytes);
                            List<PersonPictures> datalist = conn.Query<PersonPictures>("select * from PersonPictures where pictureName = ?", "picture");
                            if (datalist != null)
                                conn.Execute("delete from PersonPictures where pictureName = ?", "picture");
                            conn.Insert(new PersonPictures() { pictureName = "picture", picture = imgBytes });
                            SetPersonPicture();
                        }
                        await localFolder.CreateFileAsync("PersonPicture", CreationCollisionOption.ReplaceExisting);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message + ex.StackTrace);
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Edit));
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
                    All.Current.MarginText.Height = 30;
                    localSettings.Values["SetAllPersonPicture"] = true;
                }
                else
                {
                    All.Current.AllPicture.Visibility = Visibility.Collapsed;
                    All.Current.MarginText.Height = 60;
                    localSettings.Values["SetAllPersonPicture"] = false;
                }
            }
            toggleSwitch.Toggled += AllPageAcylic_Toggled;
        }
    }
}
