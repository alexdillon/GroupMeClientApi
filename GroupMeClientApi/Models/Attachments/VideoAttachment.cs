using Newtonsoft.Json;

namespace GroupMeClientApi.Models.Attachments
{
    /// <summary>
    /// Represents an attachment to a GroupMe <see cref="Message"/> containing a video.
    /// </summary>
    public class VideoAttachment : Attachment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoAttachment"/> class.
        /// </summary>
        internal VideoAttachment()
        {
        }

        /// <inheritdoc/>
        public override string Type { get; } = "video";

        /// <summary>
        /// Gets the URL of the video attachment.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; internal set; }

        /// <summary>
        /// Gets the URL of the video preview image.
        /// </summary>
        [JsonProperty("preview_url")]
        public string PreviewUrl { get; internal set; }
    }
}