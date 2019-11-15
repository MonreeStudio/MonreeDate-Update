using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLitePCL;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Media;
using 夏日.Models;

namespace 倒计时
{
    public class CustomData
    {
        public string Str1 { get; set; }
        public string Str2 { get; set; }
        public string Str3 { get; set; }

        public AcrylicBrush Str4 { get; set; }
        public int ItemWidth { get; set; }
        public Color BackGroundColor { get; set; }

        public CustomData()
        {
            this.Str1 = "string 1";
            this.Str2 = "string 2";
            this.Str3 = "string 3";
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
                s3 = "已过" + App.Term(Convert.ToDateTime(d4), Convert.ToDateTime(d3));
                days = Math.Abs(days);
                s2 = "已过" + days.ToString() + "天";
            }
            else
            {
                s3 = "还有" + App.Term(Convert.ToDateTime(d3), Convert.ToDateTime(d4));
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
            {
                if (days != 0)
                    s2 = "已过" + days.ToString() + "天";
                else
                {
                    s2 = "就在今天";
                }
             }
            return s2;
        }
    }

    public class CustomDataViewModel
    {
        public ObservableCollection<CustomData> CustomDatas = new ObservableCollection<CustomData>();
        public string Today = DateTime.Now.ToShortDateString().ToString();

        public CustomDataViewModel()
        {        
           
        }
    }
}
