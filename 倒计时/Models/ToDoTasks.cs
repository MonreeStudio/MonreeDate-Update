using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace 倒计时.Models
{
    public class ToDoTasks
    {
        [PrimaryKey]
        public string TaskId { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Star { get; set; }
        public string Remark { get; set; }

        public string Done { get; set; }

        public string UpdateTime { get; set; }

        public string IsDelete { get; set;
        }
        public Visibility StarVisibility { get; set; }
        public Visibility DateVisibility { get; set; }

        public Visibility RemarkVisibility { get; set; }
    }

    public class ToDoTasksViewModel
    {
        public ObservableCollection<ToDoTasks> ToDoDatas = new ObservableCollection<ToDoTasks>();

        public ToDoTasksViewModel()
        {
            //ToDoDatas.Add(new ToDoTasks() { Name = "毕业设计答辩", Date = "2020-05-02", Star = "1", Remark = "备注是", Done = "0", StarVisibility = Visibility.Collapsed});
            //ToDoDatas.Add(new ToDoTasks() { Name = "毕业设计答辩", Date = "2020-05-02", Star = "1", Remark = "备注是", Done = "0" });
            //ToDoDatas.Add(new ToDoTasks() { Name = "毕业设计答辩", Date = "2020-05-02", Star = "1", Remark = "备注是", Done = "0" });
        }
    }
}
