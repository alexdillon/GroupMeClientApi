using System;
using Newtonsoft.Json;

namespace GroupMeClientApi.Push.Bayeux.Messages
{
    /// <summary>
    /// The message sent by a Bayeux server in response to a subscription request.
    /// </summary>
    internal class SubscriptionResponse
    {
        /// <summary>
        /// Gets or sets the sequence number of this message.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Bayeux client ID.
        /// </summary>
        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the Bayeux channel this message is being sent to.
        /// </summary>
        [JsonProperty("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this request was successful.
        /// </summary>
        [JsonProperty("successful")]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the name of the subscription being requested.
        /// </summary>
        [JsonProperty("subscription")]
        public string Subscription { get; set; }
    }
}
