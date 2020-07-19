////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

namespace ab.Model
{
    public abstract class abstractEFContext : abstractEF
    {
        public bool AlarmSubscriber { get; set; }
        public bool CommandsAllowed { get; set; }
    }
}