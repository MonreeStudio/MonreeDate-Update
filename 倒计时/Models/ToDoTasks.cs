using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 倒计时.Models
{
    public class ToDoTasks
    {
        public string ToDoTaskName { get; set; }
    }

    public class ToDoTasksViewModel
    {
        public ObservableCollection<ToDoTasks> ToDoDatas = new ObservableCollection<ToDoTasks>();

        public ToDoTasksViewModel()
        {
            ToDoDatas.Add(new ToDoTasks() { ToDoTaskName = "毕业设计答辩" });
            ToDoDatas.Add(new ToDoTasks() { ToDoTaskName = "毕业设计答辩" });
            ToDoDatas.Add(new ToDoTasks() { ToDoTaskName = "毕业设计答辩" });
            
        }
    }
}
