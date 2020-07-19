////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

namespace ab.Model
{
    class TelegramMessageModel : abstractEF
    {
        public string TelegramMessageId { get; set; }
        public string FromUserId { get; set; }
    }
}