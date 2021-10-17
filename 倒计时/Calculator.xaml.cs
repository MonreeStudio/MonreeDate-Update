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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 倒计时
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Calculator : Page
    {
        public static Calculator Current;
        public double MinMyNav = MainPage.Current.MyNav.CompactModeThresholdWidth;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public Calculator()
        {
            this.InitializeComponent();
            Current = this;
            SetThemeColor();
            MainPage.Current.MyNav.IsBackEnabled = true;
            MainPage.Current.SelectedPageItem = "Calculator";
            InitCalendarView();
            Str.start = null;
            Str.end = null;
        }

        private void InitCalendarView()
        {
            TermTextBlock.Text = "阳历：\n" + DateTime.Now.ToString("yyyy-MM-dd") + "\n\n阴历：\n" + LunarCalendar.GetChineseDateTime(DateTime.Now);
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

        public class Str
        {
            static public string start;
            static public string end;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Str.start == null || Str.end == null)
            {
                SpanTime.Text = "没有选好日期哦！";
                return;
            }
            try
            {
                string t;
                DateTime d1 = Convert.ToDateTime(Str.start);
                DateTime d2 = Convert.ToDateTime(Str.end);
                DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
                DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));
                int days = (d4 - d3).Days;
                if (d4 > d3)
                    t = App.Term(d3, d4);
                else
                    t = App.Term(d4, d3);
                if (days < 0)
                    days=Math.Abs(days);
                int week, week_date;
                week = days / 7;
                week_date = days % 7;
                if (week_date == 0)
                {
                    if ((days.ToString() + "天") == t)
                    {
                        if (week > 0)
                            SpanTime.Text = "相差" + days.ToString() + "天"
                                            + "\n相差" + week.ToString() + "周";
                        else
                            SpanTime.Text = "相差" + days.ToString() + "天";
                    }
                    else
                    {
                        if (week > 0)
                            SpanTime.Text = "相差" + days.ToString() + "天\n"
                                            + "相差" + t
                                            + "\n相差" + week.ToString() + "周";
                        else
                            SpanTime.Text = "相差" + days.ToString() + "天";
                    }
                }
                    
                else
                {
                    if ((days.ToString() + "天") == t)
                    {
                        if(week > 0)
                            SpanTime.Text = "相差" + t
                                          + "\n相差" + week.ToString() + "周" + week_date.ToString() + "天";
                        else
                            SpanTime.Text = "相差" + t;
                    }
                    else
                    {
                        SpanTime.Text = "相差" + days.ToString() + "天\n"
                                          + "相差" + t
                                          + "\n相差" + week.ToString() + "周" + week_date.ToString() + "天";
                    }
                }
            } 
            catch
            {
                SpanTime.Text = "没有选好日期哦！";
            }
            finally
            {
                ;
            }
        }

        private async void Picker1_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (Picker1.Date == null)
                return;
            string str1 = Picker1.Date.ToString();
            string[] strs = str1.Split(" ");
            foreach (var str in strs)
            {
                if (str.Contains("/"))
                {
                    if (str.Contains("周") || str.Contains("星"))
                    {
                        int index = str.LastIndexOf("/");
                        str1 = str.Substring(0, index - 1);
                    }
                    else
                        str1 = str;
                }
            }
            try
            {
                DateTime s1 = Convert.ToDateTime(str1);
                Str.start = string.Format("{0}/{1}/{2}", s1.Year, s1.Month, s1.Day);
            }
            catch(Exception e)
            {
                MessageDialog AboutDialog = new MessageDialog("日期选择发生错误。\n异常类型：" + e.GetType(), "发生异常");
                await AboutDialog.ShowAsync();
            }
        }

        private async void Picker2_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (Picker2.Date == null)
                return;
            string str2 = Picker2.Date.ToString();
            string[] strs = str2.Split(" ");
            foreach (var str in strs)
            {
                if (str.Contains("/"))
                {
                    if (str.Contains("周") || str.Contains("星"))
                    {
                        int index = str.LastIndexOf("/");
                        str2 = str.Substring(0, index - 1);
                    }
                    else
                        str2 = str;
                }
            }
            try
            {
                DateTime s2 = Convert.ToDateTime(str2);
                Str.end = string.Format("{0}/{1}/{2}", s2.Year, s2.Month, s2.Day);
            }
            catch(Exception e)
            {
                MessageDialog AboutDialog = new MessageDialog("日期选择发生错误。\n异常类型：" + e.GetType(), "发生异常");
                await AboutDialog.ShowAsync();
            }

        }

        private async void TermCalendarView_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            try
            {
                var dateOffset = TermCalendarView.SelectedDates[0];
                var date = dateOffset.ToString("yyyy-MM-dd");
                TermTextBlock.Text = "阳历：\n" + date + "\n\n阴历：\n" + LunarCalendar.GetChineseDateTime(Convert.ToDateTime(date));
            }
            catch (System.Runtime.InteropServices.COMException e0)
            {

            }
            catch(Exception e)
            {
                MessageDialog Dialog = new MessageDialog("日期选择发生错误。\n异常类型：" + e.GetType(), "发生异常");
                await Dialog.ShowAsync();
            }

            //var date = Convert.ToDateTime(dateOffset);
            //

        }
    }
}
