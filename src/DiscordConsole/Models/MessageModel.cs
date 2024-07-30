using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordConsole
{
    internal class MessageModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string mobile_network_type { get; } = "unknown";

        /// <summary>
        /// 
        /// </summary>
        public required string content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public required string nonce { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool tts { get; } = false;

        /// <summary>
        /// 
        /// </summary>
        public int flags { get; } = 0;
    }
}
