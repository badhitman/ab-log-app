////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

namespace ab.Model
{
    public class CloudEmailMessage : abstractEF
    {
        public string UIDL { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Extra { get; set; }
    }
}