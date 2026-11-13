using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using 夏日.Models;

namespace 倒计时.Manager
{
    /// <summary>
    /// Application-facing schedule operations.
    ///
    /// Coordinates record changes with the settings that belong to their
    /// stable IDs. Pages handle the visible UI and delegate persistence here.
    /// </summary>
    internal sealed class CountdownService
    {
        private readonly CountdownRepository repository;
        private readonly CountdownSettingsService settings;

        public CountdownService()
            : this(new CountdownRepository(), new CountdownSettingsService())
        {
        }

        public CountdownService(CountdownRepository repository)
            : this(repository, new CountdownSettingsService())
        {
        }

        public CountdownService(CountdownRepository repository, CountdownSettingsService settings)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            this.repository = repository;
            this.settings = settings;
        }

        /// <summary>Legacy escape hatch for code that still needs the connection.</summary>
        public CountdownRepository Repository { get { return repository; } }

        // Read operations mirror CountdownRepository so callers can migrate
        // one page at a time without changing query semantics.
        public List<CountdownRecord> GetAll() { return repository.GetAll(); }
        public List<CountdownRecord> GetAllOrderByDate() { return repository.GetAllOrderByDate(); }
        public List<CountdownRecord> GetByName(string scheduleName) { return repository.GetByName(scheduleName); }
        public CountdownRecord GetById(string id) { return repository.GetById(id); }
        public List<CountdownRecord> GetPinned() { return repository.GetPinned(); }
        public List<CountdownRecord> GetPinnedPast(string today) { return repository.GetPinnedPast(today); }
        public List<CountdownRecord> GetPinnedFuture(string today) { return repository.GetPinnedFuture(today); }
        public List<CountdownRecord> GetPinnedOrderByAddTime() { return repository.GetPinnedOrderByAddTime(); }
        public List<CountdownRecord> GetFuture(string today) { return repository.GetFuture(today); }
        public List<CountdownRecord> GetPast(string today) { return repository.GetPast(today); }
        public List<CountdownRecord> GetUnorderedFuture(string today) { return repository.GetUnorderedFuture(today); }
        public List<CountdownRecord> GetUnorderedPast(string today) { return repository.GetUnorderedPast(today); }
        public List<CountdownRecord> GetByDate(string date) { return repository.GetByDate(date); }
        public CountdownRecord GetNextFuture(string today) { return repository.GetNextFuture(today); }

        /// <summary>
        /// Creates and validates a record before inserting it.
        /// </summary>
        public CountdownRecord Create(string scheduleName, string calculatedDate, string date,
            string backgroundColor, double tintOpacity, bool isPinned, string addTime,
            string endDate = null)
        {
            var record = CountdownRecord.Create(scheduleName, calculatedDate, date,
                backgroundColor, tintOpacity, isPinned ? "1" : "0",
                isPinned ? (string.IsNullOrWhiteSpace(addTime) ? DateTime.Now.ToString() : addTime) : addTime,
                endDate);
            return Insert(record);
        }

        /// <summary>Inserts a new record and returns the record with its ID.</summary>
        public CountdownRecord Insert(CountdownRecord countdown)
        {
            repository.Insert(countdown);
            // A newly created schedule must never inherit another schedule's
            // retired name-based keys, even when its name and date are reused.
            settings.InitializeSchedule(countdown.Id);
            return countdown;
        }

        public CountdownRecord CreateWithNote(CountdownRecord countdown, string note)
        {
            Insert(countdown);
            settings.SetNote(countdown.Id, countdown.Schedule_name, countdown.Date, note);
            return countdown;
        }

        /// <summary>Updates an existing record identified by its stable ID.</summary>
        public CountdownRecord Update(CountdownRecord countdown)
        {
            if (countdown == null) throw new ArgumentNullException(nameof(countdown));
            RequireId(countdown.Id);
            var original = repository.GetById(countdown.Id);
            if (original == null)
                throw new InvalidOperationException("该日程已不存在，请刷新后重试。");

            repository.Update(countdown);
            settings.MigrateSchedule(countdown.Id, original.Schedule_name, original.Date,
                countdown.Schedule_name, countdown.Date);
            return countdown;
        }

        public CountdownRecord UpdateWithNote(CountdownRecord countdown, string note)
        {
            Update(countdown);
            settings.SetNote(countdown.Id, countdown.Schedule_name, countdown.Date, note);
            return countdown;
        }

        public void DeleteById(string id) { DeleteWithSettings(id); }

        /// <summary>Deletes one schedule by GUID and reports whether it existed.</summary>
        public bool Delete(string id)
        {
            return DeleteWithSettings(id);
        }

        /// <summary>Removes the schedule and all settings scoped to its ID.</summary>
        public bool DeleteWithSettings(string id)
        {
            RequireId(id);
            bool existed = repository.GetById(id) != null;
            repository.DeleteById(id);
            settings.RemoveSchedule(id);
            return existed;
        }

        public void SetPinnedById(string id, bool isPinned)
        {
            RequireId(id);
            repository.SetPinnedById(id, isPinned);
        }

        public List<CountdownRecord> GetDesktopSchedules()
        {
            return GetDesktopSchedules(settings);
        }

        /// <summary>Resolves selected IDs while preserving their desktop slot order.</summary>
        public List<CountdownRecord> GetDesktopSchedules(CountdownSettingsService scheduleSettings)
        {
            if (scheduleSettings == null) throw new ArgumentNullException(nameof(scheduleSettings));
            var records = repository.GetAll();
            var ids = scheduleSettings.GetDesktopScheduleIds(records
                .Select(record => new ScheduleIdentity(record.Id, record.Schedule_name)));
            return ids.Select(id => records.FirstOrDefault(record =>
                    string.Equals(CountdownSettingsService.NormalizeId(record.Id), id, StringComparison.Ordinal)))
                .Where(record => record != null).ToList();
        }

        /// <summary>
        /// Returns the two query streams used by the home page.  Pinned items
        /// are returned separately so the caller can keep them at the top while
        /// de-duplicating by ID.
        /// </summary>
        public ScheduleDisplayQuery QueryDisplay(string displayMode, string today = null)
        {
            string normalizedToday = NormalizeToday(today);
            string mode = string.IsNullOrWhiteSpace(displayMode) ? "All" : displayMode.Trim();
            if (string.Equals(mode, "Upcoming", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mode, "Future", StringComparison.OrdinalIgnoreCase)) mode = "Future";
            else if (string.Equals(mode, "Past", StringComparison.OrdinalIgnoreCase)) mode = "Past";
            else if (string.Equals(mode, "Current", StringComparison.OrdinalIgnoreCase)) mode = "Current";
            else mode = "All";

            switch (mode)
            {
                case "Past":
                    return new ScheduleDisplayQuery(mode, repository.GetPinnedPast(normalizedToday),
                        repository.GetPast(normalizedToday));
                case "Future":
                    return new ScheduleDisplayQuery(mode, repository.GetPinnedFuture(normalizedToday),
                        repository.GetFuture(normalizedToday));
                case "Current":
                    return new ScheduleDisplayQuery(mode, repository.GetPinnedOrderByAddTime(),
                        repository.GetByDate(normalizedToday));
                case "All":
                    return new ScheduleDisplayQuery(mode, repository.GetPinnedOrderByAddTime(),
                        repository.GetFuture(normalizedToday).Concat(repository.GetPast(normalizedToday)).ToList());
                default:
                    return new ScheduleDisplayQuery("All", repository.GetPinnedOrderByAddTime(),
                        repository.GetFuture(normalizedToday).Concat(repository.GetPast(normalizedToday)).ToList());
            }
        }

        /// <summary>
        /// Returns a de-duplicated stream for a home-page mode.  IDs, rather
        /// than names, determine identity so duplicate schedule names remain
        /// independent.
        /// </summary>
        public List<CountdownRecord> GetDisplayRecords(string displayMode, string today = null)
        {
            var query = QueryDisplay(displayMode, today);
            var records = query.Pinned.Concat(query.Records)
                .Where(record => record != null)
                .GroupBy(record => record.Id ?? string.Empty)
                .Select(group => group.First())
                .ToList();

            if (string.Equals(query.Mode, "Current", StringComparison.OrdinalIgnoreCase))
            {
                string normalizedToday = NormalizeToday(today);
                records = records.Where(record =>
                    string.Equals(ScheduleDateCalculator.Present(record.Date, record.EndDate,
                        record.ScheduleType, "Day", ParseDate(normalizedToday)).GroupKey,
                        "Current", StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return records;
        }

        /// <summary>Builds the date/status presentation for one record.</summary>
        public SchedulePresentation Present(CountdownRecord record, string dateMode = "Day", DateTime? today = null)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            return ScheduleDateCalculator.Present(record.Date, record.EndDate, record.ScheduleType,
                dateMode, today);
        }

        private static string NormalizeToday(string today)
        {
            if (string.IsNullOrWhiteSpace(today))
                return DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            DateTime parsed;
            if (!DateTime.TryParse(today, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                && !DateTime.TryParse(today, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsed))
            {
                throw new ArgumentException("日期无效。", nameof(today));
            }
            return parsed.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private static DateTime ParseDate(string value)
        {
            DateTime parsed;
            if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                && !DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsed))
            {
                throw new ArgumentException("日期无效。", nameof(value));
            }
            return parsed.Date;
        }

        private static void RequireId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("日程 ID 不能为空。", nameof(id));
        }
    }

    /// <summary>Separate pinned and normal streams for home-page rendering.</summary>
    internal sealed class ScheduleDisplayQuery
    {
        internal ScheduleDisplayQuery(string mode, List<CountdownRecord> pinned,
            List<CountdownRecord> records)
        {
            Mode = mode;
            Pinned = pinned ?? new List<CountdownRecord>();
            Records = records ?? new List<CountdownRecord>();
        }

        public string Mode { get; private set; }
        public List<CountdownRecord> Pinned { get; private set; }
        public List<CountdownRecord> Records { get; private set; }
    }
}
