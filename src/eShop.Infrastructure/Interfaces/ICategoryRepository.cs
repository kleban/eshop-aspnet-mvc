using eShop.Domain.Entities.CategoryData;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Infrastructure.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category entity);
    }
}
