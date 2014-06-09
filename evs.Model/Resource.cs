using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace evs.Model
{
    public class Resource
    {
        public Int32 Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Email { get; set; }
        public string Street1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string Phone { get; set; }
        public string Terms { get; set; }
        public string Fax { get; set; }
        public Int32 OwnerId { get; set; }
        //public Boolean IsOwnerOwned { get; set; }
        //public String Category { get; set; }
        public string Website { get; set; }
        public string ResourceType { get; set; }
    }
}
