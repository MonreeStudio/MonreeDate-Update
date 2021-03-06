﻿using System;
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
    public sealed partial class Edit : Page
    {
        public double MinMyNav = MainPage.Current.MyNav.CompactModeThresholdWidth;
        public string EditBirthday_Date;
        public string EditSex_Sex;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public Edit()
        {
            this.InitializeComponent();
            SetThemeColor();
            EditNickName.Text = Settings.Current.PersonalNickName.Text;
            EditSign.Text = Settings.Current.PersonalSign.Text;
            if(Settings.Current.PersonalSex.Text != "未选择")
            {
                if (Settings.Current.PersonalSex.Text == "男")
                    EditSex.SelectedIndex = 0;
                else
                    EditSex.SelectedIndex = 1;
            }
            if(Settings.Current.PersonalBirthday.Text!="未设置")
                EditBirthday.Date = Convert.ToDateTime(Settings.Current.PersonalBirthday.Text);
            MainPage.Current.MyNav.IsBackEnabled = true;
            MainPage.Current.SelectedPageItem = "Edit";
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditSex_Sex = EditSex.SelectedItem.ToString();
        }

        private async void Add_Picker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (EditBirthday.Date == null)
                return;
            string Picker = EditBirthday.Date.ToString();
            string[] strs = Picker.Split(" ");
            foreach (var str in strs)
            {
                if (str.Contains("/"))
                {
                    if(str.Contains("周") || str.Contains("星"))
                    {
                        int index = str.LastIndexOf("/");
                        Picker = str.Substring(0, index - 1);
                    }
                    else
                        Picker = str;
                }
            }
            try
            {
                DateTime s1 = Convert.ToDateTime(Picker);
                EditBirthday_Date = string.Format("{0}/{1}/{2}", s1.Year, s1.Month, s1.Day);
            }
            catch(Exception e)
            {
                MessageDialog AboutDialog = new MessageDialog("日期选择发生错误。\n异常类型：" + e.GetType(), "发生异常");
                await AboutDialog.ShowAsync();
            }
            
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
                localSettings.Values["NickName"] = EditNickName.Text;
                localSettings.Values["Sign"] = EditSign.Text;
                localSettings.Values["PersonalSex"] = EditSex_Sex;
                localSettings.Values["BirthDay_Date"] = EditBirthday_Date;
                Frame.Navigate(typeof(Settings));
                Settings.Current.ReadSettings();
                PopupNotice popupNotice = new PopupNotice("个人信息已更新");
                popupNotice.ShowAPopup();
            }
            else
            {
                MessageDialog AboutDialog = new MessageDialog("请确保填入完整的信息！","提示");
                await AboutDialog.ShowAsync();
            }
            
        }
    }
}
