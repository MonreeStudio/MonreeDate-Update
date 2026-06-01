using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using 倒计时.Models;

namespace 倒计时.Manager
{
    public class SyncManager
    {
        private const string ServiceBaseAddress = "https://msdate.monreeing.com:3000";
        private static readonly HttpClient httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(20)
        };
        private readonly string userName;
        private readonly string token;
        // SQLite
        private static readonly string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "mydb.sqlite");
        private SQLite.Net.SQLiteConnection conn;


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
            EnsureIndexes();
        }

        private void EnsureIndexes()
        {
            conn.Execute("create index if not exists IX_ToDoTasks_Done_IsDelete on ToDoTasks(Done, IsDelete)");
            conn.Execute("create index if not exists IX_ToDoTasks_UpdateTime on ToDoTasks(UpdateTime)");
            conn.Execute("create index if not exists IX_ToDoTaskSteps_TaskId on ToDoTaskSteps(TaskId)");
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
            return GetCloudTaskDataAsync().GetAwaiter().GetResult();
        }

        public async Task<string> GetCloudTaskDataAsync()
        {
            string getDataJson = JsonConvert.SerializeObject(new
            {
                UserName = userName,
                Token = token
            });
            JObject responseJson = await PostJsonAsync("/data/pulldata/", getDataJson).ConfigureAwait(false);
            return responseJson["TaskList"]?.ToString() ?? "[]";
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
                ShowPopup("同步成功！");
            }
            catch (Exception e)
            {
                ShowPopup("同步失败：" + e.Message);
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
                ShowPopup("同步成功！");
            }
            catch (Exception e)
            {
                ShowPopup("同步失败：" + e.Message);
            }
        }

        public void MergeData()
        {
            try
            {
                var localData = GetLocalTaskData();
                var cloudData = GetCloudTaskData();
                JArray cloudTaskArray = (JArray)JsonConvert.DeserializeObject(cloudData);
                JArray localTaskArray = (JArray)JsonConvert.DeserializeObject(localData);
                PushData pushData = new PushData();
                pushData.UserName = userName;
                pushData.Token = token;

                Dictionary<string, TaskJson> mergedTasks = new Dictionary<string, TaskJson>();
                foreach (var cloudTaskItem in cloudTaskArray)
                {
                    UpsertLatestTask(mergedTasks, CreateTaskJson(cloudTaskItem));
                }
                foreach(var localTaskItem in localTaskArray)
                {
                    UpsertLatestTask(mergedTasks, CreateTaskJson(localTaskItem));
                }

                pushData.TaskList = mergedTasks.Values.ToList();
                string pushDataJsonStr = JsonConvert.SerializeObject(pushData);
                PushDataToLocal(pushDataJsonStr);
                PushDataToCloud(pushDataJsonStr);
                ShowPopup("同步成功！");
            }
            catch(Exception e)
            {
                ShowPopup("同步失败：" + e.Message);
            }
        }

        private static void UpsertLatestTask(Dictionary<string, TaskJson> mergedTasks, TaskJson taskJson)
        {
            if (string.IsNullOrWhiteSpace(taskJson.TaskId))
            {
                return;
            }

            if (!mergedTasks.ContainsKey(taskJson.TaskId)
                || CompareUpdateTime(taskJson.UpdateTime, mergedTasks[taskJson.TaskId].UpdateTime) >= 0)
            {
                mergedTasks[taskJson.TaskId] = taskJson;
            }
        }

        private static TaskJson CreateTaskJson(JToken item)
        {
            return new TaskJson
            {
                TaskId = ReadString(item, "TaskId"),
                TaskName = ReadString(item, "TaskName"),
                Date = ReadString(item, "Date"),
                Star = ReadString(item, "Star"),
                Done = ReadString(item, "Done"),
                Remark = ReadString(item, "Remark"),
                UpdateTime = ReadString(item, "UpdateTime"),
                IsDelete = ReadString(item, "IsDelete"),
                TaskStepList = CreateStepJsonList(item["TaskStepList"])
            };
        }

        private static List<StepJson> CreateStepJsonList(JToken stepListToken)
        {
            JArray stepArray = stepListToken as JArray;
            if (stepArray == null && stepListToken != null && !string.IsNullOrWhiteSpace(stepListToken.ToString()))
            {
                stepArray = (JArray)JsonConvert.DeserializeObject(stepListToken.ToString());
            }

            List<StepJson> stepJsons = new List<StepJson>();
            if (stepArray == null)
            {
                return stepJsons;
            }

            foreach (var subItem in stepArray)
            {
                stepJsons.Add(new StepJson
                {
                    TaskId = ReadString(subItem, "TaskId"),
                    StepId = ReadString(subItem, "StepId"),
                    Content = ReadString(subItem, "Content"),
                    Done = ReadString(subItem, "Done"),
                    UpdateTime = ReadString(subItem, "UpdateTime"),
                    IsDelete = ReadString(subItem, "IsDelete")
                });
            }

            return stepJsons;
        }

        private static string ReadString(JToken item, string propertyName)
        {
            return item[propertyName]?.ToString() ?? "";
        }

        private static int CompareUpdateTime(string first, string second)
        {
            DateTime firstTime;
            DateTime secondTime;
            bool hasFirst = TryParseUpdateTime(first, out firstTime);
            bool hasSecond = TryParseUpdateTime(second, out secondTime);

            if (hasFirst && hasSecond)
            {
                return DateTime.Compare(firstTime, secondTime);
            }

            if (hasFirst)
            {
                return 1;
            }

            if (hasSecond)
            {
                return -1;
            }

            return string.Compare(first, second, StringComparison.Ordinal);
        }

        private static bool TryParseUpdateTime(string value, out DateTime updateTime)
        {
            return DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out updateTime)
                || DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out updateTime);
        }

        private void PushDataToCloud(string pushDataJsonStr)
        {
            PushDataToCloudAsync(pushDataJsonStr).GetAwaiter().GetResult();
        }

        private async Task PushDataToCloudAsync(string pushDataJsonStr)
        {
            await PostJsonAsync("/data/pushdata/", pushDataJsonStr).ConfigureAwait(false);
        }

        private void PushDataToLocal(string pushDataJsonStr)
        {
            bool transactionStarted = false;
            try
            {
                JObject userInfoJson = JObject.Parse(pushDataJsonStr);
                string taskListJsonStr = userInfoJson["TaskList"].ToString();
                JArray taskListJson = (JArray)JsonConvert.DeserializeObject(taskListJsonStr);
                conn.BeginTransaction();    // 事务开始
                transactionStarted = true;
                foreach (var item in taskListJson)
                {
                    string stepListJsonStr = item["TaskStepList"].ToString();
                    JArray stepListJson = (JArray)JsonConvert.DeserializeObject(stepListJsonStr);
                    foreach (var subItem in stepListJson)
                    {
                        bool finish = false;
                        if (subItem["Done"].ToString().Equals("1"))
                            finish = true;
                        List<ToDoTaskSteps> tempList0 = conn.Query<ToDoTaskSteps>("select * from ToDoTaskSteps where TaskId = ? and StepId = ?", item["TaskId"].ToString(), subItem["StepId"].ToString());
                        if(tempList0.Count == 0)
                        {
                            conn.Insert(new ToDoTaskSteps() { TaskId = subItem["TaskId"].ToString(), StepId = subItem["StepId"].ToString(), Content = subItem["Content"].ToString(), Finish = finish, UnFinish = !finish, UpdateTime = subItem["UpdateTime"].ToString(), IsDelete = subItem["IsDelete"].ToString() });
                        }
                        else
                        {
                            conn.Execute("update ToDoTaskSteps set Content = ?, Finish = ?, UnFinish = ?, UpdateTime = ?, IsDelete = ? where TaskId = ? and StepId = ?",
                                subItem["Content"].ToString(), finish, !finish, subItem["UpdateTime"].ToString(), subItem["IsDelete"].ToString(), subItem["TaskId"].ToString(), subItem["StepId"].ToString());
                        }
                    }
                    List<ToDoTasks> tempList1 = conn.Query<ToDoTasks>("select * from ToDoTasks where TaskId = ?", item["TaskId"].ToString());
                    if (tempList1.Count == 0)
                    {
                        conn.Insert(new ToDoTasks() { TaskId = item["TaskId"].ToString(), Name = item["TaskName"].ToString(), Date = item["Date"].ToString(), Star = item["Star"].ToString(), Remark = item["Remark"].ToString(), Done = item["Done"].ToString(), UpdateTime = item["UpdateTime"].ToString(), IsDelete = item["IsDelete"].ToString() });
                    }
                    else
                    {
                        conn.Execute("update ToDoTasks set Name = ?, Date = ?, Star = ?, Remark = ?, Done = ?, UpdateTime = ?, IsDelete = ? where TaskId = ?",
                            item["TaskName"].ToString(), item["Date"].ToString(), item["Star"].ToString(), item["Remark"].ToString(), item["Done"].ToString(), item["UpdateTime"].ToString(), item["IsDelete"].ToString(), item["TaskId"].ToString());
                    }
                }
                conn.Commit();  // 事务结束
            }
            catch (Exception e)
            {
                if (transactionStarted)
                {
                    conn.Rollback();
                }

                throw new InvalidOperationException("写入本地数据失败：" + e.Message, e);
            }
        }

        private JObject PostJson(string relativePath, string jsonPayload)
        {
            return PostJsonAsync(relativePath, jsonPayload).GetAwaiter().GetResult();
        }

        private async Task<JObject> PostJsonAsync(string relativePath, string jsonPayload)
        {
            using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, ServiceBaseAddress + relativePath))
            {
                requestMessage.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.SendAsync(requestMessage).ConfigureAwait(false);
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException("服务器返回错误：" + (int)response.StatusCode + " " + response.ReasonPhrase);
                }

                JObject responseJson = JObject.Parse(responseBody);
                string responseCode = responseJson["Code"]?.ToString();
                if (!"0".Equals(responseCode))
                {
                    string responseMessage = responseJson["Message"]?.ToString() ?? "未知原因";
                    throw new InvalidOperationException(responseMessage);
                }

                return responseJson;
            }
        }

        private void ShowPopup(string message)
        {
            this.Invoke(() =>
            {
                PopupNotice popupNotice = new PopupNotice(message);
                popupNotice.ShowAPopup();
            });
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
