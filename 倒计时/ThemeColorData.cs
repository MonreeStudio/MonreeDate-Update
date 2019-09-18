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
            ThemeColorDatas.Add(new ThemeColorData() { colorName = "夏日蓝", themeColor = new SolidColorBrush(Colors.CornflowerBlue) });
        }
    }
}
