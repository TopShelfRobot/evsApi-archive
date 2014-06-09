using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace evs.Model
{
    
    public class EmergencyContact
    {
        public Int32 Id { get; set; }
        public string Name { get; set; }
        public string DoctorName { get; set; }
        public string Condition { get; set; }
        public string InsurancePolicyNo { get; set; }
        public string InsuranceCarrier { get; set; }
    
        //public virtual Participant Person { get; set; }
    }
}
