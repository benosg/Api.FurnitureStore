using Api.FurnitureStore.Shared;
using Microsoft.EntityFrameworkCore;


namespace Api.FurnitureStore.Data
{
    public class ApiFurnitureStoreContext : DbContext
    {
        public ApiFurnitureStoreContext(DbContextOptions options) : base(options) { }

       

        public DbSet<Client> Clients{ get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite();
        }
    }
}
