using BikeStore.Data;
using BikeStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly BikeStoreDbContext _context;

        public ClientesController(BikeStoreDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            return await _context.Clientes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound("Cliente no encontrado");
            }

            return cliente;
        }

        [HttpGet("cedula/{cedula}")]
        public async Task<ActionResult<Cliente>> GetClientePorCedula(string cedula)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Cedula == cedula);

            if (cliente == null)
            {
                return NotFound("Cliente no encontrado con la cedula indicada");
            }

            return cliente;
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<Cliente>>> BuscarClientes(
            [FromQuery] string? cedula,
            [FromQuery] string? apellido,
            [FromQuery] string? nombre)
        {
            var query = _context.Clientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(cedula))
            {
                query = query.Where(c => c.Cedula.Contains(cedula));
            }

            if (!string.IsNullOrWhiteSpace(apellido))
            {
                query = query.Where(c => c.Apellidos.Contains(apellido));
            }

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                query = query.Where(c => c.Nombres.Contains(nombre));
            }

            return await query.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Cedula))
            {
                return BadRequest("La cedula es obligatoria");
            }

            var existeCedula = await _context.Clientes.AnyAsync(c => c.Cedula == cliente.Cedula);
            if (existeCedula)
            {
                return BadRequest("Ya existe un cliente registrado con esa cedula");
            }

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.IdCliente }, cliente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente cliente)
        {
            if (id != cliente.IdCliente)
            {
                return BadRequest("El id no coincide con el cliente");
            }

            var existe = await _context.Clientes.AnyAsync(c => c.IdCliente == id);
            if (!existe)
            {
                return NotFound("Cliente no encontrado");
            }

            var cedulaDuplicada = await _context.Clientes.AnyAsync(c => c.Cedula == cliente.Cedula && c.IdCliente != id);
            if (cedulaDuplicada)
            {
                return BadRequest("Ya existe otro cliente con la misma cedula");
            }

            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error de concurrencia al actualizar el cliente");
            }

            return Ok(cliente);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound("Cliente no encontrado");
            }

            var tieneVentas = await _context.Ventas.AnyAsync(v => v.IdCliente == id);
            if (tieneVentas)
            {
                return BadRequest("No se puede eliminar el cliente porque tiene historial de ventas registrado");
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return Ok("Cliente eliminado correctamente");
        }
    }
}
