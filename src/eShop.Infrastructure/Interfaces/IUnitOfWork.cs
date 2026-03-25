using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IAddressRepository Address { get; }
        IProductRepository Product { get; }
        IRepository<eShop.Domain.Entities.ProductData.UnitOfMeasure> Unit { get; }
        void Save();
        Task SaveAsync();
    }
}
