using Newtonsoft.Json;

namespace GroupMeClientApi.Models.Attachments
{
    /// <summary>
    /// Represents an attachment to a GroupMe <see cref="Message"/> containing a location.
    /// </summary>
    public class LocationAttachment : Attachment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationAttachment"/> class.
        /// </summary>
        internal LocationAttachment()
        {
        }

        /// <inheritdoc/>
        public override string Type { get; } = "location";

        /// <summary>
        /// Gets the latitude.
        /// </summary>
        [JsonProperty("lat")]
        public string Latitude { get; internal set; }

        /// <summary>
        /// Gets the longitude.
        /// </summary>
        [JsonProperty("lng")]
        public string Longitude { get; internal set; }

        /// <summary>
        /// Gets the location name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }
    }
}
