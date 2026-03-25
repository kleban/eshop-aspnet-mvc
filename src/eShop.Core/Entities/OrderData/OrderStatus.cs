namespace eShop.Domain.Entities.OrderData
{
    public enum OrderStatus
    {
        Pending,     // щойно створено, очікує підтвердження
        Confirmed,   // підтверджено менеджером (для Serialized — тут додаються серійні номери)
        Processing,  // комплектується/готується до відправлення
        Shipped,     // передано в доставку
        Delivered,   // доставлено клієнту
        Cancelled,   // скасовано
        Refunded     // повернення коштів
    }
}
