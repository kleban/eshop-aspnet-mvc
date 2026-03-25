using eShop.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Infrastructure.Context
{
    public static class DbInitializer
    {
        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            await CreateUserAsync(userManager,
                email: "admin@eshop.com",
                password: "Admin123!",
                firstName: "Admin",
                lastName: "Admin",
                role: "Admin");

            await CreateUserAsync(userManager,
                email: "manager@eshop.com",
                password: "Manager123!",
                firstName: "Manager",
                lastName: "Manager",
                role: "Manager");

            await CreateUserAsync(userManager,
                email: "customer@eshop.com",
                password: "Customer123!",
                firstName: "Customer",
                lastName: "Customer",
                role: "Customer");
        }

        private static async Task CreateUserAsync(
            UserManager<User> userManager,
            string email,
            string password,
            string firstName,
            string lastName,
            string role)
        {
            if (await userManager.FindByEmailAsync(email) is not null)
                return;

            var user = new User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, role);
        }
    }
}
