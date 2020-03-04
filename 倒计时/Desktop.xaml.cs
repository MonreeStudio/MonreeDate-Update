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
            DesktopList.SelectRange(new ItemIndexRange(0,2));
            //ListBackgroundPanel.Background = new SolidColorBrush(Color.FromArgb(255, 241, 241, 241));
            List<DataTemple> datalist0 = conn.Query<DataTemple>("select * from DataTemple");
            foreach(var item in datalist0)
            {
                DesViewModel.DesktopDatas.Add(new DesktopEvents() { EventName = item.Schedule_name});
            }
            
        }

        private void DesktopList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var count = DesktopList.SelectedItems.Count;
            
            SelectedCountTextBlock.Text = "Tip：最多选取三个日程  " + count.ToString()+"/3";
            if (count > 3)
            {
                DesktopList.SelectedItems[3] = null;
                count--;
            }
        }

        private void TestButton2_Click(object sender, RoutedEventArgs e)
        {
            //(new BlogFeedBackgroundTask()).LoadToast();
        }

        private void TestButton1_Click(object sender, RoutedEventArgs e)
        {
            if (list.Count > 0)
            {
                localSettings.Values["ItemCount"] = list.Count;
                for(int i = 0; i < list.Count; i++)
                {
                    var desktopItemKey = "DesktopKey" + i;
                    localSettings.Values[desktopItemKey] = list[i];
                }
            }
            (new BlogFeedBackgroundTask()).CreateTool();
        }

        private void DesktopList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var count = DesktopList.SelectedItems.Count;
            var item = (DesktopEvents)e.ClickedItem;
            if (list.Contains(item.EventName))
                list.Remove(item.EventName);
            else
            {
                if (list.Count < 3)
                {
                    list.Add(item.EventName);
                    list.Distinct();
                }
            }
            SelectedEvents.Text = "已选中：";
            foreach (var name in list)
                SelectedEvents.Text += (name + " ");
        }
    }
}
