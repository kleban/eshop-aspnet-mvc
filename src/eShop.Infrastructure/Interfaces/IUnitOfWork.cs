using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IAddressRepository Address { get; }
        void Save();
        Task SaveAsync();
    }
}
