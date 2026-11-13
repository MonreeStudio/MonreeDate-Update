using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using 倒计时.Models;
using 倒计时.Manager;
using 夏日;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 倒计时
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Details : Page
    {
        public static Details Current;
        private bool sp;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private CountdownSettingsService scheduleSettings;
        string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "mydb.sqlite");    //建立数据库  
        public SQLite.Net.SQLiteConnection conn;
        DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
        IRandomAccessStream Bitmap;
        string DetailsDateMode;
        string FestivalDateMode;
        string dayNum;
        string fullCountdownText = "";

        private void SetCountdownHero(string full)
        {
            PopulateCountdownHero(full);
            if (System.Text.RegularExpressions.Regex.IsMatch(DetailsDate.Text, @"^\d+$"))
            {
                DetailsDate.FontSize = 168;
                string unit = CountdownUnit.Text.Replace("开始", "");
                switch (CountdownPrefix.Text)
                {
                    case "还有": CountdownUnit.Text = unit + "后到来"; break;
                    case "距离开始还有": CountdownUnit.Text = unit + "后开始"; break;
                    case "距离结束还有": CountdownUnit.Text = unit + "后结束"; break;
                    case "已经过去": CountdownUnit.Text = unit + "前"; break;
                    case "已结束": CountdownUnit.Text = unit + "前结束"; break;
                }
                CountdownPrefix.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>Keep countdown parsing shared by day, week and mixed-unit modes.</summary>
        private void PopulateCountdownHero(string full)
        {
            fullCountdownText = full ?? "";
            dCalDate.Text = fullCountdownText;
            DetailsDate.FontSize = 92;
            CountdownPrefix.Text = "";
            CountdownUnit.Text = "天";
            DetailsDate.Text = fullCountdownText;
            CountdownPrefix.Visibility = Visibility.Visible;
            CountdownUnit.Visibility = Visibility.Visible;

            if (string.IsNullOrEmpty(full))
                return;

            if (full == "就在今天")
            {
                CountdownPrefix.Text = "就在";
                DetailsDate.Text = "今天";
                CountdownUnit.Text = "";
                DetailsDate.FontSize = 72;
                return;
            }

            string caption = "";
            string body = full;
            string suffix = "";

            if (body.StartsWith("进行中，还剩"))
            {
                caption = "距离结束还有";
                body = body.Substring("进行中，还剩".Length);
            }
            else if (body.StartsWith("进行中，"))
            {
                // e.g. "进行中，今天结束"
                var rest = body.Substring("进行中，".Length);
                if (rest.Contains("今天"))
                {
                    CountdownPrefix.Text = "今天";
                    DetailsDate.Text = "结束";
                    CountdownUnit.Text = "";
                    DetailsDate.FontSize = 64;
                }
                else
                {
                    CountdownPrefix.Text = "进行中";
                    DetailsDate.Text = rest;
                    CountdownUnit.Text = "";
                    DetailsDate.FontSize = 48;
                }
                return;
            }
            else if (body.StartsWith("还有"))
            {
                caption = "还有";
                body = body.Substring(2);
                if (body.EndsWith("开始"))
                {
                    suffix = "开始";
                    body = body.Substring(0, body.Length - 2);
                    caption = "距离开始还有";
                }
            }
            else if (body.StartsWith("已过"))
            {
                caption = "已经过去";
                body = body.Substring(2);
            }
            else if (body.StartsWith("已结束"))
            {
                caption = "已结束";
                body = body.Substring(3);
            }

            var match = System.Text.RegularExpressions.Regex.Match(body, @"^(\d+)([天周年月])$");
            if (match.Success)
            {
                CountdownPrefix.Text = caption;
                DetailsDate.Text = match.Groups[1].Value;
                CountdownUnit.Text = match.Groups[2].Value + suffix;
                CountdownPrefix.Visibility = string.IsNullOrEmpty(caption) ? Visibility.Collapsed : Visibility.Visible;
                CountdownUnit.Visibility = string.IsNullOrEmpty(CountdownUnit.Text) ? Visibility.Collapsed : Visibility.Visible;
                return;
            }

            CountdownPrefix.Text = caption;
            DetailsDate.Text = body + suffix;
            CountdownUnit.Text = "";
            CountdownPrefix.Visibility = string.IsNullOrEmpty(caption) ? Visibility.Collapsed : Visibility.Visible;
            CountdownUnit.Visibility = Visibility.Collapsed;
            DetailsDate.FontSize = 48;
        }

        private static Color SoftTint(Color color, double strength)
        {
            strength = Math.Max(0, Math.Min(1, strength));
            return Color.FromArgb(255,
                (byte)(255 - (255 - color.R) * strength),
                (byte)(255 - (255 - color.G) * strength),
                (byte)(255 - (255 - color.B) * strength));
        }

        private static double LinearColorChannel(byte value)
        {
            double channel = value / 255.0;
            return channel <= 0.04045 ? channel / 12.92 : Math.Pow((channel + 0.055) / 1.055, 2.4);
        }

        private static double RelativeLuminance(Color color)
        {
            return 0.2126 * LinearColorChannel(color.R)
                + 0.7152 * LinearColorChannel(color.G)
                + 0.0722 * LinearColorChannel(color.B);
        }

        private static Color GetDetailsForeground(LinearGradientBrush background)
        {
            // Prefer white as a visual style; reserve dark text for very pale backgrounds.
            // This is a brightness threshold, not a minimum accessibility contrast ratio.
            const double paleBackgroundThreshold = 0.6;
            if (background.GradientStops.Count == 0) return Colors.White;
            double totalLuminance = 0;
            foreach (var stop in background.GradientStops)
            {
                totalLuminance += RelativeLuminance(stop.Color);
            }
            double averageLuminance = totalLuminance / background.GradientStops.Count;
            return averageLuminance >= paleBackgroundThreshold
                ? Color.FromArgb(255, 28, 35, 42)
                : Colors.White;
        }

        private void ApplyHeroAccent(Color color)
        {
            bool dark = ActualTheme == ElementTheme.Dark;
            // Restore the original theme-colored acrylic-style gradient behind the details page.
            double s0 = dark ? 0.22 : 0.38;
            double s1 = dark ? 0.28 : 0.48;
            double s2 = dark ? 0.36 : 0.60;
            var gradient = new LinearGradientBrush
            {
                StartPoint = new Point(0.1, 0),
                EndPoint = new Point(0.9, 1)
            };
            gradient.GradientStops.Add(new GradientStop { Color = SoftTint(color, s0), Offset = 0.0 });
            gradient.GradientStops.Add(new GradientStop { Color = SoftTint(color, s1), Offset = 0.45 });
            gradient.GradientStops.Add(new GradientStop { Color = SoftTint(color, s2), Offset = 1.0 });
            DetailsGrid.Background = gradient;

            Color primaryColor = GetDetailsForeground(gradient);
            Color secondaryColor = primaryColor;
            var primary = new SolidColorBrush(primaryColor);
            var secondary = new SolidColorBrush(secondaryColor);
            DetailsEvent.Foreground = primary;
            DetailsPickedDate.Foreground = secondary;
            LunarDate.Foreground = secondary;
            DurationText.Foreground = secondary;
            CountdownPrefix.Foreground = secondary;
            DetailsDate.Foreground = primary;
            CountdownUnit.Foreground = secondary;
            NotesTitle.Foreground = secondary;
            TipText.Foreground = primary;
            HeaderDate.Foreground = secondary;
            DetailsCommandBar.Foreground = primary;
            // Keep the status as a quiet text label; the divider lines provide the structure.
            StatusChipText.Foreground = primary;
            StatusChip.Background = new SolidColorBrush(Colors.Transparent);
            ApplyRangeProgressCardSurface(color);
            MainPage.Current?.ApplyLightImmersiveTitleBar();
        }

        // Match the schedule-list avatar card: opaque theme surface, not page-gradient wash.
        private void ApplyRangeProgressCardSurface(Color progressColor)
        {
            var resources = Application.Current.Resources;
            Brush cardBackground = resources["AppCardBackgroundBrush"] as Brush
                ?? new SolidColorBrush(Colors.White);
            Brush mediumText = resources["SystemControlForegroundBaseMediumBrush"] as Brush
                ?? new SolidColorBrush(Colors.Gray);
            Brush lowTrack = resources["SystemControlForegroundBaseLowBrush"] as Brush
                ?? new SolidColorBrush(Colors.LightGray);

            ProgressCardBorder.Background = cardBackground;
            ProgressCardBorder.BorderThickness = new Thickness(0);
            ProgressCardBorder.BorderBrush = null;
            // Drop the accent wash so the solid card surface stays legible.
            ProgressCardTint.Background = new SolidColorBrush(Colors.Transparent);
            RangeStartLabel.Foreground = mediumText;
            RangeEndLabel.Foreground = mediumText;
            RangeTodayLabel.Foreground = mediumText;
            RangeProgressTrack.Background = lowTrack;
            RangeProgressFill.Background = new SolidColorBrush(progressColor);
            RangeProgressThumb.Fill = new SolidColorBrush(progressColor);
        }

        private void DetailsGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplyResponsiveLayout(e.NewSize.Width);
        }

        private void DetailScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplyResponsiveLayout(DetailsGrid.ActualWidth);
        }

        private void ApplyResponsiveLayout(double width)
        {
            bool compact = width < 700;
            double inset = compact ? 24 : 48;
            DetailChrome.Margin = new Thickness(inset, 16, inset, 0);
            DetailChrome.Width = Math.Min(DetailChrome.MaxWidth, Math.Max(0, width - 2 * inset));
            DetailContent.Margin = new Thickness(inset, 12, inset, 40);
            HeroTextColumn.Width = new GridLength(compact ? 1 : 3, GridUnitType.Star);
            HeroCountColumn.Width = compact ? new GridLength(0) : new GridLength(2, GridUnitType.Star);
            Grid.SetRow(CountdownPanel, compact ? 1 : 0);
            Grid.SetColumn(CountdownPanel, compact ? 0 : 1);
            HeroDivider.Visibility = compact ? Visibility.Collapsed : Visibility.Visible;
            HeroInfo.Margin = new Thickness(0, 0, compact ? 0 : 32, 0);
            StatusChip.HorizontalAlignment = compact ? HorizontalAlignment.Center : HorizontalAlignment.Left;
            StatusChipText.TextAlignment = compact ? TextAlignment.Center : TextAlignment.Left;
            DetailsEvent.TextAlignment = compact ? TextAlignment.Center : TextAlignment.Left;
            DetailsPickedDate.TextAlignment = compact ? TextAlignment.Center : TextAlignment.Left;
            LunarDate.TextAlignment = compact ? TextAlignment.Center : TextAlignment.Left;
            DurationText.TextAlignment = compact ? TextAlignment.Center : TextAlignment.Left;
            bool fillViewport = compact && !hasNotes && ProgressCard.Visibility != Visibility.Visible;
            HeroInfoRow.Height = fillViewport ? new GridLength(1, GridUnitType.Star) : GridLength.Auto;
            HeroCountdownRow.Height = fillViewport ? new GridLength(1, GridUnitType.Star) : GridLength.Auto;
            HeroLayout.Height = fillViewport && DetailScrollViewer.ActualHeight > 0
                ? Math.Max(300, DetailScrollViewer.ActualHeight - DetailContent.Margin.Top - DetailContent.Margin.Bottom - DetailTopDivider.Height)
                : double.NaN;
            CountdownPanel.Margin = compact && !fillViewport ? new Thickness(0, 28, 0, 0) : compact ? new Thickness(0) : new Thickness(32, 0, 0, 0);
            // Range schedules need more room below the hero for the timeline and notes.
            // Without notes, push the main body lower so the page does not hug the chrome.
            bool isRange = sp && App.AllItem != null
                && ScheduleDateCalculator.IsRange(App.AllItem.ScheduleType, App.AllItem.EndDate);
            double heroTopMargin = isRange ? (compact ? 28 : 88) : (compact ? 52 : 152);
            if (!hasNotes)
                heroTopMargin += compact ? 32 : 64;
            HeroLayout.Margin = fillViewport ? new Thickness(0) : new Thickness(0, heroTopMargin, 0, compact ? 32 : 0);
            DetailsEvent.FontSize = compact ? 36 : 48;
        }

        private void ApplyRangeProgress(string start, string end)
        {
            DateTime startDate = Convert.ToDateTime(start);
            DateTime endDate = Convert.ToDateTime(end);
            DateTime today = DateTime.Today;
            int total = Math.Max(1, (endDate.Date - startDate.Date).Days);
            int elapsed = Math.Max(0, Math.Min(total, (today - startDate.Date).Days));
            double ratio = (double)elapsed / total;
            rangeProgressRatio = ratio;
            RangeStartLabel.Text = startDate.ToString("M月d日") + " · 开始";
            RangeEndLabel.Text = endDate.ToString("M月d日") + " · 结束";
            RangeTodayLabel.Text = "今天 · " + today.ToString("M月d日");
            ProgressCard.Visibility = Visibility.Visible;
            UpdateRangeProgressLayout();
        }

        private double rangeProgressRatio;
        private bool hasNotes;

        private void UpdateRangeProgressLayout()
        {
            double width = RangeProgressHost.ActualWidth;
            if (width <= 0)
                return;
            double fill = Math.Max(0, width * rangeProgressRatio);
            RangeProgressFill.Width = fill;
            RangeProgressThumb.Margin = new Thickness(Math.Max(0, Math.Min(width - 10, fill - 5)), 0, 0, 0);
        }

        private void RangeProgressHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateRangeProgressLayout();
        }

        public Details()
        {
            this.InitializeComponent();
            Current = this;
            scheduleSettings = new CountdownSettingsService(localSettings);
            DetailsDateMode = localSettings.Values["DateMode"]?.ToString() ?? "Day";
            FestivalDateMode = "Day";
            LoadData();
            SetBorderColor();
            SetThemeColor();
            SetAlertEnabled();
            ActualThemeChanged += (sender, args) => ApplyHeroAccent(sp ? App.AllItem.BackGroundColor : App.FestivalItem.Str4);
            DataTransferManager.GetForCurrentView().DataRequested += DataTransferManager_DataRequested;
            MainPage.Current.MyNav.IsBackEnabled = true;
            MainPage.Current.SelectedPageItem = "Details";
            
        }

        private void SetAlertEnabled()
        {
            var selectedDate = Convert.ToDateTime(sp ? App.AllItem.Str3 : App.FestivalItem.Str3);
            if (selectedDate > DateTime.Now)
                AlertButton.IsEnabled = true;
            else
                AlertButton.IsEnabled = false;
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

        private void SetBorderColor()
        {
            YellowRec.Fill = new SolidColorBrush(Color.FromArgb(255,246,247,231));
            BlueRec.Fill = new SolidColorBrush(Color.FromArgb(255, 235, 246, 252));
            GreenRec.Fill = new SolidColorBrush(Color.FromArgb(255, 241, 253, 241));
            PinkRec.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 246, 239));
            RenderGrid.Background = BlueRec.Fill;
            RenderTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 106, 164, 210));
        }
        private void LoadData()
        {
            sp = MainPage.Current.SelectedPage;
            ProgressCard.Visibility = Visibility.Collapsed;
            DurationText.Visibility = Visibility.Collapsed;
            RangeReminderHint.Visibility = Visibility.Collapsed;
            DateTime headerDate = Convert.ToDateTime(sp ? App.AllItem.Str3 : App.FestivalItem.Str3);
            HeaderDate.Text = headerDate.ToString("MM / dd");
            NotesSection.Visibility = Visibility.Collapsed;
            if (sp == true)
            {
                bool isRange = ScheduleDateCalculator.IsRange(App.AllItem.ScheduleType, App.AllItem.EndDate);
                DetailsPickedDate.Text = ScheduleDateCalculator.FormatDates(App.AllItem.Str3, App.AllItem.EndDate, App.AllItem.ScheduleType);
                DetailsEvent.Text = App.AllItem.Str1;
                SetCountdownHero(ScheduleDateCalculator.FormatCountdown(App.AllItem.Str3, App.AllItem.EndDate, App.AllItem.ScheduleType, DetailsDateMode));
                ApplyHeroAccent(App.AllItem.BackGroundColor);
                if (isRange)
                {
                    StatusChipText.Text = GetScheduleStatus(App.AllItem.Str3, App.AllItem.EndDate, true);
                    ApplyRangeProgress(App.AllItem.Str3, App.AllItem.EndDate);
                    DurationText.Text = ScheduleDateCalculator.FormatDuration(App.AllItem.Str3, App.AllItem.EndDate);
                    DurationText.Visibility = Visibility.Visible;
                    RangeReminderHint.Visibility = Visibility.Visible;
                    ToolTipService.SetToolTip(AlertButton, "为范围日程的开始日期设置提醒");
                }
                else
                {
                    StatusChipText.Text = GetScheduleStatus(App.AllItem.Str3, App.AllItem.EndDate, false);
                    LunarDate.Text = LunarCalendar.GetChineseDateTime(Convert.ToDateTime(App.AllItem.Str3));
                }
                string tip = string.Empty;
                try
                {
                    tip = scheduleSettings.GetNote(
                        App.AllItem?.RecordId,
                        App.AllItem?.Str1,
                        App.AllItem?.Str3) ?? string.Empty;
                }
                catch { tip = string.Empty; }
                
                TipText.Text = tip ?? string.Empty;
                bool hasTip = !string.IsNullOrWhiteSpace(tip);
                hasNotes = hasTip;
                TipCard.Visibility = hasTip ? Visibility.Visible : Visibility.Collapsed;
                NotesSection.Visibility = hasTip ? Visibility.Visible : Visibility.Collapsed;
                NotesTitle.Visibility = hasTip ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                EditButton.IsEnabled = false;
                DetailsEvent.Text = App.FestivalItem.Str1;
                SetCountdownHero(App.FestivalItem.Str2);
                DetailsPickedDate.Text = App.FestivalItem.Str3;
                LunarDate.Text = LunarCalendar.GetChineseDateTime(Convert.ToDateTime(App.FestivalItem.Str3));
                ApplyHeroAccent(App.FestivalItem.Str4);
                StatusChipText.Text = "节日";
                TipText.Text = "节日";
                hasNotes = false;
                TipCard.Visibility = Visibility.Collapsed;
                NotesSection.Visibility = Visibility.Collapsed;
            }
            if (localSettings.Values["Lunar"] != null)
            {
                if (localSettings.Values["Lunar"].ToString() == "1")
                    LunarDate.Visibility = Visibility.Visible;
                else
                    LunarDate.Visibility = Visibility.Collapsed;
            }
            else
                LunarDate.Visibility = Visibility.Visible;
            if (sp && ScheduleDateCalculator.IsRange(App.AllItem.ScheduleType, App.AllItem.EndDate))
                LunarDate.Visibility = Visibility.Collapsed;
            dEvent.Text = DetailsEvent.Text;
            dCalDate.Text = fullCountdownText;
            dDate.Text = DetailsPickedDate.Text;
            if (DurationText.Visibility == Visibility.Visible)
                dDate.Text += "\n" + DurationText.Text;
            RenderPicture.ProfilePicture = All.Current.AllPicture.ProfilePicture;
            ApplyResponsiveLayout(ActualWidth > 0 ? ActualWidth : 1200);
        }

        public static AcrylicBrush ColorfulBrush(Color temp)
        {
            AcrylicBrush myBrush = new AcrylicBrush();
            myBrush.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
            myBrush.TintColor = temp;
            myBrush.FallbackColor = temp;
            // Slightly denser tint keeps white type readable on light schedule colors.
            myBrush.TintOpacity = 0.92;
            return myBrush;
        }

        private static string GetScheduleStatus(string startText, string endText, bool isRange)
        {
            DateTime start = Convert.ToDateTime(startText).Date;
            DateTime today = DateTime.Today;
            if (isRange)
            {
                DateTime end = Convert.ToDateTime(endText).Date;
                if (today < start) return "倒数日";
                if (today > end) return "已结束";
                return "进行中";
            }
            return today > start ? "已结束" : "倒数日";
        }

        /// <summary>Light product chrome; accent follows the schedule color.</summary>
        private void ApplyDetailsMaterial(Color themeColor)
        {
            ApplyHeroAccent(themeColor);
        }


        private void DetailsDate_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (MainPage.Current.SelectedPage)
            {
                switch (DetailsDateMode)
                {
                    case "Day":
                        DetailsDateMode = "Week";
                        PopupNotice popupNotice0 = new PopupNotice("周数模式");
                        popupNotice0.ShowAPopup();
                        break;
                    case "Week":
                        DetailsDateMode = "Year";
                        PopupNotice popupNotice1 = new PopupNotice("年数模式");
                        popupNotice1.ShowAPopup();
                        break;
                    case "Year":
                        DetailsDateMode = "Day";
                        PopupNotice popupNotice2 = new PopupNotice("天数模式");
                        popupNotice2.ShowAPopup();
                        break;
                }
                SetCountdownHero(ScheduleDateCalculator.FormatCountdown(App.AllItem.Str3, App.AllItem.EndDate, App.AllItem.ScheduleType, DetailsDateMode));
                dCalDate.Text = fullCountdownText;
            }
            else
            {
                DateTime d1 = Convert.ToDateTime(Festival.Current.str3);
                DateTime d2 = DateTime.Now;
                DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
                DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));

                switch (FestivalDateMode)
                {
                    case "Day":
                        SetCountdownHero(All.Current.ConvertToWeek(Festival.Current.str3));
                        FestivalDateMode = "Week";
                        PopupNotice popupNotice0 = new PopupNotice("周数模式");
                        popupNotice0.ShowAPopup();
                        break;
                    case "Week":
                        if (d4 > d3)
                            SetCountdownHero("已过" + App.Term(d3, d4));
                        else
                        {
                            if (d4 < d3)
                                SetCountdownHero("还有" + App.Term(d4, d3));
                        }
                        FestivalDateMode = "Year";
                        PopupNotice popupNotice1 = new PopupNotice("年数模式");
                        popupNotice1.ShowAPopup();
                        break;
                    case "Year":
                        SetCountdownHero(CustomData.Calculator(Festival.Current.str3));
                        FestivalDateMode = "Day";
                        PopupNotice popupNotice2 = new PopupNotice("天数模式");
                        popupNotice2.ShowAPopup();
                        break;
                }
                dCalDate.Text = fullCountdownText;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EditDetails),null,new DrillInNavigationTransitionInfo());
        }

        private async void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            await RenderDialog.ShowAsync();
        }

        public static async Task<WriteableBitmap> RenderUIElement(UIElement element)
        {
            var bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(element);
            var pixelBuffer = await bitmap.GetPixelsAsync();
            var pixels = pixelBuffer.ToArray();
            //var writeableBitmap = new WriteableBitmap(500, 300);
            var writeableBitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight);
            using (Stream stream = writeableBitmap.PixelBuffer.AsStream())
            {
                await stream.WriteAsync(pixels, 0, pixels.Length);
            }
            return writeableBitmap;
        }

        private async void Share_Click(object sender, RoutedEventArgs e)
        {
            var image = await RenderUIElement(RenderBorder);
            Bitmap = await ConvertWriteableBitmapToRandomAccessStream(image);
            //var bitmap = RandomAccessStreamReference.CreateFromStream(bitImage);
            DataTransferManager.ShowShareUI();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= DataTransferManager_DataRequested;
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            
            //var bitmap = rasr.CreateFromStream(bitImage);
            //DataRequest request = args.Request;
            DataRequestDeferral deferral = args.Request.GetDeferral();
            args.Request.Data.Properties.Title = "分享日程";
            args.Request.Data.Properties.Description = "夏日——记录你的生活";
            args.Request.Data.SetText("分享自“夏日”");
            args.Request.Data.SetBitmap(RandomAccessStreamReference.CreateFromStream(Bitmap));
            deferral.Complete();
        }
        
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(RenderBorder);

            var saveFile = new FileSavePicker();
            saveFile.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            saveFile.FileTypeChoices.Add("JPEG files", new List<string>() { ".jpg" });
            saveFile.SuggestedFileName = "夏日：" + DetailsEvent.Text;
            StorageFile sFile = await saveFile.PickSaveFileAsync();
            if (sFile == null)
                return;

            var pixels = await rtb.GetPixelsAsync();
            using (IRandomAccessStream stream = await sFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await
                BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                byte[] bytes = pixels.ToArray();
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                        BitmapAlphaMode.Ignore,
                                        (uint)rtb.PixelWidth,
                                        (uint)rtb.PixelHeight,
                                        200,
                                        200,
                                        bytes);

                await encoder.FlushAsync();
            }
            PopupNotice popupNotice = new PopupNotice("保存成功");
            popupNotice.ShowAPopup();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            RenderDialog.Hide();
        }

        //private async void TestButton_Click(object sender, RoutedEventArgs e)
        //{
        //    ////var image = await RenderUIElement(RenderBorder);
        //    var rtb = new RenderTargetBitmap();
        //    await rtb.RenderAsync(RenderBorder);

        //    var saveFile = new FileSavePicker();
        //    saveFile.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        //    saveFile.FileTypeChoices.Add("JPEG files", new List<string>() { ".jpg" });
        //    saveFile.SuggestedFileName = "夏日："+ DetailsEvent.Text;
        //    StorageFile sFile = await saveFile.PickSaveFileAsync();
        //    if (sFile == null)
        //        return;

        //    var pixels = await rtb.GetPixelsAsync();
        //    using (IRandomAccessStream stream = await sFile.OpenAsync(FileAccessMode.ReadWrite))
        //    {
        //        var encoder = await
        //        BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
        //        byte[] bytes = pixels.ToArray();
        //        encoder.SetPixelData(BitmapPixelFormat.Bgra8,
        //                                BitmapAlphaMode.Ignore,
        //                                (uint)rtb.PixelWidth,
        //                                (uint)rtb.PixelHeight,
        //                                200,
        //                                200,
        //                                bytes);

        //        await encoder.FlushAsync();
        //    }
        //    PopupNotice popupNotice = new PopupNotice("保存成功");
        //    popupNotice.ShowAPopup();
        //}
        public static async Task<IRandomAccessStream> ConvertWriteableBitmapToRandomAccessStream(WriteableBitmap writeableBitmap)
        {
            var stream = new InMemoryRandomAccessStream();

            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
            Stream pixelStream = writeableBitmap.PixelBuffer.AsStream();
            byte[] pixels = new byte[pixelStream.Length];
            await pixelStream.ReadAsync(pixels, 0, pixels.Length);

            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)writeableBitmap.PixelWidth, (uint)writeableBitmap.PixelHeight, 96.0, 96.0, pixels);
            await encoder.FlushAsync();

            return stream;
        }

        private void YellowRec_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RenderGrid.Background = new SolidColorBrush(Color.FromArgb(255, 246, 247, 231));
            RenderTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 238, 177, 60));
        }

        private void BlueRec_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RenderGrid.Background = new SolidColorBrush(Color.FromArgb(255, 235, 246, 252));
            RenderTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 106, 164, 210));
        }

        private void GreenRec_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RenderGrid.Background = new SolidColorBrush(Color.FromArgb(255, 241, 253, 241));
            RenderTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 131, 179, 141));
        }

        private void PinkRec_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RenderGrid.Background = new SolidColorBrush(Color.FromArgb(255, 255, 246, 239));
            RenderTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 230, 129, 111));
        }

        private void AlertToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (AlertToggleSwitch!=null)
                SetAlert("", AlertToggleSwitch.IsOn ? "1" : "0");
        }

        private string ScheduleId => sp && App.AllItem != null ? App.AllItem.RecordId : null;

        private string GetAlert(string suffix)
        {
            return scheduleSettings.GetAlert(ScheduleId, DetailsEvent.Text, suffix);
        }

        private void SetAlert(string suffix, string value)
        {
            scheduleSettings.SetAlert(ScheduleId, DetailsEvent.Text, suffix, value);
        }

        private async void AlertButton_Click(object sender, RoutedEventArgs e)
        {
            AlertToggleSwitch.IsOn = GetAlert("") == "1";
            AlertToggleSwitch2.IsOn = GetAlert("1") == "1";
            AlertToggleSwitch3.IsOn = GetAlert("3") == "1";
            string personal = GetAlert("Personal");
            if (personal != "0")
            {
                AlertToggleSwitch4.IsOn = true;
                DayComboBox.IsEnabled = true;
                DayComboBox.SelectedIndex = personal == "2" ? 0 : Math.Max(0, Convert.ToInt32(personal) - 3);
            }
            else
            {
                AlertToggleSwitch4.IsOn = false;
                DayComboBox.IsEnabled = false;
            }
            await AlertContentDialog.ShowAsync();
        }

        private void ToastTestButton_Click(object sender, RoutedEventArgs e)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
            {
                new AdaptiveText()
                {
                    Text = "日程到期提醒："+ DetailsEvent.Text
                },
                new AdaptiveText()
                {
                    Text = "备注："+TipText.Text
                }
            }
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
        {
            new ToastButton("打开夏日", "action")
            {
                ActivationType = ToastActivationType.Foreground
            }
        }
                },
                Audio = new ToastAudio()
                {
                    Src = new Uri("ms-winsoundevent:Notification.Looping.Alarm")
                }
            };

            // Create the toast notification
            
            var toastNotif = new ToastNotification(toastContent.GetXml());
            toastNotif.ExpirationTime = DateTime.Now.AddDays(1);
            
            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        private void AlertToggleSwitch2_Toggled(object sender, RoutedEventArgs e)
        {
            if (AlertToggleSwitch2 != null)
                SetAlert("1", AlertToggleSwitch2.IsOn ? "1" : "0");
        }

        private void AlertToggleSwitch3_Toggled(object sender, RoutedEventArgs e)
        {
            if (AlertToggleSwitch3 != null)
                SetAlert("3", AlertToggleSwitch3.IsOn ? "1" : "0");
        }

        private void AlertToggleSwitch4_Toggled(object sender, RoutedEventArgs e)
        {
            if (AlertToggleSwitch4 != null)
            {
                if (AlertToggleSwitch4.IsOn == true)
                {
                    DayComboBox.IsEnabled = true;
                }
                else
                {
                    SetAlert("Personal", "0");
                    DayComboBox.IsEnabled = false;
                }
            }
            else
                AlertToggleSwitch4.IsOn = false;
        }

        private void DayComboBox_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
        {
            
        }

        private void DayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dayNum = e.AddedItems[0].ToString();
            if (AlertToggleSwitch4 != null && AlertToggleSwitch4.IsOn == true)
            {
                SetAlert("Personal", dayNum);
            }
        }
    }
}
