using BikeStore.Data;
using BikeStore.DTOs;
using BikeStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeStore.Services
{
    public class VentaService : IVentaService
    {
        private readonly BikeStoreDbContext _context;

        public VentaService(BikeStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VentaDTO>> ObtenerTodasAsync()
        {
            return await _context.Ventas
                .Include(v => v.Detalles)
                .Select(v => new VentaDTO
                {
                    IdVenta = v.IdVenta,
                    Fecha = v.Fecha,
                    IdCliente = v.IdCliente,
                    Subtotal = v.Subtotal,
                    IVA = v.IVA,
                    Total = v.Total,
                    Detalles = v.Detalles.Select(d => new DetalleVentaDTO
                    {
                        IdDetalle = d.IdDetalle,
                        IdVenta = d.IdVenta,
                        IdBicicleta = d.IdBicicleta,
                        Cantidad = d.Cantidad,
                        Precio = d.Precio,
                        Subtotal = d.Subtotal
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<VentaDTO?> ObtenerPorIdAsync(int id)
        {
            var v = await _context.Ventas
                .Include(x => x.Detalles)
                .FirstOrDefaultAsync(x => x.IdVenta == id);

            if (v == null) return null;

            return new VentaDTO
            {
                IdVenta = v.IdVenta,
                Fecha = v.Fecha,
                IdCliente = v.IdCliente,
                Subtotal = v.Subtotal,
                IVA = v.IVA,
                Total = v.Total,
                Detalles = v.Detalles.Select(d => new DetalleVentaDTO
                {
                    IdDetalle = d.IdDetalle,
                    IdVenta = d.IdVenta,
                    IdBicicleta = d.IdBicicleta,
                    Cantidad = d.Cantidad,
                    Precio = d.Precio,
                    Subtotal = d.Subtotal
                }).ToList()
            };
        }

        public async Task<VentaDTO> CrearAsync(CrearVentaDTO dto)
        {
            decimal subtotalVenta = 0;
            var detallesEntidad = new List<DetalleVenta>();

            foreach (var d in dto.Detalles)
            {
                var bici = await _context.Bicicletas.FindAsync(d.IdBicicleta);
                if (bici == null) throw new Exception($"Bicicleta con ID {d.IdBicicleta} no existe.");
                if (bici.Stock < d.Cantidad) throw new Exception($"Stock insuficiente para la bicicleta {bici.Modelo}.");

                // Descontar stock
                bici.Stock -= d.Cantidad;

                decimal subtotalDetalle = d.Precio * d.Cantidad;
                subtotalVenta += subtotalDetalle;

                detallesEntidad.Add(new DetalleVenta
                {
                    IdBicicleta = d.IdBicicleta,
                    Cantidad = d.Cantidad,
                    Precio = d.Precio,
                    Subtotal = subtotalDetalle
                });
            }

            decimal iva = subtotalVenta * 0.15m; // IVA 15%
            decimal total = subtotalVenta + iva;

            var venta = new Venta
            {
                Fecha = DateTime.Now,
                IdCliente = dto.IdCliente,
                Subtotal = subtotalVenta,
                IVA = iva,
                Total = total,
                Detalles = detallesEntidad
            };

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            return await ObtenerPorIdAsync(venta.IdVenta) ?? new VentaDTO();
        }
    }
}