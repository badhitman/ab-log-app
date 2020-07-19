////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;

namespace TelegramBot.TelegramMetadata.AvailableTypes
{
    /// <summary>
    /// This object represents a message.
    /// </summary>
    [DataContract]
    public class MessageClass
    {

        /// <summary>
        /// Formatting options
        /// The Bot API supports basic formatting for messages.You can use bold and italic text, as well as inline links and pre-formatted code in your bots' messages.
        /// Telegram clients will render them accordingly. You can use either markdown-style or HTML-style formatting.
        /// Note that Telegram clients will display an alert to the user before opening an inline link(‘Open this link?’ together with the full URL).
        /// </summary>
        public enum ParseModes
        {
            HTML,
            Markdown
        }

        /// <summary>
        /// Unique message identifier inside this chat
        /// </summary>
        [DataMember]
        public string message_id;

        /// <summary>
        /// Optional. Sender, empty for messages sent to channels
        /// </summary>
        [DataMember]
        public UserClass from;

        /// <summary>
        /// Date the message was sent in Unix time
        /// </summary>
        [DataMember]
        public long date;

        /// <summary>
        /// Conversation the message belongs to
        /// </summary>
        [DataMember]
        public ChatClass chat;

        /// <summary>
        /// Optional. For forwarded messages, sender of the original message
        /// </summary>
        [DataMember]
        public UserClass forward_from;

        /// <summary>
        /// Optional. For messages forwarded from channels, information about the original channel
        /// </summary>
        [DataMember]
        public ChatClass forward_from_chat;

        /// <summary>
        /// Optional. For messages forwarded from channels, identifier of the original message in the channel
        /// </summary>
        [DataMember]
        public long forward_from_message_id;

        /// <summary>
        /// Optional. For messages forwarded from channels, signature of the post author if present
        /// </summary>
        [DataMember]
        public string forward_signature;

        /// <summary>
        /// Optional. For forwarded messages, date the original message was sent in Unix time
        /// </summary>
        [DataMember]
        public long forward_date;

        /// <summary>
        /// Optional. For replies, the original message. Note that the Message object in this field will not contain further reply_to_message fields even if it itself is a reply.
        /// </summary>
        [DataMember]
        public MessageClass reply_to_message;

        /// <summary>
        /// Optional. Date the message was last edited in Unix time
        /// </summary>
        [DataMember]
        public long edit_date;

        /// <summary>
        /// Optional. The unique identifier of a media message group this message belongs to
        /// </summary>
        [DataMember]
        public string media_group_id;

        /// <summary>
        /// Optional. Signature of the post author for messages in channels
        /// </summary>
        [DataMember]
        public string author_signature;

        /// <summary>
        /// Optional. For text messages, the actual UTF-8 text of the message, 0-4096 characters.
        /// </summary>
        [DataMember]
        public string text;

        /// <summary>
        /// Optional. For text messages, special entities like usernames, URLs, bot commands, etc. that appear in the text
        /// </summary>
        [DataMember]
        public MessageEntityClass entities;

        /// <summary>
        /// Optional. For messages with a caption, special entities like usernames, URLs, bot commands, etc. that appear in the caption
        /// </summary>
        [DataMember]
        public MessageEntityClass caption_entities;

        /// <summary>
        /// Optional. Message is an audio file, information about the file
        /// </summary>
        [DataMember]
        public AudioClass audio;

        /// <summary>
        /// Optional. Message is a general file, information about the file
        /// </summary>
        [DataMember]
        public DocumentClass document;

        /// <summary>
        /// Optional. Message is a photo, available sizes of the photo
        /// </summary>
        [DataMember]
        public PhotoSizeClass[] photo;

        /// <summary>
        /// Optional. Caption for the audio, document, photo, video or voice, 0-200 characters
        /// </summary>
        [DataMember]
        public string caption;

        /// <summary>
        /// Optional. Message is a shared contact, information about the contact
        /// </summary>
        [DataMember]
        public ContactClass contact;

        /// <summary>
        /// Optional. New members that were added to the group or supergroup and information about them (the bot itself may be one of these members)
        /// </summary>
        [DataMember]
        public UserClass[] new_chat_members;

        /// <summary>
        /// Optional. A member was removed from the group, information about them (this member may be the bot itself)
        /// </summary>
        [DataMember]
        public UserClass left_chat_member;

        /// <summary>
        /// Optional. A chat title was changed to this value
        /// </summary>
        [DataMember]
        public string new_chat_title;

        /// <summary>
        /// Optional. A chat photo was change to this value
        /// </summary>
        [DataMember]
        public PhotoSizeClass[] new_chat_photo;

        /// <summary>
        /// Optional. Service message: the chat photo was deleted
        /// </summary>
        [DataMember]
        public bool delete_chat_photo;

        /// <summary>
        /// Optional. Service message: the group has been created
        /// </summary>
        [DataMember]
        public bool group_chat_created;

        /// <summary>
        /// Optional. Service message: the supergroup has been created. This field can‘t be received in a message coming through updates, because bot can’t be a member of a supergroup when it is created. It can only be found in reply_to_message if someone replies to a very first message in a directly created supergroup.
        /// </summary>
        [DataMember]
        public bool supergroup_chat_created;

        /// <summary>
        /// Optional. Service message: the channel has been created. This field can‘t be received in a message coming through updates, because bot can’t be a member of a channel when it is created. It can only be found in reply_to_message if someone replies to a very first message in a channel.
        /// </summary>
        [DataMember]
        public bool channel_chat_created;

        /// <summary>
        /// Optional. The group has been migrated to a supergroup with the specified identifier. This number may be greater than 32 bits and some programming languages may have difficulty/silent defects in interpreting it. But it is smaller than 52 bits, so a signed 64 bit integer or double-precision float type are safe for storing this identifier.
        /// </summary>
        [DataMember]
        public long migrate_to_chat_id;

        /// <summary>
        /// Optional. The supergroup has been migrated from a group with the specified identifier. This number may be greater than 32 bits and some programming languages may have difficulty/silent defects in interpreting it. But it is smaller than 52 bits, so a signed 64 bit integer or double-precision float type are safe for storing this identifier.
        /// </summary>
        [DataMember]
        public long migrate_from_chat_id;

        /// <summary>
        /// Optional. Specified message was pinned. Note that the Message object in this field will not contain further reply_to_message fields even if it is itself a reply.
        /// </summary>
        [DataMember]
        public MessageClass pinned_message;
    }
}
