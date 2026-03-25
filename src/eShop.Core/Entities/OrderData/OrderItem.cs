using eShop.Domain.Entities.ProductData;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Domain.Entities.OrderData
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        // Для Countable / Bulk
        public decimal? Quantity { get; set; }
        public int? UnitId { get; set; }

        // Для Serialized
        public List<ProductItem> ProductItems { get; set; } = new();

        public decimal Price { get; set; }
    }
}
