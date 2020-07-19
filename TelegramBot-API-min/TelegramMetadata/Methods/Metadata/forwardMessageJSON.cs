using System.Runtime.Serialization;
using TelegramBot.TelegramMetadata.AvailableTypes;

namespace TelegramBot.TelegramMetadata.Methods.Metadata
{
    /// <summary>
    /// Use this method to forward messages of any kind. On success, the sent Message is returned.
    /// </summary>
    [DataContract]
    public class forwardMessageJSON : _AbstractMethodsManager
    {
        /// <summary>
        /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
        /// </summary>
        [DataMember]
        public string chat_id;

        /// <summary>
        /// Unique identifier for the chat where the original message was sent (or channel username in the format @channelusername)
        /// </summary>
        [DataMember]
        public string from_chat_id;

        /// <summary>
        /// Sends the message silently. Users will receive a notification with no sound.
        /// </summary>
        [DataMember]
        public bool disable_notification;

        /// <summary>
        /// Message identifier in the chat specified in from_chat_i
        /// </summary>
        [DataMember]
        public long message_id;

        [DataContract]
        public class Result : ResponseClass
        {
            /// <summary>
            /// On success, represents a message is returned.
            /// </summary>
            [DataMember]
            public MessageClass result;
        }
    }
}
