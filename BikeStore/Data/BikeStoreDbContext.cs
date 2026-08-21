using BikeStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeStore.Data
{
    public class BikeStoreDbContext : DbContext
    {
        public BikeStoreDbContext(DbContextOptions<BikeStoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Bicicleta> Bicicletas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("Categorias");
                entity.HasKey(e => e.IdCategoria);
            });

            modelBuilder.Entity<Bicicleta>(entity =>
            {
                entity.ToTable("Bicicletas");
                entity.HasKey(e => e.IdBicicleta);
                entity.Property(e => e.Precio).HasColumnType("decimal(10,2)");
                entity.HasOne(e => e.Categoria)
                    .WithMany(c => c.Bicicletas)
                    .HasForeignKey(e => e.IdCategoria);
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Clientes");
                entity.HasKey(e => e.IdCliente);
            });

            modelBuilder.Entity<Venta>(entity =>
            {
                entity.ToTable("Ventas");
                entity.HasKey(e => e.IdVenta);
                entity.Property(e => e.Subtotal).HasColumnType("decimal(10,2)");
                entity.Property(e => e.IVA).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Total).HasColumnType("decimal(10,2)");
                entity.HasOne(e => e.Cliente)
                    .WithMany(c => c.Ventas)
                    .HasForeignKey(e => e.IdCliente);
            });

            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.ToTable("DetalleVentas");
                entity.HasKey(e => e.IdDetalle);
                entity.Property(e => e.Precio).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Subtotal).HasColumnType("decimal(10,2)");
                entity.HasOne(e => e.Venta)
                    .WithMany(v => v.Detalles)
                    .HasForeignKey(e => e.IdVenta);
                entity.HasOne(e => e.Bicicleta)
                    .WithMany(b => b.DetallesVenta)
                    .HasForeignKey(e => e.IdBicicleta);
            });
        }
    }
}