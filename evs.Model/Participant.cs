using System;
using System.ComponentModel.DataAnnotations;

namespace evs.Model
{
    public class Participant
    {
        public Int32 Id { get; set; }
        [Required]
        public Int32 OwnerId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateBirth { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyPhone { get; set; }
        public string PhoneMobile { get; set; }
        public string Phone { get; set; }
        
        public Int32 HouseId { get; set; }
        public string MembershipNumber { get; set; }
        public string MobileCarrier { get; set; }
        public Int32? ConvPartId { get; set; }
        public Int32? ConvHouseId { get; set; }
        public string ConvStatus { get; set; }

        public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }
  

        //public ICollection<Registration> Registrations { get; set; }
        //public Registration Registration { get; set; }
        // public EventureList Listing { get; set; }
        //public ICollection<Address> Addresses { get; set; }
        //public EmergencyContact EmergencyContact { get; set; }
        //public ICollection<Contact> Contacts { get; set; }
    }
}
