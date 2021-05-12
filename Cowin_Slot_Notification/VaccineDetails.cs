using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cowin_Slot_Notification
{
    [DataContract]
    class VaccineDetails
    {
        [DataMember]
        public List<Center> centers { get; set; }
    }

    [DataContract]
    public class Session
    {
        [DataMember]
        public string session_id { get; set; }
        [DataMember]
        public string date { get; set; }
        [DataMember]
        public int available_capacity { get; set; }
        [DataMember]
        public int min_age_limit { get; set; }
        [DataMember]
        public string vaccine { get; set; }
        [DataMember]
        public List<string> slots { get; set; }
    }

    [DataContract]
    public class Center
    {
        [DataMember]
        public int center_id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string address { get; set; }
        [DataMember]
        public string state_name { get; set; }
        [DataMember]
        public string district_name { get; set; }
        [DataMember]
        public string block_name { get; set; }
        [DataMember]
        public int pincode { get; set; }
        [DataMember]
        public int lat { get; set; }
        [DataMember]
        public int @long { get; set; }
        [DataMember]
        public string from { get; set; }
        [DataMember]
        public string to { get; set; }
        [DataMember]
        public string fee_type { get; set; }
        [DataMember]
        public List<Session> sessions { get; set; }
    }
}
