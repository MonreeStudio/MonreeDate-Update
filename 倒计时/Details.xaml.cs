using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using 倒计时.Models;
using 夏日;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 倒计时
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Details : Page
    {
        public static Details Current;
        private bool sp;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "mydb.sqlite");    //建立数据库  
        public SQLite.Net.SQLiteConnection conn;
        DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
        IRandomAccessStream Bitmap;
        string DetailsDateMode;
        string FestivalDateMode;
        string dayNum;
        public Details()
        {
            this.InitializeComponent();
            Current = this;
            LoadData();
            SetBorderColor();
            SetThemeColor();
            SetAlertEnabled();
            DataTransferManager.GetForCurrentView().DataRequested += DataTransferManager_DataRequested;
            MainPage.Current.MyNav.IsBackEnabled = true;
            MainPage.Current.SelectedPageItem = "Details";
            DetailsDateMode = localSettings.Values["DateMode"].ToString();
            FestivalDateMode = "Day";
            
        }

        private void SetAlertEnabled()
        {
            var selectedDate = Convert.ToDateTime(DetailsPickedDate.Text);
            if (selectedDate > DateTime.Now)
                AlertButton.IsEnabled = true;
            else
                AlertButton.IsEnabled = false;
        }
        private void SetThemeColor()
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

        private void SetBorderColor()
        {
            YellowRec.Fill = new SolidColorBrush(Color.FromArgb(255,246,247,231));
            BlueRec.Fill = new SolidColorBrush(Color.FromArgb(255, 235, 246, 252));
            GreenRec.Fill = new SolidColorBrush(Color.FromArgb(255, 241, 253, 241));
            PinkRec.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 246, 239));
            RenderGrid.Background = BlueRec.Fill;
            RenderTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 106, 164, 210));
        }
        private void LoadData()
        {
            sp = MainPage.Current.SelectedPage;
            if (sp == true)
            {
                DetailsPickedDate.Text = App.AllItem.Str3;
                DetailsEvent.Text = App.AllItem.Str1;
                DetailsDate.Text = App.AllItem.Str2;
                LunarDate.Text = LunarCalendar.GetChineseDateTime(Convert.ToDateTime(App.AllItem.Str3));
                DetailsGrid.Background = App.AllItem.Str4;
                DetailsDate.Foreground = new SolidColorBrush(App.AllItem.BackGroundColor);
                if (localSettings.Values[App.AllItem.Str1 + App.AllItem.Str3] != null)
                    TipText.Text = localSettings.Values[App.AllItem.Str1 + App.AllItem.Str3].ToString();
            }
            else
            {
                EditButton.IsEnabled = false;
                DetailsEvent.Text = App.FestivalItem.Str1;
                DetailsDate.Text = App.FestivalItem.Str2;
                DetailsPickedDate.Text = App.FestivalItem.Str3;
                LunarDate.Text = LunarCalendar.GetChineseDateTime(Convert.ToDateTime(App.FestivalItem.Str3));
                DetailsGrid.Background = ColorfulBrush(App.FestivalItem.Str4);
                DetailsDate.Foreground = new SolidColorBrush(App.FestivalItem.Str4);
                TipText.Text = "节日";
            }
            if (localSettings.Values["Lunar"] != null)
            {
                if (localSettings.Values["Lunar"].ToString() == "1")
                    LunarDate.Visibility = Visibility.Visible;
                else
                    LunarDate.Visibility = Visibility.Collapsed;
            }
            else
                LunarDate.Visibility = Visibility.Visible;
            dEvent.Text = DetailsEvent.Text;
            dCalDate.Text = DetailsDate.Text;
            dDate.Text = DetailsPickedDate.Text;
            RenderPicture.ProfilePicture = All.Current.AllPicture.ProfilePicture;
        }

        public static AcrylicBrush ColorfulBrush(Color temp)
        {
            AcrylicBrush myBrush = new AcrylicBrush();
            myBrush.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
            myBrush.TintColor = temp;
            myBrush.TintColor = temp;
            myBrush.FallbackColor = temp;
            myBrush.TintOpacity = 0.8;
            return myBrush;
        }


        private void DetailsDate_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (MainPage.Current.SelectedPage)
            {
                DateTime d1 = Convert.ToDateTime(All.Current.str3);
                DateTime d2 = DateTime.Now;
                DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
                DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));
                switch (DetailsDateMode)
                {
                    case "Day":
                        DetailsDate.Text = All.Current.ConvertToWeek(All.Current.str3);
                        DetailsDateMode = "Week";
                        PopupNotice popupNotice0 = new PopupNotice("周数模式");
                        popupNotice0.ShowAPopup();
                        break;
                    case "Week":
                        if (d4 > d3)
                            DetailsDate.Text = "已过" + App.Term(d3, d4);
                        else
                        {
                            if (d4 < d3)
                                DetailsDate.Text = "还有" + App.Term(d4, d3);
                        }
                        DetailsDateMode = "Year";
                        PopupNotice popupNotice1 = new PopupNotice("年数模式");
                        popupNotice1.ShowAPopup();
                        break;
                    case "Year":
                        DetailsDate.Text = CustomData.Calculator(All.Current.str3);
                        DetailsDateMode = "Day";
                        PopupNotice popupNotice2 = new PopupNotice("天数模式");
                        popupNotice2.ShowAPopup();
                        break;
                }
                dCalDate.Text = DetailsDate.Text;
            }
            else
            {
                DateTime d1 = Convert.ToDateTime(Festival.Current.str3);
                DateTime d2 = DateTime.Now;
                DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
                DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));

                switch (FestivalDateMode)
                {
                    case "Day":
                        DetailsDate.Text = All.Current.ConvertToWeek(Festival.Current.str3);
                        FestivalDateMode = "Week";
                        PopupNotice popupNotice0 = new PopupNotice("周数模式");
                        popupNotice0.ShowAPopup();
                        break;
                    case "Week":
                        if (d4 > d3)
                            DetailsDate.Text = "已过" + App.Term(d3, d4);
                        else
                        {
                            if (d4 < d3)
                                DetailsDate.Text = "还有" + App.Term(d4, d3);
                        }
                        FestivalDateMode = "Year";
                        PopupNotice popupNotice1 = new PopupNotice("年数模式");
                        popupNotice1.ShowAPopup();
                        break;
                    case "Year":
                        DetailsDate.Text = CustomData.Calculator(Festival.Current.str3);
                        FestivalDateMode = "Day";
                        PopupNotice popupNotice2 = new PopupNotice("天数模式");
                        popupNotice2.ShowAPopup();
                        break;
                }
                dCalDate.Text = DetailsDate.Text;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EditDetails),null,new DrillInNavigationTransitionInfo());
        }

        private void DetailsDate_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 0);
            DetailsDate.FontSize = 52;
        }

        private void DetailsDate_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
            DetailsDate.FontSize = 48;
        }

        private async void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            await RenderDialog.ShowAsync();
        }

        public static async Task<WriteableBitmap> RenderUIElement(UIElement element)
        {
            var bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(element);
            var pixelBuffer = await bitmap.GetPixelsAsync();
            var pixels = pixelBuffer.ToArray();
            //var writeableBitmap = new WriteableBitmap(500, 300);
            var writeableBitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight);
            using (Stream stream = writeableBitmap.PixelBuffer.AsStream())
            {
                await stream.WriteAsync(pixels, 0, pixels.Length);
            }
            return writeableBitmap;
        }

        private async void Share_Click(object sender, RoutedEventArgs e)
        {
            var image = await RenderUIElement(RenderBorder);
            Bitmap = await ConvertWriteableBitmapToRandomAccessStream(image);
            //var bitmap = RandomAccessStreamReference.CreateFromStream(bitImage);
            DataTransferManager.ShowShareUI();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= DataTransferManager_DataRequested;
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            
            //var bitmap = rasr.CreateFromStream(bitImage);
            //DataRequest request = args.Request;
            DataRequestDeferral deferral = args.Request.GetDeferral();
            args.Request.Data.Properties.Title = "分享日程";
            args.Request.Data.Properties.Description = "夏日——记录你的生活";
            args.Request.Data.SetText("分享自“夏日”");
            args.Request.Data.SetBitmap(RandomAccessStreamReference.CreateFromStream(Bitmap));
            deferral.Complete();
        }
        
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(RenderBorder);

            var saveFile = new FileSavePicker();
            saveFile.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            saveFile.FileTypeChoices.Add("JPEG files", new List<string>() { ".jpg" });
            saveFile.SuggestedFileName = "夏日：" + DetailsEvent.Text;
            StorageFile sFile = await saveFile.PickSaveFileAsync();
            if (sFile == null)
                return;

            var pixels = await rtb.GetPixelsAsync();
            using (IRandomAccessStream stream = await sFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await
                BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                byte[] bytes = pixels.ToArray();
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                        BitmapAlphaMode.Ignore,
                                        (uint)rtb.PixelWidth,
                                        (uint)rtb.PixelHeight,
                                        200,
                                        200,
                                        bytes);

                await encoder.FlushAsync();
            }
            PopupNotice popupNotice = new PopupNotice("保存成功");
            popupNotice.ShowAPopup();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            RenderDialog.Hide();
        }

        //private async void TestButton_Click(object sender, RoutedEventArgs e)
        //{
        //    ////var image = await RenderUIElement(RenderBorder);
        //    var rtb = new RenderTargetBitmap();
        //    await rtb.RenderAsync(RenderBorder);

        //    var saveFile = new FileSavePicker();
        //    saveFile.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        //    saveFile.FileTypeChoices.Add("JPEG files", new List<string>() { ".jpg" });
        //    saveFile.SuggestedFileName = "夏日："+ DetailsEvent.Text;
        //    StorageFile sFile = await saveFile.PickSaveFileAsync();
        //    if (sFile == null)
        //        return;

        //    var pixels = await rtb.GetPixelsAsync();
        //    using (IRandomAccessStream stream = await sFile.OpenAsync(FileAccessMode.ReadWrite))
        //    {
        //        var encoder = await
        //        BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
        //        byte[] bytes = pixels.ToArray();
        //        encoder.SetPixelData(BitmapPixelFormat.Bgra8,
        //                                BitmapAlphaMode.Ignore,
        //                                (uint)rtb.PixelWidth,
        //                                (uint)rtb.PixelHeight,
        //                                200,
        //                                200,
        //                                bytes);

        //        await encoder.FlushAsync();
        //    }
        //    PopupNotice popupNotice = new PopupNotice("保存成功");
        //    popupNotice.ShowAPopup();
        //}
        public static async Task<IRandomAccessStream> ConvertWriteableBitmapToRandomAccessStream(WriteableBitmap writeableBitmap)
        {
            var stream = new InMemoryRandomAccessStream();

            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
            Stream pixelStream = writeableBitmap.PixelBuffer.AsStream();
            byte[] pixels = new byte[pixelStream.Length];
            await pixelStream.ReadAsync(pixels, 0, pixels.Length);

            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)writeableBitmap.PixelWidth, (uint)writeableBitmap.PixelHeight, 96.0, 96.0, pixels);
            await encoder.FlushAsync();

            return stream;
        }

        private void YellowRec_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RenderGrid.Background = new SolidColorBrush(Color.FromArgb(255, 246, 247, 231));
            RenderTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 238, 177, 60));
        }

        private void BlueRec_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RenderGrid.Background = new SolidColorBrush(Color.FromArgb(255, 235, 246, 252));
            RenderTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 106, 164, 210));
        }

        private void GreenRec_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RenderGrid.Background = new SolidColorBrush(Color.FromArgb(255, 241, 253, 241));
            RenderTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 131, 179, 141));
        }

        private void PinkRec_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RenderGrid.Background = new SolidColorBrush(Color.FromArgb(255, 255, 246, 239));
            RenderTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 230, 129, 111));
        }

        private void AlertToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            //ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            var AlertName = "Alert" + DetailsEvent.Text;
            if (AlertToggleSwitch!=null)
            {
                if (AlertToggleSwitch.IsOn == true)
                {
                    localSettings.Values[AlertName] = "1";

                }
                else
                {
                    localSettings.Values[AlertName] = "0";
                }
            }
            //toggleSwitch.Toggled += AlertToggleSwitch_Toggled;
        }

        private async void AlertButton_Click(object sender, RoutedEventArgs e)
        {
            var AlertName = "Alert" + DetailsEvent.Text;
            if (localSettings.Values[AlertName] != null)
            {
                if (localSettings.Values[AlertName].ToString() == "1")
                    AlertToggleSwitch.IsOn = true;
                else
                    AlertToggleSwitch.IsOn = false;
            }
            else
            {
                AlertToggleSwitch.IsOn = false;
                localSettings.Values[AlertName] = "0";
            }
            var AlertName2 = "Alert" + DetailsEvent.Text + 1;
            if (localSettings.Values[AlertName2] != null)
            {
                if (localSettings.Values[AlertName2].ToString() == "1")
                    AlertToggleSwitch2.IsOn = true;
                else
                    AlertToggleSwitch2.IsOn = false;
            }
            else
            {
                AlertToggleSwitch2.IsOn = false;
                localSettings.Values[AlertName2] = "0";
            }
            var AlertName3 = "Alert" + DetailsEvent.Text + 3;
            if (localSettings.Values[AlertName3] != null)
            {
                if (localSettings.Values[AlertName3].ToString() == "1")
                    AlertToggleSwitch3.IsOn = true;
                else
                    AlertToggleSwitch3.IsOn = false;
            }
            else
            {
                AlertToggleSwitch3.IsOn = false;
                localSettings.Values[AlertName3] = "0";
            }
            var AlertName4 = "Alert" + DetailsEvent.Text + "Personal";
            if (localSettings.Values[AlertName4] != null)
            {
                if (localSettings.Values[AlertName4].ToString() != "0")
                {
                    AlertToggleSwitch4.IsOn = true;
                    DayComboBox.IsEnabled = true;
                    var indexValue = localSettings.Values[AlertName4].ToString();
                    if (indexValue == "2")
                        DayComboBox.SelectedIndex = 0;
                    else
                    {
                        DayComboBox.SelectedIndex = Convert.ToInt32(indexValue) - 3;
                    }
                }

                else
                {
                    AlertToggleSwitch4.IsOn = false;
                    DayComboBox.IsEnabled = false;
                }
            }
            else
                DayComboBox.IsEnabled = false;
            await AlertContentDialog.ShowAsync();
        }

        private void CloseContentDialogBtn_Click(object sender, RoutedEventArgs e)
        {
            AlertContentDialog.Hide();
        }

        private void ToastTestButton_Click(object sender, RoutedEventArgs e)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
            {
                new AdaptiveText()
                {
                    Text = "日程到期提醒："+ DetailsEvent.Text
                },
                new AdaptiveText()
                {
                    Text = "备注："+TipText.Text
                }
            }
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
        {
            new ToastButton("打开夏日", "action")
            {
                ActivationType = ToastActivationType.Foreground
            }
        }
                },
                Audio = new ToastAudio()
                {
                    Src = new Uri("ms-winsoundevent:Notification.Looping.Alarm")
                }
            };

            // Create the toast notification
            
            var toastNotif = new ToastNotification(toastContent.GetXml());
            toastNotif.ExpirationTime = DateTime.Now.AddDays(1);
            
            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        private void AlertToggleSwitch2_Toggled(object sender, RoutedEventArgs e)
        {
            var AlertName = "Alert" + DetailsEvent.Text + 1;
            if (AlertToggleSwitch2 != null)
            {
                if (AlertToggleSwitch2.IsOn == true)
                {
                    localSettings.Values[AlertName] = "1";
                }
                else
                {
                    localSettings.Values[AlertName] = "0";
                }
            }
        }

        private void AlertToggleSwitch3_Toggled(object sender, RoutedEventArgs e)
        {
            var AlertName = "Alert" + DetailsEvent.Text + 3;
            if (AlertToggleSwitch3 != null)
            {
                if (AlertToggleSwitch3.IsOn == true)
                {
                    localSettings.Values[AlertName] = "1";

                }
                else
                {
                    localSettings.Values[AlertName] = "0";
                }
            }
        }

        private void AlertToggleSwitch4_Toggled(object sender, RoutedEventArgs e)
        {
            
            var AlertName = "Alert" + DetailsEvent.Text + "Personal";
            if (AlertToggleSwitch4 != null)
            {
                if (AlertToggleSwitch4.IsOn == true)
                {
                    //localSettings.Values[AlertName] = dayNum;
                    DayComboBox.IsEnabled = true;
                }
                else
                {
                    localSettings.Values[AlertName] = "0";
                    DayComboBox.IsEnabled = false;
                }
            }
            else
                AlertToggleSwitch4.IsOn = false;
        }

        private void DayComboBox_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
        {
            
        }

        private void DayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dayNum = e.AddedItems[0].ToString();
            if (AlertToggleSwitch4 != null && AlertToggleSwitch4.IsOn == true)
            {
                var AlertName = "Alert" + DetailsEvent.Text + "Personal";
                localSettings.Values[AlertName] = dayNum;
            }
        }
    }
}
