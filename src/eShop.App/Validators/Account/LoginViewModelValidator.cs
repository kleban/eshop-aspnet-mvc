using eShopMVC.App.ViewModels.Account;
using FluentValidation;

namespace eShopMVC.App.Validators.Account
{
    public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        public LoginViewModelValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email є обов'язковим")
                .EmailAddress().WithMessage("Введіть коректний email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль є обов'язковим");
        }
    }
}
