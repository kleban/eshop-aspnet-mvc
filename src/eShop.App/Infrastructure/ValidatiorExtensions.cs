using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace eShopMVC.App.Infrastructure
{
    public static class ValidationExtensions
    {
        public static void AddToModelState(this ValidationResult validationResult, ModelStateDictionary modelState)
        {
            foreach (var error in validationResult.Errors)
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }
    }
}
