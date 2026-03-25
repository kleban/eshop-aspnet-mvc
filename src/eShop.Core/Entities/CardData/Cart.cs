using eShop.Domain.Entities.ProductData;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShop.Domain.Entities.CardData
{
    public class Cart
    {
        public int Id { get; set; }

        // Identity string GUID — навігаційна властивість до User відсутня навмисно
        // (User живе в Infrastructure, Core не залежить від нього)
        public string UserId { get; set; } = string.Empty;

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
