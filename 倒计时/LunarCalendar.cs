using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 倒计时
{
    
    public class LunarCalendar
    {
        static ChineseLunisolarCalendar cCalendar = new ChineseLunisolarCalendar();
        private static string[] tiangan = { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };
        private static string[] dizhi = { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };
        private static string[] shengxiao = { "鼠", "牛", "虎", "免", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };
        private static string[] months = { "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二(腊)" };
        private static string[] days1 = { "初", "十", "廿", "三" };
        private static string[] days = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };

        public static string GetChineseDateTime(DateTime datetime)
        {
            int lyear = cCalendar.GetYear(datetime);
            int lmonth = cCalendar.GetMonth(datetime);
            int lday = cCalendar.GetDayOfMonth(datetime);

            //获取闰月， 0 则表示没有闰月
            int leapMonth = cCalendar.GetLeapMonth(lyear);

            bool isleap = false;

            if (leapMonth > 0)
            {
                if (leapMonth == lmonth)
                {
                    //闰月
                    isleap = true;
                    lmonth--;
                }
                else if (lmonth > leapMonth)
                {
                    lmonth--;
                }
            }

            return string.Concat(GetLunisolarYear(lyear), "年 ", isleap ? "闰" : string.Empty, GetLunisolarMonth(lmonth), "月", GetLunisolarDay(lday));
        }

        public static string GetLunisolarYear(int year)
        {
            if (year > 3)
            {
                int tgIndex = (year - 4) % 10;
                int dzIndex = (year - 4) % 12;

                return string.Concat(tiangan[tgIndex], dizhi[dzIndex], "[", shengxiao[dzIndex], "]");

            }

            throw new ArgumentOutOfRangeException("无效的年份!");
        }

        public static string GetLunisolarMonth(int month)
        {
            if (month < 13 && month > 0)
            {
                return months[month - 1];
            }

            throw new ArgumentOutOfRangeException("无效的月份!");
        }

        public static string GetLunisolarDay(int day)
        {
            if (day > 0 && day < 32)
            {
                if (day != 20 && day != 30)
                {
                    return string.Concat(days1[(day - 1) / 10], days[(day - 1) % 10]);
                }
                else
                {
                    return string.Concat(days[(day - 1) / 10], days1[1]);
                }
            }

            throw new ArgumentOutOfRangeException("无效的日!");
        }

        public static string GetShengXiao(DateTime datetime)
        {
            return shengxiao[cCalendar.GetTerrestrialBranch(cCalendar.GetSexagenaryYear(datetime)) - 1];
        }
    }

}
