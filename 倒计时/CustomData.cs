using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 倒计时
{
    public class CustomData
    {
        public string Str1 { get; set; }
        public string Str2 { get; set; }
        public string Str3 { get; set; }
        public int ItemWidth { get; set; }
        public string BackGroundColor { get; set; }

        public CustomData()
        {
            this.Str1 = "string 1";
            this.Str2 = "string 2";
            this.Str3 = "string 3";
            this.BackGroundColor = "SkyBlue";
        }

        static public CustomData DateCalculator(string D, string E)
        {
            string s1 = E;
            string s2;
            string d = D;
            string str2 = DateTime.Now.ToShortDateString().ToString();
            DateTime d1 = Convert.ToDateTime(d);
            DateTime d2 = Convert.ToDateTime(str2);
            DateTime d3 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d1.Year, d1.Month, d1.Day));
            DateTime d4 = Convert.ToDateTime(string.Format("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day));
            int days = (d4 - d3).Days;
            if (days < 0)
            {
                days = Math.Abs(days);
                s2 = "已过" + days.ToString() + "天";
            }
            else
                s2 = "还有" + days.ToString() + "天";
            return new CustomData()
            {
                Str1 = s1,
                Str2 = s2,
                Str3 = d
            };
        }
    }

    public class CustomDataViewModel
    {
        public ObservableCollection<CustomData> CustomDatas = new ObservableCollection<CustomData>();

        public CustomDataViewModel()
        {
            CustomDatas.Add(new CustomData() { Str1 = "测试测试", Str2 = "已过xx天", Str3 = "201X/XX/XX", BackGroundColor = "SkyBlue" });
            CustomDatas.Add(new CustomData() { Str1 = "测试测试", Str2 = "已过xx天", Str3 = "201X/XX/XX", BackGroundColor = "Pink" });
            CustomDatas.Add(new CustomData() { Str1 = "测试测试", Str2 = "已过xx天", Str3 = "201X/XX/XX", BackGroundColor = "CornFlowerBlue" });
            CustomDatas.Add(new CustomData() { Str1 = "测试测试", Str2 = "已过xx天", Str3 = "201X/XX/XX", BackGroundColor = "Lavender" });
            CustomDatas.Add(new CustomData() { Str1 = "测试测试", Str2 = "已过xx天", Str3 = "201X/XX/XX", BackGroundColor = "Azure" });
            CustomDatas.Add(new CustomData() { Str1 = "测试测试", Str2 = "已过xx天", Str3 = "201X/XX/XX", BackGroundColor = "Purple" });
        }

        public void AddData(CustomData data)
        {
            CustomDatas.Add(data);
        }
    }
}
