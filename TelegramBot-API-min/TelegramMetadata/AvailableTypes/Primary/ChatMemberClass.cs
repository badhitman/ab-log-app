////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;

namespace TelegramBot.TelegramMetadata.AvailableTypes
{
    /// <summary>
    /// This object contains information about one member of a chat.
    /// </summary>
    [DataContract]
    public class ChatMemberClass
    {
        /// <summary>
        /// Information about the user
        /// </summary>
        [DataMember]
        public UserClass user;

        /// <summary>
        ///  The member's status in the chat. Can be “creator”, “administrator”, “member”, “restricted”, “left” or “kicked”
        /// </summary>
        [DataMember]
        public string status;

        /// <summary>
        /// Optional.Restricted and kicked only.Date when restrictions will be lifted for this user, unix time
        /// </summary>
        [DataMember]
        public int until_date;

        /// <summary>
        /// Optional.Administrators only. True, if the bot is allowed to edit administrator privileges of that user
        /// </summary>
        [DataMember]
        public bool can_be_edited;

        /// <summary>
        /// Optional.Administrators only. True, if the administrator can change the chat title, photo and other settings
        /// </summary>
        [DataMember]
        public bool can_change_info;

        /// <summary>
        /// Optional.Administrators only. True, if the administrator can post in the channel, channels only
        /// </summary>
        [DataMember]
        public bool can_post_messages;

        /// <summary>
        /// Optional.Administrators only. True, if the administrator can edit messages of other users and can pin messages, channels only
        /// </summary>
        [DataMember]
        public bool can_edit_messages;

        /// <summary>
        /// Optional.Administrators only. True, if the administrator can delete messages of other users
        /// </summary>
        [DataMember]
        public bool can_delete_messages;

        /// <summary>
        /// Optional.Administrators only. True, if the administrator can invite new users to the chat
        /// </summary>
        [DataMember]
        public bool can_invite_users;

        /// <summary>
        /// Optional.Administrators only.True, if the administrator can restrict, ban or unban chat members
        /// </summary>
        [DataMember]
        public bool can_restrict_members;

        /// <summary>
        /// Optional. Administrators only. True, if the administrator can pin messages, supergroups only
        /// </summary>
        [DataMember]
        public bool can_pin_messages;

        /// <summary>
        /// Optional.Administrators only. True, if the administrator can add new administrators with a subset of his own privileges or demote administrators that he has promoted, directly or indirectly(promoted by administrators that were appointed by the user)
        /// </summary>
        [DataMember]
        public bool can_promote_members;

        /// <summary>
        /// Optional.Restricted only.True, if the user can send text messages, contacts, locations and venues
        /// </summary>
        [DataMember]
        public bool can_send_messages;

        /// <summary>
        /// Optional. Restricted only. True, if the user can send audios, documents, photos, videos, video notes and voice notes, implies can_send_messages
        /// </summary>
        [DataMember]
        public bool can_send_media_messages;

        /// <summary>
        /// Optional.Restricted only. True, if the user can send animations, games, stickers and use inline bots, implies can_send_media_messages
        /// </summary>
        [DataMember]
        public bool can_send_other_messages;

        /// <summary>
        /// Optional.Restricted only. True, if user may add web page previews to his messages, implies can_send_media_messages
        /// </summary>
        [DataMember]
        public bool can_add_web_page_previews;
    }
}
