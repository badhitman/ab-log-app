////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;

namespace TelegramBot.TelegramMetadata.AvailableTypes
{
    /// <summary>
    /// This object represents an incoming callback query from a callback button in an inline keyboard (https://core.telegram.org/bots#inline-keyboards-and-on-the-fly-updating).
    /// If the button that originated the query was attached to a message sent by the bot, the field message will be present. If the button was attached to a message sent via the bot (in inline mode https://core.telegram.org/bots/api#inline-mode),
    /// the field inline_message_id will be present. Exactly one of the fields data or game_short_name will be present.
    /// NOTE: After the user presses a callback button, Telegram clients will display a progress bar until you call answerCallbackQuery (https://core.telegram.org/bots/api#answercallbackquery).
    /// It is, therefore, necessary to react by calling answerCallbackQuery (https://core.telegram.org/bots/api#answercallbackquery) even if no notification to the user is needed (e.g., without specifying any of the optional parameters).
    /// </summary>
    [DataContract]
    public class CallbackQueryClass
    {
        /// <summary>
        /// Unique identifier for this query
        /// </summary>
        [DataMember]
        public string id;

        /// <summary>
        /// Sender
        /// </summary>
        [DataMember]
        public UserClass from;

        /// <summary>
        /// Optional.Message with the callback button that originated the query.Note that message content and message date will not be available if the message is too old
        /// </summary>
        [DataMember]
        public MessageClass message;

        /// <summary>
        /// Optional.Identifier of the message sent via the bot in inline mode, that originated the query.
        /// </summary>
        [DataMember]
        public string inline_message_id;

        /// <summary>
        /// Global identifier, uniquely corresponding to the chat to which the message with the callback button was sent.Useful for high scores in games (https://core.telegram.org/bots/api#games).
        /// </summary>
        [DataMember]
        public string chat_instance;

        /// <summary>
        /// Optional.Data associated with the callback button. Be aware that a bad client can send arbitrary data in this field.
        /// </summary>
        [DataMember]
        public string data;

        /// <summary>
        /// Optional.Short name of a Game (https://core.telegram.org/bots/api#games) to be returned, serves as the unique identifier for the game
        /// </summary>
        [DataMember]
        public string game_short_name;
    }
}
