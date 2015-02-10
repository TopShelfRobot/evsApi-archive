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

        public string Position { get; set; }
        public string ShirtSize { get; set; }
 
    }

     
    public class UserAgent
    {
        public Int32 Id { get; set; }
        public Int32 ParticipantId { get; set; }

        [MaxLength(50)]
        public string BrowserCodeName { get; set; }
        
        [MaxLength(50)]
        public string BrowserName { get; set; }
        
        [MaxLength(150)]
        public string BrowserVersion { get; set; }
        public Boolean CookiesEnabled { get; set; }
        
        [MaxLength(25)]
        public string Platform { get; set; }
        
        [MaxLength(250)]
        public string Header { get; set; }
        
        [MaxLength(50)]
        public string SystemLanguage { get; set; }
        
        [MaxLength(50)]
        public string Ip { get; set; }
       
        [MaxLength(50)]
        public string Hostname { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        
        [MaxLength(50)]
        public string City { get; set; }
        
        [MaxLength(50)]
        public string Region { get; set; }
        
        [MaxLength(50)]
        public string Country { get; set; }
        
        [MaxLength(50)]
        public string Phone { get; set; }
        
        public Int32 OwnerId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
