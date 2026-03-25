using eShop.Domain.Entities;
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

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "role-admin",    Name = "Admin",    NormalizedName = "ADMIN",    ConcurrencyStamp = "cs-role-admin" },
                new IdentityRole { Id = "role-manager",  Name = "Manager",  NormalizedName = "MANAGER",  ConcurrencyStamp = "cs-role-manager" },
                new IdentityRole { Id = "role-customer", Name = "Customer", NormalizedName = "CUSTOMER", ConcurrencyStamp = "cs-role-customer" }
            );
        }
    }
}
