using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 倒计时.Models
{
    public class IntroPage
    {
        public string coverImage { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }

    public class IntroPageViewModel
    {
        public ObservableCollection<IntroPage> IntroImage = new ObservableCollection<IntroPage>();
        public IntroPageViewModel()
        {
            IntroImage.Add(new IntroPage() { height = 300, width = 300, coverImage = "Assets/intro1.png" });
            IntroImage.Add(new IntroPage() { height = 300, width = 300, coverImage = "Assets/intro2.png" });
            IntroImage.Add(new IntroPage() { height = 800, width = 800, coverImage = "Assets/intro3.png" });
            IntroImage.Add(new IntroPage() { height = 800, width = 800, coverImage = "Assets/intro4.png" });
        }
    }
}
