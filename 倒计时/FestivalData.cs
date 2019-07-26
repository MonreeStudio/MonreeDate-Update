using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using 倒计时;

namespace 夏日
{
    public class FestivalData
    {
        public string Str1 { get; set; }
        public string Str2 { get; set; }
        public string Str3 { get; set; }
        public Color Str4 { get; set; }
        public Brush Str5 { get; set; }
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
            FestivalDatas.Add(new FestivalData() { Str1 = "元旦", Str2 = CustomData.Calculator("2019/1/1"), Str3 = "2019/1/1",Str4 = Colors.PaleVioletRed ,Str5 = new SolidColorBrush(Colors.PaleVioletRed) });
            FestivalDatas.Add(new FestivalData() { Str1 = "春节", Str2 = CustomData.Calculator("2019/2/5"), Str3 = "2019/2/5", Str4 = Colors.OrangeRed, Str5 = new SolidColorBrush(Colors.OrangeRed) });
            FestivalDatas.Add(new FestivalData() { Str1 = "情人节❤", Str2 = CustomData.Calculator("2019/2/14"), Str3 = "2019/2/14", Str4 = Colors.LightPink, Str5 = new SolidColorBrush(Colors.LightPink) });
            FestivalDatas.Add(new FestivalData() { Str1 = "元宵节", Str2 = CustomData.Calculator("2019/2/19"), Str3 = "2019/2/19", Str4 = Colors.Gold, Str5 = new SolidColorBrush(Colors.Gold) });
            FestivalDatas.Add(new FestivalData() { Str1 = "妇女节", Str2 = CustomData.Calculator("2019/3/8"), Str3 = "2019/3/8", Str4 = Colors.HotPink, Str5 = new SolidColorBrush(Colors.HotPink) });
            FestivalDatas.Add(new FestivalData() { Str1 = "清明节", Str2 = CustomData.Calculator("2019/4/5"), Str3 = "2019/4/5", Str4 = Colors.LightGreen, Str5 = new SolidColorBrush(Colors.LightGreen) });
            FestivalDatas.Add(new FestivalData() { Str1 = "劳动节", Str2 = CustomData.Calculator("2019/5/1"), Str3 = "2019/5/1", Str4 = Colors.Gold, Str5 = new SolidColorBrush(Colors.Gold) });
            FestivalDatas.Add(new FestivalData() { Str1 = "儿童节", Str2 = CustomData.Calculator("2019/6/1"), Str3 = "2019/6/1", Str4 = Colors.Orange, Str5 = new SolidColorBrush(Colors.Orange) });
            FestivalDatas.Add(new FestivalData() { Str1 = "端午节", Str2 = CustomData.Calculator("2019/6/7"), Str3 = "2019/6/7", Str4 = Colors.ForestGreen, Str5 = new SolidColorBrush(Colors.ForestGreen) });
            FestivalDatas.Add(new FestivalData() { Str1 = "七夕❤", Str2 = CustomData.Calculator("2019/8/7"), Str3 = "2019/8/7", Str4 = Colors.LightPink, Str5 = new SolidColorBrush(Colors.LightPink) });
            FestivalDatas.Add(new FestivalData() { Str1 = "中秋节", Str2 = CustomData.Calculator("2019/9/13"), Str3 = "2019/9/13", Str4 = Colors.MediumPurple, Str5 = new SolidColorBrush(Colors.MediumPurple) });
            FestivalDatas.Add(new FestivalData() { Str1 = "国庆节", Str2 = CustomData.Calculator("2019/10/1"), Str3 = "2019/10/1", Str4 = Colors.OrangeRed, Str5 = new SolidColorBrush(Colors.OrangeRed) });
            FestivalDatas.Add(new FestivalData() { Str1 = "重阳节", Str2 = CustomData.Calculator("2019/10/7"), Str3 = "2019/10/7", Str4 = Colors.DarkOrange, Str5 = new SolidColorBrush(Colors.DarkOrange) });
            FestivalDatas.Add(new FestivalData() { Str1 = "圣诞节", Str2 = CustomData.Calculator("2019/12/25"), Str3 = "2019/12/25", Str4 = Colors.Crimson, Str5 = new SolidColorBrush(Colors.Crimson) });
        }
        
    }
}
