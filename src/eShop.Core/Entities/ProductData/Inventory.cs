using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace eShop.Domain.Entities.ProductData
{
    public class Inventory
    {

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public decimal Quantity { get; set; }  // для Bulk: кг, л, м; для Countable: шт
    }
}
