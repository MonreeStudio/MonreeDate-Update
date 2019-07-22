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
    public sealed partial class Details : Page
    {
        public Details()
        {
            this.InitializeComponent();
            //this.NavigationCacheMode = NavigationCacheMode.Enabled;
            DetailsPickedDate.Text = All.Current.str1;
            DetailsEvent.Text = All.Current.str3;
            DetailsDate.Text = All.Current.str2;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            //if (e.Parameter is string && !string.IsNullOrWhiteSpace((string)e.Parameter))
            //{
            //    InitialText.Text = $"Hi, {e.Parameter.ToString()}";
            //}
            //else
            //{
            //    InitialText.Text = "Hi!";
            //}
            //base.OnNavigatedTo(e);
        }

        private void DetailsDate_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DateTime d1 = Convert.ToDateTime(All.Current.str3);
            DateTime d2 = DateTime.Now;
            DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
            DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));

            if (DetailsDate.Text == All.Current.str2)
            {
                if (d4 > d3)
                    DetailsDate.Text = "已过" + App.term(d3, d4);
                else
                {
                    if (d4 < d3)
                        DetailsDate.Text = "还有" + App.term(d4, d3);
                }
            }
            else
                DetailsDate.Text = All.Current.str2;
        }
    }
}
