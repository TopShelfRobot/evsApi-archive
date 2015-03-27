using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace evs.Model
{
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

    public enum EventureListType
    {
        Standard = 1,
        TeamSponsored,
        TeamSuggest,
        TeamIndividual,
        Lottery
    }

    public enum MailType
    {
        OrderConfirm,
        ResetPassword,
        TeamPlayerInvite,
        MassParticipant
    };

    public enum RegStatus
    {
        Completed = 0,
        Refunded,
        RefundedTest,
        TransferredTo,
        Transferred
    };

    public enum OrderStatus
    {
        Completed = 0,
        Incomplete,
        Refunded,
        PartiallyRefunded
        //Voided
    };

    public enum AmountType
    {
        Dollars = 0,
        Percent
    }

    public enum SurchargeType
    {
        Coupon = 1,
        Discount,
        OnlineFee,
        Addon,
        USAT
        //CouponEvent,
        //CouponList
        //DiscountEvent,
        //DiscountList,
        //FeeList,
        //FeeEvent,
    }

    public enum RefundType
    {
        Order = 0,
        Registration,
        Partial
    }

}
