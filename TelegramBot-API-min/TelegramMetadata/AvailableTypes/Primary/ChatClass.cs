////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;

namespace TelegramBot.TelegramMetadata.AvailableTypes
{
    /// <summary>
    /// This object represents a chat.
    /// </summary>
    [DataContract]
    public class ChatClass
    {
        /// <summary>
        /// Unique identifier for this chat. This number may be greater than 32 bits and some programming languages may have difficulty/silent defects in interpreting it. But it is smaller than 52 bits, so a signed 64 bit integer or double-precision float type are safe for storing this identifier.
        /// </summary>
        [DataMember]
        public long id;

        /// <summary>
        /// Type of chat, can be either “private”, “group”, “supergroup” or “channel”
        /// </summary>
        [DataMember]
        public string type;

        /// <summary>
        /// Optional. Title, for supergroups, channels and group chats
        /// </summary>
        [DataMember]
        public string title;

        /// <summary>
        /// Optional. Username, for private chats, supergroups and channels if available
        /// </summary>
        [DataMember]
        public string username;

        /// <summary>
        /// Optional. First name of the other party in a private chat
        /// </summary>
        [DataMember]
        public string first_name;

        /// <summary>
        /// Optional. Last name of the other party in a private chat
        /// </summary>
        [DataMember]
        public string last_name;

        /// <summary>
        /// Optional. True if a group has ‘All Members Are Admins’ enabled.
        /// </summary>
        [DataMember]
        public bool all_members_are_administrators;

        /// <summary>
        /// Optional. Chat photo. Returned only in getChat.
        /// </summary>
        [DataMember]
        public ChatPhotoClass photo;

        /// <summary>
        /// Optional. Description, for supergroups and channel chats. Returned only in getChat.
        /// </summary>
        [DataMember]
        public string description;

        /// <summary>
        /// Optional. Chat invite link, for supergroups and channel chats. Returned only in getChat.
        /// </summary>
        [DataMember]
        public string invite_link;

        /// <summary>
        /// Optional. Pinned message, for supergroups and channel chats. Returned only in getChat.
        /// </summary>
        [DataMember]
        public MessageClass pinned_message;

        /// <summary>
        /// Optional. For supergroups, name of group sticker set. Returned only in getChat.
        /// </summary>
        [DataMember]
        public string sticker_set_name;

        /// <summary>
        /// Optional. True, if the bot can change the group sticker set. Returned only in getChat.
        /// </summary>
        [DataMember]
        public bool can_set_sticker_set;
    }
}
