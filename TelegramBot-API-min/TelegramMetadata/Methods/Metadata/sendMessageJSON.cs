////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;
using TelegramBot.TelegramMetadata.AvailableTypes;

namespace TelegramBot.TelegramMetadata.Methods.Metadata
{
    /// <summary>
    /// Use this method to send text messages.On success, the sent Message is returned.
    /// </summary>
    [DataContract]
    public class sendMessageJSON : _AbstractMethodsManager
    {
        /// <summary>
        /// Integer or String   Yes Unique identifier for the target chat or username of the target channel (in the format @channelusername)
        /// </summary>
        [DataMember]
        public string chat_id;

        /// <summary>
        /// Text of the message to be sent
        /// </summary>
        [DataMember]
        public string text;

        /// <summary>
        /// Optional Send Markdown or HTML, if you want Telegram apps to show bold, italic, fixed-width text or inline URLs in your bot's message.
        /// </summary>
        [DataMember]
        public string parse_mode;

        /// <summary>
        /// Optional    Disables link previews for links in this message
        /// </summary>
        [DataMember]
        public bool disable_web_page_preview;

        /// <summary>
        /// Optional    Sends the message silently.Users will receive a notification with no sound.
        /// </summary>
        [DataMember]
        public bool disable_notification;

        /// <summary>
        /// Optional    If the message is a reply, ID of the original message
        /// </summary>
        [DataMember]
        public long reply_to_message_id;

        [DataContract]
        public class Result : ResponseClass
        {
            /// <summary>
            /// Returns basic information about the bot in form of a User object.
            /// </summary>
            [DataMember]
            public MessageClass result;
        }
    }
}
