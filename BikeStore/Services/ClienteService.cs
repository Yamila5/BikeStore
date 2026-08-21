using BikeStore.Data;
using BikeStore.DTOs;
using BikeStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeStore.Services
{
    public class ClienteService : IClienteService
    {
        private readonly BikeStoreDbContext _context;

        public ClienteService(BikeStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClienteDTO>> ObtenerTodosAsync()
        {
            return await _context.Clientes
                .Select(c => new ClienteDTO
                {
                    IdCliente = c.IdCliente,
                    Cedula = c.Cedula,
                    Nombres = c.Nombres,
                    Apellidos = c.Apellidos,
                    Telefono = c.Telefono,
                    Correo = c.Correo
                }).ToListAsync();
        }

        public async Task<ClienteDTO?> ObtenerPorIdAsync(int id)
        {
            var c = await _context.Clientes.FindAsync(id);
            if (c == null) return null;

            return new ClienteDTO
            {
                IdCliente = c.IdCliente,
                Cedula = c.Cedula,
                Nombres = c.Nombres,
                Apellidos = c.Apellidos,
                Telefono = c.Telefono,
                Correo = c.Correo
            };
        }

        public async Task<ClienteDTO> CrearAsync(CrearClienteDTO dto)
        {
            var cliente = new Cliente
            {
                Cedula = dto.Cedula,
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Telefono = dto.Telefono,
                Correo = dto.Correo
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return new ClienteDTO
            {
                IdCliente = cliente.IdCliente,
                Cedula = cliente.Cedula,
                Nombres = cliente.Nombres,
                Apellidos = cliente.Apellidos,
                Telefono = cliente.Telefono,
                Correo = cliente.Correo
            };
        }

        public async Task<bool> ActualizarAsync(int id, CrearClienteDTO dto)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return false;

            cliente.Cedula = dto.Cedula;
            cliente.Nombres = dto.Nombres;
            cliente.Apellidos = dto.Apellidos;
            cliente.Telefono = dto.Telefono;
            cliente.Correo = dto.Correo;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return false;

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}