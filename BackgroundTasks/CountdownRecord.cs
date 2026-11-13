using SQLite.Net.Attributes;
using System;

namespace BackgroundTasks
{
    [Table("DataTemplate")]
    class CountdownRecord
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Schedule_name { get; set; }
        public string CalculatedDate { get; set; }
        public string Date { get; set; }
        public string ScheduleType { get; set; } = "Point";
        public string EndDate { get; set; }
        public string BgColor { get; set; }
        public double TintOpacity { get; set; }
        public string IsTop { get; set; }
        public string AddTime { get; set; }

        public static CountdownRecord Create(
            string scheduleName,
            string calculatedDate,
            string date,
            string backgroundColor,
            double tintOpacity,
            string isTop,
            string addTime,
            string scheduleType = "Point",
            string endDate = null)
        {
            string normalizedType = string.Equals(scheduleType, "Range", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(endDate) ? "Range" : "Point";
            return new CountdownRecord
            {
                Id = Guid.NewGuid().ToString(),
                Schedule_name = scheduleName,
                CalculatedDate = calculatedDate,
                Date = date,
                ScheduleType = normalizedType,
                EndDate = normalizedType == "Point" || string.IsNullOrWhiteSpace(endDate) ? date : endDate,
                BgColor = backgroundColor,
                TintOpacity = tintOpacity,
                IsTop = isTop,
                AddTime = addTime
            };
        }
    }

    [Table("DataTemplate")]
    class LegacyDataTemplate
    {
        [PrimaryKey]
        public string Schedule_name { get; set; }
        public string CalculatedDate { get; set; }
        public string Date { get; set; }
        public string ScheduleType { get; set; }
        public string EndDate { get; set; }
        public string BgColor { get; set; }
        public double TintOpacity { get; set; }
        public string IsTop { get; set; }
        public string AddTime { get; set; }

        public CountdownRecord ToCountdownRecord()
        {
            return CountdownRecord.Create(
                Schedule_name,
                CalculatedDate,
                Date,
                BgColor,
                TintOpacity,
                IsTop,
                AddTime,
                ScheduleType,
                EndDate);
        }
    }

    [Table("DataTemple")]
    class LegacyDataTemple
    {
        [PrimaryKey]
        public string Schedule_name { get; set; }
        public string CalculatedDate { get; set; }
        public string Date { get; set; }
        public string ScheduleType { get; set; }
        public string EndDate { get; set; }
        public string BgColor { get; set; }
        public double TintOpacity { get; set; }
        public string IsTop { get; set; }
        public string AddTime { get; set; }

        public CountdownRecord ToCountdownRecord()
        {
            return CountdownRecord.Create(
                Schedule_name,
                CalculatedDate,
                Date,
                BgColor,
                TintOpacity,
                IsTop,
                AddTime,
                ScheduleType,
                EndDate);
        }
    }
}
