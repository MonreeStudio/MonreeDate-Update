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
            ThemeColorDatas.Add(new ThemeColorData() { colorName = "    天空蓝", themeColor = new SolidColorBrush(Colors.DeepSkyBlue) });
            ThemeColorDatas.Add(new ThemeColorData() { colorName = "    寻宝橙", themeColor = new SolidColorBrush(Colors.Orange) });
            ThemeColorDatas.Add(new ThemeColorData() { colorName = "    魅焰红", themeColor = new SolidColorBrush(Colors.Crimson) });
            ThemeColorDatas.Add(new ThemeColorData() { colorName = "    高冷灰", themeColor = new SolidColorBrush(Color.FromArgb(255, 73, 92, 105)) });
            ThemeColorDatas.Add(new ThemeColorData() { colorName = "    高贵紫", themeColor = new SolidColorBrush(Color.FromArgb(255, 119, 25, 171)) });
        }
    }
}
