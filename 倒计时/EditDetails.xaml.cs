using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using 倒计时;
using 夏日.Models;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 夏日
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class EditDetails : Page
    {
        private string _Event;
        private string _PickDate;
        private string _Date;
        private string _Color;
        private double _TintOpacity;
        private string tempDate;
        private string _isTop;
        private string _tip;
        public double MinMyNav = MainPage.Current.MyNav.CompactModeThresholdWidth;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public EditDetails()
        {
            this.InitializeComponent();
            InitialData();
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

        private void InitialData()
        {
            List<DataTemple> datalist = All.Current.conn.Query<DataTemple>("select * from DataTemple where Schedule_name = ?", App.AllItem.Str1);
            foreach (var item in datalist)
            {
                _isTop = item.IsTop;
                AddEvent.Text = item.Schedule_name;
                _Event = item.Schedule_name;
                Add_Picker.Date = Convert.ToDateTime(item.Date);
                _PickDate = item.Date;
                MyEllipse.Fill = new SolidColorBrush(GetColor(item.BgColor));
                _Color = item.BgColor;
                MyColorPicker.Color = GetColor(item.BgColor);
                _TintOpacity = item.TintOpacity;
                MySlider.Value = item.TintOpacity*100;
            }
            if (localSettings.Values[All.Current.str1] != null)
            {
                TipTextbox.Text = localSettings.Values[All.Current.str1].ToString();
                _tip = localSettings.Values[All.Current.str1].ToString();
                TTB.Text = _tip;
            }
            else
            {
                TipTextbox.Text = "";
                _tip = "";
            }
            AddEvent.Text = All.Current.str1;
            Add_Picker.Date = Convert.ToDateTime(All.Current.str3);
            MyEllipse.Fill = All.Current.str4;
        }

        public static Color GetColor(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            return Color.FromArgb(a, r, g, b);
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

        private async void BgsButton_Click(object sender, RoutedEventArgs e)
        {
            await BgsDialog.ShowAsync();
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string _event = AddEvent.Text.Trim();
            _Event = _event;
            //All.Current.Model_event = _event;
            if (_Date != null && _event != "" && _Color != "" && _TintOpacity >= 0)
            {
                List<DataTemple> datalist = All.Current.conn.Query<DataTemple>("select * from DataTemple where Schedule_name = ?", All.Current.str1);
                try
                {
                   // ViewModel.CustomDatas.Remove(SelectedItem);
                    All.Current.conn.Execute("delete from DataTemple where Schedule_name = ?", All.Current.str1);
                    All.Current.ViewModel.CustomDatas.Remove(App.AllItem);

                    All.Current.conn.Insert(new DataTemple() { Schedule_name = _event, CalculatedDate = _Date, Date = _PickDate, BgColor = _Color, TintOpacity = _TintOpacity, IsTop = _isTop, AddTime = "" });
                    All.Current.ViewModel.CustomDatas.Add(new CustomData() { Str1 = _event, Str2 = _Date, Str3 = _PickDate, Str4 = All.Current.ColorfulBrush(GetColor(_Color), _TintOpacity), BackGroundColor = GetColor(_Color) });
                    localSettings.Values[_event] = _tip;
                    All.Current.NewTB.Visibility = Visibility.Collapsed;
                    All.Current.NewTB2.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    MessageDialog AboutDialog = new MessageDialog("此日程已被添加，请勿重复添加~", "提示");
                    await AboutDialog.ShowAsync();

                    foreach(var item in datalist)
                    {
                        All.Current.conn.Insert(new DataTemple() { Schedule_name = item.Schedule_name, CalculatedDate = item.CalculatedDate, Date = item.Date, BgColor = item.BgColor, TintOpacity = item.TintOpacity,IsTop = "0",AddTime = "" });
                        All.Current.ViewModel.CustomDatas.Add(new CustomData() { Str1 = item.Schedule_name, Str2 = item.CalculatedDate, Str3 = item.Date, Str4 = All.Current.ColorfulBrush(GetColor(item.BgColor), item.TintOpacity), BackGroundColor = GetColor(item.BgColor) });
                    }
                    return;
                }
                bool isPinned = SecondaryTile.Exists(All.Current.str1);
                if (isPinned)
                {
                    SecondaryTile toBeDeleted = new SecondaryTile(All.Current.str1);
                    await toBeDeleted.RequestDeleteAsync();
                }
                MainPage.Current.MyNav.SelectedItem = MainPage.Current.MyNav.MenuItems[0];
                Frame.Navigate(typeof(All));
                PopupNotice popupNotice = new PopupNotice("修改成功");
                popupNotice.ShowAPopup();
            }
            else
            {
                MessageDialog AboutDialog = new MessageDialog("请确保填入完整的信息！", "提示");
                await AboutDialog.ShowAsync();
            }
        }

        private void BgsDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            _Color = MyColorPicker.Color.ToString();
            MyEllipse.Fill = new SolidColorBrush(GetColor(_Color));
            _TintOpacity = MySlider.Value / 100;
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
            {
                if (days != 0)
                    s2 = "已过" + days.ToString() + "天";
                else
                    s2 = "就在今天";
            }
            return s2;
        }

        private async void TipButton_Click(object sender, RoutedEventArgs e)
        {
            await TipDialog.ShowAsync();
        }

        private void TipDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
           
        }

        private void TipDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            _tip = TipTextbox.Text;
            TTB.Text = _tip;
        }
    }
}
