using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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
using 倒计时.Manager;
using 夏日.Models;
using CountdownRecord = 夏日.Models.CountdownRecord;

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
        private string _EndDate;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public ThemeColorDataViewModel ViewModel = new ThemeColorDataViewModel();

        public Add()
        {  
            this.InitializeComponent();
            Current = this;
            //this.NavigationCacheMode = NavigationCacheMode.Enabled;
            _TintOpacity = 0;
            _Color = "";
            _Tip = "";
            SetThemeColor();
            SetDateDialogTitle();
            MainPage.Current.MyNav.IsBackEnabled = true;
            MainPage.Current.SelectedPageItem = "Add";
            MyColorPicker.Color = Colors.CornflowerBlue;
            End_Picker.CalendarViewStyle = Add_Picker.CalendarViewStyle;
            UpdateScheduleDates();

        }

        private void ScheduleScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ScheduleCard == null)
                return;

            // Size to the content area, including when the navigation pane opens or closes.
            double availableWidth = Math.Max(0, e.NewSize.Width - ScheduleCard.Margin.Left - ScheduleCard.Margin.Right);
            ScheduleCard.Width = Math.Min(ScheduleCard.MaxWidth, availableWidth);
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

        private void SetDateDialogTitle()
        {
            var today = DateTime.Now;
            //DateTime todayFormat = Convert.ToDateTime(string.Format("{0}年{1}月{2}日", today.Year, today.Month, today.Day));
            TodayDate.Text = "今天是" + string.Format("{0}年{1}月{2}日", today.Year, today.Month, today.Day);
        }
        private bool IsRange => ScheduleTypePicker.SelectedIndex == 1;

        private void ScheduleTypePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateScheduleDates();
        }

        private void Add_Picker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            UpdateScheduleDates();
        }

        private void UpdateScheduleDates()
        {
            if (Add_Picker == null || End_Picker == null || DateValidationText == null)
                return;

            EndDatePanel.Visibility = IsRange ? Visibility.Visible : Visibility.Collapsed;
            StartDateLabel.Text = IsRange ? "开始日期" : "日程日期";
            Add_Picker.PlaceholderText = IsRange ? "选择开始日期" : "选择一个日期";
            _PickDate = Add_Picker.Date?.DateTime.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            _EndDate = IsRange ? End_Picker.Date?.DateTime.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null;
            bool invalidRange = IsRange && Add_Picker.Date.HasValue && End_Picker.Date.HasValue
                && End_Picker.Date.Value.DateTime.Date < Add_Picker.Date.Value.DateTime.Date;
            DateValidationText.Text = invalidRange ? "结束日期不能早于开始日期。" : "";
            DateValidationText.Visibility = invalidRange ? Visibility.Visible : Visibility.Collapsed;
            _Date = _PickDate == null || (IsRange && (_EndDate == null || invalidRange))
                ? null
                : ScheduleDateCalculator.FormatCountdown(_PickDate, _EndDate, IsRange ? "Range" : "Point");
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string _event = AddEvent.Text.Trim();
            _Event = _event;
            UpdateScheduleDates();
            if (IsRange && _Date == null && Add_Picker.Date.HasValue)
            {
                await new MessageDialog(End_Picker.Date.HasValue ? "结束日期不能早于开始日期。" : "请选择日程的结束日期。", "温馨提示").ShowAsync();
                return;
            }
            //All.Current.Model_event = _event;
            if (_Date != null&&_event!=""&&_Color!=""&&_TintOpacity>=0)
            {
                try
                {
                    var record = CountdownRecord.Create(_event, _Date, _PickDate, _Color, _TintOpacity, "0", "", _EndDate);
                    All.Current.ScheduleService.CreateWithNote(record, _Tip);
                    //All.Current.ViewModel.CustomDatas.Add(new CustomData() { Str1 = _event, Str2 = _Date, Str3 = _PickDate, Str4 = All.Current.ColorfulBrush(GetColor(_Color),_TintOpacity) ,BackGroundColor = GetColor(_Color)});
                    All.Current.NewTB.Visibility = Visibility.Collapsed;
                    All.Current.NewTB2.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    MessageDialog AboutDialog = new MessageDialog("日程保存失败，请重试。","提示");
                    await AboutDialog.ShowAsync();
                    return;
                }
                MainPage.Current.MyNav.SelectedItem = MainPage.Current.MyNav.MenuItems[0];
                Frame.Navigate(typeof(All));
                All.Current.LoadDateData();
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
            TipTextbox.Text = "";
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            Add_Picker.IsCalendarOpen = true;
        }

        private async void DateChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            await DateDialog.ShowAsync();
        }

        private void DaysTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = (TextBox)sender;
            if (!Regex.IsMatch(textbox.Text, "^\\d*\\.?\\d*$") && textbox.Text != "")
            {
                int pos = textbox.SelectionStart - 1;
                textbox.Text = textbox.Text.Remove(pos, 1);
                textbox.SelectionStart = pos;
            }
        }

        private void ThemeColorSelected_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ColorGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var _item = (ThemeColorData)e.ClickedItem;
            MyColorPicker.Color = _item.themeColor.Color;
            DefaltColorFlyout.Hide();
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

        private void TipDialogPrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            _Tip = TipTextbox.Text;
            TTB.Text = _Tip;
            TipDialog.Hide();
        }

        private void TipDialogCloseButton_Click(object sender, RoutedEventArgs e)
        {
            TipTextbox.Text = "";
            TipDialog.Hide();
        }

        private async void DateDialogPrimaryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var today = DateTime.Now;
                var count = Convert.ToInt32(DaysTextBox.Text);
                if (today.AddDays(count) > Convert.ToDateTime("2120/12/31"))
                {
                    PopupNotice popupNotice = new PopupNotice("超出了最大范围");
                    popupNotice.ShowAPopup();
                }
                var res = today.AddDays(count);
                Add_Picker.Date = Convert.ToDateTime(res);
            }
            catch (FormatException e0)
            {

            }
            catch (Exception e1)
            {
                MessageDialog message = new MessageDialog("异常类型：" + e1.ToString() + "影响使用的话请及时反馈。", "发生异常！");
                await message.ShowAsync();
            }
            
            DaysTextBox.Text = "";
            DateDialog.Hide();
        }

        private void DateDialogCloseButton_Click(object sender, RoutedEventArgs e)
        {
            DaysTextBox.Text = "";
            DateDialog.Hide();
        }
    }
}
