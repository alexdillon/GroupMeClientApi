using System;
using Newtonsoft.Json;

namespace GroupMeClientApi.Models
{
    /// <summary>
    /// A wrapper that encapsulates response data from the GroupMe API, as well as <see cref="Meta"/>
    /// data about the requested operation.
    /// </summary>
    /// <typeparam name="T">The type of the response data expected from the API call.</typeparam>
    internal class ResponseWrapper<T>
    {
        /// <summary>
        /// Gets or sets the response data containing the messages.
        /// </summary>
        [JsonProperty("response")]
        public T Response { get; internal set; }

        /// <summary>
        /// Gets or sets the metadata containing additional status information from GroupMe.
        /// </summary>
        [JsonProperty("meta")]
        internal Meta Meta { get; set; }
    }
}
