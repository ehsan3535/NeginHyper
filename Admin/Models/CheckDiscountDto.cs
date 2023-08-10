namespace client.Models
{
    public class CheckDiscountDto
    {
        public string DiscountCode { get; set; }
        public Guid AddressId { get; set; }
        public Guid FreeTimeId { get; set; }
        public Guid shopcardId { get; set; }
    }
}
