using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using 夏日.Models;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 倒计时
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    /// 

    public sealed partial class Add : Page
    {
        public static Add Current;
        private string _Event { get; set; }
        private string _PickDate { get; set; }
        private string _Date { get; set; }
        private string _Color { get; set; }
        private double _TintOpacity { get; set; }
        private string _Tip { get; set; }
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public Add()
        {  
            this.InitializeComponent();
            Current = this;
            //this.NavigationCacheMode = NavigationCacheMode.Enabled;
            _TintOpacity = 0;
            _Color = "";
            _Tip = "";
            SetThemeColor();
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
                    TC.Color = Colors.DeepSkyBlue;
                    break;
                case "Orange":
                    TC.Color = Colors.Orange;
                    break;
                case "Crimson":
                    TC.Color = Colors.Crimson;
                    break;
                case "Gray":
                    TC.Color = Color.FromArgb(255, 73, 92, 105);
                    break;
                default:
                    break;
            }
        }

        private string Calculator(string s1)
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
            {if (days != 0)
                    s2 = "已过" + days.ToString() + "天";
                else
                    s2 = "就在今天";
            }
                return s2;
        }

        private async void Add_Picker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            string Picker = Add_Picker.Date.ToString();
            try
            {
                DateTime s1 = Convert.ToDateTime(Picker);
                _PickDate = s1.ToString("yyyy-MM-dd");
                //_PickDate = string.Format("{0}/{1}/{2}", s1.Year, s1.Month, s1.Day);
            }
            catch
            {
                MessageDialog AboutDialog = new MessageDialog("日期选择发生错误。", "发生异常");
                await AboutDialog.ShowAsync();
            }
            _Date = Calculator(_PickDate);
        }

        private void Add_Picker_Loaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string _event = AddEvent.Text.Trim();
            _Event = _event;
            //All.Current.Model_event = _event;
            if (_Date != null&&_event!=""&&_Color!=""&&_TintOpacity>=0)
            {
                try
                {
                    All.Current.conn.Insert(new DataTemple() { Schedule_name = _event, CalculatedDate = _Date, Date = _PickDate, BgColor = _Color, TintOpacity = _TintOpacity, IsTop = "0",AddTime = "" });
                    All.Current.ViewModel.CustomDatas.Add(new CustomData() { Str1 = _event, Str2 = _Date, Str3 = _PickDate, Str4 = All.Current.ColorfulBrush(GetColor(_Color),_TintOpacity) ,BackGroundColor = GetColor(_Color)});
                    localSettings.Values[_event] = _Tip;
                    All.Current.NewTB.Visibility = Visibility.Collapsed;
                    All.Current.NewTB2.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    MessageDialog AboutDialog = new MessageDialog("此日程已被添加，请勿重复添加~","提示");
                    await AboutDialog.ShowAsync();
                    return;
                }
                MainPage.Current.MyNav.SelectedItem = MainPage.Current.MyNav.MenuItems[0];
                Frame.Navigate(typeof(All));
                PopupNotice popupNotice = new PopupNotice("添加成功");
                popupNotice.ShowAPopup();
            }
            else
            {
                if (_Event == "")
                {
                    MessageDialog AboutDialog = new MessageDialog("你还未填写日程名哦。", "温馨提示");
                    await AboutDialog.ShowAsync();
                }
                else
                {
                    if (_Date == null)
                    {
                        MessageDialog AboutDialog = new MessageDialog("你还未选择日程的日期哦。", "温馨提示");
                        await AboutDialog.ShowAsync();
                    }
                    else
                    {
                        if (_Color == "")
                        {
                            MessageDialog AboutDialog = new MessageDialog("你还未更改背景选项，快选择你喜欢的背景颜色吧！", "温馨提示");
                            await AboutDialog.ShowAsync();
                        }
                    }
                }
            }
        }

        private async void BgsButton_Click(object sender, RoutedEventArgs e)
        {
            await BgsDialog.ShowAsync();
        }

        private void BgsDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            _Color = MyColorPicker.Color.ToString();
            MyEllipse.Fill = new SolidColorBrush(GetColor(_Color));
            _TintOpacity = MySlider.Value/100;
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

        private async void TipButton_Click(object sender, RoutedEventArgs e)
        {
            await TipDialog.ShowAsync();
        }

        private void TipDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            _Tip = TipTextbox.Text;
            TTB.Text = _Tip;
        }

        private void TipDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            
        }
    }
}
