////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;

namespace TelegramBot.TelegramMetadata.AvailableTypes
{
    /// <summary>
    /// This object represents one special entity in a text message. For example, hashtags, usernames, URLs, etc.
    /// </summary>
    [DataContract]
    public class MessageEntityClass
    {
        /// <summary>
        /// Type of the entity. Can be mention (@username), hashtag, bot_command, url, email, bold (bold text), italic (italic text), code (monowidth string), pre (monowidth block), text_link (for clickable text URLs), text_mention (for users without usernames)
        /// </summary>
        [DataMember]
        public string type;

        /// <summary>
        /// Offset in UTF-16 code units to the start of the entity
        /// </summary>
        [DataMember]
        public long offset;

        /// <summary>
        /// Length of the entity in UTF-16 code units
        /// </summary>
        [DataMember]
        public long length;

        /// <summary>
        /// Optional. For “text_link” only, url that will be opened after user taps on the text
        /// </summary>
        [DataMember]
        public string url;

        /// <summary>
        /// Optional. For “text_mention” only, the mentioned user
        /// </summary>
        [DataMember]
        public UserClass user;
    }
}
