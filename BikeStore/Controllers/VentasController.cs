using BikeStore.Data;
using BikeStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly BikeStoreDbContext _context;

        public VentasController(BikeStoreDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Venta>>> GetVentas()
        {
            return await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Bicicleta)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Venta>> GetVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Bicicleta)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null)
            {
                return NotFound("Venta no encontrada");
            }

            return venta;
        }

        [HttpGet("cliente/{idCliente}")]
        public async Task<ActionResult<IEnumerable<Venta>>> GetVentasPorCliente(int idCliente)
        {
            return await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Bicicleta)
                .Where(v => v.IdCliente == idCliente)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Venta>> PostVenta(Venta venta)
        {
            var cliente = await _context.Clientes.FindAsync(venta.IdCliente);
            if (cliente == null)
            {
                return BadRequest("El cliente especificado no existe");
            }

            if (venta.Detalles == null || !venta.Detalles.Any())
            {
                return BadRequest("La venta debe contener al menos un detalle de producto");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal subtotalVenta = 0;

                foreach (var detalle in venta.Detalles)
                {
                    if (detalle.Cantidad <= 0)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest("La cantidad debe ser mayor a cero");
                    }

                    var bicicleta = await _context.Bicicletas.FindAsync(detalle.IdBicicleta);
                    if (bicicleta == null)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest($"La bicicleta con id {detalle.IdBicicleta} no existe");
                    }

                    if (bicicleta.Stock < detalle.Cantidad)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest($"Stock insuficiente para {bicicleta.Marca} {bicicleta.Modelo}. Disponible: {bicicleta.Stock}, Solicitado: {detalle.Cantidad}");
                    }

                    // Forzar siempre el precio oficial de la bicicleta según el catálogo de la base de datos
                    detalle.Precio = bicicleta.Precio;

                    detalle.Subtotal = detalle.Cantidad * detalle.Precio;
                    subtotalVenta += detalle.Subtotal;

                    bicicleta.Stock -= detalle.Cantidad;
                    if (bicicleta.Stock == 0)
                    {
                        bicicleta.Estado = "Agotado";
                    }

                    _context.Entry(bicicleta).State = EntityState.Modified;
                }

                venta.Fecha = venta.Fecha == default ? DateTime.Now : venta.Fecha;
                venta.Subtotal = subtotalVenta;
                venta.IVA = Math.Round(subtotalVenta * 0.15m, 2);
                venta.Total = venta.Subtotal + venta.IVA;

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetVenta), new { id = venta.IdVenta }, venta);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error al procesar la venta: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null)
            {
                return NotFound("Venta no encontrada");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var detalle in venta.Detalles)
                {
                    var bicicleta = await _context.Bicicletas.FindAsync(detalle.IdBicicleta);
                    if (bicicleta != null)
                    {
                        bicicleta.Stock += detalle.Cantidad;
                        if (bicicleta.Stock > 0 && bicicleta.Estado == "Agotado")
                        {
                            bicicleta.Estado = "Disponible";
                        }
                        _context.Entry(bicicleta).State = EntityState.Modified;
                    }
                }

                _context.Ventas.Remove(venta);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok("Venta anulada y stock restablecido correctamente");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error al anular la venta: {ex.Message}");
            }
        }
    }
}
