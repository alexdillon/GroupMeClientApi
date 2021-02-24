using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GroupMeClientApi.Models;
using GroupMeClientApi.Push.Bayeux;
using Newtonsoft.Json;

namespace GroupMeClientApi.Push
{
    /// <summary>
    /// PushClient allows for subscribing to push notification and updates
    /// from GroupMe for both Direct Messages/Chats and Groups.
    /// </summary>
    public class PushClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PushClient"/> class.
        /// </summary>
        /// <param name="client">
        /// The GroupMe Client that manages the Groups and Chats
        /// being monitored.
        /// </param>
        public PushClient(GroupMeClient client)
        {
            this.Client = client;
            this.BayeuxClient = new BayeuxClient(this.GroupMePushServerUrl, this.Client.AuthToken);
        }

        /// <summary>
        /// The event that is raised when a new push notification is received.
        /// </summary>
        public event EventHandler<Notifications.Notification> NotificationReceived;

        private GroupMeClient Client { get; }

        private BayeuxClient BayeuxClient { get; set; }

        private string GroupMePushServerUrl => "https://push.groupme.com/faye";

        /// <summary>
        /// Connects to the GroupMe Server to prepare for receiving notifications.
        /// </summary>
        public void Connect()
        {
            _ = this.AddSubscriptionMe();
            this.BayeuxClient.Connect();
        }

        /// <summary>
        /// Subscribes to push notifications for a specific <see cref="IMessageContainer"/>.
        /// </summary>
        /// <param name="container">The container to receive notifications for.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task SubscribeAsync(IMessageContainer container)
        {
            if (container is Group g)
            {
                await this.AddSubscriptionGroup(g);
            }
            else
            {
                Debug.WriteLine($"Subscribe not supported for name={container.Name}");
            }
        }

        /// <summary>
        /// Unsubscribes from push notifications for a specific <see cref="IMessageContainer"/>.
        /// </summary>
        /// <param name="container">The container to unsubscribe from receive notifications for.</param>
        public void Unsubscribe(IMessageContainer container)
        {
            if (container is Group g)
            {
                this.BayeuxClient.Unsubscribe($"/group/{g.Id}");
            }
            else
            {
                Debug.WriteLine($"Unsubscribe not supported for name={container.Name}");
            }
        }

        /// <summary>
        /// Subscribes to all push notifications for the current user.
        /// </summary>
        private async Task<bool> AddSubscriptionMe()
        {
            var me = this.Client.WhoAmI();
            return await this.BayeuxClient.Subscribe($"/user/{me.Id}", (json) => this.MeCallback(json));
        }

        /// <summary>
        /// Sends a Bayeux command requesting push notifications for a specific <see cref="Group"/>.
        /// </summary>
        /// <param name="group">The Group to receive notifications for.</param>
        private async Task<bool> AddSubscriptionGroup(Group group)
        {
            // Subscribe to channel.
            return await this.BayeuxClient.Subscribe($"/group/{group.Id}", (json) => this.GroupCallback(group, json));
        }

        private void GroupCallback(Group group, string jsonString)
        {
            var notification = JsonConvert.DeserializeObject<Notifications.Notification>(jsonString);

            Console.WriteLine($"Received {jsonString} for {group.Name}");

            try
            {
                var handler = this.NotificationReceived;
                handler?.Invoke(this, notification);
            }
            catch (Exception)
            {
                Debug.WriteLine("Error handling callback for 'Group' notification");
            }
        }

        private void MeCallback(string jsonString)
        {
            var notification = JsonConvert.DeserializeObject<Notifications.Notification>(jsonString);

            Console.WriteLine($"Received {jsonString} for ME! at {DateTime.Now.ToShortTimeString()}");

            try
            {
                var handler = this.NotificationReceived;
                handler?.Invoke(this, notification);
            }
            catch (Exception)
            {
                Debug.WriteLine("Error handling callback for 'Me' notification");
            }
        }
    }
}
