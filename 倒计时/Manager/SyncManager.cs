using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using 倒计时.Models;

namespace 倒计时.Manager
{
    public class SyncManager
    {
        private string userName;
        private string token;
        // SQLite
        private static string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "mydb.sqlite");     
        private static SQLite.Net.SQLiteConnection conn;


        public SyncManager(string userName, string token)
        {
            this.userName = userName;
            this.token = token;
            ConnectToSQLite();
        }

        private void ConnectToSQLite()
        {
            conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), path);        
            conn.CreateTable<ToDoTasks>();
            conn.CreateTable<ToDoTaskSteps>();
        }


        public string GetLocalTaskData()
        {
            try
            {
                List<ToDoTasks> taskListDB = conn.Query<ToDoTasks>("select * from ToDoTasks");
                List<TaskJson> taskJsons = new List<TaskJson>();
                foreach (var item in taskListDB)
                {
                    TaskJson taskJson = new TaskJson();
                    taskJson.TaskId = item.TaskId;
                    taskJson.TaskName = item.Name;
                    taskJson.Date = item.Date;
                    taskJson.Star = item.Star;
                    taskJson.Done = item.Done;
                    taskJson.Remark = item.Remark;
                    taskJson.UpdateTime = item.UpdateTime;
                    taskJson.IsDelete = item.IsDelete;
                    List<ToDoTaskSteps> stepListDB = conn.Query<ToDoTaskSteps>("select *from ToDoTaskSteps where TaskId = ?", item.TaskId);
                    List<StepJson> stepJsons = new List<StepJson>();
                    foreach (var subItem in stepListDB)
                    {
                        StepJson stepJson = new StepJson();
                        stepJson.TaskId = subItem.TaskId;
                        stepJson.StepId = subItem.StepId;
                        stepJson.Content = subItem.Content;
                        if (subItem.Finish)
                        {
                            stepJson.Done = "1";
                        }
                        else
                        {
                            stepJson.Done = "0";
                        }
                        stepJson.UpdateTime = subItem.UpdateTime;
                        stepJson.IsDelete = subItem.IsDelete;
                        stepJsons.Add(stepJson);
                    }
                    taskJson.TaskStepList = stepJsons;
                    taskJsons.Add(taskJson);
                }
                return JsonConvert.SerializeObject(taskJsons);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string GetCloudTaskData()
        {
            try
            {
                string getDataJson = "{\"UserName\":\"" + userName + "\",\"Token\":\"" + token + "\"}";
                HttpClient client = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://msdate.monreeing.com:3000/data/pulldata/");
                requestMessage.Content = new StringContent(getDataJson);
                requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = client.SendAsync(requestMessage).GetAwaiter().GetResult();

                if (response.StatusCode.ToString() == "OK")
                {
                    string result = response.Content.ReadAsStringAsync().Result.ToString();
                    JObject jo = JObject.Parse(result);
                    string responseCode = jo["Code"].ToString();
                    string responseMsg = jo["Message"].ToString();
                    if (responseCode.Equals("0"))
                    {
                        return jo["TaskList"].ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public void UseLocalData()
        {
            var result = GetLocalTaskData();
            try
            {
                JArray localTaskArray = (JArray)JsonConvert.DeserializeObject(result);
                List<TaskJson> taskJsons = new List<TaskJson>();
                PushData pushData = new PushData();
                pushData.UserName = userName;
                pushData.Token = token;
                foreach (var item in localTaskArray)
                {

                    TaskJson taskJson = new TaskJson
                    {
                        TaskId = item["TaskId"].ToString(),
                        TaskName = item["TaskName"].ToString(),
                        Date = item["Date"].ToString(),
                        Star = item["Star"].ToString(),
                        Done = item["Done"].ToString(),
                        Remark = item["Remark"].ToString(),
                        UpdateTime = item["UpdateTime"].ToString(),
                        IsDelete = item["IsDelete"].ToString()
                    };
                    List<StepJson> stepJsons = new List<StepJson>();
                    string taskStepListStr = item["TaskStepList"].ToString();
                    JArray localStepArray = (JArray)JsonConvert.DeserializeObject(taskStepListStr);
                    foreach(var subItem in localStepArray)
                    {
                        StepJson stepJson = new StepJson()
                        {
                            TaskId = subItem["TaskId"].ToString(),
                            StepId = subItem["StepId"].ToString(),
                            Content = subItem["Content"].ToString(),
                            Done = subItem["Done"].ToString(),
                            UpdateTime = subItem["UpdateTime"].ToString(),
                            IsDelete = subItem["IsDelete"].ToString()
                        };
                        stepJsons.Add(stepJson);
                    }
                    taskJson.TaskStepList = stepJsons;
                    taskJsons.Add(taskJson);
                }
                pushData.TaskList = taskJsons;
                string pushDataJsonStr = JsonConvert.SerializeObject(pushData);
                PushDataToCloud(pushDataJsonStr);
            }
            catch
            {

            }
            
        }

        public async void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
        }

        public void UseCloudData()
        {
            var result = GetCloudTaskData();
            try
            {
                JArray localTaskArray = (JArray)JsonConvert.DeserializeObject(result);
                List<TaskJson> taskJsons = new List<TaskJson>();
                PushData pushData = new PushData();
                pushData.UserName = userName;
                pushData.Token = token;
                foreach (var item in localTaskArray)
                {

                    TaskJson taskJson = new TaskJson
                    {
                        TaskId = item["TaskId"].ToString(),
                        TaskName = item["TaskName"].ToString(),
                        Date = item["Date"].ToString(),
                        Star = item["Star"].ToString(),
                        Done = item["Done"].ToString(),
                        Remark = item["Remark"].ToString(),
                        UpdateTime = item["UpdateTime"].ToString(),
                        IsDelete = item["IsDelete"].ToString()
                    };
                    List<StepJson> stepJsons = new List<StepJson>();
                    string taskStepListStr = item["TaskStepList"].ToString();
                    JArray localStepArray = (JArray)JsonConvert.DeserializeObject(taskStepListStr);
                    foreach (var subItem in localStepArray)
                    {
                        StepJson stepJson = new StepJson()
                        {
                            TaskId = subItem["TaskId"].ToString(),
                            StepId = subItem["StepId"].ToString(),
                            Content = subItem["Content"].ToString(),
                            Done = subItem["Done"].ToString(),
                            UpdateTime = subItem["UpdateTime"].ToString(),
                            IsDelete = subItem["IsDelete"].ToString()
                        };
                        stepJsons.Add(stepJson);
                    }
                    taskJson.TaskStepList = stepJsons;
                    taskJsons.Add(taskJson);
                }
                pushData.TaskList = taskJsons;
                string pushDataJsonStr = JsonConvert.SerializeObject(pushData);
                PushDataToLocal(pushDataJsonStr);
            }
            catch (Exception e)
            {
                this.Invoke(() =>
                {
                    PopupNotice popupNotice = new PopupNotice("同步失败：" + e.Message);
                    popupNotice.ShowAPopup();
                });
            }
        }

        public void MergeData()
        {
            try
            {
                var localData = GetLocalTaskData();
                var cloudData = GetCloudTaskData();
                JArray cloudTaskArray = (JArray)JsonConvert.DeserializeObject(cloudData);
                List<TaskJson> taskJsons = new List<TaskJson>();
                PushData pushData = new PushData();
                pushData.UserName = userName;
                pushData.Token = token;
                foreach (var cloudTaskItem in cloudTaskArray)
                {
                    string taskId = cloudTaskItem["TaskId"].ToString();
                    List<ToDoTasks> tempList = conn.Query<ToDoTasks>("select * from ToDoTasks where TaskId = ?", taskId);
                    string localUpdateTimeStr = tempList[0].UpdateTime;
                    string cloudUpdateTimeStr = cloudTaskItem["UpdateTime"].ToString();
                    DateTime localUpdateTime = Convert.ToDateTime(localUpdateTimeStr);
                    DateTime cloudUpdateTime = Convert.ToDateTime(cloudUpdateTimeStr);
                    if(cloudUpdateTime >= localUpdateTime)
                    {
                        TaskJson taskJson = new TaskJson
                        {
                            TaskId = cloudTaskItem["TaskId"].ToString(),
                            TaskName = cloudTaskItem["TaskName"].ToString(),
                            Date = cloudTaskItem["Date"].ToString(),
                            Star = cloudTaskItem["Star"].ToString(),
                            Done = cloudTaskItem["Done"].ToString(),
                            Remark = cloudTaskItem["Remark"].ToString(),
                            UpdateTime = cloudTaskItem["UpdateTime"].ToString(),
                            IsDelete = cloudTaskItem["IsDelete"].ToString()
                        };
                        List<StepJson> stepJsons = new List<StepJson>();
                        string taskStepListStr = cloudTaskItem["TaskStepList"].ToString();
                        JArray localStepArray = (JArray)JsonConvert.DeserializeObject(taskStepListStr);
                        foreach (var subItem in localStepArray)
                        {
                            StepJson stepJson = new StepJson()
                            {
                                TaskId = subItem["TaskId"].ToString(),
                                StepId = subItem["StepId"].ToString(),
                                Content = subItem["Content"].ToString(),
                                Done = subItem["Done"].ToString(),
                                UpdateTime = subItem["UpdateTime"].ToString(),
                                IsDelete = subItem["IsDelete"].ToString()
                            };
                            stepJsons.Add(stepJson);
                        }
                        taskJson.TaskStepList = stepJsons;
                        taskJsons.Add(taskJson);
                    }
                    else
                    {
                        TaskJson taskJson = new TaskJson
                        {
                            TaskId = tempList[0].TaskId,
                            TaskName = tempList[0].Name,
                            Date = tempList[0].Date,
                            Star = tempList[0].Star,
                            Done = tempList[0].Done,
                            Remark = tempList[0].Remark,
                            UpdateTime = tempList[0].UpdateTime,
                            IsDelete = tempList[0].IsDelete
                        };
                        List<StepJson> stepJsons = new List<StepJson>();
                        List<ToDoTaskSteps> subTempList = conn.Query<ToDoTaskSteps>("select * from ToDoTaskSteps where TaskId = ?", taskId);
                        foreach (var subItem in subTempList)
                        {
                            StepJson stepJson = new StepJson()
                            {
                                TaskId = subItem.TaskId,
                                StepId = subItem.StepId,
                                Content = subItem.Content,
                                UpdateTime = subItem.UpdateTime,
                                IsDelete = subItem.IsDelete
                            };
                            if (subItem.Finish)
                                stepJson.Done = "1";
                            else
                                stepJson.Done = "0";
                            stepJsons.Add(stepJson);
                        }
                        taskJson.TaskStepList = stepJsons;
                        taskJsons.Add(taskJson);
                    }
                }
                pushData.TaskList = taskJsons;
                string pushDataJsonStr = JsonConvert.SerializeObject(pushData);
                PushDataToLocal(pushDataJsonStr);
                PushDataToCloud(pushDataJsonStr);
                this.Invoke(() =>
                {
                    PopupNotice popupNotice = new PopupNotice("同步成功！");
                    popupNotice.ShowAPopup();
                });
            }
            catch(Exception e)
            {
                this.Invoke(() =>
                {
                    PopupNotice popupNotice = new PopupNotice("同步失败：" + e.Message);
                    popupNotice.ShowAPopup();
                });
            }
        }

        private void PushDataToCloud(string pushDataJsonStr)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://msdate.monreeing.com:3000/data/pushdata/");
                requestMessage.Content = new StringContent(pushDataJsonStr);
                requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = client.SendAsync(requestMessage).GetAwaiter().GetResult();

                if (response.StatusCode.ToString() == "OK")
                {
                    string str = response.Content.ReadAsStringAsync().Result.ToString();
                    JObject jo = JObject.Parse(str);
                    string responseCode = jo["Code"].ToString();
                    string responseMsg = jo["Message"].ToString();
                    if (responseCode.Equals("0"))
                    {
                        this.Invoke(() =>
                        {
                            PopupNotice popupNotice = new PopupNotice("同步成功！");
                            popupNotice.ShowAPopup();
                        });
                    }
                    else
                    {
                        this.Invoke(() =>
                        {
                            PopupNotice popupNotice = new PopupNotice("同步失败：" + responseMsg);
                            popupNotice.ShowAPopup();
                        });
                    }
                }
                else
                {

                }
            }
            catch(Exception e)
            {
                this.Invoke(() =>
                {
                    PopupNotice popupNotice = new PopupNotice("同步失败：" + e.Message);
                    popupNotice.ShowAPopup();
                });
            }
        }

        private void PushDataToLocal(string pushDataJsonStr)
        {
            try
            {
                JObject userInfoJson = JObject.Parse(pushDataJsonStr);
                string taskListJsonStr = userInfoJson["TaskList"].ToString();
                JArray taskListJson = (JArray)JsonConvert.DeserializeObject(taskListJsonStr);
                conn.BeginTransaction();    // 事务开始
                foreach (var item in taskListJson)
                {
                    string stepListJsonStr = item["TaskStepList"].ToString();
                    JArray stepListJson = (JArray)JsonConvert.DeserializeObject(stepListJsonStr);
                    foreach (var subItem in stepListJson)
                    {
                        bool finish = false;
                        if (subItem["Done"].ToString().Equals("1"))
                            finish = true;
                        conn.Execute("update ToDoTaskSteps set Content = ?, Finish = ?, UnFinish = ?, UpdateTime = ?, IsDelete = ? where TaskId = ? and StepId = ?",
                            subItem["Content"].ToString(), finish, !finish, subItem["UpdateTime"].ToString(), subItem["IsDelete"].ToString(), subItem["TaskId"].ToString(), subItem["StepId"].ToString());
                    }
                    conn.Execute("update ToDoTasks set Name = ?, Date = ?, Star = ?, Remark = ?, Done = ?, UpdateTime = ?, IsDelete = ? where Name = ? and TaskId = ?",
                            item["TaskName"].ToString(), item["Date"].ToString(), item["Star"].ToString(), item["Remark"].ToString(), item["Done"].ToString(), item["UpdateTime"].ToString(), item["IsDelete"].ToString(), item["TaskName"].ToString(), item["TaskId"].ToString());
                }
                conn.Commit();  // 事务结束
                this.Invoke(() =>
                {
                    PopupNotice popupNotice = new PopupNotice("同步成功！");
                    popupNotice.ShowAPopup();
                });
            }
            catch (Exception e)
            {
                this.Invoke(() =>
                {
                    PopupNotice popupNotice = new PopupNotice("同步异常：" + e.Message);
                    popupNotice.ShowAPopup();
                });
            }
        }

        class TaskJson
        {
            public string TaskId { get; set; }
            public string TaskName { get; set; }
            public string Date { get; set; }
            public string Star { get; set; }
            public string Done { get; set; }
            public string Remark { get; set; }
            public string UpdateTime { get; set; }
            public string IsDelete { get; set; }
            public List<StepJson> TaskStepList { get; set; }
        }

        class StepJson
        {
            public string TaskId { get; set; }
            public string StepId { get; set; }
            public string Content { get; set; }
            public string Done { get; set; }
            public string UpdateTime { get; set; }
            public string IsDelete { get; set; }
        }

        class PushData
        {
            public string UserName { get; set; }
            public string Token { get; set; }
            public List<TaskJson> TaskList { get; set; }
        }
    }
}
