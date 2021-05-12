using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cowin_Slot_Notification
{
    [DataContract]
    class ApplicationConfiguration
    {
        [DataMember (Name = "configurations")]
        public List<Configuration> Configurations { get; set; }
    }
    [DataContract]
    public class Configuration
    {
        [DataMember(Name = "pincode")]
        public string Pincode { get; set; }
        [DataMember(Name = "min_age_limit")]
        public int Min_age_limit { get; set; }
        [DataMember(Name = "to_email")]
        public string To_email { get; set; }
        [DataMember(Name = "name")]
        public List<string> Name { get; set; }
    }
}
