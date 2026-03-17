using eShopMVC.App.ViewModels.Category;
using FluentValidation;

namespace eShopMVC.App.Validators.Category
{
    public class CategoryViewModelValidator : AbstractValidator<CategoryViewModel>
    {
        public CategoryViewModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.DisplayOrder)
                .GreaterThan(0);
        }
    }
}
