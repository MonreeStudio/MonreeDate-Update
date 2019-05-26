using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class Edit : Page
    {
        public double MinMyNav = MainPage.Current.MyNav.CompactModeThresholdWidth;
        public string EditBirthday_Date;
        public string EditSex_Sex;
        public Edit()
        {
            this.InitializeComponent();
            EditNickName.Text = Settings.Current.PersonalNickName.Text;
            EditSign.Text = Settings.Current.PersonalSign.Text;
            EditSex.Text = Settings.Current.PersonalSex.Text;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditSex_Sex = EditSex.SelectedItem.ToString();
        }

        private void Add_Picker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            string Picker = EditBirthday.Date.ToString();
            DateTime s1 = Convert.ToDateTime(Picker);
            EditBirthday_Date = string.Format("{0}/{1}/{2}", s1.Year, s1.Month, s1.Day);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (EditNickName.Text != null
                && EditSign.Text != null
                && EditBirthday_Date != null
                && EditSex_Sex != null
                && EditNickName.Text != ""
                && EditSign.Text != ""
                && EditSex_Sex != ""
                && EditBirthday_Date != "")
            {
                Settings.Current.PersonalNickName.Text = EditNickName.Text;
                Settings.Current.PersonalSign.Text = EditSign.Text;
                Settings.Current.PersonalBirthday.Text = EditBirthday_Date;
                Settings.Current.PersonalSex.Text = EditSex_Sex;
                Frame.Navigate(typeof(Settings));
                PopupNotice popupNotice = new PopupNotice("个人信息已更新");
                popupNotice.ShowAPopup();
            }
            else
            {
                MessageDialog AboutDialog = new MessageDialog("请确保填入完整的信息！");
                await AboutDialog.ShowAsync();
            }
            
        }
    }
}
