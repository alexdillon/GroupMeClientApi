using System.Collections.Generic;
using Newtonsoft.Json;

namespace GroupMeClientApi.Models
{
    /// <summary>
    /// Contains a list of messages and supporting information for a <see cref="Chat"/>.
    /// </summary>
    public class ChatMessageList
    {
        /// <summary>
        /// Gets the number of messages contained in a <see cref="Chat"/>.
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; internal set; }

        /// <summary>
        /// Gets the <see cref="Message"/>s in a <see cref="Chat"/>.
        /// </summary>
        [JsonProperty("direct_messages")]
        public ICollection<Message> Messages { get; internal set; }

        /// <summary>
        /// Gets the last <see cref="ReadReceipt"/> for this <see cref="Chat"/>.
        /// </summary>
        [JsonProperty("read_receipt")]
        public ReadReceipt LastReadReceipt { get; internal set; }
    }
}
