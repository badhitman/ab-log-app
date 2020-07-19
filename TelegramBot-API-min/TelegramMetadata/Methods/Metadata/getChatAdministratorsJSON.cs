////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;
using TelegramBot.TelegramMetadata.AvailableTypes;
// 
namespace TelegramBot.TelegramMetadata.MethodsMetadata.Metadata
{
    /// <summary>
    /// Use this method to get a list of administrators in a chat. On success, returns an Array of ChatMember objects
    /// that contains information about all chat administrators except other bots. If the chat is a group or a supergroup
    /// and no administrators were appointed, only the creator will be returned.
    /// </summary>
    [DataContract]
    public class getChatAdministratorsJSON : _AbstractMethodsManager
    {
        /// <summary>
        /// Integer or String	Yes	Unique identifier for the target chat or username of the target supergroup or channel (in the format @channelusername)
        /// </summary>
        [DataMember]
        public string chat_id;

        [DataContract]
        public class Result : ResponseClass
        {
            /// <summary>
            /// This object contains information about one member of a chat.
            /// </summary>
            [DataMember]
            public ChatMemberClass[] result;
        }
    }
}
