using eShop.Domain.Entities.ProductData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace eShop.Domain.Entities.CardData
{

    public class CartItem
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Cart))]
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public decimal Quantity { get; set; }          // для Bulk / Countable
        public int? UnitId { get; set; }              // для Bulk

        public int? ReservedProductItemCount { get; set; } // кількість серійних, яку планує купити
    }
}
