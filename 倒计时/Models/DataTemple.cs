using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLitePCL;
using SQLite.Net.Attributes;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace 夏日.Models
{
    class DataTemple
    {
        [PrimaryKey]
        public string Schedule_name { get; set; }
        public string CalculatedDate { get; set; }
        public string Date { get; set; }
        public string BgColor { get; set; }
        public double TintOpacity { get; set; }
        
    }
}
