using eShop.Domain.Entities.ProductData;
using System.ComponentModel.DataAnnotations;

namespace eShop.Domain.Entities
{
    public class Category : NamedBaseEntity<int>
    {
        public int DisplayOrder { get; set; }
       // public virtual ICollection<Product> Products { get; get; }
    }
}
