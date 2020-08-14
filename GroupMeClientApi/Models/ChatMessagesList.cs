using System.Collections.Generic;
using Newtonsoft.Json;

namespace GroupMeClientApi.Models
{
    /// <summary>
    /// Contains a list of <see cref="Message"/>s returned from a <see cref="Chat"/>, along with status information.
    /// </summary>
    internal class ChatMessagesList
    {
        /// <summary>
        /// Gets or sets the response data containing the messages.
        /// </summary>
        [JsonProperty("response")]
        public MessageListResponse Response { get; internal set; }

        /// <summary>
        /// Gets or sets the metadata containing additional status information from GroupMe.
        /// </summary>
        [JsonProperty("meta")]
        internal Meta Meta { get; set; }

        /// <summary>
        /// Contains a list of messages and supporting information for a <see cref="Chat"/>.
        /// </summary>
        public class MessageListResponse
        {
            /// <summary>
            /// Gets or sets the number of messages contained in a <see cref="Chat"/>.
            /// </summary>
            [JsonProperty("count")]
            public int Count { get; internal set; }

            /// <summary>
            /// Gets or sets the <see cref="Message"/>s in a <see cref="Chat"/>.
            /// </summary>
            [JsonProperty("direct_messages")]
            public ICollection<Message> Messages { get; internal set; }

            /// <summary>
            /// Gets or sets the last <see cref="ReadReceipt"/> for this <see cref="Chat"/>.
            /// </summary>
            [JsonProperty("read_receipt")]
            public ReadReceipt LastReadReceipt { get; internal set; }
        }
    }
}
