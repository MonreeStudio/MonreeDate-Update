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
using 倒计时.Manager;

namespace BackgroundTasks
{
    public sealed class BlogFeedBackgroundTask : IBackgroundTask
    {
        static string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "mydb.sqlite");    //建立数据库  
        SQLite.Net.SQLiteConnection conn;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            try
            {
                conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), path);
                conn.RunInTransaction(() =>
                {
                    if (TableExists("DataTemplate") && !TableHasColumn("DataTemplate", "Id"))
                    {
                        MigrateLegacyDataTemplateTable();
                    }

                    conn.CreateTable<CountdownRecord>();
                    EnsureScheduleColumns();
                    MigrateLegacyDataTempleTable();
                    EnsureIndexes();
                });
                var scheduleSettings = new CountdownSettingsService(localSettings);
                scheduleSettings.MigrateLegacySettings(conn.Query<CountdownRecord>("select * from DataTemplate")
                    .Select(item => new ScheduleIdentity(item.Id, item.Schedule_name, item.Date)));
                UpdateTile();
                LoadToast();
            }
            finally
            {
                if (conn != null)
                {
                    conn.Dispose();
                    conn = null;
                }
                deferral.Complete();
            }
        }

        private void EnsureScheduleColumns()
        {
            if (!TableHasColumn("DataTemplate", "ScheduleType"))
            {
                conn.Execute("alter table DataTemplate add column ScheduleType text");
            }
            if (!TableHasColumn("DataTemplate", "EndDate"))
            {
                conn.Execute("alter table DataTemplate add column EndDate text");
            }

            conn.Execute("update DataTemplate set ScheduleType = 'Point' where ScheduleType is null or trim(ScheduleType) = ''");
            conn.Execute("update DataTemplate set EndDate = Date where ScheduleType = 'Point' or EndDate is null or trim(EndDate) = ''");
        }

        private void EnsureIndexes()
        {
            conn.Execute("create index if not exists IX_DataTemplate_Date on DataTemplate(Date)");
            conn.Execute("create index if not exists IX_DataTemplate_IsTop_Date on DataTemplate(IsTop, Date)");
            conn.Execute("create index if not exists IX_DataTemplate_EndDate on DataTemplate(EndDate)");
        }

        private bool TableHasColumn(string tableName, string columnName)
        {
            var columns = conn.Query<TableInfo>("pragma table_info(" + tableName + ")");
            return columns.Any(column => string.Equals(column.name, columnName, StringComparison.OrdinalIgnoreCase));
        }

        private bool TableExists(string tableName)
        {
            return conn.ExecuteScalar<int>(
                "select count(*) from sqlite_master where type = 'table' and name = ?", tableName) > 0;
        }

        private void MigrateLegacyDataTemplateTable()
        {
            // Preserve the original table, even when it is empty; a failed copy
            // rolls back both the rename and inserts in the enclosing transaction.
            var legacyItems = conn.Query<LegacyDataTemplate>("select * from DataTemplate");
            conn.Execute("alter table DataTemplate rename to DataTemplate_Backup_" + Guid.NewGuid().ToString("N"));
            conn.CreateTable<CountdownRecord>();

            foreach (var legacyItem in legacyItems)
            {
                conn.Insert(legacyItem.ToCountdownRecord());
            }
        }

        private void MigrateLegacyDataTempleTable()
        {
            if (!TableExists("DataTemple"))
            {
                return;
            }
            var legacyItems = conn.Query<LegacyDataTemple>("select * from DataTemple");

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
            // Retain the source while preventing deleted schedules from being
            // restored again when a later foreground or background run starts.
            conn.Execute("alter table DataTemple rename to DataTemple_Backup_" + Guid.NewGuid().ToString("N"));
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
            return ScheduleDateCalculator.FormatCountdown(s1, s1, "Point");
        }

        private TileNotification LoadTile()
        {
            List<CountdownRecord> datalist0 = conn.Query<CountdownRecord>("select * from DataTemplate");
            foreach (var item in datalist0)
            {
                if (SecondaryTile.Exists(CountdownSettingsService.GetSecondaryTileId(item.Id))
                    || SecondaryTile.Exists(item.Schedule_name))
                {
                    CreateSecondaryTile(
                        item.Id,
                        item.Schedule_name,
                        ScheduleDateCalculator.FormatCountdown(item.Date, item.EndDate, item.ScheduleType),
                        item.Date,
                        ScheduleDateCalculator.FormatDates(item.Date, item.EndDate, item.ScheduleType));
                }
            }
            var nextItem = conn.Query<CountdownRecord>(
                "select * from DataTemplate where (case when ScheduleType = 'Range' and EndDate is not null and EndDate <> '' then EndDate else Date end) >= ? order by Date asc limit 1",
                DateTime.Today.ToString("yyyy-MM-dd")).FirstOrDefault();
            if (nextItem == null)
            {
                return null;
            }

            return CreateTile(
                nextItem.Id,
                nextItem.Schedule_name,
                ScheduleDateCalculator.FormatCountdown(nextItem.Date, nextItem.EndDate, nextItem.ScheduleType),
                nextItem.Date,
                ScheduleDateCalculator.FormatDates(nextItem.Date, nextItem.EndDate, nextItem.ScheduleType));
        }

        private TileNotification CreateTile(string _ScheduleId, string _ScheduleName, string _CaculatedDate, string _Date, string dateText)
        {
            // 测试磁贴
            string from = _ScheduleName;
            string subject = _CaculatedDate;
            string body = dateText;
            string displayName;
            if (localSettings.Values["TileTip"] != null && localSettings.Values["TileTip"].ToString() == "1")
            {
                var note = new CountdownSettingsService(localSettings).GetNote(_ScheduleId, _ScheduleName, _Date);
                if (string.IsNullOrEmpty(note))
                    displayName = "无备注";
                else
                    displayName = note;
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

        private void CreateSecondaryTile(string _ScheduleId, string _ScheduleName, string _CaculatedDate, string _Date, string dateText)
        {
            // 测试磁贴
            string from = _ScheduleName;
            string subject = _CaculatedDate;
            string body = dateText;
            string displayName;
            if (localSettings.Values["TileTip"] != null && localSettings.Values["TileTip"].ToString() == "1")
            {
                var note = new CountdownSettingsService(localSettings).GetNote(_ScheduleId, _ScheduleName, _Date);
                if (string.IsNullOrEmpty(note))
                    displayName = "无备注";
                else
                    displayName = note;
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
            string tileId = CountdownSettingsService.GetSecondaryTileId(_ScheduleId);
            if (!SecondaryTile.Exists(tileId) && SecondaryTile.Exists(_ScheduleName))
                tileId = _ScheduleName;
            if (SecondaryTile.Exists(tileId))
            {
                var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileId);

                updater.Update(notification);
            }
        }

        public void LoadToast()
        {
            var scheduleSettings = new CountdownSettingsService(localSettings);
            List<CountdownRecord> datalist0 = conn.Query<CountdownRecord>("select * from DataTemplate where Date = ?", DateTime.Now.ToString("yyyy-MM-dd"));
            foreach (var item in datalist0)
            {
                if (scheduleSettings.GetAlert(item.Id, item.Schedule_name, "") == "1")
                {
                    string tipText;
                    var tipTextContainer = scheduleSettings.GetNote(item.Id, item.Schedule_name, item.Date);
                    if (!string.IsNullOrEmpty(tipTextContainer))
                        tipText = "日程备注：" + tipTextContainer;
                    else
                        tipText = "日程备注：无备注。";
                    CreateToast(item, tipText, 0);
                    scheduleSettings.SetAlert(item.Id, item.Schedule_name, "", "0");
                }               
            }
            for (int i = 1; i <= 3; i += 2)
            {
                string temp = DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd");
                var datalist1 = conn.Query<CountdownRecord>("select * from DataTemplate where Date = ?", DateTime.Now.AddDays(i).ToString("yyyy-MM-dd"));
                foreach(var item in datalist1)
                {
                    var AlertName = i.ToString();
                    if(scheduleSettings.GetAlert(item.Id, item.Schedule_name, AlertName) == "1")
                    {
                        string tipText;
                        var tipTextContainer = scheduleSettings.GetNote(item.Id, item.Schedule_name, item.Date);
                        if (!string.IsNullOrEmpty(tipTextContainer))
                            tipText = "日程备注：" + tipTextContainer;
                        else
                            tipText = "日程备注：无备注。";
                        CreateToast(item, tipText, i);
                        scheduleSettings.SetAlert(item.Id, item.Schedule_name, AlertName, "0");
                    }
                }
            }
            var datalist2 = conn.Query<CountdownRecord>("select * from DataTemplate");
            foreach(var item in datalist2)
            {
                var AlertName = "Personal";
                string personalAlert = scheduleSettings.GetAlert(item.Id, item.Schedule_name, AlertName);
                if(personalAlert != "0")
                {
                    string tipText;
                    string dayNumString = personalAlert;
                    int dayNum = Convert.ToInt32(dayNumString);
                    if(item.Date == DateTime.Now.AddDays(dayNum).ToString("yyyy-MM-dd"))
                    {
                        var tipTextContainer = scheduleSettings.GetNote(item.Id, item.Schedule_name, item.Date);
                        if (!string.IsNullOrEmpty(tipTextContainer))
                            tipText = "日程备注：" + tipTextContainer;
                        else
                            tipText = "日程备注：无备注。";
                        CreateToast(item, tipText, dayNum);
                        scheduleSettings.SetAlert(item.Id, item.Schedule_name, AlertName, "0");
                    }
                }
            }
        }

        private void CreateToast(CountdownRecord item, string tipText, int dayNum)
        {
            bool isRange = ScheduleDateCalculator.IsRange(item.ScheduleType, item.EndDate);
            string text;
            if(dayNum == 0)
            {
                text = (isRange ? "日程开始提醒：" : "日程到期提醒：") + item.Schedule_name;
            }
            else
            {
                text = item.Schedule_name + "，还有" + dayNum + "天" + (isRange ? "开始" : "");
            }
            if (isRange)
            {
                tipText = ScheduleDateCalculator.FormatDates(item.Date, item.EndDate, item.ScheduleType)
                    + "\n" + tipText;
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
