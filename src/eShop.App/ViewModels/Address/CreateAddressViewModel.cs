using System.ComponentModel;

namespace eShopMVC.App.ViewModels.Address
{
    public class CreateAddressViewModel
    {
        [DisplayName("Ім'я")]
        public string FirstName { get; set; } = string.Empty;

        [DisplayName("Прізвище")]
        public string LastName { get; set; } = string.Empty;

        [DisplayName("Телефон")]
        public string PhoneNumber { get; set; } = string.Empty;

        [DisplayName("Адреса (вулиця, будинок, квартира)")]
        public string AddressLine { get; set; } = string.Empty;

        [DisplayName("Місто")]
        public string City { get; set; } = string.Empty;

        [DisplayName("Область")]
        public string Region { get; set; } = string.Empty;

        [DisplayName("Поштовий індекс")]
        public string PostalCode { get; set; } = string.Empty;

        [DisplayName("Країна")]
        public string Country { get; set; } = "Україна";

        [DisplayName("Зробити основною")]
        public bool IsDefault { get; set; }
    }
}
