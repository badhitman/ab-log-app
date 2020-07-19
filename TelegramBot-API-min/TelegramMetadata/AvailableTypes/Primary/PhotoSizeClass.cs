////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;

namespace TelegramBot.TelegramMetadata.AvailableTypes
{
    /// <summary>
    /// This object represents one size of a photo or a file/sticker thumbnail.
    /// </summary>
    [DataContract]
    public class PhotoSizeClass
    {
        /// <summary>
        /// Unique identifier for this file
        /// </summary>
        [DataMember]
        public string file_id;

        /// <summary>
        /// Photo width
        /// </summary>
        [DataMember]
        public int width;

        /// <summary>
        /// Photo height
        /// </summary>
        [DataMember]
        public int height;

        /// <summary>
        /// Optional.File size
        /// </summary>
        [DataMember]
        public int file_size;
    }
}
