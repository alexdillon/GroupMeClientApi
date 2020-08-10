using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GroupMeClientApi.Models
{
    /// <summary>
    /// Represents a GroupMe Contact.
    /// </summary>
    public class Contact : IAvatarSource
    {
        /// <summary>
        /// Gets a value indicating whether the <see cref="Contact"/> has the GroupMe app installed.
        /// </summary>
        [JsonProperty("app_installed")]
        public bool AppInstalled { get; internal set; }

        /// <summary>
        /// Gets the Url to the <see cref="Contact"/>'s avatar picture.
        /// </summary>
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; internal set; }

        /// <summary>
        /// Gets the Url to the <see cref="Contact"/>'s avatar.
        /// </summary>
        public string ImageOrAvatarUrl
        {
            get
            {
                return this.AvatarUrl;
            }
        }

        /// <inheritdoc />
        bool IAvatarSource.IsRoundedAvatar => true;

        /// <summary>
        /// Gets a value indicating whether the <see cref="Contact"/> has been blocked.
        /// </summary>
        [JsonProperty("blocked")]
        public bool Blocked { get; internal set; }

        /// <summary>
        /// Gets the Unix Timestamp when the <see cref="Contact"/> was created.
        /// </summary>
        [JsonProperty("created_at")]
        public int CreatedAtUnixTime { get; internal set; }

        /// <summary>
        /// Gets the time when the <see cref="Contact"/> was created.
        /// </summary>
        public DateTime CreatedAtTime => DateTimeOffset.FromUnixTimeSeconds(this.CreatedAtUnixTime).ToLocalTime().DateTime;

        /// <summary>
        /// Gets a value indicating whether the <see cref="Contact"/> is hidden.
        /// </summary>
        [JsonProperty("hidden")]
        public bool Hidden { get; internal set; }

        /// <summary>
        /// Gets the <see cref="Contact"/>'s unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the MRI of a <see cref="Contact"/>. Unsure of what this value represents.
        /// </summary>
        [JsonProperty("mri")]
        public string Mri { get; internal set; }

        /// <summary>
        /// Gets a <see cref="Contact"/>'s full name or username.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the reason for a <see cref="Contact"/>. Unsure of what this value represents.
        /// </summary>
        [JsonProperty("id")]
        public int Reason { get; internal set; }

        /// <summary>
        /// Gets the Unix Timestamp when the <see cref="Contact"/> was last updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public int UpdatedAtUnixTime { get; internal set; }

        /// <summary>
        /// Gets the time when the <see cref="Contact"/> was last updated.
        /// </summary>
        public DateTime UpdatedAtTime => DateTimeOffset.FromUnixTimeSeconds(this.UpdatedAtUnixTime).ToLocalTime().DateTime;

        /// <summary>
        /// Gets the user id for a <see cref="Contact"/>.
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; internal set; }
    }
}
