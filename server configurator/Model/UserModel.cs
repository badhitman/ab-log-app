////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace ab.Model
{
    public class UserModel : abstractEFContext
    {
        public string Email { get; set; }
        public string Phone { get; set; }

        //public List<TelegramUserModel> TelegramUsers { get; set; }
    }
}