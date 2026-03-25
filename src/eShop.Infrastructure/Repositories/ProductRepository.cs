using eShop.Domain.Entities.ProductData;
using eShop.Infrastructure.Context;
using eShop.Infrastructure.Interfaces;

namespace eShop.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ShopContext ctx) : base(ctx) { }

        public void Update(Product entity)
        {
            var fromDb = _ctx.Products.FirstOrDefault(p => p.Id == entity.Id);
            if (fromDb is null) return;

            fromDb.Name        = entity.Name;
            fromDb.Description = entity.Description;
            fromDb.Price       = entity.Price;
            fromDb.Type        = entity.Type;
            fromDb.CategoryId  = entity.CategoryId;
            fromDb.BaseUnitId  = entity.BaseUnitId;
        }
    }
}
