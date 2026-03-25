using eShop.Domain.Entities.ProductData;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace eShopMVC.App.ViewModels.Product
{
    public class EditProductViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Назва")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Опис")]
        public string? Description { get; set; }

        [Display(Name = "Ціна (грн)")]
        public decimal Price { get; set; }

        [Display(Name = "Тип товару")]
        public ProductType Type { get; set; }

        [Display(Name = "Категорія")]
        public int? CategoryId { get; set; }

        [Display(Name = "Одиниця виміру")]
        public int? BaseUnitId { get; set; }

        public IEnumerable<SelectListItem> CategoryList { get; set; } = [];
        public IEnumerable<SelectListItem> UnitList { get; set; } = [];
    }
}
