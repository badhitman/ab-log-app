using System.Runtime.Serialization;

namespace TelegramBot.TelegramMetadata.Methods.Metadata
{
    /// <summary>
    /// Use this method to delete a message, including service messages, with the following limitations:
    /// - A message can only be deleted if it was sent less than 48 hours ago.
    /// - Bots can delete outgoing messages in private chats, groups, and supergroups.
    /// - Bots granted can_post_messages permissions can delete outgoing messages in channels.
    /// - If the bot is an administrator of a group, it can delete any message there.
    /// - If the bot has can_delete_messages permission in a supergroup or a channel, it can delete any message there.
    /// Returns True on success.
    /// </summary>
    [DataContract]
    public class deleteMessageJSON : _AbstractMethodsManager
    {
        /// <summary>
        /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
        /// </summary>
        [DataMember]
        public string chat_id;

        /// <summary>
        /// Identifier of the message to delete
        /// </summary>
        [DataMember]
        public string message_id;

        [DataContract]
        public class Result : ResponseClass
        {
            /// <summary>
            /// Returns True on success
            /// </summary>
            [DataMember]
            public bool result;
        }
    }
}
