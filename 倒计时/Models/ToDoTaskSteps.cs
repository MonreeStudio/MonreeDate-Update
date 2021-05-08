using SQLite.Net.Attributes;
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
        [PrimaryKey]
        public string StepId { get; set; }
        public string TaskId { get; set; }

        public string Content { get; set; }

        public bool Finish { get; set; }

        public bool UnFinish { get; set; }

        public string UpdateTime { get; set; }

        public string IsDelete { get; set; }
    }

    public class ToDoTaskStepsViewModel
    {
        public ObservableCollection<ToDoTaskSteps> ToDoTaskStepsDatas = new ObservableCollection<ToDoTaskSteps>();
        public ToDoTaskStepsViewModel()
        {
        
        }
    }
}
