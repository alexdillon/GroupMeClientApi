using Newtonsoft.Json;

namespace GroupMeClientApi.Models.Attachments
{
    /// <summary>
    /// Represents an attachment to a GroupMe <see cref="Message"/> containing an quoted message.
    /// </summary>
    public class ReplyAttachment : Attachment
    {
        /// <inheritdoc/>
        public override string Type { get; } = "reply";

        /// <summary>
        /// Gets the unique identifier of the user who sent the message being replied to.
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; internal set; }

        /// <summary>
        /// Gets the ID of the message being replied to.
        /// </summary>
        [JsonProperty("reply_id")]
        public string RepliedMessageId { get; internal set; }

        /// <summary>
        /// Gets the ID of the base message in the reply chain.
        /// </summary>
        [JsonProperty("base_reply_id")]
        public string BaseRepliedMessageId { get; internal set; }
    }
}
