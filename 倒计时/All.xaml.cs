using SQLite.Net.Platform.WinRT;
using SQLitePCL;
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
        public double MyNavCMTW = MainPage.Current.MyNav.CompactModeThresholdWidth;
        public static All Current;
        public string Model_event;
        public string Model_Date;
        public CustomData SelectedItem;
        public String dbname;
        public int _index;
        private double percentage;
        private bool TopTap;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        
        public All()
        {
            this.InitializeComponent();
            //建立数据库连接   
            conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), path);
            //建表              
            conn.CreateTable<DataTemple>(); //默认表名同范型参数    
            //以下等效上面的建表   
            //conn.CreateTable(typeof(DataTemple));  
            Current = this;
            TopTap = true;
            Today.Text= DateTime.Now.ToShortDateString().ToString();
            TopText.Text = "今年你已经走过了" + DateTime.Now.DayOfYear.ToString() + "天啦！";
            MyProgressBar.Value = 100 * (DateTime.Now.DayOfYear / MyProgressBar.Width);
            loadSettings();
            loadDateData();
            //localSettings.Values["TopDate"] = null;
        }

        private void loadDateData()
        {
            ViewModel.CustomDatas.Clear();
            List<DataTemple> datalist0 = conn.Query<DataTemple>("select * from DataTemple where Schedule_name = ?", localSettings.Values["TopDate"]);
            List<DataTemple> datalist1 = conn.Query<DataTemple>("select * from DataTemple where Date >= ? order by Date asc", DateTime.Now.ToString("yyyy-MM-dd"));
            List<DataTemple> datalist2 = conn.Query<DataTemple>("select * from DataTemple where Date < ? order by Date desc", DateTime.Now.ToString("yyyy-MM-dd"));
            if ((datalist1.Count() + datalist2.Count()) == 0)
            {
                NewTB.Visibility = Visibility.Visible;
                NewTB2.Visibility = Visibility.Visible;
            }
            else
            {
                NewTB.Visibility = Visibility.Collapsed;
                NewTB2.Visibility = Visibility.Collapsed;
            }
            foreach (var item in datalist0)
            {
                if(item!=null)
                    ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = CustomData.Calculator(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
            }
            if(localSettings.Values["TopDate"]==null)
            {
                foreach (var item in datalist1)
                {
                    ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = CustomData.Calculator(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                }
                foreach (var item in datalist2)
                {
                    ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = CustomData.Calculator(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                }
            }
            else
            {
                foreach (var item in datalist1)
                {
                    if(item.Schedule_name!=localSettings.Values["TopDate"].ToString())
                        ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = CustomData.Calculator(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                }
                foreach (var item in datalist2)
                {
                    if (item.Schedule_name != localSettings.Values["TopDate"].ToString())
                        ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = CustomData.Calculator(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                }
            }
            
        }

        private void loadSettings()
        {
            if (localSettings.Values["SetAllPageAcrylic"] != null)
            {
                if (localSettings.Values["SetAllPageAcrylic"].Equals(true))
                {
                    AcrylicBrush myBrush = new AcrylicBrush();
                    myBrush.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
                    myBrush.TintColor = Color.FromArgb(255, 255, 255, 255);
                    myBrush.FallbackColor = Color.FromArgb(255, 255, 255, 255);
                    myBrush.TintOpacity = 0.8;
                    AllPageStackPanel.Background = myBrush;
                }
                else
                {
                    AllPageStackPanel.Background = new SolidColorBrush(Colors.White);
                }
            }
        }

        public AcrylicBrush ColorfulBrush(Color temp,double tintOpacity)
        {
            AcrylicBrush myBrush = new AcrylicBrush();
            myBrush.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
            myBrush.TintColor = temp;
            myBrush.TintColor = temp;
            myBrush.FallbackColor = temp;
            myBrush.TintOpacity = tintOpacity;
            return myBrush;
        }

        private string Calculator(string s1)
        {
            string str1 = s1;
            string str2 = DateTime.Now.ToShortDateString().ToString();
            string s2, s3;
            DateTime d1 = Convert.ToDateTime(str1);
            DateTime d2 = Convert.ToDateTime(str2);
            DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
            DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));
            int days = (d4 - d3).Days;
            if (days < 0)
            {
                s3 = "还有" + App.term(Convert.ToDateTime(d4), Convert.ToDateTime(d3));
                days = Math.Abs(days);
                s2 = "还有" + days.ToString() + "天";
            }
            else
            {
                s3 = "已过" + App.term(Convert.ToDateTime(d3), Convert.ToDateTime(d4));
                s2 = "已过" + days.ToString() + "天";
            }
            return s2;
        }


        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //((NavigationViewItem)MainPage.Current.MyNav.MenuItems[2]).IsSelected = true;
            MainPage.Current.MyNav.SelectedItem = MainPage.Current.MyNav.MenuItems[2];
            Frame.Navigate(typeof(Add));
        }

        private void MyGirdView_ItemClick(object sender, ItemClickEventArgs e)
        {
            
            var _item = (CustomData)e.ClickedItem;
            //SelectedItem = _item;
            str1 = _item.Str1;
            str2 = _item.Str2;
            str3 = _item.Str3;
            str4 = _item.Str4;
            BgsColor = _item.BackGroundColor;
           
            MainPage.Current.SelectedPage = true;
            Frame.Navigate(typeof(Details));
        }

        private void MyGirdView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string key = e.AddedItems[0].ToString();
                
            }
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            
        }

        private void MyGirdView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var _item = (e.OriginalSource as FrameworkElement)?.DataContext as CustomData;
            SelectedItem = _item;
            if (SelectedItem != null)
            {
                MyFlyout.IsEnabled = true;
                if (localSettings.Values["TopDate"] == null)
                {
                    FS.Visibility = Visibility.Visible;
                    SetTop.Visibility = Visibility.Visible;
                    SetTop.IsEnabled = true;
                    Cancel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (localSettings.Values["TopDate"].ToString() == SelectedItem.Str1)
                    {
                        FS.Visibility = Visibility.Visible;
                        SetTop.Visibility = Visibility.Collapsed;
                        Cancel.Visibility = Visibility.Visible;
                        Cancel.IsEnabled = true;
                    }
                    else
                    {
                        SetTop.Visibility = Visibility.Collapsed;
                        Cancel.Visibility = Visibility.Collapsed;
                        FS.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                MyFlyout.IsEnabled = false;
                FS.Visibility = Visibility.Collapsed;
                SetTop.Visibility = Visibility.Collapsed;
                Cancel.Visibility = Visibility.Collapsed;
            }

        }

        private void MyGridView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AbbFlyout.Hide();
        }

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await DeleteDialog.ShowAsync();
        }

        private void TopText_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (TopTap == true)
            {
                TopTap = false;
                percentage = 100 * (DateTime.Now.DayOfYear / MyProgressBar.Width);
                percentage = (int)percentage;
                TopText.Text = "今年你已经走过了" + percentage.ToString() + "%啦！";
            }
            else
            {
                TopText.Text = "今年你已经走过了" + DateTime.Now.DayOfYear.ToString() + "天啦！";
                TopTap = true;
            }
        }

        private void MyGridView_Drop(object sender, DragEventArgs e)
        {
           // ViewModel.CustomDatas.Add(new CustomData() { Str1 = "排序", Str2 = CustomData.Calculator("2018/2/2"), Str3 = "2018/2/2", Str4 = ColorfulBrush(Colors.SkyBlue, 0.6), BackGroundColor = Colors.SkyBlue });
        }

        private void SetTop_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["TopDate"] = SelectedItem.Str1;
            loadDateData();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["TopDate"] = null;
            loadDateData();
        }

        private void DeleteDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (localSettings.Values["TopDate"] != null)
            {
                if (localSettings.Values["TopDate"].ToString() == SelectedItem.Str1)
                    localSettings.Values["TopDate"] = null;
            }
            int _start = ViewModel.CustomDatas.Count();
            ViewModel.CustomDatas.Remove(SelectedItem);
            conn.Execute("delete from DataTemple where Schedule_name = ?", SelectedItem.Str1);
            int _end = ViewModel.CustomDatas.Count();
            if (_start != _end)
            {
                PopupNotice popupNotice = new PopupNotice("删除成功");
                popupNotice.ShowAPopup();
            }

            List<DataTemple> datalist = conn.Query<DataTemple>("select * from DataTemple");
            if (datalist.Count() == 0)
            {
                NewTB.Visibility = Visibility.Visible;
                NewTB2.Visibility = Visibility.Visible;
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
