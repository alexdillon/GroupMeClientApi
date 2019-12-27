﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroupMeClientApi.Models
{
    /// <summary>
    /// <see cref="IMessageContainer"/> represents a generic named collection of <see cref="Message"/>s,
    /// along with common client functionality and features. <see cref="Group"/> and <see cref="Chat"/> are
    /// two possible implementations of this interface.
    /// </summary>
    public interface IMessageContainer : IAvatarSource
    {
        /// <summary>
        /// Gets a unique identfier for this <see cref="IMessageContainer"/>.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the Date and Time when this collection was last updated.
        /// </summary>
        DateTime UpdatedAtTime { get; }

        /// <summary>
        /// Gets the Date and Time when this collection was created.
        /// </summary>
        DateTime CreatedAtTime { get; }

        /// <summary>
        /// Gets a list of <see cref="Message"/>s in this collection.
        /// </summary>
        List<Message> Messages { get; }

        /// <summary>
        /// Gets a copy of the latest message for preview purposes.
        /// Note that API Operations, like <see cref="Message.LikeMessage"/> cannot be performed.
        /// See <see cref="Messages"/> list instead for full message objects.
        /// </summary>
        Message LatestMessage { get; }

        /// <summary>
        /// Gets the total number of messages within this <see cref="IMessageContainer"/>.
        /// </summary>
        int TotalMessageCount { get; }

        /// <summary>
        /// Gets the <see cref="GroupMeClient"/> that manages this collection.
        /// </summary>
        GroupMeClient Client { get; }

        /// <summary>
        /// Gets the latest Read Receipt for this collection.
        /// </summary>
        ChatMessagesList.MessageListResponse.ReadReceipt ReadReceipt { get; }

        /// <summary>
        /// Sends a new message to this <see cref="IMessageContainer"/>.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>A <see cref="bool"/> indicating the success of the send operation.</returns>
        Task<bool> SendMessage(Message message);

        /// <summary>
        /// Returns a set of messages from a this container.
        /// </summary>
        /// <param name="mode">The method that should be used to determine the set of messages returned.</param>
        /// <param name="messageId">The Message Id that will be used by the sorting mode set in <paramref name="mode"/>.</param>
        /// <returns>A list of <see cref="Message"/>.</returns>
        Task<ICollection<Message>> GetMessagesAsync(MessageRetreiveMode mode = MessageRetreiveMode.None, string messageId = "");

        /// <summary>
        /// Returns a set of messages from a this container in the largest possible block-size.
        /// </summary>
        /// <param name="mode">The method that should be used to determine the set of messages returned.</param>
        /// <param name="messageId">The Message Id that will be used by the sorting mode set in <paramref name="mode"/>.</param>
        /// <returns>A list of <see cref="Message"/>.</returns>
        Task<ICollection<Message>> GetMaxMessagesAsync(MessageRetreiveMode mode = MessageRetreiveMode.None, string messageId = "");

        /// <summary>
        /// Returns the authenticated user who is accessing this collection.
        /// </summary>
        /// <returns>A <see cref="Member"/>.</returns>
        Member WhoAmI();
    }
}
