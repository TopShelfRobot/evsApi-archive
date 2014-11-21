using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace evs.Model
{
    public class EventureOrder
    {
        public Int32 Id { get; set; }
        public Int32 HouseId { get; set; }
        public Int32 OwnerId { get; set; }

        public decimal Amount { get; set; }
        public Int32 CardProcessorFeeInCents { get; set; }
        public Int32 LocalFeeInCents { get; set; }
        public Int32 LocalApplicationFee { get; set; }
        
        public string AuthorizationCode { get; set; }
        public string Token { get; set; }
        public string CardId { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string CardFingerprint { get; set; }
        public string CardExpires { get; set; }
        public string CardType { get; set; }
        public string CardOrigin { get; set; }
        public string CardCvcCheck { get; set; }

        public string Status { get; set; }
        public Boolean Voided { get; set; }
        public Int32 ConvOrderId { get; set; }
        public PaymentType PaymentType { get; set; }
        public Int32 PaymentTypeId { get; set; }  //cash, credit, check
        public OrderType OrderTypeId { get; set; }    //manual online reg

        public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }
        
    

    //[ForeignKey("HouseId")]
        //public virtual Participant House { get; set; }

        public ICollection<Registration> Registrations { get; set; }
        public ICollection<Surcharge> Surcharges { get; set; } 
    }

    public enum PaymentType
    {
        credit = 0,
        cash = 1,
        giftCertificate = 2,
        check = 3
    }
    public enum OrderType
    {
        online = 0,
        manual
    }

}
