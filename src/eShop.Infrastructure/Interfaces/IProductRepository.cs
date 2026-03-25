using eShop.Domain.Entities.ProductData;

namespace eShop.Infrastructure.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product entity);
    }
}
