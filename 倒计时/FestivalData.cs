using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Media;
using 倒计时.Manager;

namespace 倒计时
{
    public class FestivalData
    {
        public string Str1 { get; set; }
        public string Str2 { get; set; }
        public string Str3 { get; set; }
        public string CountdownPrefix { get; set; }
        public string CountdownValue { get; set; }
        public string CountdownUnit { get; set; }
        public Color Str4 { get; set; }
        public AcrylicBrush Str5 { get; set; }
        public string BackGroundColor { get; set; }

        public static FestivalData Current;

        public FestivalData()
        {
            Current = this;
            Str1 = "string 1";
            Str2 = "string 2";
            Str3 = "string 3";
            CountdownPrefix = "";
            CountdownValue = "";
            CountdownUnit = "";
            BackGroundColor = "SkyBlue";
        }
    }

    public class FestivalDataViewModel
    {
        public ObservableCollection<FestivalData> FestivalDatas { get; } = new ObservableCollection<FestivalData>();

        private sealed class FestivalRule
        {
            public string Name { get; set; }
            public Color Color { get; set; }
            public Func<DateTime, DateTime> GetDate { get; set; }
        }

        public FestivalDataViewModel()
        {
            var rules = new List<FestivalRule>
            {
                Solar("元旦", 1, 1, Color.FromArgb(255, 241, 147, 156)),
                Lunar("春节", 1, 1, Color.FromArgb(255, 166, 27, 41)),
                Solar("情人节❤", 2, 14, Colors.LightPink),
                Lunar("元宵节", 1, 15, Color.FromArgb(255, 251, 182, 18)),
                Solar("妇女节", 3, 8, Color.FromArgb(255, 231, 124, 142)),
                SolarTerm("清明节", "清明", Color.FromArgb(255, 18, 170, 156)),
                Solar("劳动节", 5, 1, Color.FromArgb(255, 251, 185, 87)),
                Solar("儿童节", 6, 1, Colors.Orange),
                Lunar("端午节", 5, 5, Color.FromArgb(255, 105, 167, 148)),
                Lunar("七夕❤", 7, 7, Colors.LightPink),
                Solar("教师节", 9, 10, Colors.MediumPurple),
                Lunar("中秋节", 8, 15, Color.FromArgb(255, 166, 27, 41)),
                Solar("国庆节", 10, 1, Colors.OrangeRed),
                Lunar("重阳节", 9, 9, Colors.DarkOrange),
                Solar("圣诞节", 12, 25, Colors.Crimson)
            };

            foreach (var rule in rules)
            {
                var date = rule.GetDate(DateTime.Today);
                var dateText = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                var presentation = ScheduleDateCalculator.Present(dateText, null, "Point");
                FestivalDatas.Add(new FestivalData
                {
                    Str1 = rule.Name,
                    Str2 = CustomData.Calculator(dateText),
                    Str3 = dateText,
                    CountdownPrefix = presentation.Prefix,
                    CountdownValue = presentation.Value,
                    CountdownUnit = presentation.Unit,
                    Str4 = rule.Color,
                    Str5 = All.Current.ColorfulBrush(rule.Color, 0.7),
                    BackGroundColor = rule.Color.ToString()
                });
            }

            var ordered = FestivalDatas.OrderBy(item => DateTime.ParseExact(item.Str3, "yyyy-MM-dd", CultureInfo.InvariantCulture)).ToList();
            FestivalDatas.Clear();
            foreach (var item in ordered)
                FestivalDatas.Add(item);
        }

        private static FestivalRule Solar(string name, int month, int day, Color color)
        {
            return new FestivalRule
            {
                Name = name,
                Color = color,
                GetDate = today => NextSolarDate(today, month, day)
            };
        }

        private static FestivalRule Lunar(string name, int month, int day, Color color)
        {
            return new FestivalRule
            {
                Name = name,
                Color = color,
                GetDate = today => NextLunarDate(today, month, day)
            };
        }

        private static FestivalRule SolarTerm(string name, string term, Color color)
        {
            return new FestivalRule
            {
                Name = name,
                Color = color,
                GetDate = today => NextSolarTerm(today, term)
            };
        }

        private static DateTime NextSolarDate(DateTime today, int month, int day)
        {
            var date = new DateTime(today.Year, month, day);
            return date < today ? date.AddYears(1) : date;
        }

        private static DateTime NextLunarDate(DateTime today, int month, int day)
        {
            var calendar = new ChineseLunisolarCalendar();
            for (var offset = 0; offset <= 400; offset++)
            {
                var date = today.AddDays(offset);
                var lunarYear = calendar.GetYear(date);
                var lunarMonth = calendar.GetMonth(date);
                var leapMonth = calendar.GetLeapMonth(lunarYear);
                var isLeapMonth = leapMonth > 0 && lunarMonth == leapMonth;
                if (leapMonth > 0 && lunarMonth >= leapMonth)
                    lunarMonth--;
                if (!isLeapMonth && lunarMonth == month && calendar.GetDayOfMonth(date) == day)
                    return date;
            }
            throw new InvalidOperationException("无法计算农历节日日期。");
        }

        private static DateTime NextSolarTerm(DateTime today, string term)
        {
            for (var offset = 0; offset <= 400; offset++)
            {
                var date = today.AddDays(offset);
                if (LunarCalendarConverter.GetTerm(date.Year, date.Month, date.Day) == term)
                    return date;
            }
            throw new InvalidOperationException("无法计算节气日期。");
        }
    }
}
