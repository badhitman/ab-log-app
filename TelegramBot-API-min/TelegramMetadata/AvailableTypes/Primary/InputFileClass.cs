////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using MultiTool;
using System.IO;
using System.Runtime.Serialization;

namespace TelegramBot.TelegramMetadata.AvailableTypes
{
    /// <summary>
    /// This object represents the contents of a file to be uploaded. Must be posted using multipart/form-data in the usual way that files are uploaded via the browser.
    /// </summary>
    [DataContract]
    public class InputFileClass
    {
        /// <summary>
        /// Имя поля отправляемого файла
        /// </summary>
        [DataMember]
        public string FieldName { get; set; }

        /// <summary>
        /// Имя файла отправляемого файла
        /// </summary>
        [DataMember]
        public string FileName { get; set; }

        /// <summary>
        /// Mime-Type отправляемого файла
        /// </summary>
        [DataMember]
        public string MimeType => MimeTypeMap.GetMimeType(Path.GetExtension(FileName));

        /// <summary>
        /// Отправляемые данные
        /// </summary>
        [DataMember]
        public byte[] Data { get; set; }
    }
}
