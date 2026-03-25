using eShop.Domain.Entities.CategoryData;
using eShop.Domain.Entities.ProductData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace eShop.Infrastructure.Context
{
    public static class ShopDataSeedExtension
    {
        public static void Seed(this ModelBuilder builder)
        {
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Ноутбуки", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Планшети", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Телефони", DisplayOrder = 3 });

            builder.Entity<UnitOfMeasure>().HasData(
                new UnitOfMeasure { Id = 1, Name = "Штука",     Symbol = "шт" },
                new UnitOfMeasure { Id = 2, Name = "Кілограм",  Symbol = "кг" },
                new UnitOfMeasure { Id = 3, Name = "Літр",      Symbol = "л"  }
            );

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "role-admin",    Name = "Admin",    NormalizedName = "ADMIN",    ConcurrencyStamp = "cs-role-admin" },
                new IdentityRole { Id = "role-manager",  Name = "Manager",  NormalizedName = "MANAGER",  ConcurrencyStamp = "cs-role-manager" },
                new IdentityRole { Id = "role-customer", Name = "Customer", NormalizedName = "CUSTOMER", ConcurrencyStamp = "cs-role-customer" }
            );
        }
    }
}
