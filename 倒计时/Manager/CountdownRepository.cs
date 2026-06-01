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
            if (!TableHasColumn("DataTemplate", "Id"))
            {
                MigrateLegacyDataTemplateTable();
            }

            connection.CreateTable<CountdownRecord>();
            MigrateLegacyDataTempleTable();
            EnsureIndexes();
        }

        private void EnsureIndexes()
        {
            connection.Execute("create index if not exists IX_DataTemplate_Date on DataTemplate(Date)");
            connection.Execute("create index if not exists IX_DataTemplate_IsTop_Date on DataTemplate(IsTop, Date)");
        }

        private bool TableHasColumn(string tableName, string columnName)
        {
            try
            {
                var columns = connection.Query<TableInfo>("pragma table_info(" + tableName + ")");
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
                legacyItems = connection.Query<LegacyDataTemplate>("select * from DataTemplate");
            }
            catch
            {
                return;
            }

            if (legacyItems.Count == 0)
            {
                return;
            }

            connection.Execute("drop table if exists DataTemplate");
            connection.CreateTable<CountdownRecord>();

            foreach (var legacyItem in legacyItems)
            {
                connection.Insert(legacyItem.ToCountdownRecord());
            }
        }

        private void MigrateLegacyDataTempleTable()
        {
            List<LegacyDataTemple> legacyItems;

            try
            {
                legacyItems = connection.Query<LegacyDataTemple>("select * from DataTemple");
            }
            catch
            {
                return;
            }

            if (legacyItems.Count == 0)
            {
                return;
            }

            foreach (var legacyItem in legacyItems)
            {
                var existingItems = connection.Query<CountdownRecord>(
                    "select * from DataTemplate where Schedule_name = ?",
                    legacyItem.Schedule_name);

                if (existingItems.Count > 0)
                {
                    continue;
                }

                connection.Insert(legacyItem.ToCountdownRecord());
            }
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

        public List<CountdownRecord> GetPinned()
        {
            return connection.Query<CountdownRecord>(
                "select * from DataTemplate where IsTop = ?",
                "1");
        }

        public List<CountdownRecord> GetPinnedPast(string today)
        {
            return connection.Query<CountdownRecord>(
                "select * from DataTemplate where IsTop = ? and Date < ? order by AddTime desc",
                "1",
                today);
        }

        public List<CountdownRecord> GetPinnedFuture(string today)
        {
            return connection.Query<CountdownRecord>(
                "select * from DataTemplate where IsTop = ? and Date >= ? order by AddTime desc",
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
                "select * from DataTemplate where Date >= ? order by Date asc",
                today);
        }

        public List<CountdownRecord> GetPast(string today)
        {
            return connection.Query<CountdownRecord>(
                "select * from DataTemplate where Date < ? order by Date desc",
                today);
        }

        public List<CountdownRecord> GetUnorderedFuture(string today)
        {
            return connection.Query<CountdownRecord>(
                "select * from DataTemplate where Date >= ?",
                today);
        }

        public List<CountdownRecord> GetUnorderedPast(string today)
        {
            return connection.Query<CountdownRecord>(
                "select * from DataTemplate where Date < ?",
                today);
        }

        public List<CountdownRecord> GetByDate(string date)
        {
            return connection.Query<CountdownRecord>(
                "select * from DataTemplate where Date = ?",
                date);
        }

        public CountdownRecord GetNextFuture(string today)
        {
            var items = connection.Query<CountdownRecord>(
                "select * from DataTemplate where Date >= ? order by Date asc limit 1",
                today);

            if (items.Count == 0)
            {
                return null;
            }

            return items[0];
        }

        public void Insert(CountdownRecord countdown)
        {
            EnsureRecordId(countdown);
            connection.Insert(countdown);
        }

        public void DeleteByName(string scheduleName)
        {
            connection.Execute(
                "delete from DataTemplate where Schedule_name = ?",
                scheduleName);
        }

        public void ReplaceByName(string scheduleName, CountdownRecord countdown)
        {
            DeleteByName(scheduleName);
            Insert(countdown);
        }

        public void SetPinned(string scheduleName, bool isPinned)
        {
            var items = GetByName(scheduleName);
            DeleteByName(scheduleName);

            foreach (var item in items)
            {
                item.IsTop = isPinned ? "1" : "0";
                item.AddTime = isPinned ? DateTime.Now.ToString() : "";
                Insert(item);
            }
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
