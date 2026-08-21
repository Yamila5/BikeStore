using BikeStore.Data;
using BikeStore.DTOs;
using BikeStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeStore.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly BikeStoreDbContext _context;

        public CategoriaService(BikeStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoriaDTO>> ObtenerTodasAsync()
        {
            return await _context.Categorias
                .Select(c => new CategoriaDTO
                {
                    IdCategoria = c.IdCategoria,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    Activo = c.Activo
                }).ToListAsync();
        }

        public async Task<CategoriaDTO?> ObtenerPorIdAsync(int id)
        {
            var c = await _context.Categorias.FindAsync(id);
            if (c == null) return null;

            return new CategoriaDTO
            {
                IdCategoria = c.IdCategoria,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Activo = c.Activo
            };
        }

        public async Task<CategoriaDTO> CrearAsync(CrearCategoriaDTO dto)
        {
            var categoria = new Categoria
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Activo = dto.Activo
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return new CategoriaDTO
            {
                IdCategoria = categoria.IdCategoria,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                Activo = categoria.Activo
            };
        }

        public async Task<bool> ActualizarAsync(int id, CrearCategoriaDTO dto)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return false;

            categoria.Nombre = dto.Nombre;
            categoria.Descripcion = dto.Descripcion;
            categoria.Activo = dto.Activo;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return false;

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}