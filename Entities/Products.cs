using Entities.ProductComment;
using System;
using System.Collections.Generic;

namespace Entities.Product
{
    public class Products : BaseEntity
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Pureweight { get; set; }
        public string Allweight { get; set; }
        public string GoodForWhat { get; set; }
        public string KeepWay { get; set; }
        public string KeepTime { get; set; }
        public string Matterial { get; set; }
        public string ImageCoverUrl { get; set; }
        public int Price { get; set; }
        public int Discount { get; set; }
        public int Percent { get; set; }
        public int Count { get; set; }
        public int Rate { get; set; }
        public int TotalRate { get; set; }
        public int darage { get; set; }

        public string Detail { get; set; }
        public ProductCategory Categorys { get; set; }
        public Guid CategorysId { get; set; }
        public ICollection<ProductComments> Comments { get; set; }
        public ICollection<ProductCategory> Categories { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }




        //public Guid SameProduct1Id { get; set; }
        //public Product SameProduct1 { get; set; }
        //public Guid SameProduct2Id { get; set; }
        //public Product SameProduct2 { get; set; }
        //public Guid SameProduct3Id { get; set; }
        //public Product SameProduct3 { get; set; }
    }
}
