using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;

namespace DiscordJob
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string AuthFile;
        private string ConfigFile;
        private bool TaskExecuting = false;
        private bool CloseingHide = false;
        public MainWindow()
        {
            InitializeComponent();

            AuthFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "auth.json");
            ConfigFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            LoadAuth();
            LoadTasks();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        private void btnSaveAuth_Click(object sender, RoutedEventArgs e)
        {
            var auth = tbAuth.Text.Trim();
            if (string.IsNullOrWhiteSpace(auth))
            {
                MessageBox.Show("请填充完整");
                return;
            }
            System.IO.File.WriteAllText(AuthFile, auth);
            MessageBox.Show("保存成功");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (TaskExecuting)
            {
                MessageBox.Show("请先停止任务");
                return;
            }

            var title = tvTitle.Text.Trim();
            var content = tvContent.Text.Trim();
            var cron = tvCron.Text.Trim();
            var channelId = tvChannelId.Text.Trim();
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(cron) || string.IsNullOrWhiteSpace(channelId))
            {
                MessageBox.Show("请填充完整");
                return;
            }
            if (!cron.EndsWith('h') && !cron.EndsWith('m') && !cron.EndsWith('s'))
            {
                MessageBox.Show("循环时间必须以(h/m/s)为结尾");
                return;
            }

            var task = new TaskModel()
            {
                Title = title,
                Content = content,
                ChannelId = channelId,
                Cron = cron
            };
            SaveTasks(task);

            tvTitle.Text = "";
            tvContent.Text = "";
            tvCron.Text = "";
            tvChannelId.Text = "";
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (TaskExecuting)
            {
                TaskExecuting = false;
                btnStart.Content = "启动任务";
                LoadTasks();
                return;
            }

            var tasks = GetTasks();
            if (tasks.Count <= 0)
            {
                MessageBox.Show("暂无任务可执行");
                return;
            }
            var auth = GetAuth();
            if (string.IsNullOrEmpty(auth))
            {
                MessageBox.Show("用户授权码未设置");
                return;
            }

            TaskExecuting = true;
            btnStart.Content = "停止任务";
            for (int i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                Task.Factory.StartNew(async () =>
                {
                    await ExecuteTask(i, task);
                });
            }
        }


        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadTasks();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lbTask.SelectedItem == null)
            {
                MessageBox.Show($"请先选择要删除的行");
                return;
            }
            var selectTask = (TaskModel)lbTask.SelectedItem;
            SaveTasks(selectTask, true);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if(CloseingHide)
            {
                Hide();
                Topmost = false;
                e.Cancel = true;
                return;
            }
            var dialogResult = MessageBox.Show("关闭窗口或者隐藏到工具栏继续运行，点击【确定】隐藏后继续运行", "请选择", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (dialogResult == MessageBoxResult.OK)
            {
                CloseingHide = true;

                Hide();
                Topmost = false;
                e.Cancel = true;
                return;
            }
            base.OnClosing(e);
        }

        private void btnOpenMainWindow_Click(object sender, RoutedEventArgs e)
        {
            Show();
            Topmost = true;
        }

        private void btnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        #region Method
        private string GetAuth()
        {
            if (!System.IO.File.Exists(AuthFile))
            {
                System.IO.File.Create(AuthFile);
                return string.Empty;
            }

            var auth = System.IO.File.ReadAllText(AuthFile);
            return auth;
        }

        private void LoadAuth()
        {
            var auth = GetAuth();
            tbAuth.Dispatcher.Invoke(() =>
            {
                tbAuth.Text = auth;
            });

        }

        private void LoadTasks()
        {
            var tasks = GetTasks();
            lbTask.Dispatcher.Invoke(() =>
            {
                lbTask.ItemsSource = tasks;
            });

        }

        private List<TaskModel> GetTasks()
        {
            if (!System.IO.File.Exists(ConfigFile))
            {
                System.IO.File.Create(ConfigFile);
                return new List<TaskModel>();
            }

            var taskJson = System.IO.File.ReadAllText(ConfigFile);
            var tasks = taskJson.TryDeserialize<List<TaskModel>>() ?? new List<TaskModel>();
            return tasks;
        }

        private void SaveTasks(TaskModel task, bool remove = false)
        {
            var taskJson = System.IO.File.ReadAllText(ConfigFile);
            var tasks = taskJson.TryDeserialize<List<TaskModel>>() ?? new List<TaskModel>();

            var index = tasks.FindIndex(oo => oo.Key == task.Key);
            if (remove)
            {
                tasks.RemoveAt(index);
            }
            else
            {
                if (index < 0)
                    tasks.Add(task);
                else
                    tasks[index] = task;
            }

            System.IO.File.WriteAllText(ConfigFile, tasks.TrySerialize(true));
            LoadTasks();
        }

        private void AddLog(string message)
        {
            tbLog.Dispatcher.Invoke(() =>
            {
                var exist = tbLog.Text;
                if (exist.Length > 200 * 200)
                {
                    exist = exist.Substring(0, 200 * 200);
                }
                tbLog.Text = $"{message} \r\n{exist}";
            });
        }

        private async Task ExecuteTask(int index, TaskModel task)
        {
            var channelId = task.ChannelId;
            var content = task.Content;

            var auth = GetAuth();

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"https://discord.com/api/v9/channels/{channelId}/messages");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.0.0 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Authorization", auth);

            var lastExecuteTime = DateTime.MinValue;

            var cronNum = task.Cron.Substring(0, task.Cron.Length - 1).ToInt32OrDefault();
            var timeSpan = new TimeSpan();
            if (task.Cron.EndsWith('h'))
                timeSpan = new TimeSpan(cronNum, 0, 0);
            else if (task.Cron.EndsWith('m'))
                timeSpan = new TimeSpan(0, cronNum, 0);
            else
                timeSpan = new TimeSpan(0, 0, cronNum);

            do
            {
                if (!TaskExecuting)
                    break;

                try
                {
                    var nextExecuteTime = lastExecuteTime.Add(timeSpan);
                    if (nextExecuteTime.AddSeconds(new Random().Next(10, 30)) > DateTime.Now)
                    {
                        var remainSeconds = (int)(nextExecuteTime - DateTime.Now).TotalSeconds;
                        task.LastExecResult = $"上次执行{lastExecuteTime:MM-dd HH:mm:ss},{remainSeconds}秒后执行下一个";
                        SaveTasks(task);

                        AddLog($"{DateTime.Now:yy-MM-dd HH:mm:ss} {task.Title} {remainSeconds}秒后执行下一个");
                        Thread.Sleep(1000);
                        continue;
                    }

                    var message = new MessageModel()
                    {
                        content = content,
                        nonce = $"1265980{(index > 9 ? $"{index}" : $"0{index}")}{DateTime.Now:MMddHHmmss}"
                    };
                    var stringContent = new StringContent(message.TrySerialize());
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var httpRequest = new HttpRequestMessage()
                    {
                        Method = HttpMethod.Post,
                        Content = stringContent,
                    };
                    var httpResponse = await httpClient.SendAsync(httpRequest);
                    var httpResponseString = await httpResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"{DateTime.Now:MM-dd HH:mm:ss} {httpResponseString}");
                    lastExecuteTime = DateTime.Now;
                    task.LastExecResult = $"本次执行{lastExecuteTime:MM-dd HH:mm:ss}";
                    SaveTasks(task);

                    AddLog($"{DateTime.Now:yy-MM-dd HH:mm:ss} {task.Title} 执行结果：{httpResponseString}");

                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    task.LastExecResult = $"上次执行{lastExecuteTime:MM-dd HH:mm:ss},本次执行{DateTime.Now:MM-dd HH:mm:ss}异常";
                    SaveTasks(task);

                    AddLog($"{DateTime.Now:yy-MM-dd HH:mm:ss} {task.Title} 执行异常：{ex.Message}");

                    Thread.Sleep(1000);
                }

            } while (true);
        }
        #endregion
    }
}