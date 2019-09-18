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
            FestivalDatas.Add(new FestivalData() { Str1 = "元旦", Str2 = CustomData.Calculator("2019-01-01"), Str3 = "2019-01-01", Str4 = Colors.PaleVioletRed, Str5 = All.Current.ColorfulBrush(Colors.PaleVioletRed,0.7) });
            FestivalDatas.Add(new FestivalData() { Str1 = "春节", Str2 = CustomData.Calculator("2019-02-05"), Str3 = "2019-02-05", Str4 = Colors.OrangeRed, Str5 = All.Current.ColorfulBrush(Colors.OrangeRed, 0.7) });
            FestivalDatas.Add(new FestivalData() { Str1 = "情人节❤", Str2 = CustomData.Calculator("2019-02-14"), Str3 = "2019-02-14", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
            FestivalDatas.Add(new FestivalData() { Str1 = "元宵节", Str2 = CustomData.Calculator("2019-02-19"), Str3 = "2019-02-19", Str4 = Colors.Gold, Str5 = All.Current.ColorfulBrush(Colors.Gold, 0.7) });
            FestivalDatas.Add(new FestivalData() { Str1 = "妇女节", Str2 = CustomData.Calculator("2019-03-08"), Str3 = "2019-03-08", Str4 = Colors.HotPink, Str5 = All.Current.ColorfulBrush(Colors.HotPink, 0.7) });
            FestivalDatas.Add(new FestivalData() { Str1 = "清明节", Str2 = CustomData.Calculator("2019-04-05"), Str3 = "2019-04-05", Str4 = Colors.LightGreen, Str5 = All.Current.ColorfulBrush(Colors.LightGreen, 0.7) });
            FestivalDatas.Add(new FestivalData() { Str1 = "劳动节", Str2 = CustomData.Calculator("2019-05-01"), Str3 = "2019-05-01", Str4 = Colors.Gold, Str5 = All.Current.ColorfulBrush(Colors.Gold, 0.7) });
            FestivalDatas.Add(new FestivalData() { Str1 = "儿童节", Str2 = CustomData.Calculator("2019-06-01"), Str3 = "2019-06-01", Str4 = Colors.Orange, Str5 = All.Current.ColorfulBrush(Colors.Orange, 0.7) });
            FestivalDatas.Add(new FestivalData() { Str1 = "端午节", Str2 = CustomData.Calculator("2019-06-07"), Str3 = "2019-06-07", Str4 = Colors.ForestGreen, Str5 = All.Current.ColorfulBrush(Colors.ForestGreen, 0.7) });
            FestivalDatas.Add(new FestivalData() { Str1 = "七夕❤", Str2 = CustomData.Calculator("2019-08-07"), Str3 = "2019-08-07", Str4 = Colors.LightPink, Str5 = All.Current.ColorfulBrush(Colors.LightPink, 0.7) });
            FestivalDatas.Add(new FestivalData() { Str1 = "中秋节", Str2 = CustomData.Calculator("2019-09-13"), Str3 = "2019-09-13", Str4 = Colors.MediumPurple, Str5 = All.Current.ColorfulBrush(Colors.MediumPurple, 0.7) });
            FestivalDatas.Add(new FestivalData() { Str1 = "国庆节", Str2 = CustomData.Calculator("2019-10-01"), Str3 = "2019-10-01", Str4 = Colors.OrangeRed, Str5 = All.Current.ColorfulBrush(Colors.OrangeRed, 0.7) });
            FestivalDatas.Add(new FestivalData() { Str1 = "重阳节", Str2 = CustomData.Calculator("2019-10-07"), Str3 = "2019-10-07", Str4 = Colors.DarkOrange, Str5 = All.Current.ColorfulBrush(Colors.DarkOrange, 0.7) });
            FestivalDatas.Add(new FestivalData() { Str1 = "圣诞节", Str2 = CustomData.Calculator("2019-12-25"), Str3 = "2019-12-25", Str4 = Colors.Crimson, Str5 = All.Current.ColorfulBrush(Colors.Crimson, 0.7) });
        }
        
    }
}
