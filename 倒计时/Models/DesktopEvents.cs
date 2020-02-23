using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 倒计时.Models
{
    public class DesktopEvents
    {
        public string EventName { get; set; }
    }

    public class DesktopEventsViewModel
    {
        public ObservableCollection<DesktopEvents> DesktopDatas = new ObservableCollection<DesktopEvents>();
        public DesktopEventsViewModel()
        {

        }
    }
}
