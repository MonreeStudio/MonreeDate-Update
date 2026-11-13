using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using 夏日.Models;

namespace 倒计时.Manager
{
    /// <summary>Owns Windows tile operations and the shared foreground/background tile template.</summary>
    internal sealed class CountdownTileService
    {
        private readonly CountdownService schedules;
        private readonly CountdownSettingsService settings;
        private Task initialization;

        public CountdownTileService(CountdownService schedules, CountdownSettingsService settings)
        {
            this.schedules = schedules ?? throw new ArgumentNullException(nameof(schedules));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task InitializeAsync()
        {
            if (initialization == null) initialization = BindExistingTilesAsync();
            try { await initialization; }
            catch { initialization = null; throw; }
        }

        private async Task BindExistingTilesAsync()
        {
            var tiles = await SecondaryTile.FindAllAsync();
            // A legacy name tile gets one deterministic owner; renaming never changes that owner.
            var identities = schedules.GetAll().OrderBy(record => record.Id, StringComparer.Ordinal)
                .Select(record => new ScheduleIdentity(record.Id, record.Schedule_name));
            settings.MigrateLegacyTiles(identities, tiles.Select(tile => tile.TileId));
        }

        public bool IsPinned(string scheduleId)
        {
            return settings.GetSecondaryTileIds(scheduleId).Any(SecondaryTile.Exists);
        }

        public async Task RefreshAllAsync()
        {
            await InitializeAsync();
            foreach (var record in schedules.GetAll()) UpdateSecondaryContent(record);
            RefreshMainTile();
        }

        public async Task RefreshSecondaryTilesAsync()
        {
            await InitializeAsync();
            foreach (var record in schedules.GetAll()) UpdateSecondaryContent(record);
        }

        public void RefreshMainTile()
        {
            var record = schedules.GetNextFuture(DateTime.Today.ToString("yyyy-MM-dd"));
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            if (record == null)
            {
                updater.Clear();
                settings.SetMainTileId(string.Empty);
                return;
            }
            updater.Update(CreateNotification(record));
            settings.SetMainTileId(record.Id, record.Schedule_name);
            // Becoming the main tile never removes a user-pinned secondary tile.
        }

        public async Task<bool> PinAsync(string scheduleId)
        {
            await InitializeAsync();
            var record = RequireRecord(scheduleId);
            if (IsPinned(record.Id)) return true;
            var tile = new SecondaryTile(CountdownSettingsService.GetSecondaryTileId(record.Id),
                "夏日：" + record.Schedule_name, TileArguments(record.Id),
                new Uri("ms-appx:///Assets/Square150x150Logo.scale-150.png"), Windows.UI.StartScreen.TileSize.Default);
            tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.scale-400.png");
            tile.VisualElements.Square310x310Logo = new Uri("ms-appx:///Assets/LargeTile.scale-400.png");
            tile.VisualElements.Square44x44Logo = new Uri("ms-appx:///Assets/Square44x44Logo.scale-400.png");
            bool accepted = await tile.RequestCreateAsync();
            if (accepted) UpdateSecondaryContent(record);
            settings.SetTileState(record.Id, record.Schedule_name, IsPinned(record.Id));
            return accepted;
        }

        /// <summary>Only removes IDs owned by this record. A cancelled Windows prompt stops deletion.</summary>
        public async Task<bool> UnpinAsync(string scheduleId)
        {
            await InitializeAsync();
            var record = RequireRecord(scheduleId);
            foreach (string tileId in settings.GetSecondaryTileIds(record.Id).Distinct(StringComparer.Ordinal))
            {
                if (!SecondaryTile.Exists(tileId)) continue;
                if (!await new SecondaryTile(tileId).RequestDeleteAsync())
                {
                    settings.SetTileState(record.Id, record.Schedule_name, IsPinned(record.Id));
                    return false;
                }
            }
            settings.SetTileState(record.Id, record.Schedule_name, false);
            return true;
        }

        /// <summary>Refreshes names/content after an edit without deleting or recreating existing tiles.</summary>
        public async Task UpdateScheduleAsync(string scheduleId)
        {
            await InitializeAsync();
            var record = RequireRecord(scheduleId);
            foreach (string tileId in settings.GetSecondaryTileIds(record.Id).Distinct(StringComparer.Ordinal))
            {
                if (!SecondaryTile.Exists(tileId)) continue;
                var tile = new SecondaryTile(tileId)
                {
                    DisplayName = "夏日：" + record.Schedule_name,
                    Arguments = TileArguments(record.Id)
                };
                if (!await tile.UpdateAsync()) throw new InvalidOperationException("磁贴名称未能更新，请重试。");
            }
            UpdateSecondaryContent(record);
            RefreshMainTile();
        }

        private CountdownRecord RequireRecord(string scheduleId)
        {
            return schedules.GetById(scheduleId) ?? throw new InvalidOperationException("该日程已不存在，请刷新后重试。");
        }

        private static string TileArguments(string id)
        {
            return "action=viewSchedule&scheduleId=" + Uri.EscapeDataString(id);
        }

        private void UpdateSecondaryContent(CountdownRecord record)
        {
            foreach (string tileId in settings.GetSecondaryTileIds(record.Id).Distinct(StringComparer.Ordinal))
            {
                if (SecondaryTile.Exists(tileId))
                    TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileId).Update(CreateNotification(record));
            }
        }

        private TileNotification CreateNotification(CountdownRecord record)
        {
            string brandingName = "夏日";
            if (ApplicationData.Current.LocalSettings.Values["TileTip"]?.ToString() == "1")
            {
                brandingName = settings.GetNote(record.Id, record.Schedule_name, record.Date);
                if (string.IsNullOrWhiteSpace(brandingName)) brandingName = "无备注";
            }
            return new TileNotification(BuildTileContent(record.Schedule_name,
                ScheduleDateCalculator.FormatCountdown(record.Date, record.EndDate, record.ScheduleType),
                ScheduleDateCalculator.FormatDates(record.Date, record.EndDate, record.ScheduleType), brandingName).GetXml());
        }

        /// <summary>Reusable by the background task; preserves the existing adaptive tile appearance.</summary>
        public static TileContent BuildTileContent(string name, string countdown, string dates, string brandingName)
        {
            return new TileContent
            {
                Visual = new TileVisual
                {
                    DisplayName = brandingName,
                    TileMedium = new TileBinding
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive
                        {
                            TextStacking = TileTextStacking.Center,
                            Children =
                            {
                                new AdaptiveText { Text = name, HintStyle = AdaptiveTextStyle.Base, HintAlign = AdaptiveTextAlign.Center },
                                new AdaptiveText { Text = countdown, HintStyle = AdaptiveTextStyle.Body, HintAlign = AdaptiveTextAlign.Center }
                            }
                        }
                    },
                    TileWide = new TileBinding
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive
                        {
                            Children =
                            {
                                new AdaptiveGroup
                                {
                                    Children =
                                    {
                                        new AdaptiveSubgroup
                                        {
                                            HintTextStacking = AdaptiveSubgroupTextStacking.Center,
                                            Children =
                                            {
                                                new AdaptiveText { Text = name, HintStyle = AdaptiveTextStyle.Base },
                                                new AdaptiveText { Text = countdown, HintStyle = AdaptiveTextStyle.Subtitle },
                                                new AdaptiveText { Text = dates }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    TileLarge = new TileBinding
                    {
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive
                        {
                            Children =
                            {
                                new AdaptiveGroup
                                {
                                    Children =
                                    {
                                        new AdaptiveSubgroup
                                        {
                                            Children =
                                            {
                                                new AdaptiveText { Text = name, HintStyle = AdaptiveTextStyle.Subtitle },
                                                new AdaptiveText { Text = countdown, HintStyle = AdaptiveTextStyle.Title },
                                                new AdaptiveText { Text = dates, HintStyle = AdaptiveTextStyle.BodySubtle }
                                            }
                                        }
                                    }
                                },
                                new AdaptiveGroup
                                {
                                    Children =
                                    {
                                        new AdaptiveSubgroup
                                        {
                                            Children =
                                            {
                                                new AdaptiveText { Text = "不要因为繁忙而忘记\n生活", HintStyle = AdaptiveTextStyle.Subtitle },
                                                new AdaptiveText { Text = "脚踏实地，仰望星空。\n永远相信美好的事情即将发生！", HintStyle = AdaptiveTextStyle.CaptionSubtle }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
