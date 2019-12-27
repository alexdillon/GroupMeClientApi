﻿using GroupMeClientApi.Models;
using Newtonsoft.Json;

namespace GroupMeClientApi.Push.Notifications
{
    /// <summary>
    /// Represents a notification indicating that a new Line (Group) Message has been sent.
    /// </summary>
    public class LineMessageCreateNotification : Notification
    {
        /// <inheritdoc/>
        public override string Type { get; } = "line.create";

        /// <summary>
        /// Gets the alert text.
        /// </summary>
        [JsonProperty("alert")]
        public string Alert { get; internal set; }

        /// <summary>
        /// Gets the Unix Timestamp when the notification was received.
        /// </summary>
        [JsonProperty("received_at")]
        public string ReceivedAtUnixTimestamp { get; internal set; }

        /// <summary>
        /// Gets a preview of the created message.
        /// </summary>
        [JsonProperty("subject")]
        public Message Message { get; internal set; }
    }
}
