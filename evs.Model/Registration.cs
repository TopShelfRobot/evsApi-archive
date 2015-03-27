﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace evs.Model
{
    public class Registration
    {

        public Registration() 
        {
            this.Answers = new List<Answer>();
            //this.EventListCharge = new List<EventListCharge>();
        }
        public Int32 Id { get; set; }
        //[MaxLength(75)]
        //public string Name { get; set; }           //this is redundant  //mjb 052814
        public Int32 ParticipantId { get; set; }
        public Int32 EventureListId { get; set; }
        public Int32 EventureOrderId { get; set; }
        public decimal TotalAmount { get; set; }  //this is amount with fees, coupons, etc
        public decimal ListAmount { get; set; }   //this is the amount from eventurelist at time of purchase
        //public Int32 Score { get; set; }
        public Int32? CouponId { get; set; }
        public Int32? GroupId { get; set; }    //mjb fix this in 3.0
        public Int32? Group2Id { get; set; }
        public Int32 Quantity { get; set; }
        public Boolean Redeemed { get; set; }
        public Int32? ConvRegId { get; set; }
        public Int32? StockAnswerSetId { get; set; }

        public ICollection<RegistrationPost> Posts { get; set; }
       

        public DateTime DateCreated { get; set; }
        //public DateTime DateModified { get; set; }
        //public Int32 ModifiedById { get; set; }
        //public Int32 CreatedById { get; set; }

        public RegStatus RegStatus { get; set; }


        [ForeignKey("GroupId")]
        public virtual EventureGroup EventureGroup { get; set; }

        


        public string Type { get; set; }

        public virtual StockAnswerSet StockAnswerSet { get; set; }
        public virtual Participant Participant { get; set; }
        public virtual EventureList EventureList { get; set; }
        //public ICollection<Surcharge> Surcharges { get; set; }
        public EventureOrder EventureOrder { get; set; }
        public ICollection<Answer> Answers { get; set; }
        //public ICollection<EventListCharge> EventListCharges { get; set; }
        
        
        //public virtual EventureGroup EventureGroup { get; set; }

        //public ICollection<Participant> Participants { get; set; }
        //public ICollection<QResult> Result { get; set; }
        ////public Invoice Invoice { get; set; }
        ////public ICollection<Coupon> Coupons { get; set; }
        ////public RegistrationStatus RegistrationStatus { get; set; }
    }
}
