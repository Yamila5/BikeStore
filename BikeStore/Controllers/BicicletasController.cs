using BikeStore.Data;
using BikeStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BicicletasController : ControllerBase
    {
        private readonly BikeStoreDbContext _context;

        public BicicletasController(BikeStoreDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bicicleta>>> GetBicicletas()
        {
            return await _context.Bicicletas
                .Include(b => b.Categoria)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Bicicleta>> GetBicicleta(int id)
        {
            var bicicleta = await _context.Bicicletas
                .Include(b => b.Categoria)
                .FirstOrDefaultAsync(b => b.IdBicicleta == id);

            if (bicicleta == null)
            {
                return NotFound("Bicicleta no encontrada");
            }

            return bicicleta;
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<Bicicleta>>> BuscarBicicletas(
            [FromQuery] string? marca,
            [FromQuery] string? modelo,
            [FromQuery] int? idCategoria)
        {
            var query = _context.Bicicletas.Include(b => b.Categoria).AsQueryable();

            if (!string.IsNullOrWhiteSpace(marca))
            {
                query = query.Where(b => b.Marca.Contains(marca));
            }

            if (!string.IsNullOrWhiteSpace(modelo))
            {
                query = query.Where(b => b.Modelo.Contains(modelo));
            }

            if (idCategoria.HasValue && idCategoria.Value > 0)
            {
                query = query.Where(b => b.IdCategoria == idCategoria.Value);
            }

            return await query.ToListAsync();
        }

        [HttpGet("categoria/{idCategoria}")]
        public async Task<ActionResult<IEnumerable<Bicicleta>>> GetBicicletasPorCategoria(int idCategoria)
        {
            return await _context.Bicicletas
                .Include(b => b.Categoria)
                .Where(b => b.IdCategoria == idCategoria)
                .ToListAsync();
        }

        [HttpGet("stock-bajo")]
        public async Task<ActionResult<IEnumerable<Bicicleta>>> GetBicicletasStockBajo([FromQuery] int limite = 5)
        {
            return await _context.Bicicletas
                .Include(b => b.Categoria)
                .Where(b => b.Stock > 0 && b.Stock <= limite)
                .ToListAsync();
        }

        [HttpGet("agotadas")]
        public async Task<ActionResult<IEnumerable<Bicicleta>>> GetBicicletasAgotadas()
        {
            return await _context.Bicicletas
                .Include(b => b.Categoria)
                .Where(b => b.Stock == 0 || b.Estado == "Agotado")
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Bicicleta>> PostBicicleta(Bicicleta bicicleta)
        {
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.IdCategoria == bicicleta.IdCategoria);
            if (!categoriaExiste)
            {
                return BadRequest("La categoria seleccionada no existe");
            }

            if (bicicleta.Stock < 0)
            {
                return BadRequest("El stock no puede ser negativo");
            }

            if (bicicleta.Precio < 0)
            {
                return BadRequest("El precio no puede ser negativo");
            }

            if (bicicleta.Stock == 0)
            {
                bicicleta.Estado = "Agotado";
            }
            else if (string.IsNullOrWhiteSpace(bicicleta.Estado))
            {
                bicicleta.Estado = "Disponible";
            }

            _context.Bicicletas.Add(bicicleta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBicicleta), new { id = bicicleta.IdBicicleta }, bicicleta);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBicicleta(int id, Bicicleta bicicleta)
        {
            if (id != bicicleta.IdBicicleta)
            {
                return BadRequest("El id no coincide con la bicicleta");
            }

            var existe = await _context.Bicicletas.AnyAsync(b => b.IdBicicleta == id);
            if (!existe)
            {
                return NotFound("Bicicleta no encontrada");
            }

            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.IdCategoria == bicicleta.IdCategoria);
            if (!categoriaExiste)
            {
                return BadRequest("La categoria especificada no existe");
            }

            if (bicicleta.Stock < 0)
            {
                return BadRequest("El stock no puede ser negativo");
            }

            if (bicicleta.Precio < 0)
            {
                return BadRequest("El precio no puede ser negativo");
            }

            if (bicicleta.Stock == 0)
            {
                bicicleta.Estado = "Agotado";
            }
            else if (bicicleta.Estado == "Agotado")
            {
                bicicleta.Estado = "Disponible";
            }

            _context.Entry(bicicleta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error de concurrencia al actualizar la bicicleta");
            }

            return Ok(bicicleta);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBicicleta(int id)
        {
            var bicicleta = await _context.Bicicletas.FindAsync(id);
            if (bicicleta == null)
            {
                return NotFound("Bicicleta no encontrada");
            }

            var tieneVentas = await _context.DetalleVentas.AnyAsync(d => d.IdBicicleta == id);
            if (tieneVentas)
            {
                return BadRequest("No se puede eliminar la bicicleta porque tiene registros de ventas asociados");
            }

            _context.Bicicletas.Remove(bicicleta);
            await _context.SaveChangesAsync();

            return Ok("Bicicleta eliminada correctamente");
        }
    }
}
