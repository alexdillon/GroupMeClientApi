using System.Collections.Generic;
using Newtonsoft.Json;

namespace GroupMeClientApi.Models
{
    /// <summary>
    /// <see cref="ChatsList"/> provides a list of Chats, along with additional status information.
    /// </summary>
    internal class ChatsList
    {
        /// <summary>
        /// Gets or sets the list of Chats.
        /// </summary>
        [JsonProperty("response")]
        public IList<Chat> Chats { get; internal set; }

        /// <summary>
        /// Gets or sets the Metadata for the API Call.
        /// </summary>
        [JsonProperty("meta")]
        public Meta Meta { get; internal set; }
    }
}
