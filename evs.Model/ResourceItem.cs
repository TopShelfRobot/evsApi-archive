using System;
using System.ComponentModel.DataAnnotations;

namespace evs.Model
{
    public class ResourceItem
    {
        public Int32 Id { get; set; }
        [Required]
        public string Name { get; set; }
        public Int32 ResourceId { get; set; }
        public decimal Cost { get; set; }
        public Int32 ResourceItemCategoryId { get; set; }
        public Boolean Active { get; set; }
        public Int32 OwnerId { get; set; }
        
        public virtual ResourceItemCategory ResourceItemCategory { get; set; }
        public virtual Resource Resource { get; set; }
    }
}
