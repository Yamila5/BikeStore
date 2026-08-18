using BikeStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeStore.Data
{
    public class BikeStoreDbContext : DbContext
    {
        public BikeStoreDbContext(
            DbContextOptions<BikeStoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Bicicleta> Bicicletas { get; set; }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Venta> Ventas { get; set; }

        public DbSet<DetalleVenta> DetalleVentas { get; set; }
    }
}