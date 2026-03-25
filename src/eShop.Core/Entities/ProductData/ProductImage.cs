using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Domain.Entities.ProductData
{
    public class ProductImage : BaseEntity<int>
    {
        public string Path { get; set; } 
        public Product Product { get; set; }
        public bool IsFront { get; set; }

    }
}
