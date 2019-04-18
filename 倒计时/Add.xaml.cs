using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class Add : Page
    {
        public string _PickDate;
        public Add()
        {  
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void Add_Picker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            string Picker = Add_Picker.Date.ToString();
            DateTime s1 = Convert.ToDateTime(Picker);
            _PickDate = string.Format("{0}/{1}/{2}", s1.Year, s1.Month, s1.Day);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //All.Current.AllPageStackPanel.Background = new SolidColorBrush(Colors.Black);
            //All.Current.TopText.Text = "为什么没有用啊？";

            //MainPage.Current.MyNav.Header = "夏日";
            string _event = AddEvent.Text;
            Frame.Navigate(typeof(All), _event);
        }
    }
}
