using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using 倒计时;
using 夏日;
using 夏日.Models;

namespace 倒计时
{
    public class FestivalData
    {
        public string Str1 { get; set; }
        public string Str2 { get; set; }
        public string Str3 { get; set; }
        public Color Str4 { get; set; }
        public AcrylicBrush Str5 { get; set; }
        public string BackGroundColor { get; set; }

        public static FestivalData Current;
        public FestivalData()
        {
            Current = this;
            this.Str1 = "string 1";
            this.Str2 = "string 2";
            this.Str3 = "string 3";
            this.BackGroundColor = "SkyBlue";
        }

        
    }

    public class FestivalDataViewModel
    {
        public ObservableCollection<FestivalData> FestivalDatas = new ObservableCollection<FestivalData>();
        public FestivalDataViewModel()
        {
            if (DateTime.Now.Year.ToString().Equals("2019"))
            {
                if(DateTime.Now.Date <= Convert.ToDateTime("2019-01-01"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "元旦", Str2 = CustomData.Calculator("2019-01-01"), Str3 = "2019-01-01", Str4 = Color.FromArgb(255,241,147,156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 241, 147, 156), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "元旦", Str2 = CustomData.Calculator("2020-01-01"), Str3 = "2020-01-01", Str4 = Color.FromArgb(255, 241, 147, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 241, 147, 156), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2019-02-05"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "春节", Str2 = CustomData.Calculator("2019-02-05"), Str3 = "2019-02-05", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "春节", Str2 = CustomData.Calculator("2020-01-25"), Str3 = "2020-01-25", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2019-02-14"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "情人节❤", Str2 = CustomData.Calculator("2019-02-14"), Str3 = "2019-02-14", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "情人节❤", Str2 = CustomData.Calculator("2020-02-14"), Str3 = "2020-02-14", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2019-02-19"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "元宵节", Str2 = CustomData.Calculator("2019-02-19"), Str3 = "2019-02-19", Str4 = Color.FromArgb(255, 251, 182, 18), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 182, 18), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "元宵节", Str2 = CustomData.Calculator("2020-02-08"), Str3 = "2020-02-08", Str4 = Color.FromArgb(255, 251, 182, 18), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 182, 18), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2019-03-08"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "妇女节", Str2 = CustomData.Calculator("2019-03-08"), Str3 = "2019-03-08", Str4 = Color.FromArgb(255, 231, 124, 142), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 231, 124, 142), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "妇女节", Str2 = CustomData.Calculator("2020-03-08"), Str3 = "2020-03-08", Str4 = Color.FromArgb(255, 231, 124, 142), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 231, 124, 142), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2019-04-05"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "清明节", Str2 = CustomData.Calculator("2019-04-05"), Str3 = "2019-04-05", Str4 = Color.FromArgb(255, 18, 170, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 18, 170, 156), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "清明节", Str2 = CustomData.Calculator("2020-04-04"), Str3 = "2020-04-04", Str4 = Color.FromArgb(255, 18, 170, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 18, 170, 156), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2019-05-01"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "劳动节", Str2 = CustomData.Calculator("2019-05-01"), Str3 = "2019-05-01", Str4 = Color.FromArgb(255, 251, 185, 87), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 185, 87), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "劳动节", Str2 = CustomData.Calculator("2020-05-01"), Str3 = "2020-05-01", Str4 = Color.FromArgb(255, 251, 185, 87), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 185, 87), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2019-06-01"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "儿童节", Str2 = CustomData.Calculator("2019-06-01"), Str3 = "2019-06-01", Str4 = Colors.Orange, Str5 = All.Current.ColorfulBrush(Colors.Orange, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "儿童节", Str2 = CustomData.Calculator("2020-06-01"), Str3 = "2020-06-01", Str4 = Colors.Orange, Str5 = All.Current.ColorfulBrush(Colors.Orange, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2019-06-07"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "端午节", Str2 = CustomData.Calculator("2019-06-07"), Str3 = "2019-06-07", Str4 = Color.FromArgb(255, 105, 167, 148), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 105, 167, 148), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "端午节", Str2 = CustomData.Calculator("2020-06-25"), Str3 = "2020-06-25", Str4 = Color.FromArgb(255, 105, 167, 148), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 105, 167, 148), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2019-08-07"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "七夕❤", Str2 = CustomData.Calculator("2019-08-07"), Str3 = "2019-08-07", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "七夕❤", Str2 = CustomData.Calculator("2020-08-25"), Str3 = "2020-08-25", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2020-09-10"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "教师节", Str2 = CustomData.Calculator("2019-09-10"), Str3 = "2019-09-10", Str4 = Colors.MediumPurple, Str5 = All.Current.ColorfulBrush(Colors.MediumPurple, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "教师节", Str2 = CustomData.Calculator("2020-09-10"), Str3 = "2020-09-10", Str4 = Colors.MediumPurple, Str5 = All.Current.ColorfulBrush(Colors.MediumPurple, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2019-09-13"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "中秋节", Str2 = CustomData.Calculator("2019-09-13"), Str3 = "2019-09-13", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "中秋节", Str2 = CustomData.Calculator("2020-10-01"), Str3 = "2020-10-01", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2019-10-01"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "国庆节", Str2 = CustomData.Calculator("2019-10-01"), Str3 = "2019-10-01", Str4 = Colors.OrangeRed, Str5 = All.Current.ColorfulBrush(Colors.OrangeRed, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "国庆节", Str2 = CustomData.Calculator("2020-10-01"), Str3 = "2020-10-01", Str4 = Colors.OrangeRed, Str5 = All.Current.ColorfulBrush(Colors.OrangeRed, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2019-10-07"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "重阳节", Str2 = CustomData.Calculator("2019-10-07"), Str3 = "2019-10-07", Str4 = Colors.DarkOrange, Str5 = All.Current.ColorfulBrush(Colors.DarkOrange, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "重阳节", Str2 = CustomData.Calculator("2020-10-25"), Str3 = "2020-10-25", Str4 = Colors.DarkOrange, Str5 = All.Current.ColorfulBrush(Colors.DarkOrange, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2019-12-25"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "圣诞节", Str2 = CustomData.Calculator("2019-12-25"), Str3 = "2019-12-25", Str4 = Colors.Crimson, Str5 = All.Current.ColorfulBrush(Colors.Crimson, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "圣诞节", Str2 = CustomData.Calculator("2020-12-25"), Str3 = "2020-12-25", Str4 = Colors.Crimson, Str5 = All.Current.ColorfulBrush(Colors.Crimson, 0.7) });
            }
            else
            {
                if (DateTime.Now.Date <= Convert.ToDateTime("2020-01-01"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "元旦", Str2 = CustomData.Calculator("2020-01-01"), Str3 = "2020-01-01", Str4 = Color.FromArgb(255, 241, 147, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 241, 147, 156), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "元旦", Str2 = CustomData.Calculator("2021-01-01"), Str3 = "2021-01-01", Str4 = Color.FromArgb(255, 241, 147, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 241, 147, 156), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2020-01-25"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "春节", Str2 = CustomData.Calculator("2020-01-25"), Str3 = "2020-01-25", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "春节", Str2 = CustomData.Calculator("2021-02-12"), Str3 = "2021-02-12", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2020-02-08"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "元宵节", Str2 = CustomData.Calculator("2020-02-08"), Str3 = "2020-02-08", Str4 = Color.FromArgb(255, 251, 182, 18), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 182, 18), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "元宵节", Str2 = CustomData.Calculator("2021-02-26"), Str3 = "2021-02-26", Str4 = Color.FromArgb(255, 251, 182, 18), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 182, 18), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2020-02-14"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "情人节❤", Str2 = CustomData.Calculator("2020-02-14"), Str3 = "2020-02-14", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "情人节❤", Str2 = CustomData.Calculator("2021-02-14"), Str3 = "2021-02-14", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2020-03-08"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "妇女节", Str2 = CustomData.Calculator("2020-03-08"), Str3 = "2020-03-08", Str4 = Color.FromArgb(255, 231, 124, 142), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 231, 124, 142), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "妇女节", Str2 = CustomData.Calculator("2021-03-08"), Str3 = "2021-03-08", Str4 = Color.FromArgb(255, 231, 124, 142), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 231, 124, 142), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2020-04-04"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "清明节", Str2 = CustomData.Calculator("2020-04-04"), Str3 = "2020-04-04", Str4 = Color.FromArgb(255, 18, 170, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 18, 170, 156), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "清明节", Str2 = CustomData.Calculator("2021-04-04"), Str3 = "2021-04-04", Str4 = Color.FromArgb(255, 18, 170, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 18, 170, 156), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2020-05-01"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "劳动节", Str2 = CustomData.Calculator("2020-05-01"), Str3 = "2020-05-01", Str4 = Color.FromArgb(255, 251, 185, 87), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 185, 87), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "劳动节", Str2 = CustomData.Calculator("2021-05-01"), Str3 = "2021-05-01", Str4 = Color.FromArgb(255, 251, 185, 87), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 185, 87), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2019-12-25"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "儿童节", Str2 = CustomData.Calculator("2020-06-01"), Str3 = "2020-06-01", Str4 = Colors.Orange, Str5 = All.Current.ColorfulBrush(Colors.Orange, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "儿童节", Str2 = CustomData.Calculator("2021-06-01"), Str3 = "2021-06-01", Str4 = Colors.Orange, Str5 = All.Current.ColorfulBrush(Colors.Orange, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2020-06-25"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "端午节", Str2 = CustomData.Calculator("2020-06-25"), Str3 = "2020-06-25", Str4 = Color.FromArgb(255, 105, 167, 148), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 105, 167, 148), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "端午节", Str2 = CustomData.Calculator("2021-06-14"), Str3 = "2021-06-14", Str4 = Color.FromArgb(255, 105, 167, 148), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 105, 167, 148), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2020-08-25"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "七夕❤", Str2 = CustomData.Calculator("2020-08-25"), Str3 = "2020-08-25", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "七夕❤", Str2 = CustomData.Calculator("2021-08-14"), Str3 = "2021-08-14", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2020-09-10"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "教师节", Str2 = CustomData.Calculator("2020-09-10"), Str3 = "2020-09-10", Str4 = Colors.MediumPurple, Str5 = All.Current.ColorfulBrush(Colors.Gold, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "教师节", Str2 = CustomData.Calculator("2021-09-10"), Str3 = "2021-09-10", Str4 = Colors.MediumPurple, Str5 = All.Current.ColorfulBrush(Colors.Gold, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2020-10-01"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "中秋节", Str2 = CustomData.Calculator("2020-10-01"), Str3 = "2020-10-01", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "中秋节", Str2 = CustomData.Calculator("2021-09-01"), Str3 = "2021-09-21", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2020-10-01"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "国庆节", Str2 = CustomData.Calculator("2020-10-01"), Str3 = "2020-10-01", Str4 = Colors.OrangeRed, Str5 = All.Current.ColorfulBrush(Colors.OrangeRed, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "国庆节", Str2 = CustomData.Calculator("2021-10-01"), Str3 = "2021-10-01", Str4 = Colors.OrangeRed, Str5 = All.Current.ColorfulBrush(Colors.OrangeRed, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2020-10-25"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "重阳节", Str2 = CustomData.Calculator("2020-10-25"), Str3 = "2020-10-25", Str4 = Colors.DarkOrange, Str5 = All.Current.ColorfulBrush(Colors.DarkOrange, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "重阳节", Str2 = CustomData.Calculator("2021-10-14"), Str3 = "2021-10-14", Str4 = Colors.DarkOrange, Str5 = All.Current.ColorfulBrush(Colors.DarkOrange, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2020-12-25"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "圣诞节", Str2 = CustomData.Calculator("2020-12-25"), Str3 = "2020-12-25", Str4 = Colors.Crimson, Str5 = All.Current.ColorfulBrush(Colors.Crimson, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "圣诞节", Str2 = CustomData.Calculator("2021-12-25"), Str3 = "2021-12-25", Str4 = Colors.Crimson, Str5 = All.Current.ColorfulBrush(Colors.Crimson, 0.7) });
            }
        } 
    }
}
