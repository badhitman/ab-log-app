////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;

namespace ab.Model
{
    public class LogRowModel : abstractEF
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public LogStatusesEnum Status { get; set; }
        public string TAG { get; set; }
    }
}