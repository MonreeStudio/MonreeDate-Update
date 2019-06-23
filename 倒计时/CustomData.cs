using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLitePCL;
using Windows.Storage;

namespace 倒计时
{
    public class CustomData
    {

        private ApplicationDataContainer _appSettings;

        public string Str1 { get; set; }
        public string Str2 { get; set; }
        public string Str3 { get; set; }

        public int ItemWidth { get; set; }
        public string BackGroundColor { get; set; }

        public CustomData()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            // Simple setting

            localSettings.Values["exampleSetting"] = "Hello Windows";
            // Simple setting
            Object value = localSettings.Values["exampleSetting"];

            this.Str1 = "string 1";
            this.Str2 = "string 2";
            this.Str3 = "string 3";
            this.BackGroundColor = "SkyBlue";
            _appSettings = ApplicationData.Current.LocalSettings;
        }

        static public CustomData DateCalculator(string D, string E)
        {
            string s1 = E;
            string s2, s3;
            string d = D;
            string str2 = DateTime.Now.ToShortDateString().ToString();
            DateTime d1 = Convert.ToDateTime(d);
            DateTime d2 = Convert.ToDateTime(str2);
            DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
            DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));
            int days = (d4 - d3).Days;
            if (days < 0)
            {
                s3 = "已过" + App.term(Convert.ToDateTime(d4), Convert.ToDateTime(d3));
                days = Math.Abs(days);
                s2 = "已过" + days.ToString() + "天";
            }
            else
            {
                s3 = "还有" + App.term(Convert.ToDateTime(d3), Convert.ToDateTime(d4));
                s2 = "还有" + days.ToString() + "天";
            }
            return new CustomData()
            {
                Str1 = s1,
                Str2 = s2,
                Str3 = d,
            };
        }

       static public string Calculator(string s1)
        {
            string str1 = s1;
            string str2 = DateTime.Now.ToShortDateString().ToString();
            string s2;
            DateTime d1 = Convert.ToDateTime(str1);
            DateTime d2 = Convert.ToDateTime(str2);
            DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
            DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));
            int days = (d4 - d3).Days;
            if (days < 0)
            {
                days = -days;
                s2 = "还有" + days.ToString() + "天";
            }
            else
                s2 = "已过" + days.ToString() + "天";
            return s2;
        }
    }

    public class CustomDataViewModel
    {
        public ObservableCollection<CustomData> CustomDatas = new ObservableCollection<CustomData>();
        public string Today = DateTime.Now.ToShortDateString().ToString();

        public CustomDataViewModel()
        {
            //SQLiteConnection _connection = new SQLiteConnection(App.DB_NAME);
            //using (var statement = _connection.Prepare(App.SQL_CREATE_TABLE))
            //{
            //    statement.Step();
            //}

            //using (var statement = _connection.Prepare(App.SQL_INSERT))
            //{
            //    statement.Bind(1,CustomDatas.Add(new CustomData() { Str1 = "Together", Str2 = CustomData.Calculator("2018/12/24"), Str3 = "2018/12/24" }));
            //    statement.Bind(2, value);
            //    statement.Step();
            //}
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            // Simple setting

            localSettings.Values["exampleSetting"] = "Hello Windows";
            // Simple setting
            Object value = localSettings.Values["exampleSetting"];

            CustomDatas.Add(new CustomData() { Str1 = "Together", Str2 = CustomData.Calculator("2018/12/24"), Str3 = "2018/12/24" });
            CustomDatas.Add(new CustomData() { Str1 = "大学英语六级", Str2 = CustomData.Calculator("2019/6/15"), Str3 = "2019/6/15"});
            CustomDatas.Add(new CustomData() { Str1 = "英语专业八级", Str2 = CustomData.Calculator("2020/3/23"), Str3 = "2020/3/23"});
            CustomDatas.Add(new CustomData() { Str1 = "小异的生日", Str2 = CustomData.Calculator("2019/7/30"), Str3 = "2019/7/30"});
            CustomDatas.Add(new CustomData() { Str1 = "许嵩深圳歌友会", Str2 = CustomData.Calculator("2019/5/11"), Str3 = "2019/5/11"});
            CustomDatas.Add(new CustomData() { Str1 = "青年晚报演唱会广州站", Str2 = CustomData.Calculator("2017/10/7"), Str3 = "2017/10/7"});
            CustomDatas.Add(new CustomData() { Str1 = "Meet", Str2 = CustomData.Calculator("2017/8/14"), Str3 = "2017/8/14" });
        }

        public void AddData(CustomData data)
        {
            CustomDatas.Add(data);
        }
    }
}
