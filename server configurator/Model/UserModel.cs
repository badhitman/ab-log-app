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

        public override string ToString()
        {
            string ret_name = Name;
            if (!string.IsNullOrWhiteSpace(Email))
            {
                ret_name = $"{ret_name} [e:{Email}]";
            }
            if (!string.IsNullOrWhiteSpace(Phone))
            {
                ret_name = $"{ret_name} [t:{Phone}]";
            }

#if DEBUG
            ret_name = $"#{Id}; {ret_name}";
#endif
            return ret_name;
        }
    }
}