using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace eShop.Domain.Entities.ProductData
{
    public class ProductItem
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string SerialNumber { get; set; }
        public ProductItemStatus Status { get; set; }
    }
}
