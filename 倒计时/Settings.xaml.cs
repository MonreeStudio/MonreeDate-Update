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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

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
        public int index = 0;

      //  private StorageFile sampleFile;
      //  private string filename = "sampleFile.dat";
        StorageFolder storageFolder = ApplicationData.Current.LocalFolder;


        public Settings()
        {
            this.InitializeComponent();
            Current = this;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            Loaded += OnSettingsPageLoaded;
            ReadSettings();
            GetAppVersion();
            GetSystemVersion();
            GetPlatform();
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
            //Package package = Package.Current;
            //reminder = package.Id.Architecture.ToString();
            //reminder = package.DisplayName;
            //EasClientDeviceInformation eas = new EasClientDeviceInformation();
            //reminder = eas.SystemManufacturer;
            SystemVersion.Text = "系统版本：Windows "+ reminder;
        }

        private void GetPlatform()
        {
            Package package = Package.Current;
            Platform.Text = "平台架构：" + package.Id.Architecture.ToString();
        }

        private static async Task<BitmapImage> OpenWriteableBitmapFile(StorageFile file)
        {

            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
            {
                //BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                //WriteableBitmap image = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                //image.SetSource(stream);

                //return image;

                var fileStream = await file.OpenReadAsync();
                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(fileStream);
                return bitmap;
                
            }
        }

        public static object GetSetting(string name)
        {
            if (localSettings.Values.ContainsKey(name))
            {
                return localSettings.Values[name];
            }
            else
            {
                return null;
            }
        }

        public static void RemoveSetting(string name)
        {
            if (localSettings.Values.ContainsKey(name))
            {
                localSettings.Values.Remove(name);
            }
            else
            {
                //
            }
        }

        private void OnSettingsPageLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private async void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            await AboutContent.ShowAsync();
            //MessageDialog AboutDialog = new MessageDialog("emmmmm\n...........");
            //await AboutDialog.ShowAsync();
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

        private void element_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            // Scale back down to 1.0
            CreateOrUpdateSpringAnimation(1.0f);

            (sender as UIElement).StartAnimation(_springAnimation);
        }

        // Gets the rectangle of the element 

        private async void MyPersonPicture_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MessageDialog AboutDialog = new MessageDialog("更换头像的功能正在紧急开发中，\n您依旧可以在本地选择头像，但暂时不会被保存。\n","非常抱歉！");
            AboutDialog.Commands.Add(new UICommand("继续加油", cmd => { }, commandId: 0));
            AboutDialog.Commands.Add(new UICommand("好的收到", cmd => { }, commandId: 1));
            await AboutDialog.ShowAsync();
            
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
                    MyPersonPicture.ProfilePicture = srcImage;
                    //localSettings.Values["PersonPicture"] = srcImage;

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
                //Guid bitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
                //using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None))
                //{
                //    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(bitmapEncoderGuid, stream);
                //    WriteableBitmap bmp = new WriteableBitmap(srcImage.PixelWidth, srcImage.PixelHeight);
                //    Stream pixelStream = bmp.PixelBuffer.AsStream();

                //    byte[] pixels = new byte[pixelStream.Length];
                //    await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                //    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                //              (uint)bmp.PixelWidth,
                //              (uint)bmp.PixelHeight,
                //              96.0,
                //              96.0,
                //              pixels);
                //    await encoder.FlushAsync();
                //}

                if (result.Status == LaunchUriStatus.Success && result.Result != null)
                {
                    try
                    {
                        var stream = await destination.OpenReadAsync();
                        var bitmap = new BitmapImage();
                        await bitmap.SetSourceAsync(stream);
                        MyPersonPicture.ProfilePicture = bitmap;
                        index++;

                        await localFolder.CreateFileAsync("PersonPicture", CreationCollisionOption.ReplaceExisting);
                        localSettings.Values["PersonPicture"] = bitmap;
                      
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
    }
}
