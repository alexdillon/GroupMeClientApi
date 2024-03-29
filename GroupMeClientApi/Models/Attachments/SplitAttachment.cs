﻿using Newtonsoft.Json;

namespace GroupMeClientApi.Models.Attachments
{
    /// <summary>
    /// Represents an attachment to a GroupMe <see cref="Message"/> containing a bill-split.
    /// </summary>
    public class SplitAttachment : Attachment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SplitAttachment"/> class.
        /// </summary>
        internal SplitAttachment()
        {
        }

        /// <inheritdoc/>
        public override string Type { get; } = "split";

        /// <summary>
        /// Gets the token associated with this Split.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; internal set; }
    }
}
