using eShop.Domain.Entities.UserData;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Domain.Entities.OrderData
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
