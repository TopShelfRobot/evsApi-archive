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
        public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }
    }

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
        public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }

        public virtual ResourceItemCategory ResourceItemCategory { get; set; }
        public virtual Resource Resource { get; set; }
    }

    public class ResourceItemCategory
    {
        public Int32 Id { get; set; }
        public string Name { get; set; }
        public Int32 OwnerId { get; set; }
        public Boolean Active { get; set; }
        public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }
    }

    public class EventureExpense
    {
        public Int32 Id { get; set; }
        public Int32 ResourceItemId { get; set; }
        public Int32 ResourceItemCategoryId { get; set; }
        public decimal Cost { get; set; }
        public string CostType { get; set; }
        public Int32 PerRegNumber { get; set; }
        public Int32 EventureId { get; set; }
        public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }
        public virtual ResourceItem ResourceItem { get; set; }
        public virtual ResourceItemCategory ResourceItemCategory { get; set; }
    }

    public class EventurePlanItem
    {
        public Int32 Id { get; set; }
        public string Task { get; set; }
        //public string Resource { get; set; }
        public Int32 ResourceId { get; set; }
        public DateTime DateDue { get; set; }
        public Int32 EventureId { get; set; }
        public Boolean IsCompleted { get; set; }
        public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }

        //[ForeignKey("ResourceId")]
        public virtual Resource Resource { get; set; }

    }
    //are we using this??
    public class EventureService
    {
        public Int32 Id { get; set; }
        public Int32 VendorServiceId { get; set; }
        public string VendorServiceText { get; set; }
        public decimal Amount { get; set; }
        public Boolean IsVariable { get; set; }
        public bool Active { get; set; }
        public Int32 EventureId { get; set; }
        public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }
    }
}
