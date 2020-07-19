////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;
using TelegramBot.TelegramMetadata.AvailableTypes;

namespace TelegramBot.TelegramMetadata.MethodsMetadata.Metadata
{
    /// <summary>
    /// Простой метод для тестирования токен аутентификации вашего бота. Не требует никаких параметров. Возвращает основную информацию о бота в виде объекта пользователя.
    /// </summary>
    [DataContract]
    public class getMeJSON : _AbstractMethodsManager
    {
        [DataContract]
        public class Result : ResponseClass
        {
            /// <summary>
            /// Основная информация о боте в виде объекта пользователя
            /// </summary>
            [DataMember]
            public UserClass result;
        }
    }
}
