////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;

namespace TelegramBot.TelegramMetadata.AvailableTypes
{
    /// <summary>
    /// Represents a photo to be sent.
    /// </summary>
    [DataContract]
    public class InputMediaPhotoClass: InputMediaClass
    {
        /// <summary>
        /// Type of the result, must be photo
        /// </summary>
        [DataMember]
        public string type;

        /// <summary>
        /// File to send.Pass a file_id to send a file that exists on the Telegram servers(recommended), pass an HTTP URL for Telegram to get a file from the Internet, or pass "attach://<file_attach_name>" to upload a new one using multipart/form-data under<file_attach_name> name.More info on Sending Files https://core.telegram.org/bots/api#sending-files
        /// </summary>
        [DataMember]
        public string media;

        /// <summary>
        /// Optional.Caption of the photo to be sent, 0-200 characters
        /// </summary>
        [DataMember]
        public string caption;
    }
}
