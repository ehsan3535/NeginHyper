using System.ComponentModel.DataAnnotations;

namespace Entities.Constants
{
    public enum PaymentStatus
    {
        [Display(Name = "در انتظار پرداخت")]
        Waiting,

        [Display(Name = "پرداخت شده")]
        Payed,

        [Display(Name = "در حال ارسال")]
        Sending,

        [Display(Name = "ارسال شده")]
        Sent,

        [Display(Name = "دریافت شده ")]
        Gived,

             [Display(Name = "نقدی ")]
        Cash
    }
}
