////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;

namespace TelegramBot.TelegramMetadata.AvailableTypes
{
    /// <summary>
    /// This object represents a chat photo.
    /// </summary>
    [DataContract]
    public class ChatPhotoClass
    {
        /// <summary>
        /// Unique file identifier of small (160x160) chat photo. This file_id can be used only for photo download.
        /// </summary>
        [DataMember]
        public string small_file_id;

        /// <summary>
        /// Unique file identifier of big (640x640) chat photo. This file_id can be used only for photo download.
        /// </summary>
        [DataMember]
        public string big_file_id;
    }
}
