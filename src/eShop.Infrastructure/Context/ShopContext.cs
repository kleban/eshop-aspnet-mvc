using eShop.Domain.Entities;
using eShop.Domain.Entities.CardData;
using eShop.Domain.Entities.OrderData;
using eShop.Domain.Entities.ProductData;
using eShop.Domain.Entities.UserData;
using eShop.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using User = eShop.Infrastructure.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace eShop.Infrastructure.Context
{
    public class ShopContext : IdentityDbContext<User>
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>();

        // ProductData
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<ProductItem> ProductItems => Set<ProductItem>();
        public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();
        public DbSet<Inventory> Inventories => Set<Inventory>();

        // UserData
        public DbSet<Address> Addresses => Set<Address>();

        // CardData
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();

        // OrderData
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Address → User (Identity string FK, без навігаційної властивості)
            modelBuilder.Entity<Address>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cart → User (Identity string FK)
            modelBuilder.Entity<Cart>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order → User (Identity string FK)
            modelBuilder.Entity<Order>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order → Address (nullable FK, без каскаду — адреса може бути видалена незалежно)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingAddress)
                .WithMany()
                .HasForeignKey(o => o.ShippingAddressId)
                .OnDelete(DeleteBehavior.SetNull);

            // Decimal precision (18,2) — гроші; (18,4) — кількість/вага
            modelBuilder.Entity<Product>()
                .Property(p => p.Price).HasPrecision(18, 2);

            modelBuilder.Entity<Inventory>()
                .Property(i => i.Quantity).HasPrecision(18, 4);

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.Quantity).HasPrecision(18, 4);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount).HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price).HasPrecision(18, 2);
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Quantity).HasPrecision(18, 4);

            modelBuilder.Seed();
        }
    }
}
