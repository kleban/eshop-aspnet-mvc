using eShop.Domain.Entities.ProductData;
using System.ComponentModel.DataAnnotations;

namespace eShop.Domain.Entities.CategoryData
{
    public class Category : NamedBaseEntity<int>
    {
        public int DisplayOrder { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
