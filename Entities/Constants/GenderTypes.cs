using System.ComponentModel.DataAnnotations;

namespace Entities.Constants
{
    public enum GenderType
    {
        [Display(Name = "آقا")]
        Male,

        [Display(Name = "خانم")]
        Female
    }
    public enum SchoolGenderType
    {
        [Display(Name = "پسرانه")]
        Male,

        [Display(Name = "دخترانه")]
        Female,

        [Display(Name = "مختلط")]
        Both
    }
    public enum Mojavez
    {
        [Display(Name = "دارد")]
        Male,

        [Display(Name = "ندارد")]
        Female,

        [Display(Name = "موقت")]
        Both
    }
}
