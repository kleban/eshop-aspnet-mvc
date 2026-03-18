using eShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

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
        }
    }
}

