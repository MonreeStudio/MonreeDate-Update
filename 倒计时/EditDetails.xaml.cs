using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        private string _EndDate;
        private CountdownRecord _record;
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
            _record = All.Current.ScheduleService.GetById(App.AllItem.RecordId);
            if (_record == null)
            {
                UpdateButton.IsEnabled = false;
                DateValidationText.Text = "该日程已不存在，请返回列表刷新。";
                DateValidationText.Visibility = Visibility.Visible;
                return;
            }
            _isTop = _record.IsTop;
            AddEvent.Text = _record.Schedule_name;
            _Event = _record.Schedule_name;
            Add_Picker.Date = DateTime.Parse(_record.Date, CultureInfo.InvariantCulture);
            End_Picker.Date = string.IsNullOrWhiteSpace(_record.EndDate)
                ? (DateTimeOffset?)null
                : new DateTimeOffset(DateTime.Parse(_record.EndDate, CultureInfo.InvariantCulture));
            ScheduleTypePicker.SelectedIndex = ScheduleDateCalculator.IsRange(_record.ScheduleType, _record.EndDate) ? 1 : 0;
            MyEllipse.Fill = new SolidColorBrush(GetColor(_record.BgColor));
            _Color = _record.BgColor;
            MyColorPicker.Color = GetColor(_record.BgColor);
            _TintOpacity = _record.TintOpacity;
            MySlider.Value = _record.TintOpacity * 100;
            _tip = new CountdownSettingsService(localSettings).GetNote(_record.Id, _record.Schedule_name, _record.Date);
            TipTextbox.Text = _tip;
            TTB.Text = _tip;
            End_Picker.CalendarViewStyle = Add_Picker.CalendarViewStyle;
            UpdateScheduleDates();
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

        private async void BgsButton_Click(object sender, RoutedEventArgs e)
        {
            await BgsDialog.ShowAsync();
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string _event = AddEvent.Text.Trim();
            _Event = _event;
            UpdateScheduleDates();
            if (_record == null)
                return;
            if (IsRange && _Date == null && Add_Picker.Date.HasValue)
            {
                await new MessageDialog(End_Picker.Date.HasValue ? "结束日期不能早于开始日期。" : "请选择日程的结束日期。", "提示").ShowAsync();
                return;
            }
            if (_Date != null && _event != "" && _Color != "" && _TintOpacity >= 0)
            {
                try
                {
                    var updated = CountdownRecord.Create(_event, _Date, _PickDate, _Color, _TintOpacity, _isTop, _record.AddTime, _EndDate);
                    updated.Id = _record.Id;
                    All.Current.ScheduleService.UpdateWithNote(updated, _tip);
                    All.Current.NewTB.Visibility = Visibility.Collapsed;
                    All.Current.NewTB2.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    MessageDialog AboutDialog = new MessageDialog("日程保存失败，请重试。", "提示");
                    await AboutDialog.ShowAsync();
                    return;
                }
                // Keep the existing canonical or legacy tile ID; only refresh
                // its display name/content after the record has been edited.
                try { await All.Current.TileService.UpdateScheduleAsync(_record.Id); }
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message, "磁贴更新失败").ShowAsync();
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
