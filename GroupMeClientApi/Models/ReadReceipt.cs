using System;
using Newtonsoft.Json;

namespace GroupMeClientApi.Models
{
    /// <summary>
    /// Represents a read receipt indicating information on whether a message has been received and read.
    /// </summary>
    public class ReadReceipt
    {
        /// <summary>
        /// Gets the identifier for the receipt.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the identifier for the <see cref="Chat"/> the receipt is associated with.
        /// </summary>
        [JsonProperty("chat_id")]
        public string ChatId { get; internal set; }

        /// <summary>
        /// Gets the identifier for the <see cref="Message"/> the receipt is associated with.
        /// </summary>
        [JsonProperty("message_id")]
        public string MessageId { get; internal set; }

        /// <summary>
        /// Gets the identifier for <see cref="Member"/> the receipt was sent from.
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; internal set; }

        /// <summary>
        /// Gets the Unix Timestamp for when the associated <see cref="Message"/> was read by the <see cref="Member"/>.
        /// </summary>
        [JsonProperty("read_at")]
        public int ReadAtUnixTime { get; internal set; }

        /// <summary>
        /// Gets the Date and Time for when the associated <see cref="Message"/> was read by the <see cref="Member"/>.
        /// </summary>
        public DateTime ReadAtTime => DateTimeOffset.FromUnixTimeSeconds(this.ReadAtUnixTime).ToLocalTime().DateTime;
    }
}
