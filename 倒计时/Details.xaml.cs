using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using Windows.UI.Popups;
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
        public Details()
        {
            this.InitializeComponent();
            Current = this;
            LoadData();
            SetBorderColor();
            SetThemeColor();
            DataTransferManager.GetForCurrentView().DataRequested += DataTransferManager_DataRequested;
            MainPage.Current.MyNav.IsBackEnabled = true;
            MainPage.Current.SelectedPageItem = "Details";
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
                DetailsGrid.Background = ColorfulBrush(App.FestivalItem.Str4);
                DetailsDate.Foreground = new SolidColorBrush(App.FestivalItem.Str4);
                TipText.Text = "节日";
            }
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

                if (DetailsDate.Text == CustomData.Calculator(All.Current.str3))
                {
                    if (d4 > d3)
                        DetailsDate.Text = "已过" + App.Term(d3, d4);
                    else
                    {
                        if (d4 < d3)
                            DetailsDate.Text = "还有" + App.Term(d4, d3);
                    }
                }
                else
                    DetailsDate.Text = CustomData.Calculator(All.Current.str3);
            }
            else
            {
                DateTime d1 = Convert.ToDateTime(Festival.Current.str3);
                DateTime d2 = DateTime.Now;
                DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
                DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));

                if (DetailsDate.Text == Festival.Current.str2)
                {
                    if (d4 > d3)
                        DetailsDate.Text = "已过" + App.Term(d3, d4);
                    else
                    {
                        if (d4 < d3)
                            DetailsDate.Text = "还有" + App.Term(d4, d3);
                    }
                }
                else
                    DetailsDate.Text = Festival.Current.str2;
            }
            dCalDate.Text = DetailsDate.Text;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EditDetails),null,new DrillInNavigationTransitionInfo());
        }

        private void DetailsDate_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 0);
            DetailsDate.FontSize = 58;
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
    }
}
