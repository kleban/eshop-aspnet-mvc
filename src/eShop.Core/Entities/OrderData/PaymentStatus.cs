namespace eShop.Domain.Entities.OrderData
{
    public enum PaymentStatus
    {
        Unpaid,    // не оплачено
        Paid,      // оплачено
        Refunded   // кошти повернено
    }
}
