using eShop.Domain.Entities.UserData;
using eShop.Infrastructure.Context;
using eShop.Infrastructure.Interfaces;

namespace eShop.Infrastructure.Repositories
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(ShopContext ctx) : base(ctx) { }

        public void Update(Address entity)
        {
            var fromDb = _ctx.Addresses.FirstOrDefault(a => a.Id == entity.Id);

            if (fromDb is null)
                return;

            fromDb.FirstName   = entity.FirstName;
            fromDb.LastName    = entity.LastName;
            fromDb.PhoneNumber = entity.PhoneNumber;
            fromDb.AddressLine = entity.AddressLine;
            fromDb.City        = entity.City;
            fromDb.Region      = entity.Region;
            fromDb.PostalCode  = entity.PostalCode;
            fromDb.Country     = entity.Country;
            fromDb.IsDefault   = entity.IsDefault;
        }

        public void UnsetDefault(string userId)
        {
            var defaults = _ctx.Addresses
                .Where(a => a.UserId == userId && a.IsDefault)
                .ToList();

            foreach (var addr in defaults)
                addr.IsDefault = false;
        }
    }
}
