////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace ab.Model
{
    public class HardwareModel : abstractEFContext
    {
        public string Address { get; set; }
        public string Password { get; set; }

        public List<PortHardwareModel> Ports { get; set; }

        public override string ToString()
        {
            string hw_name = $"{Name} ({Address})";
#if DEBUG
            hw_name = $"#{Id}; {hw_name}";
#endif
            return hw_name;
        }
    }
}