using Microsoft.EntityFrameworkCore;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Data
{
    public class Dsw2025TpiContext : DbContext
    {
        public Dsw2025TpiContext(DbContextOptions<Dsw2025TpiContext> options)
            : base(options)
        {
        }

        // DbSet para cada entidad principal
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica automáticamente todas las configuraciones que implementan IEntityTypeConfiguration<Entities>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Dsw2025TpiContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
