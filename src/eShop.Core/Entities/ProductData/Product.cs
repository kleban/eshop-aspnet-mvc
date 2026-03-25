using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShop.Domain.Entities.ProductData
{
    public class Product : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public ProductType Type { get; set; }

        [ForeignKey(nameof(Category))]
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        // Одиниця виміру — для Bulk (кг, л, м) та Countable (шт)
        // Для Serialized не використовується
        [ForeignKey(nameof(BaseUnit))]
        public int? BaseUnitId { get; set; }
        public UnitOfMeasure? BaseUnit { get; set; }

        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

        // Serialized: окремі екземпляри з серійними номерами
        public virtual ICollection<ProductItem> Items { get; set; } = new List<ProductItem>();
    }
}
