////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;

namespace TelegramBot.TelegramMetadata.AvailableTypes
{
    /// <summary>
    /// This object represents a phone contact.
    /// </summary>
    [DataContract]
    public class ContactClass
    {
        /// <summary>
        ///  Contact's phone number
        /// </summary>
        [DataMember]
        public string phone_number;

        /// <summary>
        ///  Contact's first name
        /// </summary>
        [DataMember]
        public string first_name;

        /// <summary>
        /// Optional.Contact's last name
        /// </summary>
        [DataMember]
        public string last_name;

        /// <summary>
        /// Optional.Contact's user identifier in Telegram
        /// </summary>
        [DataMember]
        public int user_id;
    }
}
