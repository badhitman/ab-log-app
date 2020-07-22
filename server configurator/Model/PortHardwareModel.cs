////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

namespace ab.Model
{
    public class PortHardwareModel : abstractEF
    {
        public int HardwareId { get; set; }
        public HardwareModel Hardware { get; set; }

        public int PortNumb { get; set; }
    }
}