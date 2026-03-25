using eShop.Infrastructure.Context;
using eShop.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ShopContext _ctx;

        public ICategoryRepository Category { get; }
        public IAddressRepository Address { get; }

        public UnitOfWork(ShopContext ctx)
        {
            _ctx = ctx;
            Category = new CategoryRepository(_ctx);
            Address  = new AddressRepository(_ctx);
        }

        public void Save()
        {
            _ctx.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _ctx.SaveChangesAsync();
        }
    }
}
