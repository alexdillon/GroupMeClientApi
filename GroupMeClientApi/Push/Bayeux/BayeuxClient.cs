using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GroupMeClientApi.Push.Bayeux.Messages;
using Newtonsoft.Json;

namespace GroupMeClientApi.Push.Bayeux
{
    /// <summary>
    /// <see cref="BayeuxClient"/> provides a client for connection to a push notification
    /// server using the Bayeux/Faye protocol.
    /// </summary>
    internal class BayeuxClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BayeuxClient"/> class.
        /// </summary>
        /// <param name="endpoint">The server to connect to.</param>
        /// <param name="accessToken">The GroupMe Access Token to provide in the extended connection data.</param>
        public BayeuxClient(string endpoint, string accessToken)
        {
            this.EndPoint = endpoint;
            this.AccessToken = accessToken;

            this.IdCounter = 1;
        }

        private enum State
        {
            Disconnected,
            Handshaking,
            Connected,
            Polling,
        }

        private string EndPoint { get; }

        private string AccessToken { get; }

        private int IdCounter { get; set; }

        private string ClientId { get; set; }

        private State CurrentState { get; set; }

        private HttpClient HttpClient { get; } = new HttpClient();

        private Stopwatch ConnectionTime { get; } = new Stopwatch();

        private CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

        private Dictionary<string, Action<string>> Subscriptions { get; } = new Dictionary<string, Action<string>>();

        private TimeSpan DefaultTimeout => TimeSpan.FromSeconds(30);

        private TimeSpan LongPollTimeout => TimeSpan.FromMinutes(10);

        private TimeSpan MaxConnectionDuration => TimeSpan.FromMinutes(30);

        /// <summary>
        /// Connects to the Bayeux server.
        /// </summary>
        public void Connect()
        {
            Task.Run(this.ConnectionLoop);
        }

        /// <summary>
        /// Subscribes immediately to a new subscription.
        /// </summary>
        /// <param name="subscription">The Bayeux name of the subscription.</param>
        /// <param name="messageCallback">The callback to execute when new data is pushed for this subscription.</param>
        /// <returns>A value indiciating whether the subscription was successful.</returns>
        public async Task<bool> Subscribe(string subscription, Action<string> messageCallback)
        {
            if (!this.Subscriptions.ContainsKey(subscription))
            {
                this.Subscriptions.Add(subscription, messageCallback);

                if (this.CurrentState == State.Polling)
                {
                    // Subscribe to the channel now
                    return await this.Subscribe(subscription);
                }
                else
                {
                    // Just wait for the main connection loop to pick up the subscription
                    return true;
                }
            }

            return true;
        }

        /// <summary>
        /// Unsubscribes from a notifications channel.
        /// </summary>
        /// <param name="subscription">The name of the subscription to unsubscribe.</param>
        public void Unsubscribe(string subscription)
        {
            if (this.Subscriptions.ContainsKey(subscription))
            {
                this.Subscriptions.Remove(subscription);
            }
        }

        private async Task ConnectionLoop()
        {
            while (!this.CancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    switch (this.CurrentState)
                    {
                        case State.Disconnected:
                            this.CurrentState = State.Handshaking;
                            var handshakeSuccess = await this.Handshake();

                            if (!handshakeSuccess)
                            {
                                this.CurrentState = State.Disconnected;
                            }
                            else
                            {
                                this.ConnectionTime.Restart();
                            }

                            break;

                        case State.Handshaking:
                            // We shouldn't ever be here?
                            await Task.Delay(TimeSpan.FromSeconds(1));
                            break;

                        case State.Connected:
                            // Restore subscriptions
                            foreach (var sub in this.Subscriptions)
                            {
                                await this.Subscribe(sub.Key);
                            }

                            this.CurrentState = State.Polling;

                            break;

                        case State.Polling:
                            var pollSuccess = await this.LongPolling();

                            if (!pollSuccess || this.ConnectionTime.Elapsed > this.MaxConnectionDuration)
                            {
                                this.CurrentState = State.Disconnected;
                                this.ClientId = string.Empty;
                            }

                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Notifications - Exception in Bayeux main loop: {ex.Message}");
                    throw;
                }

                this.CancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
        }

        private async Task<bool> Handshake()
        {
            string[] supportedProtocols = { "long-polling" };
            var message = new
            {
                channel = "/meta/handshake",
                version = "1.0",
                supportedConnectionTypes = supportedProtocols,
                successful = false,
                id = this.IdCounter,
            };

            var result = await this.PostMessage(new object[] { message }, this.DefaultTimeout);

            if (result != null && result.IsSuccessStatusCode)
            {
                var resultJson = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<List<HandshakeResponse>>(resultJson).FirstOrDefault();
                if (response != null)
                {
                    // Success
                    this.ClientId = response.ClientId;
                    this.CurrentState = State.Connected;
                    Debug.WriteLine($"Handshake - ClientId: {this.ClientId}");
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> Subscribe(string subName)
        {
            var subMsg = new
            {
                channel = "/meta/subscribe",
                clientId = this.ClientId,
                subscription = subName,
                id = this.IdCounter,
                ext = new
                {
                    access_token = this.AccessToken,
                    timestamp = DateTime.Now.ToUniversalTime().ToBinary(),
                },
            };

            var result = await this.PostMessage(new object[] { subMsg }, this.DefaultTimeout);
            if (result != null && result.IsSuccessStatusCode)
            {
                var resultJson = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<List<SubscriptionResponse>>(resultJson).FirstOrDefault();
                if (response != null)
                {
                    // Success
                    return response.Success;
                }
            }

            return false;
        }

        private async Task<bool> LongPolling()
        {
            Debug.WriteLine($"Notifications - Running Long Poll: {DateTime.Now.ToLongTimeString()}");

            var pollMsg = new
            {
                channel = "/meta/connect",
                clientId = this.ClientId,
                connectionType = "long-polling",
                id = this.IdCounter,
            };

            var result = await this.PostMessage(new object[] { pollMsg }, this.LongPollTimeout);
            if (result == null)
            {
                // Timeout or Web Exception
                return false;
            }
            else if (!result.IsSuccessStatusCode)
            {
                // Something went wrong
                return false;
            }
            else if (result.IsSuccessStatusCode)
            {
                try
                {
                    var resultJson = await result.Content.ReadAsStringAsync();
                    var decoded = Newtonsoft.Json.Linq.JArray.Parse(resultJson);

                    var connectionResponse = decoded[0];
                    var handshakeInfo = connectionResponse.ToObject<HandshakeResponse>();

                    var payload = decoded[1]["data"];
                    var channel = decoded[1]["channel"].ToString();

                    var payloadJson = payload.ToString();

                    if (this.Subscriptions.ContainsKey(channel))
                    {
                        this.Subscriptions[channel].Invoke(payloadJson);
                    }

                    return handshakeInfo.Successful;
                }
                catch (Exception ex)
                {
                    // If the messages aren't formatted as expected, or anything fails to parse
                    // indicate that the connection is failed
                    Debug.WriteLine($"Notifications - Exception in Bayeux parsing: {ex.Message}");
                    return false;
                }
            }

            return false;
        }

        private async Task<HttpResponseMessage> PostMessage(object message, TimeSpan timeout)
        {
            try
            {
                this.IdCounter++;

                var requestCts = new CancellationTokenSource();
                requestCts.CancelAfter(timeout);

                var result = await this.HttpClient.PostAsync(
                    this.EndPoint,
                    new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"),
                    requestCts.Token);

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
