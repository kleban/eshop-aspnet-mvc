using System.ComponentModel.DataAnnotations.Schema;

namespace eShop.Domain.Entities.ProductData
{
    public class ProductImage : BaseEntity<int>
    {
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string Path { get; set; } = string.Empty;
        public bool IsFront { get; set; }
    }
}
