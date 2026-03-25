using CategoryEntity = eShop.Domain.Entities.CategoryData.Category;

namespace eShopMVC.App.ViewModels.Product
{
    public class ProductCatalogViewModel
    {
        public IEnumerable<ProductCardViewModel> Products { get; set; } = [];
        public IEnumerable<CategoryEntity> Categories { get; set; } = [];
        public int? SelectedCategoryId { get; set; }
        public string? Search { get; set; }
    }
}
