////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;

namespace TelegramBot.TelegramMetadata
{
    /// <summary>
    /// Класс сериализованного ответа сервера
    /// </summary>
    [DataContract]
    public class ResponseClass : SerialiserJSON
    {
        /// <summary>
        /// Статус ответа. Если true, значит в поле result ответ. Если false, то смотрите поле description описание ошибки
        /// </summary>
        [DataMember]
        public bool ok;

        /// <summary>
        /// Необязательный. Описание ошибки (в случае возникновления таковой)
        /// </summary>
        [DataMember]
        public string description;

        /// <summary>
        /// Необязательный. Код ошибки (в случае возникновления таковой) .
        /// </summary>
        [DataMember]
        public int error_code;
    }
}
