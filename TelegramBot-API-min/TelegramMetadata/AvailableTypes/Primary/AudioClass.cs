////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;

namespace TelegramBot.TelegramMetadata.AvailableTypes
{
    /// <summary>
    /// This object represents an audio file to be treated as music by the Telegram clients.
    /// </summary>
    [DataContract]
    public class AudioClass
    {
        /// <summary>
        /// Unique identifier for this file
        /// </summary>
        [DataMember]
        public string file_id;

        /// <summary>
        /// Duration of the audio in seconds as defined by sender
        /// </summary>
        [DataMember]
        public int duration;

        /// <summary>
        /// Optional.Performer of the audio as defined by sender or by audio tags
        /// </summary>
        [DataMember]
        public string performer;

        /// <summary>
        /// Optional. Title of the audio as defined by sender or by audio tags
        /// </summary>
        [DataMember]
        public string title;

        /// <summary>
        /// Optional. MIME type of the file as defined by sender
        /// </summary>
        [DataMember]
        public string mime_type;

        /// <summary>
        /// Integer Optional. File size
        /// </summary>
        [DataMember]
        public int file_size;
    }
}
