using System.ComponentModel;

namespace eShopMVC.App.ViewModels.Account
{
    public class RegisterViewModel
    {
        [DisplayName("Ім'я")]
        public string FirstName { get; set; } = string.Empty;

        [DisplayName("Прізвище")]
        public string LastName { get; set; } = string.Empty;

        [DisplayName("Email")]
        public string Email { get; set; } = string.Empty;

        [DisplayName("Пароль")]
        public string Password { get; set; } = string.Empty;

        [DisplayName("Підтвердження пароля")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
