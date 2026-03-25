using eShopMVC.App.ViewModels.Address;
using FluentValidation;

namespace eShopMVC.App.Validators.Address
{
    public class CreateAddressViewModelValidator : AbstractValidator<CreateAddressViewModel>
    {
        public CreateAddressViewModelValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ім'я є обов'язковим")
                .MaximumLength(50).WithMessage("Максимум 50 символів");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Прізвище є обов'язковим")
                .MaximumLength(50).WithMessage("Максимум 50 символів");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Телефон є обов'язковим")
                .Matches(@"^\+?[\d\s\-\(\)]{7,20}$").WithMessage("Невірний формат телефону");

            RuleFor(x => x.AddressLine)
                .NotEmpty().WithMessage("Адреса є обов'язковою")
                .MaximumLength(200).WithMessage("Максимум 200 символів");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Місто є обов'язковим")
                .MaximumLength(100).WithMessage("Максимум 100 символів");

            RuleFor(x => x.Region)
                .MaximumLength(100).WithMessage("Максимум 100 символів");

            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("Поштовий індекс є обов'язковим")
                .Matches(@"^\d{5}$").WithMessage("Індекс має містити 5 цифр");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Країна є обов'язковою")
                .MaximumLength(100).WithMessage("Максимум 100 символів");
        }
    }
}
