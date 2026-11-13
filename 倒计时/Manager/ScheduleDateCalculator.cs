using System;
using System.Globalization;

namespace 倒计时.Manager
{
    /// <summary>All-day schedules include both their start and end date.</summary>
    internal static class ScheduleDateCalculator
    {
        public static bool IsRange(string scheduleType, string endDate)
        {
            return string.Equals(scheduleType, "Range", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(endDate);
        }

        public static string FormatCountdown(string date, string endDate, string scheduleType,
            string mode = "Day", DateTime? today = null)
        {
            DateTime start = ParseDate(date);
            DateTime current = (today ?? DateTime.Today).Date;

            if (!IsRange(scheduleType, endDate))
            {
                if (current == start)
                {
                    return "就在今天";
                }

                return current < start
                    ? "还有" + FormatInterval(current, start, mode)
                    : "已过" + FormatInterval(start, current, mode);
            }

            DateTime end = ParseDate(endDate);
            ValidateRange(start, end);
            if (current < start)
            {
                return "还有" + FormatInterval(current, start, mode) + "开始";
            }

            if (current > end)
            {
                return "已结束" + FormatInterval(end, current, mode);
            }

            return current == end
                ? "进行中，今天结束"
                : "进行中，还剩" + FormatInterval(current, end, mode);
        }

        public static string FormatDates(string date, string endDate, string scheduleType)
        {
            return IsRange(scheduleType, endDate) ? date + " ～ " + endDate : date;
        }

        public static string FormatDuration(string date, string endDate)
        {
            DateTime start = ParseDate(date);
            DateTime end = ParseDate(endDate);
            ValidateRange(start, end);
            // The range is still calculated inclusively; keep the presentation concise.
            return "共 " + ((end - start).Days + 1) + " 天";
        }

        public static SchedulePresentation Present(string date, string endDate, string scheduleType,
            string mode = "Day", DateTime? today = null)
        {
            DateTime current = (today ?? DateTime.Today).Date;
            DateTime start = ParseDate(date);
            bool range = IsRange(scheduleType, endDate);
            DateTime end = range ? ParseDate(endDate) : start;
            ValidateRange(start, end);
            string full = FormatCountdown(date, endDate, scheduleType, mode, current);
            var result = new SchedulePresentation
            {
                IsRange = range,
                GroupKey = current > end ? "Past" : current < start ? "Upcoming" : "Current",
                Status = current > end ? (range ? "已结束" : "已过")
                    : current < start ? "即将到来" : range ? "进行中" : "今天",
                Progress = !range || current < start ? 0 : current >= end ? 100
                    : 100.0 * (current - start).Days / (end - start).Days,
                StartLabel = start.ToString("M月d日") + " · 开始",
                EndLabel = end.ToString("M月d日") + " · 结束",
                TodayLabel = "今天 · " + current.ToString("M月d日"),
                Prefix = current > end ? (range ? "已结束" : "已过")
                    : current < start ? (range ? "距离开始" : "还有") : range ? "距离结束" : "",
                Value = full,
                Unit = ""
            };
            if (current == end)
            {
                result.Value = range ? "今天结束" : "就在今天";
                result.Prefix = range ? "进行中" : "";
            }
            else
            {
                int firstDigit = -1;
                for (int i = 0; i < full.Length; i++)
                    if (char.IsDigit(full[i])) { firstDigit = i; break; }
                if (firstDigit >= 0)
                {
                    string value = full.Substring(firstDigit);
                    if (value.EndsWith("开始")) value = value.Substring(0, value.Length - 2);
                    // Split only a simple interval. Keep mixed units together.
                    int digits = 0;
                    while (digits < value.Length && char.IsDigit(value[digits])) digits++;
                    string unit = value.Substring(digits);
                    if (unit == "天" || unit == "周" || unit == "年" || unit == "个月")
                    {
                        result.Value = value.Substring(0, digits);
                        result.Unit = unit;
                    }
                    else result.Value = value;
                }
            }
            return result;
        }

        private static string FormatInterval(DateTime start, DateTime end, string mode)
        {
            int days = (end - start).Days;
            if (string.Equals(mode, "YMD", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mode, "Year", StringComparison.OrdinalIgnoreCase))
            {
                return FormatYmd(start, end);
            }

            if (string.Equals(mode, "Week", StringComparison.OrdinalIgnoreCase))
            {
                int weeks = days / 7;
                int remainingDays = days % 7;
                if (weeks > 0)
                {
                    return weeks + "周" + (remainingDays == 0 ? "" : remainingDays + "天");
                }
            }

            return days + "天";
        }

        private static string FormatYmd(DateTime start, DateTime end)
        {
            int years = end.Year - start.Year;
            DateTime cursor = start.AddYears(years);
            if (cursor > end) { years--; cursor = start.AddYears(years); }
            int months = (end.Year - cursor.Year) * 12 + end.Month - cursor.Month;
            DateTime yearStart = cursor;
            cursor = yearStart.AddMonths(months);
            if (cursor > end) { months--; cursor = yearStart.AddMonths(months); }
            int days = (end - cursor).Days;
            string result = "";
            if (years > 0) result += years + "年";
            if (months > 0) result += months + "个月";
            if (days > 0) result += days + "天";
            return string.IsNullOrEmpty(result) ? "0天" : result;
        }

        private static DateTime ParseDate(string value)
        {
            DateTime parsed;
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                || DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsed))
            {
                return parsed.Date;
            }

            throw new ArgumentException("日程日期无效。", nameof(value));
        }

        private static void ValidateRange(DateTime start, DateTime end)
        {
            if (end < start)
            {
                throw new ArgumentException("结束日期不能早于开始日期。", nameof(end));
            }
        }
    }

    internal sealed class SchedulePresentation
    {
        public bool IsRange { get; set; }
        public string GroupKey { get; set; }
        public string Status { get; set; }
        public double Progress { get; set; }
        public string StartLabel { get; set; }
        public string EndLabel { get; set; }
        public string TodayLabel { get; set; }
        public string Prefix { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
    }
}
