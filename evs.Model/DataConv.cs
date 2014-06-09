using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace evs.Model
{
    public class Dc_Reg
    {
        public Int32 Id { get; set; }
        public string Event { get; set; }
        public string AgeGroup { get; set; }
        public string RegistrationDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string BirthDate { get; set; }
        public string MembershipNumber { get; set; }
        public string Experience { get; set; }
        public string SpecialNotes { get; set; }
        public string ShirtSize { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string HomePhone { get; set; }
        public string ParentName { get; set; }
        public string Email { get; set; }
        public string AgreeToTerms { get; set; }
        public string Cell { get; set; }
        public string Carrier { get; set; }
        public string SecondaryEmail { get; set; }
        public string SecondaryCell { get; set; }
        public string SecondaryCarrier { get; set; }
        public string EmergContact { get; set; }
        public string EmergPhone { get; set; }
        public string DrName { get; set; }
        public string DrPhone { get; set; }
        public string MedicalCondition { get; set; }
        public string MedInsCarrier { get; set; }
        public string MedInsPolicyNumber { get; set; }
        public int Status { get; set; }
        public bool Converted { get; set; }
        public string ConvCode { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }

    public class Dc_Part
    {
        public Int32 Id { get; set; }
        public Int32 OldId { get; set; }
        public string OwnerId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string BirthDate { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Actions { get; set; }
        public int Status { get; set; }
        public bool Converted { get; set; }
        public string ConvCode { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        //public Int32 ConvPartId { get; set; }
        //public Int32 ConvOwnerId { get; set; }
        //public Int32 
    }

    public class Dc_StagePart
    {
        public Int32 Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateBirth { get; set; }
        public string Email { get; set; }
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
        public string OwnerName { get; set; }
        //public Int32 OwnerId { get; set; }
        public Int32 HouseId { get; set; }
        public int Status { get; set; }
        public bool Converted { get; set; }
        public string ConvCode { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }

    public class Dc_Order
    {
        public Int32 Id { get; set; }
        public string OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string ProductName { get; set; }
        public string UnitCost { get; set; }
        public string Participant { get; set; }
        public string Options { get; set; }
        public string CardType { get; set; }
        public string OrderTotal { get; set; }
        public string TransactionId { get; set; }
        public int Status { get; set; }
        public bool Converted { get; set; }
        public string ConvCode { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PartFirstName { get; set; }
        public string PartLastName { get; set; }
    }

    public class Dc_StateLookup
    {
        public int Id { get; set; }
        public string StateName { get; set; }
        public string StateAbbrev { get; set; }
    }
}
