using System.Runtime.Serialization;
using TelegramBot.TelegramMetadata.AvailableTypes;

namespace TelegramBot.TelegramMetadata.Methods.Metadata
{
    [DataContract]
    public class sendMediaGroupJSON : _AbstractMethodsManager
    {
        /// <summary>
        /// Integer or String 	Yes 	Unique identifier for the target chat or username of the target channel (in the format @channelusername)
        /// </summary>
        [DataMember]
        public string chat_id;

        /// <summary>
        /// Array of InputMediaPhoto and InputMediaVideo 	Yes 	A JSON-serialized array describing photos and videos to be sent, must include 2–10 items 
        /// </summary>
        [DataMember]
        public object[] media;

        /// <summary>
        /// Boolean 	Optional 	Sends the message silently. Users will receive a notification with no sound.
        /// </summary>
        [DataMember]
        public bool disable_notification;

        /// <summary>
        /// Integer 	Optional 	If the message is a reply, ID of the original message
        /// </summary>
        [DataMember]
        public string reply_to_message_id;

        public class Result : ResponseClass
        {
            [DataMember]
            public MessageClass[] result;
        }
    }
}