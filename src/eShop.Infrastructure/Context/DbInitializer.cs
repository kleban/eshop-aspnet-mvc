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
