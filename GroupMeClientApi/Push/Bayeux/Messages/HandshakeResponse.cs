using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GroupMeClientApi.Push.Bayeux.Messages
{
    /// <summary>
    /// The response message sent by a Bayeux server in response to a handshake request.
    /// </summary>
    internal class HandshakeResponse
    {
        /// <summary>
        /// Gets or sets the Bayeux channel the handshake was sent to.
        /// </summary>
        [JsonProperty("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request was successful.
        /// </summary>
        [JsonProperty("successful")]
        public bool Successful { get; set; }

        /// <summary>
        /// Gets or sets the bayeux protocol version.
        /// </summary>
        [JsonProperty("version")]
        public decimal Version { get; set; }

        /// <summary>
        /// Gets or sets a listing of connection methods supported by the push server.
        /// </summary>
        [JsonProperty("supportedConnectionTypes")]
        public List<string> SupportedConnectionTypes { get; set; }

        /// <summary>
        /// Gets or sets the Bayeux client ID.
        /// </summary>
        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets Bayeux connection advice provided by the server.
        /// </summary>
        [JsonProperty("advice")]
        public ServerAdvice Advice { get; set; }

        /// <summary>
        /// Connection advice provided by the server.
        /// </summary>
        public class ServerAdvice
        {
            /// <summary>
            /// Gets or sets the reconnection interval.
            /// </summary>
            [JsonProperty("reconnect")]
            public string Reconnect { get; set; }

            /// <summary>
            /// Gets or sets the polling interval.
            /// </summary>
            [JsonProperty("interval")]
            public int Interval { get; set; }

            /// <summary>
            /// Gets or sets the Bayeux timeout.
            /// </summary>
            [JsonProperty("timeout")]
            public int Timeout { get; set; }
        }
    }
}
