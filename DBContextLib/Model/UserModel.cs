////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

namespace ab.Model
{
    public class UserModel : abstractEFContext
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string TelegramId { get; set; }
    }
}