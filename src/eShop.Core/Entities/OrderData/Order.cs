using eShop.Domain.Entities.UserData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eShop.Domain.Entities.OrderData
{
    public class Order
    {
        public int Id { get; set; }

        // Identity string GUID — навігаційна властивість до User відсутня навмисно
        // (User живе в Infrastructure, Core не залежить від нього)
        public string UserId { get; set; } = string.Empty;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public decimal TotalAmount { get; set; }

        // Адреса доставки — зберігаємо FK на Address.
        // ВАЖЛИВО: при зміні замовлення ця адреса може бути відредагована користувачем.
        // Якщо потрібна незмінна копія — розгляньте зберігання snapshot-полів прямо в Order.
        [ForeignKey(nameof(ShippingAddress))]
        public int? ShippingAddressId { get; set; }
        public Address? ShippingAddress { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
