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
    public sealed partial class Festival : Page
    {
        public double MinMyNav = MainPage.Current.MyNav.CompactModeThresholdWidth;
        public static Festival Current;
        string str1, str2, str3;
        public Festival()
        {
            this.InitializeComponent();
            //this.NavigationCacheMode = NavigationCacheMode.Enabled;
            Current = this;

            Date1.Text = Calculator(StartName1.Text);
            Date2.Text = Calculator(StartName2.Text);
            Date3.Text = Calculator(StartName3.Text);
            Date4.Text = Calculator(StartName4.Text);
            Date5.Text = Calculator(StartName5.Text);
            Date6.Text = Calculator(StartName6.Text);
            Date7.Text = Calculator(StartName7.Text);
            Date8.Text = Calculator(StartName8.Text);
            Date9.Text = Calculator(StartName9.Text);
            Date10.Text = Calculator(StartName10.Text);
            Date11.Text = Calculator(StartName11.Text);
            Date12.Text = Calculator(StartName12.Text);
            Date13.Text = Calculator(StartName13.Text);
            Date14.Text = Calculator(StartName14.Text);
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
                s2 = "已过" + days.ToString() + "天";
            return s2;
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var _item = (CustomData)e.ClickedItem;
            str1 = _item.Str1;
            str2 = _item.Str2;
            str3 = _item.Str3;

            Frame.Navigate(typeof(Details));
        }
    }
}
