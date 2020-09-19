using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

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
        /// Gets the ISO8601 timestamp when the <see cref="Contact"/> was created.
        /// </summary>
        [JsonProperty("created_at_iso8601")]
        public string CreatedAtIso8601Time { get; internal set; }

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
        /// Gets a <see cref="Contact"/>'s full name or username.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the Unix Timestamp when the <see cref="Contact"/> was last updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public int UpdatedAtUnixTime { get; internal set; }

        /// <summary>
        /// Gets the ISO 8601 timestamp when the <see cref="Contact"/> was last updated.
        /// </summary>
        [JsonProperty("updated_at_iso8601")]
        public string UpdatedAtIso8601Time { get; internal set; }

        /// <summary>
        /// Gets the time when the <see cref="Contact"/> was last updated.
        /// </summary>
        public DateTime UpdatedAtTime => DateTimeOffset.FromUnixTimeSeconds(this.UpdatedAtUnixTime).ToLocalTime().DateTime;

        /// <summary>
        /// Gets the user id for a <see cref="Contact"/>.
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; internal set; }

        /// <summary>
        /// Gets the <see cref="GroupMeClient"/> that manages.
        /// </summary>
        public GroupMeClient Client { get; internal set; }

        /// <summary>
        /// Blocks this <see cref="Contact"/>.
        /// </summary>
        /// <returns>True if this <see cref="Contact"/> is successfully blocked.</returns>
        public async Task<bool> BlockContact()
        {
            var me = this.Client.WhoAmI();
            var request = this.Client.CreateRestRequestV3($"/blocks?user={me.Id}&otherUser={this.Id}", Method.POST);

            var cancellationTokenSource = new CancellationTokenSource();
            var restResponse = await this.Client.ExecuteRestRequestAsync(request, cancellationTokenSource.Token);

            return restResponse.StatusCode == System.Net.HttpStatusCode.OK;
        }

        /// <summary>
        /// Unblocks this <see cref="Contact"/>.
        /// </summary>
        /// <returns>True if this <see cref="Contact"/> is successfully unblocked.</returns>
        public async Task<bool> UnblockContact()
        {
            var me = this.Client.WhoAmI();
            var request = this.Client.CreateRestRequestV3($"/blocks?user={me.Id}&otherUser={this.Id}", Method.DELETE);

            var cancellationTokenSource = new CancellationTokenSource();
            var restResponse = await this.Client.ExecuteRestRequestAsync(request, cancellationTokenSource.Token);

            return restResponse.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
