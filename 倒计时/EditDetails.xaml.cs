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
using 倒计时.Manager;
using 夏日.Models;
using CountdownRecord = 夏日.Models.CountdownRecord;

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
        private string _isTop;
        private string _tip;
        public double MinMyNav = MainPage.Current.MyNav.CompactModeThresholdWidth;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public EditDetails()
        {
            this.InitializeComponent();
            InitialData();
            SetThemeColor();
            MainPage.Current.MyNav.IsBackEnabled = true;
            MainPage.Current.SelectedPageItem = "EditDetails";
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
            List<CountdownRecord> datalist = All.Current.CountdownRepository.GetByName(App.AllItem.Str1);
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
            if (localSettings.Values[All.Current.str1 + All.Current.str3] != null)
            {
                TipTextbox.Text = localSettings.Values[All.Current.str1 + All.Current.str3].ToString();
                _tip = localSettings.Values[All.Current.str1 + All.Current.str3].ToString();
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
            if (Add_Picker.Date == null)
                return;
            string Picker = Add_Picker.Date.ToString();
            string[] strs = Picker.Split(" ");
            foreach (var str in strs)
            {
                if (str.Contains("/"))
                {
                    if (str.Contains("周") || str.Contains("星"))
                    {
                        int index = str.LastIndexOf("/");
                        Picker = str.Substring(0, index - 1);
                    }
                    else
                        Picker = str;
                }
            }
            try
            {
                DateTime s1 = Convert.ToDateTime(Picker);
                _PickDate = s1.ToString("yyyy-MM-dd");
                //_PickDate = string.Format("{0}/{1}/{2}", s1.Year, s1.Month, s1.Day);
            }
            catch(Exception e)
            {
                MessageDialog AboutDialog = new MessageDialog("日期选择发生错误。\n异常类型：" + e.GetType(), "发生异常");
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
            if (_Date != null && _event != "" && _Color != "" && _TintOpacity >= 0)
            {
                List<CountdownRecord> datalist = All.Current.CountdownRepository.GetByName(All.Current.str1);
                try
                {
                    All.Current.CountdownRepository.ReplaceByName(All.Current.str1, CountdownRecord.Create(_event, _Date, _PickDate, _Color, _TintOpacity, _isTop, ""));
                    localSettings.Values[_event + _PickDate] = _tip;
                    All.Current.NewTB.Visibility = Visibility.Collapsed;
                    All.Current.NewTB2.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    MessageDialog AboutDialog = new MessageDialog("此日程已被添加，请勿重复添加~", "提示");
                    await AboutDialog.ShowAsync();

                    foreach(var item in datalist)
                    {
                        All.Current.CountdownRepository.Insert(CountdownRecord.Create(item.Schedule_name, item.CalculatedDate, item.Date, item.BgColor, item.TintOpacity, "0", ""));
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
                All.Current.LoadDateData();
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
            
        }

        private string Calculator(string s1)
        {
            return CountdownDateCalculator.FormatDayCountdown(s1);
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

        private void TipDialogPrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            _tip = TipTextbox.Text;
            TTB.Text = _tip;
            TipDialog.Hide();
        }

        private void TipDialogCloseButton_Click(object sender, RoutedEventArgs e)
        {
            TipDialog.Hide();
        }

        private void BgsDialogPrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            _Color = MyColorPicker.Color.ToString();
            MyEllipse.Fill = new SolidColorBrush(GetColor(_Color));
            _TintOpacity = MySlider.Value / 100;
            BgsDialog.Hide();
        }

        private void BgsDialogCloseButton_Click(object sender, RoutedEventArgs e)
        {
            BgsDialog.Hide();
        }
    }
}
