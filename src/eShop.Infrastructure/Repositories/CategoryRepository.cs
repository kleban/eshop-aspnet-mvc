using eShop.Domain.Entities.CategoryData;
using eShop.Infrastructure.Context;
using eShop.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ShopContext ctx) : base(ctx)
        {
        }

        public void Update(Category entity)
        {

            var categoryFromDb = _ctx.Categories.FirstOrDefault(x => x.Id == entity.Id);

            if (categoryFromDb is not null)
            {
                categoryFromDb.Name = entity.Name;
                categoryFromDb.DisplayOrder = entity.DisplayOrder;
            }
        }
    }
}
