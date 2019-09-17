using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;


namespace SQLiteBackground
{
    public sealed class TableTemplate
    {
        [PrimaryKey]
        public string Schedule_name { get; set; }
        public string CalculatedDate { get; set; }
        public string Date { get; set; }
        public string BgColor { get; set; }
        public double TintOpacity { get; set; }
        public string IsTop { get; set; }
        public string AddTime { get; set; }
    }
}
