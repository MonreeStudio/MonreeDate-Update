using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.SocialInfo;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
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
    public sealed partial class Feedback : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public double MinMyNav = MainPage.Current.MyNav.CompactModeThresholdWidth;
        private static string address = "monreestudio@outlook.com";
        private string systemInfoJson;
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
            InitFaqText();
            InitVersionText();
        }

        private void InitSystemInfro()
        {
            var sysInfo = new Newtonsoft.Json.Linq.JObject
            {
                ["应用名称"] = ApplicationName,
                ["应用版本号"] = ApplicationVersion,
                ["语言"] = Culture?.Name ?? Culture?.ToString(),
                ["系统架构"] = OperatingSystemArchitecture.ToString(),
                ["系统版本号"] = OperatingSystemVersion.ToString(),
                ["设备类型"] = DeviceFamily?.ToString(),
                ["设备型号"] = DeviceModel?.ToString(),
                ["设备制造商"] = DeviceManufacturer?.ToString(),
            };
            systemInfoJson = sysInfo.ToString(Newtonsoft.Json.Formatting.Indented);
            SysInfoTextBlock.Text = string.Join("\n", sysInfo.Properties().Select(property => $"{property.Name}：{property.Value}"));
        }

        private async void InitFaqText()
        {
            try
            {
                FaqProgressBar.IsActive = true;
                HttpClient client = new HttpClient();
                Uri uri = new Uri("http://www.monreeing.com/notice/get_uwp_notice.xml");
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                string xml = await response.Content.ReadAsStringAsync();
                StringBuilder sb = new StringBuilder();
                if (xml != null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    var grid = doc.FirstChild.ChildNodes;
                    for (var i = 0; i < grid.Count; i++)
                    {
                        var temp = grid[i];
                        if (temp.NodeName == "content")
                            sb.Append(temp.InnerText + "\n\n");
                    }
                }
                FaqText.Text = sb.ToString();
                FaqProgressBar.IsActive = false;
            }
            catch (Exception e)
            {
                PopupNotice popupNotice = new PopupNotice("网络异常： " + e.GetType().ToString());
                popupNotice.ShowAPopup();
            }
        }

        private async void InitVersionText()
        {
            try
            {
                VersionProgressBar.IsActive = true;
                HttpClient client = new HttpClient();
                Uri uri = new Uri("http://www.monreeing.com/notice/monreedate_version_info.xml");
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                string xml = await response.Content.ReadAsStringAsync();
                StringBuilder sb = new StringBuilder();
                if (xml != null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    var grid = doc.FirstChild.ChildNodes;
                    for (var i = 0; i < grid.Count; i++)
                    {
                        var temp = grid[i];
                        if (temp.NodeName == "content")
                            sb.Append(temp.InnerText + "\n\n");
                    }
                }
                VersionText.Text = sb.ToString();
                VersionProgressBar.IsActive = false;
            }
            catch (Exception e)
            {
                PopupNotice popupNotice = new PopupNotice("网络异常： " + e.GetType().ToString());
                popupNotice.ShowAPopup();
            }
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
            await FeedbackAsync("请填写反馈标题", "←请在此处回车换行并输入反馈内容————————\n" + systemInfoJson);
        }
        public static async Task FeedbackAsync(string subject, string body)
        {
            if (address == null)
                return;
            var mailto = new Uri($"mailto:{address}?subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(body)}");
            await Launcher.LaunchUriAsync(mailto);
        }
    }
}
