using eShopMVC.App.ViewModels.Account;
using FluentValidation;

namespace eShopMVC.App.Validators.Account
{
    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterViewModelValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ім'я є обов'язковим")
                .MaximumLength(50).WithMessage("Ім'я не може бути більше 50 символів");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Прізвище є обов'язковим")
                .MaximumLength(50).WithMessage("Прізвище не може бути більше 50 символів");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email є обов'язковим")
                .EmailAddress().WithMessage("Введіть коректний email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль є обов'язковим")
                .MinimumLength(6).WithMessage("Пароль має містити мінімум 6 символів")
                .Matches("[A-Z]").WithMessage("Пароль має містити хоча б одну велику літеру")
                .Matches("[0-9]").WithMessage("Пароль має містити хоча б одну цифру");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Підтвердження пароля є обов'язковим")
                .Equal(x => x.Password).WithMessage("Паролі не співпадають");
        }
    }
}
