////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;

namespace TelegramBot.TelegramMetadata.GettingUpdates
{
    /// <summary>
    /// Use this method to receive incoming updates using long polling (wiki).
    /// An Array of Update objects is returned.
    /// </summary>
    [DataContract]
    class getUpdatesJSON : _AbstractMethodsManager
    {
        /// <summary>
        /// Optional 	Identifier of the first update to be returned.
        /// Must be greater by one than the highest among the identifiers of previously received updates.
        /// By default, updates starting with the earliest unconfirmed update are returned.
        /// An update is considered confirmed as soon as getUpdates is called with an offset higher than its update_id.
        /// The negative offset can be specified to retrieve updates starting from -offset update from the end of the updates queue. All previous updates will forgotten.
        /// </summary>
        [DataMember]
        public long offset;

        /// <summary>
        /// Optional 	Limits the number of updates to be retrieved. Values between 1—100 are accepted. Defaults to 100.
        /// </summary>
        [DataMember]
        public int limit;

        /// <summary>
        /// Optional 	Timeout in seconds for long polling. Defaults to 0, i.e. usual short polling. Should be positive, short polling should be used for testing purposes only.
        /// </summary>
        [DataMember]
        public long timeout;

        /// <summary>
        /// Optional 	List the types of updates you want your bot to receive.
        /// For example, specify [“message”, “edited_channel_post”, “callback_query”] to only receive updates of these types.
        /// See Update for a complete list of available update types. Specify an empty list to receive all updates regardless of type (default).
        /// If not specified, the previous setting will be used.
        /// 
        /// Please note that this parameter doesn't affect updates created before the call to the getUpdates, so unwanted updates may be received for a short period of time.
        /// 
        ///     Notes
        ///     1. This method will not work if an outgoing webhook is set up.
        ///     2. Чтобы избежать повторяющихся обновлений, пересчитывайте смещение после каждого ответа сервера.
        /// </summary>
        [DataMember]
        public string[] allowed_updates;

        [DataContract]
        public class Result : ResponseClass
        {
            [DataMember]
            public Update[] result;
        }
    }
}
