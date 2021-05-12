using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class LoginPage : Page
    {
        private DispatcherTimer timer;
        private int getCodeMinute;
        private string signUpJson;
        private string loginJson;
        private string getVCodeJson;
        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public LoginPage()
        {
            this.InitializeComponent();
            MainPage.Current.MyNav.IsBackEnabled = true;
            MainPage.Current.SelectedPageItem = "LoginPage";
            LoginGrid.Visibility = Visibility.Visible;
            SignUpGrid.Visibility = Visibility.Collapsed;

        }

        private void GoToSignUp_Click(object sender, RoutedEventArgs e)
        {
            LoginGrid.Visibility = Visibility.Collapsed;
            SignUpGrid.Visibility = Visibility.Visible;
        }

        private void GoToLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginGrid.Visibility = Visibility.Visible;
            SignUpGrid.Visibility = Visibility.Collapsed;
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserNameTextBox0.Text.Equals(""))
            {
                PopupNotice popupNotice = new PopupNotice("邮箱不得为空");
                popupNotice.ShowAPopup();
                return;
            }
            if (!IsEmail(UserNameTextBox0.Text))
            {
                PopupNotice popupNotice = new PopupNotice("邮箱格式错误");
                popupNotice.ShowAPopup();
                return;
            }
            if (PasswordPB0.Password.Equals(""))
            {
                PopupNotice popupNotice = new PopupNotice("密码不得为空");
                popupNotice.ShowAPopup();
                return;
            }
            if(PasswordPB0.Password.Length >= 8 && PasswordPB0.Password.Length <= 17)
            {
                PopupNotice popupNotice = new PopupNotice("密码长度须大于7位少于18位");
                popupNotice.ShowAPopup();
                return;
            }
            if(!RePasswordPB.Password.Equals(PasswordPB0.Password))
            {
                PopupNotice popupNotice = new PopupNotice("两次输入密码不一致");
                popupNotice.ShowAPopup();
                return;
            }
            if (VCodeTextBox.Text.Equals(""))
            {
                PopupNotice popupNotice = new PopupNotice("验证码不得为空");
                popupNotice.ShowAPopup();
                return;
            }
            try
            {
                signUpJson = "{\"User\": {\"UserName\": \"" + UserNameTextBox0.Text + "\",\"Password\": \"" + PasswordPB0.Password + "\",\"Code\": \"" + VCodeTextBox.Text + "\"}}";
                ThreadStart threadStart = new ThreadStart(DoSignUp);
                Thread thread = new Thread(threadStart);
                thread.Start();

            }
            catch (Exception ex)
            {
                LoginPageProgressBar.IsActive = false;
                PopupNotice popupNotice = new PopupNotice("服务器错误：" + ex.Message);
                popupNotice.ShowAPopup();

            }
        }

        private void DoSignUp()
        {
            try
            {
                this.Invoke(() =>
                {
                    LoginPageProgressBar.IsActive = true;
                });
                HttpClient client = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://msdate.monreeing.com:3000/user/signup/");
                requestMessage.Content = new StringContent(signUpJson);
                requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = client.SendAsync(requestMessage).GetAwaiter().GetResult();

                if (response.StatusCode.ToString() == "OK")
                {
                    string result = response.Content.ReadAsStringAsync().Result.ToString();
                    JObject jo = JObject.Parse(result);
                    string responseCode = jo["Code"].ToString();
                    string responseMsg = jo["Message"].ToString();
                    if (responseCode.Equals("0"))
                    {
                        this.Invoke(() =>
                        {
                            PopupNotice popupNotice = new PopupNotice("注册成功！");
                            popupNotice.ShowAPopup();
                            LoginGrid.Visibility = Visibility.Collapsed;
                            SignUpGrid.Visibility = Visibility.Visible;
                        });
                    }
                    else
                    {
                        this.Invoke(() =>
                        {
                            PopupNotice popupNotice = new PopupNotice("注册失败：" + responseMsg);
                            popupNotice.ShowAPopup();
                        });
                    }
                }
                else
                {
                    this.Invoke(() =>
                    {
                        PopupNotice popupNotice = new PopupNotice("注册失败：服务器异常");
                        popupNotice.ShowAPopup();
                    });
                }
                this.Invoke(() =>
                {
                    LoginPageProgressBar.IsActive = false;
                });
            } 
            catch (Exception e)
            {
                this.Invoke(() =>
                {
                    LoginPageProgressBar.IsActive = false;
                    PopupNotice popupNotice = new PopupNotice("注册失败：" + e.Message);
                    popupNotice.ShowAPopup();
                });
            }
        }

        private void GetVCodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserNameTextBox0.Text.Equals(""))
            {
                PopupNotice popupNotice = new PopupNotice("邮箱不得为空");
                popupNotice.ShowAPopup();
                return;
            }
            if(!IsEmail(UserNameTextBox0.Text))
            {
                PopupNotice popupNotice = new PopupNotice("邮箱格式错误");
                popupNotice.ShowAPopup();
                return;
            }
            try
            {
                // 请求服务器发送验证码
                getVCodeJson = "{\"Email\":\"" + UserNameTextBox0.Text + "\"}";
                ThreadStart threadStart = new ThreadStart(DoGetVCode);
                Thread thread = new Thread(threadStart);
                thread.Start();
            }
            catch (Exception ex)
            {
                PopupNotice popupNotice = new PopupNotice("服务器错误：" + ex.Message);
                popupNotice.ShowAPopup();
            }
            getCodeMinute = 60;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;//每秒触发这个事件，以刷新指针
            timer.Start();
            GetVCodeButton.Visibility = Visibility.Collapsed;
            ReSendTextBlock.Visibility = Visibility.Visible;
        }

        public async void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
        }

        public void DoGetVCode()
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://msdate.monreeing.com:3000/email/signup_code/");
                requestMessage.Content = new StringContent(getVCodeJson);
                requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = client.SendAsync(requestMessage).GetAwaiter().GetResult();

                if (response.StatusCode.ToString() == "OK")
                {
                    string r = response.Content.ReadAsStringAsync().Result.ToString();
                    //Console.WriteLine(r);
                }
                else
                {
                    this.Invoke(() =>
                    {
                        PopupNotice popupNotice = new PopupNotice("服务器错误：未知原因");
                        popupNotice.ShowAPopup();
                    });
                    
                }
            }
            catch (Exception e)
            {
                this.Invoke(() =>
                {
                    PopupNotice popupNotice = new PopupNotice("获取验证码失败：" + e.Message);
                    popupNotice.ShowAPopup();
                });
            }
        }
        public bool IsEmail(string inputData)
        {
            Regex RegEmail = new Regex("^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$");
            Match m = RegEmail.Match(inputData);
            return m.Success;
        }

        private void Timer_Tick(object sender, object e)
        {
            if(getCodeMinute == 0)
            {
                timer.Stop();
                GetVCodeButton.Visibility = Visibility.Visible;
                ReSendTextBlock.Visibility = Visibility.Collapsed;
            }
            ReSendTextBlock.Text = "重新获取（" + --getCodeMinute + "s)";
        }

        class User
        {
            public string Email { get; set; }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserNameTextBox1.Text.Equals(""))
            {
                PopupNotice popupNotice = new PopupNotice("邮箱不得为空");
                popupNotice.ShowAPopup();
                return;
            }
            if (!IsEmail(UserNameTextBox1.Text))
            {
                PopupNotice popupNotice = new PopupNotice("邮箱格式错误");
                popupNotice.ShowAPopup();
                return;
            }
            if (PasswordPB1.Password.Equals(""))
            {
                PopupNotice popupNotice = new PopupNotice("密码不得为空");
                popupNotice.ShowAPopup();
                return;
            }
            if (!(bool)PolicyCheckBox.IsChecked)
            {
                PopupNotice popupNotice = new PopupNotice("请阅读并同意隐私协议");
                popupNotice.ShowAPopup();
                return;
            }
            try
            {
                loginJson = "{\"User\": {\"UserName\": \"" + UserNameTextBox1.Text + "\",\"Password\": \"" + PasswordPB1.Password + "\"}}";
                ThreadStart threadStart = new ThreadStart(DoLogin);
                Thread thread = new Thread(threadStart);
                thread.Start();

            }
            catch (Exception ex)
            {
                PopupNotice popupNotice = new PopupNotice("服务器错误：" + ex.Message);
                popupNotice.ShowAPopup();
            }
        }

        private void DoLogin()
        {
            try
            {
                this.Invoke(() =>
                {
                    LoginPageProgressBar.IsActive = true;
                });
                HttpClient client = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://msdate.monreeing.com:3000/user/login/");
                requestMessage.Content = new StringContent(loginJson);
                requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = client.SendAsync(requestMessage).GetAwaiter().GetResult();

                if (response.StatusCode.ToString() == "OK")
                {
                    string result = response.Content.ReadAsStringAsync().Result.ToString();
                    JObject jo = JObject.Parse(result);
                    if (jo["Code"].ToString().Equals("0"))
                    {
                        string userInfo = jo["User"].ToString();
                        JObject userInfoJson = JObject.Parse(userInfo);
                        string token = userInfoJson["Token"].ToString();
                        string timeStamp = userInfoJson["TimeStamp"].ToString();
                        localSettings.Values["Token"] = token;
                        localSettings.Values["TimeStamp"] = timeStamp;
                        localSettings.Values["UserName"] = userInfoJson["UserName"].ToString();
                        this.Invoke(() =>
                        {
                            PopupNotice popupNotice = new PopupNotice("登录成功");
                            popupNotice.ShowAPopup();
                            Frame.GoBack();
                            Settings.Current.SignOutButton.Visibility = Visibility.Visible;
                            Settings.Current.SyncButton.Visibility = Visibility.Visible;
                            Settings.Current.LoginButton.Visibility = Visibility.Collapsed;
                        });
                    }
                    else
                    {
                        this.Invoke(() =>
                        {
                            string errorMsg = jo["Message"].ToString();
                            PopupNotice popupNotice = new PopupNotice("登录失败" + errorMsg);
                            popupNotice.ShowAPopup();
                        });
                    }
                }
                else
                {
                    this.Invoke(() =>
                    {
                        PopupNotice popupNotice = new PopupNotice("服务器错误：未知原因");
                        popupNotice.ShowAPopup();
                    });
                }
                this.Invoke(() =>
                {
                    LoginPageProgressBar.IsActive = false;
                });
            }
            catch (Exception e)
            {
                this.Invoke(() =>
                {
                    LoginPageProgressBar.IsActive = false;
                    PopupNotice popupNotice = new PopupNotice("登录失败：" + e.Message);
                    popupNotice.ShowAPopup();
                });
            }
        }
    }
}
