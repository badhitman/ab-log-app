////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Runtime.Serialization;

namespace TelegramBot.TelegramMetadata.AvailableTypes
{
    /// <summary>
    /// This object represents the content of a media message to be sent. It should be one of
    ///  ~ InputMediaPhoto
    ///  ~ InputMediaVideo
    /// </summary>
    [DataContract]
    public abstract class InputMediaClass { };
}
