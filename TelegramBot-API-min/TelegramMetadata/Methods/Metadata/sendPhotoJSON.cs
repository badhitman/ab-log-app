using System.Runtime.Serialization;
using TelegramBot.TelegramMetadata.AvailableTypes;

namespace TelegramBot.TelegramMetadata.Methods.Metadata
{
    [DataContract]
    public class sendPhotoJSON : _AbstractMethodsManager
    {
        /// <summary>
        /// Integer or String 	Yes 	Unique identifier for the target chat or username of the target channel (in the format @channelusername)
        /// </summary>
        [DataMember]
        public string chat_id;

        /// <summary>
        /// InputFile or String 	Yes 	Photo to send. Pass a file_id as String to send a photo that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a photo from the Internet, or upload a new photo using multipart/form-data. More info on Sending Files »
        /// </summary>
        [DataMember]
        public object photo;

        /// <summary>
        /// String 	Optional 	Photo caption (may also be used when resending photos by file_id), 0-1024 characters
        /// </summary>
        [DataMember]
        public string caption;

        /// <summary>
        /// String 	Optional 	Send Markdown or HTML, if you want Telegram apps to show bold, italic, fixed-width text or inline URLs in the media caption.
        /// </summary>
        [DataMember]
        public string parse_mode;

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

        [DataContract]
        public class Result : ResponseClass
        {
            [DataMember]
            public MessageClass result;
        }
    }
}
