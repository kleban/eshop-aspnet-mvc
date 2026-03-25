namespace eShop.Domain.Entities.UserData
{
    /// <summary>
    /// Збережена адреса доставки користувача.
    /// UserId — string (Identity GUID), навігаційна властивість до User
    /// відсутня навмисно: User живе в Infrastructure, Core не залежить від нього.
    /// </summary>
    public class Address : BaseEntity<int>
    {
        // Власник адреси (Identity string GUID)
        public string UserId { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string AddressLine { get; set; } = string.Empty;  // вул. Хрещатик, 1, кв. 5
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = "Україна";

        // Чи є ця адреса адресою за замовчуванням для користувача
        public bool IsDefault { get; set; }
    }
}
