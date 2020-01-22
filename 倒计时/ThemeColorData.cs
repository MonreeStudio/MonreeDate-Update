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
    public class ThemeColorData
    {
        public SolidColorBrush themeColor { get; set; }
        public string colorName { get; set; }
        public static ThemeColorData Current;
        public ThemeColorData()
        {
            Current = this;
        }
    }

    public class ThemeColorDataViewModel
    {
        public ObservableCollection<ThemeColorData> ThemeColorDatas = new ObservableCollection<ThemeColorData>();
        public ThemeColorDataViewModel()
        {
            ThemeColorDatas.Add(new ThemeColorData() { colorName = "    夏日蓝", themeColor = new SolidColorBrush(Colors.CornflowerBlue) });
            ThemeColorDatas.Add(new ThemeColorData() { colorName = "    天空蓝", themeColor = new SolidColorBrush(Color.FromArgb(255, 2, 136, 235)) });
            ThemeColorDatas.Add(new ThemeColorData() { colorName = "    寻宝橙", themeColor = new SolidColorBrush(Color.FromArgb(255, 229, 103, 44)) });
            ThemeColorDatas.Add(new ThemeColorData() { colorName = "    魅焰红", themeColor = new SolidColorBrush(Colors.Crimson) });
            ThemeColorDatas.Add(new ThemeColorData() { colorName = "    高冷灰", themeColor = new SolidColorBrush(Color.FromArgb(255, 73, 92, 105)) });
            ThemeColorDatas.Add(new ThemeColorData() { colorName = "    高雅紫", themeColor = new SolidColorBrush(Color.FromArgb(255, 119, 25, 171)) });
            ThemeColorDatas.Add(new ThemeColorData() { colorName = "    治愈粉", themeColor = new SolidColorBrush(Color.FromArgb(255, 236, 155, 173)) });
            ThemeColorDatas.Add(new ThemeColorData() { colorName = "    清新绿", themeColor = new SolidColorBrush(Color.FromArgb(255, 124, 178, 56)) });
            ThemeColorDatas.Add(new ThemeColorData() { colorName = "    翡冷翠", themeColor = new SolidColorBrush(Color.FromArgb(255, 8, 128, 126)) });
            ThemeColorDatas.Add(new ThemeColorData() { colorName = "    咖啡褐", themeColor = new SolidColorBrush(Color.FromArgb(255, 183, 133, 108)) });
        }
    }
}
