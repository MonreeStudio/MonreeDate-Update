using Microsoft.Graphics.Canvas.Effects;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using 夏日;
using 夏日.Models;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 倒计时
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DesktopTool : Page
    {
        static string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "mydb.sqlite");    //建立数据库  
        static SQLite.Net.SQLiteConnection conn;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        List<DataTemple> list;
        DispatcherTimer timer;
        public ToolDataViewModel ToolViewModel = new ToolDataViewModel();
        private int viewHeight;
        public static DesktopTool Current;

        public DesktopTool()
        {
            this.InitializeComponent();
            Current = this;
            list = new List<DataTemple>();
            //建立数据库连接   
            conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), path);
            //建表              
            conn.CreateTable<DataTemple>(); //默认表名同范型参数 
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;//每秒触发这个事件，以刷新指针
            timer.Start();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            var title = ApplicationView.GetForCurrentView().TitleBar;
            title.BackgroundColor = Colors.SkyBlue;
            title.ForegroundColor = Colors.Transparent;
            title.ButtonBackgroundColor = title.ButtonInactiveBackgroundColor = Colors.Transparent;
            title.ButtonHoverBackgroundColor = Colors.Gray;
            title.ButtonPressedBackgroundColor = Colors.Gray;
            title.ButtonForegroundColor = title.ButtonHoverForegroundColor;
            InitializeFrostedGlass(rootGrid, 0);
            localSettings.Values["FirstlyStart"] = "1";
            LoadData();
            if (localSettings.Values["Pip"] == null)
                localSettings.Values["Pip"] = "1";
            if (ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.Default 
                && 
                localSettings.Values["Pip"].ToString() == "1")
            {
                Pip();
            }
            //Pip();
            
        }

        private async void ToCompactOverlay()
        {
            var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            preferences.CustomSize = new Size(330, viewHeight);
            await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, preferences);
            ApplicationView.PreferredLaunchViewSize = new Size(3000, 3000);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        private void Timer_Tick(object sender, object e)
        {
            //if (localSettings.Values["Pip"].ToString() == "1"
            //    &&
            //    localSettings.Values["FirstlyStart"].ToString() == "1"
            //    &&
            //    ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.Default)
            //{
            //    localSettings.Values["FirstlyStart"] = "0";
            //    //Pip();
            //    ToCompactOverlay();
            //}
            RefreshData();
            if (ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.CompactOverlay)
            {
                SetTopBtn.Visibility = Visibility.Collapsed;
                DeSetTopBtn.Visibility = Visibility.Visible;
            }
            else
            {
                
                SetTopBtn.Visibility = Visibility.Visible;
                DeSetTopBtn.Visibility = Visibility.Collapsed;
            }
        }

        public void LoadData()
        {
            if (localSettings.Values["CornerName"] == null)
            {
                CornerNameTextBlock.Text = "夏日";
            }
            else
            {
                CornerNameTextBlock.Text = localSettings.Values["CornerName"].ToString();
            }
            if (ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.CompactOverlay)
            {
                SetTopBtn.Visibility = Visibility.Collapsed;
                DeSetTopBtn.Visibility = Visibility.Visible;
            }
            else
            {
                SetTopBtn.Visibility = Visibility.Visible;
                DeSetTopBtn.Visibility = Visibility.Collapsed;
            }
            int count = (int)localSettings.Values["ItemCount"];
            List<DataTemple> datalist = new List<DataTemple>();
            var allData = conn.Query<DataTemple>("select *from DataTemple");
            switch (count)
            {
                case 1:
                    viewHeight = 110;
                    ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(330, viewHeight));
                    string a1 = localSettings.Values["DesktopKey0"].ToString();
                    foreach (var item in allData)
                    {
                        if (item.Schedule_name == a1)
                            datalist.Add(item);
                    }
                    break;
                case 2:
                    viewHeight = 230;
                    ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(330, viewHeight));
                    string b1 = localSettings.Values["DesktopKey0"].ToString();
                    string b2 = localSettings.Values["DesktopKey1"].ToString();
                    foreach (var item in allData)
                    {
                        if (item.Schedule_name == b1 || item.Schedule_name == b2)
                            datalist.Add(item);
                    }
                    break;
                case 3:
                    viewHeight = 315;
                    ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(330, viewHeight));
                    string c1 = localSettings.Values["DesktopKey0"].ToString();
                    string c2 = localSettings.Values["DesktopKey1"].ToString();
                    string c3 = localSettings.Values["DesktopKey2"].ToString();
                    foreach (var item in allData)
                    {
                        if (item.Schedule_name == c1 || item.Schedule_name == c2 || item.Schedule_name == c3)
                            datalist.Add(item);
                    }
                    break;
                default:
                    viewHeight = 110;
                    ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(330, viewHeight));
                    break;
            }
            switch (count)
            {
                case 1:
                    List<DataTemple> a1 = conn.Query<DataTemple>("select * from DataTemple where Schedule_name = ?", datalist[0].Schedule_name);
                    foreach (var item in a1)
                    {
                        ToolViewModel.ToolDatas.Add(new ToolData { ScheduleName = item.Schedule_name, Date = item.Date, CalDate = Calculator(item.Date) });
                    }
                    break;
                case 2:
                    List<DataTemple> b1 = conn.Query<DataTemple>("select * from DataTemple where Schedule_name = ?", datalist[0].Schedule_name);
                    foreach (var item in b1)
                    {
                        ToolViewModel.ToolDatas.Add(new ToolData { ScheduleName = item.Schedule_name, Date = item.Date, CalDate = Calculator(item.Date) });
                    }
                    List<DataTemple> b2 = conn.Query<DataTemple>("select * from DataTemple where Schedule_name = ?", datalist[1].Schedule_name);
                    foreach (var item in b2)
                    {
                        ToolViewModel.ToolDatas.Add(new ToolData { ScheduleName = item.Schedule_name, Date = item.Date, CalDate = Calculator(item.Date) });
                    }
                    break;
                case 3:
                    List<DataTemple> c1 = conn.Query<DataTemple>("select * from DataTemple where Schedule_name = ?", datalist[0].Schedule_name);
                    foreach (var item in c1)
                    {
                        ToolViewModel.ToolDatas.Add(new ToolData { ScheduleName = item.Schedule_name, Date = item.Date, CalDate = Calculator(item.Date) });
                    }
                    List<DataTemple> c2 = conn.Query<DataTemple>("select * from DataTemple where Schedule_name = ?", datalist[1].Schedule_name);
                    foreach (var item in c2)
                    {
                        ToolViewModel.ToolDatas.Add(new ToolData { ScheduleName = item.Schedule_name, Date = item.Date, CalDate = Calculator(item.Date) });
                    }
                    List<DataTemple> c3 = conn.Query<DataTemple>("select * from DataTemple where Schedule_name = ?", datalist[2].Schedule_name);
                    foreach (var item in c3)
                    {
                        ToolViewModel.ToolDatas.Add(new ToolData { ScheduleName = item.Schedule_name, Date = item.Date, CalDate = Calculator(item.Date) });
                    }
                    break;
                default:
                    break;
            }
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

        private void InitializeFrostedGlass(UIElement glassHost, int selectionCode)
        {
            Color color;
            float amount1, amount2;
            if (localSettings.Values["Colorful"] != null 
                &&
                localSettings.Values["ToolColor"]!=null
                &&
                localSettings.Values["Colorful"].ToString() == "1")
            {
                string _color = localSettings.Values["ToolColor"].ToString();
                color = GetColor(_color);
                amount1 = 0.5f;
                amount2 = 0.5f;
            }
            else
            {
                color = Color.FromArgb(255, 245, 245, 245);
                amount1 = 0.7f;
                amount2 = 0.3f;
            }
            Visual hostVisual = ElementCompositionPreview.GetElementVisual(glassHost);
            Compositor compositor = hostVisual.Compositor;
            GaussianBlurEffect glassEffect;
            if (selectionCode == 0)
            {
                glassEffect = new GaussianBlurEffect
                {
                    BlurAmount = 10f,
                    BorderMode = EffectBorderMode.Hard,
                    Source = new ArithmeticCompositeEffect
                    {
                        MultiplyAmount = 0,
                        Source1Amount = amount1,
                        Source2Amount = amount2,
                        Source1 = new CompositionEffectSourceParameter("backdropBrush"),
                        Source2 = new ColorSourceEffect
                        {
                            Color = color
                            //Color = Color.FromArgb(255, 236, 155, 173)
                            //Color = Color.FromArgb(255, 119, 25, 171)
                            //Color = Colors.CornflowerBlue
                            //Color = Color.FromArgb(255, 73, 92, 105)
                            //Color = Color.FromArgb(255, 2, 136, 235)
                            //Color = Color.FromArgb(255, 82, 163, 242)
                            //Color = Color.FromArgb(255, 245, 245, 245)
                        }
                    }
                };
            }
            else
            {
                glassEffect = new GaussianBlurEffect
                {
                    BlurAmount = 15f,
                    BorderMode = EffectBorderMode.Hard,
                    Source = new ArithmeticCompositeEffect
                    {
                        MultiplyAmount = 0,
                        Source1Amount = amount1,
                        Source2Amount = amount2,
                        Source1 = new CompositionEffectSourceParameter("backdropBrush"),
                        Source2 = new ColorSourceEffect
                        {
                            Color = color
                            //Color = Color.FromArgb(255, 236, 155, 173)
                            //Color = Color.FromArgb(255, 119, 25, 171)
                            //Color = Colors.CornflowerBlue
                            //Color = Color.FromArgb(255, 73, 92, 105)
                            //Color = Color.FromArgb(255, 2, 136, 235)
                            //Color = Color.FromArgb(255, 245, 245, 245)
                        }
                    }
                };
            }
            var effectFactory = compositor.CreateEffectFactory(glassEffect);
            
            var backdropBrush = compositor.CreateHostBackdropBrush();
            var effectBrush = effectFactory.CreateBrush();
            effectBrush.SetSourceParameter("backdropBrush", backdropBrush);
            var glassVisual = compositor.CreateSpriteVisual();
            glassVisual.Brush = effectBrush;
            ElementCompositionPreview.SetElementChildVisual(glassHost, glassVisual);
            var bindSizeAnimation = compositor.CreateExpressionAnimation("hostVisual.Size");
            bindSizeAnimation.SetReferenceParameter("hostVisual", hostVisual);
            glassVisual.StartAnimation("Size", bindSizeAnimation);
        }

        public static List<T> MyFindListBoxChildOfType<T>(DependencyObject root) where T : class
        {
            var MyQueue = new Queue<DependencyObject>();
            MyQueue.Enqueue(root);
            List<T> list = new List<T>();
            while (MyQueue.Count > 0)
            {
                DependencyObject current = MyQueue.Dequeue();
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(current); i++)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var typedChild = child as T;
                    if (typedChild != null)
                    {
                        list.Add(typedChild);
                        //return typedChild;
                    }
                    MyQueue.Enqueue(child);
                }
            }
            return list;
        }

        public string Calculator(string s1)
        {
            string str1 = s1;
            string str2 = DateTime.Now.ToShortDateString().ToString();
            string s2;
            DateTime d1 = Convert.ToDateTime(str1);
            DateTime d2 = Convert.ToDateTime(str2);
            DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
            DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));
            int days = (d4 - d3).Days;
            if (days < 0)
            {
                days = -days;
                s2 = "还有" + days.ToString() + "天";
            }
            else
            {
                if (days != 0)
                    s2 = "已过" + days.ToString() + "天";
                else
                {
                    s2 = "就在今天";
                }
            }
            return s2;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e != null)
            {
                try
                {
                    list = (List<DataTemple>)e.Parameter;

                    //localSettings.Values["DesktopPin"] = false;
                }
                catch { }
            }
        }

        private void RefreshData()
        {
            var timeNow = DateTime.Now;
            if (Convert.ToInt32(timeNow.Hour) == 0
                && Convert.ToInt32(timeNow.Minute) == 0
                && Convert.ToInt32(timeNow.Second) == 0)
            {
                ToolViewModel.ToolDatas.Clear();
                LoadData();
            }
        }

        public async void Pip()
        {
            localSettings.Values["Pip"] = "1";
            var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            preferences.CustomSize = new Size(330, viewHeight);
            await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, preferences);
            //if (localSettings.Values["DesktopPin"] == null)
            //    localSettings.Values["DesktopPin"] = false;
            //if (localSettings.Values["DesktopPin"].Equals(false))
            //{
            //    localSettings.Values["DesktopPin"] = true;
                Frame.Navigate(typeof(DesktopTool), null, new SuppressNavigationTransitionInfo());
                //timer = new DispatcherTimer();
                //timer.Interval = new TimeSpan(0, 0, 1);
                //timer.Tick += Timer_Tick;//每秒触发这个事件，以刷新指针

            //}
            if (ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.CompactOverlay)
            {
                SetTopBtn.Visibility = Visibility.Collapsed;
                DeSetTopBtn.Visibility = Visibility.Visible;
            }
            if (ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.Default)
            {
                SetTopBtn.Visibility = Visibility.Visible;
                DeSetTopBtn.Visibility = Visibility.Collapsed;
            }
            ApplicationView.PreferredLaunchViewSize = new Size(3000, 3000);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            ////返回默认模式
            //var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.Default);
            //await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default, preferences);
        }

        public async void Unpip()
        {
            localSettings.Values["Pip"] = "0";
            var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.Default);
            preferences.CustomSize = new Size(330, viewHeight - 20);
            await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default, preferences);
            ApplicationView.PreferredLaunchViewSize = new Size(330, viewHeight - 20);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            Frame.Navigate(typeof(DesktopTool), null, new SuppressNavigationTransitionInfo());
            //timer = new DispatcherTimer();
            //timer.Interval = new TimeSpan(0, 0, 1);
            //timer.Tick += Timer_Tick;//每秒触发这个事件，以刷新指针
            if (ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.Default)
            {
                SetTopBtn.Visibility = Visibility.Visible;
                DeSetTopBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                SetTopBtn.Visibility = Visibility.Collapsed;
                DeSetTopBtn.Visibility = Visibility.Visible;
            }

        }

        private void SetTopBtn_Click(object sender, RoutedEventArgs e)
        {
            Pip();
        }

        private void DeSetTopBtn_Click(object sender, RoutedEventArgs e)
        {
            Unpip();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            ToolViewModel.ToolDatas.Clear();
            LoadData();
        }

        private async void DisplayMainViewBtn_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["ReStart"] = "1";
            await CoreApplication.RequestRestartAsync(string.Empty);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var gridList = MyFindListBoxChildOfType<Grid>(DesktopListView);
            foreach(var item in gridList)
            {
                if (item.Name == "ListGrid")
                    InitializeFrostedGlass(item, 1);
            }
        }
    }
}
