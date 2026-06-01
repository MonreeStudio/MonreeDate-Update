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
            if (DateTime.Now.Year.ToString().Equals("2023"))
            {
                if (DateTime.Now.Date <= Convert.ToDateTime("2023-01-01"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "元旦", Str2 = CustomData.Calculator("2023-01-01"), Str3 = "2023-01-01", Str4 = Color.FromArgb(255, 241, 147, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 241, 147, 156), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "元旦", Str2 = CustomData.Calculator("2024-01-01"), Str3 = "2024-01-01", Str4 = Color.FromArgb(255, 241, 147, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 241, 147, 156), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2023-01-22"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "春节", Str2 = CustomData.Calculator("2023-01-22"), Str3 = "2023-01-22", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "春节", Str2 = CustomData.Calculator("2024-02-10"), Str3 = "2024-02-10", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2023-02-14"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "情人节❤", Str2 = CustomData.Calculator("2023-02-14"), Str3 = "2023-02-14", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "情人节❤", Str2 = CustomData.Calculator("2024-02-14"), Str3 = "2024-02-14", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2023-02-05"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "元宵节", Str2 = CustomData.Calculator("2023-02-05"), Str3 = "2023-02-05", Str4 = Color.FromArgb(255, 251, 182, 18), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 182, 18), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "元宵节", Str2 = CustomData.Calculator("2024-02-24"), Str3 = "2024-02-24", Str4 = Color.FromArgb(255, 251, 182, 18), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 182, 18), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2023-03-08"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "妇女节", Str2 = CustomData.Calculator("2023-03-08"), Str3 = "2023-03-08", Str4 = Color.FromArgb(255, 231, 124, 142), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 231, 124, 142), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "妇女节", Str2 = CustomData.Calculator("2024-03-08"), Str3 = "2024-03-08", Str4 = Color.FromArgb(255, 231, 124, 142), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 231, 124, 142), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2023-04-05"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "清明节", Str2 = CustomData.Calculator("2023-04-05"), Str3 = "2023-04-05", Str4 = Color.FromArgb(255, 18, 170, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 18, 170, 156), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "清明节", Str2 = CustomData.Calculator("2024-04-04"), Str3 = "2024-04-04", Str4 = Color.FromArgb(255, 18, 170, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 18, 170, 156), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2023-05-01"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "劳动节", Str2 = CustomData.Calculator("2023-05-01"), Str3 = "2023-05-01", Str4 = Color.FromArgb(255, 251, 185, 87), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 185, 87), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "劳动节", Str2 = CustomData.Calculator("2024-05-01"), Str3 = "2024-05-01", Str4 = Color.FromArgb(255, 251, 185, 87), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 185, 87), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2023-06-01"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "儿童节", Str2 = CustomData.Calculator("2023-06-01"), Str3 = "2023-06-01", Str4 = Colors.Orange, Str5 = All.Current.ColorfulBrush(Colors.Orange, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "儿童节", Str2 = CustomData.Calculator("2024-06-01"), Str3 = "2024-06-01", Str4 = Colors.Orange, Str5 = All.Current.ColorfulBrush(Colors.Orange, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2023-06-22"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "端午节", Str2 = CustomData.Calculator("2023-06-22"), Str3 = "2023-06-22", Str4 = Color.FromArgb(255, 105, 167, 148), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 105, 167, 148), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "端午节", Str2 = CustomData.Calculator("2024-06-10"), Str3 = "2024-06-10", Str4 = Color.FromArgb(255, 105, 167, 148), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 105, 167, 148), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2023-08-22"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "七夕❤", Str2 = CustomData.Calculator("2023-08-22"), Str3 = "2023-08-22", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "七夕❤", Str2 = CustomData.Calculator("2024-08-10"), Str3 = "2024-08-10", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2023-09-10"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "教师节", Str2 = CustomData.Calculator("2023-09-10"), Str3 = "2023-09-10", Str4 = Colors.MediumPurple, Str5 = All.Current.ColorfulBrush(Colors.MediumPurple, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "教师节", Str2 = CustomData.Calculator("2024-09-10"), Str3 = "2024-09-10", Str4 = Colors.MediumPurple, Str5 = All.Current.ColorfulBrush(Colors.MediumPurple, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2023-09-29"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "中秋节", Str2 = CustomData.Calculator("2023-09-29"), Str3 = "2023-09-29", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "中秋节", Str2 = CustomData.Calculator("2024-09-17"), Str3 = "2024-09-17", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2023-10-01"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "国庆节", Str2 = CustomData.Calculator("2023-10-01"), Str3 = "2023-10-01", Str4 = Colors.OrangeRed, Str5 = All.Current.ColorfulBrush(Colors.OrangeRed, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "国庆节", Str2 = CustomData.Calculator("2024-10-01"), Str3 = "2024-10-01", Str4 = Colors.OrangeRed, Str5 = All.Current.ColorfulBrush(Colors.OrangeRed, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2023-10-23"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "重阳节", Str2 = CustomData.Calculator("2023-10-23"), Str3 = "2023-10-23", Str4 = Colors.DarkOrange, Str5 = All.Current.ColorfulBrush(Colors.DarkOrange, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "重阳节", Str2 = CustomData.Calculator("2024-10-11"), Str3 = "2024-10-11", Str4 = Colors.DarkOrange, Str5 = All.Current.ColorfulBrush(Colors.DarkOrange, 0.7) });
                if (DateTime.Now.Date <= Convert.ToDateTime("2023-12-25"))
                    FestivalDatas.Add(new FestivalData() { Str1 = "圣诞节", Str2 = CustomData.Calculator("2023-12-25"), Str3 = "2023-12-25", Str4 = Colors.Crimson, Str5 = All.Current.ColorfulBrush(Colors.Crimson, 0.7) });
                else
                    FestivalDatas.Add(new FestivalData() { Str1 = "圣诞节", Str2 = CustomData.Calculator("2024-12-25"), Str3 = "2024-12-25", Str4 = Colors.Crimson, Str5 = All.Current.ColorfulBrush(Colors.Crimson, 0.7) });
            }
            else
            {
                if (DateTime.Now.Year.ToString().Equals("2024"))
                {
                    if (DateTime.Now.Date <= Convert.ToDateTime("2024-01-01"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "元旦", Str2 = CustomData.Calculator("2024-01-01"), Str3 = "2024-01-01", Str4 = Color.FromArgb(255, 241, 147, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 241, 147, 156), 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "元旦", Str2 = CustomData.Calculator("2025-01-01"), Str3 = "2025-01-01", Str4 = Color.FromArgb(255, 241, 147, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 241, 147, 156), 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2024-02-10"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "春节", Str2 = CustomData.Calculator("2024-02-10"), Str3 = "2024-02-10", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "春节", Str2 = CustomData.Calculator("2025-01-29"), Str3 = "2025-01-29", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2024-02-24"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "元宵节", Str2 = CustomData.Calculator("2024-02-24"), Str3 = "2024-02-24", Str4 = Color.FromArgb(255, 251, 182, 18), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 182, 18), 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "元宵节", Str2 = CustomData.Calculator("2025-02-12"), Str3 = "2025-02-12", Str4 = Color.FromArgb(255, 251, 182, 18), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 182, 18), 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2024-02-14"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "情人节❤", Str2 = CustomData.Calculator("2024-02-14"), Str3 = "2024-02-14", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "情人节❤", Str2 = CustomData.Calculator("2025-02-14"), Str3 = "2025-02-14", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2024-03-08"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "妇女节", Str2 = CustomData.Calculator("2024-03-08"), Str3 = "2024-03-08", Str4 = Color.FromArgb(255, 231, 124, 142), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 231, 124, 142), 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "妇女节", Str2 = CustomData.Calculator("2025-03-08"), Str3 = "2025-03-08", Str4 = Color.FromArgb(255, 231, 124, 142), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 231, 124, 142), 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2024-04-04"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "清明节", Str2 = CustomData.Calculator("2024-04-04"), Str3 = "2024-04-04", Str4 = Color.FromArgb(255, 18, 170, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 18, 170, 156), 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "清明节", Str2 = CustomData.Calculator("2025-04-04"), Str3 = "2025-04-04", Str4 = Color.FromArgb(255, 18, 170, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 18, 170, 156), 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2024-05-01"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "劳动节", Str2 = CustomData.Calculator("2024-05-01"), Str3 = "2024-05-01", Str4 = Color.FromArgb(255, 251, 185, 87), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 185, 87), 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "劳动节", Str2 = CustomData.Calculator("2025-05-01"), Str3 = "2025-05-01", Str4 = Color.FromArgb(255, 251, 185, 87), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 185, 87), 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2024-06-01"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "儿童节", Str2 = CustomData.Calculator("2024-06-01"), Str3 = "2024-06-01", Str4 = Colors.Orange, Str5 = All.Current.ColorfulBrush(Colors.Orange, 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "儿童节", Str2 = CustomData.Calculator("2025-06-01"), Str3 = "2025-06-01", Str4 = Colors.Orange, Str5 = All.Current.ColorfulBrush(Colors.Orange, 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2024-06-10"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "端午节", Str2 = CustomData.Calculator("2024-06-10"), Str3 = "2024-06-10", Str4 = Color.FromArgb(255, 105, 167, 148), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 105, 167, 148), 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "端午节", Str2 = CustomData.Calculator("2025-05-31"), Str3 = "2025-05-31", Str4 = Color.FromArgb(255, 105, 167, 148), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 105, 167, 148), 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2024-08-10"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "七夕❤", Str2 = CustomData.Calculator("2024-08-10"), Str3 = "2024-08-10", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "七夕❤", Str2 = CustomData.Calculator("2025-08-29"), Str3 = "2025-08-29", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2024-09-10"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "教师节", Str2 = CustomData.Calculator("2024-09-10"), Str3 = "2024-09-10", Str4 = Colors.MediumPurple, Str5 = All.Current.ColorfulBrush(Colors.MediumPurple, 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "教师节", Str2 = CustomData.Calculator("2025-09-10"), Str3 = "2025-09-10", Str4 = Colors.MediumPurple, Str5 = All.Current.ColorfulBrush(Colors.MediumPurple, 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2024-09-17"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "中秋节", Str2 = CustomData.Calculator("2024-09-17"), Str3 = "2024-09-17", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "中秋节", Str2 = CustomData.Calculator("2025-10-06"), Str3 = "2025-10-06", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2024-10-01"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "国庆节", Str2 = CustomData.Calculator("2024-10-01"), Str3 = "2024-10-01", Str4 = Colors.OrangeRed, Str5 = All.Current.ColorfulBrush(Colors.OrangeRed, 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "国庆节", Str2 = CustomData.Calculator("2025-10-01"), Str3 = "2025-10-01", Str4 = Colors.OrangeRed, Str5 = All.Current.ColorfulBrush(Colors.OrangeRed, 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2024-10-11"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "重阳节", Str2 = CustomData.Calculator("2024-10-11"), Str3 = "2024-10-11", Str4 = Colors.DarkOrange, Str5 = All.Current.ColorfulBrush(Colors.DarkOrange, 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "重阳节", Str2 = CustomData.Calculator("2025-10-29"), Str3 = "2025-10-29", Str4 = Colors.DarkOrange, Str5 = All.Current.ColorfulBrush(Colors.DarkOrange, 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2024-12-25"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "圣诞节", Str2 = CustomData.Calculator("2024-12-25"), Str3 = "2024-12-25", Str4 = Colors.Crimson, Str5 = All.Current.ColorfulBrush(Colors.Crimson, 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "圣诞节", Str2 = CustomData.Calculator("2025-12-25"), Str3 = "2025-12-25", Str4 = Colors.Crimson, Str5 = All.Current.ColorfulBrush(Colors.Crimson, 0.7) });
                }
                else
                {
                    if (DateTime.Now.Date <= Convert.ToDateTime("2025-01-01"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "元旦", Str2 = CustomData.Calculator("2025-01-01"), Str3 = "2025-01-01", Str4 = Color.FromArgb(255, 241, 147, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 241, 147, 156), 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "元旦", Str2 = CustomData.Calculator("2026-01-01"), Str3 = "2026-01-01", Str4 = Color.FromArgb(255, 241, 147, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 241, 147, 156), 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2025-01-29"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "春节", Str2 = CustomData.Calculator("2025-01-29"), Str3 = "2025-01-29", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "春节", Str2 = CustomData.Calculator("2026-02-17"), Str3 = "2026-02-17", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2025-02-12"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "元宵节", Str2 = CustomData.Calculator("2025-02-12"), Str3 = "2025-02-12", Str4 = Color.FromArgb(255, 251, 182, 18), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 182, 18), 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "元宵节", Str2 = CustomData.Calculator("2026-03-03"), Str3 = "2026-03-03", Str4 = Color.FromArgb(255, 251, 182, 18), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 182, 18), 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2025-02-14"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "情人节❤", Str2 = CustomData.Calculator("2025-02-14"), Str3 = "2025-02-14", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "情人节❤", Str2 = CustomData.Calculator("2026-02-14"), Str3 = "2026-02-14", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2025-03-08"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "妇女节", Str2 = CustomData.Calculator("2025-03-08"), Str3 = "2025-03-08", Str4 = Color.FromArgb(255, 231, 124, 142), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 231, 124, 142), 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "妇女节", Str2 = CustomData.Calculator("2026-03-08"), Str3 = "2026-03-08", Str4 = Color.FromArgb(255, 231, 124, 142), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 231, 124, 142), 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2025-04-04"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "清明节", Str2 = CustomData.Calculator("2025-04-04"), Str3 = "2025-04-04", Str4 = Color.FromArgb(255, 18, 170, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 18, 170, 156), 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "清明节", Str2 = CustomData.Calculator("2026-04-05"), Str3 = "2026-04-05", Str4 = Color.FromArgb(255, 18, 170, 156), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 18, 170, 156), 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2025-05-01"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "劳动节", Str2 = CustomData.Calculator("2025-05-01"), Str3 = "2025-05-01", Str4 = Color.FromArgb(255, 251, 185, 87), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 185, 87), 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "劳动节", Str2 = CustomData.Calculator("2026-05-01"), Str3 = "2026-05-01", Str4 = Color.FromArgb(255, 251, 185, 87), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 251, 185, 87), 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2025-06-01"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "儿童节", Str2 = CustomData.Calculator("2025-06-01"), Str3 = "2025-06-01", Str4 = Colors.Orange, Str5 = All.Current.ColorfulBrush(Colors.Orange, 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "儿童节", Str2 = CustomData.Calculator("2026-06-01"), Str3 = "2026-06-01", Str4 = Colors.Orange, Str5 = All.Current.ColorfulBrush(Colors.Orange, 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2025-05-31"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "端午节", Str2 = CustomData.Calculator("2025-05-31"), Str3 = "2025-05-31", Str4 = Color.FromArgb(255, 105, 167, 148), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 105, 167, 148), 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "端午节", Str2 = CustomData.Calculator("2026-06-19"), Str3 = "2026-06-19", Str4 = Color.FromArgb(255, 105, 167, 148), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 105, 167, 148), 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2025-08-29"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "七夕❤", Str2 = CustomData.Calculator("2025-08-29"), Str3 = "2025-08-29", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "七夕❤", Str2 = CustomData.Calculator("2026-08-19"), Str3 = "2026-08-19", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2025-09-10"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "教师节", Str2 = CustomData.Calculator("2025-09-10"), Str3 = "2025-09-10", Str4 = Colors.MediumPurple, Str5 = All.Current.ColorfulBrush(Colors.MediumPurple, 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "教师节", Str2 = CustomData.Calculator("2026-09-10"), Str3 = "2026-09-10", Str4 = Colors.MediumPurple, Str5 = All.Current.ColorfulBrush(Colors.MediumPurple, 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2025-10-06"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "中秋节", Str2 = CustomData.Calculator("2025-10-06"), Str3 = "2025-10-06", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "中秋节", Str2 = CustomData.Calculator("2026-09-25"), Str3 = "2026-09-25", Str4 = Color.FromArgb(255, 166, 27, 41), Str5 = All.Current.ColorfulBrush(Color.FromArgb(255, 166, 27, 41), 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2025-10-01"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "国庆节", Str2 = CustomData.Calculator("2025-10-01"), Str3 = "2025-10-01", Str4 = Colors.OrangeRed, Str5 = All.Current.ColorfulBrush(Colors.OrangeRed, 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "国庆节", Str2 = CustomData.Calculator("2026-10-01"), Str3 = "2026-10-01", Str4 = Colors.OrangeRed, Str5 = All.Current.ColorfulBrush(Colors.OrangeRed, 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2025-10-29"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "重阳节", Str2 = CustomData.Calculator("2025-10-29"), Str3 = "2025-10-29", Str4 = Colors.DarkOrange, Str5 = All.Current.ColorfulBrush(Colors.DarkOrange, 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "重阳节", Str2 = CustomData.Calculator("2026-10-18"), Str3 = "2026-10-18", Str4 = Colors.DarkOrange, Str5 = All.Current.ColorfulBrush(Colors.DarkOrange, 0.7) });
                    if (DateTime.Now.Date <= Convert.ToDateTime("2025-12-25"))
                        FestivalDatas.Add(new FestivalData() { Str1 = "圣诞节", Str2 = CustomData.Calculator("2025-12-25"), Str3 = "2025-12-25", Str4 = Colors.Crimson, Str5 = All.Current.ColorfulBrush(Colors.Crimson, 0.7) });
                    else
                        FestivalDatas.Add(new FestivalData() { Str1 = "圣诞节", Str2 = CustomData.Calculator("2026-12-25"), Str3 = "2026-12-25", Str4 = Colors.Crimson, Str5 = All.Current.ColorfulBrush(Colors.Crimson, 0.7) });
                }
            }
        }
    }
}
