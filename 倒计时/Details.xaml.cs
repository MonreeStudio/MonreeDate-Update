using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
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
        public Details()
        {
            this.InitializeComponent();
            Current = this;
            //this.NavigationCacheMode = NavigationCacheMode.Enabled;
            sp = MainPage.Current.SelectedPage;
            if (sp == true)
            {
                EditButton.IsEnabled = true;
                DetailsPickedDate.Text = All.Current.str3;
                DetailsEvent.Text = All.Current.str1;
                DetailsDate.Text = All.Current.str2;
                DetailsGrid.Background = All.Current.str4;
                DetailsDate.Foreground = new SolidColorBrush(All.Current.BgsColor);
            }
            else
            {
                EditButton.IsEnabled = false;
                DetailsEvent.Text = Festival.Current.str1;
                DetailsDate.Text = Festival.Current.str2;
                DetailsPickedDate.Text = Festival.Current.str3;
                DetailsGrid.Background =  ColorfulBrush(Festival.Current.str4);
                DetailsDate.Foreground = new SolidColorBrush(Festival.Current.str4);
            }
        }

        public AcrylicBrush ColorfulBrush(Color temp)
        {
            AcrylicBrush myBrush = new AcrylicBrush();
            myBrush.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
            myBrush.TintColor = temp;
            myBrush.TintColor = temp;
            myBrush.FallbackColor = temp;
            myBrush.TintOpacity = 0.8;
            return myBrush;
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
            if (MainPage.Current.SelectedPage)
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
            else
            {
                DateTime d1 = Convert.ToDateTime(Festival.Current.str3);
                DateTime d2 = DateTime.Now;
                DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
                DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));

                if (DetailsDate.Text == Festival.Current.str2)
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
                    DetailsDate.Text = Festival.Current.str2;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EditDetails));
        }
    }
}
