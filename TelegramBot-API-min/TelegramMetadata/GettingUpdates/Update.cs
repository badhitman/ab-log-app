////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;
using TelegramBot.TelegramMetadata.AvailableTypes;

namespace TelegramBot.TelegramMetadata.GettingUpdates
{
    /// <summary>
    /// Этот объект представляет входящее обновление.
    /// At most one of the optional parameters can be present in any given update.
    /// </summary>
    [DataContract]
    public class Update : SerialiserJSON
    {
        /// <summary>
        /// The update‘s unique identifier. Update identifiers start from a certain positive number and increase sequentially.
        /// This ID becomes especially handy if you’re using Webhooks, since it allows you to ignore repeated updates or to restore the correct update sequence,
        /// should they get out of order. If there are no new updates for at least a week,
        /// then identifier of the next update will be chosen randomly instead of sequentially.
        /// </summary>
        [DataMember]
        public long update_id;

        /// <summary>
        /// Optional. New incoming message of any kind — text, photo, sticker, etc.
        /// </summary>
        [DataMember]
        public MessageClass message;

        /// <summary>
        /// Optional. New version of a message that is known to the bot and was edited
        /// </summary>
        [DataMember]
        public MessageClass edited_message;

        /// <summary>
        /// Optional. New incoming channel post of any kind — text, photo, sticker, etc.
        /// </summary>
        [DataMember]
        public MessageClass channel_post;

        /// <summary>
        /// Optional. New version of a channel post that is known to the bot and was edited
        /// </summary>
        [DataMember]
        public MessageClass edited_channel_post;

        /// <summary>
        /// Optional. New incoming callback query
        /// </summary>
        [DataMember]
        public CallbackQueryClass callback_query;
    }
}
