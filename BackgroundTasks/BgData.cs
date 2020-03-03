using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTasks
{
    public sealed class BgData
    {
        public string ScheduleName { get; set; }
        public string Date { get; set; }
        public string CalDate { get; set; }

        public BgData()
        {

        }

    }
    class BgDataViewModel
    {
        public ObservableCollection<BgData> BgDatas = new ObservableCollection<BgData>();
    }
}