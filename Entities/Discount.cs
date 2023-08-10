namespace Entities
{
    public class Discount : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int DiscountPercent { get; set; }
    }
}
