using Microsoft.Toolkit.Uwp.Notifications;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
            if (!TableHasColumn("DataTemplate", "Id"))
            {
                MigrateLegacyDataTemplateTable();
            }

            conn.CreateTable<CountdownRecord>(); //默认表名同范型参数
            MigrateLegacyDataTempleTable();
            EnsureIndexes();
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();  // 如果没有用到异步任务就不需要Defferal
            UpdateTile();   //更新磁贴
            LoadToast();    //加载通知
            deferral.Complete();
        }

        private void EnsureIndexes()
        {
            conn.Execute("create index if not exists IX_DataTemplate_Date on DataTemplate(Date)");
            conn.Execute("create index if not exists IX_DataTemplate_IsTop_Date on DataTemplate(IsTop, Date)");
        }

        private bool TableHasColumn(string tableName, string columnName)
        {
            try
            {
                var columns = conn.Query<TableInfo>("pragma table_info(" + tableName + ")");
                return columns.Any(column => string.Equals(column.name, columnName, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        private void MigrateLegacyDataTemplateTable()
        {
            List<LegacyDataTemplate> legacyItems;

            try
            {
                legacyItems = conn.Query<LegacyDataTemplate>("select * from DataTemplate");
            }
            catch
            {
                return;
            }

            if (legacyItems.Count == 0)
            {
                return;
            }

            conn.Execute("drop table if exists DataTemplate");
            conn.CreateTable<CountdownRecord>();

            foreach (var legacyItem in legacyItems)
            {
                conn.Insert(legacyItem.ToCountdownRecord());
            }
        }

        private void MigrateLegacyDataTempleTable()
        {
            List<LegacyDataTemple> legacyItems;

            try
            {
                legacyItems = conn.Query<LegacyDataTemple>("select * from DataTemple");
            }
            catch
            {
                return;
            }

            foreach (var legacyItem in legacyItems)
            {
                var existingItems = conn.Query<CountdownRecord>(
                    "select * from DataTemplate where Schedule_name = ?",
                    legacyItem.Schedule_name);

                if (existingItems.Count > 0)
                {
                    continue;
                }

                conn.Insert(legacyItem.ToCountdownRecord());
            }
        }

        public void UpdateTile()
        {
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            TileNotification tileNotification = LoadTile();
            if(tileNotification != null)
            {
                updater.Update(tileNotification);
            }
            else
            {
                updater.Clear();
            }
        }
        public static string Calculator(string s1)
        {
            DateTime targetDate = ParseDate(s1);
            DateTime today = DateTime.Today;
            int days = (today - targetDate).Days;

            if (days < 0)
            {
                return "还有" + Math.Abs(days) + "天";
            }

            if (days == 0)
            {
                return "就在今天";
            }

            return "已过" + days + "天";
        }

        private static DateTime ParseDate(string date)
        {
            DateTime parsedDate;
            if (DateTime.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out parsedDate)
                || DateTime.TryParse(date, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out parsedDate))
            {
                return parsedDate.Date;
            }

            return Convert.ToDateTime(date).Date;
        }

        private TileNotification LoadTile()
        {
            List<CountdownRecord> datalist0 = conn.Query<CountdownRecord>("select * from DataTemplate");
            foreach (var item in datalist0)
            {
                if (SecondaryTile.Exists(item.Schedule_name))
                {
                    CreateSecondaryTile(item.Schedule_name, Calculator(item.Date), item.Date);
                }
            }
            List<CountdownRecord> datalist = conn.Query<CountdownRecord>("select * from DataTemplate where Date >= ? order by Date asc limit 1", DateTime.Now.ToString("yyyy-MM-dd"));
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

        private TileNotification CreateTile(string _ScheduleName, string _CaculatedDate, string _Date)
        {
            // 测试磁贴
            string from = _ScheduleName;
            string subject = _CaculatedDate;
            string body = _Date;
            string displayName;
            if (localSettings.Values["TileTip"] != null && localSettings.Values["TileTip"].ToString() == "1")
            {
                if (localSettings.Values[_ScheduleName + _Date] == null)
                    displayName = "无备注";
                else
                    displayName = localSettings.Values[_ScheduleName + _Date].ToString();
            }
            else
                displayName = "夏日";
            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    DisplayName = displayName,
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

        private void CreateSecondaryTile(string _ScheduleName, string _CaculatedDate, string _Date)
        {
            // 测试磁贴
            string from = _ScheduleName;
            string subject = _CaculatedDate;
            string body = _Date;
            string displayName;
            if (localSettings.Values["TileTip"] != null && localSettings.Values["TileTip"].ToString() == "1")
            {
                if (localSettings.Values[_ScheduleName + _Date] == null)
                    displayName = "无备注";
                else
                    displayName = localSettings.Values[_ScheduleName + _Date].ToString();
            }
            else
                displayName = "夏日";
            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    DisplayName = displayName,
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
            List<CountdownRecord> datalist0 = conn.Query<CountdownRecord>("select * from DataTemplate where Date = ?", DateTime.Now.ToString("yyyy-MM-dd"));
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
                    CreateToast(item.Schedule_name, tipText, 0);
                    localSettings.Values[AlertName] = "0";
                }               
            }
            for (int i = 1; i <= 3; i += 2)
            {
                string temp = DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd");
                var datalist1 = conn.Query<CountdownRecord>("select * from DataTemplate where Date = ?", DateTime.Now.AddDays(i).ToString("yyyy-MM-dd"));
                foreach(var item in datalist1)
                {
                    var AlertName = "Alert" + item.Schedule_name + i;
                    if(localSettings.Values[AlertName]!=null && localSettings.Values[AlertName].ToString() == "1")
                    {
                        string tipText;
                        var tipTextContainer = localSettings.Values[item.Schedule_name + item.Date];
                        if (tipTextContainer != null && tipTextContainer.ToString() != "")
                            tipText = "日程备注：" + tipTextContainer.ToString();
                        else
                            tipText = "日程备注：无备注。";
                        CreateToast(item.Schedule_name, tipText, i);
                        localSettings.Values[AlertName] = "0";
                    }
                }
            }
            var datalist2 = conn.Query<CountdownRecord>("select * from DataTemplate");
            foreach(var item in datalist2)
            {
                var AlertName = "Alert" + item.Schedule_name + "Personal";
                
                if(localSettings.Values[AlertName]!=null && localSettings.Values[AlertName].ToString() != "0")
                {
                    string tipText;
                    string dayNumString = localSettings.Values[AlertName].ToString();
                    int dayNum = Convert.ToInt32(dayNumString);
                    if(item.Date == DateTime.Now.AddDays(dayNum).ToString("yyyy-MM-dd"))
                    {
                        var tipTextContainer = localSettings.Values[item.Schedule_name + item.Date];
                        if (tipTextContainer != null && tipTextContainer.ToString() != "")
                            tipText = "日程备注：" + tipTextContainer.ToString();
                        else
                            tipText = "日程备注：无备注。";
                        CreateToast(item.Schedule_name, tipText, dayNum);
                        localSettings.Values[AlertName] = "0";
                    }
                }
            }
        }

        private void CreateToast(string Schedule_name,string tipText,int dayNum)
        {
            string text;
            if(dayNum == 0)
            {
                text = "日程到期提醒：" + Schedule_name;
            }
            else
            {
                text = Schedule_name + "，还有" + dayNum + "天";
            }
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
                    Text = text
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

        private class TableInfo
        {
            public string name { get; set; }
        }
    }
}
