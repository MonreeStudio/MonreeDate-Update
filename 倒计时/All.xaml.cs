﻿using SQLite.Net.Platform.WinRT;
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
        private ApplicationDataContainer _appSettings;  
        public CustomDataViewModel ViewModel = new CustomDataViewModel();
        public string str1, str2, str3, str4;
        public double MyNavCMTW = MainPage.Current.MyNav.CompactModeThresholdWidth;
        public static All Current;
        public string Model_event;
        public string Model_Date;
        public CustomData SelectedItem;
        public String dbname;
        public int _index;
        private double percentage;
        public All()
        {
            this.InitializeComponent();
            this.InitializeComponent();
            //建立数据库连接   
            conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), path);
            //建表              
            conn.CreateTable<DataTemple>(); //默认表名同范型参数    
            //以下等效上面的建表   
            //conn.CreateTable(typeof(DataTemple));  
            
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            _appSettings = ApplicationData.Current.LocalSettings;
            BindKeyList();
            Current = this;
            Today.Text= DateTime.Now.ToShortDateString().ToString();
            percentage = 100 * (DateTime.Now.DayOfYear / MyProgressBar.Width);
            percentage = (int)percentage;
            TopText.Text = "今年你已经走过了" + DateTime.Now.DayOfYear.ToString() + "天啦！";
            MyProgressBar.Value = 100 * (DateTime.Now.DayOfYear / MyProgressBar.Width);

            List<DataTemple> datalist = conn.Query<DataTemple>("select * from DataTemple");

            if (datalist.Count() == 0)
                NewTB.Visibility = Visibility.Visible;
            else
                NewTB.Visibility = Visibility.Collapsed;

            foreach (var item in datalist)
            {
                ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = item.CalculatedDate, Str3 = item.Date });
            }

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

        

        private void BindKeyList()
        {
            foreach (string key in _appSettings.Values.Keys)
            {
                MyGridView.Items.Add(key);
            }
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
            
            Frame.Navigate(typeof(Add));
        }

        private void MyGirdView_ItemClick(object sender, ItemClickEventArgs e)
        {
            
            var _item = (CustomData)e.ClickedItem;
            //SelectedItem = _item;
            str1 = _item.Str1;
            str2 = _item.Str2;
            str3 = _item.Str3;
            str4 = _item.Str3;

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
                MyFlyout.IsEnabled = true;
            else
                MyFlyout.IsEnabled = false;
            //string str = ((FrameworkElement)e.OriginalSource).DataContext.ToString();
            //Copy.Text = str;
            //menuFlyout.ShowAt(lvVerses, e.GetPosition(this.lvVerses));

            //MenuFlyout myFlyout = new MenuFlyout();
            //MenuFlyoutItem firstItem = new MenuFlyoutItem { Text = "删除日程" };
            //myFlyout.Items.Add(firstItem);
            ////if you only want to show in left or buttom 
            ////myFlyout.Placement = FlyoutPlacementMode.Left;
            //FrameworkElement senderElement = sender as FrameworkElement;
            ////the code can show the flyout in your mouse click 
            //myFlyout.ShowAt(sender as UIElement, e.GetPosition(sender as UIElement));
        }

        private void MyGridView_Loaded(object sender, RoutedEventArgs e)
        {
            //CustomData item = new CustomData(); // Get persisted item
            //if (item != null)
            //{
            //   MyGridView.ScrollIntoView(item);
            //    ConnectedAnimation animation =
            //        ConnectedAnimationService.GetForCurrentView().GetAnimation("portrait");
            //    if (animation != null)
            //    {
            //        await MyGridView.TryStartConnectedAnimationAsync(
            //            animation, item, "GridViewStackPanel");
            //    }
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AbbFlyout.Hide();
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            int _start = ViewModel.CustomDatas.Count();
            ViewModel.CustomDatas.Remove(SelectedItem);
            conn.Execute("delete from DataTemple where Schedule_name = ?", SelectedItem.Str1);
            if (MyGridView.SelectedIndex > -1)
            {
                _appSettings.Values.Remove(MyGridView.SelectedItem.ToString());
                MyGridView.Items.Clear();
                BindKeyList();
            }
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
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var selectedItems = MyListView.Items.Cast<ListViewItem>()
            //    .Where(p => p.IsSelected)
            //    .Select(t => t.Content.ToString())
            //    .ToArray();
            var wid = MyGridView.Width;
            TopText.Text = wid.ToString();
        }

        private void MyListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(Add));
        }
    }
}
