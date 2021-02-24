using System;

namespace GroupMeClientApi.Models
{
    /// <summary>
    /// <see cref="Placeholders"/> provides support for using simulated, cached, or historical
    /// data to create GroupMe compatible objects that are API compatible with some functionality
    /// provided by <see cref="GroupMeClient"/>.
    /// Note that placeholders may be limited in functionality by GroupMe if they do not correspond to actual data.
    /// </summary>
    public class Placeholders
    {
        /// <summary>
        /// Creates a placeholder group chat.
        /// </summary>
        /// <param name="id">The unique group ID.</param>
        /// <param name="name">The name of the group.</param>
        /// <returns>A placeholder <see cref="Group"/>.</returns>
        public static Group CreatePlaceholderGroup(string id, string name)
        {
            return new Group()
            {
                Id = id,
                Name = name,
            };
        }

        /// <summary>
        /// Creates a placeholder direct message thread.
        /// </summary>
        /// <param name="id">The unique group ID.</param>
        /// <param name="otherUser">The user this DM thread is with.</param>
        /// <param name="conversationId">The conversation ID of this chat.</param>
        /// <returns>A placeholder <see cref="Group"/>.</returns>
        public static Chat CreatePlaceholderChat(string id, Member otherUser, string conversationId)
        {
            return new Chat()
            {
                Id = id,
                OtherUser = otherUser,
                ConversationId = conversationId,
            };
        }

        /// <summary>
        /// Creates a placeholder direct message thread.
        /// </summary>
        /// <param name="id">The unique group ID.</param>
        /// <param name="otherUserName">The name of the user this DM thread is with.</param>
        /// <param name="conversationId">The conversation ID of this chat.</param>
        /// <param name="otherUserID">The ID of the user this DM thread is with.</param>
        /// <param name="latestMessage">The latest message in the chat.</param>
        /// <returns>A placeholder <see cref="Group"/>.</returns>
        public static Chat CreatePlaceholderChat(string id, string otherUserName, string conversationId, string otherUserID = null, Message latestMessage = null)
        {
            otherUserID = otherUserID ?? Guid.NewGuid().ToString();

            return new Chat()
            {
                Id = id,
                LatestMessage = latestMessage,
                ConversationId = conversationId,
                OtherUser = new Member()
                {
                    Name = otherUserName,
                    Nickname = otherUserName,
                    Id = otherUserID,
                },
            };
        }

        /// <summary>
        /// Creates a placeholder member profile.
        /// </summary>
        /// <param name="id">The unique member ID.</param>
        /// <param name="name">The user's name.</param>
        /// <returns>Aa placeholder member.</returns>
        public static Member CreatePlaceholderMember(string id, string name)
        {
            return new Member()
            {
                Id = id,
                UserId = id,
                Name = name,
            };
        }
    }
}
