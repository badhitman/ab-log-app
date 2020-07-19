using System.Runtime.Serialization;
using TelegramBot.TelegramMetadata.AvailableTypes;

namespace TelegramBot.TelegramMetadata.Methods.Metadata
{
    /// <summary>
    /// Use this method to send general files. On success, the sent Message is returned.
    /// Bots can currently send files of any type of up to 50 MB in size, this limit may be changed in the future.
    /// </summary>
    [DataContract]
    public class sendDocumentJSON : _AbstractMethodsManager
    {
        /// <summary>
        /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
        /// </summary>
        [DataMember]
        public string chat_id;

        /// <summary>
        /// InputFile or String 	Yes 	File to send. Pass a file_id as String to send a file that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a file from the Internet, or upload a new one using multipart/form-data.
        /// </summary>
        [DataMember]
        public object document;

        /// <summary>
        /// InputFile or String 	Optional 	Thumbnail of the file sent; can be ignored if thumbnail generation for the file is supported server-side. The thumbnail should be in JPEG format and less than 200 kB in size. A thumbnail‘s width and height should not exceed 320. Ignored if the file is not uploaded using multipart/form-data. Thumbnails can’t be reused and can be only uploaded as a new file, so you can pass “attach://<file_attach_name>” if the thumbnail was uploaded using multipart/form-data under <file_attach_name>.
        /// </summary>
        [DataMember]
        public object thumb;

        /// <summary>
        /// String 	Optional 	Document caption (may also be used when resending documents by file_id), 0-1024 characters
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
