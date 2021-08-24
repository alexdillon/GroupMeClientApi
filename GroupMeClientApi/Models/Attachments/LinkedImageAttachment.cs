using Newtonsoft.Json;

namespace GroupMeClientApi.Models.Attachments
{
    /// <summary>
    /// Represents an attachment to a GroupMe <see cref="Message"/> containing an image.
    /// </summary>
    public class LinkedImageAttachment : Attachment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedImageAttachment"/> class.
        /// </summary>
        internal LinkedImageAttachment()
        {
        }

        /// <inheritdoc/>
        public override string Type { get; } = "linked_image";

        /// <summary>
        /// Gets the URL of the image attachment.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; internal set; }
    }
}