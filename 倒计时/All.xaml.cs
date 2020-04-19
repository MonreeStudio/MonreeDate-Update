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
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications; // Notifications library
using Windows.Data.Xml.Dom;
using Windows.UI.StartScreen;
using Windows.UI.Core;
using 倒计时.Models;
using Windows.UI.Xaml.Media.Imaging;
using BackgroundTasks;

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
        private double MinMyNav = MainPage.Current.MyNav.CompactModeThresholdWidth;
        public static All Current;
        public string Model_event;
        public string Model_Date;
        //public CustomData SelectedItem;
        public String dbname;
        public int _index;
        private double percentage;
        private bool TopTap;
        DispatcherTimer timer;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public All()
        {
            this.InitializeComponent();           
            //建立数据库连接   
            conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), path);
            //建表              
            conn.CreateTable<DataTemple>(); //默认表名同范型参数    
            Current = this;
            LoadAllPage();
            MainPage.Current.MyNav.IsBackEnabled = false;
            MainPage.Current.SelectedPageItem = "All";
            NavigationCacheMode = NavigationCacheMode.Enabled;
            localSettings.Values["newMainId"] = "0";
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;//每秒触发这个事件，以刷新指针
            timer.Start();
            localSettings.Values["mainViewId"] = ApplicationView.GetForCurrentView().Id;
            localSettings.Values["newMain"] = "0";
        }

        private void Timer_Tick(object sender, object e)
        {
            //SetToptext();
            
            var timeNow = DateTime.Now;
            if (Convert.ToInt32(timeNow.Hour) == 0
                && Convert.ToInt32(timeNow.Minute) == 0
                && Convert.ToInt32(timeNow.Second) == 0)
            {
                LoadAllPage();
            }
        }

        public void LoadAllPage()
        {
            SetToptext();
            SetTitle();
            LoadSettings();
            LoadDateData();
            LoadTile();
            SetThemeColor();
            SetPersonPicture();
        }

        private async void SetPersonPicture()
        {
            try
            {
                List<PersonPictures> datalist = conn.Query<PersonPictures>("select * from PersonPictures where pictureName = ?", "picture");
                foreach (var item in datalist)
                {
                    try
                    {
                        MemoryStream stream = new MemoryStream(item.picture);
                        BitmapImage bitmap = new BitmapImage();
                        await bitmap.SetSourceAsync(stream.AsRandomAccessStream());
                        AllPicture.ProfilePicture = bitmap;
                    }
                    catch (ArgumentNullException ex)
                    {
                        throw ex;
                    }
                }
            }
            catch { }
        }

        public void SetThemeColor()
        {
            if (localSettings.Values["ThemeColor"] == null)
                localSettings.Values["ThemeColor"] = "CornflowerBlue";
            switch (localSettings.Values["ThemeColor"].ToString())
            {
                case "CornflowerBlue":
                    TC.Color = Colors.CornflowerBlue;
                    MyProgressBar.Background = new SolidColorBrush(Colors.CornflowerBlue);
                    MyProgressBar.Foreground = new SolidColorBrush(Colors.SkyBlue);
                    break;
                case "DeepSkyBlue":
                    TC.Color = Color.FromArgb(255, 2, 136, 235);
                    MyProgressBar.Background = new SolidColorBrush(Colors.CornflowerBlue);
                    MyProgressBar.Foreground = new SolidColorBrush(Colors.SkyBlue);
                    break;
                case "Orange":
                    TC.Color = Color.FromArgb(255, 229, 103, 44);
                    MyProgressBar.Background = new SolidColorBrush(Colors.Gold);
                    MyProgressBar.Foreground = new SolidColorBrush(Color.FromArgb(255, 229, 103, 44));
                    break;
                case "Crimson":
                    TC.Color = Colors.Crimson;
                    MyProgressBar.Background = new SolidColorBrush(Colors.DarkRed);
                    MyProgressBar.Foreground = new SolidColorBrush(Colors.Crimson);
                    break;
                case "Gray":
                    TC.Color = Color.FromArgb(255, 73, 92, 105);
                    MyProgressBar.Background = new SolidColorBrush(Colors.Black);
                    MyProgressBar.Foreground = new SolidColorBrush(Colors.Gray);
                    break;
                case "Purple":
                    TC.Color = Color.FromArgb(255, 119, 25, 171);
                    MyProgressBar.Background = new SolidColorBrush(Colors.MediumPurple);
                    MyProgressBar.Foreground = new SolidColorBrush(Color.FromArgb(255, 119, 25, 171));
                    break;
                case "Pink":
                    TC.Color = Color.FromArgb(255, 239, 130, 160);
                    MyProgressBar.Background = new SolidColorBrush(Color.FromArgb(255, 239, 130, 160));
                    MyProgressBar.Foreground = new SolidColorBrush(Colors.DeepPink);
                    break;
                case "Green":
                    TC.Color = Color.FromArgb(255, 124, 178, 56);
                    MyProgressBar.Background = new SolidColorBrush(Color.FromArgb(255, 124, 178, 56));
                    MyProgressBar.Foreground = new SolidColorBrush(Colors.ForestGreen);
                    break;
                case "DeepGreen":
                    TC.Color = Color.FromArgb(255, 8, 128, 126);
                    MyProgressBar.Background = new SolidColorBrush(Color.FromArgb(255, 124, 178, 56));
                    MyProgressBar.Foreground = new SolidColorBrush(Color.FromArgb(255, 8, 128, 126));
                    break;
                case "Coffee":
                    TC.Color = Color.FromArgb(255, 183, 133, 108);
                    MyProgressBar.Background = new SolidColorBrush(Color.FromArgb(255, 183, 133, 108));
                    MyProgressBar.Foreground = new SolidColorBrush(Colors.Brown);
                    break;
                default:
                    break;
            }
        }

        private void SetToptext()
        {
            if (localSettings.Values["TopTap"] == null)
                localSettings.Values["TopTap"] = "1";
            Today.Text = DateTime.Now.ToString("yyyy/MM/dd");
            if (localSettings.Values["TopTap"].ToString() == "0")
            {
                percentage = 100 * (DateTime.Now.DayOfYear / MyProgressBar.Width);
                percentage = (int)percentage;
                TopText.Text = "今年你已经走过了" + percentage.ToString() + "%啦！";
            }
            else
            {
                TopText.Text = "今年你已经走过了" + DateTime.Now.DayOfYear.ToString() + "天啦！";
            }
            MyProgressBar.Value = 100 * (DateTime.Now.DayOfYear / MyProgressBar.Width);
        }

        private void SetTitle()
        {
            var applicationView = CoreApplication.GetCurrentView();
            applicationView.TitleBar.ExtendViewIntoTitleBar = true;
            var title = ApplicationView.GetForCurrentView().TitleBar;
            title.BackgroundColor = Colors.SkyBlue;
            title.ForegroundColor = Colors.Transparent;
            title.ButtonBackgroundColor = title.ButtonInactiveBackgroundColor = Colors.Transparent;
            title.ButtonHoverBackgroundColor = Colors.White;
            title.ButtonPressedBackgroundColor = Colors.White;
            title.ButtonForegroundColor = title.ButtonHoverForegroundColor;
        }

        public async void LoadTile()
        {
            List<DataTemple> datalist = conn.Query<DataTemple>("select * from DataTemple where Date >= ? order by Date asc limit 1", DateTime.Now.ToString("yyyy-MM-dd"));
            string _ScheduleName = "";
            string _CaculatedDate = "";
            string _Date = "";

            foreach (var item in datalist)
            {
                _ScheduleName = item.Schedule_name;
                _CaculatedDate = CustomData.Calculator(item.Date);
                _Date = item.Date;
            }
            if (_ScheduleName != "" && _CaculatedDate != "" && _Date != "")
            {
                CreateTile(_ScheduleName, _CaculatedDate, _Date);
            }

            bool isPinned = SecondaryTile.Exists(_ScheduleName);
            if(isPinned)            
            {
                SecondaryTile toBeDeleted = new SecondaryTile(_ScheduleName);
                await toBeDeleted.RequestDeleteAsync();
            }
        }

        private void CreateTile(string _ScheduleName, string _CaculatedDate, string _Date)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            // 测试磁贴
            string from = _ScheduleName;
            string subject = _CaculatedDate;
            string body = _Date;
            string displayName;
            if (localSettings.Values["TileTip"] != null && localSettings.Values["TileTip"].ToString() == "1")
            {
                if (localSettings.Values[_ScheduleName + _Date] == null)
                    displayName = "无备注";
                else
                    displayName = localSettings.Values[_ScheduleName + _Date].ToString();
            }
            else
                displayName = "夏日";
                TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    
                    DisplayName = displayName,
                    TileMedium = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            TextStacking = TileTextStacking.Center,
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = from,
                                    HintStyle = AdaptiveTextStyle.Base,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = subject,
                                    HintStyle = AdaptiveTextStyle.Body,
                                    HintAlign = AdaptiveTextAlign.Center
                                }
                            }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = from,
                                        HintStyle = AdaptiveTextStyle.Base
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = subject,
                                        HintStyle = AdaptiveTextStyle.Subtitle
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = body
                                    }
                                },
                                HintTextStacking = AdaptiveSubgroupTextStacking.Center
                            }
                        }
                    }
                }
                        }
                    },

                    TileLarge = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = from,
                                        HintStyle = AdaptiveTextStyle.Subtitle
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = subject,
                                        HintStyle = AdaptiveTextStyle.Title
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = body,
                                        HintStyle = AdaptiveTextStyle.BodySubtle
                                    }
                                }
                            }
                        }
                    },
                    //new AdaptiveText()
                    //{
                    //    Text = ""
                    //},
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = "不要因为繁忙而忘记\n生活",
                                        HintStyle = AdaptiveTextStyle.Subtitle
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = "脚踏实地，仰望星空。\n永远相信美好的事情即将发生！",
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    }
                                    //new AdaptiveText()
                                    //{
                                    //    Text = "永远相信美好的事情即将发生！",
                                    //    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    //}
                                }
                            }
                        }
                    }
                }
                        }
                    }
                }
            };
            var notification = new TileNotification(content.GetXml());
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
            localSettings.Values["mainTile"] = _ScheduleName;
            //var badge = new BadgeNotification(content.GetXml());
            //BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);
        }

        public void LoadDateData()
        {
            AllProgressRing.IsActive = true;
            AllScrollViewer.ChangeView(null, 0, null);
            if (localSettings.Values["DisplayMode"] == null)
                localSettings.Values["DisplayMode"] = "All";
            var dm = localSettings.Values["DisplayMode"].ToString();
            switch (dm)
            {
                case "All":
                    LoadAll();
                    break;
                case "Past":
                    LoadPast();
                    break;
                case "Future":
                    LoadFuture();
                    break;
                default:
                    break;
            }
            AllProgressRing.IsActive = false;
        }

        private void LoadAll()
        {
            ContentChoise.Label = "显示全部";
            if (localSettings.Values["DateMode"] == null)
                localSettings.Values["DateMode"] = "Day";
            ViewModel.CustomDatas.Clear();
            List<DataTemple> datalist0 = conn.Query<DataTemple>("select * from DataTemple where IsTop = ? order by AddTime desc", "1");
            List<DataTemple> datalist1 = conn.Query<DataTemple>("select * from DataTemple where Date >= ? order by Date asc", DateTime.Now.ToString("yyyy-MM-dd"));
            List<DataTemple> datalist2 = conn.Query<DataTemple>("select * from DataTemple where Date < ? order by Date desc", DateTime.Now.ToString("yyyy-MM-dd"));
            if ((datalist1.Count() + datalist2.Count()) == 0)
            {
                NewTB.Text = "创建你的第一个日程吧！";
                NewTB2.Text = "（创建后可右键删除）";
                NewTB.Visibility = Visibility.Visible;
                NewTB2.Visibility = Visibility.Visible;
            }
            else
            {
                NewTB.Visibility = Visibility.Collapsed;
                NewTB2.Visibility = Visibility.Collapsed;
            }
            var DateMode = localSettings.Values["DateMode"].ToString();
            switch (DateMode)
            {
                case "Day":
                    ChangeABB.Label = "天数模式";
                    foreach (var item in datalist0)
                    {
                        if (item != null)
                            ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = CustomData.Calculator(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                    }
                    if (datalist0.Count() == 0)
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
                            if (item.IsTop == "0")
                                ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = CustomData.Calculator(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                        foreach (var item in datalist2)
                        {
                            if (item.IsTop == "0")
                                ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = CustomData.Calculator(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                    }
                    break;
                case "Week":
                    ChangeABB.Label = "周数模式";
                    foreach (var item in datalist0)
                    {
                        if (item != null)
                            ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToWeek(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                    }
                    if (datalist0.Count() == 0)
                    {
                        foreach (var item in datalist1)
                        {
                            ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToWeek(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                        foreach (var item in datalist2)
                        {
                            ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToWeek(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                    }
                    else
                    {
                        foreach (var item in datalist1)
                        {
                            if (item.IsTop == "0")
                                ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToWeek(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                        foreach (var item in datalist2)
                        {
                            if (item.IsTop == "0")
                                ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToWeek(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                    }
                    break;
                case "Year":
                    ChangeABB.Label = "年数模式";
                    foreach (var item in datalist0)
                    {

                        if (item != null)
                            ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToYMD(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                    }
                    if (datalist0.Count() == 0)
                    {
                        foreach (var item in datalist1)
                        {
                            if (item.CalculatedDate == "就在今天")
                                ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = CustomData.Calculator(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                            else
                                ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToYMD(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                        foreach (var item in datalist2)
                        {
                            ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToYMD(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                    }
                    else
                    {
                        foreach (var item in datalist1)
                        {
                            if (item.IsTop == "0")
                                ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToYMD(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                        foreach (var item in datalist2)
                        {
                            if (item.IsTop == "0")
                                ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToYMD(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                    }
                    break;
            }
        }
        
        private void LoadPast()
        {
            ContentChoise.Label = "显示已过日程";
            if (localSettings.Values["DateMode"] == null)
                localSettings.Values["DateMode"] = "Day";
            ViewModel.CustomDatas.Clear();
            List<DataTemple> datalist1 = conn.Query<DataTemple>("select * from DataTemple where IsTop = ? and Date < ? order by AddTime desc", "1", DateTime.Now.ToString("yyyy-MM-dd"));
            List<DataTemple> datalist2 = conn.Query<DataTemple>("select * from DataTemple where Date < ? order by Date desc", DateTime.Now.ToString("yyyy-MM-dd"));
            if (datalist2.Count() == 0)
            {
                NewTB.Text = "这里空空如也~";
                NewTB2.Text = "（暂无已过日程）";
                NewTB.Visibility = Visibility.Visible;
                NewTB2.Visibility = Visibility.Visible;
            }
            else
            {
                NewTB.Visibility = Visibility.Collapsed;
                NewTB2.Visibility = Visibility.Collapsed;
            }
            var DateMode = localSettings.Values["DateMode"].ToString();
            switch (DateMode)
            {
                case "Day":
                    ChangeABB.Label = "天数模式";
                    foreach (var item in datalist1)
                    {
                        if (item != null)
                            ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = CustomData.Calculator(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                    }
                    if (datalist1.Count() == 0)
                    {
                        foreach (var item in datalist2)
                        {
                            ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = CustomData.Calculator(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                    }
                    else
                    {
                        foreach (var item in datalist2)
                        {
                            if (item.IsTop == "0")
                                ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = CustomData.Calculator(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                    }
                    break;
                case "Week":
                    ChangeABB.Label = "周数模式";
                    foreach (var item in datalist1)
                    {
                        if (item != null)
                            ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToWeek(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                    }
                    if (datalist1.Count() == 0)
                    {
                        foreach (var item in datalist2)
                        {
                            ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToWeek(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                    }
                    else
                    {
                        foreach (var item in datalist2)
                        {
                            if (item.IsTop == "0")
                                ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToWeek(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                    }
                    break;
                case "Year":
                    ChangeABB.Label = "年数模式";
                    foreach (var item in datalist1)
                    {
                        if (item != null)
                            ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToYMD(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                    }
                    if (datalist1.Count() == 0)
                    {
                        foreach (var item in datalist2)
                        {
                            ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToYMD(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                    }
                    else
                    {
                        foreach (var item in datalist2)
                        {
                            if (item.IsTop == "0")
                                ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToYMD(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                    }
                    break;
            }
        }

        private void LoadFuture()
        {
            ContentChoise.Label = "显示未过日程";
            if (localSettings.Values["DateMode"] == null)
                localSettings.Values["DateMode"] = "Day";
            ViewModel.CustomDatas.Clear();
            List<DataTemple> datalist1 = conn.Query<DataTemple>("select * from DataTemple where IsTop = ? and Date >= ? order by AddTime desc", "1", DateTime.Now.ToString("yyyy-MM-dd"));
            List<DataTemple> datalist2 = conn.Query<DataTemple>("select * from DataTemple where Date >= ? order by Date asc", DateTime.Now.ToString("yyyy-MM-dd"));
            if (datalist2.Count() == 0)
            {
                NewTB.Text = "这里空空如也~";
                NewTB2.Text = "（暂无未过日程）";
                NewTB.Visibility = Visibility.Visible;
                NewTB2.Visibility = Visibility.Visible;
            }
            else
            {
                NewTB.Visibility = Visibility.Collapsed;
                NewTB2.Visibility = Visibility.Collapsed;
            }
            var DateMode = localSettings.Values["DateMode"].ToString();
            switch (DateMode)
            {
                case "Day":
                    ChangeABB.Label = "天数模式";
                    foreach (var item in datalist1)
                    {
                        if (item != null)
                            ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = CustomData.Calculator(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                    }
                    if (datalist1.Count() == 0)
                    {
                        foreach (var item in datalist2)
                        {
                            ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = CustomData.Calculator(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                    }
                    else
                    {
                        foreach (var item in datalist2)
                        {
                            if (item.IsTop == "0")
                                ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = CustomData.Calculator(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                    }
                    break;
                case "Week":
                    ChangeABB.Label = "周数模式";
                    foreach (var item in datalist1)
                    {
                        if (item != null)
                            ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToWeek(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                    }
                    if (datalist1.Count() == 0)
                    {
                        foreach (var item in datalist2)
                        {
                            ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToWeek(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                    }
                    else
                    {
                        foreach (var item in datalist2)
                        {
                            if (item.IsTop == "0")
                                ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToWeek(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                    }
                    break;
                case "Year":
                    ChangeABB.Label = "年数模式";
                    foreach (var item in datalist1)
                    {
                        if (item != null)
                            ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToYMD(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                    }
                    if (datalist1.Count() == 0)
                    {
                        foreach (var item in datalist2)
                        {
                            if (item.CalculatedDate == "就在今天")
                                ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = CustomData.Calculator(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                            else
                                ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToYMD(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                    }
                    else
                    {
                        foreach (var item in datalist2)
                        {
                            if (item.IsTop == "0")
                                ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = ConvertToYMD(item.Date), Str3 = item.Date, Str4 = ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                        }
                    }
                    break;
            }
        }
        public void LoadSettings()
        {
            UserName.Visibility = Visibility.Collapsed;
            UserSign.Visibility = Visibility.Collapsed;
            if(localSettings.Values["NickName"]!=null&&localSettings.Values["Sign"]!=null)
            {
                UserName.Text = localSettings.Values["NickName"].ToString();
                UserSign.Text = localSettings.Values["Sign"].ToString();
            }
            else
            {
                UserName.Text = "游客";
                UserSign.Text = "这个人好懒，什么也没写……";
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
            if (localSettings.Values["SetAllPersonPicture"] != null)
            {
                if (localSettings.Values["SetAllPersonPicture"].Equals(true))
                {
                    AllPicture.Visibility = Visibility.Visible;
                    AllCommandBar.Margin = new Thickness(0, 50, 10, 2);
                    MarginText.Height = 30;
                    TopText.Margin = new Thickness(0);
                    MyProgressBar.Margin = new Thickness(0, 0, 0, 10);
                }
                else
                {
                    AllPicture.Visibility = Visibility.Collapsed;
                    All.Current.AllCommandBar.Margin = new Thickness(0, 50, 10, 25);
                    MarginText.Height = 50;
                    TopText.Margin = new Thickness(0, 25, 0, 0);
                    MyProgressBar.Margin = new Thickness(0);
                }
            }
            else
                AllPicture.Visibility = Visibility.Visible;
            List<DataTemple> datalist = conn.Query<DataTemple>("select * from DataTemple");
            foreach(var item in datalist)
            {
                if (SecondaryTile.Exists(item.Schedule_name))
                {
                    CreateSecondaryTile(item.Schedule_name, CustomData.Calculator(item.Date), item.Date);
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

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //((NavigationViewItem)MainPage.Current.MyNav.MenuItems[2]).IsSelected = true;
            MainPage.Current.MyNav.SelectedItem = MainPage.Current.MyNav.MenuItems[2];
            Frame.Navigate(typeof(Add));
        }

        private void MyGirdView_ItemClick(object sender, ItemClickEventArgs e)
        {   
            var _item = (CustomData)e.ClickedItem;
            AllItem = _item;
            str1 = _item.Str1;
            str2 = _item.Str2;
            str3 = _item.Str3;
            str4 = _item.Str4;
            BgsColor = _item.BackGroundColor;
            MainPage.Current.SelectedPage = true;
            Frame.Navigate(typeof(Details), null, new DrillInNavigationTransitionInfo());
        }

        private void MyGirdView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string key = e.AddedItems[0].ToString();
                
            }
        }

        private string ConvertToYMD(String _date)
        {
            string CDate = "";
            DateTime d1 = Convert.ToDateTime(_date);
            DateTime d2 = DateTime.Now;
            DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
            DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));
            if (d4 > d3)
                CDate = "已过" + Term(d3, d4);
            else
            {
                if (d4 < d3)
                    CDate = "还有" + Term(d4, d3);
                else
                    CDate = "就在今天";
            }
            return CDate;
        }

        public string ConvertToWeek(string _date)
        {
            string CDate = "";
            DateTime d1 = Convert.ToDateTime(_date);
            DateTime d2 = DateTime.Now;
            DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
            DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));
            if (d4 > d3)
            {
                int days = (d4 - d3).Days;
                int week, week_date;
                week = days / 7;
                week_date = days % 7;
                if (week != 0)
                {
                    if (week_date == 0)
                        CDate = "已过" + week.ToString() + "周";
                    else
                        CDate = "已过" + week.ToString() + "周" + week_date.ToString() + "天";
                }
                else
                {
                    CDate = "已过" + week_date.ToString() + "天";
                }
            }
            else
            {
                if (d4 < d3)
                {
                    int days = (d3 - d4).Days;
                    int week, week_date;
                    week = days / 7;
                    week_date = days % 7;
                    if (week != 0)
                    {
                        if (week_date == 0)
                            CDate = "还有" + week.ToString() + "周";
                        else
                            CDate = "还有" + week.ToString() + "周" + week_date.ToString() + "天";
                    }
                    else
                    {
                        CDate = "还有" + week_date.ToString() + "天";
                    }
                }
                else
                    CDate = "就在今天";
            }
            return CDate;
        }

        private void MyGirdView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var _item = (e.OriginalSource as FrameworkElement)?.DataContext as CustomData;
            List<DataTemple> toplist = conn.Query<DataTemple>("select * from DataTemple where IsTop = ?", "1");
            AllItem = _item;
            if (AllItem != null)
            {
                if (localSettings.Values["mainTile"] == null)
                    localSettings.Values["mainTile"] = "";
                if (localSettings.Values["mainTile"].ToString().Equals(AllItem.Str1))
                {
                    PinToSC.Visibility = Visibility.Collapsed;
                    unPinToSC.Visibility = Visibility.Collapsed;
                    FS2.Visibility = Visibility.Visible;
                    MainTile.Visibility = Visibility.Visible;
                }
                else
                {
                    MainTile.Visibility = Visibility.Collapsed;
                    bool isPinned = SecondaryTile.Exists(AllItem.Str1);
                    if (!isPinned)
                    {
                        PinToSC.Visibility = Visibility.Visible;
                        FS2.Visibility = Visibility.Visible;
                        unPinToSC.Visibility = Visibility.Collapsed;
                        localSettings.Values[AllItem.Str1] = "unpined";
                    }
                    else
                    {
                        if (isPinned && localSettings.Values["mainTile"].ToString() != AllItem.Str1)
                        {
                            PinToSC.Visibility = Visibility.Collapsed;
                            unPinToSC.Visibility = Visibility.Visible;
                            FS2.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            PinToSC.Visibility = Visibility.Visible;
                            unPinToSC.Visibility = Visibility.Collapsed;
                            FS2.Visibility = Visibility.Visible;
                        }
                    }
                }
                MyFlyout.IsEnabled = true;
                if (toplist.Count() == 0)
                {
                    FS.Visibility = Visibility.Visible;
                    SetTop.Visibility = Visibility.Visible;
                    SetTop.IsEnabled = true;
                    Cancel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    bool in_toplist = false;
                    foreach(var item in toplist)
                    {
                        if (item.Schedule_name == AllItem.Str1)
                            in_toplist = true;
                    }
                    if (in_toplist==true)
                    {
                        FS.Visibility = Visibility.Visible;
                        SetTop.Visibility = Visibility.Collapsed;
                        Cancel.Visibility = Visibility.Visible;
                        Cancel.IsEnabled = true;
                    }
                    else
                    {
                        SetTop.Visibility = Visibility.Visible;
                        Cancel.Visibility = Visibility.Collapsed;
                        FS.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                MyFlyout.IsEnabled = false;
                MainTile.Visibility = Visibility.Collapsed;
                FS.Visibility = Visibility.Collapsed;
                FS2.Visibility = Visibility.Collapsed;
                PinToSC.Visibility = Visibility.Collapsed;
                unPinToSC.Visibility = Visibility.Collapsed;
                SetTop.Visibility = Visibility.Collapsed;
                Cancel.Visibility = Visibility.Collapsed;
            }

        }

        private void MyGridView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //AbbFlyout.Hide();
        }

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await DeleteDialog.ShowAsync();
        }

        private void TopText_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (localSettings.Values["TopTap"].ToString() == "1")
            {
                localSettings.Values["TopTap"] = "0";
                percentage = 100 * (DateTime.Now.DayOfYear / MyProgressBar.Width);
                percentage = (int)percentage;
                TopText.Text = "今年你已经走过了" + percentage.ToString() + "%啦！";
            }
            else
            {
                TopText.Text = "今年你已经走过了" + DateTime.Now.DayOfYear.ToString() + "天啦！";
                localSettings.Values["TopTap"] = "1";
            }
        }

        private void SetTop_Click(object sender, RoutedEventArgs e)
        {
            //            conn.Execute("update DataTemple set IsTop = ? where Schedule_name = ?", "1", AllItem.Str1);
            List<DataTemple> _datalist = conn.Query<DataTemple>("select * from DataTemple where Schedule_name = ?", AllItem.Str1);
            conn.Execute("delete from DataTemple where Schedule_name = ?", AllItem.Str1);
            foreach (var item in _datalist)
            {
                conn.Insert(new DataTemple() { Schedule_name = item.Schedule_name, CalculatedDate = item.CalculatedDate, Date = item.Date, BgColor = item.BgColor, TintOpacity = item.TintOpacity, IsTop = "1", AddTime = DateTime.Now.ToString() });
            }
            LoadDateData();
            LoadSettings();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //conn.Execute("update DataTemple set IsTop = ? where Schedule_name = ?", "0", AllItem.Str1);
            List<DataTemple> _datalist = conn.Query<DataTemple>("select * from DataTemple where Schedule_name = ?", AllItem.Str1);
            conn.Execute("delete from DataTemple where Schedule_name = ?", AllItem.Str1);
            foreach (var item in _datalist)
            {
                conn.Insert(new DataTemple() { Schedule_name = item.Schedule_name, CalculatedDate = item.CalculatedDate, Date = item.Date, BgColor = item.BgColor, TintOpacity = item.TintOpacity, IsTop = "0", AddTime = "" });
            }
            LoadDateData();
            LoadSettings();
        }

        private async void DeleteDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            bool isPinned = SecondaryTile.Exists(AllItem.Str1);
            if (isPinned)
            {
                SecondaryTile toBeDeleted = new SecondaryTile(AllItem.Str1);
                await toBeDeleted.RequestDeleteAsync();
            }
            int _start = ViewModel.CustomDatas.Count();
            ViewModel.CustomDatas.Remove(AllItem);
            conn.Execute("delete from DataTemple where Schedule_name = ?", AllItem.Str1);
            int _end = ViewModel.CustomDatas.Count();
            if (_start != _end)
            {
                PopupNotice popupNotice = new PopupNotice("删除成功");
                popupNotice.ShowAPopup();
            }
            var dm = localSettings.Values["DisplayMode"].ToString();
            switch (dm)
            {
                case "All":
                    {
                        List<DataTemple> datalist = conn.Query<DataTemple>("select * from DataTemple");
                        if (datalist.Count() == 0)
                        {
                            NewTB.Text = "创建你的第一个日程吧！";
                            NewTB2.Text = "（创建后可右键删除）";
                            NewTB.Visibility = Visibility.Visible;
                            NewTB2.Visibility = Visibility.Visible;
                        }
                    }
                    break;
                case "Past":
                    {
                        List<DataTemple> datalist = conn.Query<DataTemple>("select * from DataTemple where Date < ?", DateTime.Now.ToString("yyyy-MM-dd"));
                        if(datalist.Count==0)
                        {
                            NewTB.Text = "这里空空如也~";
                            NewTB2.Text = "（暂无已过日程）";
                            NewTB.Visibility = Visibility.Visible;
                            NewTB2.Visibility = Visibility.Visible;
                        }
                    }
                    break;
                case "Future":
                    {
                        List<DataTemple> datalist = conn.Query<DataTemple>("select * from DataTemple where Date >= ?", DateTime.Now.ToString("yyyy-MM-dd"));
                        if (datalist.Count == 0)
                        {
                            NewTB.Text = "这里空空如也~";
                            NewTB2.Text = "（暂无未过日程）";
                            NewTB.Visibility = Visibility.Visible;
                            NewTB2.Visibility = Visibility.Visible;
                        }
                    }
                    break;
            }
            
            LoadTile();
        }

        private async void PinToSC_Click(object sender, RoutedEventArgs e)
        {
            string tileId = AllItem.Str1;
            string displayName = "夏日：" + AllItem.Str1;
            string arguments = "action=viewCity&zipCode=" + AllItem.Str1;
            SecondaryTile tile = new SecondaryTile(
                tileId,
                displayName,
                arguments,
                new Uri("ms-appx:///Assets/Square150x150Logo.scale-150.png"),
                Windows.UI.StartScreen.TileSize.Default);
            tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.scale-400.png");
            tile.VisualElements.Square310x310Logo = new Uri("ms-appx:///Assets/LargeTIle.scale-400.png");
            tile.VisualElements.Square44x44Logo = new Uri("ms-appx:///Assets/Square44x44Logo.scale-400.png");
            bool isPinned = await tile.RequestCreateAsync();
            CreateSecondaryTile(AllItem.Str1, CustomData.Calculator(AllItem.Str3), AllItem.Str3);
            if (isPinned)
            {
                PinToSC.Visibility = Visibility.Collapsed;
                unPinToSC.Visibility = Visibility.Visible;
                localSettings.Values[AllItem.Str1] = "pined";
                PopupNotice popupNotice = new PopupNotice("日程固定成功");
                popupNotice.ShowAPopup();
            }
            LoadDateData();
            LoadSettings();
            LoadTile();
        }

        private async void unPinToSC_Click(object sender, RoutedEventArgs e)
        {
            SecondaryTile toBeDeleted = new SecondaryTile(AllItem.Str1);
            await toBeDeleted.RequestDeleteAsync();
            PinToSC.Visibility = Visibility.Visible;
            unPinToSC.Visibility = Visibility.Collapsed;
            localSettings.Values[AllItem.Str1] = "unpined";
            PopupNotice popupNotice = new PopupNotice("日程已取消固定");
            popupNotice.ShowAPopup();
            LoadDateData();
            LoadSettings();
            LoadTile();
        }

        private void GridViewStackPanel_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 0);
        }

        private void GridViewStackPanel_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
        }

        private void TopText_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 0);
        }

        private void TopText_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
        }

        private void AllPicture_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            UserName.Visibility = Visibility.Visible;
            UserSign.Visibility = Visibility.Visible;
        }

        private void AllPicture_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            UserName.Visibility = Visibility.Collapsed;
            UserSign.Visibility = Visibility.Collapsed;
        }

        private void AllPicture_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainPage.Current.MyNav.SelectedItem = MainPage.Current.MyNav.SettingsItem;
            Frame.Navigate(typeof(Settings));
        }

        private void DisplayAll_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["DisplayMode"] = "All";
            LoadDateData();
        }

        private void DisplatPast_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["DisplayMode"] = "Past";
            LoadDateData();
        }

        private void DisplayFuture_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["DisplayMode"] = "Future";
            LoadDateData();
        }

        private void RootThumb_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AllScrollViewer.ChangeView(null, 0, null);
        }

        private void AllScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            //var y = AllScrollViewer.VerticalOffset;
            //if (y == 0)
            //    RootThumb.Visibility = Visibility.Collapsed;
            //else
            //    RootThumb.Visibility = Visibility.Visible;
            //RootThumb.Margin = new Thickness(0, y, 20, 20);
        }

        private void AllScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var y = AllScrollViewer.VerticalOffset;
            if (y == 0)
            {
                //AllThumbShadow.Visibility = Visibility.Collapsed;
                RootThumb.Visibility = Visibility.Collapsed;
            }
            else
            {
                //AllThumbShadow.Visibility = Visibility.Visible;
                RootThumb.Visibility = Visibility.Visible;
            }
        }

        private void DayMode_Click(object sender, RoutedEventArgs e)
        {
            ChangeABB.Label = "天数模式";
            localSettings.Values["DateMode"] = "Day";
            LoadDateData();
        }

        private void WeekMode_Click(object sender, RoutedEventArgs e)
        {
            ChangeABB.Label = "周数模式";
            localSettings.Values["DateMode"] = "Week";
            LoadDateData();
        }

        private void YearMode_Click(object sender, RoutedEventArgs e)
        {
            ChangeABB.Label = "年数模式";
            localSettings.Values["DateMode"] = "Year";
            LoadDateData();
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

        public void CreateSecondaryTile(string _ScheduleName, string _CaculatedDate, string _Date)
        {

            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            // 测试磁贴
            string from = _ScheduleName;
            string subject = _CaculatedDate;
            string body = _Date;
            string displayName;
            if (localSettings.Values["TileTip"] != null && localSettings.Values["TileTip"].ToString() == "1")
            {
                if (localSettings.Values[_ScheduleName + _Date] == null)
                    displayName = "无备注";
                else
                    displayName = localSettings.Values[_ScheduleName + _Date].ToString();
            }
            else
                displayName = "夏日";
            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    DisplayName = displayName,
                    TileMedium = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            TextStacking = TileTextStacking.Center,
                            Children =
                    {
                    new AdaptiveText()
                    {
                        Text = from,
                        HintStyle = AdaptiveTextStyle.Base,
                        HintAlign = AdaptiveTextAlign.Center
                    },
                    new AdaptiveText()
                    {
                        Text = subject,
                        HintStyle = AdaptiveTextStyle.Body,
                        HintAlign = AdaptiveTextAlign.Center
                    }
                }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = from,
                                        HintStyle = AdaptiveTextStyle.Base
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = subject,
                                        HintStyle = AdaptiveTextStyle.Subtitle
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = body
                                    }
                                },
                                HintTextStacking = AdaptiveSubgroupTextStacking.Center
                            }
                        }
                    }
                }
                        }
                    },

                    TileLarge = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = from,
                                        HintStyle = AdaptiveTextStyle.Subtitle
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = subject,
                                        HintStyle = AdaptiveTextStyle.Title
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = body,
                                        HintStyle = AdaptiveTextStyle.BodySubtle
                                    }
                                }
                            }
                        }
                    },
                    //new AdaptiveText()
                    //{
                    //    Text = ""
                    //},
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = "不要因为繁忙而忘记\n生活",
                                        HintStyle = AdaptiveTextStyle.Subtitle
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = "脚踏实地，仰望星空。\n永远相信美好的事情即将发生！",
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    }
                                    //new AdaptiveText()
                                    //{
                                    //    Text = "永远相信美好的事情即将发生！",
                                    //    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    //}
                                }
                            }
                        }
                    }
                }
                        }
                    }
                }
            };
            var notification = new TileNotification(content.GetXml());
            if (SecondaryTile.Exists(_ScheduleName))
            {
                var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(_ScheduleName);

                updater.Update(notification);
            }
        }
    }
}

