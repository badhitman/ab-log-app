////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;

namespace ab.Model
{
    public class ReportModel : abstractEF
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public TaskModel OwnerTask { get; set; }
        public int OwnerTaskId { get; set; }

        public int CommandId { get; set; }

        public bool Success { get; set; }
    }
}