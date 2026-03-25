using eShop.Domain.Entities.ProductData;
using eShop.Domain.Entities.UserData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace eShop.Domain.Entities.CardData
{
    public class Cart
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User? User { get; set; }
        public ICollection<CartItem> Items { get; set; } = new HashSet<CartItem>();
    }
}
