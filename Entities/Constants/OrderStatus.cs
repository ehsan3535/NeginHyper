using System.ComponentModel.DataAnnotations;

namespace Entities.Constants
{
    public enum OrderStatus
    {
        [Display(Name = "در انتظار")]
        Waiting,

        [Display(Name = "پراخت شده")]
        Payed,

        [Display(Name = "اتمام مهلت پرداخت")]
        Passed
    }
}
