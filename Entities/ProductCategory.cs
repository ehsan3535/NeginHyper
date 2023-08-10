namespace Entities
{
    public class ProductCategory : BaseEntity
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string MenuImageUrl { get; set; }

    }
}
