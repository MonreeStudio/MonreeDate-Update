using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace 倒计时.Manager
{
    public static class CountdownDateCalculator
    {
        public static string FormatDayCountdown(string date)
        {
            DateTime targetDate = ParseDate(date);
            DateTime today = DateTime.Today;
            int days = (today - targetDate).Days;

            if (days < 0)
            {
                return "还有" + Math.Abs(days) + "天";
            }

            if (days == 0)
            {
                return "就在今天";
            }

            return "已过" + days + "天";
        }

        public static string FormatWeekCountdown(string date)
        {
            DateTime targetDate = ParseDate(date);
            DateTime today = DateTime.Today;

            if (today == targetDate)
            {
                return "就在今天";
            }

            int days = Math.Abs((today - targetDate).Days);
            int weekCount = days / 7;
            int remainingDays = days % 7;
            string prefix = today > targetDate ? "已过" : "还有";

            if (weekCount == 0)
            {
                return prefix + remainingDays + "天";
            }

            if (remainingDays == 0)
            {
                return prefix + weekCount + "周";
            }

            return prefix + weekCount + "周" + remainingDays + "天";
        }

        public static string FormatYearMonthDayCountdown(string date)
        {
            DateTime targetDate = ParseDate(date);
            DateTime today = DateTime.Today;

            if (today == targetDate)
            {
                return "就在今天";
            }

            if (today > targetDate)
            {
                return "已过" + FormatDateRange(targetDate, today);
            }

            return "还有" + FormatDateRange(today, targetDate);
        }

        public static string FormatDateRange(DateTime startDate, DateTime endDate)
        {
            DateTime normalizedStartDate = NormalizeDate(startDate);
            DateTime normalizedEndDate = NormalizeDate(endDate);

            if (normalizedStartDate >= normalizedEndDate)
            {
                throw new Exception("开始日期必须小于结束日期");
            }

            var dateParts = new
            {
                StartMonth = normalizedStartDate.Month,
                EndMonth = normalizedEndDate.Month,
                StartDay = normalizedStartDate.Day,
                EndDay = normalizedEndDate.Day
            };
            int monthDifference = (normalizedEndDate.Year - normalizedStartDate.Year) * 12
                + (dateParts.EndMonth - dateParts.StartMonth);
            int yearDifference = monthDifference / 12;

            int[] dateDifference = new int[3] { 0, 0, 0 };
            if (yearDifference > 0)
            {
                if (dateParts.EndMonth == dateParts.StartMonth && dateParts.EndDay < dateParts.StartDay)
                {
                    dateDifference[0] = yearDifference - 1;
                }
                else
                {
                    dateDifference[0] = yearDifference;
                }
            }

            if (dateParts.EndDay >= dateParts.StartDay)
            {
                dateDifference[1] = monthDifference % 12;
                dateDifference[2] = dateParts.EndDay - dateParts.StartDay;
            }
            else
            {
                int adjustedMonthDifference = monthDifference - 1;
                dateDifference[1] = adjustedMonthDifference % 12;
                TimeSpan daySpan = normalizedEndDate - normalizedStartDate.AddMonths(adjustedMonthDifference);
                dateDifference[2] = daySpan.Days;
            }

            StringBuilder result = new StringBuilder();
            if (dateDifference.Sum() > 0)
            {
                if (dateDifference[0] > 0)
                {
                    result.Append($"{dateDifference[0]}年");
                }

                if (dateDifference[1] > 0)
                {
                    result.Append($"{dateDifference[1]}个月");
                }

                if (dateDifference[2] > 0)
                {
                    result.Append($"{dateDifference[2]}天");
                }

                return result.ToString();
            }

            TimeSpan timeSpan = endDate - startDate;
            int[] timeDifference = new int[2] { timeSpan.Hours, timeSpan.Minutes % 60 };
            if (timeDifference[0] > 0)
            {
                result.Append($"{timeDifference[0]}小时");
            }

            if (timeDifference[1] > 0)
            {
                result.Append($"{timeDifference[1]}分钟");
            }

            if (timeDifference.Sum() <= 0)
            {
                result.Append($"{timeSpan.Seconds}秒");
            }

            return result.ToString();
        }

        private static DateTime NormalizeDate(DateTime date)
        {
            return date.Date;
        }

        private static DateTime ParseDate(string date)
        {
            DateTime parsedDate;
            if (DateTime.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out parsedDate)
                || DateTime.TryParse(date, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out parsedDate))
            {
                return parsedDate.Date;
            }

            return Convert.ToDateTime(date).Date;
        }
    }
}
