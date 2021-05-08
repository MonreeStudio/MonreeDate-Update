using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using 倒计时.Models;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 倒计时
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ToDo : Page
    {
        public double MinMyNav = MainPage.Current.MyNav.CompactModeThresholdWidth;
        public ToDoTasksViewModel ToDoTaskViewModel1 = new ToDoTasksViewModel();
        public ToDoTasksViewModel ToDoTaskViewModel2 = new ToDoTasksViewModel();
        public ToDoTaskStepsViewModel ToDoTaskStepsViewModel = new ToDoTaskStepsViewModel();

        string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "mydb.sqlite");    //建立数据库  
        public SQLite.Net.SQLiteConnection conn;

        private ToDoTasks CurrentItem;
        private ToDoTaskSteps CurrentTaskStep;

        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public ThemeColorDataViewModel ViewModel = new ThemeColorDataViewModel();
        public ToDo()
        {
            this.InitializeComponent();
            // 建立数据库连接
            string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "mydb.sqlite");    //建立数据库  

            conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), path);
            //建表              
            conn.CreateTable<ToDoTasks>(); //默认表名同范型参数 
            conn.CreateTable<ToDoTaskSteps>();
            SetThemeColor();
            LoadData();
            MainPage.Current.MyNav.IsBackEnabled = true;
            MainPage.Current.SelectedPageItem = "ToDo";

        }

        public void SetThemeColor()
        {
            if (localSettings.Values["ThemeColor"] == null)
                localSettings.Values["ThemeColor"] = "CornflowerBlue";
            switch (localSettings.Values["ThemeColor"].ToString())
            {
                case "CornflowerBlue":
                    TC.Color = Colors.CornflowerBlue;
                    break;
                case "DeepSkyBlue":
                    TC.Color = Color.FromArgb(255, 2, 136, 235);
                    break;
                case "Orange":
                    TC.Color = Color.FromArgb(255, 229, 103, 44);
                    break;
                case "Crimson":
                    TC.Color = Colors.Crimson;
                    break;
                case "Gray":
                    TC.Color = Color.FromArgb(255, 73, 92, 105);
                    break;
                case "Purple":
                    TC.Color = Color.FromArgb(255, 119, 25, 171);
                    break;
                case "Pink":
                    TC.Color = Color.FromArgb(255, 239, 130, 160);
                    break;
                case "Green":
                    TC.Color = Color.FromArgb(255, 124, 178, 56);
                    break;
                case "DeepGreen":
                    TC.Color = Color.FromArgb(255, 8, 128, 126);
                    break;
                case "Coffee":
                    TC.Color = Color.FromArgb(255, 183, 133, 108);
                    break;
                default:
                    break;
            }
        }

        private void LoadData()
        {
            ToDoTaskViewModel1.ToDoDatas.Clear();
            ToDoTaskViewModel2.ToDoDatas.Clear();
            List<ToDoTasks> datalist0 = conn.Query<ToDoTasks>("select * from ToDoTasks where Done = ? and IsDelete = ?", "0", "0");
            List<ToDoTasks> datalist1 = conn.Query<ToDoTasks>("select * from ToDoTasks where Done = ? and IsDelete = ? order by UpdateTime desc", 1, "0");
            foreach (var item in datalist0)
            {
                if(item.Star == "1")
                {
                    item.StarVisibility = Visibility.Visible;
                }
                else
                {
                    item.StarVisibility = Visibility.Collapsed;
                }
                if(!item.Date.Equals(""))
                {
                    item.DateVisibility = Visibility.Visible;
                }
                else
                {
                    item.DateVisibility = Visibility.Collapsed;
                }
                if(!item.Remark.Equals(""))
                {
                    item.RemarkVisibility = Visibility.Visible;
                }
                else
                {
                    item.RemarkVisibility = Visibility.Collapsed;
                }
                ToDoTaskViewModel1.ToDoDatas.Add(new ToDoTasks() { TaskId = item.TaskId, Name = item.Name, Date = item.Date, Remark = item.Remark, StarVisibility = item.StarVisibility, DateVisibility = item.DateVisibility, RemarkVisibility = item.RemarkVisibility });

            }
            foreach (var item in datalist1)
            {
                if (item.Star == "1")
                {
                    item.StarVisibility = Visibility.Visible;
                }
                else
                {
                    item.StarVisibility = Visibility.Collapsed;
                }
                if (!item.Date.Equals(""))
                {
                    item.DateVisibility = Visibility.Visible;
                }
                else
                {
                    item.DateVisibility = Visibility.Collapsed;
                }
                if (!item.Remark.Equals(""))
                {
                    item.RemarkVisibility = Visibility.Visible;
                }
                else
                {
                    item.RemarkVisibility = Visibility.Collapsed;
                }
                ToDoTaskViewModel2.ToDoDatas.Add(new ToDoTasks() { TaskId = item.TaskId, Name = item.Name, Date = item.Date, Remark = item.Remark, StarVisibility = item.StarVisibility, DateVisibility = item.DateVisibility, RemarkVisibility = item.RemarkVisibility });
            }
        }

        private void ToDoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ToDoList_ItemClick(object sender, ItemClickEventArgs e)
        {
            CurrentItem = (ToDoTasks)e.ClickedItem;
            TaskCard.IsOpen = true;
            ToDo_Picker.Date = null;
            ToDoRemarkTextBox.Text = "";
            if (!CurrentItem.Date.Equals(""))
                ToDo_Picker.Date = Convert.ToDateTime(CurrentItem.Date);
            TaskCard.Header = CurrentItem.Name;
            ToDoRemarkTextBox.Text = CurrentItem.Remark;
            LoadSteps();
        }

        private void LoadSteps()
        {
            ToDoTaskStepsViewModel.ToDoTaskStepsDatas.Clear();
            string taskId = CurrentItem.TaskId;
            List<ToDoTaskSteps> tempList = conn.Query<ToDoTaskSteps>("select * from ToDoTaskSteps where TaskId = ?", CurrentItem.TaskId);
            foreach (var item in tempList)
            {
                if(item.IsDelete.Equals("0"))
                    ToDoTaskStepsViewModel.ToDoTaskStepsDatas.Add(new ToDoTaskSteps() { TaskId = item.TaskId, StepId = item.StepId, Content = item.Content, Finish = item.Finish, UnFinish = item.UnFinish });
            }
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if(TaskNameTextBox.Text.Equals(""))
            {
                PopupNotice popupNotice = new PopupNotice("计划名称不能为空~");
                popupNotice.ShowAPopup();
            }
            else
            {
                List<ToDoTasks> tempList = conn.Query<ToDoTasks>("select * from ToDoTasks where Name = ?", TaskNameTextBox.Text);
                if(tempList.Count != 0)
                {
                    if(tempList[0].IsDelete.Equals("1"))
                    {
                        conn.Execute("update ToDoTasks set Name = ?, Date = ?, Star = ?, Remark = ?, Done = ?, UpdateTime = ?, IsDelete = ?  where TaskId = ?",
                            TaskNameTextBox.Text, "", "0", "", "0", DateTime.Now.ToString(), "0", tempList[0].TaskId);
                        ToDoTaskViewModel1.ToDoDatas.Add(new ToDoTasks() { TaskId = tempList[0].TaskId, Name = TaskNameTextBox.Text, Date = "", Remark = "", UpdateTime = DateTime.Now.ToString(), StarVisibility = Visibility.Collapsed, DateVisibility = Visibility.Collapsed, RemarkVisibility = Visibility.Collapsed });
                    }
                    else
                    {
                        PopupNotice popupNotice0 = new PopupNotice("请勿重复添加计划");
                        popupNotice0.ShowAPopup();
                    }
                }
                else
                {
                    string uuid = Guid.NewGuid().ToString();
                    conn.Insert(new ToDoTasks() { TaskId = uuid, Name = TaskNameTextBox.Text, Date = "", Star = "0", Remark = "", Done = "0", UpdateTime = DateTime.Now.ToString(), IsDelete = "0" });
                    ToDoTaskViewModel1.ToDoDatas.Add(new ToDoTasks() {TaskId = uuid, Name = TaskNameTextBox.Text, Date = "", Remark = "", UpdateTime = DateTime.Now.ToString(),StarVisibility = Visibility.Collapsed, DateVisibility = Visibility.Collapsed, RemarkVisibility = Visibility.Collapsed });
                }
                PopupNotice popupNotice = new PopupNotice("添加成功");
                popupNotice.ShowAPopup();
                TaskNameTextBox.Text = "";
            }
        }

        private void ToDoList1_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            CurrentItem = (e.OriginalSource as FrameworkElement)?.DataContext as ToDoTasks;
            if(CurrentItem.StarVisibility == Visibility.Visible)
            {
                StarButton1.Visibility = Visibility.Collapsed;
                UnStarButton1.Visibility = Visibility.Visible;
            }
            else
            {
                StarButton1.Visibility = Visibility.Visible;
                UnStarButton1.Visibility = Visibility.Collapsed;
            }
        }

        private void ToDoList2_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            CurrentItem = (e.OriginalSource as FrameworkElement)?.DataContext as ToDoTasks;
            if (CurrentItem.StarVisibility == Visibility.Visible)
            {
                StarButton2.Visibility = Visibility.Collapsed;
                UnStarButton2.Visibility = Visibility.Visible;
            }
            else
            {
                StarButton2.Visibility = Visibility.Visible;
                UnStarButton2.Visibility = Visibility.Collapsed;
            }
        }

        private async void DeleteButton1_Click(object sender, RoutedEventArgs e)
        {
            await DeleteTaskDialog.ShowAsync();
        }

        private async void DeleteButton2_Click(object sender, RoutedEventArgs e)
        { 
            await DeleteTaskDialog.ShowAsync(); 
        }
    

        private void StarButton1_Click(object sender, RoutedEventArgs e)
        {
            conn.Execute("update ToDoTasks set Star = ? , UpdateTime = ? where TaskId = ?", "1", DateTime.Now.ToString(), CurrentItem.TaskId);
            LoadData();
            PopupNotice popupNotice = new PopupNotice("标记成功");
            popupNotice.ShowAPopup();
        }

        private void UnStarButton1_Click(object sender, RoutedEventArgs e)
        {
            conn.Execute("update ToDoTasks set Star = ? , UpdateTime = ? where TaskId = ?", "0", DateTime.Now.ToString(), CurrentItem.TaskId);
            LoadData();
            PopupNotice popupNotice = new PopupNotice("标记成功");
            popupNotice.ShowAPopup();
        }

        private void UnDoneButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            conn.Execute("update ToDoTasks set Done = ? where TaskId = ?", "1", button.Tag);
            conn.Execute("update ToDoTasks set UpdateTime = ? where TaskId = ?", DateTime.Now.ToString(), button.Tag);
            LoadData();
            PopupNotice popupNotice = new PopupNotice("标记成功");
            popupNotice.ShowAPopup();
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            conn.Execute("update ToDoTasks set Done = ? where TaskId = ?", "0", button.Tag);
            conn.Execute("update ToDoTasks set UpdateTime = ? where TaskId = ?", DateTime.Now.ToString(), button.Tag);
            LoadData();
            PopupNotice popupNotice = new PopupNotice("标记成功");
            popupNotice.ShowAPopup();
        }

        private void ToDoList2_ItemClick(object sender, ItemClickEventArgs e)
        {
            CurrentItem = (ToDoTasks)e.ClickedItem;
            ToDo_Picker.Date = null;
            ToDoRemarkTextBox.Text = "";
            if (!CurrentItem.Date.Equals(""))
                ToDo_Picker.Date = Convert.ToDateTime(CurrentItem.Date);
            ToDoRemarkTextBox.Text = CurrentItem.Remark;
            TaskCard.IsOpen = true;
        }

        private void ToDoList2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DeleteTaskDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //conn.Execute("delete from ToDoTasks where TaskId = ?", CurrentItem.TaskId);
            conn.Execute("update ToDoTasks set IsDelete = ?, UpdateTime = ? where TaskId = ?", "1", DateTime.Now.ToString(), CurrentItem.TaskId);
            //conn.Execute("delete from ToDoTaskSteps where TaskId = ?", CurrentItem.TaskId);
            conn.Execute("update ToDoTaskSteps set IsDelete = ?, UpdateTime = ? where TaskId = ?", "1", DateTime.Now.ToString(), CurrentItem.TaskId);
            ToDoTaskViewModel1.ToDoDatas.Remove(CurrentItem);
            ToDoTaskViewModel2.ToDoDatas.Remove(CurrentItem);
            PopupNotice popupNotice = new PopupNotice("删除成功");
            popupNotice.ShowAPopup();
        }

        private void UpdateTastButton_Click(object sender, RoutedEventArgs e)
        {
            string date = "";
            if(ToDo_Picker.Date != null)
            {
                date = Convert.ToDateTime(ToDo_Picker.Date.ToString()).ToString("yyyy-MM-dd");
            }
            conn.Execute("update ToDoTasks set Date = ?, UpdateTime = ? where TaskId = ?", date, DateTime.Now.ToString(), CurrentItem.TaskId);
            conn.Execute("update ToDoTasks set Remark = ?, UpdateTime = ? where TaskId = ?", ToDoRemarkTextBox.Text, DateTime.Now.ToString(), CurrentItem.TaskId);
            TaskCard.IsOpen = false;
            LoadData();
            ToDo_Picker.Date = null;
            ToDoRemarkTextBox.Text = "";
            PopupNotice popupNotice = new PopupNotice("更新成功");
            popupNotice.ShowAPopup();
        }

        private void StepCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void TaskStepList_ItemClick(object sender, ItemClickEventArgs e)
        {
            
        }

        private void UpdateTaskStepDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (DialogStepNameTextBox.Text.Equals(""))
                return;
            string time = DateTime.Now.ToString();
            conn.Execute("update ToDoTaskSteps set Content = ?, UpdateTime = ? where TaskId = ? and StepId = ?", DialogStepNameTextBox.Text, time, CurrentItem.TaskId, CurrentTaskStep.StepId);
            conn.Execute("update ToDoTasks set UpdateTime = ? where TaskId = ?", time, CurrentItem.TaskId);
            LoadSteps();
            PopupNotice popupNotice = new PopupNotice("修改成功");
            popupNotice.ShowAPopup();
        }

        private void DoneStepButton_Click(object sender, RoutedEventArgs e)
        {
            string time = DateTime.Now.ToString();
            conn.Execute("update ToDoTaskSteps set Finish = ?, UpdateTime = ? where StepId = ? and TaskId = ?", true, time, CurrentTaskStep.StepId, CurrentItem.TaskId);
            conn.Execute("update ToDoTaskSteps set UnFinish = ?, UpdateTime = ? where StepId = ? and TaskId = ?", false, time, CurrentTaskStep.StepId, CurrentItem.TaskId);
            conn.Execute("update ToDoTasks set UpdateTime = ? where TaskId = ?", time, CurrentItem.TaskId);
            LoadSteps();
        }

        private void UnDoneStepButton_Click(object sender, RoutedEventArgs e)
        {
            string time = DateTime.Now.ToString();
            conn.Execute("update ToDoTaskSteps set Finish = ?, UpdateTime = ? where StepId = ? and TaskId = ?", false, time, CurrentTaskStep.StepId, CurrentItem.TaskId);
            conn.Execute("update ToDoTaskSteps set UnFinish = ?, UpdateTime = ? where StepId = ? and TaskId = ?", true, time, CurrentTaskStep.StepId, CurrentItem.TaskId);
            conn.Execute("update ToDoTasks set UpdateTime = ? where TaskId = ?", time, CurrentItem.TaskId);
            LoadSteps();
        }

        private async void EditSetpButton_Click(object sender, RoutedEventArgs e)
        {
            DialogStepNameTextBox.Text = CurrentTaskStep.Content;
            await UpdateTaskStepDialog.ShowAsync();
        }

        private void TaskStepList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            CurrentTaskStep = (e.OriginalSource as FrameworkElement)?.DataContext as ToDoTaskSteps;
            if(!CurrentTaskStep.Finish)
            {
                DoneStepButton.Visibility = Visibility.Visible;
                UnDoneStepButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                DoneStepButton.Visibility = Visibility.Collapsed;
                UnDoneStepButton.Visibility = Visibility.Visible;
            }
        }

        private void AddStepButton_Click(object sender, RoutedEventArgs e)
        {
            if (StepNameTextBox.Text.Equals(""))
            {
                PopupNotice popupNotice = new PopupNotice("步骤名称不能为空~");
                popupNotice.ShowAPopup();
            }
            else
            {
                List<ToDoTaskSteps> tempList = conn.Query<ToDoTaskSteps>("select * from ToDoTaskSteps where Content = ? and TaskId = ?", StepNameTextBox.Text, CurrentItem.TaskId);
                if (tempList.Count != 0)
                {
                    string time = DateTime.Now.ToString();
                    if(tempList[0].IsDelete.Equals("1"))
                    {
                        conn.Execute("update ToDoTaskSteps set Content = ?, Finish = ?, UnFinish = ?, UpdateTime = ?, IsDelete = ?  where TaskId = ? and StepId = ?",
                             StepNameTextBox.Text, false, true, time, "0", CurrentItem.TaskId, tempList[0].StepId);
                        conn.Execute("update ToDoTasks set UpdateTime = ? where Name = ?", time, TaskCard.Header.ToString());
                        ToDoTaskStepsViewModel.ToDoTaskStepsDatas.Add(new ToDoTaskSteps() {StepId = tempList[0].StepId, TaskId = CurrentItem.TaskId, Content = StepNameTextBox.Text, Finish = false, UnFinish = true });
                    }
                    else
                    {
                        PopupNotice popupNotice0 = new PopupNotice("请勿重复添加步骤~");
                        popupNotice0.ShowAPopup();
                    }
                }
                else
                {
                    string time = DateTime.Now.ToString();
                    string uuid = Guid.NewGuid().ToString();
                    conn.Insert(new ToDoTaskSteps() {TaskId = CurrentItem.TaskId , StepId = uuid, Content = StepNameTextBox.Text, Finish = false, UnFinish = true, UpdateTime = time, IsDelete = "0"});
                    conn.Execute("update ToDoTasks set UpdateTime = ? where Name = ?", time, TaskCard.Header.ToString());
                    ToDoTaskStepsViewModel.ToDoTaskStepsDatas.Add(new ToDoTaskSteps() { TaskId = CurrentItem.TaskId, StepId = uuid, Content = StepNameTextBox.Text, Finish = false, UnFinish = true });
                }
                PopupNotice popupNotice = new PopupNotice("添加成功");
                popupNotice.ShowAPopup();
                StepNameTextBox.Text = "";
            }
        }

        private void DeleteStepButton_Click(object sender, RoutedEventArgs e)
        {
            string time = DateTime.Now.ToString();
            conn.Execute("update ToDoTaskSteps set IsDelete = ?, UpdateTime = ? where TaskId = ? and StepId = ?", "1", time, CurrentItem.TaskId, CurrentTaskStep.StepId);
            conn.Execute("update ToDoTasks set UpdateTime = ? where TaskId = ?", time, CurrentItem.TaskId);
            ToDoTaskStepsViewModel.ToDoTaskStepsDatas.Remove(CurrentTaskStep);
            PopupNotice popupNotice = new PopupNotice("删除成功");
            popupNotice.ShowAPopup();
        }
    }
}
