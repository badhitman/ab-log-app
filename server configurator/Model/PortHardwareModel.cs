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

        public override string ToString()
        {
            string port_name = string.Empty;
            if (string.IsNullOrWhiteSpace(Name))
            {
                port_name = $"P{PortNumb}";
            }
            else
            {
                port_name = $"{Name} (P{PortNumb})";
            }
            return port_name;
        }
    }
}