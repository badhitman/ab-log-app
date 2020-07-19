////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;

namespace TelegramBot.TelegramMetadata.AvailableTypes
{
    /// <summary>
    /// This object represents a general file (as opposed to photos (https://core.telegram.org/bots/api#photosize), voice messages (https://core.telegram.org/bots/api#voice) and audio files (https://core.telegram.org/bots/api#audio)).
    /// </summary>
    [DataContract]
    public class DocumentClass
    {
        /// <summary>
        /// Unique file identifier
        /// </summary>
        [DataMember]
        public string file_id;

        /// <summary>
        /// Optional.Document thumbnail as defined by sender
        /// </summary>
        [DataMember]
        public PhotoSizeClass thumb;

        /// <summary>
        /// Optional. Original filename as defined by sender
        /// </summary>
        [DataMember]
        public string file_name;

        /// <summary>
        /// Optional. MIME type of the file as defined by sender
        /// </summary>
        [DataMember]
        public string mime_type;

        /// <summary>
        /// Optional. File size
        /// </summary>
        [DataMember]
        public int file_size;
    }
}
