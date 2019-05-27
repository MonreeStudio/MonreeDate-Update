using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        public Calculator()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            Current = this;
        }

        public class Str
        {
            static public string start;
            static public string end;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string t;
                DateTime d1 = Convert.ToDateTime(Str.start);
                DateTime d2 = Convert.ToDateTime(Str.end);
                DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
                DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));
                int days = (d4 - d3).Days;
                if (d4 > d3)
                    t = App.term(d3, d4);
                else
                    t = App.term(d4, d3);
                if (days < 0)
                    days=Math.Abs(days);
                int week, week_date;
                week = days / 7;
                week_date = days % 7;
                if (week_date == 0)
                    SpanTime.Text = "相差" + days.ToString() + "天\n"
                                   + "相差" +t 
                                    + "\n相差" + week.ToString() + "周";
                else
                {
                    if ((days.ToString() + "天") == t)
                    {
                        SpanTime.Text = "相差" + t 
                                          + "\n相差" + week.ToString() + "周" + week_date.ToString() + "天";
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
                SpanTime.Text = "你输错了！";
            }
            finally
            {
                ;
            }
        }

        private void Picker1_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            string str1 = Picker1.Date.ToString();
            DateTime s1 = Convert.ToDateTime(str1);
            Str.start = string.Format("{0}/{1}/{2}", s1.Year, s1.Month, s1.Day);
        }

        private void Picker2_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            string str2 = Picker2.Date.ToString();
            DateTime s2 = Convert.ToDateTime(str2);
            Str.end = string.Format("{0}/{1}/{2}", s2.Year, s2.Month, s2.Day);
        }
    }
}
