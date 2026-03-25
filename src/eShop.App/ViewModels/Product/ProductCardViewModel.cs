namespace eShopMVC.App.ViewModels.Product
{
    public class ProductCardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? CategoryName { get; set; }
        public string TypeDisplay { get; set; } = string.Empty;
    }
}
