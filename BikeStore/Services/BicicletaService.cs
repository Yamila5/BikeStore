using BikeStore.Data;
using BikeStore.DTOs;
using BikeStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeStore.Services
{
    public class BicicletaService : IBicicletaService
    {
        private readonly BikeStoreDbContext _context;

        public BicicletaService(BikeStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BicicletaDTO>> ObtenerTodasAsync()
        {
            return await _context.Bicicletas
                .Include(b => b.Categoria)
                .Select(b => new BicicletaDTO
                {
                    IdBicicleta = b.IdBicicleta,
                    IdCategoria = b.IdCategoria,
                    Marca = b.Marca,
                    Modelo = b.Modelo,
                    Precio = b.Precio,
                    Stock = b.Stock,
                    Estado = b.Estado,
                    NombreCategoria = b.Categoria != null ? b.Categoria.Nombre : null
                }).ToListAsync();
        }

        public async Task<BicicletaDTO?> ObtenerPorIdAsync(int id)
        {
            var b = await _context.Bicicletas
                .Include(x => x.Categoria)
                .FirstOrDefaultAsync(x => x.IdBicicleta == id);

            if (b == null) return null;

            return new BicicletaDTO
            {
                IdBicicleta = b.IdBicicleta,
                IdCategoria = b.IdCategoria,
                Marca = b.Marca,
                Modelo = b.Modelo,
                Precio = b.Precio,
                Stock = b.Stock,
                Estado = b.Estado,
                NombreCategoria = b.Categoria?.Nombre
            };
        }

        public async Task<BicicletaDTO> CrearAsync(CrearBicicletaDTO dto)
        {
            var bici = new Bicicleta
            {
                IdCategoria = dto.IdCategoria,
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                Precio = dto.Precio,
                Stock = dto.Stock,
                Estado = dto.Estado
            };

            _context.Bicicletas.Add(bici);
            await _context.SaveChangesAsync();

            return new BicicletaDTO
            {
                IdBicicleta = bici.IdBicicleta,
                IdCategoria = bici.IdCategoria,
                Marca = bici.Marca,
                Modelo = bici.Modelo,
                Precio = bici.Precio,
                Stock = bici.Stock,
                Estado = bici.Estado
            };
        }

        public async Task<bool> ActualizarAsync(int id, CrearBicicletaDTO dto)
        {
            var bici = await _context.Bicicletas.FindAsync(id);
            if (bici == null) return false;

            bici.IdCategoria = dto.IdCategoria;
            bici.Marca = dto.Marca;
            bici.Modelo = dto.Modelo;
            bici.Precio = dto.Precio;
            bici.Stock = dto.Stock;
            bici.Estado = dto.Estado;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var bici = await _context.Bicicletas.FindAsync(id);
            if (bici == null) return false;

            _context.Bicicletas.Remove(bici);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}