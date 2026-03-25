using eShop.Domain.Entities.ProductData;
using eShop.Domain.Entities.UserData;
using eShop.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using User = eShop.Infrastructure.Entities.User;

namespace eShop.Infrastructure.Context
{
    public static class DbInitializer
    {
        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var db = scope.ServiceProvider.GetRequiredService<ShopContext>();

            await SeedProductsAsync(db);

            var admin = await CreateUserAsync(userManager,
                email: "admin@eshop.com", password: "Admin123!",
                firstName: "Адмін", lastName: "Системний", role: "Admin");

            var manager = await CreateUserAsync(userManager,
                email: "manager@eshop.com", password: "Manager123!",
                firstName: "Менеджер", lastName: "Продажів", role: "Manager");

            var customer = await CreateUserAsync(userManager,
                email: "customer@eshop.com", password: "Customer123!",
                firstName: "Іван", lastName: "Петренко", role: "Customer");

            if (admin is not null)
                await SeedAddressesAsync(db, admin.Id, new[]
                {
                    new Address
                    {
                        FirstName   = "Адмін",
                        LastName    = "Системний",
                        PhoneNumber = "+380671234567",
                        AddressLine = "вул. Грушевського, 5, офіс 10",
                        City        = "Київ",
                        Region      = "Київська",
                        PostalCode  = "01008",
                        Country     = "Україна",
                        IsDefault   = true
                    }
                });

            if (manager is not null)
                await SeedAddressesAsync(db, manager.Id, new[]
                {
                    new Address
                    {
                        FirstName   = "Менеджер",
                        LastName    = "Продажів",
                        PhoneNumber = "+380509876543",
                        AddressLine = "вул. Сумська, 12, кв. 34",
                        City        = "Харків",
                        Region      = "Харківська",
                        PostalCode  = "61000",
                        Country     = "Україна",
                        IsDefault   = true
                    }
                });

            if (customer is not null)
                await SeedAddressesAsync(db, customer.Id, new[]
                {
                    new Address
                    {
                        FirstName   = "Іван",
                        LastName    = "Петренко",
                        PhoneNumber = "+380631112233",
                        AddressLine = "вул. Хрещатик, 22, кв. 7",
                        City        = "Київ",
                        Region      = "Київська",
                        PostalCode  = "01001",
                        Country     = "Україна",
                        IsDefault   = true
                    },
                    new Address
                    {
                        FirstName   = "Іван",
                        LastName    = "Петренко",
                        PhoneNumber = "+380631112233",
                        AddressLine = "вул. Дерибасівська, 1, кв. 2",
                        City        = "Одеса",
                        Region      = "Одеська",
                        PostalCode  = "65000",
                        Country     = "Україна",
                        IsDefault   = false
                    }
                });
        }

        // ── Products ─────────────────────────────────────────────────────────────

        private static async Task SeedProductsAsync(ShopContext db)
        {
            if (await db.Products.AnyAsync()) return;

            var products = new[]
            {
                // Ноутбуки (CategoryId = 1)
                new Product
                {
                    Name        = "Ноутбук ASUS VivoBook 15",
                    Description = "15.6\" Full HD, Intel Core i5, 8 ГБ RAM, 512 ГБ SSD, Windows 11",
                    Price       = 24999m,
                    Type        = ProductType.Serialized,
                    CategoryId  = 1,
                    BaseUnitId  = 1
                },
                new Product
                {
                    Name        = "Ноутбук Lenovo IdeaPad 3",
                    Description = "15.6\" Full HD, AMD Ryzen 5, 16 ГБ RAM, 256 ГБ SSD, без ОС",
                    Price       = 19499m,
                    Type        = ProductType.Serialized,
                    CategoryId  = 1,
                    BaseUnitId  = 1
                },

                // Планшети (CategoryId = 2)
                new Product
                {
                    Name        = "Планшет Samsung Galaxy Tab A9",
                    Description = "10.1\", 4 ГБ RAM, 64 ГБ, Wi-Fi, Android 14",
                    Price       = 9999m,
                    Type        = ProductType.Serialized,
                    CategoryId  = 2,
                    BaseUnitId  = 1
                },
                new Product
                {
                    Name        = "Планшет Apple iPad 10",
                    Description = "10.9\", Apple A14 Bionic, 64 ГБ, Wi-Fi, iPadOS 17",
                    Price       = 16999m,
                    Type        = ProductType.Serialized,
                    CategoryId  = 2,
                    BaseUnitId  = 1
                },

                // Телефони (CategoryId = 3)
                new Product
                {
                    Name        = "Смартфон Samsung Galaxy A55",
                    Description = "6.6\" Super AMOLED, Exynos 1480, 8 ГБ RAM, 128 ГБ, 5G",
                    Price       = 14999m,
                    Type        = ProductType.Serialized,
                    CategoryId  = 3,
                    BaseUnitId  = 1
                },
                new Product
                {
                    Name        = "Смартфон Xiaomi Redmi Note 13",
                    Description = "6.67\" AMOLED, Snapdragon 685, 6 ГБ RAM, 128 ГБ, 108 Мп камера",
                    Price       = 8499m,
                    Type        = ProductType.Serialized,
                    CategoryId  = 3,
                    BaseUnitId  = 1
                },
            };

            db.Products.AddRange(products);
            await db.SaveChangesAsync();
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static async Task<User?> CreateUserAsync(
            UserManager<User> userManager,
            string email, string password,
            string firstName, string lastName, string role)
        {
            if (await userManager.FindByEmailAsync(email) is not null)
                return null; // вже існує — пропускаємо, адреси теж не дублюємо

            var user = new User
            {
                UserName       = email,
                Email          = email,
                EmailConfirmed = true,
                FirstName      = firstName,
                LastName       = lastName
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return null;

            await userManager.AddToRoleAsync(user, role);
            return user;
        }

        private static async Task SeedAddressesAsync(
            ShopContext db, string userId, IEnumerable<Address> addresses)
        {
            // Якщо у цього користувача вже є адреси — не дублюємо
            if (await db.Addresses.AnyAsync(a => a.UserId == userId))
                return;

            foreach (var address in addresses)
            {
                address.UserId = userId;
                db.Addresses.Add(address);
            }

            await db.SaveChangesAsync();
        }
    }
}
