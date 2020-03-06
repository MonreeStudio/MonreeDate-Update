using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using 倒计时.Models;
using 夏日.Models;
using BackgroundTasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Popups;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 倒计时
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Desktop : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public double MinMyNav = MainPage.Current.MyNav.CompactModeThresholdWidth;
        public DesktopEventsViewModel DesViewModel = new DesktopEventsViewModel();
        public DesktopEventsViewModel DesViewModel2 = new DesktopEventsViewModel();
        string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "mydb.sqlite");    //建立数据库  
        public SQLite.Net.SQLiteConnection conn;
        public static Desktop Current;
        List<string> list;
        public Desktop()
        {
            this.InitializeComponent();
            Current = this;
            // 建立数据库连接
            conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), path);
            //建表              
            conn.CreateTable<DataTemple>(); //默认表名同范型参数 
            list = new List<string>();
            MainPage.Current.MyNav.IsBackEnabled = true;
            MainPage.Current.SelectedPageItem = "Calculator";
            SetThemeColor();
            InitialData();
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
        
        private void InitialData()
        {
            int num = 0;
            List<DataTemple> datalist0 = conn.Query<DataTemple>("select * from DataTemple");
            foreach(var item in datalist0)
            {
                var pinToDesktopFlag = "DesktopFlag" + item.Schedule_name;
                if (localSettings.Values[pinToDesktopFlag] != null && localSettings.Values[pinToDesktopFlag].ToString() == "1")
                {
                    list.Add(item.Schedule_name);
                    DesViewModel2.DesktopDatas.Add(new DesktopEvents() { EventName = item.Schedule_name });
                    num++;
                    SelectedCountTextBlock.Text = "Tip：最多选取三个日程  " + num.ToString() + "/3";
                }
                else
                {
                    DesViewModel.DesktopDatas.Add(new DesktopEvents() { EventName = item.Schedule_name });
                }
            }
        }

        private void DesktopList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var count = DesktopList.SelectedItems.Count;
            //SelectedCountTextBlock.Text = "Tip：最多选取三个日程  " + count.ToString() + "/3";
            //if (count > 3)
            //{
            //    DesktopList.SelectedItems[3] = null;
            //    count--;
            //}
        }

        private void TestButton2_Click(object sender, RoutedEventArgs e)
        {
            List<DataTemple> datalist0 = conn.Query<DataTemple>("select * from DataTemple");
            foreach (var _item in datalist0)
            {
                var pinToDesktopFlag = "DesktopFlag" + _item.Schedule_name;
                if (localSettings.Values[pinToDesktopFlag] != null && localSettings.Values[pinToDesktopFlag].ToString() == "1")
                {
                    localSettings.Values[pinToDesktopFlag] = "0";
                }
            }
            DesViewModel.DesktopDatas.Clear();
            DesViewModel2.DesktopDatas.Clear();
            SelectedCountTextBlock.Text = "Tip：最多选取三个日程  " + DesktopList2.Items.Count + "/3";
            InitialData();
        }

        private async void TestButton1_Click(object sender, RoutedEventArgs e)
        {
            list.Clear();
            List<DataTemple> datalist0 = conn.Query<DataTemple>("select * from DataTemple");
            foreach (var _item in datalist0)
            {
                var pinToDesktopFlag = "DesktopFlag" + _item.Schedule_name;
                if (localSettings.Values[pinToDesktopFlag] != null && localSettings.Values[pinToDesktopFlag].ToString() == "1")
                    list.Add(_item.Schedule_name);
            }
            localSettings.Values["ItemCount"] = list.Count;
            if (list.Count > 0)
            {
                for(int i = 0; i < list.Count; i++)
                {
                    var desktopItemKey = "DesktopKey" + i;
                    var pinToDesktopFlag = "DesktopFlag" + list[i];
                    localSettings.Values[desktopItemKey] = list[i];
                    localSettings.Values[pinToDesktopFlag] = "1";
                }
                (new BlogFeedBackgroundTask()).CreateTool();
            }
            else
            {
                MessageDialog messageDialog = new MessageDialog("您尚未选择任何日程。", "温馨提示");
                await messageDialog.ShowAsync();
            }
            //DesViewModel.DesktopDatas.Clear();
            //DesViewModel2.DesktopDatas.Clear();
            //InitialData();
        }

        private void DesktopList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (DesktopEvents)e.ClickedItem;
            if (DesktopList2.Items.Count() < 3)
            {
                //list.Add(item.EventName);
                var pinToDesktopFlag = "DesktopFlag" + item.EventName;
                localSettings.Values[pinToDesktopFlag] = "1";
                DesViewModel2.DesktopDatas.Add(new DesktopEvents() { EventName = item.EventName });
                DesViewModel.DesktopDatas.Remove(item);
                SelectedCountTextBlock.Text = "Tip：最多选取三个日程  " + DesktopList2.Items.Count + "/3";
            }
            //var count = DesktopList.SelectedItems.Count;
            //var item = (DesktopEvents)e.ClickedItem;
            //if (list.Contains(item.EventName))
            //    list.Remove(item.EventName);
            //else
            //{
            //    if (list.Count < 3)
            //    {
            //        list.Add(item.EventName);
            //        list.Distinct();
            //    }
            //}
            //SelectedEvents.Text = "已选中：";
            //foreach (var name in list)
            //    SelectedEvents.Text += (name + " ");
        }

        private void DesktopList2_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (DesktopEvents)e.ClickedItem;
            //list.Remove(item.EventName);
            var pinToDesktopFlag = "DesktopFlag" + item.EventName;
            localSettings.Values[pinToDesktopFlag] = "0";
            DesViewModel.DesktopDatas.Add(new DesktopEvents() { EventName = item.EventName });
            DesViewModel2.DesktopDatas.Remove(item);
            SelectedCountTextBlock.Text = "Tip：最多选取三个日程  " + DesktopList2.Items.Count + "/3";
        }
    }
}
