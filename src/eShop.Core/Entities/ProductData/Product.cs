using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace eShop.Domain.Entities.ProductData
{
    public class Product : BaseEntity<int>
    {
        public string? Name { get; set; }
        public Category? Category { get; set; }

        [ForeignKey(nameof(Category))]
        public int? CategoryId { get; set; }
        public string? Description { get; set; }
        public double Quantity { get; set; }
        public decimal Price { get; set; }
        [ForeignKey(nameof(UnitOfMeasure))]
        public int? BaseUnitId { get; set; } // для Bulk або Countable
        public UnitOfMeasure BaseUnit { get; set; }
        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}
