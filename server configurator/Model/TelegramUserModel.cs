////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

namespace ab.Model
{
    public class TelegramUserModel : abstractEF
    {
        public long TelegramParentBotId { get; set; }
        public long TelegramId { get; set; }
        public string UserName { get; set; }

        public int LinkedUserId { get; set; }
        //public UserModel LinkedUser { get; set; }

        public override string ToString()
        {
            string full_name = string.IsNullOrWhiteSpace(Name) ? "" : $"Name: {Name}; ";
            string user_name = string.IsNullOrWhiteSpace(UserName) ? "" : $"User: @{UserName}; ";
            string ret_name = $"{full_name}{user_name} tid: {TelegramId};";
            return ret_name;
        }
    }
}