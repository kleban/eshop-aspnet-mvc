using System.ComponentModel.DataAnnotations;

namespace eShop.Domain.Entities
{
    public class Category : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
