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
            sp = MainPage.Current.SelectedPage;
            if (sp == true)
            {
                DetailsPickedDate.Text = App.AllItem.Str3;
                DetailsEvent.Text = App.AllItem.Str1;
                DetailsDate.Text = App.AllItem.Str2;
                DetailsGrid.Background = App.AllItem.Str4;
                DetailsDate.Foreground = new SolidColorBrush(App.AllItem.BackGroundColor);

            }
            else
            {
                EditButton.IsEnabled = false;
                DetailsEvent.Text = App.FestivalItem.Str1;
                DetailsDate.Text = App.FestivalItem.Str2;
                DetailsPickedDate.Text = App.FestivalItem.Str3;
                DetailsGrid.Background =  ColorfulBrush(App.FestivalItem.Str4);
                DetailsDate.Foreground = new SolidColorBrush(App.FestivalItem.Str4);
            }
        }

        public static AcrylicBrush ColorfulBrush(Color temp)
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

        }

        private void DetailsDate_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (MainPage.Current.SelectedPage)
            {
                
                DateTime d1 = Convert.ToDateTime(All.Current.str3);
                DateTime d2 = DateTime.Now;
                DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
                DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));

                if (DetailsDate.Text == CustomData.Calculator(All.Current.str3))
                {
                    if (d4 > d3)
                        DetailsDate.Text = "已过" + App.Term(d3, d4);
                    else
                    {
                        if (d4 < d3)
                            DetailsDate.Text = "还有" + App.Term(d4, d3);
                    }
                }
                else
                    DetailsDate.Text = CustomData.Calculator(All.Current.str3);
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
                        DetailsDate.Text = "已过" + App.Term(d3, d4);
                    else
                    {
                        if (d4 < d3)
                            DetailsDate.Text = "还有" + App.Term(d4, d3);
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
