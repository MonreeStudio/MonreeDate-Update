using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 倒计时.Models
{
    public class ToDoTaskSteps
    {
        public string TaskName { get; set; }

        public string Content { get; set; }

        public bool Finish { get; set; }

        public bool UnFinish { get; set; }
    }

    public class ToDoTaskStepsViewModel
    {
        public ObservableCollection<ToDoTaskSteps> ToDoTaskStepsDatas = new ObservableCollection<ToDoTaskSteps>();
        public ToDoTaskStepsViewModel()
        {
            ToDoTaskStepsDatas.Add(new ToDoTaskSteps() { TaskName = "", Content = "一个小脚印", Finish = true });
        }
    }
}
