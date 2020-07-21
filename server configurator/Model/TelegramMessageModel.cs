////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

namespace ab.Model
{
    public class TelegramMessageModel : abstractEF
    {
        public string MessageId { get; set; }
        public int UserSenderId { get; set; }
        public long ChatId { get; set; }
        public long UpdateId { get; set; }
        public long TelegramBotId { get; set; }
    }
}