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

                var result = $"{Title} - {lastExecResult}";
                return result;
            }
        }
    }
}
