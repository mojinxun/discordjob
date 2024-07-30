using System.Text.Json.Serialization;

namespace DiscordJob
{
    internal class TaskModel
    {
        /// <summary>
        /// 
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public required string ChannelId { get; set; }

        /// <summary>
        /// 发送内容
        /// </summary>
        public required string Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public required string Cron { get; set; }

        /// <summary>
        /// 随机秒 10,11
        /// </summary>
        public string? RdSecond { get; set; }

        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool IsPause { get; set; }

        /// <summary>
        /// 最后一次执行的状态
        /// </summary>
        public bool? LastExec { get; set; }

        /// <summary>
        /// 最后执行状态
        /// </summary>
        [JsonIgnore]
        public string? LastExecResult { get; set; }


        [JsonIgnore]
        public string Key
        {
            get
            {
                return $"{Title}{ChannelId}{Content}";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public string ShowResult
        {
            get
            {
                var lastExecResult = LastExecResult;
                if (string.IsNullOrWhiteSpace(lastExecResult))
                    lastExecResult = "未执行";
                else if (LastExec.HasValue && !LastExec.Value)
                    lastExecResult = $"执行失败";

                var result = $"{(IsPause ? "【暂停】" : "")}{Title} - {lastExecResult}";
                return result;
            }
        }
    }
}
