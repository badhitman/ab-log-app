﻿////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

namespace ab.Model
{
    public class CloudEmailMessageModel : abstractEF
    {
        public string UIDL { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Extra { get; set; }

        public override string ToString()
        {
            string email_name = Name;
            return base.ToString();
        }
    }
}