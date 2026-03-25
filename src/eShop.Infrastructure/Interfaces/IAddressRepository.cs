using eShop.Domain.Entities.UserData;

namespace eShop.Infrastructure.Interfaces
{
    public interface IAddressRepository : IRepository<Address>
    {
        void Update(Address entity);

        /// <summary>
        /// Знімає IsDefault з усіх адрес користувача перед встановленням нової основної.
        /// </summary>
        void UnsetDefault(string userId);
    }
}
