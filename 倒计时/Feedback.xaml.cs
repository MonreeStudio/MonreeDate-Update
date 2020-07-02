using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.SocialInfo;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI;
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
    public sealed partial class Feedback : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public double MinMyNav = MainPage.Current.MyNav.CompactModeThresholdWidth;
        private static string address = "monreestudio@outlook.com";
        public string ApplicationName => SystemInformation.ApplicationName;
        public string ApplicationVersion => $"{SystemInformation.ApplicationVersion.Major}.{SystemInformation.ApplicationVersion.Minor}.{SystemInformation.ApplicationVersion.Build}.{SystemInformation.ApplicationVersion.Revision}";
        public CultureInfo Culture => SystemInformation.Culture;
        public ProcessorArchitecture OperatingSystemArchitecture => SystemInformation.OperatingSystemArchitecture;
        public OSVersion OperatingSystemVersion => SystemInformation.OperatingSystemVersion;
        public string DeviceFamily => SystemInformation.DeviceFamily;
        public string DeviceModel => SystemInformation.DeviceModel;
        public string DeviceManufacturer => SystemInformation.DeviceManufacturer;
        public PackageVersion FirstVersionInstalled => SystemInformation.FirstVersionInstalled;
        public long TotalLaunchCount => SystemInformation.TotalLaunchCount;

        public Feedback()
        {
            this.InitializeComponent();
            MainPage.Current.MyNav.IsBackEnabled = true;
            MainPage.Current.SelectedPageItem = "Feedback";
            MainPage.Current.AllItem.IsSelected = false;
            MainPage.Current.DesktopItem.IsSelected = false;
            MainPage.Current.NewItem.IsSelected = false;
            MainPage.Current.CalculatorItem.IsSelected = false;
            MainPage.Current.FestivalItem.IsSelected = false;
            MainPage.Current.TimerItem.IsSelected = false;
            InitSystemInfro();
            SetThemeColor();
        }

        private void InitSystemInfro()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("基本信息\n应用名称：" + ApplicationName
                + "\n应用版本号：" + ApplicationVersion
                + "\n语言：" + Culture
                + "\n系统架构：" + OperatingSystemArchitecture
                + "\n系统版本号：" + OperatingSystemVersion
                + "\n设备类型：" + DeviceFamily
                + "\n设备型号：" + DeviceModel
                + "\n设备制造商：" + DeviceManufacturer
                );
            SysInfoTextBlock.Text = sb.ToString();
        }

        public void SetThemeColor()
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

        private async void SendMailButton_Click(object sender, RoutedEventArgs e)
        {
            await FeedbackAsync("请填写反馈标题", "←请在此处回车换行并输入反馈内容————————" + SysInfoTextBlock.Text);
        }
        public static async Task FeedbackAsync(string subject, string body)
        {
            if (address == null)
                return;
            var mailto = new Uri($"mailto:{address}?subject={subject}&body={body}");
            await Launcher.LaunchUriAsync(mailto);
        }
    }
}
