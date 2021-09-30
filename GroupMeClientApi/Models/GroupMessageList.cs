using System.Collections.Generic;
using Newtonsoft.Json;

namespace GroupMeClientApi.Models
{
    /// <summary>
    /// Contains a list of <see cref="Message"/>s returned from a <see cref="Group"/>, along with status information.
    /// </summary>
    public class GroupMessageList
    {
        /// <summary>
        /// Gets the number of messages in a <see cref="Group"/>.
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; internal set; }

        /// <summary>
        /// Gets the <see cref="Message"/>s in a <see cref="Group"/>.
        /// </summary>
        [JsonProperty("messages")]
        public IList<Message> Messages { get; internal set; }
    }
}