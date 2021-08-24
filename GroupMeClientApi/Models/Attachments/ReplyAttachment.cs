using Newtonsoft.Json;

namespace GroupMeClientApi.Models.Attachments
{
    /// <summary>
    /// Represents an attachment to a GroupMe <see cref="Message"/> containing an quoted message.
    /// </summary>
    public class ReplyAttachment : Attachment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReplyAttachment"/> class.
        /// </summary>
        internal ReplyAttachment()
        {
        }

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

        /// <summary>
        /// Creates a new attachment indicating that a message is being sent in reply to an
        /// existing message.
        /// </summary>
        /// <param name="replyToMessage">The existing message that is being replied to.</param>
        /// <returns>A new reply attachment.</returns>
        public static ReplyAttachment CreateReplyAttachment(Message replyToMessage)
        {
            return new ReplyAttachment()
            {
                BaseRepliedMessageId = replyToMessage.Id,
                RepliedMessageId = replyToMessage.Id,
            };
        }
    }
}
