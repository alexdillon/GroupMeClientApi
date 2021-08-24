using JsonSubTypes;
using Newtonsoft.Json;

namespace GroupMeClientApi.Models.Attachments
{
    /// <summary>
    /// Generic type to represent an attachment to a GroupMe <see cref="Message"/>.
    /// </summary>
    [JsonConverter(typeof(JsonSubtypes), "Type")]
    [JsonSubtypes.KnownSubType(typeof(EmojiAttachment), "emoji")]
    [JsonSubtypes.KnownSubType(typeof(ImageAttachment), "image")]
    [JsonSubtypes.KnownSubType(typeof(LinkedImageAttachment), "linked_image")]
    [JsonSubtypes.KnownSubType(typeof(VideoAttachment), "video")]
    [JsonSubtypes.KnownSubType(typeof(LocationAttachment), "location")]
    [JsonSubtypes.KnownSubType(typeof(SplitAttachment), "split")]
    [JsonSubtypes.KnownSubType(typeof(MentionsAttachment), "mentions")]
    [JsonSubtypes.KnownSubType(typeof(FileAttachment), "file")]
    [JsonSubtypes.KnownSubType(typeof(ReplyAttachment), "reply")]
    public class Attachment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Attachment"/> class.
        /// </summary>
        internal Attachment()
        {
        }

        /// <summary>
        /// Gets the attachment type.
        /// </summary>
        [JsonProperty("type")]
        public virtual string Type { get; }
    }
}
