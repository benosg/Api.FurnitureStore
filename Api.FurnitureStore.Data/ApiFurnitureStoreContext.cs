﻿using Api.FurnitureStore.Shared;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Api.FurnitureStore.Data
{
    public class ApiFurnitureStoreContext : IdentityDbContext
    {
        public ApiFurnitureStoreContext(DbContextOptions options) : base(options) 
        {
        }

       

        public DbSet<Client> Clients{ get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderDetail>()
                        .HasKey(od => new { od.OrderId, od.ProductId });
        }
    }
}
