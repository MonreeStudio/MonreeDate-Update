using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 倒计时
{
    public class ToolData
    {
        public string ScheduleName { get; set; }
        public string Date { get; set; }
        public string CalDate { get; set; }
        public ToolData()
        {
            
        }
    }

    public class ToolDataViewModel
    {
        public ObservableCollection<ToolData> ToolDatas = new ObservableCollection<ToolData>();

        public ToolDataViewModel()
        {
            
        }
    }
}
