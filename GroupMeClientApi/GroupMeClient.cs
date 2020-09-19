using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GroupMeClientApi.Models;
using Newtonsoft.Json;
using RestSharp;

namespace GroupMeClientApi
{
    /// <summary>
    /// <see cref="GroupMeClient"/> allows for interaction with the GroupMe API for messaging functionality.
    /// </summary>
    public class GroupMeClient
    {
        /// <summary>
        /// The Base URL for GroupMe's V2 API endpoint.
        /// </summary>
        private const string GroupMeAPIUrlV2 = "https://v2.groupme.com/";

        /// <summary>
        /// The Base URL for GroupMe's V3 API endpoint.
        /// </summary>
        private const string GroupMeAPIUrlV3 = "https://api.groupme.com/v3";

        /// <summary>
        /// The Base URL for GroupMe's V4 API endpoint.
        /// </summary>
        private const string GroupMeAPIUrlV4 = "https://api.groupme.com/v4";

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupMeClient"/> class to perform GroupMe API Operations.
        /// </summary>
        /// <param name="authToken">The OAuth Token used to authenticate the client.</param>
        public GroupMeClient(string authToken)
        {
            this.AuthToken = authToken;

            this.GroupsList = new List<Group>();
            this.ChatsList = new List<Chat>();
            this.ContactsList = new List<Contact>();
        }

        /// <summary>
        /// Gets or sets the <see cref="ImageDownloader"/> that is used for image downloads.
        /// </summary>
        public virtual ImageDownloader ImageDownloader { get; set; } = new ImageDownloader();

        /// <summary>
        /// Gets an enumeration of <see cref="Contact"/>s controlled by the API Client.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Contact"/>s.</returns>
        public virtual IEnumerable<Contact> Contacts => this.ContactsList;

        /// <summary>
        /// Gets the Auth Token used to authenticate a GroupMe API Call.
        /// </summary>
        internal string AuthToken { get; }

        /// <summary>
        /// Gets the <see cref="RestClient"/> that is used to perform GroupMe API calls.
        /// </summary>
        private RestClient ApiClient { get; } = new RestClient();

        /// <summary>
        /// Gets the <see cref="HttpClient"/> that is used for all raw HTTP connections.
        /// </summary>
        private HttpClient HttpClient { get; } = new HttpClient();

        /// <summary>
        /// Gets or sets the client used to subscribe to push notifications, if enabled.
        /// </summary>
        private Push.PushClient PushClient { get; set; }

        /// <summary>
        /// Gets or sets the internal cache of <see cref="Group"/>s that are tracked by this Client.
        /// </summary>
        private List<Group> GroupsList { get; set; }

        /// <summary>
        /// Gets or sets the internal cache of <see cref="Chat"/>s that are tracked by this Client.
        /// </summary>
        private List<Chat> ChatsList { get; set; }

        /// <summary>
        /// Gets or sets the internal cache of <see cref="Contact"/>s that are tracked by this Client.
        /// </summary>
        private List<Contact> ContactsList { get; set; }

        /// <summary>
        /// Gets or sets the cached information returned from the GroupMe Identity endpoint
        /// that uniquely identifies the currently authenticated user.
        /// </summary>
        private Member CachedMe { get; set; }

        /// <summary>
        /// Gets a enumeration of <see cref="Group"/>s controlled by the API Client.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> for the <see cref="Group"/>.
        /// </returns>
        public virtual IEnumerable<Group> Groups()
        {
            foreach (var group in this.GroupsList)
            {
                yield return group;
            }
        }

        /// <summary>
        /// Gets a enumeration of <see cref="Chat"/>s controlled by the API Client.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> for the <see cref="Chat"/>.
        /// </returns>
        public virtual IEnumerable<Chat> Chats()
        {
            foreach (var chat in this.ChatsList)
            {
                yield return chat;
            }
        }

        /// <summary>
        /// Returns a listing of all Group Chats a user is a member of.
        /// </summary>
        /// <returns>A list of <see cref="Group"/>.</returns>
        public virtual async Task<ICollection<Group>> GetGroupsAsync()
        {
            const int MaxPerPage = 500; // GroupMe allows 500 Groups/page

            var request = this.CreateRestRequestV3($"/groups", Method.GET);
            request.AddParameter("per_page", MaxPerPage); // always all available groups.

            var cancellationTokenSource = new CancellationTokenSource();
            var restResponse = await this.ApiClient.ExecuteAsync(request, cancellationTokenSource.Token);
            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var results = JsonConvert.DeserializeObject<GroupsList>(restResponse.Content);

                foreach (var group in results.Groups)
                {
                    // ensure every Group has a reference to the parent client (this)
                    group.Client = this;

                    var oldGroup = this.GroupsList.Find(g => g.Id == group.Id);

                    if (oldGroup == null)
                    {
                        this.GroupsList.Add(group);
                    }
                    else
                    {
                        DataMerger.MergeGroup(oldGroup, group);
                    }
                }

                return results.Groups;
            }
            else
            {
                throw new System.Net.WebException($"Failure retreving /Groups. Status Code {restResponse.StatusCode}");
            }
        }

        /// <summary>
        /// Returns a listing of all Direct Messages / Chats a user is a member of.
        /// </summary>
        /// <returns>A list of <see cref="Chat"/>.</returns>
        public virtual async Task<ICollection<Chat>> GetChatsAsync()
        {
            const int MaxPerPage = 100; // GroupMe allows 100 Chats/Page.

            var request = this.CreateRestRequestV3($"/chats", Method.GET);
            request.AddParameter("per_page", MaxPerPage); // always all available chats.

            var cancellationTokenSource = new CancellationTokenSource();
            var restResponse = await this.ApiClient.ExecuteAsync(request, cancellationTokenSource.Token);

            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var results = JsonConvert.DeserializeObject<ChatsList>(restResponse.Content);

                foreach (var chat in results.Chats)
                {
                    // ensure every Chat has a reference to the parent client (this)
                    chat.Client = this;

                    // required to establish a constant, non-foreign-key Primary Key for Chat
                    chat.Id = chat.OtherUser.Id;

                    var oldChat = this.ChatsList.Find(g => g.Id == chat.Id);

                    if (oldChat == null)
                    {
                        this.ChatsList.Add(chat);
                    }
                    else
                    {
                        DataMerger.MergeChat(oldChat, chat);
                    }
                }

                return results.Chats;
            }
            else
            {
                throw new System.Net.WebException($"Failure retreving /Groups. Status Code {restResponse.StatusCode}");
            }
        }

        /// <summary>
        /// Returns a listing of a maximum of 200 <see cref="Contact"/>s of the user, as specified by input parameters.
        /// </summary>
        /// <param name="retrieveSinceMode">Specify whether to employ "since" retrieval mode for receiving list of <see cref="Contact"/>s.</param>
        /// <param name="messageCreatedAtIso8601">The ISO8601 timestamp used to determine which contacts to receive if the "since" retrieval mode is true.</param>
        /// <returns>A list of <see cref="Contact"/>s of maximum length 200.</returns>
        public virtual async Task<ICollection<Contact>> GetContactsAsync(bool retrieveSinceMode = false, string messageCreatedAtIso8601 = "")
        {
            const string IncludeBlocked = "true";     // based on GroupMe Web, can retrieve 200 contacts per API request

            var request = this.CreateRestRequestV4($"/relationships", Method.GET);
            request.AddParameter("include_blocked", IncludeBlocked);

            if (retrieveSinceMode)
            {
                request.AddParameter("since", messageCreatedAtIso8601);
            }

            var cancellationTokenSource = new CancellationTokenSource();
            var restResponse = await this.ApiClient.ExecuteAsync(request, cancellationTokenSource.Token);

            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var results = JsonConvert.DeserializeObject<ContactsList>(restResponse.Content);

                foreach (var contact in results.Contacts)
                {
                    var oldContact = this.ContactsList.Find(c => c.Id == contact.Id);
                    if (oldContact == null)
                    {
                        this.ContactsList.Add(contact);
                    }
                    else
                    {
                        DataMerger.MergeContact(oldContact, contact);
                    }

                    contact.Client = this;
                }

                return results.Contacts;
            }
            else
            {
                throw new System.Net.WebException($"Failure retrieving /Contacts. Status Code {restResponse.StatusCode}");
            }
        }

        /// <summary>
        /// Returns a listing of all <see cref="Contact"/>s of the user.
        /// </summary>
        /// <returns>A list of <see cref="Contact"/>s.</returns>
        public virtual async Task<ICollection<Contact>> GetAllContactsAsync()
        {
            var contacts = await this.GetContactsAsync();
            var numContactsReturned = this.ContactsList.Count;
            while (numContactsReturned > 0)
            {
                var earliestContact = this.ContactsList.OrderByDescending(c => c.CreatedAtIso8601Time).First();
                var returnedContacts = await this.GetContactsAsync(true, earliestContact.CreatedAtIso8601Time);
                numContactsReturned = returnedContacts.Count;
            }

            return this.ContactsList;
        }

        /// <summary>
        /// Returns the authenticated user. A cached copy
        /// will be returned unless an update is forced.
        /// </summary>
        /// <param name="forceUpdate">Force an API refresh.</param>
        /// <returns>A <see cref="Member"/>.</returns>
        public virtual Member WhoAmI(bool forceUpdate = false)
        {
            if (this.CachedMe != null && !forceUpdate)
            {
                return this.CachedMe;
            }

            var request = this.CreateRestRequestV3($"/users/me", Method.GET);

            var restResponse = this.ApiClient.Execute(request);

            if (restResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var results = JsonConvert.DeserializeObject<MemberResponse>(restResponse.Content);
                this.CachedMe = results.Member;

                return this.CachedMe;
            }
            else
            {
                throw new System.Net.WebException($"Failure retreving /Users/Me. Status Code {restResponse.StatusCode}");
            }
        }

        /// <summary>
        /// Enables subscribing to push notifications for all <see cref="Groups"/>
        /// and <see cref="Chat"/> controlled by this client.
        /// This must be enabled before calling <see cref="GetChatsAsync"/> or <see cref="GetGroupsAsync"/>.
        /// </summary>
        /// <returns>The <see cref="PushClient"/> that used for push operations.</returns>
        public virtual Push.PushClient EnablePushNotifications()
        {
            this.PushClient = new Push.PushClient(this);

            this.PushClient.Connect();

            return this.PushClient;
        }

        /// <summary>
        /// Creates a new <see cref="RestRequest"/> object to perform a GroupMe API Call including the Authorization Token.
        /// </summary>
        /// <param name="resource">The GroupMe API resource to call.</param>
        /// <param name="method">The method used for the API Call.</param>
        /// <returns>A <see cref="RestRequest"/> with the user's access token.</returns>
        internal RestRequest CreateRawRestRequest(string resource, Method method)
        {
            var request = new RestRequest(resource, method)
            {
                JsonSerializer = JsonAdapter.Default,
            };

            request.AddHeader("X-Access-Token", this.AuthToken);

            return request;
        }

        /// <summary>
        /// Creates a new <see cref="RestRequest"/> object to perform a GroupMe API Call to the V2 endpoint.
        /// The generated request includes the Authorization Token.
        /// </summary>
        /// <param name="resource">The GroupMe API resource to call.</param>
        /// <param name="method">The method used for the API Call.</param>
        /// <returns>A <see cref="RestRequest"/> with the user's access token.</returns>
        internal RestRequest CreateRestRequestV2(string resource, Method method)
        {
            return this.CreateRawRestRequest($"{GroupMeAPIUrlV2}{resource}", method);
        }

        /// <summary>
        /// Creates a new <see cref="RestRequest"/> object to perform a GroupMe API Call to the V3 endpoint.
        /// The generated request includes the Authorization Token.
        /// </summary>
        /// <param name="resource">The GroupMe API resource to call.</param>
        /// <param name="method">The method used for the API Call.</param>
        /// <returns>A <see cref="RestRequest"/> with the user's access token.</returns>
        internal RestRequest CreateRestRequestV3(string resource, Method method)
        {
            return this.CreateRawRestRequest($"{GroupMeAPIUrlV3}{resource}", method);
        }

        /// <summary>
        /// Creates a new <see cref="RestRequest"/> object to perform a GroupMe API Call to the V4 endpoint.
        /// The generated request includes the Authorization Token.
        /// </summary>
        /// <param name="resource">The GroupMe API resource to call.</param>
        /// <param name="method">The method used for the API Call.</param>
        /// <returns>A <see cref="RestRequest"/> with the user's access token.</returns>
        internal RestRequest CreateRestRequestV4(string resource, Method method)
        {
            return this.CreateRawRestRequest($"{GroupMeAPIUrlV4}{resource}", method);
        }

        /// <summary>
        /// Executes a REST request against the GroupMe API.
        /// Authentication parameteters are automatically included with the request.
        /// </summary>
        /// <param name="request">The request to execute.</param>
        /// <param name="cancellationToken">A token to allow for cancellation of the <see cref="Task"/>.</param>
        /// <returns>The response provided from the REST endpoint.</returns>
        internal Task<IRestResponse> ExecuteRestRequestAsync(RestRequest request, CancellationToken cancellationToken)
        {
            return this.ApiClient.ExecuteAsync(request, cancellationToken);
        }

        /// <summary>
        /// Executes a REST request against the GroupMe API using HTTP POST.
        /// Authentication parameteters are automatically included with the request.
        /// </summary>
        /// <param name="endPoint">The REST endpoint to connect to.</param>
        /// <param name="requestBody">The body of the request.</param>
        /// <param name="contentType">The content type of the request.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to asychronously cancel the request.</param>
        /// <param name="uploadProgress">A monitor object to receive status updates about the asychronous upload progress.</param>
        /// <returns>The REST server response.</returns>
        internal Task<HttpResponseMessage> ExecuteRestRequestAsync(string endPoint, byte[] requestBody, string contentType, CancellationToken cancellationToken, UploadProgress uploadProgress)
        {
            var content = new ProgressableBlockContent(requestBody, uploadProgress);
            content.Headers.Add("X-Access-Token", this.AuthToken);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            content.Headers.ContentLength = requestBody.Length;

            return this.HttpClient.PostAsync(new Uri(endPoint), content, cancellationToken);
        }
    }
}
