using System.ComponentModel.DataAnnotations.Schema;

namespace eShop.Domain.Entities.ProductData
{
    public class Inventory : BaseEntity<int>
    {
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        // Для Bulk: кг, л, м...  Для Countable: кількість штук
        // Для Serialized не використовується (рахується через ProductItem.Status == InStock)
        public decimal Quantity { get; set; }
    }
}
