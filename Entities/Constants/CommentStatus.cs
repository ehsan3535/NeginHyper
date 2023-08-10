using System.ComponentModel.DataAnnotations;

namespace Entities.Constants
{

    public enum CommentStatus
    {
        [Display(Name = "خوانده نشده")]
        NotSeen,

        [Display(Name = "تایید شده")]
        Accepted,

        [Display(Name = "تایید نشده")]
        InAccepted
    }

}
