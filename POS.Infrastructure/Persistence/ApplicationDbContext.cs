using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
namespace POS.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerTransaction> CustomerTransactions { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<HeldSale> HeldSales { get; set; }
        public DbSet<BoxTransaction> BoxTransactions { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<SalesReturn> SalesReturns { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<SupplierTransaction> SupplierTransactions { get; set; }
        public DbSet<SalesReturnItem> SalesReturnItems { get; set; }
        public DbSet<SystemLicense> Licenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>().ToTable("User");
            modelBuilder.Entity<AppRole>().ToTable("Role");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserToken");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");

            
            foreach (var foreignkey in modelBuilder.Model.GetEntityTypes()
               .SelectMany(e => e.GetForeignKeys()))
            {
                foreignkey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Product>().Property(p => p.Cost).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BoxTransaction>().Property(p => p.Amount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Shift>().Property(s => s.StartingCash).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Shift>().Property(s => s.ExpectedCash).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Shift>().Property(s => s.ActualCash).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<SalesReturnItem>()
            .Property(p => p.Quantity)
            .HasColumnType("decimal(18, 3)");
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.User)
                .WithMany() 
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SalesReturnItem>()
                .Property(p => p.UnitPrice)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<SalesReturn>()
                .Property(p => p.TotalAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<SalesReturn>()
                .HasOne(s => s.Sale)
                .WithMany()
                .HasForeignKey(s => s.SaleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shift>()
                .HasOne(s => s.User)
                .WithMany(u => u.Shifts)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaleItem>()
                .HasOne(si => si.Sale)
                .WithMany(s => s.SaleItems)
                .HasForeignKey(si => si.SaleId)
                .IsRequired(false); 

            modelBuilder.Entity<SaleItem>()
                .HasOne(si => si.HeldSale)
                .WithMany(hs => hs.SaleItems)
                .HasForeignKey(si => si.HeldSaleId)
                .IsRequired(false); 
        }
    }
}
