////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;

namespace TelegramBot.TelegramMetadata.AvailableTypes
{
    /// <summary>
    /// This object represents a file ready to be downloaded. The file can be downloaded via the link https://api.telegram.org/file/bot<token>/<file_path>.
    /// It is guaranteed that the link will be valid for at least 1 hour. When the link expires, a new one can be requested by calling getFile.
    /// </summary>
    [DataContract]
    public class FileClass
    {
        /// <summary>
        /// Unique identifier for this file
        /// </summary>
        [DataMember]
        public string file_id;

        /// <summary>
        /// Optional.File size, if known
        /// </summary>
        [DataMember]
        public int file_size;

        /// <summary>
        /// Optional. File path. Use https://api.telegram.org/file/bot<token>/<file_path> to get the file.
        /// </summary>
        [DataMember]
        public string file_path;
    }
}
