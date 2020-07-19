////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;
using TelegramBot.TelegramMetadata.AvailableTypes;

namespace TelegramBot.TelegramMetadata.Methods.Metadata
{
    /// <summary>
    /// Use this method to get basic info about a file and prepare it for downloading.
    /// For the moment, bots can download files of up to 20MB in size. On success, a File object is returned.
    /// The file can then be downloaded via the link https://api.telegram.org/file/bot<token>/<file_path>, where <file_path> is taken from the response.
    /// It is guaranteed that the link will be valid for at least 1 hour. When the link expires, a new one can be requested by calling getFile again.
    /// </summary>
    [DataContract]
    class getFileJSON : _AbstractMethodsManager
    {
        /// <summary>
        /// File identifier to get info about
        /// </summary>
        [DataMember]
        public string file_id;

        [DataContract]
        public class Result : ResponseClass
        {
            /// <summary>
            /// This object represents a file ready to be downloaded. The file can be downloaded via the link https://api.telegram.org/file/bot<token>/<file_path>.
            /// It is guaranteed that the link will be valid for at least 1 hour. When the link expires, a new one can be requested by calling getFile.
            /// Maximum file size to download is 20 MB
            /// </summary>
            [DataMember]
            public FileClass result;
        }
    }
}
