using SQLite.Net.Attributes;
using System;
using System.Globalization;

namespace 夏日.Models
{
    [Table("DataTemplate")]
    public class CountdownRecord
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Schedule_name { get; set; }
        public string CalculatedDate { get; set; }
        public string Date { get; set; }
        public string ScheduleType { get; set; } = "Point";
        public string EndDate { get; set; }

        [Ignore]
        public string StartDate
        {
            get { return Date; }
            set { Date = value; }
        }
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
            string endDate = null)
        {
            var record = new CountdownRecord
            {
                Id = Guid.NewGuid().ToString(),
                Schedule_name = scheduleName,
                CalculatedDate = calculatedDate,
                Date = date,
                ScheduleType = string.IsNullOrWhiteSpace(endDate) ? "Point" : "Range",
                EndDate = endDate,
                BgColor = backgroundColor,
                TintOpacity = tintOpacity,
                IsTop = isTop,
                AddTime = addTime
            };
            record.NormalizeAndValidate();
            return record;
        }

        public void NormalizeAndValidate()
        {
            DateTime start;
            if (!TryParseDate(Date, out start))
            {
                throw new ArgumentException("请选择有效的开始日期。", nameof(Date));
            }

            if (string.IsNullOrWhiteSpace(ScheduleType))
            {
                ScheduleType = "Point";
            }

            if (string.Equals(ScheduleType, "Range", StringComparison.OrdinalIgnoreCase))
            {
                DateTime end;
                if (!TryParseDate(EndDate, out end))
                {
                    throw new ArgumentException("请选择有效的结束日期。", nameof(EndDate));
                }

                if (end < start)
                {
                    throw new ArgumentException("结束日期不能早于开始日期。", nameof(EndDate));
                }

                ScheduleType = "Range";
                EndDate = end.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            else if (string.Equals(ScheduleType, "Point", StringComparison.OrdinalIgnoreCase))
            {
                ScheduleType = "Point";
                EndDate = start.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            else
            {
                throw new ArgumentException("不支持的日程类型。", nameof(ScheduleType));
            }

            Date = start.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private static bool TryParseDate(string value, out DateTime date)
        {
            DateTime parsed;
            bool valid = DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                || DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsed);
            date = parsed.Date;
            return valid;
        }
    }

    [Table("DataTemplate")]
    internal class LegacyDataTemplate
    {
        [PrimaryKey]
        public string Schedule_name { get; set; }
        public string CalculatedDate { get; set; }
        public string Date { get; set; }
        public string BgColor { get; set; }
        public double TintOpacity { get; set; }
        public string IsTop { get; set; }
        public string AddTime { get; set; }
        // Optional columns are present in some upgraded legacy tables.
        public string ScheduleType { get; set; }
        public string EndDate { get; set; }

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
                string.Equals(ScheduleType, "Range", StringComparison.OrdinalIgnoreCase) ? EndDate : null);
        }
    }

    [Table("DataTemple")]
    internal class LegacyDataTemple
    {
        [PrimaryKey]
        public string Schedule_name { get; set; }
        public string CalculatedDate { get; set; }
        public string Date { get; set; }
        public string BgColor { get; set; }
        public double TintOpacity { get; set; }
        public string IsTop { get; set; }
        public string AddTime { get; set; }
        public string ScheduleType { get; set; }
        public string EndDate { get; set; }

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
                string.Equals(ScheduleType, "Range", StringComparison.OrdinalIgnoreCase) ? EndDate : null);
        }
    }
}
