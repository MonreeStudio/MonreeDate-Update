using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;
using 夏日.Models;

namespace 倒计时.Manager
{
    public class CountdownRepository
    {
        private const string DatabaseFileName = "mydb.sqlite";
        // A range remains active through its inclusive end date.
        private const string EffectiveEndDate = "(case when ScheduleType = 'Range' and EndDate is not null and EndDate <> '' then EndDate else Date end)";
        private readonly SQLiteConnection connection;

        public CountdownRepository()
            : this(CreateConnection())
        {
        }

        public CountdownRepository(SQLiteConnection connection)
        {
            this.connection = connection;
            EnsureTables();
        }

        public SQLiteConnection Connection => connection;

        private static SQLiteConnection CreateConnection()
        {
            string databasePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DatabaseFileName);
            return new SQLiteConnection(new SQLitePlatformWinRT(), databasePath);
        }

        private void EnsureTables()
        {
            connection.RunInTransaction(() =>
            {
                if (TableExists("DataTemplate") && !TableHasColumn("DataTemplate", "Id"))
                {
                    MigrateLegacyDataTemplateTable();
                }

                connection.CreateTable<CountdownRecord>();
                EnsureRangeColumns();
                MigrateLegacyDataTempleTable();
                EnsureIndexes();
            });
        }

        private void EnsureRangeColumns()
        {
            if (!TableHasColumn("DataTemplate", "ScheduleType"))
            {
                connection.Execute("alter table DataTemplate add column ScheduleType text");
            }
            if (!TableHasColumn("DataTemplate", "EndDate"))
            {
                connection.Execute("alter table DataTemplate add column EndDate text");
            }
            connection.Execute("update DataTemplate set ScheduleType = 'Point' where ScheduleType is null or trim(ScheduleType) = ''");
            connection.Execute("update DataTemplate set EndDate = Date where ScheduleType = 'Point' or EndDate is null or trim(EndDate) = ''");
        }

        private void EnsureIndexes()
        {
            connection.Execute("create index if not exists IX_DataTemplate_Date on DataTemplate(Date)");
            connection.Execute("create index if not exists IX_DataTemplate_IsTop_Date on DataTemplate(IsTop, Date)");
        }

        private bool TableHasColumn(string tableName, string columnName)
        {
            var columns = connection.Query<TableInfo>("pragma table_info(" + tableName + ")");
            return columns.Any(column => string.Equals(column.name, columnName, StringComparison.OrdinalIgnoreCase));
        }

        private bool TableExists(string tableName)
        {
            return connection.ExecuteScalar<int>(
                "select count(*) from sqlite_master where type = 'table' and name = ?", tableName) > 0;
        }

        private void MigrateLegacyDataTemplateTable()
        {
            // Keep the original table as a backup, including empty legacy tables.
            // The outer transaction rolls back the rename if copying any row fails.
            var legacyItems = connection.Query<LegacyDataTemplate>("select * from DataTemplate");
            string backupTable = "DataTemplate_Backup_" + Guid.NewGuid().ToString("N");
            connection.Execute("drop index if exists IX_DataTemplate_Date");
            connection.Execute("drop index if exists IX_DataTemplate_IsTop_Date");
            connection.Execute("alter table DataTemplate rename to " + backupTable);
            connection.CreateTable<CountdownRecord>();

            foreach (var legacyItem in legacyItems)
            {
                connection.Insert(CreateLegacyRecord(legacyItem.Schedule_name, legacyItem.CalculatedDate,
                    legacyItem.Date, legacyItem.BgColor, legacyItem.TintOpacity, legacyItem.IsTop, legacyItem.AddTime,
                    legacyItem.ScheduleType, legacyItem.EndDate));
            }
        }

        private void MigrateLegacyDataTempleTable()
        {
            if (!TableExists("DataTemple"))
            {
                return;
            }
            var legacyItems = connection.Query<LegacyDataTemple>("select * from DataTemple");
            foreach (var legacyItem in legacyItems)
            {
                var existingItems = connection.Query<CountdownRecord>(
                    "select * from DataTemplate where Schedule_name = ?",
                    legacyItem.Schedule_name);

                if (existingItems.Count > 0)
                {
                    continue;
                }

                connection.Insert(CreateLegacyRecord(legacyItem.Schedule_name, legacyItem.CalculatedDate,
                    legacyItem.Date, legacyItem.BgColor, legacyItem.TintOpacity, legacyItem.IsTop, legacyItem.AddTime,
                    legacyItem.ScheduleType, legacyItem.EndDate));
            }
            // Retain a backup while preventing deleted schedules from being reimported on restart.
            connection.Execute("alter table DataTemple rename to DataTemple_Backup_" + Guid.NewGuid().ToString("N"));
        }

        private static CountdownRecord CreateLegacyRecord(string name, string countdown, string date,
            string color, double opacity, string isTop, string addTime, string scheduleType, string endDate)
        {
            // Migration preserves old values verbatim; validation applies to future writes.
            bool isRange = ScheduleDateCalculator.IsRange(scheduleType, endDate);
            return new CountdownRecord
            {
                Id = Guid.NewGuid().ToString(), Schedule_name = name, CalculatedDate = countdown,
                Date = date, ScheduleType = isRange ? "Range" : "Point", EndDate = isRange ? endDate : date, BgColor = color,
                TintOpacity = opacity, IsTop = isTop, AddTime = addTime
            };
        }

        public List<CountdownRecord> GetAll()
        {
            return connection.Query<CountdownRecord>("select * from DataTemplate");
        }

        public List<CountdownRecord> GetAllOrderByDate()
        {
            return connection.Query<CountdownRecord>("select * from DataTemplate order by Date asc");
        }

        public List<CountdownRecord> GetByName(string scheduleName)
        {
            return connection.Query<CountdownRecord>(
                "select * from DataTemplate where Schedule_name = ?",
                scheduleName);
        }

        public CountdownRecord GetById(string id)
        {
            return connection.Query<CountdownRecord>("select * from DataTemplate where Id = ?", id).FirstOrDefault();
        }

        public List<CountdownRecord> GetPinned()
        {
            return connection.Query<CountdownRecord>(
                "select * from DataTemplate where IsTop = ?",
                "1");
        }

        public List<CountdownRecord> GetPinnedPast(string today)
        {
            return connection.Query<CountdownRecord>(
                "select * from DataTemplate where IsTop = ? and " + EffectiveEndDate + " < ? order by AddTime desc",
                "1",
                today);
        }

        public List<CountdownRecord> GetPinnedFuture(string today)
        {
            return connection.Query<CountdownRecord>(
                "select * from DataTemplate where IsTop = ? and " + EffectiveEndDate + " >= ? order by AddTime desc",
                "1",
                today);
        }

        public List<CountdownRecord> GetPinnedOrderByAddTime()
        {
            return connection.Query<CountdownRecord>(
                "select * from DataTemplate where IsTop = ? order by AddTime desc",
                "1");
        }

        public List<CountdownRecord> GetFuture(string today)
        {
            return connection.Query<CountdownRecord>(
                "select * from DataTemplate where " + EffectiveEndDate + " >= ? order by Date asc",
                today);
        }

        public List<CountdownRecord> GetPast(string today)
        {
            return connection.Query<CountdownRecord>(
                "select * from DataTemplate where " + EffectiveEndDate + " < ? order by " + EffectiveEndDate + " desc",
                today);
        }

        public List<CountdownRecord> GetUnorderedFuture(string today)
        {
            return connection.Query<CountdownRecord>(
                "select * from DataTemplate where " + EffectiveEndDate + " >= ?",
                today);
        }

        public List<CountdownRecord> GetUnorderedPast(string today)
        {
            return connection.Query<CountdownRecord>(
                "select * from DataTemplate where " + EffectiveEndDate + " < ?",
                today);
        }

        public List<CountdownRecord> GetByDate(string date)
        {
            return connection.Query<CountdownRecord>(
                "select * from DataTemplate where Date <= ? and " + EffectiveEndDate + " >= ?",
                date, date);
        }

        public CountdownRecord GetNextFuture(string today)
        {
            var items = connection.Query<CountdownRecord>(
                "select * from DataTemplate where " + EffectiveEndDate + " >= ? order by Date asc limit 1",
                today);

            if (items.Count == 0)
            {
                return null;
            }

            return items[0];
        }

        public void Insert(CountdownRecord countdown)
        {
            if (countdown == null) throw new ArgumentNullException(nameof(countdown));
            countdown.NormalizeAndValidate();
            EnsureRecordId(countdown);
            connection.Insert(countdown);
        }

        public void Update(CountdownRecord countdown)
        {
            if (countdown == null) throw new ArgumentNullException(nameof(countdown));
            if (string.IsNullOrWhiteSpace(countdown.Id)) throw new ArgumentException("日程 ID 不能为空。", nameof(countdown));
            countdown.NormalizeAndValidate();
            if (connection.Update(countdown) == 0)
            {
                throw new InvalidOperationException("该日程已不存在，请刷新后重试。");
            }
        }

        public void DeleteById(string id)
        {
            connection.Execute("delete from DataTemplate where Id = ?", id);
        }

        public void DeleteByName(string scheduleName)
        {
            connection.Execute(
                "delete from DataTemplate where Schedule_name = ?",
                scheduleName);
        }

        public void ReplaceByName(string scheduleName, CountdownRecord countdown)
        {
            if (countdown == null) throw new ArgumentNullException(nameof(countdown));
            countdown.NormalizeAndValidate();
            connection.RunInTransaction(() =>
            {
                var existing = GetByName(scheduleName).FirstOrDefault();
                if (existing == null)
                {
                    Insert(countdown);
                }
                else
                {
                    countdown.Id = existing.Id;
                    Update(countdown);
                }
            });
        }

        public void SetPinned(string scheduleName, bool isPinned)
        {
            connection.Execute("update DataTemplate set IsTop = ?, AddTime = ? where Schedule_name = ?",
                isPinned ? "1" : "0", isPinned ? DateTime.Now.ToString() : "", scheduleName);
        }

        public void SetPinnedById(string id, bool isPinned)
        {
            connection.Execute("update DataTemplate set IsTop = ?, AddTime = ? where Id = ?",
                isPinned ? "1" : "0", isPinned ? DateTime.Now.ToString() : "", id);
        }

        private static void EnsureRecordId(CountdownRecord countdown)
        {
            if (string.IsNullOrWhiteSpace(countdown.Id))
            {
                countdown.Id = Guid.NewGuid().ToString();
            }
        }

        private class TableInfo
        {
            public string name { get; set; }
        }
    }
}
