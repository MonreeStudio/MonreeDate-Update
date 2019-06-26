using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

        public Settings()
        {
            this.InitializeComponent();
            Current = this;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            Loaded += OnSettingsPageLoaded;

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
                //MyPersonPicture.ProfilePicture = (BitmapImage)_PersonPicture;
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
            MessageDialog AboutDialog = new MessageDialog("emmmmm\n...........");
            await AboutDialog.ShowAsync();
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
            var srcImage = new BitmapImage();
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    await srcImage.SetSourceAsync(stream);
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
                if (result.Status == LaunchUriStatus.Success && result.Result != null)
                {
                    try
                    {
                        var stream = await destination.OpenReadAsync();
                        var bitmap = new BitmapImage();
                        await bitmap.SetSourceAsync(stream);
                        MyPersonPicture.ProfilePicture = bitmap;
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
    }
}
