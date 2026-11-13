using System;
using System.IO;
using System.Linq;
using SQLite.Net;
using SQLite.Net.Platform.Generic;
using 倒计时.Manager;
using 夏日.Models;

// The production repository is compiled unchanged. Only its unused UWP default
// connection factory is adapted; all migration/query tests use real SQLite.
namespace Windows.Storage
{
    public class ApplicationData
    {
        public static ApplicationData Current => throw new NotSupportedException();
        public StorageFolder LocalFolder => throw new NotSupportedException();
        public ApplicationDataContainer LocalSettings => throw new NotSupportedException();
    }
    public class StorageFolder { public string Path => throw new NotSupportedException(); }
    public class ApplicationDataContainer
    {
        public System.Collections.Generic.IDictionary<string, object> Values { get; } =
            new System.Collections.Generic.Dictionary<string, object>();
    }
}
namespace SQLite.Net.Platform.WinRT
{
    public class SQLitePlatformWinRT : SQLitePlatformGeneric
    {
    }
}

internal static class ScheduleTests
{
    private static int assertions;
    private static readonly DateTime Today = new DateTime(2026, 9, 15);

    private static void Main()
    {
        DateBoundaries();
        HomePresentation();
        WithDatabase(TestServiceSettingsById);
        LegacySettingsMigrateOnce();
        WithDatabase(TestCurrentSchemaMigrationAndQueries);
        WithDatabase(connection => TestLegacyMigration(connection, false));
        WithDatabase(connection => TestLegacyMigration(connection, true));
        WithDatabase(TestOldSpellingMigration);
        WithDatabase(TestLegacyRangeMigration);
        WithDatabase(TestMigrationRollback);
        Console.WriteLine("PASS: " + assertions + " schedule/date/database assertions.");
    }

    private static void DateBoundaries()
    {
        Equal("还有1天开始", Countdown("2026-09-16", "2026-09-20"), "range before start");
        Equal("进行中，还剩5天", Countdown("2026-09-15", "2026-09-20"), "range on start");
        Equal("进行中，还剩5天", Countdown("2026-09-10", "2026-09-20"), "range during");
        Equal("进行中，今天结束", Countdown("2026-09-10", "2026-09-15"), "range inclusive end");
        Equal("已结束1天", Countdown("2026-09-10", "2026-09-14"), "range after end");
        Equal("进行中，今天结束", Countdown("2026-09-15", "2026-09-15"), "same-day range");
        Equal("共 3 天", ScheduleDateCalculator.FormatDuration("2024-02-28", "2024-03-01"), "leap-day duration");
        Equal("共 2 天", ScheduleDateCalculator.FormatDuration("2026-12-31", "2027-01-01"), "year boundary");
        Equal("共 1 天", ScheduleDateCalculator.FormatDuration("2026-09-15", "2026-09-15"), "same-day duration");
        Equal("还有1周1天开始", Countdown("2026-09-23", "2026-09-24", "Week"), "week mode");
        Equal("还有1个月开始", Countdown("2026-10-15", "2026-10-16", "Year"), "legacy year setting");
        Equal("还有1年开始", Countdown("2027-09-15", "2027-09-16", "YMD"), "YMD mode");
        Equal("还有1天开始", Countdown("2026-09-16", "2026-09-20", "unknown"), "unknown mode fallback");
        Equal("就在今天", ScheduleDateCalculator.FormatCountdown("2026-09-15", null, "Point", "Day", Today), "point today");
        Equal("已过1天", ScheduleDateCalculator.FormatCountdown("2026-09-14", null, "Point", "Day", Today), "point past");
        Equal("还有1天", ScheduleDateCalculator.FormatCountdown("2026-09-16", null, "Point", "Day", Today), "point future");
        Equal("2026-09-15 ～ 2026-09-20", ScheduleDateCalculator.FormatDates("2026-09-15", "2026-09-20", "Range"), "range date label");
        Equal("2026-09-15", ScheduleDateCalculator.FormatDates("2026-09-15", "2026-09-15", "Point"), "point date label");
        Throws<ArgumentException>(() => Countdown("2026-09-20", "2026-09-10"), "invalid calculator range");
        Throws<ArgumentException>(() => Record("bad", "2026-09-20", "2026-09-10"), "invalid record range");
        Throws<ArgumentException>(() => Record("bad", "not a date"), "invalid date");
        var point = Record("point", "2026-09-15");
        Equal("Point", point.ScheduleType, "default point type");
        Equal(point.Date, point.EndDate, "point end normalized");
    }

    private static void HomePresentation()
    {
        var before = ScheduleDateCalculator.Present("2026-09-16", "2026-09-20", "Range", "Day", Today);
        Equal("Upcoming", before.GroupKey, "home range before start group");
        Equal("距离开始", before.Prefix, "home range before start caption");
        Equal("1", before.Value, "home range before start value");
        Equal("天", before.Unit, "home range before start unit");
        Equal(0.0, before.Progress, "home progress before start");
        var active = ScheduleDateCalculator.Present("2026-09-10", "2026-09-20", "Range", "Day", Today);
        Equal("Current", active.GroupKey, "home active range group");
        Equal(50.0, active.Progress, "home active range progress");
        Equal("5", active.Value, "home active range countdown");
        var end = ScheduleDateCalculator.Present("2026-09-10", "2026-09-15", "Range", "Day", Today);
        Equal("Current", end.GroupKey, "home ending today is still current");
        Equal("今天结束", end.Value, "home ending today text");
        Equal("", end.Unit, "home ending today has no numeric unit");
        Equal(100.0, end.Progress, "home ending today full progress");
        var singleDay = ScheduleDateCalculator.Present("2026-09-15", "2026-09-15", "Range", "Day", Today);
        Equal(100.0, singleDay.Progress, "home single-day range no division by zero");
        var past = ScheduleDateCalculator.Present("2026-09-10", "2026-09-14", "Range", "Day", Today);
        Equal("Past", past.GroupKey, "home ended range group");
        Equal("1", past.Value, "home ended range elapsed days");
        Equal(100.0, past.Progress, "home ended range progress clamped");
        var point = ScheduleDateCalculator.Present("2026-09-15", null, "Point", "Day", Today);
        Equal("Current", point.GroupKey, "home point today group");
        Equal(false, point.IsRange, "home point has no timeline");
        Equal("就在今天", point.Value, "home point today countdown");
        var mixed = ScheduleDateCalculator.Present("2026-09-24", null, "Point", "Week", Today);
        Equal("1周2天", mixed.Value, "home mixed units preserved");
        Equal("", mixed.Unit, "home mixed units not duplicated");
        var wholeWeek = ScheduleDateCalculator.Present("2026-09-22", null, "Point", "Week", Today);
        Equal("1", wholeWeek.Value, "home whole week number");
        Equal("周", wholeWeek.Unit, "home whole week unit");
    }

    private static void TestServiceSettingsById(SQLiteConnection connection)
    {
        var values = new System.Collections.Generic.Dictionary<string, object>();
        var settings = new CountdownSettingsService(values);
        var service = new CountdownService(new CountdownRepository(connection), settings);
        var first = Record("same name", "2026-09-15");
        var second = Record("same name", "2026-09-16");
        service.Insert(first);
        service.Insert(second);
        settings.SetNote(first.Id, first.Schedule_name, first.Date, "first note");
        settings.SetNote(second.Id, second.Schedule_name, second.Date, "second note");
        Equal("first note", settings.GetNote(first.Id, first.Schedule_name, first.Date), "same-name note isolates first id");
        Equal("second note", settings.GetNote(second.Id, second.Schedule_name, second.Date), "same-name note isolates second id");

        first.Schedule_name = "renamed";
        first.Date = "2026-09-17";
        service.UpdateWithNote(first, "renamed note");
        Equal("renamed note", settings.GetNote(first.Id, first.Schedule_name, first.Date), "rename writes new id note");
        Equal("second note", settings.GetNote(second.Id, second.Schedule_name, second.Date), "rename leaves duplicate id note");

        settings.SetDesktopScheduleIds(new[] { second.Id, first.Id });
        var desktop = service.GetDesktopSchedules(settings);
        Equal(2, desktop.Count, "desktop resolves two ids");
        Equal(second.Id, desktop[0].Id, "desktop preserves slot order");
        Equal(first.Id, desktop[1].Id, "desktop preserves second slot");

        service.DeleteWithSettings(first.Id);
        Equal(null, settings.GetNote(first.Id, first.Schedule_name, first.Date), "delete removes id note");
        Equal(1, service.GetAll().Count, "delete removes only selected duplicate");
    }

    private static void LegacySettingsMigrateOnce()
    {
        var values = new System.Collections.Generic.Dictionary<string, object>
        {
            ["same2026-09-15"] = "legacy note",
            ["Alertsame"] = "1",
            ["DesktopKey0"] = "same",
            ["ItemCount"] = 1
        };
        var settings = new CountdownSettingsService(values);
        string firstId = Guid.NewGuid().ToString();
        string secondId = Guid.NewGuid().ToString();
        settings.MigrateLegacySettings(new[]
        {
            new ScheduleIdentity(firstId, "same", "2026-09-15"),
            new ScheduleIdentity(secondId, "same", "2026-09-16")
        });
        Equal("legacy note", settings.GetNote(firstId, "same", "2026-09-15"), "legacy note assigned to first GUID");
        Equal(null, settings.GetNote(secondId, "same", "2026-09-16"), "duplicate GUID does not inherit legacy note");
        Equal("1", settings.GetAlert(firstId, "same", ""), "legacy alert assigned to first GUID");
        Equal(false, values.ContainsKey("same2026-09-15"), "legacy note key retired");
        Equal(false, values.ContainsKey("Alertsame"), "legacy alert key retired");
        settings.InitializeSchedule(secondId);
        Equal(false, values.ContainsKey(CountdownSettingsService.NoteKey(secondId)), "new GUID starts clean");
    }

    private static void TestCurrentSchemaMigrationAndQueries(SQLiteConnection connection)
    {
        connection.Execute("create table DataTemplate (Id text primary key, Schedule_name text, CalculatedDate text, Date text, BgColor text, TintOpacity real, IsTop text, AddTime text)");
        connection.Execute("insert into DataTemplate values ('old-id', 'old', 'old countdown', '2026-09-14', '#abcdef', 0.5, '1', 'saved time')");
        var repository = new CountdownRepository(connection);
        var old = repository.GetById("old-id");
        Equal("Point", old.ScheduleType, "existing row type migrated");
        Equal(old.Date, old.EndDate, "existing row end migrated");
        Equal("#abcdef", old.BgColor, "existing appearance retained");
        Equal("saved time", old.AddTime, "existing pin metadata retained");
        var active = Record("same name", "2026-09-10", "2026-09-20");
        var future = Record("same name", "2026-09-16", "2026-09-21");
        var ended = Record("ended", "2026-09-01", "2026-09-14");
        var endsToday = Record("ends today", "2026-09-01", "2026-09-15");
        repository.Insert(active); repository.Insert(future); repository.Insert(ended); repository.Insert(endsToday);
        Equal(3, repository.GetFuture("2026-09-15").Count, "future includes active and end today");
        Equal(2, repository.GetPast("2026-09-15").Count, "past excludes active");
        Equal(3, repository.GetUnorderedFuture("2026-09-15").Count, "unordered future");
        Equal(2, repository.GetUnorderedPast("2026-09-15").Count, "unordered past");
        Equal(2, repository.GetByDate("2026-09-15").Count, "calendar range coverage");
        Equal(endsToday.Id, repository.GetNextFuture("2026-09-15").Id, "next includes active");
        repository.SetPinnedById(active.Id, true);
        Equal("0", repository.GetById(future.Id).IsTop, "pin isolates duplicate name");
        Equal(1, repository.GetPinnedFuture("2026-09-15").Count, "pinned active retained");
        Equal(1, repository.GetPinnedPast("2026-09-15").Count, "pinned past");
        string stableId = active.Id;
        active.Date = "2026-09-11";
        repository.Update(active);
        Equal(stableId, repository.GetById(stableId).Id, "edit preserves id");
        Equal("2026-09-11", repository.GetById(stableId).Date, "edit persisted");
        active.EndDate = "2026-09-01";
        Throws<ArgumentException>(() => repository.Update(active), "invalid update rejected");
        Equal("2026-09-20", repository.GetById(stableId).EndDate, "invalid update leaves data intact");
        repository.DeleteById(stableId);
        Equal(future.Id, repository.GetByName("same name").Single().Id, "delete isolates duplicate name");
        repository = new CountdownRepository(connection);
        Equal(4, repository.GetAll().Count, "migration repeat is idempotent");
        Equal("Range", repository.GetById(future.Id).ScheduleType, "migration preserves existing range type");
        Equal("2026-09-21", repository.GetById(future.Id).EndDate, "migration preserves existing range end");
        future.ScheduleType = "Point";
        repository.Update(future);
        Equal(future.Date, repository.GetById(future.Id).EndDate, "range-to-point clears old end");
    }

    private static void TestLegacyMigration(SQLiteConnection connection, bool empty)
    {
        connection.Execute("create table DataTemplate (Schedule_name text primary key, CalculatedDate text, Date text, BgColor text, TintOpacity real, IsTop text, AddTime text)");
        if (!empty) connection.Execute("insert into DataTemplate values ('legacy', 'count', '2026-09-15', 'Blue', 0.2, '1', 'original')");
        var repository = new CountdownRepository(connection);
        Equal(empty ? 0 : 1, repository.GetAll().Count, "legacy migrated including empty schema");
        if (!empty)
        {
            var item = repository.GetAll().Single();
            Equal("Point", item.ScheduleType, "legacy becomes point");
            Equal(item.Date, item.EndDate, "legacy end date");
            Equal("original", item.AddTime, "legacy metadata retained");
        }
        Equal(1, connection.ExecuteScalar<int>("select count(*) from sqlite_master where type = 'table' and name like 'DataTemplate_Backup_%'"), "legacy backup retained");
        repository.Insert(Record("new", "2026-10-01", "2026-10-02"));
        repository = new CountdownRepository(connection);
        Equal(empty ? 1 : 2, repository.GetAll().Count, "legacy migration not repeated");
    }

    private static void TestOldSpellingMigration(SQLiteConnection connection)
    {
        connection.Execute("create table DataTemple (Schedule_name text primary key, Date text)");
        connection.Execute("insert into DataTemple values ('older', '2026-09-15')");
        var repository = new CountdownRepository(connection);
        var item = repository.GetAll().Single();
        Equal("older", item.Schedule_name, "older table copied");
        repository.DeleteById(item.Id);
        repository = new CountdownRepository(connection);
        Equal(0, repository.GetAll().Count, "deleted imported item not resurrected");
    }

    private static void TestMigrationRollback(SQLiteConnection connection)
    {
        connection.Execute("create table DataTemplate (Schedule_name text primary key, Date text)");
        connection.Execute("insert into DataTemplate values ('retained', '2026-09-15')");
        // Make creation of the migrated schema fail after its transaction renames the old table.
        connection.Execute("create view IX_DataTemplate_Date as select 1");
        Throws<SQLiteException>(() => new CountdownRepository(connection), "migration failure propagated");
        Equal("retained", connection.ExecuteScalar<string>("select Schedule_name from DataTemplate"), "failed migration rolls back original table");
        Equal(0, connection.ExecuteScalar<int>("select count(*) from sqlite_master where type = 'table' and name like 'DataTemplate_Backup_%'"), "failed migration leaves no partial backup");
    }

    private static void TestLegacyRangeMigration(SQLiteConnection connection)
    {
        connection.Execute("create table DataTemplate (Schedule_name text primary key, Date text, ScheduleType text, EndDate text)");
        connection.Execute("insert into DataTemplate values ('legacy range', '2026-09-10', 'Range', '2026-09-20')");
        var repository = new CountdownRepository(connection);
        var item = repository.GetAll().Single();
        Equal("Range", item.ScheduleType, "legacy range type retained");
        Equal("2026-09-20", item.EndDate, "legacy range end retained");
        Equal(1, repository.GetFuture("2026-09-15").Count, "legacy range remains active");
    }

    private static string Countdown(string start, string end, string mode = "Day") =>
        ScheduleDateCalculator.FormatCountdown(start, end, "Range", mode, Today);

    private static CountdownRecord Record(string name, string date, string end = null) =>
        CountdownRecord.Create(name, "", date, "Blue", 0.5, "0", "", end);

    private static void WithDatabase(Action<SQLiteConnection> test)
    {
        // Real in-memory SQLite: no application database is accessed or modified.
        using (var connection = new SQLiteConnection(new SQLitePlatformGeneric(), ":memory:")) test(connection);
    }

    private static void Equal<T>(T expected, T actual, string label)
    {
        assertions++;
        if (!object.Equals(expected, actual)) throw new Exception(label + ": expected " + expected + ", got " + actual);
    }

    private static void Throws<T>(Action action, string label) where T : Exception
    {
        assertions++;
        try { action(); } catch (T) { return; }
        throw new Exception(label + ": expected " + typeof(T).Name);
    }
}
