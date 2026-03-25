using System.ComponentModel;

namespace eShopMVC.App.ViewModels.Account
{
    public class LoginViewModel
    {
        [DisplayName("Email")]
        public string Email { get; set; } = string.Empty;

        [DisplayName("Пароль")]
        public string Password { get; set; } = string.Empty;

        [DisplayName("Запам'ятати мене")]
        public bool RememberMe { get; set; }
    }
}
