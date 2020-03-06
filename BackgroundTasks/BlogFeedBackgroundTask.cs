﻿using Microsoft.Toolkit.Uwp.Notifications;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.Web.Syndication;
using Microsoft.QueryStringDotNET; // QueryString.NET
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml;
using Windows.UI.ViewManagement;
using Windows.UI.Popups;

namespace BackgroundTasks
{
    public sealed class BlogFeedBackgroundTask : IBackgroundTask
    {
        static string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "mydb.sqlite");    //建立数据库  
        static SQLite.Net.SQLiteConnection conn;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            //建立数据库连接   
            conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), path);
            //建表              
            conn.CreateTable<DataTemple>(); //默认表名同范型参数    
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();  // 如果没有用到异步任务就不需要Defferal
            UpdateTile();   //更新磁贴
            LoadToast();    //加载通知
            deferral.Complete();
        }

        public async void CreateTool()
        {
            var num = CoreApplication.Views.Count();
            //if (localSettings.Values["newViewId"] != null)
            //    ApplicationViewSwitcher.SwitchAsync(Convert.ToInt32(localSettings.Values["newViewId"])).Close();

            //建立数据库连接   
            conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), path);
            //建表              
            conn.CreateTable<DataTemple>(); //默认表名同范型参数    
            int count = (int)localSettings.Values["ItemCount"];
            List<DataTemple> datalist = new List<DataTemple>();
            var allData = conn.Query<DataTemple>("select *from DataTemple");
            switch (count)
            {
                case 1:
                    string a1 = localSettings.Values["DesktopKey0"].ToString();
                    foreach (var item in allData)
                    {
                        if (item.Schedule_name == a1)
                            datalist.Add(item);
                    }
                    break;
                case 2:
                    string b1 = localSettings.Values["DesktopKey0"].ToString();
                    string b2 = localSettings.Values["DesktopKey1"].ToString();
                    foreach (var item in allData)
                    {
                        if (item.Schedule_name == b1 || item.Schedule_name == b2)
                            datalist.Add(item);
                    }
                    break;
                case 3:
                    string c1 = localSettings.Values["DesktopKey0"].ToString();
                    string c2 = localSettings.Values["DesktopKey1"].ToString();
                    string c3 = localSettings.Values["DesktopKey2"].ToString();
                    foreach (var item in allData)
                    {
                        if (item.Schedule_name == c1 || item.Schedule_name == c2 || item.Schedule_name == c3)
                            datalist.Add(item);
                    }
                    break;
                default:
                    return;
            }
            //var num = CoreApplication.Views.Count();
            //coreWindow.Activate();
            //CoreApplication.GetCurrentView();
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(Tool), datalist, new SuppressNavigationTransitionInfo());
                Window.Current.Content = frame;
                Window.Current.Activate();
                newViewId = ApplicationView.GetForCurrentView().Id;
                localSettings.Values["newViewId"] = newViewId;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            localSettings.Values["DesktopPin"] = false;
        }
        

        public static void UpdateTile()
        {
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();
            if(LoadTile()!=null)
                updater.Update(LoadTile());
        }
        public static string Calculator(string s1)
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
            {
                if (days != 0)
                    s2 = "已过" + days.ToString() + "天";
                else
                {
                    s2 = "就在今天";
                }
            }
            return s2;
        }

        private static TileNotification LoadTile()
        {
            List<DataTemple> datalist0 = conn.Query<DataTemple>("select * from DataTemple");
            foreach (var item in datalist0)
            {
                if (SecondaryTile.Exists(item.Schedule_name))
                {
                    CreateSecondaryTile(item.Schedule_name, Calculator(item.Date), item.Date);
                }
            }
            List<DataTemple> datalist = conn.Query<DataTemple>("select * from DataTemple where Date >= ? order by Date asc limit 1", DateTime.Now.ToString("yyyy-MM-dd"));
            string _ScheduleName = "";
            string _CaculatedDate = "";
            string _Date = "";

            foreach (var item in datalist)
            {
                _ScheduleName = item.Schedule_name;
                _CaculatedDate = Calculator(item.Date);
                _Date = item.Date;

            }
            if (_ScheduleName != "" && _CaculatedDate != "" && _Date != "")
            {
                var tile = CreateTile(_ScheduleName, _CaculatedDate, _Date);
                return tile;
            }
            else
                return null;
        }

        private static TileNotification CreateTile(string _ScheduleName, string _CaculatedDate, string _Date)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            // 测试磁贴
            string from = _ScheduleName;
            string subject = _CaculatedDate;
            string body = _Date;

            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    DisplayName = "夏日",
                    TileMedium = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            TextStacking = TileTextStacking.Center,
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = from,
                                    HintStyle = AdaptiveTextStyle.Base,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = subject,
                                    HintStyle = AdaptiveTextStyle.Body,
                                    HintAlign = AdaptiveTextAlign.Center
                                }
                            }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = from,
                                        HintStyle = AdaptiveTextStyle.Base
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = subject,
                                        HintStyle = AdaptiveTextStyle.Subtitle
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = body
                                    }
                                },
                                HintTextStacking = AdaptiveSubgroupTextStacking.Center
                            }
                        }
                    }
                }
                        }
                    },

                    TileLarge = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = from,
                                        HintStyle = AdaptiveTextStyle.Subtitle
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = subject,
                                        HintStyle = AdaptiveTextStyle.Title
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = body,
                                        HintStyle = AdaptiveTextStyle.BodySubtle
                                    }
                                }
                            }
                        }
                    },
                    //new AdaptiveText()
                    //{
                    //    Text = ""
                    //},
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = "不要因为繁忙而忘记\n生活",
                                        HintStyle = AdaptiveTextStyle.Subtitle
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = "脚踏实地，仰望星空。\n永远相信美好的事情即将发生！",
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    }
                                    //new AdaptiveText()
                                    //{
                                    //    Text = "永远相信美好的事情即将发生！",
                                    //    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    //}
                                }
                            }
                        }
                    }
                }
                        }
                    }
                }
            };
            var notification = new TileNotification(content.GetXml());
            return notification;
        }

        private static void CreateSecondaryTile(string _ScheduleName, string _CaculatedDate, string _Date)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            // 测试磁贴
            string from = _ScheduleName;
            string subject = _CaculatedDate;
            string body = _Date;

            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    DisplayName = "夏日",
                    TileMedium = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            TextStacking = TileTextStacking.Center,
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = from,
                                    HintStyle = AdaptiveTextStyle.Base,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = subject,
                                    HintStyle = AdaptiveTextStyle.Body,
                                    HintAlign = AdaptiveTextAlign.Center
                                }
                            }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = from,
                                        HintStyle = AdaptiveTextStyle.Base
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = subject,
                                        HintStyle = AdaptiveTextStyle.Subtitle
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = body
                                    }
                                },
                                HintTextStacking = AdaptiveSubgroupTextStacking.Center
                            }
                        }
                    }
                }
                        }
                    },

                    TileLarge = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = from,
                                        HintStyle = AdaptiveTextStyle.Base
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = subject,
                                        HintStyle = AdaptiveTextStyle.Title
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = body,
                                        HintStyle = AdaptiveTextStyle.BodySubtle
                                    }
                                }
                            }
                        }
                    },
                    //new AdaptiveText()
                    //{
                    //    Text = ""
                    //},
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = "不要因为繁忙而忘记\n生活",
                                        HintStyle = AdaptiveTextStyle.Subtitle
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = "脚踏实地，仰望星空。\n永远相信美好的事情即将发生！",
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    }
                                    //new AdaptiveText()
                                    //{
                                    //    Text = "永远相信美好的事情即将发生！",
                                    //    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                    //}
                                }
                            }
                        }
                    }
                }
                        }
                    }
                }
            };
            var notification = new TileNotification(content.GetXml());
            if (SecondaryTile.Exists(_ScheduleName))
            {
                var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(_ScheduleName);

                updater.Update(notification);
            }
        }

        public void LoadToast()
        {
            List<DataTemple> datalist0 = conn.Query<DataTemple>("select * from DataTemple where Date = ?", DateTime.Now.ToString("yyyy-MM-dd"));
            foreach (var item in datalist0)
            {
                var AlertName = "Alert" + item.Schedule_name;
                if (localSettings.Values[AlertName] != null && localSettings.Values[AlertName].ToString()=="1")
                {
                    string tipText;
                    var tipTextContainer = localSettings.Values[item.Schedule_name + item.Date];
                    if (tipTextContainer != null && tipTextContainer.ToString() != "")
                        tipText = "日程备注：" + tipTextContainer.ToString();
                    else
                        tipText = "日程备注：无备注。";
                    CreateToast(item.Schedule_name, tipText);
                    localSettings.Values[AlertName] = "0";
                }
            }
        }

        private void CreateToast(string Schedule_name,string tipText)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
            {
                new AdaptiveText()
                {
                    Text = "日程到期提醒："+ Schedule_name
                },
                new AdaptiveText()
                {
                    Text = tipText
                }
            }
                    }
                }, 
                Actions = new ToastActionsCustom()
                {
                    Buttons =
        {
            new ToastButton("打开夏日", "action")
            {
                ActivationType = ToastActivationType.Foreground
            }
        }
                },
                Audio = new ToastAudio()
                {
                    Src = new Uri("ms-winsoundevent:Notification.Looping.Alarm")
                }
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());
            toastNotif.ExpirationTime = DateTime.Now.AddDays(1);
            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }
    }
}