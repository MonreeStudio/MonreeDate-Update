using SQLite.Net.Attributes;
using System;

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
            string addTime)
        {
            return new CountdownRecord
            {
                Id = Guid.NewGuid().ToString(),
                Schedule_name = scheduleName,
                CalculatedDate = calculatedDate,
                Date = date,
                BgColor = backgroundColor,
                TintOpacity = tintOpacity,
                IsTop = isTop,
                AddTime = addTime
            };
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

        public CountdownRecord ToCountdownRecord()
        {
            return CountdownRecord.Create(
                Schedule_name,
                CalculatedDate,
                Date,
                BgColor,
                TintOpacity,
                IsTop,
                AddTime);
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

        public CountdownRecord ToCountdownRecord()
        {
            return CountdownRecord.Create(
                Schedule_name,
                CalculatedDate,
                Date,
                BgColor,
                TintOpacity,
                IsTop,
                AddTime);
        }
    }
}
