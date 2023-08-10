using Entities.Product;
using System;

namespace Entities
{
    public class ProductImage : BaseEntity
    {
        public Products Product { get; set; }
        public Guid ProductId { get; set; }
        public string ImageLink { get; set; }
    }
}
